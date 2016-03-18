using System;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a hare corpse")]
    public class Rabbit : BaseCreature
    {
        public override double MaxSkillScrollWorth { get { return 0.0; } }
        public override bool DropsGold { get { return false; } }
        [Constructable]
        public Rabbit(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a rabbit";
            Body = 205;

            if (0.5 >= Utility.RandomDouble())
                Hue = Utility.RandomAnimalHue();

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 10);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 150;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8485; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 20; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 50; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
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

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 1; } }
        
        public Rabbit(Serial serial) : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            if (this.ControlMaster != null)
                AwardAchievementForKiller(AchievementTriggers.Trigger_KillTamedRabbDogCatBird);
            else
                AwardAchievementForKiller(AchievementTriggers.Trigger_KillRabbitDogCat);
        }

        public override int GetAttackSound() { return 0xC9; }
        public override int GetHurtSound() { return 0xCA; }
        public override int GetDeathSound() { return 0xCB; }

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