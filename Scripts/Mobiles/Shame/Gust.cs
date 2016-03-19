using Server.Achievements;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a gust corpse")]
    public class Gust : BaseCreature
    {
        public override double DispelDifficulty { get { return 117.5; } }
        public override double DispelFocus { get { return 45.0; } }

        [Constructable]
        public Gust(): base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a gust";
            Body = 13;
            Hue = 2401;
            BaseSoundID = 655;

            SetStr(75);
            SetDex(75);
            SetInt(50);

            SetHits(125);
            SetMana(1000);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 50);

            Fame = 4500;
            Karma = -4500;

            VirtualArmor = 25;
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.05;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < .1)
                {
                    double damage = (double)(Utility.RandomMinMax(DamageMin, DamageMax)) * 2;
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*unleashes gust of air*");

                    Effects.PlaySound(this.Location, this.Map, 0x64C);

                    Animate(12, 6, 1, true, false, 0);

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, defender, damage, 5, -1, "", "The creature knocks you back with a gust of air!");
                }
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            AwardDailyAchievementForKiller(PvECategory.KillGusts);
            AwardAchievementForKiller(AchievementTriggers.Trigger_AirElementalKilled);
        }

        public Gust(Serial serial): base(serial)
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
