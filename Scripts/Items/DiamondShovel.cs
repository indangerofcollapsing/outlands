using System;
using Server;
using Server.Engines.Harvest;
using Server.Network;

namespace Server.Items
{
	public class DiamondShovel : BaseHarvestTool
	{		
		public override HarvestSystem HarvestSystem{ get{ return Mining.System; } }

		[Constructable]
		public DiamondShovel() : this( 2500 )
		{
		}

		[Constructable]
		public DiamondShovel( int uses ) : base( uses, 0xF39 )
		{
            Name = "a diamond shovel";

            Hue = 2500;
            
            Weight = 5.0;

            UsesRemaining = uses;
            ShowUsesRemaining = true;
		}

		public DiamondShovel( Serial serial ) : base( serial )
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "Durability: " + UsesRemaining.ToString()));
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "diamond shovel"));
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