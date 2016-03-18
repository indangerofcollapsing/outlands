using System;
using System.Collections.Generic;
using Server.Regions;
using Server.Mobiles;
using Server.Engines.PartySystem;
using Server.Guilds;
using Server.Achievements;
using Server.Items;

namespace Server.Custom
{
    public enum FireDungeonBosses
    {
        BaronVonGeddon,
        OgrePatriarch,
        RubyDragon,
        ImmortalFlame,
        Finished
    }

    public static class FireDungeon
    {
        public static readonly TimeSpan BossTimeLimit = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan PostDungeonBootTimeLimit = TimeSpan.FromMinutes(10);
        public static readonly TimeSpan PerBossCoolDown = TimeSpan.FromDays(1);

        public const int MinimumGroupSize = 5;
        public const int EntranceRange = 50;
        public const bool AllowPets = false;

        public static FireDungeonInstance InstanceOne { get; set; }
        public static FireDungeonInstance InstanceTwo { get; set; }

        public static bool Active { get { return InstanceOne != null || InstanceTwo != null; } }

        public static bool Full
        {
            get
            {
                return InstanceOne != null && InstanceTwo != null;
            }
        }

        private static FireDungeonRegion m_RegionOne;
        public static FireDungeonRegion RegionOne { get { return m_RegionOne; } set { if (value != null) m_RegionOne = value; } }

        private static FireDungeonRegion m_RegionTwo;
        public static FireDungeonRegion RegionTwo { get { return m_RegionTwo; } set { if (value != null) m_RegionTwo = value; } }

        public static void RespawnTankDragon(Map map)
        {
            if (Active)
            {
                TimerCallback spawnDragon;

                if (map == Map.Felucca)
                    spawnDragon = InstanceOne.SpawnTankDragon;
                else
                    spawnDragon = InstanceTwo.SpawnTankDragon;

                Timer.DelayCall(TimeSpan.FromMinutes(2.0), spawnDragon);
            }
        }

        public static void ExitTeleporter_OnMoveOver(Mobile m)
        {
            m.SendGump(new FireDungeonConfirmExitGump(m));
        }

        public static void Teleporter_OnMoveOver(Mobile m)
        {
            if (Active)
            {
                var pm = m as PlayerMobile;
                if (InstanceOne != null && InstanceOne.OriginalParty.Contains(pm))
                    InstanceOne.LoadPlayer(m);
                else if (InstanceTwo != null && InstanceTwo.OriginalParty.Contains(pm))
                    InstanceTwo.LoadPlayer(m);
                else if (Full)
                    m.SendMessage("Fire Dungeon is currently occupied. Please try again later.");
                else
                    m.SendGump(new FireDungeonConfirmEntranceGump(m));
            }
            else
            {
                m.SendGump(new FireDungeonConfirmEntranceGump(m));
            }
        }

        public static void CheckEntry(Mobile m)
        {
            if (Active)
            {
                var pm = m as PlayerMobile;
                if (InstanceOne != null && InstanceOne.OriginalParty.Contains(pm))
                    InstanceOne.LoadPlayer(m);
                else if (InstanceTwo != null && InstanceTwo.OriginalParty.Contains(pm))
                    InstanceTwo.LoadPlayer(m);
                else if (Full)
                {
                    m.SendMessage("Fire Dungeon is currently occupied. Please try again later.");
                    return;
                }
            }

            var party = Party.Get(m);
            List<PlayerMobile> members;

            Predicate<PlayerMobile> canEnterCheck = (PlayerMobile pm) => { return pm.NextFireAttempt < DateTime.Now; };

            if (party == null || party.Count < MinimumGroupSize)
            {
                m.SendMessage("You must be a in party of at least five members prior to entering Fire Dungeon.");
            }
            else if (!CheckPartyRange(party, m.Location, out members))
            {
                m.SendMessage("Not enough of your party is nearby.");
            }
            else if (!members.TrueForAll(canEnterCheck))
            {
                m.SendMessage("One or more of your party members cannot attempt Fire Dungeon again so soon.");
            }
            else
            {
                if (InstanceOne == null)
                {
                    InstanceOne = new FireDungeonInstance(Map.Felucca);
                    InstanceOne.LoadPlayers(members);
                    InstanceOne.Spawn();
                    m.SendMessage("All members of your party within range have entered Fire Dungeon.");
                }
                else if (InstanceTwo == null)
                {
                    InstanceTwo = new FireDungeonInstance(Map.Trammel);
                    InstanceTwo.LoadPlayers(members);
                    InstanceTwo.Spawn();
                    m.SendMessage("All members of your party within range have entered Fire Dungeon.");
                }
                else
                {
                    m.SendMessage("Fire Dungeon is currently occupied. Please try again later.");
                }
            }
        }

