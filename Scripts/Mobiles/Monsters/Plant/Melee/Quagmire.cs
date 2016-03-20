using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a quagmire corpse" )]
	public class Quagmire : BaseCreature
	{
		[Constructable]
		public Quagmire() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8 )
		{
			Name = "a quagmire";
			Body = 789;
			BaseSoundID = 352;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 32;

			Fame = 1500;
			Karma = -1500;			
		}

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }        
		
		public override int GetAngerSound()
		{
			return 353;
		}		

		public Quagmire( Serial serial ) : base( serial )
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
