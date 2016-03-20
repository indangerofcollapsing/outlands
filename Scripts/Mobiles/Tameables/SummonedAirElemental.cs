using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an air elemental corpse")]
    public class SummonedAirElemental : BaseCreature
    {
        [Constructable]
        public SummonedAirElemental() : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an air elemental";
            Body = 13;
            Hue = 0x4001;
            BaseSoundID = 655;

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(400);
            SetMana(2000);

            SetDamage(14, 16);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Meditation, 100);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Magery, 50);            

            VirtualArmor = 50;

            ControlSlots = 2;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public SummonedAirElemental(Serial serial): base(serial)
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

            if (BaseSoundID == 263)
                BaseSoundID = 655;
        }
    }
}