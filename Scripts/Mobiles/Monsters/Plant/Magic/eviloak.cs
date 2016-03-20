using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an evil oak corpse" )]
	public class EvilOak : BaseCreature
	{
		[Constructable]
		public EvilOak() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an evil oak";
			Body = 47;
			BaseSoundID = 442;
			Hue = 0x8B0;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(5000);
            SetMana(5000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 115500;
			Karma = -115500;
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }   
        public override bool DisallowAllMoves { get { return true; } }		

		public EvilOak( Serial serial ) : base( serial )
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