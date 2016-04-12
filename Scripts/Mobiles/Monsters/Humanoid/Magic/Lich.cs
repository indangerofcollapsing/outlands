using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;


namespace Server.Mobiles
{
	[CorpseName( "a liche's corpse" )]
	public class Lich : BaseCreature
	{
		[Constructable]
		public Lich() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lich";
			Body = 24;
			BaseSoundID = 0x3E9;

            SetStr(50);
            SetDex(75);
            SetInt(100);

            SetHits(400);
            SetMana(1000);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 8000;
			Karma = -8000;
		}
        
        public override int PoisonResistance { get { return 5; } }
        public override bool CanRummageCorpses { get { return true; } }

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);
		}

		public Lich( Serial serial ) : base( serial )
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
