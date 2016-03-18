using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "an orcish corpse" )]
	public class OrcishLord : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public OrcishLord() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an orcish lord";
			Body = 138;
			BaseSoundID = 0x45A;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 2500;
			Karma = -2500;

			switch ( Utility.Random( 5 ) )
			{
				case 0: PackItem( new OrcHelm() );  break;
				case 1: PackItem( new MortarPestle() ); break;
				case 2: PackItem( new Bottle() ); break;
				case 3: PackItem( new RawRibs() ); break;
				case 4: PackItem( new Shovel() ); break;
			}

			PackItem( new RingmailChest() );
		}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            AwardDailyAchievementForKiller(NewbCategory.KillOrcs);
            AwardAchievementForKiller(AchievementTriggers.Trigger_OrcKilled);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new CaveMoss());
        }	
		
		public override bool CanRummageCorpses{ get{ return true; } }
		public override int Meat{ get{ return 1; } }

		public OrcishLord( Serial serial ) : base( serial )
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
