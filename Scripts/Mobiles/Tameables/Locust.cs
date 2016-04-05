using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a locust corpse")]
    public class Locust : BaseCreature
    {
        [Constructable]
        public Locust(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a locust";
            Body = 738;
            Hue = 0;
            BaseSoundID = 0x3BF;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(100);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;             

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 50.0;

            Fame = 500;
            Karma = -500;
        }

        public override bool IsHighSeasBodyType { get { return true; } }
        public override bool HasAlternateHighSeasHurtAnimation { get { return true; } }

        public override int TamedItemId { get { return 8534; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 125; } }
        public override int TamedBaseMinDamage { get { return 5; } }
        public override int TamedBaseMaxDamage { get { return 7; } }
        public override double TamedBaseWrestling { get { return 60; } }
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
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .10;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .01;
                    else
                        effectChance = .25;
                }
            }

            SpecialAbilities.StunSpecialAbility(effectChance, this, defender, .10, 10, -1, true, "", "The creature buzzes about, making it difficult to swing your weapon!");
        }

        public override int GetDeathSound() { return 0x386;}

        public Locust(Serial serial): base(serial)
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