using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Custom
{
    public class UOACZScavengeVegetation : UOACZBaseScavengeObject
    {
        public override string NoYieldRemainingSingleClickText { get { return "(picked clean)"; } }
        public override string LockedSingleClickText { get { return "(locked)"; } }

        public override string NoYieldRemainingText { get { return "It appears to have been picked clean."; } }
        public override string InteractText { get { return "You begin foraging."; } }

        public override string ScavengeResultSuccessWithTrapText { get { return "Your foraging was successful, but something is amiss..."; } }
        public override string ScavengeResultSuccessText { get { return "Your foraging was successful."; } }
        public override string ScavengeResultFailWithTrapText { get { return "Your foraging turns up nothing, but something is amiss..."; } }
        public override string ScavengeResultFailText { get { return "You forage for a while but fail to find anything."; } }

        public override string ScavengeUndeadTrapText { get { return "*noise from harvesting draws unwanted attention...*"; } }

        public UOACZScavengeVegetationSpawner m_Spawner;

        [Constructable]
        public UOACZScavengeVegetation(): base()
        {
            Name = "harvestable vegetation";
        }

        public override void Init()
        {
            #region ItemIDs

            switch (Utility.RandomMinMax(1, 22))
            {
                case 1: ItemID = 3894; break;
                case 2: ItemID = 16150; Hue = 0; break;
                case 3: ItemID = 16150; Hue = 2538; break;
                case 4: ItemID = 16150; Hue = 2520; break;
                case 5: ItemID = 16150; Hue = 2625; break;
                case 6: ItemID = 16140; Hue = 2599; break;
                case 7: ItemID = 16140; Hue = 2635; break;
                case 8: ItemID = 16140; Hue = 2215; break;
                case 9: ItemID = 16140; Hue = 2425; break;
                case 10: ItemID = 16140; Hue = 2585; break;
                case 11: ItemID = 16140; Hue = 2712; break;

                case 12: ItemID = 3273; Hue = 0; break;
                case 13: ItemID = 3273; Hue = 2522; break;
                case 14: ItemID = 3273; Hue = 2651; break;
                case 15: ItemID = 3273; Hue = 2587; break;

                case 16: ItemID = 3498; Hue = 0; StartingYieldCount += 10; break;
                case 17: ItemID = 3498; Hue = 2653; StartingYieldCount += 10; break;

                case 18: ItemID = 3239; Hue = 0; break;
                case 19: ItemID = 3237; Hue = 0; break;

                case 20: ItemID = 3272; Hue = 0; break;
                case 21: ItemID = 3272; Hue = 2514; break;

                case 22: ItemID = 3220; Hue = 0; break;
            }

            #endregion
            
            ScavengeDifficulty = Utility.RandomMinMax(75, 150);

            if (Utility.RandomDouble() <= .25)            
                TrapType = ScavengeTrapType.Undead;

            MaxPlayerInteractions = 5;

            base.Init();
        }

        public override void CreateLoot()
        {
            for (int a = 0; a < StartingYieldCount; a++)
            {
                double itemResult = Utility.RandomDouble();

                if (itemResult <= .40)
                {
                    DropItem(new UOACZFruit());
                }

                else if (itemResult <= .80)
                {
                    DropItem(new UOACZVegetable());
                }

                else if (itemResult <= .98)
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: DropItem(new UOACZMushrooms()); break;
                        case 2: DropItem(new UOACZMushrooms()); break;
                        case 3: DropItem(new UOACZHerbs()); break;
                        case 4: DropItem(new UOACZHerbs()); break;
                        case 5: DropItem(new UOACZNestOfEggs()); break;
                        case 6: DropItem(new UOACZNestOfEggs()); break;
                        case 7: DropItem(new UOACZHoney()); break;                        
                        case 8: DropItem(new UOACZPumpkin()); break;                        
                    }
                }

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
                player.m_UOACZAccountEntry.ScavengeableVegetationItems++;
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

        public UOACZScavengeVegetation(Serial serial): base(serial)
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
                m_Spawner = (UOACZScavengeVegetationSpawner)reader.ReadItem();
            }
        }
    }
}