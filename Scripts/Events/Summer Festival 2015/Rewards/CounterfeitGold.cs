using System;

namespace Server.Items
{
    public class CounterfeitGold : Item
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }

        [Constructable]
        public CounterfeitGold() : base(0xEEF)
        {
            Name = ("Counterfeit Gold");

            Weight = 10.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("It looks remarkably similar to real gold.");
            from.SendSound(0x2E6);
        }

        public override int GetDropSound()
        {
            return 0x2E6;
        }

        public CounterfeitGold(Serial serial) : base(serial)
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