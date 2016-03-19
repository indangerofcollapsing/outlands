using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Engines.Help;
using Server.Engines.ConPVP;
using Server.ContextMenus;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Bushido;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Regions;
using Server.Accounting;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Spells.Spellweaving;
using Server.Engines.PartySystem;
using Server.Commands;
using Server.Achievements;
using Server.Custom;
using Server.SkillHandlers;
using Server.ArenaSystem;
using Server.Custom.Battlegrounds;
using Server.Custom.Battlegrounds.Regions;
using Server.Guilds;
using Server.Custom.Items.Totem;
using Server.Custom.Townsystem;
using Server.Custom.Regions;
using Server.Poker;
using System.Text;
// ~DungeonMiningSystem

namespace Server.Mobiles
{
    #region Enums
    [Flags]
    public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
    {
        None = 0x00000000,
        Glassblowing = 0x00000001,
        Masonry = 0x00000002,
        SandMining = 0x00000004,
        StoneMining = 0x00000008,
        ToggleMiningStone = 0x00000010,
        KarmaLocked = 0x00000020,
        AutoRenewInsurance = 0x00000040,
        UseOwnFilter = 0x00000080,
        PublicMyRunUO = 0x00000100,
        PagingSquelched = 0x00000200,
        Young = 0x00000400,
        AcceptGuildInvites = 0x00000800,
        DisplayChampionTitle = 0x00001000,
        HasStatReward = 0x00002000,
        RefuseTrades = 0x00004000,
        Paladin = 0x00010000,
        KilledByPaladin = 0x00020000,
        YewJailed = 0x00040000,
        BoatMovement = 0x00080000
    }

    public enum NpcGuild
    {
        None,
        MagesGuild,
        WarriorsGuild,
        ThievesGuild,
        RangersGuild,
        HealersGuild,
        MinersGuild,
        MerchantsGuild,
        TinkersGuild,
        TailorsGuild,
        FishermensGuild,
        BardsGuild,
        BlacksmithsGuild,
        DetectivesGuild
    }

    public enum SolenFriendship
    {
        None,
        Red,
        Black
    }

    public enum BlockMountType
    {
        None = -1,
        Dazed = 1040024,
        BolaRecovery = 1062910,
        DismountRecovery = 1070859
    }

    // Set Bonuses
    [Flags]
    public enum SetBonus
    {
        None = 0,
        Mage = 1,
        Warrior = 1 << 1,
        HythlothTier1 = 1 << 2,
        HythlothTier2 = 1 << 3,
        HythlothTier3 = 1 << 4,
        HythlothTier4 = 1 << 5,
        DestardTier1 = 1 << 6,
        DestardTier2 = 1 << 7,
        DestardTier3 = 1 << 8,
        DestardTier4 = 1 << 9,
        DeceitTier1 = 1 << 10,
        DeceitTier2 = 1 << 11,
        DeceitTier3 = 1 << 12,
        DeceitTier4 = 1 << 13,
        ShameTier1 = 1 << 14,
        ShameTier2 = 1 << 15,
        ShameTier3 = 1 << 16,
        ShameTier4 = 1 << 17,
    }

    public enum DamageDisplayMode
    {
        None,
        PrivateMessage,
        PrivateOverhead
    }

    public enum StealthStepsDisplayMode
    {
        None,
        PrivateMessage,
        PrivateOverhead
    }

    public enum HenchmenSpeechDisplayMode
    {
        Normal,
        Infrequent,
        IdleOnly,
        IdleOnlyInfrequent,
        CombatOnly,
        CombatOnlyInfrequent,
        None
    }

    public enum PeacemakingModeEnum
    {
        Combat,
        CrowdControl
    }

    #endregion

    public partial class PlayerMobile : Mobile, IHonorTarget
    {
        private class GhostScoutingTimer : Timer
        {
            private PlayerMobile m_Player;
            private static List<string> ExcludedDungeons = new List<string>() {
                "Khaldun",
                "The Painted Caves",
                "Terathan Keep",
            };

            public GhostScoutingTimer(PlayerMobile player) : base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.OneMinute;
                m_Player = player;
            }

            protected override void OnTick()
            {
                base.OnTick();
                if (m_Player == null || m_Player.Deleted || m_Player.Alive)
                {
                    Stop();
                    return;
                }

                if (m_Player.Region is DungeonRegion)
                {
                    bool corpseInDungeon = false;
                    if (m_Player.Corpse != null && !m_Player.Corpse.Deleted)
                    {
                        // check if corpse in dungeon
                        var region = Region.Find(m_Player.Corpse.Location, m_Player.Corpse.Map);
                        corpseInDungeon = region is DungeonRegion;
                    }

                    if (corpseInDungeon || (m_Player.Region is FireDungeonRegion) || ExcludedDungeons.Contains(m_Player.Region.Name)) return;

                    var dungeon = m_Player.Region as DungeonRegion;

                    if (dungeon.EntranceLocation != Point3D.Zero)
                    {
                        m_Player.MoveToWorld(dungeon.EntranceLocation, Map.Felucca);
                    }
                    else
                    {
                        m_Player.MoveToWorld(new Point3D(1484, 1612, 20), Map.Felucca); // britain healer
                    }
                    m_Player.SendMessage("You have been ejected from the dungeon.");
                }
            }
        }

        ///////////////////////////////////////////////
        // IPY ADDITIONS 
        ///////////////////////////////////////////////

        public static void PlayerCountCommand(CommandEventArgs e)
        {
            e.Mobile.SendMessage(string.Format("{0} online", Server.RemoteAdmin.ServerInfo.NetStateCount()));
        }

        public static void SetThresholdCommand(CommandEventArgs e)
        {
            double multip = Server.RemoteAdmin.ServerInfo.Multiplier;
            double.TryParse(e.ArgString, out multip);
            Server.RemoteAdmin.ServerInfo.Multiplier = multip;
            e.Mobile.SendMessage(string.Format("Multiplier has been set to {0}", multip));
        }

        public static void ToggleThresholdCommand(CommandEventArgs e)
        {
            Server.RemoteAdmin.ServerInfo.SpoofPlayerCount = !Server.RemoteAdmin.ServerInfo.SpoofPlayerCount;
            e.Mobile.SendMessage("Threshold has been {0}.", Server.RemoteAdmin.ServerInfo.SpoofPlayerCount ? "enabled" : "disabled");
        }
        
        public static void GoToEntranceCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;
            var region = player.Region as DungeonRegion;

