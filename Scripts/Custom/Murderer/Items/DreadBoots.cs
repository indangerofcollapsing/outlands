using System;

namespace Server.Items
{
	[FlipableAttribute( 0x170b, 0x170c )]
	public class DreadBoots : BaseShoes
	{
        public override int PlayerClassCurrencyValue { get { return 2500; } }

		public override CraftResource DefaultResource{ get{ return CraftResource.RegularLeather; } }

		[Constructable]
		public DreadBoots() : this( 0 )
		{
		}

		[Constructable]
		public DreadBoots( int hue ) : base( 0x170B, hue )
		{
			Weight = 3.0;
            Name = "Dread Boots";            
        
            PlayerClass = PlayerClass.Murderer;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.MurdererItemHue;
 
            LootType = Server.LootType.Blessed;
		}

        public DreadBoots(Serial serial): base(serial)
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
