using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class AvalancheTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "An Avalanche Trap"; } }
        public override string TrapName { get { return "an avalanche trap"; } }
        public override int TrapItemId { get { return 6174; } }
        public override int TrapHue { get { return 2310; } }
        public override int TrapTextHue { get { return 2310; } }

        public override int TriggerRadius { get { return 2; } }
        public override int EffectRadius { get { return 2; } }

        public int effectCount = 25;

        public int minDamage = 100;
        public int maxDamage = 200;

        [Constructable]
        public AvalancheTrap(): base()
        {
        }

        public AvalancheTrap(Serial serial): base(serial)
        {
        }

        public override void Resolve(TinkerTrapPlaced tinkerTrapPlaced)
        {
            if (tinkerTrapPlaced == null)
            {
                base.Resolve(tinkerTrapPlaced);
                return;
            }   

            Mobile owner = tinkerTrapPlaced.Owner;
            Point3D location = tinkerTrapPlaced.Location;
            Map map = tinkerTrapPlaced.Map;

            Effects.PlaySound(Location, Map, 0x3E5);

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                Effects.PlaySound(location, map, 0x221);
            });            

            int effectRadius = tinkerTrapPlaced.EffectRadius;

            int rows = (effectRadius * 2) + 1;
            int columns = (effectRadius * 2) + 1;

            List<Point3D> m_EffectLocations = new List<Point3D>();

            for (int a = 1; a < rows + 1; a++)
            {
                for (int b = 1; b < columns + 1; b++)
                {
                    Point3D newPoint = new Point3D(location.X + (-1 * (effectRadius + 1)) + a, location.Y + (-1 * (effectRadius + 1)) + b, location.Z);

                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                    if (!m_EffectLocations.Contains(newPoint))
                        m_EffectLocations.Add(newPoint);
                }
            }

            if (m_EffectLocations.Count > 0)
            {
                for (int a = 0; a < effectCount; a++)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .1), delegate
                    {
                        Point3D newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(newLocation.X - 1, newLocation.Y - 1, newLocation.Z + 50), map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                        int rockItemId = Utility.RandomList(6001, 6002, 6003, 6004, 4963, 4967, 4970, 4973);
                        int rockHue = 2314;  

                        Effects.SendMovingParticles(startLocation, endLocation, rockItemId, 8, 0, false, false, rockHue, 0, 9501, 0, 0, 0x100);

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            Blood rocks = new Blood();
                            rocks.Name = "rock";
                            rocks.Hue = rockHue;
                            rocks.ItemID = rockItemId;

                            rocks.MoveToWorld(newLocation, map);

                            Blood dirt = new Blood();
                            dirt.Name = "dirt";
                            dirt.ItemID = Utility.RandomList(7681, 7682);
                            Point3D dirtLocation = new Point3D(newLocation.X + Utility.RandomList(-1, 1), newLocation.Y + Utility.RandomList(-1, 1), newLocation.Z);
                            dirt.MoveToWorld(dirtLocation, Map);

                            Effects.PlaySound(newLocation, map, 0x11D);

                            IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newLocation, 0);

                            Queue m_Queue = new Queue();

                            foreach (Mobile mobile in mobilesOnTile)
                            {
                                bool validTarget = TinkerTrap.IsValidMobileTarget(mobile);
                                
                                if (validTarget)
                                    m_Queue.Enqueue(mobile);
                            }

                            mobilesOnTile.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                if (mobile is PlayerMobile)
                                    damage *= .25;

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, owner, (int)damage, 100, 0, 0, 0, 0);
                            }
                        });
                    });
                }
            }

            base.Resolve(tinkerTrapPlaced);
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
