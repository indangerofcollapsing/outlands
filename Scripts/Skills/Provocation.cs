using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server.SkillHandlers
{
    public class Provocation
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Provocation].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            BaseInstrument.PickInstrument(m, new InstrumentPickedCallback(OnPickedInstrument));

            return TimeSpan.FromSeconds(1.0);
        }

        public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.SendLocalizedMessage(501587); // Whom do you wish to incite?
            from.Target = new InternalFirstTarget(from, instrument);
        }

        private class InternalFirstTarget : Target
        {
            private BaseInstrument m_Instrument;

            public InternalFirstTarget(Mobile from, BaseInstrument instrument)
                : base(BaseInstrument.GetBardRange(from, SkillName.Provocation), false, TargetFlags.None)
            {
                m_Instrument = instrument;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature && from.CanBeHarmful((Mobile)targeted, true))
                {
                    BaseCreature creature = (BaseCreature)targeted;

                    if (!m_Instrument.IsChildOf(from.Backpack))
                        from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!	

                    else if (creature.Unprovokable)
                        from.SendMessage("That creature is not provokable.");

                    else if (creature.Controlled && creature.ControlMaster != null)
                        from.SendLocalizedMessage(501590); // They are too loyal to their master to be provoked.	

                    else if (BaseBoat.FindBoatAt(creature.Location, creature.Map) != null)
                        from.SendMessage("You may not provoke creatures in sea vessels.");

                    else
                    {
                        if (creature.NextBardingEffectAllowed > DateTime.UtcNow)
                        {
                            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, creature.NextBardingEffectAllowed, false, true, true, true, true);

                            from.SendMessage("That target is not vulnerable to barding attempts for another " + timeRemaining + ".");
                            return;
                        }

                        from.SendMessage("Whom do you wish them to attack?");
                        from.Target = new InternalSecondTarget(from, m_Instrument, creature);
                    }
                }

                else
                    from.SendLocalizedMessage(501589); // You can't incite that!				
            }
        }

        private class InternalSecondTarget : Target
        {
            private BaseCreature bc_FirstCreature;
            private BaseInstrument m_Instrument;

            public InternalSecondTarget(Mobile from, BaseInstrument instrument, BaseCreature creature)
                : base(BaseInstrument.GetBardRange(from, SkillName.Provocation), false, TargetFlags.None)
            {
                m_Instrument = instrument;
                bc_FirstCreature = creature;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                if (targeted is BaseCreature)
                {
                    BaseCreature bc_Target = targeted as BaseCreature;

                    if (!m_Instrument.IsChildOf(from.Backpack))
                        from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!				

                    else if (bc_Target.Unprovokable)
                        from.SendMessage("That creature is not provokable.");

                    else if (bc_FirstCreature.Map != bc_Target.Map || !bc_FirstCreature.InRange(bc_Target, BaseInstrument.GetBardRange(from, SkillName.Provocation)))
                        from.SendLocalizedMessage(1049450); // The creatures you are trying to provoke are too far away from each other for your music to have an effect.

                    else if (BaseBoat.FindBoatAt(bc_Target.Location, bc_Target.Map) != null)
                        from.SendMessage("You may not provoke creatures in sea vessels.");

                    else if (bc_FirstCreature != bc_Target)
                    {
                        if (from.CanBeHarmful(bc_FirstCreature, true) && from.CanBeHarmful(bc_Target, true))
                        {
                            if (bc_FirstCreature.NextBardingEffectAllowed > DateTime.UtcNow)
                            {
                                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_FirstCreature.NextBardingEffectAllowed, false, true, true, true, true);

                                from.SendMessage("Your original target is not vulnerable to barding attempts for another " + timeRemaining + ".");
                                return;
                            }

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

                                from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ProvocationFailureCooldown * 1000);

                                return;
                            }
                        }

                        else
                            return;

                        double creatureDifficulty = Math.Max(bc_FirstCreature.InitialDifficulty, bc_Target.InitialDifficulty);
                        double firstEffectiveBardSkill = from.Skills[SkillName.Peacemaking].Value + BaseInstrument.GetBardBonusSkill(from, bc_FirstCreature, m_Instrument);
                        double secondEffectiveBardSkill = from.Skills[SkillName.Peacemaking].Value + BaseInstrument.GetBardBonusSkill(from, bc_Target, m_Instrument);
                        double effectiveBardSkill = Math.Max(firstEffectiveBardSkill, secondEffectiveBardSkill);

                        double successChance = BaseInstrument.GetBardSuccessChance(effectiveBardSkill, creatureDifficulty);
                        TimeSpan effectDuration = BaseInstrument.GetBardDuration(bc_Target, creatureDifficulty);

                        if (BaseInstrument.CheckSkillGain(successChance))
                            from.CheckSkill(SkillName.Provocation, 0.0, 120.0, 1.0);

                        if (from.AccessLevel > AccessLevel.Player)
                            from.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");

                        if (Utility.RandomDouble() <= successChance)
                        {
                            from.SendLocalizedMessage(501602); // Your music succeeds, as you start a fight.

                            m_Instrument.PlayInstrumentWell(from);
                            m_Instrument.ConsumeUse(from);
                            bc_FirstCreature.Provoke(from, bc_Target, true, effectDuration, false);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ProvocationSuccessCooldown * 1000);
                        }

                        else
                        {
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ProvocationFailureCooldown * 1000);

                            string failureMessage = BaseInstrument.GetFailureMessage(successChance, SkillName.Provocation);

                            from.SendMessage(failureMessage);
                        }
                    }

                    else
                        from.SendLocalizedMessage(501593); // You can't tell someone to attack themselves!					
                }

                else if (targeted is PlayerMobile)
                {
                    PlayerMobile player = (PlayerMobile)targeted;

                    if (bc_FirstCreature.Map != player.Map || !bc_FirstCreature.InRange(player, BaseInstrument.GetBardRange(from, SkillName.Provocation)))
                        from.SendLocalizedMessage(1049450); // The creatures you are trying to provoke are too far away from each other for your music to have an effect.

                    else if (BaseBoat.FindBoatAt(player.Location, player.Map) != null)
                        from.SendMessage("You may not provoke creatures in sea vessels.");

                    else
                    {
                        if (from.CanBeHarmful(bc_FirstCreature, true) && from.CanBeHarmful(player, true))
                        {
                            if (bc_FirstCreature.NextBardingEffectAllowed > DateTime.UtcNow)
                            {
                                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, bc_FirstCreature.NextBardingEffectAllowed, false, true, true, true, true);

                                from.SendMessage("That target is not vulnerable to barding attempts for another " + timeRemaining + ".");
                                return;
                            }

                            if (!BaseInstrument.CheckMusicianship(from))
                            {
                                from.SendLocalizedMessage(500612); // You play poorly, and there is no effect. 

                                m_Instrument.PlayInstrumentBadly(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ProvocationFailureCooldown * 1000);

                                return;
                            }

                            double creatureDifficulty = bc_FirstCreature.InitialDifficulty;
                            double effectiveBardSkill = from.Skills[SkillName.Peacemaking].Value + BaseInstrument.GetBardBonusSkill(from, bc_FirstCreature, m_Instrument);

                            double successChance = BaseInstrument.GetBardSuccessChance(effectiveBardSkill, creatureDifficulty);
                            TimeSpan effectDuration = BaseInstrument.GetBardDuration(bc_FirstCreature, creatureDifficulty);

                            if (BaseInstrument.CheckSkillGain(successChance))
                                from.CheckSkill(SkillName.Provocation, 0.0, 120.0, 1.0);

                            if (from.AccessLevel > AccessLevel.Player)
                                from.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");

                            if (Utility.RandomDouble() <= successChance)
                            {
                                from.SendLocalizedMessage(501602); // Your music succeeds, as you start a fight.

                                m_Instrument.PlayInstrumentWell(from);
                                m_Instrument.ConsumeUse(from);
                                bc_FirstCreature.Provoke(from, player, true, effectDuration, false);

                                from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ProvocationSuccessCooldown * 1000);
                            }

                            else
                            {
                                m_Instrument.PlayInstrumentBadly(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ProvocationFailureCooldown * 1000);

                                string failureMessage = BaseInstrument.GetFailureMessage(successChance, SkillName.Provocation);

                                from.SendMessage(failureMessage);
                            }
                        }

                        else
                            return;
                    }
                }

                else
                    from.SendLocalizedMessage(501589); // You can't incite that!				
            }
        }
    }
}