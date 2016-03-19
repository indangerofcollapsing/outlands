using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class WebTrap : TinkerTrap
    {
        public override string TrapLabel { get { return "A Web Trap"; } }
        public override string TrapName { get { return "a web trap"; } }
        public override int TrapItemId { get { return 6183; } }
        public override int TrapHue { get { return 1150; } }
        public override int TrapTextHue { get { return 1150; } }

        public override int TriggerRadius { get { return 3; } }
        public override int EffectRadius { get { return 3; } }

        public int minDuration = 8;
        public int maxDuration = 12;

        [Constructable]
        public WebTrap(): base()
        {
        }

        public WebTrap(Serial serial): base(serial)
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
                Effects.PlaySound(location, map, 0x580);
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
                    
                    double duration = Utility.RandomMinMax(minDuration, maxDuration);

                    if (mobile is PlayerMobile)
                        duration *= .5;

                    SpecialAbilities.EntangleSpecialAbility(1.0, owner, mobile, 1.0, duration, -1, false, "", "");

                    int webId = Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X, mobile.Location.Y, mobile.Location.Z), mobile.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z), map);

                    Effects.PlaySound(mobile, mobile.Map, 0x4F1);
                    Effects.SendMovingEffect(startLocation, endLocation, webId, 5, 0, false, false, 0, 0);

                    double distance = mobile.GetDistanceToSqrt(location);
                    double destinationDelay = (double)distance * .08;

                    Point3D mobileLocation = mobile.Location;
                    Map mobileMap = mobile.Map;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {                        
                        Point3D targetLocation = mobileLocation;
                        targetLocation.Z += 1;

                        Effects.SendLocationParticles(EffectItem.Create(targetLocation, mobileMap, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308), 0, 150, 0, 0, 5029, 0);
                        targetLocation.Z += 1;

                        Effects.SendLocationParticles(EffectItem.Create(targetLocation, mobileMap, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308), 0, 150, 0, 0, 5029, 0);
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
