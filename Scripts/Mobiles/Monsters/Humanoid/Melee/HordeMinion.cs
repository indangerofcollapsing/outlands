using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a horde minion corpse" )]
	public class HordeMinion : BaseCreature
	{
		[Constructable]
		public HordeMinion () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a horde minion";
			Body = 776;
			BaseSoundID = 357;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 500;
			Karma = -500;

			AddItem( new LightSource() );		
		}

		public override int GetIdleSound(){return 338;}
		public override int GetAngerSound(){return 338;}
		public override int GetDeathSound(){return 338;}
		public override int GetAttackSound(){return 406;}
		public override int GetHurtSound(){return 194;}

		public HordeMinion( Serial serial ) : base( serial )
		{
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