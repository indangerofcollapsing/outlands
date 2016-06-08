using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

using Server.Misc;
using Server.Multis;

namespace Server.SkillHandlers
{
    public class Peacemaking
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Peacemaking].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            BaseInstrument.PickInstrument(m, new InstrumentPickedCallback(OnPickedInstrument));

            return TimeSpan.FromSeconds(1.0); // Cannot use another skill for 1 second
        }

        public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.RevealingAction();
            from.SendLocalizedMessage(1049525); // Whom do you wish to calm?
            from.Target = new InternalTarget(from, instrument);
        }

        private class InternalTarget : Target
        {
            private BaseInstrument m_Instrument;
            private bool m_SetSkillTime = true;

            public InternalTarget(Mobile from, BaseInstrument instrument)
                : base(12, false, TargetFlags.None)
            {
                m_Instrument = instrument;
            }

            protected override void OnTarget(Mobile from, object objTarget)
            {
                from.RevealingAction();

                BaseCreature bc_Target = objTarget as BaseCreature;

                if (bc_Target == null)
                {
                    from.SendLocalizedMessage(1049528); // You cannot calm that!	
                    return;
                }

                if (BaseBoat.FindBoatAt(bc_Target.Location, bc_Target.Map) != null)
                    from.SendMessage("You may not calm targets in sea vessels.");

                else if (from.Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)))
                    from.SendMessage("You may not peacemake in this area.");

                else if (bc_Target.Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)))
                    from.SendMessage("You may not peacemake there.");

                else if (!m_Instrument.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!				

                else
                {
                    if (!from.CanBeHarmful(bc_Target, false))
                    {
                        from.SendMessage("That cannot be pacified.");
                        return;
                    }

                    if (bc_Target != null)
                    {
                        if (bc_Target.Uncalmable)
                        {
                            from.SendMessage("That creature cannot be pacified.");
                            return;
                        }

                        if (bc_Target.NextBardingEffectAllowed > DateTime.UtcNow)
                        {
                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_Target.NextBardingEffectAllowed, false, true, true, true, true);

                            from.SendMessage("That target is not vulnerable to barding attempts for another " + timeRemaining + ".");
                            return;
                        }
                    }

                    if (!BaseInstrument.CheckMusicianship(from))
                    {
                        from.SendMessage("You struggle with basic musicianship and your song has no effect.");

                        m_Instrument.PlayInstrumentBadly(from);
                        m_Instrument.ConsumeUse(from);

                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.PeacemakingFailureCooldown * 1000);
                    }

                    else
                    {
                        double creatureDifficulty = bc_Target.InitialDifficulty;
                        double effectiveBardSkill = from.Skills[SkillName.Peacemaking].Value + BaseInstrument.GetBardBonusSkill(from, bc_Target, m_Instrument);

                        double successChance = BaseInstrument.GetBardSuccessChance(effectiveBardSkill, creatureDifficulty);
                        TimeSpan effectDuration = BaseInstrument.GetBardDuration(bc_Target, creatureDifficulty);

                        if (BaseInstrument.CheckSkillGain(successChance))
                            from.CheckSkill(SkillName.Peacemaking, 0.0, 120.0, 1.0);

                        if (from.AccessLevel > AccessLevel.Player)
                            from.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");

                        if (Utility.RandomDouble() <= successChance)
                        {
                            m_Instrument.PlayInstrumentWell(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.PeacemakingSuccessCooldown * 1000);

                            if (bc_Target.Spell != null)
                                bc_Target.Spell = null;

                            from.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.

                            bc_Target.Pacify(from, effectDuration, true);
                        }

                        else
                        {
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.PeacemakingFailureCooldown * 1000);

                            string failureMessage = BaseInstrument.GetFailureMessage(successChance, SkillName.Peacemaking);

                            from.SendMessage(failureMessage);
                        }
                    }
                }
            }
        }
    }
}