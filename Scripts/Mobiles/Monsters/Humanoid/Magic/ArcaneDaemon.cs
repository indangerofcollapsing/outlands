using System;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("an arcane daemon corpse")]
    public class ArcaneDaemon : BaseCreature
    {
        [Constructable]
        public ArcaneDaemon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an arcane daemon";
            Body = 0x310;
            BaseSoundID = 0x47D;

            SetStr(75);
            SetDex(50);
            SetInt(75);

            SetHits(400);
            SetMana(1000);

            SetDamage(12, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 7000;
            Karma = -10000;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public ArcaneDaemon(Serial serial)
            : base(serial)
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
