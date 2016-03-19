using System;
using Server;
using Server.Items;

public class BagOfDirt : Bag
{
    [Constructable]
    public BagOfDirt()
    {
        Name = "a bag of dirt";

        DropItem(new FertileDirt(Utility.RandomMinMax(8, 12)));
    }

    public BagOfDirt(Serial serial) : base(serial)
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