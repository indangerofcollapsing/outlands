using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles.Vendors.NPC
{
    // THIS IS NOT USED
    // DO NOT DELETE
    // REQUIRED BECAUSE THE CUSTOM VENDORS R DUM
    public class MonsterCoin : Item
    {

        public override double DefaultWeight
        {
            get { return 0.01; }
        }

        [Constructable]
        public MonsterCoin()
            : this(1)
        {
        }

        [Constructable]
        public MonsterCoin(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        public override string DefaultName
        {
            get
            {
                return "Valorite Coin";
            }
        }

        [Constructable]
        public MonsterCoin(int amount)
            : base(0xEF0)
        {
            Hue = 201;
            Stackable = true;
            Amount = amount;
            LootType = LootType.Regular;
            Server.Custom.CurrencyTracking.RegisterMonsterCoin(amount);
        }

        public MonsterCoin(Serial serial)
            : base(serial)
        {
        }

        public override int GetDropSound()
        {
            if (Amount <= 1)
                return 0x2E4;
            else if (Amount <= 5)
                return 0x2E5;
            else
                return 0x2E6;
        }

        public override void OnDelete()
        {
            Server.Custom.CurrencyTracking.DeleteMonsterCoin(this.Amount);
            base.OnDelete();
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

            Server.Custom.CurrencyTracking.RegisterMonsterCoin(this.Amount);
        }
    }
}
