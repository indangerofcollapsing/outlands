using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;


namespace Server.Custom
{
    public class UOACZScavengeContainer : UOACZBaseScavengeObject
    {
        public UOACZCityScavengeSpawner m_Spawner;

        [Constructable]
        public UOACZScavengeContainer(): base()
        {
            Name = "scavengable container";
        }

        public override void Init()
        {
            bool lockable = true;

            #region ItemIDs

            switch (Utility.RandomMinMax(1, 25))
            {
                case 1: Name = "small chest"; ItemID = 2472; break;
                case 2: Name = "small chest"; ItemID = 3712; break;
                case 3: Name = "small crate"; ItemID = 2473; break;
                case 4: Name = "small crate"; ItemID = 3710; break;
                case 5: Name = "small chest"; ItemID = 2474; break;
                case 6: Name = "small chest"; ItemID = 3709; break;
                case 7: Name = "kettle"; ItemID = 2541; lockable = false; break;
                case 8: Name = "large vase"; ItemID = 2885; lockable = false; break;
                case 9: Name = "large vase";  ItemID = 2887; lockable = false; break;
                case 10: Name = "small vase"; ItemID = 2886; lockable = false; break;
                case 11: Name = "small vase"; ItemID = 2888; lockable = false; break;
                case 12: Name = "metal chest"; ItemID = 2475; break;
                case 13: Name = "metal chest"; ItemID = 3708; break;
                case 14: Name = "large crate"; ItemID = 3644; break;
                case 15: Name = "large crate"; ItemID = 3645; break;
                case 16: Name = "medium crate"; ItemID = 3646; break;
                case 17: Name = "medium crate"; ItemID = 3647; break;
                case 18: Name = "metal chest"; ItemID = 3648;  break;
                case 19: Name = "metal chest"; ItemID = 3649; break;
                case 20: Name = "wooden chest"; ItemID = 3650; break;
                case 21: Name = "wooden chest"; ItemID = 3651; break;
                case 22: Name = "barrel"; ItemID = 3703; lockable = false; break;
                case 23: Name = "closed barrel"; ItemID = 4014; break;
                case 24: Name = "large vase"; ItemID = 17074; lockable = false; break;
                case 25: Name = "large vase"; ItemID = 17075; lockable = false; break;
            }

            #endregion

            ScavengeDifficulty = Utility.RandomMinMax(100, 200);

            if (lockable)
            {
                if (Utility.RandomDouble() <= .33)                
                    Locked = true;     
            }

            if (Utility.RandomDouble() <= .40)
            {
                TrapDifficulty = Utility.RandomMinMax(50, 100);

                switch (Utility.RandomMinMax(1, 15))
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
                    case 10: TrapType = ScavengeTrapType.Explosion; break;
                    case 11: TrapType = ScavengeTrapType.Explosion; break;
                    case 12: TrapType = ScavengeTrapType.Explosion; break;
                    case 13: TrapType = ScavengeTrapType.Poison; break;
                    case 14: TrapType = ScavengeTrapType.Poison; break;
                    case 15: TrapType = ScavengeTrapType.Hinder; break;
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
                        case 6: DropItem(new UOACZBottleOfAle() { Charges = Utility.RandomMinMax(1, 3) }); break;
                        case 7: DropItem(new UOACZBottleOfFruitJuice() { Charges = Utility.RandomMinMax(1, 3) }); break;
                        case 8: DropItem(new UOACZBottleOfLiquor() { Charges = Utility.RandomMinMax(1, 3) }); break;
                        case 9: DropItem(new UOACZBottleOfWine() { Charges = Utility.RandomMinMax(1, 3) }); break;
                        case 10: DropItem(new UOACZJugOfCider() { Charges = Utility.RandomMinMax(1, 3) }); break;
                    }
                }

                else if (itemResult <= .40)
                {
                    switch (Utility.RandomMinMax(1, 20))
                    {
                        case 1: DropItem(new UOACZBowlOfDough()); break;
                        case 2: DropItem(new UOACZSackOfFlour()); break;
                        case 3: DropItem(new UOACZVegetable()); break;
                        case 4: DropItem(new UOACZFruit()); break;
                        case 5: DropItem(new UOACZMushrooms()); break;
                        case 6: DropItem(new UOACZNestOfEggs()); break;
                        case 7: DropItem(new UOACZCheeseWheel()); break;
                        case 8: DropItem(new UOACZHoney()); break;
                        case 9: DropItem(new UOACZHerbs()); break;
                        case 10: DropItem(new UOACZPumpkin()); break;
                        case 11: DropItem(new UOACZHoneyBread()); break;
                        case 12: DropItem(new UOACZBakedPie()); break;
                        case 13: DropItem(new UOACZBreadLoaves()); break;
                        case 14: DropItem(new UOACZBreadRolls()); break;
                        case 15: DropItem(new UOACZCheesePastry()); break;
                        case 16: DropItem(new UOACZMeatPie()); break;
                        case 17: DropItem(new UOACZTrayOfRolls()); break;
                        case 18: DropItem(new UOACZVegetableStew()); break;
                        case 19: DropItem(new UOACZPumpkinSoup()); break;
                        case 20: DropItem(new UOACZMushroomSoup()); break;
                    }
                }

