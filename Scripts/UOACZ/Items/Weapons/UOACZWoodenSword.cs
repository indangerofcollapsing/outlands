using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
    [FlipableAttribute(0x13B9, 0x13Ba)]
    public class UOACZWoodenSword : BaseSword
    {
        public override int BaseMinDamage { get { return 5; } }
        public override int BaseMaxDamage { get { return 10; } }
        public override int BaseSpeed { get { return 50; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int BaseHitSound { get { return 0x232; } }
        public override int BaseMissSound { get { return 0x23A; } }

        [Constructable]
        public UOACZWoodenSword(): base(0x13B9)
        {
            Name = "wooden sword";

            Weight = 6.0;
            Hue = 2415;
        }

        public UOACZWoodenSword(Serial serial): base(serial)
        {
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