        public static void OnExitRequest(Mobile m)
        {
            var pm = m as PlayerMobile;
            if (pm == null)
                return;

            var instance = pm.Map == Map.Felucca ? InstanceOne : InstanceTwo;
            RemovePlayer(m, instance);
        }

        public static void RemovePlayer(Mobile m)
        {
            var kickoutLocation = new Point3D(2923, 3410, 5);
            if (m.Map == Map.Internal)
            {
                if (AllowPets) BaseCreature.TeleportPets(m, kickoutLocation, Map.Felucca);
                m.LogoutLocation = kickoutLocation;
                m.LogoutMap = Map.Felucca;
            }
            else
            {
                Timer.DelayCall(TimeSpan.FromTicks(1), delegate
                {
                    if (AllowPets) BaseCreature.TeleportPets(m, kickoutLocation, Map.Felucca);
                    m.MoveToWorld(kickoutLocation, Map.Felucca);
                });
            }
        }

        public static void RemovePlayer(Mobile m, FireDungeonInstance instance)
        {
            var pm = m as PlayerMobile;
            if (instance != null && instance.Players != null && instance.Players.Contains(pm))
                instance.Players.Remove(pm);

            if (instance != null && (instance.Players == null || instance.Players.Count == 0))
                instance.Destroy();

            RemovePlayer(m);
        }

        public static bool CheckPartyRange(Party p, Point3D loc, out List<PlayerMobile> members)
        {
            members = new List<PlayerMobile>();
            foreach (PartyMemberInfo info in p.Members)
            {
                if (info.Mobile.InRange(loc, EntranceRange))
                {
                    if (info.Mobile is PlayerMobile)
                        members.Add(info.Mobile as PlayerMobile);
                }

            }

            return members.Count >= MinimumGroupSize;
        }
    }

    public class FireDungeonInstance
    {
        private enum TrashType
        {
            LavaSurger,
            CoreHound,
            CharredProtector
        }

        private InternalTimer m_Timer;
        public List<PlayerMobile> Players { get; set; }
        public List<PlayerMobile> OriginalParty { get; set; }

        public BaronVonGeddon BaronVonGeddon { get; set; }
        public ImmortalFlameBoss ImmortalFlameBoss { get; set; }
        public SuperOgreLord Clompur { get; set; }
        public SuperOgreLord Stompur { get; set; }
        public RubyDragon RubyDragon { get; set; }
        public DragonHandler RubyDragonHandler { get; set; }
        public TankDragon TankDragon { get; set; }

        public List<CoreHound> CoreHounds { get; set; }
        public List<CharredProtector> CharredProtectors { get; set; }
        public List<LavaSurger> LavaSurgers { get; set; }

        public FireRopeController InstanceFireRope;

        public FireDungeonBosses CurrentState { get; set; }
        public double DifficultyFactor = 1.0;
        private double PerPlayerDifficultyIncrease = 0.02;

        private DateTime m_NextState;
        public DateTime NextState { get { return m_NextState; } }

        public Map m_Map;
        public FireDungeonExitTeleporter ExitTeleport { get; set; }
        public Moongate ExitMoongate { get; set; }
        public DungeonPersistance Persistance { get; set; }

        private TimeLeftTimer m_TimeLeftTimer;