                else if (itemResult <= .60)
                {
                    switch (Utility.RandomMinMax(1, 10))
                    {
                        case 1: DropItem(new Arrow(Utility.RandomMinMax(15, 30))); break;
                        case 2: DropItem(new Bolt(Utility.RandomMinMax(15, 30))); break;
                        case 3: DropItem(new Feather(50)); break;
                        case 4: DropItem(new Log(40)); break;
                        case 5: DropItem(new IronIngot(Utility.RandomMinMax(15, 30))); break;
                        case 6: DropItem(new Cloth(Utility.RandomMinMax(5, 10))); break;
                        case 7: DropItem(new Leather(Utility.RandomMinMax(4, 6))); break;
                        case 8: DropItem(new Bottle(Utility.RandomMinMax(3, 5))); break;
                        case 9: DropItem(new UOACZBowl()); break;
                        case 10: DropItem(new UOACZBowl()); break; 
                    }
                }

                else if (itemResult <= .70)
                {
                    switch (Utility.RandomMinMax(1, 10))
                    {
                        case 1: DropItem(new Bandage(Utility.RandomMinMax(3, 5))); break;
                        case 2: DropItem(new Bandage(Utility.RandomMinMax(3, 5))); break;
                        case 3: DropItem(new Bandage(Utility.RandomMinMax(4, 6))); break;
                        case 4: DropItem(new Bandage(Utility.RandomMinMax(4, 6))); break;
                        case 5: DropItem(new Bandage(Utility.RandomMinMax(5, 7))); break;
                        case 6: DropItem(new HealPotion()); break;
                        case 7: DropItem(new HealPotion()); break;
                        case 8: DropItem(new CurePotion()); break;
                        case 9: DropItem(new RefreshPotion()); break;
                        case 10: DropItem(new UOACZHerbBasket()); break; 
                    }
                }                

                else if (itemResult <= .80)
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

                else if (itemResult <= .90)
                {
                    switch (Utility.RandomMinMax(1, 8))
                    {
                        case 1: DropItem(new BlackPearl(Utility.RandomMinMax(4, 6))); break;
                        case 2: DropItem(new Bloodmoss(Utility.RandomMinMax(4, 6))); break;
                        case 3: DropItem(new MandrakeRoot(Utility.RandomMinMax(4, 6))); break;
                        case 4: DropItem(new Garlic(Utility.RandomMinMax(4, 6))); break;
                        case 5: DropItem(new Ginseng(Utility.RandomMinMax(4, 6))); break;
                        case 6: DropItem(new Nightshade(Utility.RandomMinMax(4, 6))); break;
                        case 7: DropItem(new SpidersSilk(Utility.RandomMinMax(4, 6))); break;
                        case 8: DropItem(new SulfurousAsh(Utility.RandomMinMax(4, 6))); break;
                    }
                }

                else if (itemResult <= .94)
                {
                    switch (Utility.RandomMinMax(1, 9))
                    {   
                        case 1: DropItem(new UOACZMortarPestle()); break;
                        case 2: DropItem(new UOACZSaw()); break;
                        case 3: DropItem(new UOACZSkillet()); break;
                        case 4: DropItem(new UOACZTinkersTools()); break;
                        case 5: DropItem(new UOACZTongs()); break;
                        case 6: DropItem(new UOACZRepairHammer()); break;
                        case 7: DropItem(new UOACZLockpickKit()); break;
                        case 8: DropItem(new UOACZFishingPole()); break;
                        case 10: DropItem(new UOACZPickaxe()); break;
                    }
                }

                else if (itemResult <= .98)
                {
                    if (Utility.RandomDouble() <= .80)
                    {
                        switch(Utility.RandomMinMax(1, 2))
                        {
                            case 1: DropItem(Loot.RandomWeapon()); break;
                            case 2: DropItem(Loot.RandomArmorOrShield()); break;
                        }
                    }

                    else if (Utility.RandomDouble() <= .95)
                    {
                        switch (Utility.RandomMinMax(1, 2))
                        {
                            case 1:
                                BaseWeapon weapon = Loot.RandomWeapon();

                                weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(1, 3);
                                weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(1, 3);
                                weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(1, 3);

                                weapon.Identified = true;

                                DropItem(weapon);
                            break;

                            case 2:
                                BaseArmor armor = Loot.RandomArmorOrShield();

                                armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(1, 3);
                                armor.Durability = (ArmorDurabilityLevel)Utility.RandomMinMax(1, 3);

                                armor.Identified = true;

                                DropItem(armor);
                            break;
                        }
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
            bool scavengeResult = false;

            int minimumPlayerValue = 50;

            double searcherValue = 1 + player.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Searcher);

            minimumPlayerValue = (int)(Math.Round((double)minimumPlayerValue * searcherValue));

            if (lockpickAttempt)
            {
                int playerLockpickValue = Utility.RandomMinMax(0, minimumPlayerValue + (int)(Math.Round(player.Skills.Lockpicking.Value * UOACZSystem.scavengeSkillScalar)));
                int lockpickTarget = Utility.RandomMinMax(0, LockDifficulty);

                if (playerLockpickValue >= LockDifficulty)
                {
                    Effects.PlaySound(player.Location, player.Map, 0x4A);
                    scavengeResult = true;
                }

                else
                    Effects.PlaySound(player.Location, player.Map, 0x3A4);
            }

            else
            {
                scavengeResult = true;
                Effects.PlaySound(player.Location, player.Map, 0x2E2);                
            }

            if (scavengeResult)
            {
                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                player.m_UOACZAccountEntry.ScavengeableContainerItems++;
            }

            return scavengeResult;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)
                m_Spawner.m_Items.Remove(this);
        }

        public UOACZScavengeContainer(Serial serial): base(serial)
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