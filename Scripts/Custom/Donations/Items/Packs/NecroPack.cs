using System;
using System.Collections.Generic;

namespace Server.Items
{
    public class NecroPack : Bag
    {
        public enum NecroPackType
        {
            Green, 
            Red,
            Black
        }

        public override string DefaultName {
            get {
                return "necromancer's bag";
            }
        }

        [Constructable]
        public NecroPack()
        {
            PackItems((NecroPackType)Utility.Random(3));
        }

        [Constructable]
        public NecroPack(NecroPackType type) {
            PackItems(type);
        }

        public void PackItems(NecroPackType type)
        {
            switch (type) {
                case NecroPackType.Green:
                    DropItem(new BloodyPentagramDeed { Hue = 2006 });
                    DropItem(new DriedOnions { Name = "Shrunken Heads", Hue = 2406 });
                    DropItem(new Item {ItemID = 0x1ECD, Hue = 1268 }); //crystal
                    DropItem(new MonsterStatuette(MonsterStatuetteType.Zombie));
                    DropItem(new Item(0x2556)); //lich's staff
                    DropItem(new Item(0xd18)); //mushrooms
                    break;
                case NecroPackType.Red:
                    DropItem(new BloodyPentagramDeed { Hue = 2118 });
                    DropItem(new DriedOnions { Name = "Shrunken Heads", Hue = 2406 });
                    DropItem(new Item {ItemID = 0x1f19, Hue = 2118 }); //crystal
                    DropItem(new MonsterStatuette(MonsterStatuetteType.Zombie) { Hue = 2118 });
                    DropItem(new Item(0x2556)); //lich's staff
                    DropItem(new Item(0x1844) { Hue = 2117} ); //flask
                    DropItem(new Item(0xd18)); //mushrooms
                    break;
                case NecroPackType.Black:
                    DropItem(new BloodyPentagramDeed { Hue = 2019 });
                    DropItem(new DriedOnions { Name = "Shrunken Heads", Hue = 2406 });
                    DropItem(new Item {ItemID = 0x2206, Hue = 2406 }); //crystal
                    DropItem(new MonsterStatuette(MonsterStatuetteType.Zombie) { Hue = 2106 });
                    DropItem(new Item(0x2556)); //lich's staff
                    DropItem(new Item(0x1844) { Hue = 2406} ); //flask
                    DropItem(new Item(0xd18)); //mushrooms
                    DropItem(new Item(0x26b7)); //mushrooms
                    DropItem(new RecallRune{ItemID = 0x1f15 }); //backwards rune

                    break;

            }
        }

        public NecroPack(Serial serial)
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
        }
    }
}
