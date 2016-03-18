using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;
using Server.Accounting;
using Server.Commands;
using Server.Commands.Generic;
using Server.Network;
using Server.Achievements;
using Server.Custom.Battlegrounds;
using Server.Custom.Battlegrounds.Regions;

namespace Server.Custom.Townsystem
{
	[Parsable]
	[CustomEnum(new string[] { "Order", "Chaos", "Balance" })]
    public class AllianceInfo
    {
        public static void Initialize()
        {
            CommandSystem.Register("ResetAlliances", AccessLevel.Administrator, ResetAlliances);
        }
        public static void ResetAlliances(CommandEventArgs e)
        {
            ResetAlliances();
        }

        public static void ResetAlliances()
        {
            foreach (Faction faction in Faction.Factions)
                faction.Alliance = null;

            Alliances.Clear();

            Faction britain = Faction.Parse("Britain");
            Faction trinsic = Faction.Parse("Trinsic");
            Faction magincia = Faction.Parse("Magincia");

            britain.Alliance = new AllianceInfo(britain, "Order", 5550, 111);
            trinsic.Alliance = new AllianceInfo(trinsic, "Balance", 5590, 2213);
			magincia.Alliance = new AllianceInfo(magincia, "Chaos", 5566, 2118);
        }

        private static readonly int m_MaximumAlliances = 3;
        public static List<AllianceInfo> Alliances = new List<AllianceInfo>();

        private string m_Name;
        private Faction m_Leader;
        private List<Faction> m_Members;
        private int m_BannerID;
        private int m_BannerHue;

        public int BannerID { get { return m_BannerID; } set { m_BannerID = value;} }
        public int BannerHue { get { return m_BannerHue; } set { m_BannerHue = value; } }

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        public List<Faction> Members
        {
            get { return m_Members; }
            set { m_Members = value; }
        }
		public Faction Leader
		{
			get { return m_Leader; }
		}

        public bool IsMember(Faction f)
        {
            if (f.Alliance != this)
                return false;

            return m_Members.Contains(f);
        }

		public static AllianceInfo Parse(string name)
		{
			for (int i = 0; i < Alliances.Count; ++i)
			{
				AllianceInfo alliance = Alliances[i];

				if (Insensitive.Equals(alliance.Name, name))
					return alliance;
			}
			return null;
		}
        public AllianceInfo(Faction leader, string name = "", int bannerID = 5550, int bannerHue = 107)
        {
            m_Name = name;
            m_Leader = leader;
            m_BannerID = bannerID;
            m_BannerHue = bannerHue;

            m_Members = new List<Faction>();

            if (Alliances.Count >= m_MaximumAlliances)
                return;

            leader.Alliance = this;
            Alliances.Add(this);
        }

        public void AddFaction(Faction f, bool do_timecheck = true)
        {
			if (do_timecheck && f.State.NextAllowedAllianceChange > DateTime.UtcNow)
				return;
            if (m_Members.Contains(f))
                return;

			if (f.Alliance != null)
			{
				f.Alliance.RemoveFaction(f);
				if (f.Alliance != null)
					return; // sanity, faction leader.
			}
			f.Alliance = this;

            m_Members.Add(f);
            InvalidateMemberProperties();

			f.State.NextAllowedAllianceChange = DateTime.UtcNow + TimeSpan.FromDays(14.0);
        }
        public void RemoveFaction(Faction f)
        {
            if (f == m_Leader)
                return;

            if (m_Members.Contains(f))
            {
                m_Members.Remove(f);
                InvalidateMemberProperties();
            }

            f.Alliance = null;
		}
        public void InvalidateMemberProperties()
        {
            if (m_Leader != null)
                m_Leader.InvalidateMemberProperties();

            foreach (Faction f in m_Members)
                f.InvalidateMemberProperties();
        }

