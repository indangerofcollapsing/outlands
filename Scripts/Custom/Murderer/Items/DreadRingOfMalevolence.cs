using System;
using Server.Mobiles;

namespace Server.Items
{
    public class DreadRingOfMalevolence : GoldRing
    {
        public override int PlayerClassCurrencyValue { get { return 10000; } }

        public DateTime m_NextUsageAllowed = DateTime.MinValue;
        public TimeSpan m_UsageCooldown = TimeSpan.FromMinutes(60);

        [Constructable]
        public DreadRingOfMalevolence()
        {
            Name = "Dread Ring of Malevolence";
            Hue = PlayerClassPersistance.MurdererItemHue;

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;
        }

        public DreadRingOfMalevolence(Serial serial): base(serial)
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

            if (!pm_From.Murderer || PlayerClassOwner != pm_From)
            {
                from.SendMessage("Only the Murderous owner of this item may use it.");
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
                        Effects.PlaySound(from.Location, from.Map, 0x653);

                        Point3D location = new Point3D(from.Location.X + 1, from.Location.Y + 1, from.Location.Z);

                        Effects.SendLocationParticles(EffectItem.Create(location, from.Map, TimeSpan.FromSeconds(0.25)), 0x3798, 5, 90, 2499, 0, 5029, 0);

                        m_NextUsageAllowed = DateTime.UtcNow + m_UsageCooldown;
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
