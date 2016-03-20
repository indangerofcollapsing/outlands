// Delceri
using System;
using Server;
using Server.Items;

public class EasterBasket : BaseContainer
{
    [Constructable]
    public EasterBasket(): base(0xE7A)
    {
        Name = "an easter basket";

        Weight = 1.0;
        Hue = Utility.RandomMinMax(2, 362);

        int eggs = Utility.RandomMinMax(5, 10);

        for (int a = 0; a < eggs; a++)
        {
            this.DropItem(new EasterEgg());
        }
    }

    public EasterBasket(Serial serial): base(serial)
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