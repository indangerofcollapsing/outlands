using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Gumps;

namespace Server
{
    public class DamageTracker
    {
        public enum DamageType
        {
            MeleeDamage,
            SpellDamage,
            PoisonDamage,
            FollowerDamage,
            ProvocationDamage,            
            DamageTaken,
            FollowerDamageTaken,
            HealingDealt
        }

        public PlayerMobile m_Player;
        public bool m_Running = false;

        public bool m_Collapsed = false;

        public DamageTrackerTimer m_Timer;

        public int MeleeDamage = 0;
        public int SpellDamage = 0;
        public int FollowerDamage = 0;
        public int ProvocationDamage = 0;
        public int PoisonDamage = 0;
        public int DamageTaken = 0;
        public int FollowerDamageTaken = 0;
        public int HealingDealt = 0;

        public bool AddMeleeDamageToTotal = true;
        public bool AddSpellDamageToTotal = true;
        public bool AddPoisonDamageToTotal = true;
        public bool AddProvocationDamageToTotal = true;
        public bool AddFollowerDamageToTotal = true;

        public TimeSpan RunningTime = TimeSpan.FromSeconds(0);
        
        public bool AutoStopEnabled = false;
        public TimeSpan AutoStopDuration = AutoStopMinimumDuration;
        public static TimeSpan AutoStopMinimumDuration = TimeSpan.FromSeconds(5);

        public static TimeSpan TickSpeed = TimeSpan.FromSeconds(1);

        public DamageTracker(PlayerMobile player)
        {
            m_Player = player;
        }
        
        public void ClearResults()
        {
            RunningTime = TimeSpan.FromSeconds(0);

            MeleeDamage = 0;
            SpellDamage = 0;
            FollowerDamage = 0;
            ProvocationDamage = 0;
            PoisonDamage = 0;
            DamageTaken = 0;
            FollowerDamageTaken = 0;
            HealingDealt = 0;
        }

        public static void RecordDamage(Mobile owner, Mobile from, Mobile target, DamageType damageType, int amount)
        {
            PlayerMobile player = owner as PlayerMobile;

            if (player == null)
                return;

            if (player.m_DamageTracker == null)
                player.m_DamageTracker = new DamageTracker(player);

            switch (damageType)
            {
                case DamageType.MeleeDamage:
                    if (player.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerMeleeDamageTextHue, "You attack " + target.Name + " for " + amount.ToString() + " damage.");

                    if (player.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerMeleeDamageTextHue, false, "-" + amount.ToString(), player.NetState);                    
                break;

                case DamageType.SpellDamage: 
                    if (player.m_ShowSpellDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerSpellDamageTextHue, "Your spell hits " + target.Name + " for " + amount.ToString() + " damage.");

                    if (player.m_ShowSpellDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerSpellDamageTextHue, false, "-" + amount.ToString(), player.NetState);
                break;

                case DamageType.FollowerDamage:
                    if (player.m_ShowFollowerDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerFollowerDamageTextHue, "Follower: " + from.Name + " attacks " + target.Name + " for " + amount.ToString() + " damage.");

                    if (player.m_ShowFollowerDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerFollowerDamageTextHue, false, "-" + amount.ToString(), player.NetState);
                break;

                case DamageType.ProvocationDamage:
                    if (player.m_ShowProvocationDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerProvocationDamageTextHue, "Provocation: " + from.Name + " inflicts " + amount.ToString() + " damage on " + target.Name + ".");

                    if (player.m_ShowProvocationDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerProvocationDamageTextHue, false, "-" + amount.ToString(), player.NetState);
                break;

                case DamageType.PoisonDamage: 
                    if (player.m_ShowPoisonDamage == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerPoisonDamageTextHue, "You inflict " + amount.ToString() + " poison damage on " + target.Name + ".");

                    if (player.m_ShowPoisonDamage == DamageDisplayMode.PrivateOverhead)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerPoisonDamageTextHue, false, "-" + amount.ToString(), player.NetState);
                break;

                case DamageType.DamageTaken:
                    if (player.m_ShowDamageTaken == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerDamageTakenTextHue, from.Name + " attacks you for " + amount.ToString() + " damage.");

                    if (player.m_ShowDamageTaken == DamageDisplayMode.PrivateOverhead)
                        player.PrivateOverheadMessage(MessageType.Regular, player.PlayerDamageTakenTextHue, false, "-" + amount.ToString(), player.NetState);
                break;

                case DamageType.FollowerDamageTaken:
                    if (player.m_ShowFollowerDamageTaken == DamageDisplayMode.PrivateMessage)
                        player.SendMessage(player.PlayerFollowerDamageTakenTextHue, "Follower: " + target.Name + " is hit for " + amount.ToString() + " damage.");

                    if (player.m_ShowFollowerDamageTaken == DamageDisplayMode.PrivateOverhead && player.NetState != null)
                        target.PrivateOverheadMessage(MessageType.Regular, player.PlayerFollowerDamageTakenTextHue, false, "-" + amount.ToString(), player.NetState);
                break;
            }
            
