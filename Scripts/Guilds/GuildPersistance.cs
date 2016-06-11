﻿using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Accounting;
using Server.Targeting;
using Server.Multis;

namespace Server.Items
{
    public enum GuildMemberRank
    {
        Recruit,
        Initiate,
        Veteran,
        Officer,
        Guildmaster
    }

    public static class Guilds
    {
        public static int GuildNameCharacterLimit = 35;
        public static int GuildAbbreviationCharacterLimit = 3;

        public static int GuildRegistrationFee = 50000;

        public static int GuildTextHue = 63;
        public static TimeSpan InactivityThreshold = TimeSpan.FromDays(60);

        public static int GuildGumpChangePageSound = 0x057;
        public static int GuildGumpOpenGumpSound = 0x055;

        public static string[] GuildRankNames = new string[] { "Recruit", "Initiate", "Veteran", "Officer", "Guildmaster" };

        public static GuildPersistanceItem PersistanceItem;

        public static List<Guild> m_Guilds = new List<Guild>();
        
        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new GuildPersistanceItem();               
            });
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckCreateGuildGuildSettings(player);
        }

        public static void CheckCreateGuildGuildSettings(PlayerMobile player)
        {
            if (player == null)
                return;

            if (player.m_GuildGumpSettings == null)
                player.m_GuildGumpSettings = new GuildGumpSettings(player);

            else if (player.m_GuildGumpSettings.Deleted)
                player.m_GuildGumpSettings = new GuildGumpSettings(player);
        }

        public static List<GuildGumpPageType> GetGuildPageTypeList(PlayerMobile player)
        {
            List<GuildGumpPageType> guildPageTypes = new List<GuildGumpPageType>();

            if (player == null)
                return guildPageTypes;

            CheckCreateGuildGuildSettings(player);

            if (player.Guild == null)
            {
                guildPageTypes.Add(GuildGumpPageType.CreateGuild);
                guildPageTypes.Add(GuildGumpPageType.Invitations);
            }

            else
            {
                guildPageTypes.Add(GuildGumpPageType.Overview);
                guildPageTypes.Add(GuildGumpPageType.Members);
                guildPageTypes.Add(GuildGumpPageType.Candidates);
                guildPageTypes.Add(GuildGumpPageType.Diplomacy);
                guildPageTypes.Add(GuildGumpPageType.Faction);
            }            

            return guildPageTypes;
        }

        public static void GuildGumpCheckGuild(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateGuildGuildSettings(player);

            if (player.Guild != null)
            {
                if (player.m_GuildGumpSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild || player.m_GuildGumpSettings.m_GuildGumpPage == GuildGumpPageType.Invitations)
                    player.m_GuildGumpSettings.m_GuildGumpPage = GuildGumpPageType.Overview;
            }

            else
            {
                if (!(player.m_GuildGumpSettings.m_GuildGumpPage == GuildGumpPageType.CreateGuild || player.m_GuildGumpSettings.m_GuildGumpPage == GuildGumpPageType.Invitations))
                    player.m_GuildGumpSettings.m_GuildGumpPage = GuildGumpPageType.CreateGuild;
            }
        }

        public static void SendGuildGump(PlayerMobile player)
        {
            if (player == null)
                return;

            CheckCreateGuildGuildSettings(player);

            GuildGumpCheckGuild(player);
            
            List<GuildGumpPageType> m_GuildTabs = Guilds.GetGuildPageTypeList(player);

            if (m_GuildTabs.Count == 0)
                return;

            int GuildTabsPerPage = 4;
            int TotalGuildTabs = m_GuildTabs.Count;
            int TotalGuildTabPages = (int)(Math.Ceiling((double)TotalGuildTabs / (double)GuildTabsPerPage));

            if (!m_GuildTabs.Contains(player.m_GuildGumpSettings.m_GuildGumpPage))
                player.m_GuildGumpSettings.m_GuildGumpPage = m_GuildTabs[0];

            //Find Last Selected Guild Tab and Navigate to Page
            int m_GuildTabIndex = m_GuildTabs.IndexOf(player.m_GuildGumpSettings.m_GuildGumpPage);
            int m_GuildTabPage = (int)(Math.Floor((double)m_GuildTabIndex / (double)GuildTabsPerPage));

            CloseAllGuildGumps(player);

            player.SendSound(GuildGumpOpenGumpSound);
                                    
            switch (player.m_GuildGumpSettings.m_GuildGumpPage)
            {
                case GuildGumpPageType.CreateGuild: player.SendGump(new CreateGuildGump(player, m_GuildTabPage)); break;
                case GuildGumpPageType.Invitations: player.SendGump(new GuildInvitationsGump(player, m_GuildTabPage)); break;

                case GuildGumpPageType.Overview: player.SendGump(new GuildOverviewGump(player, m_GuildTabPage)); break;
                case GuildGumpPageType.Members: player.SendGump(new GuildMembersGump(player, m_GuildTabPage)); break;
                case GuildGumpPageType.Candidates: player.SendGump(new GuildCandidatesGump(player, m_GuildTabPage)); break;
                case GuildGumpPageType.Diplomacy: player.SendGump(new GuildDiplomacyGump(player, m_GuildTabPage)); break;
            }
        }

        public static void CloseAllGuildGumps(PlayerMobile player)
        {
            if (player.HasGump(typeof(CreateGuildGump)))
                player.CloseGump(typeof(CreateGuildGump));

            if (player.HasGump(typeof(GuildInvitationsGump)))
                player.CloseGump(typeof(GuildInvitationsGump));
           
            if (player.HasGump(typeof(GuildOverviewGump)))
                player.CloseGump(typeof(GuildOverviewGump));           

            if (player.HasGump(typeof(GuildMembersGump)))
                player.CloseGump(typeof(GuildMembersGump));

            if (player.HasGump(typeof(GuildCandidatesGump)))
                player.CloseGump(typeof(GuildCandidatesGump));

            if (player.HasGump(typeof(GuildDiplomacyGump)))
                player.CloseGump(typeof(GuildDiplomacyGump));
        }

        public static void StandaloneGuildGump(PlayerMobile player, Guild guild)
        {
            if (player == null) return;
            if (guild == null) return;
        }

        public static bool GuildNameExists(string guildName)
        {
            foreach (Guild guild in m_Guilds)
            {
                if (guild == null) continue;
                if (guild.Name.ToLower() == guildName.ToLower())
                    return true;
            }

            return false;
        }

        public static bool GuildAbbreviationExists(string guildAbbreviation)
        {
            foreach (Guild guild in m_Guilds)
            {
                if (guild == null) continue;
                if (guild.m_Abbreviation.ToLower() == guildAbbreviation.ToLower())
                    return true;
            }

            return false;
        }

        public static bool CheckProfanity(string s)
        {
            return CheckProfanity(s, 50);
        }

        public static bool CheckProfanity(string s, int maxLength)
        {
            if (s.Length < 1 || s.Length > maxLength)
                return false;

            char[] exceptions = ProfanityProtection.Exceptions;

            s = s.ToLower();

            for (int i = 0; i < s.Length; ++i)
            {
                char c = s[i];

                if ((c < 'a' || c > 'z') && (c < '0' || c > '9'))
                {
                    bool except = false;

                    for (int j = 0; !except && j < exceptions.Length; j++)
                        if (c == exceptions[j])
                            except = true;

                    if (!except)
                        return false;
                }
            }

            string[] disallowed = ProfanityProtection.Disallowed;

            for (int i = 0; i < disallowed.Length; i++)
            {
                if (s.IndexOf(disallowed[i]) != -1)
                    return false;
            }

            return true;
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
           
            //Version 0
            if (version >= 0)
            {
            }
        }
    }  

    public class GuildPersistanceItem : Item
    {
        public override string DefaultName { get { return "GuildPersistance"; } }

        public GuildPersistanceItem(): base(0x0)
        {
            Movable = false;
        }

        public GuildPersistanceItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            Guilds.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Movable = false;

            Guilds.PersistanceItem = this;
            Guilds.Deserialize(reader);
        }
    }

    public class GuildGumpSettings: Item
    {
        public PlayerMobile m_Player;

        public GuildGumpPageType m_GuildGumpPage = GuildGumpPageType.CreateGuild;
        public bool m_ShowGuildTitle = true;

        [Constructable]
        public GuildGumpSettings(PlayerMobile player): base(0x0)
        {
            Visible = false;
            Movable = false;

            m_Player = player;

            if (player == null)
                Delete();

            else
                m_Player.m_GuildGumpSettings = this;
        }

        public GuildGumpSettings(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Player);
            writer.Write((int)m_GuildGumpPage);
            writer.Write(m_ShowGuildTitle);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = reader.ReadMobile() as PlayerMobile;
                m_GuildGumpPage = (GuildGumpPageType)reader.ReadInt();
                m_ShowGuildTitle = reader.ReadBool();
            }

            if (m_Player == null)
                Delete();

            else
                m_Player.m_GuildGumpSettings = this;
        }
    }

    public class GuildMemberEntry
    {
        public PlayerMobile m_Player;
        public GuildMemberRank m_Rank = GuildMemberRank.Recruit;
        public DateTime m_JoinDate = DateTime.UtcNow;
        public PlayerMobile m_DeclaredFealty;

        public GuildMemberEntry(PlayerMobile player, GuildMemberRank rank, DateTime joinDate, PlayerMobile declaredFealty)
        {
            m_Player = player;
            m_Rank = m_Rank;
            m_JoinDate = joinDate;
            m_DeclaredFealty = declaredFealty;
        }
    }

    public class FealtyVote
    {
        public string m_AccountName = "";
        public DateTime m_LastOnline;
        public PlayerMobile m_FealtyTarget;
        public bool m_Active;

        public FealtyVote(string accountName, DateTime lastOnline, PlayerMobile fealtyTarget, bool active)
        {
            m_AccountName = accountName;
            m_LastOnline = lastOnline;
            m_FealtyTarget = fealtyTarget;
            m_Active = active;
        }
    }

    public class Guild : Item
    {
        public PlayerMobile m_Guildmaster;

        public string m_Abbreviation = "";
        public DateTime m_CreationTime;

        public int m_Icon = 4014;
        public int m_IconHue = 0;

        public string m_Website = "http://www.outlandsuo.com/index.html";

        public BaseHouse m_Guildhouse;

        public Faction m_Faction = null;

        public string[] m_RankNames = new string[] { Guilds.GuildRankNames[0], Guilds.GuildRankNames[1], Guilds.GuildRankNames[2], Guilds.GuildRankNames[3], Guilds.GuildRankNames[4] };

        public List<GuildMemberEntry> m_Members = new List<GuildMemberEntry>(); 
       
        public List<Guild> m_Allies = new List<Guild>();
        public List<Guild> m_AlliesRequestsSent = new List<Guild>();
        public List<Guild> m_AlliesRequestsReceived = new List<Guild>();

        public List<Guild> m_Enemies = new List<Guild>();
        public List<Guild> m_EnemiesRequestsSent = new List<Guild>();
        public List<Guild> m_EnemiesRequestsReceived = new List<Guild>();

        public List<PlayerMobile> m_Candidates = new List<PlayerMobile>();    

        [Constructable]
        public Guild(string name, string abbreviation): base(0x0)
        {
            Visible = false;
            Movable = false;

            Name = name;
            m_Abbreviation = abbreviation;

            m_CreationTime = DateTime.UtcNow;

            Guilds.m_Guilds.Add(this);
        }

        public Guild(Serial serial): base(serial)
        {
        }

        public string GetRankName(GuildMemberRank rank)
        {
            string rankName = "";

            int rankIndex = (int)rank;

            if (rankIndex < m_RankNames.Length)
                rankName = m_RankNames[rankIndex];

            return rankName;
        }

        public int GetRankHue(GuildMemberRank rank)
        {
            int rankHue = 0;

            switch (rank)
            {
                case GuildMemberRank.Recruit: rankHue = 2655; break;
                case GuildMemberRank.Initiate: rankHue = 2599; break;
                case GuildMemberRank.Veteran: rankHue = 169; break;
                case GuildMemberRank.Officer: rankHue = 2603; break;
                case GuildMemberRank.Guildmaster: rankHue = 1259; break;
            }

            return rankHue;
        }

        public bool CanAddCandidates(GuildMemberRank rank)
        {
            switch (rank)
            {
                case GuildMemberRank.Recruit: return false;
                case GuildMemberRank.Initiate: return true;
                case GuildMemberRank.Veteran: return true;
                case GuildMemberRank.Officer: return true;
                case GuildMemberRank.Guildmaster: return true;
            }

            return false;
        }

        public bool CanApproveCandidates(GuildMemberRank rank)
        {
            switch (rank)
            {
                case GuildMemberRank.Recruit: return false;
                case GuildMemberRank.Initiate: return false;
                case GuildMemberRank.Veteran: return false;
                case GuildMemberRank.Officer: return true;
                case GuildMemberRank.Guildmaster: return true;
            }

            return false;
        }

        public void GuildAnnouncement(string message, List<PlayerMobile> ignoredPlayers, GuildMemberRank minimumGuildRank)
        {
            List<GuildMemberEntry> memberEntries = GetGuildMemberEntries();

            foreach (GuildMemberEntry entry in memberEntries)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;
                if (ignoredPlayers.Contains(entry.m_Player)) continue;
                if (((int)entry.m_Rank < (int)minimumGuildRank)) continue;

                entry.m_Player.SendMessage(Guilds.GuildTextHue, message);                
            }
        }

        public bool IsMember(PlayerMobile player)
        {
            if (player == null)
                return false;

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry.m_Player == player)
                    return true;
            }

            return false;
        }

        public bool IsActive(PlayerMobile player)
        {
            if (player == null)
                return false;

            if (IsMember(player))
            {
                if (player.LastOnline + Guilds.InactivityThreshold >= DateTime.UtcNow)
                    return true;
            }

            return false;
        }

        public List<PlayerMobile> GetGuildMembers()
        {
            List<PlayerMobile> members = new List<PlayerMobile>();

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry == null) continue;
                if (entry.m_Player == null) continue;
                if (entry.m_Player.Deleted) continue;

                members.Add(entry.m_Player);
            }

            return members;
        }

        public GuildMemberEntry GetGuildMemberEntry(PlayerMobile player)
        {
            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry.m_Player == player)
                    return entry;
            }

            return null;
        }

        public List<GuildMemberEntry> GetGuildMemberEntries()
        {
            List<GuildMemberEntry> memberEntries = new List<GuildMemberEntry>();

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry == null)
                    continue;

                memberEntries.Add(entry);
            }

            return memberEntries;
        }

        public bool IsAllied(Guild guild)
        {
            foreach (Guild otherGuild in m_Allies)
            {
                if (guild == otherGuild)
                    return true;
            }

            return false;
        }

        public bool IsEnemy(Guild guild)
        {
            foreach (Guild otherGuild in m_Enemies)
            {
                if (guild == otherGuild)
                    return true;
            }

            return false;
        }

        public bool AddMember(PlayerMobile player)
        {
            if (player == null)
                return false;

            if (m_Candidates.Contains(player))
                m_Candidates.Remove(player);

            if (IsMember(player))
                return false;

            GuildMemberEntry entry = new GuildMemberEntry(player, GuildMemberRank.Recruit, DateTime.UtcNow, m_Guildmaster);

            player.Guild = this;
            player.m_GuildMemberEntry = entry;

            m_Members.Add(entry);           

            return true;
        }

        public void DismissMember(PlayerMobile player, bool forced, bool announce)
        {
            if (!IsMember(player))
                return;

            GuildMemberEntry entry = GetGuildMemberEntry(player);

            if (entry == null)
                return;

            bool wasGuildmaster = (entry.m_Rank == GuildMemberRank.Guildmaster);

            if (m_Members.Contains(entry))
                m_Members.Remove(entry);
            
            string guildText = Name + " [" + m_Abbreviation + "]";

            if (player != null)
            {
                player.Guild = null;
                player.m_GuildMemberEntry = null;

                if (forced)
                    player.SendMessage(Guilds.GuildTextHue, "You have been removed from " + guildText + ".");

                else
                    player.SendMessage(Guilds.GuildTextHue, "You leave the guild.");

                if (announce)
                {
                    if (forced)
                        GuildAnnouncement(player.RawName + " has been removed from the guild.", new List<PlayerMobile> { }, GuildMemberRank.Recruit);

                    else
                        GuildAnnouncement(player.RawName + " has left the guild.", new List<PlayerMobile> { }, GuildMemberRank.Recruit);
                }
            } 

            if (wasGuildmaster)            
                AssignNewGuildmaster(player);            
        }

        public void AssignNewGuildmaster(PlayerMobile previousGuildmaster)
        {
            string guildText = Name + " [" + m_Abbreviation + "]";

            List<PlayerMobile> members = GetGuildMembers();

            if (members.Count == 0)
            {
                if (previousGuildmaster != null)
                    previousGuildmaster.SendMessage(Guilds.GuildTextHue, guildText + " has disbanded.");

                DisbandGuild();

                return;
            }

            else
            {
                List<GuildMemberEntry> memberEntries = GetGuildMemberEntries();

                List<FealtyVote> fealtyVotes = new List<FealtyVote>();
                List<FealtyVote> inactiveMemberfealtyVotes = new List<FealtyVote>();

                foreach (GuildMemberEntry entry in memberEntries)
                {
                    if (entry == null) continue;
                    if (entry.m_Player == null) continue;
                    if (entry.m_Player.Deleted) continue;
                    if (!IsMember(entry.m_DeclaredFealty)) continue;

                    Account account = entry.m_Player.Account as Account;

                    if (account == null)
                        continue;

                    bool isActiveMember = IsActive(entry.m_Player);

                    //TEST: FINISH!!!

                    if (isActiveMember)
                    {
                        bool foundAccountVote = false;

                        FealtyVote fealtyVote = new FealtyVote(account.Username, entry.m_Player.LastOnline, entry.m_DeclaredFealty, isActiveMember);      
                    }

                    else
                    {
                        bool foundAccountVote = false;

                        FealtyVote fealtyVote = new FealtyVote(account.Username, entry.m_Player.LastOnline, entry.m_DeclaredFealty, isActiveMember);      
                    }                                 
                }
            }
        }


        public void DisbandGuild()
        {
            string guildText = Name + " [" + m_Abbreviation + "]";

            GuildAnnouncement(guildText + " has disbanded.", new List<PlayerMobile> { }, GuildMemberRank.Recruit);

            Delete();
        }

        public void AuditGuild()
        {
            List<PlayerMobile> members = GetGuildMembers();

            if (members.Count == 0)
                DisbandGuild();

            else if (m_Guildmaster == null)
                AssignNewGuildmaster(m_Guildmaster);

            else if (m_Guildmaster.Deleted)
                AssignNewGuildmaster(m_Guildmaster);
        }

        public void OnFealtyChange()
        {
        }

        public void OnPlayerDeleted(PlayerMobile player)
        {
            if (player == null)
                return;

            DismissMember(player, false, true);
        }        

        public void RecruitMember(PlayerMobile player, PlayerMobile targetPlayer)
        {
            if (!IsMember(player))
            {
                player.SendMessage("You are not a member of this guild.");
                return;
            }

            GuildMemberEntry playerEntry = GetGuildMemberEntry(player);

            if (playerEntry == null)
                return;

            if (CanApproveCandidates(playerEntry.m_Rank))
            {
                player.SendMessage("Which player do you wish to add to the guild?");
                player.Target = new RecruitMemberTarget(player, this);
            }

            else if (CanAddCandidates(playerEntry.m_Rank))
            {
                player.SendMessage("Which player do you wish to nominate as a candidate for guild membership?");
                player.Target = new RecruitMemberTarget(player, this);
            }

            else
            {
                player.SendMessage("You do not have a high enough rank in this guild to recruit new members.");
                return;
            }
        }

        private class RecruitMemberTarget : Target
        {           
            public PlayerMobile m_Player;
            public Guild m_Guild; 

            public RecruitMemberTarget(PlayerMobile player, Guild guild): base(100, false, TargetFlags.None)
            {               
                m_Player = player;
                m_Guild = guild;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Player == null) return;
                if (m_Player.Deleted) return;

                if (m_Guild == null)
                {
                    m_Player.SendMessage("That guild no longer exists.");
                    return;
                }                

                if (!m_Guild.IsMember(m_Player))
                {
                    m_Player.SendMessage("You are not a member of this guild.");
                    return;
                }

                GuildMemberEntry playerEntry = m_Guild.GetGuildMemberEntry(m_Player);

                if (playerEntry == null)
                    return;

                bool canImmediatelyAdd = m_Guild.CanAddCandidates(playerEntry.m_Rank);

                PlayerMobile playerTarget = target as PlayerMobile;

                if (playerTarget == null)
                {
                    m_Player.SendMessage("That is not a player.");
                    return;
                }

                if (Utility.GetDistance(m_Player.Location, playerTarget.Location) > 20)
                {
                    m_Player.SendMessage("That player is too far away.");
                    return;
                }

                if (playerTarget.Guild != null)
                {
                    m_Player.SendMessage("That player is already in a guild.");
                    return;
                }

                if (canImmediatelyAdd)
                {
                    if (m_Guild.AddMember(playerTarget))
                    {
                        string guildText = m_Guild.Name + " [" + m_Guild.m_Abbreviation + "]";

                        m_Guild.GuildAnnouncement(playerTarget.RawName + " has been made a member of the guild.", new List<PlayerMobile>() { playerTarget }, GuildMemberRank.Recruit);  

                        playerTarget.SendMessage(Guilds.GuildTextHue, "You have been made a guild member of " + guildText + ".");

                        return;
                    }

                    else
                    {
                        m_Player.SendMessage("That player is already a member of this guild or unable to join a guild at this time.");

                        return;
                    }
                }

                else
                {
                    if (m_Guild.m_Candidates.Contains(playerTarget))
                    {
                        m_Player.SendMessage("That player is already a candidate for membership in this guild.");
                        return;
                    }

                    else
                    {
                        m_Guild.m_Candidates.Add(playerTarget);

                        m_Player.SendMessage(Guilds.GuildTextHue, playerTarget.RawName + " has been made a candidate for guild membership." );
                        playerTarget.SendMessage(Guilds.GuildTextHue, "You have been made a candidate for guild membership.");

                        string message = playerTarget.RawName + " has been made a candidate for membership in the guild.";

                        m_Guild.GuildAnnouncement(message, new List<PlayerMobile>() { m_Player, playerTarget }, GuildMemberRank.Officer);  

                        return;
                    }
                }                
            }
        }

        public override void OnDelete()
        {
            Queue m_Queue = new Queue();

            foreach (GuildMemberEntry entry in m_Members)
            {
                if (entry.m_Player == null) continue;

                entry.m_Player.Guild = null;
                entry.m_Player.m_GuildMemberEntry = null;

                m_Queue.Enqueue(entry);
            }

            while (m_Queue.Count > 0)
            {
                GuildMemberEntry entry = (GuildMemberEntry)m_Queue.Dequeue();

                if (m_Members.Contains(entry))
                    m_Members.Remove(entry);
            }

            foreach (Guild guild in Guilds.m_Guilds)
            {
                if (guild == null) continue;
                if (guild.Deleted) continue;

                if (guild.m_Allies.Contains(this))
                    guild.m_Allies.Remove(this);

                if (guild.m_AlliesRequestsSent.Contains(this))
                    guild.m_AlliesRequestsSent.Remove(this);

                if (guild.m_AlliesRequestsReceived.Contains(this))
                    guild.m_AlliesRequestsReceived.Remove(this);

                if (guild.m_Enemies.Contains(this))
                    guild.m_Enemies.Remove(this);

                if (guild.m_EnemiesRequestsSent.Contains(this))
                    guild.m_EnemiesRequestsSent.Remove(this);

                if (guild.m_EnemiesRequestsReceived.Contains(this))
                    guild.m_EnemiesRequestsReceived.Remove(this);
            }

            if (Guilds.m_Guilds.Contains(this))
                Guilds.m_Guilds.Remove(this);

            base.OnDelete();
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_Abbreviation);
            writer.Write(m_CreationTime);
            writer.Write(m_Icon);
            writer.Write(m_IconHue);
            writer.Write(m_Website);
            writer.Write(m_Guildhouse);
            writer.Write(m_Faction);

            //Rank Names
            writer.Write(m_RankNames.Length);
            for (int a = 0; a < m_RankNames.Length; a++)
            {
                writer.Write(m_RankNames[a]);
            }

            //Members
            writer.Write(m_Members.Count);
            for (int a = 0; a < m_Members.Count; a++)
            {
                writer.Write(m_Members[a].m_Player);
                writer.Write((int)m_Members[a].m_Rank);
                writer.Write(m_Members[a].m_JoinDate);
                writer.Write(m_Members[a].m_DeclaredFealty);
            }

            //Lists
            writer.Write(m_Allies.Count);
            for (int a = 0; a < m_Allies.Count; a++)
            {
                writer.Write(m_Allies[a]);
            }

            writer.Write(m_AlliesRequestsSent.Count);
            for (int a = 0; a < m_AlliesRequestsSent.Count; a++)
            {
                writer.Write(m_AlliesRequestsSent[a]);
            }

            writer.Write(m_AlliesRequestsReceived.Count);
            for (int a = 0; a < m_AlliesRequestsReceived.Count; a++)
            {
                writer.Write(m_AlliesRequestsReceived[a]);
            }

            writer.Write(m_Enemies.Count);
            for (int a = 0; a < m_Enemies.Count; a++)
            {
                writer.Write(m_Enemies[a]);
            }

            writer.Write(m_EnemiesRequestsSent.Count);
            for (int a = 0; a < m_EnemiesRequestsSent.Count; a++)
            {
                writer.Write(m_EnemiesRequestsSent[a]);
            }

            writer.Write(m_EnemiesRequestsReceived.Count);
            for (int a = 0; a < m_EnemiesRequestsReceived.Count; a++)
            {
                writer.Write(m_EnemiesRequestsReceived[a]);
            }

            writer.Write(m_Candidates.Count);
            for (int a = 0; a < m_Candidates.Count; a++)
            {
                writer.Write(m_Candidates[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Abbreviation = reader.ReadString();
                m_CreationTime = reader.ReadDateTime();
                m_Icon = reader.ReadInt();
                m_IconHue = reader.ReadInt();
                m_Website = reader.ReadString();
                m_Guildhouse = reader.ReadItem() as BaseHouse;
                m_Faction = reader.ReadItem() as Faction;

                //Rank Names
                int rankNamesCount = reader.ReadInt();
                for (int a = 0; a < rankNamesCount; a++)
                {
                    m_RankNames[a] = reader.ReadString();
                }
                
                //Members
                int membersCount = reader.ReadInt();
                for (int a = 0; a < membersCount; a++)
                {
                    PlayerMobile player = reader.ReadMobile() as PlayerMobile;
                    GuildMemberRank rank = (GuildMemberRank)reader.ReadInt();
                    DateTime joinDate = reader.ReadDateTime();
                    PlayerMobile fealtyPlayer = reader.ReadMobile() as PlayerMobile;

                    if (player != null)
                    {
                        if (!player.Deleted)
                        {
                            GuildMemberEntry guildMemberEntry = new GuildMemberEntry(player, rank, joinDate, fealtyPlayer);

                            player.m_GuildMemberEntry = guildMemberEntry;

                            m_Members.Add(guildMemberEntry);
                        }
                    }                    
                }

                //Lists
                int alliesCount = reader.ReadInt();
                for (int a = 0; a < alliesCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;

                    if (entry != null)
                    {
                        if (!entry.Deleted)
                            m_Allies.Add(entry);
                    }
                }

                int alliesRequestsSentCount = reader.ReadInt();
                for (int a = 0; a < alliesRequestsSentCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;                   

                    if (entry != null)
                    {
                        if (!entry.Deleted)
                            m_AlliesRequestsSent.Add(entry);
                    }
                }

                int alliesRequestsReceivedCount = reader.ReadInt();
                for (int a = 0; a < alliesRequestsReceivedCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;

                    if (entry != null)
                    {
                        if (!entry.Deleted)
                            m_AlliesRequestsReceived.Add(entry);
                    }
                }

                int enemiesCount = reader.ReadInt();
                for (int a = 0; a < enemiesCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;

                    if (entry != null)
                    {
                        if (!entry.Deleted)
                            m_Enemies.Add(entry);
                    }
                }

                int enemiesRequestsSentCount = reader.ReadInt();
                for (int a = 0; a < enemiesRequestsSentCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;

                    if (entry != null)
                    {
                        if (!entry.Deleted)
                            m_EnemiesRequestsSent.Add(entry);
                    }
                }

                int enemiesRequestsReceivedCount = reader.ReadInt();
                for (int a = 0; a < enemiesRequestsReceivedCount; a++)
                {
                    Guild entry = reader.ReadItem() as Guild;

                    if (entry != null)
                    {
                        if (!entry.Deleted)
                            m_EnemiesRequestsReceived.Add(entry);
                    }
                }

                int candidatesCount = reader.ReadInt();
                for (int a = 0; a < candidatesCount; a++)
                {
                    PlayerMobile candidate = reader.ReadMobile() as PlayerMobile;

                    if (candidate != null)
                    {
                        if (!candidate.Deleted)
                            m_Candidates.Add(candidate);
                    }
                }
            }

            //-----

            Guilds.m_Guilds.Add(this);

            AuditGuild();
        }
    }
}
