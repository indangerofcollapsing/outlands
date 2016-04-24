using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a fire elemental corpse")]
    public class SummonedFireElemental : BaseCreature
    {
        [Constructable]
        public SummonedFireElemental(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a fire elemental";
            Body = 15;
            BaseSoundID = 838;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(400);
            SetMana(2000);

            SetDamage(14, 16);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);           
            SetSkill(SkillName.MagicResist, 125);

            VirtualArmor = 25;

            ControlSlots = 2;

            AddItem(new LightSource());
        }

        public override void SetUniqueAI()
        {
            CastOnlyFireSpells = true;
        }

        public override void SetTamedAI()
        {
            AISubGroup = AISubGroupType.MeleeMage3;
            UpdateAI(false);
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Elemental; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.Summoned; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.MeleeMage3; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public SummonedFireElemental(Serial serial): base(serial)
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
