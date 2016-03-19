using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml;
using System.IO;
using System.Text;

using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Achievements;
using Server.Accounting;
using Server.Targeting;
using Server.Commands.Generic;

namespace Server.Achievements
{
	//////////////////////////////////
	// AchievementCommands: Ingame commands
	//////////////////////////////////
	public static class AchievementCommands
	{
		public static void Initialize()
		{
            CommandSystem.Register("ach", AccessLevel.Player, new CommandEventHandler(ShowAchievementTracker));
			CommandSystem.Register("achreload", AccessLevel.GameMaster, new CommandEventHandler(ReloadAchievementXML));
            CommandSystem.Register("achtick", AccessLevel.GameMaster, new CommandEventHandler(TickAchievementTrigger));
            CommandSystem.Register("achset", AccessLevel.GameMaster, new CommandEventHandler(SetAchievementTriggerCount));
            CommandSystem.Register("achclear", AccessLevel.GameMaster, new CommandEventHandler(ClearAchievementTriggerCount));
		}

		[Usage("[ach")]
		[Description("Shows the achievement tracker")]
		private static void ShowAchievementTracker(CommandEventArgs e)
		{
            Mobile mobile = e.Mobile;

            mobile.SendGump(new AchievementTrackerGump(mobile, mobile, -1, 0));
		}

		[Usage("[achreload")]
		[Description("Reloads the achievement configuration XML")]
		private static void ReloadAchievementXML(CommandEventArgs e)
		{
            AchievementSystemImpl.Instance.LoadFromXML();
		}

		[Usage("[achtick <trigger_id> <amount>")]
		[Description("Ticks an achievement trigger X amount of times")]
		private static void TickAchievementTrigger(CommandEventArgs e)
		{
			if (e.Arguments.Length == 2)
			{
				int count = e.GetInt32(1);
				for (int i = 0; i < count; ++i)
					AchievementSystemImpl.Instance.TickProgress(e.Mobile, (AchievementTriggers)e.GetInt32(0));
			}
		}

		[Usage("[achset <trigger_id> <value>")]
		[Description("Sets an absolute value of an achievement trigger")]
		private static void SetAchievementTriggerCount(CommandEventArgs e)
		{
			if( e.Arguments.Length == 2 )
                AchievementSystemImpl.Instance.SetProgress(e.Mobile, (UInt16)e.GetInt32(0), e.GetInt32(1));
		}

		[Usage("[achclear ")]
		[Description("Clears all achievement progress")]
		private static void ClearAchievementTriggerCount(CommandEventArgs e)
		{
			// clear all progress and claim status
			PlayerMobile pm = e.Mobile as PlayerMobile;
            PlayerAccomplishments player_achievements = (pm.Account as Account).AccountAchievements;
            for (int i = 0; i < AchievementSystemImpl.Instance.m_AllAchievements.Count; ++i)
			{
                player_achievements.ClearHasBeenRewarded(AchievementSystemImpl.Instance.m_AllAchievements[i]);
			}
            for (int i = 0; i < AchievementSystemImpl.Instance.m_AllAchievementTriggers.Count; ++i)
			{
                player_achievements.SetTriggerProgress(i, 0);
			}
		}
        [Usage("[achinspect ")]
		[Description("Inspect achievements status of another player")]
        private class AchievementInspectTarget : Target
        {
            public AchievementInspectTarget() : base(-1, true, TargetFlags.None) { }
            protected override void OnTarget(Mobile from, object o)
            {
                if(o is PlayerMobile)
                    from.SendGump(new AchievementTrackerGump(o as Mobile, from, 0, 0));
            }
        }
		private static void InspectAchievements(CommandEventArgs e)
		{
			// clear all progress and claim status
            e.Mobile.Target = new AchievementInspectTarget();
		}
        
	}

	//////////////////////////////////
	// PlayerAccomplishments: The number of times a player has triggered achievement triggers
	//////////////////////////////////
	public class PlayerAccomplishments
	{
        private int[] m_AchievementTriggers;	// m_AchievementTriggers[i] is the number of times the trigger 'i' has triggered for this character.
		private SortedSet<int> m_ClaimedRewards; // contains the hashstrings of the rewards the player has claimed.
        private int DATA_VERSION = 2;
		public int m_NumUnclaimedAchievements;
		public bool m_ShowNotifications;

