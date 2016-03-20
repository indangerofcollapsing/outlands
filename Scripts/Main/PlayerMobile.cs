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
using Server.Guilds;
using Server.Regions;
using Server.Poker;
using System.Text;

namespace Server.Mobiles
{
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
    
    public enum BlockMountType
    {
        None = -1,
        Dazed = 1040024,
        BolaRecovery = 1062910,
        DismountRecovery = 1070859
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

    public partial class PlayerMobile : Mobile
    {
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

        #region Commands

        public static void Initialize()
        {
            if (FastwalkPrevention)
                PacketHandlers.RegisterThrottler(0x02, new ThrottlePacketCallback(MovementThrottle_Callback));

            EventSink.Login += new LoginEventHandler(OnLogin);
            EventSink.Logout += new LogoutEventHandler(OnLogout);
            EventSink.Connected += new ConnectedEventHandler(EventSink_Connected);
            EventSink.Disconnected += new DisconnectedEventHandler(EventSink_Disconnected);

            CommandSystem.Register("WipePlayerMobiles", AccessLevel.Administrator, new CommandEventHandler(WipeAllPlayerMobiles_OnCommand));
            CommandSystem.Register("UseTrapPouch", AccessLevel.Player, new CommandEventHandler(UseTrappedPouch_OnCommand));

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

            CommandSystem.Register("gotoentrance", AccessLevel.Administrator, new CommandEventHandler(GoToEntranceCommand));
            CommandSystem.Register("playercount", AccessLevel.Administrator, new CommandEventHandler(PlayerCountCommand));
            CommandSystem.Register("setthreshold", AccessLevel.Administrator, new CommandEventHandler(SetThresholdCommand));
            CommandSystem.Register("togglethreshold", AccessLevel.Administrator, new CommandEventHandler(ToggleThresholdCommand));

            //Used for Locally Testing Content
            CommandSystem.Register("CreateTestLoadout", AccessLevel.GameMaster, new CommandEventHandler(CreateTestLoadout));
            CommandSystem.Register("Anim", AccessLevel.GameMaster, new CommandEventHandler(Anim));
            CommandSystem.Register("AnimationTest", AccessLevel.GameMaster, new CommandEventHandler(AnimationTest));
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

        [Usage("UseTrappedPouch")]
        [Description("Uses a trapped pouch in your backpack")]
        public static void UseTrappedPouch_OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if (pm != null && pm.Backpack != null)
            {
                if (pm.m_LastTrapPouchUse + TimeSpan.FromSeconds(0.75) > DateTime.UtcNow)                
                    pm.SendMessage("You must wait 0.75 seconds between each use of this command");                

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

        #endregion

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

                if (pm_From.AccessLevel > AccessLevel.Player)
                    pm_From.Send(SpeedControl.MountSpeed);
            }

            //Player Enhancements
            if (pm_From.m_PlayerEnhancementAccountEntry == null)
                PlayerEnhancementPersistance.CreatePlayerEnhancementAccountEntry(pm_From);

            if (pm_From.m_PlayerEnhancementAccountEntry.Deleted)
                PlayerEnhancementPersistance.CreatePlayerEnhancementAccountEntry(pm_From);

            PlayerCustomization.OnLoginAudit(pm_From);

            //Audit Enhancements For New Entries Available
            pm_From.m_PlayerEnhancementAccountEntry.AuditCustomizationEntries();
            pm_From.m_PlayerEnhancementAccountEntry.AuditSpellHueEntries();

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
                    PokerPlayer player = pm.m_PokerGame.GetPlayer(pm);
                    if (player != null)
                    {
                        if (pm.m_PokerGame.Players != null && pm.m_PokerGame.Players.Contains(player))
                            pm.m_PokerGame.RemovePlayer(player);
                    }
                }

                if (YoungChatListeners.Contains(pm))
                    YoungChatListeners.Remove(pm);

                TimeSpan gameTime = DateTime.UtcNow - pm.m_SessionStart;

                pm.m_GameTime += gameTime;
                
                pm.m_SpeechLog = null;
                pm.LastOnline = DateTime.UtcNow;
                pm.SetSallos(false);
            }

            DisguiseTimers.StopTimer(from);
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

        public DateTime m_LastTrapPouchUse = DateTime.UtcNow;
        
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
        
