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
    public class NecromancersGravestone : Item
    {
        public DateTime m_NextUseAllowed;
        public TimeSpan UsageCooldown = TimeSpan.FromMinutes(10);

        public RuneType m_RuneType;

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return false; } }

        public enum RuneType
        {
            Fire,
            Ice,
            Earth,
            Energy,
            Poison,
            Curse
        }

        public static int fireHue = 2117;
        public static int iceHue = 2121;
        public static int earthHue = 2108;
        public static int energyHue = 20;
        public static int poisonHue = 59;
        public static int curseHue = 2103;

        [Constructable]
        public NecromancersGravestone(): base(0x0EDD)
        {
            Name = "ancient necromancer's gravestone";
            Weight = 10.0;
            Hue = 2563;
        }

        public NecromancersGravestone(Serial serial): base(serial)
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

            Activate(from);
        }

        public void Activate(Mobile from)
        {
            if (this == null) return;
            if (from == null) return;

            Point3D m_RuneLocation = from.Location;

            //Necromancer Rune
            m_RuneType = (RuneType)Utility.RandomMinMax(0, 5);

            int runeHue = fireHue;

            switch (m_RuneType)
            {
                case RuneType.Fire: runeHue = fireHue; break;
                case RuneType.Ice: runeHue = iceHue; break;
                case RuneType.Earth: runeHue = earthHue; break;
                case RuneType.Energy: runeHue = energyHue; break;
                case RuneType.Poison: runeHue = poisonHue; break;
                case RuneType.Curse: runeHue = curseHue; break;
            }

            Dictionary<Point3D, int> m_RunePieces = new Dictionary<Point3D, int>();

            m_RunePieces.Add(new Point3D(m_RuneLocation.X - 1, m_RuneLocation.Y - 1, m_RuneLocation.Z), 0x3083);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X - 1, m_RuneLocation.Y, m_RuneLocation.Z), 0x3080);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X, m_RuneLocation.Y - 1, m_RuneLocation.Z), 0x3082);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X + 1, m_RuneLocation.Y - 1, m_RuneLocation.Z), 0x3081);

            m_RunePieces.Add(new Point3D(m_RuneLocation.X -1, m_RuneLocation.Y + 1, m_RuneLocation.Z), 0x307D);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X, m_RuneLocation.Y, m_RuneLocation.Z), 0x307F);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X + 1, m_RuneLocation.Y, m_RuneLocation.Z), 0x307E);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X, m_RuneLocation.Y + 1, m_RuneLocation.Z), 0x307C);
            m_RunePieces.Add(new Point3D(m_RuneLocation.X + 1, m_RuneLocation.Y + 1, m_RuneLocation.Z), 0x307B);            

            foreach (KeyValuePair<Point3D, int> pair in m_RunePieces)
            {
                Blood runePiece = new Blood();
                runePiece.ItemID = pair.Value;
                runePiece.Name = "a rune";
                runePiece.Hue = runeHue;
                runePiece.MoveToWorld(pair.Key, Map);
            }

            Blood flame = new Blood();
            flame.ItemID = 0x19AB;
            flame.Name = "a runeflame";
            flame.Hue = runeHue;
            flame.MoveToWorld(new Point3D(m_RuneLocation.X, m_RuneLocation.Y, m_RuneLocation.Z), Map);

            Effects.SendLocationParticles(EffectItem.Create(m_RuneLocation, Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 125, runeHue, 0, 5029, 0);

            int projectiles = 20;

            int minRadius = 1;
            int maxRadius = 8;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(m_RuneLocation, true, false, m_RuneLocation, Map, projectiles, 20, minRadius, maxRadius, false);

            if (m_ValidLocations.Count == 0)
                return;

            m_NextUseAllowed = DateTime.UtcNow + UsageCooldown;

            int particleSpeed = 5; //10
            double distanceDelayInterval = .12; //.08

            for (int a = 0; a < projectiles; a++)
            {
                double distance;
                double destinationDelay;

                Timer.DelayCall(TimeSpan.FromSeconds(a * .25), delegate
                {
                    if (this == null) return;
                    if (this.Deleted) return;

                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(m_RuneLocation.X, m_RuneLocation.Y, m_RuneLocation.Z + 3), Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 6), Map);

                    newLocation.Z += 5;

                    switch (m_RuneType)
                    {
                        case RuneType.Fire:
                            Effects.PlaySound(m_RuneLocation, Map, 0x359);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x36D4, particleSpeed, 0, false, false, 0, 0);

                            distance = Utility.GetDistanceToSqrt(m_RuneLocation, newLocation);
                            destinationDelay = (double)distance * distanceDelayInterval;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;

                                Effects.PlaySound(newLocation, Map, 0x591);
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(0.5)), 0x36BD, 20, 10, 5044);
                            });
                        break;

                        case RuneType.Ice:
                            Effects.PlaySound(m_RuneLocation, Map, 0x64F);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x36D4, particleSpeed, 0, false, false, 1153, 0);

                            distance = Utility.GetDistanceToSqrt(m_RuneLocation, newLocation);
                            destinationDelay = (double)distance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;

                                Effects.PlaySound(newLocation, Map, 0x208);
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 1153, 0, 5029, 0);
                            });
                        break;

                        case RuneType.Earth:
                            int itemId = Utility.RandomList(4963, 4964, 4965, 4966, 4968, 4969, 4971, 4972);

                            Effects.PlaySound(m_RuneLocation, Map, 0x5D3);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, particleSpeed, 0, false, false, 0, 0);

                            distance = Utility.GetDistanceToSqrt(m_RuneLocation, newLocation);
                            destinationDelay = (double)distance * distanceDelayInterval;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;

                                Effects.PlaySound(newLocation, Map, 0x11D);

                                int mudItems = Utility.RandomMinMax(1, 2);

                                //Rubble
                                Blood dirt = new Blood();
                                dirt.Name = "rubble";
                                dirt.ItemID = Utility.RandomList(7681, 7682);

                                Point3D dirtLocation = new Point3D(newLocation.X, newLocation.Y, newLocation.Z);

                                dirt.MoveToWorld(dirtLocation, Map);
                            });
                        break;

                        case RuneType.Energy:
                            Effects.PlaySound(m_RuneLocation, Map, 0x211);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x3818, particleSpeed, 0, false, false, runeHue, 0);

                            distance = Utility.GetDistanceToSqrt(m_RuneLocation, newLocation);
                            destinationDelay = (double)distance * distanceDelayInterval;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;

                                Effects.PlaySound(newLocation, Map, 0x211);
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(5)), 0x3973, 10, 125, 5029);
                            });
                        break;

                        case RuneType.Poison:
                            Effects.PlaySound(m_RuneLocation, Map, 0x22F);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x372A, particleSpeed, 0, false, false, runeHue, 0);

                            distance = Utility.GetDistanceToSqrt(m_RuneLocation, newLocation);
                            destinationDelay = (double)distance * distanceDelayInterval;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;

                                Effects.PlaySound(newLocation, Map, 0x22F);
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(0.25)), 0x372A, 10, 20, runeHue, 0, 5029, 0);
                            });
                        break;

                        case RuneType.Curse:
                            Effects.PlaySound(m_RuneLocation, Map, 0x56E);
                            Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 0x573E, particleSpeed, 0, false, false, 0, 0);

                            distance = Utility.GetDistanceToSqrt(m_RuneLocation, newLocation);
                            destinationDelay = (double)distance * distanceDelayInterval;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;

                                Effects.PlaySound(newLocation, Map, 0x56E);
                                Effects.SendLocationParticles(EffectItem.Create(newLocation, Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, runeHue, 0, 5029, 0);
                            });
                        break;
                    }
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