		public Dictionary<int, LinkedList<long>> m_ActiveTimers; // Active timers for timebased achievements, not serialized.

		public PlayerAccomplishments()
		{
			m_ShowNotifications = true;
			m_AchievementTriggers = new int[(int)AchievementTriggers.Trigger_NumTriggers];
			m_ClaimedRewards = new SortedSet<int>();
			m_NumUnclaimedAchievements = 0;
			m_ActiveTimers = new Dictionary<int, LinkedList<long>>();
		}

        public void ReadFrom(string strdata)
        {
            if (strdata.Length == 0)
                return; // nothing to do here, new account probably.

            // from string to stream (slow)
            String[] arr = strdata.Split('-');
            byte[] data = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
                data[i] = Convert.ToByte(arr[i], 16);

            // version
            int offset = 0;
            int read_version = BitConverter.ToInt32(data, offset); offset += sizeof(int);
            switch(read_version)
            {
				case 2:
					{
						UInt16 user_options = BitConverter.ToUInt16(data, offset);
						offset += sizeof(UInt16);
						m_ShowNotifications = user_options == 1;
						goto case 1;
					}
                case 1:
					{
						goto case 0;
					}
                case 0:
					{
						// trigger counts
						ushort read_numtriggervalues = BitConverter.ToUInt16(data, offset); offset += sizeof(ushort);
						for (int i = 0; i < read_numtriggervalues; ++i)
						{
							m_AchievementTriggers[i] = BitConverter.ToInt32(data, offset);
							offset += sizeof(Int32);
						}

						// claimed rewards
						ushort read_numclaimed = BitConverter.ToUInt16(data, offset); offset += sizeof(ushort);
						for (int i = 0; i < read_numclaimed; ++i)
						{
							m_ClaimedRewards.Add(BitConverter.ToInt32(data, offset));
							offset += sizeof(Int32);
						}
						break;
					}
            }
        }

        public string SaveTo()
        {
            // save data to memory buffer.
            MemoryStream mem_stream = new MemoryStream();

            // version
            mem_stream.Write(BitConverter.GetBytes(DATA_VERSION), 0, sizeof(int));

			// user options
			UInt16 user_options = m_ShowNotifications ? (UInt16)1 : (UInt16)0;
			mem_stream.Write(BitConverter.GetBytes(user_options), 0, sizeof(UInt16));
            
            // trigger counts
			ushort numvalues = (ushort)m_AchievementTriggers.Length;
            mem_stream.Write( BitConverter.GetBytes(numvalues), 0, sizeof(ushort) );
            foreach (Int32 triggercount in m_AchievementTriggers)
                mem_stream.Write( BitConverter.GetBytes(triggercount), 0, sizeof(Int32) );

            // claimed rewards
            ushort numclaimed = (ushort)m_ClaimedRewards.Count;
            mem_stream.Write(BitConverter.GetBytes(numclaimed), 0, sizeof(ushort));
			foreach (Int32 hash in m_ClaimedRewards)
                mem_stream.Write(BitConverter.GetBytes(hash), 0, sizeof(Int32));


            // FROM STREAM - TO STRING
            mem_stream.Position = 0;
            byte[] out_array = new byte[mem_stream.Length];
            mem_stream.Read(out_array, 0, (int)mem_stream.Length);
            string s = BitConverter.ToString(out_array, 0);

            return s;
        }

		public int GetTriggerProgress(int trigger_id)
		{
			return m_AchievementTriggers[trigger_id];
		}

		public bool GetIsCompleted(Achievement achievement)
		{
			return m_AchievementTriggers[achievement.m_TriggerID] >= achievement.m_TriggerCountRequired;
		}

		public bool GetHasBeenRewarded(Achievement achievement)
		{
			return m_ClaimedRewards.Contains(achievement.m_NameHash);
		}

		public void SetHasBeenRewarded(Achievement achievement)
		{
			m_ClaimedRewards.Add(achievement.m_NameHash);
		}

		public void ClearHasBeenRewarded(Achievement achievement)
		{
			m_ClaimedRewards.Remove(achievement.m_NameHash);
		}

		// Set progress
		public void SetTriggerProgress(int trigger, int value)
		{
			m_AchievementTriggers[trigger] = value;
		}

