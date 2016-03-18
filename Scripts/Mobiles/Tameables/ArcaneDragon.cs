using System;
using Server;
using Server.Items;
using Server.Mobiles.Vendors.NPC;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "an arcane dragon corpse" )]
	public class ArcaneDragon : BaseCreature
	{
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

		[Constructable]
		public ArcaneDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "an arcane dragon";
            Body = 59;			
            Hue = 2587;
			BaseSoundID = 362;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(900);
            SetMana(2000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 125);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 18000;
			Karma = -18000;			

			Tamable = true;
			ControlSlots = 4;
			MinTameSkill = 105.1;
		}

        public override int Meat { get { return 10; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 9780; } }
        public override int TamedItemHue { get { return 295; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return -15; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 24; } }
        public override int TamedBaseMaxDamage { get { return 26; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 75; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 100; } }
        public override int TamedBaseMaxMana { get { return 1500; } }
        public override double TamedBaseMagicResist { get { return 150; } }
        public override double TamedBaseMagery { get { return 75; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 150; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;            
        }

        public override void SetTamedAI()
        {
            SetSubGroup(AISubgroup.MeleeMage3);
            UpdateAI(false);

            DictCombatSpell[CombatSpell.SpellDamage4] += 2;
            DictCombatSpell[CombatSpell.SpellDamage5] += 2;
            DictCombatSpell[CombatSpell.SpellDamage6] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage7] -= 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;     
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // IPY ACHIEVEMENT TRIGGER 
            if (!IsBonded)
            {
                AwardAchievementForKiller(AchievementTriggers.Trigger_DragonKilled);
                AwardDailyAchievementForKiller(PvECategory.KillDragons);
            }
            // END IPY ACHIEVEMENT TRIGGER
        }

        
        
		public ArcaneDragon( Serial serial ) : base( serial )
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