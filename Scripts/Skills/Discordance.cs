using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

namespace Server.SkillHandlers
{
    public class Discordance
    {
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
            public TimeSpan m_Duration;
            public DateTime m_EndTime;
            public bool m_Ending;
            public Timer m_Timer;
            public double m_Effect;

            public DiscordanceInfo(Mobile from, Mobile creature, double effect)
            {
                m_From = from;
                m_Creature = creature;
                m_EndTime = DateTime.UtcNow;
                m_Ending = false;
                m_Effect = effect;
            }
        }

        private static Hashtable m_Table = new Hashtable();

        public static void InsertDiscordanceInfo(Mobile target, DiscordanceInfo info)
        {
            if (m_Table.Contains(target))
            {
                var targetInfo = m_Table[target] as DiscordanceInfo;

                if (targetInfo != null)
                    targetInfo.m_EndTime = DateTime.UtcNow + targetInfo.m_Duration;

                info.m_Timer.Stop();
            }

            else
                m_Table[target] = info;
        }

        public static DiscordanceInfo GetInfo(Mobile target)
        {
            DiscordanceInfo info = m_Table[target] as DiscordanceInfo;

            return info;
        }

        public static bool GetEffect(Mobile targ, ref double effect)
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
                        if (bc_Target.NextBardingEffectAllowed > DateTime.UtcNow)
                        {
                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_Target.NextBardingEffectAllowed, false, true, true, true, true);

                            from.SendMessage("That target is not vulnerable to barding attempts for another " + timeRemaining + ".");
                            return;
                        }

                        if (!BaseInstrument.CheckMusicianship(from))
                        {
                            from.SendMessage("You struggle with basic musicianship and your song has no effect.");
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DiscordanceFailureCooldown * 1000);

                            return;
                        }

                        double creatureDifficulty = bc_Target.InitialDifficulty;
                        double effectiveBardSkill = from.Skills[SkillName.Discordance].Value + BaseInstrument.GetBardBonusSkill(from, bc_Target, m_Instrument);

                        double successChance = BaseInstrument.GetBardSuccessChance(effectiveBardSkill, creatureDifficulty);
                        TimeSpan effectDuration = BaseInstrument.GetBardDuration(bc_Target, creatureDifficulty);

                        if (BaseInstrument.CheckSkillGain(successChance))
                            from.CheckSkill(SkillName.Discordance, 0.0, 120.0, 1.0);

                        if (from.AccessLevel > AccessLevel.Player)
                            from.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");

                        if (Utility.RandomDouble() <= successChance)
                        {
                            from.DoHarmful(bc_Target, true);

                            from.SendMessage("You play successfully, disrupting your opponent's skills and weakening them significantly.");
                            m_Instrument.PlayInstrumentWell(from);
                            m_Instrument.ConsumeUse(from);

                            double discordanceModifier = BaseInstrument.DiscordanceModifier;

                            DungeonArmor.PlayerDungeonArmorProfile bardDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(from, null);

                            if (bardDungeonArmor.MatchingSet && !bardDungeonArmor.InPlayerCombat)
                                discordanceModifier += bardDungeonArmor.DungeonArmorDetail.DiscordanceEffectBonus;

                            DiscordanceInfo info = new DiscordanceInfo(from, bc_Target, discordanceModifier);

                            info.m_Duration = effectDuration;
                            info.m_EndTime = DateTime.UtcNow + effectDuration;
                            info.m_Timer = Timer.DelayCall<DiscordanceInfo>(TimeSpan.Zero, TimeSpan.FromSeconds(1), new TimerStateCallback<DiscordanceInfo>(ProcessDiscordance), info);

                            Discordance.InsertDiscordanceInfo(bc_Target, info);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DiscordanceSuccessCooldown * 1000);
                            
                            bc_Target.NextBardingEffectAllowed = DateTime.UtcNow + bc_Target.BardingEffectCooldown;
                        }

                        else
                        {
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.DiscordanceFailureCooldown * 1000);

                            string failureMessage = BaseInstrument.GetFailureMessage(successChance, SkillName.Discordance);

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