		// Returns the new count
		public int TickProgress(int trigger, int amount)
		{
			m_AchievementTriggers[trigger] += amount;
			return m_AchievementTriggers[trigger];
		}
	}

	//////////////////////////////////
	// AchievementTrigger: 
	//		A trigger is some sort of game event that can be tied to one or more achievements.
	//		All triggers are enumerated in AchievementTriggers.cs
	//////////////////////////////////
	public class AchievementTrigger
	{
		public AchievementTrigger(string description)
		{
			m_Achievements = new List<int>();
			m_Description = description;
            m_CompositeTriggerID = -1;
		}
		public List<int>	m_Achievements; // Indices to the achievements that are triggered by this trigger.
		public string		m_Description;	// Describes what needs to be done for this trigger to fire.
        public int          m_CompositeTriggerID;
	}

    public class CompositeAchievementTrigger
    {
        public CompositeAchievementTrigger()
        {
            m_InTriggers = new List<AchievementTriggers>();
        }

        public bool IsCompleted(PlayerAccomplishments player_state)
        {
            foreach (AchievementTriggers basetrigger in m_InTriggers)
            {
                foreach (int achievement_id in AchievementSystemImpl.Instance.m_AllAchievementTriggers[(int)basetrigger].m_Achievements)
                {
                    if (!player_state.GetIsCompleted(AchievementSystemImpl.Instance.m_AllAchievements[achievement_id]))
                        return false;
                }
            }
            return true;
        }
        public List<AchievementTriggers>    m_InTriggers; // Indices to the achievements that are triggered by this trigger.
        public AchievementTriggers          m_OutTrigger;	// Describes what needs to be done for this trigger to fire.
    }

	//////////////////////////////////
	// AchievementCategory: Helper class for categorization of achievements
	//////////////////////////////////
	public class AchievementCategory
	{
		public string m_Category;
		public List<int> m_AchievementIndices;
		public AchievementCategory(string category)
		{
			m_Category = category;
			m_AchievementIndices = new List<int>();
		}
	}

	//////////////////////////////////
	// Achievement: A static representation of a single achievement. 
	//////////////////////////////////
	public class Achievement
	{
		public string				m_Name;						// displayed name
		public int					m_NameHash;					// cached

		public int					m_TriggerCountRequired;		
		public int					m_TriggerID;

		public Point2D				m_RewardItemGraphicsOffset; // for the award gump
		public string				m_RewardDescription;		
		public string				m_RewardWindowShortDesc;	// for the award gump only
		public int					m_RewardItemGraphics;		// If reward is a simple item then this is the item id
		public Type					m_RewardItemType;			// For complex rewards.
        public int                  m_RewardItemAmount;		
        public string				m_RewardTitle;
        public string				m_RewardTitle2;				
		public int					m_RewardHue;
		public bool					m_MustBeDiscovered;
		public int					m_TimeLimitSeconds;			// if != 0 the achievement must be completed within this many seconds
		
		public Achievement(string name)
        {
            m_Name = name;
			m_NameHash = name.GetHashCode();

			m_RewardItemGraphicsOffset = Point2D.Zero;
			m_MustBeDiscovered = false;
            m_RewardItemAmount = 1;
        }
	}

	//////////////////////////////////
	// AchievementSystem: Management of achievements, gains, increases etc.
	//////////////////////////////////
    public class AchievementSystemImpl : AchievementSystemBase
	{
		public class AchievementSystemTweaks
		{
			public string	m_AwardMsgString;
			public int		m_AwardMsgColor;
			public int		m_AwardSoundID;
			public string	m_ProgressMsgString;
			public int		m_ProgressMsgColor;
			public string	m_TrackerTitle;
		}

		public static List<string> m_CategorySortOrder = new List<string> { "PvP", "PvE", "Crafting", "Taming", "Gathering", "Exploration", "Positive", "Wealth", "Time Challenges", "Mastery", "Veteran" };

		public List<AchievementTrigger> m_AllAchievementTriggers; // triggers link between the action and the awarded achievement(s)
		public List<Achievement> m_AllAchievements;
		public List<AchievementCategory> m_CategoriesLookup; // precomputed list for quicker lookup of e.g "all PvE achievements"
        public List<CompositeAchievementTrigger> m_CompositeAchievementTriggers;
        public AchievementSystemTweaks m_SysTweaks;
        public static AchievementSystemImpl Instance;