        public DamageDisplayMode m_ShowMeleeDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowSpellDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowFollowerDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowProvocationDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowPoisonDamage = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowDamageTaken = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowFollowerDamageTaken = DamageDisplayMode.None;
        public DamageDisplayMode m_ShowHealing = DamageDisplayMode.None;

        public int PlayerMeleeDamageTextHue = 0x022;
        public int PlayerSpellDamageTextHue = 0x075;
        public int PlayerFollowerDamageTextHue = 0x59;
        public int PlayerProvocationDamageTextHue = 0x90;
        public int PlayerPoisonDamageTextHue = 0x03F;
        public int PlayerDamageTakenTextHue = 0;
        public int PlayerFollowerDamageTakenTextHue = 0;
        public int PlayerHealingTextHue = 2213;

        public StealthStepsDisplayMode m_StealthStepsDisplayMode = StealthStepsDisplayMode.PrivateMessage;
        public HenchmenSpeechDisplayMode m_HenchmenSpeechDisplayMode = HenchmenSpeechDisplayMode.Normal;

        public bool m_ShowAdminFilterText = true; 

        public bool m_AutoStealth = true;

        private BaseBoat m_BoatOccupied = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseBoat BoatOccupied { get { return m_BoatOccupied; } set { m_BoatOccupied = value; } }        
        
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
               
        public int SystemOverloadActions = 0;       
        public static int SystemOverloadActionThreshold = 180; //Player flagged if attacking a single target this many times over the SystemOverloadInterval
        public static TimeSpan SystemOverloadInterval = TimeSpan.FromSeconds(60); 
                       
        public DateTime LastTeamSwitch = DateTime.MinValue;
        
        private PokerGame m_PokerGame;
        public PokerGame PokerGame
        {
            get { return m_PokerGame; }
            set { m_PokerGame = value; }
        }

        private int m_NumGoldCoinsGenerated;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NumGoldCoinsGenerated
        {
            get { return m_NumGoldCoinsGenerated; }
            set { m_NumGoldCoinsGenerated = value; }
        }
        
        #region Insta-Hit

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
        
        #region Captcha
        
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

            public HarvestTimer(PlayerMobile player, DateTime started): base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(15))
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

                    SendGump((new Server.Custom.AntiRailing.HarvestGump(this)));

                    if (m_HarvestTimer != null && m_HarvestTimer.Running)
                        m_HarvestTimer.Stop();

