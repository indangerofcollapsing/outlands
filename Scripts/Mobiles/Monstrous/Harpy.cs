using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a harpy corpse")]
    public class Harpy : BaseCreature
    {
        [Constructable]
        public Harpy(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a harpy";
            Body = 30;
            BaseSoundID = 402;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;            

            Fame = 2500;
            Karma = -2500;
        }

        public override void SetUniqueAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Monstrous; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override bool CanFly { get { return true; } }

        public override bool HasFeathers { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetAttackSound() { return 916; }
        public override int GetAngerSound() { return 916; }
        public override int GetDeathSound() { return 917; }
        public override int GetHurtSound() { return 919; }
        public override int GetIdleSound() { return 918; }     

        public Harpy(Serial serial): base(serial)
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
