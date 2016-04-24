using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a bird corpse")]
    public class Bird : BaseCreature
    {
        [Constructable]
        public Bird(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            if (Utility.RandomBool())
            {
                Hue = 0x901;

                switch (Utility.Random(3))
                {
                    case 0: Name = "a crow"; break;
                    case 2: Name = "a raven"; break;
                    case 1: Name = "a magpie"; break;
                }
            }

            else
            {
                Hue = Utility.RandomBirdHue();
                Name = NameList.RandomName("bird");
            }

            Body = 6;
            BaseSoundID = 0x1B;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1;
            Karma = 0;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }
        
        public override int TamedItemId { get { return 8430; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 50; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Beastial; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Fast; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.NeutralMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override bool HasFeathers { get { return true; } }
        public override bool CanFly { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public Bird(Serial serial): base(serial)
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