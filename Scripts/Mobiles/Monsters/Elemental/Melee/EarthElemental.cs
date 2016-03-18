using System;
using System.Collections;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("an earth elemental corpse")]
    public class EarthElemental : BaseCreature
    {
        public override double DispelDifficulty { get { return 117.5; } }
        public override double DispelFocus { get { return 45.0; } }

        [Constructable]
        public EarthElemental(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an earth elemental";
            Body = 14;
            BaseSoundID = 268;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 75;

            Fame = 3500;
            Karma = -3500;           

            ControlSlots = 2;
           
            PackItem(new IronOre(3));
            PackItem(new MandrakeRoot(3));
            PackItem(new FertileDirt());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            AwardAchievementForKiller(AchievementTriggers.Trigger_EarthElementalKilled);
            AwardDailyAchievementForKiller(NewbCategory.KillEarthElementals);

            switch (Utility.Random(1000))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new SummonEarthElementalScroll())); } break;
            }
        }

        public EarthElemental(Serial serial): base(serial)
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
