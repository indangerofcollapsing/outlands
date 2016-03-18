using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Items;
using Server.Custom.Misc;

namespace Server.Custom
{   
    public class LoHSystem
    {  
        public static TimeSpan TimeBetweenEvents = TimeSpan.FromHours(6);
        public static int EventNotificationLeadtime = 5; //Minutes

        public static string EventNotificationMessage = "The League of Heroes will announce an important message through the Town Criers in {0} minute(s).";
        public static string EventCompletionMessage = "Based on \"{0}\" {1} has been awarded with a very special prize - Congratulations";
        public static int MoongateHue = 2204;
        public static int MonsterSpawnDelay = 120; //Seconds after the gates have been spawned
        public static int DecayTime = 30; // Minutes
        public static int ServerResetEventDelay = 120; //Minutes

        public static TimeSpan MoongateDuration = TimeSpan.FromMinutes(10);

        public static LoHSystem Instance;

        public static List<WinnerSelector> m_WinnerSelectors;
        public static List<LoHEvent> m_LoHEvents;

        public int m_MinutesUntilGo;
        public TownCrierEntry m_MonsterAnnounceTCEntry;
        public TownCrierEntry m_AboutToStartTCEntry;
        public Timer m_CountdownTimer;
        public Timer m_NextEventStartTimer;
        public Timer m_DespawnCheckTimer;
        public DateTime? m_BossSpawnTime;

        int m_SelectedWinConditionIdx;
        int m_SelectedChallengeIdx;

        public LoHSystem()
        {
            m_NextEventStartTimer = Timer.DelayCall(TimeSpan.FromMinutes(ServerResetEventDelay), TimeSpan.FromMinutes(ServerResetEventDelay), 0, new TimerStateCallback<LoHSystem>(OnTimeToNextChallengeTick), this);
        }

        static LoHSystem()
        {
            Instance = new LoHSystem();

            m_LoHEvents = new List<LoHEvent>()
			{
                new LoHSandWyvernEvent(),
                new LoHBoneDragonEvent(),
                new LoHEarthSerpentEvent(),
                new LoHScarabBeetleEvent(),
                new LoHEnvelopingDarknessEvent(),
                new LoHChaosBeastEvent(),
                new LoHFeasterEvent(),
                new LoHMantidReaverEvent(),
                new LoHRagWitchEvent(),
                new LoHWolvenHunterEvent(),
                new LoHCrystalElementalEvent(),
                new LoHMurgleKingEvent(),
                new LoHAnimatedAltarEvent(),
                new LoHMarrowfiendEvent(),
                new LoHPlaguebearerEvent(),
			};

            m_WinnerSelectors = new List<WinnerSelector>() 
            {
                new LoHWinnerSelectors_HighestDamage(),
                new LoHWinnerSelectors_Killshot(),
                new LoHWinnerSelectors_RandomDamager()
            };            
        }

        public static void Initialize()
        {
            CommandSystem.Register("StartLoH", AccessLevel.GameMaster, x => { LoHSystem.Instance.ClearAllState(); LoHSystem.Instance.OnCountdownDone(); });
            CommandSystem.Register("LoHTimer", AccessLevel.Player, new CommandEventHandler(ShowLoHCountdownTimer));
        }

        public static void ShowLoHCountdownTimer(CommandEventArgs e)
        {
            if (LoHSystem.Instance != null && e.Mobile != null)
            {
                Mobile from = e.Mobile;

                if (LoHSystem.Instance.m_BossSpawnTime.HasValue)
                {
                    TimeSpan timePassed = DateTime.UtcNow - LoHSystem.Instance.m_BossSpawnTime.Value;
                    from.SendMessage("The League of Heroes has already opened the portal for {0} hours and {1} minutes", timePassed.Hours, timePassed.Minutes);
                }

                else
                {
                    if (LoHSystem.Instance.m_NextEventStartTimer.Running)
                    {
                        TimeSpan timeLeft = LoHSystem.Instance.m_NextEventStartTimer.Next - DateTime.UtcNow;
                        from.SendMessage("The League of Heroes will announce an important message through the Town Criers in {0} hours and {1} minutes", timeLeft.Hours, timeLeft.Minutes);
                    }

                    else                    
                        from.SendMessage("The League of Heroes will announce an important message through the Town Criers shortly");                    
                }
            }
        }

        public static void DoCountdownNotification(object LoHEventInstance)
        {
            LoHSystem self = (LoHSystem)LoHEventInstance;

            if (self.m_MinutesUntilGo == 0)            
                self.OnCountdownDone();
            
            else
            {
                string tickmessage = String.Format(EventNotificationMessage, self.m_MinutesUntilGo);

                if (self.m_MinutesUntilGo % 2 != 0)
                    CommandHandlers.BroadcastMessage(AccessLevel.Player, 0x4ea, String.Format(EventNotificationMessage, self.m_MinutesUntilGo));                

                self.m_AboutToStartTCEntry.Lines[0] = tickmessage;

                --self.m_MinutesUntilGo;
            }
        }

