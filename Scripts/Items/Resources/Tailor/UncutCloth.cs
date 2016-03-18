using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
    public class UltraRareCloth : UncutCloth
    {
        public static int[] Hues
        {
            get
            {
                return new int[]
                {  
                    2707,
                    2718,
                    2711,
                    2716,
                    1175
                };
            }
        }

        [Constructable]
        public UltraRareCloth(): base(Utility.RandomMinMax(3, 8))
        {
            Hue = Utility.RandomList(Hues);
        }

        public UltraRareCloth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class RareCloth : UncutCloth
    {
        public static int[] Hues
        {
            get
            {
                return new int[] {
                  2707, // dark reddish brown
                  1762,
                  1777,
                  1772,
                  1757,
                };
            }
        }

        [Constructable]
        public RareCloth()
            : base(Utility.RandomMinMax(3, 8))
        {
            Hue = Utility.RandomList(Hues);
        }

        public RareCloth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class TownCloth : UncutCloth
    {
        public static int[] Hues
        {
            get
            {
                return new int[] {
                  2125, // brit
                  1109, // cove
                  1761, // jhelom
                  1158, // magincia
                  2123, // minoc
                  1278, // moonglow
                  2118, // nujel'm
                  1156, // ocllo
                  1157, // serps
                  1196, // skara
                  1151, // trinsic
                  1366, // vesper
                  2212, // yew
                };
            }
        }

        [Constructable]
        public TownCloth()
            : base(Utility.RandomMinMax(3, 8))
        {
            Hue = Utility.RandomList(Hues);
        }

        public TownCloth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class BossCloth : UncutCloth
    {
        public static int[] Hues
        {
            get
            {
                return new int[] {
                  1150, // white
                  1281, // gold
                  1266, // vesper
                  2052, // almost black
                };
            }
        }

        [Constructable]
        public BossCloth()
            : base(Utility.RandomMinMax(3, 8))
        {
            Hue = Utility.RandomList(Hues);
        }

        public BossCloth(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }


	[FlipableAttribute( 0x1765, 0x1767 )]
	public class UncutCloth : Item, IScissorable, IDyable, ICommodity
	{
        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (dropped is UncutCloth && dropped.Hue == Hue && Stackable && dropped.Stackable)
            {
                Amount += dropped.Amount;
                dropped.Delete();

                if (playSound && from != null)
                {
                    int soundID = GetDropSound();

                    if (soundID == -1)
                        soundID = 0x42;

                    from.SendSound(soundID, GetWorldLocation());
                }
                return true;

            }
            else
                return false;
        }

		int ICommodity.DescriptionNumber { get { return LabelNumber; } }
		bool ICommodity.IsDeedable { get { return true; } }

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public UncutCloth() : this( 1 )
		{
		}

		[Constructable]
		public UncutCloth( int amount ) : base( 0x1767 )
		{
			Stackable = true;
			Amount = amount;
		}

		public UncutCloth( Serial serial ) : base( serial )
		{
		}

		public bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

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

		public override void OnSingleClick( Mobile from )
		{
			int number = (Amount == 1) ? 1049124 : 1049123;

			from.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, number, "", Amount.ToString() ) );
		}
		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( Deleted || !from.CanSee( this ) ) return false;

			base.ScissorHelper( from, new Bandage(), 1 );

			return true;
		}
	}
}