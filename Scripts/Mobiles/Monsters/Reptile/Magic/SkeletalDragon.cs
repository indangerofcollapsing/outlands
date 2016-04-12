using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a skeletal dragon corpse")]
    public class SkeletalDragon : BaseCreature
    {
        [Constructable]
        public SkeletalDragon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a skeletal dragon";
            Body = 104;
            BaseSoundID = 0x488;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(1800);
            SetMana(2000);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 22500;
            Karma = -22500;
        }

        public override int PoisonResistance { get { return 5; } }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public SkeletalDragon(Serial serial): base(serial)
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
