using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Achievements;

namespace Server.Achievements
{
	class AchivementSpecialRewards
	{
		private static Type WispLanternType = typeof(WispLantern);

		public static void Modify(Achievement achievement, Item reward_item)
		{
			if (achievement.m_RewardItemType == WispLanternType)
			{
				// rename pet and set pethue
				((WispLantern)reward_item).SetWispHue(achievement.m_RewardHue);
				((WispLantern)reward_item).SetWispName(achievement.m_RewardDescription);
			}
		}
	}
}
