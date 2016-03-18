using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an imp corpse")]
    public class Imp : BaseCreature
    {
        [Constructable]
        public Imp(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an imp";
            Body = 74;
            BaseSoundID = 422;
            Hue = 2118;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(75);
            SetMana(1000);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;            

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 75;
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 6; } }
        public override HideType HideType { get { return HideType.Spined; } }
        public override bool CanFly { get { return true; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 9631; } }
        public override int TamedItemHue { get { return 2118; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 70; } }
        public override double TamedBaseEvalInt { get { return 50; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 75; } }
        public override int TamedBaseMaxMana { get { return 1000; } }
        public override double TamedBaseMagicResist { get { return 100; } }
        public override double TamedBaseMagery { get { return 50; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 125; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetTamedAI()
        {
            SetSubGroup(AISubgroup.Mage2);
            UpdateAI(false);

            DictCombatRange[CombatRange.Withdraw] = 0;

            DictCombatSpell[CombatSpell.SpellDamage3] += 1;
            DictCombatSpell[CombatSpell.SpellDamage4] += 1;
            DictCombatSpell[CombatSpell.SpellDamage5] -= 1;
            DictCombatSpell[CombatSpell.SpellDamage6] -= 1;

            SpellDelayMin *= 1.33;
            SpellDelayMax *= 1.33;
        }

        public Imp(Serial serial): base(serial)
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
