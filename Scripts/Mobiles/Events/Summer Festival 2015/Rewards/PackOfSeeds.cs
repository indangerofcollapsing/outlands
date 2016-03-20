using System;
using Server;
using Server.Items;
using Server.Engines.Plants;

public class PackOfSeeds : Pouch
{
    [Constructable]
    public PackOfSeeds()
    {
        Name = "a pack of seeds";

        Seed seed = new Seed();
        seed.Amount = Utility.RandomMinMax(8, 12);

        DropItem(seed);
    }

    public PackOfSeeds(Serial serial) : base(serial)
    {
    }

    public override void Serialize(GenericWriter writer)
    {
        base.Serialize(writer);
        writer.Write((int)0); 
    }

    public override void Deserialize(GenericReader reader)
    {
        base.Deserialize(reader);
        int version = reader.ReadInt();
    }
}