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
    public class DemonwebCocoon : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        [Constructable]
        public DemonwebCocoon(): base(0x10DA)
        {
            Name = "a demonweb cocoon";
            Weight = 10.0;
        }

        public DemonwebCocoon(Serial serial): base(serial)
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

            ShootWebs(from);
        }

        public void ShootWebs(Mobile from)
        {
            if (this == null) return;
            if (this.Deleted) return;
            if (from == null) return;

            int websToAdd = 20;

            int minRadius = 1;
            int maxRadius = 8;

            int websValid = 0;
            
            for (int a = 0; a < websToAdd; a++)
            {
                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, true, false, Location, Map, 1, 20, minRadius, maxRadius, false);
                
                if (m_ValidLocations.Count > 0)
                {
                    websValid++;

                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    int webId = Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z), Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z), Map);

                    Effects.PlaySound(newLocation, Map, 0x4F1);
                    Effects.SendMovingEffect(startLocation, endLocation, webId, 5, 0, false, false, 0, 0);

                    double distance = Utility.GetDistanceToSqrt(Location, newLocation);
                    double destinationDelay = (double)distance * .08;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (this == null) return;
                        if (this.Deleted) return;

                        Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(15)), webId, 5, 4000, 0, 0, 5029, 0);
                    });
                }
            }

            if (websValid > 0)
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