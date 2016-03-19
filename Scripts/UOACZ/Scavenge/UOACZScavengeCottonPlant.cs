using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Custom
{
    public class UOACZScavengeCottonPlant : UOACZBaseScavengeObject
    {
        public override string NoYieldRemainingSingleClickText { get { return "(picked clean)"; } }
        public override string LockedSingleClickText { get { return "(locked)"; } }

        public override string NoYieldRemainingText { get { return "It appears to have been picked clean."; } }
        public override string InteractText { get { return "You begin harvesting."; } }

        public override string ScavengeResultSuccessWithTrapText { get { return "Your harvesting was successful, but something is amiss..."; } }
        public override string ScavengeResultSuccessText { get { return "Your harvesting was successful."; } }
        public override string ScavengeResultFailWithTrapText { get { return "Your harvesting turns up nothing, but something is amiss..."; } }
        public override string ScavengeResultFailText { get { return "You harvest for a while but fail to find anything."; } }

        public override string ScavengeUndeadTrapText { get { return "*noise from harvesting draws unwanted attention...*"; } }

        public UOACZScavengeCottonSpawner m_Spawner;

        [Constructable]
        public UOACZScavengeCottonPlant(): base()
        {
            Name = "cotton plant";
        }

        public UOACZScavengeCottonPlant(Serial serial): base(serial)
        {
        }

        public override void Init()
        {
            ItemID = Utility.RandomList(3151, 3152);

            ScavengeDifficulty = Utility.RandomMinMax(75, 125);

            if (Utility.RandomDouble() <= .25)            
                TrapType = ScavengeTrapType.Undead;

            InteractionRange = 2;

            base.Init();
        }        

        public override void CreateLoot()
        {
            for (int a = 0; a < StartingYieldCount; a++)
            {
                double itemResult = Utility.RandomDouble();

                if (itemResult <= .90)                
                    DropItem(new UOACZCotton());                

                else if (itemResult <= .98)                
                    DropItem(new UOACZHerbs());                               

                else                
                    DropItem(new UOACZHumanUpgradeToken());                
            }

            base.CreateLoot();
        }

        public override void DoAction(PlayerMobile player)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player)) return;

            player.Animate(32, 5, 1, true, false, 0);
            player.RevealingAction();

            Effects.PlaySound(player.Location, player.Map, 0x3E3);
        }

        public override bool GetScavengeResult(PlayerMobile player, bool lockpickAttempt)
        {
            bool scavengeResult = false;

            int minimumPlayerValue = 50;

            int playerForageValue = Utility.RandomMinMax(0, minimumPlayerValue + (int)(Math.Round(player.Skills.Camping.Value * UOACZSystem.scavengeSkillScalar)));
            int forageTarget = Utility.RandomMinMax(0, ScavengeDifficulty);

            if (playerForageValue >= forageTarget)      
                scavengeResult = true;

            if (scavengeResult)
            {
                Effects.PlaySound(player.Location, player.Map, 0x5AB);

                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZHarvesting, 1);

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.ScavengeableCottonItems++;
            }

            else
                Effects.PlaySound(player.Location, player.Map, 0x059);

            return scavengeResult;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)
                m_Spawner.m_Items.Remove(this);
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
                m_Spawner = (UOACZScavengeCottonSpawner)reader.ReadItem();
            }
        }
    }
}