            if (!player.m_DamageTracker.m_Running)
                return;

            switch (damageType)
            {
                case DamageType.MeleeDamage: player.m_DamageTracker.MeleeDamage += amount; break;
                case DamageType.SpellDamage: player.m_DamageTracker.SpellDamage += amount; break;
                case DamageType.FollowerDamage: player.m_DamageTracker.FollowerDamage += amount; break;
                case DamageType.ProvocationDamage: player.m_DamageTracker.ProvocationDamage += amount; break;
                case DamageType.PoisonDamage: player.m_DamageTracker.PoisonDamage += amount; break;
                case DamageType.DamageTaken: player.m_DamageTracker.DamageTaken += amount; break;
                case DamageType.FollowerDamageTaken: player.m_DamageTracker.FollowerDamageTaken += amount; break;
                case DamageType.HealingDealt: player.m_DamageTracker.HealingDealt += amount; break;
            }
        }

        public void RefreshGump()
        {
            if (m_Player != null)
            {
                if (m_Player.HasGump(typeof(DamageTrackerGump)))
                {
                    m_Player.CloseGump(typeof(DamageTrackerGump));
                    m_Player.SendGump(new DamageTrackerGump(m_Player));
                }
            }
        }

        public void StopTimer()
        {
            m_Running = false;

            if (m_Timer != null)
            {                
                m_Timer.Stop();
                m_Timer = null;
            }
        }

        public void StartTimer()
        {
            m_Running = true;

            if (m_Timer == null)
            {
                m_Timer = new DamageTrackerTimer(this);
                m_Timer.Start();
            }

            else
                m_Timer.Start();
        }

        public class DamageTrackerTimer : Timer
        {
            public DamageTracker m_DamageTracker;

            public DamageTrackerTimer(DamageTracker damageTracker): base(TickSpeed, TickSpeed)
            {
                m_DamageTracker = damageTracker;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_DamageTracker == null)
                {
                    Stop();
                    return;
                }

                if (m_DamageTracker.m_Player == null)
                {
                    Stop();
                    return;
                }

                if (m_DamageTracker.m_Player.Deleted || m_DamageTracker.m_Player.NetState == null)
                {
                    Stop();
                    return;
                }

                if (!m_DamageTracker.m_Running)
                {
                    Stop();
                    return;
                }
                
                m_DamageTracker.RunningTime += DamageTracker.TickSpeed;

                if (m_DamageTracker.AutoStopEnabled)
                {
                    if (m_DamageTracker.RunningTime >= m_DamageTracker.AutoStopDuration)
                        m_DamageTracker.StopTimer();         
                }