        public void AllianceChat( Mobile from, int hue, string text )
		{
            if (Leader != null) {
                Leader.SendAllianceMessage(from, hue, text);
            }

			for( int i = 0; i < m_Members.Count; i++ ) {
                var f = m_Members[i];
                if (f != null)
                    f.SendAllianceMessage(from, hue, text);
			}
		}

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)3); // version
            writer.Write(m_BannerHue);
            writer.Write(m_BannerID);

            writer.Write(m_Members.Count);
            foreach (Faction f in m_Members)
                Faction.WriteReference(writer, f);

            writer.Write(m_Name);
        }

        public static AllianceInfo Deserialize(GenericReader reader, Faction faction)
        {
            int version = reader.ReadEncodedInt();

            var info = new AllianceInfo(faction);

            switch(version)
            {
				case 3:
					{
						goto case 2;
					}
				case 2:
					{
						goto case 1;
					}
				case 1:
                    {
                        info.BannerHue = reader.ReadInt();
                        info.BannerID = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
						if( 2 > version )
							reader.ReadDateTime();

                        int count1 = reader.ReadInt();
                        for (int i = 0; i < count1; i++)
                        {
                            var fac = Faction.ReadReference(reader);
                            if (fac != null)
                                info.Members.Add(fac);
                        }

						if (3 > version)
						{
							int count2 = reader.ReadInt();
							for (int i = 0; i < count2; i++)
								Faction.ReadReference(reader);
						}


                        info.Name = reader.ReadString();
						if (info.Name == "Order")
							info.BannerHue = 109; // changed from 107 which is a tad too dark
                        break;
                    }
			}
            return info;
        }
    }

    [Parsable]
    [CustomEnum(new string[] { "Britain", "Cove", "Jhelom", "Magincia", "Minoc", "Moonglow", "Nujel'm", "Ocllo", "Serpent's Hold", "Skara Brae", "Trinsic", "Vesper", "Yew" })]
    public abstract class Faction : IComparable
    {
        public int ZeroRankOffset;

		private FactionDefinition m_Definition;
		private FactionState m_State;

		public FactionDefinition Definition
		{
			get{ return m_Definition; }
			set
			{
				m_Definition = value;
			}
		}

		public FactionState State
		{
			get{ return m_State; }
			set{ m_State = value; }
		}

		public List<PlayerState> Members
		{
			get{ return m_State.Members; }
			set{ m_State.Members = value; }
		}

        public void AllianceChat(Mobile from, string text)
        {
            PlayerMobile pm = from as PlayerMobile;
            // changed to town alliance chat
            Town town = Town.Find(from);
            if (town != null)
            {
                town.AllianceChat(from, (pm == null) ? 0x3B2 : pm.AllianceMessageHue, text);
            }
        }

        public AllianceInfo Alliance
        {
            get { return m_State.Alliance; }
            set { 
                m_State.Alliance = value;

                var town = Town.Parse(Definition.FriendlyName);

				if (town != null)
				{
					foreach (AllianceFlag flag in town.AllianceFlags)
						flag.Invalidate();

					TownDecorationItem.OnTownChanged(town); // alliance changed
				}	
            }
        }

		public static readonly TimeSpan LeavePeriod = TimeSpan.FromSeconds(1);

        public bool IsAlly(Faction f)
        {
            if (f == null || f == this)
                return true;
            else if (Alliance == null)
                return false;
            else if (Alliance.Members == null)
            {
                Alliance.Members = new List<Faction>();
            }

            return f == Alliance.Leader || Alliance.Members.Contains(f);
        }

        public bool IsEnemy(Faction f)
        {
            if (f == null || f == this)
                return false;
            else if (Alliance == null)
                return true;
            else if (Alliance.Members == null)
            {
                Alliance.Members = new List<Faction>();
            }

			if( Alliance.Name == "Chaos" )
				return true; // always at war with everyone else
			else
				return f != Alliance.Leader && !Alliance.Members.Contains(f);
        }

        public void InvalidateMemberProperties()
        {
            if (Members != null)
            {
                for (int i = 0; i < Members.Count; i++)
                {
                    PlayerState ps = Members[i];
                    if (ps.Mobile != null)
                    {
                        ps.Mobile.InvalidateProperties();
                        ps.Mobile.Delta(MobileDelta.Noto);
                    }
                }
            }
        }

        public void SendAllianceMessage(Mobile from, int hue, string text)
        {
            Packet p = null; 

            for (int j = 0; j < Members.Count; j++)
            {
                var ps = Members[j];
                if (ps == null) continue;

                Mobile m = ps.Mobile;
                if (m == null) continue;

                NetState state = m.NetState;

                if (state != null)
                {
                    if (p == null)
                        p = Packet.Acquire(new UnicodeMessage(from.Serial, from.Body, MessageType.Alliance, hue, 3, from.Language, from.Name, text));

                    state.Send(p);
                }
            }

            Packet.Release(p);
        }

		public void Broadcast( string text )
		{
			Broadcast( 0x3B2, text );
		}

        public static void BroadcastAll(string text)
        {
            foreach (Faction f in Factions)
            {
                f.Broadcast(text);
            }
        }

		public void Broadcast( int hue, string text )
		{
			List<PlayerState> members = Members;

			for ( int i = 0; i < members.Count; ++i )
				members[i].Mobile.SendMessage( hue, text );
		}

		public void Broadcast( string format, params object[] args )
		{
			Broadcast( String.Format( format, args ) );
		}

		public void Broadcast( int hue, string format, params object[] args )
		{
			Broadcast( hue, String.Format( format, args ) );
		}

		public void BeginBroadcast( Mobile from )
		{
			from.SendLocalizedMessage( 1010265 ); // Enter Faction Message
			from.Prompt = new BroadcastPrompt( this );
		}

		public void EndBroadcast( Mobile from, string text )
		{
            string btext = "";
            if (from.AccessLevel == AccessLevel.Player)
            {
                btext = String.Format("{0} [{1} King]: ", from.Name, Town.CheckCitizenship(from));
                Town.CheckCitizenship(from).State.RegisterBroadcast();
            }
            btext = String.Concat(btext, text);
            Broadcast( Definition.HueBroadcast, btext );
		}

		private class BroadcastPrompt : Prompt
		{
			private Faction m_Faction;

			public BroadcastPrompt( Faction faction )
			{
				m_Faction = faction;
			}

			public override void OnResponse( Mobile from, string text )
			{
				m_Faction.EndBroadcast( from, text );
			}
		}

        public static bool CompareFactions(Mobile m1, Mobile m2)
        {
            Faction f1 = Find(m1);
            Faction f2 = Find(m2);

            return (f1 !=null && f2 != null && f1 == f2);
        }

		public static void HandleAtrophy()
		{
			foreach ( Faction f in Factions )
			{
				if ( !f.State.IsAtrophyReady )
					return;
			}

			List<PlayerState> activePlayers = new List<PlayerState>();

			foreach ( Faction f in Factions )
			{
				foreach ( PlayerState ps in f.Members )
				{
					if ( ps.KillPoints > 0 && ps.IsActive )
						activePlayers.Add( ps );
				}
			}

			int distrib = 0;

			foreach ( Faction f in Factions )
				distrib += f.State.CheckAtrophy();

			if ( activePlayers.Count == 0 )
				return;

			for ( int i = 0; i < distrib; ++i )
				activePlayers[Utility.Random( activePlayers.Count )].KillPoints++;
		}

        public static void AwardVendorCredits()
        {
            var list = OCLeaderboard.GetActivePlayers();
            int[] basePoints = new int[Town.Towns.Count];
            Town tempTown;

            for (int i = 0; i < Town.Towns.Count; ++i)
            {
                basePoints[i] = 0;
                for (int j = 0; j < Town.Towns.Count; ++j)
                {
                    tempTown = Town.Towns[i];

					if (tempTown == null || tempTown.ControllingTown == null)
                        continue;

					if (tempTown == Town.Towns[j].ControllingTown)
                        ++basePoints[i];
                }
            }

            foreach (Mobile m in list)
            {
                var ps = PlayerState.Find(m);
                if (ps == null || ps.Faction == null)
                    continue;

                int index = Faction.Factions.IndexOf(ps.Faction);

                if (index >= basePoints.Length)
                    continue;

                int credit = (int)Math.Ceiling((double)basePoints[index] + ((double)ps.KillPoints * 0.01 ));

                if (credit > 0)
                {
                    m.SendMessage("You have been awarded {0} militia vendor credits!", credit);
                    ps.VendorCredit += credit;
                }
                else
                    m.SendMessage("You have not qualified for any militia vendor credits.");
            }
        }


        public static void DistributePoints(int distrib)
        {
			List<PlayerState> activePlayers = new List<PlayerState>();

			foreach ( Faction f in Factions ) {
				foreach ( PlayerState ps in f.Members ) {
					if ( ps.KillPoints > 0 && ps.IsActive ) {
						activePlayers.Add( ps );
					}
				}
			}

			if ( activePlayers.Count > 0 ) {
				for ( int i = 0; i < distrib; ++i ) {
					activePlayers[Utility.Random( activePlayers.Count )].KillPoints++;
				}
			}
		}

		public void BeginHonorLeadership( Mobile from )
		{
			from.SendLocalizedMessage( 502090 ); // Click on the player whom you wish to honor.
			from.BeginTarget( 12, false, TargetFlags.None, new TargetCallback( HonorLeadership_OnTarget ) );
		}

		public void HonorLeadership_OnTarget( Mobile from, object obj )
		{
			if ( obj is Mobile )
			{
				Mobile recv = (Mobile) obj;

				PlayerState giveState = PlayerState.Find( from );
				PlayerState recvState = PlayerState.Find( recv );

				if ( giveState == null )
					return;

				if ( recvState == null || recvState.Faction != giveState.Faction )
				{
					from.SendLocalizedMessage( 1042497 ); // Only faction mates can be honored this way.
				}
				else if ( giveState.KillPoints < 5 )
				{
					from.SendLocalizedMessage( 1042499 ); // You must have at least five kill points to honor them.
				}
				else
				{
					recvState.LastHonorTime = DateTime.Now;
					giveState.KillPoints -= 5;
					recvState.KillPoints += 4;

					// TODO: Confirm no message sent to giver
					recv.SendLocalizedMessage( 1042500 ); // You have been honored with four kill points.
				}
			}
			else
			{
				from.SendLocalizedMessage( 1042496 ); // You may only honor another player.
			}
		}

		public virtual void AddMember( Mobile mob )
		{
            Town town = Town.CheckCitizenship(mob);
            if (town == null)
                return;

            town.AddMilitiaMember(mob);

			Members.Insert( ZeroRankOffset, new PlayerState( mob, this, Members ) );
			//mob.AddToBackpack( FactionItem.Imbue( new Robe(), this, false, Definition.HuePrimary ) );
			//mob.SendLocalizedMessage( 1010374 ); // You have been granted a robe which signifies your faction

			mob.InvalidateProperties();
			mob.Delta( MobileDelta.Noto );

			mob.FixedEffect( 0x373A, 10, 30 );
			mob.PlaySound( 0x209 );
		}
		public void RemoveMember( Mobile mob )
		{
			PlayerState pl = PlayerState.Find( mob );

			if ( pl == null || !Members.Contains( pl ) )
				return;

            Town town = Town.CheckCitizenship(mob);

            if (town != null)
                town.RemoveMilitiaMember(mob);

			int killPoints = pl.KillPoints;

			if( mob.Backpack != null )
			{
				//Ordinarily, through normal faction removal, this will never find any sigils.
				//Only with a leave delay less than the ReturnPeriod or a Faction Kick/Ban, will this ever do anything
				/*Item[] sigils = mob.Backpack.FindItemsByType( typeof( Sigil ) );

				for ( int i = 0; i < sigils.Length; ++i )
					((Sigil)sigils[i]).ReturnHome();*/
			}

			if ( pl.RankIndex != -1 ) {
				while ( ( pl.RankIndex + 1 ) < ZeroRankOffset ) {
					PlayerState pNext = Members[pl.RankIndex+1];
					Members[pl.RankIndex+1] = pl;
					Members[pl.RankIndex] = pNext;
					pl.RankIndex++;
					pNext.RankIndex--;
				}

				ZeroRankOffset--;
			}

			Members.Remove( pl );

            pl.RemoveTitle(); //removes the [titles title

			if ( mob is PlayerMobile )
				((PlayerMobile)mob).TownsystemPlayerState = null;

			mob.InvalidateProperties();
			mob.Delta( MobileDelta.Noto );

			if ( mob is PlayerMobile )
				((PlayerMobile)mob).ValidateEquipment();

			if ( killPoints > 0 )
				DistributePoints( killPoints );

            mob.SendMessage("You are no longer serving the militia");
		}
       
		public void JoinAlone( Mobile mob )
		{
			AddMember( mob );
			mob.SendLocalizedMessage( 1005058 ); // You have joined the faction
		}

		public static bool IsFactionBanned( Mobile mob )
		{
			Account acct = mob.Account as Account;

			if ( acct == null )
				return false;

			return ( acct.GetTag( "FactionBanned" ) != null );
		}

		public void OnJoinAccepted( Mobile mob )
		{
			PlayerMobile pm = mob as PlayerMobile;

			if ( pm == null )
				return; // sanity

			PlayerState pl = PlayerState.Find( pm );
            Town town = Town.CheckCitizenship(mob);

            if (pm.Young)
                pm.SendLocalizedMessage(1010104); // You cannot join a faction as a young player
            else if (town == null || town.HomeFaction != this)
                return;
            else if (pl != null && pl.IsLeaving)
                pm.SendLocalizedMessage(1005051); // You cannot use the faction stone until you have finished quitting your current faction
            //else if ( AlreadyHasCharInFaction( pm ) )
            //pm.SendLocalizedMessage( 1005059 ); // You cannot join a faction because you already declared your allegiance with another character
            else if (IsFactionBanned(mob))
                pm.SendLocalizedMessage(1005052); // You are currently banned from the faction system
            else if (pm.Skills[SkillName.AnimalTaming].BaseFixedPoint > 0)
                pm.SendMessage("Animal tamers are not able to participate in militia wars.");
            else if (pm.Skills[SkillName.Begging].BaseFixedPoint > 0)
                pm.SendMessage("Beggers are not able to participate in militia wars.");
            else if (pm.ShortTermMurders > 4)
                pm.SendMessage("Murderers cannot join the militia.");
            else if (!CanHandleInflux(1))
            {
                pm.SendLocalizedMessage(1018031); // In the interest of faction stability, this faction declines to accept new members for now.
            }
            else
            {
                JoinAlone(mob);
            }
		}

		public Faction()
		{
			m_State = new FactionState( this );
		}

		public override string ToString()
		{
			return m_Definition.FriendlyName;
		}

		public int CompareTo( object obj )
		{
			return m_Definition.Sort - ((Faction)obj).m_Definition.Sort;
		}

		public static bool CheckLeaveTimer( Mobile mob )
		{
			PlayerState pl = PlayerState.Find( mob );

			if ( pl == null || !pl.IsLeaving )
				return false;

			if ((pl.Leaving + LeavePeriod) >= DateTime.Now)
				return false;

			mob.SendLocalizedMessage( 1005163 ); // You have now quit your faction

			pl.Faction.RemoveMember( mob );

			return true;
		}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler( EventSink_Login );
			EventSink.Logout += new LogoutEventHandler( EventSink_Logout );

			Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), TimeSpan.FromMinutes( 10.0 ), new TimerCallback( HandleAtrophy ) );

            CommandSystem.Register("BroadcastMessage", AccessLevel.GameMaster, new CommandEventHandler(BroadcastMessage_OnCommand) );
            CommandSystem.Register( "FactionElection", AccessLevel.GameMaster, new CommandEventHandler( FactionElection_OnCommand ) );
            //CommandSystem.Register( "FactionItemReset", AccessLevel.Administrator, new CommandEventHandler( FactionItemReset_OnCommand ) );
			CommandSystem.Register( "FactionReset", AccessLevel.Administrator, new CommandEventHandler( FactionReset_OnCommand ) );
			CommandSystem.Register( "FactionTownReset", AccessLevel.Administrator, new CommandEventHandler( FactionTownReset_OnCommand ) );
		}

        public static void BroadcastMessage_OnCommand(CommandEventArgs e)
        {
            PlayerMobile from = (PlayerMobile)e.Mobile;
            from.SendMessage("Enter Broadcast Message");
            from.Prompt = new BroadcastPrompt(from.TownsystemPlayerState.Faction);
        }

		public static void FactionTownReset_OnCommand( CommandEventArgs e )
		{

		}

		public static void FactionReset_OnCommand( CommandEventArgs e )
		{
			List<Faction> factions = Faction.Factions;

			for ( int i = 0; i < factions.Count; ++i )
			{
				Faction f = factions[i];

				List<PlayerState> playerStateList = new List<PlayerState>( f.Members );

				for( int j = 0; j < playerStateList.Count; ++j )
					f.RemoveMember( playerStateList[j].Mobile );
			}
		}

		public static void FactionElection_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Target a faction stone to open its election properties." );
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( FactionElection_OnTarget ) );
		}

		public static void FactionElection_OnTarget( Mobile from, object obj )
		{
		}

		public static void FactionKick_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Target a player to remove them from their faction." );
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( FactionKick_OnTarget ) );
		}

		public static void FactionKick_OnTarget( Mobile from, object obj )
		{
			if ( obj is Mobile )
			{
				Mobile mob = (Mobile) obj;
				PlayerState pl = PlayerState.Find( (Mobile) mob );

				if ( pl != null )
				{
					pl.Faction.RemoveMember( mob );

					mob.SendMessage( "You have been kicked from your faction." );
					from.SendMessage( "They have been kicked from their faction." );
				}
				else
				{
					from.SendMessage( "They are not in a faction." );
				}
			}
			else
			{
				from.SendMessage( "That is not a player." );
			}
		}

		public static void ProcessTick()
		{
		}

		public static void HandleDeath( Mobile mob )
		{
			HandleDeath( mob, null );
		}

		#region Skill Loss
		public const double SkillLossFactor = 0.2;
		public static readonly double SkillLossPeriod = 3; // minutes

		private static Hashtable m_SkillLoss = new Hashtable();

		private class SkillLossContext
		{
			public Timer m_Timer;
			public ArrayList m_Mods;
		}

		public static void ApplySkillLoss( Mobile mob, bool suicide = false )
		{
			double loss_factor = SkillLossFactor;
			double loss_time = SkillLossPeriod;

			// Treasury reward items can modify duration and amount.
			// The modifications are accumulative
			foreach(Item ei in mob.Items)
			{
				Server.Custom.Townsystem.TreasuryReward treas_item = ei as Server.Custom.Townsystem.TreasuryReward;
				if(treas_item != null)
				{
					loss_factor = treas_item.ModifyFactionSkillLossAmount(loss_factor);
					loss_time = treas_item.ModifyFactionSkillLossDuration(loss_time);
				}
			}

            var pm = mob as PlayerMobile;
            if (pm.Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Reprieve))
                loss_time *= 0.9;

			SkillLossContext context = (SkillLossContext)m_SkillLoss[mob];

			if ( context != null )
				return;

			context = new SkillLossContext();
			m_SkillLoss[mob] = context;

			ArrayList mods = context.m_Mods = new ArrayList();

			for ( int i = 0; i < mob.Skills.Length; ++i )
			{
				Skill sk = mob.Skills[i];
				double baseValue = sk.Base;

				if ( baseValue > 0 )
				{
					SkillMod mod = new DefaultSkillMod(sk.SkillName, true, -(baseValue * loss_factor));

					mods.Add( mod );
					mob.AddSkillMod( mod );
				}
			}

            string msg;

            if (suicide)
                msg = "You have committed suicide and must suffer a temporary statloss.";
            else
                msg = "You have been slain by a faction enemy and must suffer a temporary statloss.";

            // Keep track of if a player is in stat loss for dousing/capping braziers
            var pmobile = mob as PlayerMobile;
            if (pmobile != null)
                pmobile.IsInStatLoss = true;

            Timer.DelayCall(TimeSpan.FromSeconds(3), delegate { mob.SendMessage(msg); });

			context.m_Timer = Timer.DelayCall(TimeSpan.FromMinutes(loss_time), new TimerStateCallback(ClearSkillLoss_Callback), mob);
		}

		private static void ClearSkillLoss_Callback( object state )
		{
			ClearSkillLoss( (Mobile) state );
		}

		public static bool ClearSkillLoss( Mobile mob )
		{
			SkillLossContext context = (SkillLossContext)m_SkillLoss[mob];

			if ( context == null ) {
				return false;
			}

			m_SkillLoss.Remove( mob );

			ArrayList mods = context.m_Mods;

			for ( int i = 0; i < mods.Count; ++i )
				mob.RemoveSkillMod( (SkillMod) mods[i] );

			context.m_Timer.Stop();

            // Keep track of if a player is in stat loss for dousing/capping braziers
            var pmobile = mob as PlayerMobile;
            if (pmobile != null)
                pmobile.IsInStatLoss = false;

			return true;
		}
		#endregion

		public static Faction FindSmallestFaction()
		{
			List<Faction> factions = Factions;
			Faction smallest = null;

			for ( int i = 0; i < factions.Count; ++i )
			{
				Faction faction = factions[i];

				if ( smallest == null || faction.Members.Count < smallest.Members.Count )
					smallest = faction;
			}

			return smallest;
		}

		public static bool StabilityActive()
		{
			return false;
		}

		public bool CanHandleInflux( int influx )
		{
			if( !StabilityActive())
				return true;

			Faction smallest = FindSmallestFaction();

			if ( smallest == null )
				return true; // sanity

			return true;
		}

		public static void HandleDeath( Mobile victim, Mobile killer )
		{
            if (victim.Player)
                if (((PlayerMobile)victim).IsInArenaFight)
                    return;

			if ( killer == null )
				killer = victim.FindMostRecentDamager( true );

			PlayerState killerState = PlayerState.Find( killer );

			if ( killerState == null )
				return;
            
			PlayerState victimState = PlayerState.Find( victim );

			if ( victimState == null )
				return;


			bool in_arena = victim.Region != null && (victim.Region is ArenaSystem.ArenaCombatRegion || victim.Region is BattlegroundRegion);
            in_arena = in_arena || ((victim is PlayerMobile) && ((PlayerMobile)victim).DuelContext != null);

            if ((killer == victim || killerState.Faction != victimState.Faction) && !in_arena)
                ApplySkillLoss(victim, killer == victim);

			
            // no point loss for arena/bg deaths/kills
			if ( killerState.Town != victimState.Town && !in_arena )
			{
				OCLeaderboard.RegisterKill(killer);

				if ( victimState.KillPoints <= -6 )
				{
					killer.SendLocalizedMessage( 501693 ); // This victim is not worth enough to get kill points from. 
				}
				else
				{
					int award = Math.Max( victimState.KillPoints / 10, 1 );

					if ( award > 40 )
						award = 40;

                    int keyAward = 20 * award;

                    Custom.Townsystem.Town town = Custom.Townsystem.Town.FromRegion(victim.Region);
                    if (town != null && Custom.Townsystem.OCTimeSlots.IsActiveTown(town))
                    {
                        if (town.KingOfTheHillTimer != null && town.KingOfTheHillTimer.Running)
                        {
                            var playerKiller = killer as PlayerMobile;
                            if (award > 0)
                                town.KingOfTheHillTimer.AddPoints(playerKiller.Citizenship, award * 5);
                        }
                    }

					if ( victimState.CanGiveKillPointsTo( killer ) )
					{
						victimState.IsActive = true;
                        killerState.IsActive = true;

						victimState.KillPoints -= award;
						killerState.KillPoints += award;
                        ((PlayerMobile)killer).TreasuryKeys += keyAward;

						// ACHIEVEMENT
                        AchievementSystem.Instance.TickProgress(killer, AchievementTriggers.Trigger_KillMiltiiaEnemy);
                        DailyAchievement.TickProgress(Category.PvP, (PlayerMobile)killer, PvPCategory.KillPlayers);
						// ACHIEVEMENT


                        //LEADERBOARD
						OCLeaderboard.RegisterKillPoints(killer, award);
						OCLeaderboard.RegisterKillPoints(victim, -award);

						int offset = ( award != 1 ? 0 : 2 ); // for pluralization

						string args = String.Format( "{0}\t{1}\t{2}", award, victim.Name, killer.Name );

						killer.SendLocalizedMessage( 1042737 + offset, args ); // Thou hast been honored with ~1_KILL_POINTS~ kill point(s) for vanquishing ~2_DEAD_PLAYER~!
                        killer.SendMessage(string.Format("Thou hast earned {0} treasury keys for vanquishing {1}.", keyAward, victim.Name));
						victim.SendLocalizedMessage( 1042738 + offset, args ); // Thou has lost ~1_KILL_POINTS~ kill point(s) to ~3_ATTACKER_NAME~ for being vanquished!

						victimState.OnGivenKillPointsTo( killer );
					}
					else
					{
						killer.SendLocalizedMessage( 1042231 ); // You have recently defeated this enemy and thus their death brings you no honor.
					}
				}
			}
		}

		private static void EventSink_Logout( LogoutEventArgs e )
		{
			Mobile mob = e.Mobile;

			Container pack = mob.Backpack;

			if ( pack == null )
				return;
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			Mobile mob = e.Mobile;

			CheckLeaveTimer( mob );
		}

		public static readonly Map Facet = Map.Felucca;

		public static void WriteReference( GenericWriter writer, Faction fact )
		{
			int idx = Factions.IndexOf( fact );

			writer.WriteEncodedInt( (int) (idx + 1) );
		}

		public static List<Faction> Factions{ get{ return Reflector.Factions; } }

		public static Faction ReadReference( GenericReader reader )
		{
			int idx = reader.ReadEncodedInt() - 1;

			if ( idx >= 0 && idx < Factions.Count )
				return Factions[idx];

			return null;
		}

		public static Faction Find( Mobile mob )
		{
			return Find( mob, false, false );
		}

		public static Faction Find( Mobile mob, bool inherit )
		{
			return Find( mob, inherit, false );
		}

		public static Faction Find( Mobile mob, bool inherit, bool creatureAllegiances )
		{
			PlayerState pl = PlayerState.Find( mob );

			if ( pl != null )
				return pl.Faction;

			if ( inherit && mob is BaseCreature )
			{
				BaseCreature bc = (BaseCreature)mob;

				if ( bc.Controlled )
					return Find( bc.ControlMaster, false );
				else if ( bc.Summoned )
					return Find( bc.SummonMaster, false );
                //else if ( creatureAllegiances && mob is BaseFactionGuard )
                //    return ((BaseFactionGuard)mob).Faction; // now used by custom town
				//else if ( creatureAllegiances )
					//return bc.FactionAllegiance;
			}

			return null;
		}

		public static Faction Parse( string name )
		{
			List<Faction> factions = Factions;

			for ( int i = 0; i < factions.Count; ++i )
			{
				Faction faction = factions[i];

				if ( Insensitive.Equals( faction.Definition.FriendlyName, name ) )
					return faction;
			}

			return null;
		}
	}

	public enum FactionKickType
	{
		Kick,
		Ban,
		Unban
	}

	public class FactionKickCommand : BaseCommand
	{
		private FactionKickType m_KickType;

		public FactionKickCommand( FactionKickType kickType )
		{
			m_KickType = kickType;

			AccessLevel = AccessLevel.GameMaster;
			Supports = CommandSupport.AllMobiles;
			ObjectTypes = ObjectTypes.Mobiles;

			switch ( m_KickType )
			{
				case FactionKickType.Kick:
				{
					Commands = new string[]{ "FactionKick" };
					Usage = "FactionKick";
					Description = "Kicks the targeted player out of his current faction. This does not prevent them from rejoining.";
					break;
				}
				case FactionKickType.Ban:
				{
					Commands = new string[]{ "FactionBan" };
					Usage = "FactionBan";
					Description = "Bans the account of a targeted player from joining factions. All players on the account are removed from their current faction, if any.";
					break;
				}
				case FactionKickType.Unban:
				{
					Commands = new string[]{ "FactionUnban" };
					Usage = "FactionUnban";
					Description = "Unbans the account of a targeted player from joining factions.";
					break;
				}
			}
		}

		public override void Execute( CommandEventArgs e, object obj )
		{
			Mobile mob = (Mobile)obj;

			switch ( m_KickType )
			{
				case FactionKickType.Kick:
				{
					PlayerState pl = PlayerState.Find( mob );

					if ( pl != null )
					{
						pl.Faction.RemoveMember( mob );
						mob.SendMessage( "You have been kicked from your faction." );
						AddResponse( "They have been kicked from their faction." );
					}
					else
					{
						LogFailure( "They are not in a faction." );
					}

					break;
				}
				case FactionKickType.Ban:
				{
					Account acct = mob.Account as Account;

					if ( acct != null )
					{
						if ( acct.GetTag( "FactionBanned" ) == null )
						{
							acct.SetTag( "FactionBanned", "true" );
							AddResponse( "The account has been banned from joining factions." );
						}
						else
						{
							AddResponse( "The account is already banned from joining factions." );
						}

						for ( int i = 0; i < acct.Length; ++i )
						{
							mob = acct[i];

							if ( mob != null )
							{
								PlayerState pl = PlayerState.Find( mob );

								if ( pl != null )
								{
									pl.Faction.RemoveMember( mob );
									mob.SendMessage( "You have been kicked from your faction." );
									AddResponse( "They have been kicked from their faction." );
								}
							}
						}
					}
					else
					{
						LogFailure( "They have no assigned account." );
					}

					break;
				}
				case FactionKickType.Unban:
				{
					Account acct = mob.Account as Account;

					if ( acct != null )
					{
						if ( acct.GetTag( "FactionBanned" ) == null )
						{
							AddResponse( "The account is not already banned from joining factions." );
						}
						else
						{
							acct.RemoveTag( "FactionBanned" );
							AddResponse( "The account may now freely join factions." );
						}
					}
					else
					{
						LogFailure( "They have no assigned account." );
					}

					break;
				}
			}
		}
	}
}