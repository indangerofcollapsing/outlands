using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    [Flipable(0x141B, 0x141C)]
    public class ArmoredOrcMask : BaseArmoredHat
    {
        public override string DefaultName { get { return "an armored orc mask"; } }

        public override bool CanBeDyed
        {
            get
            {
                return false;
            }
        }

        [Constructable]
        public ArmoredOrcMask()
            : base(0x141B)
        {
        }

        public ArmoredOrcMask(Serial serial)
            : base(serial)
        {
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
    }

    public class ArmoredOgreLordMask : BaseArmoredHat
    {
        public override string DefaultName { get { return "an armored ogre lord mask"; } }

        [Constructable]
        public ArmoredOgreLordMask()
            : base(5147)
        {
        }

        public ArmoredOgreLordMask(Serial serial)
            : base(serial)
        {
        }

        public override bool CanBeDyed
        {
            get
            {
                return false;
            }
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
    }

    public class ArmoredOrcishKinMask : BaseArmoredHat
    {
        public override string DefaultName { get { return "an armored mask of orcish kin"; } }

        public override bool CanBeDyed
        {
            get
            {
                return false;
            }
        }

        public override bool OnEquip(Mobile from)
        {
            if (from is PlayerMobile && Hue == 0x8A4)
            {
                var player = from as PlayerMobile;
                int original = Hue;
                Hue = player.Hue & 0xFFF;
                OriginalHue = original;
            }
            return base.OnEquip(from);
        }

        public override bool OnDragLift(Mobile from)
        {
            Hue = OriginalHue;
            return base.OnDragLift(from);
        }

        public override void OnRemoved(object parent)
        {
            Hue = OriginalHue;
            base.OnRemoved(parent);
        }

        [Constructable]
        public ArmoredOrcishKinMask()
            : base(0x141B)
        {
            Hue = 0x8A4;
        }

        public ArmoredOrcishKinMask(Serial serial)
            : base(serial)
        {
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

    }
}
