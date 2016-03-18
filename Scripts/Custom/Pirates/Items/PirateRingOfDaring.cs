using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public class PirateRingofDaring : GoldRing
    {
        public override int PlayerClassCurrencyValue { get { return 10000; } }

        public DateTime m_NextUsageAllowed = DateTime.MinValue;
        public TimeSpan m_UsageCooldown = TimeSpan.FromMinutes(60);

        [Constructable]
        public PirateRingofDaring()
        {
            Name = "Pirate Ring of Daring";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;

            LootType = Server.LootType.Blessed; 
        }

        public PirateRingofDaring(Serial serial): base(serial)
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
                        Effects.PlaySound(from.Location, from.Map, 0x012);
                        m_NextUsageAllowed = DateTime.UtcNow + m_UsageCooldown;

                        for (int a = 0; a < 3; a++)
                        {
                            Blood water = new Blood();
                            water.Hue = 2592;
                            water.Name = "water";
                            water.ItemID = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655);

                            Point3D location = new Point3D(from.Location.X + Utility.RandomList(-1, 1), from.Location.Y + Utility.RandomList(-1, 1), from.Z);

                            water.MoveToWorld(location, from.Map);
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
