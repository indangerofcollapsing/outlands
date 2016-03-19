using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Gumps;

namespace Server.Custom
{
    public class MysteryFountain : Item
    {
        public List<Mobile> m_Mobiles = new List<Mobile>();

        public int m_CurrentItem = 0;
        public int m_MaxItems = 6;
        public int m_TreasurePileLevel = 0;

        public FountainAddon m_Fountain;

        [Constructable]
        public MysteryFountain(): base(5635)
        {
            Name = "a mysterious, decrepit fountain";

            Movable = false;
            Hue = 2500;

            //50% Chance for dungeon armor upgrade hammer
            if (Utility.RandomDouble() <= .5)
                m_MaxItems++;

            Timer.DelayCall(TimeSpan.FromMilliseconds(50), new TimerCallback(AddComponents));
        }

        public virtual void AddComponents()
        {
            if (this == null) return;
            if (Deleted) return;

            FountainAddon fountain = new FountainAddon();

            fountain.Hue = 2076;
            fountain.MoveToWorld(Location, Map);
            m_Fountain = fountain;
            
            m_TreasurePileLevel = 1;            

            if (Utility.RandomMinMax(1, 3) == 1)          
                m_TreasurePileLevel = 2;

            if (Utility.RandomMinMax(1, 6) == 1)
                m_TreasurePileLevel = 3;

            if (Utility.RandomMinMax(1, 10) == 1)
                m_TreasurePileLevel = 4;

            Z += 8;
        } 
        
        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }        

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.Alive && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You must be alive to use this.");
                return;
            }

            if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far away to activate this.");
                return;
            }

            if (!from.CanBeginAction(typeof(FishingPole)))
            {
                from.SendMessage("You must wait a few moments before attempting to use that again.");
                return;
            }

            if (m_CurrentItem < m_MaxItems)
            {
                bool hasFishingPole = false;
                FishingPole fishingPole = from.FindItemOnLayer(Layer.TwoHanded) as FishingPole;

                if (fishingPole != null)
                    hasFishingPole = true;

                from.BeginAction(typeof(FishingPole));

                Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                {
                    if (from != null)
                        from.EndAction(typeof(FishingPole));
                });

                from.Direction = Utility.GetDirection(from.Location, Location);

                if (hasFishingPole)
                {
                    from.SendMessage("You fish around within the fountain's water...");
                    from.Animate(11, 5, 1, true, false, 0);

                    from.RevealingAction();
                    
                    Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
                    {
                        if (this == null) return;
                        if (Deleted) return;
                        if (from == null) return;
                        if (!from.Alive || from.Deleted) return;

                        Effects.PlaySound(Location, Map, 0x027);
                    });                   
                }

                else
                {
                    from.SendMessage("You kneel down and begin searching the fountain's water. You feel, however, that a fishing pole and some skill at fishing would be of great benefit here...");

                    from.RevealingAction();

                    from.Animate(32, 5, 1, true, false, 0); //Bend Down
                    Effects.PlaySound(Location, Map, 0x027);
                }
                                                
                int waterCount = Utility.RandomMinMax(1, 2);
                for (int a = 0; a < waterCount; a++)
                {
                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                    water.Name = "foul water";
                    water.Hue = 1107;

                    Point3D waterLocation = new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z);

                    SpellHelper.AdjustField(ref waterLocation, Map, 12, false);
                    water.MoveToWorld(waterLocation, Map);
                }

                Timer.DelayCall(TimeSpan.FromSeconds(5), delegate
                {
                    if (from == null) return;
                    if (from.Deleted || !from.Alive) return;
                    if (this == null) return;
                    if (Deleted) return;

                    RetrievalAttempt(from, hasFishingPole);
                });
            }

            else
                from.SendMessage("The fountain's water appears to be devoid of life and items.");
        }

        public void RetrievalAttempt(Mobile from, bool fishing)
        {
            if (m_CurrentItem >= m_MaxItems)
                return;

            PlayerMobile pm_From = from as PlayerMobile;
            
            if (pm_From == null)
                return;            

            if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far from the fountain to continue searching.");
                return;
            }

            pm_From.RevealingAction();

            double baseChance = .10;

            if (fishing)            
                baseChance *= 1.25 + (.25 * (from.Skills.Fishing.Value / 120));            
            
            if (Utility.RandomDouble() <= baseChance)
            {
                m_CurrentItem++;

                Item item = null;

                switch (m_CurrentItem)
                {
                    case 1:
                        Bag bag = new Bag();

                        for (int a = 0; a < 10; a++)
                        {
                            bag.DropItem(CraftingComponent.GetRandomCraftingComponent(1));
                        }

                        item = bag;
                    break;

                    case 2: item = new MiniatureHouse(); break;
                    case 3:                        switch (m_TreasurePileLevel)
                        {
                            case 1: item = new TreasurePileSmallAddonDeed(); break;
                            case 2: item = new TreasurePileMediumAddonDeed(); break;
                            case 3: item = new TreasurePileLargeAddonDeed(); break;
                            case 4: item = new TreasurePileHugeAddonDeed(); break;
                        }
                    break;
                    case 4: item = new MiniatureTree(); break;  
                    case 5: item = BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Unspecified); break;
                    case 6: item = new MiniatureFountain(); break;
                    case 7: item = new DungeonArmorUpgradeHammer(); break;
                    
                        
                    break;
                }

                if (item != null)
                {
                    if (pm_From.AddToBackpack(item))
                    {
                        pm_From.SendMessage("You retrieve an item from the fountain!");
                        pm_From.PlaySound(0x025);

                        return;
                    }

                    else
                    {
                        item.MoveToWorld(from.Location, from.Map);

                        pm_From.PlaySound(0x025);
                        pm_From.SendMessage("You retrieve an item from the fountain! Your pack, however, was full and the item has been placed at your feet.");

                        return;
                    }
                }

                int waterCount = Utility.RandomMinMax(1, 2);
                for (int a = 0; a < waterCount; a++)
                {
                    TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                    water.Name = "foul water";
                    water.Hue = 1107;

                    Point3D waterLocation = new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z);

                    SpellHelper.AdjustField(ref waterLocation, Map, 12, false);
                    water.MoveToWorld(waterLocation, Map);
                }
            }

            else
            {
                if (Utility.RandomDouble() <= .25)
                {
                    PublicOverheadMessage(MessageType.Regular, 0, false, "something ancient stirs from within the fountain!");

                    int waterCount = Utility.RandomMinMax(3, 5);
                    for (int a = 0; a < waterCount; a++)
                    {
                        TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                        water.Name = "foul water";
                        water.Hue = 1107;

                        Point3D waterLocation = new Point3D(Location.X + Utility.RandomMinMax(-2, 2), Location.Y + Utility.RandomMinMax(-2, 2), Location.Z);

                        SpellHelper.AdjustField(ref waterLocation, Map, 12, false);
                        water.MoveToWorld(waterLocation, Map);
                    }

                    int creatureCount = 0;

                    double result = Utility.RandomDouble();

                    if (result < .10)
                    {
                        creatureCount = 1;

                        for (int a = 0; a < creatureCount; a++)
                        {                            
                            new FountainOfEvil().MoveToWorld(from.Location, from.Map);
                        }
                    }                        

                    else if (result < .15)
                    {
                        creatureCount = Utility.RandomMinMax(1, 2);

                        for (int a = 0; a < creatureCount; a++)
                        {                            
                            new ElderWaterElemental().MoveToWorld(from.Location, from.Map);
                        }
                    }

                    else if (result < .20)
                    {
                        creatureCount = Utility.RandomMinMax(1, 2);

                        for (int a = 0; a < creatureCount; a++)
                        {
                            new ElderToxicElemental().MoveToWorld(from.Location, from.Map);
                        }
                    }

                    else if (result < .25)
                    {
                        creatureCount = Utility.RandomMinMax(1, 2);

                        for (int a = 0; a < creatureCount; a++)
                        {
                            new ElderPoisonElemental().MoveToWorld(from.Location, from.Map);                            
                        }
                    }

                    else if (result < .50)
                    {
                        creatureCount = Utility.RandomMinMax(2, 4);

                        for (int a = 0; a < creatureCount; a++)
                        {
                            WaterElemental waterElemental = new WaterElemental();
                            waterElemental.Hue = 1107;
                            waterElemental.Name = "foul water";
                            waterElemental.MoveToWorld(from.Location, from.Map);
                        }
                    }

                    else if (result < .75)
                    {
                        creatureCount = Utility.RandomMinMax(2, 4);

                        for (int a = 0; a < creatureCount; a++)
                        {
                            new VoidSlime().MoveToWorld(from.Location, from.Map);
                        }
                    }                    

                    else
                    {
                        creatureCount = Utility.RandomMinMax(3, 5);

                        for (int a = 0; a < creatureCount; a++)
                        {
                            Puddle foulPuddle = new Puddle();
                            foulPuddle.Hue = 1107;
                            foulPuddle.Name = "foul puddle";
                            foulPuddle.MoveToWorld(from.Location, from.Map);
                        }
                    }

                    return;
                }

                else
                {
                    pm_From.SendMessage("You search the fountain but are unable to locate anything of value.");
                    return;
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int a = 0; a < m_Mobiles.Count; ++a)
            {
                if (m_Mobiles[a] != null)
                {
                    if (!m_Mobiles[a].Deleted)
                        m_Mobiles[a].Delete();
                }
            }

            if (m_Fountain != null)
            {
                if (!m_Fountain.Deleted)
                    m_Fountain.Delete();
            }

            if (!Deleted)
                Delete();
        }

        public MysteryFountain(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_CurrentItem);
            writer.Write(m_MaxItems);
            writer.Write(m_TreasurePileLevel);
            writer.Write(m_Fountain);

            writer.Write(m_Mobiles.Count);
            for (int a = 0; a < m_Mobiles.Count; a++)
            {
                writer.Write(m_Mobiles[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Mobiles = new List<Mobile>();

            //Version 0
            if (version >= 0)
            {
                m_CurrentItem = reader.ReadInt();
                m_MaxItems = reader.ReadInt();
                m_TreasurePileLevel = reader.ReadInt();
                m_Fountain = reader.ReadItem() as FountainAddon;                

                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();
                    m_Mobiles.Add(creature);
                }
            }
        }
    }    
}
