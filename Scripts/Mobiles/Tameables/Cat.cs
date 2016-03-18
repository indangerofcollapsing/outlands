using System;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a cat corpse")]
    [TypeAlias("Server.Mobiles.Housecat")]
    public class Cat : BaseCreature
    {
        public override bool DropsGold { get { return false; } }
        public override double MaxSkillScrollWorth { get { return 0.0; } }

        [Constructable]
        public Cat(): base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "a cat";
            Body = 0xC9;
            Hue = Utility.RandomAnimalHue();
            BaseSoundID = 0x69;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(50);

            SetDamage(3, 6);

            SetSkill(SkillName.Wrestling, 25);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 200;
            Karma = 0;            

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 25;
        }

        public override int Meat { get { return 1; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8475; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
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
        
        public Cat(Serial serial): base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            if (this.ControlMaster != null)
                AwardAchievementForKiller(AchievementTriggers.Trigger_KillTamedRabbDogCatBird);
            else
                AwardAchievementForKiller(AchievementTriggers.Trigger_KillRabbitDogCat);
            base.OnDeath(c);
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