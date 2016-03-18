using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Achievements;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an oreling corpse")]
    public class EarthOreling : BaseCreature
    {
        [Constructable]
        public EarthOreling(): base(AIType.AI_Generic, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "an earth oreling";
            Body = 305;
            BaseSoundID = 268;
            
            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 50);            
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 50;

            Fame = 1500;
            Karma = -1500;            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            AwardAchievementForKiller(AchievementTriggers.Trigger_EarthElementalKilled);
            AwardDailyAchievementForKiller(NewbCategory.KillEarthElementals);
        }

        public EarthOreling(Serial serial): base(serial)
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
