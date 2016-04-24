using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an energy vortex corpse")]
    public class EnergyVortex : BaseCreature
    {
        [Constructable]
        public EnergyVortex(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an energy vortex";

            Body = 164;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(1000);

            SetDamage(28, 32);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 0;
            Karma = 0;            

            ControlSlots = 2;
        }

        public override void SetUniqueAI()
        {
            ReturnsHome = false;
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.None; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.Summoned; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Berserk; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override int PoisonResistance { get { return 5; } }

        public override bool DeleteCorpseOnDeath { get { return Summoned; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool IsHouseSummonable { get { return true; } }     

        public override int GetAngerSound() {return 0x15;}
        public override int GetAttackSound() {return 0x28;}

        public EnergyVortex(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}