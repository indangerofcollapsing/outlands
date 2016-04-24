using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a wyvern corpse")]
    public class Wyvern : BaseCreature
    {
        [Constructable]
        public Wyvern(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a wyvern";
            Body = 62;
            BaseSoundID = 362;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(250);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 50;

            Fame = 4000;
            Karma = -4000;

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 90;
        }

        public override int TamedItemId { get { return 9684; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return -37; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 200; } }
        public override int TamedBaseMinDamage { get { return 12; } }
        public override int TamedBaseMaxDamage { get { return 14; } }
        public override double TamedBaseWrestling { get { return 80; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 50; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Medium; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override int PoisonResistance { get { return 4; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetAttackSound(){ return 713;}
        public override int GetAngerSound() { return 718;}
        public override int GetDeathSound() { return 716; }
        public override int GetHurtSound()  {return 721; }
        public override int GetIdleSound(){return 725;}

        public Wyvern(Serial serial): base(serial)
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
