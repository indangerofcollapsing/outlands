using System;
using Server;
using Server.Items;
using System.Collections;

namespace Server.Mobiles
{
    [CorpseName("an energy vortex corpse")]
    public class EnergyVortex : BaseCreature
    {
        public override bool DeleteCorpseOnDeath { get { return Summoned; } }
        public override bool AlwaysMurderer { get { return true; } }

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

            VirtualArmor = 50;

            Fame = 0;
            Karma = 0;            

            ControlSlots = 2;
        }

        public override void SetUniqueAI()
        {
            ReturnsHome = false;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override int GetAngerSound() {  return 0x15;}
        public override int GetAttackSound() {   return 0x28;   }

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