		private static Type m_EntityType = typeof(IEntity);

        static AchievementSystemImpl()
        {
            Instance = new AchievementSystemImpl();
            AchievementSystem.Instance = Instance as AchievementSystemBase;
        }

        private void CheckCompositeAchievements(PlayerMobile owner, Achievement achievement, PlayerAccomplishments player_state)
        {
            int ct_id = AchievementSystemImpl.Instance.m_AllAchievementTriggers[achievement.m_TriggerID].m_CompositeTriggerID;
            if (ct_id >= 0 && AchievementSystemImpl.Instance.m_CompositeAchievementTriggers[ct_id].IsCompleted(player_state))
            {
                AchievementSystem.Instance.TickProgress(owner, AchievementSystemImpl.Instance.m_CompositeAchievementTriggers[ct_id].m_OutTrigger);
            }
        }

		//
		public void TryClaimReward(Mobile mobile, int achievement_index)
		{
            Debug.Assert(mobile.Player);
			PlayerMobile player_mobile = mobile as PlayerMobile;
			Achievement achievement = m_AllAchievements[achievement_index];
            PlayerAccomplishments player_achievements = (player_mobile.Account as Account).AccountAchievements;
            
            if (player_achievements.GetIsCompleted(achievement) && !player_achievements.GetHasBeenRewarded(achievement))
			{
				// Item reward(s)
				Item reward_item = null;
				if (achievement.m_RewardItemType != null)
				{
					//complex reward or no item reward at all
					if (!m_EntityType.IsAssignableFrom(achievement.m_RewardItemType))
					{
						Debug.Assert(false, "AchievementSystem could not create reward item, no such item type");
					}
					else
					{
						object obj = Activator.CreateInstance(achievement.m_RewardItemType);
						if (obj is Item)
						{
							reward_item = obj as Item;

							// and the hacks goes in here.
							AchivementSpecialRewards.Modify(achievement, reward_item);
						}
					}
				}
				else if (achievement.m_RewardItemGraphics >= 0)
				{
					// simple item
					reward_item = new Item(achievement.m_RewardItemGraphics);
				}
                //else : no item reward

                if (reward_item != null)
                {
                    if( achievement.m_RewardDescription.Length > 0 )
                        reward_item.Name = achievement.m_RewardDescription;
                    if(achievement.m_RewardItemAmount > 0)
                        reward_item.Amount = achievement.m_RewardItemAmount;

                    reward_item.Hue = achievement.m_RewardHue;

                    // Put it in the bank
                    if (player_mobile.BankBox.TryDropItem(player_mobile, reward_item, false))
                    {
                        player_achievements.SetHasBeenRewarded(achievement);
                        CheckCompositeAchievements(player_mobile, achievement, player_achievements);
                        player_mobile.SendMessage(44, "Your reward has been placed in your bank box.");

                    }
                    else
                    {
                        player_mobile.SendMessage(0x26, "Could not claim reward. Your bank box is full");
                    }
                }
                else // no item reward
                {
                    player_achievements.SetHasBeenRewarded(achievement);
                    CheckCompositeAchievements(player_mobile, achievement, player_achievements);
                }
			}
		}

		// Sparkles (unless player is hidden)!
		private void OnAchievementCompleted(PlayerMobile player, Achievement achievement, int achievement_index, bool do_popups)
		{
			object[] args = { achievement.m_Name };
            player.SendMessage(AchievementSystemImpl.Instance.m_SysTweaks.m_AwardMsgColor, AchievementSystemImpl.Instance.m_SysTweaks.m_AwardMsgString, args);

			CheckCompositeAchievements(player, achievement, (player.Account as Account).AccountAchievements);

			if (do_popups)
				player.SendGump(new AchievementNotificationGump(player, achievement_index));

			if (!player.Hidden && player.AccessLevel == AccessLevel.Player)
			{
				// sound and some sparkles
				player.PlaySound(AchievementSystemImpl.Instance.m_SysTweaks.m_AwardSoundID);
				Effects.SendLocationEffect(player, player.Map, 0x375a, 48, 10);
				Point3D startLoc = new Point3D(player.X, player.Y, player.Z + 10);
				float[] times = { 0.9f, 0.9f, 1.0f, 1.5f, 1.8f, 2.5f };
				for (int i = 0; i < 6; ++i)
				{
					Point3D endLoc = new Point3D(startLoc.X + Utility.RandomMinMax(-2, 2), startLoc.Y + Utility.RandomMinMax(-2, 2), startLoc.Z + 32);
					Effects.SendMovingEffect(new Entity(Serial.Zero, startLoc, player.Map), new Entity(Serial.Zero, endLoc, player.Map), 0x36E4, 5, 0, false, false);
					Timer.DelayCall(TimeSpan.FromSeconds(times[i]), new TimerStateCallback(FinishAwardSparkles), new object[] { player, endLoc, player.Map });
				}
			}
		}

