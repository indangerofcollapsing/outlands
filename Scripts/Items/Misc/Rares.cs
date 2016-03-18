using System;

namespace Server.Items
{

	public class RareCraftableItem : Item
	{
		public RareCraftableItem(Serial s) : base(s) { }
		public RareCraftableItem(int itemId) : base(itemId) { }

		public override bool Decays
		{
			get
			{
				return false;
			}
		}
		public override void OnDoubleClick(Mobile from)
		{
			from.SendMessage("This item can be used in rares crafting");
			base.OnDoubleClick(from);
		}
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
		}
	}

	public class Rope : RareCraftableItem
	{
		[Constructable]
		public Rope() : this( 1 )
		{
		}

		[Constructable]
		public Rope( int amount ) : base( 0x14F8 )
		{
            Hue = 2514;

			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		public Rope( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            //-----------

            Hue = 2514;
		}
	}

	public class IronWire : RareCraftableItem
	{
		[Constructable]
		public IronWire() : this( 1 )
		{
		}

		[Constructable]
		public IronWire( int amount ) : base( 0x1876 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		

		public IronWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
				Weight = 5.0;
		}
	}

	public class SilverWire : Item
	{
		[Constructable]
		public SilverWire() : this( 1 )
		{
		}

		[Constructable]
		public SilverWire( int amount ) : base( 0x1877 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		

		public SilverWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
				Weight = 5.0;
		}
	}

	public class GoldWire : Item
	{
		[Constructable]
		public GoldWire() : this( 1 )
		{
		}

		[Constructable]
		public GoldWire( int amount ) : base( 0x1878 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

		

		public GoldWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
				Weight = 5.0;
		}
	}

	public class CopperWire : Item
	{
		[Constructable]
		public CopperWire() : this( 1 )
		{
		}

		[Constructable]
		public CopperWire( int amount ) : base( 0x1879 )
		{
			Stackable = true;
			Weight = 5.0;
			Amount = amount;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		

		public CopperWire( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 && Weight == 2.0 )
				Weight = 5.0;
		}
	}

	public class WhiteDriedFlowers : Item
	{
		[Constructable]
		public WhiteDriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public WhiteDriedFlowers( int amount ) : base( 0xC3C )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		

		public WhiteDriedFlowers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class GreenDriedFlowers : Item
	{
		[Constructable]
		public GreenDriedFlowers() : this( 1 )
		{
		}

		[Constructable]
		public GreenDriedFlowers( int amount ) : base( 0xC3E )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		

		public GreenDriedFlowers( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DriedOnions : Item
	{
		[Constructable]
		public DriedOnions() : this( 1 )
		{
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		[Constructable]
		public DriedOnions( int amount ) : base( 0xC40 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		

		public DriedOnions( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class DriedHerbs : Item
	{
		[Constructable]
		public DriedHerbs() : this( 1 )
		{
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		[Constructable]
		public DriedHerbs( int amount ) : base( 0xC42 )
		{
			Stackable = true;
			Weight = 1.0;
			Amount = amount;
		}

		

		public DriedHerbs( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class HorseShoes : Item
	{
		[Constructable]
		public HorseShoes() : base( 0xFB6 )
		{
			Weight = 3.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public HorseShoes( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ForgedMetal : Item
	{
		[Constructable]
		public ForgedMetal() : base( 0xFB8 )
		{
			Weight = 5.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public ForgedMetal( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class Whip : Item
	{
		[Constructable]
		public Whip() : base( 0x166E )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public Whip( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PaintsAndBrush : Item
	{
		[Constructable]
		public PaintsAndBrush() : base( 0xFC1 )
		{
			Weight = 1.0;
		}

		public PaintsAndBrush( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class PenAndInk : Item
	{
		[Constructable]
		public PenAndInk() : base( 0xFBF )
		{
			Weight = 1.0;
		}

		public PenAndInk( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}

	public class ChiselsNorth : Item
	{
		[Constructable]
		public ChiselsNorth() : base( 0x1026 )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public ChiselsNorth( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class ChiselsWest : Item
	{
		[Constructable]
		public ChiselsWest() : base( 0x1027 )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public ChiselsWest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtyPan : Item
	{
		[Constructable]
		public DirtyPan() : base( 0x9E8 )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtyPan( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtySmallRoundPot : Item
	{
		[Constructable]
		public DirtySmallRoundPot() : base( 0x9E7 )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtySmallRoundPot( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtyPot : Item
	{
		[Constructable]
		public DirtyPot() : base( 0x9E6 )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtyPot( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtyRoundPot : Item
	{
		[Constructable]
		public DirtyRoundPot() : base( 0x9DF )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtyRoundPot( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtyFrypan : Item
	{
		[Constructable]
		public DirtyFrypan() : base( 0x9DE )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtyFrypan( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtySmallPot : Item
	{
		[Constructable]
		public DirtySmallPot() : base( 0x9DD )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtySmallPot( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	public class DirtyKettle : Item
	{
		[Constructable]
		public DirtyKettle() : base( 0x9DC )
		{
			Weight = 1.0;
		}

        public override bool Decays
        {
            get
            {
                return false;
            }
        }

		public DirtyKettle( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}

	////////////////////////////////////////////////////////////
	// IPY Craftable rares and ingredients
	////////////////////////////////////////////////////////////
	[FurnitureAttribute]
	public class DyeableCurtainSouth : Item, Server.Items.IDyable
	{
		[Constructable]
		public DyeableCurtainSouth() : base(0x160E) { Weight = 1.0; }
		public DyeableCurtainSouth(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "curtain"; } }

		public bool Dye(Mobile from, DyeTub sender)
		{
			if (Deleted)
				return false;
			Hue = sender.DyedHue;
			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadEncodedInt();
		}
	}

	[FurnitureAttribute]
	public class DyeableCurtainEast : Item, Server.Items.IDyable
	{
		[Constructable]
		public DyeableCurtainEast() : base(0x160D) { Weight = 1.0; }
		public DyeableCurtainEast(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "curtain"; } }

		public bool Dye(Mobile from, DyeTub sender)
		{
			if (Deleted)
				return false;
			Hue = sender.DyedHue;
			return true;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareFishnetSmall : RareCraftableItem
	{
		public override string DefaultName
		{
			get
			{
				return "fishing net (rare)";
			}
		}

		[Constructable]
		public RareFishnetSmall()
			: base(0x0DCB)
		{
			Weight = 1.0;
		}

		public RareFishnetSmall(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();
		}
	}

	public class RareFishnetLarge : RareCraftableItem
	{
		[Constructable]
		public RareFishnetLarge() : base(0x0DCA) {Weight = 1.0;	}
		public RareFishnetLarge(Serial serial): base(serial) {}
		public override string DefaultName { get { return "fishing net (rare)"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareFlask : RareCraftableItem
	{
		[Constructable]
		public RareFlask() : base(0x182D) { Weight = 1.0; }
		public RareFlask(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "flask (rare)"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}
	public class RareVial : RareCraftableItem
	{
		[Constructable]
		public RareVial() : base(0x21FE) { Weight = 1.0; }
		public RareVial(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "vial"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareArrowBundle : RareCraftableItem
	{
		[Constructable]
		public RareArrowBundle() : base(0x0F40) { Weight = 1.0; }
		public RareArrowBundle(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "arrows"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareBoltBundle : RareCraftableItem
	{
		[Constructable]
		public RareBoltBundle() : base(0x1BFC) { Weight = 1.0; }
		public RareBoltBundle(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "bolts"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareGoldIngotBundle : RareCraftableItem
	{
		[Constructable]
		public RareGoldIngotBundle() : base(0x1BEA) { Weight = 10.0; }
		public RareGoldIngotBundle(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "gold ingots"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareCopperIngotBundle : RareCraftableItem
	{
		[Constructable]
		public RareCopperIngotBundle() : base(0x1BE4) { Weight = 10.0; }
		public RareCopperIngotBundle(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "copper ingots"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareBeeswax : RareCraftableItem
	{
		[Constructable]
		public RareBeeswax() : base(0x1426) { Weight = 1.0; }
		public RareBeeswax(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "beeswax"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareHorseDung : RareCraftableItem
	{
		[Constructable]
		public RareHorseDung() : base(0x0F3B) { Weight = 1.0; }
		public RareHorseDung(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "horse dung"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareBook : RareCraftableItem
	{
		[Constructable]
		public RareBook() : base(0x0FF4) { Weight = 1.0; }
		public RareBook(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "book"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareOpenBook : RareCraftableItem
	{
		[Constructable]
		public RareOpenBook() : base(0x0FBD) { Weight = 1.0; }
		public RareOpenBook(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "book"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareLogPile : RareCraftableItem
	{
		[Constructable]
		public RareLogPile() : base(0x1BE1) { Weight = 25.0; }
		public RareLogPile(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "logs"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareFoldedSheets : RareCraftableItem
	{
		[Constructable]
		public RareFoldedSheets() : base(0x0A92) { Weight = 1.0; }
		public RareFoldedSheets(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "folded sheets"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareChains : RareCraftableItem
	{
		[Constructable]
		public RareChains() : base(0x1A07) { Weight = 10.0; }
		public RareChains(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "chains"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareNecroScroll : RareCraftableItem
	{
		[Constructable]
		public RareNecroScroll() : base(0x2265) { Weight = 1; }
		public RareNecroScroll(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "forbidden words"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}


	public class RareBrokenChair : RareCraftableItem
	{
		[Constructable]
		public RareBrokenChair() : base(0x0C1C) { Weight = 10.0; }
		public RareBrokenChair(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "broken chair"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RarePegboard : RareCraftableItem
	{
		[Constructable]
		public RarePegboard() : base(0x0c39) { Weight = 10.0; }
		public RarePegboard(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "peg board"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareDartboard : RareCraftableItem
	{
		[Constructable]
		public RareDartboard() : base(0x1E2E) { Weight = 10.0; }
		public RareDartboard(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "dartboard"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RareSkeleton : RareCraftableItem
	{
		[Constructable]
		public RareSkeleton() : base(0x1D8F) { Weight = 10.0; }
		public RareSkeleton(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "skeleton (rare)"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	public class RarePot : RareCraftableItem
	{
		[Constructable]
		public RarePot() : base(0x09E0) { Weight = 10.0; }
		public RarePot(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "pot"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}
	}

	// vendor sold! Ingredient in the rares crafting (goldsink item)
	public class TransformationDust : RareCraftableItem
	{
		[Constructable]
		public TransformationDust() : base(0x5745) { Weight = 1.0; LootType = LootType.Blessed; }
		public TransformationDust(Serial serial) : base(serial) { }
		public override string DefaultName { get { return "transformation dust"; } }

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.WriteEncodedInt(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadEncodedInt();
		}

		public override void OnDoubleClick(Mobile from)
		{
			Server.Commands.CommandSystem.Handle(from, "[rctest");
            //base.OnDoubleClick(from);
		}
	}
}