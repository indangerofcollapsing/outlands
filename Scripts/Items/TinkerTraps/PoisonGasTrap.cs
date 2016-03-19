using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class PoisonGasTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "A Poison Gas Trap"; } }
        public override string TrapName { get { return "a poison gas trap"; } }
        public override int TrapItemId { get { return 6177; } }
        public override int TrapHue { get { return 2209; } }
        public override int TrapTextHue { get { return 2209; } }

        public override int TriggerRadius { get { return 2; } }
        public override int EffectRadius { get { return 2; } }

        public int effectCount = 25;

        public int minPoisonLevel = 2;
        public int maxPoisonLevel = 4;

        [Constructable]
        public PoisonGasTrap(): base()
        {
        }

        public PoisonGasTrap(Serial serial): base(serial)
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
                Effects.PlaySound(location, map, 0x231);
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

                        Point3D effectLocation = newLocation;
                        effectLocation.Z += 5;

                        Effects.SendLocationParticles(EffectItem.Create(effectLocation, map, TimeSpan.FromSeconds(0.50)), 0x11A6, 10, 20, 0, 0, 5029, 0);

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

                            int poisonLevel = Utility.RandomMinMax(minPoisonLevel, maxPoisonLevel);

                            Poison poison = Poison.GetPoison(poisonLevel);

                            mobile.ApplyPoison(owner, poison);
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
