using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a chromatic dragon corpse")]
    public class ChromaticDragon : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public ChromaticDragon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a chromatic dragon";
            Body = 12;
            Hue = 2500;
            BaseSoundID = 362;  

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(2000);
            SetMana(1000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;

            Tameable = true;
            ControlSlots = 4;
            MinTameSkill = 110.1;
        }

        public override bool CanFly { get { return true; } }

        public override int TamedItemId { get { return 9780; } }
        public override int TamedItemHue { get { return 2500; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return -10; } }

        public override int TamedBaseMaxHits { get { return 500; } }
        public override int TamedBaseMinDamage { get { return 26; } }
        public override int TamedBaseMaxDamage { get { return 28; } }
        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseEvalInt { get { return 25; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 50; } }
        public override int TamedBaseMaxMana { get { return 500; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 25; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 125; } }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.2;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;            
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage1;
            UpdateAI(false);

            DictCombatSpell[CombatSpell.SpellDamage1] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage2] -= 2;
            DictCombatSpell[CombatSpell.SpellDamage3] += 2;
            DictCombatSpell[CombatSpell.SpellDamage4] += 2;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;   
        }

        public override bool OnBeforeHarmfulSpell()
        {
            double effectChance = .15;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                    effectChance = .15;
            }

            if (Utility.RandomDouble() <= effectChance)
                MagicDamageAbsorb = 1;            

            return true;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }      

        public ChromaticDragon(Serial serial): base(serial)
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
