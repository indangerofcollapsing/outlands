using System;

namespace Server.Items
{
	public class Bloodroot : Item
	{
		[Constructable]
        public Bloodroot(): base(0xD39)
		{
            Name = "bloodroot";
            Hue = 2117;
            Weight = 1;
		}

		public Bloodroot( Serial serial ) : base( serial )
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