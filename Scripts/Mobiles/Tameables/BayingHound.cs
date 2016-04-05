using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;


namespace Server.Mobiles
{
    [CorpseName("a baying hound corpse")]
    public class BayingHound : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public BayingHound(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a baying hound";
            Body = 739;
            Hue = 0;
            BaseSoundID = 0x575;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 80.0;

            Fame = 500;
            Karma = -500;
        }
        
        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;
        }

        public override int TamedItemId { get { return 8535; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 175; } }
        public override int TamedBaseMinDamage { get { return 7; } }
        public override int TamedBaseMaxDamage { get { return 9; } }
        public override double TamedBaseWrestling { get { return 80; } }
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

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .15;

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

            SpecialAbilities.StunSpecialAbility(effectChance, this, defender, .10, 10, -1, true, "", "The creature clings to you, making it difficult to use your weapon!");
        }

        public override int GetDeathSound()
        {
            return 0x386;
        }

        public BayingHound(Serial serial): base(serial)
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