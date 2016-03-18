namespace Server.Items
{

    public class StealthWarriorShirt : Shirt
    {
        [Constructable]
        public StealthWarriorShirt()
        {
            Name = "Stealth Warrior Shirt [1/4]";
            Hue = 902;

            DecorativeEquipment = true;
        }

        public StealthWarriorShirt(Serial serial) : base( serial )
		{
        }

        public override bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutCloth(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class StealthWarriorPants : LongPants
    {
        [Constructable]
        public StealthWarriorPants()
        {
            Name = "Stealth Warrior Pants [2/4]";
            Hue = 902;

            DecorativeEquipment = true;
        }

        public StealthWarriorPants(Serial serial) : base( serial )
		{
        }

        public override bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutCloth(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }


    public class StealthWarriorSandals : Sandals
    {
        [Constructable]
        public StealthWarriorSandals()
        {
            Name = "Stealth Warrior Sandals [3/4]";
            Hue = 902;

            DecorativeEquipment = true;
        }

        public StealthWarriorSandals(Serial serial) : base(serial)
        {
        }

        public override bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutLeather(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class StealthWarriorBandana : Bandana
    {
        [Constructable]
        public StealthWarriorBandana()
        {
            Name = "Stealth Warrior Bandana [4/4]";
            Hue = 902;

            DecorativeEquipment = true;
        }

        public StealthWarriorBandana(Serial serial) : base(serial)
        {
        }

        public override bool Scissor(Mobile from, Scissors scissors) { return Helpers.CutCloth(this, from); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            DecorativeEquipment = true;
        }
    }

    public class BagOfStealthWarriorSuit : Bag
    {
        [Constructable]
        public BagOfStealthWarriorSuit()
        {
            Name = "a bag of Stealth Warrior Suit";

            DropItem(new StealthWarriorShirt());
            DropItem(new StealthWarriorPants());
            DropItem(new StealthWarriorSandals());
            DropItem(new StealthWarriorBandana());

        }

        public BagOfStealthWarriorSuit(Serial serial) : base(serial)
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
