using System;
using System.Collections.Generic;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class TamableTatteredAncientMummyWrapping : Item
    {
        [Constructable]
        public TamableTatteredAncientMummyWrapping()
            : base(0xE21)
        {
            Name = "tattered ancient mummy wrapping";
            Hue = 2313;
            LootType = Server.LootType.Blessed;
        }

        public TamableTatteredAncientMummyWrapping(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target the zombie you wish to tame.");
            from.Target = new InternalTarget(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }

        public static void WearOff(Zombie z)
        {
            if (z != null && !z.Deleted)
            {
                z.Controlled = false;
                z.ControlMaster = null;
                z.PublicOverheadMessage(Network.MessageType.Regular, 0, false, "The zombie breaks free from the mummy wrapping.");
            }
        }

        public class InternalTarget : Target
        {
            TamableTatteredAncientMummyWrapping m_Wrapping;

            public InternalTarget(TamableTatteredAncientMummyWrapping wrapping)
                : base(12, false, TargetFlags.None)
            {
                m_Wrapping = wrapping;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Wrapping == null || m_Wrapping.Deleted)
                    return;

                if (!(targeted is Zombie))
                {
                    from.SendMessage("You must target a zombie.");
                    return;
                }

                if (from.Followers > 9)
                {
                    from.SendMessage("You may only have 10 followers at a time.");
                    return;
                }

                Zombie zom = targeted as Zombie;

                if (zom.Controlled)
                {
                    from.SendMessage("That target is already wrapped.");
                    return;
                }

                m_Wrapping.Delete();

                zom.Controlled = true;
                zom.ControlMaster = from;
                TimeSpan length = TimeSpan.FromMinutes(Utility.RandomMinMax(5,20));
				zom.TatteredWrappingEnd = DateTime.UtcNow + length;

                Timer.DelayCall(length, delegate
                {
                    WearOff(zom);
                });
            }
        }
    }
}
