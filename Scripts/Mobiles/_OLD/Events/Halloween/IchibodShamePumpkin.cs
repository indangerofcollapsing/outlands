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
    public class IchibodShamePumpkin : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public IchibodShamePumpkin(): base(18065)
        {
            Name = "the head of Ichibod Shame";

            Hue = 2075;
            Weight = 5.0;
        }

        public IchibodShamePumpkin(Serial serial): base(serial)
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

            PumpkinExplosion(from);
        }

        public void PumpkinExplosion(Mobile from)
        {
            if (this == null) return;
            if (from == null) return;

            int pumpkins = 100;

            int minRadius = 1;
            int maxRadius = 8;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(from.Location, true, false, from.Location, Map, pumpkins, 50, minRadius, maxRadius, false);

            if (m_ValidLocations.Count == 0)
                return;

            m_NextUseAllowed = DateTime.UtcNow + UsageCooldown;

            Effects.PlaySound(Location, Map, 0x246);

            for (int a = 0; a < pumpkins; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .025), delegate
                {
                    if (this == null) return;
                    if (this.Deleted) return;

                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    Effects.PlaySound(newLocation, Map, Utility.RandomList(0x5D3, 0x5D2, 0x5A2, 0x580));

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 3), Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), Map);

                    int itemId = Utility.RandomList(3178, 3179, 3180);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, 5, 0, false, false, 0, 0);

                    double distance = Utility.GetDistanceToSqrt(Location, newLocation);

                    double destinationDelay = (double)distance * .10;                  

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        Effects.PlaySound(newLocation, Map, Utility.RandomList(0x5DE, 0x5DA, 0x5D8));

                        TimedStatic pumpkinExplosion = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4655), 5);
                        pumpkinExplosion.Name = "smashed pumpkin";
                        pumpkinExplosion.Hue = 1359;
                        pumpkinExplosion.MoveToWorld(newLocation, Map);
                    });
                });
            }
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