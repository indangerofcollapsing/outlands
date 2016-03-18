using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a rotting corpse")]
    public class RottingCorpse : BaseCreature
    {
        [Constructable]
        public RottingCorpse()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rotting corpse";
            Body = 155;
            BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(1250);

            SetDamage(25, 35);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 33);

            VirtualArmor = 25;

            Fame = 6000;
            Karma = -6000;
            PackItem(new Bone(15));
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override Poison HitPoison { get { return Poison.Lethal; } }

        public override bool OnBeforeDeath()
        {
            AwardDailyAchievementForKiller(PvECategory.KillRottingCorpses);

            if (Utility.Random(250) == 0)
            {
                AddItem(TitleDye.VeryRareTitleDye(Server.Custom.PlayerTitleColors.EVeryRareColorTypes.SnowWhiteTitleHue));
            }

            return base.OnBeforeDeath();
        }

        public RottingCorpse(Serial serial): base(serial)
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