            if (region != null)
            {
                player.MoveToWorld(region.EntranceLocation, Map.Felucca);
            }
        }

        public TimeSpan PowerHourDuration { get { return TimeSpan.FromHours(1) + PowerHourBonus; } }

        private TimeSpan m_PowerHourBonus = TimeSpan.Zero;
        public TimeSpan PowerHourBonus { get { return m_PowerHourBonus; } set { m_PowerHourBonus = value; } }

        public void BoostPowerHourDuration()
        {
            if (NetState != null)
            {
                var timeOnline = m_BankGameTime + (DateTime.UtcNow - SessionStart);
                m_PowerHourBonus = TimeSpan.FromMinutes(Math.Min(5 * timeOnline.TotalHours, 120));
                m_BankGameTime = TimeSpan.Zero;
            }
        }

        [Usage("ShowTownChat")]
        [Description("Toggles Town Chat On and Off")]
        public static void ShowTownChatCommand(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player.ShowTownChat)
            {
                player.ShowTownChat = false;
                player.SendMessage("You will no longer see town alliance chat.");
            }
            else
            {
                player.ShowTownChat = true;
                player.SendMessage("You will now see town alliance chat.");
            }
        }

        [Usage("ShowMeleeDamage")]
        [Description("Cycles between Display Modes of Player Melee Damage")]
        public static void ShowMeleeDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowMeleeDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowMeleeDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting player melee damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowMeleeDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting player melee damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowMeleeDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting player melee damage display mode: None");
                    break;
            }
        }

        [Usage("ShowSpellDamage")]
        [Description("Cycles between Display Modes of Player Spell Damage")]
        public static void ShowSpellDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowSpellDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowSpellDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting player spell damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowSpellDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting player spell damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowSpellDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting player spell damage display mode: None");
                    break;
            }
        }

        [Usage("ShowFollowerDamage")]
        [Description("Cycles between Display Modes of Player's Followers Damage")]
        public static void ShowFollowerDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowFollowerDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowFollowerDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting follower damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowFollowerDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting follower damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowFollowerDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting follower damage display mode: None");
                    break;
            }
        }

        [Usage("ShowProvokeDamage")]
        [Description("Cycles between Display Modes of Player's Provoked Creature Damage")]
        public static void ShowProvocationDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowProvocationDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowProvocationDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting provoked creature damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowProvocationDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting provoked creature damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowProvocationDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting provoked creature damage display mode: None");
                    break;
            }
        }

        [Usage("ShowPoisonDamage")]
        [Description("Cycles between Display Modes of Player's Provoked Creature Damage")]
        public static void ShowPoisonDamage(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowPoisonDamage)
            {
                case DamageDisplayMode.None:
                    player.m_ShowPoisonDamage = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting poison damage display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowPoisonDamage = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting poison damage display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowPoisonDamage = DamageDisplayMode.None;
                    player.SendMessage("Setting poison damage display mode: None");
                    break;
            }
        }

        [Usage("ShowDamageTaken")]
        [Description("Cycles between Display Modes of Player's Damage They Take")]
        public static void ShowDamageTaken(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowDamageTaken)
            {
                case DamageDisplayMode.None:
                    player.m_ShowDamageTaken = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting damage taken display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowDamageTaken = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting damage taken display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowDamageTaken = DamageDisplayMode.None;
                    player.SendMessage("Setting damage taken display mode: None");
                    break;
            }
        }

        [Usage("ShowFollowerDamageTaken")]
        [Description("Cycles between Display Modes of Player's Followers Damage Taken")]
        public static void ShowFollowerDamageTaken(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowFollowerDamageTaken)
            {
                case DamageDisplayMode.None:
                    player.m_ShowFollowerDamageTaken = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting follower damage taken display mode: System Message");
                    break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowFollowerDamageTaken = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting follower damage taken display mode: Overhead Text");
                    break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowFollowerDamageTaken = DamageDisplayMode.None;
                    player.SendMessage("Setting follower damage taken display mode: None");
                    break;
            }
        }

        [Usage("ShowHealing")]
        [Description("Cycles between Display Modes of Healing Effects")]
        public static void ShowHealing(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_ShowHealing)
            {
                case DamageDisplayMode.None:
                    player.m_ShowHealing = DamageDisplayMode.PrivateMessage;
                    player.SendMessage("Setting healing display mode: System Message");
                break;

                case DamageDisplayMode.PrivateMessage:
                    player.m_ShowHealing = DamageDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting healing display mode: Overhead Text");
                break;

                case DamageDisplayMode.PrivateOverhead:
                    player.m_ShowHealing = DamageDisplayMode.None;
                    player.SendMessage("Setting healing display mode: None");
                break;
            }
        }

        [Usage("ShowStealthSteps")]
        [Description("Cycles between Display Modes of Player's Stealth Steps Feedback")]
        public static void ShowStealthSteps(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_StealthStepsDisplayMode)
            {
                case StealthStepsDisplayMode.None:
                    player.m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateMessage;
                    player.SendMessage("Setting stealth steps display mode: System Message");
                break;

                case StealthStepsDisplayMode.PrivateMessage:
                    player.m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateOverhead;
                    player.SendMessage("Setting stealth steps display mode: Overhead Text");
                break;

                case StealthStepsDisplayMode.PrivateOverhead:
                    player.m_StealthStepsDisplayMode = StealthStepsDisplayMode.None;
                    player.SendMessage("Setting stealth steps display mode: None");
                break;
            }
        }

        [Usage("ShowHenchmenSpeech")]
        [Description("Cycles between Modes of Controlled Henchmen Speech")]
        public static void ShowHenchmenSpeech(CommandEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            switch (player.m_HenchmenSpeechDisplayMode)
            {
                case HenchmenSpeechDisplayMode.Normal:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Infrequent;
                    player.SendMessage("Setting henchmen speech mode: Infrequent");
                break;

                case HenchmenSpeechDisplayMode.Infrequent:
                player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.IdleOnly;
                    player.SendMessage("Setting henchmen speech mode: Idle Only");
                break;

                case HenchmenSpeechDisplayMode.IdleOnly:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.IdleOnlyInfrequent;
                    player.SendMessage("Setting henchmen speech mode: Idle Only - Infrequent");
                break;

                case HenchmenSpeechDisplayMode.IdleOnlyInfrequent:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.CombatOnly;
                    player.SendMessage("Setting henchmen speech mode: Combat Only");
                break;

                case HenchmenSpeechDisplayMode.CombatOnly:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.CombatOnlyInfrequent;
                    player.SendMessage("Setting henchmen speech mode: Combat Only - Infrequent");
                break;

                case HenchmenSpeechDisplayMode.CombatOnlyInfrequent:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.None;
                    player.SendMessage("Setting henchmen speech mode: None");
                break;

                case HenchmenSpeechDisplayMode.None:
                    player.m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Normal;
                    player.SendMessage("Setting henchmen speech mode: Normal");
                break;
            }
        }

        [Usage("ShowAdminTextFilter")]
        [Description("Toggles between Text Filter Modes")]
        public static void ShowAdminTextFilter(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm.m_ShowAdminFilterText == true)
            {
                pm.SendMessage("Admin Text Filter is now disabled.");
                pm.m_ShowAdminFilterText = false;
            }

            else
            {
                pm.SendMessage("Admin Text Filter is now enabled.");
                pm.m_ShowAdminFilterText = true;
            }
        }

        [Usage("AutoStealth")]
        [Description("Toggles between AutoStealth Modes")]
        public static void ToggleAutoStealth(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm.m_AutoStealth == true)
            {
                pm.SendMessage("Auto-Stealth is now disabled.");
                pm.m_AutoStealth = false;
            }

            else
            {
                pm.SendMessage("Auto-Stealth is now enabled.");
                pm.m_AutoStealth = true;
            }
        }

        [Usage("FireDungeonTimer")]
        [Description("Displays the time until the player can attempt fire again.")]
        public static void FireDungeonTimer_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            pm.Say(String.Format("{0}", pm.NextFireAttempt));
        }

        [Usage("WipePlayerMobiles")]
        [Description("Changes the password of the commanding players account. Requires the same C-class IP address as the account's creator.")]
        public static void WipeAllPlayerMobiles_OnCommand(CommandEventArgs e)
        {
            List<PlayerMobile> to_delete = new List<PlayerMobile>();
            foreach (Mobile m in World.Mobiles.Values)
            {
                PlayerMobile pm = m as PlayerMobile;
                if (pm != null && pm.AccessLevel == AccessLevel.Player)
                {
                    to_delete.Add(pm);
                }
            }
            foreach (PlayerMobile p in to_delete)
            {
                p.Delete();
            }
        }

        DateTime m_LastTrapPouchUse = DateTime.UtcNow;
        [Usage("UseTrappedPouch")]
        [Description("Uses a trapped pouch in your backpack")]
        public static void UseTrappedPouch_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;
            if (pm != null && pm.Backpack != null)
            {
                if (pm.m_LastTrapPouchUse + TimeSpan.FromSeconds(0.75) > DateTime.UtcNow)
                {
                    pm.SendMessage("You must wait 0.75 seconds between each use of this command");
                }
                else
                {
                    List<TrapableContainer> tcs = pm.Backpack.FindItemsByType<TrapableContainer>();
                    foreach (TrapableContainer tc in tcs)
                    {
                        if (tc != null && tc.TrapType == TrapType.MagicTrap)
                        {
                            tc.Open(pm);
                            Target.Cancel(pm);
                            pm.m_LastTrapPouchUse = DateTime.UtcNow;
                            return;
                        }
                    }
                }
            }
        }

        public enum ArenaPreferenceKeys
        {
            TeamSelection,
            EraSelection,
            RulesetSelection,
            OptionsSelection,
        }

        public Dictionary<ArenaPreferenceKeys, int> ArenaPreferences = new Dictionary<ArenaPreferenceKeys, int>()
        {
            { ArenaPreferenceKeys.TeamSelection, 0 },
            { ArenaPreferenceKeys.EraSelection, 0 },
            { ArenaPreferenceKeys.RulesetSelection, 0 },
            { ArenaPreferenceKeys.OptionsSelection, 0 },
        };


        // Passive taming skill gains - when your pet attacks a monster you have a chance of gaining animal taming.
        public DateTime m_LastPassiveTamingSkillGain = DateTime.MinValue;
        public BaseCreature m_LastPassiveTamingSkillAttacked; // the controlled pets last target
        public BaseCreature m_LastPassiveExpAttacked; // the controlled pets last target for XP Gain Purposes

        public int MurderCountDecayHours = 48;

        public static int DamageEntryClaimExpiration = 120;

        public static int MinDamageRequiredForPlayerDeath = 25;
        public static int MinDamageRequiredForPaladinDeath = 25;
        public static int MinDamageRequiredForMurdererDeath = 25;
        public static int MinIndividualDamageRequiredForDeathClaim = 10;

        public int ItemsNotCraftedBySold = 0;
        public DateTime ResetItemsNotCraftedByDateTime;

        public List<PlayerClassItemRansomEntry> m_PlayerClassRansomedItemsAvailable = new List<PlayerClassItemRansomEntry>();

        // UOAC deathcam
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool IsDeathCam { get; set; }

        public DamageDisplayMode m_ShowMeleeDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowSpellDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowFollowerDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowProvocationDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowPoisonDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowDamageTaken = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowFollowerDamageTaken = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowHealing = DamageDisplayMode.None;

        public StealthStepsDisplayMode m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateMessage;
        public HenchmenSpeechDisplayMode m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Normal;
        public bool m_ShowAdminFilterText = true; 

        public bool m_AutoStealth = true;

        private BaseBoat m_BoatOccupied = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat BoatOccupied { get { return m_BoatOccupied; } set { m_BoatOccupied = value; } }

        public int PlayerMeleeDamageTextHue = 0x022; //Red
        public int PlayerSpellDamageTextHue = 0x075; //Purple
        public int PlayerFollowerDamageTextHue = 0x59; //Blue  
        public int PlayerProvocationDamageTextHue = 0x90; //Orange
        public int PlayerPoisonDamageTextHue = 0x03F; //Green
        public int PlayerDamageTakenTextHue = 0; //White
        public int PlayerFollowerDamageTakenTextHue = 0; //White
        public int PlayerHealingTextHue = 2213; //Yellow

        private bool m_DamageVulnerable = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DamageVulnerable { get { return m_DamageVulnerable; } set { m_DamageVulnerable = value; } }

        public PlayerEnhancementAccountEntry m_PlayerEnhancementAccountEntry = null;
        public InfluenceAccountEntry m_InfluenceAccountEntry = null;
        public UOACZAccountEntry m_UOACZAccountEntry = null;

        public override bool KeepsItemsOnDeath { get { return (AccessLevel > AccessLevel.Player || Region is UOACZRegion); } }

        public DateTime NextEmoteAllowed = DateTime.UtcNow;
        public static TimeSpan EmoteCooldownLong = TimeSpan.FromSeconds(120);
        public static TimeSpan EmoteCooldownShort = TimeSpan.FromSeconds(30);

        public EventCalendarAccount m_EventCalendarAccount = null;
        public MHSPlayerEntry m_MHSPlayerEntry = null;
        public WorldChatAccountEntry m_WorldChatAccountEntry = null;        

        public static int SkillCap = 7000;
        public static int MaxBonusSkillCap = 200;

        private int m_BonusSkillCap = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BonusSkillCap
        {
            get { return m_BonusSkillCap; }
            set
            {
                m_BonusSkillCap = value;

                if (m_BonusSkillCap > MaxBonusSkillCap)
                    m_BonusSkillCap = MaxBonusSkillCap;

                SkillCap = SkillCap + m_BonusSkillCap;
            }
        }

        private DateTime m_TinkerTrapPlacementWindow = DateTime.MinValue;
        [CommandProperty(AccessLevel.Counselor)]
        public DateTime TinkerTrapPlacementWindow
        {
            get { return m_TinkerTrapPlacementWindow; }
            set { m_TinkerTrapPlacementWindow = value; }
        }

        private int m_TinkerTrapsPlaced = 0;
        [CommandProperty(AccessLevel.Counselor)]
        public int TinkerTrapsPlaced
        {
            get { return m_TinkerTrapsPlaced; }
            set { m_TinkerTrapsPlaced = value; }
        }

        //Overload Protection: Track Player "Spammable Actions" That Might Overload Server if Done Too Frequently
        public int SystemOverloadActions = 0;       
        public static int SystemOverloadActionThreshold = 180; //Player flagged if attacking a single target this many times over the SystemOverloadInterval
        public static TimeSpan SystemOverloadInterval = TimeSpan.FromSeconds(60); 

        // UOAC siege BGs
        private bool m_IsDragging = false;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool IsDragging 
        { 
            get { return m_IsDragging; } 
            set 
            { 
                m_IsDragging = value;
                if (value)
                    Send(SpeedControl.WalkSpeed);
                else
                    Send(SpeedControl.Disable);
            } 
        }

        public override bool AllowTrades { get { return !(Region is ArenaSpectatorRegion); } }

        public DateTime LastTeamSwitch = DateTime.MinValue;

        public class SpectatorTimer : Timer
        {
            PlayerMobile m_From;
            public SpectatorTimer(PlayerMobile from)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.FiveSeconds;
                m_From = from;
            }

            protected override void OnTick()
            {
                base.OnTick();
                if (m_From != null && !m_From.Deleted && m_From.Spectating)
                {
                    m_From.SendMessage(33, "You are spectating, you can type [leave to quit at any time.");
                }
                else
                {
                    Stop();
                }
            }
        }

        public SpectatorTimer SpectatingTimer { get; set; }

        // IPY - Skillscrolls
        #region Skillscrolls
        public Skill[] LastSkillGain = new Skill[5];
        public static TimeSpan DecayLastSkillGain = TimeSpan.FromHours(2);
        public DateTime[] _StartedLastSkillGain = new DateTime[5]; // Start point of timer
        private Timer[] _TimerLastSkillGain = new Timer[5]; // Timer itself
        public void startLastSkillGainDecay(TimeSpan delay, int position)
        {
            if (this._TimerLastSkillGain[position] != null)
            {
                if (this._TimerLastSkillGain[position].Running)
                {
                    this._TimerLastSkillGain[position].Stop();
                }

                this._TimerLastSkillGain[position] = null;
            }

            this._TimerLastSkillGain[position] = Timer.DelayCall(delay, new TimerCallback(deleteLastSkillGain));
            this._StartedLastSkillGain[position] = DateTime.UtcNow;
        }

        private PokerGame m_PokerGame; //Edit for Poker System
        public PokerGame PokerGame
        {
            get { return m_PokerGame; }
            set { m_PokerGame = value; }
        }

        // print out array debug test to console
        public void printLastSkillGain()
        {
            DateTime now = DateTime.UtcNow;
            for (int n = 0; n <= this.LastSkillGain.Length - 1; n++)
            {
                if (this.LastSkillGain[n] == null)
                {
                    this.SendMessage("LastSkillGain[" + n + "] = null");
                }
                else
                {
                    this.SendMessage("LastSkillGain[" + n + "] = " + this.LastSkillGain[n].SkillName + " / Decay: " + now.Subtract(_StartedLastSkillGain[n]).ToString());
                }
            }
        }

        // Size of the array minus nulls
        public int countLastSkillGain()
        {
            int currentSize = 0;
            while (currentSize <= (this.LastSkillGain.Length - 1) && this.LastSkillGain[currentSize] != null)
            {
                currentSize++;
            }
            currentSize--;
            return currentSize;
        }

        // Timer delete lastskill item and resort
        public void deleteLastSkillGain()
        {
            int currentSize = this.countLastSkillGain();
            for (int n = 0; n <= currentSize; n++)
            {
                if (n != currentSize)
                {
                    this.LastSkillGain[n] = this.LastSkillGain[n + 1];
                }
                else
                {
                    this.LastSkillGain[n] = null;
                }
            }
        }

        public static List<SkillName> SkillPool = new List<SkillName>
        {
            SkillName.Magery,
            SkillName.Fencing,
            SkillName.Archery,
            SkillName.Wrestling,
            SkillName.Parry,
            SkillName.Swords,
            SkillName.Macing,
            SkillName.Tactics,
            SkillName.MagicResist,
            SkillName.Musicianship,
            SkillName.Provocation,
            SkillName.EvalInt,
            SkillName.Anatomy,
            SkillName.Meditation,
            SkillName.Peacemaking,
            SkillName.Healing,
            SkillName.Discordance,
            SkillName.SpiritSpeak,
            SkillName.ArmsLore,
        };

        public void addLastSkillGain(Skill skill)
        {
            //Accepted skills (Also change in SkillCheck.cs and BestCombatSkill() PlayMobile.cs)
            int currentSize = 0;
            int dupeLocation = 0;
            bool dupe = false;

            if (this.LastSkillGain[0] == null)
            {
                //First Entry made no sort/delete/duplication check needed
                this.LastSkillGain[0] = skill;
                startLastSkillGainDecay(DecayLastSkillGain, 0);
            }
            else
            {
                //Not the first entry so duplication check/deletion check/sort/add
                //Duplication check
                while (currentSize <= (this.LastSkillGain.Length - 1) && this.LastSkillGain[currentSize] != null)
                {
                    if (this.LastSkillGain[currentSize] == skill)
                    {
                        dupeLocation = currentSize;
                        dupe = true;
                    }
                    currentSize++; ;
                }

                currentSize--;

                if (dupe == true)
                {
                    for (int n = dupeLocation; n <= currentSize; n++)
                    {
                        if ((n + 1) <= currentSize)
                        {
                            this.LastSkillGain[n] = this.LastSkillGain[n + 1];

                            //if not the last record as the duplication needs to be refreshed in the else{}
                            if (n == currentSize)
                            {
                                //Reset timer due to duplication but only on the duped skill gain
                                startLastSkillGainDecay(DecayLastSkillGain, n);
                            }
                        }
                        else
                        {
                            this.LastSkillGain[n] = skill;
                            startLastSkillGainDecay(DecayLastSkillGain, n);
                        }
                    }
                }
                else
                {
                    //sort everything to add new
                    for (int n = 0; n <= (this.LastSkillGain.Length - 1); n++)
                    {
                        if (currentSize == (this.LastSkillGain.Length - 1))
                        {
                            // check to make sure the array is full or not
                            if (this.LastSkillGain[n] != null)
                            {
                                // check to make sure we are not at the end of the array as we can't n+1
                                if (n != (this.LastSkillGain.Length - 1) && (this.LastSkillGain[n + 1] != null))
                                {
                                    this.LastSkillGain[n] = this.LastSkillGain[n + 1];
                                    startLastSkillGainDecay(DecayLastSkillGain, n);
                                }
                                else
                                {
                                    // add new skill as this is the end of the array
                                    this.LastSkillGain[n] = skill;
                                    startLastSkillGainDecay(DecayLastSkillGain, 0);
                                    break;
                                }
                            }
                            else
                            {
                                this.LastSkillGain[n] = skill;
                                startLastSkillGainDecay(DecayLastSkillGain, 0);
                                break;
                            }
                        }
                        else
                        {
                            this.LastSkillGain[currentSize + 1] = skill;
                            startLastSkillGainDecay(DecayLastSkillGain, (currentSize + 1));
                        }
                    }
                }
            }
        }

        // Picks the best combat skill.
        public Skill BestCombatSkill(double maxSkill)
        {
            int length = LastSkillGain.Length;
            int start = Utility.Random(length);

            for (int i = 0; i < length; i++)
            {
                int index = (start + i) % length;
                if (LastSkillGain[index] != null && LastSkillGain[index].Base < maxSkill && LastSkillGain[index].Lock == SkillLock.Up)
                    return LastSkillGain[index];
            }

            List<Skill> choices = new List<Skill>();

            foreach (var s in SkillPool)
            {
                var skill = Skills[s];
                if (skill.Lock != SkillLock.Up)
                    continue;
                if (skill.BaseFixedPoint >= skill.CapFixedPoint || skill.Base > maxSkill || skill.BaseFixedPoint <= 150)
                    continue;

                choices.Add(skill);
            }

            return choices.Count > 0 ? choices[Utility.Random(choices.Count)] : null;
        }
        #endregion

        // IPY - Power hour / boost mode
        #region IPY Resources Over Time
        private DateTime m_PowerHourReset;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PowerHourReset
        {
            get { return m_PowerHourReset; }
            set { m_PowerHourReset = value; }
        }

        #endregion // Resources Over Time

        // IPY - Instahit	
        #region IPY Instahit
        private DateTime m_NextInstahit;
        public BaseWeapon m_LastWeaponHeld;
        private BaseWeapon instahitDefault;
        public TimeSpan m_InstahitCounter = new TimeSpan();
        public NewInstahit ni;
        public bool m_HasTimerRunning;
        private bool m_NoNewTimer;
        private DateTime m_LastSwing;
        public DateTime LastSwing
        {
            get { return m_LastSwing; }
            set { m_LastSwing = value; }
        }
        [CommandProperty(AccessLevel.Administrator)]
        public bool NoNewTimer
        {
            get { return m_NoNewTimer; }
            set { m_NoNewTimer = value; }
        }

        public BaseWeapon InstahitDefault
        {
            get { return instahitDefault; }
        }



        // PvE Metrics/tracking
        private int m_NumGoldCoinsGenerated;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NumGoldCoinsGenerated
        {
            get { return m_NumGoldCoinsGenerated; }
            set { m_NumGoldCoinsGenerated = value; }
        }

        // Custom harvesting
        #region IPY Custom harvesting
        private Item m_TempStashedHarvest = null;
        private Server.Engines.Harvest.HarvestDefinition m_TempStashedHarvestDef;
        private List<DateTime> m_FailedHarvestAttempts = new List<DateTime>();
        private bool m_HarvestLockedout = false;
        public static int s_HarvestLockoutTime = 15; // also decay time for fails
        public static int s_HarvestFailsForLockout = 10;
        public HarvestTimer m_HarvestTimer;

        public class HarvestTimer : Timer
        {
            private PlayerMobile m_Player;
            private DateTime m_Start;
            private static TimeSpan CaptchaTimeLimit = TimeSpan.FromSeconds(45);

            public HarvestTimer(PlayerMobile player, DateTime started)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(15))
            {
                Priority = TimerPriority.FiveSeconds;
                m_Player = player;
                m_Start = started;
            }

            protected override void OnTick()
            {
                if (m_Player == null || m_Player.NetState == null || m_Player.Deleted)
                {
                    Stop();
                    return;
                }

                if (DateTime.UtcNow > m_Start + CaptchaTimeLimit)
                {
                    if (m_Player.TempStashedHarvest != null && !(m_Player.TempStashedHarvest is BaseTreasureChest))
                        m_Player.TempStashedHarvest.Delete();

                    m_Player.TempStashedHarvest = null;
                    m_Player.SendMessage(33, "You have taken too long to correctly answer the captcha, your harvest is lost");
                    m_Player.FailedHarvestAttempts.Add(DateTime.UtcNow);
                    m_Player.CloseGump(typeof(Server.Custom.AntiRailing.HarvestGump));
                    Stop();
                }
            }
        }

        public bool HarvestLockedout
        {
            get
            {
                RefreshFailedHarvests();
                if (m_HarvestLockedout)
                {
                    // move to jail
                    LastLocation = new Point3D(Location);
                    Location = new Point3D(5274, 1164, 0);
                    SendMessage(0x22, "You have been moved to jail for failing the harvest captcha.");
                    FailedHarvestAttempts.Clear();
                    if (Account != null && Account is Account)
                    {
                        (Account as Account).Comments.Add(new AccountComment("Harvest System", string.Format("{0} failed the harvest captcha.", Name)));
                        Server.Commands.CommandHandlers.BroadcastMessage(AccessLevel.GameMaster, 0x482, String.Format("{0} has been jailed for failing the captcha, please check in with them.", Name));
                    }
                    if (m_HarvestTimer != null && m_HarvestTimer.Running)
                        m_HarvestTimer.Stop();
                }
                return m_HarvestLockedout;
            }
        }

        public List<DateTime> FailedHarvestAttempts
        {
            get { return m_FailedHarvestAttempts; }
            set { m_FailedHarvestAttempts = value; }
        }
        public Server.Engines.Harvest.HarvestDefinition TempStashedHarvestDef
        {
            get { return m_TempStashedHarvestDef; }
            set { m_TempStashedHarvestDef = value; }
        }

        private void RefreshFailedHarvests()
        {
            // refresh timers
            bool already_locked_out = m_FailedHarvestAttempts.Count > s_HarvestFailsForLockout;
            DateTime latest = DateTime.UtcNow.AddMinutes(-s_HarvestLockoutTime * (already_locked_out ? 2 : 1)); // 30 min decay time once you get locked out, 15m otherwise
            m_FailedHarvestAttempts.RemoveAll(elem => (elem < latest));
            m_HarvestLockedout = m_FailedHarvestAttempts.Count >= s_HarvestFailsForLockout;
        }

        public Item TempStashedHarvest
        {
            get { return m_TempStashedHarvest; }
            set
            {
                if (value != null)
                {
                    m_TempStashedHarvest = value;
                    this.SendGump((new Server.Custom.AntiRailing.HarvestGump(this)));
                    if (m_HarvestTimer != null && m_HarvestTimer.Running)
                        m_HarvestTimer.Stop();

                    m_HarvestTimer = new HarvestTimer(this, DateTime.UtcNow);
                    m_HarvestTimer.Start();
                }
            }
        }
        #endregion             

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextInstahit
        {
            get
            {
                TimeSpan ts = m_NextInstahit - DateTime.UtcNow;
                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;
                return ts;
            }
            set
            {
                try { m_NextInstahit = DateTime.UtcNow + value; }
                catch { }
            }
        }

        public static void CompareInstahit(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            BaseWeapon weapon = from.Weapon as BaseWeapon;

            pm.m_InstahitCounter += TimeSpan.FromSeconds(1);
        }

        public class NewInstahit : Timer
        {
            private Mobile from;

            public NewInstahit(Mobile m)
                : base(TimeSpan.FromSeconds(0))
            {
                Priority = TimerPriority.OneSecond;
                from = m;

                if (!this.Running)
                    this.Start();
            }

            protected override void OnTick()
            {
                PlayerMobile pm = from as PlayerMobile;

                this.Start();

                pm.m_HasTimerRunning = true;
                //pm.Say( "" + pm.m_InstahitCounter.ToString() );
                CompareInstahit(pm);
            }
        }
        #endregion

        // IPY - Player Flags
        #region Player flags
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Paladin
        {
            get { return GetFlag(PlayerFlag.Paladin); }
            set { SetFlag(PlayerFlag.Paladin, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Murderer
        {
            get
            {
                return (ShortTermMurders >= 5);
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Pirate
        {
            get
            {
                return true;
            }
        }

        private PlayerMobile m_LastPlayerKilledBy;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile LastPlayerKilledBy
        {
            get { return m_LastPlayerKilledBy; }
            set { m_LastPlayerKilledBy = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool KilledByPaladin
        {
            get { return GetFlag(PlayerFlag.KilledByPaladin); }
            set { SetFlag(PlayerFlag.KilledByPaladin, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool YewJailed
        {
            get { return GetFlag(PlayerFlag.YewJailed); }
            set { SetFlag(PlayerFlag.YewJailed, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BoatMovement
        {
            get { return GetFlag(PlayerFlag.BoatMovement); }
            set { SetFlag(PlayerFlag.BoatMovement, value); }
        }
        #endregion

        // IPY - Achievements
        #region achievements
        private static void CheckAccountAgeAchievements(object mobile)
        {
            if (mobile is PlayerMobile)
            {
                PlayerMobile pm = mobile as PlayerMobile;
                if (pm.Deleted)
                    return;
                TimeSpan diff = DateTime.UtcNow - ((Account)pm.Account).Created;

                if (diff.TotalDays >= 365.0)
                    AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_365dayOldAccount);
                if (diff.TotalDays >= 180.0)
                    AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_180dayOldAccount);
                if (diff.TotalDays >= 90.0)
                    AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_90dayOldAccount);
                if (diff.TotalDays >= 30.0)
                    AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_30dayOldAccount);
                if (diff.TotalDays >= 7.0)
                    AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_7dayOldAccount);
                if (diff.TotalDays >= 1.0)
                    AchievementSystem.Instance.TickProgress(pm, AchievementTriggers.Trigger_1dayOldAccount);
            }

        }
        #endregion
        // IPY - Misc
        #region IPY Misc

        public PlayerTitleColors TitleColorState { get; set; }
        public int SelectedTitleColorIndex;
        public EColorRarity SelectedTitleColorRarity;
        private DateTime m_LastDeathByPlayer;
        private int m_CanReprieve;
        private bool CanReprieveBool = false;
        public TimeSpan m_TimeSpanDied;
        public DateTime m_DateTimeDied;
        public TimeSpan m_TimeSpanResurrected;
        public YewJail.YewJailItem m_YewJailItem;
        public List<string> PreviousNames { get; set; }
        public DateTime HueModEnd { get; set; }
        public TimeSpan LoginElapsedTime { get; set; }
        public bool m_UserOptHideFameTitles;
        private DateTime m_Created;
        public DateTime CreatedOn { set { m_Created = value; } get { return m_Created; } }
        public Boolean CloseBankRunebookGump;

        public List<VengeanceEntry> m_VengeanceEntries = new List<VengeanceEntry>();

        public List<SpellEntry> m_SpellEntries = new List<SpellEntry>();
              
        public List<Mobile> m_ShortTermMurders;
        public List<Mobile> m_PaladinsKilled;

        private int m_UniqueMurders = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UniqueMurders
        {
            get { return m_UniqueMurders; }
            set { m_UniqueMurders = value; }
        }

        public Dictionary<PlayerMobile, DateTime> DictUniqueMurderEntries = new Dictionary<PlayerMobile, DateTime>();

        public TimeSpan m_ShortTermElapse;
        public TimeSpan m_LongTermElapse;
        public TimeSpan m_minOrderJoinTime;
        
        private DateTime m_PaladinRejoinAllowed = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PaladinRejoinAllowed
        {
            get { return m_PaladinRejoinAllowed; }
            set { m_PaladinRejoinAllowed = value; }
        }

        private DateTime m_PaladinProbationExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PaladinProbationExpiration
        {
            get { return m_PaladinProbationExpiration; }
            set { m_PaladinProbationExpiration = value; }
        }

        private DateTime m_PenanceExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PenanceExpiration
        {
            get { return m_PenanceExpiration; }
            set { m_PenanceExpiration = value; }
        }

        private bool m_IsInTempStatLoss = false;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public bool IsInTempStatLoss
        {
            get { return m_IsInTempStatLoss; }
            set { m_IsInTempStatLoss = value; }
        }

        public bool IsInUOACZ
        {
            get
            {
                if (!(Region is UOACZRegion))
                    return false;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);
                return m_UOACZAccountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.None;
            }
        }

        public bool IsUOACZHuman
        {
            get
            {
                UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

                if (!(Region is UOACZRegion))
                    return false;

                return m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human;
            }
        }

        public bool IsUOACZUndead
        {
            get
            {
                if (!(Region is UOACZRegion))
                    return false;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);
                return m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead;
            }
        }

        private int m_RestitutionFee;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RestitutionFee
        {
            get { return m_RestitutionFee; }
            set { m_RestitutionFee = value; }
        }

        private int m_RestitutionFeesToDistribute = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RestitutionFeesToDistribute
        {
            get { return m_RestitutionFeesToDistribute; }
            set { m_RestitutionFeesToDistribute = value; }
        }

        private bool m_MurdererDeathGumpNeeded = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool MurdererDeathGumpNeeded
        {
            get { return m_MurdererDeathGumpNeeded; }
            set { m_MurdererDeathGumpNeeded = value; }
        }

        public void EnterContestedRegion(bool ressingHere)
        {
            if ((Murderer || Paladin) && Alive && CheckAccountForPenance(this) && m_IsInTempStatLoss == false)
            {
                //All Murderers and Paladins on Account are now in Temporary Statloss
                Account acc = Account as Account;

                for (int i = 0; i < (acc.Length - 1); i++)
                {
                    Mobile m = acc.accountMobiles[i] as Mobile;

                    if (m != null)
                    {
                        PlayerMobile player = m as PlayerMobile;

                        if (player != null)
                        {
                            if (player.Paladin || player.Murderer)
                                player.IsInTempStatLoss = true;

                            break;
                        }
                    }
                }

                ApplyTempSkillLoss();

                if (ressingHere)
                    SendMessage("You are now in a restricted region while in a state of penance. All murderers and paladins on your account will now be under the effect of temporary statloss until all penance timers on your account have expired.");
                else
                    SendMessage("You have entered a restricted region while in a state of penance. All murderers and paladins on your account will now be under the effect of temporary statloss until all penance timers on your account have expired.");
            }
        }

        public static bool CheckAccountForPenance(Mobile from)
        {
            bool penanceRemaining = false;

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;

            Account acc = pm_From.Account as Account;

            if (acc == null)
            {
                Console.WriteLine(string.Format("{0} does not have an account, deleting.", pm_From.Name));
                pm_From.Delete();
                return false;
            }

            for (int i = 0; i < (acc.Length - 1); i++)
            {
                Mobile m = acc.accountMobiles[i] as Mobile;

                if (m != null)
                {
                    PlayerMobile player = m as PlayerMobile;

                    if (player != null)
                    {
                        if (player.m_PenanceExpiration > DateTime.UtcNow && !player.Deleted)
                            return true;

                        break;
                    }
                }
            }

            return penanceRemaining;
        }

        public static bool CheckAccountForStatloss(Mobile from)
        {
            bool tempStatLoss = false;

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;

            Account acc = pm_From.Account as Account;

            for (int i = 0; i < (acc.Length - 1); i++)
            {
                Mobile m = acc.accountMobiles[i] as Mobile;

                if (m != null)
                {
                    PlayerMobile player = m as PlayerMobile;

                    if (player != null)
                    {
                        if (player.IsInTempStatLoss)
                            tempStatLoss = true;

                        break;
                    }
                }
            }

            return tempStatLoss;
        }

        public TempStatLossTimer m_TempStatLossTimer;
        public ArrayList m_TempStatLossSkillMods = new ArrayList();

        public class TempStatLossTimer : Timer
        {
            private PlayerMobile m_Player;

            public TempStatLossTimer(PlayerMobile player)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            {
                m_Player = player;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                //Player is in Statloss and Penance is Now Expired for All Characters on this Account
                if (m_Player.IsInTempStatLoss && !CheckAccountForPenance(m_Player))
                {
                    m_Player.IsInTempStatLoss = false;
                    m_Player.RemoveTempStatLoss();

                    m_Player.SendMessage("You are now longer under the effects of temporary statloss.");

                    this.Stop();
                }
            }
        }

        public void ApplyPermanentSkillLoss(double skillLossAmount)
        {
            for (int a = 0; a < Skills.Length; ++a)
            {
                Skill skill = Skills[a];

                if (skill.Base > 0)
                {
                   double oldValue = skill.Base;
                   double newValue = Math.Floor(oldValue * (1 - skillLossAmount) * 10) / 10;

                   skill.Base = newValue;
                }
            }
        }

        public void ApplyTempSkillLoss()
        {
            for (int a = 0; a < Skills.Length; ++a)
            {
                Skill skill = Skills[a];

                double baseValue = skill.Base;

                if (baseValue > 0)
                {
                    SkillMod skillMod = new DefaultSkillMod(skill.SkillName, true, -(baseValue * PaladinEvents.StatLossSkillScalar));

                    m_TempStatLossSkillMods.Add(skillMod);
                    AddSkillMod(skillMod);
                }
            }

            if (m_TempStatLossTimer == null)
            {
                m_TempStatLossTimer = new TempStatLossTimer(this);
                m_TempStatLossTimer.Start();
            }

            else
                m_TempStatLossTimer.Start();
        }

        public void RemoveTempStatLoss()
        {
            int skillMods = m_TempStatLossSkillMods.Count;

            for (int a = 0; a < m_TempStatLossSkillMods.Count; a++)
            {
                RemoveSkillMod((SkillMod)m_TempStatLossSkillMods[a]);
            }

            m_TempStatLossSkillMods.Clear();

            if (m_TempStatLossTimer != null)
                m_TempStatLossTimer.Stop();
        }

        private DateTime m_HideRestrictionExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime HideRestrictionExpiration
        {
            get { return m_HideRestrictionExpiration; }
            set { m_HideRestrictionExpiration = value; }
        }

        public DateTime m_RecallRestrictionExpiration;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime RecallRestrictionExpiration
        {
            get { return m_RecallRestrictionExpiration; }
            set { m_RecallRestrictionExpiration = value; }
        }

        private DateTime m_NextBountyNote;

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextBountyNote
        {
            get
            {
                TimeSpan ts = m_NextBountyNote - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextBountyNote = DateTime.UtcNow + value; }
                catch { }
            }
        }

        private bool m_IsInArenaFight;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool IsInArenaFight
        {
            get { return m_IsInArenaFight; }
            set { m_IsInArenaFight = value; }
        }

        public bool CheckPlayerAccountsForCommonGuild(PlayerMobile player2)
        {
            bool foundSameGuild = false;

            List<Guilds.BaseGuild> m_Guilds = new List<Guilds.BaseGuild>();
            List<Guilds.BaseGuild> m_Player2Guilds = new List<Guilds.BaseGuild>();

            Account account = Account as Account;

            if (account != null)
            {
                for (int a = 0; a < (account.Length - 1); a++)
                {
                    Mobile m_Mobile = account.accountMobiles[a] as Mobile;

                    if (m_Mobile != null)
                    {
                        if (!m_Mobile.Deleted && m_Mobile.Guild != null)
                            m_Guilds.Add(m_Mobile.Guild);
                    }
                }
            }

            Account player2Account = player2.Account as Account;

            if (player2Account != null)
            {
                for (int a = 0; a < (player2Account.Length - 1); a++)
                {
                    Mobile m_Mobile = player2Account.accountMobiles[a] as Mobile;

                    if (m_Mobile != null)
                    {
                        if (!m_Mobile.Deleted && m_Mobile.Guild != null)
                            m_Player2Guilds.Add(m_Mobile.Guild);
                    }
                }
            }

            foreach (Guilds.BaseGuild player1Guild in m_Guilds)
            {
                foreach (Guilds.BaseGuild player2Guild in m_Player2Guilds)
                {
                    if (player1Guild == player2Guild)
                    {
                        return true;
                        break;
                    }
                }
            }

            return false;
        }

        public override void DoHarmful(Mobile target, bool indirect)
        {
            if (target == null)            
                return;            

            bool pvpValid = false;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;            

            if (target != this)
            {
                LastCombatTime = DateTime.UtcNow;
                target.LastCombatTime = DateTime.UtcNow;

                if (bc_Target != null)
                {
                    if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile && bc_Target.ControlMaster != this)
                    {
                        PlayerMobile pm_Controller = bc_Target.ControlMaster as PlayerMobile;
                        PlayerMobile pm_TargetController = bc_Target.ControlMaster as PlayerMobile;

                        bc_Target.LastPlayerCombatTime = DateTime.UtcNow;

                        PlayerVsPlayerCombatOccured(pm_Controller);          
                        pm_TargetController.PlayerVsPlayerCombatOccured(this);
                    }
                }

                if (pm_Target != null)
                {
                    PlayerVsPlayerCombatOccured(pm_Target);  
                    pm_Target.PlayerVsPlayerCombatOccured(this);
                }
            }

            base.DoHarmful(target, indirect);
        }

        public void CapStatMods(Mobile mobile)
        {
            //Enhanced Spellbook: Wizard has Buff Spells with 5x Duration.
            //Need to bring their duration down to normal maximum if another player does a harmful action to them        
            for (int i = 0; i < mobile.StatMods.Count; ++i)
            {
                StatMod check = mobile.StatMods[i];

                if (mobile.Region is UOACZRegion)
                    return;

                if (check.Type == StatType.Str || check.Type == StatType.Dex || check.Type == StatType.Int)
                {
                    if (check.Duration >= TimeSpan.FromSeconds(120))
                        check.Duration = TimeSpan.FromSeconds(120);
                }
            }
        }

        public override void OnHeal(ref int amount, Mobile from)
        {
            base.OnHeal(ref amount, from);

            SpecialAbilities.HealingOccured(from, this, amount);

            if (this.Region is BattlegroundRegion)
            {
                Battleground bg = ((BattlegroundRegion)this.Region).Battleground;
                if (bg.State == BattlegroundState.Active && from is PlayerMobile)
                {
                    // only score amount healed
                    int diff = HitsMax - Hits;
                    int scoredAmount = amount > diff ? diff : amount;
                    bg.CurrentScoreboard.UpdateScore(from as PlayerMobile, Categories.Healing, scoredAmount);
                }
            }

            //IPY OC LEADERBOARD
            var fromPlayer = from as PlayerMobile;
            if (fromPlayer != null && fromPlayer.IsInMilitia && this.IsInMilitia && this.Citizenship == fromPlayer.Citizenship)
            {
                Custom.Townsystem.Town town = Custom.Townsystem.Town.FromRegion(from.Region);
                if (town != null && Custom.Townsystem.OCTimeSlots.IsActiveTown(town))
                {
                    int diff = HitsMax - Hits;
                    int scoredAmount = amount > diff ? diff : amount;
                    Custom.Townsystem.OCLeaderboard.RegisterHealing(from, scoredAmount);
                }
            }
        }

        private DateTime m_T2AAccess;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public DateTime T2AAccess
        {
            get { return m_T2AAccess; }
            set { m_T2AAccess = value; }
        }

        // Easy UO Detection
        private Serial m_LastTarget;
        [CommandProperty(AccessLevel.GameMaster)]
        public Serial LastTarget
        {
            get { return m_LastTarget; }
            set { m_LastTarget = value; }
        }


        [CommandProperty(AccessLevel.GameMaster)]//IPY (Sean)
        public int TicketsOpenedSinceLastReset
        {
            get
            {
                if (DonationPlayerState == null)
                    return 0;
                else
                    return DonationPlayerState.TicketsOpenedSinceLastReset;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]//IPY (Sean)
        public int TotalOgre2TicketsOpened
        {
            get
            {
                if (DonationPlayerState == null)
                    return 0;
                else
                    return DonationPlayerState.TotalOgre2TicketsOpened;
            }
        }

        // Set bonus
        private SetBonus m_ActiveSetBonuses;
        [CommandProperty(AccessLevel.GameMaster)]
        public SetBonus ActiveSetBonuses
        {
            get { return m_ActiveSetBonuses; }
            set { m_ActiveSetBonuses = value; }
        }

        public bool HasActiveSetBonus(SetBonus bonus)
        {
            return (m_ActiveSetBonuses & bonus) == bonus;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CanReprieve
        {
            get { return m_CanReprieve; }
            set { m_CanReprieve = value; }
        }

        public override bool ShowFameTitle
        {
            get { return m_UserOptHideFameTitles ? false : base.ShowFameTitle; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastDeathByPlayer
        {
            get { return m_LastDeathByPlayer; }
            set { m_LastDeathByPlayer = value; }
        }

        #region PlayerClass Scores
        //PlayerClass Scores

        private int m_PreviousPaladinScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PreviousPaladinScore
        {
            get { return m_PreviousPaladinScore; }
            set { m_PreviousPaladinScore = value; }
        }

        private int m_PaladinScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PaladinScore
        {
            get { return m_PaladinScore; }
            set
            {
                int previousScore = m_PaladinScore;

                m_PaladinScore = value;

                int scoreChange = m_PaladinScore - previousScore;

                PlayerClassPersistance.UpdatePlayerScore(PlayerClassPersistance.PlayerClassScoreType.Paladin, this, scoreChange);
            }
        }

        private int m_MurdererScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MurdererScore
        {
            get { return m_MurdererScore; }
            set
            {
                int previousScore = m_MurdererScore;

                m_MurdererScore = value;

                int scoreChange = m_MurdererScore - previousScore;

                PlayerClassPersistance.UpdatePlayerScore(PlayerClassPersistance.PlayerClassScoreType.Murderer, this, scoreChange);
            }
        }

        private int m_PirateScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PirateScore
        {
            get { return m_PirateScore; }
            set
            {
                int previousScore = m_PirateScore;

                m_PirateScore = value;

                int scoreChange = m_PirateScore - previousScore;

                PlayerClassPersistance.UpdatePlayerScore(PlayerClassPersistance.PlayerClassScoreType.Pirate, this, scoreChange);
            }
        }

        #endregion

        [CommandProperty(AccessLevel.GameMaster)]
        public string AddPrefixTitle
        {
            set { if (!m_TitlesPrefix.Contains(value)) { m_TitlesPrefix.Add(value); } }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string RemovePrefixTitle
        {
            set
            {
                if (m_TitlesPrefix.Contains(value))
                {
                    m_TitlesPrefix.Remove(value);
                }
                if (m_CurrentPrefix == value)
                    m_CurrentPrefix = "";
            }
        }

        public Server.Custom.DonationState DonationPlayerState { get; set; }
        public Scripts.Custom.FireDungeonPlayerState FireDungeonState { get; set; }

        private SlayerEntry m_RepelGroupEntry;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public SlayerEntry RepelGroupEntry
        {
            get { return m_RepelGroupEntry; }
            set { m_RepelGroupEntry = value; }
        }

        private int m_PaladinPoints;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int PaladinCurrencyValue
        {
            get { return m_PaladinPoints; }
            set { m_PaladinPoints = value; }
        }

        private DateTime m_PaladinPointsDecayed;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public DateTime PaladinPointLastDecayTime
        {
            get { return m_PaladinPointsDecayed; }
            set { m_PaladinPointsDecayed = value; }
        }

        private QuestStep m_Step;
        [CommandProperty(AccessLevel.GameMaster)]
        public QuestStep Step
        {
            get { return m_Step; }
            set { m_Step = value; }
        }

        private List<string> m_TitlesPrefix = new List<string>();
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public List<string> TitlesPrefix
        {
            get { return m_TitlesPrefix; }
            set { m_TitlesPrefix = value; }
        }

        private string m_CurrentPrefix = "";
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public string CurrentPrefix
        {
            get { return m_CurrentPrefix; }
            set { m_CurrentPrefix = value; }
        }
        #endregion

        // IPY - Townsystem / factions
        #region Townsystem
        private DateTime m_LastTownSquareNotification = DateTime.MinValue;
        public DateTime LastTownSquareNotification
        {
            get { return m_LastTownSquareNotification; }
            set { m_LastTownSquareNotification = value; }
        }

        public bool SquelchCitizenship;
        private Custom.Townsystem.PlayerState m_TownsystemPlayerState;
        public Custom.Townsystem.PlayerState TownsystemPlayerState
        {
            get { return m_TownsystemPlayerState; }
            set { m_TownsystemPlayerState = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int OCBAllianceScore
        {
            get { return m_TownsystemPlayerState != null ? m_TownsystemPlayerState.AllianceScore : 0; }
            set { if (m_TownsystemPlayerState != null) m_TownsystemPlayerState.AllianceScore = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MilitiaVendorCredits
        {
            get { return m_TownsystemPlayerState == null ? 0 : m_TownsystemPlayerState.VendorCredit; }
            set { if (m_TownsystemPlayerState != null) m_TownsystemPlayerState.VendorCredit = value; }
        }

        //Citizenship
        private Custom.Townsystem.CitizenshipState m_CitizenshipPlayerState;
        public Custom.Townsystem.CitizenshipState CitizenshipPlayerState
        {
            get { return m_CitizenshipPlayerState == null ? null : m_CitizenshipPlayerState; }
            set { m_CitizenshipPlayerState = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Custom.Townsystem.Town Citizenship
        {
            get { return CitizenshipPlayerState == null ? null : CitizenshipPlayerState.Town; }
            set
            {
                if (value == null || (Citizenship != null && Citizenship == value))
                    return;

                if (Citizenship != null)
                    Custom.Townsystem.Town.RemoveCitizen(this, false);

                Custom.Townsystem.Town.AddCitizen(this, value);
            }
        }
        private bool m_IsInStatLoss = false;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Counselor)]
        public bool IsInStatLoss
        {
            get { return m_IsInStatLoss; }
            set { m_IsInStatLoss = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool IsInMilitia
        {
            get { return (TownsystemPlayerState != null); }
            set
            {
                if (Citizenship == null || IsInMilitia == value)
                    return;

                if (value)
                    Citizenship.HomeFaction.OnJoinAccepted(this);
                else
                    TownsystemPlayerState.Faction.RemoveMember(this);
            }
        }

        // Scoring
        private int m_TotalTreasuryKeys;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TreasuryKeys
        {
            get { return m_TotalTreasuryKeys; }
            set { m_TotalTreasuryKeys = value; }
        }

        private int m_TotalAccumulatedTreasuryKeys; // total lifetime
        public int TotalAccumulatedTreasuryKeys
        {
            get { return m_TotalAccumulatedTreasuryKeys; }
            set { m_TotalAccumulatedTreasuryKeys = value; }
        }
        #endregion

        //Parry Special Ability
        #region ParrySpecialAbility

        private DateTime m_ParrySpecialAbilityActivated = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ParrySpecialAbilityActivated
        {
            get { return m_ParrySpecialAbilityActivated; }
            set { m_ParrySpecialAbilityActivated = value; }
        }

        #endregion

        //////////////////////////////////////////////////////////////////////////
        // END IPY ADDITIONS
        //////////////////////////////////////////////////////////////////////////

        private class CountAndTimeStamp
        {
            private int m_Count;
            private DateTime m_Stamp;

            public CountAndTimeStamp()
            {
            }

            public DateTime TimeStamp { get { return m_Stamp; } }
            public int Count
            {
                get { return m_Count; }
                set { m_Count = value; m_Stamp = DateTime.UtcNow; }
            }
        }

        private DesignContext m_DesignContext;

        private static List<PlayerMobile> m_YoungChatListeners = new List<PlayerMobile>();
        public static List<PlayerMobile> YoungChatListeners
        {
            get { return m_YoungChatListeners; }
        }

        private NpcGuild m_NpcGuild;
        private DateTime m_NpcGuildJoinTime;
        private DateTime m_NextBODTurnInTime;
        private Point3D m_LastLocation = Point3D.Zero;
        private TimeSpan m_NpcGuildGameTime;
        private PlayerFlag m_Flags;
        private int m_StepsTaken;
        private int m_Profession;
        private bool m_IsStealthing; // IsStealthing should be moved to Server.Mobiles
        private bool m_IgnoreMobiles; // IgnoreMobiles should be moved to Server.Mobiles
        private int m_NonAutoreinsuredItems; // number of items that could not be automatically reinsured because gold in bank was not enough
        private bool m_NinjaWepCooldown;

        private bool m_ShowTownChat = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowTownChat { get { return m_ShowTownChat; } set { m_ShowTownChat = value; } }

        /*
         * a value of zero means, that the mobile is not executing the spell. Otherwise,
         * the value should match the BaseMana required
        */
        private int m_ExecutesLightningStrike; // move to Server.Mobiles??

        private DateTime m_LastOnline;
        private Server.Guilds.RankDefinition m_GuildRank;

        private bool m_Companion;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public bool Companion
        {
            get { return m_Companion; }
            set
            {
                m_Companion = value;
                if (value)
                {
                    if (!YoungChatListeners.Contains(this))
                        YoungChatListeners.Add(this);
                }
                else
                {
                    if (YoungChatListeners.Contains(this))
                        YoungChatListeners.Remove(this);
                }
            }
        }

        private PlayerMobile m_CompanionTarget;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerMobile CompanionTarget
        {
            get { return m_CompanionTarget; }
            set { m_CompanionTarget = value; }
        }

        private Point3D m_CompanionLastLocation;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D CompanionLastLocation
        {
            get { return m_CompanionLastLocation; }
            set { m_CompanionLastLocation = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LastLocation
        {
            get { return m_LastLocation; }
            set { m_LastLocation = value; }
        }

        private SpyglassAction m_SpyglassAction = SpyglassAction.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public SpyglassAction SpyglassAction
        {
            get { return m_SpyglassAction; }
            set { m_SpyglassAction = value; }
        }

        private DateTime m_NextSpyglassActionAllowed = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextSpyglassActionAllowed
        {
            get { return m_NextSpyglassActionAllowed; }
            set { m_NextSpyglassActionAllowed = value; }
        }

        private bool m_PetBattleUnlocked = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PetBattleUnlocked
        {
            get { return m_PetBattleUnlocked; }
            set { m_PetBattleUnlocked = value; }
        }

        private BaseInstrument m_LastInstrument;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseInstrument LastInstrument
        {
            get { return m_LastInstrument; }
            set { m_LastInstrument = value; }
        }

        private DateTime m_NextBattlegroundTime = DateTime.MinValue;
        [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public DateTime NextBattlegroundTime
        {
            get { return m_NextBattlegroundTime; }
            set { m_NextBattlegroundTime = value; }
        }

        public PetBattleCreatureCollection PetBattleCreatureCollection;
        public DateTime LastPetBattleActivity = DateTime.UtcNow;

        private int m_GuildMessageHue, m_AllianceMessageHue;

        private List<Mobile> m_AutoStabled;
        private List<Mobile> m_AllFollowers;

        #region Getters & Setters

        public List<Mobile> AutoStabled { get { return m_AutoStabled; } }

        public bool NinjaWepCooldown
        {
            get
            {
                return m_NinjaWepCooldown;
            }
            set
            {
                m_NinjaWepCooldown = value;
            }
        }

        public List<Mobile> AllFollowers
        {
            get
            {
                if (m_AllFollowers == null)
                    m_AllFollowers = new List<Mobile>();
                return m_AllFollowers;
            }
        }

        public Server.Guilds.RankDefinition GuildRank
        {
            get
            {
                if (this.AccessLevel >= AccessLevel.GameMaster)
                    return Server.Guilds.RankDefinition.Leader;
                else
                    return m_GuildRank;
            }
            set { m_GuildRank = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int GuildMessageHue
        {
            get { return m_GuildMessageHue; }
            set { m_GuildMessageHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AllianceMessageHue
        {
            get { return m_AllianceMessageHue; }
            set { m_AllianceMessageHue = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Profession
        {
            get { return m_Profession; }
            set { m_Profession = value; }
        }

        public int StepsTaken
        {
            get { return m_StepsTaken; }
            set { m_StepsTaken = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsStealthing // IsStealthing should be moved to Server.Mobiles
        {
            get { return m_IsStealthing; }
            set { m_IsStealthing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreMobiles // IgnoreMobiles should be moved to Server.Mobiles
        {
            get
            {
                return m_IgnoreMobiles;
            }
            set
            {
                if (m_IgnoreMobiles != value)
                {
                    m_IgnoreMobiles = value;
                    Delta(MobileDelta.Flags);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public NpcGuild NpcGuild
        {
            get { return m_NpcGuild; }
            set { m_NpcGuild = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NpcGuildJoinTime
        {
            get { return m_NpcGuildJoinTime; }
            set { m_NpcGuildJoinTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextBODTurnInTime
        {
            get { return m_NextBODTurnInTime; }
            set { m_NextBODTurnInTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastOnline
        {
            get { return m_LastOnline; }
            set { m_LastOnline = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public long LastMoved
        {
            get { return LastMoveTime; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NpcGuildGameTime
        {
            get { return m_NpcGuildGameTime; }
            set { m_NpcGuildGameTime = value; }
        }

        private int m_ToTItemsTurnedIn;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ToTItemsTurnedIn
        {
            get { return m_ToTItemsTurnedIn; }
            set { m_ToTItemsTurnedIn = value; }
        }

        private int m_ToTTotalMonsterFame;

        [CommandProperty(AccessLevel.GameMaster)]
        public int ToTTotalMonsterFame
        {
            get { return m_ToTTotalMonsterFame; }
            set { m_ToTTotalMonsterFame = value; }
        }

        public int ExecutesLightningStrike
        {
            get { return m_ExecutesLightningStrike; }
            set { m_ExecutesLightningStrike = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ToothAche
        {
            get { return CandyCane.GetToothAche(this); }
            set { CandyCane.SetToothAche(this, value); }
        }

        #endregion

        #region PlayerFlags
        public PlayerFlag Flags
        {
            get { return m_Flags; }
            set { m_Flags = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool PagingSquelched
        {
            get { return GetFlag(PlayerFlag.PagingSquelched); }
            set { SetFlag(PlayerFlag.PagingSquelched, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Glassblowing
        {
            get { return GetFlag(PlayerFlag.Glassblowing); }
            set { SetFlag(PlayerFlag.Glassblowing, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Masonry
        {
            get { return GetFlag(PlayerFlag.Masonry); }
            set { SetFlag(PlayerFlag.Masonry, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool SandMining
        {
            get { return GetFlag(PlayerFlag.SandMining); }
            set { SetFlag(PlayerFlag.SandMining, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool StoneMining
        {
            get { return GetFlag(PlayerFlag.StoneMining); }
            set { SetFlag(PlayerFlag.StoneMining, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ToggleMiningStone
        {
            get { return GetFlag(PlayerFlag.ToggleMiningStone); }
            set { SetFlag(PlayerFlag.ToggleMiningStone, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool KarmaLocked
        {
            get { return GetFlag(PlayerFlag.KarmaLocked); }
            set { SetFlag(PlayerFlag.KarmaLocked, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool AutoRenewInsurance
        {
            get { return GetFlag(PlayerFlag.AutoRenewInsurance); }
            set { SetFlag(PlayerFlag.AutoRenewInsurance, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool UseOwnFilter
        {
            get { return GetFlag(PlayerFlag.UseOwnFilter); }
            set { SetFlag(PlayerFlag.UseOwnFilter, value); }
        }
        
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AcceptGuildInvites
        {
            get { return GetFlag(PlayerFlag.AcceptGuildInvites); }
            set { SetFlag(PlayerFlag.AcceptGuildInvites, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HasStatReward
        {
            get { return GetFlag(PlayerFlag.HasStatReward); }
            set { SetFlag(PlayerFlag.HasStatReward, value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool RefuseTrades
        {
            get { return GetFlag(PlayerFlag.RefuseTrades); }
            set { SetFlag(PlayerFlag.RefuseTrades, value); }
        }
        #endregion

        #region Auto Arrow Recovery
        private Dictionary<Type, int> m_RecoverableAmmo = new Dictionary<Type, int>();

        public Dictionary<Type, int> RecoverableAmmo
        {
            get { return m_RecoverableAmmo; }
        }

        public void RecoverAmmo()
        {
            if (Core.SE && Alive)
            {
                foreach (KeyValuePair<Type, int> kvp in m_RecoverableAmmo)
                {
                    if (kvp.Value > 0)
                    {
                        Item ammo = null;

                        try
                        {
                            ammo = Activator.CreateInstance(kvp.Key) as Item;
                        }
                        catch
                        {
                        }

                        if (ammo != null)
                        {
                            string name = ammo.Name;
                            ammo.Amount = kvp.Value;

                            if (name == null)
                            {
                                if (ammo is Arrow)
                                    name = "arrow";
                                else if (ammo is Bolt)
                                    name = "bolt";
                            }

                            if (name != null && ammo.Amount > 1)
                                name = String.Format("{0}s", name);

                            if (name == null)
                                name = String.Format("#{0}", ammo.LabelNumber);

                            PlaceInBackpack(ammo);
                            SendLocalizedMessage(1073504, String.Format("{0}\t{1}", ammo.Amount, name)); // You recover ~1_NUM~ ~2_AMMO~.
                        }
                    }
                }

                m_RecoverableAmmo.Clear();
            }
        }

        #endregion

        private DateTime m_AnkhNextUse;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AnkhNextUse
        {
            get { return m_AnkhNextUse; }
            set { m_AnkhNextUse = value; }
        }

        private Boolean m_TrueHidden = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public Boolean TrueHidden
        {
            get { return m_TrueHidden; }
            set { m_TrueHidden = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan DisguiseTimeLeft
        {
            get { return DisguiseTimers.TimeRemaining(this); }
        }

        private DateTime m_PeacedUntil;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime PeacedUntil
        {
            get { return m_PeacedUntil; }
            set { m_PeacedUntil = value; }
        }

        #region Scroll of Alacrity
        private DateTime m_AcceleratedStart;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime AcceleratedStart
        {
            get { return m_AcceleratedStart; }
            set { m_AcceleratedStart = value; }
        }

        private SkillName m_AcceleratedSkill;

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName AcceleratedSkill
        {
            get { return m_AcceleratedSkill; }
            set { m_AcceleratedSkill = value; }
        }
        #endregion

        public static Direction GetDirection4(Point3D from, Point3D to)
        {
            int dx = from.X - to.X;
            int dy = from.Y - to.Y;

            int rx = dx - dy;
            int ry = dx + dy;

            Direction ret;

            if (rx >= 0 && ry >= 0)
                ret = Direction.West;
            else if (rx >= 0 && ry < 0)
                ret = Direction.South;
            else if (rx < 0 && ry < 0)
                ret = Direction.East;
            else
                ret = Direction.North;

            return ret;
        }

        public override bool AllowItemUse(Item item)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
            {
                if (!UOACZSystem.IsUndeadUsableItem(this, item))
                    return false;
            }

            #region Dueling
            if (m_DuelContext != null && !m_DuelContext.AllowItemUse(this, item))
                return false;
            #endregion

            return DesignContext.Check(this);
        }

        public override bool OnDragLift(Item item)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
            {
                if (item.RootParentEntity is Corpse || item.RootParentEntity is BaseContainer)                
                    return false;                
            }

            return base.OnDragLift(item);
        }

        public override bool OnDroppedItemInto(Item item, Container container, Point3D loc)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
            {
                if (UOACZSystem.IsUndeadUsableItem(this, item) && container.RootParentEntity == this)
                    return true;

                return false;
            }

            return base.OnDroppedItemInto(item, container, loc);
        }

        public override bool OnDroppedItemOnto(Item item, Item target)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                if (item is UOACZSurvivalTome || item is UOACZCorruptionTome)
                    return false;
            }

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                return false;

            return base.OnDroppedItemOnto(item, target);
        }

        public override bool OnDroppedItemToMobile(Item item, Mobile target)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                if (item is UOACZSurvivalTome || item is UOACZCorruptionTome)
                    return false;
            }

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                return false;

            return base.OnDroppedItemToMobile(item, target);
        }

        public override bool OnDroppedItemToWorld(Item item, Point3D location)
        {
            if (!base.OnDroppedItemToWorld(item, location))
                return false;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                if (item is UOACZSurvivalTome || item is UOACZCorruptionTome)
                    return false;
            }

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
            {
                if (UOACZSystem.IsUndeadUsableItem(this, item))
                    return false;
            }

            if (Core.AOS)
            {
                IPooledEnumerable mobiles = Map.GetMobilesInRange(location, 0);

                foreach (Mobile m in mobiles)
                {
                    if (m.Z >= location.Z && m.Z < location.Z + 16 && (!m.Hidden || m.AccessLevel == AccessLevel.Player))
                    {
                        mobiles.Free();
                        return false;
                    }
                }

                mobiles.Free();
            }

            BounceInfo bi = item.GetBounce();

            if (bi != null)
            {
                Type type = item.GetType();

                if (type.IsDefined(typeof(FurnitureAttribute), true) || type.IsDefined(typeof(DynamicFlipingAttribute), true))
                {
                    object[] objs = type.GetCustomAttributes(typeof(FlipableAttribute), true);

                    if (objs != null && objs.Length > 0)
                    {
                        FlipableAttribute fp = objs[0] as FlipableAttribute;

                        if (fp != null)
                        {
                            int[] itemIDs = fp.ItemIDs;

                            Point3D oldWorldLoc = bi.m_WorldLoc;
                            Point3D newWorldLoc = location;

                            if (oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y)
                            {
                                Direction dir = GetDirection4(oldWorldLoc, newWorldLoc);

                                if (itemIDs.Length == 2)
                                {
                                    switch (dir)
                                    {
                                        case Direction.North:
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East:
                                        case Direction.West: item.ItemID = itemIDs[1]; break;
                                    }
                                }
                                else if (itemIDs.Length == 4)
                                {
                                    switch (dir)
                                    {
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East: item.ItemID = itemIDs[1]; break;
                                        case Direction.North: item.ItemID = itemIDs[2]; break;
                                        case Direction.West: item.ItemID = itemIDs[3]; break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public override int GetPacketFlags()
        {
            int flags = base.GetPacketFlags();

            if (m_IgnoreMobiles)
                flags |= 0x10;

            return flags;
        }

        public override int GetOldPacketFlags()
        {
            int flags = base.GetOldPacketFlags();

            if (m_IgnoreMobiles)
                flags |= 0x10;

            return flags;
        }

        public bool GetFlag(PlayerFlag flag)
        {
            return ((m_Flags & flag) != 0);
        }

        public void SetFlag(PlayerFlag flag, bool value)
        {
            if (value)
                m_Flags |= flag;
            else
                m_Flags &= ~flag;
        }

        public DesignContext DesignContext
        {
            get { return m_DesignContext; }
            set { m_DesignContext = value; }
        }

        public static void Initialize()
        {
            if (FastwalkPrevention)
                PacketHandlers.RegisterThrottler(0x02, new ThrottlePacketCallback(MovementThrottle_Callback));

            EventSink.Login += new LoginEventHandler(OnLogin);
            EventSink.Logout += new LogoutEventHandler(OnLogout);
            EventSink.Connected += new ConnectedEventHandler(EventSink_Connected);
            EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);
            if (Core.SE)
            {
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(CheckPets));
            }

            CommandSystem.Register("WipePlayerMobiles", AccessLevel.Administrator, new CommandEventHandler(WipeAllPlayerMobiles_OnCommand));
            CommandSystem.Register("UseTrapPouch", AccessLevel.Player, new CommandEventHandler(UseTrappedPouch_OnCommand));
            CommandSystem.Register("FireDungeonTimer", AccessLevel.Player, new CommandEventHandler(FireDungeonTimer_OnCommand));

            CommandSystem.Register("ShowMeleeDamage", AccessLevel.Player, new CommandEventHandler(ShowMeleeDamage));
            CommandSystem.Register("ShowSpellDamage", AccessLevel.Player, new CommandEventHandler(ShowSpellDamage));
            CommandSystem.Register("ShowFollowerDamage", AccessLevel.Player, new CommandEventHandler(ShowFollowerDamage));
            CommandSystem.Register("ShowProvocationDamage", AccessLevel.Player, new CommandEventHandler(ShowProvocationDamage));
            CommandSystem.Register("ShowPoisonDamage", AccessLevel.Player, new CommandEventHandler(ShowPoisonDamage));
            CommandSystem.Register("ShowDamageTaken", AccessLevel.Player, new CommandEventHandler(ShowDamageTaken));
            CommandSystem.Register("ShowFollowerDamageTaken", AccessLevel.Player, new CommandEventHandler(ShowFollowerDamageTaken));
            CommandSystem.Register("ShowHealing", AccessLevel.Player, new CommandEventHandler(ShowHealing));

            CommandSystem.Register("ShowStealthSteps", AccessLevel.Player, new CommandEventHandler(ShowStealthSteps));
            CommandSystem.Register("ShowHenchmenSpeech", AccessLevel.Player, new CommandEventHandler(ShowHenchmenSpeech));
            CommandSystem.Register("ShowAdminTextFilter", AccessLevel.Counselor, new CommandEventHandler(ShowAdminTextFilter));

            CommandSystem.Register("AutoStealth", AccessLevel.Player, new CommandEventHandler(ToggleAutoStealth));

            CommandSystem.Register("GetDifficulty", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.GetDifficulty));
            CommandSystem.Register("Provoke", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.AdminProvoke));
            CommandSystem.Register("Tame", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.AdminTame));
            CommandSystem.Register("GotoCurrentWaypoint", AccessLevel.Counselor, new CommandEventHandler(BaseCreature.GotoCurrentWaypoint));

            CommandSystem.Register("ShowTownChat", AccessLevel.Player, new CommandEventHandler(ShowTownChatCommand));
            CommandSystem.Register("gotoentrance", AccessLevel.Administrator, new CommandEventHandler(GoToEntranceCommand));
            CommandSystem.Register("playercount", AccessLevel.Administrator, new CommandEventHandler(PlayerCountCommand));
            CommandSystem.Register("setthreshold", AccessLevel.Administrator, new CommandEventHandler(SetThresholdCommand));
            CommandSystem.Register("togglethreshold", AccessLevel.Administrator, new CommandEventHandler(ToggleThresholdCommand));

            //Used for Locally Testing Content
            CommandSystem.Register("CreateTestLoadout", AccessLevel.GameMaster, new CommandEventHandler(CreateTestLoadout));
            CommandSystem.Register("Anim", AccessLevel.GameMaster, new CommandEventHandler(Anim));
            CommandSystem.Register("AnimationTest", AccessLevel.GameMaster, new CommandEventHandler(AnimationTest));
        }

        [Usage("CreateTestLoadout")]
        [Description("Sets Character Stats, Skills, and Equipment for TESting")]
        public static void CreateTestLoadout(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.SendMessage("Target the character to put in testing mode");
            player.Target = new CreateTestLoadoutTarget(player);
        }

        private class CreateTestLoadoutTarget : Target
        {
            public CreateTestLoadoutTarget(Mobile from)
                : base(100, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (target is PlayerMobile)
                {
                    PlayerMobile pm_Target = target as PlayerMobile;

                    if (!pm_Target.Alive)
                        return;

                    pm_Target.RawStr = 10000;
                    pm_Target.Hits = pm_Target.HitsMax;

                    pm_Target.RawDex = 200;
                    pm_Target.Stam = pm_Target.StamMax;

                    pm_Target.RawInt = 1000;
                    pm_Target.Mana = pm_Target.ManaMax;

                    pm_Target.Young = false;

                    foreach (Skill skill in pm_Target.Skills)
                    {
                        skill.Base = 100;
                    }

                    pm_Target.DeleteAllEquipment();

                    pm_Target.Backpack.DropItem(new Arrow(2000));
                    pm_Target.AddItem(new Bow());

                    TotalRefreshPotion potion = new TotalRefreshPotion();
                    potion.Amount = 50;
                    pm_Target.Backpack.DropItem(potion);

                    Bandage bandage = new Bandage();
                    bandage.Amount = 200;
                    pm_Target.Backpack.DropItem(bandage);

                    BagOfReagents bagOfReagents = new BagOfReagents();
                    pm_Target.Backpack.DropItem(bagOfReagents);

                    bagOfReagents = new BagOfReagents();
                    pm_Target.Backpack.DropItem(bagOfReagents);

                    Spellbook spellbook = new Spellbook();
                    if (spellbook.BookCount == 64)
                        spellbook.Content = ulong.MaxValue;
                    else
                        spellbook.Content = (1ul << spellbook.BookCount) - 1;

                    pm_Target.Backpack.DropItem(spellbook);

                    int dungeonCount = Enum.GetNames(typeof(BaseDungeonArmor.DungeonEnum)).Length;

                    BaseDungeonArmor.DungeonEnum dungeon = (BaseDungeonArmor.DungeonEnum)Utility.RandomMinMax(1, dungeonCount - 1);

                    pm_Target.AddItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Helmet));
                    pm_Target.AddItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gorget));
                    pm_Target.AddItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Arms));
                    pm_Target.AddItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gloves));
                    pm_Target.AddItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Chest));
                    pm_Target.AddItem(BaseDungeonArmor.CreateDungeonArmor(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Legs));

                    BaseDungeonArmor.DungeonArmorDetail dungeonArmorDetail = new BaseDungeonArmor.DungeonArmorDetail(dungeon, BaseDungeonArmor.ArmorTierEnum.Tier1);

                    pm_Target.AddItem(new Cloak(dungeonArmorDetail.Hue));
                }

                else
                {
                    from.SendMessage("That is not a player.");
                    return;
                }
            }
        }

        public void DeleteAllEquipment()
        {
            //Clean Out Backpack
            if (Backpack != null)
            {
                if (!Backpack.Deleted)
                {
                    Backpack.Delete();
                    AddItem(new Backpack());
                }
            }

            List<Layer> m_Layers = new List<Layer>();

            m_Layers.Add(Layer.Arms);
            m_Layers.Add(Layer.Bracelet);
            m_Layers.Add(Layer.Cloak);
            m_Layers.Add(Layer.Earrings);
            m_Layers.Add(Layer.FirstValid);
            m_Layers.Add(Layer.Gloves);
            m_Layers.Add(Layer.Helm);
            m_Layers.Add(Layer.InnerLegs);
            m_Layers.Add(Layer.InnerTorso);
            m_Layers.Add(Layer.MiddleTorso);
            m_Layers.Add(Layer.Neck);
            m_Layers.Add(Layer.OneHanded);
            m_Layers.Add(Layer.OuterLegs);
            m_Layers.Add(Layer.OuterTorso);
            m_Layers.Add(Layer.Pants);
            m_Layers.Add(Layer.Ring);
            m_Layers.Add(Layer.Shirt);
            m_Layers.Add(Layer.Shoes);
            m_Layers.Add(Layer.Talisman);
            m_Layers.Add(Layer.TwoHanded);
            m_Layers.Add(Layer.Waist);

            foreach (Layer layer in m_Layers)
            {
                Item item = FindItemOnLayer(layer);

                if (item != null)
                {
                    if (!item.Deleted)
                        item.Delete();
                }
            }
        }

        [Usage("Anim <action> <frameCount>")]
        [Description("Makes your character do a specified animation.")]
        public static void Anim(CommandEventArgs e)
        {
            if (e.Length == 2)            
                e.Mobile.Animate(e.GetInt32(0), e.GetInt32(1), 1, true, false, 0);
        }

        [Usage("Animation Test")]
        [Description("Loop through all animations of a Bodyvalue")]
        public static void AnimationTest(CommandEventArgs arg)
        {
            PlayerMobile player = arg.Mobile as PlayerMobile;

            if (player == null)
                return;

            int animations = 32;
            int frameCount = 15;
            int delayBetween = 10;

            Point3D location = player.Location;
            Map map = player.Map;

            for (int a = 1; a < animations + 1; a++)
            {
                int animation = a;

                Timer.DelayCall(TimeSpan.FromSeconds((animation - 1) * delayBetween), delegate
                {
                    if (player == null) return;
                    if (player.Location != location) return;

                    player.Say("Animation: " + animation.ToString());
                    player.Animate(animation, frameCount, 1, true, false, 0);
                });           
            }
        }

        private static void CheckPets()
        {
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (m is PlayerMobile)
                {
                    PlayerMobile pm = (PlayerMobile)m;

                    if (((!pm.Mounted || (pm.Mount != null && pm.Mount is EtherealMount)) && (pm.AllFollowers.Count > pm.AutoStabled.Count)) ||
                        (pm.Mounted && (pm.AllFollowers.Count > (pm.AutoStabled.Count + 1))))
                    {
                        pm.AutoStablePets(); /* autostable checks summons, et al: no need here */
                    }
                }
            }
        }

        private MountBlock m_MountBlock;

        public BlockMountType MountBlockReason
        {
            get
            {
                return (CheckBlock(m_MountBlock)) ? m_MountBlock.m_Type : BlockMountType.None;
            }
        }

        private static bool CheckBlock(MountBlock block)
        {
            return ((block is MountBlock) && block.m_Timer.Running);
        }

        private class MountBlock
        {
            public BlockMountType m_Type;
            public Timer m_Timer;

            public MountBlock(TimeSpan duration, BlockMountType type, Mobile mobile)
            {
                m_Type = type;

                m_Timer = Timer.DelayCall(duration, new TimerStateCallback<Mobile>(RemoveBlock), mobile);
            }

            private void RemoveBlock(Mobile mobile)
            {
                (mobile as PlayerMobile).m_MountBlock = null;
            }
        }

        public void SetMountBlock(BlockMountType type, TimeSpan duration, bool dismount)
        {
            if (dismount)
            {
                if (this.Mount != null)
                {
                    this.Mount.Rider = null;
                }
                else if (AnimalForm.UnderTransformation(this))
                {
                    AnimalForm.RemoveContext(this, true);
                }
            }

            if ((m_MountBlock == null) || !m_MountBlock.m_Timer.Running || (m_MountBlock.m_Timer.Next < (DateTime.UtcNow + duration)))
            {
                m_MountBlock = new MountBlock(duration, type, this);
            }
        }

        public override void OnSkillInvalidated(Skill skill)
        {
            if (Core.AOS && skill.SkillName == SkillName.MagicResist)
                UpdateResistances();
        }

        public override int GetMaxResistance(ResistanceType type)
        {
            if (AccessLevel > AccessLevel.Player)
                return 100;

            int max = base.GetMaxResistance(type);

            if (type != ResistanceType.Physical && 60 < max && Spells.Fourth.CurseSpell.UnderEffect(this))
                max = 60;

            if (Core.ML && this.Race == Race.Elf && type == ResistanceType.Energy)
                max += 5; //Intended to go after the 60 max from curse

            return max;
        }

        protected override void OnRaceChange(Race oldRace)
        {
            ValidateEquipment();
            UpdateResistances();
        }

        public override int MaxWeight { get { return (((Core.ML && this.Race == Race.Human) ? 100 : 40) + (int)(3.5 * this.Str)); } }

        private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

        public override void OnNetStateChanged()
        {
            m_LastGlobalLight = -1;
            m_LastPersonalLight = -1;
        }

        public override void ComputeBaseLightLevels(out int global, out int personal)
        {
            global = LightCycle.ComputeLevelFor(this);

            bool racialNightSight = (Core.ML && this.Race == Race.Elf);

            if (this.LightLevel < 21 && (AosAttributes.GetValue(this, AosAttribute.NightSight) > 0 || racialNightSight))
                personal = 21;
            else
                personal = this.LightLevel;
        }

        public override void CheckLightLevels(bool forceResend)
        {
            NetState ns = this.NetState;

            if (ns == null)
                return;

            int global, personal;

            ComputeLightLevels(out global, out personal);

            if (!forceResend)
                forceResend = (global != m_LastGlobalLight || personal != m_LastPersonalLight);

            if (!forceResend)
                return;

            m_LastGlobalLight = global;
            m_LastPersonalLight = personal;

            ns.Send(GlobalLightLevel.Instantiate(global));
            ns.Send(new PersonalLightLevel(this, personal));
        }

        public override int GetMinResistance(ResistanceType type)
        {
            int magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
            int min = int.MinValue;

            if (magicResist >= 1000)
                min = 40 + ((magicResist - 1000) / 50);
            else if (magicResist >= 400)
                min = (magicResist - 400) / 15;

            if (min > MaxPlayerResistance)
                min = MaxPlayerResistance;

            int baseMin = base.GetMinResistance(type);

            if (min < baseMin)
                min = baseMin;

            return min;
        }

        private static void OnLogin(LoginEventArgs e)
        {
            Mobile from = e.Mobile;

            CheckAtrophies(from);

            from.FollowersMax = 5;

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From != null)
                pm_From.m_SessionStart = DateTime.UtcNow;

            if (AccountHandler.LockdownLevel > AccessLevel.Player)
            {
                string notice;

                Accounting.Account acct = from.Account as Accounting.Account;

                if (acct == null || !acct.HasAccess(from.NetState))
                {
                    if (from.AccessLevel == AccessLevel.Player)
                        notice = "The server is currently under lockdown. No players are allowed to log in at this time.";

                    else
                        notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";

                    Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(Disconnect), from);
                }

                else if (from.AccessLevel >= AccessLevel.Administrator)                
                    notice = "The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";                

                else                
                    notice = "The server is currently under lockdown. You have sufficient access level to connect.";                

                from.SendGump(new NoticeGump(1060637, 30720, notice, 0xFFC000, 300, 140, null, null));
                return;
            }

            if (pm_From != null)
            {
                if ((pm_From.Young || pm_From.Companion) && !YoungChatListeners.Contains(pm_From))
                    YoungChatListeners.Add(pm_From);

                Timer.DelayCall(TimeSpan.FromSeconds(5), new TimerStateCallback(CheckAccountAgeAchievements), from as object);
                pm_From.ClaimAutoStabledPets();

                // Re-hue items if talisman is equiped
                Item ocbtalisman = pm_From.FindItemOnLayer(Layer.Talisman);
                HerosTalisman heroTalisman = ocbtalisman as HerosTalisman;
                VeteranKnightsTalisman veteranTalisman = ocbtalisman as VeteranKnightsTalisman;

                if (heroTalisman != null || veteranTalisman != null)
                {
                    foreach (Item i in pm_From.Items)
                    {
                        if (heroTalisman != null)
                            heroTalisman.HueItem(i);
                        if (veteranTalisman != null)
                            veteranTalisman.HueItem(i);
                    }
                }

                if (pm_From.AccessLevel > AccessLevel.Player)
                    pm_From.Send(SpeedControl.MountSpeed);
            }            

            //Player is Murderer And Needs To Resolve The Murderer Ress Gump (Pick From Discount Options)
            if (pm_From.MurdererDeathGumpNeeded)
            {
                if (pm_From.ShortTermMurders <= 5)
                    PaladinEvents.MurdererPunishmentResolved(pm_From);

                else
                    pm_From.SendGump(new Custom.Paladin.MurdererDeathGump(pm_From, 1));
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (CheckAccountForPenance(pm_From) && pm_From.IsInTempStatLoss && (pm_From.Murderer || pm_From.Paladin))
                    pm_From.SendMessage("You and all other murderers and paladins on your account are currently under the effects of temporary statloss until all penance timers on your account have expired.");
            });

            //Player Enhancements
            if (pm_From.m_PlayerEnhancementAccountEntry == null)
                PlayerEnhancementPersistance.CreatePlayerEnhancementAccountEntry(pm_From);

            if (pm_From.m_PlayerEnhancementAccountEntry.Deleted)
                PlayerEnhancementPersistance.CreatePlayerEnhancementAccountEntry(pm_From);

            PlayerCustomization.OnLoginAudit(pm_From);

            //Audit Enhancements For New Entries Available
            pm_From.m_PlayerEnhancementAccountEntry.AuditCustomizationEntries();
            pm_From.m_PlayerEnhancementAccountEntry.AuditSpellHueEntries();

            //Influence System 
            //TEST 
            //InfluencePersistance.CheckCreateInfluenceAccountEntry(pm_From);   
            InfluencePersistance.OnLogin(pm_From);
            
            //UOACZ
            UOACZSystem.OnLogin(pm_From);

            //World Chat
            ChatPersistance.OnLogin(pm_From);

            //Monster Hunter Society
            MHSPersistance.CheckAndCreateMHSAccountEntry(pm_From);

            //Event Calendar Account
            EventCalendarPersistance.CheckAndCreateEventCalendarAccount(pm_From);

            //Dungeon Armor
            BaseDungeonArmor.CheckForAndUpdateDungeonArmorProperties(pm_From);

            //OverloadProtectionSystem
            pm_From.SystemOverloadActions = 0;
        }

        public void ResetRegenTimers()
        {
            //Reset Regen Timers
            if (m_HitsTimer != null)
            {
                m_HitsTimer.Stop();
                m_HitsTimer = null;

                m_HitsTimer = new Mobile.HitsTimer(this);
                m_HitsTimer.Start();
            }

            if (m_StamTimer != null)
            {
                m_StamTimer.Stop();
                m_StamTimer = null;

                m_StamTimer = new Mobile.StamTimer(this);
                m_StamTimer.Start();
            }

            if (m_ManaTimer != null)
            {
                m_ManaTimer.Stop();
                m_ManaTimer = null;

                m_ManaTimer = new Mobile.ManaTimer(this);
                m_ManaTimer.Start();
            }
        }

        private bool m_NoDeltaRecursion;

        public void ValidateEquipment()
        {
            if (m_NoDeltaRecursion || Map == null || Map == Map.Internal)
                return;

            if (this.Items == null)
                return;

            m_NoDeltaRecursion = true;
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(ValidateEquipment_Sandbox));
        }

        private void ValidateEquipment_Sandbox()
        {
            try
            {
                if (Map == null || Map == Map.Internal)
                    return;

                List<Item> items = this.Items;

                if (items == null)
                    return;

                bool moved = false;

                int str = this.Str;
                int dex = this.Dex;
                int intel = this.Int;

                #region Factions
                int factionItemCount = 0;
                #endregion

                Mobile from = this;

                #region Ethics
                Ethics.Ethic ethic = Ethics.Ethic.Find(from);
                #endregion

                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (i >= items.Count)
                        continue;

                    Item item = items[i];

                    #region Ethics
                    if ((item.SavedFlags & 0x100) != 0)
                    {
                        if (item.Hue != Ethics.Ethic.Hero.Definition.PrimaryHue)
                        {
                            item.SavedFlags &= ~0x100;
                        }
                        else if (ethic != Ethics.Ethic.Hero)
                        {
                            from.AddToBackpack(item);
                            moved = true;
                            continue;
                        }
                    }
                    else if ((item.SavedFlags & 0x200) != 0)
                    {
                        if (item.Hue != Ethics.Ethic.Evil.Definition.PrimaryHue)
                        {
                            item.SavedFlags &= ~0x200;
                        }
                        else if (ethic != Ethics.Ethic.Evil)
                        {
                            from.AddToBackpack(item);
                            moved = true;
                            continue;
                        }
                    }
                    #endregion

                    if (item is BaseWeapon)
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        bool drop = false;

                        if (dex < weapon.DexRequirement)
                            drop = true;
                        else if (str < AOS.Scale(weapon.StrRequirement, 100 - weapon.GetLowerStatReq()))
                            drop = true;
                        else if (intel < weapon.IntRequirement)
                            drop = true;
                        else if (weapon.RequiredRace != null && weapon.RequiredRace != this.Race)
                            drop = true;

                        if (drop)
                        {
                            string name = weapon.Name;

                            if (name == null)
                                name = String.Format("#{0}", weapon.LabelNumber);

                            from.SendLocalizedMessage(1062001, name); // You can no longer wield your ~1_WEAPON~
                            from.AddToBackpack(weapon);
                            moved = true;
                        }
                    }
                    else if (item is BaseArmor)
                    {
                        BaseArmor armor = (BaseArmor)item;

                        bool drop = false;

                        if (!armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (!armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (armor.RequiredRace != null && armor.RequiredRace != this.Race)
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = armor.ComputeStatBonus(StatType.Str), strReq = armor.ComputeStatReq(StatType.Str);
                            int dexBonus = armor.ComputeStatBonus(StatType.Dex), dexReq = armor.ComputeStatReq(StatType.Dex);
                            int intBonus = armor.ComputeStatBonus(StatType.Int), intReq = armor.ComputeStatReq(StatType.Int);

                            if (dex < dexReq || (dex + dexBonus) < 1)
                                drop = true;
                            else if (str < strReq || (str + strBonus) < 1)
                                drop = true;
                            else if (intel < intReq || (intel + intBonus) < 1)
                                drop = true;
                        }

                        if (drop)
                        {
                            string name = armor.Name;

                            if (name == null)
                                name = String.Format("#{0}", armor.LabelNumber);

                            if (armor is BaseShield)
                                from.SendLocalizedMessage(1062003, name); // You can no longer equip your ~1_SHIELD~
                            else
                                from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack(armor);
                            moved = true;
                        }
                    }
                    else if (item is BaseClothing)
                    {
                        BaseClothing clothing = (BaseClothing)item;

                        bool drop = false;

                        if (!clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (!clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster)
                        {
                            drop = true;
                        }
                        else if (clothing.RequiredRace != null && clothing.RequiredRace != this.Race)
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = clothing.ComputeStatBonus(StatType.Str);
                            int strReq = clothing.ComputeStatReq(StatType.Str);

                            if (str < strReq || (str + strBonus) < 1)
                                drop = true;
                        }

                        if (drop)
                        {
                            string name = clothing.Name;

                            if (name == null)
                                name = String.Format("#{0}", clothing.LabelNumber);

                            from.SendLocalizedMessage(1062002, name); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack(clothing);
                            moved = true;
                        }
                    }

                    Factions.FactionItem factionItem = Factions.FactionItem.Find(item);

                    if (factionItem != null)
                    {
                        bool drop = false;

                        Factions.Faction ourFaction = Factions.Faction.Find(this);

                        if (ourFaction == null || ourFaction != factionItem.Faction)
                            drop = true;
                        else if (++factionItemCount > Factions.FactionItem.GetMaxWearables(this))
                            drop = true;

                        if (drop)
                        {
                            from.AddToBackpack(item);
                            moved = true;
                        }
                    }
                }

                if (moved)
                    from.SendLocalizedMessage(500647); // Some equipment has been moved to your backpack.
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                m_NoDeltaRecursion = false;
            }
        }

        public override void Delta(MobileDelta flag)
        {
            base.Delta(flag);

            if ((flag & MobileDelta.Stat) != 0)
                ValidateEquipment();           
        }

        private static void Disconnect(object state)
        {
            NetState ns = ((Mobile)state).NetState;

            if (ns != null)
                ns.Dispose();
        }

        private static void OnLogout(LogoutEventArgs e)
        {
            PlayerMobile player = e.Mobile as PlayerMobile;

            if (player == null)
                return;

            player.AutoStablePets();  
        }

        private static void EventSink_Connected(ConnectedEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                pm.m_SessionStart = DateTime.UtcNow;

                if (pm.m_Quest != null)
                    pm.m_Quest.StartTimer();

                pm.BedrollLogout = false;
                pm.LastOnline = DateTime.UtcNow;
            }

            DisguiseTimers.StartTimer(e.Mobile);

            Timer.DelayCall(TimeSpan.Zero, new TimerStateCallback(ClearSpecialMovesCallback), e.Mobile);
        }

        private static void ClearSpecialMovesCallback(object state)
        {
            Mobile from = (Mobile)state;

            SpecialMove.ClearAllMoves(from);
        }

        private static void EventSink_Disconnected(DisconnectedEventArgs e)
        {
            Mobile from = e.Mobile;
            DesignContext context = DesignContext.Find(from);

            if (context != null)
            {
                /* Client disconnected
                 *  - Remove design context
                 *  - Eject all from house
                 *  - Restore relocated entities
                 */

                // Remove design context
                DesignContext.Remove(from);

                // Eject all from house
                from.RevealingAction();

                foreach (Item item in context.Foundation.GetItems())
                    item.Location = context.Foundation.BanLocation;

                foreach (Mobile mobile in context.Foundation.GetMobiles())
                    mobile.Location = context.Foundation.BanLocation;

                // Restore relocated entities
                context.Foundation.RestoreRelocatedEntities();
            }

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null)
            {
                if (pm.m_PokerGame != null)
                {
                    PokerPlayer player = pm.m_PokerGame.GetPlayer( pm );
                    if (player != null)
                    {
                        if (pm.m_PokerGame.Players != null && pm.m_PokerGame.Players.Contains( player ))
                            pm.m_PokerGame.RemovePlayer( player );
                    }
                }

                if (YoungChatListeners.Contains(pm))
                    YoungChatListeners.Remove(pm);

                pm.IsDragging = false;

                if (pm.Region is BattlegroundRegion)
                {
                    Battleground bg = ((BattlegroundRegion)pm.Region).Battleground;
                    // allow disconnected players to rejoin competitve ctf games
                    if (!(bg is PickedTeamCTFBattleground))
                        bg.Leave(pm);
                }
                else
                {
                    Battleground.Instances.ForEach(bg => bg.Queue.Leave(pm));
                    // remove disconnects from team picker
                    var bgs = Battleground.Instances.Where(bg => bg is PickedTeamCTFBattleground);
                    foreach (var bg in bgs)
                    {
                        var cctfbg = bg as PickedTeamCTFBattleground;
                        if (cctfbg != null && cctfbg.ReadyPlayers.Contains(pm))
                            cctfbg.ReadyPlayers.Remove(pm);

                        var teams = bg.Teams.Where(t => t.Contains(pm));
                        foreach (var team in teams)
                        {
                            team.Players.Remove(pm);
                            ((PickedTeamCTFBattleground)bg).UpdateTeamSelectionGumps();
                        }
                    }
                }

                TimeSpan gameTime = DateTime.UtcNow - pm.m_SessionStart;
                pm.m_GameTime += gameTime;
                pm.m_BankGameTime += gameTime;

                Custom.Townsystem.CitizenshipState.RegisterGameTime(from, gameTime);

                if (pm.m_Quest != null)
                    pm.m_Quest.StopTimer();

                RemoveWindFragment(pm);

                pm.m_SpeechLog = null;
                pm.LastOnline = DateTime.UtcNow;
                pm.SetSallos(false);

                if (pm.m_SpellToleranceTimer != null)
                {
                    pm.m_SpellToleranceTimer.Stop();
                    pm.m_SpellToleranceTimer = null;
                }
            }

            DisguiseTimers.StopTimer(from);
        }

        // Wind Battleground, return fragment home if player disconnects
        private static void RemoveWindFragment(Mobile player)
        {
            if (WindFragment.ExistsOn(player))
            {
                Container pack = player.Backpack;
                Item fragment;
                if (pack != null && (fragment = pack.FindItemByType(typeof(WindFragment))) != null)
                    ((WindFragment)fragment).ReturnHome();
            }
        }

        #region Sallos
        private bool _Sallos;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Sallos
        {
            get { return _Sallos; }
        }

        public void SetSallos(bool value)
        {
            _Sallos = value;
        }
        #endregion

        public override void RevealingAction()
        {
            if (m_DesignContext != null)
                return;

            Spells.Sixth.InvisibilitySpell.RemoveTimer(this);
            InvisPotion.RemoveTimer(this);

            if (Spectating && !(Region is BattlegroundRegion))
                Spectating = false;

            base.RevealingAction();

            TrueHidden = false;
            m_IsStealthing = false; // IsStealthing should be moved to Server.Mobiles
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Hidden
        {
            get
            {
                return base.Hidden;
            }
            set
            {
                if (value && WindFragment.ExistsOn(this))
                {
                    SendMessage("You may not hide with the Wind Fragment.");
                    return;
                }

                if (value && Custom.Battlegrounds.Items.CTFFlag.ExistsOn(this))
                {
                    SendMessage("You may not hide with a CTF flag.");
                    return;
                }

                if (value && HideRestrictionExpiration > DateTime.UtcNow)
                {
                    string hideRestrictionRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, HideRestrictionExpiration, false, false, false, true, true);
                    
                    SendMessage("You are unable to hide for another " + hideRestrictionRemaining + ".");

                    return;
                }

                base.Hidden = value;

                RemoveBuff(BuffIcon.Invisibility);	//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

                if (!Hidden)
                {
                    RemoveBuff(BuffIcon.HidingAndOrStealth);
                }
                else// if( !InvisibilitySpell.HasTimer( this ) )
                {
                    BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));	//Hidden/Stealthing & You Are Hidden
                }
            }
        }

        public override void OnSubItemAdded(Item item)
        {
            if (AccessLevel < AccessLevel.GameMaster && item.IsChildOf(this.Backpack))
            {
                int maxWeight = WeightOverloading.GetMaxWeight(this);
                int curWeight = Mobile.BodyWeight + this.TotalWeight;

                if (curWeight > maxWeight)
                    this.SendLocalizedMessage(1019035, true, String.Format(" : {0} / {1}", curWeight, maxWeight));
            }
        }

        public override bool CanBeHarmful(Mobile target, bool message, bool ignoreOurBlessedness)
        {
            if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
                return false;

            if ((target is BaseCreature && ((BaseCreature)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier || target is BaseCannonGuard)
            {
                if (message)
                {
                    if (target.Title == null)
                        SendMessage("{0} cannot be harmed.", target.Name);
                    else
                        SendMessage("{0} {1} cannot be harmed.", target.Name, target.Title);
                }

                return false;
            }

            return base.CanBeHarmful(target, message, ignoreOurBlessedness);
        }

        public override bool CanBeBeneficial(Mobile target, bool message, bool allowDead)
        {
            if (m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null))
                return false;

            return base.CanBeBeneficial(target, message, allowDead);
        }

        public override bool CheckContextMenuDisplay(IEntity target)
        {
            return (m_DesignContext == null);
        }

        public override void OnItemAdded(Item item)
        {
            base.OnItemAdded(item);

            if (item is BaseArmor || item is BaseWeapon)
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if (this.NetState != null)
                CheckLightLevels(false);

            // IPY HUE TALISMANS
            // This is a little messy but the OnItemAdded needs to be checked to give a fairly smooth user experience.

            // arena talismans
            if (Region is ArenaSystem.ArenaCombatRegion)
            {
                Item italisman = FindItemOnLayer(Layer.Talisman);
                ArenaRewardTotem arena_talisman = italisman as ArenaRewardTotem;
                if (arena_talisman != null)
                {
                    foreach (Item i in Items)
                        arena_talisman.HueItem(i);
                }
            }

            // OCB talismans
            Item ocbtalisman = FindItemOnLayer(Layer.Talisman);
            HerosTalisman heroTalisman = ocbtalisman as HerosTalisman;
            VeteranKnightsTalisman veteranTalisman = ocbtalisman as VeteranKnightsTalisman;

            if (heroTalisman != null || veteranTalisman != null)
            {
                foreach (Item i in Items)
                {
                    if (heroTalisman != null)
                        heroTalisman.HueItem(i);
                    if (veteranTalisman != null)
                        veteranTalisman.HueItem(i);
                }
            }
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item is BaseArmor || item is BaseWeapon)
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if (this.NetState != null)
                CheckLightLevels(false);

            // IPY ARENA ONLY HUES
            item.Hue = item.OriginalHue;
            // IPY ARENA ONLY HUES
        }

        public override double ArmorRating
        {
            get
            {
                //BaseArmor ar;
                double rating = 0.0;

                AddArmorRating(ref rating, NeckArmor);
                AddArmorRating(ref rating, HandArmor);
                AddArmorRating(ref rating, HeadArmor);
                AddArmorRating(ref rating, ArmsArmor);
                AddArmorRating(ref rating, LegsArmor);
                AddArmorRating(ref rating, ChestArmor);
                AddArmorRating(ref rating, ShieldArmor);

                return VirtualArmor + VirtualArmorMod + rating;
            }
        }

        private void AddArmorRating(ref double rating, Item armor)
        {
            BaseArmor ar = armor as BaseArmor;

            if (ar != null && (!Core.AOS || ar.ArmorAttributes.MageArmor == 0))
                rating += ar.ArmorRatingScaled;
        }

        #region [Stats]Max
        [CommandProperty(AccessLevel.GameMaster)]
        public override int HitsMax
        {
            get
            {
                int strBase = this.RawStr;
                int strOffs = GetStatOffset(StatType.Str);

                /*		if ( Core.AOS )
                        {
                            strBase = this.Str;
                            strOffs += AosAttributes.GetValue( this, AosAttribute.BonusHits );
                        }
                        else
                        {
                            strBase = this.RawStr;
                        }
                */
                return strBase + strOffs;
                //	return strBase;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int StamMax
        {
            get { return base.StamMax + AosAttributes.GetValue(this, AosAttribute.BonusStam); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int ManaMax
        {
            get { return base.ManaMax + AosAttributes.GetValue(this, AosAttribute.BonusMana) + ((Core.ML && Race == Race.Elf) ? 20 : 0); }
        }
        #endregion

        #region Stat Getters/Setters

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Str
        {
            get
            {
                if (Core.ML && this.AccessLevel == AccessLevel.Player)
                    return Math.Min(base.Str, 150);

                return base.Str;
            }
            set
            {
                base.Str = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Int
        {
            get
            {
                if (Core.ML && this.AccessLevel == AccessLevel.Player)
                    return Math.Min(base.Int, 150);

                return base.Int;
            }
            set
            {
                base.Int = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override int Dex
        {
            get
            {
                if (Core.ML && this.AccessLevel == AccessLevel.Player)
                    return Math.Min(base.Dex, 150);

                return base.Dex;
            }
            set
            {
                base.Dex = value;
            }
        }

        #endregion

        public override bool Move(Direction d)
        {
            NetState ns = this.NetState;

            if (ns != null)
            {
                if (HasGump(typeof(ResurrectGump)) || HasGump(typeof(Server.Custom.Paladin.MurdererDeathGump)))
                {
                    if (Alive)
                    {
                        CloseGump(typeof(ResurrectGump));
                        CloseGump(typeof(Server.Custom.Paladin.MurdererDeathGump));
                    }
                    else
                    {
                        SendLocalizedMessage(500111); // You are frozen and cannot move.
                        return false;
                    }
                }
            }

            if (CloseBankRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseBankRunebookGump = false;
            }

            int speed = ComputeMovementSpeed(d);

            bool res;

            if (!Alive)
                Server.Movement.MovementImpl.IgnoreMovableImpassables = true;

            res = base.Move(d);

            Server.Movement.MovementImpl.IgnoreMovableImpassables = false;

            if (!res)
                return false;

            m_NextMovementTime += speed;

            return true;
        }

        public override bool CheckMovement(Direction d, out int newZ)
        {
            DesignContext context = m_DesignContext;

            if (context == null)
                return base.CheckMovement(d, out newZ);

            HouseFoundation foundation = context.Foundation;

            newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

            int newX = this.X, newY = this.Y;
            Movement.Movement.Offset(d, ref newX, ref newY);

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            return (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map);
        }       

        public SkillName[] AnimalFormRestrictedSkills { get { return m_AnimalFormRestrictedSkills; } }

        private SkillName[] m_AnimalFormRestrictedSkills = new SkillName[]
		{
			SkillName.ArmsLore,	SkillName.Begging, SkillName.Discordance, SkillName.Forensics,
			SkillName.Inscribe, SkillName.ItemID, SkillName.Meditation, SkillName.Peacemaking,
			SkillName.Provocation, SkillName.RemoveTrap, SkillName.SpiritSpeak, SkillName.Stealing,	
			SkillName.TasteID
		};

        public override bool AllowSkillUse(SkillName skill)
        {
            if (AnimalForm.UnderTransformation(this))
            {
                for (int i = 0; i < m_AnimalFormRestrictedSkills.Length; i++)
                {
                    if (m_AnimalFormRestrictedSkills[i] == skill)
                    {
                        SendLocalizedMessage(1070771); // You cannot use that skill in this form.
                        return false;
                    }
                }
            }

            #region Dueling
            if (m_DuelContext != null && !m_DuelContext.AllowSkillUse(this, skill))
                return false;
            #endregion

            return DesignContext.Check(this);
        }

        private bool m_LastProtectedMessage;
        private int m_NextProtectionCheck = 10;

        public virtual void RecheckTownProtection()
        {
            m_NextProtectionCheck = 10;

            Regions.GuardedRegion reg = (Regions.GuardedRegion)this.Region.GetRegion(typeof(Regions.GuardedRegion));
            bool isProtected = (reg != null && !reg.IsDisabled());

            if (isProtected != m_LastProtectedMessage)
            {
                if (isProtected)
                    SendLocalizedMessage(500112); // You are now under the protection of the town guards.
                else
                    SendLocalizedMessage(500113); // You have left the protection of the town guards.

                m_LastProtectedMessage = isProtected;
            }
        }

        public override void MoveToWorld(Point3D loc, Map map)
        {
            base.MoveToWorld(loc, map);
            if (CloseBankRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseBankRunebookGump = false;
            }
            RecheckTownProtection();
        }

        public override void SetLocation(Point3D loc, bool isTeleport)
        {
            if (CloseBankRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseBankRunebookGump = false;
            }

            if (!isTeleport && AccessLevel == AccessLevel.Player)
            {
                // moving, not teleporting
                int zDrop = (this.Location.Z - loc.Z);

                if (zDrop > 20) // we fell more than one story
                    Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
            }

            base.SetLocation(loc, isTeleport);

            if (isTeleport || --m_NextProtectionCheck == 0)
                RecheckTownProtection();
        }

        public override void UpdateRegion()
        {
            Region newRegion = Region.Find(Location, Map);

            if (UOACZPersistance.Active && Region is UOACZRegion && !(newRegion is UOACZRegion) && Map != Map.Internal)
                UOACZSystem.PlayerExitUOACZRegion(this);

            base.UpdateRegion();
        }

        public override void OnRegionChange(Region Old, Region New)
        {
            // IPY ACHIEVEMENT (Exploration)
            if (New.IndexedName == IndexedRegionName.NotIndexed)
                return;

            // IPY Special notification hack hack (Enter orc fort)
            if (New.IndexedName == IndexedRegionName.YewOrcFort_IPY)
                SendMessage("Beware! You are entering Orc Territory!");

            //Check For Potential Temporary Statloss 
            if (Region.IsTempStatlossRegion(New))
                EnterContestedRegion(false);

            // Fast region check (no hierarchical string-comp crap)
            switch (New.IndexedName)
            {
                case IndexedRegionName.HedgeMaze: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreHedgeMaze); break;
                case IndexedRegionName.Britain: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreBritain); break;
                case IndexedRegionName.Wind: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreWind); break;
                case IndexedRegionName.BuccaneeersDen: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreBuccsDen); break;
                case IndexedRegionName.Cove: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreCove); break;
                case IndexedRegionName.Jhelom: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreJhelom); break;
                case IndexedRegionName.Magincia: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreMagincia); break;
                case IndexedRegionName.Minoc: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreMinoc); break;
                case IndexedRegionName.Moonglow: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreMoonglow); break;
                case IndexedRegionName.Nujelm: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreNujelm); break;
                case IndexedRegionName.Occlo: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreOcclo); break;
                case IndexedRegionName.SerpentsHold: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreSerpentsHold); break;
                case IndexedRegionName.SkaraBrae: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreSkaraBrae); break;
                case IndexedRegionName.Trinsic: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreTrinsic); break;
                case IndexedRegionName.Vesper: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreVesper); break;
                case IndexedRegionName.Yew: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreYew); break;
                case IndexedRegionName.Despise:
                    AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreDespise);
                    DailyAchievement.TickProgress(Category.Newb, this, NewbCategory.VisitDespise);

                    break;
                case IndexedRegionName.Deceit: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreDeceit); break;
                case IndexedRegionName.Destard: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreDestard); break;
                case IndexedRegionName.Wrong: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreWrong); break;
                case IndexedRegionName.Covetous: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreCovetous); break;
                case IndexedRegionName.Shame:
                    AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreShame);
                    DailyAchievement.TickProgress(Category.Newb, this, NewbCategory.VisitShame);
                    break;
                case IndexedRegionName.Hythloth: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreHythloth); break;
                case IndexedRegionName.FireDungeon: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreFireDungeon); break;
                case IndexedRegionName.IceDungeon: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreIceDungeon); break;
                case IndexedRegionName.TerathanKeep: AchievementSystem.Instance.TickProgress(this, AchievementTriggers.Trigger_ExploreTerathanKeep); break;
                default:
                    break;
            }
            // IPY ACHIEVEMENT (Exploration)
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from == this)
            {
                if (m_Quest != null)
                    m_Quest.GetContextMenuEntries(list);

                // IPY Gump
                list.Add(new CallbackEntry(10008, new ContextCallback(ShowIPYGump)));
                // IPY Gump

                if (Alive && InsuranceEnabled)
                {
                    list.Add(new CallbackEntry(6201, new ContextCallback(ToggleItemInsurance)));

                    if (AutoRenewInsurance)
                        list.Add(new CallbackEntry(6202, new ContextCallback(CancelRenewInventoryInsurance)));
                    else
                        list.Add(new CallbackEntry(6200, new ContextCallback(AutoRenewInventoryInsurance)));
                }

                BaseHouse house = BaseHouse.FindHouseAt(this);

                if (house != null)
                {
                    if (Alive && house.InternalizedVendors.Count > 0 && house.IsOwner(this))
                        list.Add(new CallbackEntry(6204, new ContextCallback(GetVendor)));

                    //if (house.IsAosRules)
                    //    list.Add(new CallbackEntry(6207, new ContextCallback(LeaveHouse)));
                }

                if (m_JusticeProtectors.Count > 0)
                    list.Add(new CallbackEntry(6157, new ContextCallback(CancelProtection)));

                //if( Alive )
                //	list.Add( new CallbackEntry( 6210, new ContextCallback( ToggleChampionTitleDisplay ) ) );
            }
        }

        private void CancelProtection()
        {
            for (int i = 0; i < m_JusticeProtectors.Count; ++i)
            {
                Mobile prot = m_JusticeProtectors[i];

                string args = String.Format("{0}\t{1}", this.Name, prot.Name);

                prot.SendLocalizedMessage(1049371, args); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
                this.SendLocalizedMessage(1049371, args); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
            }

            m_JusticeProtectors.Clear();
        }

        #region Insurance

        private static int GetInsuranceCost(Item item)
        {
            return 600; // TODO
        }

        private void ToggleItemInsurance()
        {
            if (!CheckAlive())
                return;

            BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
            SendLocalizedMessage(1060868); // Target the item you wish to toggle insurance status on <ESC> to cancel
        }

        public void ShowIPYGump()
        {
            this.SendGump(new IPYGump(this));
        }

        private bool CanInsure(Item item)
        {
            if ((item is Container && !(item is BaseQuiver)) || item is BagOfSending || item is KeyRing || item is PotionKeg)
                return false;

            if (item.Stackable)
                return false;

            if (item.LootType == LootType.Cursed)
                return false;

            if (item.ItemID == 0x204E) // death shroud
                return false;

            if (item.Layer == Layer.Mount)
                return false;

            if (item.LootType == LootType.Blessed || item.LootType == LootType.Newbied || item.BlessedFor == this)
            {
                //SendLocalizedMessage( 1060870, "", 0x23 ); // That item is blessed and does not need to be insured
                return false;
            }

            return true;
        }

        private void ToggleItemInsurance_Callback(Mobile from, object obj)
        {
            if (!CheckAlive())
                return;

            ToggleItemInsurance_Callback(from, obj as Item, true);
        }

        private void ToggleItemInsurance_Callback(Mobile from, Item item, bool target)
        {
            if (item == null || !item.IsChildOf(this))
            {
                if (target)
                    BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));

                SendLocalizedMessage(1060871, "", 0x23); // You can only insure items that you have equipped or that are in your backpack
            }
            else if (item.Insured)
            {
                item.Insured = false;

                SendLocalizedMessage(1060874, "", 0x35); // You cancel the insurance on the item

                if (target)
                {
                    BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
                    SendLocalizedMessage(1060868, "", 0x23); // Target the item you wish to toggle insurance status on <ESC> to cancel
                }
            }
            else if (!CanInsure(item))
            {
                if (target)
                    BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));

                SendLocalizedMessage(1060869, "", 0x23); // You cannot insure that
            }
            else
            {
                if (!item.PayedInsurance)
                {
                    int cost = GetInsuranceCost(item);

                    if (Banker.Withdraw(from, cost))
                    {
                        SendLocalizedMessage(1060398, cost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                        item.PayedInsurance = true;
                    }
                    else
                    {
                        SendLocalizedMessage(1061079, "", 0x23); // You lack the funds to purchase the insurance
                        return;
                    }
                }

                item.Insured = true;

                SendLocalizedMessage(1060873, "", 0x23); // You have insured the item

                if (target)
                {
                    BeginTarget(-1, false, TargetFlags.None, new TargetCallback(ToggleItemInsurance_Callback));
                    SendLocalizedMessage(1060868, "", 0x23); // Target the item you wish to toggle insurance status on <ESC> to cancel
                }
            }
        }

        private void AutoRenewInventoryInsurance()
        {
            if (!CheckAlive())
                return;

            SendLocalizedMessage(1060881, "", 0x23); // You have selected to automatically reinsure all insured items upon death
            AutoRenewInsurance = true;
        }

        private void CancelRenewInventoryInsurance()
        {
            if (!CheckAlive())
                return;

            if (Core.SE)
            {
                if (!HasGump(typeof(CancelRenewInventoryInsuranceGump)))
                    SendGump(new CancelRenewInventoryInsuranceGump(this, null));
            }
            else
            {
                SendLocalizedMessage(1061075, "", 0x23); // You have cancelled automatically reinsuring all insured items upon death
                AutoRenewInsurance = false;
            }
        }

        private class CancelRenewInventoryInsuranceGump : Gump
        {
            private PlayerMobile m_Player;
            private ItemInsuranceMenuGump m_InsuranceGump;

            public CancelRenewInventoryInsuranceGump(PlayerMobile player, ItemInsuranceMenuGump insuranceGump)
                : base(250, 200)
            {
                m_Player = player;
                m_InsuranceGump = insuranceGump;

                AddBackground(0, 0, 240, 142, 0x13BE);
                AddImageTiled(6, 6, 228, 100, 0xA40);
                AddImageTiled(6, 116, 228, 20, 0xA40);
                AddAlphaRegion(6, 6, 228, 142);

                AddHtmlLocalized(8, 8, 228, 100, 1071021, 0x7FFF, false, false); // You are about to disable inventory insurance auto-renewal.

                AddButton(6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 118, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

                AddButton(114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(148, 118, 450, 20, 1071022, 0x7FFF, false, false); // DISABLE IT!
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (!m_Player.CheckAlive())
                    return;

                if (info.ButtonID == 1)
                {
                    m_Player.SendLocalizedMessage(1061075, "", 0x23); // You have cancelled automatically reinsuring all insured items upon death
                    m_Player.AutoRenewInsurance = false;
                }
                else
                {
                    m_Player.SendLocalizedMessage(1042021); // Cancelled.
                }

                if (m_InsuranceGump != null)
                    m_Player.SendGump(m_InsuranceGump.NewInstance());
            }
        }

        private void OpenItemInsuranceMenu()
        {
            if (!CheckAlive())
                return;

            List<Item> items = new List<Item>();

            foreach (Item item in Items)
            {
                if (DisplayInItemInsuranceGump(item))
                    items.Add(item);
            }

            Container pack = Backpack;

            if (pack != null)
                items.AddRange(pack.FindItemsByType<Item>(true, DisplayInItemInsuranceGump));

            // TODO: Investigate item sorting

            CloseGump(typeof(ItemInsuranceMenuGump));

            if (items.Count == 0)
                SendLocalizedMessage(1114915, "", 0x35); // None of your current items meet the requirements for insurance.
            else
                SendGump(new ItemInsuranceMenuGump(this, items.ToArray()));
        }

        private bool DisplayInItemInsuranceGump(Item item)
        {
            return ((item.Visible || AccessLevel >= AccessLevel.GameMaster) && (item.Insured || CanInsure(item)));
        }

        private class ItemInsuranceMenuGump : Gump
        {
            private PlayerMobile m_From;
            private Item[] m_Items;
            private bool[] m_Insure;
            private int m_Page;

            public ItemInsuranceMenuGump(PlayerMobile from, Item[] items)
                : this(from, items, null, 0)
            {
            }

            public ItemInsuranceMenuGump(PlayerMobile from, Item[] items, bool[] insure, int page)
                : base(25, 50)
            {
                m_From = from;
                m_Items = items;

                if (insure == null)
                {
                    insure = new bool[items.Length];

                    for (int i = 0; i < items.Length; ++i)
                        insure[i] = items[i].Insured;
                }

                m_Insure = insure;
                m_Page = page;

                AddPage(0);

                AddBackground(0, 0, 520, 510, 0x13BE);
                AddImageTiled(10, 10, 500, 30, 0xA40);
                AddImageTiled(10, 50, 500, 355, 0xA40);
                AddImageTiled(10, 415, 500, 80, 0xA40);
                AddAlphaRegion(10, 10, 500, 485);

                AddButton(15, 470, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(50, 472, 80, 20, 1011012, 0x7FFF, false, false); // CANCEL

                if (from.AutoRenewInsurance)
                    AddButton(360, 10, 9723, 9724, 1, GumpButtonType.Reply, 0);
                else
                    AddButton(360, 10, 9720, 9722, 1, GumpButtonType.Reply, 0);

                AddHtmlLocalized(395, 14, 105, 20, 1114122, 0x7FFF, false, false); // AUTO REINSURE

                AddButton(395, 470, 0xFA5, 0xFA6, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(430, 472, 50, 20, 1006044, 0x7FFF, false, false); // OK

                AddHtmlLocalized(10, 14, 150, 20, 1114121, 0x7FFF, false, false); // <CENTER>ITEM INSURANCE MENU</CENTER>

                AddHtmlLocalized(45, 54, 70, 20, 1062214, 0x7FFF, false, false); // Item
                AddHtmlLocalized(250, 54, 70, 20, 1061038, 0x7FFF, false, false); // Cost
                AddHtmlLocalized(400, 54, 70, 20, 1114311, 0x7FFF, false, false); // Insured

                int balance = Banker.GetBalance(from);
                int cost = 0;

                for (int i = 0; i < items.Length; ++i)
                {
                    if (insure[i])
                        cost += GetInsuranceCost(items[i]);
                }

                AddHtmlLocalized(15, 420, 300, 20, 1114310, 0x7FFF, false, false); // GOLD AVAILABLE:
                AddLabel(215, 420, 0x481, balance.ToString());
                AddHtmlLocalized(15, 435, 300, 20, 1114123, 0x7FFF, false, false); // TOTAL COST OF INSURANCE:
                AddLabel(215, 435, 0x481, cost.ToString());

                if (cost != 0)
                {
                    AddHtmlLocalized(15, 450, 300, 20, 1114125, 0x7FFF, false, false); // NUMBER OF DEATHS PAYABLE:
                    AddLabel(215, 450, 0x481, (balance / cost).ToString());
                }

                for (int i = page * 4, y = 72; i < (page + 1) * 4 && i < items.Length; ++i, y += 75)
                {
                    Item item = items[i];
                    Rectangle2D b = ItemBounds.Table[item.ItemID];

                    AddImageTiledButton(40, y, 0x918, 0x918, 0, GumpButtonType.Page, 0, item.ItemID, item.Hue, 40 - b.Width / 2 - b.X, 30 - b.Height / 2 - b.Y);
                    AddItemProperty(item.Serial);

                    if (insure[i])
                    {
                        AddButton(400, y, 9723, 9724, 100 + i, GumpButtonType.Reply, 0);
                        AddLabel(250, y, 0x481, GetInsuranceCost(item).ToString());
                    }
                    else
                    {
                        AddButton(400, y, 9720, 9722, 100 + i, GumpButtonType.Reply, 0);
                        AddLabel(250, y, 0x66C, GetInsuranceCost(item).ToString());
                    }
                }

                if (page >= 1)
                {
                    AddButton(15, 380, 0xFAE, 0xFAF, 3, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(50, 380, 450, 20, 1044044, 0x7FFF, false, false); // PREV PAGE
                }

                if ((page + 1) * 4 < items.Length)
                {
                    AddButton(400, 380, 0xFA5, 0xFA7, 4, GumpButtonType.Reply, 0);
                    AddHtmlLocalized(435, 380, 70, 20, 1044045, 0x7FFF, false, false); // NEXT PAGE
                }
            }

            public ItemInsuranceMenuGump NewInstance()
            {
                return new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0 || !m_From.CheckAlive())
                    return;

                switch (info.ButtonID)
                {
                    case 1: // Auto Reinsure
                        {
                            if (m_From.AutoRenewInsurance)
                            {
                                if (!m_From.HasGump(typeof(CancelRenewInventoryInsuranceGump)))
                                    m_From.SendGump(new CancelRenewInventoryInsuranceGump(m_From, this));
                            }
                            else
                            {
                                m_From.AutoRenewInventoryInsurance();
                                m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page));
                            }

                            break;
                        }
                    case 2: // OK
                        {
                            m_From.SendGump(new ItemInsuranceMenuConfirmGump(m_From, m_Items, m_Insure, m_Page));

                            break;
                        }
                    case 3: // Prev
                        {
                            if (m_Page >= 1)
                                m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page - 1));

                            break;
                        }
                    case 4: // Next
                        {
                            if ((m_Page + 1) * 4 < m_Items.Length)
                                m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page + 1));

                            break;
                        }
                    default:
                        {
                            int idx = info.ButtonID - 100;

                            if (idx >= 0 && idx < m_Items.Length)
                                m_Insure[idx] = !m_Insure[idx];

                            m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page));

                            break;
                        }
                }
            }
        }

        private class ItemInsuranceMenuConfirmGump : Gump
        {
            private PlayerMobile m_From;
            private Item[] m_Items;
            private bool[] m_Insure;
            private int m_Page;

            public ItemInsuranceMenuConfirmGump(PlayerMobile from, Item[] items, bool[] insure, int page)
                : base(250, 200)
            {
                m_From = from;
                m_Items = items;
                m_Insure = insure;
                m_Page = page;

                AddBackground(0, 0, 240, 142, 0x13BE);
                AddImageTiled(6, 6, 228, 100, 0xA40);
                AddImageTiled(6, 116, 228, 20, 0xA40);
                AddAlphaRegion(6, 6, 228, 142);

                AddHtmlLocalized(8, 8, 228, 100, 1114300, 0x7FFF, false, false); // Do you wish to insure all newly selected items?

                AddButton(6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0);
                AddHtmlLocalized(40, 118, 450, 20, 1060051, 0x7FFF, false, false); // CANCEL

                AddButton(114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(148, 118, 450, 20, 1073996, 0x7FFF, false, false); // ACCEPT
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (!m_From.CheckAlive())
                    return;

                if (info.ButtonID == 1)
                {
                    for (int i = 0; i < m_Items.Length; ++i)
                    {
                        Item item = m_Items[i];

                        if (item.Insured != m_Insure[i])
                            m_From.ToggleItemInsurance_Callback(m_From, item, false);
                    }
                }
                else
                {
                    m_From.SendLocalizedMessage(1042021); // Cancelled.
                    m_From.SendGump(new ItemInsuranceMenuGump(m_From, m_Items, m_Insure, m_Page));
                }
            }
        }

        #endregion

        private void ToggleTrades()
        {
            RefuseTrades = !RefuseTrades;
        }

        private void GetVendor()
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (CheckAlive() && house != null && house.IsOwner(this) && house.InternalizedVendors.Count > 0)
            {
                CloseGump(typeof(ReclaimVendorGump));
                SendGump(new ReclaimVendorGump(house));
            }
        }

        //private void LeaveHouse()
        //{
        //    BaseHouse house = BaseHouse.FindHouseAt(this);

        //    if (house != null)
        //        this.Location = house.BanLocation;
        //}

        private delegate void ContextCallback();

        private class CallbackEntry : ContextMenuEntry
        {
            private ContextCallback m_Callback;

            public CallbackEntry(int number, ContextCallback callback)
                : this(number, -1, callback)
            {
            }

            public CallbackEntry(int number, int range, ContextCallback callback)
                : base(number, range)
            {
                m_Callback = callback;
            }

            public override void OnClick()
            {
                if (m_Callback != null)
                    m_Callback();
            }
        }

        public override void DisruptiveAction()
        {
            if (Meditating)
            {
                RemoveBuff(BuffIcon.ActiveMeditation);
            }

            base.DisruptiveAction();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this == from && !Warmode)
            {
                IMount mount = Mount;

                if (mount != null && !DesignContext.Check(this))
                    return;
            }

            base.OnDoubleClick(from);
        }

        public override void DisplayPaperdollTo(Mobile to)
        {
            if (DesignContext.Check(this))
                base.DisplayPaperdollTo(to);
        }

        private static bool m_NoRecursion;

        public override bool CheckEquip(Item item)
        {
            if (!base.CheckEquip(item))
                return false;

            //UOACZ
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Undead)
                return false;

            #region Dueling
            if (m_DuelContext != null && !m_DuelContext.AllowItemEquip(this, item))
                return false;
            #endregion

            #region Factions
            Factions.FactionItem factionItem = Factions.FactionItem.Find(item);

            if (factionItem != null)
            {
                Factions.Faction faction = Factions.Faction.Find(this);

                if (faction == null)
                {
                    SendLocalizedMessage(1010371); // You cannot equip a faction item!
                    return false;
                }
                else if (faction != factionItem.Faction)
                {
                    SendLocalizedMessage(1010372); // You cannot equip an opposing faction's item!
                    return false;
                }
                else
                {
                    int maxWearables = Factions.FactionItem.GetMaxWearables(this);

                    for (int i = 0; i < Items.Count; ++i)
                    {
                        Item equiped = Items[i];

                        if (item != equiped && Factions.FactionItem.Find(equiped) != null)
                        {
                            if (--maxWearables == 0)
                            {
                                SendLocalizedMessage(1010373); // You do not have enough rank to equip more faction items!
                                return false;
                            }
                        }
                    }
                }
            }
            #endregion

            if (this.AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && this.HasTrade)
            {
                BounceInfo bounce = item.GetBounce();

                if (bounce != null)
                {
                    if (bounce.m_Parent is Item)
                    {
                        Item parent = (Item)bounce.m_Parent;

                        if (parent == this.Backpack || parent.IsChildOf(this.Backpack))
                            return true;
                    }
                    else if (bounce.m_Parent == this)
                    {
                        return true;
                    }
                }

                SendLocalizedMessage(1004042); // You can only equip what you are already carrying while you have a trade pending.
                return false;
            }

            return true;
        }

        public override bool CheckTrade(Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            int msgNum = 0;

            //// no trades allowed inside the arena areas
            //if (Region is ArenaRegion || to.Region is ArenaRegion)
            //{
            //    SendMessage("Trading is not allowed in this area");
            //    return false;
            //}

            if (cont == null)
            {
                if (to.Holding != null)
                    msgNum = 1062727; // You cannot trade with someone who is dragging something.
                else if (this.HasTrade)
                    msgNum = 1062781; // You are already trading with someone else!
                else if (to.HasTrade)
                    msgNum = 1062779; // That person is already involved in a trade
                else if (to is PlayerMobile && ((PlayerMobile)to).RefuseTrades)
                    msgNum = 1154111; // ~1_NAME~ is refusing all trades.
            }

            if (msgNum == 0)
            {
                if (cont != null)
                {
                    plusItems += cont.TotalItems;
                    plusWeight += cont.TotalWeight;
                }

                if (this.Backpack == null || !this.Backpack.CheckHold(this, item, false, checkItems, plusItems, plusWeight))
                    msgNum = 1004040; // You would not be able to hold this if the trade failed.
                else if (to.Backpack == null || !to.Backpack.CheckHold(to, item, false, checkItems, plusItems, plusWeight))
                    msgNum = 1004039; // The recipient of this trade would not be able to carry this.
                else
                    msgNum = CheckContentForTrade(item);
            }

            if (msgNum != 0)
            {
                if (message)
                {
                    if (msgNum == 1154111)
                        SendLocalizedMessage(msgNum, to.Name);
                    else
                        SendLocalizedMessage(msgNum);
                }

                return false;
            }

            return true;
        }

        private static int CheckContentForTrade(Item item)
        {
            if (item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None)
                return 1004044; // You may not trade trapped items.

            if (SkillHandlers.StolenItem.IsStolen(item))
                return 1004043; // You may not trade recently stolen items.

            if (item is Container)
            {
                foreach (Item subItem in item.Items)
                {
                    int msg = CheckContentForTrade(subItem);

                    if (msg != 0)
                        return msg;
                }
            }

            return 0;
        }

        public override bool CheckNonlocalDrop(Mobile from, Item item, Item target)
        {
            if (!base.CheckNonlocalDrop(from, item, target))
                return false;

            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            Container pack = this.Backpack;
            if (from == this && this.HasTrade && (target == pack || target.IsChildOf(pack)))
            {
                BounceInfo bounce = item.GetBounce();

                if (bounce != null && bounce.m_Parent is Item)
                {
                    Item parent = (Item)bounce.m_Parent;

                    if (parent == pack || parent.IsChildOf(pack))
                        return true;
                }

                SendLocalizedMessage(1004041); // You can't do that while you have a trade pending.
                return false;
            }

            return true;
        }

        protected override void OnLocationChange(Point3D oldLocation)
        {
            CheckLightLevels(false);

            // Check For Entering a Stat-Loss Triggering Zone
            if (SpellHelper.IsFeluccaDungeon(Map, Location) || SpellHelper.IsDungeonBossArea(Map, Location) || SpellHelper.IsGraveYardArea(Map, Location))
                EnterContestedRegion(false);

            BaseBoat boat = BaseBoat.FindBoatAt(Location, Map);

            if (boat == null)
                m_BoatOccupied = null;
            else
                m_BoatOccupied = boat;

            #region Dueling
            if (m_DuelContext != null)
                m_DuelContext.OnLocationChanged(this);
            #endregion

            DesignContext context = m_DesignContext;

            if (context == null || m_NoRecursion)
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            int newX = this.X, newY = this.Y;
            int newZ = foundation.Z + HouseFoundation.GetLevelZ(context.Level, context.Foundation);

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            if (newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map)
            {
                if (Z != newZ)
                    Location = new Point3D(X, Y, newZ);

                m_NoRecursion = false;
                return;
            }

            Location = new Point3D(foundation.X, foundation.Y, newZ);
            Map = foundation.Map;          

            m_NoRecursion = false;
        }

        public override bool OnMoveOver(Mobile m)
        {
            BaseCreature bc_Creature = m as BaseCreature;

            if (bc_Creature != null)
            {
                bool allowMoveOver = (!Alive || !m.Alive || IsDeadBondedPet || m.IsDeadBondedPet) || (Hidden && AccessLevel > AccessLevel.Player);

                if (allowMoveOver)
                    return true;
            }

            #region Dueling
            if (Region.IsPartOf(typeof(Engines.ConPVP.SafeZone)) && m is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)m;

                if (pm.DuelContext == null || pm.DuelPlayer == null || !pm.DuelContext.Started || pm.DuelContext.Finished || pm.DuelPlayer.Eliminated)
                    return true;
            }
            #endregion

            return base.OnMoveOver(m);
        }

        public override bool CheckShove(Mobile shoved)
        {
            if (Spectating && !(Region is BattlegroundRegion))
                Spectating = false;            
            
            bool InStamFreeRange = (int)GetDistanceToSqrt(StamFreeMoveSource) <= BaseCreature.StamFreeMoveRange;

            //Currently Allowed Stamina-Free Movement
            if (StamFreeMoveExpiration > DateTime.UtcNow && InStamFreeRange || BoatOccupied != null)
                return true;            
            
            if (shoved.Blessed)
                return true;

            else if (TransformationSpellHelper.UnderTransformation(this, typeof(WraithFormSpell)))
                return true;

            else if (UOACZSystem.IsUOACZValidMobile(this))
            {
                if (!shoved.Alive || !Alive || shoved.IsDeadBondedPet || IsDeadBondedPet)
                    return true;

                else if (shoved.Hidden && shoved.AccessLevel > AccessLevel.Player)
                    return true;

                if (shoved.Spectating || Spectating)
                    return true;

                if (!Pushing)
                {
                    Pushing = true;

                    int number;

                    if (this.AccessLevel > AccessLevel.Player)
                        number = shoved.Hidden ? 1019041 : 1019040;

                    else
                    {
                        if (Stam >= 10)
                        {
                            number = shoved.Hidden ? 1019043 : 1019042;
                            Stam -= 10;                            
                        }

                        else
                            return false;
                    }

                    SendLocalizedMessage(number);
                }

                return true;
            }

            return base.CheckShove(shoved);
        }

        protected override void OnMapChange(Map oldMap)
        {
            if (oldMap == Map.Ilshenar)              
                this.LightLevel = 0;
           
            if (AccessLevel == AccessLevel.Player)
                if (Mount != null)
                    Mount.Rider = null;

            if ((Map != Factions.Faction.Facet && oldMap == Factions.Faction.Facet) || (Map == Factions.Faction.Facet && oldMap != Factions.Faction.Facet))
                InvalidateProperties();

            #region Dueling
            if (m_DuelContext != null)
                m_DuelContext.OnMapChanged(this);
            #endregion

            DesignContext context = m_DesignContext;

            if (context == null || m_NoRecursion)
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            if (Map != foundation.Map)
                Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override void OnBeneficialAction(Mobile target, bool isCriminal)
        {
            if (Region is UOACZRegion)
            {
                base.OnBeneficialAction(target, isCriminal);
                return;
            }

            if (m_SentHonorContext != null)
                m_SentHonorContext.OnSourceBeneficialAction(target);

            if (!(this.Region is BattlegroundRegion))
            {
                PlayerMobile pm_Target = target as PlayerMobile;

                if (Paladin && pm_Target != null)
                {
                    if (pm_Target.Murderer)
                    {
                        SendMessage(PaladinEvents.paladinDismissalTextHue, "Your aiding of a murderer is counterproductive towards the goal of The Order. You are no longer a Paladin of Trinsic, but may seek reinstatement in " + PaladinEvents.PaladinAidingMurdererDays .ToString() + " days time.");

                        if (PaladinEvents.RemoveAllPaladinsOnAccount(this, PaladinEvents.PaladinAidingMurdererDays))
                            SendMessage(PaladinEvents.paladinDismissalTextHue, "All of your paladin associates have been dismissed from the guild.");
                    }                    
                }

                if (target is BaseFactionGuard)
                {
                    var guard = target as BaseFactionGuard;
                    if (CitizenshipPlayerState != null && guard.Town != null)
                    {
                        if (Citizenship == guard.Town)
                            AssistedOwnMilitia = true;
                    }
                }

                if (pm_Target != null)
                {
                    Town targetTown = pm_Target.Citizenship;
                    bool militia = pm_Target.IsInMilitia;

                    if (Citizenship != null && targetTown != null && (militia || pm_Target.AssistedOwnMilitia))
                    {
                        if (Citizenship == targetTown)
                            AssistedOwnMilitia = true;
                    }
                }

            }

            base.OnBeneficialAction(target, isCriminal);
        }

        public override void CriminalAction(bool message)
        {
            base.CriminalAction(message);

            //Not Currently on Paladin Probation
            if (Paladin && m_PaladinProbationExpiration < DateTime.UtcNow && !(Region is UOACZRegion))
            {
                PlaySound(0x5CE);                
                Effects.SendLocationParticles(EffectItem.Create(Location, Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2051, 0, 5029, 0);

                SendMessage(PaladinEvents.paladinDismissalTextHue, "Your criminal act is counterproductive towards the goal of The Order and you are now on probation for the next " + PaladinEvents.PaladinCriminalPunishmentHours + " hours.");

                m_PaladinProbationExpiration = DateTime.UtcNow + TimeSpan.FromHours(PaladinEvents.PaladinCriminalPunishmentHours);
            }
        }

        public override bool CheckDisrupt(int damage, Mobile from)
        {
            bool disrupt = true;

            BaseCreature bc_From = from as BaseCreature;

            if (bc_From != null)
            {
                if (bc_From.IsControlledCreature())
                {
                    double disruptChance = (double)damage * bc_From.TamedDamageAgainstPlayerDisruptChance;

                    if (Utility.RandomDouble() >= disruptChance)
                        disrupt = false;                    
                }
            }

            return disrupt;
        }
        
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {            
            BaseCreature bc_From = from as BaseCreature;

            BandageContext bandageContext = BandageContext.GetContext(this);

            if (bandageContext != null)
            {
                bool causeSlip = true;

                if (bc_From != null)
                {
                    if (!CheckDisrupt(amount, bc_From))
                        causeSlip = false;
                }               

                if (causeSlip)
                    bandageContext.Slip();              
            }

            if (Confidence.IsRegenerating(this))
                Confidence.StopRegenerating(this);

            WeightOverloading.FatigueOnDamage(this, amount, 1.0);

            if (m_ReceivedHonorContext != null)
                m_ReceivedHonorContext.OnTargetDamaged(from, amount);

            if (m_SentHonorContext != null)
                m_SentHonorContext.OnSourceDamaged(from, amount);

            if (willKill && from is PlayerMobile)
                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(((PlayerMobile)from).RecoverAmmo));

            if (Paladin == true)
                PaladinEvents.PaladinOnHit(amount, from, this, willKill);

            if (this.Region is BattlegroundRegion && from is PlayerMobile)
            {
                Battleground bg = ((BattlegroundRegion)this.Region).Battleground;
                if (bg.Active && ((PlayerMobile)from) != this)
                    bg.CurrentScoreboard.UpdateScore(from as PlayerMobile, Categories.Damage, amount);
            }

            //IPY OC LEADERBOARD (Sean)
            var fromPlayer = from as PlayerMobile;
            if (fromPlayer != null && fromPlayer.IsInMilitia && this.IsInMilitia && this.Citizenship != fromPlayer.Citizenship)
            {
                Custom.Townsystem.Town town = Custom.Townsystem.Town.FromRegion(from.Region);
                if (town != null && Custom.Townsystem.OCTimeSlots.IsActiveTown(town))
                {
                    Server.Custom.Townsystem.OCLeaderboard.RegisterDamage(from, amount);
                }
            }

            base.OnDamage(amount, from, willKill);
        }

        public virtual void AddToSpellTolerance(String spellName, Mobile from)
        {
            double expirationDuration = creatureSpellExpiration;

            //Ignore Self Damage
            if (from == this)
                return;

            if (from is PlayerMobile)
                expirationDuration = playerSpellExpiration;

            SpellEntry spellEntry = new SpellEntry(spellName, from, DateTime.UtcNow + TimeSpan.FromSeconds(expirationDuration));

            m_SpellEntries.Add(spellEntry);

            if (m_SpellToleranceTimer == null)
                m_SpellToleranceTimer = new SpellToleranceTimer(this);

            if (!m_SpellToleranceTimer.Running)
                m_SpellToleranceTimer.Start();
        }

        public const double playerSpellExpiration = 1; //Length in seconds a Player Spell Exists in Spell Tolerance List
        public const double creatureSpellExpiration = 1; //Length in seconds a Creature Spell Exists in Spell Tolerance List

        public SpellToleranceTimer m_SpellToleranceTimer;

        public PlayerCombatTimer m_PlayerCombatTimer;

        public class PlayerCombatTimer : Timer
        {
            private PlayerMobile m_Player;

            public PlayerCombatTimer(PlayerMobile player): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Player = player;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Player == null)
                    Stop();

                if (m_Player.Deleted)
                    Stop();

                if (m_Player.LastPlayerCombatTime + m_Player.PlayerCombatExpirationDelay < DateTime.UtcNow)
                {
                    BaseDungeonArmor.CheckForAndUpdateDungeonArmorProperties(m_Player);
                    Stop();
                }
            }
        }

        public class SpellToleranceTimer : Timer
        {
            private PlayerMobile m_Owner;

            public SpellToleranceTimer(PlayerMobile player): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Owner = player;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                //Entries to Remove
                List<SpellEntry> entriesToRemove = new List<SpellEntry>();

                if (m_Owner.m_SpellEntries != null)
                {
                    for (int a = 0; a < m_Owner.m_SpellEntries.Count; a++)
                    {
                        SpellEntry entry = m_Owner.m_SpellEntries[a];

                        //Spell Expired
                        if (entry.m_Expiration <= DateTime.UtcNow)
                        {
                            entriesToRemove.Add(entry);
                        }
                    }

                    //Spell Expired: Remove From Entries List
                    int entriesCount = entriesToRemove.Count;

                    for (int a = 0; a < entriesCount; a++)
                    {
                        m_Owner.m_SpellEntries.Remove(entriesToRemove[a]);
                    }

                    //No Active Spells Left: Stop Timer
                    if (m_Owner.m_SpellEntries.Count == 0)
                    {
                        this.Stop();
                    }
                }
            }
        }

        public override void OnAfterResurrect()
        {
            if (!Warmode && NetState != null && AccessLevel == Server.AccessLevel.Player)
            {
                IPooledEnumerable enu = GetClientsInRange(Core.GlobalMaxUpdateRange);
                foreach (NetState state in enu)
                {
                    if (state == null || NetState == null || state.Mobile == this || !CanSee(state.Mobile))
                        continue;

                    Send(MobileIncoming.Create(NetState, this, state.Mobile));

                    if (NetState.StygianAbyss)
                    {
                        if (Poison != null)
                            Send(new HealthbarPoison(state.Mobile));

                        if (Blessed || YellowHealthbar)
                            Send(new HealthbarYellow(state.Mobile));
                    }

                    if (IsDeadBondedPet)
                        Send(new BondedStatus(0, state.Mobile.Serial, 1));

                    if (ObjectPropertyList.Enabled)
                    {
                        Send(OPLPacket);
                    }

                }

                enu.Free();
            }

            //Reapply Kin Paint
            if (KinPaintHue != -1 && KinPaintExpiration > DateTime.UtcNow)
            {
                HueMod = KinPaintHue;
            }

            //After Ressurection Check if Player is Ressing in Contested Region While in Penance (Potential for Temp Stat Loss)
            Region region = Region.Find(Location, Map);

            if (Region.IsTempStatlossRegion(region))
                EnterContestedRegion(true);

            //Player Enhancement Customization: Lifegiver
            bool reborn = PlayerEnhancementPersistance.IsCustomizationEntryActive(this, CustomizationType.Reborn);

            if (reborn)            
                CustomizationAbilities.Reborn(this);            
        }

        private void StartGhostScoutTimer()
        {
            if (m_GhostScoutTimer != null)
            {
                m_GhostScoutTimer.Stop();
                m_GhostScoutTimer = null;
            }

            m_GhostScoutTimer = new GhostScoutingTimer(this);
            m_GhostScoutTimer.Start();
        }

        public override void Resurrect()
        {
            m_TimeSpanResurrected = this.GameTime;

            if (KilledByPaladin)
                KilledByPaladin = false;

            if (CloseBankRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseBankRunebookGump = false;
            }

            bool wasAlive = this.Alive;

            base.Resurrect();

            if (Alive && !wasAlive && !(Region is UOACZRegion))
            {
                Item deathRobe = new DeathRobe();

                if (Backpack.FindItemByType<DeathRobe>() != null)
                    deathRobe.Delete();

                else if (!EquipItem(deathRobe))
                    deathRobe.Delete();

                if (m_GhostScoutTimer != null)
                {
                    m_GhostScoutTimer.Stop();
                    m_GhostScoutTimer = null;
                }
            }
        }

        public override double RacialSkillBonus
        {
            get
            {
                if (Core.ML && this.Race == Race.Human)
                    return 20.0;

                return 0;
            }
        }

        public override void OnWarmodeChanged()
        {
            if (!Alive && NetState != null && AccessLevel == Server.AccessLevel.Player)
            {                
                IPooledEnumerable enu = GetClientsInRange(Core.GlobalMaxUpdateRange);
                try
                {
                    foreach (NetState state in enu)
                    {
                        if (state == null || NetState == null)
                            continue;

                        if (state.Mobile == this || state.Mobile == null)
                            continue;

                        if (state.Mobile.AccessLevel > AccessLevel.Player)
                            continue;

                        if (!Warmode)
                        {
                            if (Utility.InUpdateRange(state.Mobile.Location, Location))
                                Send(state.Mobile.RemovePacket);
                        }

                        else
                        {
                            if (state.Mobile.Alive && !state.Mobile.Hidden)
                            {
                                Send(MobileIncoming.Create(NetState, this, state.Mobile));

                                if (NetState.StygianAbyss)
                                {
                                    if (Poison != null)
                                        Send(new HealthbarPoison(state.Mobile));

                                    if (Blessed || YellowHealthbar)
                                        Send(new HealthbarYellow(state.Mobile));
                                }

                                if (IsDeadBondedPet)
                                    Send(new BondedStatus(0, state.Mobile.Serial, 1));

                                if (ObjectPropertyList.Enabled)
                                    Send(OPLPacket);
                            }
                        }

                    }
                }
                catch (NullReferenceException exception)
                {
                    // SNUFF IT LOL
                }

                enu.Free();
            }

            if (!Warmode)
                Timer.DelayCall(TimeSpan.FromSeconds(10), new TimerCallback(RecoverAmmo));
        }

        private Mobile m_InsuranceAward;
        private int m_InsuranceCost;
        private int m_InsuranceBonus;

        private List<Item> m_EquipSnapshot;

        public List<Item> EquipSnapshot
        {
            get { return m_EquipSnapshot; }
        }

        public void YoungPlayerChat(string text)
        {
            string message = string.Format("[{0}{1}]: {2}", Name, Companion ? " [Companion]" : "", text);
            foreach (var young in YoungChatListeners)
                young.SendMessage(32, message);
        }
        
        public override bool OnBeforeDeath()
        {
            StartGhostScoutTimer();

            NetState state = NetState;

            if (state != null)
                state.CancelAllTrades();
            
            m_EquipSnapshot = new List<Item>(this.Items);

            m_NonAutoreinsuredItems = 0;
            m_InsuranceCost = 0;
            m_InsuranceAward = base.FindMostRecentDamager(false);

            if (m_InsuranceAward is BaseCreature)
            {
                Mobile master = ((BaseCreature)m_InsuranceAward).GetMaster();

                if (master != null)
                    m_InsuranceAward = master;
            }

            if (m_InsuranceAward != null && (!m_InsuranceAward.Player || m_InsuranceAward == this))
                m_InsuranceAward = null;

            if (m_InsuranceAward is PlayerMobile)
                ((PlayerMobile)m_InsuranceAward).m_InsuranceBonus = 0;

            if (m_ReceivedHonorContext != null)
                m_ReceivedHonorContext.OnTargetKilled();
            if (m_SentHonorContext != null)
                m_SentHonorContext.OnSourceKilled();

            RecoverAmmo();

            Custom.Townsystem.Faction.HandleDeath(this, base.FindMostRecentDamager(true));

            DropHolding(); // IPY : After townsystem handle death

            return base.OnBeforeDeath();
        }

        private bool CheckInsuranceOnDeath(Item item)
        {
            if (InsuranceEnabled && item.Insured)
            {
                #region Dueling
                if (m_DuelPlayer != null && m_DuelContext != null && m_DuelContext.Registered && m_DuelContext.Started && !m_DuelPlayer.Eliminated)
                    return true;
                #endregion

                if (AutoRenewInsurance)
                {
                    int cost = GetInsuranceCost(item);

                    if (m_InsuranceAward != null)
                        cost /= 2;

                    if (Banker.Withdraw(this, cost))
                    {
                        m_InsuranceCost += cost;
                        item.PayedInsurance = true;
                        SendLocalizedMessage(1060398, cost.ToString()); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                    }
                    else
                    {
                        SendLocalizedMessage(1061079, "", 0x23); // You lack the funds to purchase the insurance
                        item.PayedInsurance = false;
                        item.Insured = false;
                        m_NonAutoreinsuredItems++;
                    }
                }
                else
                {
                    item.PayedInsurance = false;
                    item.Insured = false;
                }

                if (m_InsuranceAward != null)
                {
                    if (Banker.Deposit(m_InsuranceAward, 300))
                    {
                        if (m_InsuranceAward is PlayerMobile)
                            ((PlayerMobile)m_InsuranceAward).m_InsuranceBonus += 300;
                    }
                }

                return true;
            }

            return false;
        }

        public override DeathMoveResult GetParentMoveResultFor(Item item)
        {
            // It seems all items are unmarked on death, even blessed/insured ones
            if (item.QuestItem)
                item.QuestItem = false;

            if (CheckInsuranceOnDeath(item))
                return DeathMoveResult.MoveToBackpack;

            DeathMoveResult res = base.GetParentMoveResultFor(item);

            if (res == DeathMoveResult.MoveToCorpse && item.Movable && (Region.IsBlessedRegion || (Young && !Criminal)))
                res = DeathMoveResult.MoveToBackpack;

            return res;
        }

        public override DeathMoveResult GetInventoryMoveResultFor(Item item)
        {
            DeathMoveResult result;

            // It seems all items are unmarked on death, even blessed/insured ones
            if (item.QuestItem)
                item.QuestItem = false;

            //PlayerClass Items
            if (item.PlayerClass != PlayerClass.None && !(Region is BattlegroundRegion))
            {
                if (item is BaseWeapon || item is Spellbook)
                {
                    item.PlayerClass = PlayerClass.None;
                    item.PlayerClassOwner = null;
                    item.PlayerClassRestricted = false;

                    item.Hue = 0;
                }
            }

            if (CheckInsuranceOnDeath(item))
                return DeathMoveResult.MoveToBackpack;

            result = base.GetInventoryMoveResultFor(item);

            if (result == DeathMoveResult.MoveToCorpse && item.Movable && (Region.IsBlessedRegion || (Young && !Criminal)))            
                result = DeathMoveResult.MoveToBackpack;            

            return result;
        }

        public override void OnDeath(Container c)
        {
            if (m_NonAutoreinsuredItems > 0)
            {
                SendLocalizedMessage(1061115);
            }

            base.OnDeath(c);

            SpecialAbilities.ClearSpecialEffects(this);

            if (CloseBankRunebookGump)
            {
                CloseGump(typeof(RunebookGump));
                CloseBankRunebookGump = false;
            }

            BandageContext bandageContext = BandageContext.GetContext(this);

            if (bandageContext != null)
                bandageContext.StopHeal();

            m_EquipSnapshot = null;

            if (KinPaintHue == -1)
                HueMod = -1;

            NameMod = null;
            SetHairMods(-1, -1);

            PolymorphSpell.StopTimer(this);
            IncognitoSpell.StopTimer(this);
            DisguiseTimers.RemoveTimer(this);

            EndAction(typeof(PolymorphSpell));
            EndAction(typeof(IncognitoSpell));

            MeerMage.StopEffect(this, false);

            SkillHandlers.StolenItem.ReturnOnDeath(this, c);

            if (m_PermaFlags.Count > 0)
            {
                m_PermaFlags.Clear();

                if (c is Corpse)
                    ((Corpse)c).Criminal = true;

                if (SkillHandlers.Stealing.ClassicMode)
                    Criminal = true;
            }

            //Determine if Murdered
            List<Mobile> killers = new List<Mobile>();
            List<Mobile> toGive = new List<Mobile>();

            foreach (AggressorInfo ai in this.Aggressors)
            {
                if (ai != null && ai.Attacker != null && ai.Attacker.Player && ai.CanReportMurder && !ai.Reported)
                {
                    if (ai.Attacker.AccessLevel == Server.AccessLevel.Player)
                    {
                        killers.Add(ai.Attacker);

                        ai.Reported = true;
                        ai.CanReportMurder = false;
                    }
                }

                if (ai != null && ai.Attacker != null && ai.Attacker.Player && (DateTime.UtcNow - ai.LastCombatTime) < TimeSpan.FromSeconds(30.0) && !toGive.Contains(ai.Attacker))
                    toGive.Add(ai.Attacker);
            }

            foreach (AggressorInfo ai in this.Aggressed)
            {
                if (ai != null && ai.Defender != null && ai.Defender.Player && (DateTime.UtcNow - ai.LastCombatTime) < TimeSpan.FromSeconds(30.0) && !toGive.Contains(ai.Defender))
                    toGive.Add(ai.Defender);
            }

            foreach (Mobile g in toGive)
            {
                int n = Notoriety.Compute(g, this);

                int theirKarma = this.Karma, ourKarma = g.Karma;
                bool innocent = (n == Notoriety.Innocent);
                bool criminal = (n == Notoriety.Criminal || n == Notoriety.Murderer);

                int fameAward = this.Fame / 200;
                int karmaAward = 0;

                if (innocent)
                    karmaAward = (ourKarma > -2500 ? -850 : -110 - (this.Karma / 100));

                else if (criminal)
                    karmaAward = 50;

                Server.Misc.Titles.AwardFame(g, fameAward, false);
                Server.Misc.Titles.AwardKarma(g, karmaAward, true);
            }

            if (NpcGuild == NpcGuild.ThievesGuild)
                return;

            bool justiceDisabledZone = IsInArenaFight || (Region is BattlegroundRegion) || DuelContext != null ||
                                        SpellHelper.InBuccs(Map, Location) || SpellHelper.InYewOrcFort(Map, Location) || SpellHelper.InYewCrypts(Map, Location) ||
                                        GreyZoneTotem.InGreyZoneTotemArea(Location, Map) || Hotspot.InHotspotArea(Location, Map, true);
            //Has Valid Killer
            if (killers.Count > 0)
            {
                //Justice-Enabled Zone
                if (!justiceDisabledZone && !(Region is UOACZRegion))
                {
                    Point3D randomLocation = BaseOrb.GetOrbWildernessRandomLocation(Location, Map);
                    new ReportMurdererGump.GumpTimer(this, DeathEventType.Player, killers, DateTime.UtcNow, Location, randomLocation, Map).Start();
                }
            }

            #region UOACZ
           
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (IsUOACZHuman)
            {
                foreach (Mobile mobile in killers)
                {                
                    PlayerMobile playerKiller = mobile as PlayerMobile;

                    if (playerKiller == null)
                        continue;

                    if (playerKiller.IsUOACZHuman)
                    {
                        UOACZPersistance.CheckAndCreateUOACZAccountEntry(playerKiller);
                        playerKiller.m_UOACZAccountEntry.HumanPlayersKilledAsHuman++;

                        m_UOACZAccountEntry.HumanProfile.CauseOfDeath = UOACZAccountEntry.HumanProfileEntry.CauseOfDeathType.PlayerHuman;

                        UOACZSystem.ChangeStat(playerKiller, UOACZSystem.UOACZStatType.Honor, UOACZSystem.CommitMurderHonorLoss, true);                        
                        
                        foreach (NetState state in NetState.Instances)
                        {
                            Mobile m_Mobile = state.Mobile;
                            PlayerMobile player = m_Mobile as PlayerMobile;

                            if (player == null)
                                continue;

                            if (UOACZRegion.ContainsMobile(player))
                                player.SendMessage(UOACZSystem.redTextHue, playerKiller.Name + " [Human] has killed " + Name + " [Human].");
                        }

                        AchievementSystemImpl.Instance.TickProgressMulti(playerKiller, AchievementTriggers.Trigger_UOACZMurderAnotherHuman, 1);
                    }
                }
            }

            #endregion

            //Player, Paladin, and Murderer Handling
            bool killedByPlayer = false;
            bool killedByPaladin = false;
            bool killedByMurderer = false;

            double totalPlayerDamage = 0;
            double totalPaladinDamage = 0;
            double totalMurdererDamage = 0;

            int totalDamage = 0;

            bool playerInGuild = Guild != null;

            Dictionary<PlayerMobile, int> damageInflicted = new Dictionary<PlayerMobile, int>();

            //If Not in Justice-Free Zone
            if (!justiceDisabledZone)
            {
                //Damage Entries for Player
                foreach (DamageEntry de in this.DamageEntries)
                {
                    if (de == null)
                        continue;

                    if (de.HasExpired)
                        continue;

                    if (de.Damager == this)
                        continue;

                    if (de.LastDamage + TimeSpan.FromSeconds(DamageEntryClaimExpiration) <= DateTime.UtcNow)
                        continue;

                    PlayerMobile playerDamager = de.Damager as PlayerMobile;
                    PlayerMobile creatureOwner = null;
                    BaseCreature bc_Creature = de.Damager as BaseCreature;

                    bool sameGuild = false;

                    //Same Guild: Ignore Damage
                    if (playerDamager != null)
                    {
                        if (Guild != null && playerDamager.Guild != null)
                        {
                            if (Guild == playerDamager.Guild)
                                continue;
                        }

                        if (damageInflicted.ContainsKey(playerDamager))
                            damageInflicted[playerDamager] += de.DamageGiven;

                        else
                            damageInflicted.Add(playerDamager, de.DamageGiven);
                    }

                    //Damager is Creature: And Is Controlled By Someone
                    else if (bc_Creature != null)
                    {
                        if (bc_Creature.Summoned && bc_Creature.SummonMaster != null)
                        {
                            if (bc_Creature.SummonMaster is PlayerMobile)
                                creatureOwner = bc_Creature.SummonMaster as PlayerMobile;
                        }

                        else if (bc_Creature.Controlled && bc_Creature.ControlMaster != null)
                        {
                            if (bc_Creature.ControlMaster is PlayerMobile)
                                creatureOwner = bc_Creature.ControlMaster as PlayerMobile;
                        }

                        else if (bc_Creature.BardProvoked && bc_Creature.BardMaster != null)
                        {
                            if (bc_Creature.BardMaster is PlayerMobile)
                                creatureOwner = bc_Creature.BardMaster as PlayerMobile;
                        }

                        //Creature is Controlled by Player in Some Fashion
                        if (creatureOwner != null)
                        {
                            if (creatureOwner == this)
                                continue;

                            if (creatureOwner.Guild != null && this.Guild != null)
                            {
                                if (this.Guild == creatureOwner.Guild)
                                    continue;
                            }
                            if (damageInflicted.ContainsKey(creatureOwner))
                                damageInflicted[creatureOwner] += de.DamageGiven;
                            else
                                damageInflicted.Add(creatureOwner, de.DamageGiven);
                        }
                    }
                }
            }

            PlayerMobile highestPlayerDamager = null;
            PlayerMobile highestPaladinDamager = null;
            PlayerMobile highestMurdererDamager = null;

            int highestPlayerDamage = 0;
            int highestPaladinDamage = 0;
            int highestMurdererDamage = 0;

            int playerClaimCount = 0;
            int paladinClaimCount = 0;
            int murdererClaimCount = 0;
            
            //Check Player Damage Entries
            foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
            {
                if (pair.Key == null) continue;

                PlayerMobile playerDamager = pair.Key;

                if (playerDamager == null) continue;
                if (playerDamager.Deleted) continue;

                int damageAmount = pair.Value;                

                //Vengeance 
                if (Vengeance.HasVengeanceAgainstTarget(playerDamager, this) && !(playerDamager.Region is UOACZRegion))
                    playerDamager.DecreaseVengeanceEntryPoints(this, Vengeance.DeathPoints);

                //Determine Claims
                totalDamage += damageAmount;
                totalPlayerDamage += damageAmount;

                //Player Damage
                if (damageAmount >= MinIndividualDamageRequiredForDeathClaim)
                {
                    playerClaimCount++;

                    if (IsUOACZHuman)
                    {
                        if (playerDamager.IsUOACZUndead)
                        {   
                            UOACZPersistance.CheckAndCreateUOACZAccountEntry(playerDamager);
                            playerDamager.m_UOACZAccountEntry.HumanPlayersKilledAsUndead++;

                            if (m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                                UOACZSystem.ChangeStat(playerDamager, UOACZSystem.UOACZStatType.UndeadScore, UOACZSystem.UndeadPlayerKillFatiguedHumanPlayerScore, true);
                            else
                                UOACZSystem.ChangeStat(playerDamager, UOACZSystem.UOACZStatType.UndeadScore, UOACZSystem.UndeadPlayerKillHumanPlayerScore, true);
                            
                            foreach (NetState state in NetState.Instances)
                            {
                                Mobile mobile = state.Mobile;
                                PlayerMobile player = mobile as PlayerMobile;

                                if (player == null)
                                    continue;

                                if (UOACZRegion.ContainsMobile(player))
                                    player.SendMessage(UOACZSystem.redTextHue, playerDamager.Name + " [Undead] has killed " + Name + " [Human].");
                            }

                            AchievementSystemImpl.Instance.TickProgressMulti(playerDamager, AchievementTriggers.Trigger_UOACZKillHumanPlayer, 1);

                            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate
                            {
                                if (!UOACZSystem.IsUOACZValidMobile(this)) return;
                                if (!UOACZSystem.IsUOACZValidMobile(playerDamager)) return;

                                if (IsUOACZUndead)
                                    AchievementSystemImpl.Instance.TickProgressMulti(playerDamager, AchievementTriggers.Trigger_UOACZCauseUndeadTransformation, 1);
                            });  
                        }
                    }

                    if (IsUOACZUndead)
                    {
                        if (playerDamager.IsUOACZHuman)
                        {
                            if (m_UOACZAccountEntry.FatigueExpiration > DateTime.UtcNow)
                                UOACZSystem.ChangeStat(playerDamager, UOACZSystem.UOACZStatType.HumanScore, UOACZSystem.HumanPlayerKillFatiguedUndeadPlayerScore, true);
                            else
                                UOACZSystem.ChangeStat(playerDamager, UOACZSystem.UOACZStatType.HumanScore, UOACZSystem.HumanPlayerKillUndeadPlayerScore, true);

                            UOACZPersistance.CheckAndCreateUOACZAccountEntry(playerDamager);
                            playerDamager.m_UOACZAccountEntry.UndeadPlayersKilledAsHuman++;                            

                            foreach (NetState state in NetState.Instances)
                            {
                                Mobile mobile = state.Mobile;
                                PlayerMobile player = mobile as PlayerMobile;

                                if (player == null)
                                    continue;

                                if (UOACZRegion.ContainsMobile(player))
                                    player.SendMessage(UOACZSystem.redTextHue, playerDamager.Name + " [Human] has killed " + Name + " [Undead].");
                            }

                            AchievementSystemImpl.Instance.TickProgressMulti(playerDamager, AchievementTriggers.Trigger_UOACZKillUndeadPlayer, 1);
                        }
                    }

                    if (damageAmount > highestPlayerDamage)
                    {
                        highestPlayerDamager = playerDamager;
                        highestPlayerDamage = damageAmount;
                    }
                }

                //Paladin Damage
                if (playerDamager.Paladin && playerDamager.PaladinProbationExpiration < DateTime.UtcNow)
                {
                    totalPaladinDamage += damageAmount;

                    if (damageAmount >= MinIndividualDamageRequiredForDeathClaim)
                    {
                        paladinClaimCount++;

                        if (damageAmount > highestPaladinDamage)
                        {
                            highestPaladinDamager = playerDamager;
                            highestPaladinDamage = damageAmount;
                        }
                    }
                }

                //Murderer Damage
                else if (playerDamager.Murderer)
                {
                    totalMurdererDamage += damageAmount;

                    if (damageAmount >= MinIndividualDamageRequiredForDeathClaim)
                    {
                        murdererClaimCount++;

                        if (damageAmount > highestMurdererDamage)
                        {
                            highestMurdererDamager = playerDamager;
                            highestMurdererDamage = damageAmount;
                        }
                    }
                }
            }

            //If Non-Instant Killed: i.e by GM or Explosive Chest
            if (totalDamage > 0)
            {
                if (totalPlayerDamage >= MinDamageRequiredForPlayerDeath && playerClaimCount > 0 && highestPlayerDamager != null)
                    killedByPlayer = true;

                if (totalPaladinDamage >= MinDamageRequiredForPaladinDeath && paladinClaimCount > 0 && highestPaladinDamager != null)
                    killedByPaladin = true;

                if (totalMurdererDamage >= MinDamageRequiredForMurdererDeath && murdererClaimCount > 0 && highestMurdererDamager != null)
                    killedByMurderer = true;
            }

            //Player is Murderer
            if (Murderer && !justiceDisabledZone && !(Region is UOACZRegion))
            {
                //Count as Killed By Paladin
                if (killedByPaladin)
                {
                    int punishableCounts = (ShortTermMurders - 5);

                    //Has New Punishable Murder Counts
                    if (punishableCounts >= 1)
                    {
                        m_MurdererDeathGumpNeeded = true;

                        Timer.DelayCall(TimeSpan.FromSeconds(2), delegate { this.SendMessage("You have been slain by a Paladin!"); });

                        int penanceDurationMinutes = PaladinEvents.minimumPaladinPenanceDuration + (int)((double)punishableCounts * (double)PaladinEvents.penancePerMurderCount);

                        if (Citizenship != null && Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Reprieve))
                            penanceDurationMinutes = (int)((double)penanceDurationMinutes * 0.90);

                        if (penanceDurationMinutes > PaladinEvents.maximumPenanceDuration)
                            penanceDurationMinutes = PaladinEvents.maximumPenanceDuration;

                        //Already in Penance
                        if (PenanceExpiration > DateTime.UtcNow)
                            PenanceExpiration = PenanceExpiration + TimeSpan.FromMinutes(penanceDurationMinutes);
                        else
                            PenanceExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(penanceDurationMinutes);
                                                
                        List<Mobile> m_Killers = new List<Mobile>();
                        
                        //Paladin Rewards: Highest Damaging Paladin 
                        PaladinEvents.PaladinKillMurdererResult(highestPaladinDamager, this, true, Location, Map);

                        //Whom to Assign Credit
                        KilledByPaladin = true;
                        m_LastPlayerKilledBy = highestPaladinDamager;

                        m_Killers.Add(highestPaladinDamager);

                        //Send Murderer Death Gump: Final Selections for Discounts
                        Timer.DelayCall(TimeSpan.FromSeconds(1), delegate { CloseAllGumps(); });
                        Timer.DelayCall(TimeSpan.FromSeconds(1.5), delegate
                        {
                            SendGump(new Custom.Paladin.MurdererDeathGump(this, 1));
                        });

                        //Paladin Rewards: Other Paladin Damagers
                        foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
                        {
                            if (pair.Key == null) continue;

                            PlayerMobile otherPlayer = pair.Key;

                            if (otherPlayer == null)
                                continue;

                            int damageAmount = pair.Value;

                            if (otherPlayer.Paladin && damageAmount < MinIndividualDamageRequiredForDeathClaim)                            
                                otherPlayer.SendMessage("You assist in the apprehension of the murderer, however your individual contribution was not significant enough to warrant a reward from The Order.");
                            
                            else if (otherPlayer != highestPaladinDamager && otherPlayer.Paladin && otherPlayer.PaladinProbationExpiration < DateTime.UtcNow)
                            {
                                PaladinEvents.PaladinKillMurdererResult(otherPlayer, this, false, Location, Map);
                                m_Killers.Add(otherPlayer);
                            }
                        }

                        Point3D randomLocation = BaseOrb.GetOrbWildernessRandomLocation(Location, Map);
                        DeathEventEntry deathEventEntry = new DeathEventEntry(this, DeathEventType.Murderer, m_Killers, DateTime.UtcNow, Location, randomLocation, Map);

                        PaladinEvents.AddDeathEventEntry(deathEventEntry);                        
                    }

                    else
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(2), delegate { this.SendMessage("You have been slain by a Paladin, however you have not commited any recent crimes, and therefore receive no punishment."); });

                        highestPaladinDamager.SendMessage("You slay the murderer, however they have recently paid restitution for their crimes and are of no further interest to the Order of the Silver Serpent.");

                        foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
                        {
                            if (pair.Key == null) continue;

                            PlayerMobile otherPlayer = pair.Key;
                            int damageAmount = pair.Value;

                            if (otherPlayer != highestPaladinDamager && otherPlayer.Paladin && otherPlayer.PaladinProbationExpiration < DateTime.UtcNow)
                                otherPlayer.SendMessage("You assist in the apprehension of the murderer, however they have recently paid restitution for their crimes and are of no further interest to the Order of the Silver Serpent.");
                        }
                    }
                }

                //Not Enough Paladin Damage to Qualify as Paladin Killed
                else
                {
                    foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
                    {
                        PlayerMobile player = pair.Key;

                        if (player.Paladin && player.PaladinProbationExpiration < DateTime.UtcNow)
                            player.SendMessage("The vile murderer is slain, however, Paladins of the Order did not contribute enough in their actions to claim responsibility towards their death.");
                    }
                }
            }

            //Player is Paladin
            if (Paladin && !justiceDisabledZone && !(Region is UOACZRegion))
            {
                //Killed By a Murderer
                if (killedByMurderer)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(2), delegate { this.SendMessage("You have been slain by a Murderer!"); });

                    int dishonor = PaladinEvents.dishonorDuration;

                    if (Citizenship != null && Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Reprieve))
                        dishonor = (int)((double)dishonor * 0.9);

                    //Already in Penance
                    if (PenanceExpiration > DateTime.UtcNow)
                        PenanceExpiration = PenanceExpiration + TimeSpan.FromMinutes(dishonor);
                    else
                        PenanceExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(dishonor);

                    List<Mobile> m_Killers = new List<Mobile>();

                    //Highest Damaging Murderer 
                    MurdererEvents.MurdererKillPaladinResult(highestMurdererDamager, this, true);

                    highestMurdererDamager.m_PaladinsKilled.Add(this);

                    //Whom to Assign Credit
                    m_LastPlayerKilledBy = highestMurdererDamager;

                    m_Killers.Add(highestMurdererDamager);

                    //Other Murderer Damagers
                    foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
                    {
                        if (pair.Key == null) continue;

                        PlayerMobile otherPlayer = pair.Key;

                        int damageAmount = pair.Value;

                        if (damageAmount < MinIndividualDamageRequiredForDeathClaim)
                            continue;

                        if (otherPlayer.Murderer && otherPlayer != highestMurdererDamager)
                        {
                            MurdererEvents.MurdererKillPaladinResult(otherPlayer, this, false);

                            m_Killers.Add(otherPlayer);
                            otherPlayer.m_PaladinsKilled.Add(this);
                        }
                    }

                    Point3D randomLocation = BaseOrb.GetOrbWildernessRandomLocation(Location, Map);
                    DeathEventEntry deathEventEntry = new DeathEventEntry(this, DeathEventType.Paladin, m_Killers, DateTime.UtcNow, Location, randomLocation, Map);
                    PaladinEvents.AddDeathEventEntry(deathEventEntry);
                }

                else
                {
                    foreach (KeyValuePair<PlayerMobile, int> pair in damageInflicted)
                    {
                        PlayerMobile player = pair.Key;

                        if (player.Murderer)
                            player.SendMessage("The cowardly paladin is slain, however, you do not feel that you can rightly claim responsibility for their well-deserved death.");
                    }
                }
            }
            
            //Last Mobile to Damage Player
            Mobile killer = FindMostRecentDamager(true);

            PlayerMobile pm_Killer = killer as PlayerMobile;

            if (killer is BaseCreature)
            {
                BaseCreature bc = (BaseCreature)killer;

                Mobile master = bc.GetMaster();

                if (master != null)
                    killer = master;
            }

            if (highestPlayerDamager != null)
            {
                // check for totem on killer
                Item finisher = highestPlayerDamager.FindItemOnLayer(Layer.Talisman);

                if (finisher != null && finisher is FinisherTotem && !(Region is UOACZRegion))
                {
                    ((FinisherTotem)finisher).PerformFinisher(Location, Map);
                }                
            }

            //Player Enhancement Customization: Carnage and Violent Death
            bool carnage = PlayerEnhancementPersistance.IsCustomizationEntryActive(killer, CustomizationType.Carnage);
            bool violentDeath = PlayerEnhancementPersistance.IsCustomizationEntryActive(this, CustomizationType.ViolentDeath);
            
            if (carnage && Utility.RandomDouble() >= .75)
                carnage = false;

            if (violentDeath && Utility.RandomDouble() >= .75)
                violentDeath = false;           

            if ((carnage || violentDeath) && !(Region is UOACZRegion))
                CustomizationAbilities.PlayerDeathExplosion(Location, Map, carnage, violentDeath);  

            if (!(Region is BattlegroundRegion))
                Server.Guilds.Guild.HandleDeath(this, killer);

            #region Dueling
            if (m_DuelContext != null)
                m_DuelContext.OnDeath(this, c);
            #endregion

            if (Region is BattlegroundRegion)
            {
                ((BattlegroundRegion)this.Region).HandleDeath(this, c);
                Battleground bg = ((BattlegroundRegion)this.Region).Battleground;
                if (bg.Active && LastKiller is PlayerMobile)
                {
                    if (LastKiller != this)
                    {
                        bg.CurrentScoreboard.UpdateScore(LastKiller as PlayerMobile, Categories.Kills, 1);
                        DailyAchievement.TickProgress(Category.PvP, (PlayerMobile)LastKiller, PvPCategory.KillPlayers);
                    }
                    bg.CurrentScoreboard.UpdateScore(this, Categories.Deaths, 1);
                    bg.KillFeedBroadcast(LastKiller as PlayerMobile, this);
                }
            }

            if (m_BuffTable != null)
            {
                List<BuffInfo> list = new List<BuffInfo>();

                foreach (BuffInfo buff in m_BuffTable.Values)
                {
                    if (!buff.RetainThroughDeath)
                    {
                        list.Add(buff);
                    }
                }

                for (int i = 0; i < list.Count; i++)
                {
                    RemoveBuff(list[i]);
                }
            }

            if (this.YewJailed)
                this.m_YewJailItem.OnDeath();

            OnWarmodeChanged();
        }

        public void ConsiderSins()
        {
            this.SendMessage("Murder Counts: {0}", ShortTermMurders);
            this.SendMessage("Lifetime Murder Counts: {0}", Kills);

            int days;
            int hours;
            int minutes;
            int seconds;

            string sTime = "";

            if (this.ShortTermMurders > 0)
            {
                days = Math.Abs((GameTime - m_ShortTermElapse).Days);
                hours = Math.Abs((GameTime - m_ShortTermElapse).Hours);
                minutes = Math.Abs((GameTime - m_ShortTermElapse).Minutes);

                if (minutes >= 60)
                    hours++;

                if (hours >= 24)
                    days++;

                sTime = "";

                if (days > 1)
                    sTime += days.ToString() + " days ";

                else if (days == 1)
                    sTime += days.ToString() + " day ";

                if (hours > 1)
                    sTime += hours.ToString() + " hours ";

                else if (hours == 1)
                    sTime += hours.ToString() + " hour ";

                if (minutes > 1)
                    sTime += minutes.ToString() + " minutes ";

                else if (minutes == 1)
                    sTime += minutes.ToString() + " minute ";

                sTime = sTime.Trim();

                if (sTime != "")
                    SendMessage("Your next murder count will decay in " + sTime + ".");
            }

            if (!Alive && RestitutionFee > 0)
            {
                int restitutionRemaining = RestitutionFee;

                SendMessage("You are responsible for the restitution of " + restitutionRemaining.ToString() + " gold before you are freely resurrectable.");
            }

            //Find Longest Current Penance Timer on Account
            DateTime longestPenance = DateTime.UtcNow;

            Account acc = Account as Account;

            for (int i = 0; i < (acc.Length - 1); i++)
            {
                Mobile m = acc.accountMobiles[i] as Mobile;

                if (m != null)
                {
                    PlayerMobile player = m as PlayerMobile;

                    if (player != null)
                    {
                        if (player.m_PenanceExpiration > DateTime.UtcNow && player.m_PenanceExpiration > longestPenance)
                            longestPenance = player.m_PenanceExpiration;

                        break;
                    }
                }
            }

            if (longestPenance > DateTime.UtcNow)
            {
                days = longestPenance.Subtract(DateTime.UtcNow).Days;
                hours = longestPenance.Subtract(DateTime.UtcNow).Hours;
                minutes = longestPenance.Subtract(DateTime.UtcNow).Minutes;

                if (minutes >= 60)
                    hours++;

                if (hours >= 24)
                    days++;

                sTime = "";

                if (days > 1)
                    sTime += days.ToString() + " days ";

                else if (days == 1)
                    sTime += days.ToString() + " day ";

                if (hours > 1)
                    sTime += hours.ToString() + " hours ";

                else if (hours == 1)
                    sTime += hours.ToString() + " hour ";

                if (minutes > 1)
                    sTime += minutes.ToString() + " minutes ";

                else if (minutes == 1)
                    sTime += minutes.ToString() + " minute ";

                sTime = sTime.Trim();

                if (sTime != "")
                {
                    if (IsInTempStatLoss)
                        this.SendMessage("You are currently in temporary statloss and will remain so for another " + sTime + ".");

                    else if (!IsInTempStatLoss)
                        SendMessage("Your account is under the risk of temporary statloss for another " + sTime + " if you enter any dungeon or contested area.");
                }
            }

            //Paladin Timers
            if (!Murderer)
            {
                if (PaladinRejoinAllowed > DateTime.UtcNow)
                {
                    days = PaladinRejoinAllowed.Subtract(DateTime.UtcNow).Days;
                    hours = PaladinRejoinAllowed.Subtract(DateTime.UtcNow).Hours;
                    minutes = PaladinRejoinAllowed.Subtract(DateTime.UtcNow).Minutes;

                    if (minutes >= 60)
                        hours++;

                    if (hours >= 24)
                        days++;

                    sTime = "";

                    if (days > 1)
                        sTime += days.ToString() + " days ";
                    else if (days == 1)
                        sTime += days.ToString() + " day ";

                    if (hours > 1)
                        sTime += hours.ToString() + " hours ";
                    else if (hours == 1)
                        sTime += hours.ToString() + " hour ";

                    if (minutes > 1)
                        sTime += minutes.ToString() + " minutes ";
                    else if (minutes == 1)
                        sTime += minutes.ToString() + " minute ";

                    sTime = sTime.Trim();

                    if (sTime != "")
                        SendMessage("You may rejoin the Paladin Order in " + sTime + ".");
                }

                else if (PaladinProbationExpiration > DateTime.UtcNow)
                {
                    days = PaladinProbationExpiration.Subtract(DateTime.UtcNow).Days;
                    hours = PaladinProbationExpiration.Subtract(DateTime.UtcNow).Hours;
                    minutes = PaladinProbationExpiration.Subtract(DateTime.UtcNow).Minutes;

                    if (minutes >= 60)
                        hours++;

                    if (hours >= 24)
                        days++;

                    sTime = "";

                    if (days > 1)
                        sTime += days.ToString() + " days ";
                    else if (days == 1)
                        sTime += days.ToString() + " day ";

                    if (hours > 1)
                        sTime += hours.ToString() + " hours ";
                    else if (hours == 1)
                        sTime += hours.ToString() + " hour ";

                    if (minutes > 1)
                        sTime += minutes.ToString() + " minutes ";
                    else if (minutes == 1)
                        sTime += minutes.ToString() + " minute ";

                    sTime = sTime.Trim();

                    if (sTime != "")
                        SendMessage("You are on probation within the Paladin Order for another " + sTime + ".");
                }
            }
        }

        private List<Mobile> m_PermaFlags;
        private List<Mobile> m_VisList;
        private Hashtable m_AntiMacroTable;
        private TimeSpan m_GameTime;
        private TimeSpan m_BankGameTime;
        private DateTime m_SessionStart;
        private DateTime m_LastEscortTime;
        private DateTime m_LastPetBallTime;
        private DateTime m_NextSmithBulkOrder;
        private DateTime m_NextTailorBulkOrder;
        private DateTime m_SavagePaintExpiration;
        private SkillName m_Learning = (SkillName)(-1);
        private DateTime m_NextFireAttempt = DateTime.MinValue;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextFireAttempt
        {
            get { return m_NextFireAttempt; }
            set { try { m_NextFireAttempt = value; } catch { } }
        }

        public SkillName Learning
        {
            get { return m_Learning; }
            set { m_Learning = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan SavagePaintExpiration
        {
            get
            {
                TimeSpan ts = m_SavagePaintExpiration - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                m_SavagePaintExpiration = DateTime.UtcNow + value;
            }
        }

        private int m_KinPaintHue = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int KinPaintHue
        {
            get { return m_KinPaintHue; }
            set { m_KinPaintHue = value; }
        }

        private DateTime m_KinPaintExpiration = DateTime.MinValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime KinPaintExpiration
        {
            get { return m_KinPaintExpiration; }
            set { m_KinPaintExpiration = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextSmithBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextSmithBulkOrder - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextSmithBulkOrder = DateTime.UtcNow + value; }
                catch { }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextTailorBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextTailorBulkOrder - DateTime.UtcNow;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try { m_NextTailorBulkOrder = DateTime.UtcNow + value; }
                catch { }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastEscortTime
        {
            get { return m_LastEscortTime; }
            set { m_LastEscortTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastPetBallTime
        {
            get { return m_LastPetBallTime; }
            set { m_LastPetBallTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HideExiledStatus { get { return NameMod != null && NameMod.Length > 0; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool HideMurdererStatus { get; set; }

        public PlayerMobile()
        {
            m_ShortTermMurders = new List<Mobile>();
            m_PaladinsKilled = new List<Mobile>();

            m_minOrderJoinTime = TimeSpan.Zero;

            m_LastTarget = Serial.MinusOne;
            m_AutoStabled = new List<Mobile>();

            m_VisList = new List<Mobile>();
            m_PermaFlags = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();

            m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            m_GameTime = TimeSpan.Zero;

            m_ShortTermElapse = TimeSpan.FromHours(MurderCountDecayHours);
            m_LongTermElapse = TimeSpan.FromHours(0.0);

            m_JusticeProtectors = new List<Mobile>();
            m_GuildRank = Guilds.RankDefinition.Lowest;

            m_ChampionTitles = new ChampionTitleInfo();
            m_UserOptHideFameTitles = true;

            TitleColorState = new PlayerTitleColors();
            
            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (CheckAccountForPenance(this) && IsInTempStatLoss && (Murderer || Paladin))
                    ApplyTempSkillLoss();
            });

            m_SpecialAbilityEffectEntries = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToAdd = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToRemove = new List<SpecialAbilityEffectEntry>();
        }

        public override bool MutateSpeech(List<Mobile> hears, ref string text, ref object context)
        {
            if (Alive)
                return false;

            if (Core.ML && Skills[SkillName.SpiritSpeak].Value >= 100.0)
                return false;

            for (int i = 0; i < hears.Count; ++i)
            {
                Mobile m = hears[i];

                if (m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0)
                    return false;
            }

            return base.MutateSpeech(hears, ref text, ref context);
        }

        public override void DoSpeech(string text, int[] keywords, MessageType type, int hue)
        {
            if (type == MessageType.Guild) //Guilds.Guild.NewGuildSystem && ( || type == MessageType.Alliance
            {
                Guilds.Guild g = this.Guild as Guilds.Guild;

                if (g == null)                
                    SendLocalizedMessage(1063142); // You are not in a guild!
                
                else	//Type == MessageType.Guild
                {
                    m_GuildMessageHue = hue;

                    g.GuildChat(this, text);
                    SendToStaffMessage(this, "[Guild]: {0}", text);
                }
            }

            else if (type == MessageType.Alliance)
            {

                if (Citizenship != null && ShowTownChat)
                {
                    Citizenship.AllianceChat(this, hue, text);
                    SendToStaffMessage(this, "[Alliance]: {0}", text);
                }

                else
                    SendLocalizedMessage(1071020); // You are not in an alliance!                
            }

            else            
                base.DoSpeech(text, keywords, type, hue);            
        }

        public void SendAllianceMessage(Mobile from, int hue, string text)
        {
            Packet p = null;

            NetState state = this.NetState;

            if (state != null)
            {
                if (p == null)
                    p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Alliance, hue, 3, from.Language, from.Name, text));

                state.Send(p);
            }

            Packet.Release(p);
        }

        public static void SendToStaffMessage(Mobile from, string text)
        {
            Packet p = null;

            foreach (NetState ns in from.GetClientsInRange(8))
            {
                Mobile mob = ns.Mobile;

                if (mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel)
                {
                    if (p == null)
                        p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text));

                    ns.Send(p);
                }
            }

            Packet.Release(p);
        }

        private static void SendToStaffMessage(Mobile from, string format, params object[] args)
        {
            SendToStaffMessage(from, String.Format(format, args));
        }

        public override bool RangeExemption(Mobile mobileTarget)
        {
            if (mobileTarget == null)
                return false;

            double totalValue = 0;

            GetSpecialAbilityEntryValue(SpecialAbilityEffect.Phalanx, out totalValue);

            int extraRange = (int)(Math.Floor(totalValue));

            BaseWeapon weapon = Weapon as BaseWeapon;

            if (weapon != null)
            {
                if (!(weapon is BaseRanged))
                {
                    int adjustedRange = weapon.MaxRange + extraRange;

                    bool foundBlockingItem = false;

                    IPooledEnumerable itemsOnTile = Map.GetItemsInRange(mobileTarget.Location, 1);

                    foreach (Item item in itemsOnTile)
                    {
                        if (Utility.GetDistance(Location, item.Location) > 1)
                            continue;

                        if (item is UOACZStatic || item is UOACZBreakableStatic)
                        {
                            foundBlockingItem = true;
                            break;
                        }
                    }

                    itemsOnTile.Free();

                    if (InRange(mobileTarget, adjustedRange) && foundBlockingItem)
                        return true;
                }
            }

            return false;
        }

        public override bool IsHindered()
        {
            double hinderValue = 0;
            
            GetSpecialAbilityEntryValue(SpecialAbilityEffect.Hinder, out hinderValue);

            if (hinderValue != 0)
                return true;

            return base.IsHindered();
        }

        public virtual void OnGaveMeleeAttack(Mobile defender)
        {
            BaseDungeonArmor.PlayerDungeonArmorProfile attackerDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(this, null);

            if (attackerDungeonArmor.MatchingSet && !attackerDungeonArmor.InPlayerCombat && defender is BaseCreature)
            {
                double flamestrikeChance = attackerDungeonArmor.DungeonArmorDetail.FlamestrikeOnMeleeAttackChance;
                double energySiphonChance = attackerDungeonArmor.DungeonArmorDetail.EnergySiphonOnMeleeAttackChance;

                int effectHue = effectHue = attackerDungeonArmor.DungeonArmorDetail.EffectHue;

                BaseWeapon weapon = Weapon as BaseWeapon;

                if (weapon != null)
                {
                    int weaponSpeedAdjusted = weapon.OldSpeed;

                    if (weaponSpeedAdjusted > 60)
                        weaponSpeedAdjusted = 60;

                    if (weaponSpeedAdjusted < 20)
                        weaponSpeedAdjusted = 20;

                    double speedScalar = 1 + ((60 - (double)weaponSpeedAdjusted) / 40);

                    flamestrikeChance *= speedScalar;
                    energySiphonChance *= speedScalar;
                }

                int damage = Utility.RandomMinMax(40, 60);

                if (Utility.RandomDouble() <= flamestrikeChance)
                {                        
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);
                    SpecialAbilities.FlamestrikeSpecialAbility(1.0, this, defender, damage, 0, -1, true, "", "");                    
                }

                if (Utility.RandomDouble() <= energySiphonChance)
                {   
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, effectHue, 0, 5005, 0);
                    SpecialAbilities.EnergySiphonSpecialAbility(1.0, this, defender, 1.0, 1, -1, true, "You siphon energy from your target.", "");   
                }                
            }
        }

        public virtual void OnGotMeleeAttack(Mobile attacker)
        {
            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(this, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature)
            {
                double flamestrikeChance = defenderDungeonArmor.DungeonArmorDetail.FlamestrikeOnReceiveMeleeHitChance;                
                    
                int damage = Utility.RandomMinMax(40, 60);

                if (Utility.RandomDouble() < flamestrikeChance)
                {
                    Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5005);

                    SpecialAbilities.FlamestrikeSpecialAbility(1.0, null, attacker, damage, 0, -1, true, "", "");
                }
            }                       
        }

        public void PlayerVsPlayerCombatOccured(PlayerMobile pm_From)
        {
            if (pm_From == null)
                return;            

            LastCombatTime = DateTime.UtcNow;
            pm_From.LastCombatTime = DateTime.UtcNow;
                        
            if (Guild != null && pm_From.Guild != null && Guild == pm_From.Guild)
                return;

            if (Party != null && pm_From.Party != null && Party == pm_From.Party)
                return;

            //Cancel Polymorph Potion Effect
            if (!pm_From.CanBeginAction(typeof(PolymorphPotion)) && pm_From.BodyMod != 0)
            {
                pm_From.SendMessage("Your polymorph potion effect fades as you enter combat with another player.");

                pm_From.BodyMod = 0;
                pm_From.HueMod = -1;

                pm_From.EndAction(typeof(PolymorphPotion));
                pm_From.EndAction(typeof(PolymorphSpell));

                BaseArmor.ValidateMobile(pm_From);
            }

            LastPlayerCombatTime = DateTime.UtcNow;
            pm_From.LastPlayerCombatTime = DateTime.UtcNow;            

            BaseDungeonArmor.CheckForAndUpdateDungeonArmorProperties(this);
            CapStatMods(this);

            if (m_PlayerCombatTimer == null)
            {
                m_PlayerCombatTimer = new PlayerCombatTimer(this);
                m_PlayerCombatTimer.Start();
            }

            else
            {
                m_PlayerCombatTimer.Stop();
                m_PlayerCombatTimer = null;

                m_PlayerCombatTimer = new PlayerCombatTimer(this);
                m_PlayerCombatTimer.Start();
            }
        }

        public override int AbsorbDamage(Mobile attacker, int damage, bool physical, bool melee)
        {
            if (!physical)
                return damage;

            BaseCreature bc_Attacker = attacker as BaseCreature;

            //UOACZ Handling
            if (IsUOACZUndead)
            {
                double adjustedVirtualArmor = (double)m_UOACZAccountEntry.UndeadProfile.VirtualArmor;
                double totalValue = 0;
                double pierceScalar = 1;
                double virtualArmorMultiplier = .0025;

                GetSpecialAbilityEntryValue(SpecialAbilityEffect.Fortitude, out totalValue);
                adjustedVirtualArmor += totalValue;

                GetSpecialAbilityEntryValue(SpecialAbilityEffect.Pierce, out totalValue);
                pierceScalar -= totalValue;

                if (pierceScalar > 1)
                    pierceScalar = 1;

                if (pierceScalar < 0)
                    pierceScalar = 0;

                int damageReduction = (int)((double)damage * adjustedVirtualArmor * virtualArmorMultiplier * pierceScalar);

                if (damageReduction > damage)
                    damageReduction = damage;

                damage -= damageReduction;

                totalValue = 0;

                GetSpecialAbilityEntryValue(SpecialAbilityEffect.ShieldOfBones, out totalValue);

                if (totalValue > 0)
                {
                    double damageScalar = 1 - totalValue;

                    if (damageScalar < 0)
                        damageScalar = 0;

                    damage = (int)(Math.Round((double)damage * damageScalar));

                    if (damage < 0)
                        damage = 0;
                }

                return damage;
            }
            
            //Standard OSI Handling: Player vs Player Melee Only
            if (melee)
            {
                //Determine Which Layer of Target Armor Was Hit
                double chance = Utility.RandomDouble();

                Item armorItem;

                if (chance < 0.07)
                    armorItem = NeckArmor;

                else if (chance < 0.14)
                    armorItem = HandArmor;

                else if (chance < 0.28)
                    armorItem = ArmsArmor;

                else if (chance < 0.43)
                    armorItem = HeadArmor;

                else if (chance < 0.65)
                    armorItem = LegsArmor;

                else
                    armorItem = ChestArmor;

                IWearableDurability armor = armorItem as IWearableDurability;

                if (attacker is PlayerMobile && (armorItem as BaseDungeonArmor) != null)
                    armor = null;

                if (attacker != null && armor != null)
                {
                    BaseWeapon attackerWeapon = attacker.Weapon as BaseWeapon;
                                       
                    damage = armor.OnHit(attackerWeapon, damage);
                }
            }

            //Any Other Physical Impact
            else
            {
                double finalArmorRating = ArmorRating;

                double totalValue = 0;
                
                if (bc_Attacker != null)
                    bc_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Pierce, out totalValue);

                if (totalValue < 0)
                    totalValue = 0;

                if (totalValue > 1)
                    totalValue = 1;

                finalArmorRating *= (1 - totalValue);

                double minDamageReduction = (finalArmorRating * .33) / 100;
                double maxDamageReduction = (finalArmorRating * .66) / 100;

                double damageReduction = 1 - (minDamageReduction + ((maxDamageReduction - minDamageReduction) * Utility.RandomDouble()));

                damage = (int)(Math.Round((double)damage * damageReduction));

                if (damage < 1)
                    damage = 1;

                return damage;
            }

            return damage;
        }

        public override void Damage(int amount, Mobile from)
        {
            double damage = (double)amount;

            if (from != null)
            {
                if (from != this)
                {
                    BaseCreature bc_Source = from as BaseCreature;
                    PlayerMobile pm_Source = from as PlayerMobile;

                    LastCombatTime = DateTime.UtcNow;
                    from.LastCombatTime = DateTime.UtcNow;

                    if (bc_Source != null)
                    {
                        if (bc_Source.Controlled && bc_Source.ControlMaster is PlayerMobile && bc_Source.ControlMaster != this)
                        {
                            PlayerMobile pm_SourceController = bc_Source.ControlMaster as PlayerMobile;                            

                            bc_Source.LastPlayerCombatTime = DateTime.UtcNow;

                            PlayerVsPlayerCombatOccured(pm_SourceController);
                        }
                    }

                    if (pm_Source != null && pm_Source != this)
                    {
                        PlayerVsPlayerCombatOccured(pm_Source);
                        pm_Source.PlayerVsPlayerCombatOccured(this);
                    }
                }

                //Discordance
                int discordancePenalty = 0;

                BaseCreature bc_From = from as BaseCreature;

                if (bc_From != null)
                {
                    //Damage is Coming from a Creature that is Discorded
                    if (SkillHandlers.Discordance.GetEffect(bc_From, ref discordancePenalty))
                        damage *= (1 - (double)(Math.Abs(discordancePenalty)) / 100);
                }
            }

            //Ship-Based Combat
            if (BaseBoat.UseShipBasedDamageModifer(from, this))
                damage *= BaseBoat.shipBasedDamageToPlayerScalar;

            if (damage < 1)
                damage = 1;

            amount = (int)damage;

            if (from != null && m_ShowDamageTaken == DamageDisplayMode.PrivateMessage)
                SendMessage(PlayerDamageTakenTextHue, from.Name + " attacks you for " + amount.ToString() + " damage.");

            if (m_ShowDamageTaken == DamageDisplayMode.PrivateOverhead)
                PrivateOverheadMessage(MessageType.Regular, PlayerDamageTakenTextHue, false, "-" + amount.ToString(), NetState);

            base.Damage(amount, from);
        }

        #region Poison
        public override ApplyPoisonResult ApplyPoison(Mobile from, Poison poison)
        {
            if (!Alive)
                return ApplyPoisonResult.Immune;

            #region AOS - NOT USED
            if (Spells.Necromancy.EvilOmenSpell.TryEndEffect(this))
                poison = PoisonImpl.IncreaseLevel(poison);
            #endregion

            double effectChance = this.Skills[SkillName.Parry].Value / 100;
            double randomResult = Utility.RandomDouble();

            //If Successful Chance (100% at GM), currently wearing shield, and within 20 seconds of using Parrying Shield Ability
            if (effectChance >= randomResult && from is BaseCreature && (this.FindItemOnLayer(Layer.TwoHanded) is BaseShield) && (this.ParrySpecialAbilityActivated + TimeSpan.FromSeconds(20) > DateTime.UtcNow))
            {
                return ApplyPoisonResult.Immune;
            }

            ApplyPoisonResult result = base.ApplyPoison(from, poison);

            if (from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer)
            {
                (PoisonTimer as PoisonImpl.PoisonTimer).From = from;
            }

            return result;
        }

        public override bool CheckPoisonImmunity(Mobile from, Poison poison)
        {
            if (Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished))
                return true;

            if (IsUOACZUndead)
                return true;

            return base.CheckPoisonImmunity(from, poison);
        }

        public override void OnPoisonImmunity(Mobile from, Poison poison)
        {
            if (this.Young && (DuelContext == null || !DuelContext.Started || DuelContext.Finished))
                SendLocalizedMessage(502808); // You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
            else
                base.OnPoisonImmunity(from, poison);
        }

        #endregion

        public PlayerMobile(Serial s): base(s)
        {
            m_VisList = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();           
        }

        public List<Mobile> VisibilityList
        {
            get { return m_VisList; }
        }

        public List<Mobile> PermaFlags
        {
            get { return m_PermaFlags; }
        }

        // luck returned as an int representing percentage
        public override int Luck
        {
            get
            {
                int luck = 0;
                if (Citizenship != null && Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Luck))
                    luck += 5;
                if (Guild != null && Guild is Guild)
                {
                    var pguild = Guild as Guild;
                    luck += (int)pguild.Level;
                }
                return luck;
            }
        }

        public override bool IsHarmfulCriminal(Mobile target)
        {
            if (SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0)
            {
                int noto = Notoriety.Compute(this, target);

                if (noto == Notoriety.Innocent)
                    target.Delta(MobileDelta.Noto);

                return false;
            }

            //EXILES LIST
            Custom.Townsystem.Town t = Custom.Townsystem.Town.FromLocation(target.Location, target.Map);
            if (t != null && t.Exiles.Contains(target))
                return false;

            if (target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled)
                return false;

            if (Core.ML && target is BaseCreature && ((BaseCreature)target).Controlled && this == ((BaseCreature)target).ControlMaster)
                return false;

            return base.IsHarmfulCriminal(target);
        }

        public bool AntiMacroCheck(Skill skill, object obj)
        {
            if (obj == null || m_AntiMacroTable == null || this.AccessLevel != AccessLevel.Player)
                return true;

            Hashtable tbl = (Hashtable)m_AntiMacroTable[skill];
            if (tbl == null)
                m_AntiMacroTable[skill] = tbl = new Hashtable();

            CountAndTimeStamp count = (CountAndTimeStamp)tbl[obj];
            if (count != null)
            {
                if (count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
                {
                    count.Count = 1;
                    return true;
                }
                else
                {
                    ++count.Count;
                    if (count.Count <= SkillCheck.Allowance)
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                tbl[obj] = count = new CountAndTimeStamp();
                count.Count = 1;

                return true;
            }
        }

        private void RevertHair()
        {
            SetHairMods(-1, -1);
        }

        private Engines.BulkOrders.BOBFilter m_BOBFilter;

        public Engines.BulkOrders.BOBFilter BOBFilter
        {
            get { return m_BOBFilter; }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            CreatedOn = DateTime.UtcNow;

            m_SpecialAbilityEffectEntries = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToAdd = new List<SpecialAbilityEffectEntry>();
            m_SpecialAbilityEffectEntriesToRemove = new List<SpecialAbilityEffectEntry>();

            m_PlayerClassRansomedItemsAvailable = new List<PlayerClassItemRansomEntry>();

            //Safety Measures
            Squelched = false;
            Frozen = false;
            CantWalk = false;

            if (version < 69)
                ShowTownChat = true;            

            //---------------------------

            m_PaladinsKilled = new List<Mobile>();

            if (version >= 88)
            {
                m_EventCalendarAccount = (EventCalendarAccount)reader.ReadItem() as EventCalendarAccount;
                m_BonusSkillCap = reader.ReadInt();
                m_MHSPlayerEntry = (MHSPlayerEntry)reader.ReadItem() as MHSPlayerEntry;
            }

            if (version >= 87)
            {
                m_ShowHealing = (DamageDisplayMode)reader.ReadInt();
            }

            if (version >= 86)
            {
                m_WorldChatAccountEntry = (WorldChatAccountEntry)reader.ReadItem() as WorldChatAccountEntry;
            }

            if (version >= 85)
            {
                m_UOACZAccountEntry = (UOACZAccountEntry)reader.ReadItem() as UOACZAccountEntry;
            }

            if (version >= 84)
            {
                m_HideRestrictionExpiration = reader.ReadDateTime();
            }

            m_peacemakingMode = PeacemakingModeEnum.Combat; //Default value
            if (version >= 83)
            {
                m_peacemakingMode = (PeacemakingModeEnum)reader.ReadInt();
            }

            if (version >= 82)
            {
                m_HenchmenSpeechDisplayMode = (HenchmenSpeechDisplayMode)reader.ReadInt();
            }

            if (version >= 81)
            {   
                m_PowerHourBonus = reader.ReadTimeSpan();
                m_BankGameTime = reader.ReadTimeSpan();                
            }

            if (version >= 80)
            {
                m_StealthStepsDisplayMode = (StealthStepsDisplayMode)reader.ReadInt();
                m_ShowAdminFilterText = reader.ReadBool();
            }

            if (version >= 79)
            {
                m_TinkerTrapPlacementWindow = reader.ReadDateTime();
                m_TinkerTrapsPlaced = reader.ReadInt();
            }

            if (version >= 78)
            {
                m_PlayerEnhancementAccountEntry = (PlayerEnhancementAccountEntry)reader.ReadItem() as PlayerEnhancementAccountEntry;
                m_InfluenceAccountEntry = reader.ReadItem() as InfluenceAccountEntry;
            }            

            if (version >= 77)
            {
                m_ShowFollowerDamageTaken = (DamageDisplayMode)reader.ReadInt();
            }

            if (version >= 76)
            {
                m_PaladinsKilled = reader.ReadStrongMobileList();
                m_MurdererDeathGumpNeeded = reader.ReadBool();
            }

            if (version >= 75)
            {
            }

            if (version >= 74)
            {
                m_RestitutionFeesToDistribute = reader.ReadInt();
                m_LastPlayerKilledBy = (PlayerMobile)reader.ReadMobile();
            }

            if (version >= 73)
            {
                m_PaladinRejoinAllowed = reader.ReadDateTime();
                m_PaladinProbationExpiration = reader.ReadDateTime();
            }

            if (version >= 72)
            {
                m_LastInstrument = (BaseInstrument)reader.ReadItem();
            }

            if (version >= 71)
            {
                m_ShowDamageTaken = (DamageDisplayMode)reader.ReadInt();
            }

            if (version >= 70)
            {
                m_ShowProvocationDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowPoisonDamage = (DamageDisplayMode)reader.ReadInt();
                m_AutoStealth = reader.ReadBool();
                m_BoatOccupied = (BaseBoat)reader.ReadItem();
            }

            if (version >= 69)
            {
                ShowTownChat = reader.ReadBool();
            }

            if (version >= 68)
            {
                int ransomEntries = reader.ReadInt();

                for (int a = 0; a < ransomEntries; a++)
                {
                    Mobile m_Ransomer = reader.ReadMobile();

                    string str = reader.ReadString();
                    Type itemType = Type.GetType(str);

                    int m_RansomCost = reader.ReadInt();

                    PlayerClassItemRansomEntry ransomEntry = new PlayerClassItemRansomEntry(m_Ransomer, itemType, m_RansomCost);

                    m_PlayerClassRansomedItemsAvailable.Add(ransomEntry);
                }
            }

            if (version >= 67)
            {
                KinPaintHue = reader.ReadInt();
                KinPaintExpiration = reader.ReadDateTime();
            }

            if (version >= 66)
            {
                int specialAbilityEntries = reader.ReadInt();

                for (int a = 0; a < specialAbilityEntries; a++)
                {
                    SpecialAbilityEffect effect = (SpecialAbilityEffect)reader.ReadInt();
                    Mobile owner = reader.ReadMobile();
                    double value = reader.ReadDouble();
                    DateTime expiration = reader.ReadDateTime();

                    SpecialAbilityEffectEntry entry = new SpecialAbilityEffectEntry(effect, owner, value, expiration);

                    m_SpecialAbilityEffectEntries.Add(entry);
                }
            }

            if (version >= 65)
            {
                m_ShowMeleeDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowSpellDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowFollowerDamage = (DamageDisplayMode)reader.ReadInt();

                m_IsInTempStatLoss = reader.ReadBool();
                m_RecallRestrictionExpiration = reader.ReadDateTime();
            }

            if (version >= 64)
            {
                m_LastLocation = reader.ReadPoint3D();
            }

            if (version >= 63)
            {
                m_SpyglassAction = (SpyglassAction)reader.ReadInt();
                m_NextSpyglassActionAllowed = reader.ReadDateTime();

                m_SpyglassAction = Server.Items.SpyglassAction.None;
            }

            if (version >= 62)
            {
                m_PreviousPaladinScore = reader.ReadInt();
            }

            if (version >= 61)
            {
                m_PaladinScore = reader.ReadInt();
                m_MurdererScore = reader.ReadInt();
                m_PirateScore = reader.ReadInt();
            }

            if (version >= 60)
            {
                m_UniqueMurders = reader.ReadInt();

                int murderEntriesCount = reader.ReadInt();
                for (int i = 0; i < murderEntriesCount; ++i)
                {
                    PlayerMobile pm_Victim = reader.ReadMobile() as PlayerMobile;
                    DateTime murderDate = reader.ReadDateTime();

                    if (pm_Victim != null)
                        DictUniqueMurderEntries.Add(pm_Victim, murderDate);
                }
            }

            if (version >= 59)
            {
                m_PetBattleUnlocked = reader.ReadBool();
                LastPetBattleActivity = reader.ReadDateTime();
            }

            if (version >= 58)
            {
                m_SpellEntries = new List<SpellEntry>();

                int spellEntriesCount = reader.ReadInt();

                for (int i = 0; i < spellEntriesCount; ++i)
                {
                    String spellName = (String)reader.ReadString();
                    Mobile from = reader.ReadMobile() as Mobile;
                    DateTime expiration = (DateTime)reader.ReadDateTime();

                    SpellEntry entry = new SpellEntry(spellName, from, expiration);

                    m_SpellEntries.Add(entry);
                }

                m_SpellToleranceTimer = new SpellToleranceTimer(this);

                if (m_SpellEntries.Count > 0)
                {
                    m_SpellToleranceTimer.Start();
                }
            }

            if (version >= 57)
            {
                m_VengeanceEntries = new List<VengeanceEntry>();

                int entriesCount = reader.ReadInt();

                for (int i = 0; i < entriesCount; ++i)
                {
                    PlayerMobile pm_Owner = reader.ReadMobile() as PlayerMobile;
                    PlayerMobile pm_Offender = reader.ReadMobile() as PlayerMobile;

                    bool m_Purchasable = reader.ReadBool();
                    int m_GoldCost = reader.ReadInt();
                    string m_Message = reader.ReadString();

                    DateTime m_CreationDate = DateTime.UtcNow;
                    int m_PointsRemaining = VengeanceEntry.PointsRemainingDefault;

                    if (version >= 75)
                    {
                        m_CreationDate = reader.ReadDateTime();
                        m_PointsRemaining = reader.ReadInt();
                    }

                    bool allowEntry = true;
                    bool forceCreationDate = false;

                    if (m_CreationDate == DateTime.MinValue)
                    {
                        m_CreationDate = DateTime.UtcNow;
                        forceCreationDate = true;
                    }                    

                    if (pm_Owner == null)
                        allowEntry = false;

                    else
                    {
                        if (pm_Owner.Deleted)
                            allowEntry = false;
                    }

                    if (pm_Offender == null)
                        allowEntry = false;

                    else
                    {
                        if (pm_Offender.Deleted)
                            allowEntry = false;
                    }

                    //Contract Has Expired
                    if (m_CreationDate != DateTime.MinValue && m_CreationDate + VengeanceEntry.ExpirationDuration <= DateTime.UtcNow)
                        allowEntry = false;

                    if (allowEntry)
                    {
                        VengeanceEntry entry = new VengeanceEntry(pm_Owner, pm_Offender, m_Purchasable, m_GoldCost, m_Message, m_CreationDate, m_PointsRemaining);
                        
                        m_VengeanceEntries.Add(entry);  
                      
                        Timer.DelayCall(TimeSpan.FromMilliseconds(1000), delegate
                        {
                            if (this != null && entry != null && forceCreationDate)
                            {
                                if (entry.m_Purchasable)
                                    Vengeance.UpdatePublicVengeanceEntry(entry, false);
                            }
                        }); 
                    }
                }

                m_RestitutionFee = reader.ReadInt();
                m_PenanceExpiration = reader.ReadDateTime();
            }

            if (version >= 56)
            {
                m_ParrySpecialAbilityActivated = reader.ReadDateTime();
            }

            if (version >= 55)
            {
                m_NextFireAttempt = reader.ReadDateTime();
            }

            if (version >= 54)
            {
                m_CompanionLastLocation = reader.ReadPoint3D();
            }

            if (version >= 53)
            {
                m_Companion = reader.ReadBool();
            }

            if (version >= 52)
            {
                m_NumGoldCoinsGenerated = reader.ReadInt();
            }

            if (version >= 51)
            {
                m_PowerHourReset = reader.ReadDateTime();
            }

            if (version >= 50)
            {
                m_PeacedUntil = reader.ReadDateTime();
            }

            if (version >= 49)
            {
                CreatedOn = reader.ReadDateTime();
            }

            if (version >= 48)
            {
                TreasuryKeys = (int)reader.ReadUShort();
                TotalAccumulatedTreasuryKeys = (int)reader.ReadUShort();
            }

            if (version >= 47)
            {
                SelectedTitleColorIndex = (int)reader.ReadByte();
                SelectedTitleColorRarity = (EColorRarity)reader.ReadByte();
            }

            if (version >= 46)
            {
                TitleColorState = new PlayerTitleColors();
                TitleColorState.Deserialize(reader);
            }

            if (version >= 45)
            {
                m_UserOptHideFameTitles = reader.ReadBool();
            }

            if (version >= 44)
            {
                int count = reader.ReadInt();
                if (count > 0)
                {
                    PreviousNames = new List<string>();
                    for (int i = 0; i < count; i++)
                    {
                        PreviousNames.Add(reader.ReadString());
                    }
                }
            }

            if (version >= 43)
            {
                LoginElapsedTime = reader.ReadTimeSpan();
            }

            if (version >= 42)
            {
                FireDungeonState = Scripts.Custom.FireDungeonPlayerState.Deserialize(reader);
            }

            if (version >= 41)
            {
                DonationPlayerState = Server.Custom.DonationState.Deserialize(reader);
            }

            if (version >= 40)
            {
                m_T2AAccess = reader.ReadDateTime();
            }

            if (version >= 39)
            {
                //Removed
            }

            if (version >= 38)
            {
                m_DateTimeDied = reader.ReadDateTime();
            }

            if (version >= 37)
            {
                SquelchCitizenship = reader.ReadBool();
            }

            if (version >= 36)
            {
                m_PaladinPoints = reader.ReadInt();
                m_PaladinPointsDecayed = reader.ReadDateTime();
            }

            if (version >= 35)
            {
                int count = reader.ReadInt();
                for (int i = 0; i < count; ++i)
                {
                    string title = reader.ReadString();
                    m_TitlesPrefix.Add(title);
                }

                int count2 = reader.ReadInt();
                for (int i = 0; i < count2; ++i)
                {
                    string dummy = reader.ReadString();
                }

                m_CurrentPrefix = reader.ReadString();
            }

            if (version >= 34)
            {
                m_YewJailItem = (YewJail.YewJailItem)reader.ReadItem();
            }

            if (version >= 33)
            {
                m_minOrderJoinTime = reader.ReadTimeSpan();
                m_ShortTermMurders = reader.ReadStrongMobileList();
                m_TimeSpanDied = reader.ReadTimeSpan();
                m_TimeSpanResurrected = reader.ReadTimeSpan();
            }

            if (version >= 32)
            {               
                m_ActiveSetBonuses = (SetBonus)reader.ReadInt();
            }

            if (version >= 31)
            {
                //Removed: Stat Gains
                if (51 > version)
                {
                    reader.ReadDateTime();
                    reader.ReadInt();
                }
            }

            if (version >= 30)
            {
                if (51 > version)
                {
                    //Removed: Skill Gain Timers
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; ++i)
                    {
                        reader.ReadInt();
                        reader.ReadDateTime();
                    }
                }

                if (51 > version)
                {
                    //Removed: Skill Gains
                    int count = reader.ReadInt();
                    for (int i = 0; i < count; ++i)
                    {
                        reader.ReadInt();
                        reader.ReadInt();
                    }
                }
            }

            if (version >= 29)
            {
                //SkillScroll Decay timer for LastSkillGain deserialize

                int count = reader.ReadInt();
                if (count != 0)
                {
                    for (int n = 0; n < count; n++)
                    {
                        _StartedLastSkillGain[n] = reader.ReadDateTime();

                        DateTime now = DateTime.UtcNow;
                        TimeSpan newDelay = now.Subtract(_StartedLastSkillGain[n]);

                        if (newDelay > DecayLastSkillGain)                        
                            newDelay = TimeSpan.Zero;                        

                        _TimerLastSkillGain[n] = Timer.DelayCall(newDelay, new TimerCallback(deleteLastSkillGain));
                    }
                }             
            }

            if (version >= 28)
            {
                //SkillScroll LastSkillGain Deserialize
                
                int count = reader.ReadInt();
                if (count != 0)
                {
                    int skillNum;
                    for (int n = 0; n < count; n++)
                    {
                        skillNum = reader.ReadInt();
                        LastSkillGain[n] = Skills[skillNum];
                    }

                    // if no timer delete skill
                    DateTime badDate = DateTime.Parse("1/1/0001 12:00:00 AM");

                    for (int n = 0; n < count; n++)
                    {
                        if (_StartedLastSkillGain[n] == null || _StartedLastSkillGain[n].Equals(badDate))
                        
                            deleteLastSkillGain();                        
                    }
                }
            }

            if (version >= 27)
            {
                m_AnkhNextUse = reader.ReadDateTime();
            }

            if (version >= 26)
            {
                m_AutoStabled = reader.ReadStrongMobileList();
            }

            if (version >= 25)
            {
                int recipeCount = reader.ReadInt();

                if (recipeCount > 0)
                {
                    m_AcquiredRecipes = new Dictionary<int, bool>();

                    for (int i = 0; i < recipeCount; i++)
                    {
                        int r = reader.ReadInt();
                        if (reader.ReadBool())	//Don't add in recipies which we haven't gotten or have been removed
                            m_AcquiredRecipes.Add(r, true);
                    }
                }
            }

            if (version >= 24)
            {
                m_LastHonorLoss = reader.ReadDeltaTime();
            }

            if (version >= 23)
            {
                m_ChampionTitles = new ChampionTitleInfo(reader);
            }

            if (version >= 22)
            {
                m_LastValorLoss = reader.ReadDateTime();
                m_Step = (QuestStep)reader.ReadInt();
            }

            if (version >= 21)
            {
                m_ToTItemsTurnedIn = reader.ReadEncodedInt();
                m_ToTTotalMonsterFame = reader.ReadInt();
                m_LastTarget = (Serial)reader.ReadInt();
            }

            if (version >= 20)
            {
                m_AllianceMessageHue = reader.ReadEncodedInt();
                m_GuildMessageHue = reader.ReadEncodedInt();
               
                if (CanReprieveBool)
                {
                    m_CanReprieve = 0;
                    reader.ReadInt();
                }

                else
                    m_CanReprieve = reader.ReadInt();

                m_LastDeathByPlayer = reader.ReadDateTime();
            }

            if (version >= 19)
            {
                int rank = reader.ReadEncodedInt();
                int maxRank = Guilds.RankDefinition.Ranks.Length - 1;

                if (rank > maxRank)
                    rank = maxRank;

                m_GuildRank = Guilds.RankDefinition.Ranks[rank];
                m_LastOnline = reader.ReadDateTime();
            }

            if (version >= 18)
            {
                m_SolenFriendship = (SolenFriendship)reader.ReadEncodedInt();
            }

            if (version >= 17)
            {
                //Removed
            }

            if (version >= 16)
            {
                m_Quest = QuestSerializer.DeserializeQuest(reader);

                if (m_Quest != null)
                    m_Quest.From = this;

                int count = reader.ReadEncodedInt();

                if (count > 0)
                {
                    m_DoneQuests = new List<QuestRestartInfo>();

                    for (int i = 0; i < count; ++i)
                    {
                        Type questType = QuestSerializer.ReadType(QuestSystem.QuestTypes, reader);
                        DateTime restartTime;

                        if (version < 17)
                            restartTime = DateTime.MaxValue;

                        else
                            restartTime = reader.ReadDateTime();

                        m_DoneQuests.Add(new QuestRestartInfo(questType, restartTime));
                    }
                }

                m_Profession = reader.ReadEncodedInt();
            }

            if (version >= 15)
            {
                m_LastCompassionLoss = reader.ReadDeltaTime();
                m_NoNewTimer = reader.ReadBool();
            }

            if (version >= 14)
            {
                m_CompassionGains = reader.ReadEncodedInt();

                if (m_CompassionGains > 0)
                    m_NextCompassionDay = reader.ReadDeltaTime();

                NextBountyNote = reader.ReadTimeSpan();
            }

            if (version >= 13)
            {
                //Removed
            }

            if (version >= 12)
            {
                m_BOBFilter = new Engines.BulkOrders.BOBFilter(reader);
            }

            if (version >= 11)
            {
                if (version < 13)
                {
                    List<Item> payed = reader.ReadStrongItemList();

                    for (int i = 0; i < payed.Count; ++i)
                        payed[i].PayedInsurance = true;
                }
            }

            if (version >= 10)
            {
                if (reader.ReadBool())
                {
                    m_HairModID = reader.ReadInt();
                    m_HairModHue = reader.ReadInt();
                    m_BeardModID = reader.ReadInt();
                    m_BeardModHue = reader.ReadInt();
                }
            }

            if (version >= 9)
            {
                SavagePaintExpiration = reader.ReadTimeSpan();
            }

            if (version >= 8)
            {
                m_NpcGuild = (NpcGuild)reader.ReadInt();
                m_NpcGuildJoinTime = reader.ReadDateTime();
                m_NpcGuildGameTime = reader.ReadTimeSpan();
            }

            if (version >= 7)
            {
                m_PermaFlags = reader.ReadStrongMobileList();
            }

            if (version >= 6)
            {
                NextTailorBulkOrder = reader.ReadTimeSpan();
            }

            if (version >= 5)
            {
                NextSmithBulkOrder = reader.ReadTimeSpan();
            }

            if (version >= 4)
            {
                m_LastJusticeLoss = reader.ReadDeltaTime();
                m_JusticeProtectors = reader.ReadStrongMobileList();
            }

            if (version >= 3)
            {
                m_LastSacrificeGain = reader.ReadDeltaTime();
                m_LastSacrificeLoss = reader.ReadDeltaTime();
                m_AvailableResurrects = reader.ReadInt();
            }

            if (version >= 2)
            {
                m_Flags = (PlayerFlag)reader.ReadInt();
            }

            if (version >= 1)
            {
                m_LongTermElapse = reader.ReadTimeSpan();
                m_ShortTermElapse = reader.ReadTimeSpan();
                m_GameTime = reader.ReadTimeSpan();
            }

            if (version >= 0)
            {
                m_AutoStabled = new List<Mobile>();
            }

            //----------------

            PetBattleCreatureCollection = new PetBattleCreatureCollection();

            // Professions weren't verified on 1.0 RC0
            if (!CharacterCreation.VerifyProfession(m_Profession))
                m_Profession = 0;

            if (m_PermaFlags == null)
                m_PermaFlags = new List<Mobile>();

            if (m_JusticeProtectors == null)
                m_JusticeProtectors = new List<Mobile>();

            if (m_BOBFilter == null)
                m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            if (m_GuildRank == null)
                m_GuildRank = Guilds.RankDefinition.Member;	//Default to member if going from older version to new version (only time it should be null)

            if (m_LastOnline == DateTime.MinValue && Account != null)
                m_LastOnline = ((Account)Account).LastLogin;

            if (m_ChampionTitles == null)
                m_ChampionTitles = new ChampionTitleInfo();

            if (AccessLevel > AccessLevel.Player)
                m_IgnoreMobiles = true;

            if (TitleColorState == null)
                TitleColorState = new PlayerTitleColors();

            List<Mobile> list = this.Stabled;

            m_LastPassiveTamingSkillGain = DateTime.MinValue;

            for (int i = 0; i < list.Count; ++i)
            {
                BaseCreature bc = list[i] as BaseCreature;

                if (bc != null)
                {
                    bc.IsStabled = true;
                    bc.StabledBy = this;

                    bc.OwnerAbandonTime = DateTime.MaxValue;
                }
            }

            CheckAtrophies(this);

            if (Hidden)	//Hiding is the only buff where it has an effect that's serialized.
                AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));

            //Reapply Kin Paint
            if (KinPaintHue != -1 && KinPaintExpiration > DateTime.UtcNow)
            {
                HueMod = KinPaintHue;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (CheckAccountForPenance(this) && IsInTempStatLoss && (Murderer || Paladin))
                    ApplyTempSkillLoss();

                else if (!CheckAccountForPenance(this) && IsInTempStatLoss)
                    IsInTempStatLoss = false;
            });

            if (Spectating)
            {
                if (Region is BattlegroundRegion)
                {
                    SpectatingTimer = new SpectatorTimer(this);
                    SpectatingTimer.Start();
                }
                else
                {
                    Spectating = false;
                }
            }

            if (!Alive)
            {
                StartGhostScoutTimer();
            }

            if (!(Region.Find(LogoutLocation, LogoutMap) is GuardedRegion))
            {
                // crash protection
                Hidden = true;
                Poison = null;
            }

            if (Region.Find(LogoutLocation, LogoutMap) is UOACZRegion)
                Hidden = false;

            if (LastPlayerCombatTime + PlayerCombatExpirationDelay > DateTime.UtcNow)
            {
                m_PlayerCombatTimer = new PlayerCombatTimer(this);
                m_PlayerCombatTimer.Start();
            }            
        }

        private GhostScoutingTimer m_GhostScoutTimer;

        public override void Serialize(GenericWriter writer)
        {
            //cleanup our anti-macro table 
            foreach (Hashtable t in m_AntiMacroTable.Values)
            {
                ArrayList remove = new ArrayList();
                foreach (CountAndTimeStamp time in t.Values)
                {
                    if (time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.UtcNow)
                        remove.Add(time);
                }

                for (int i = 0; i < remove.Count; ++i)
                    t.Remove(remove[i]);
            }

            CheckKillDecay();

            if (KinPaintHue != -1)
            {
                if (DateTime.UtcNow >= KinPaintExpiration)
                {
                    KinPaintHue = -1;
                    KinPaintExpiration = DateTime.MinValue;

                    BodyMod = 0;
                    HueMod = -1;

                    SendMessage("Your kin paint has faded.");
                }
            }

            if (KinPaintHue != -1)
            {
                if (HueModEnd > DateTime.MinValue && HueModEnd < DateTime.UtcNow)
                {
                    HueMod = -1;
                    HueModEnd = DateTime.MinValue;
                }
            }

            CheckAtrophies(this);

            base.Serialize(writer);

            writer.Write((int)88); //Version
            
            //Version 88
            writer.Write(m_EventCalendarAccount);
            writer.Write(m_BonusSkillCap);
            writer.Write(m_MHSPlayerEntry);

            //Version 87
            writer.Write((int)m_ShowHealing);

            //Version 86
            writer.Write(m_WorldChatAccountEntry);

            //Version 85
            writer.Write(m_UOACZAccountEntry);

            //Version 84
            writer.Write(m_HideRestrictionExpiration);

            //Version 83
            writer.Write((int)m_peacemakingMode);

            //Version 82
            writer.Write((int)m_HenchmenSpeechDisplayMode);

            //Version 81            
            writer.Write(m_PowerHourBonus);
            writer.Write(m_BankGameTime);

            //Version 80
            writer.Write((int)m_StealthStepsDisplayMode);
            writer.Write(m_ShowAdminFilterText);
            
            //Version 79
            writer.Write(m_TinkerTrapPlacementWindow);
            writer.Write(m_TinkerTrapsPlaced);

            //Version 78
            writer.Write(m_PlayerEnhancementAccountEntry);
            writer.Write(m_InfluenceAccountEntry);
            
            //Version 77
            writer.Write((int)m_ShowFollowerDamageTaken);

            //Version 76
            writer.Write(m_PaladinsKilled, true);
            writer.Write(m_MurdererDeathGumpNeeded);

            //Version 75 (In Version 57 Vengeance Entry) 

            //Version 74
            writer.Write(m_RestitutionFeesToDistribute);
            writer.Write(m_LastPlayerKilledBy);            

            //Version 73
            writer.Write(m_PaladinRejoinAllowed);
            writer.Write(m_PaladinProbationExpiration);

            //Version 72
            writer.Write(m_LastInstrument);
            
            //Version 71
            writer.Write((int)m_ShowDamageTaken);

            //Version 70
            writer.Write((int)m_ShowProvocationDamage);
            writer.Write((int)m_ShowPoisonDamage);
            writer.Write(m_AutoStealth);
            writer.Write(m_BoatOccupied);
            
            //Version 69
            writer.Write(ShowTownChat);

            //Version 68
            int playerClassRansomedItemsAvailable = m_PlayerClassRansomedItemsAvailable.Count;
            writer.Write(playerClassRansomedItemsAvailable);

            for (int a = 0; a < playerClassRansomedItemsAvailable; a++)
            {
                writer.Write(m_PlayerClassRansomedItemsAvailable[a].m_Ransomer);
                writer.Write((string)m_PlayerClassRansomedItemsAvailable[a].m_ItemType.ToString());
                writer.Write(m_PlayerClassRansomedItemsAvailable[a].m_GoldCost);
            }

            //Version 67
            writer.Write(KinPaintHue);
            writer.Write(KinPaintExpiration);

            //Version 66
            int specialAbilityEffectEntries = m_SpecialAbilityEffectEntries.Count;
            writer.Write((int)specialAbilityEffectEntries);

            for (int a = 0; a < specialAbilityEffectEntries; a++)
            {
                writer.Write((int)m_SpecialAbilityEffectEntries[a].m_SpecialAbilityEffect);
                writer.Write((Mobile)m_SpecialAbilityEffectEntries[a].m_Owner);
                writer.Write((double)m_SpecialAbilityEffectEntries[a].m_Value);
                writer.Write((DateTime)m_SpecialAbilityEffectEntries[a].m_Expiration);
            }

            //Version 65
            writer.Write((int)m_ShowMeleeDamage);
            writer.Write((int)m_ShowSpellDamage);
            writer.Write((int)m_ShowFollowerDamage);

            writer.Write(m_IsInTempStatLoss);
            writer.Write(m_RecallRestrictionExpiration);

            //Version 64
            writer.Write(m_LastLocation);

            //Version 63
            writer.Write((int)m_SpyglassAction);
            writer.Write(m_NextSpyglassActionAllowed);

            //Version 62
            writer.Write(m_PreviousPaladinScore);

            //Version 61
            writer.Write(m_PaladinScore);
            writer.Write(m_MurdererScore);
            writer.Write(m_PirateScore);

            //Version 60
            writer.Write(m_UniqueMurders);

            writer.Write((int)DictUniqueMurderEntries.Count);
            foreach (KeyValuePair<PlayerMobile, DateTime> pair in DictUniqueMurderEntries)
            {
                writer.Write(pair.Key);
                writer.Write(pair.Value);
            }

            //Version 59
            writer.Write(m_PetBattleUnlocked);
            writer.Write(LastPetBattleActivity);

            //Version 58
            writer.Write((int)m_SpellEntries.Count);

            foreach (SpellEntry entry in m_SpellEntries)
            {
                writer.Write(entry.m_SpellName);
                writer.Write(entry.m_From);
                writer.Write(entry.m_Expiration);
            }

            //Version 57
            writer.Write((int)m_VengeanceEntries.Count);

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                writer.Write(entry.m_Owner);
                writer.Write(entry.m_Offender);

                writer.Write(entry.m_Purchasable);
                writer.Write(entry.m_GoldCost);
                writer.Write(entry.m_Message);

                //Version 75
                writer.Write(entry.m_CreationDate);
                writer.Write(entry.m_PointsRemaining);
            }

            writer.Write(m_RestitutionFee);
            writer.Write(m_PenanceExpiration);

            // version 56
            writer.Write(m_ParrySpecialAbilityActivated);

            // version 55
            writer.Write(m_NextFireAttempt);

            // version 54
            writer.Write(m_CompanionLastLocation);

            // version 53
            writer.Write(m_Companion);

            // version 52
            writer.Write((int)m_NumGoldCoinsGenerated);

            // version 51 removed ROT system and added powerhour
            writer.Write(PowerHourReset);

            // version 50 - This is actually version 28 in the runUO main trunk
            writer.Write((DateTime)m_PeacedUntil);

            // version 49
            writer.Write(CreatedOn);

            // version 48
            writer.Write((ushort)TreasuryKeys);
            writer.Write((ushort)TotalAccumulatedTreasuryKeys);

            // version 47
            writer.Write((byte)SelectedTitleColorIndex);
            writer.Write((byte)SelectedTitleColorRarity);

            // version 46
            TitleColorState.Serialize(writer);

            //version 45 
            writer.Write(m_UserOptHideFameTitles);

            //version 44
            if (PreviousNames == null)
            {
                writer.Write((int)0);
            }
            else
            {
                writer.Write(PreviousNames.Count);
                foreach (var name in PreviousNames)
                {
                    writer.Write(name);
                }
            }

            //version 43
            writer.Write(LoginElapsedTime);

            //version 42
            Scripts.Custom.FireDungeonPlayerState.Serialize(writer, this);

            //version 41
            Server.Custom.DonationState.Serialize(writer, this);

            //version 40
            //T2A Access
            writer.Write((DateTime)m_T2AAccess);

            //Paladins
            writer.Write((DateTime)m_DateTimeDied);

            //Squelch Citizenship
            writer.Write((bool)SquelchCitizenship);

            //case 36: Paladin titles
            writer.Write((int)m_PaladinPoints);
            writer.Write((DateTime)m_PaladinPointsDecayed);

            //case 35: Generic titles
            writer.Write((int)m_TitlesPrefix.Count);

            for (int n = 0; n < m_TitlesPrefix.Count; n++)
            {
                if (m_TitlesPrefix[n] != null)
                {
                    writer.Write(m_TitlesPrefix[n]);
                }
            }

            writer.Write((int)0); // TITLE SUFFIX LEGACY (removed)

            writer.Write((string)m_CurrentPrefix);

            writer.WriteItem((YewJail.YewJailItem)m_YewJailItem); //IPY Jail Control

            //Time to wait before re-joining The Paladin Order if you commit murder
            if (m_minOrderJoinTime == null)
                m_minOrderJoinTime = TimeSpan.Zero;

            if (m_ShortTermMurders == null)
                m_ShortTermMurders = new List<Mobile>();           

            writer.Write((TimeSpan)m_minOrderJoinTime);     //IPY
            //Track Short Term Murdered List
            writer.Write(m_ShortTermMurders, true);         //IPY

            //Murdercount decay while alive only
            writer.Write((TimeSpan)m_TimeSpanDied);         //IPY
            writer.Write((TimeSpan)m_TimeSpanResurrected);  //IPY

            // Set bonuses
            writer.Write((int)m_ActiveSetBonuses);

            // SkillScroll Serialize Decay timers for LastSkillGains
            int countTimer = 0;
            DateTime badDate = DateTime.Parse("1/1/0001 12:00:00 AM");
            for (int n = 0; n < 5; n++)
            {
                if (!this._StartedLastSkillGain[countTimer].Equals(badDate) && this._StartedLastSkillGain[countTimer] != null && this._TimerLastSkillGain[countTimer].Running)
                {
                    countTimer++;
                }
            }

            writer.Write((int)countTimer);

            for (int n = 0; n < countTimer; n++)
            {
                if (this._StartedLastSkillGain[n] != null)
                {
                    writer.Write(this._StartedLastSkillGain[n]);
                }
            }
            // SkillScroll Serialize LastSkillGain[] nummmnut
            // Find all non null elements in LastSkillGain[]
            int count = 0;
            while ((count < 5) && (this.LastSkillGain[count] != null))
            {
                count++;
            }
            // SkillScroll Serialize end

            writer.Write((int)count);

            // Serialize Skill in order of LastSkillGain[]
            for (int n = 0; n < 5; n++)
            {
                if (this.LastSkillGain[n] != null)
                {
                    writer.Write((int)this.LastSkillGain[n].SkillID);
                }
            }

            // SkillScroll Serialize LastSkillGain[] End

            writer.Write((DateTime)m_AnkhNextUse);
            writer.Write(m_AutoStabled, true);

            if (m_AcquiredRecipes == null)
            {
                writer.Write((int)0);
            }
            else
            {
                writer.Write(m_AcquiredRecipes.Count);

                foreach (KeyValuePair<int, bool> kvp in m_AcquiredRecipes)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value);
                }
            }

            writer.WriteDeltaTime(m_LastHonorLoss);

            ChampionTitleInfo.Serialize(writer, m_ChampionTitles);

            writer.Write(m_LastValorLoss);
            writer.Write((int)m_Step); // IPY

            writer.WriteEncodedInt(m_ToTItemsTurnedIn);
            writer.Write(m_ToTTotalMonsterFame);
            writer.Write((Serial)m_LastTarget); // IPY

            writer.WriteEncodedInt(m_AllianceMessageHue);
            writer.WriteEncodedInt(m_GuildMessageHue);
            writer.Write((int)m_CanReprieve); // IPY
            writer.Write((DateTime)m_LastDeathByPlayer); // IPY

            writer.WriteEncodedInt(m_GuildRank.Rank);
            writer.Write(m_LastOnline);

            writer.WriteEncodedInt((int)m_SolenFriendship);

            QuestSerializer.Serialize(m_Quest, writer);

            if (m_DoneQuests == null)
            {
                writer.WriteEncodedInt((int)0);
            }
            else
            {
                writer.WriteEncodedInt((int)m_DoneQuests.Count);

                for (int i = 0; i < m_DoneQuests.Count; ++i)
                {
                    QuestRestartInfo restartInfo = m_DoneQuests[i];

                    QuestSerializer.Write((Type)restartInfo.QuestType, QuestSystem.QuestTypes, writer);
                    writer.Write((DateTime)restartInfo.RestartTime);
                }
            }

            writer.WriteEncodedInt((int)m_Profession);

            writer.WriteDeltaTime(m_LastCompassionLoss);
            writer.Write((bool)m_NoNewTimer); // IPY

            writer.WriteEncodedInt(m_CompassionGains);
            if (m_CompassionGains > 0)
                writer.WriteDeltaTime(m_NextCompassionDay);

            writer.Write(NextBountyNote); // IPY

            m_BOBFilter.Serialize(writer);

            bool useMods = (m_HairModID != -1 || m_BeardModID != -1);

            writer.Write(useMods);

            if (useMods)
            {
                writer.Write((int)m_HairModID);
                writer.Write((int)m_HairModHue);
                writer.Write((int)m_BeardModID);
                writer.Write((int)m_BeardModHue);
            }

            writer.Write(SavagePaintExpiration);

            writer.Write((int)m_NpcGuild);
            writer.Write((DateTime)m_NpcGuildJoinTime);
            writer.Write((TimeSpan)m_NpcGuildGameTime);

            writer.Write(m_PermaFlags, true);

            writer.Write(NextTailorBulkOrder);

            writer.Write(NextSmithBulkOrder);

            writer.WriteDeltaTime(m_LastJusticeLoss);
            writer.Write(m_JusticeProtectors, true);

            writer.WriteDeltaTime(m_LastSacrificeGain);
            writer.WriteDeltaTime(m_LastSacrificeLoss);
            writer.Write(m_AvailableResurrects);

            writer.Write((int)m_Flags);

            writer.Write(m_LongTermElapse);
            writer.Write(m_ShortTermElapse);
            writer.Write(this.GameTime);
        }

        public static void CheckAtrophies(Mobile m)
        {
            SacrificeVirtue.CheckAtrophy(m);
            JusticeVirtue.CheckAtrophy(m);
            CompassionVirtue.CheckAtrophy(m);
            ValorVirtue.CheckAtrophy(m);

            if (m is PlayerMobile)
                ChampionTitleInfo.CheckAtrophy((PlayerMobile)m);
        }

        public void CheckKillDecay()
        {
            if (m_ShortTermElapse < this.GameTime)
            {
                m_ShortTermElapse += TimeSpan.FromHours(MurderCountDecayHours);

                bool wasMurderer = false;

                if (Murderer)
                    wasMurderer = true;

                if (ShortTermMurders > 0)
                {
                    --ShortTermMurders;

                    if (wasMurderer)
                    {
                        SendMessage("You are no longer a murderer.");

                        PlayerClassPersistance.RemoveTitles(this, PlayerClassPersistance.MurdererTitles);
                        PlayerClassPersistance.RemovePlayerClassEquipment(this, PlayerClass.Murderer);
                    }
                }
            }
        }

        public void ResetKillTime()
        {
            m_ShortTermElapse = this.GameTime + TimeSpan.FromHours(MurderCountDecayHours);
        }

        public bool FindVengeanceEntry(PlayerMobile offender)
        {
            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Offender == offender && entry.m_PointsRemaining > 0 && entry.m_CreationDate + VengeanceEntry.ExpirationDuration > DateTime.UtcNow)
                    return true;
            }

            return false;
        }

        public VengeanceEntry GetVengeanceEntry(PlayerMobile offender)
        {
            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Offender == offender)
                    return entry;
            }

            return null;
        }

        public bool AddVengeanceEntry(PlayerMobile offender, DateTime creationDate, int pointsRemaining)
        {
            VengeanceEntry entryOffender = null;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Offender == offender)
                    entryOffender = entry;
            }

            //No Existing Entry Found
            if (entryOffender == null)
            {
                VengeanceEntry newEntry = new VengeanceEntry(this, offender, false, 0, "", creationDate, pointsRemaining);
                m_VengeanceEntries.Add(newEntry);

                return true;
            }

            return false;
        }

        public bool UpdateVengeanceEntry(VengeanceEntry entryToUpdate, bool createNewEntry)
        {
            if (m_VengeanceEntries == null || entryToUpdate == null)
                return false;

            if (entryToUpdate.m_Owner == null || entryToUpdate.m_Offender == null)
                return false;

            if (entryToUpdate.m_Owner.Deleted || entryToUpdate.m_Offender.Deleted)
                return false;

            VengeanceEntry targetEntry = null;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Owner == entryToUpdate.m_Owner && entry.m_Offender == entryToUpdate.m_Offender)
                    targetEntry = entry;
            }

            //New Entry
            if (targetEntry == null)
            {
                if (createNewEntry)
                    m_VengeanceEntries.Add(entryToUpdate);

                return true;
            }

            //Updating Old Entry
            else
            {
                targetEntry.m_Purchasable = entryToUpdate.m_Purchasable;
                targetEntry.m_GoldCost = entryToUpdate.m_GoldCost;
                targetEntry.m_Message = entryToUpdate.m_Message;

                targetEntry.m_CreationDate = entryToUpdate.m_CreationDate;
                targetEntry.m_PointsRemaining = entryToUpdate.m_PointsRemaining;

                return true;
            }

            return false;
        }

        public bool DecreaseVengeanceEntryPoints(Mobile offender, int pointsReduced)
        {
            if (offender == null)
                return false;

            PlayerMobile pm_Offender = offender as PlayerMobile;
            BaseCreature bc_Offender = offender as BaseCreature;

            if (bc_Offender != null)
            {
                if (bc_Offender.Controlled && bc_Offender.ControlMaster is PlayerMobile)
                    pm_Offender = bc_Offender.ControlMaster as PlayerMobile;
            }

            if (pm_Offender == null)
                return false;

            VengeanceEntry entryToRemove = null;

            bool foundEntry = false;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Offender == pm_Offender)
                {
                    foundEntry = true;
                    entry.m_PointsRemaining -= pointsReduced;

                    if (entry.m_Purchasable)
                        Vengeance.UpdatePublicVengeanceEntry(entry, false);

                    string pointsReducedText = "points";

                    if (pointsReduced == 1)
                        pointsReducedText = "point";

                    string pointsRemainingText = "points";

                    if (entry.m_PointsRemaining == 1)
                        pointsRemainingText = "point";

                    if (entry.m_PointsRemaining <= 0)
                         entryToRemove = entry;
                    else
                        SendMessage("Vengeance against " + pm_Offender.RawName + " reduced by " + pointsReduced.ToString() + " " + pointsReducedText + " (" + entry.m_PointsRemaining.ToString() + " " + pointsRemainingText + " remaining)");

                    break;
                }
            }

            if (entryToRemove != null)
            {
                SendMessage("Vengeance has been fulfilled against " + pm_Offender.RawName + ".");

                m_VengeanceEntries.Remove(entryToRemove);

                return true;
            }

            if (foundEntry)
                return true;

            return false;
        }

        public bool RemoveVengeanceEntry(PlayerMobile offender)
        {
            VengeanceEntry entryToRemove = null;

            foreach (VengeanceEntry entry in m_VengeanceEntries)
            {
                if (entry.m_Offender == offender)
                {
                    entryToRemove = entry;
                    break;
                }
            }

            if (entryToRemove != null)
            {
                m_VengeanceEntries.Remove(entryToRemove);
                return true;
            }

            return false;
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Developer)]
        public DateTime SessionStart
        {
            get { return m_SessionStart; }
            set { m_SessionStart = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan GameTime
        {
            get
            {
                if (NetState != null)
                    return m_GameTime + (DateTime.UtcNow - m_SessionStart);
                else
                    return m_GameTime;
            }
        }

        private bool SameParty(Mobile target)
        {
            bool sameParty = false;
            if (Party != null && target.Party != null)
            {
                if (Party == target.Party)
                    sameParty = true;
            }
            return sameParty;
        }

        private bool SameGuild(Mobile target)
        {
            bool sameGuild = false;
            if (Guild != null && target.Guild != null)
            {
                if (Guild == target.Guild)
                    sameGuild = true;
            }
            return sameGuild;
        }

        public override bool CanSee(Mobile m)
        {
            if (m is CharacterStatue)
                ((CharacterStatue)m).OnRequestedAnimation(this);

            BaseCreature bc_Creature = m as BaseCreature;
            //PlayerMobile pm_Player = m as PlayerMobile;

            if (bc_Creature != null)
            {
                if (bc_Creature.Hidden && bc_Creature.Controlled)
                {
                    if (bc_Creature.ControlMaster == this)
                        return true;

                    //if (SameParty(bc_Creature.ControlMaster) || SameGuild(bc_Creature.ControlMaster))
                    //    return true;
                }
            }

            //if (pm_Player != null)
            //{
            //    if (SameParty(pm_Player) || SameGuild(pm_Player))
            //        return true;
            //}

            if (m != this && !Alive && !Warmode && !m.Hidden && m is PlayerMobile && AccessLevel == AccessLevel.Player && m.AccessLevel == AccessLevel.Player)
            {
                Send(m.RemovePacket);
                return false;
            }

            if (m is PlayerMobile && ((PlayerMobile)m).m_VisList.Contains(this))
                return true;

            return base.CanSee(m);
        }

        public virtual void CheckedAnimate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
        {
            if (!Mounted)
            {
                base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
            }
        }

        public override void Animate(int action, int frameCount, int repeatCount, bool forward, bool repeat, int delay)
        {
            base.Animate(action, frameCount, repeatCount, forward, repeat, delay);
        }

        public override bool CanSee(Item item)
        {
            if (m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer(item))
                return false;

            return base.CanSee(item);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Factions.Faction faction = Factions.Faction.Find(this);

            if (faction != null)
                faction.RemoveMember(this);

            //MLQuestSystem.HandleDeletion( this );

            BaseHouse.HandleDeletion(this);

            DisguiseTimers.RemoveTimer(this);

            if (m_SpellToleranceTimer != null)
            {
                m_SpellToleranceTimer.Stop();
                m_SpellToleranceTimer = null;
            }

            if (m_TempStatLossTimer != null)
            {
                m_TempStatLossTimer.Stop();
                m_TempStatLossTimer = null;
            }
        }

        public override bool NewGuildDisplay { get { return Server.Guilds.Guild.NewGuildSystem; } }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Map == Factions.Faction.Facet)
            {
                Factions.PlayerState pl = Factions.PlayerState.Find(this);

                if (pl != null)
                {
                    Factions.Faction faction = pl.Faction;

                    if (faction.Commander == this)
                        list.Add(1042733, faction.Definition.PropName); // Commanding Lord of the ~1_FACTION_NAME~
                    else if (pl.Sheriff != null)
                        list.Add(1042734, "{0}\t{1}", pl.Sheriff.Definition.FriendlyName, faction.Definition.PropName); // The Sheriff of  ~1_CITY~, ~2_FACTION_NAME~
                    else if (pl.Finance != null)
                        list.Add(1042735, "{0}\t{1}", pl.Finance.Definition.FriendlyName, faction.Definition.PropName); // The Finance Minister of ~1_CITY~, ~2_FACTION_NAME~
                    else if (pl.MerchantTitle != Factions.MerchantTitle.None)
                        list.Add(1060776, "{0}\t{1}", Factions.MerchantTitles.GetInfo(pl.MerchantTitle).Title, faction.Definition.PropName); // ~1_val~, ~2_val~
                    else
                        list.Add(1060776, "{0}\t{1}", pl.Rank.Title, faction.Definition.PropName); // ~1_val~, ~2_val~
                }
            }

            if (Core.ML)
            {
                for (int i = AllFollowers.Count - 1; i >= 0; i--)
                {
                    BaseCreature c = AllFollowers[i] as BaseCreature;

                    if (c != null && c.ControlOrder == OrderType.Guard)
                    {
                        list.Add(501129); // guarded
                        break;
                    }
                }
            }
        }

        //IPY: Added this for Custom Titles (Sean)
        private static string[] m_GuildTypes = new string[]
		{
			"",
			" (Chaos)",
			" (Order)"
		};

        public override void AggressiveAction(Mobile aggressor, bool criminal, bool causeCombat)
        {
            if (aggressor == null) return;
            if (aggressor.Deleted) return;
            if (aggressor == this) return;
            if (Blessed) return;
            if (aggressor.Blessed) return;

            base.AggressiveAction(aggressor, criminal, causeCombat);
        }

        public override void PushNotoriety(Mobile from, Mobile to, bool aggressor)
        {
            NotorietyHandlers.PushNotoriety(from, to, aggressor);
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Deleted)
                return;

            else if (AccessLevel == AccessLevel.Player && DisableHiddenSelfClick && Hidden && from == this)
                return;

            try
            {
                bool show_faction = IsInMilitia;
                bool show_guild = GuildClickMessage && Guild != null && (DisplayGuildTitle || (Player && Guild.Type != Guilds.GuildType.Regular));
                bool show_custom_ipy_title = CurrentPrefix != null && CurrentPrefix.Length > 0;
                bool show_other_titles = true;

                if (from.Region is UOACZRegion)
                {
                    show_faction = false;
                    show_guild = false;
                    show_custom_ipy_title = false;
                    show_other_titles = false;
                }

                int newhue;

                if (NameHue != -1)
                    newhue = NameHue;

                else if (AccessLevel > AccessLevel.Player)
                    newhue = 11;

                else if (from.ShortTermMurders > 4 && Paladin && !(Region is BattlegroundRegion) && !(Region is UOACZRegion))
                    newhue = 2060;

                else
                    newhue = Notoriety.GetHue(Notoriety.Compute(from, this));

                if (show_guild && show_faction) // FACTION AND GUILD
                {
                    // sorry about this one whoever else has to understand it in the future...
                    // [GuildTitle, Abbrev][O,C,B] OR [Abbrev][Order] if not guildtitle...
                    // Selected Title
                    // Lord Puppz
                    // Jimmy - updated to just say [Militia] or [M] for town based militias
                    string guild_str = GuildTitle == null ? Guild.Abbreviation : GuildTitle + ", " + Guild.Abbreviation;
                    string alliance_title = "Militia";
                    string ally_title = GuildTitle == null ? alliance_title : alliance_title[0].ToString();
                    string line_one = String.Concat("[", guild_str, "][", ally_title, "]");

                    PrivateOverheadMessage(MessageType.Label, Citizenship.HomeFaction.Definition.HuePrimary, true, line_one, from.NetState);
                }

                else if (show_faction) // FACTION NO GUILD
                {
                    string text = String.Concat("[", Citizenship.Definition.FriendlyName, "][Militia]");
                    PrivateOverheadMessage(MessageType.Label, Citizenship.HomeFaction.Definition.HuePrimary, true, text, from.NetState);
                }

                else if (show_guild) // GUILD NO FACTION
                {
                    string title = GuildTitle;
                    string type;

                    if (title == null)
                        title = "";
                    else
                        title = title.Trim();

                    if (Guild.Type >= 0 && (int)Guild.Type < m_GuildTypes.Length)
                        type = m_GuildTypes[(int)Guild.Type];
                    else
                        type = "";

                    string text = String.Format(title.Length <= 0 ? "[{1}]{2}" : "[{0}, {1}]{2}", title, Guild.Abbreviation, type);
                    PrivateOverheadMessage(MessageType.Regular, SpeechHue, true, text, from.NetState);
                }

                if (show_custom_ipy_title)
                    PrivateOverheadMessage(MessageType.Label, PlayerTitleColors.GetSpokenColorValue(SelectedTitleColorIndex, SelectedTitleColorRarity), true, CurrentPrefix, from.NetState);

                if (show_other_titles)
                {
                    string fullname_line = "";

                    if ((ShowFameTitle && (Player || Body.IsHuman) && Fame >= 10000))
                        fullname_line = Female ? "Lady " : "Lord ";

                    fullname_line += Name == null ? String.Empty : Name;
                    fullname_line = ApplyNameSuffix(fullname_line); // (Young) for example

                    if (AssistedOwnMilitia && !IsInMilitia)
                        fullname_line += " [Militia Flagged]";

                    PrivateOverheadMessage(MessageType.Label, newhue, AsciiClickMessage, fullname_line, from.NetState);
                }

                else
                {
                    string fullname_line = "";

                    fullname_line += Name == null ? String.Empty : Name;

                    PrivateOverheadMessage(MessageType.Label, newhue, AsciiClickMessage, fullname_line, from.NetState);
                }
            } 

            catch(Exception e)
            {
                Console.WriteLine("OnSingleClick failed {0}", e.Message);
            }
        }

        protected override bool OnMove(Direction d)
        {
            #region Poker
            if (m_PokerGame != null)
            {
                if (!HasGump(typeof(PokerLeaveGump)))
                {
                    SendGump(new PokerLeaveGump(this, m_PokerGame));
                    return false;
                }
            }
            #endregion
            
            if (AccessLevel != AccessLevel.Player)
                return true;
            
            bool stealthMove = false;

            bool leaveFootsteps = false;
            double footstepChance = .33;
            
            BaseDungeonArmor.PlayerDungeonArmorProfile stealtherDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(this, null);

            if (stealtherDungeonArmor.MatchingSet && stealtherDungeonArmor.DungeonArmorDetail.StealthLeavesFootprints)
                leaveFootsteps = true;            

            if (Hidden && DesignContext.Find(this) == null)	//Hidden & NOT customizing a house
            {
                if (!Mounted && (Skills.Stealth.Value >= 20.0 || UOACZSystem.IsUOACZValidMobile(this)))
                {
                    bool running = (d & Direction.Running) != 0;

                    if (running && !UOACZSystem.IsUOACZValidMobile(this))
                    {
                        AllowedStealthSteps = -1;
                        RevealingAction();
                        
                        return true;
                    }

                    AllowedStealthSteps--;
                    stealthMove = true;

                    if (m_AutoStealth && !UOACZSystem.IsUOACZValidMobile(this))
                    {
                        if (AllowedStealthSteps < 0 || CanBeginAction(typeof(Stealth)))
                        {
                            NextSkillTime = Core.TickCount + (int)Server.SkillHandlers.Stealth.OnUse(this).TotalMilliseconds;

                            //If Stealth Success
                            if (Hidden)
                            {
                                AllowedStealthSteps--;
                                stealthMove = true;

                                if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage && AllowedStealthSteps < 100)
                                    SendMessage("You have " + this.AllowedStealthSteps.ToString() + " stealth steps remaining.");

                                else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead && AllowedStealthSteps < 100)
                                    PrivateOverheadMessage(MessageType.Regular, 0, false, "You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.", NetState);
                            }
                        }

                        else
                        {
                            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage && AllowedStealthSteps < 100)
                                SendMessage("You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.");

                            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead && AllowedStealthSteps < 100)
                                PrivateOverheadMessage(MessageType.Regular, 0, false, "You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.", NetState);
                        }
                    }

                    else
                    {
                        if (AllowedStealthSteps < 0)
                            RevealingAction();

                        else
                        {
                            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage && AllowedStealthSteps < 100)
                                SendMessage("You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.");

                            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead && AllowedStealthSteps < 100)
                                PrivateOverheadMessage(MessageType.Regular, 0, false, "You have " + AllowedStealthSteps.ToString() + " stealth steps remaining.", NetState);
                        }                            
                    }

                    if (Hidden && leaveFootsteps && Utility.RandomDouble() <= footstepChance)
                        new Footsteps(d).MoveToWorld(Location, Map);
                }

                else
                    RevealingAction();
            }            

            if (UOACZSystem.IsUOACZValidMobile(this))
            {
                if (IsUOACZUndead)
                {
                    double totalValue = 0;

                    GetSpecialAbilityEntryValue(SpecialAbilityEffect.Ignite, out totalValue);

                    if (Utility.RandomDouble() <= totalValue)
                    {
                        PlaySound(0x208);
                        new UOACZFirefield(this).MoveToWorld(Location, Map);
                    }

                    GetSpecialAbilityEntryValue(SpecialAbilityEffect.Bile, out totalValue);

                    if (Utility.RandomDouble() <= totalValue)
                    {
                        PlaySound(0x230);
                        new UOACZBile(this).MoveToWorld(Location, Map);
                    }

                    if (!stealthMove && DateTime.UtcNow >= m_UOACZAccountEntry.UndeadProfile.m_NextMoveSoundAllowed)
                    {
                        m_UOACZAccountEntry.UndeadProfile.m_NextMoveSoundAllowed = DateTime.UtcNow + m_UOACZAccountEntry.UndeadProfile.MoveSoundDelay;
                        Effects.PlaySound(Location, Map, GetIdleSound());
                    }
                }
            }

            return true;
        }

        public override void SendStealthReadyNotification()
        {
            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage)
                SendMessage("You feel comfortable enough to begin stealthing.");

            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead)
                PrivateOverheadMessage(MessageType.Regular, 0, false, "You feel comfortable enough to begin stealthing.", NetState);
        }

        public override void SendStealthMovementReadyNotification()
        {
            if (m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateMessage)
                SendMessage("You feel ready to continue stealthing.");

            else if (NetState != null && m_StealthStepsDisplayMode == StealthStepsDisplayMode.PrivateOverhead)
                PrivateOverheadMessage(MessageType.Regular, 0, false, "You feel ready to continue stealthing.", NetState);
        }

        private bool m_BedrollLogout;

        public bool BedrollLogout
        {
            get { return m_BedrollLogout; }
            set { m_BedrollLogout = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Paralyzed
        {
            get
            {
                return base.Paralyzed;
            }
            set
            {
                base.Paralyzed = value;

                if (value)
                    AddBuff(new BuffInfo(BuffIcon.Paralyze, 1075827));	//Paralyze/You are frozen and can not move
                else
                    RemoveBuff(BuffIcon.Paralyze);
            }
        }

        public void RemoveGoldFromBankbox(int amount)
        {
            Item[] gold, checks;
            int balance = Banker.GetBalance(this, out gold, out checks);

            if (balance < amount)
                return;

            for (int i = 0; amount > 0 && i < gold.Length; ++i)
            {
                if (gold[i].Amount <= amount)
                {
                    amount -= gold[i].Amount;
                    gold[i].Delete();
                }

                else
                {
                    gold[i].Amount -= amount;
                    amount = 0;
                }
            }

            for (int i = 0; amount > 0 && i < checks.Length; ++i)
            {
                BankCheck check = (BankCheck)checks[i];

                if (check.Worth <= amount)
                {
                    amount -= check.Worth;
                    check.Delete();
                }
                else
                {
                    check.Worth -= amount;
                    amount = 0;
                }
            }
        }

        #region Ethics
        private Ethics.Player m_EthicPlayer;

        [CommandProperty(AccessLevel.GameMaster)]
        public Ethics.Player EthicPlayer
        {
            get { return m_EthicPlayer; }
            set { m_EthicPlayer = value; }
        }
        #endregion

        #region Factions
        private Factions.PlayerState m_FactionPlayerState;

        public Factions.PlayerState FactionPlayerState
        {
            get { return m_FactionPlayerState; }
            set { m_FactionPlayerState = value; }
        }
        #endregion

        #region Dueling
        private Engines.ConPVP.DuelContext m_DuelContext;
        private Engines.ConPVP.DuelPlayer m_DuelPlayer;

        public Engines.ConPVP.DuelContext DuelContext
        {
            get { return m_DuelContext; }
        }

        public Engines.ConPVP.DuelPlayer DuelPlayer
        {
            get { return m_DuelPlayer; }
            set
            {
                bool wasInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

                m_DuelPlayer = value;

                if (m_DuelPlayer == null)
                    m_DuelContext = null;
                else
                    m_DuelContext = m_DuelPlayer.Participant.Context;

                bool isInTourny = (m_DuelContext != null && !m_DuelContext.Finished && m_DuelContext.m_Tournament != null);

                if (wasInTourny != isInTourny)
                    SendEverything();
            }
        }
        #endregion

        #region Quests
        private QuestSystem m_Quest;
        private List<QuestRestartInfo> m_DoneQuests;
        private SolenFriendship m_SolenFriendship;

        public QuestSystem Quest
        {
            get { return m_Quest; }
            set { m_Quest = value; }
        }

        public List<QuestRestartInfo> DoneQuests
        {
            get { return m_DoneQuests; }
            set { m_DoneQuests = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SolenFriendship SolenFriendship
        {
            get { return m_SolenFriendship; }
            set { m_SolenFriendship = value; }
        }
        #endregion

        #region MyRunUO Invalidation       
        
        public override void OnKillsChange(int oldValue)
        {
            if (this.Young && this.ShortTermMurders > oldValue)
            {
                Account acc = this.Account as Account;

                if (acc != null)
                    acc.RemoveYoungStatus(0);
            }
        }

        public override void OnGenderChanged(bool oldFemale)
        {           
        }

        public override void OnGuildChange(Server.Guilds.BaseGuild oldGuild)
        {           
        }

        public override void OnGuildTitleChange(string oldTitle)
        {           
        }

        public override void OnKarmaChange(int oldValue)
        {          
        }

        public override void OnFameChange(int oldValue)
        {          
        }

        public override void OnSkillChange(SkillName skill, double oldBase)
        {
            if (this.Young && this.SkillsTotal >= 5000)
            {
                Account acc = this.Account as Account;

                if (acc != null)
                    acc.RemoveYoungStatus(1019036); // You have successfully obtained a respectable skill level, and have outgrown your status as a young player!
            }
        }

        public override void OnAccessLevelChanged(AccessLevel oldLevel)
        {
            if (AccessLevel == AccessLevel.Player)
                IgnoreMobiles = false;
            else
                IgnoreMobiles = true;
        }

        public override void OnRawStatChange(StatType stat, int oldValue)
        {
        }

        public void ReleaseAllFollowers()
        {
            var toRelease = new List<BaseCreature>();
            foreach (Mobile follower in AllFollowers)
            {
                if (follower == null) continue;
                if (follower is BladeSpirits || follower is EnergyVortex) continue;

                BaseCreature bc_Follower = follower as BaseCreature;

                if (bc_Follower != null)
                {
                    if (bc_Follower.AIObject != null)   
                        toRelease.Add(bc_Follower);                    
                }
            }

            foreach (var follower in toRelease)
                follower.AIObject.DoOrderRelease();
        }

        public override void OnDelete()
        {
            if (m_ReceivedHonorContext != null)
                m_ReceivedHonorContext.Cancel();
            if (m_SentHonorContext != null)
                m_SentHonorContext.Cancel();

            ReleaseAllFollowers();

            ArenaSystem.ArenaSystem.OnCharacterDeleted(this);

            if (Citizenship != null)            
                Town.RemoveCitizen(this, true);

            #region UOACZ

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(this);

            if (m_UOACZAccountEntry.MostRecentPlayer == this)
            {
                switch (m_UOACZAccountEntry.ActiveProfile)
                {
                    case UOACZAccountEntry.ActiveProfileType.Human:
                        UOACZSystem.DepositUOACZItems(this, true);
                    break;

                    case UOACZAccountEntry.ActiveProfileType.Undead:
                        UOACZSystem.DepositUOACZItems(this, false);
                    break;
                }
            }

            UOACZCharacterSnapshot snapshot = UOACZPersistance.FindCharacterSnapshot(this);

            if (snapshot != null)
            {
                if (!snapshot.Deleted)
                    snapshot.Delete();
            }

            #endregion
        }

        #endregion

        #region Fastwalk Prevention
        private static bool FastwalkPrevention = true; // Is fastwalk prevention enabled?
        private static int FastwalkThreshold = 400; // Fastwalk prevention will become active after 0.4 seconds

        private long m_NextMovementTime;
        private bool m_HasMoved;

        public virtual bool UsesFastwalkPrevention { get { return (AccessLevel < AccessLevel.Counselor) && !(Region is HorseLandRegion); } }

        public override int ComputeMovementSpeed(Direction dir, bool checkTurning)
        {
            if (checkTurning && (dir & Direction.Mask) != (this.Direction & Direction.Mask))
                return Mobile.RunMount;	// We are NOT actually moving (just a direction change)
                        
            TransformContext context = TransformationSpellHelper.GetContext(this);

            if (context != null && context.Type == typeof(ReaperFormSpell))
                return Mobile.WalkFoot;

            bool running = ((dir & Direction.Running) != 0);

            bool onHorse = (this.Mount != null);

            AnimalFormContext animalContext = AnimalForm.GetContext(this);

            if (onHorse || (animalContext != null && animalContext.SpeedBoost))
                return (running ? Mobile.RunMount : Mobile.WalkMount);

            return (running ? Mobile.RunFoot : Mobile.WalkFoot);
        }

        public static bool MovementThrottle_Callback(NetState ns)
        {
            PlayerMobile pm = ns.Mobile as PlayerMobile;

            if (pm == null || !pm.UsesFastwalkPrevention)
                return true;

            if (!pm.m_HasMoved)
            {
                // has not yet moved
                pm.m_NextMovementTime = Core.TickCount;
                pm.m_HasMoved = true;
                return true;
            }

            long ts = pm.m_NextMovementTime - Core.TickCount;

            if (ts < 0)
            {
                // been a while since we've last moved
                pm.m_NextMovementTime = Core.TickCount;
                return true;
            }

            return (ts < FastwalkThreshold);
        }

        #endregion

        #region Enemy of One
        private Type m_EnemyOfOneType;
        private bool m_WaitingForEnemy;

        public Type EnemyOfOneType
        {
            get { return m_EnemyOfOneType; }
            set
            {
                Type oldType = m_EnemyOfOneType;
                Type newType = value;

                if (oldType == newType)
                    return;

                m_EnemyOfOneType = value;

                DeltaEnemies(oldType, newType);
            }
        }

        public bool WaitingForEnemy
        {
            get { return m_WaitingForEnemy; }
            set { m_WaitingForEnemy = value; }
        }

        private void DeltaEnemies(Type oldType, Type newType)
        {
            IPooledEnumerable eable = this.GetMobilesInRange(18);

            foreach (Mobile m in eable)
            {
                Type t = m.GetType();

                if (t == oldType || t == newType)
                {
                    NetState ns = this.NetState;

                    if (ns != null)
                    {
                        if (ns.StygianAbyss)
                        {
                            ns.Send(new MobileMoving(m, Notoriety.Compute(this, m)));
                        }
                        else
                        {
                            ns.Send(new MobileMovingOld(m, Notoriety.Compute(this, m)));
                        }
                    }
                }
            }

            eable.Free();
        }

        #endregion

        #region Hair and beard mods
        private int m_HairModID = -1, m_HairModHue;
        private int m_BeardModID = -1, m_BeardModHue;

        public void SetHairMods(int hairID, int beardID)
        {
            if (hairID == -1)
                InternalRestoreHair(true, ref m_HairModID, ref m_HairModHue);
            else if (hairID != -2)
                InternalChangeHair(true, hairID, ref m_HairModID, ref m_HairModHue);

            if (beardID == -1)
                InternalRestoreHair(false, ref m_BeardModID, ref m_BeardModHue);
            else if (beardID != -2)
                InternalChangeHair(false, beardID, ref m_BeardModID, ref m_BeardModHue);
        }

        private void CreateHair(bool hair, int id, int hue)
        {
            if (hair)
            {
                //TODO Verification?
                HairItemID = id;
                HairHue = hue;
            }
            else
            {
                FacialHairItemID = id;
                FacialHairHue = hue;
            }
        }

        private void InternalRestoreHair(bool hair, ref int id, ref int hue)
        {
            if (id == -1)
                return;

            if (hair)
                HairItemID = 0;
            else
                FacialHairItemID = 0;

            //if( id != 0 )
            CreateHair(hair, id, hue);

            id = -1;
            hue = 0;
        }

        private void InternalChangeHair(bool hair, int id, ref int storeID, ref int storeHue)
        {
            if (storeID == -1)
            {
                storeID = hair ? HairItemID : FacialHairItemID;
                storeHue = hair ? HairHue : FacialHairHue;
            }
            CreateHair(hair, id, 0);
        }

        #endregion

        #region Virtues
        private DateTime m_LastSacrificeGain;
        private DateTime m_LastSacrificeLoss;
        private int m_AvailableResurrects;

        public DateTime LastSacrificeGain { get { return m_LastSacrificeGain; } set { m_LastSacrificeGain = value; } }
        public DateTime LastSacrificeLoss { get { return m_LastSacrificeLoss; } set { m_LastSacrificeLoss = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int AvailableResurrects { get { return m_AvailableResurrects; } set { m_AvailableResurrects = value; } }

        private DateTime m_NextJustAward;
        private DateTime m_LastJusticeLoss;
        private List<Mobile> m_JusticeProtectors;

        public DateTime LastJusticeLoss { get { return m_LastJusticeLoss; } set { m_LastJusticeLoss = value; } }
        public List<Mobile> JusticeProtectors { get { return m_JusticeProtectors; } set { m_JusticeProtectors = value; } }

        private DateTime m_LastCompassionLoss;
        private DateTime m_NextCompassionDay;
        private int m_CompassionGains;

        public DateTime LastCompassionLoss { get { return m_LastCompassionLoss; } set { m_LastCompassionLoss = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextCompassionDay { get { return m_NextCompassionDay; } set { m_NextCompassionDay = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CompassionGains { get { return m_CompassionGains; } set { m_CompassionGains = value; } }

        private DateTime m_LastValorLoss;

        public DateTime LastValorLoss { get { return m_LastValorLoss; } set { m_LastValorLoss = value; } }

        private DateTime m_LastHonorLoss;
        private DateTime m_LastHonorUse;
        private bool m_HonorActive;
        private HonorContext m_ReceivedHonorContext;
        private HonorContext m_SentHonorContext;
        public DateTime m_hontime;

        public DateTime LastHonorLoss { get { return m_LastHonorLoss; } set { m_LastHonorLoss = value; } }
        public DateTime LastHonorUse { get { return m_LastHonorUse; } set { m_LastHonorUse = value; } }
        public bool HonorActive { get { return m_HonorActive; } set { m_HonorActive = value; } }
        public HonorContext ReceivedHonorContext { get { return m_ReceivedHonorContext; } set { m_ReceivedHonorContext = value; } }
        public HonorContext SentHonorContext { get { return m_SentHonorContext; } set { m_SentHonorContext = value; } }
        #endregion

        #region Young system
        [CommandProperty(AccessLevel.GameMaster)]

        public bool Young
        {
            get
            {
                if (Region is UOACZRegion)
                    return false;

                return GetFlag(PlayerFlag.Young); 
            }

            set
            {
                SetFlag(PlayerFlag.Young, value);

                if (value)
                {
                    if (!YoungChatListeners.Contains(this))
                        YoungChatListeners.Add(this);
                }
                else
                {
                    if (YoungChatListeners.Contains(this))
                        YoungChatListeners.Remove(this);
                }

                InvalidateProperties();
            }
        }

        public override string ApplyNameSuffix(string suffix)
        {
            if (Young)
            {
                if (suffix.Length == 0)
                    suffix = "(Young)";
                else
                    suffix = String.Concat(suffix, " (Young)");
            }

            #region Ethics
            if (m_EthicPlayer != null)
            {
                if (suffix.Length == 0)
                    suffix = m_EthicPlayer.Ethic.Definition.Adjunct.String;
                else
                    suffix = String.Concat(suffix, " ", m_EthicPlayer.Ethic.Definition.Adjunct.String);
            }
            #endregion

            if (Core.ML && this.Map == Factions.Faction.Facet)
            {
                Factions.Faction faction = Factions.Faction.Find(this);

                if (faction != null)
                {
                    string adjunct = String.Format("[{0}]", faction.Definition.Abbreviation);
                    if (suffix.Length == 0)
                        suffix = adjunct;
                    else
                        suffix = String.Concat(suffix, " ", adjunct);
                }
            }

            return base.ApplyNameSuffix(suffix);
        }


        public override TimeSpan GetLogoutDelay()
        {
            if ((Young) || BedrollLogout || TestCenter.Enabled)
                return TimeSpan.Zero;

            return base.GetLogoutDelay();
        }

        private DateTime m_LastYoungMessage = DateTime.MinValue;

        public bool CheckYoungProtection(Mobile from)
        {
            if (!this.Young)
                return false;

            if (Region is UOACZRegion)
                return false;

            if (Region is BaseRegion && !((BaseRegion)Region).YoungProtected)
                return false;

            if (Region.IsPartOf(typeof(DungeonRegion)))
                return false;

            if (from is BaseCreature && ((BaseCreature)from).IgnoreYoungProtection)
                return false;

            if (this.Quest != null && this.Quest.IgnoreYoungProtection(from))
                return false;

            if (DateTime.UtcNow - m_LastYoungMessage > TimeSpan.FromMinutes(1.0))
            {
                m_LastYoungMessage = DateTime.UtcNow;
                SendLocalizedMessage(1019067); // A monster looks at you menacingly but does not attack.  You would be under attack now if not for your status as a new citizen of Britannia.
            }

            return true;
        }

        private DateTime m_LastYoungHeal = DateTime.MinValue;

        public bool CheckYoungHealTime()
        {
            if (DateTime.UtcNow - m_LastYoungHeal > TimeSpan.FromMinutes(5.0))
            {
                m_LastYoungHeal = DateTime.UtcNow;
                return true;
            }

            return false;
        }

        /*private static Point3D[] m_TrammelDeathDestinations = new Point3D[]
			{
				new Point3D( 1481, 1612, 20 ),
				new Point3D( 2708, 2153,  0 ),
				new Point3D( 2249, 1230,  0 ),
				new Point3D( 5197, 3994, 37 ),
				new Point3D( 1412, 3793,  0 ),
				new Point3D( 3688, 2232, 20 ),
				new Point3D( 2578,  604,  0 ),
				new Point3D( 4397, 1089,  0 ),
				new Point3D( 5741, 3218, -2 ),
				new Point3D( 2996, 3441, 15 ),
				new Point3D(  624, 2225,  0 ),
				new Point3D( 1916, 2814,  0 ),
				new Point3D( 2929,  854,  0 ),
				new Point3D(  545,  967,  0 ),
				new Point3D( 3665, 2587,  0 )
			};

        private static Point3D[] m_IlshenarDeathDestinations = new Point3D[]
			{
				new Point3D( 1216,  468, -13 ),
				new Point3D(  723, 1367, -60 ),
				new Point3D(  745,  725, -28 ),
				new Point3D(  281, 1017,   0 ),
				new Point3D(  986, 1011, -32 ),
				new Point3D( 1175, 1287, -30 ),
				new Point3D( 1533, 1341,  -3 ),
				new Point3D(  529,  217, -44 ),
				new Point3D( 1722,  219,  96 )
			};

        private static Point3D[] m_MalasDeathDestinations = new Point3D[]
			{
				new Point3D( 2079, 1376, -70 ),
				new Point3D(  944,  519, -71 )
			};

        private static Point3D[] m_TokunoDeathDestinations = new Point3D[]
			{
				new Point3D( 1166,  801, 27 ),
				new Point3D(  782, 1228, 25 ),
				new Point3D(  268,  624, 15 )
			};

        public bool YoungDeathTeleport()
        {
            if (this.Region.IsPartOf(typeof(Jail))
                || this.Region.IsPartOf("Samurai start location")
                || this.Region.IsPartOf("Ninja start location")
                || this.Region.IsPartOf("Ninja cave"))
                return false;

            Point3D loc;
            Map map;

            DungeonRegion dungeon = (DungeonRegion)this.Region.GetRegion(typeof(DungeonRegion));
            if (dungeon != null && dungeon.EntranceLocation != Point3D.Zero)
            {
                loc = dungeon.EntranceLocation;
                map = dungeon.EntranceMap;
            }
            else
            {
                loc = this.Location;
                map = this.Map;
            }

            Point3D[] list;

            if (map == Map.Trammel)
                list = m_TrammelDeathDestinations;
            else if (map == Map.Ilshenar)
                list = m_IlshenarDeathDestinations;
            else if (map == Map.Malas)
                list = m_MalasDeathDestinations;
            else if (map == Map.Tokuno)
                list = m_TokunoDeathDestinations;
            else
                return false;

            Point3D dest = Point3D.Zero;
            int sqDistance = int.MaxValue;

            for (int i = 0; i < list.Length; i++)
            {
                Point3D curDest = list[i];

                int width = loc.X - curDest.X;
                int height = loc.Y - curDest.Y;
                int curSqDistance = width * width + height * height;

                if (curSqDistance < sqDistance)
                {
                    dest = curDest;
                    sqDistance = curSqDistance;
                }
            }

            this.MoveToWorld(dest, map);
            return true;
        }*/

        private void SendYoungDeathNotice()
        {
            this.SendGump(new YoungDeathNotice());
        }

        #endregion

        public override bool CanHear(Mobile from)
        {
            #region UOACZ

            if (IsUOACZHuman)
            {
                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From != null)
                {
                    if (pm_From.IsUOACZUndead)
                        return false;
                }
            }

            if (IsUOACZUndead)
            {
                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From != null)
                {
                    if (pm_From.IsUOACZHuman)
                        return false;
                }
            }

            #endregion

            return true;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return base.HandlesOnSpeech(from);
        }

        #region Speech log
        private SpeechLog m_SpeechLog;

        public SpeechLog SpeechLog { get { return m_SpeechLog; } }

        public override void OnSpeech(SpeechEventArgs e)
        {
            if (SpeechLog.Enabled && this.NetState != null)
            {
                if (m_SpeechLog == null)
                    m_SpeechLog = new SpeechLog();

                m_SpeechLog.Add(e.Mobile, e.Speech);
            }
        }

        #endregion

        #region Champion Titles
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DisplayChampionTitle
        {
            get { return GetFlag(PlayerFlag.DisplayChampionTitle); }
            set { SetFlag(PlayerFlag.DisplayChampionTitle, value); }
        }

        private ChampionTitleInfo m_ChampionTitles;

        [CommandProperty(AccessLevel.GameMaster)]
        public ChampionTitleInfo ChampionTitles { get { return m_ChampionTitles; } set { } }

        private void ToggleChampionTitleDisplay()
        {
            if (!CheckAlive())
                return;

            if (DisplayChampionTitle)
                SendLocalizedMessage(1062419, "", 0x23); // You have chosen to hide your monster kill title.
            else
                SendLocalizedMessage(1062418, "", 0x23); // You have chosen to display your monster kill title.

            DisplayChampionTitle = !DisplayChampionTitle;
        }

        [PropertyObject]
        public class ChampionTitleInfo
        {
            public static TimeSpan LossDelay = TimeSpan.FromDays(1.0);
            public const int LossAmount = 90;

            private class TitleInfo
            {
                private int m_Value;
                private DateTime m_LastDecay;

                public int Value { get { return m_Value; } set { m_Value = value; } }
                public DateTime LastDecay { get { return m_LastDecay; } set { m_LastDecay = value; } }

                public TitleInfo()
                {
                }

                public TitleInfo(GenericReader reader)
                {
                    int version = reader.ReadEncodedInt();

                    switch (version)
                    {
                        case 0:
                            {
                                m_Value = reader.ReadEncodedInt();
                                m_LastDecay = reader.ReadDateTime();
                                break;
                            }
                    }
                }

                public static void Serialize(GenericWriter writer, TitleInfo info)
                {
                    writer.WriteEncodedInt((int)0); // version

                    writer.WriteEncodedInt(info.m_Value);
                    writer.Write(info.m_LastDecay);
                }
            }

            private TitleInfo[] m_Values;

            private int m_Harrower;	//Harrower titles do NOT decay

            public int GetValue(ChampionSpawnType type)
            {
                return GetValue((int)type);
            }

            public void SetValue(ChampionSpawnType type, int value)
            {
                SetValue((int)type, value);
            }

            public void Award(ChampionSpawnType type, int value)
            {
                Award((int)type, value);
            }

            public int GetValue(int index)
            {
                if (m_Values == null || index < 0 || index >= m_Values.Length)
                    return 0;

                if (m_Values[index] == null)
                    m_Values[index] = new TitleInfo();

                return m_Values[index].Value;
            }

            public DateTime GetLastDecay(int index)
            {
                if (m_Values == null || index < 0 || index >= m_Values.Length)
                    return DateTime.MinValue;

                if (m_Values[index] == null)
                    m_Values[index] = new TitleInfo();

                return m_Values[index].LastDecay;
            }

            public void SetValue(int index, int value)
            {
                if (m_Values == null)
                    m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                if (value < 0)
                    value = 0;

                if (index < 0 || index >= m_Values.Length)
                    return;

                if (m_Values[index] == null)
                    m_Values[index] = new TitleInfo();

                m_Values[index].Value = value;
            }

            public void Award(int index, int value)
            {
                if (m_Values == null)
                    m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                if (index < 0 || index >= m_Values.Length || value <= 0)
                    return;

                if (m_Values[index] == null)
                    m_Values[index] = new TitleInfo();

                m_Values[index].Value += value;
            }

            public void Atrophy(int index, int value)
            {
                if (m_Values == null)
                    m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                if (index < 0 || index >= m_Values.Length || value <= 0)
                    return;

                if (m_Values[index] == null)
                    m_Values[index] = new TitleInfo();

                int before = m_Values[index].Value;

                if ((m_Values[index].Value - value) < 0)
                    m_Values[index].Value = 0;
                else
                    m_Values[index].Value -= value;

                if (before != m_Values[index].Value)
                    m_Values[index].LastDecay = DateTime.UtcNow;
            }

            public override string ToString()
            {
                return "...";
            }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Pestilence { get { return GetValue(ChampionSpawnType.Pestilence); } set { SetValue(ChampionSpawnType.Pestilence, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Abyss { get { return GetValue(ChampionSpawnType.Abyss); } set { SetValue(ChampionSpawnType.Abyss, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Arachnid { get { return GetValue(ChampionSpawnType.Arachnid); } set { SetValue(ChampionSpawnType.Arachnid, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int ColdBlood { get { return GetValue(ChampionSpawnType.ColdBlood); } set { SetValue(ChampionSpawnType.ColdBlood, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int ForestLord { get { return GetValue(ChampionSpawnType.ForestLord); } set { SetValue(ChampionSpawnType.ForestLord, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int SleepingDragon { get { return GetValue(ChampionSpawnType.SleepingDragon); } set { SetValue(ChampionSpawnType.SleepingDragon, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int UnholyTerror { get { return GetValue(ChampionSpawnType.UnholyTerror); } set { SetValue(ChampionSpawnType.UnholyTerror, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int VerminHorde { get { return GetValue(ChampionSpawnType.VerminHorde); } set { SetValue(ChampionSpawnType.VerminHorde, value); } }

            [CommandProperty(AccessLevel.GameMaster)]
            public int Harrower { get { return m_Harrower; } set { m_Harrower = value; } }

            public ChampionTitleInfo()
            {
            }

            public ChampionTitleInfo(GenericReader reader)
            {
                int version = reader.ReadEncodedInt();

                switch (version)
                {
                    case 0:
                        {
                            m_Harrower = reader.ReadEncodedInt();

                            int length = reader.ReadEncodedInt();
                            m_Values = new TitleInfo[length];

                            for (int i = 0; i < length; i++)
                            {
                                m_Values[i] = new TitleInfo(reader);
                            }

                            if (m_Values.Length != ChampionSpawnInfo.Table.Length)
                            {
                                TitleInfo[] oldValues = m_Values;
                                m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                                for (int i = 0; i < m_Values.Length && i < oldValues.Length; i++)
                                {
                                    m_Values[i] = oldValues[i];
                                }
                            }
                            break;
                        }
                }
            }

            public static void Serialize(GenericWriter writer, ChampionTitleInfo titles)
            {
                writer.WriteEncodedInt((int)0); // version

                writer.WriteEncodedInt(titles.m_Harrower);

                int length = titles.m_Values.Length;
                writer.WriteEncodedInt(length);

                for (int i = 0; i < length; i++)
                {
                    if (titles.m_Values[i] == null)
                        titles.m_Values[i] = new TitleInfo();

                    TitleInfo.Serialize(writer, titles.m_Values[i]);
                }
            }

            public static void CheckAtrophy(PlayerMobile pm)
            {
                ChampionTitleInfo t = pm.m_ChampionTitles;
                if (t == null)
                    return;

                if (t.m_Values == null)
                    t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                for (int i = 0; i < t.m_Values.Length; i++)
                {
                    if ((t.GetLastDecay(i) + LossDelay) < DateTime.UtcNow)
                    {
                        t.Atrophy(i, LossAmount);
                    }
                }
            }

            public static void AwardHarrowerTitle(PlayerMobile pm)	//Called when killing a harrower.  Will give a minimum of 1 point.
            {
                ChampionTitleInfo t = pm.m_ChampionTitles;
                if (t == null)
                    return;

                if (t.m_Values == null)
                    t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                int count = 1;

                for (int i = 0; i < t.m_Values.Length; i++)
                {
                    if (t.m_Values[i].Value > 900)
                        count++;
                }

                t.m_Harrower = Math.Max(count, t.m_Harrower);	//Harrower titles never decay.
            }
        }

        #endregion

        #region Recipes

        private Dictionary<int, bool> m_AcquiredRecipes;

        public virtual bool HasRecipe(Recipe r)
        {
            if (r == null)
                return false;

            return HasRecipe(r.ID);
        }

        public virtual bool HasRecipe(int recipeID)
        {
            if (m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey(recipeID))
                return m_AcquiredRecipes[recipeID];

            return false;
        }

        public virtual void AcquireRecipe(Recipe r)
        {
            if (r != null)
                AcquireRecipe(r.ID);
        }

        public virtual void AcquireRecipe(int recipeID)
        {
            if (m_AcquiredRecipes == null)
                m_AcquiredRecipes = new Dictionary<int, bool>();

            m_AcquiredRecipes[recipeID] = true;
        }

        public virtual void ResetRecipes()
        {
            m_AcquiredRecipes = null;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int KnownRecipes
        {
            get
            {
                if (m_AcquiredRecipes == null)
                    return 0;

                return m_AcquiredRecipes.Count;
            }
        }

        #endregion

        #region Buff Icons

        public void ResendBuffs()
        {
            if (!BuffInfo.Enabled || m_BuffTable == null)
                return;

            NetState state = this.NetState;

            if (state != null && state.BuffIcon)
            {
                foreach (BuffInfo info in m_BuffTable.Values)
                {
                    state.Send(new AddBuffPacket(this, info));
                }
            }
        }

        private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

        public void AddBuff(BuffInfo b)
        {
            if (!BuffInfo.Enabled || b == null)
                return;

            RemoveBuff(b);	//Check & subsequently remove the old one.

            if (m_BuffTable == null)
                m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();

            m_BuffTable.Add(b.ID, b);

            NetState state = this.NetState;

            if (state != null && state.BuffIcon)
            {
                state.Send(new AddBuffPacket(this, b));
            }
        }

        public void RemoveBuff(BuffInfo b)
        {
            if (b == null)
                return;

            RemoveBuff(b.ID);
        }

        public void RemoveBuff(BuffIcon b)
        {
            if (m_BuffTable == null || !m_BuffTable.ContainsKey(b))
                return;

            BuffInfo info = m_BuffTable[b];

            if (info.Timer != null && info.Timer.Running)
                info.Timer.Stop();

            m_BuffTable.Remove(b);

            NetState state = this.NetState;

            if (state != null && state.BuffIcon)
            {
                state.Send(new RemoveBuffPacket(this, b));
            }

            if (m_BuffTable.Count <= 0)
                m_BuffTable = null;
        }
        #endregion

        #region Peacemaking Mode

        private PeacemakingModeEnum m_peacemakingMode;
        [CommandProperty(AccessLevel.GameMaster)]
        public PeacemakingModeEnum PeacemakingMode
        {
            get { return m_peacemakingMode; }
            set { m_peacemakingMode = value; }
        }

        #endregion

        public void AutoStablePets()
        {
            if (Core.SE && AllFollowers.Count > 0)
            {
                for (int i = m_AllFollowers.Count - 1; i >= 0; --i)
                {
                    BaseCreature pet = AllFollowers[i] as BaseCreature;

                    if (pet == null || pet.ControlMaster == null)
                        continue;

                    if (pet.Summoned)
                    {
                        if (pet.Map != Map)
                        {
                            pet.PlaySound(pet.GetAngerSound());
                            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(pet.Delete));
                        }
                        continue;
                    }

                    if (pet is IMount && ((IMount)pet).Rider != null)
                        continue;

                    if ((pet is PackLlama || pet is PackHorse || pet is Beetle) && (pet.Backpack != null && pet.Backpack.Items.Count > 0))
                        continue;

                    if (pet is BaseEscortable)
                        continue;

                    pet.ControlTarget = null;
                    pet.ControlOrder = OrderType.Stay;
                    pet.Internalize();

                    pet.SetControlMaster(null);
                    pet.SummonMaster = null;

                    pet.IsStabled = true;
                    pet.StabledBy = this;

                    pet.OwnerAbandonTime = DateTime.MaxValue;

                    pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

                    Stabled.Add(pet);
                    m_AutoStabled.Add(pet);
                }
            }
        }

        public void ClaimAutoStabledPets()
        {
            if (!Core.SE || m_AutoStabled.Count <= 0)
                return;

            if (!Alive)
            {
                SendLocalizedMessage(1076251); // Your pet was unable to join you while you are a ghost.  Please re-login once you have ressurected to claim your pets.				
                return;
            }

            for (int i = m_AutoStabled.Count - 1; i >= 0; --i)
            {
                BaseCreature pet = m_AutoStabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;
                    pet.StabledBy = null;

                    pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

                    if (Stabled.Contains(pet))
                        Stabled.Remove(pet);

                    continue;
                }

                if ((Followers + pet.ControlSlots) <= FollowersMax)
                {
                    pet.SetControlMaster(this);

                    if (pet.Summoned)
                        pet.SummonMaster = this;

                    pet.ControlTarget = this;
                    pet.ControlOrder = OrderType.Follow;

                    pet.MoveToWorld(Location, Map);

                    pet.IsStabled = false;
                    pet.StabledBy = null;

                    pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

                    pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully Happy

                    if (Stabled.Contains(pet))
                        Stabled.Remove(pet);
                }

                else
                {
                    SendLocalizedMessage(1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.
                }
            }

            m_AutoStabled.Clear();
        }

        public bool SpecialAbilityEffectLookupInProgress = false;

        public List<SpecialAbilityEffectEntry> m_SpecialAbilityEffectEntries = new List<SpecialAbilityEffectEntry>();
        public List<SpecialAbilityEffectEntry> m_SpecialAbilityEffectEntriesToAdd = new List<SpecialAbilityEffectEntry>();
        public List<SpecialAbilityEffectEntry> m_SpecialAbilityEffectEntriesToRemove = new List<SpecialAbilityEffectEntry>();

        public SpecialAbilityEffectTimer m_SpecialAbilityEffectTimer;
        public class SpecialAbilityEffectTimer : Timer
        {
            private Mobile m_Mobile;

            public SpecialAbilityEffectTimer(Mobile mobile)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(100))
            {
                m_Mobile = mobile;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                PlayerMobile pm_Player = m_Mobile as PlayerMobile;

                if (pm_Player == null)
                    return;

                List<SpecialAbilityEffectEntry> entriesToRemove = new List<SpecialAbilityEffectEntry>();

                int entries = pm_Player.m_SpecialAbilityEffectEntries.Count;

                for (int a = 0; a < entries; a++)
                {
                    if (pm_Player.SpecialAbilityEffectLookupInProgress)
                        break;

                    SpecialAbilityEffectEntry entry = pm_Player.m_SpecialAbilityEffectEntries[a];

                    if (entry == null)
                        continue;

                    if (entry.m_SpecialAbilityEffect == SpecialAbilityEffect.Hinder)
                    {
                        if (DateTime.UtcNow >= entry.m_Expiration)
                        {
                            pm_Player.Frozen = false;

                            if (!(pm_Player.Region is UOACZRegion))
                                pm_Player.SendMessage("You are no longer hindered.");
                        }
                    }

                    if (entry.m_SpecialAbilityEffect == SpecialAbilityEffect.Petrify)
                    {
                        if (DateTime.UtcNow >= entry.m_Expiration)
                        {                            
                            if (!KinPaint.IsWearingKinPaint(pm_Player))
                                pm_Player.HueMod = -1;

                            pm_Player.Frozen = false;
                            pm_Player.SendMessage("You are no longer petrified.");
                        }
                    }

                    if (entry.m_SpecialAbilityEffect == SpecialAbilityEffect.Entangle)
                    {
                        if (DateTime.UtcNow >= entry.m_Expiration)
                        {
                            pm_Player.CantWalk = false;
                            pm_Player.SendMessage("You are no longer entangled.");
                        }
                    }

                    if (entry.m_SpecialAbilityEffect == SpecialAbilityEffect.Bleed)
                    {
                        if (DateTime.UtcNow >= entry.m_Expiration)
                        {
                            int minBlood = 1;
                            int maxBlood = 2;                            

                            if (!pm_Player.Hidden)
                                SpecialAbilities.AddBloodEffect(pm_Player, minBlood, maxBlood);

                            int damage = (int)entry.m_Value;

                            int finalAdjustedDamage = AOS.Damage(pm_Player, entry.m_Owner, damage, 0, 100, 0, 0, 0);

                            Mobile m_Owner = entry.m_Owner as Mobile;
                            BaseCreature bc_Owner = entry.m_Owner as BaseCreature;
                            PlayerMobile pm_Owner = entry.m_Owner as PlayerMobile;

                            if (bc_Owner != null)
                                bc_Owner.DisplayFollowerDamage(pm_Player, finalAdjustedDamage);
                            
                            if (pm_Owner != null)
                            {
                                if (pm_Owner.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                                    pm_Owner.SendMessage(pm_Owner.PlayerMeleeDamageTextHue, pm_Player.Name + " bleeds for " + finalAdjustedDamage.ToString() + " damage.");

                                if (pm_Owner.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                                    pm_Player.PrivateOverheadMessage(MessageType.Regular, pm_Owner.PlayerMeleeDamageTextHue, false, "-" + finalAdjustedDamage.ToString(), pm_Owner.NetState);
                            }
                        }
                    }

                    if (entry.m_SpecialAbilityEffect == SpecialAbilityEffect.Disease)
                    {
                        if (DateTime.UtcNow >= entry.m_Expiration)
                        {
                            int damage = (int)entry.m_Value;
                           
                            if (!pm_Player.Hidden)
                            {
                                Effects.PlaySound(pm_Player.Location, pm_Player.Map, 0x5CB);
                                Effects.SendLocationParticles(EffectItem.Create(pm_Player.Location, pm_Player.Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, 2199, 0, 5029, 0);

                                pm_Player.PublicOverheadMessage(MessageType.Regular, 1103, false, "*looks violently ill*");

                                Blood blood = new Blood();
                                blood.Hue = 2200;
                                blood.MoveToWorld(pm_Player.Location, pm_Player.Map);

                                int extraBlood = Utility.RandomMinMax(1, 2);

                                for (int i = 0; i < extraBlood; i++)
                                {
                                    Blood moreBlood = new Blood();
                                    moreBlood.Hue = 2200;
                                    moreBlood.MoveToWorld(new Point3D(pm_Player.Location.X + Utility.RandomMinMax(-1, 1), pm_Player.Location.Y + Utility.RandomMinMax(-1, 1), pm_Player.Location.Z), pm_Player.Map);
                                }
                            }

                            int finalAdjustedDamage = AOS.Damage(pm_Player, entry.m_Owner, damage, 0, 100, 0, 0, 0);

                            Mobile m_Owner = entry.m_Owner as Mobile;
                            BaseCreature bc_Owner = entry.m_Owner as BaseCreature;
                            PlayerMobile pm_Owner = entry.m_Owner as PlayerMobile;

                            if (bc_Owner != null)
                                bc_Owner.DisplayFollowerDamage(pm_Player, finalAdjustedDamage);
                        }
                    }

                    if (DateTime.UtcNow >= entry.m_Expiration)
                        pm_Player.RemoveSpecialAbilityEffectEntry(entry);
                }

                if (pm_Player.m_SpecialAbilityEffectEntries.Count == 0)
                    this.Stop();
            }
        }

        public AddSpecialAbilityEffectTimer m_AddSpecialAbilityEffectTimer;
        public class AddSpecialAbilityEffectTimer : Timer
        {
            private Mobile m_Mobile;

            public AddSpecialAbilityEffectTimer(Mobile mobile)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(50))
            {
                m_Mobile = mobile;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                PlayerMobile pm_Player = m_Mobile as PlayerMobile;

                if (pm_Player == null)
                    return;

                int entriesToAdd = pm_Player.m_SpecialAbilityEffectEntriesToAdd.Count;

                for (int a = 0; a < entriesToAdd; a++)
                {
                    if (pm_Player.SpecialAbilityEffectLookupInProgress)
                        break;

                    pm_Player.m_SpecialAbilityEffectEntries.Add(pm_Player.m_SpecialAbilityEffectEntriesToAdd[0]);
                    pm_Player.m_SpecialAbilityEffectEntriesToAdd.RemoveAt(0);

                    if (pm_Player.m_SpecialAbilityEffectTimer == null)
                    {
                        pm_Player.m_SpecialAbilityEffectTimer = new SpecialAbilityEffectTimer(pm_Player);
                        pm_Player.m_SpecialAbilityEffectTimer.Start();
                    }

                    else
                    {
                        if (!pm_Player.m_SpecialAbilityEffectTimer.Running)
                            pm_Player.m_SpecialAbilityEffectTimer.Start();
                    }
                }

                if (pm_Player.m_SpecialAbilityEffectEntriesToAdd.Count == 0)
                    this.Stop();
            }
        }

        public RemoveSpecialAbilityEffectTimer m_RemoveSpecialAbilityEffectTimer;
        public class RemoveSpecialAbilityEffectTimer : Timer
        {
            private Mobile m_Mobile;

            public RemoveSpecialAbilityEffectTimer(Mobile mobile)
                : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(50))
            {
                m_Mobile = mobile;
                Priority = TimerPriority.TwentyFiveMS;
            }

            protected override void OnTick()
            {
                PlayerMobile pm_Player = m_Mobile as PlayerMobile;

                if (pm_Player == null)
                    return;

                int entriesToRemove = pm_Player.m_SpecialAbilityEffectEntriesToRemove.Count;
                for (int a = 0; a < entriesToRemove; a++)
                {
                    if (pm_Player.SpecialAbilityEffectLookupInProgress)
                        break;

                    pm_Player.m_SpecialAbilityEffectEntries.Remove(pm_Player.m_SpecialAbilityEffectEntriesToRemove[0]);
                    pm_Player.m_SpecialAbilityEffectEntriesToRemove.RemoveAt(0);

                    if (pm_Player.m_SpecialAbilityEffectEntries.Count == 0)
                    {
                        if (pm_Player.m_SpecialAbilityEffectTimer != null)
                        {
                            if (!pm_Player.m_SpecialAbilityEffectTimer.Running)
                                pm_Player.m_SpecialAbilityEffectTimer.Stop();
                        }
                    }
                }

                if (pm_Player.m_SpecialAbilityEffectEntriesToRemove.Count == 0)
                    this.Stop();
            }
        }

        public void AddSpecialAbilityEffectEntry(SpecialAbilityEffectEntry entryToAdd)
        {
            m_SpecialAbilityEffectEntriesToAdd.Add(entryToAdd);

            if (m_AddSpecialAbilityEffectTimer == null)
            {
                m_AddSpecialAbilityEffectTimer = new AddSpecialAbilityEffectTimer(this);
                m_AddSpecialAbilityEffectTimer.Start();
            }

            else
            {
                if (!m_AddSpecialAbilityEffectTimer.Running)
                    m_AddSpecialAbilityEffectTimer.Start();
            }
        }

        public void RemoveSpecialAbilityEffectEntry(SpecialAbilityEffectEntry entryToRemove)
        {
            m_SpecialAbilityEffectEntriesToRemove.Add(entryToRemove);

            if (m_RemoveSpecialAbilityEffectTimer == null)
            {
                m_RemoveSpecialAbilityEffectTimer = new RemoveSpecialAbilityEffectTimer(this);
                m_RemoveSpecialAbilityEffectTimer.Start();
            }

            else
            {
                if (!m_RemoveSpecialAbilityEffectTimer.Running)
                    m_RemoveSpecialAbilityEffectTimer.Start();
            }
        }

        public void GetSpecialAbilityEntryValue(SpecialAbilityEffect effectType, out double value)
        {
            int count = 0;
            double totalValue = 0;

            if (m_SpecialAbilityEffectEntries != null)
            {
                SpecialAbilityEffectLookupInProgress = true;

                foreach (SpecialAbilityEffectEntry entry in m_SpecialAbilityEffectEntries)
                {
                    if (entry.m_SpecialAbilityEffect == effectType && DateTime.UtcNow < entry.m_Expiration)
                        totalValue += entry.m_Value;
                }

                SpecialAbilityEffectLookupInProgress = false;
            }

            value = totalValue;
        }
    }
}
