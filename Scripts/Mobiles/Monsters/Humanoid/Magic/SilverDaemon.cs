using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a silver daemon corpse" )]
	public class SilverDaemon : BaseCreature
	{
		[Constructable]
		public SilverDaemon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a silver daemon";
			Body = 9;
			BaseSoundID = 357;
			Hue = 0x835;

            SetStr(100);
            SetDex(75);
            SetInt(100);

            SetHits(2500);
            SetMana(2000);

            SetDamage(20, 40);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Poisoning, 25);

            VirtualArmor = 25;            

			Fame = 50000;
			Karma = -50000;
		}
        
        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int PoisonResistance { get { return 4; } }

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

		public SilverDaemon( Serial serial ) : base( serial )
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