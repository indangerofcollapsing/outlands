using System;

namespace Server.Items
{
    [Flipable(0x1537, 0x1538)]
    public class DreadKilt : BaseOuterLegs
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }

        [Constructable]
        public DreadKilt(): this(0)
        {
        }

        [Constructable]
        public DreadKilt(int hue)
            : base(0x1537, hue)
        {
            Weight = 2.0;
            Name = "Dread Kilt";

            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;

            LootType = Server.LootType.Blessed;
        }

        public DreadKilt(Serial serial): base(serial)
        {
        }

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
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