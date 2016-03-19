using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class BleedingSpikeTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "A Bleeding Spike Trap"; } }
        public override string TrapName { get { return "a bleeding spike trap"; } }
        public override int TrapItemId { get { return 6175; } }
        public override int TrapHue { get { return 2117; } }
        public override int TrapTextHue { get { return 2117; } }

        public override int TriggerRadius { get { return 3; } }
        public override int EffectRadius { get { return 3; } }

        public int minDamage = 100;
        public int maxDamage = 200;

        [Constructable]
        public BleedingSpikeTrap(): base()
        {
        }

        public BleedingSpikeTrap(Serial serial): base(serial)
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
                Effects.PlaySound(location, map, 0x524);
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

            foreach (Point3D point in m_EffectLocations)
            {
                Effects.SendLocationParticles(EffectItem.Create(point, map, TimeSpan.FromSeconds(0.25)), Utility.RandomList(0x119B, 0x11A1), 10, 10, 0, 0, 5029, 0);

                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(point, 0);

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

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);

                    Blood blood = new Blood();
                    Point3D bloodLocation = new Point3D(mobile.Location.X + Utility.RandomList(-1, 1), mobile.Location.Y + Utility.RandomList(-1, 1), mobile.Location.Z);
                    blood.MoveToWorld(bloodLocation, Map);

                    double damage = Utility.RandomMinMax(minDamage, maxDamage);

                    if (mobile is PlayerMobile)
                        damage *= .33;

                    SpecialAbilities.BleedSpecialAbility(1.0, owner, mobile, damage, 8.0, -1, true, "", "");                    
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
