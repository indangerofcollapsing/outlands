using System;
using Server;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a beholder corpse")]
    public class Beholder : BaseCreature
    {
        [Constructable]
        public Beholder(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a beholder";
            Body = 22;
            BaseSoundID = 377;
            Hue = 2117;

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(100);
            SetMana(1000);

            SetDamage(4, 6);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);  

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 25;
        }

        public override void SetUniqueAI()
        {   
            DictCombatSpell[CombatSpell.SpellDamage5] = 8;

            CombatSpecialActionMinDelay = 5;
            CombatSpecialActionMaxDelay = 10;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.CauseWounds] = 1;

            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }
       
        public Beholder(Serial serial): base(serial)
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
