using System;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class SpringSeed : Item
    {
        public int[] forest_Tiles = new int[102]
        {
            0x00C4, 0x00C5, 0x00C6, 0x00C7, 0x00C8, 0x00C9, 0x00CA, 0x00CB,
            0x00CC, 0x00CD, 0x00CE, 0x00CF, 0x00D0, 0x00D1, 0x00D2, 0x00D3,
            0x00D4, 0x00D5, 0x00D6, 0x00D7, 0x00F8, 0x00F9, 0x00FA, 0x00FB,
            0x0100, 0x0101, 0x0102, 0x0103, 0x015D, 0x015E, 0x015F, 0x0160,
            0x0161, 0x0162, 0x0163, 0x0164, 0x0165, 0x0166, 0x0167, 0x0168,
            0x0324, 0x0325, 0x0326, 0x0327, 0x0328, 0x0329, 0x032A, 0x032B,
            0x054F, 0x0550, 0x0551, 0x0552, 0x05F1, 0x05F2, 0x05F2, 0x05F3,
            0x05F4, 0x05F5, 0x05F6, 0x05F7, 0x05F8, 0x05F9, 0x05FA, 0x05FB,
            0x05FC, 0x05FD, 0x05FE, 0x05FF, 0x0600, 0x0601, 0x0602, 0x0603,
            0x0604, 0x0612, 0x0613, 0x0614, 0x0653, 0x0654, 0x0655, 0x0656,
            0x065B, 0x065C, 0x065D, 0x065E, 0x065F, 0x0660, 0x0661, 0x0662,
            0x066B, 0x066C, 0x066D, 0x066E, 0x06AF, 0x06B0, 0x06B1, 0x06B2,
            0x06B3, 0x06B4, 0x06BB, 0x06BC, 0x06BD, 0x06BE
        };

        public bool m_PlayerRequiresForestTiles = true;

        public static int Radius = 5;

        [Constructable]
        public SpringSeed() : this(1)
        {
        }

        [Constructable]
        public SpringSeed(int amount) : base(0xDCF)
        {
            Amount = amount;

            Name = "spring seed";
            Stackable = true;

            Weight = 1.0;
            Hue = 1167;
        }

        public SpringSeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                return;
            }

            from.Target = new InternalTarget(this);
            from.SendMessage("Choose a forest location to plant the seed.");
        }

        private class InternalTarget : Target
        {
            private readonly SpringSeed m_SpringSeed;

            public InternalTarget(SpringSeed springseed) : base(4, true, TargetFlags.None)
            {
                m_SpringSeed = springseed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_SpringSeed == null || m_SpringSeed.Deleted)
                    return;

                if (!m_SpringSeed.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    return;
                }

                LandTarget land = targeted as LandTarget;

                if (land == null)
                    from.SendMessage("You cannot plant a seed there!");

                else
                {
                    IPoint3D targetLocation = targeted as IPoint3D;

                    if (targetLocation == null)
                        return;

                    Point3D location = new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z);
                    Map map = from.Map;

                    bool isForest = false;

                    foreach (int forestTile in m_SpringSeed.forest_Tiles)
                    {
                        if (land.TileID == forestTile || !m_SpringSeed.m_PlayerRequiresForestTiles || from.AccessLevel > AccessLevel.Player)
                        {
                            isForest = true;
                            break;
                        }
                    }

                    if (isForest)
                    {
                        bool hasMulti = false;
                        bool nearbyPlayer = false;

                        int minRange = -1 * SpringSeed.Radius;
                        int maxRange = SpringSeed.Radius + 1;

                        for (int a = minRange; a < maxRange; a++)
                        {
                            for (int b = minRange; b < maxRange; b++)
                            {
                                Point3D newLocation = new Point3D(location.X + a, location.Y + b, location.Z);
                                SpellHelper.AdjustField(ref newLocation, map, 12, false);

                                if (BaseMulti.GetMultisAt(newLocation, map).Count > 0)
                                {
                                    hasMulti = true;
                                    break;
                                }
                            }
                        }

                        if (hasMulti)
                        {
                            from.SendMessage("That location is too near to a building or blocking entity.");
                            return;
                        }

                        if (SpellHelper.IsTown(location, from))
                        {
                            from.SendMessage("You feel this would be better suited to be planted outside of town.");
                            return;
                        }

                        if (!map.CanSpawnMobile(location))
                        {
                            from.SendMessage("That cannot be planted there.");
                            return;
                        }

                        IPooledEnumerable mobilesNearby = map.GetMobilesInRange(location, 8);

                        foreach (Mobile mobile in mobilesNearby)
                        {
                            BaseCreature creature = mobile as BaseCreature;
                            PlayerMobile player = mobile as PlayerMobile;

                            if (mobile != from)
                            {
                                if (creature != null)
                                {
                                    if (creature.Controlled && creature.ControlMaster is PlayerMobile && creature.ControlMaster != from)
                                    {
                                        nearbyPlayer = true;
                                        from.SendMessage("That location is too near another player's tamed creature.");

                                        break;
                                    }
                                }

                                if (player != null)
                                {
                                    nearbyPlayer = true;
                                    from.SendMessage("That location is too near another player.");

                                    break;
                                }
                            }
                        }

                        mobilesNearby.Free();

                        if (nearbyPlayer)
                            return;

                        SpringSeedSpawnSite springSeedSpawnSite = new SpringSeedSpawnSite();
                        springSeedSpawnSite.MoveToWorld(location, map);
                        springSeedSpawnSite.BeginSpawn(from);

                        m_SpringSeed.Amount--;

                        if (m_SpringSeed.Amount <= 0)
                            m_SpringSeed.Delete();
                    }

                    else
                    {
                        from.SendMessage("You cannot plant a seed there!");
                        return;
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}