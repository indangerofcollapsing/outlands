using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Guilds;
using Server.Misc;
using Server.Multis;

namespace Server.SkillHandlers
{
    public class Peacemaking
    {
        public static double PlayerDifficulty = 20;
        public static double MaxDifficulty = 40;
        public static double SkillRequiredPerDifficulty = 120 / MaxDifficulty;
        public static double SuccessChanceScalar = .05;
        public static double MinimumSkillDivisorChance = 10; // 10% at GM

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

                Mobile target = objTarget as Mobile;

                if (target == null)
                {
                    from.SendLocalizedMessage(1049528); // You cannot calm that!	
                    return;
                }

                if (BaseBoat.FindBoatAt(target.Location, target.Map) != null)
                    from.SendMessage("You may not calm creatures in sea vessels.");

                else if (from.Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)))
                    from.SendMessage("You may not peacemake in this area.");

                else if (target.Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)))
                    from.SendMessage("You may not peacemake there.");

                else if (!m_Instrument.IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!				

                else
                {
                    //Area Peacemaking
                    if (target == from)
                    {
                        if (!BaseInstrument.CheckMusicianship(from))
                        {
                            from.NextSkillTime = Core.TickCount + 5000;

                            from.SendMessage("You struggle with basic musicianship and your song has no effect.");

                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);
                        }

                        else if (!from.CheckSkill(SkillName.Peacemaking, 0.0, 120.0))
                        {
                            from.NextSkillTime = Core.TickCount + 5000;

                            from.SendLocalizedMessage(500613); // You attempt to calm everyone, but fail.

                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);
                        }

                        else
                        {
                            from.NextSkillTime = Core.TickCount + 10000;
                            ////Set cool down base on the tune/peacemaking mode
                            ////If this is a crowd control mode, then set to 6 sec, otherwise set to 10 sec as usual.
                            //if (from is PlayerMobile && ((PlayerMobile)from).PeacemakingMode == PeacemakingModeEnum.CrowdControl)
                            //    from.NextSkillTime = Core.TickCount + 6000;
                            //else
                            //from.NextSkillTime = Core.TickCount + 10000;

                            m_Instrument.PlayInstrumentWell(from);
                            m_Instrument.ConsumeUse(from);

                            Map map = from.Map;

                            if (map != null)
                            {
                                int range = BaseInstrument.GetBardRange(from, SkillName.Peacemaking);

                                bool calmed = false;

                                from.PublicOverheadMessage(MessageType.Regular, 0x3B2, false, "*plays calming music*");

                                IPooledEnumerable eable = from.GetMobilesInRange(range);

                                foreach (Mobile m in eable)
                                {
                                    BaseCreature bc_Target = m as BaseCreature;

                                    if ((m is BaseCreature && ((BaseCreature)m).Uncalmable) || (m is BaseCreature && ((BaseCreature)m).AreaPeaceImmune) || m == from || !from.CanBeHarmful(m, false))
                                        continue;

                                    if (bc_Target != null)
                                    {
                                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                                            continue;

                                        if (bc_Target.BardProvoked || bc_Target.BardPacified)
                                            continue;

                                        if (bc_Target.InitialDifficulty >= MaxDifficulty)
                                            continue;
                                    }

                                    if (BaseBoat.FindBoatAt(m.Location, m.Map) != null)
                                        continue;

                                    calmed = true;

                                    m.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
                                    m.Combatant = null;
                                    m.Warmode = false;

                                    //Set cool down base on the tune/peacemaking mode
                                    //If this is a crowd control mode, then set to 6 sec, otherwise set to 10 sec as usual.

                                    //Apply pacify effect based on the peacemaking mode
                                    if (m is BaseCreature && !((BaseCreature)m).BardPacified)
                                    {
                                        ((BaseCreature)m).Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(1.0), false);

                                        //   if (from is PlayerMobile &&
                                        //((PlayerMobile)from).PeacemakingMode == PeacemakingModeEnum.CrowdControl)
                                        //   {
                                        //       ((BaseCreature)m).Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(1.0), false, PeacemakingModeEnum.CrowdControl);
                                        //   }
                                        //   else
                                        //   {
                                        //       ((BaseCreature)m).Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(1.0), false, PeacemakingModeEnum.Combat);
                                        //   }
                                    }
                                }

                                eable.Free();

                                if (!calmed)
                                    from.SendLocalizedMessage(1049648); // You play hypnotic music, but there is nothing in range for you to calm.
                                else
                                    from.SendLocalizedMessage(500615); // You play your hypnotic music, stopping the battle.
                            }
                        }
                    }

                    //Targeted Peacemaking
                    else
                    {
                        BaseCreature bc_Target = target as BaseCreature;

                        if (!from.CanBeHarmful(target, false))
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

                            if (bc_Target.IsBoss() || bc_Target.IsMiniBoss() || bc_Target.IsLoHBoss() || bc_Target.IsEventBoss())// || bc_Target.InitialDifficulty >= MaxDifficulty)
                            {
                                from.SendMessage("You have no chance of pacifying that.");
                                return;
                            }
                        }

                        if (!BaseInstrument.CheckMusicianship(from))
                        {
                            from.SendMessage("You struggle with basic musicianship and your song has no effect.");

                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + 5000;
                        }

                        else
                        {
                            double targetDifficulty;

                            if (bc_Target != null)
                                targetDifficulty = bc_Target.InitialDifficulty;
                            else
                                targetDifficulty = PlayerDifficulty;

                            double effectiveBardSkill = from.Skills[SkillName.Peacemaking].Value;

                            if (m_Instrument.Quality == InstrumentQuality.Exceptional)
                                effectiveBardSkill += 5;

                            effectiveBardSkill += m_Instrument.GetBonusesFor(target);

                            double effectiveDifficulty = targetDifficulty * SkillRequiredPerDifficulty;
                            double successChance = (effectiveBardSkill - effectiveDifficulty) * SuccessChanceScalar;

                            double minimumSuccessChance = (effectiveBardSkill / MinimumSkillDivisorChance) / 100.0;

                            //Only GM+ Peace can have min chance perk
                            if (successChance < minimumSuccessChance && from.Skills[SkillName.Peacemaking].Value > 0)
                                successChance = minimumSuccessChance;

                            double chanceResult = Utility.RandomDouble();

                            //Skill Gain Check
                            from.CheckTargetSkill(SkillName.Peacemaking, target, effectiveDifficulty - 25.0, effectiveDifficulty + 25.0);

                            PlayerMobile pm_From = from as PlayerMobile;

                            if (pm_From != null)
                            {
                                if (pm_From.AccessLevel > AccessLevel.Player)
                                    pm_From.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");
                            }

                            if (chanceResult <= successChance)
                            {
                                m_Instrument.PlayInstrumentWell(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + 10000;

                                /*

                                if (bc_Target != null)
                                {
                                    if (bc_Target.Spell != null)
                                        bc_Target.Spell = null;

                                    from.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.

                                    double duration = BaseCreature.PeacemakingCreatureDuration;

                                    //Tamed Creature Peacemaking Duration Limit                                    
                                    if (bc_Target.Controlled && bc_Target.ControlMaster != null)
                                        duration = BaseCreature.PeacemakingFollowerDurationLimit;


                                    if (from is PlayerMobile &&
                                     ((PlayerMobile)from).PeacemakingMode == PeacemakingModeEnum.CrowdControl)
                                    {
                                        bc_Target.Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(duration), true, PeacemakingModeEnum.CrowdControl);
                                    }
                                    else
                                    {
                                        bc_Target.Pacify(from, DateTime.UtcNow + TimeSpan.FromSeconds(duration), true, PeacemakingModeEnum.Combat);
                                    }

                                }

                                else
                                {
                                    from.SendLocalizedMessage(1049532); // You play hypnotic music, calming your target.

                                    target.SendLocalizedMessage(500616); // You hear lovely music, and forget to continue battling!
                                    target.Combatant = null;
                                    target.Warmode = false;
                                }
                                */
                            }

                            else
                            {
                                m_Instrument.PlayInstrumentBadly(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + 5000;

                                string failureMessage = "You fail to pacify your opponent. You estimate the task to be beyond your skill.";

                                if (successChance > 0)
                                    failureMessage = "You fail to pacify your opponent. You estimate the task to be near impossible.";

                                if (successChance >= .05)
                                    failureMessage = "You fail to pacify your opponent. You estimate the task to be very difficult.";

                                if (successChance >= .25)
                                    failureMessage = "You fail to pacify your opponent. You estimate the task to be somewhat challenging.";

                                if (successChance >= .50)
                                    failureMessage = "You fail to pacify your opponent. You estimate the task to be fairly reasonable.";

                                if (successChance >= .75)
                                    failureMessage = "You fail to pacify your opponent. You estimate the task to be easy.";

                                if (successChance >= .95)
                                    failureMessage = "You fail to pacify your opponent. You estimate the task to be trivial.";

                                from.SendMessage(failureMessage);
                            }
                        }
                    }
                }
            }
        }
    }
}