        public FireDungeonInstance(Map map, DungeonPersistance persistance = null)
        {
            CoreHounds = new List<CoreHound>();
            LavaSurgers = new List<LavaSurger>();
            CharredProtectors = new List<CharredProtector>();

            m_Map = map;
            if (persistance != null)
                Persistance = persistance;
            else
                Persistance = new DungeonPersistance(this);
        }

        public void Resume()
        {
            if (Players == null || Players.Count < FireDungeon.MinimumGroupSize)
            {
                Destroy();
            }
            else
            {
                if (TankDragon == null)
                    SpawnTankDragon();

                if (CurrentState == FireDungeonBosses.Finished)
                    m_NextState = DateTime.UtcNow + FireDungeon.PostDungeonBootTimeLimit;
                else
                    m_NextState = DateTime.UtcNow + FireDungeon.BossTimeLimit;

                m_Timer = new InternalTimer(this);
                m_Timer.Start();

                m_TimeLeftTimer = new TimeLeftTimer(this);
                m_TimeLeftTimer.Start();
            }
        }

        public void LoadPlayer(Mobile player, Point3D? position = null)
        {
            Players.Add(player as PlayerMobile);
            Point3D location = position ?? new Point3D(5683, 1421, 40);
            if (FireDungeon.AllowPets) BaseCreature.TeleportPets(player, location, m_Map);
            player.MoveToWorld(location, m_Map);
            AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_ExploreFireDungeon);
        }

        public void LoadPlayers(List<PlayerMobile> members)
        {
            OriginalParty = members;
            Players = new List<PlayerMobile>();

            DifficultyFactor += (members.Count - 5) * PerPlayerDifficultyIncrease;

            Point3D start = new Point3D(5683, 1421, 40);
            Point3D curr;
            int x = 0;
            foreach (Mobile m in members)
            {
                curr = new Point3D(start.X + x++, start.Y, start.Z);
                LoadPlayer(m, curr);
            }

            SendMessageToAllPlayers(String.Format("You have {0} minutes to slay Baron Von Geddon!", FireDungeon.BossTimeLimit.Minutes));
        }

        public void Spawn()
        {
            if (BaronVonGeddon != null) BaronVonGeddon.Delete();

            BaronVonGeddon = new BaronVonGeddon(DifficultyFactor);
            BaronVonGeddon.MoveToWorld(new Point3D(5727, 1465, 0), m_Map);
            BaronVonGeddon.Home = new Point3D(5727, 1465, 0);
            BaronVonGeddon.RangeHome = 6;

            SpawnTankDragon();

            SpawnTrash(new Point3D(5739, 1435, 11));

            if (InstanceFireRope != null) InstanceFireRope.Delete();

            InstanceFireRope = FireRope.GenerateItem(m_Map);

            m_NextState = DateTime.UtcNow + FireDungeon.BossTimeLimit;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            m_TimeLeftTimer = new TimeLeftTimer(this);
            m_TimeLeftTimer.Start();
        }

        public void SpawnTrash(Point3D location)
        {
            Array types = Enum.GetValues(typeof(TrashType));
            var type = (TrashType)types.GetValue(Utility.Random(types.Length));

            switch (type)
            {
                case TrashType.CharredProtector:
                    SpawnCharredProtectors(Utility.RandomMinMax(1, 3), location);
                    break;
                case TrashType.CoreHound:
                    SpawnCoreHounds(Utility.RandomMinMax(1, 3), location);
                    break;
                case TrashType.LavaSurger:
                    SpawnLavaSurgers(Utility.RandomMinMax(1, 3), location);
                    break;

            }
        }

        public void SpawnCharredProtectors(int amount, Point3D location)
        {
            for (int i = 0; i < amount; i++)
            {
                var mob = new CharredProtector();
                CharredProtectors.Add(mob);
                mob.MoveToWorld(location, m_Map);
            }
        }

        public void SpawnCoreHounds(int amount, Point3D location)
        {
            for (int i = 0; i < amount; i++)
            {
                var mob = new CoreHound();
                CoreHounds.Add(mob);
                mob.MoveToWorld(location, m_Map);
            }
        }

