using Server.Achievements;
using Server.Items;
using Server.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Mobiles
{
    [CorpseName("an orcish executioner corpse")]
    public class OrcishExecutioner : BaseOrc
    {
        [Constructable]
        public OrcishExecutioner(): base()
        {            
            Name = "an orcish executioner";

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(600);

            AttackSpeed = 30;

            SetDamage(18, 28);

            SetSkill(SkillName.Archery, 95);
            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Fame = 5000;
            Karma = -5000;            

            AddItem(new OrcMask() { Movable = false, Hue = 1776 });
            AddItem(new BoneArms() { Movable = false, Hue = 0 });
            AddItem(new BoneGloves() { Movable = false, Hue = 1775 });
            AddItem(new Kilt() { Movable = false, Hue = 2051 });
            AddItem(new Boots() { Movable = false, Hue = 2051 });

            AddItem(new ExecutionersAxe());
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.2;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }
        
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (Global_AllowAbilities)
            {
                if (!willKill)
                {
                    double hitsPercent = (double)Hits / (double)HitsMax;

                    double minAttackSpeed = 30;
                    double maxAttackBonus = 80;

                    int finalAttackSpeed = (int)(minAttackSpeed + (maxAttackBonus * (1 - hitsPercent)));

                    AttackSpeed = finalAttackSpeed;

                    if (Utility.RandomDouble() < .5 && !BardPacified)
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*becomes more enraged*");
                }
            }

            base.OnDamage(amount, from, willKill);
        }
        
        public override bool OnBeforeDeath()
        {
            if (Global_AllowAbilities)
                AttackSpeed = 30;

            AwardDailyAchievementForKiller(PvECategory.KillOrcishExecutioners);

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            if (Region is Server.Regions.DungeonRegion && Utility.Random(500) == 0)
                c.AddItem(TitleDye.VeryRareTitleDye(Server.Custom.PlayerTitleColors.EVeryRareColorTypes.DarkGreyTitleHue));
                
            AwardAchievementForKiller(AchievementTriggers.Trigger_OrcKilled);
            base.OnDeath(c);
        }

        public OrcishExecutioner(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (Global_AllowAbilities)
                AttackSpeed = 30;
        }
    } 
}
