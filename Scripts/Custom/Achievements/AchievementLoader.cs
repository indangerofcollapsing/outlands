using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;


namespace Server.Achievements
{
	public static class AchievementLoader
	{
		public static bool Load(out List<AchievementTrigger> out_triggers, 
                                out List<Achievement> out_achievements, 
                                out List<AchievementCategory> out_category_lookup, 
                                out AchievementSystemImpl.AchievementSystemTweaks sys_tweaks, 
                                out List<CompositeAchievementTrigger> out_composite_triggers)
		{
			out_triggers = null;
			out_achievements = new List<Achievement>();
			out_category_lookup = new List<AchievementCategory>();
			sys_tweaks = new AchievementSystemImpl.AchievementSystemTweaks();
            out_composite_triggers = new List<CompositeAchievementTrigger>();

			if (!System.IO.File.Exists("Data/achievements.xml"))
			{
				Console.WriteLine("Error: Data/achievements.xml does not exist");
				return false;
			}

			Console.Write("Achievements: Loading...");
			XmlDocument doc = new XmlDocument();
			doc.Load(System.IO.Path.Combine(Core.BaseDirectory, "Data/achievements.xml"));
			XmlElement root = doc["Root"];
			if (root == null)
			{
				Console.WriteLine("Could not find root element 'Root' in achievements.xml");
			}
			else
			{
				// <Achievements>
				XmlNode achievement_root = root.SelectSingleNode("Achievements");
				XmlNodeList triggerlist = achievement_root.SelectNodes("achievementTrigger");
				out_triggers = new List<AchievementTrigger>((int)AchievementTriggers.Trigger_NumTriggers);
				for (int i = 0; i < out_triggers.Capacity; ++i)
					out_triggers.Insert(i, new AchievementTrigger("NONE SET"));
				foreach (XmlElement trigger in triggerlist)
				{
					ReadTrigger(trigger, ref out_triggers, ref out_achievements, ref out_category_lookup);
				}


                // <CompositeAchievements>
                XmlNode comptrigger_root = root.SelectSingleNode("CompositeAchievements");
                XmlNodeList comp_trigger_list = comptrigger_root.SelectNodes("compositetrigger");
                foreach (XmlElement composite_trigger in comp_trigger_list)
                {
                    out_composite_triggers.Add( ReadCompositeTrigger(composite_trigger, ref out_triggers, out_composite_triggers.Count) );
                }


				// <SystemTweaks>
				XmlNode tweaks_xml = root.SelectSingleNode("SystemTweaks");
				ReadTweaks(tweaks_xml, ref sys_tweaks);
			}


			// sort categories in static order
			for (int i = 0; i < AchievementSystemImpl.m_CategorySortOrder.Count; ++i)
			{
				for (int j = 0; j < out_category_lookup.Count; ++j)
				{
					if (out_category_lookup[j].m_Category == AchievementSystemImpl.m_CategorySortOrder[i] )
					{	// swap to category to correct place
						if (i != j) // already in correct place?
						{
							AchievementCategory tmp = out_category_lookup[i]; // swap
							out_category_lookup[i] = out_category_lookup[j];
							out_category_lookup[j] = tmp;
						}
					}
				}
			}


			Console.WriteLine("done ({0} achievements)", out_achievements.Count);
			return true;
		}