		private static void FinishAwardSparkles(object state)
		{
			object[] states = (object[])state;
			int hue = Utility.RandomList(0x47E, 0x47F, 0x480, 0x482, 0x66D, 0x0, 0x0, 0x0);
			int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);
			Effects.PlaySound((Point3D)states[1], (Map)states[2], Utility.Random(0x11B, 4));
			Effects.SendLocationEffect((Point3D)states[1], (Map)states[2], 0x373A + (0x10 * Utility.Random(4)), 16, 10, hue, renderMode);
		}

		private bool UpdateAchievementTimer(Achievement achievement, PlayerAccomplishments player_state, Mobile owner)
		{
			LinkedList<long> time_entries = player_state.m_ActiveTimers.ContainsKey(achievement.m_NameHash) ? player_state.m_ActiveTimers[achievement.m_NameHash] : null;
		
			// clear out entries older than the achievement time limit
			if (time_entries != null)
			{
				while (time_entries.Count > 0)
				{
					if ((DateTime.UtcNow - DateTime.FromBinary(time_entries.First.Value)).TotalSeconds > achievement.m_TimeLimitSeconds)
						time_entries.RemoveFirst();
					else
						break; // elements are sorted by age, oldest first
				}
			}
			else
			{
				time_entries = new LinkedList<long>();
				player_state.m_ActiveTimers.Add(achievement.m_NameHash, time_entries);
			}

			time_entries.AddLast(DateTime.UtcNow.ToBinary());
			player_state.SetTriggerProgress(achievement.m_TriggerID, time_entries.Count);

			// clean out if achievement got completed this time
			if (time_entries.Count >= achievement.m_TriggerCountRequired)
			{
				player_state.m_ActiveTimers.Remove(achievement.m_NameHash);
				return true;
			}
			else
			{
				if (player_state.m_ShowNotifications && time_entries.Count > 3) // don't show this until progressed a bit
					owner.PrivateOverheadMessage(Network.MessageType.Regular, 2125, false, String.Format("[{0}/{1}]", time_entries.Count, achievement.m_TriggerCountRequired), owner.NetState);
				return false;
			}
		}

		// Tick progress X number of times
		// This is the most time critical part of the system.
		public void TickProgress(Mobile mobile, AchievementTriggers trigger)
		{
			TickProgressMulti(mobile, trigger, 1);
		}


