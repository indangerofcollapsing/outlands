using Server;
using System;
using System.Collections;
using Server.Multis;
using Server.Targeting;
using Server.Items;

namespace Server.Multis.Deeds
{
	public class SandstoneSpaHouseDeed : HouseDeed
	{
		[Constructable]
		public SandstoneSpaHouseDeed() : base( 0x145, new Point3D( 0, 0, 0 ) )
		{
            this.Name = "Sandstone Spa House Deed";
		}

		public SandstoneSpaHouseDeed( Serial serial ) : base( serial )
		{
		}

		public override bool HasEastFacingDoor()
		{
			return true;
		}

		public override BaseHouse GetHouse( Mobile owner )
		{
			return new SandstoneSpaHouse( owner );
		}

		public override Rectangle2D[] Area{ get{ return SandstoneSpaHouse.AreaArray; } }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
	
	// NOT SURE WHY THE FUCK THIS HOUSE WARPS ON PLACEMENT!
    public class SmallStoneTempleHouseDeed : HouseDeed
    {
        [Constructable]
        public SmallStoneTempleHouseDeed()
            : base(0x149, new Point3D(0, 4, 0))
        {
            this.Name = "Small Stone Temple Deed";
        }

        public SmallStoneTempleHouseDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}


        public override BaseHouse GetHouse(Mobile owner)
        {
            return new SmallStoneTempleHouse(owner);
        }

        public override Rectangle2D[] Area { get { return SmallStoneTempleHouse.AreaArray; } }

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


	// MUST BE REDONE IN MUL! FOUNDATION IS BROKEN.
    public class MagistrateHouseDeed : HouseDeed
    {
        [Constructable]
        public MagistrateHouseDeed()
            : base(0x14B, new Point3D(0, 0, 0))
        {
            this.Name = "Magistrate's House Deed";
        }

        public MagistrateHouseDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new MagistrateHouse(owner);
        }

        public override Rectangle2D[] Area { get { return MagistrateHouse.AreaArray; } }

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

    public class PaladinKeepDeed : HouseDeed
    {
        [Constructable]
        public PaladinKeepDeed()
            : base(0x14F, new Point3D(0, 0, 0))
        {
            this.Name = "Paladin's Keep Deed";
        }

        public PaladinKeepDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return false;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new PaladinKeep(owner);
        }

        public override Rectangle2D[] Area { get { return PaladinKeep.AreaArray; } }

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


    public class GreatPaladinHallDeed : HouseDeed
    {
        [Constructable]
        public GreatPaladinHallDeed()
            : base(0x142, new Point3D(0, 0, 0))
        {
            this.Name = "Great Paladin's Hall Deed";
        }

        public GreatPaladinHallDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new GreatPaladinHall(owner);
        }

        public override Rectangle2D[] Area { get { return GreatPaladinHall.AreaArray; } }

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

    public class StableHouseDeed : HouseDeed
    {
        [Constructable]
        public StableHouseDeed()
            : base(0x151, new Point3D(0, 0, 0))
        {
            this.Name = "Stable Deed";
        }

        public StableHouseDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new StableHouse(owner);
        }

        public override Rectangle2D[] Area { get { return StableHouse.AreaArray; } }

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

    public class ArbiterEstateDeed : HouseDeed
    {
        [Constructable]
        public ArbiterEstateDeed()
            : base(0x159, new Point3D(0, 0, 0))
        {
            this.Name = "Arbiter's Estate Deed";
        }

        public ArbiterEstateDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new ArbiterEstate(owner);
        }

        public override Rectangle2D[] Area { get { return ArbiterEstate.AreaArray; } }

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

    public class TwoStoryBalconyDeed : HouseDeed
    {
        [Constructable]
        public TwoStoryBalconyDeed()
            : base(0x163, new Point3D(0, 2, 0))
        {
            this.Name = "Two Story House with Balcony";
        }

        public TwoStoryBalconyDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return false;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new TwoStoryBalconyHouse(owner);
        }

        public override Rectangle2D[] Area { get { return TwoStoryBalconyHouse.AreaArray; } }

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

    public class EastSmallStoneHouseDeed : HouseDeed
    {
        [Constructable]
        public EastSmallStoneHouseDeed()
            : base(0x15F, new Point3D(0, 0, 0))
        {
            this.Name = "East Small Stone House";
        }

        public EastSmallStoneHouseDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new EastSmallStoneHouse(owner);
        }

        public override Rectangle2D[] Area { get { return EastSmallStoneHouse.AreaArray; } }

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

    public class EastSmallBrickHouseDeed : HouseDeed
    {
        [Constructable]
        public EastSmallBrickHouseDeed()
            : base(0x161, new Point3D(0,0,0))
        {
            this.Name = "East Small Brick House";
        }

        public EastSmallBrickHouseDeed(Serial serial)
            : base(serial)
        {
        }

		public override bool HasEastFacingDoor()
		{
			return true;
		}

        public override BaseHouse GetHouse(Mobile owner)
        {
            return new EastSmallBrickHouse(owner);
        }

        public override Rectangle2D[] Area { get { return EastSmallBrickHouse.AreaArray; } }

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