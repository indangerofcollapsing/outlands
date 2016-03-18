using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Achievements;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a rock elemental corpse")]
    public class RockElemental : BaseCreature
    {        
        [Constructable]
        public RockElemental(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rock elemental";
            Body = 14;
            Hue = 1108;
            BaseSoundID = 268;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(300);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 100;

            Fame = 3500;
            Karma = -3500;

            PackItem(new IronOre(4) {ItemID = 6584});
            PackItem(new MandrakeRoot(4));        
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

        public RockElemental(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

    }
}
