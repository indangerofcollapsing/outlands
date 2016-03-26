using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Custom
{
    public class UOACZScavengeDebris : UOACZBaseScavengeObject
    {
        public UOACZCityScavengeSpawner m_Spawner;

        [Constructable]
        public UOACZScavengeDebris(): base()
        {
            Name = "scavengable debris";             
        }

        public override void Init()
        {
            #region ItemIDs

            switch (Utility.RandomMinMax(1, 22))
            {

                case 1: ItemID = 2324; break;
                case 2: ItemID = 3094; break;
                case 3: ItemID = 3117; break;
                case 4: ItemID = 3118; break;
                case 5: ItemID = 3119; break;
                case 6: ItemID = 3120; break;                
                case 7: ItemID = 6468; break;               
                case 8: ItemID = 7129; break;
                case 9: ItemID = 7132; break;
                case 10: ItemID = 7075; break;
                case 11: ItemID = 7076; break;
                case 12: ItemID = 7082; break;
                case 13: ItemID = 7087; break;
                case 14: ItemID = 7099; break;
                case 15: ItemID = 7100; break;
                case 16: ItemID = 7094; break;
                case 17: ItemID = 7093; break;
                case 18: ItemID = 7083; break;
                case 19: ItemID = 7084; break;
                case 20: ItemID = 7096; break;
                case 21: ItemID = 7101; break;
                case 22: ItemID = 7090; StartingYieldCount += 20; MaxPlayerInteractions = 6; break;

                //case 7: ItemID = 3892; Hue = 1102; break;
                //case 9: ItemID = 7128; break;
                //case 10: ItemID = 7131; break;
                //case 13: ItemID = 7152; break;
                //case 14: ItemID = 7155; break;
                //case 15: ItemID = 7153; break;
                //case 16: ItemID = 7156; break;
                //case 17: ItemID = 7134; break;
                //case 18: ItemID = 7137; break;
                //case 19: ItemID = 7135; break;
                //case 20: ItemID = 7138; break;
            }

            #endregion
            
            ScavengeDifficulty = Utility.RandomMinMax(75, 150);

            if (Utility.RandomDouble() <= .33)
            {
                TrapDifficulty = Utility.RandomMinMax(50, 125);

                switch (Utility.RandomMinMax(1, 14))
                {
                    case 1: TrapType = ScavengeTrapType.Undead; break;
                    case 2: TrapType = ScavengeTrapType.Undead; break;
                    case 3: TrapType = ScavengeTrapType.Undead; break;
                    case 4: TrapType = ScavengeTrapType.Undead; break;
                    case 5: TrapType = ScavengeTrapType.Undead; break;
                    case 6: TrapType = ScavengeTrapType.Undead; break;
                    case 7: TrapType = ScavengeTrapType.Undead; break;
                    case 8: TrapType = ScavengeTrapType.Undead; break;
                    case 9: TrapType = ScavengeTrapType.Undead; break;
                    case 10: TrapType = ScavengeTrapType.Undead; break;
                    case 11: TrapType = ScavengeTrapType.Explosion; break;
                    case 12: TrapType = ScavengeTrapType.Explosion; break;
                    case 13: TrapType = ScavengeTrapType.Hinder; break;
                    case 14: TrapType = ScavengeTrapType.Poison; break;
                }
            }

            base.Init();
        }

        public override void CreateLoot()
        {
            for (int a = 0; a < StartingYieldCount; a++)
            {
                double itemResult = Utility.RandomDouble();

                if (itemResult <= .20)
                {
                    switch (Utility.RandomMinMax(1, 10))
                    {
                        case 1: DropItem(new UOACZGlassOfCider()); break;
                        case 2: DropItem(new UOACZGlassOfLiquor()); break;
                        case 3: DropItem(new UOACZGlassOfMilk()); break;
                        case 4: DropItem(new UOACZGlassOfMilk()); break;
                        case 5: DropItem(new UOACZGlassOfWine()); break;
                        case 6: DropItem(new UOACZBottleOfAle() { Charges = Utility.RandomMinMax(1, 2) }); break;
                        case 7: DropItem(new UOACZBottleOfFruitJuice() { Charges = Utility.RandomMinMax(1, 2) }); break;
                        case 8: DropItem(new UOACZBottleOfLiquor() { Charges = Utility.RandomMinMax(1, 2) }); break;                        
                        case 9: DropItem(new UOACZBottleOfWine() { Charges = Utility.RandomMinMax(1, 2) }); break;
                        case 10: DropItem(new UOACZJugOfCider() { Charges = Utility.RandomMinMax(1, 2) }); break;                       
                    }
                }

                else if (itemResult <= .40)
                {
                    switch (Utility.RandomMinMax(1, 16))
                    {
                        case 1: DropItem(new UOACZSackOfFlour()); break;
                        case 2: DropItem(new UOACZSackOfFlour()); break;
                        case 3: DropItem(new UOACZSackOfFlour()); break;
                        case 4: DropItem(new UOACZVegetable()); break;
                        case 5: DropItem(new UOACZVegetable()); break;
                        case 6: DropItem(new UOACZVegetable()); break;
                        case 7: DropItem(new UOACZFruit()); break;
                        case 8: DropItem(new UOACZFruit()); break;
                        case 9: DropItem(new UOACZFruit()); break;
                        case 10: DropItem(new UOACZMushrooms()); break;
                        case 11: DropItem(new UOACZMushrooms()); break;
                        case 12: DropItem(new UOACZBreadRolls()); break;
                        case 13: DropItem(new UOACZCheeseWheel()); break;
                        case 14: DropItem(new UOACZCuredFish()); break;
                        case 15: DropItem(new UOACZHoney()); break;
                        case 16: DropItem(new UOACZHerbs()); break;   
                    }
                }

                else if (itemResult <= .60)
                {
                    switch (Utility.RandomMinMax(1, 10))
                    {
                        case 1: DropItem(new Arrow(Utility.RandomMinMax(10, 20))); break;
                        case 2: DropItem(new Bolt(Utility.RandomMinMax(10, 20))); break;
                        case 3: DropItem(new Feather(25)); break;
                        case 4: DropItem(new Log(25)); break;
                        case 5: DropItem(new IronIngot(Utility.RandomMinMax(10, 20))); break;
                        case 6: DropItem(new Cloth(Utility.RandomMinMax(3, 5))); break;
                        case 7: DropItem(new Leather(Utility.RandomMinMax(3, 5))); break;
                        case 8: DropItem(new Bottle(Utility.RandomMinMax(2, 4))); break;
                        case 9: DropItem(new UOACZBowl()); break;
                        case 10: DropItem(new UOACZBowl()); break; 
                    }
                }

                else if (itemResult <= .80)
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: DropItem(new Bandage(Utility.RandomMinMax(2, 3))); break;
                        case 2: DropItem(new Bandage(Utility.RandomMinMax(2, 3))); break;
                        case 3: DropItem(new Bandage(Utility.RandomMinMax(3, 4))); break;
                        case 4: DropItem(new Bandage(Utility.RandomMinMax(3, 4))); break;
                        case 5: DropItem(new Bandage(Utility.RandomMinMax(4, 6))); break;
                        case 6: DropItem(new HealPotion()); break;
                        case 7: DropItem(new HealPotion()); break;
                        case 8: DropItem(new CurePotion()); break;
                        case 9: DropItem(new RefreshPotion()); break;                        
                        case 10: DropItem(new UOACZHerbBasket()); break;      
                    }
                }                  

                else if (itemResult <= .90)
                {
                    switch (Utility.RandomMinMax(1, 9))
                    {
                        case 1: DropItem(new UOACZTorch()); break;
                        case 2: DropItem(new UOACZTorch()); break;
                        case 3: DropItem(new UOACZOilFlask()); break;
                        case 4: DropItem(new UOACZOilFlask()); break;
                        case 5: DropItem(new UOACZIronWire()); break;
                        case 6: DropItem(new UOACZIronWire()); break;
                        case 7: DropItem(new UOACZIronWire()); break;
                        case 8: DropItem(new UOACZRope(2)); break;
                        case 9: DropItem(new UOACZRope(2)); break;
                    }
                }

                else if (itemResult <= .95)
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: DropItem(new BlackPearl(Utility.RandomMinMax(2, 4))); break;
                        case 2: DropItem(new Bloodmoss(Utility.RandomMinMax(2, 4))); break;
                        case 3: DropItem(new MandrakeRoot(Utility.RandomMinMax(2, 4))); break;
                        case 4: DropItem(new Garlic(Utility.RandomMinMax(2, 4))); break;
                        case 5: DropItem(new Ginseng(Utility.RandomMinMax(2, 4))); break;
                        case 6: DropItem(new Nightshade(Utility.RandomMinMax(2, 4))); break;
                        case 7: DropItem(new SpidersSilk(Utility.RandomMinMax(2, 4))); break;
                        case 8: DropItem(new SulfurousAsh(Utility.RandomMinMax(2, 4))); break;
                    }
                }

                else if (itemResult <= .98)
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {           
                        case 1: DropItem(new UOACZMortarPestle()); break;
                        case 2: DropItem(new UOACZSaw()); break;
                        case 3: DropItem(new UOACZSkillet()); break;
                        case 4: DropItem(new UOACZTinkersTools()); break;
                        case 5: DropItem(new UOACZTongs()); break;
                        case 6: DropItem(new UOACZRepairHammer()); break;
                        case 7: DropItem(new UOACZLockpickKit()); break;
                        case 8: DropItem(new UOACZFishingPole()); break;
                    }
                }   

                else
                    DropItem(new UOACZHumanUpgradeToken());
            }

            base.CreateLoot();
        }

        public override void DoAction(PlayerMobile player)
        {
            player.Animate(32, 5, 1, true, false, 0);
            player.RevealingAction();

            Effects.PlaySound(player.Location, player.Map, 0x3E3);
        }
        
        public override bool GetScavengeResult(PlayerMobile player, bool lockpickAttempt)
        {
            bool scavengeResult = true;

            if (scavengeResult)
            {
                Effects.PlaySound(player.Location, player.Map, 0x2E2);

                AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZScavenging, 1);

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.ScavengeableDebrisItems++;
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

        public UOACZScavengeDebris(Serial serial): base(serial)
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
                m_Spawner = (UOACZCityScavengeSpawner)reader.ReadItem();
            }
        }
    }
}