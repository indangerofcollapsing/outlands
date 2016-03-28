using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.SkillHandlers
{
    public class Discordance
    {
        public static double MaxDifficulty = 45;
        public static double ReducedDifficultyThreshold = 75;
        public static double SkillRequiredPerDifficulty = 120 / MaxDifficulty;
        public static double MinimumSkillDivisorChance = 6.7; // 15% at GM
        public static double SuccessChanceScalar = .05;

        public static double DiscordanceModifier = .25; //Percent penalty to Dex, Skills, Damage Dealt By Creature as well as Percent Increased in Damage Taken by Creature
        public static double ReducedEffectModifier = .20;

        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Discordance].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            BaseInstrument.PickInstrument(m, new InstrumentPickedCallback(OnPickedInstrument));

            return TimeSpan.FromSeconds(1.0);
        }

        public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.RevealingAction();
            from.SendLocalizedMessage(1049541); // Choose the target for your song of discordance.
            from.Target = new DiscordanceTarget(from, instrument);
        }

        public class DiscordanceInfo
        {
            public Mobile m_From;
            public Mobile m_Creature;
            public DateTime m_EndTime;
            public bool m_Ending;
            public Timer m_Timer;
            public int m_Effect;
            public ArrayList m_Mods;

            public DiscordanceInfo(Mobile from, Mobile creature, int effect, ArrayList mods)
            {
                m_From = from;
                m_Creature = creature;
                m_EndTime = DateTime.UtcNow;
                m_Ending = false;
                m_Effect = effect;
                m_Mods = mods;
            }

            public void Apply()
            {
                for (int i = 0; i < m_Mods.Count; ++i)
                {
                    object mod = m_Mods[i];

                    if (mod is ResistanceMod)
                        m_Creature.AddResistanceMod((ResistanceMod)mod);

                    else if (mod is StatMod)
                        m_Creature.AddStatMod((StatMod)mod);

                    else if (mod is SkillMod)
                        m_Creature.AddSkillMod((SkillMod)mod);
                }
            }

            public void Clear()
            {
                for (int i = 0; i < m_Mods.Count; ++i)
                {
                    object mod = m_Mods[i];

                    if (mod is ResistanceMod)
                        m_Creature.RemoveResistanceMod((ResistanceMod)mod);

                    else if (mod is StatMod)
                        m_Creature.RemoveStatMod(((StatMod)mod).Name);

                    else if (mod is SkillMod)
                        m_Creature.RemoveSkillMod((SkillMod)mod);
                }
            }
        }

        private static Hashtable m_Table = new Hashtable();

        public static void InsertDiscordanceInfo(Mobile target, DiscordanceInfo info)
        {
            if (m_Table.Contains(target))
            {
                var targetInfo = m_Table[target] as DiscordanceInfo;

                if (targetInfo != null)
                    targetInfo.m_EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(BaseCreature.DiscordanceCreatureDuration);

                info.m_Timer.Stop();
            }

            else
            {
                m_Table[target] = info;
                info.Apply();
            }
        }

        public static bool GetEffect(Mobile targ, ref int effect)
        {
            DiscordanceInfo info = m_Table[targ] as DiscordanceInfo;

            if (info == null)
                return false;

            effect = info.m_Effect;

            return true;
        }

        public static void ProcessDiscordance(DiscordanceInfo info)
        {
            Mobile from = info.m_From;
            Mobile targ = info.m_Creature;

            bool ends = false;

            if (!targ.Alive || targ.Deleted || !from.Alive || from.Hidden)
                ends = true;

            else
            {
                int range = (int)targ.GetDistanceToSqrt(from);
                int maxRange = 24;

                if (from.Map != targ.Map || range > maxRange)
                    ends = true;
            }

            if (ends || info.m_Ending || info.m_EndTime < DateTime.UtcNow)
            {
                if (info.m_Timer != null)
                    info.m_Timer.Stop();

                info.Clear();
                m_Table.Remove(targ);
            }

            else
                targ.FixedEffect(0x376A, 1, 32);
        }

        public class DiscordanceTarget : Target
        {
            private BaseInstrument m_Instrument;

            public DiscordanceTarget(Mobile from, BaseInstrument inst)
                : base(12, false, TargetFlags.None)
            {
                m_Instrument = inst;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                from.RevealingAction();

                if (!m_Instrument.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!    

                else if (target is BaseCreature)
                {
                    BaseCreature bc_Target = target as BaseCreature;

                    if (from.CanBeHarmful(bc_Target, true))
                    {
                        if (!BaseInstrument.CheckMusicianship(from))
                        {
                            from.SendMessage("You struggle with basic musicianship and your song has no effect.");
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DiscordanceFailureCooldown * 1000);

                            return;
                        }

                        double creatureDifficulty = bc_Target.InitialDifficulty;

                        double effectiveBardSkill = from.Skills[SkillName.Discordance].Value;
                        
                        //if (m_Instrument.Quality == Quality.Exceptional)
                            //effectiveBardSkill += 5;

                        effectiveBardSkill += 0; //m_Instrument.GetBonusesFor(bc_Target);

                        double effectiveDifficulty = creatureDifficulty * SkillRequiredPerDifficulty;
                        double successChance = (effectiveBardSkill - effectiveDifficulty) * SuccessChanceScalar;

                        double minimumSuccessChance = (effectiveBardSkill / MinimumSkillDivisorChance) / 100.0;

                        //Only GM+ Discord can have min chance perk
                        if (successChance < minimumSuccessChance && from.Skills[SkillName.Discordance].Value >= 90)
                            successChance = minimumSuccessChance;

                        double chanceResult = Utility.RandomDouble();

                        //Skill Gain Check
                        if (successChance > 0 && successChance < 1.40)
                            from.CheckSkill(SkillName.Discordance, 0.0, 120.0, 1.0);

                        PlayerMobile pm_From = from as PlayerMobile;

                        if (pm_From != null)
                        {
                            if (pm_From.AccessLevel > AccessLevel.Player)
                                pm_From.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");
                        }

                        if (chanceResult <= successChance)
                        {
                            from.DoHarmful(bc_Target, true);

                            from.SendMessage("You play successfully, disrupting your opponent's skills and weakening them significantly.");
                            m_Instrument.PlayInstrumentWell(from);
                            m_Instrument.ConsumeUse(from);

                            ArrayList mods = new ArrayList();

                            int effect;
                            double scalar;

                            double adjustedDiscordanceModifier = DiscordanceModifier;

                            DungeonArmor.PlayerDungeonArmorProfile bardDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(from, null);

                            if (bardDungeonArmor.MatchingSet && !bardDungeonArmor.InPlayerCombat)                            
                                adjustedDiscordanceModifier += bardDungeonArmor.DungeonArmorDetail.DiscordanceEffectBonus;                            

                            effect = (int)(-100 * adjustedDiscordanceModifier);
                            scalar = -1 * adjustedDiscordanceModifier;

                            if (bc_Target.InitialDifficulty >= MaxDifficulty)
                            {
                                effect = (int)((double)effect * ReducedEffectModifier);
                                scalar *= ReducedEffectModifier;
                            }

                            for (int i = 0; i < bc_Target.Skills.Length; ++i)
                            {
                                if (bc_Target.Skills[i].Value > 0)
                                    mods.Add(new DefaultSkillMod((SkillName)i, true, bc_Target.Skills[i].Base * scalar));
                            }

                            double duration = BaseCreature.DiscordanceCreatureDuration;
                            
                            DiscordanceInfo info = new DiscordanceInfo(from, bc_Target, Math.Abs(effect), mods);
                            info.m_EndTime = DateTime.UtcNow + TimeSpan.FromSeconds(duration);
                            info.m_Timer = Timer.DelayCall<DiscordanceInfo>(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerStateCallback<DiscordanceInfo>(ProcessDiscordance), info);

                            Discordance.InsertDiscordanceInfo(bc_Target, info);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DiscordanceSuccessCooldown * 1000);
                        }

                        else
                        {
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DiscordanceFailureCooldown * 1000);

                            string failureMessage = "You fail to disrupt your opponent. You estimate the task to be beyond your skill.";

                            if (successChance > 0)
                                failureMessage = "You fail to disrupt your opponent. You estimate the task to be near impossible.";

                            if (successChance >= .05)
                                failureMessage = "You fail to disrupt your opponent. You estimate the task to be very difficult.";

                            if (successChance >= .25)
                                failureMessage = "You fail to disrupt your opponent. You estimate the task to be somewhat challenging.";

                            if (successChance >= .50)
                                failureMessage = "You fail to disrupt your opponent. You estimate the task to be fairly reasonable.";

                            if (successChance >= .75)
                                failureMessage = "You fail to disrupt your opponent. You estimate the task to be easy.";

                            if (successChance >= .95)
                                failureMessage = "You fail to disrupt your opponent. You estimate the task to be trivial.";

                            from.SendMessage(failureMessage);
                        }
                    }

                    else
                        return;
                }

                else
                    from.SendLocalizedMessage(1049535); // A song of discord would have no effect on that.                
            }
        }
    }
}