using System;
using Server;
using Server.Engines.Harvest;
using Server.Network;

namespace Server.Items
{
	public class DiamondPickaxe : BaseAxe, IUsesRemaining
	{		
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

        public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 2; } }
        public override int BaseSpeed { get { return 40; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

		public override WeaponAnimation BaseAnimation{ get{ return WeaponAnimation.Slash1H; } }

		[Constructable]
		public DiamondPickaxe() : this( 2500 )
		{
		}

		[Constructable]
		public DiamondPickaxe( int uses ) : base( 0xE86 )
		{
            Name = "a diamond pickaxe";

            Hue = 2500;

			Weight = 11.0;
			
			UsesRemaining = uses;
			ShowUsesRemaining = true;
		}

		public DiamondPickaxe( Serial serial ) : base( serial )
		{
		}

        public override void OnSingleClick(Mobile from)
        {           
            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "Durability: " + UsesRemaining.ToString()));
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "diamond pickaxe"));
            }
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