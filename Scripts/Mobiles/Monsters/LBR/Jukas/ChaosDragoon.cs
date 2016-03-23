using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a chaos dragoon corpse" )]
	public class ChaosDragoon : BaseCreature
	{
		[Constructable]
		public ChaosDragoon() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.15, 0.4 )
		{
			Name = "a chaos dragoon";
			Body = 0x190;
			Hue = Utility.RandomSkinHue();

			SetStr( 176, 225 );
			SetDex( 81, 95 );
			SetInt( 61, 85 );

			SetHits( 176, 225 );

			SetDamage( 24, 26 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Cold, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			//SetResistance( ResistanceType.Physical, 25, 38 );
			//SetResistance( ResistanceType.Fire, 25, 38 );
			//SetResistance( ResistanceType.Cold, 25, 38 );
			//SetResistance( ResistanceType.Poison, 25, 38 );
			//SetResistance( ResistanceType.Energy, 25, 38 );

			SetSkill( SkillName.Fencing, 77.6, 92.5 );
			SetSkill( SkillName.Healing, 60.3, 90.0 );
			SetSkill( SkillName.Macing, 77.6, 92.5 );
			SetSkill( SkillName.Anatomy, 77.6, 87.5 );
			SetSkill( SkillName.MagicResist, 77.6, 97.5 );
			SetSkill( SkillName.Swords, 77.6, 92.5 );
			SetSkill( SkillName.Tactics, 77.6, 87.5 );

			Fame = 5000;
			Karma = -5000;
		}

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

		public override int GetIdleSound()
		{
			return 0x2CE;
		}

		public override int GetDeathSound()
		{
			return 0x2CC;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override int GetAttackSound()
		{
			return 0x2C8;
		}
		
		public override bool AutoDispel{ get{ return true; } }
		public override bool BardImmune{ get{ return !Core.AOS; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool ShowFameTitle{ get{ return false; } }

		public override bool OnBeforeDeath()
		{
			IMount mount = this.Mount;

			if ( mount != null )
				mount.Rider = null;

			return base.OnBeforeDeath();
		}

		public override void AlterMeleeDamageTo( Mobile to, ref int damage )
		{
			if ( to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Hiryu || to is LesserHiryu || to is Daemon )
				damage *= 3;
		}

		public ChaosDragoon( Serial serial ) : base( serial )
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
