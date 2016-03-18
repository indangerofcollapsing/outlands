using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class HuntingSnare : TinkerTrap
    {
        public override string TrapLabel { get { return "A Hunting Snare"; } }
        public override string TrapName { get { return "a hunting snare"; } }
        public override int TrapItemId { get { return 6182; } }
        public override int TrapHue { get { return 2563; } }
        public override int TrapTextHue { get { return 2563; } }

        public override int TriggerRadius { get { return 4; } }
        public override int EffectRadius { get { return 4; } }
        public override bool SingleLine { get { return true; } }

        public int minDuration = 40;
        public int maxDuration = 60;

        [Constructable]
        public HuntingSnare(): base()
        {
        }

        public HuntingSnare(Serial serial): base(serial)
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
                Effects.PlaySound(location, map, 0x64B);
            });

            List<Point3D> m_EffectLocations = new List<Point3D>();

            int triggerRadius = TriggerRadius;

            if (UOACZRegion.ContainsItem(tinkerTrapPlaced))
                triggerRadius += 1;

            int rows = (triggerRadius * 2) + 1;
            int columns = (triggerRadius * 2) + 1;
            
            bool eastWest = true;

            if (tinkerTrapPlaced.LineFacing == Direction.West || tinkerTrapPlaced.LineFacing == Direction.East)
                eastWest = false;

            if (eastWest)
            {
                for (int a = 0; a < rows; a++)
                {
                    Point3D newPoint = new Point3D(tinkerTrapPlaced.Location.X + (-1 * (triggerRadius)) + a, tinkerTrapPlaced.Location.Y, tinkerTrapPlaced.Location.Z);

                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                    if (!m_EffectLocations.Contains(newPoint))
                        m_EffectLocations.Add(newPoint);                    
                }
            }

            else
            {
                for (int b = 0; b < columns; b++)
                {
                    Point3D newPoint = new Point3D(tinkerTrapPlaced.Location.X, tinkerTrapPlaced.Location.Y + (-1 * (triggerRadius)) + b, tinkerTrapPlaced.Location.Z);

                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                    if (!m_EffectLocations.Contains(newPoint))
                        m_EffectLocations.Add(newPoint);
                }
            }

            foreach (Point3D point in m_EffectLocations)
            {
                int itemId = 0x3967;

                if (!eastWest)
                    itemId = 0x3979;

                TimedStatic trapField = new TimedStatic(itemId, .5);
                trapField.Name = "hunting snare";
                trapField.Hue = 2563;
                trapField.MoveToWorld(point, map);

                IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(point, 1);

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

                    if (mobile is UOACZBaseUndead)
                        duration *= .1;

                    if (mobile is PlayerMobile)
                        duration *= .05;

                    mobile.PublicOverheadMessage(MessageType.Regular, 0, false, "*entangled*");

                    mobile.FixedEffect(0x376A, 10, 16, 2562, 0);
                    mobile.PlaySound(0x204);  

                    SpecialAbilities.EntangleSpecialAbility(1.0, owner, mobile, 1.0, duration, 0, true, "", "You have been caught in a hunting snare!");
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