                m_DamageTracker.RefreshGump();
            }
        }
    }

    public class DamageTrackerGump : Gump
    {
        public PlayerMobile m_Player;

        public DamageTrackerGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;
            if (player.m_DamageTracker == null) return;

            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            int totalDamage = 0;

            if (player.m_DamageTracker.AddMeleeDamageToTotal)
                totalDamage += player.m_DamageTracker.MeleeDamage;

            if (player.m_DamageTracker.AddSpellDamageToTotal)
                totalDamage += player.m_DamageTracker.SpellDamage;

            if (player.m_DamageTracker.AddFollowerDamageToTotal)
                totalDamage += player.m_DamageTracker.FollowerDamage;

            if (player.m_DamageTracker.AddProvocationDamageToTotal)
                totalDamage += player.m_DamageTracker.ProvocationDamage;

            if (player.m_DamageTracker.AddPoisonDamageToTotal)
                totalDamage += player.m_DamageTracker.PoisonDamage;

            string timeElapsed = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + m_Player.m_DamageTracker.RunningTime, true, false, true, true, true);

            if (m_Player.m_DamageTracker.RunningTime == TimeSpan.FromSeconds(0))
                timeElapsed = "0s";

            string autostopHours = m_Player.m_DamageTracker.AutoStopDuration.Hours.ToString() + "h";
            string autostopMinutes = m_Player.m_DamageTracker.AutoStopDuration.Minutes.ToString() + "m";
            string autostopSeconds = m_Player.m_DamageTracker.AutoStopDuration.Seconds.ToString() + "s";
            
            if (player.m_DamageTracker.m_Collapsed)
            {
                AddAlphaRegion(9, 7, 150, 90);

                AddButton(16, 18, 9900, 9900, 1, GumpButtonType.Reply, 0); //Expand Gump    

                AddLabel(42, 17, 2577, "Total Damage");
                AddLabel(130, 17, textHue, Utility.CreateCurrencyString(totalDamage));

                AddItem(39, 38, 6160); //Hour Glass

                AddButton(46, 69, 4017, 4019, 2, GumpButtonType.Reply, 0); //Reset

                //Running
                if (m_Player.m_DamageTracker.m_Running)
                {
                    AddLabel(72, 43, 63, timeElapsed);
                    AddButton(88, 69, 4020, 4022, 3, GumpButtonType.Reply, 0); //Pause
                }

                //Paused
                else
                {
                    AddLabel(72, 43, 2213, timeElapsed);
                    AddButton(88, 69, 4005, 4007, 3, GumpButtonType.Reply, 0); //Start                   
                }
            }

            else
            {
                AddAlphaRegion(3, 9, 169, 376); 

                //Collapse
                AddButton(16, 20, 9906, 9906, 1, GumpButtonType.Reply, 0); //Collapse Gump

                //Damage Dealt
                AddImage(40, 17, 2445, 2425);         
                AddLabel(52, 18, textHue, "Damage Dealt");
                AddLabel(49, 45, 34, "Melee");
                if (player.m_DamageTracker.AddMeleeDamageToTotal)
                    AddButton(91, 45, 211, 210, 20, GumpButtonType.Reply, 0);
                else
                    AddButton(91, 45, 210, 211, 20, GumpButtonType.Reply, 0);
                AddLabel(117, 45, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.MeleeDamage));

                AddLabel(52, 65, 117, "Spell");
                if (player.m_DamageTracker.AddSpellDamageToTotal)
                    AddButton(91, 65, 211, 210, 21, GumpButtonType.Reply, 0);
                else
                    AddButton(91, 65, 210, 211, 21, GumpButtonType.Reply, 0);
                AddLabel(117, 65, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.SpellDamage));

                AddLabel(42, 85, 63, "Poison");
                if (player.m_DamageTracker.AddPoisonDamageToTotal)
                    AddButton(91, 85, 211, 210, 22, GumpButtonType.Reply, 0);
                else
                    AddButton(91, 85, 210, 211, 22, GumpButtonType.Reply, 0);
                AddLabel(117, 85, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.PoisonDamage));

                AddLabel(11, 105, 2417, "Provocation");
                if (player.m_DamageTracker.AddProvocationDamageToTotal)
                    AddButton(91, 105, 211, 210, 23, GumpButtonType.Reply, 0);
                else
                    AddButton(91, 105, 210, 211, 23, GumpButtonType.Reply, 0);

                AddLabel(117, 105, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.ProvocationDamage));

                AddLabel(26, 125, 89, "Followers");
                if (player.m_DamageTracker.AddFollowerDamageToTotal)
                    AddButton(91, 125, 211, 210, 24, GumpButtonType.Reply, 0);
                else
                    AddButton(91, 125, 210, 211, 24, GumpButtonType.Reply, 0);
                AddLabel(117, 125, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.FollowerDamage));
               
                AddLabel(48, 144, 2577, "Total");
                AddLabel(117, 144, textHue, Utility.CreateCurrencyString(totalDamage));

                 //Damage Taken
                AddImage(40, 169, 2445, 2425);
                AddLabel(50, 171, textHue, "Damage Taken");
                AddLabel(82, 198, 2603, "Self");
                AddLabel(117, 198, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.DamageTaken));

                AddLabel(52, 219, 2562, "Followers");
                AddLabel(117, 219, textHue, Utility.CreateCurrencyString(player.m_DamageTracker.FollowerDamageTaken));

                //Stop Timer At
                AddImage(40, 243, 2445, 2425);
                AddLabel(49, 245, textHue, "Stop Timer At");
                if (m_Player.m_DamageTracker.AutoStopEnabled)
                    AddButton(18, 283, 2154, 2151, 4, GumpButtonType.Reply, 0);
                else
                    AddButton(18, 283, 2151, 2154, 4, GumpButtonType.Reply, 0);

                //Hours
                AddButton(58, 273, 5600, 5600, 5, GumpButtonType.Reply, 0);
                AddLabel(Utility.CenteredTextOffset(65, autostopHours), 288, textHue, autostopHours);
                AddButton(58, 308, 5606, 5606, 6, GumpButtonType.Reply, 0);

                AddButton(91, 273, 5600, 5600, 7, GumpButtonType.Reply, 0);
                AddLabel(Utility.CenteredTextOffset(98, autostopMinutes), 288, textHue, autostopMinutes);
                AddButton(91, 308, 5606, 5606, 8, GumpButtonType.Reply, 0);

                AddButton(122, 273, 5600, 5600, 9, GumpButtonType.Reply, 0);
                AddLabel(Utility.CenteredTextOffset(130, autostopSeconds), 288, textHue, autostopSeconds);
                AddButton(122, 308, 5606, 5606, 10, GumpButtonType.Reply, 0);                                

                //Reset
                AddLabel(8, 330, 2116, "Reset");
                AddButton(50, 330, 4017, 4019, 2, GumpButtonType.Reply, 0);                

                //Running
                if (m_Player.m_DamageTracker.m_Running)
                {
                    AddLabel(77, 361, 63, timeElapsed); //Time Elapsed

                    AddLabel(93, 331, 2599, "Pause");
                    AddButton(134, 329, 4020, 4022, 3, GumpButtonType.Reply, 0);
                }

                //Paused
                else
                {
                    AddLabel(77, 361, 2213, timeElapsed); //Time Elapsed

                    AddLabel(93, 331, 2599, "Start");
                    AddButton(134, 329, 4005, 4007, 3, GumpButtonType.Reply, 0);
                }

                AddItem(44, 356, 6160); //Hourglass
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.m_DamageTracker == null) return;

            bool closeGump = true;

            if (m_Player.m_DamageTracker.m_Collapsed)
            {
                switch (info.ButtonID)
                {
                    //Expand
                    case 1:
                        m_Player.m_DamageTracker.m_Collapsed = false;

                        closeGump = false;
                    break;

                    //Clear Results
                    case 2:
                        m_Player.m_DamageTracker.ClearResults();

                        closeGump = false;
                    break;

                    //Start and Stop
                    case 3:
                        if (m_Player.m_DamageTracker.m_Running)
                            m_Player.m_DamageTracker.StopTimer();

                        else
                            m_Player.m_DamageTracker.StartTimer();

                        closeGump = false;
                    break;
                }
            }

            else
            {

                switch (info.ButtonID)
                {
                    //Collapse
                    case 1:
                        m_Player.m_DamageTracker.m_Collapsed = true;

                        closeGump = false;
                    break;

                    //Clear Results
                    case 2:
                        m_Player.m_DamageTracker.ClearResults();
                        m_Player.SendMessage("Damage Tracker results and time elapsed reset.");

                        closeGump = false;
                    break;

                    //Start and Stop
                    case 3:
                        if (m_Player.m_DamageTracker.m_Running)
                        {
                            m_Player.m_DamageTracker.StopTimer();
                            m_Player.SendMessage("Damage Tracker paused.");
                        }

                        else
                        {
                            m_Player.m_DamageTracker.StartTimer();
                            m_Player.SendMessage("Damage Tracker started.");
                        }

                        closeGump = false;
                    break;

                    //Auto-Stop                    
                    case 4:
                        m_Player.m_DamageTracker.AutoStopEnabled = !m_Player.m_DamageTracker.AutoStopEnabled;

                        string autoStopTime = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + m_Player.m_DamageTracker.AutoStopDuration, false, false, true, true, true);

                        if (m_Player.m_DamageTracker.AutoStopEnabled)
                            m_Player.SendMessage("Damage Tracker will now automatically stop after " + autoStopTime + " has elapsed.");

                        else
                            m_Player.SendMessage("Damage Tracker will now run freely.");

                        closeGump = false;
                    break;

                    //Auto-Stop: Add 1 Hour                   
                    case 5:
                        m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration + TimeSpan.FromMinutes(60);

                        closeGump = false;
                    break;

                    //Auto-Stop: Remove 1 Hour                   
                    case 6:
                        if (m_Player.m_DamageTracker.AutoStopDuration.Hours > 0)
                        {
                            m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration - TimeSpan.FromMinutes(60);

                            if (m_Player.m_DamageTracker.AutoStopDuration < DamageTracker.AutoStopMinimumDuration)
                                m_Player.m_DamageTracker.AutoStopDuration = DamageTracker.AutoStopMinimumDuration;
                        }

                        closeGump = false;
                    break;

                    //Auto-Stop: Add 1 Minute                 
                    case 7:
                        m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration + TimeSpan.FromMinutes(1);

                        closeGump = false;
                    break;

                    //Auto-Stop: Remove 1 Minute                   
                    case 8:
                        if (m_Player.m_DamageTracker.AutoStopDuration.Minutes > 0 || m_Player.m_DamageTracker.AutoStopDuration.Hours > 0)
                        {
                            m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration - TimeSpan.FromMinutes(1);

                            if (m_Player.m_DamageTracker.AutoStopDuration < DamageTracker.AutoStopMinimumDuration)
                                m_Player.m_DamageTracker.AutoStopDuration = DamageTracker.AutoStopMinimumDuration;
                        }

                        closeGump = false;
                    break;

                    //Auto-Stop: Add 5 Seconds                 
                    case 9:
                        m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration + TimeSpan.FromSeconds(5);

                        closeGump = false;
                    break;

                    //Auto-Stop: Remove 5 Seconds                   
                    case 10:
                        if (m_Player.m_DamageTracker.AutoStopDuration.Seconds > 5 || m_Player.m_DamageTracker.AutoStopDuration.Minutes > 0 || m_Player.m_DamageTracker.AutoStopDuration.Hours > 0)
                        {
                            m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration - TimeSpan.FromSeconds(5);

                            if (m_Player.m_DamageTracker.AutoStopDuration < DamageTracker.AutoStopMinimumDuration)
                                m_Player.m_DamageTracker.AutoStopDuration = DamageTracker.AutoStopMinimumDuration;                          
                        }

                        closeGump = false;
                    break;

                    //Add to Total: Melee                    
                    case 20:
                        m_Player.m_DamageTracker.AddMeleeDamageToTotal = !m_Player.m_DamageTracker.AddMeleeDamageToTotal;

                        if (m_Player.m_DamageTracker.AddMeleeDamageToTotal)
                            m_Player.SendMessage("Your melee damage inflicted will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Your melee damage inflicted will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Spell
                    case 21:
                        m_Player.m_DamageTracker.AddSpellDamageToTotal = !m_Player.m_DamageTracker.AddSpellDamageToTotal;

                        if (m_Player.m_DamageTracker.AddSpellDamageToTotal)
                            m_Player.SendMessage("Your spell damage inflicted will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Your spell damage inflicted will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Poison
                    case 22:
                        m_Player.m_DamageTracker.AddPoisonDamageToTotal = !m_Player.m_DamageTracker.AddPoisonDamageToTotal;

                        if (m_Player.m_DamageTracker.AddPoisonDamageToTotal)
                            m_Player.SendMessage("Your poison damage inflicted will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Your poison damage inflicted will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Provocation
                    case 23:
                        m_Player.m_DamageTracker.AddProvocationDamageToTotal = !m_Player.m_DamageTracker.AddProvocationDamageToTotal;

                        if (m_Player.m_DamageTracker.AddPoisonDamageToTotal)
                            m_Player.SendMessage("Damage caused by your provoked creatures will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Damaged caused by your provoked creatures will no longer be added to Total damage.");

                        closeGump = false;
                    break;

                    //Add to Total: Follower
                    case 24:
                        m_Player.m_DamageTracker.AddFollowerDamageToTotal = !m_Player.m_DamageTracker.AddFollowerDamageToTotal;

                        if (m_Player.m_DamageTracker.AddPoisonDamageToTotal)
                            m_Player.SendMessage("Damage caused by your followers will now be added to Total damage.");

                        else
                            m_Player.SendMessage("Damage caused by your followers will no longer be added to Total damage.");

                        closeGump = false;
                    break;                    
                }
            }
            
            if (!closeGump)
            {
                m_Player.CloseGump(typeof(DamageTrackerGump));
                m_Player.SendGump(new DamageTrackerGump(m_Player));
            }
        }
    }
}
