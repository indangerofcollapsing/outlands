using System;
using Server;
using Server.Guilds;
using Server.Gumps;

namespace Server.Items
{
	public class BalanceShield : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 1; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 0; } }
		public override int BasePoisonResistance{ get{ return 0; } }
		public override int BaseEnergyResistance{ get{ return 0; } }

		public override int InitMinHits{ get{ return 100; } }
		public override int InitMaxHits{ get{ return 125; } }

		public override int AosStrReq{ get{ return 95; } }

		public override int ArmorBase{ get{ return 32; } }


        public override string DefaultName
        {
            get
            {
                return "Balance Shield";
            }
        }
		[Constructable]
		public BalanceShield() : base( 7108 )
		{
			if ( !Core.AOS )
				LootType = LootType.Newbied;

			Weight = 5.0;
            Hue = 2214;
		}

        public BalanceShield(Serial serial)
            : base(serial)
		{
		}
        
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}

		public override bool OnEquip( Mobile from )
		{
			return Validate( from ) && base.OnEquip( from );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( Validate( Parent as Mobile ) )
				base.OnSingleClick( from );
		}

        public virtual bool Validate(Mobile m)
        {
            return true;
        }
    }
}