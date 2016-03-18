using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    public class MaggotsMeat : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public MaggotsMeat(): base(0x1871)
        {
            Name = "maggot's meat";
            Weight = 10.0;
        }

        public MaggotsMeat(Serial serial): base(serial)
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

            CorpseExplosion(from);
        }

        public void CorpseExplosion(Mobile from)
        {
            if (this == null) return;
            if (from == null) return;

            int bodyParts = 20;

            int minRadius = 1;
            int maxRadius = 8;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, bodyParts, 20, minRadius, maxRadius, false);

            if (m_ValidLocations.Count == 0)
                return;

            m_NextUseAllowed = DateTime.UtcNow + UsageCooldown;  

            for (int a = 0; a < bodyParts; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .025), delegate
                {
                    if (this == null) return;
                    if (this.Deleted) return;

                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    Effects.PlaySound(newLocation, Map, 0x4F1);

                    IEntity effectStartLocation = new Entity(Serial.Zero, Location, Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 2), Map);

                    int itemId = Utility.RandomList
                        (
                        7389, 7397, 7398, 7395, 7402, 7408, 7407, 7393, 7584, 7405, 7585, 7600, 7587, 7602, 7394,
                        7404, 7391, 7396, 7399, 7403, 7406, 7586, 7599, 7588, 7601, 7392, 7392, 7583, 7597, 7390
                        );

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, 5, 0, false, false, 0, 0);

                    double distance = Utility.GetDistanceToSqrt(Location, newLocation);

                    double destinationDelay = (double)distance * .16;
                    double explosionDelay = ((double)distance * .16) + 1;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        new Blood().MoveToWorld(new Point3D(newLocation.X + Utility.RandomMinMax(-1, 1), newLocation.Y + Utility.RandomMinMax(-1, 1), newLocation.Z + 2), Map);
                        Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(2.0)), itemId, 0, 50, 0, 0, 5029, 0);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(explosionDelay), delegate
                    {
                        DetonateCorpse(newLocation, Map);
                    });
                });
            }
        }

        public static void DetonateCorpse(Point3D location, Map map)
        {
            Effects.PlaySound(location, map, 0x22A);
            Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(1.0)), 0x091D, 5, 10, 134, 0, 5029, 0);
            
            new Blood().MoveToWorld(location, map);
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