		public void TickProgressMulti(Mobile mobile, AchievementTriggers trigger, int amount)
		{
            PlayerMobile player = mobile as PlayerMobile;

            if (player == null)
                return;

            //Region Specific Override
            if (player.IsInUOACZ && !UOACZAchievements.m_AchievementsList.Contains(trigger))
                return;

			UInt16 trigger_index = (UInt16)trigger;
            #if DEBUG
			            if (trigger_index > (UInt16)AchievementTriggers.Trigger_NumTriggers)
				            return; // not a valid trigger
            #endif
			Debug.Assert(mobile.Player);
			Debug.Assert(amount >= 0);

			PlayerMobile player_mobile = mobile as PlayerMobile;
			if (player_mobile == null || player_mobile.Account == null)
				return; // we had a random startup crash here, couldn't reproduce so guarding here and hoping for the best... :/

			PlayerAccomplishments player_state = (player_mobile.Account as Account).AccountAchievements;

			int before = player_state.GetTriggerProgress(trigger_index);
			int after = player_state.TickProgress(trigger_index, amount);

			bool notified = false;
			for (int i = 0; i < m_AllAchievementTriggers[trigger_index].m_Achievements.Count; ++i)
			{
				int achievement_index = m_AllAchievementTriggers[trigger_index].m_Achievements[i];
				Achievement achievement = m_AllAchievements[achievement_index];
				if (before >= achievement.m_TriggerCountRequired )
				{
					continue; // this must be done or timebased achievements will reset
				}

				// TRICK! The timebased achievements keep their "tickcount" internally until it's completed, at which point the achievement is awarded.
				//			UpdateAchievementTimer takes care of all that simply returns true if the timebased achievement is completed
				if (achievement.m_TimeLimitSeconds > 0)
				{
					if (UpdateAchievementTimer(achievement, player_state, mobile))
						OnAchievementCompleted(player_mobile, achievement, achievement_index, player_state.m_ShowNotifications);
				}
				else if (achievement.m_TriggerCountRequired > before && after >= achievement.m_TriggerCountRequired)
				{
					OnAchievementCompleted(player_mobile, achievement, achievement_index, player_state.m_ShowNotifications);
				}
				else if (before == 0 && after > 0 && !notified)
				{
					player_mobile.SendMessage(AchievementSystemImpl.Instance.m_SysTweaks.m_ProgressMsgColor, AchievementSystemImpl.Instance.m_SysTweaks.m_ProgressMsgString);
					object[] args = { achievement.m_Name, achievement.m_TriggerCountRequired };
					player_mobile.SendMessage(AchievementSystemImpl.Instance.m_SysTweaks.m_ProgressMsgColor, "\"{0}\" - 1/{1}", args);
					notified = true; // don't spam the player
				}
			}
		}

		// Force a progress value (debug only)
		internal void SetProgress(Mobile mobile, UInt16 trigger, int value)
		{
			if (trigger >= (UInt16)AchievementTriggers.Trigger_NumTriggers)
				return; // not a valid trigger

            Debug.Assert(mobile.Player);
			PlayerMobile player_mobile = mobile as PlayerMobile;

            PlayerAccomplishments player_achievements = (player_mobile.Account as Account).AccountAchievements;
			player_achievements.SetTriggerProgress(trigger, Math.Max(0, value));

			for(int i = 0; i < m_AllAchievementTriggers[trigger].m_Achievements.Count; ++i )
				player_achievements.ClearHasBeenRewarded(m_AllAchievements[m_AllAchievementTriggers[trigger].m_Achievements[i]]); 
		}

		public static void PostLoad()
		{
			// Generate initial report
			AchievementsReport.GenerateReport();
		}
		public static void Save(WorldSaveEventArgs e)
		{
			AchievementsReport.GenerateReport();
			AchievementsReport.GenerateHTML();
		}
		public static void Initialize()
		{
			EventSink.WorldSave += new WorldSaveEventHandler(Save);
			EventSink.ServerStarted += new ServerStartedEventHandler(PostLoad);

			// load xml
			AchievementSystemImpl.Instance.LoadFromXML();

			// count non-claimed achievements
			foreach (IAccount iacc in Accounts.GetAccounts())
			{
				Account acc = (Account)iacc;
				PlayerAccomplishments account_achievements = acc.AccountAchievements;
				foreach (Achievement ach in AchievementSystemImpl.Instance.m_AllAchievements)
				{
					if (account_achievements.GetIsCompleted(ach) && !account_achievements.GetHasBeenRewarded(ach))
						++account_achievements.m_NumUnclaimedAchievements;
				}
			}
		}

		public void LoadFromXML()
        {
            // parse XML, create achievements and lookup tables
            Achievements.AchievementLoader.Load(out m_AllAchievementTriggers, out m_AllAchievements, out m_CategoriesLookup, out m_SysTweaks, out m_CompositeAchievementTriggers);

			// make sure no two achievements have the same hash, that is a fatal error
			HashSet<int> allhashes = new HashSet<int>();
            foreach (Achievement achievement in m_AllAchievements)
			{
				// Either two achievements with the same name or hash collision
				object[] args = { achievement.m_Name };
				Debug.Assert( !allhashes.Contains(achievement.m_NameHash), " ", "ACHIEVEMENT NAME COLLISION. THIS NAME MUST BE CHANGED: \"{0}\"", args ); 
				allhashes.Add(achievement.m_NameHash);
			}
        }
	};
}

