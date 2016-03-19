using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public class CompanionRobe : BaseSuit
	{
		[Constructable]
		public CompanionRobe() : base( AccessLevel.Counselor, 2125, 0x204F )
		{
            this.LootType = LootType.Blessed;
		}

        public CompanionRobe(Serial serial) : base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            if (this.Name != null)
            {
                from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, 0, 3, "", this.Name));
            }
            else
            {
                from.Send(new AsciiMessage(Serial, ItemID, MessageType.Label, 0, 3, "", "a GM robe"));
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