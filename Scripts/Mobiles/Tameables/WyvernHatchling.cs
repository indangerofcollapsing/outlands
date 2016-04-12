using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wyvern hatchling corpse")]
    public class WyvernHatchling : BaseCreature
    {
        [Constructable]
        public WyvernHatchling(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wyvern hatchling";
            Body = 733;
            Hue = 0;
            BaseSoundID = 0x646;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            VirtualArmor = 25;    

            SetSkill(SkillName.Poisoning, 15);

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 80.0;

            Fame = 1000;
            Karma = -1000;
        }

        public override int TamedItemId { get { return 8529; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 10; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 6; } }
        public override int TamedBaseMaxDamage { get { return 8; } }
        public override double TamedBaseWrestling { get { return 75; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 25; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override int PoisonResistance { get { return 4; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetDeathSound() { return 0x2CD;}

        public WyvernHatchling(Serial serial): base(serial)
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