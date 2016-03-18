using Server.Accounting;
using Server.Achievements;
using Server.Guilds;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Achievements
{
    class PlayerProgress
    {
        public CrafterProgress Crafter { get; set; }
        public NewbProgress Newb { get; set; }
        public PvEProgress PvE { get; set; }
        public PvPProgress PvP { get; set; }

        public bool Complete;

        public PlayerProgress()
        {
            Complete = false;
            Crafter = new CrafterProgress();
            Newb = new NewbProgress();
            PvE = new PvEProgress();
            PvP = new PvPProgress();
        }

        internal void Serialize(GenericWriter writer)
        {
            writer.Write(Complete);
            Crafter.Serialize(writer);
            Newb.Serialize(writer);
            PvE.Serialize(writer);
            PvP.Serialize(writer);
        }

        internal void Deserialize(GenericReader reader)
        {
            Complete = reader.ReadBool();
            Crafter.Deserialize(reader);
            Newb.Deserialize(reader);
            PvE.Deserialize(reader);
            PvP.Deserialize(reader);
        }

        internal void Tick(Category category, Enum line, PlayerMobile player, int increment)
        {
            if (Complete) return;

            switch (category)
            {
                case Category.Crafter:
                    Crafter.Tick(line, increment);
                    break;
                case Category.Newb:
                    Newb.Tick(line, increment);
                    break;
                case Category.PvE:
                    PvE.Tick(line, increment);
                    break;
                case Category.PvP:
                    PvP.Tick(line, increment);
                    break;
            }

            Complete = CheckCompletion();
            if (Complete)
                RewardPlayer(player);

        }

        private void RewardPlayer(PlayerMobile player)
        {
            AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_DailyAchievement);

            player.AddToBackpack(new DailyAchievementReward(player));
            if (!player.Hidden)
            {
                player.PlaySound(AchievementSystemImpl.Instance.m_SysTweaks.m_AwardSoundID);
                Effects.SendLocationEffect(player, player.Map, 0x375a, 48, 10);
            }
            player.SendMessage("You have been awarded for your completing your daily achievement quest.");

            if (player.Guild != null && player.Guild is Guild)
            {
                var playerGuild = player.Guild as Guild;
                playerGuild.TickExperience();
            }
        }

        private bool CheckCompletion()
        {
            return Completed >= DailyAchievement.CompletionRequirement;
        }

        public int Completed
        {
            get
            {
                int completed = 0;

                foreach (var progress in Crafter.Progress)
                    if (LineCompleted(Category.Crafter, progress.Key, progress.Value))
                        completed++;

                foreach (var progress in PvE.Progress)
                    if (LineCompleted(Category.PvE, progress.Key, progress.Value))
                        completed++;

                foreach (var progress in PvP.Progress)
                    if (LineCompleted(Category.PvP, progress.Key, progress.Value))
                        completed++;

                foreach (var progress in Newb.Progress)
                    if (LineCompleted(Category.Newb, progress.Key, progress.Value))
                        completed++;
                return completed;
            }
        }

        public int ProgressFor(Category category, Enum line)
        {

            switch (category)
            {
                case Category.Crafter:
                    if (Crafter.Progress.ContainsKey(line))
                        return Crafter.Progress[line];
                    break;
                case Category.Newb:
                    if (Newb.Progress.ContainsKey(line))
                        return Newb.Progress[line];
                    break;
                case Category.PvE:
                    if (PvE.Progress.ContainsKey(line))
                        return PvE.Progress[line];
                    break;
                case Category.PvP:
                    if (PvP.Progress.ContainsKey(line))
                        return PvP.Progress[line];
                    break;
            }
            return 0;
        }

        private bool LineCompleted(Category category, Enum line, int value)
        {
            return value >= DailyAchievement.Rules[category][line];
        }
    }

    class PvPProgress : BaseProgress
    {
        public PvPProgress()
            : base(Category.PvP)
        {
        }

        internal void Serialize(GenericWriter writer)
        {
            writer.Write(Progress.Count);
            foreach (var kvp in Progress)
            {
                writer.Write(kvp.Key.ToString());
                writer.Write(kvp.Value);
            }
        }

        internal void Deserialize(GenericReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string enumValue = reader.ReadString();
                int currentProgress = reader.ReadInt();
                Progress.Add((PvPCategory)Enum.Parse(typeof(PvPCategory), enumValue), currentProgress);
            }
        }
    }

    class PvEProgress : BaseProgress
    {
        public PvEProgress()
            : base(Category.PvE)
        {

        }

        internal void Serialize(GenericWriter writer)
        {
            writer.Write(Progress.Count);
            foreach (var kvp in Progress)
            {
                writer.Write(kvp.Key.ToString());
                writer.Write(kvp.Value);
            }
        }

        internal void Deserialize(GenericReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string enumValue = reader.ReadString();
                int currentProgress = reader.ReadInt();
                Progress.Add((PvECategory)Enum.Parse(typeof(PvECategory), enumValue), currentProgress);
            }
        }
    }

    class CrafterProgress : BaseProgress
    {
        public CrafterProgress()
            : base(Category.Crafter)
        {
        }

        internal void Serialize(GenericWriter writer)
        {
            writer.Write(Progress.Count);
            foreach (var kvp in Progress)
            {
                writer.Write(kvp.Key.ToString());
                writer.Write(kvp.Value);
            }
        }

        internal void Deserialize(GenericReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string enumValue = reader.ReadString();
                int currentProgress = reader.ReadInt();
                Progress.Add((CrafterCategory)Enum.Parse(typeof(CrafterCategory), enumValue), currentProgress);
            }
        }
    }

    class NewbProgress : BaseProgress
    {
        public NewbProgress()
            : base(Category.Newb)
        {
        }

        internal void Serialize(GenericWriter writer)
        {
            writer.Write(Progress.Count);
            foreach (var kvp in Progress)
            {
                writer.Write(kvp.Key.ToString());
                writer.Write(kvp.Value);
            }
        }

        internal void Deserialize(GenericReader reader)
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                string enumValue = reader.ReadString();
                int currentProgress = reader.ReadInt();
                Progress.Add((NewbCategory)Enum.Parse(typeof(NewbCategory), enumValue), currentProgress);
            }
        }
    }

    class BaseProgress
    {
        protected Category Category;
        public Dictionary<Enum, int> Progress = new Dictionary<Enum, int>();

        public BaseProgress(Category category)
        {
            Category = category;
        }

        public void Tick(Enum line, int increment = 1)
        {
            if (!Progress.Keys.Contains(line))
                Progress[line] = 0;

            // don't exceed max
            int max = DailyAchievement.Rules[Category][line];
            if (Progress[line] + increment > max)
                Progress[line] = max;
            else
                Progress[line] += increment;
        }
    }
}
