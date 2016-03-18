using System;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Regions;

namespace Server.Items
{
	[FlipableAttribute( 0x1f14, 0x1f15, 0x1f16, 0x1f17 )]
	public class BoatRune : Item
	{
        public BaseBoat m_Boat;

        [Constructable]
		public BoatRune(BaseBoat boat, Mobile owner) : base( 0x1F14 )
		{
			Weight = 1.0;
            Hue = 88;

            m_Boat = boat;            
		}

        public BoatRune(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            if (m_Boat != null)
            {
                if (!m_Boat.Deleted && m_Boat.Owner != null)
                    Name = "a boat rune [Bound to " + m_Boat.Owner.RawName + "]";
            }

            else
                Name = "a boat rune";

 	        base.OnSingleClick(from);
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int) 0); //version

            writer.Write(m_Boat);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            m_Boat = (BaseBoat)reader.ReadItem();
		}
	}
}