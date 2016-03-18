using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Targeting;

namespace Server.ArenaSystem
{
    public class ArenaTeamCharter : Item
    {
        public static string s_defaultName = "Unnamed Team";
        public static readonly int s_gid = 0x1F23;
        public static readonly int s_capacityMax = 3;
        public string TeamName { get; set;}
        public List<PlayerMobile> m_members;
		public int Capacity { get { return m_members.Count; } }

		// Safety management for pending invitation gumps
		public List<PlayerMobile> m_inviteGumpHolders = new List<PlayerMobile>();

        // To prevent any bugs that may arise due to players having
        // an invitation gump open, [Submitted] prevents any additions to the
        // charter once the team has been officially created.
        public bool Submitted { get; set; }
        public Serial OwnerSerial { get; private set; }

        [Constructable]
        public ArenaTeamCharter()
            : base(s_gid)
        {
            OwnerSerial = 0;
            LootType = LootType.Blessed;
            TeamName = s_defaultName;
            Submitted = false;
            m_members = new List<PlayerMobile>();
        }
        public ArenaTeamCharter(Serial serial)
            : base(serial)
        {
            LootType = LootType.Blessed;
            m_members = new List<PlayerMobile>();
			TeamName = s_defaultName;
			Submitted = false;
        }
        public override string DefaultName
        {
            get
            {
                return String.Format("A {0} arena team charter", Submitted ? "used" : "unused");
            }
        }

		public override void Serialize(GenericWriter writer)
		{
			/* OLD LEGACY TO NOT BREAK BINARY COMPATIBILITY */
			writer.Write((int)0);
			writer.Write("");
			writer.Write((bool)true);
			writer.Write((int)0);
			writer.Write(OwnerSerial);
			base.Serialize(writer);
		}
		public override void Deserialize(GenericReader reader)
		{
			/* OLD LEGACY TO NOT BREAK BINARY COMPATIBILITY */
			/*int version =*/ reader.ReadInt();
			/*TeamName =*/ reader.ReadString();
			/*Submitted =*/ reader.ReadBool();
			int membersCount = reader.ReadInt();
			for (int i = 0; i < membersCount; ++i)
			{
				/*Serial ser = */reader.ReadInt();
				//PlayerMobile player = (PlayerMobile)World.FindMobile(ser);
				//if (player != null)
				//{
				//    m_members.Add(player);
				//}
			}
			/*OwnerSerial = */reader.ReadInt();

			base.Deserialize(reader);
		}

		public override void OnDelete()
		{
			CloseInviteGumps();
			base.OnDelete();
		}

		public void CloseInviteGumps()
		{
			// close all other gumps immediately.
			foreach (PlayerMobile pm in m_inviteGumpHolders)
			{
				if (pm.HasGump(typeof(ArenaTeamCharterGump.CharterInvitationGump)))
				{
					pm.CloseGump(typeof(ArenaTeamCharterGump.CharterInvitationGump));
					pm.SendMessage("The invitation was withdrawn");
				}
			}
			m_inviteGumpHolders.Clear();
		}

        public bool AddPlayer(PlayerMobile player)
        {
            if (Submitted)
                return false;

            if (m_members.Count < s_capacityMax)
            {
                m_members.Add(player);
                return true;
            }
            return false;
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (OwnerSerial == 0)
            {
                // Owner must be on the charter.
                AddPlayer((PlayerMobile)from);
                OwnerSerial = from.Serial;
            }

            from.SendGump(new ArenaTeamCharterGump(this));
        }

