using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class PirateNecklaceOfAmbition : GoldNecklace
    {
        public override int PlayerClassCurrencyValue { get { return 20000; } }

        public DateTime m_NextUsageAllowed = DateTime.MinValue;
        public TimeSpan m_UsageCooldown = TimeSpan.FromMinutes(60);

        [Constructable]
        public PirateNecklaceOfAmbition()
        {
            Name = "Pirate Necklace of Ambition";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed; 
        }

        public PirateNecklaceOfAmbition(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (!pm_From.Pirate || PlayerClassOwner != pm_From)
            {
                from.SendMessage("Only the Pirate owner of this item may use it.");
                return;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }

            if (from.InRange(Location, 2) || RootParentEntity is PlayerMobile || from.AccessLevel > AccessLevel.Player)
            {
                PlayerMobile pm_Owner = RootParentEntity as PlayerMobile;

                if (from == pm_Owner && from.Alive)
                {
                    from.RevealingAction();

                    if (m_NextUsageAllowed <= DateTime.UtcNow || from.AccessLevel > AccessLevel.Player)
                    {
                        m_NextUsageAllowed = DateTime.UtcNow + m_UsageCooldown;

                        int projectiles = 20;

                        int minRadius = 5;
                        int maxRadius = 10;

                        List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(from.Location, true, false, from.Location, Map, projectiles, 20, minRadius, maxRadius, false);

                        if (m_ValidLocations.Count == 0)
                            return;

                        int particleSpeed = 5;
                        double distanceDelayInterval = .12;

                        for (int a = 0; a < projectiles; a++)
                        {
                            double distance;
                            double destinationDelay;

                            Timer.DelayCall(TimeSpan.FromSeconds(a * .25), delegate
                            {
                                if (this == null) return;
                                if (this.Deleted) return;

                                Effects.SendLocationEffect(from.Location, from.Map, 0x36CB, 10, 0, 0);

                                Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(from.Location.X, from.Location.Y, from.Location.Z + 6), Map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 6), Map);

                                newLocation.Z += 5;

                                Effects.PlaySound(from.Location, Map, 0x664);
                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0xE73, particleSpeed, 0, false, false, 0, 0);
                            });
                        }
                    }

                    else
                        pm_Owner.SendMessage("This item may only be used once per hour.");
                }
            }

            else
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); //Version

            //Version 0
            writer.Write(m_NextUsageAllowed);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            //Version 0
            if (version >= 0)
            {
                m_NextUsageAllowed = reader.ReadDateTime();
            }
        }
    }
}
