using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "an elder air elemental corpse" )]
	public class ElderAirElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public ElderAirElemental () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an elder air elemental";
			Body = 13;
			Hue = 2598;
			BaseSoundID = 655;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(500);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 4500;
			Karma = -4500;
			
			ControlSlots = 3;
			CanSwim = true;

			PackItem( new BlackPearl( 3 ) );
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.075;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < .2)
                {
                    double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax)) * 2;
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes gust of air*");

                    Effects.PlaySound(this.Location, this.Map, 0x64C);

                    Animate(12, 6, 1, true, false, 0);

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, defender, damage, 8, -1, "", "The creature knocks you back with a gust of air!");
                }
            }
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // IPY ACHIEVEMENT TRIGGER 
            AwardAchievementForKiller(AchievementTriggers.Trigger_AirElementalKilled);
            // END IPY ACHIEVEMENT TRIGGER

            switch (Utility.Random(250))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonAirElementalScroll())); } break;
            }
        }	
        
		public ElderAirElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 655;
		}
	}
}
