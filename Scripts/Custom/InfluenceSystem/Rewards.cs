using System;
using Server.Items;

namespace Server
{
    #region Items
   
    public class BarTopGlassMugs : Item
    {
        [Constructable]
        public BarTopGlassMugs(): base(6465)
        {
            Name = "glass mugs";
            Weight = 5.0;
        }

        public BarTopGlassMugs(Serial serial): base(serial)
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

    public class BarTopGoblets : Item
    {
        [Constructable]
        public BarTopGoblets(): base(6466)
        {
            Name = "goblets";
            Weight = 5.0;
        }

        public BarTopGoblets(Serial serial) : base(serial)
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

    [Flipable(2829, 2830)]
    public class DisplayCaseSmall : Item
    {
        [Constructable]
        public DisplayCaseSmall(): base(2829)
        {
            Name = "a display case";
            Weight = 10.0;
        }

        public DisplayCaseSmall(Serial serial): base(serial)
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

    [Flipable(4202, 4219)]
    public class HideRack : Item
    {
        [Constructable]
        public HideRack(): base(4202)
        {
            Name = "a hide rack";
            Weight = 10.0;
        }

        public HideRack(Serial serial): base(serial)
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

    [Flipable(0x1E6F, 0x1E78)]
    public class UnfinishedWoodenChair : Item
    {
        [Constructable]
        public UnfinishedWoodenChair(): base(0x1E6F)
        {
            Name = "an unfinished wooden chair";
            Weight = 5.0;
        }

        public UnfinishedWoodenChair(Serial serial): base(serial)
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

    [Flipable(0x1E71, 0x1E7A)]
    public class UnfinishedDresser : Item
    {
        [Constructable]
        public UnfinishedDresser(): base(0x1E71)
        {
            Name = "an unfinished wooden dresser";
            Weight = 10.0;
        }

        public UnfinishedDresser(Serial serial): base(serial)
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

    [Flipable(0x1E73, 0x1E7c)]
    public class UnfinishedTableLegs : Item
    {
        [Constructable]
        public UnfinishedTableLegs(): base(0x1E73)
        {
            Name = "an unfinished table legs";
            Weight = 3.0;
        }

        public UnfinishedTableLegs(Serial serial): base(serial)
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

    public class UnfinishedTablePieces : Item
    {
        [Constructable]
        public UnfinishedTablePieces(): base(0x1E75)
        {
            Name = "an unfinished table pieces";
            Weight = 5.0;
        }

        public UnfinishedTablePieces(Serial serial): base(serial)
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

    [Flipable(0x1E76, 0x1E7E)]
    public class UnfinishedBookshelf : Item
    {
        [Constructable]
        public UnfinishedBookshelf(): base(0x1E76)
        {
            Name = "an unfinished bookshelf";
            Weight = 10.0;
        }

        public UnfinishedBookshelf(Serial serial): base(serial)
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

    [Flipable(0x118F, 0x1191)]
    public class TableWithRunner : Item
    {
        [Constructable]
        public TableWithRunner(): base(0x118F)
        {
            Name = "a table with runner";
            Weight = 5.0;
        }

        public TableWithRunner(Serial serial): base(serial)
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

    public class CityLampPost : Item
    {
        [Constructable]
        public CityLampPost(): base(Utility.RandomList(0x0B20, 0x0B22, 0xB24))
        {
            Name = "a lamp post";
            Weight = 10.0;
        }

        public CityLampPost(Serial serial): base(serial)
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

    public class LavishPillow : Item
    {
        [Constructable]
        public LavishPillow(): base(0x13AC)
        {
            Name = "a lavish pillow";
            Weight = 1.0;

            Hue = Utility.RandomList(2076, 2620, 2566);
        }

        public LavishPillow(Serial serial): base(serial)
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

    public class MiniatureHome : Item
    {
        [Constructable]
        public MiniatureHome(): base(Utility.RandomList(0x22C4, 0x22C9, 0x22DE, 0x22DF, 0x22E0, 0x22E1, 0x22F3, 0x22F4, 0x22F5,
            0x22F6, 0x22FB, 0x2300))
        {
            Name = "a miniature home";
            Weight = 1.0;
        }

        public MiniatureHome(Serial serial): base(serial)
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

    public class MiniatureTownPiece : Item
    {
        [Constructable]
        public MiniatureTownPiece(): base(0x2313)
        {
            Weight = 1.0;            

            switch(Utility.RandomMinMax(1, 4))
            {
                case 1: ItemID = 0x2313; Name = "miniature town piece (1 of 4)"; break;
                case 2: ItemID = 0x2314; Name = "miniature town piece (2 of 4)"; break;
                case 3: ItemID = 0x2315; Name = "miniature town piece (3 of 4)"; break;
                case 4: ItemID = 0x2316; Name = "miniature town piece (4 of 4)"; break;
            }
        }

