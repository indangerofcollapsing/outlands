using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;

namespace Server
{
    public class OrbFromTheDeep : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public OrbFromTheDeep(): base(13807)
        { 
            Name = "an orb from the deep";

            Hue = 2600;
            Weight = 10.0;
        }

        public OrbFromTheDeep(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null)
                return;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }

            if (ParentEntity != null)
            {
                from.SendMessage("That may not be used while inside a container.");
                    return;
            }

            if (!from.InRange(Location, 2) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            if (m_NextUseAllowed > DateTime.UtcNow && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You may only use this item once every 10 minutes.");
                return;
            }                     

            WaveBlast(from);
        }

        public void WaveBlast(Mobile from)
        {
            if (this == null) return;
            if (Deleted) return;
            if (from == null) return;

            Effects.PlaySound(Location, Map, 0x656);

            int radius = 10;

            int minRange = -1 * radius;
            int maxRange = radius + 1;

            for (int a = minRange; a < maxRange; a++)
            {
                for (int b = minRange; b < maxRange; b++)
                {
                    Point3D location = Location;
                    Map map = Map;

                    Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);
                    SpellHelper.AdjustField(ref newLocation, map, 12, false);

                    if (!map.InLOS(location, newLocation))
                        continue;

                    double distance = Utility.GetDistanceToSqrt(location, newLocation);
                    double distanceDelay = (distance * .125);                   

                    Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay), delegate
                    {
                        if (Utility.RandomDouble() <= .66)                        
                            Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2121, 0, 5029, 0);                        
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(distanceDelay + 1.25), delegate
                    {
                        if (Utility.RandomDouble() <= .25)
                            return;

                        TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 10);
                        water.Name = "water";
                        water.Hue = 2120;

                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        Effects.PlaySound(newLocation, map, 0x027);
                        water.MoveToWorld(newLocation, map);                            
                    });
                }
            }
            
            m_NextUseAllowed = DateTime.UtcNow + UsageCooldown;  
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            writer.Write(m_NextUseAllowed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_NextUseAllowed = reader.ReadDateTime();
            }
        }
    }
}