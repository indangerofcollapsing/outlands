using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName("a chaos dragoon elite corpse")]
	public class ChaosDragoonElite : BaseCreature
	{
		[Constructable]
		public ChaosDragoonElite()
			: base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.15, 0.4)
		{
			Name = "a chaos dragoon elite";
			Body = 0x190;
			Hue = Utility.RandomSkinHue();

			SetStr(276, 350);
			SetDex(66, 90);
			SetInt(126, 150);

			SetHits(276, 350);

			SetDamage(29, 34);

			SetDamageType(ResistanceType.Physical, 100);

			/*SetResistance(ResistanceType.Physical, 45, 55);
			SetResistance(ResistanceType.Fire, 15, 25);
			SetResistance(ResistanceType.Cold, 50);
			SetResistance(ResistanceType.Poison, 25, 35);
			SetResistance(ResistanceType.Energy, 25, 35);*/


			SetSkill(SkillName.Tactics, 80.1, 100.0);
			SetSkill(SkillName.MagicResist, 100.1, 110.0);
			SetSkill(SkillName.Anatomy, 80.1, 100.0);
			SetSkill(SkillName.Magery, 85.1, 100.0);
			SetSkill(SkillName.EvalInt, 85.1, 100.0);
			SetSkill(SkillName.Swords, 72.5, 95.0);
			SetSkill(SkillName.Fencing, 85.1, 100);
			SetSkill(SkillName.Macing, 85.1, 100);

			Fame = 8000;
			Karma = -8000;
            
			BaseWeapon melee = null;

			switch (Utility.Random(3))
			{
				case 0: melee = new Kryss(); break;
				case 1: melee = new Broadsword(); break;
				case 2: melee = new Katana(); break;
			}

			melee.Movable = false;
			AddItem(melee);
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
		
		public override bool AutoDispel { get { return true; } }
		public override bool BardImmune { get { return !Core.AOS; } }
		public override bool CanRummageCorpses { get { return true; } }
		public override bool AlwaysMurderer { get { return true; } }
		public override bool ShowFameTitle { get { return false; } }

		public override bool OnBeforeDeath()
		{
			IMount mount = this.Mount;

			if ( mount != null )
			{
				if ( mount is SwampDragon )
					((SwampDragon)mount).HasBarding = false;

				mount.Rider = null;
			}

			return base.OnBeforeDeath();
		}

		public override void AlterMeleeDamageTo(Mobile to, ref int damage)
		{
			if ( to is Dragon || to is WhiteWyrm || to is SwampDragon || to is Drake || to is Nightmare || to is Hiryu || to is LesserHiryu || to is Daemon )
				damage *= 3;
		}

		public ChaosDragoonElite(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
