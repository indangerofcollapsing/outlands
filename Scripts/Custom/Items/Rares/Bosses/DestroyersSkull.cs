using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using System.IO;
using Server.Multis;
using Server.Targeting;
using Server.Targets;
using Server.Regions;
using Server.Spells;
using Server.Custom;

namespace Server
{
    public class DestroyersSkull : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public DestroyersSkull(): base(0x2234)
        {
            Name = "skull of the destroyer";
            Weight = 10.0;
            Hue = 2301;
        }

        public DestroyersSkull(Serial serial): base(serial)
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

            FireBreath(from);
        }

        public void FireBreath(Mobile from)
        {
            if (this == null) return;
            if (this.Deleted) return;
            if (from == null) return;

            Effects.PlaySound(Location, Map, 0x227);

            Dictionary<Point3D, double> m_BreathTiles = new Dictionary<Point3D, double>();

            double tileDelay = .10;
            int distance = 8;
            
            m_BreathTiles.Add(Location, 0);

            Direction breathDirection = Direction.South;

            Point3D previousPoint = Location;            
            Point3D nextPoint;

            for (int a = 0; a < distance; a++)
            {
                nextPoint = SpecialAbilities.GetPointByDirection(previousPoint, breathDirection);
                
                bool canFit = SpellHelper.AdjustField(ref nextPoint, Map, 12, false);
                                
                if (Map != null && canFit && Map.InLOS(Location, nextPoint))           
                {
                    if (!m_BreathTiles.ContainsKey(nextPoint))
                        m_BreathTiles.Add(nextPoint, a * tileDelay);                    
                }

                List<Point3D> perpendicularPoints = SpecialAbilities.GetPerpendicularPoints(previousPoint, nextPoint, a + 1);

                foreach (Point3D point in perpendicularPoints)
                {
                    Point3D ppoint = new Point3D(point.X, point.Y, point.Z);

                    canFit = SpellHelper.AdjustField(ref ppoint, Map, 12, false);
                    
                    if (Map != null && canFit && Map.InLOS(Location, ppoint))                    
                    {
                        if (!m_BreathTiles.ContainsKey(ppoint))
                            m_BreathTiles.Add(ppoint, a * tileDelay);                        
                    }
                }

                previousPoint = nextPoint;

                Timer.DelayCall(TimeSpan.FromSeconds(a * tileDelay), delegate
                {
                    if (this == null) return;
                    if (this.Deleted) return;

                    Effects.PlaySound(nextPoint, Map, 0x208);
                });
            }

            foreach (KeyValuePair<Point3D, double> pair in m_BreathTiles)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(pair.Value), delegate
                {
                    if (this == null) return;
                    if (this.Deleted) return;

                    Point3D breathLocation = pair.Key;

                    Effects.SendLocationParticles(EffectItem.Create(breathLocation, Map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 0, 0, 5052, 0);

                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                    {
                        if (this == null) return;
                        if (this.Deleted) return;

                        //Fire Field Resolution
                        if (Utility.RandomDouble() < .25)
                        {
                            int duration = 5;

                            Effects.PlaySound(breathLocation, Map, 0x208);
                            Effects.SendLocationParticles(EffectItem.Create(breathLocation, Map, TimeSpan.FromSeconds(duration)), 0x3996, 10, 100, 5029);

                            for (int a = 1; a < (duration + 1); a++)
                            {
                                Timer.DelayCall(TimeSpan.FromSeconds(a), delegate
                                {
                                    Effects.PlaySound(breathLocation, Map, 0x208);
                                });
                            }
                        }
                    });
                });
            }

            if (m_BreathTiles.Count > 0)
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