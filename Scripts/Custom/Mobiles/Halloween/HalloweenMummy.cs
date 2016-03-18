using System;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("a tattered mummy corpse")]
    public class HalloweenMummy : BaseCreature
    {

        [Constructable]
        public HalloweenMummy() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a greater mummy";
            Body = 154;
            Hue = 2970;
            BaseSoundID = 0x1D7;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(1600);
            SetMana(2000);

            SetDamage(30, 45);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);
            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 100;

            Fame = 25000;
            Karma = -25000;

            PackItem(new Bandage(25));

        }

        public override int Meat { get { return 1; } }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.33;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .50;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .01;
                    else
                        effectChance = .50;
                }
            }

            SpecialAbilities.DiseaseSpecialAbility(effectChance, this, defender, 20, 60, -1, true, "", "You've been afflicted with incurable mummy rot!");
        }

        public HalloweenMummy(Serial serial) : base(serial)
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