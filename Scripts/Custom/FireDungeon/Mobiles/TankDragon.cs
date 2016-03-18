using System;
using Server;
using Server.Items;
using Server.Achievements;

using Server.Custom;

namespace Server.Mobiles
{
    [CorpseName("a tank dragon corpse")]
    public class TankDragon : BaseCreature
    {
        [Constructable]
        public TankDragon(double difficultyMultiplier = 1)
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a tank dragon";
            Body = Utility.RandomList(12, 59);
            BaseSoundID = 362;

            

            SetStr(100);
            SetDex(50);
            SetInt(100);

            SetHits(750);
            SetMana(1000);

            SetHits((int)Math.Ceiling(Hits * difficultyMultiplier));

            SetDamage((int)Math.Ceiling(16.0 * difficultyMultiplier), (int)Math.Ceiling(22.0 * difficultyMultiplier));

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 70;

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 95;
        }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 9780; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return -10; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 600; } }
        public override int TamedBaseMinDamage { get { return 32; } }
        public override int TamedBaseMaxDamage { get { return 35; } }
        public override double TamedBaseWrestling { get { return 120; } }
        public override double TamedBaseEvalInt { get { return 37; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 50; } }
        public override int TamedBaseMaxMana { get { return 750; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 37; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 150; } }
        public override int TamedBaseVirtualArmor { get { return 37; } }        

        public override void OnDeath(Container c)
        {
            FireDungeon.RespawnTankDragon(Map);
            base.OnDeath(c);
        }

        public override bool ReacquireOnMovement { get { return !Controlled; } }        
        //public override int TreasureMapLevel { get { return 3; } }
        public override int Meat { get { return 5; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        //public override int Scales{ get{ return 7; } }
        //public override ScaleType ScaleType{ get{ return ( Body == 12 ? ScaleType.Yellow : ScaleType.Red ); } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }
        public override bool CanAngerOnTame { get { return true; } }
        public override bool CanFly { get { return true; } }

        public TankDragon(Serial serial)
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