using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Custom
{
    public class UOACZScavengeOre : UOACZBaseScavengeObject
    {
        public override string NoYieldRemainingSingleClickText { get { return "(thoroughly mined)"; } }
        public override string LockedSingleClickText { get { return "(locked)"; } }

        public override string NoYieldRemainingText { get { return "It appears to be devoid of any more usable ore."; } }
        public override string InteractText { get { return "You begin mining."; } }

        public override string ScavengeResultSuccessWithTrapText { get { return "Your mining attempt was successful but something is amiss..."; } }
        public override string ScavengeResultSuccessText { get { return "You have mined some ore."; } }
        public override string ScavengeResultFailWithTrapText { get { return "Your mining attempt yields nothing, but something is amiss..."; } }
        public override string ScavengeResultFailText { get { return "You mine for some time but fail to find locate any ore."; } }

        public override string ScavengeUndeadTrapText { get { return "*noise from mining draws unwanted attention...*"; } }

        public override TimeSpan ScavengeDuration { get { return TimeSpan.FromSeconds(3); } }

        public UOACZScavengeOreSpawner m_Spawner;

        [Constructable]
        public UOACZScavengeOre(): base()
        {
            Name = "large ore formation";
        }

        public override void Init()
        {
            ItemID = 6002;
            Hue = 2500;

            InteractionRange = 2;

            MaxPlayerInteractions = 10;
            StartingYieldCount = 30;

            ScavengeDifficulty = Utility.RandomMinMax(75, 150);

            if (Utility.RandomDouble() <= .25)
                TrapType = ScavengeTrapType.Undead;

            base.Init();
        }

        public override void CreateLoot()
        {
            for (int a = 0; a < StartingYieldCount; a++)
            {
                double itemResult = Utility.RandomDouble();

                if (itemResult <= .40)                
                    DropItem(new IronOre(Utility.RandomMinMax(1, 2)));                

                else if (itemResult <= .80)
                    DropItem(new IronOre(Utility.RandomMinMax(2, 3)));           

                else if (itemResult <= .98)
                    DropItem(new IronOre(Utility.RandomMinMax(3, 4)));              

                else                
                    DropItem(new UOACZHumanUpgradeToken());                
            }

            base.CreateLoot();
        }     

        public override bool CanInteract(PlayerMobile player)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player)) return false;
            if (!player.IsUOACZHuman) return false;

            Item oneHand = player.FindItemOnLayer(Layer.OneHanded);
            Item firstValid = player.FindItemOnLayer(Layer.FirstValid);

            if (!(oneHand is UOACZPickaxe || firstValid is UOACZPickaxe))
            {
                player.SendMessage("You must equip this pickaxe in order to use it.");
                return false;
            }

            UOACZPickaxe pickaxe = null;

            if (oneHand is UOACZPickaxe)
                pickaxe = oneHand as UOACZPickaxe;

            if (firstValid is UOACZPickaxe)
                pickaxe = firstValid as UOACZPickaxe;

            if (pickaxe == null)
                return false;

            if (pickaxe.UsesRemaining > 1)
                pickaxe.UsesRemaining--;

            else
                pickaxe.Delete();

            return base.CanInteract(player);
        }

        public override void DoAction(PlayerMobile player)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player)) return;

            player.Animate(12, 7, 1, true, false, 0);
            player.PlaySound(Utility.RandomList(0x125, 0x126));

            player.RevealingAction();
        }

        public override bool GetScavengeResult(PlayerMobile player, bool lockpickAttempt)
        {
            bool scavengeResult = false;

            int minimumPlayerValue = 50;

            int playerMiningValue = Utility.RandomMinMax(0, minimumPlayerValue + (int)(Math.Round(player.Skills.Mining.Value * UOACZSystem.scavengeSkillScalar)));
            int miningTarget = Utility.RandomMinMax(0, ScavengeDifficulty);

            if (playerMiningValue >= miningTarget)
            {
                scavengeResult = true;

                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZHarvesting, 1);
                
                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.ScavengeableOreFormationItems++;
            }
            
            if (scavengeResult)
                Effects.PlaySound(player.Location, player.Map, 0x5CA);

            return scavengeResult;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)
                m_Spawner.m_Items.Remove(this);
        }

        public UOACZScavengeOre(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Spawner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Spawner = (UOACZScavengeOreSpawner)reader.ReadItem();
            }
        }
    }
}