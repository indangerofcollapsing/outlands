using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a mongbat corpse" )]
	public class Mongbat : BaseCreature
	{
		[Constructable]
		public Mongbat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mongbat";
			Body = 39;
			BaseSoundID = 422;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;            

			Fame = 150;
			Karma = -150;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }

        public override int Meat { get { return 1; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8441; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 15; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 50; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

		public override void OnDeath( Container c )
		{
			base.OnDeath(c);

			// IPY ACHIEVEMENT 
			AwardAchievementForKiller(AchievementTriggers.Trigger_KillMongbat_TIMED);
			// IPY ACHIEVEMENT 

			switch (Utility.Random(1000))
			{
				case 1: { c.AddItem(new Diamond()); } break;
				case 2: { c.AddItem(new Ruby()); } break;
				case 3: { c.AddItem(new HealPotion()); } break;
				case 4: { c.AddItem(new HealScroll()); } break;
			}
		}		
		
		public Mongbat( Serial serial ) : base( serial )
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
