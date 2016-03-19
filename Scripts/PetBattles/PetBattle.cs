using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Regions;
using Server.Network;
using Server.Engines.PartySystem;
using Server.Accounting;
using Server.Achievements;

namespace Server.Custom
{
    public enum PetBattleState
    {
        WaitingForPlayers,
        ConfirmingPlayers,
        DeterminingRules,
        SelectingCreatures,
        StartCountdown,
        Battling,
        PostBattle
    }

    public enum PetBattleFormat
    {
        Solo1vs1,
        Solo2vs2,
        Simultaneous2vs2,
        Solo3vs3,
        Simultaneous3vs3,
    }

    public enum PetBattleResult
    {
        Win,
        Tie,
        Loss
    }

    public class PetBattle : Item
    {
        public PetBattle(Serial serial)
            : base(serial)
        {
        }

        [Constructable]
        public PetBattle(PetBattleSignupStone petBattleSignupStone)
            : base(0x186C)
        {
            Name = "a pet battle instance";

            m_PetBattleSignupStone = petBattleSignupStone;

            Visible = false;
            Movable = false;

            StartTimer();
        }

        public PetBattleSignupStone m_PetBattleSignupStone;        

        public List<PlayerMobile> WaitingList = new List<PlayerMobile>();
        public List<PlayerMobile> NeedConfirmationList = new List<PlayerMobile>();
        public List<PlayerMobile> ReadyList = new List<PlayerMobile>();

        public TimeSpan ConfirmationTimeout = TimeSpan.FromMinutes(0.5);
        public TimeSpan DetermineRulesTimeout = TimeSpan.FromMinutes(2);
        public TimeSpan SelectCreaturesTimeout = TimeSpan.FromMinutes(3);
        public TimeSpan BattleStartCountdown = TimeSpan.FromSeconds(8);
        public TimeSpan BattleDuration = TimeSpan.FromMinutes(3);
        public TimeSpan BattleEndCountdown = TimeSpan.FromSeconds(10);
        public TimeSpan PostBattleTimeout = TimeSpan.FromSeconds(3);

        public PetBattleRegion Region { get; set; }
        public Rectangle2D PetBattleRectangle = new Rectangle2D(new Point2D(5900, 396), new Point2D(6002, 498));

        public DateTime m_LastStateChange = DateTime.UtcNow;
        public int m_SecondsSinceStateChange = -1;

        public InternalTimer m_PetBattleTimer;

        public int m_RequiredPlayers = 2;

        public static int[] baseFee = new int[] { 250, 1000, 1000, 1000, 1000 };
        public static int[] levelExperience = new int[] { 0, 10, 30, 60, 100, 150, 210, 280, 360 };
        
        public static string[] PetBattleTitles = new string[] { "Pet Battle Novice", "Pet Battle Adept", "Pet Battle Expert", "Pet Battle Master", "Pet Battle Grandmaster" };
        public static int[] maxedCreaturesForTitles = new int[] { 1, 2, 4, 7, 10};

        public List<Team> m_Teams = new List<Team>();
        public PetBattleFormat m_BattleFormat = PetBattleFormat.Solo1vs1;
        public int m_CreaturesPerTeam = 1;
        public int fee = baseFee[0];
        public int wager = 0;

        public Mobile Announcer;     

        public int ticks = 0;
        public int opportunityTicks = 0;

        public int team1Hue = 0x059;
        public int team2Hue = 0x022;        

        public PetBattleState m_CurrentState;
        public PetBattleState CurrentState
        {
            get
            {
                return m_CurrentState;
            }

            set
            {
                m_CurrentState = value;
                m_LastStateChange = DateTime.UtcNow;
            }
        }

        public void AnnounceAbility(BaseCreature creature, string name, string description, PetAbilityType type, bool hasHitCheck, bool hit)
        {
            if (creature == null)
                return;

            if (creature.PetBattleTotem == null)
                return;

            if (creature.PetBattleTotem.Team == null)
                return;

            if (creature.m_PetBattleOpponent == null)
                return;
            
            if (hasHitCheck)
            {
                if (hit)
                {
                    if (Announcer != null)
                    {
                        if (!Announcer.Deleted && Announcer.Alive)
                            Announcer.PublicOverheadMessage(MessageType.Regular, creature.PetBattleTotem.Team.textHue, false, creature.PetBattleTotem.Team.m_Player.RawName + "'s " + creature.PetBattleTitle + " uses " + name + " and hits!");
                     }

                    creature.m_PetBattleOwner.SendMessage(creature.PetBattleTotem.Team.textHue, "Your " + creature.PetBattleTitle + " uses " + name + " and hit!");
                    creature.m_PetBattleOpponent.SendMessage(creature.PetBattleTotem.Team.textHue, "Their " + creature.PetBattleTitle + " used " + name + " and hit!");
                }

                else
                {
                    if (Announcer != null)
                    {
                        if (!Announcer.Deleted && Announcer.Alive)
                            Announcer.PublicOverheadMessage(MessageType.Regular, creature.PetBattleTotem.Team.textHue, false, creature.PetBattleTotem.Team.m_Player.RawName + "'s " + creature.PetBattleTitle + " uses " + name + " and misses!");
                    }
                    creature.m_PetBattleOwner.SendMessage(creature.PetBattleTotem.Team.textHue, "Your " + creature.PetBattleTitle + " uses " + name + " and misses!");
                    creature.m_PetBattleOpponent.SendMessage(creature.PetBattleTotem.Team.textHue, "Their " + creature.PetBattleTitle + " used " + name + " and misses!");
                }                
            }

            else
            {
                if (type == PetAbilityType.Opportunity)
                {
                    if (Announcer != null)
                    {
                        if (!Announcer.Deleted && Announcer.Alive)
                            Announcer.PublicOverheadMessage(MessageType.Regular, creature.PetBattleTotem.Team.textHue, false, "Opportunity for " + creature.PetBattleTotem.Team.m_Player.RawName + "'s " + creature.PetBattleTitle + ": " + name);
                    }

                    creature.m_PetBattleOwner.SendMessage(creature.PetBattleTotem.Team.textHue, "Opportunity for your " + creature.PetBattleTitle + ": " + description);
                    creature.m_PetBattleOpponent.SendMessage(creature.PetBattleTotem.Team.textHue, "Opportunity for their " + creature.PetBattleTitle + ": " + description);
                }

                else
                {
                    if (Announcer != null)
                    {
                        if (!Announcer.Deleted && Announcer.Alive)
                            Announcer.PublicOverheadMessage(MessageType.Regular, creature.PetBattleTotem.Team.textHue, false, creature.PetBattleTotem.Team.m_Player.RawName + "'s " + creature.PetBattleTitle + " uses " + name);
                    }

                    creature.m_PetBattleOwner.SendMessage(creature.PetBattleTotem.Team.textHue, "Your " + creature.PetBattleTitle + " uses " + name);
                    creature.m_PetBattleOpponent.SendMessage(creature.PetBattleTotem.Team.textHue, "Their " + creature.PetBattleTitle + " used " + name);
                }
            }            
        }

