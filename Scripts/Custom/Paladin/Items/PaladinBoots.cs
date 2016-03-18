using System;

namespace Server.Items
{
	[FlipableAttribute( 0x170b, 0x170c )]
	public class PaladinBoots : BaseShoes
	{
        public override int PlayerClassCurrencyValue { get { return 2500; } }

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public PaladinBoots() : this( 0 )
		{
		}

		[Constructable]
		public PaladinBoots( int hue ) : base( 0x170B, hue )
		{
			Weight = 3.0;
            Name = "Paladin Boots";            
        
            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;
 
            LootType = Server.LootType.Blessed;
		}

        public PaladinBoots(Serial serial): base(serial)
		{
		}

        public override bool Dye(Mobile from, DyeTub sender)
        {
            return false;
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
	}
}
