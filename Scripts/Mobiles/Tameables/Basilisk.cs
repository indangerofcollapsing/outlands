using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a basilisk corpse")]
    public class Basilisk : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public Basilisk(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a basilisk";
            Body = 794;
            Hue = 2574;
            BaseSoundID = 362;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(600);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;       

            Tamable = true;
            ControlSlots = 2;
            MinTameSkill = 100.1;

            Fame = 1000;
            Karma = -1000;
        }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 9753; } }
        public override int TamedItemHue { get { return 2574; } }
        public override int TamedItemXOffset { get { return -10; } }
        public override int TamedItemYOffset { get { return 10; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 18; } }
        public override int TamedBaseMaxDamage { get { return 20; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 150; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 150; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 15; } }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.1;
        }

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

            SpecialAbilities.PetrifySpecialAbility(effectChance, this, defender, 1.0, 5.0, -1, true, "", "You are petrified by their gaze!");           
        }

        public override int GetAngerSound() { return 0x2C0; }
        public override int GetAttackSound() { return 0x622; }
        public override int GetHurtSound() { return 0x625; }

        public Basilisk(Serial serial): base(serial)
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