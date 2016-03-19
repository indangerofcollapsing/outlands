using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class RazorSawTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "A Razor Saw Trap"; } }
        public override string TrapName { get { return "a razor saw trap"; } }
        public override int TrapItemId { get { return 6184; } }
        public override int TrapHue { get { return 1892; } }
        public override int TrapTextHue { get { return 1892; } }

        public override int TriggerRadius { get { return 2; } }
        public override int EffectRadius { get { return 2; } }

        public int effectCount = 25;

        public int minDamage = 50;
        public int maxDamage = 100;

        public double pierceValue = .50;
        public int pierceDuration = 30;

        [Constructable]
        public RazorSawTrap(): base()
        {
        }

        public RazorSawTrap(Serial serial): base(serial)
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
                Effects.PlaySound(location, map, 0x21C);
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
                    Timer.DelayCall(TimeSpan.FromSeconds(a * .10), delegate
                    {
                        Point3D newLocation = m_EffectLocations[Utility.RandomMinMax(0, m_EffectLocations.Count - 1)];

                        Effects.PlaySound(location, map, 0x5CA);

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.50)), Utility.RandomList(0x11AD, 0x11B2), 8, 15, 0, 0, 5029, 0);

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

                            SpecialAbilities.PierceSpecialAbility(1.0, owner, mobile, pierceValue, pierceDuration, -1, true, "", "Razor sawblades pierce your armor, reducing it's effectiveness!");

                            double damage = Utility.RandomMinMax(minDamage, maxDamage);

                            if (mobile is PlayerMobile)

                            damage *= .33;

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, owner, (int)damage, 100, 0, 0, 0, 0);
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
