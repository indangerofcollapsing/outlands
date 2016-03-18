using System;

namespace Server.Items
{
    public class Gold : Item
    {
        public override double DefaultWeight
        {
            get { return (Core.ML ? (0.02 / 3) : 0.02); }
        }

        [Constructable]
        public Gold()
            : this(1)
        {
        }

        [Constructable]
        public Gold(int amountFrom, int amountTo)
            : this(Utility.RandomMinMax(amountFrom, amountTo))
        {
        }

        [Constructable]
        public Gold(int amount)
            : base(0xEED)
        {
            if (amount < 0)
                amount = 0;

            Stackable = true;
            Amount = amount;
            Name = "gold coin";
            Server.Custom.CurrencyTracking.RegisterGold(Amount);
        }

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            return dropped.Movable && Movable && base.StackWith(from, dropped, playSound);
        }

        public Gold(Serial serial)
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

        public override int GetTotal(TotalType type)
        {
            int baseTotal = base.GetTotal(type);

            if (type == TotalType.Gold)
                baseTotal += this.Amount;

            return baseTotal;
        }

        protected override void OnAmountChange(int oldValue)
        {
            int newValue = this.Amount;

            UpdateTotal(this, TotalType.Gold, newValue - oldValue);

            Server.Custom.CurrencyTracking.RegisterGold(newValue - oldValue);
        }

        public override void OnDelete()
        {
            Server.Custom.CurrencyTracking.DeleteGold(this.Amount);
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

            Server.Custom.CurrencyTracking.RegisterGold(this.Amount);
        }
    }
}