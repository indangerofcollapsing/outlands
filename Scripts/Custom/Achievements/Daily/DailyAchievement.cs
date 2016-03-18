using Server.Achievements;
using Server.Commands;
using Server.Custom.Townsystem;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using Server.ExtensionMethods;

namespace Server.Achievements
{
    static class DailyAchievement
    {
        public static Dictionary<string, PlayerProgress> PlayerProgress = new Dictionary<string, PlayerProgress>();
        public static DateTime LastWipe = DateTime.Now;
        public static Dictionary<Category, Dictionary<Enum, int>> Rules = new Dictionary<Category, Dictionary<Enum, int>>();
        public static int CompletionRequirement = 7;
        private static DailyAchievementTimer m_Timer;


        public static void Initialize()
        {
            new DailyAchievementPersistence();

            if (Rules.Count == 0)
                InitializeRules();

            CommandSystem.Register("DailyWipe", AccessLevel.Developer, x => { ResetProgress(); });

            m_Timer = new DailyAchievementTimer();
            m_Timer.Start();
        }

        private static void InitializeRules()
        {
            Rules.Clear();
            Rules[Category.Crafter] = new Dictionary<Enum, int>();
            Rules[Category.Newb]    = new Dictionary<Enum, int>();
            Rules[Category.PvE]     = new Dictionary<Enum, int>();
            Rules[Category.PvP]     = new Dictionary<Enum, int>();

            foreach (var achievement in (CrafterCategory[])Enum.GetValues(typeof(CrafterCategory)))
            {
                Rules[Category.Crafter][achievement] = achievement.GetAttribute<AchievementTargetAttribute>().Target;
            }

            int pveLimit = 10;
            var pveOptions = (PvECategory[])Enum.GetValues(typeof(PvECategory));
            // grab X random pve achievements
            while(Rules[Category.PvE].Count < pveLimit)
            {
                var selected = pveOptions[Utility.RandomMinMax(0, pveOptions.Length - 1)];
                if (!Rules[Category.PvE].ContainsKey(selected))
                    Rules[Category.PvE][selected] = selected.GetAttribute<AchievementTargetAttribute>().Target;
            }

            foreach (var achievement in (PvPCategory[])Enum.GetValues(typeof(PvPCategory)))
            {
                Rules[Category.PvP][achievement] = achievement.GetAttribute<AchievementTargetAttribute>().Target;
            }

            foreach (var achievement in (NewbCategory[])Enum.GetValues(typeof(NewbCategory)))
            {
                Rules[Category.Newb][achievement] = achievement.GetAttribute<AchievementTargetAttribute>().Target;
            }
        }

        public static void ResetProgress()
        {
            if (DateTime.Today.Date > LastWipe.Date)
            {
                PlayerProgress.Clear();
                LastWipe = DateTime.Now;
                Town.GlobalTownCrierBroadcast(new string[] { "Daily achievement progress has been reset!" }, TimeSpan.FromMinutes(1));
                InitializeRules();
            }
        }

        public static void TickProgress(Category category, PlayerMobile player, Enum line, int increment = 1)
        {
            if (player == null) return;
            if (player.Region is UOACZRegion) return;

            var progress = GetPlayerProgress(player.Account.Username);

            // special case newb to only young accounts
            if (category == Category.Newb && !player.Young)
                return;

            if (Rules[category].ContainsKey(line))
                progress.Tick(category, line, player, increment);
        }

        public static PlayerProgress GetPlayerProgress(string accountName)
        {
            if (!PlayerProgress.ContainsKey(accountName))
                PlayerProgress[accountName] = new PlayerProgress();

            return PlayerProgress[accountName];
        }
    }

    public class DailyAchievementPersistence : Item
    {
        public override string DefaultName { get { return "Daily Achievement Persistence - Internal"; } }
        private static DailyAchievementPersistence m_Instance;

        public DailyAchievementPersistence()
            : base(0)
        {
            Movable = false;
            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public DailyAchievementPersistence(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version

            writer.Write(DailyAchievement.LastWipe);

            writer.Write(DailyAchievement.Rules.Count);
            foreach (var kvp in DailyAchievement.Rules)
            {
                writer.Write((int)kvp.Key);

                writer.Write(DailyAchievement.Rules[kvp.Key].Count);

                foreach (var rule in DailyAchievement.Rules[kvp.Key])
                {
                    writer.Write(Convert.ToInt32(rule.Key));
                    writer.Write(rule.Value);
                }
            }

            writer.Write(DailyAchievement.PlayerProgress.Count);
            foreach (var kvp in DailyAchievement.PlayerProgress)
            {
                writer.Write(kvp.Key); // account name
                kvp.Value.Serialize(writer);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();


            switch (version)
            {
                case 0:
                    {
                        DailyAchievement.LastWipe = reader.ReadDateTime();

                        int ruleCount = reader.ReadInt();
                        for (int i = 0; i < ruleCount; i++)
                        {
                            Category cat = (Category)reader.ReadInt();
                            int categoryCount = reader.ReadInt();

                            DailyAchievement.Rules[cat] = new Dictionary<Enum, int>();
                            for (int x = 0; x < categoryCount; x++)
                            {
                                switch (cat)
                                {
                                    case Category.PvP:
                                        DailyAchievement.Rules[cat][(PvPCategory)reader.ReadInt()] = reader.ReadInt();
                                        break;
                                    case Category.PvE:
                                        DailyAchievement.Rules[cat][(PvECategory)reader.ReadInt()] = reader.ReadInt();
                                        break;
                                    case Category.Crafter:
                                        DailyAchievement.Rules[cat][(CrafterCategory)reader.ReadInt()] = reader.ReadInt();
                                        break;
                                    case Category.Newb:
                                        DailyAchievement.Rules[cat][(NewbCategory)reader.ReadInt()] = reader.ReadInt();
                                        break;
                                }
                            }
                        }

                        int progressCount = reader.ReadInt();
                        for (int i = 0; i < progressCount; i++)
                        {
                            string accountName = reader.ReadString();
                            PlayerProgress progress = new PlayerProgress();
                            progress.Deserialize(reader);

                            if (accountName != null)
                                DailyAchievement.PlayerProgress[accountName] = progress;
                        }
                    } break;
            }
            m_Instance = this;
        }

    }

    public class DailyAchievementTimer : Timer
    {
        public DailyAchievementTimer() : base(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1))
        {
            Priority = TimerPriority.OneMinute;
        }

        protected override void OnTick()
        {
            DailyAchievement.ResetProgress();
        }
    }
}
