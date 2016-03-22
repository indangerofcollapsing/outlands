using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x26BD, 0x26C7 )]
	public class BladedStaff : BaseSpear
	{
		public override SkillName BaseSkill{ get{ return SkillName.Swords; } }

		[Constructable]
		public BladedStaff() : base( 0x26BD )
		{
			Weight = 4.0;
		}

		public BladedStaff( Serial serial ) : base( serial )
		{
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