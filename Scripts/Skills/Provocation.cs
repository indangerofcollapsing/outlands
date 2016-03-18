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
        public static double PlayerDifficulty = 20;
        public static double MaxDifficulty = 35;
        public static double SkillRequiredPerDifficulty = 120 / MaxDifficulty;
        public static double SuccessChanceScalar = .05;
        public static double EasiestCreatureDifficultyScalar = .25;
        public static double MinimumSkillDivisorChance = 20; //5% at GM

        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Provocation].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            BaseInstrument.PickInstrument(m, new InstrumentPickedCallback(OnPickedInstrument));

            return TimeSpan.FromSeconds(1.0); // Cannot use another skill for 1 second
        }

        public static void OnPickedInstrument(Mobile from, BaseInstrument instrument)
        {
            from.SendLocalizedMessage(501587); // Whom do you wish to incite?
            from.Target = new InternalFirstTarget(from, instrument);
        }

        private class InternalFirstTarget : Target
        {
            private BaseInstrument m_Instrument;

            public InternalFirstTarget(Mobile from, BaseInstrument instrument): base(BaseInstrument.GetBardRange(from, SkillName.Provocation), false, TargetFlags.None)
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

                    else if (creature.IsMiniBoss() || creature.IsBoss() || creature.IsLoHBoss() || creature.IsEventBoss())					
                        from.SendMessage("You have no chance of provoking that.");

                    else if (creature.Unprovokable)
                        from.SendMessage("That creature is not provokable.");

                    else if (creature.Controlled && creature.ControlMaster != null)
                        from.SendLocalizedMessage(501590); // They are too loyal to their master to be provoked.	

                    else if (BaseBoat.FindBoatAt(creature.Location, creature.Map) != null)
                        from.SendMessage("You may not provoke creatures in sea vessels.");

                    else
                    {
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

            public InternalSecondTarget(Mobile from, BaseInstrument instrument, BaseCreature creature): base(BaseInstrument.GetBardRange(from, SkillName.Provocation), false, TargetFlags.None)
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

                    else if (bc_Target.IsMiniBoss() || bc_Target.IsBoss() || bc_Target.IsLoHBoss() || bc_Target.IsEventBoss())
                        from.SendMessage("You have no chance of provoking that.");

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
                            if (!BaseInstrument.CheckMusicianship(from))
                            {
                                from.SendMessage("You struggle with basic musicianship and your song has no effect.");

                                m_Instrument.PlayInstrumentBadly(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + 5000;

                                return;
                            }
                        }

                        else
                            return;

                        double hardestTargetDifficulty = Math.Max(bc_FirstCreature.InitialDifficulty, bc_Target.InitialDifficulty);
                        double easiestTargetDifficulty = Math.Min(bc_FirstCreature.InitialDifficulty, bc_Target.InitialDifficulty);

                        double bardingDifficulty = hardestTargetDifficulty + (easiestTargetDifficulty * EasiestCreatureDifficultyScalar);

                        double effectiveBardSkill = from.Skills[SkillName.Provocation].Value;

                        if (m_Instrument.Quality == InstrumentQuality.Exceptional)
                            effectiveBardSkill += BaseInstrument.ExceptionalQualitySkillBonus;

                        //Slayer Bonuses
                        double firstCreatureBonus = m_Instrument.GetBonusesFor(bc_FirstCreature);
                        double secondCreatureBonus = m_Instrument.GetBonusesFor(bc_Target);

                        double bestBonus = Math.Max(firstCreatureBonus, secondCreatureBonus);
                        effectiveBardSkill += bestBonus;

                        double effectiveDifficulty = bardingDifficulty * SkillRequiredPerDifficulty;
                        double successChance = (effectiveBardSkill - effectiveDifficulty) * SuccessChanceScalar;

                        double minimumSuccessChance = (effectiveBardSkill / MinimumSkillDivisorChance) / 100.0;

                        //Only GM+ Provo can have min chance perk
                        if (successChance < minimumSuccessChance && from.Skills[SkillName.Provocation].Value >= 100)
                            successChance = minimumSuccessChance;

                        double chanceResult = Utility.RandomDouble();

                        //Skill Gain Check
                        if (successChance > 0 && successChance < 1.25)
                            from.CheckSkill(SkillName.Provocation, 0.0, 120.0);

                        PlayerMobile pm_From = from as PlayerMobile;

                        if (pm_From != null)
                        {
                            if (pm_From.AccessLevel > AccessLevel.Player)
                                pm_From.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");
                        }                        

                        if (chanceResult <= successChance)
                        {
                            from.SendLocalizedMessage(501602); // Your music succeeds, as you start a fight.

                            m_Instrument.PlayInstrumentWell(from);
                            m_Instrument.ConsumeUse(from);
                            bc_FirstCreature.Provoke(from, bc_Target, true, false);

                            from.NextSkillTime = Core.TickCount + 10000;
                        }

                        else
                        {
                            m_Instrument.PlayInstrumentBadly(from);
                            m_Instrument.ConsumeUse(from);

                            from.NextSkillTime = Core.TickCount + 5000;

                            string failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be beyond your skill.";

                            if (successChance > 0)
                                failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be near impossible.";

                            if (successChance >= .05)
                                failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be very difficult.";

                            if (successChance >= .25)
                                failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be somewhat challenging.";

                            if (successChance >= .50)
                                failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be fairly reasonable.";

                            if (successChance >= .75)
                                failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be easy.";

                            if (successChance >= .95)
                                failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be trivial.";

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
                            if (!BaseInstrument.CheckMusicianship(from))
                            {
                                from.SendLocalizedMessage(500612); // You play poorly, and there is no effect. 

                                m_Instrument.PlayInstrumentBadly(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + 5000;

                                return;
                            }

                            double bardingDifficulty = bc_FirstCreature.InitialDifficulty;

                            double effectiveBardSkill = from.Skills[SkillName.Provocation].Value;

                            if (m_Instrument.Quality == InstrumentQuality.Exceptional)
                                effectiveBardSkill += BaseInstrument.ExceptionalQualitySkillBonus;

                            effectiveBardSkill += m_Instrument.GetBonusesFor(bc_FirstCreature);

                            double effectiveDifficulty = bardingDifficulty * SkillRequiredPerDifficulty;
                            double successChance = (effectiveBardSkill - effectiveDifficulty) * SuccessChanceScalar;

                            double chanceResult = Utility.RandomDouble();

                            //Skill Gain Check
                            if (successChance > 0 && successChance < 1.0)
                                from.CheckSkill(SkillName.Provocation, 0.0, 120.0);

                            PlayerMobile pm_From = from as PlayerMobile;

                            if (pm_From != null)
                            {
                                if (pm_From.AccessLevel > AccessLevel.Player)
                                    pm_From.SendMessage("Chance of success was: " + Math.Round(successChance * 100, 3).ToString() + "%");
                            }

                            if (chanceResult <= successChance)
                            {
                                from.SendLocalizedMessage(501602); // Your music succeeds, as you start a fight.

                                m_Instrument.PlayInstrumentWell(from);
                                m_Instrument.ConsumeUse(from);
                                bc_FirstCreature.Provoke(from, player, true, false);

                                from.NextSkillTime = Core.TickCount + 10000;
                            }

                            else
                            {
                                m_Instrument.PlayInstrumentBadly(from);
                                m_Instrument.ConsumeUse(from);

                                from.NextSkillTime = Core.TickCount + 5000;

                                string failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be beyond your skill.";

                                if (successChance > 0)
                                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be near impossible.";

                                if (successChance >= .05)
                                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be very difficult.";

                                if (successChance >= .25)
                                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be somewhat challenging.";

                                if (successChance >= .50)
                                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be fairly reasonable.";

                                if (successChance >= .75)
                                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be easy.";

                                if (successChance >= .95)
                                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be trivial.";

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