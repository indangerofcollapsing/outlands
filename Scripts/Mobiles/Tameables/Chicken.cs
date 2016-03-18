using System;
using Server.Mobiles;
using Server.Spells;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a chicken corpse")]
    public class Chicken : BaseCreature
    {
        public override double MaxSkillScrollWorth { get { return 0.0; } }
        public override bool DropsGold { get { return false; } }

        [Constructable]
        public Chicken(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a chicken";
            Body = 0xD0;
            BaseSoundID = 0x6E;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(25);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 20);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 150;
            Karma = 0;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 0.0;
        }

        public override int MaxExperience { get { return 100; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8401; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

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

        public override int Meat { get { return 2; } }
        public override MeatType MeatType { get { return MeatType.ChickenLeg; } }
        public override FoodType FavoriteFood { get { return FoodType.GrainsAndHay; } }
        public override bool CanFly { get { return true; } }

        public override int Feathers { get { return 50; } }

        public override bool OnBeforeDeath()
        {
            if (Utility.RandomDouble() < 0.003)
                PackItem(SpellScroll.MakeMaster(new PolymorphScroll()));

            return base.OnBeforeDeath();
        }

        public override int GetDeathSound() { return 0x072; }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            // IPY ACHIEVEMENT
            AwardAchievementForKiller(AchievementTriggers.Trigger_ChickenKilled_TIMED);
            // IPY ACHIEVEMENT
        }

        public Chicken(Serial serial)
            : base(serial)
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