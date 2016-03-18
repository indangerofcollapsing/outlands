using Server.Gumps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.ExtensionMethods;

namespace Server.Achievements
{
    public class DailyAchievementTrackerGump : Gump
    {
        private Mobile m_AchievementOwner;
        private Mobile m_Beholder;
        private PlayerProgress m_PlayerProgress;

        public DailyAchievementTrackerGump(Mobile owner, Mobile viewer)
            : base(20, 20)
        {
            m_AchievementOwner = owner;
            m_Beholder = viewer;
            m_PlayerProgress = DailyAchievement.GetPlayerProgress(owner.Account.Username);

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(33, 25, 495, 472, 9270);
            AddLabel(230, 53, 1153, @"Daily Achievements");

            AddLabel(90, 115, 1153, @"PvP Achievements");
            AddHtml(70, 150, 200, 100, ListProgressForCategory(Category.PvP), true, true);

            AddLabel(320, 115, 1153, @"PvE Achievements");
            AddHtml(300, 150, 200, 100, ListProgressForCategory(Category.PvE), true, true);

            AddLabel(90, 290, 1153, @"Crafter Achievements");
            AddHtml(70, 325, 200, 100, ListProgressForCategory(Category.Crafter), true, true);

            AddLabel(320, 290, 1153, @"Young Achievements");
            AddHtml(300, 325, 200, 100, ListProgressForCategory(Category.Newb), true, true);

            AddLabel(200, 85, 1153, String.Format("Progress {0} / {1} Completed", m_PlayerProgress.Completed, DailyAchievement.CompletionRequirement));

            //Compute for remaining time before achievement reset at 12 AM CST 
            var resetTime = DateTime.UtcNow.Date.AddDays(1).AddHours(6);
            var timeLeft = resetTime - DateTime.UtcNow;
            AddLabel(100, 450, 1153, String.Format("Achievements Reset in {0} hour(s) and {1} minute(s)", timeLeft.Hours, timeLeft.Minutes));

            AddButton(430, 450, 247, 248, 0, GumpButtonType.Reply, 0);
        }

        private string ListProgressForCategory(Category category)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var achievement in DailyAchievement.Rules[category])
            {
                int progress = m_PlayerProgress.ProgressFor(category, achievement.Key);

                builder.AppendFormat("{0}/{1} - {2}<br />",
                    progress,
                    achievement.Value,
                    achievement.Key.GetAttribute<DescriptionAttribute>().Description);
            }

            return builder.ToString();
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
            switch (info.ButtonID)
            {
                case 0:
                    sender.Mobile.SendGump(new AchievementTrackerGump(m_AchievementOwner, m_Beholder, 0, 0));
                    break;
            }
        }
    }
}