                    m_HarvestTimer = new HarvestTimer(this, DateTime.UtcNow);
                    m_HarvestTimer.Start();
                }
            }
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

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Murderer 
        {
            get 
            {
                return (ShortTermMurders >= 5);
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
        public bool BoatMovement
        {
            get { return GetFlag(PlayerFlag.BoatMovement); }
            set { SetFlag(PlayerFlag.BoatMovement, value); }
        }
        
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

        public bool CheckPlayerAccountsForCommonGuild(PlayerMobile player2)
        {
            bool foundSameGuild = false;

            List<Guilds.BaseGuild> m_Guilds = new List<Guilds.BaseGuild>();
            List<Guilds.BaseGuild> secondPlayerGuilds = new List<Guilds.BaseGuild>();

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

            Account secondPlayerAccount = player2.Account as Account;

            if (secondPlayerAccount != null)
            {
                for (int a = 0; a < (secondPlayerAccount.Length - 1); a++)
                {
                    Mobile m_Mobile = secondPlayerAccount.accountMobiles[a] as Mobile;

                    if (m_Mobile != null)
                    {
                        if (!m_Mobile.Deleted && m_Mobile.Guild != null)
                            secondPlayerGuilds.Add(m_Mobile.Guild);
                    }
                }
            }

            foreach (Guilds.BaseGuild player1Guild in m_Guilds)
            {
                foreach (Guilds.BaseGuild player2Guild in secondPlayerGuilds)
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
        
        public PlayerTitleColors TitleColorState { get; set; }
        public int SelectedTitleColorIndex;
        public EColorRarity SelectedTitleColorRarity;        
        private int m_CanReprieve;
        private bool CanReprieveBool = false;
        public TimeSpan m_TimeSpanDied;
        public DateTime m_DateTimeDied;
        public TimeSpan m_TimeSpanResurrected;
        public List<string> PreviousNames = new List<string>();
        public DateTime HueModEnd { get; set; }
        public TimeSpan LoginElapsedTime { get; set; }

        private DateTime m_Created = DateTime.UtcNow;
        public DateTime CreatedOn { set { m_Created = value; } get { return m_Created; } }
        public Boolean CloseBankRunebookGump;
        
        public TimeSpan m_ShortTermElapse;
        public TimeSpan m_LongTermElapse;

        #region UOACZ

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

        #endregion
        
        public void EnterContestedRegion(bool ressingHere)
        {            
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
            //Bring Boosted Stat Durations Down to Normal Maximum If PvP Occurs
            TimeSpan MaximumPvPDuration = TimeSpan.FromMinutes(2);

            for (int i = 0; i < mobile.StatMods.Count; ++i)
            {
                StatMod check = mobile.StatMods[i];

                if (mobile.Region is UOACZRegion)
                    return;

                if (check.Type == StatType.Str || check.Type == StatType.Dex || check.Type == StatType.Int)
                {
                    if (check.Duration >= MaximumPvPDuration)
                        check.Duration = MaximumPvPDuration;
                }
            }
        }

        public override void OnHeal(ref int amount, Mobile from)
        {
            base.OnHeal(ref amount, from);

            SpecialAbilities.HealingOccured(from, this, amount);            
        }
        
        //Easy UO Detection
        private Serial m_LastTarget;
        [CommandProperty(AccessLevel.GameMaster)]
        public Serial LastTarget
        {
            get { return m_LastTarget; }
            set { m_LastTarget = value; }
        }

        public bool m_UserOptHideFameTitles;
        public override bool ShowFameTitle
        {
            get { return m_UserOptHideFameTitles ? false : base.ShowFameTitle; }
        }

        private DateTime m_LastDeathByPlayer;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime LastDeathByPlayer
        {
            get { return m_LastDeathByPlayer; }
            set { m_LastDeathByPlayer = value; }
        }
      
        private int m_PirateScore = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int PirateScore
        {
            get { return m_PirateScore; }
            set
            {
                m_PirateScore = value;
            }
        }

        public Server.Custom.DonationState DonationPlayerState { get; set; }

        private DateTime m_LastTownSquareNotification = DateTime.MinValue;
        public DateTime LastTownSquareNotification
        {
            get { return m_LastTownSquareNotification; }
            set { m_LastTownSquareNotification = value; }
        }
        
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
       
        private TimeSpan m_NpcGuildGameTime;

        private PlayerFlag m_Flags;                       

        private bool m_IgnoreMobiles; // IgnoreMobiles should be moved to Server.Mobiles        
        private int m_NonAutoreinsuredItems; // number of items that could not be automatically reinsured because gold in bank was not enough
        
        private DateTime m_LastOnline;
        private Server.Guilds.RankDefinition m_GuildRank = Server.Guilds.RankDefinition.Lowest;

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

        private Point3D m_LastLocation = Point3D.Zero;
        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D LastLocation
        {
            get { return m_LastLocation; }
            set { m_LastLocation = value; }
        }
        
        private BaseInstrument m_LastInstrument;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseInstrument LastInstrument
        {
            get { return m_LastInstrument; }
            set { m_LastInstrument = value; }
        }

        private List<Mobile> m_AutoStabled = new List<Mobile>();
        public List<Mobile> AutoStabled
        {
            get { return m_AutoStabled; } 
        }

        private List<Mobile> m_AllFollowers;
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

        private int m_Profession;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Profession
        {
            get { return m_Profession; }
            set { m_Profession = value; }
        }

        private int m_StepsTaken;
        public int StepsTaken
        {
            get { return m_StepsTaken; }
            set { m_StepsTaken = value; }
        }

        private bool m_IsStealthing;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsStealthing
        {
            get { return m_IsStealthing; }
            set { m_IsStealthing = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IgnoreMobiles
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
                if (Mount != null)                
                    Mount.Rider = null;
                
                else if (AnimalForm.UnderTransformation(this))                
                    AnimalForm.RemoveContext(this, true);                
            }

            if ((m_MountBlock == null) || !m_MountBlock.m_Timer.Running || (m_MountBlock.m_Timer.Next < (DateTime.UtcNow + duration)))            
                m_MountBlock = new MountBlock(duration, type, this);            
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

        public override void RevealingAction()
        {
            if (m_DesignContext != null)
                return;

            Spells.Sixth.InvisibilitySpell.RemoveTimer(this);                  

            base.RevealingAction();

            TrueHidden = false;
            m_IsStealthing = false; // IsStealthing should be moved to Server.Mobiles
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override bool Hidden
        {
            get { return base.Hidden; }

            set
            {
                if (value && HideRestrictionExpiration > DateTime.UtcNow)
                {
                    string hideRestrictionRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, HideRestrictionExpiration, false, false, false, true, true);
                    
                    SendMessage("You are unable to hide for another " + hideRestrictionRemaining + ".");

                    return;
                }

                base.Hidden = value;

                RemoveBuff(BuffIcon.Invisibility);	//Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

                if (!Hidden)                
                    RemoveBuff(BuffIcon.HidingAndOrStealth);
                
                else // if( !InvisibilitySpell.HasTimer( this ) )                
                    BuffInfo.AddBuff(this, new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));	//Hidden/Stealthing & You Are Hidden                
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

            if ((target is BaseCreature && ((BaseCreature)target).IsInvulnerable) || target is PlayerVendor)
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

            if (NetState != null)
                CheckLightLevels(false);           
        }

        public override void OnItemRemoved(Item item)
        {
            base.OnItemRemoved(item);

            if (item is BaseArmor || item is BaseWeapon)
            {
                Hits = Hits; Stam = Stam; Mana = Mana;
            }

            if (NetState != null)
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
                if (HasGump(typeof(ResurrectGump)) )
                {
                    if (Alive)                    
                        CloseGump(typeof(ResurrectGump));   

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
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            if (from == this)
            {
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
                }
            }
        }

        private void CancelProtection()
        {
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
            
            base.OnBeneficialAction(target, isCriminal);
        }

        public override void CriminalAction(bool message)
        {
            base.CriminalAction(message);
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
                        
            base.OnDamage(amount, from, willKill);
        }
                
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

        public override void Resurrect()
        {
            m_TimeSpanResurrected = this.GameTime;
            
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
            
            DropHolding();

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

            if (item.QuestItem)
                item.QuestItem = false;

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

                Server.Misc.FameKarmaTitles.AwardFame(g, fameAward, false);
                Server.Misc.FameKarmaTitles.AwardKarma(g, karmaAward, true);
            }

            if (NpcGuild == NpcGuild.ThievesGuild)
                return;

            bool justiceDisabledZone = DuelContext != null ||
                                        SpellHelper.InBuccs(Map, Location) || SpellHelper.InYewOrcFort(Map, Location) || SpellHelper.InYewCrypts(Map, Location) ||
                                        GreyZoneTotem.InGreyZoneTotemArea(Location, Map) || Hotspot.InHotspotArea(Location, Map, true);

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

                if (totalMurdererDamage >= MinDamageRequiredForMurdererDeath && murdererClaimCount > 0 && highestMurdererDamager != null)
                    killedByMurderer = true;
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

            //Player Enhancement Customization: Carnage and Violent Death
            bool carnage = PlayerEnhancementPersistance.IsCustomizationEntryActive(killer, CustomizationType.Carnage);
            bool violentDeath = PlayerEnhancementPersistance.IsCustomizationEntryActive(this, CustomizationType.ViolentDeath);
            
            if (carnage && Utility.RandomDouble() >= .75)
                carnage = false;

            if (violentDeath && Utility.RandomDouble() >= .75)
                violentDeath = false;           

            if ((carnage || violentDeath) && !(Region is UOACZRegion))
                CustomizationAbilities.PlayerDeathExplosion(Location, Map, carnage, violentDeath);  
            
            if (m_DuelContext != null)
                m_DuelContext.OnDeath(this, c);                     

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
            
            OnWarmodeChanged();
        }

        public void ConsiderSins()
        {
            SendMessage("Murder Counts: {0}", ShortTermMurders);
            SendMessage("Lifetime Murder Counts: {0}", Kills);

            if (ShortTermMurders > 0)
            {
                TimeSpan expiration = GameTime - m_ShortTermElapse;

                string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + expiration, false, true, true, true, false);

                SendMessage("Your next murder count will decay in " + timeRemaining + ".");
            }
        }

        private List<Mobile> m_PermaFlags = new List<Mobile>();
        private List<Mobile> m_VisList;
        private Hashtable m_AntiMacroTable;
        private TimeSpan m_GameTime;
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
            m_LastTarget = Serial.MinusOne;
            m_AutoStabled = new List<Mobile>();

            m_VisList = new List<Mobile>();
            m_PermaFlags = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();

            m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            m_GameTime = TimeSpan.Zero;

            m_ShortTermElapse = TimeSpan.FromHours(MurderCountDecayHours);
            m_LongTermElapse = TimeSpan.FromHours(0.0);

            m_UserOptHideFameTitles = true;

            TitleColorState = new PlayerTitleColors();
            
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
                    //m_GuildMessageHue = hue;

                    g.GuildChat(this, text);
                    SendToStaffMessage(this, "[Guild]: {0}", text);
                }
            }

            else if (type == MessageType.Alliance)
            {               
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

        private Engines.BulkOrders.BOBFilter m_BOBFilter = new Engines.BulkOrders.BOBFilter();
        public Engines.BulkOrders.BOBFilter BOBFilter
        {
            get { return m_BOBFilter; }
        }        
        
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
                        
            writer.Write((int)0); //Version

            //Version 0            
            writer.Write(m_SpecialAbilityEffectEntries.Count);
            for (int a = 0; a < m_SpecialAbilityEffectEntries.Count; a++)
            {
                writer.Write((int)m_SpecialAbilityEffectEntries[a].m_SpecialAbilityEffect);
                writer.Write((Mobile)m_SpecialAbilityEffectEntries[a].m_Owner);
                writer.Write((double)m_SpecialAbilityEffectEntries[a].m_Value);
                writer.Write((DateTime)m_SpecialAbilityEffectEntries[a].m_Expiration);
            }
            
            writer.Write(PreviousNames.Count);
            for (int a = 0; a < PreviousNames.Count; a++)
            {
                writer.Write(PreviousNames[a]);
            }
            
            writer.WriteEncodedInt(m_GuildRank.Rank);            

            writer.Write(m_EventCalendarAccount);
            writer.Write(m_BonusSkillCap);
            writer.Write(m_MHSPlayerEntry);
            writer.Write((int)m_ShowHealing);
            writer.Write(m_WorldChatAccountEntry);
            writer.Write(m_UOACZAccountEntry);
            writer.Write(m_HideRestrictionExpiration);
            writer.Write((int)m_HenchmenSpeechDisplayMode);
            writer.Write((int)m_StealthStepsDisplayMode);
            writer.Write(m_ShowAdminFilterText);
            writer.Write(m_PlayerEnhancementAccountEntry);
            writer.Write(m_InfluenceAccountEntry);
            writer.Write((int)m_ShowFollowerDamageTaken);
            writer.Write(m_LastPlayerKilledBy);
            writer.Write(m_LastInstrument);
            writer.Write((int)m_ShowDamageTaken);
            writer.Write((int)m_ShowProvocationDamage);
            writer.Write((int)m_ShowPoisonDamage);
            writer.Write(m_AutoStealth);
            writer.Write(m_BoatOccupied); 
            writer.Write(KinPaintHue);
            writer.Write(KinPaintExpiration);
            writer.Write((int)m_ShowMeleeDamage);
            writer.Write((int)m_ShowSpellDamage);
            writer.Write((int)m_ShowFollowerDamage);
            writer.Write(m_RecallRestrictionExpiration);
            writer.Write(m_LastLocation);
            writer.Write(m_PirateScore);
            writer.Write(m_CompanionLastLocation);
            writer.Write(m_Companion);
            writer.Write((int)m_NumGoldCoinsGenerated);
            writer.Write(CreatedOn);
            writer.Write((byte)SelectedTitleColorIndex);
            writer.Write((byte)SelectedTitleColorRarity);
            TitleColorState.Serialize(writer);
            writer.Write(m_UserOptHideFameTitles);
            writer.Write(LoginElapsedTime);
            Server.Custom.DonationState.Serialize(writer, this);
            writer.Write((DateTime)m_DateTimeDied);
            writer.Write((TimeSpan)m_TimeSpanDied);
            writer.Write((TimeSpan)m_TimeSpanResurrected);
            writer.Write((DateTime)m_AnkhNextUse);
            writer.Write(m_AutoStabled, true);
            writer.Write((Serial)m_LastTarget);
            writer.Write((DateTime)m_LastDeathByPlayer);
            writer.Write(m_LastOnline);
            writer.Write((bool)m_NoNewTimer); // IPY
            m_BOBFilter.Serialize(writer);
            writer.Write((int)m_NpcGuild);
            writer.Write((DateTime)m_NpcGuildJoinTime);
            writer.Write((TimeSpan)m_NpcGuildGameTime);
            writer.Write(m_PermaFlags, true);
            writer.Write(NextTailorBulkOrder);
            writer.Write(NextSmithBulkOrder);
            writer.Write((int)m_Flags);
            writer.Write(m_LongTermElapse);
            writer.Write(m_ShortTermElapse);
            writer.Write(GameTime);
            
            writer.Write((int)m_HairModID);
            writer.Write((int)m_HairModHue);
            writer.Write((int)m_BeardModID);
            writer.Write((int)m_BeardModHue);  
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
                   
            //Version 0
            if (version >= 0)
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

                int previousNamesCount = reader.ReadInt();
                for (int i = 0; i < previousNamesCount; i++)
                {
                    PreviousNames.Add(reader.ReadString());
                }
                
                int rank = reader.ReadEncodedInt();
                int maxRank = Guilds.RankDefinition.Ranks.Length - 1;

                if (rank > maxRank)
                    rank = maxRank;

                m_GuildRank = Guilds.RankDefinition.Ranks[rank];

                m_EventCalendarAccount = (EventCalendarAccount)reader.ReadItem() as EventCalendarAccount;
                m_BonusSkillCap = reader.ReadInt();
                m_MHSPlayerEntry = (MHSPlayerEntry)reader.ReadItem() as MHSPlayerEntry;
                m_ShowHealing = (DamageDisplayMode)reader.ReadInt();
                m_WorldChatAccountEntry = (WorldChatAccountEntry)reader.ReadItem() as WorldChatAccountEntry;
                m_UOACZAccountEntry = (UOACZAccountEntry)reader.ReadItem() as UOACZAccountEntry;
                m_HideRestrictionExpiration = reader.ReadDateTime();
                m_HenchmenSpeechDisplayMode = (HenchmenSpeechDisplayMode)reader.ReadInt();
                m_StealthStepsDisplayMode = (StealthStepsDisplayMode)reader.ReadInt();
                m_ShowAdminFilterText = reader.ReadBool();
                m_PlayerEnhancementAccountEntry = (PlayerEnhancementAccountEntry)reader.ReadItem() as PlayerEnhancementAccountEntry;
                m_InfluenceAccountEntry = reader.ReadItem() as InfluenceAccountEntry;
                m_ShowFollowerDamageTaken = (DamageDisplayMode)reader.ReadInt();
                m_LastPlayerKilledBy = (PlayerMobile)reader.ReadMobile();
                m_LastInstrument = (BaseInstrument)reader.ReadItem();
                m_ShowDamageTaken = (DamageDisplayMode)reader.ReadInt();
                m_ShowProvocationDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowPoisonDamage = (DamageDisplayMode)reader.ReadInt();
                m_AutoStealth = reader.ReadBool();
                m_BoatOccupied = (BaseBoat)reader.ReadItem();
                KinPaintHue = reader.ReadInt();
                KinPaintExpiration = reader.ReadDateTime();
                m_ShowMeleeDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowSpellDamage = (DamageDisplayMode)reader.ReadInt();
                m_ShowFollowerDamage = (DamageDisplayMode)reader.ReadInt();
                m_RecallRestrictionExpiration = reader.ReadDateTime();
                m_LastLocation = reader.ReadPoint3D();
                m_PirateScore = reader.ReadInt();
                m_CompanionLastLocation = reader.ReadPoint3D();
                m_Companion = reader.ReadBool();
                m_NumGoldCoinsGenerated = reader.ReadInt();
                CreatedOn = reader.ReadDateTime();
                SelectedTitleColorIndex = (int)reader.ReadByte();
                SelectedTitleColorRarity = (EColorRarity)reader.ReadByte();
                TitleColorState = new PlayerTitleColors();
                TitleColorState.Deserialize(reader);
                m_UserOptHideFameTitles = reader.ReadBool();
                LoginElapsedTime = reader.ReadTimeSpan();
                DonationPlayerState = Server.Custom.DonationState.Deserialize(reader);
                m_DateTimeDied = reader.ReadDateTime();
                m_TimeSpanDied = reader.ReadTimeSpan();
                m_TimeSpanResurrected = reader.ReadTimeSpan();
                m_AnkhNextUse = reader.ReadDateTime();
                m_AutoStabled = reader.ReadStrongMobileList();
                m_LastTarget = (Serial)reader.ReadInt();
                m_LastDeathByPlayer = reader.ReadDateTime();
                m_LastOnline = reader.ReadDateTime();
                m_NoNewTimer = reader.ReadBool();
                m_BOBFilter = new Engines.BulkOrders.BOBFilter(reader);
                m_NpcGuild = (NpcGuild)reader.ReadInt();
                m_NpcGuildJoinTime = reader.ReadDateTime();
                m_NpcGuildGameTime = reader.ReadTimeSpan();
                m_PermaFlags = reader.ReadStrongMobileList();
                NextTailorBulkOrder = reader.ReadTimeSpan();
                NextSmithBulkOrder = reader.ReadTimeSpan();
                m_Flags = (PlayerFlag)reader.ReadInt();
                m_LongTermElapse = reader.ReadTimeSpan();
                m_ShortTermElapse = reader.ReadTimeSpan();
                m_GameTime = reader.ReadTimeSpan();

                m_HairModID = reader.ReadInt();
                m_HairModHue = reader.ReadInt();
                m_BeardModID = reader.ReadInt();
                m_BeardModHue = reader.ReadInt();
            }

            //----------------  

            //Safety Measures
            Squelched = false;
            Frozen = false;
            CantWalk = false;
            
            if (m_LastOnline == DateTime.MinValue && Account != null)
                m_LastOnline = ((Account)Account).LastLogin;
            
            if (AccessLevel > AccessLevel.Player)
                m_IgnoreMobiles = true;

            if (TitleColorState == null)
                TitleColorState = new PlayerTitleColors();

            List<Mobile> list = Stabled;
            
            for (int i = 0; i < list.Count; ++i)
            {
                BaseCreature bc = list[i] as BaseCreature;

                if (bc != null)
                {
                    bc.IsStabled = true;
                    bc.StabledBy = this;

                    bc.OwnerAbandonTime = DateTime.UtcNow + TimeSpan.FromDays(1000);
                }
            }

            CheckAtrophies(this);

            if (Hidden)	//Hiding is the only buff where it has an effect that's serialized.
                AddBuff(new BuffInfo(BuffIcon.HidingAndOrStealth, 1075655));

            //Reapply Kin Paint
            if (KinPaintHue != -1 && KinPaintExpiration > DateTime.UtcNow)            
                HueMod = KinPaintHue;            

            if (!(Region.Find(LogoutLocation, LogoutMap) is GuardedRegion))
            {
                // crash protection
                Hidden = true;
                Poison = null;
            }

            if (Region.Find(LogoutLocation, LogoutMap) is UOACZRegion)
                Hidden = false;

            if (LastPlayerCombatTime > DateTime.MinValue)
            {
                if (LastPlayerCombatTime + PlayerCombatExpirationDelay > DateTime.UtcNow)
                {
                    m_PlayerCombatTimer = new PlayerCombatTimer(this);
                    m_PlayerCombatTimer.Start();
                }
            }            
        }

        public static void CheckAtrophies(Mobile m)
        {           
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
                        SendMessage("You are no longer a murderer.");                    
                }
            }
        }

        public void ResetKillTime()
        {
            m_ShortTermElapse = this.GameTime + TimeSpan.FromHours(MurderCountDecayHours);
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

            BaseHouse.HandleDeletion(this);

            DisguiseTimers.RemoveTimer(this);
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
                bool show_guild = GuildClickMessage && Guild != null && (DisplayGuildTitle || (Player && Guild.Type != Guilds.GuildType.Regular));
                bool show_other_titles = true;

                if (from.Region is UOACZRegion)
                { 
                    show_guild = false;
                    show_other_titles = false;
                }

                int newhue;

                if (NameHue != -1)
                    newhue = NameHue;

                else if (AccessLevel > AccessLevel.Player)
                    newhue = 11;

                else
                    newhue = Notoriety.GetHue(Notoriety.Compute(from, this));

                if (show_guild) // GUILD NO FACTION
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
                
                if (show_other_titles)
                {
                    string fullname_line = "";

                    if ((ShowFameTitle && (Player || Body.IsHuman) && Fame >= 10000))
                        fullname_line = Female ? "Lady " : "Lord ";

                    fullname_line += Name == null ? String.Empty : Name;
                    fullname_line = ApplyNameSuffix(fullname_line); // (Young) for example
                    
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
            ReleaseAllFollowers();
                        
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

        public virtual bool UsesFastwalkPrevention { get { return (AccessLevel < AccessLevel.Counselor); } }

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

                    pet.OwnerAbandonTime = DateTime.UtcNow + TimeSpan.FromDays(1000);

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
