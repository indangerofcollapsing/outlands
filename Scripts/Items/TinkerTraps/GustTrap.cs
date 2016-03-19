using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class GustTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "A Gust Trap"; } }
        public override string TrapName { get { return "a gust trap"; } }
        public override int TrapItemId { get { return 6179; } }
        public override int TrapHue { get { return 2219; } }
        public override int TrapTextHue { get { return 2219; } }

        public override int TriggerRadius { get { return 3; } }
        public override int EffectRadius { get { return 3; } }

        public int minKnockbackRange = 15;
        public int maxKnockbackrange = 20;

        public int minDamage = 100;
        public int maxDamage = 200;

        [Constructable]
        public GustTrap(): base()
        {
        }

        public GustTrap(Serial serial): base(serial)
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
                Effects.PlaySound(location, map, 0x5C7);
            });  

            int effectRadius = tinkerTrapPlaced.EffectRadius;

            int rows = (effectRadius * 2) + 1;
            int columns = (effectRadius * 2) + 1;

            IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(location, effectRadius);

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

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X, mobile.Location.Y, mobile.Location.Z), mobile.Map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z), map);

                Effects.PlaySound(mobile, mobile.Map, 0x4F1);
                Effects.SendMovingEffect(startLocation, endLocation, 0x1FB7, 5, 0, false, false, 0, 0);

                double damage = Utility.RandomMinMax(minDamage, maxDamage);

                if (mobile is PlayerMobile)
                    damage *= .25;

                int distance = Utility.RandomMinMax(minKnockbackRange, maxKnockbackrange);

                SpecialAbilities.KnockbackSpecialAbility(1.0, location, mobile, mobile, damage, distance, -1, "", "");
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
