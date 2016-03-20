using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "an ancient liche's corpse" )]
	public class AncientLich : BaseCreature
	{
		[Constructable]
		public AncientLich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "ancient lich" );
			Body = 78;
			BaseSoundID = 412;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(2000);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 125);
            SetSkill(SkillName.EvalInt, 125);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 23000;
			Karma = -23000;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        
		public override void OnDeath( Container c )
		{
    		base.OnDeath( c );
		}			

        public override int GetIdleSound(){return 0x19D;}
        public override int GetAngerSound(){return 0x175;}
        public override int GetDeathSound(){return 0x108;}
        public override int GetAttackSound(){return 0xE2;}
        public override int GetHurtSound(){return 0x28B;}

		public AncientLich( Serial serial ) : base( serial )
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
