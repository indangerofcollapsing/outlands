using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    [FlipableAttribute(0xF43, 0xF44)]
    public class UOACZHatchet : BaseAxe
    {
        public override int BaseMinDamage { get { return 5; } }
        public override int BaseMaxDamage { get { return 10; } }
        public override int BaseSpeed { get { return 40; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public UOACZHatchet(): base(0xF43)
        {
            Weight = 2.0;
        }

        public UOACZHatchet(Serial serial): base(serial)
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