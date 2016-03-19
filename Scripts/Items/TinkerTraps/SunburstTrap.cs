using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class SunburstTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "A Sunburst Trap"; } }
        public override string TrapName { get { return "a sunburst trap"; } }
        public override int TrapItemId { get { return 6173; } }
        public override int TrapHue { get { return 2515; } }
        public override int TrapTextHue { get { return 149; } }

        public override int TriggerRadius { get { return 2; } }
        public override int EffectRadius { get { return 2; } }

        public int effectCount = 50;

        public int minDamage = 30;
        public int maxDamage = 60;

        public double fireFieldChance = .20;
        public int fireFieldDuration = 30;

        [Constructable]
        public SunburstTrap(): base()
        {
        }

        public SunburstTrap(Serial serial): base(serial)
        {
        }

        public override void Resolve(TinkerTrapPlaced tinkerTrapPlaced)
        {
            if (tinkerTrapPlaced == null)
                base.Resolve(tinkerTrapPlaced);

            Mobile owner = tinkerTrapPlaced.Owner;
            Point3D location = tinkerTrapPlaced.Location;
            Map map = tinkerTrapPlaced.Map;

            Effects.PlaySound(Location, Map, 0x3E5);

            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
            {
                Effects.PlaySound(location, map, 0x5CA);
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
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                    {
                        Point3D newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];

                        Effects.PlaySound(location, map, 0x5CA);

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2515, 0, 5029, 0);

                        bool UOACZ = false;

                        if (UOACZRegion.ContainsItem(tinkerTrapPlaced))
                        {
                            fireFieldChance = .15;
                            UOACZ = true;
                        }

                        if (Utility.RandomDouble() <= fireFieldChance)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                            {
                                if (UOACZ)
                                {
                                    UOACZFirefield fireField = new UOACZFirefield(owner);
                                    fireField.MoveToWorld(newLocation, map);
                                }

                                else
                                {
                                    SingleFireField singleFireField = new SingleFireField(owner, 0, 1, fireFieldDuration, 3, 5, false, true, false, -1, true);
                                    singleFireField.MoveToWorld(newLocation, map);
                                }
                            });
                        }

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
                                damage *= .5;

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, owner, (int)damage, 0, 100, 0, 0, 0);
                        }
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
