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
            FollowerDamage,
            ProvocationDamage,
            PoisonDamage,
            DamageTaken,
            FollowerDamageTaken,
            HealingDealt
        }

        public PlayerMobile m_Player;
        public bool m_Running = false;
        
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
        public bool AddFollowerDamageToTotal = true;
        public bool AddProvocationDamageToTotal = true;
        public bool AddPoisonDamageToTotal = true;

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

        public static void RecordDamage(Mobile mobile, Mobile from, Mobile target, DamageType damageType, int amount)
        {
            PlayerMobile player = mobile as PlayerMobile;

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
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null) return;
            if (m_Player.m_DamageTracker == null) return;

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Collapse + Expand
                case 1:
                    closeGump = false;
                break;

                //Clear Results
                case 2:
                    m_Player.m_DamageTracker.ClearResults();

                    closeGump = false;
                break;

                //Stop
                case 3:
                    m_Player.m_DamageTracker.StopTimer();

                    closeGump = false;
                break;

                //Start
                case 4:
                    m_Player.m_DamageTracker.StartTimer();

                    closeGump = false;
                break;

                //Auto-Stop                    
                case 5:
                    m_Player.m_DamageTracker.AutoStopEnabled = !m_Player.m_DamageTracker.AutoStopEnabled;

                    closeGump = false;
                break;

                //Auto-Stop: Add 10 Minutes                   
                case 6:
                    m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration.Add(TimeSpan.FromMinutes(10));

                    closeGump = false;
                break;

                //Auto-Stop: Remove 10 Minutes                   
                case 7:
                    m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration.Subtract(TimeSpan.FromMinutes(10));

                    if (m_Player.m_DamageTracker.AutoStopDuration < DamageTracker.AutoStopMinimumDuration)
                        m_Player.m_DamageTracker.AutoStopDuration = DamageTracker.AutoStopMinimumDuration;

                    closeGump = false;
                break;

                //Auto-Stop: Add 1 Minute                 
                case 8:
                    m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration.Add(TimeSpan.FromMinutes(1));

                    closeGump = false;
                break;

                //Auto-Stop: Remove 1 Minute                   
                case 9:
                    m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration.Subtract(TimeSpan.FromMinutes(1));

                    if (m_Player.m_DamageTracker.AutoStopDuration < DamageTracker.AutoStopMinimumDuration)
                        m_Player.m_DamageTracker.AutoStopDuration = DamageTracker.AutoStopMinimumDuration;

                    closeGump = false;
                break;

                //Auto-Stop: Add 5 Seconds                 
                case 10:
                    m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration.Add(TimeSpan.FromSeconds(5));

                    closeGump = false;
                break;

                //Auto-Stop: Remove 5 Seconds                   
                case 11:
                    m_Player.m_DamageTracker.AutoStopDuration = m_Player.m_DamageTracker.AutoStopDuration.Subtract(TimeSpan.FromSeconds(5));

                    if (m_Player.m_DamageTracker.AutoStopDuration < DamageTracker.AutoStopMinimumDuration)
                        m_Player.m_DamageTracker.AutoStopDuration = DamageTracker.AutoStopMinimumDuration;

                    closeGump = false;
                break;

                //Add to Total: Melee                    
                case 20:
                    m_Player.m_DamageTracker.AddMeleeDamageToTotal = !m_Player.m_DamageTracker.AddMeleeDamageToTotal;

                    closeGump = false;
                break;

                //Add to Total: Spell
                case 21:
                    m_Player.m_DamageTracker.AddSpellDamageToTotal = !m_Player.m_DamageTracker.AddSpellDamageToTotal;

                    closeGump = false;
                break;

                //Add to Total: Follower
                case 22:
                    m_Player.m_DamageTracker.AddFollowerDamageToTotal = !m_Player.m_DamageTracker.AddFollowerDamageToTotal;

                    closeGump = false;
                break;

                //Add to Total: Provocation
                case 23:
                    m_Player.m_DamageTracker.AddProvocationDamageToTotal = !m_Player.m_DamageTracker.AddProvocationDamageToTotal;

                    closeGump = false;
                break;

                //Add to Total: Poison
                case 24:
                    m_Player.m_DamageTracker.AddPoisonDamageToTotal = !m_Player.m_DamageTracker.AddPoisonDamageToTotal;

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(DamageTrackerGump));
                m_Player.SendGump(new DamageTrackerGump(m_Player));
            }
        }
    }
}