        // Guards
        private void CloseGumps(Mobile from)
        {
            if (from.HasGump(typeof(ArenaTeamCharterGump)))
            {
                from.CloseGump(typeof(ArenaTeamCharterGump));
            }
			CloseInviteGumps();
        }
        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            CloseGumps(from);
            return base.OnDroppedToWorld(from, p);
        }
        public override bool OnDroppedInto(Mobile from, Items.Container target, Point3D p)
        {
            CloseGumps(from);
            return base.OnDroppedInto(from, target, p);
        }
        public override bool OnDroppedOnto(Mobile from, Item target, Point3D p)
        {
            CloseGumps(from);
            return base.OnDroppedOnto(from, target, p);
        }
        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            CloseGumps(from);
            return base.OnDroppedToMobile(from, target);
        }
    }

    class ArenaTeamCharterGump : Gump
    {
        private ArenaTeamCharter m_charter;

        enum EGumpActions
        {
            eGA_CloseCharter = 0,
            eGA_AcceptCharter,
            eGA_AddPlayer = 10,
            eGA_RemovePlayer = 13,
        }

        public ArenaTeamCharterGump(ArenaTeamCharter charter)
            : base(100, 100)
        {
            m_charter = charter;
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);
            AddBackground(14, 9, 513, 241, 83);
            string header = "This charter is the first step towards creating an arena team. To change the name of your team, enter a new name, and press the button to its left. To send players an invitation to your team, select the button to the left of an \"Open\" slot and target the invitee. You can have up to 3 members in your team, and can be in no more than 1 1v1, 2v2, or 3v3 team. Once you're committed to the team composition, select [Okay], and the team will be entered into the Arena system!";
            AddHtml(272, 33, 224, 188, header, true, true);

            // Team name
            AddLabel(42, 27, 0xFF, "Team Name");
            AddBackground(36, 47, 218, 35, 2620);
            if (!charter.Submitted)
            {
                AddTextEntry(75, 54, 170, 20, 0xFF, 0, m_charter.TeamName);
            }
            else
            {
                AddLabel(75, 54, 0xFF, m_charter.TeamName);
            }
            

            // Team composition
            AddLabel(39, 85, 0xFF, "Team Composition");
            AddBackground(36, 104, 218, 79, 2620);

            int index = 0;
            foreach (PlayerMobile pm in m_charter.m_members)
            {
                AddLabel(76, 112 + index * 22, 0xFF, pm.Name);
                if (!charter.Submitted && pm.Serial != m_charter.OwnerSerial)
                {
                    AddButton(40, 110 + index * 22, 4017, 4019, (int)EGumpActions.eGA_RemovePlayer + index, GumpButtonType.Reply, 0);
                }
                ++index;
            }

            if (!charter.Submitted && index < 3)
            {
                AddLabel(76, 112 + index * 22, 0xFF, "Open");
                AddButton(40, 110 + index * 22, 4008, 4010, (int)EGumpActions.eGA_AddPlayer, GumpButtonType.Reply, 0);
            }
			// Okay/Cancel
			AddButton(85, 197, 242, 241, (int)EGumpActions.eGA_CloseCharter, GumpButtonType.Reply, 0);
			AddButton(153, 197, 247, 248, (int)EGumpActions.eGA_AcceptCharter, GumpButtonType.Reply, 0);
        }
        public override void OnResponse(NetState sender, RelayInfo info)
        {
			switch (info.ButtonID)
            {
                case (int)EGumpActions.eGA_AcceptCharter:
                    {
						if (m_charter.m_members.Count == 1)
						{
							m_charter.TeamName = sender.Mobile.Name;
							sender.Mobile.SendMessage("Your 1v1 team will be named after you.");
						}
						else
						{
							TextRelay teamname_entry = info.GetTextEntry(0);
							if (teamname_entry != null && teamname_entry.Text.Length < 17)
							{
								string text = (teamname_entry == null ? m_charter.TeamName : teamname_entry.Text.Trim());
								m_charter.TeamName = text;
							}
							else
							{
								sender.Mobile.SendMessage("The team name must be less than {0} characters.", 17);
								sender.Mobile.SendGump(new ArenaTeamCharterGump(m_charter));
							}
						}

						// Validate the charter.
						if (ArenaSystem.SubmitTeam(sender.Mobile, m_charter))
						{
							m_charter.CloseInviteGumps();
							m_charter.Submitted = true;
							m_charter.Hue = 0x8AD;
						}
						else
						{
							sender.Mobile.SendGump(new ArenaTeamCharterGump(m_charter));
						}
                        break;
                    }
                case (int)EGumpActions.eGA_AddPlayer:
                    {
                        sender.Mobile.Target = new InvitePlayerToCharterTarget(m_charter);
                        break;
                    }
                case (int)EGumpActions.eGA_RemovePlayer:
                    {
                        if (m_charter.m_members.Count > 0)
                            m_charter.m_members.RemoveAt(0);

                        sender.Mobile.SendGump(new ArenaTeamCharterGump(m_charter));
                        break;
                    }
                case (int)EGumpActions.eGA_RemovePlayer + 1:
                    {
                        if (m_charter.m_members.Count > 1)
                            m_charter.m_members.RemoveAt(1);

                        sender.Mobile.SendGump(new ArenaTeamCharterGump(m_charter));
                        break;
                    }
                case (int)EGumpActions.eGA_RemovePlayer + 2:
                    {
                        if (m_charter.m_members.Count > 2)
                            m_charter.m_members.RemoveAt(2);

                        sender.Mobile.SendGump(new ArenaTeamCharterGump(m_charter));
                        break;
                    }
            }
        }

        public class InvitePlayerToCharterTarget : Target
        {
            private ArenaTeamCharter m_charter;

            public InvitePlayerToCharterTarget(ArenaTeamCharter charter)
                : base(-1, false, TargetFlags.None)
            {
                m_charter = charter;
            }
            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile target = targeted as PlayerMobile;
                if (target == null)
                {
                    from.SendMessage("You must select a player.");
                    return;
                }

                // Prevent duplicate entries
                PlayerMobile charteredPlayer = m_charter.m_members.Find(x => x.Name == target.Name);
                if (charteredPlayer != null)
                {
                    from.SendMessage("{0} is already a member of the charter.", target.Name);
                    return;
                }

                if (m_charter.RootParent == target)
                {
                    if (m_charter.AddPlayer((PlayerMobile)from))
                    {
                        from.SendMessage("You have been added to the charter for {0}.", m_charter.TeamName);
                    }
                }
                else
                {
                    if (target.HasGump(typeof(CharterInvitationGump)))
                    {
                        from.SendMessage("This player is already considering an invitation.");
                    }
                    else
                    {
						m_charter.m_inviteGumpHolders.Add(target);
                        target.SendGump(new CharterInvitationGump(from, m_charter));
                        from.SendMessage("An invitation has been sent to {0}.", target.Name);
                    }
                    
                }

                from.SendGump(new ArenaTeamCharterGump(m_charter));
            }
        }

        public class CharterInvitationGump : Gump
        {
            private Mobile m_inviter;
            private ArenaTeamCharter m_charter;

            enum EGumpActions
            {
                eGA_CancelInvitation = 0,
                eGA_AcceptInvitation,
            }
            public CharterInvitationGump(Mobile inviter, ArenaTeamCharter charter)
                : base(100, 100)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                m_inviter = inviter;
                m_charter = charter;

                AddPage(0);
                AddBackground(14, 10, 513, 215, 83);

                // Header w/icon
                AddImage(37, 30, 21507);
                string header = string.Format("{0} has invited you to join {1}. Would you like to join this team?", m_inviter.Name, m_charter.TeamName);
                AddHtml(113, 30, 393, 68, header, true, false);

                // Team composition
                AddBackground(42, 121, 38, 79, 2620);
                AddBackground(42, 121, 218, 79, 2620);
                AddLabel(45, 102, 0xFF, "Team Composition");
                List<PlayerMobile> memberList = m_charter.m_members;
                int memberCount = memberList.Count;
                for (int i = 0; i < memberCount; ++i)
                {
                    AddLabel(55, 129 + i * 22, 0xFF, memberList[i].Name);
                }

                // Okay/Cancel
                AddButton(387, 147, 247, 248, (int)EGumpActions.eGA_AcceptInvitation, GumpButtonType.Reply, 0);
                AddButton(319, 147, 243, 248, (int)EGumpActions.eGA_CancelInvitation, GumpButtonType.Reply, 0);

            }
            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case (int)EGumpActions.eGA_AcceptInvitation:
                        {
                            if (m_charter.AddPlayer((PlayerMobile)sender.Mobile))
                            {
                                m_inviter.SendMessage("{0} accepted your invitation.", sender.Mobile.Name);
                                sender.Mobile.SendMessage("You have accepted the invitation to {0}.", m_charter.TeamName);
                            }
                            else
                            {
                                m_inviter.SendMessage("{0} accepted your invitation, but there was no room!", sender.Mobile.Name);
                                sender.Mobile.SendMessage("You accepted the invitation to {0}, but there was no room!", m_charter.TeamName);
                            }

                            if (m_inviter.HasGump(typeof(ArenaTeamCharterGump)))
                            {
                                m_inviter.CloseGump(typeof(ArenaTeamCharterGump));
                                m_inviter.SendGump(new ArenaTeamCharterGump(m_charter));
                            }

                            break;
                        }
                    default:
                        {
                            m_inviter.SendMessage("{0} declined your invitation.", sender.Mobile.Name);
                            sender.Mobile.SendMessage("You have declined the invitation to {0}.", m_charter.TeamName);
                            break;
                        }
                }
            }
        }
    }
    public class ArenaBoard : Item, IArenaItem
    {
        public int m_parentSerialId;

        [Constructable]
        public ArenaBoard(bool isFacingSouth, Serial parentSerialId)
            : base(isFacingSouth ? 0x1E5E : 0x1E5F)
        {
            Movable = false;
            m_parentSerialId = parentSerialId;
        }
        public ArenaBoard(Serial serial)
            : base(serial)
        { 
            Movable = false;
        }
        public override string DefaultName
        {
            get { return "An Arena Board"; }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this.Location, 3) && from.AccessLevel <= AccessLevel.Player)
            {
                from.SendMessage("You must be within 3 tiles to use this.");
                return;
            }
            if (!(ArenaSystem.GetTeamsForPlayer((PlayerMobile)from).Count > 0))
            {
                from.SendMessage("You can not access this board until you are part of a team. See an arena vendor to purchase a charter.");
                return;
            }
            if (from.HasGump(typeof(ArenaBoardMainGump)))
            {
                from.CloseGump(typeof(ArenaBoardMainGump));
            }
            if (from.HasGump(typeof(ArenaBoardHistoryGump)))
            {
                from.CloseGump(typeof(ArenaBoardHistoryGump));
            }
            if (from.HasGump(typeof(ArenaBoardTemplateGump)))
            {
                from.CloseGump(typeof(ArenaBoardTemplateGump));
            }

            from.SendGump(new ArenaBoardMainGump(from, Location));
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem

        public class ArenaQueueStatusGump : Gump
        {
            enum EGumpActions
            {
                eGA_LeaveQueue = 1,
            }
            public ArenaQueueStatusGump(string name, bool templated, EArenaMatchRestrictions restrictions, EArenaMatchEra era)
            : base(20, 680)
            {
                this.Closable = false;
                this.Dragable = true;
                this.Resizable = false;

                AddBackground(8, 4, 501, 46, 83);
                AddLabel(32, 18, 0xFF, String.Format("{0} queued for {1} {2} {3}.", name, templated ? "PRACTICE" : "RANKED",
                    ArenaSystem.s_restrictionNames[(int)restrictions], ArenaSystem.s_eraNames[(int)era]));
                AddButton(429, 16, 243, 241, (int)EGumpActions.eGA_LeaveQueue, GumpButtonType.Reply, 0);

            }
            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case (int)EGumpActions.eGA_LeaveQueue:
                        {
                            ArenaSystem.LeaveQueue((PlayerMobile)sender.Mobile);
                            break;
                        }
                }
            }
        }

        private class ArenaBoardTemplateGump : Gump
        {
            private ArenaTemplate m_template;
            private Point3D m_Location;
            enum EGumpActions
            {
                eGA_Close = 0,
                eGA_Back,
                eGA_Save,
            }
            public ArenaBoardTemplateGump(Mobile from, Point3D location)
                : base(100, 100)
            {
                m_Location = location;
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                AddPage(0);

                AddBackground(43, 12, 538, 526, 83);
                AddBackground(69, 151, 36, 360,  2620);
                AddBackground(69, 151, 486, 361, 2620);

                string header = "This screen allows you to create a custom stat/skill set to be used in templated matches. Your total allocations can not exceed 700 for skills and 225 for stats, with a maximum of 100 in any skill, and 100 in any stat. If you choose an armor group, your equipped armor will be swapped out with the specified type. To commit the template, press [Okay]. To return to the previous screen, press [Back].";
                AddHtml(68, 36, 486, 99, header, (bool)true, (bool)true);

                // Retrieve the last-saved template data.
                m_template = ArenaSystem.GetTemplate((PlayerMobile)from);
                if (m_template == null)
                {
                    // First time using the template.
                    m_template = new ArenaTemplate(from);
                    ArenaSystem.CacheTemplate((PlayerMobile)from, m_template);
                }

                AddPage(1);

                int xAlign = 92;
                AddLabel(xAlign, 169, 0xFF, "Skills:");

                int textEntryId = 0;
                int row = 0;
                int y = 189;
                int yOffset = 20;
                foreach (Pair<SkillName, int> skillPair in m_template.m_skillList)
                {
                    SkillInfo skillInfo = SkillInfo.Table[(int)skillPair.First];
                    AddLabel(xAlign, y, 0xFF, skillInfo.Name);
                    AddImageTiled(xAlign + 130, y, 30, 20, 0xBBC);
                    AddTextEntry(xAlign + 130, y, 30, 20, 0xFF, textEntryId, skillPair.Second.ToString());

                    if (++row > 10)
                    {
                        row = 0;
                        xAlign += 170;
                        y = 189;
                    }
                    else
                    {
                        y += yOffset;
                    }
                    ++textEntryId;
                }


                // Stats
                xAlign = 440;
                y = 169;
                AddLabel(xAlign, 169, 0xFF, "Stats:");
                y += yOffset;
                int statIdx = 0;
                foreach (Pair<StatType, int> statPair in m_template.m_statList)
                {
                    int textY = y + statIdx * yOffset;
                    AddLabel(xAlign, textY, 0xFF, ArenaTemplate.s_statNames[statIdx]);
                    AddImageTiled(xAlign + 60, textY, 30, 20, 0xBBC);
                    AddTextEntry(xAlign + 60, textY, 30, 20, 0xFF, textEntryId, statPair.Second.ToString());
                    ++textEntryId;
                    ++statIdx;
                }

                // Armor
                y = 288;
                AddGroup(0);
                AddLabel(xAlign, y,0xFF, "Armor:");
                y += 24;

                int currentArmorGroup = m_template.m_armorGroup;
                for (int i = 0; i < 5; ++i)
                {
                    AddRadio(xAlign, y, 210, 211, i == currentArmorGroup ? true : false, i);
                    AddLabel(xAlign + 25, y - 3, 0xFF, ArenaTemplate.s_armorGroups[i]);
                    y += yOffset;
                }

                // Cancel
                AddButton(405, 473, 243, 241, (int)EGumpActions.eGA_Close, GumpButtonType.Reply, 0);
                // Okay
                AddButton(473, 473, 247, 248, (int)EGumpActions.eGA_Save, GumpButtonType.Reply, 0);

                AddLabel(112, 481, 0xFF, "Back");
                AddButton(76, 481, 4008, 4010, (int)EGumpActions.eGA_Back, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (m_template == null || m_template.m_skillList == null || m_template.m_statList == null || m_template.m_serialId == null || sender == null || sender.Mobile == null)
                    return;

                try
                {

                    switch (info.ButtonID)
                    {
                        case (int)EGumpActions.eGA_Back:
                            {
                                sender.Mobile.SendGump(new ArenaBoardMainGump(sender.Mobile, m_Location));
                                break;
                            }
                        case (int)EGumpActions.eGA_Save:
                            {
                                
                                int skillLen = m_template.m_skillList.Length;
                                int[] skills = new int[skillLen];
                                int totalSkillPoints = 0;
                                for (int i = 0; i < skillLen; ++i)
                                {
                                    int val = 0;
                                    if (Int32.TryParse(info.GetTextEntry(i).Text, out val))
                                    {
                                        if (val < 0 || val > 100)
                                        {
                                            int tableIdx = (int)m_template.m_skillList[i].First;
                                            string skillName = SkillInfo.Table[tableIdx].Name;
                                            sender.Mobile.SendMessage("Your template was discarded because {0} was outside the allowed range (0-100)", skillName);
                                            return;
                                        }

                                        totalSkillPoints += val;
                                        skills[i] = val;
                                    }
                                    else
                                    {
                                        int tableIdx = (int)m_template.m_skillList[i].First;
                                        string skillName = SkillInfo.Table[tableIdx].Name;
                                        sender.Mobile.SendMessage("Your template was discarded because {0} was in an invalid format (0-100 allowed).", skillName);
                                        
                                    }
                                }

                                if (totalSkillPoints > 700)
                                {
                                    sender.Mobile.SendMessage("Your template was discarded because you exceeded 700 skill points.");
                                    return;
                                }

                                int statLen = m_template.m_statList.Length;
                                int[] stats = new int[statLen];
                                int totalStatPoints = 0;
                                for (int i = 0; i < statLen; ++i)
                                {
                                    int val = 0;
                                    if (Int32.TryParse(info.GetTextEntry(i + skillLen).Text, out val))
                                    {
                                        if (val < 0 || val > 100)
                                        {
                                            string statName = ArenaTemplate.s_statNames[i];
                                            sender.Mobile.SendMessage("Your template was discarded because {0} was outside the allowed range (0-100).", statName);
                                            return;
                                        }
                                        totalStatPoints += val;
                                        stats[i] = val;
                                    }
                                    else
                                    {
                                        string statName = ArenaTemplate.s_statNames[i];
                                        sender.Mobile.SendMessage("Your template was discarded because {0} was in an invalid format (0-100 allowed).", statName);
                                        return;
                                    }
                                }

                                if (totalStatPoints > 225)
                                {
                                    sender.Mobile.SendMessage("Your template was discarded because you exceeded 225 stat points.");
                                    return;
                                }

                                // Armor group 0 is "no-modify".
                                int armorGroup = 0;
                                for (int i = 0; i < 5; ++i)
                                {
                                    if (info.IsSwitched(i))
                                    {
                                        armorGroup = i;
                                        break;
                                    }
                                }
                                m_template.m_armorGroup = armorGroup;

                                // Commit the valid template.
                                int idx = 0;
                                foreach (Pair<StatType, int> statPair in m_template.m_statList)
                                {
                                    statPair.Second = stats[idx++];
                                }
                                idx = 0;
                                foreach (Pair<SkillName, int> skillPair in m_template.m_skillList)
                                {
                                    skillPair.Second = skills[idx++];
                                }

                                sender.Mobile.SendMessage("Your template has been saved!");
                                    
                                break;
                            }
                    }
                    }

                    catch(Exception ex)
                    {
                        Console.WriteLine("Arena Template Error {0}", ex.Message);
                    }
                }
        }
        private class ArenaBoardHistoryGump : Gump
        {
            private Point3D m_Location;
            enum EGumpActions
            {
                eGA_Close = 0,
                eGA_Back,
            }
            public ArenaBoardHistoryGump(EArenaMatchRestrictions restrictions, EArenaMatchEra era, bool templated, ArenaTeam team, Point3D location)
                : base(100, 100)
            {
                m_Location = location;
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                AddPage(0);

                AddBackground(43, 12, 538, 526, 83);
                AddBackground(69, 151, 36, 360, 2620);
                AddBackground(69, 151, 486, 361, 2620);

                string header = "Your most recent matches are listed below. The winners' names are green and the losers' names are red. In the case of a draw, neither team is colored. The adjustment to your rating as a result of the match is shown on the right. You can return to the previous screen by pressing [Back].";
                AddHtml(68, 36, 486, 99, header, (bool)true, (bool)true);

                AddPage(1);

                string seasonName = "an unknown season";
                ArenaSeason season = ArenaSeasonTracker.GetCurrentSeason();
                if (season != null)
                {
                    seasonName = season.SeasonName;
                }

				AddLabel(92, 169, 0xFF, String.Format("Recent match history for {0}, {1}", team.TeamName, seasonName));
                AddLabel(92, 190, 0xFF, String.Format("Ladder: {0} {1} {2}           Rating: {3}",
                    ArenaSystem.s_eraNames[(int)era],
                    ArenaSystem.s_restrictionNames[(int)restrictions],
                    templated ? "templated" : "non-templated",
                    team.GetScore(restrictions, era, templated)));

                int alignX = 93;
                int y = 229;
                int yOffset = 20;
                int green = 1271;
                int red = 337;

                List<LimitedArenaMatchResult> matchHistoryList = team.GetMatchResults(restrictions,era,templated);
                if (matchHistoryList.Count > 0)
                {
                    AddLabel(alignX, y, 0xFF, "Your team:");
                    AddLabel(alignX + 200, y, 0xFF, "Enemy team:");
                    AddLabel(alignX + 360, y, 0xFF, "Adjustments:");
                    y += yOffset;

                    foreach (LimitedArenaMatchResult result in matchHistoryList)
                    {
                        if (season != null && season.SeasonName != result.m_seasonName)
                        {
                            // This is match history for something that occurred in a different season, and
                            // shouldn't be reflected in the history results.
                            continue;
                        }

                        // Player's team is left-aligned
                        int localScoreDelta = 0;
                        string leftName;
                        string rightName;
						int localTeam = result.m_team1Name == team.TeamName ? 1 : 2;
                        if (localTeam == 1)
                        {
                            leftName = result.m_team1Name;
                            rightName = result.m_team2Name;
                            localScoreDelta = result.m_team1ScoreDelta;
                        }
                        else
                        {
                            leftName = result.m_team2Name;
                            rightName = result.m_team1Name;
                            localScoreDelta = result.m_team2ScoreDelta;
                        }

                        ArenaMatch.EMatchEndType endType = result.m_endType;
                        bool draw = endType == ArenaMatch.EMatchEndType.eMET_Draw;
						bool localVictory = endType == ArenaMatch.EMatchEndType.eMET_Win && result.m_winnerName == team.TeamName;

                        int localColor = draw ? 0xFF : (localVictory ? green : red);
                        var enemyColor = draw ? 0xFF : (localVictory ? red : green);
                        AddLabel(alignX, y, localColor, leftName);
                        AddLabel(alignX + 160, y, 0xFF, "vs.");
                        AddLabel(alignX + 200, y, enemyColor, rightName);
                        AddLabel(alignX + 360, y, localColor, String.Format("{0}{1}", localVictory ? "+" : "", localScoreDelta));
                        y += yOffset;
                    }
                }
                else
                {
                    AddLabel(alignX, y, 0xFF, "No history found for this ladder.");
                }
                

                AddLabel(112, 481, 0xFF, "Back");
                AddButton(76, 481, 4008, 4010, (int)EGumpActions.eGA_Back, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case (int)EGumpActions.eGA_Back:
                        {
                            sender.Mobile.SendGump(new ArenaBoardMainGump(sender.Mobile, m_Location));
                            break;
                        }
                }
            }
        }
        private class ArenaBoardMainGump : Gump
        {
            private Point3D m_Location;

            enum EGumpActions
            {
                eGA_Close = 0,
                eGA_JoinQueue,
                eGA_LeaveQueue,
                eGA_Skills,
                eGA_Help,
                eGA_ViewHistory,
				eGA_Leaderboards,
			}
            enum EEraSwitchId
            {
                eESI_IPY = 1,
                eESI_T2A = 2,
                eESI_UOR = 3,
            }
            enum ERestrictionsSwitchId
            {
                eRSI_Chaos = 4,
                eRSI_Order = 5,
            }
			enum ESkillsSwitchId
			{
				eSSI_TemplatedSkills = 6,
				eSSI_CurrentSkills = 7,
			}

            public ArenaBoardMainGump(Mobile from, Point3D location)
                : base(100, 100)
            {
                m_Location = location;
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                AddPage(0);

                AddBackground(43, 12, 538, 546, 83);
                AddBackground(69, 151, 36, 360, 2620);
                AddBackground(69, 151, 486, 381, 2620);

                string header = "Welcome to the arena board! Once you have selected a team and a ladder, press [Okay] to join the queue. You can be in one queue at a time. To abandon your place in the queue, press [Cancel]. To customize your template skills and stats for use in an unranked match, press [Skills]. For an explanation into the eras, rules, or use of templates, press [Help]. To close the board, simply right-click the interface.";
                AddHtml(68, 36, 486, 99, header, (bool)true, (bool)true);

                AddPage(1);

                int alignX = 95;
                // Team selection
                AddLabel(alignX, 170, 0xFF, "Step 1: Select team:");
                List<ArenaTeam> teamList = ArenaSystem.GetTeamsForPlayer((PlayerMobile)from);
                var player = from as PlayerMobile;

                AddGroup(0);
                int y = 195;
                int idx = 10;

                for (int i = 0; i < teamList.Count; i++)
                {
                    ArenaTeam team = teamList[i];
                    bool check = player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.TeamSelection] == i;
                    AddRadio(alignX, y, 210, 211, check, idx++);
                    AddLabel(119, y, 0xFF, String.Format("{0}  {1}v{1}", team.TeamName, team.Capacity));
                    y += 20;
                }

                // Ladder selection
                int alignY = 273;
                {
                    AddLabel(alignX, 273, 0xFF, "Step 2: Select a ladder:");
                    alignY = 293;
                    AddLabel(alignX, alignY, 0xFF, "Era:");
                    AddLabel(alignX + 75, alignY, 0xFF, "Rules:");
                    AddLabel(alignX + 150, alignY, 0xFF, "Options:");


                    AddGroup(1);
                    alignY = 313;
                    int len = (int)EArenaMatchEra.eAMR_NumEras;
                    for (int i = 0; i < len; ++i)
                    {
                        // 12/6/13 Xiani - IPY is the only supported ruleset at this time.
                        if (EArenaMatchEra.eAMR_IPY != (EArenaMatchEra)i)
                            continue;

                        AddLabel(alignX + 20, alignY, 0xFF, ArenaSystem.s_eraNames[i]);
                        AddRadio(alignX, alignY, 210, 211, player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.EraSelection] == i, (int)EEraSwitchId.eESI_IPY + i);
                        alignY += 20;
                    }

                    AddGroup(2);
                    alignX += 75;
                    alignY = 313;
                    len = (int)EArenaMatchRestrictions.eAMC_NumRestrictions;
                    for (int i = 0; i < len; ++i)
                    {
                        AddLabel(alignX + 20, alignY, 0xFF, ArenaSystem.s_restrictionNames[i]);
                        AddRadio(alignX, alignY, 210, 211, player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.RulesetSelection] == i, (int)ERestrictionsSwitchId.eRSI_Chaos + i);
                        alignY += 20;
                    }

                    alignX += 75;
                    alignY = 313;

					// Template / Current skills
					AddGroup(3);
					AddLabel(alignX + 23, alignY, 0xFF, "Ranked Match - Using current skill set");
					AddRadio(alignX, alignY, 210, 211, player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.OptionsSelection] == 0, (int)ESkillsSwitchId.eSSI_CurrentSkills);
					alignY += 20;
                    AddLabel(alignX + 23, alignY, 0xFF, "Unranked Match - Using custom skill set");
                    AddRadio(alignX, alignY, 210, 211, player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.OptionsSelection] == 1, (int)ESkillsSwitchId.eSSI_TemplatedSkills);

					
                }

                //// Arena options
                //{
                //    alignX = 377;
                //    alignY = 170;
                //    AddLabel(alignX, alignY, 0xFF, "Allowed arena options:");
                //    alignY += 20;
                //    AddLabel(alignX + 23, alignY, 0xFF, "Variable height");
                //    AddCheck(alignX, alignY, 210, 211, true, 31);
                //    alignY += 20;
                //    AddLabel(alignX + 23, alignY, 0xFF, "Fixed Obstacles");
                //    AddCheck(alignX, alignY, 210, 211, true, 32);
                //    alignY += 20;
                //    AddLabel(alignX + 23, alignY, 0xFF, "Dynamic Obstacles");
                //    AddCheck(alignX, alignY, 210, 211, true, 33);
                //}

                // Management options
                AddLabel(96, 398, 0xFF, "Selection options:");
				AddLabel(136, 418, 0xFF, "Leaderboards");
				AddButton(96, 417, 4008, 4010, (int)EGumpActions.eGA_Leaderboards, GumpButtonType.Reply, 0);
				AddLabel(136, 438, 0xFF, "View recent history for current ladder selected");
				AddButton(96, 437, 4008, 4010, (int)EGumpActions.eGA_ViewHistory, GumpButtonType.Reply, 0);

                // Skills
                AddButton(262, 495, 2017, 2016, (int)EGumpActions.eGA_Skills, GumpButtonType.Reply, 0);
                // Help
				AddButton(333, 495, 2033, 2032, (int)EGumpActions.eGA_Help, GumpButtonType.Reply, 0);
                // Cancel
				AddButton(405, 493, 243, 241, (int)EGumpActions.eGA_LeaveQueue, GumpButtonType.Reply, 0);
                // Okay
				AddButton(473, 493, 247, 248, (int)EGumpActions.eGA_JoinQueue, GumpButtonType.Reply, 0);
            }

            private void GetSelectionFromResponse(NetState sender, RelayInfo info, ref ArenaTeam team,
                ref EArenaMatchEra era, ref EArenaMatchRestrictions restrictions, ref bool templated)
            {
                var player = sender.Mobile as PlayerMobile;
                if (info.IsSwitched((int)EEraSwitchId.eESI_IPY))
                {
                    era = EArenaMatchEra.eAMR_IPY;
                }
                else if (info.IsSwitched((int)EEraSwitchId.eESI_T2A))
                {
                    era = EArenaMatchEra.eAMR_T2A;
                }
                else if (info.IsSwitched((int)EEraSwitchId.eESI_UOR))
                {
                    era = EArenaMatchEra.eAMR_Pub16;
                }
                player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.EraSelection] = (int)era;

                if (info.IsSwitched((int)ERestrictionsSwitchId.eRSI_Order))
                {
                    restrictions = EArenaMatchRestrictions.eAMC_Order;
                }
                else if (info.IsSwitched((int)ERestrictionsSwitchId.eRSI_Chaos))
                {
                    restrictions = EArenaMatchRestrictions.eAMC_Chaos;
                }

                player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.RulesetSelection] = (int)restrictions;

				templated = info.IsSwitched((int)ESkillsSwitchId.eSSI_TemplatedSkills);

                player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.OptionsSelection] = templated ? 1 : 0;

                int teamIdx = -1;
                if (info.IsSwitched(10))
                {
                    teamIdx = 0;
                }
                else if (info.IsSwitched(11))
                {
                    teamIdx = 1;
                }
                else if (info.IsSwitched(12))
                {
                    teamIdx = 2;
                }
				if (teamIdx != -1)
				{
					List<ArenaTeam> teamsList = ArenaSystem.GetTeamsForPlayer((PlayerMobile)sender.Mobile);
					team = teamsList[teamIdx];
				}
				else
				{
					sender.Mobile.SendMessage("Internal error, make sure you are using the official UOAC client");
				}
                player.ArenaPreferences[PlayerMobile.ArenaPreferenceKeys.TeamSelection] = teamIdx;
            }
            public override void OnResponse(NetState sender, RelayInfo info)
            {
                switch (info.ButtonID)
                {
                    case (int)EGumpActions.eGA_JoinQueue:
                        {
                            EArenaMatchEra era = EArenaMatchEra.eAMR_IPY;
                            EArenaMatchRestrictions restrictions = EArenaMatchRestrictions.eAMC_Order;
                            bool templated = false;
                            ArenaTeam team = null;

                            GetSelectionFromResponse(sender, info, ref team, ref era, ref restrictions, ref templated);
                            
                            // Special exception case; no expectation for it ever to happen.
                            if (team == null)
                                return;

                            ArenaSystem.JoinQueue(team, restrictions, era, templated, m_Location);
                            break;
                        }
                    case (int)EGumpActions.eGA_LeaveQueue:
                        {
                            ArenaSystem.LeaveQueue((PlayerMobile)sender.Mobile);

                            break;
                        }
                    case (int)EGumpActions.eGA_Help:
                        {
                            break;
                        }
                    case (int)EGumpActions.eGA_Skills:
                        {
                            sender.Mobile.SendGump(new ArenaBoardTemplateGump(sender.Mobile, m_Location));
                            break;
                        }
                    case (int)EGumpActions.eGA_ViewHistory:
                        {
                            EArenaMatchEra era = EArenaMatchEra.eAMR_IPY;
                            EArenaMatchRestrictions restrictions = EArenaMatchRestrictions.eAMC_Order;
                            bool templated = false;
                            ArenaTeam team = null;
                            GetSelectionFromResponse(sender, info, ref team, ref era, ref restrictions, ref templated);

                            if (team == null)
                                return;

                            sender.Mobile.SendGump(new ArenaBoardHistoryGump(restrictions, era, templated, team, m_Location));
                            break;
                        }
					case (int)EGumpActions.eGA_Leaderboards:
						{
							sender.Mobile.SendGump(new ArenaLeaderboardsGump(sender.Mobile, EArenaMatchEra.eAMR_IPY, EArenaMatchRestrictions.eAMC_Order, 0));
							break;
						}
                }
            }
        }
    }
}