        public void SpawnLavaSurgers(int amount, Point3D location)
        {
            for (int i = 0; i < amount; i++)
            {
                var mob = new LavaSurger();
                LavaSurgers.Add(mob);
                mob.MoveToWorld(location, m_Map);
            }
        }

        public void SpawnImmortalFlame()
        {
            if (ImmortalFlameBoss != null) ImmortalFlameBoss.Delete();

            ImmortalFlameBoss = new ImmortalFlameBoss(DifficultyFactor, new Point3D(5654, 1422, 44), new Point3D(5640, 1422, 44), new Point3D(5640, 1400, 44), new Point3D(5654, 1400, 44));
            ImmortalFlameBoss.MoveToWorld(new Point3D(5647, 1409, 44), m_Map);

            SpawnTrash(new Point3D(5654, 1390, -1));
        }

        public void SpawnRubyDragon()
        {
            if (RubyDragon != null) RubyDragon.Delete();

            RubyDragon = new RubyDragon(DifficultyFactor);
            RubyDragon.MoveToWorld(new Point3D(5680, 1308, -1), m_Map);
            RubyDragon.Home = new Point3D(5680, 1308, -1);
            RubyDragon.RangeHome = 6;

            if (RubyDragonHandler != null) RubyDragonHandler.Delete();

            RubyDragonHandler = new DragonHandler(DifficultyFactor);
            RubyDragonHandler.MoveToWorld(new Point3D(5680, 1308, -1), m_Map);
            RubyDragonHandler.Home = new Point3D(5680, 1308, -1);
            RubyDragonHandler.RangeHome = 6;
            RubyDragonHandler.Dragon = RubyDragon;

            SpawnTrash(new Point3D(5787, 1348, 0));
        }

        public void SpawnOgreLords()
        {
            if (Clompur != null) Clompur.Delete();

            Clompur = new SuperOgreLord(DifficultyFactor);
            Clompur.MoveToWorld(new Point3D(5864, 1469, -1), m_Map);
            Clompur.Home = new Point3D(5864, 1469, -1);
            Clompur.RangeHome = 6;
            Clompur.Name = "Clomp'ur";

            if (Stompur != null) Stompur.Delete();

            Stompur = new SuperOgreLord(DifficultyFactor);
            Stompur.MoveToWorld(new Point3D(5864, 1469, -1), m_Map);
            Stompur.Home = new Point3D(5864, 1469, -1);
            Stompur.RangeHome = 6;
            Stompur.Name = "Stomp'ur";

            SpawnTrash(new Point3D(5796, 1491, 34));
        }

        public void SpawnTankDragon()
        {
            if (TankDragon != null)
                TankDragon.Delete();

            TankDragon = new TankDragon(DifficultyFactor);
            TankDragon.MoveToWorld(new Point3D(5697, 1433, 0), m_Map);
            TankDragon.Home = new Point3D(5697, 1433, 0);
            TankDragon.RangeHome = 6;
        }

        public void BroadcastTimeLeft()
        {
            TimeSpan timeLeft = m_NextState - DateTime.UtcNow;
            SendMessageToAllPlayers(String.Format("You have {0} minutes before failing Fire Dungeon.", timeLeft.Minutes));
        }

        public void Destroy()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            if (m_TimeLeftTimer != null)
                m_TimeLeftTimer.Stop();

            if (BaronVonGeddon != null && !BaronVonGeddon.Deleted)
                BaronVonGeddon.Delete();

            if (TankDragon != null && !TankDragon.Deleted)
                TankDragon.Delete();

            if (ImmortalFlameBoss != null && !ImmortalFlameBoss.Deleted)
                ImmortalFlameBoss.Delete();

            if (Clompur != null && !Clompur.Deleted)
                Clompur.Delete();

            if (Stompur != null && !Stompur.Deleted)
                Stompur.Delete();

            if (RubyDragon != null && !RubyDragon.Deleted)
                RubyDragon.Delete();

