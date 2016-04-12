using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a daemon corpse")]
    public class SummonedDaemon : BaseCreature
    {
        [Constructable]
        public SummonedDaemon(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("daemon");
            Body = 9;
            BaseSoundID = 357;

            SetStr(100);
            SetDex(50);
            SetInt(50);

            SetHits(500);
            SetMana(1000);

            SetDamage(20, 22);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 25);
            SetSkill(SkillName.EvalInt, 25);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 50;
            ControlSlots = 2;
        }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage1;
            UpdateAI(false);
        }

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.Summoned; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.MeleeMage1; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override bool CanFly { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public SummonedDaemon(Serial serial) : base(serial)
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