        public class Team
        {
            public PlayerMobile m_Player { get; set; }
            public List<PetBattleCreatureEntry> m_CreatureEntries { get; set; }            
            public int m_TeamNumber = 1;
            public int m_CurrentCreaturePosition = 1;
            public PetBattleFormat selectedFormat = PetBattleFormat.Solo1vs1;
            public int goldWager = 0;
            public bool ready = false;
            public int grimoireBrowsingPage = 1;
            public int browsingCreaturePosition = 0;
            public int textHue = 0;

            public Team(PlayerMobile player)
            {
                m_Player = player;
                m_CreatureEntries = new List<PetBattleCreatureEntry>();                
            }
        }        

        public void GetPlayers(int numberOfPlayers)
        {
            for (int a = 0; a < numberOfPlayers; a++)
            {
                PlayerMobile pm = WaitingList[0];

                if (pm != null)
                {
                    AddPlayerToNeedConfirmationList(pm);
                    RemovePlayerFromWaitingList(pm);

                    pm.SendMessage("You are next in line to take part in a pet battle. Double click the signup stone in the next 30 seconds to confirm your participation.");
                }
            }
        }

        //WaitingForPlayers
        public bool FindPlayerInWaitingList(PlayerMobile m)
        {
            return WaitingList.Contains(m);
        }

        public void AddPlayerToWaitingList(PlayerMobile m)
        {
            if (!WaitingList.Contains(m))
                WaitingList.Add(m);
        }

        public void RemovePlayerFromWaitingList(PlayerMobile m)
        {
            if (WaitingList.Contains(m))
                WaitingList.Remove(m);
        }

        public void RefreshWaitingList()
        {
            for (int i = 0; i < WaitingList.Count; i++)
            {
                if (WaitingList[i].NetState == null)
                {
                    WaitingList.RemoveAt(i--);
                }
            }
        }

        public bool CheckForEnoughWaitingPlayers()
        {
            RefreshWaitingList();

            //Enough Players
            if (WaitingList.Count >= m_RequiredPlayers)
            {
                return true;
            }

            return false;
        }

        //Confirming Players
        public void BeginConfirmingPlayers()
        {
            RefreshWaitingList();

            GetPlayers(m_RequiredPlayers);

            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.ConfirmingPlayers;
        }

        public bool FindPlayerInNeedConfirmationList(PlayerMobile m)
        {
            return NeedConfirmationList.Contains(m);
        }

        public void AddPlayerToNeedConfirmationList(PlayerMobile m)
        {
            if (!NeedConfirmationList.Contains(m))
                NeedConfirmationList.Add(m);
        }

        public void RemovePlayerFromNeedConfirmationList(PlayerMobile m)
        {
            if (NeedConfirmationList.Contains(m))
                NeedConfirmationList.Remove(m);
        }

        public void RefreshNeedConfirmationList()
        {
            for (int i = 0; i < NeedConfirmationList.Count; i++)
            {
                if (NeedConfirmationList[i].NetState == null)
                {
                    NeedConfirmationList.RemoveAt(i--);
                }
            }
        }

        //Ready List
        public void AddPlayerToReadyList(PlayerMobile m)
        {
            if (!ReadyList.Contains(m))
                ReadyList.Add(m);
        }

        public void RemovePlayerFromReadyList(PlayerMobile m)
        {
            if (ReadyList.Contains(m))
                ReadyList.Remove(m);
        }

        public void RefreshReadyList()
        {
            for (int i = 0; i < ReadyList.Count; i++)
            {
                if (ReadyList[i].NetState == null)
                {
                    ReadyList.RemoveAt(i--);
                }
            }
        }

        public bool CheckForEnoughReadyPlayers()
        {
            RefreshReadyList();

            //Enough Players
            if (ReadyList.Count >= m_RequiredPlayers)
            {
                return true;
            }

            return false;
        }

        public void AddTeam(PlayerMobile player)
        {
            m_Teams.Add(new Team(player));
        }

        public void RemoveTeam(PlayerMobile player)
        {
            foreach (Team team in m_Teams)
            {
                if (team.m_Player == player)
                {
                    m_Teams.Remove(team);
                    break;
                }
            }
        }

        public void ClearTeams()
        {
            m_Teams = new List<Team>();
        }

        public bool FindPlayerInReadyList(PlayerMobile m)
        {
            return ReadyList.Contains(m);
        }        

        //Determining Rules
        public void BeginDeterminingRules()
        {
            RefreshReadyList();

            foreach (PlayerMobile pm in ReadyList)
            {
                AddTeam(pm);
            }

            foreach (PlayerMobile pm in ReadyList)
            {
                pm.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                pm.SendGump(new Gumps.PetBattleDetermineRulesGump(this, pm));
            }

            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.DeterminingRules;
        }       

        //Determining Rules
        public void BeginSelectingCreatures()
        {
            //Creature Button Groups
            switch (m_BattleFormat)
            {
                case PetBattleFormat.Solo2vs2:
                    m_CreaturesPerTeam = 2;                   
                break;

                case PetBattleFormat.Simultaneous2vs2:
                    m_CreaturesPerTeam = 2;       
                break;

                case PetBattleFormat.Solo3vs3:
                    m_CreaturesPerTeam = 3;       
                break;

                case PetBattleFormat.Simultaneous3vs3:
                    m_CreaturesPerTeam = 3;       
                break;
            }
            
            //Get Creature Collection for Players and Populate Initial Creatures
            foreach (Team team in m_Teams)
            {
                team.ready = false;
                team.m_Player.PetBattleCreatureCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(team.m_Player);

                for (int a = 0; a < m_CreaturesPerTeam; a++)
                {
                    team.m_CreatureEntries.Add(team.m_Player.PetBattleCreatureCollection.m_CreatureEntries[a]);
                }
            }

            //Send Each Player Select Creatures Gump
            foreach (Team team in m_Teams)
            {
                team.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team.m_Player.SendGump(new Gumps.PetBattleSelectCreaturesGump(this, team.m_Player));     
            }
            
            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.SelectingCreatures;
        }