		// Read XML hierarchy
		private static Achievement ReadAchievement(XmlElement xml_achievement, int triggerid)
		{
			string triggercount = xml_achievement.GetAttribute("triggercount");
			string item_graphics = xml_achievement.GetAttribute("item_graphics");
			string item_name = xml_achievement.GetAttribute("item_name");
            string item_amount = xml_achievement.GetAttribute("item_amount");
			string rewarddescription = xml_achievement.GetAttribute("reward_onclickdesc");
			string achievementname = xml_achievement.GetAttribute("achievement_name");
			string gfxitemoffset = xml_achievement.GetAttribute("gfxoffset");
            string rewardhue = xml_achievement.GetAttribute("itemhue");
			string reward_window_shortdesc = xml_achievement.GetAttribute("reward_window_shortdesc");
			string timelimit = xml_achievement.GetAttribute("time_limit");

			if (String.IsNullOrEmpty(triggercount))
				Console.WriteLine("Warning: Data/achievements.xml - malformed achievement (triggercount) under trigger {0}", triggerid);
			else if (String.IsNullOrEmpty(achievementname))
				Console.WriteLine("Warning: Data/achievements.xml - malformed achievement (achievement_name) under trigger {0}", triggerid);
			else if(String.IsNullOrEmpty(rewardhue))
				Console.WriteLine("Warning: Data/achievements.xml - malformed achievement (itemhue) under trigger {0}", triggerid);
			else
			{
				Achievement new_achievement = new Achievement(achievementname);

				new_achievement.m_RewardDescription = rewarddescription;
				new_achievement.m_RewardWindowShortDesc = reward_window_shortdesc;
				Int32.TryParse(triggercount, out new_achievement.m_TriggerCountRequired);

				if (!String.IsNullOrEmpty(gfxitemoffset))
				{
					string[] tokens = gfxitemoffset.Split(' ');
					int x,y;
					if (tokens.Length == 2 && Int32.TryParse(tokens[0], out x) && Int32.TryParse(tokens[1], out y))
					{
						new_achievement.m_RewardItemGraphicsOffset.X = x;
						new_achievement.m_RewardItemGraphicsOffset.Y = y;
					}
				}

				new_achievement.m_RewardTitle = xml_achievement.GetAttribute("reward_title_prefix");
				new_achievement.m_RewardTitle2 = xml_achievement.GetAttribute("reward_title_suffix");
				new_achievement.m_MustBeDiscovered = xml_achievement.HasAttribute("must_be_discovered");
				new_achievement.m_RewardHue = Int32.Parse(rewardhue, System.Globalization.NumberStyles.Integer);

				// reward amount
                int tmp = 0;
                if (item_amount.Length > 0 && Int32.TryParse(item_amount, out tmp))
                    new_achievement.m_RewardItemAmount = tmp;

				// timelimit
				if (!String.IsNullOrEmpty(timelimit) && Int32.TryParse(timelimit, out tmp))
					new_achievement.m_TimeLimitSeconds = tmp;

				// graphics ID for reward window and for simple reward.
				new_achievement.m_RewardItemGraphics = -1;
				int parsed_hexnumber = 0;
				if (item_graphics.Length > 0 && Int32.TryParse(item_graphics, System.Globalization.NumberStyles.HexNumber, null, out parsed_hexnumber))
				{
					new_achievement.m_RewardItemGraphics = parsed_hexnumber;
				}

				// reward item type for complex (usable) rewards
				new_achievement.m_RewardItemType = ScriptCompiler.FindTypeByName(item_name);
				new_achievement.m_TriggerID = triggerid;
				return new_achievement;
			}
			return null;
		}
		private static void ReadTrigger(XmlElement xml_trigger, ref List<AchievementTrigger> out_triggers, ref List<Achievement> out_achievements, ref List<AchievementCategory> out_category_lookup)
		{
			string id_str = xml_trigger.GetAttribute("id");
			string category_str = xml_trigger.GetAttribute("category");
			string description = xml_trigger.GetAttribute("description");

			int trigger_id = 0;

			// validate properties
			if (String.IsNullOrEmpty(category_str))
				Console.WriteLine("Warning: Data/achievements.xml - trigger without category set");
			else if ((String.IsNullOrEmpty(id_str) || !Int32.TryParse(id_str, out trigger_id)))
				Console.WriteLine("Warning: Data/achievements.xml - trigger without a proper ID set.");
			else if (String.IsNullOrEmpty(description))
				Console.WriteLine("Warning: Data/achievements.xml - trigger without a description set.");
			else
			{
				// create category lookup table if it doesn't already exist
				AchievementCategory category = null;
				for (int i = 0; i < out_category_lookup.Count; ++i)
				{
					if (out_category_lookup[i].m_Category == category_str)
					{
						category = out_category_lookup[i];
						break;
					}
				}
				if (category == null)
				{
					category = new AchievementCategory(category_str);
					out_category_lookup.Add(category);
				}

				// read achievements tied to this trigger
				AchievementTrigger new_trigger = new AchievementTrigger(description);
				foreach (XmlElement xml_achievement in xml_trigger.SelectNodes("achievement"))
				{
					Achievement new_achievement = ReadAchievement(xml_achievement, trigger_id);
					Debug.Assert(new_achievement != null, "Failed creating achievements, check the log");
					category.m_AchievementIndices.Add(out_achievements.Count);
					new_trigger.m_Achievements.Add(out_achievements.Count);
					out_achievements.Add(new_achievement);
				}

				Debug.Assert(out_triggers.Count > trigger_id, "TRIGGER INDEX OUT OF RANGE.", "Did you forget to add the new trigger in code?");
				out_triggers[trigger_id] = new_trigger;
			}
		}

		private static void ReadTweaks(XmlNode tweaks_xml, ref AchievementSystemImpl.AchievementSystemTweaks sys_tweaks)
		{
			XmlElement award_message = tweaks_xml["award_message"];
			if( award_message != null )
			{
				sys_tweaks.m_AwardMsgString = award_message.GetAttribute("string");
				sys_tweaks.m_AwardMsgColor = Int32.Parse(award_message.GetAttribute("color"), System.Globalization.NumberStyles.HexNumber);
				sys_tweaks.m_AwardSoundID = Int32.Parse(award_message.GetAttribute("sound_id"), System.Globalization.NumberStyles.HexNumber);
			}

			XmlElement progress_message = tweaks_xml["progress_msg"];
			if( progress_message != null )
			{
				sys_tweaks.m_ProgressMsgString = progress_message.GetAttribute("string");
				sys_tweaks.m_ProgressMsgColor = Int32.Parse(progress_message.GetAttribute("color"), System.Globalization.NumberStyles.HexNumber);
			}

			XmlElement tracker_title = tweaks_xml["tracker_title"];
			if( progress_message != null )
			{
				sys_tweaks.m_TrackerTitle = tracker_title.GetAttribute("string");
			}
		}

        private static CompositeAchievementTrigger ReadCompositeTrigger(XmlElement composite_trigger_xml, ref List<AchievementTrigger> all_triggers, int triggerindex)
        {
            string in_triggers = composite_trigger_xml.GetAttribute("intriggers");
            string out_trigger = composite_trigger_xml.GetAttribute("outtrigger");

            CompositeAchievementTrigger comptrigger = new CompositeAchievementTrigger();
            string[] intriggerlist = in_triggers.Split(' ');
            foreach(string it in intriggerlist)
                comptrigger.m_InTriggers.Add((AchievementTriggers)Int32.Parse(it, System.Globalization.NumberStyles.Integer));

            comptrigger.m_OutTrigger = (AchievementTriggers)Int32.Parse(out_trigger, System.Globalization.NumberStyles.Integer);

            // set back-reference in all basetriggers for quick reverse lookup
            foreach(AchievementTriggers basetrigger in comptrigger.m_InTriggers)
            {
                all_triggers[(int)basetrigger].m_CompositeTriggerID = triggerindex;
            }
            return comptrigger;
        }
        
	}
}
