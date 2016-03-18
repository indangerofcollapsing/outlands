using System;
using Server;
using Server.Custom.Townsystem;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a chaos elemental corpse")]
    public class chaosElemental : BaseCreature
    {
        public override double DispelDifficulty { get { return 117.5; } }
        public override double DispelFocus { get { return 45.0; } }

        [Constructable]
        public chaosElemental(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a chaos elemental";
            Body = 15;
            BaseSoundID = 838;
            Hue = 1247;

            SetStr(75);
            SetDex(50);
            SetInt(100);

            SetHits(500);
            SetMana(2000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 150);

            VirtualArmor = 25;            

            Fame = 15000;
            Karma = -15000;

            ControlSlots = 3;

            PackItem(new SulfurousAsh(3));

            AddItem(new LightSource());
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            AwardDailyAchievementForKiller(PvPCategory.KillChaosElementals);

            switch (Utility.Random(500))
            {
                case 0: { c.AddItem(new SummonFireElementalScroll()); } break;
                case 1: { c.AddItem(new ForgedMetal()); } break;
                case 2: { c.AddItem(new Ruby()); } break;
                case 3: { c.AddItem(new BlankScroll()); } break;
                case 4: { c.AddItem(new ReactiveArmorScroll()); } break;
            }
        }

        public override bool OnBeforeDeath()
        {
            if (LastPlayerKiller != null && LastPlayerKiller.IsInMilitia)
            {
                AddItem(new BrazierDust(Utility.RandomMinMax(9, 20)));
            }
            return base.OnBeforeDeath();
        }

        public chaosElemental(Serial serial): base(serial)
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

            if (BaseSoundID == 274)
                BaseSoundID = 838;
        }
    }
}
