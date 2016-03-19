using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
    public class SanguineousTrap : Item
    {
        public enum TrapType
        {
            Saw,
            Spikes
        }

        public TrapType m_TrapType = TrapType.Spikes;
        public Mobile m_Owner;
        public DateTime m_Expiration = DateTime.UtcNow + TimeSpan.FromMinutes(5);
        public DateTime m_NextGlow = DateTime.UtcNow;

        public bool m_TrapSprung = false;
        
        [Constructable]
        public SanguineousTrap(TrapType trapType, Mobile owner, DateTime expiration): base(6179)
        {
            Name = "sanguineous trap";            

            Movable = false;
            Visible = false;

            Hue = 2118;

            m_TrapType = trapType;
            m_Owner = owner;
            m_Expiration = expiration;

            if (OtherTrapOnTile())
                Delete();  
        }

        public void TriggerTrap()
        {
            if (!SpecialAbilities.Exists(m_Owner))
                return;

            Point3D location = Location;
            Map map = Map;
            Mobile owner = m_Owner;

            double effectInterval = .5;
            int loops = 20;

            Timer.DelayCall(TimeSpan.FromSeconds((effectInterval * loops )+ .1), delegate
            {
                if (this == null) 
                    return;
                
                Delete();
            });
            
            switch (m_TrapType)
            {
                case TrapType.Spikes:
                    TimedStatic trap = new TimedStatic(4513, effectInterval * (double)loops);
                    trap.Name = "spike trap";
                    trap.Hue = 0;
                    trap.MoveToWorld(location, map);

                    trap = new TimedStatic(4507, effectInterval * (double)loops);
                    trap.Name = "spike trap";
                    trap.Hue = 0;
                    trap.MoveToWorld(location, map);

                    Effects.PlaySound(location, map, 0x524);

                    for (int a = 0; a < loops; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * effectInterval), delegate
                        {
                            if (this == null) return;
                            if (Deleted) return;
                            if (!SpecialAbilities.Exists(owner)) return;                            

                            bool hitMobile = false;
                            
                            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, 1);

                            Queue m_Queue = new Queue();

                            foreach (Mobile mobile in nearbyMobiles)
                            {
                                if (mobile == owner) continue;
                                if (!SpecialAbilities.MonsterCanDamage(owner, mobile)) continue;

                                m_Queue.Enqueue(mobile);
                                hitMobile = true;
                            }

                            nearbyMobiles.Free();                            

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                double bleedDamage = 8;

                                if (mobile is BaseCreature)
                                    bleedDamage *= 2;

                                SpecialAbilities.BleedSpecialAbility(1.0, null, mobile, bleedDamage, 8.0, -1, true, "", "The trap cuts into you deeply, causing you to bleed.");
                            }

                            if (hitMobile)                            
                                Effects.PlaySound(location, map, 0x524);                            
                        });
                    }
                break;                

                case TrapType.Saw:
                    Point3D adjustedLocation = location;

                    trap = new TimedStatic(4525, effectInterval * (double)loops);
                    trap.Name = "saw trap";
                    trap.Hue = 2118;
                    trap.MoveToWorld(adjustedLocation, map);

                    trap = new TimedStatic(4530, effectInterval * (double)loops);
                    trap.Name = "saw trap";
                    trap.Hue = 2118;
                    trap.MoveToWorld(adjustedLocation, map);

                    adjustedLocation.Y++;

                    trap = new TimedStatic(4530, effectInterval * (double)loops);
                    trap.Name = "saw trap";
                    trap.Hue = 2118;
                    trap.MoveToWorld(adjustedLocation, map);

                    adjustedLocation.Y--;
                    adjustedLocation.X++;

                    trap = new TimedStatic(4525, effectInterval * (double)loops);
                    trap.Name = "saw trap";
                    trap.Hue = 2118;
                    trap.MoveToWorld(adjustedLocation, map);                    

                    Effects.PlaySound(location, map, 0x21C);

                    for (int a = 0; a < loops; a++)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(a * effectInterval), delegate
                        {
                            if (this == null) return;
                            if (Deleted) return;
                            if (!SpecialAbilities.Exists(owner)) return; 

                            bool hitMobile = false;
                            
                            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, 1);

                            Queue m_Queue = new Queue();

                            foreach (Mobile mobile in nearbyMobiles)
                            {
                                if (mobile == owner) continue;
                                if (!SpecialAbilities.MonsterCanDamage(owner, mobile)) continue;

                                m_Queue.Enqueue(mobile);
                                hitMobile = true;
                            }

                            nearbyMobiles.Free();                            

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                SpecialAbilities.PierceSpecialAbility(1.0, null, mobile, .1, 60, -1, true, "", "The trap pierces your armor, temporarily weakening it!");

                                double damage = (double)Utility.RandomMinMax(4, 8);

                                if (mobile is BaseCreature)
                                    damage *= 2;

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, null, (int)damage, 100, 0, 0, 0, 0);
                            }

                            if (hitMobile)
                                Effects.PlaySound(location, map, 0x21C);
                        });
                    }
                break;
            }
        }

        public SanguineousTrap(Serial serial): base(serial)
        {
        }

        public bool OtherTrapOnTile()
        {
            IPooledEnumerable itemsOnTile = GetItemsInRange(0);

            bool foundOtherTrap = false;

            foreach (Item item in itemsOnTile)
            {
                if (item is SanguineousTrap)
                {
                    foundOtherTrap = true;
                    break;
                }
            }

            itemsOnTile.Free();

            return foundOtherTrap;
        }

        public override bool OnMoveOver(Mobile mobile)
        {
            if (mobile == null) return true;
            if (mobile.Deleted || !mobile.Alive) return true;

            if (m_TrapSprung)
                return true;

            if (m_Owner == null)
                return true;

            if (m_Owner == mobile)
                return true;

            if (SpecialAbilities.MonsterCanDamage(m_Owner, mobile))
            {
                m_TrapSprung = true;

                mobile.PublicOverheadMessage(Network.MessageType.Regular, 0, false, "*springs a trap*");
                Effects.PlaySound(Location, Map, 0x3E5);

                TriggerTrap();
            }

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version   

            writer.Write((int)m_TrapType);
            writer.Write(m_Owner);
            writer.Write(m_Expiration);
            writer.Write(m_TrapSprung);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_TrapType = (TrapType)reader.ReadInt();
                m_Owner = reader.ReadMobile();
                m_Expiration = reader.ReadDateTime();
                m_TrapSprung = reader.ReadBool();
            }
        }
    }
}