using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;



namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class LichLord : BaseCreature
	{
		[Constructable]
		public LichLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lich lord";
			Body = 79;
			BaseSoundID = 412;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(800);
            SetMana(4000);

            SetDamage(15, 30);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 20000;
			Karma = -18000;
		}

        public override int PoisonResistance { get { return 5; } }
        public override bool CanRummageCorpses { get { return true; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );
		}

		public LichLord( Serial serial ) : base( serial )
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
