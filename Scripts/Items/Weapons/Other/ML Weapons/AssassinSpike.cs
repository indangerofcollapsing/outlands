using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2D21, 0x2D2D )]
	public class AssassinSpike : BaseKnife
	{
		public override int BaseMissSound{ get{ return 0x239; } }
		public override SkillName BaseSkill { get { return SkillName.Fencing; } }

		[Constructable]
		public AssassinSpike() : base( 0x2D21 )
		{
			Weight = 4.0;
		}

		public AssassinSpike( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}