        public void StartCountdown()
        {
            ClearAllState();
           
            m_AboutToStartTCEntry = GlobalTownCrierEntryList.Instance.AddEntry(new string[] { "I will soon announce an important message from the League of Heroes!" }, TimeSpan.FromMinutes(EventNotificationLeadtime));
            m_CountdownTimer = Timer.DelayCall(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), 6, new TimerStateCallback<LoHSystem>(DoCountdownNotification), this);
        }

        void ClearAllState()
        {
            m_AboutToStartTCEntry = null;
            m_MinutesUntilGo = EventNotificationLeadtime;

            if (m_CountdownTimer != null)
                m_CountdownTimer.Stop();

            if (m_NextEventStartTimer != null)
                m_NextEventStartTimer.Stop();

            if (m_DespawnCheckTimer != null)
                m_DespawnCheckTimer.Stop();

            if (m_SelectedChallengeIdx >= 0)
            {
                m_LoHEvents[m_SelectedChallengeIdx].Despawn();
                StopAnnounceMonster();
            }

            m_SelectedChallengeIdx = -1;
            m_BossSpawnTime = null;
        }

        static void OnTimeToNextChallengeTick(object LoHEventInstance)
        {
            LoHSystem self = (LoHSystem)LoHEventInstance;

            self.StartCountdown();
        }

        void StartNextEventTimer()
        {
            m_NextEventStartTimer = Timer.DelayCall(TimeBetweenEvents, TimeBetweenEvents, 0, new TimerStateCallback<LoHSystem>(OnTimeToNextChallengeTick), this);
        }

        void Restart(bool spawnExitGates = true)
        {
            if (spawnExitGates)
            {
                int selected = m_SelectedChallengeIdx;

                Timer.DelayCall(TimeSpan.FromMinutes(1.5), () =>
                {
                    HomeTownPortal portal = new HomeTownPortal();

                    portal.MoveToWorld(m_LoHEvents[selected].PortalLocation, Map.Felucca);

                    Moongate moongate = new Moongate() { Dispellable = false };

                    moongate.MoveToWorld(m_LoHEvents[selected].PortalLocation, Map.Felucca);

                    Timer.DelayCall(TimeSpan.FromMinutes(1), () =>
                    {
                        if (portal != null && !portal.Deleted)
                            portal.Delete();

                        if (moongate != null && !moongate.Deleted)
                            moongate.Delete();
                    });
                });
            }

            ClearAllState();
            StartNextEventTimer();
        }

        void SelectMonster()
        {
            m_SelectedChallengeIdx = Utility.Random(m_LoHEvents.Count);           
        }

        void SelectWinCondition()
        {
            m_SelectedWinConditionIdx = Utility.Random(m_WinnerSelectors.Count);
        }

        void StartAnnounceMonster()
        {
            GlobalTownCrierEntryList.Instance.RemoveEntry(m_AboutToStartTCEntry);

            LoHEvent LoHEventInstance = m_LoHEvents[m_SelectedChallengeIdx];
            string[] eventAnnouncementText = new string[] { LoHEventInstance.AnnouncementText + " Join your fellow hunters now, for glory and treasure!"};

            m_MonsterAnnounceTCEntry = GlobalTownCrierEntryList.Instance.AddEntry(eventAnnouncementText, TimeSpan.FromDays(1));
        }

        void StopAnnounceMonster()
        {
            GlobalTownCrierEntryList.Instance.RemoveEntry(m_MonsterAnnounceTCEntry);
        }

        void SpawnMonster()
        {
            m_LoHEvents[m_SelectedChallengeIdx].Spawn(() => { LoHSystem.Instance.OnMonsterDeath(); });

            m_DespawnCheckTimer = Timer.DelayCall(TimeSpan.FromMinutes(DecayTime), TimeSpan.FromMinutes(DecayTime), 1, new TimerStateCallback<LoHSystem>(OnDespawnTimerUp), this);
        }

        static void OnDespawnTimerUp(object LoHEventInstance)
        {
            LoHSystem self = (LoHSystem)LoHEventInstance;

            self.Restart(false);
        }

        void SpawnPortals()
        {
            foreach (TownCrier tc in TownCrier.Instances)
            {
                LoHMoongate mg = new LoHMoongate(m_LoHEvents[m_SelectedChallengeIdx].PortalLocation, Map.Felucca);

                mg.Hue = MoongateHue;
                mg.MoveToWorld(tc.Location, Map.Felucca);
            }
        }

        Mobile SelectWinner(BaseCreature monster_killed)
        {
            if (monster_killed.DamageEntries.Count == 0)
                return null;

            return m_WinnerSelectors[m_SelectedWinConditionIdx].SelectWinner(monster_killed);
        }

        bool CheckWinCondition(Mobile roundwinner)
        {
            return false;
        }

