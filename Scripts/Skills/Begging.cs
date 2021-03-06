using System;
using Server;
using Server.Misc;
using Server.Targeting;
using Server.Items;
using Server.Network;

namespace Server.SkillHandlers
{
    public class Begging
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.Begging].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            m.Target = new InternalTarget();
            m.RevealingAction();

            m.SendLocalizedMessage(500397); // To whom do you wish to grovel?

            return TimeSpan.FromHours(6.0);
        }

        private class InternalTarget : Target
        {
            private bool m_SetSkillTime = true;

            public InternalTarget(): base(12, false, TargetFlags.None)
            {
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_SetSkillTime)
                    from.NextSkillTime = Core.TickCount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                from.RevealingAction();

                int number = -1;

                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;

                    if (targ.Player) // We can't beg from players                    
                        number = 500398; // Perhaps just asking would work better.                    

                    else if (!targ.Body.IsHuman) // Make sure the NPC is human                    
                        number = 500399; // There is little chance of getting money from that!
                    
                    else if (!from.InRange(targ, 2))
                    {
                        if (!targ.Female)
                            number = 500401; // You are too far away to beg from him.
                        else
                            number = 500402; // You are too far away to beg from her.
                    }
                    
                    else if (!Core.ML && from.Mounted) // If we're on a mount, who would give us money? TODO: guessed it's removed since ML                    
                        number = 500404; // They seem unwilling to give you any money.
                    
                    else
                    {
                        // Face eachother
                        from.Direction = from.GetDirectionTo(targ);
                        targ.Direction = targ.GetDirectionTo(from);

                        from.Animate(32, 5, 1, true, false, 0); // Bow

                        new InternalTimer(from, targ).Start();

                        m_SetSkillTime = false;
                    }
                }

                else // Not a Mobile                
                    number = 500399; // There is little chance of getting money from that!                

                if (number != -1)
                    from.SendLocalizedMessage(number);
            }

            private class InternalTimer : Timer
            {
                private Mobile m_From, m_Target;

                public InternalTimer(Mobile from, Mobile target): base(TimeSpan.FromSeconds(2.0))
                {
                    m_From = from;
                    m_Target = target;
                    Priority = TimerPriority.TwoFiftyMS;
                }

                protected override void OnTick()
                {
                    Container theirPack = m_Target.Backpack;

                    if (theirPack == null)                    
                        m_From.SendLocalizedMessage(500404); // They seem unwilling to give you any money. 

                    else if (m_From.CheckTargetSkill(SkillName.Begging, m_Target, 0.0, 120.0, 1.0))
                    {
                        int toConsume = theirPack.GetAmount(typeof(Gold)) / 10;
                        int max = 10 + (m_From.Fame / 2500);

                        if (max > 14)
                            max = 14;

                        else if (max < 10)
                            max = 10;

                        if (toConsume > max)
                            toConsume = max;

                        if (toConsume > 0)
                        {
                            int consumed = theirPack.ConsumeUpTo(typeof(Gold), toConsume);

                            if (consumed > 0)
                            {
                                m_Target.PublicOverheadMessage(MessageType.Regular, m_Target.SpeechHue, 500405); // I feel sorry for thee...

                                Gold gold = new Gold(consumed);

                                m_From.AddToBackpack(gold);

                                //TEST: REDO
                                /*
                                if (m_From.Skills.Begging.Value == 100)
                                {
                                    var randomNumber = Utility.RandomMinMax(1, 100);

                                    if (randomNumber <= 75) //75% chance
                                    {
                                        switch (Utility.Random(2))
                                        {
                                            case 0: m_From.AddToBackpack(new Pitcher(BeverageType.Water)); break;

                                            default: m_From.AddToBackpack(new FrenchBread()); break;
                                        }
                                    }

                                    else if (randomNumber <= 99) //24% chance
                                    {
                                        switch (Utility.Random(7))
                                        {
                                            case 0: m_From.AddToBackpack(new Lantern()); break;
                                            case 1: m_From.AddToBackpack(new CheesePizza()); break;
                                            case 2: m_From.AddToBackpack(new WoodenBowlOfStew()); break;
                                            case 3: m_From.AddToBackpack(new CheeseWedge()); break;
                                            case 4: m_From.AddToBackpack(new Dates()); break;
                                            case 5: m_From.AddToBackpack(new Shirt()); break;

                                            default: m_From.AddToBackpack(new Pitcher(BeverageType.Liquor)); break;
                                        }
                                    }

                                    else // 1% chance
                                    {
                                        switch (Utility.RandomMinMax(1, 6))
                                        {
                                            case 1: m_From.AddToBackpack(new Bedroll()); break;
                                            case 2: m_From.AddToBackpack(new Pitcher(BeverageType.Wine)); break;
                                            case 3: m_From.AddToBackpack(new BeverageBottle(BeverageType.Wine)); break;
                                            case 4: m_From.AddToBackpack(new FlowerGarland()); break;
                                            case 5: m_From.AddToBackpack(new FishingPole()); break;
                                            case 6: m_From.AddToBackpack(new Turnip()); break;
                                        }
                                    }
                                }
                                */

                                m_From.PlaySound(gold.GetDropSound());

                                if (m_From.Karma > -3000)
                                {
                                    int toLose = m_From.Karma + 3000;

                                    if (toLose > 40)
                                        toLose = 40;

                                    FameKarmaTitles.AwardKarma(m_From, -toLose, true);
                                }
                            }

                            else                            
                                m_Target.PublicOverheadMessage(MessageType.Regular, m_Target.SpeechHue, 500407); // I have not enough money to give thee any!
                        }

                        else                        
                            m_Target.PublicOverheadMessage(MessageType.Regular, m_Target.SpeechHue, 500407); // I have not enough money to give thee any! 

                        m_From.NextSkillTime = Core.TickCount + (int)(SkillCooldown.BeggingSuccessCooldown * 1000);
                    }

                    else
                    {
                        m_From.SendLocalizedMessage(500404); // They seem unwilling to give you any money.
                        m_From.NextSkillTime = Core.TickCount + (int)(SkillCooldown.BeggingFailureCooldown * 1000);
                    }
                }
            }
        }
    }
}