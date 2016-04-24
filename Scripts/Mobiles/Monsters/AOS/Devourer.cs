using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a devourer of souls corpse")]
    public class Devourer : BaseCreature
    {
        [Constructable]
        public Devourer()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a devourer of souls";
            Body = 303;
            BaseSoundID = 357;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(1000);
            SetMana(2000);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

            Fame = 9500;
            Karma = -9500;
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.2;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.3, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!", "-1");
        }

        public Devourer(Serial serial): base(serial)
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