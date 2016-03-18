using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;

namespace Server.Items
{
    public class DerelictCargo : Item
    {
        public enum CrateType
        {
            Bronze,
            Silver,
            Gold
        }

        public CrateType m_CrateType = CrateType.Bronze;

        public int m_HitPoints = 250;

        public List<Item> m_Items = new List<Item>();

        [Constructable]
        public DerelictCargo(): base()
        {
            Name = "derelict cargo (common)";
            ItemID = Utility.RandomList(4014, 2473, 3703, 3715, 3711, 3645, 3644, 3647, 3648, 2475, 3710, 7808, 7809);

            double typeChance = Utility.RandomDouble();

            Hue = 1039;

            if (typeChance >= .80 && typeChance < .98)
            {
                m_CrateType = CrateType.Silver;
                Hue = 2500;
                Name = "derelict cargo (valuable)";
            }

            if (typeChance >= .98)
            {
                m_CrateType = CrateType.Gold;
                Hue = 2213;
                Name = "derelict cargo (rare)";
            }
                                    
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        public DerelictCargo(Serial serial): base(serial)
        {
        }

        public void AddComponents()
        {
            if (Deleted)
                return;           

            //Seaweed
            GroupItem(new Static(Utility.RandomList(3514, 3515)), -1, 0, 0);
            GroupItem(new Static(Utility.RandomList(3514, 3515)), 0, -1, 0);
            GroupItem(new Static(Utility.RandomList(3514, 3515)), 1, 0, 0);
            GroupItem(new Static(Utility.RandomList(3514, 3515)), 0, 1, 0);
            GroupItem(new Static(Utility.RandomList(3514, 3515)), 1, -1, 0);
            GroupItem(new Static(Utility.RandomList(3514, 3515)), -1, 1, 0);

            //Debris
            GroupItem(new Static(3117), -1, 0, 1);
            GroupItem(new Static(7604), 0, -1, 1);
            GroupItem(new Static(3119), 1, 0, 1);
            GroupItem(new Static(3118), 0, 1, 1);

            this.Z += 1;

            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("This may be destroyed with cannon fire.");
        }

        public virtual void GroupItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (item != null)
                item.Movable = false;

            m_Items.Add(item);

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Delete();

            this.Delete();
        }

        public void TakeDamage(Mobile from, int damage)
        {
            if (from == null)
                return;
            
            if (damage < 1)
                damage = 1;
            
            //Break
            if (damage >= m_HitPoints)
            {   
                Effects.PlaySound(Location, Map, 0x11B); 

                m_HitPoints = 0;
                
                PlayerMobile player = from as PlayerMobile;
                BaseBoat playerBoat = BaseBoat.FindBoatAt(player.Location, player.Map);

                int doubloonValue = 10;

                switch (m_CrateType)
                {
                    case CrateType.Bronze: doubloonValue = Utility.RandomMinMax(10, 25); break;
                    case CrateType.Silver: doubloonValue = Utility.RandomMinMax(25, 100); break;
                    case CrateType.Gold: doubloonValue = Utility.RandomMinMax(100, 250); break;
                }

                if (player != null && playerBoat != null)
                {
                    if (!playerBoat.Deleted)
                    {
                        double playerClassGearBonus = 1 + (PlayerClassPersistance.PlayerClassCurrencyBonusPerItem * (double)PlayerClassPersistance.GetPlayerClassArmorItemCount(player, PlayerClass.Pirate));

                        doubloonValue = (int)((double)doubloonValue * playerClassGearBonus);

                        if (doubloonValue < 1)
                            doubloonValue = 1;
                        
                        if (playerBoat.DepositDoubloons(doubloonValue))
                        {
                            Doubloon doubloonPile = new Doubloon(doubloonValue);
                            player.SendSound(doubloonPile.GetDropSound());
                            doubloonPile.Delete();

                            playerBoat.doubloonsEarned += doubloonValue;

                            player.SendMessage("You have recovered " + doubloonValue.ToString() + " doubloons worth of materials in the cargo! The coins have been placed in your ship's hold.");

                        }

                        else
                            player.SendMessage("You have earned doubloons, but alas there was not enough room in your ship's hold to place them all!");
                    }
                }                            

                Point3D startLocation = Location;
                IEntity effectStartLocation = new Entity(Serial.Zero, Location, Map);

                Effects.SendLocationParticles(effectStartLocation, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 0, 0, 5044, 0);

                int debrisCount = Utility.RandomMinMax(6, 8);

                for (int a = 0; a < debrisCount; a++)
                {
                    int itemId;

                    Point3D endLocation;
                    IEntity effectEndLocation;

                    if (Utility.RandomDouble() <= .80)
                    {
                        itemId = Utility.RandomList(7041, 7044, 7053, 7067, 7068, 7069, 7070, 7604, 7605);
                        endLocation = new Point3D(X + Utility.RandomMinMax(-3, 3), Y + Utility.RandomMinMax(-3, 3), Z + 5);
                        
                        effectEndLocation = new Entity(Serial.Zero, endLocation, Map);
                    }

                    else
                    {
                        itemId = Utility.RandomList(3117, 3118, 3119, 3120);
                        endLocation = new Point3D(X + Utility.RandomMinMax(-2, 2), Y + Utility.RandomMinMax(-2, 2), Z);

                        effectEndLocation = new Entity(Serial.Zero, endLocation, Map);
                    }

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, 5, 0, false, false, 0, 0);

                    double distance = Utility.GetDistanceToSqrt(startLocation, endLocation);

                    double destinationDelay = (double)distance * .16;
                    double explosionDelay = ((double)distance * .16) + 1;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (this == null)
                            return;

                        Blood debris = new Blood();
                        debris.Name = "debris";
                        debris.ItemID = itemId;

                        debris.MoveToWorld(endLocation, Map);
                        Effects.PlaySound(endLocation, Map, 0x027);
                    });
                }                

                Delete();
            }

            //Damage
            else
            {
                int debrisCount = Utility.RandomMinMax(1, 2);

                for (int a = 0; a < debrisCount; a++)
                {
                    int itemId = Utility.RandomList(7041, 7044, 7053, 7067, 7068, 7069, 7070, 7604, 7605);

                    Blood debris = new Blood();
                    debris.Name = "debris";
                    debris.ItemID = itemId;

                    Point3D endLocation = new Point3D(X + Utility.RandomMinMax(-2, 2), Y + Utility.RandomMinMax(-2, 2), Z + 5);

                    debris.MoveToWorld(endLocation, Map);
                    Effects.PlaySound(endLocation, Map, 0x027);
                }

                //Wood Breaking Sound
                Effects.PlaySound(Location, Map, Utility.RandomList(0x11D)); //0x3B7, 0x3B5, 0x3B1

                m_HitPoints -= damage;
            } 
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write((int)m_CrateType);

            writer.Write(m_HitPoints);

            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            m_CrateType = (CrateType)reader.ReadInt();
           
            m_HitPoints = reader.ReadInt();

            int itemsCount = reader.ReadInt();

            for (int i = 0; i < itemsCount; ++i)
            {
                m_Items.Add(reader.ReadItem());
            }
        }
    }
}