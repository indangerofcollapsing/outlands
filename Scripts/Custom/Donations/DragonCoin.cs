using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class DragonCoin : Item
    {
        public override string DefaultName { get { return "Dragon Coin"; } }

        public DragonCoin()
            : this(1)
        {
        }

        [Constructable]
        public DragonCoin(int amount)
            : base(0xEED)
        {
            Stackable = true;
            Amount = amount;
            LootType = Server.LootType.Blessed;
            Hue = 1164;
            Weight = 0.01;
            DonationItem = true;
        }

        public DragonCoin(Serial serial)
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

            DonationItem = true;
        }
    }
}
