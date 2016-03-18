using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a reapers corpse" )]
	public class Reaper : BaseCreature
	{
		[Constructable]
		public Reaper() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a reaper";
			Body = 47;
			BaseSoundID = 442;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(300);
            SetMana(2000);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

			Fame = 3500;
			Karma = -3500;

			PackItem( new Log( 10 ) );
			PackItem( new MandrakeRoot( 5 ) );
            PackItem( new Engines.Plants.Seed() );
      
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool DisallowAllMoves { get { return true; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(new RangerArms()); } break;
                case 1: { c.AddItem(new RangerChest()); } break;
                case 2: { c.AddItem(new RangerGloves()); } break;
                case 3: { c.AddItem(new RangerGorget()); } break;
                case 4: { c.AddItem(new RangerLegs()); } break;
            }
        }

		public override bool OnBeforeDeath()
		{
			PackItem( new Log( 10 ) );			

			return base.OnBeforeDeath();
		}
		
		public Reaper( Serial serial ) : base( serial )
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