        void AwardPrize(PlayerMobile winner)
        {
            if (winner.Backpack != null)
            {
                string displayName = m_LoHEvents[m_SelectedChallengeIdx].DisplayName;
                int rewardHue = m_LoHEvents[m_SelectedChallengeIdx].RewardHue;

                Item reward = new RushChallengeGoodiebag(winner, displayName, rewardHue);

                winner.Backpack.AddItem(reward);
            }
        }

        void AnnounceWinner(PlayerMobile winner)
        {
            string s = String.Format(EventCompletionMessage, m_WinnerSelectors[m_SelectedWinConditionIdx].GetShortDescription(), winner.Name);

            Region r = Region.Find(m_LoHEvents[m_SelectedChallengeIdx].MonsterLocation, Map.Felucca);

            r.ForEachPlayerInRegion((Mobile mob) => { mob.SendMessage(0x4ea, s); });
        }

        void OnCountdownDone()
        {           
            SelectMonster();
            SelectWinCondition();
            StartAnnounceMonster();
            SpawnPortals();

            Timer.DelayCall(TimeSpan.FromSeconds(MonsterSpawnDelay), TimeSpan.FromSeconds(MonsterSpawnDelay), 1, new TimerStateCallback<LoHSystem>(OnDelayedSpawnTimerUp), this);

            m_BossSpawnTime = DateTime.UtcNow;
        }

        static void OnDelayedSpawnTimerUp(object LoHEventInstance)
        {
            LoHSystem self = (LoHSystem)LoHEventInstance;
            self.SpawnMonster();
        }

        void OnMonsterDeath()
        {
            m_LoHEvents[m_SelectedChallengeIdx].GenerateLoot();

            PlayerMobile winner = SelectWinner(m_LoHEvents[m_SelectedChallengeIdx].m_LoHCreature) as PlayerMobile;

            if (winner != null)
            {
                AnnounceWinner(winner);
                AwardPrize(winner);
            }

            Timer.DelayCall(TimeSpan.FromSeconds(2), () => { Restart(); });
        }
    }

    public class LoHMoongate : Moongate
    {
        public LoHMoongate(Point3D destLoc, Map destMap): base(destLoc, destMap)
        {
            Dispellable = false;

            Timer.DelayCall(LoHSystem.MoongateDuration, new TimerCallback(Delete));
        }

        public LoHMoongate(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete();
        }
    }

    public class RushChallengeGoodiebag : MetalBox
    {
        [Constructable]
        public RushChallengeGoodiebag(PlayerMobile winner, string monster_name, int monster_hue)
        {
            Hue = 2407;
            Weight = 1.0;
            Name = String.Format("a reward for slaying {0}", monster_name);

            int seed = Utility.Random(100);

            if (40 > seed)
            {	// 40%
                DropItem(TitleDye.RandomCommonTitleDye());
            }

            else if (60 > seed)
            {	// 20%
                AddItem(new RunebookDyeTub() { UsesRemaining = 5, LootType = LootType.Regular });
            }

            else if (70 > seed)
            {	// 10% lvl 4,5 or 6 TMap
                DropItem(new TreasureMap(Utility.Random(4, 3), Map.Felucca));
            }

            else if (85 > seed)
            {	// 15%
                DropItem(TitleDye.RandomUncommonTitleDye());
            }

            else if (97 > seed)
            {	// 12% glove chance.
                DropItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gloves));
                DropItem(new ArcaneDust());
            }

            else if (99 > seed)
            {	// 2%
                WispLantern i = new WispLantern();

                i.Hue = monster_hue;
                i.SetWispHue(monster_hue);
                i.SetWispName(String.Format("Essence of {0}", monster_name));

                DropItem(i);
            }

            else
            {	// 1%
                if (Utility.RandomBool())                
                    DropItem(PowerScroll.CreateRandom(5, 20));                

                else
                {
                    var i = new SavageMask();
                    i.Name = String.Format("{0}'s Visage", monster_name);
                    i.LootType = LootType.Blessed;
                    DropItem(i);
                }
            }

            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(Loot.RandomGem());
            DropItem(SkillScroll.Generate(winner, 120.0, 0));
            DropItem(SkillScroll.Generate(winner, 120.0, 1));
            DropItem(SkillScroll.Generate(winner, 120.0, 2));
            DropItem(new RareCloth());

            // random high level usable weapon OR weapon enhancement.
            if (Utility.RandomBool())
            {
                var weapon = Loot.RandomWeapon();

                if (weapon != null)
                {
                    weapon.DamageLevel = (WeaponDamageLevel)(3 + Utility.Random(3));
                    weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);

                    if (Utility.Random(50) == 0)
                        weapon.Slayer = SlayerName.Silver;

                    DropItem(weapon);
                }
            }

            else            
                DropItem(new Server.Custom.Ubercrafting.WeaponDamageEnhancer());            
        }

        public RushChallengeGoodiebag(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