        public MiniatureTownPiece(Serial serial): base(serial)
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

    [Flipable(0x44D5, 0x44D9)]
    public class AntiqueStandingClock : Item
    {
        [Constructable]
        public AntiqueStandingClock(): base(0x44D5)
        {
            Name = "an antique standing clock";
            Weight = 15;
        }

        public AntiqueStandingClock(Serial serial): base(serial)
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

    [Flipable(0x2DDD, 0x2DDE)]
    public class LuxuriousBookstand : Item
    {
        [Constructable]
        public LuxuriousBookstand(): base(0x2DDD)
        {
            Name = "a luxurious bookstand";
            Weight = 5;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousBookstand(Serial serial) : base(serial)
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

    [Flipable(0x2DDF, 0x2DE0)]
    public class LuxuriousCouch : Item
    {
        [Constructable]
        public LuxuriousCouch(): base(0x2DDF)
        {
            Name = "a luxurious couch";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousCouch(Serial serial)
            : base(serial)
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

    [Flipable(0x2DE1, 0x2DE2)]
    public class LuxuriousOvalTable : Item
    {
        [Constructable]
        public LuxuriousOvalTable(): base(0x2DE1)
        {
            Name = "a luxurious oval table";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousOvalTable(Serial serial): base(serial)
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

    [Flipable(0x2DE3, 0x2DE4)]
    public class LuxuriousChair : Item
    {
        [Constructable]
        public LuxuriousChair(): base(0x2DE3)
        {
            Name = "a luxurious chair";
            Weight = 5;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousChair(Serial serial): base(serial)
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

    [Flipable(0x2DE7, 0x2DE8)]
    public class LuxuriousTable : Item
    {
        [Constructable]
        public LuxuriousTable(): base(0x2DE7)
        {
            Name = "a luxurious table";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousTable(Serial serial)
            : base(serial)
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

    [Flipable(0x2DE9, 0x2DE9)]
    public class LargeLuxuriousChest : Item
    {
        [Constructable]
        public LargeLuxuriousChest(): base(0x2DE9)
        {
            Name = "a large, luxurious chest";
            Weight = 15;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LargeLuxuriousChest(Serial serial): base(serial)
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

    [Flipable(0x2DEC, 0x2DED)]
    public class LargeLuxuriousChair : Item
    {
        [Constructable]
        public LargeLuxuriousChair(): base(0x2DEC)
        {
            Name = "a large, luxurious chair";
            Weight = 10;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LargeLuxuriousChair(Serial serial): base(serial)
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

    [Flipable(0x2DEF, 0x2DF0)]
    public class LuxuriousBookshelf : Item
    {
        [Constructable]
        public LuxuriousBookshelf(): base(0x2DEF)
        {
            Name = "a luxurious bookshelf";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousBookshelf(Serial serial)
            : base(serial)
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

    [Flipable(0x2DF1, 0x2DF2)]
    public class LuxuriousChest : Item
    {
        [Constructable]
        public LuxuriousChest(): base(0x2DF1)
        {
            Name = "a luxurious chest";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousChest(Serial serial): base(serial)
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

    [Flipable(0x2DF3 , 0x2DF4)]
    public class LuxuriousStorageBox : Item
    {
        [Constructable]
        public LuxuriousStorageBox(): base(0x2DF3)
        {
            Name = "a luxurious storage box";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousStorageBox(Serial serial): base(serial)
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

    [Flipable(0x2DF5, 0x2DF6)]
    public class LuxuriousReadingDesk : Item
    {
        [Constructable]
        public LuxuriousReadingDesk(): base(0x2DF5)
        {
            Name = "a luxurious reading desk";
            Weight = 20;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousReadingDesk(Serial serial): base(serial)
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

    [Flipable(0x2DD3, 0x2DD4)]
    public class LuxuriousAlchemyTable : Item
    {
        [Constructable]
        public LuxuriousAlchemyTable(): base(0x2DD3)
        {
            Name = "a luxurious alchemy table";
            Weight = 25;

            Hue = Utility.RandomList(2654, 2500, 2588, 2635);
        }

        public LuxuriousAlchemyTable(Serial serial): base(serial)
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
#endregion

    #region AddOns

    //Display Case: Large
    public class DisplayCaseLargeEastAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			    {2816, 0, 0, 0},
                {2818, -1, 0, 0},
                {2813, 0, 0, 3},
                {2815, -1, 0, 3},
		};

        public override BaseAddonDeed Deed { get { return new DisplayCaseLargeEastAddonDeed(); } }

        [Constructable]
        public DisplayCaseLargeEastAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public DisplayCaseLargeEastAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DisplayCaseLargeEastAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DisplayCaseLargeEastAddon(); } }

        [Constructable]
        public DisplayCaseLargeEastAddonDeed()
        {
            Name = "a large display case deed (east)";
        }

        public DisplayCaseLargeEastAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DisplayCaseLargeNorthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			    {2822, 0, 0, 0},
                {2824, 0, -1, 0},
                {2819, 0, 0, 3},
                {2821, 0, -1, 3},
		};

        public override BaseAddonDeed Deed { get { return new DisplayCaseLargeNorthAddonDeed(); } }

        [Constructable]
        public DisplayCaseLargeNorthAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public DisplayCaseLargeNorthAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DisplayCaseLargeNorthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DisplayCaseLargeNorthAddon(); } }

        [Constructable]
        public DisplayCaseLargeNorthAddonDeed()
        {
            Name = "a large display case deed (north)";
        }

        public DisplayCaseLargeNorthAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    } 

    //Display Case: Medium
    public class DisplayCaseMediumEastAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			    {2826, 0, 0, 0},
                {2828, 0, 0, 3},
		};

        public override BaseAddonDeed Deed { get { return new DisplayCaseMediumEastAddonDeed(); } }

        [Constructable]
        public DisplayCaseMediumEastAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public DisplayCaseMediumEastAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DisplayCaseMediumEastAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DisplayCaseMediumEastAddon(); } }

        [Constructable]
        public DisplayCaseMediumEastAddonDeed()
        {
            Name = "a medium display case deed (east)";
        }

        public DisplayCaseMediumEastAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DisplayCaseMediumNorthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			    {2825, 0, 0, 0},
                {2827, 0, 0, 3},
		};

        public override BaseAddonDeed Deed { get { return new DisplayCaseMediumNorthAddonDeed(); } }

        [Constructable]
        public DisplayCaseMediumNorthAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public DisplayCaseMediumNorthAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class DisplayCaseMediumNorthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new DisplayCaseMediumNorthAddon(); } }

        [Constructable]
        public DisplayCaseMediumNorthAddonDeed()
        {
            Name = "a medium display case deed (north)";
        }

        public DisplayCaseMediumNorthAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }   
 
    //Large Bench
    public class LargeBenchEastAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{2961, 0, 0, 0},
            {2962, -1, 0, 0},
		};

        public override BaseAddonDeed Deed { get { return new LargeBenchEastAddonDeed(); } }

        [Constructable]
        public LargeBenchEastAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public LargeBenchEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeBenchEastAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBenchEastAddon(); } }

        [Constructable]
        public LargeBenchEastAddonDeed()
        {
            Name = "a large bench deed (east)";
        }

        public LargeBenchEastAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeBenchNorthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{2963, 0, 0, 0},
            {2964, 0, -1, 0},
		};

        public override BaseAddonDeed Deed { get { return new LargeBenchNorthAddonDeed(); } }

        [Constructable]
        public LargeBenchNorthAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public LargeBenchNorthAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class LargeBenchNorthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new LargeBenchNorthAddon(); } }

        [Constructable]
        public LargeBenchNorthAddonDeed()
        {
            Name = "a large bench deed (north)";
        }

        public LargeBenchNorthAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    } 

    //Hide Rack
    public class HideRackLargeEastAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{4224, 0, 0, 0},
            {4223, -1, 0, 0},
		};

        public override BaseAddonDeed Deed { get { return new HideRackLargeEastAddonDeed(); } }

        [Constructable]
        public HideRackLargeEastAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public HideRackLargeEastAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HideRackLargeEastAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new HideRackLargeEastAddon(); } }

        [Constructable]
        public HideRackLargeEastAddonDeed()
        {
            Name = "a large hide rack deed (east)";
        }

        public HideRackLargeEastAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HideRackLargeNorthAddon : BaseAddon
    {
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
			{4207, 0, 0, 0},
            {4206, 0, -1, 0},
		};

        public override BaseAddonDeed Deed { get { return new HideRackLargeNorthAddonDeed(); } }

        [Constructable]
        public HideRackLargeNorthAddon()
        {
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 4; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);
        }

        public HideRackLargeNorthAddon(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class HideRackLargeNorthAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return new HideRackLargeNorthAddon(); } }

        [Constructable]
        public HideRackLargeNorthAddonDeed()
        {
            Name = "a large hide rack deed (north)";
        }

        public HideRackLargeNorthAddonDeed(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    } 

    #endregion
}