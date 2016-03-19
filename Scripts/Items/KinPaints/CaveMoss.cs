using System;

namespace Server.Items
{
	public class CaveMoss : Item
	{
		[Constructable]
        public CaveMoss(): base(0xDD0)
		{
            Name = "cave moss";
            Hue = 2503;
            Weight = 1;
		}

        public CaveMoss(Serial serial): base(serial)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("You estimate that when properly cooked, this could be fashioned into a paint of some kind.");
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
}