        public void BeginStartCountdown()
        {
            bool petBattleInvalid = false;

            if (Announcer != null)
            {
                if (!Announcer.Deleted && Announcer.Alive)
                    Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, "Pet Battle beginning between " + m_Teams[0].m_Player.RawName + " and " + m_Teams[1].m_Player.RawName + "...");
            }

            foreach (Team team in m_Teams)
            {
                team.m_Player.CloseGump(typeof(Gumps.PetBattleSelectCreaturesGump));                
            }

            for (int a = 0; a < m_Teams.Count; a++)
            {
                Team team = m_Teams[a];
                team.m_TeamNumber = a + 1;
            }

            foreach (PetBattleTotem petBattleTotem in m_PetBattleSignupStone.m_PetBattleTotems)
            {   
                Team team = m_Teams[petBattleTotem.TeamNumber - 1];

                PetBattleCreatureEntry creatureEntry = team.m_CreatureEntries[petBattleTotem.PositionNumber - 1];                

                ConfigureTotem(petBattleTotem, creatureEntry, team);                             
            }

            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.StartCountdown;
        }

        public void ConfigureTotem(PetBattleTotem petBattleTotem, PetBattleCreatureEntry creatureEntry, Team team)
        {
            BaseCreature creature = (BaseCreature)Activator.CreateInstance(creatureEntry.m_Type);
            
            creature.PetBattleOffensivePowerMax = creatureEntry.m_OffensivePower;
            creature.PetBattleDefensivePowerMax = creatureEntry.m_DefensivePower;
            
            petBattleTotem.m_Team = team;
            petBattleTotem.m_Player = team.m_Player;
            petBattleTotem.m_Creature = creature;
            petBattleTotem.SetCreature(creature.PetBattleItemId, creature.PetBattleItemHue, creature.PetBattleStatueOffsetZ);
            petBattleTotem.m_Active = true;            

            petBattleTotem.offensivePowerMax = creature.PetBattleOffensivePowerMax;
            petBattleTotem.defensivePowerMax = creature.PetBattleDefensivePowerMax;            

            for (int a = 0; a < petBattleTotem.m_OffensiveLanterns.Count; a++)
            {
                if (a < petBattleTotem.offensivePowerMax)
                {
                    petBattleTotem.m_OffensiveLanterns[a].Visible = true;
                    petBattleTotem.m_OffensiveLanterns[a].Douse();
                }

                else
                {
                    petBattleTotem.m_OffensiveLanterns[a].Visible = false;
                }
            }

            for (int a = 0; a < petBattleTotem.m_DefensiveLanterns.Count; a++)
            {
                if (a < petBattleTotem.defensivePowerMax)
                {
                    petBattleTotem.m_DefensiveLanterns[a].Visible = true;
                    petBattleTotem.m_DefensiveLanterns[a].Douse();
                }

                else
                {
                    petBattleTotem.m_DefensiveLanterns[a].Visible = false;
                }
            }

            for (int a = 0; a < petBattleTotem.m_OpportunityLanterns.Count; a++)
            {
                if (a < petBattleTotem.opportunityPowerMax)
                {
                    petBattleTotem.m_OpportunityLanterns[a].Visible = true;
                    petBattleTotem.m_OpportunityLanterns[a].Douse();
                }

                else
                {
                    petBattleTotem.m_OpportunityLanterns[a].Visible = false;
                }
            }

            creature.m_PetBattle = m_PetBattleSignupStone.m_PetBattle;
            creature.m_PetBattleTotem = petBattleTotem;
            creature.m_PetBattleTeam = team;
            creature.m_PetBattleOwner = team.m_Player;
            creature.m_PetBattleAbilityEffectTimer = new BaseCreature.PetBattleAbilityEffectTimer(creature);
            creature.m_PetBattleAbilityEffectTimer.Start();

            if (team.m_TeamNumber == 1)
            {
                team.textHue = team1Hue;
                creature.m_PetBattleTeamHue = team1Hue;
            }

            else
            {
                team.textHue = team2Hue;
                creature.m_PetBattleTeamHue = team2Hue;
            }

            foreach (Team checkTeam in m_PetBattleSignupStone.m_PetBattle.m_Teams)
            {
                if (checkTeam.m_Player != team.m_Player)
                {
                    creature.m_PetBattleOpponent = checkTeam.m_Player;
                    break;
                }
            }

            creature.MoveToWorld(petBattleTotem.m_PetBattleStartTile.Location, petBattleTotem.m_PetBattleStartTile.Map);
            creature.Frozen = true;            

            if (team.m_TeamNumber == 1)
                creature.Direction = Direction.East;
            else
                creature.Direction = Direction.West;            
        }

        public void UnleashCreatures()
        {
            //Creatures Battle One at a Time: Starting with Position 1
            if (m_BattleFormat == PetBattleFormat.Solo1vs1 || m_BattleFormat == PetBattleFormat.Solo2vs2 || m_BattleFormat == PetBattleFormat.Solo3vs3)
            {
                foreach (Team team in m_Teams)
                {
                    team.m_CurrentCreaturePosition = 1;
                }
                
                foreach (PetBattleTotem petBattleTotem in m_PetBattleSignupStone.m_PetBattleTotems)
                {
                    if (petBattleTotem.PositionNumber == 1)
                    {
                        petBattleTotem.Unlocked = true;
                        petBattleTotem.m_Creature.Frozen = false;                       
                    }
                }
            }

            //Creatures Battle All at Once
            else
            {
                foreach (PetBattleTotem petBattleTotem in m_PetBattleSignupStone.m_PetBattleTotems)
                {
                    petBattleTotem.Unlocked = true;
                    petBattleTotem.m_Creature.Frozen = false;                   
                }
            }            
        }

        public void BeginBattling()
        {
            if (Announcer != null)
            {
                if (!Announcer.Deleted && Announcer.Alive)
                    Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, "Pet Battle Begins!");
            }
            
