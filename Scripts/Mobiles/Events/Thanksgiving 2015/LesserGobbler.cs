using System;
using Server.Mobiles;
using Server.Spells;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a lesser gobbler corpse")]
    public class LesserGobbler : BaseCreature
    {
        [Constructable]
        public LesserGobbler(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a lesser gobbler";

            Body = 0xD0;
            Hue = 2635;

            BaseSoundID = 0x6E;

            SetStr(25);
            SetDex(100);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            AttackSpeed = 50;

            Fame = 150;
            Karma = 0;
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 100;

            UniqueCreatureDifficultyScalar = 2;
        }

        public override bool CanFly { get { return true; } }
        public override int GetDeathSound() { return 0x072; }

        public override bool OnBeforeDeath()
        {
            GreatGobbler.FeatherExplosion(Location, Map, 10);
            GreatGobbler.DamageCorpse(Location, Map, false);

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public LesserGobbler(Serial serial): base(serial)
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