            if (RubyDragonHandler != null && !RubyDragonHandler.Deleted)
                RubyDragonHandler.Delete();

            if (InstanceFireRope != null && !InstanceFireRope.Deleted)
                InstanceFireRope.Delete();

            if (ExitTeleport != null && !ExitTeleport.Deleted)
                ExitTeleport.Delete();

            if (ExitMoongate != null && !ExitMoongate.Deleted)
                ExitMoongate.Delete();

            if (Persistance != null && !Persistance.Deleted)
                Persistance.Delete();

            foreach (var mob in CoreHounds)
            {
                if (mob != null && !mob.Deleted)
                    mob.Delete();
            }
            CoreHounds.Clear();

            foreach (var mob in LavaSurgers)
            {
                if (mob != null && !mob.Deleted)
                    mob.Delete();
            }
            LavaSurgers.Clear();

            foreach (var mob in CharredProtectors)
            {
                if (mob != null && !mob.Deleted)
                    mob.Delete();
            }
            CharredProtectors.Clear();

            EjectAllPlayers();

            OriginalParty.Clear();
            Players.Clear();

            if (m_Map == Map.Felucca)
                FireDungeon.InstanceOne = null;
            else
                FireDungeon.InstanceTwo = null;
        }

        public void SendMessageToAllPlayers(string msg)
        {
            if (Players != null)
            {
                foreach (Mobile m in Players)
                {
                    m.SendMessage(msg);
                }

            }
        }

        public void SendSparkles()
        {
            if (Players != null)
            {
                foreach (Mobile mob in Players)
                {
                    mob.PlaySound(0x1E7);
                    mob.FixedParticles(0x373A, 10, 15, 5018, EffectLayer.Waist);
                }
            }
        }

        public void EjectAllPlayers()
        {
            if (Players == null)
                return;

            Point3D start = new Point3D(2923, 3410, 5);
            Point3D curr;
            int y = 0;
            foreach (Mobile m in Players)
            {
                curr = new Point3D(start.X, start.Y + y++, 5);
                if (m.Map == Map.Internal)
                {
                    m.LogoutMap = Map.Felucca;
                    m.LogoutLocation = curr;
                }
                else
                {
                    if (!(m.Region is FireDungeonRegion)) continue;

                    if (FireDungeon.AllowPets) BaseCreature.TeleportPets(m, curr, Map.Felucca);
                    m.MoveToWorld(curr, Map.Felucca);
                }
            }

        }

        public void UpdateNextFireAttempt(bool firstboss = false)
        {
            foreach (var player in OriginalParty)
            {
                if (firstboss)
                    player.NextFireAttempt = DateTime.Now.Date + TimeSpan.FromDays(2);
                else
                    player.NextFireAttempt += FireDungeon.PerBossCoolDown;
            }
        }

        private void TickExperience() 
        {
            var guilds = new HashSet<Guild>();
            foreach (var player in Players)
            {
                if (player.Guild != null && player.Guild is Guild)
                {
                    var playerGuild = player.Guild as Guild;
                    if (!guilds.Contains(playerGuild))
                        guilds.Add(playerGuild);
                }
            }
            foreach (var guild in guilds)
                guild.TickExperience();
        }

        public void Slice()
        {
            DateTime now = DateTime.UtcNow;

            switch (CurrentState)
            {
                case FireDungeonBosses.BaronVonGeddon:
                    {
                        if (BaronVonGeddon == null || BaronVonGeddon.Deleted)
                        {
                            SendSparkles();
                            SendMessageToAllPlayers("After the Ogre Patriarch!");
                            CurrentState = FireDungeonBosses.OgrePatriarch;
                            m_NextState = now + FireDungeon.BossTimeLimit;
                            UpdateNextFireAttempt(true);
                            SpawnOgreLords();
                            TickExperience();
                            foreach(var player in Players)
                            {
                                AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_BaronVonGeddon);
                            }
                        }

                        break;
                    }
                case FireDungeonBosses.OgrePatriarch:
                    {
                        if ((Clompur == null || Clompur.Deleted) && (Stompur == null || Stompur.Deleted))
                        {
                            SendSparkles();
                            SendMessageToAllPlayers("Slay the Ruby Dragon!");
                            CurrentState = FireDungeonBosses.RubyDragon;
                            m_NextState = now + FireDungeon.BossTimeLimit;
                            UpdateNextFireAttempt();
                            SpawnRubyDragon();
                            TickExperience();
                            foreach(var player in Players)
                            {
                                AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_Ogres);
                            }
                        }

                        break;
                    }
                case FireDungeonBosses.RubyDragon:
                    {
                        if ((RubyDragon == null || RubyDragon.Deleted) && (RubyDragonHandler == null || RubyDragonHandler.Deleted))
                        {
                            SendSparkles();
                            SendMessageToAllPlayers("You must douse the Immortal Flame before it is too late!");
                            CurrentState = FireDungeonBosses.ImmortalFlame;
                            m_NextState = now + FireDungeon.BossTimeLimit;
                            UpdateNextFireAttempt();
                            SpawnImmortalFlame();
                            TickExperience();
                            foreach(var player in Players)
                            {
                                AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_RubyDragon);
                            }
                        }

                        break;
                    }
                case FireDungeonBosses.ImmortalFlame:
                    {
                        if (ImmortalFlameBoss == null || ImmortalFlameBoss.Deleted)
                        {
                            SendSparkles();
                            SendMessageToAllPlayers("You have completed Fire Dungeon. The dungeon will reset in ten minutes.");
                            CurrentState = FireDungeonBosses.Finished;
                            m_NextState = now + FireDungeon.PostDungeonBootTimeLimit;
                            UpdateNextFireAttempt();
                            SpawnExitGate();
                            TickExperience();
                            foreach(var player in Players)
                            {
                                AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_ImmortalFlame);
                                AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_FireDungeon);
                            }
                        }

                        break;
                    }
            }

            if (now > m_NextState)
            {
                Destroy();
                return;
            }
        }

        private void SpawnExitGate()
        {
            var point = new Point3D(5647, 1400, 44);
            ExitTeleport = new FireDungeonExitTeleporter();
            ExitMoongate = new Moongate() { Hue = 2101, Name = "Exit" };
            ExitMoongate.MoveToWorld(point, m_Map);
            ExitTeleport.MoveToWorld(point, m_Map);
        }

        public class InternalTimer : Timer
        {
            private FireDungeonInstance m_Instance;

            public InternalTimer(FireDungeonInstance instance)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(5))
            {
                m_Instance = instance;
            }

            protected override void OnTick()
            {
                m_Instance.Slice();
            }
        }

        public class TimeLeftTimer : Timer
        {
            private FireDungeonInstance m_Instance;

            public TimeLeftTimer(FireDungeonInstance instance)
                : base(TimeSpan.Zero, TimeSpan.FromMinutes(5))
            {
                m_Instance = instance;
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                m_Instance.BroadcastTimeLeft();
            }
        }
    }

    public class DungeonPersistance : Item
    {
        private FireDungeonInstance m_Instance;

        public override string DefaultName { get { return "Fire Dungeon Persistance - Internal"; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public FireDungeonBosses State { get { return m_Instance.CurrentState; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan FailTime { get { return DateTime.UtcNow - m_Instance.NextState; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Map Map { get { return m_Instance.m_Map; } }


        public DungeonPersistance(FireDungeonInstance instance)
            : base(0x0)
        {
            Movable = false;
            m_Instance = instance;
        }

        public DungeonPersistance(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
            // this has to be written first
            writer.Write(m_Instance.m_Map);

            // version 1
            writer.Write(m_Instance.CharredProtectors.Count);
            foreach (var charredProtector in m_Instance.CharredProtectors)
                writer.Write(charredProtector);
            
            writer.Write(m_Instance.CoreHounds.Count);
            foreach (var coreHound in m_Instance.CoreHounds)
                writer.Write(coreHound);

            writer.Write(m_Instance.LavaSurgers.Count);
            foreach (var lavaSurger in m_Instance.LavaSurgers)
                writer.Write(lavaSurger);

            // version 0
            writer.Write(m_Instance.ExitMoongate);
            writer.Write(m_Instance.ExitTeleport);
            writer.Write(m_Instance.OriginalParty.Count);
            foreach (PlayerMobile m in m_Instance.OriginalParty)
                writer.Write(m);

            writer.Write(m_Instance.InstanceFireRope);

            writer.Write(m_Instance.Players.Count);
            foreach (PlayerMobile m in m_Instance.Players)
                writer.Write(m);

            writer.Write(m_Instance.BaronVonGeddon);
            writer.Write(m_Instance.TankDragon);
            writer.Write(m_Instance.ImmortalFlameBoss);
            writer.Write(m_Instance.RubyDragon);
            writer.Write(m_Instance.RubyDragonHandler);
            writer.Write(m_Instance.Clompur);
            writer.Write(m_Instance.Stompur);
            writer.Write((byte)m_Instance.CurrentState);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Map map = reader.ReadMap();
            m_Instance = new FireDungeonInstance(map, this);
            switch (version)
            {
                case 1:
                    {
                        int count = reader.ReadInt();
                        m_Instance.CharredProtectors = new List<CharredProtector>();
                        for (int i = 0; i < count; i++)
                        {
                            var mob = reader.ReadMobile() as CharredProtector;
                            if (mob != null) m_Instance.CharredProtectors.Add(mob);
                        }

                        count = reader.ReadInt();
                        m_Instance.CoreHounds = new List<CoreHound>();
                        for(int i = 0; i < count; i++)
                        {
                            var mob = reader.ReadMobile() as CoreHound;
                            if (mob != null) m_Instance.CoreHounds.Add(mob);
                        }

                        count = reader.ReadInt();
                        m_Instance.LavaSurgers = new List<LavaSurger>();
                        for(int i = 0; i < count; i++)
                        {
                            var mob = reader.ReadMobile() as LavaSurger;
                            if (mob != null) m_Instance.LavaSurgers.Add(mob);
                        }
                        goto case 0;
                    }
                case 0:
                    {
                        m_Instance.ExitMoongate = reader.ReadItem() as Moongate;
                        m_Instance.ExitTeleport = reader.ReadItem() as FireDungeonExitTeleporter;

                        int count = reader.ReadInt();
                        m_Instance.OriginalParty = new List<PlayerMobile>();
                        for (int i = 0; i < count; i++)
                        {
                            var mob = reader.ReadMobile() as PlayerMobile;
                            if (mob != null)
                                m_Instance.OriginalParty.Add(mob);
                        }

                        m_Instance.InstanceFireRope = reader.ReadItem() as FireRopeController;

                        count = reader.ReadInt();
                        m_Instance.Players = new List<PlayerMobile>();
                        for (int i = 0; i < count; i++)
                        {
                            var mob = reader.ReadMobile() as PlayerMobile;
                            if (mob != null)
                                m_Instance.Players.Add(mob);
                        }


                        m_Instance.BaronVonGeddon = reader.ReadMobile() as BaronVonGeddon;
                        m_Instance.TankDragon = reader.ReadMobile() as TankDragon;
                        m_Instance.ImmortalFlameBoss = reader.ReadMobile() as ImmortalFlameBoss;
                        m_Instance.RubyDragon = reader.ReadMobile() as RubyDragon;
                        m_Instance.RubyDragonHandler = reader.ReadMobile() as DragonHandler;
                        m_Instance.Clompur = reader.ReadMobile() as SuperOgreLord;
                        m_Instance.Stompur = reader.ReadMobile() as SuperOgreLord;
                        m_Instance.CurrentState = (FireDungeonBosses)reader.ReadByte();

                        m_Instance.Resume();
                        break;
                    }
            }
            if (map == Map.Felucca)
                FireDungeon.InstanceOne = m_Instance;
            else
                FireDungeon.InstanceTwo = m_Instance;
        }

    }
}