            foreach (Team team in m_Teams)
            {
                team.m_Player.SendMessage("Pet Battle begins!");
            }

            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.Battling;
        }       

        public bool CheckRulesConfirmation()
        {
            bool agreed = true;

            PetBattleFormat petBattleFormat = PetBattleFormat.Solo1vs1;

            Team team1 = m_Teams[0];
            Team team2 = m_Teams[1];

            if (team1 == null || team2 == null)
                return false;

            if (team1.m_Player == null || team2.m_Player == null)
                return false;

            if (team1.ready == false || team2.ready == false)
                return false;

            //Rules Format
            if (team1.selectedFormat != team2.selectedFormat)
            {
                team1.m_Player.SendMessage("You and your opponent have not agreed upon a Pet Battle format.");
                team1.ready = false;

                team2.m_Player.SendMessage("You and your opponent have not agreed upon a Pet Battle format.");
                team2.ready = false;

                team1.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team1.m_Player.SendGump(new Gumps.PetBattleDetermineRulesGump(this, team1.m_Player));

                team2.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team2.m_Player.SendGump(new Gumps.PetBattleDetermineRulesGump(this, team2.m_Player));

                return false;
            }

            //Same Wager
            if (team1.goldWager != team2.goldWager)
            {
                team1.m_Player.SendMessage("You and your opponent have not agreed upon a wager.");
                team1.ready = false;

                team2.m_Player.SendMessage("You and your opponent have not agreed upon a wager.");
                team2.ready = false;

                team1.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team1.m_Player.SendGump(new Gumps.PetBattleDetermineRulesGump(this, team1.m_Player));

                team2.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team2.m_Player.SendGump(new Gumps.PetBattleDetermineRulesGump(this, team2.m_Player));

                return false;
            }

            //Validate Gold Amounts in Banks
            int goldWager = team1.goldWager;

            bool player1EnoughGold = (Banker.GetBalance(team1.m_Player) >= goldWager + fee);
            bool player2EnoughGold = (Banker.GetBalance(team2.m_Player) >= goldWager + fee);

            if (!player1EnoughGold || !player2EnoughGold)
            {
                team1.m_Player.SendMessage("Both players must have enough funds to cover the fee and the wager in their bank account.");
                team1.ready = false;

                team2.m_Player.SendMessage("Both players must have enough funds to cover the fee and the wager in their bank account.");
                team2.ready = false;

                team1.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team1.m_Player.SendGump(new Gumps.PetBattleDetermineRulesGump(this, team1.m_Player));

                team2.m_Player.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                team2.m_Player.SendGump(new Gumps.PetBattleDetermineRulesGump(this, team2.m_Player));

                return false;
            }

            return agreed;
        }

        public bool CheckSelectingCreaturesConfirmation()
        {
            bool agreed = true;

            Team team1 = m_Teams[0];
            Team team2 = m_Teams[1];

            if (team1 == null || team2 == null)
                return false;

            if (team1.m_Player == null || team2.m_Player == null)
                return false;

            if (team1.ready == false || team2.ready == false)
                return false;

            return agreed;
        }

        public void IncreasePowerLevels()
        {           
            foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
            {
                if (totem.Unlocked)
                    totem.IncreasePower();
            }            
        }

        public void IncreaseOpportunityPowerLevels()
        {
            foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
            {
                if (totem.Unlocked)
                    totem.IncreaseOpportunityPower();
            }
        }

        public void PetBattleCreatureDeath(BaseCreature creature)
        {
            if (CurrentState != PetBattleState.Battling)
                return;
            
            Team playerTeam = creature.m_PetBattleTotem.m_Team;
            Team opponentTeam = null;

            int position = creature.m_PetBattleTotem.PositionNumber;

            foreach (Team team in m_Teams)
            {
                if (team != playerTeam)
                {
                    opponentTeam = team;
                    break;
                }
            }

            if (Announcer != null)
            {
                if (!Announcer.Deleted && Announcer.Alive)
                    Announcer.PublicOverheadMessage(MessageType.Regular, creature.PetBattleTotem.Team.textHue, false, creature.PetBattleTotem.Team.m_Player.RawName + "'s " + creature.PetBattleTitle + " has died!");
            }
            
            playerTeam.m_Player.SendMessage("Your " + creature.PetBattleTitle + " has died!");

            //One Creature at a Time
            if (m_BattleFormat == PetBattleFormat.Solo1vs1 || m_BattleFormat == PetBattleFormat.Solo2vs2 || m_BattleFormat == PetBattleFormat.Solo3vs3)
            {
                //Player's Last Creature
                if (position >= m_CreaturesPerTeam)
                {
                    BattleComplete();
                }

                //Add Player's Next Creature
                else
                {
                    playerTeam.m_CurrentCreaturePosition++;

                    PetBattleTotem opponentTotem = null;

                    foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
                    {
                        if (totem.Team == opponentTeam && totem.PositionNumber == playerTeam.m_CurrentCreaturePosition)
                        {
                            opponentTotem = totem;
                            break;
                        }
                    }
                    
                    foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
                    {
                        if (totem != null)
                        {
                            if (totem.Team == playerTeam && totem.PositionNumber == playerTeam.m_CurrentCreaturePosition)
                            {
                                totem.Unlocked = true;

                                if (totem.m_Creature != null)
                                {
                                    BaseCreature newCreature = totem.m_Creature;

                                    newCreature.Frozen = false;

                                    if (Announcer != null)
                                    {
                                        if (!Announcer.Deleted && Announcer.Alive)
                                            Announcer.PublicOverheadMessage(MessageType.Regular, creature.PetBattleTotem.Team.textHue, false, playerTeam.m_Player.RawName + "'s " + newCreature.PetBattleTitle + "enters the match...");
                                    }

                                    playerTeam.m_Player.SendMessage("Your " + newCreature.PetBattleTitle + " enters the match.");
                                    opponentTeam.m_Player.SendMessage("Your opponent's " + newCreature.PetBattleTitle + " enters the match.");

                                    if (opponentTotem.m_Creature != null)
                                    {
                                        opponentTotem.m_Creature.Location = opponentTotem.m_PetBattleStartTile.Location;
                                    }
                                }

                                break;
                            }
                        }
                    }
                }
            }

            else
            {
                int deadCreatures = 0;

                foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
                {
                    if (totem != null)
                    {
                        if (totem.Team != null && totem.Team == playerTeam)
                        {
                            if (totem.m_Creature != null)
                            {
                                if (!totem.m_Creature.Alive)
                                    deadCreatures++;
                            }
                        }
                    }
                }

                //All of Player's Creatures Are Dead
                if (deadCreatures >= m_CreaturesPerTeam)
                {
                    BattleComplete();
                }
            }
        }

        public void BattleComplete()
        {   
            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.PostBattle;

            DetermineWinner();
        }         

        public void DetermineWinner()
        {
            int team1CreaturesRemaining = 0;
            int team2CreaturesRemaining = 0;
            double team1HealthPercentRemaining = 0;            
            double team2HealthPercentRemaining = 0;

            Team team1;
            Team team2;

            if (m_PetBattleSignupStone.PetBattle.m_Teams[0].m_TeamNumber == 1)
            {
                team1 = m_Teams[0];
                team2 = m_Teams[1];
            }

            else
            {
                team1 = m_Teams[1];
                team2 = m_Teams[0];
            }
            
            for (int a = 0; a < m_PetBattleSignupStone.m_PetBattleTotems.Count; a++)
            {
                PetBattleTotem totem = m_PetBattleSignupStone.m_PetBattleTotems[a];

                if (totem == null)
                    continue;

                BaseCreature creature = totem.m_Creature;
                
                if (creature != null)
                {
                    if (creature.Alive)
                    {
                        if (totem.TeamNumber == 1)
                        {
                            team1CreaturesRemaining++;
                            team1HealthPercentRemaining += (double)creature.Hits / (double)creature.HitsMax;
                        }

                        else
                        {
                            team2CreaturesRemaining++;
                            team2HealthPercentRemaining += (double)creature.Hits / (double)creature.HitsMax;
                        }
                    }
                }
            }

            //Determine Winner: Player With Most Creatures, or if Tied, Most Percent Health Remaining
            if (team1CreaturesRemaining > team2CreaturesRemaining)
                PetBattleVictory(team1);
            else if (team1CreaturesRemaining < team2CreaturesRemaining)
                PetBattleVictory(team2);
            else
            {
                if (team1HealthPercentRemaining > team2HealthPercentRemaining)
                    PetBattleVictory(team1);
                else if (team1HealthPercentRemaining < team2HealthPercentRemaining)
                    PetBattleVictory(team2);
                else
                    PetBattleVictory(null);
            }
        }

        public void PetBattleVictory(Team winner)
        {
            //Tie
            if (winner == null)
            {
                if (Announcer != null)
                {
                    if (!Announcer.Deleted && Announcer.Alive)
                        Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, "The match ends in a tie!");
                }
                
                foreach (Team team in m_Teams)
                {   
                    team.m_Player.SendMessage("The match ends in a tie! Wagers are returned.");
                    Banker.Deposit(team.m_Player, wager);

                    AssignExperience(team, PetBattleResult.Tie);                                  
                }
            }

            else
            {
                Team winningTeam = winner;

                if (Announcer != null)
                {
                    if (!Announcer.Deleted && Announcer.Alive)
                        Announcer.PublicOverheadMessage(MessageType.Regular, winningTeam.textHue, false, winningTeam.m_Player.RawName + " wins the match!");
                }

                if (wager > 0)
                    winningTeam.m_Player.SendMessage("You have won the match and " + wager.ToString() + " gold!");
                else
                    winningTeam.m_Player.SendMessage("You have won the match!");

                Banker.Deposit(winningTeam.m_Player, wager);

                AssignExperience(winningTeam, PetBattleResult.Win);

                Team losingTeam = null;

                foreach (Team team in m_Teams)
                {
                    if (team != winningTeam)
                        losingTeam = team;
                }                

                losingTeam.m_Player.SendMessage("You have lost the match!");
                AssignExperience(losingTeam, PetBattleResult.Loss);               
            }

            foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
            {
                totem.Unlocked = false;

                if (totem.m_Creature.Alive)
                {                    
                    totem.m_Creature.Frozen = true;
                    totem.m_Creature.Poison = null;
                    totem.m_Creature.m_PetBattleAbilityEffectEntries.Clear();

                    if (totem.TeamNumber == 1)                    
                        totem.m_Creature.Direction = Direction.East;                    
                    else
                        totem.m_Creature.Direction = Direction.West;
                }
            }
                        
            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.PostBattle;
        }

        public void AssignExperience(Team team, PetBattleResult result)
        {
            DailyAchievement.TickProgress(Category.PvP, team.m_Player, PvPCategory.PetBattles);

            foreach (PetBattleCreatureEntry creatureEntry in team.m_CreatureEntries)
            {
                int experience = 0;
                
                switch (result)
                {
                    case PetBattleResult.Win:                        
                        creatureEntry.m_Experience += 10;
                        creatureEntry.m_Wins++;
                    break;

                    case PetBattleResult.Tie:
                        creatureEntry.m_Experience += 5;
                        creatureEntry.m_Ties++;
                    break;

                    case PetBattleResult.Loss:
                        creatureEntry.m_Experience += 5;
                        creatureEntry.m_Losses++;
                    break;
                }                
                
                CheckPetLevel(team, creatureEntry);

                PetBattlePersistance.UpdatePlayerCreatureEntry(team.m_Player, creatureEntry);
            }
        }

        public void CheckPetLevel(Team team, PetBattleCreatureEntry creatureEntry)
        {
            int previousLevel = creatureEntry.m_Level;
            int newLevel = 1;

            int maxLevelCreatures = 0;

            PetBattleCreatureCollection playerCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(team.m_Player);

            foreach (PetBattleCreatureEntry entry in playerCollection.m_CreatureEntries)
            {
                if (entry.m_Level == PetBattle.levelExperience.Length)
                    maxLevelCreatures++;
            }
         
            BaseCreature creature = (BaseCreature)Activator.CreateInstance(creatureEntry.m_Type);
            
            if (creatureEntry.m_Experience >= levelExperience[levelExperience.Length - 1])            
                newLevel = levelExperience.Length;    
            else
            {
                for (int a = 0; a < levelExperience.Length - 1; a++)
                {
                    if (creatureEntry.m_Experience >= levelExperience[a] && creatureEntry.m_Experience < levelExperience[a + 1])
                    {
                        newLevel = a + 1;
                    }
                }
            }            

            //Level Increased
            if (newLevel > creatureEntry.m_Level)
            {
                creatureEntry.m_Level = newLevel;

                creatureEntry.m_OffensivePower = (int)(Math.Floor((double)creatureEntry.m_Level / 2) + 1);
                creatureEntry.m_DefensivePower = (int)(Math.Ceiling((double)creatureEntry.m_Level / 2));

                if (Announcer != null)
                {
                    if (!Announcer.Deleted && Announcer.Alive)
                        Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, team.m_Player.RawName + "'s " + creature.PetBattleTitle + " has reached level " + creatureEntry.m_Level + "!");
                }

                team.m_Player.SendMessage("Your " + creature.PetBattleTitle + " has reached level " + creatureEntry.m_Level + "!");

                //Creature Hits Max Level: Earn Creature Totem Reward
                if (creatureEntry.m_Level == PetBattle.levelExperience.Length)
                {
                    int newMaxLevelCreaturesTotal = maxLevelCreatures + 1;

                    bool packOk = false;

                    Container pack = team.m_Player.Backpack;

                    if (pack != null)
                    {
                        if (pack.TotalItems < pack.MaxItems && pack.Weight < pack.MaxWeight)
                        {
                            packOk = true;
                            pack.AddItem(new PetBattleRewardTotem(team.m_Player, creatureEntry.m_Type, creature.PetBattleTitle));

                            team.m_Player.SendMessage("A pet battle reward statue has been placed in your pack.");
                        }
                    }

                    if (!packOk)
                    {
                        BankBox bank = team.m_Player.BankBox;

                        if (bank != null)
                        {
                            if (bank.TotalItems < bank.MaxItems)
                            {
                                pack.AddItem(new PetBattleRewardTotem(team.m_Player, creatureEntry.m_Type, creature.PetBattleTitle));
                                team.m_Player.SendMessage("A pet battle reward statue has been placed in your bank box.");
                            }
                        }
                    }

                    //New Title Earned
                    for (int a = 0; a < maxedCreaturesForTitles.Length; a++)
                    {
                        if (newMaxLevelCreaturesTotal == maxedCreaturesForTitles[a])
                        {
                            string newTitle = PetBattleTitles[a];

                            Account account = team.m_Player.Account as Account;

                            //For Each Character on Account
                            if (account != null)
                            {
                                for (int b = 0; b < (account.Length - 1); b++)
                                {
                                    PlayerMobile player = account.accountMobiles[b] as PlayerMobile;

                                    if (player != null)
                                    {
                                        if (!player.Deleted && !player.TitlesPrefix.Contains(newTitle))
                                        {
                                            player.TitlesPrefix.Add(newTitle);
                                            player.SendMessage("The title of " + newTitle + " has now been added to your list of selectable titles.");

                                            player.FixedParticles(0x375A, 9, 40, 5027, EffectLayer.Waist);
                                            player.PlaySound(0x1F7);                                            
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }

            creature.Delete();                        
        }        

        public void ProcessTimer()
        {
            m_SecondsSinceStateChange++;           

            //Refresh PetBattle Activity Timer for All Players Waiting For or In This Pet Battle
            foreach (PlayerMobile player in WaitingList)            
                player.LastPetBattleActivity = DateTime.UtcNow;            

            foreach (PlayerMobile player in NeedConfirmationList)            
                player.LastPetBattleActivity = DateTime.UtcNow;            

            foreach (PlayerMobile player in ReadyList)            
                player.LastPetBattleActivity = DateTime.UtcNow;            
            
            switch (CurrentState)
            {
                case PetBattleState.WaitingForPlayers:
                {
                    //Enough Players on Waiting List
                    if (CheckForEnoughWaitingPlayers())
                    {
                        BeginConfirmingPlayers();
                        break;
                    }

                    break;
                }

                case PetBattleState.ConfirmingPlayers:
                {
                    //Enough Players Confirmed Ready
                    if (CheckForEnoughReadyPlayers())
                    {
                        BeginDeterminingRules();
                        break;
                    }

                    //Timeout: Not Enough Confirmed Players
                    if (m_LastStateChange + ConfirmationTimeout < DateTime.UtcNow)
                    {
                        RefreshNeedConfirmationList();

                        int playersToRemove = NeedConfirmationList.Count;

                        for (int a = 0; a < playersToRemove; a++)
                        {
                            PlayerMobile pm = NeedConfirmationList[0];

                            pm.SendMessage("You did not confirm your pet battle entry in time to particpate.");
                            RemovePlayerFromNeedConfirmationList(pm);
                        }

                        RefreshWaitingList();
                        RefreshReadyList();

                        int validParticipants = ReadyList.Count;
                        int newPartipicantsNeeded = m_RequiredPlayers - validParticipants;

                        //Need New Opponents and There Are Enough on Waiting List
                        if (WaitingList.Count >= newPartipicantsNeeded)
                        {
                            foreach (PlayerMobile pm in ReadyList)
                            {
                                pm.SendMessage("One or more of your opponents dropped out. Finding new opponent...");
                            }

                            GetPlayers(newPartipicantsNeeded);

                            m_LastStateChange = DateTime.UtcNow;
                            m_SecondsSinceStateChange = 0;
                            CurrentState = PetBattleState.ConfirmingPlayers;

                            break;
                        }

                        //Need New Opponents But Not Enough Currently On Waiting List
                        else
                        {
                            int playersToMoveToWaitingList = ReadyList.Count;

                            for (int a = 0; a < playersToMoveToWaitingList; a++)
                            {
                                PlayerMobile pm = ReadyList[0];

                                pm.SendMessage("One or more of your opponents dropped out and not enough players are available. You have been placed back atop the waiting list.");

                                AddPlayerToWaitingList(pm);
                                RemovePlayerFromReadyList(pm);
                            }

                            m_LastStateChange = DateTime.UtcNow;
                            m_SecondsSinceStateChange = 0;
                            CurrentState = PetBattleState.WaitingForPlayers;

                            break;
                        }
                    }

                    break;
                }

                case PetBattleState.DeterminingRules:
                {
                    if (CheckRulesConfirmation())
                    {
                        m_BattleFormat = m_Teams[0].selectedFormat;
                        wager = m_Teams[0].goldWager;
                        fee = baseFee[(int)m_BattleFormat];

                        Banker.Withdraw(m_Teams[0].m_Player, wager + fee);
                        Banker.Withdraw(m_Teams[1].m_Player, wager + fee);                            

                        BeginSelectingCreatures();

                        break;
                    }

                    //Timeout: Took Too Long For Setup
                    if (m_LastStateChange + DetermineRulesTimeout < DateTime.UtcNow)
                    {
                        m_BattleFormat = PetBattleFormat.Solo1vs1;
                        wager = 0;
                        fee = baseFee[0];

                        foreach (Team team in m_Teams)
                        {
                            team.m_Player.SendMessage("As both players were unable to agree upon a format, 1vs1 has been automatically selected...");
                        }

                        BeginSelectingCreatures();
                    }

                    break;
                }

                case PetBattleState.SelectingCreatures:
                {
                    if (CheckSelectingCreaturesConfirmation())
                    {
                        BeginStartCountdown();

                        break;
                    }

                    //Timeout: Took Too Long For Creature Selection
                    if (m_LastStateChange + SelectCreaturesTimeout < DateTime.UtcNow)
                    {
                        Banker.Deposit(m_Teams[0].m_Player, wager + fee);
                        Banker.Deposit(m_Teams[1].m_Player, wager + fee);

                        foreach (Team team in m_Teams)
                        {
                            team.m_Player.SendMessage("Pet Battle beginning automatically with currently selected creatures...");
                        }

                        BeginStartCountdown();
                    }

                    break;
                }

                case PetBattleState.StartCountdown:
                {
                    foreach (Team team in m_Teams)
                    {                       
                        team.m_Player.LastPetBattleActivity = DateTime.UtcNow;
                    }                   

                    int secondsRemaining = Math.Abs((int)((DateTime.UtcNow - (m_LastStateChange + BattleStartCountdown)).TotalSeconds));
                    

                    if (secondsRemaining == 6)
                    {
                        foreach (Team team in m_Teams)
                        {
                            team.m_Player.SendMessage("Pet Battle beginning in...");
                        }
                    }

                    else if (secondsRemaining <= 5 && secondsRemaining > 0)
                    {
                        foreach (Team teamInstance in m_Teams)
                        {
                            teamInstance.m_Player.SendMessage(secondsRemaining.ToString());
                        }
                    }

                    else if (secondsRemaining <= 0)
                    {
                        UnleashCreatures();
                        BeginBattling();
                    }                        

                    break;
                }

                case PetBattleState.Battling:
                {
                    ticks++;
                    opportunityTicks++;

                    foreach (Team team in m_Teams)
                    {                        
                        team.m_Player.LastPetBattleActivity = DateTime.UtcNow;
                    }

                    //Grant Power Levels
                    if (ticks >= 6)
                    {
                        IncreasePowerLevels();
                        ticks = 0;
                    }

                    //Grant Opportunity Power Levels
                    if (opportunityTicks >= 30)
                    {
                        IncreaseOpportunityPowerLevels();
                        opportunityTicks = 0;
                    }

                    int secondsRemaining = Math.Abs((int)Math.Ceiling((DateTime.UtcNow - (m_LastStateChange + BattleDuration)).TotalSeconds));
                    secondsRemaining--;

                    if (secondsRemaining == 30)
                    {
                        foreach (Team teamInstance in m_Teams)
                        {
                            teamInstance.m_Player.SendMessage("30 seconds remaining!");
                        }

                        if (Announcer != null)
                        {
                            if (!Announcer.Deleted && Announcer.Alive)
                                Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, "30 seconds remaining!");
                        }
                    }

                    else if (secondsRemaining == 11)
                    {
                        foreach (Team teamInstance in m_Teams)
                        {
                            teamInstance.m_Player.SendMessage("Pet Battle ending in...");
                        }

                        if (Announcer != null)
                        {
                            if (!Announcer.Deleted && Announcer.Alive)
                                Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, "Pet Battle ending in...");
                        }
                    }

                    else if (secondsRemaining > 0 && secondsRemaining <= 10)
                    {
                        foreach (Team teamInstance in m_Teams)
                        {
                            teamInstance.m_Player.SendMessage(secondsRemaining.ToString());
                        }

                        if (Announcer != null)
                        {
                            if (!Announcer.Deleted && Announcer.Alive)
                                Announcer.PublicOverheadMessage(MessageType.Regular, 0, false, secondsRemaining.ToString());
                        }
                    }

                    else if (secondsRemaining <= 0)
                    {
                        foreach (Team teamInstance in m_Teams)
                        {
                            teamInstance.m_Player.SendMessage("Pet Battle complete!");
                        }

                        BattleComplete();
                    }

                    else
                    {
                    }                  
                                     
                    break;
                }

                case PetBattleState.PostBattle:
                {
                    foreach (Team team in m_Teams)
                    {                        
                        team.m_Player.LastPetBattleActivity = DateTime.UtcNow;
                    }
                    
                    int secondsRemaining = Math.Abs((int)((DateTime.UtcNow - (m_LastStateChange + PostBattleTimeout)).TotalSeconds));

                    if (secondsRemaining <= 0)
                    {
                        foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
                        {
                            totem.Reset();
                        }

                        foreach (Team team in m_Teams)
                        {
                            team.m_Player.PetBattleCreatureCollection = null;                            
                        }

                        PetBattleReset();
                    }

                    break;
                }
            }
        }

        public void PetBattleReset()
        {
            PlayerMobile player1 = m_Teams[0].m_Player;
            PlayerMobile player2 = m_Teams[1].m_Player;

            RemovePlayerFromWaitingList(player1);
            RemovePlayerFromWaitingList(player2);
            RemovePlayerFromNeedConfirmationList(player1);
            RemovePlayerFromNeedConfirmationList(player2);
            RemovePlayerFromReadyList(player1);
            RemovePlayerFromReadyList(player2);

            player1.PetBattleCreatureCollection = null;
            player2.PetBattleCreatureCollection = null;            
            
            m_Teams.Clear();
            m_BattleFormat = PetBattleFormat.Solo1vs1;
            m_CreaturesPerTeam = 1;
            wager = 0;
            fee = baseFee[0];
            ticks = 0;
            opportunityTicks = 0;
            
            m_LastStateChange = DateTime.UtcNow;
            m_SecondsSinceStateChange = 0;
            CurrentState = PetBattleState.WaitingForPlayers;
        }

        public void StartTimer()
        {
            if (m_PetBattleTimer == null || !m_PetBattleTimer.Running)
            {
                m_PetBattleTimer = new InternalTimer(this);
                m_PetBattleTimer.Start();
            }
        }

        public class InternalTimer : Timer
        {
            public PetBattle m_PetBattle;

            public InternalTimer(PetBattle petBattle)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_PetBattle = petBattle;
            }

            protected override void OnTick()
            {
                if (!Running)
                    return;

                m_PetBattle.ProcessTimer();
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_PetBattleTimer != null)
            {
                m_PetBattleTimer.Stop();
                m_PetBattleTimer = null;
            }

            this.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_PetBattleSignupStone);

            writer.Write((int)WaitingList.Count);
            foreach (PlayerMobile pm in WaitingList)
            {
                writer.Write(pm);
            }

            writer.Write((int)NeedConfirmationList.Count);
            foreach (PlayerMobile pm in NeedConfirmationList)
            {
                writer.Write(pm);
            }

            writer.Write((int)ReadyList.Count);
            foreach (PlayerMobile pm in ReadyList)
            {
                writer.Write(pm);
            }
          
            writer.Write(m_SecondsSinceStateChange); 

            //Team: Start
            writer.Write((int)m_Teams.Count);
            foreach (Team team in m_Teams)
            {
                writer.Write(team.m_Player);

                writer.Write((int)team.m_CreatureEntries.Count);
                foreach (PetBattleCreatureEntry entry in team.m_CreatureEntries)
                {
                    writer.Write(entry.m_Type.ToString());

                    writer.Write(entry.m_Level);
                    writer.Write(entry.m_OffensivePower);
                    writer.Write(entry.m_DefensivePower);

                    writer.Write(entry.m_Experience);
                    writer.Write(entry.m_Wins);
                    writer.Write(entry.m_Ties);
                    writer.Write(entry.m_Losses);
                }

                writer.Write(team.m_TeamNumber);
                writer.Write(team.m_CurrentCreaturePosition);
                writer.Write((int)team.selectedFormat);
                writer.Write(team.goldWager);
                writer.Write(team.ready);
                writer.Write(team.browsingCreaturePosition);
                writer.Write(team.textHue);
            }
            //Team: End

            writer.Write((int)m_BattleFormat);
            writer.Write(m_CreaturesPerTeam);
            writer.Write(fee);
            writer.Write(wager);
            writer.Write(ticks);
            writer.Write(opportunityTicks);
            writer.Write((int)m_CurrentState);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            WaitingList = new List<PlayerMobile>();
            NeedConfirmationList = new List<PlayerMobile>();
            ReadyList = new List<PlayerMobile>();
            m_Teams = new List<Team>();

            if (version >= 0)
            {
                m_PetBattleSignupStone = reader.ReadItem() as PetBattleSignupStone;
                
                int waitingListLength = reader.ReadInt();
                for (int i = 0; i < waitingListLength; ++i)
                {
                    WaitingList.Add(reader.ReadMobile() as PlayerMobile);
                }                

                int needConfirmationListLength = reader.ReadInt();
                for (int i = 0; i < needConfirmationListLength; ++i)
                {
                    NeedConfirmationList.Add(reader.ReadMobile() as PlayerMobile);
                }

                int readyListLength = reader.ReadInt();
                for (int i = 0; i < readyListLength; ++i)
                {
                    ReadyList.Add(reader.ReadMobile() as PlayerMobile);
                }

                m_SecondsSinceStateChange = reader.ReadInt();
                m_LastStateChange = (DateTime.UtcNow).Subtract(TimeSpan.FromSeconds(m_SecondsSinceStateChange));              

                //Team: Start  
                int teams = reader.ReadInt();
                for (int a = 0; a < teams; ++a)
                {
                    Team team = new Team(reader.ReadMobile() as PlayerMobile);

                    team.m_CreatureEntries = new List<PetBattleCreatureEntry>();

                    int creatureEntries = reader.ReadInt();
                    for (int b = 0; b < creatureEntries; ++b)
                    {
                        string str = reader.ReadString();
                        Type type = Type.GetType(str);

                        PetBattleCreatureEntry creatureEntry = new PetBattleCreatureEntry(type);

                        creatureEntry.m_Level = reader.ReadInt();
                        creatureEntry.m_OffensivePower = reader.ReadInt();
                        creatureEntry.m_DefensivePower = reader.ReadInt();

                        creatureEntry.m_Experience = reader.ReadInt();
                        creatureEntry.m_Wins = reader.ReadInt();
                        creatureEntry.m_Ties = reader.ReadInt();
                        creatureEntry.m_Losses = reader.ReadInt();

                        team.m_CreatureEntries.Add(creatureEntry);
                    }

                    team.m_TeamNumber = reader.ReadInt();
                    team.m_CurrentCreaturePosition = reader.ReadInt();
                    team.selectedFormat = (PetBattleFormat)reader.ReadInt();
                    team.goldWager = reader.ReadInt();
                    team.ready = reader.ReadBool();
                    team.browsingCreaturePosition = reader.ReadInt();
                    team.textHue = reader.ReadInt();

                    m_Teams.Add(team);
                }
                //Team: End

                m_BattleFormat = (PetBattleFormat)reader.ReadInt();
                m_CreaturesPerTeam = reader.ReadInt();
                fee = reader.ReadInt();
                wager = reader.ReadInt();
                ticks = reader.ReadInt();
                opportunityTicks = reader.ReadInt();
                m_CurrentState = (PetBattleState)reader.ReadInt();
            }

            Timer.DelayCall(TimeSpan.FromSeconds(1), PostDeserializeReset);
        }

        public void PostDeserializeReset()
        {
            if (m_PetBattleSignupStone != null)
            {
                m_PetBattleSignupStone.m_PetBattle = this;

                if (m_PetBattleSignupStone.Announcer != null)
                    Announcer = m_PetBattleSignupStone.Announcer;                

                if (m_PetBattleSignupStone.m_PetBattleTotems != null)
                {
                    foreach (PetBattleTotem totem in m_PetBattleSignupStone.m_PetBattleTotems)
                    {
                        if (totem != null)
                        {
                            totem.m_PetBattleSignupStone = m_PetBattleSignupStone;

                            foreach (Team team in m_Teams)
                            {
                                if (team != null)
                                {
                                    if (team.m_Player == totem.m_Player && team.m_Player != null)
                                    {
                                        totem.m_Team = team;

                                        if (totem.m_Creature != null)
                                        {
                                            totem.m_Creature.m_PetBattle = this;
                                            totem.m_Creature.m_PetBattleTeam = team;
                                            totem.m_Creature.m_PetBattleTotem = totem;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            StartTimer();
        }
    }
}