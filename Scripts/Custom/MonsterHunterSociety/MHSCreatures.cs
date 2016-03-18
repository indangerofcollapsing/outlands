using System;
using Server;
using Server.Regions;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Custom;

namespace Server
{
    public enum MHSGroupType
    {
        Champion,
        Boss,
        LoHBoss,
        Rare,
        Paragon
    }

    public static class MHSCreatures
    {
        public static int BossTextHue = 2115;
        public static int ChampionTextHue = 2603;
        public static int LoHBossTextHue = 2607;
        public static int RareTextHue = 1101;
        public static int ParagonTextHue = 2124;

        #region MHS Creatures Lists

        public static List<Type> BossList = new List<Type>()
        { 
            typeof(TheDeepOne),
            typeof(EmperorDragon),
            typeof(DemonwebQueen),
            typeof(Maggot),
            typeof(AncientNecromancer),
        };

        public static List<Type> ChampionList = new List<Type>()
        { 
            typeof(Atraxis),
        };

        public static List<Type> LoHBossList = new List<Type>()
        { 
            typeof(SuperOgreLord),
        };

        public static List<Type> RareList = new List<Type>()
        { 
            typeof(Dragon),
        };

        public static List<Type> ParagonList = new List<Type>()
        { 
            typeof(Dragon),
        };

        #endregion

        #region MHS Creatures Tasks

        public static List<MHSCreatureTask> GetCreatureTasks(MHSGroupType groupType)
        {
            List<MHSCreatureTask> m_Tasks = new List<MHSCreatureTask>();

            switch (groupType)
            {
                case MHSGroupType.Boss:
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage66PercentOfTotalNoPoisonOrCreatureDamage, false, 8));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage66PercentOfTotal, false, 7));               
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage33PercentOfTotalNoPoisonOrCreatureDamage, false, 6)); 
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage33PercentOfTotal, false, 5));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotal, true, 3));
                break;

                case MHSGroupType.Champion: 
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage95PercentOfTotalNoPoisonOrCreatureDamage, false, 6));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage95PercentOfTotal, false, 5));               
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage66PercentOfTotalNoPoisonOrCreatureDamage, false, 4)); 
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage66PercentOfTotal, false, 3));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotal, true, 2));
                break;

                case MHSGroupType.LoHBoss:
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage66PercentOfTotalNoPoisonOrCreatureDamage, false, 6));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage66PercentOfTotal, false, 5));               
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage33PercentOfTotalNoPoisonOrCreatureDamage, false, 4)); 
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage33PercentOfTotal, false, 3));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotal, true, 2));
                break;

                case MHSGroupType.Rare:
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage95PercentOfTotalNoPoisonOrCreatureDamage, false, 5));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage95PercentOfTotal, false, 4));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotalNoPoisonOrCreatureDamage, false, 3));                     
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotal, true, 2));
                break;

                case MHSGroupType.Paragon:
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage95PercentOfTotalNoPoisonOrCreatureDamage, false, 4));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage95PercentOfTotal, false, 3));
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotalNoPoisonOrCreatureDamage, false, 2));                    
                    m_Tasks.Add(new MHSCreatureTask(MHSTaskType.Damage5PercentOfTotal, true, 1));
                break;
            }

            return m_Tasks;
        }

        #endregion        

        public static MHSCreatureDetail GetCreatureDetail(MHSGroupType groupType, Type creatureType)
        {
            MHSCreatureDetail creatureDetail = new MHSCreatureDetail();            

            switch (groupType)
            {
                #region Bosses

                case MHSGroupType.Boss:                    
                    if (creatureType == typeof(TheDeepOne))
                    {
                        creatureDetail.m_Name = "The Deep One";
                        creatureDetail.m_Title = "Oceanic Terror";

                        creatureDetail.m_IconItemID = 17037;
                        creatureDetail.m_IconHue = 2569;
                        creatureDetail.m_IconOffsetX = 0;
                        creatureDetail.m_IconOffsetY = 0;
                    }

                    if (creatureType == typeof(EmperorDragon))
                    {
                        creatureDetail.m_Name = "Lyth the Destroyer";
                        creatureDetail.m_Title = "Emperor Dragon";

                        creatureDetail.m_IconItemID = 9780;
                        creatureDetail.m_IconHue = 0;
                        creatureDetail.m_IconOffsetX = 0;
                        creatureDetail.m_IconOffsetY = 0;
                    }

                    if (creatureType == typeof(DemonwebQueen))
                    {
                        creatureDetail.m_Name = "Demonweb Queen";
                        creatureDetail.m_Title = "Arachnid Matriarch";

                        creatureDetail.m_IconItemID = 8531;
                        creatureDetail.m_IconHue = 0;
                        creatureDetail.m_IconOffsetX = 0;
                        creatureDetail.m_IconOffsetY = 0;
                    }

                    if (creatureType == typeof(Maggot))
                    {
                        creatureDetail.m_Name = "Maggot";
                        creatureDetail.m_Title = "Diseased Behemoth";

                        creatureDetail.m_IconItemID = 11650;
                        creatureDetail.m_IconHue = 0;
                        creatureDetail.m_IconOffsetX = 0;
                        creatureDetail.m_IconOffsetY = 0;
                    }

                    if (creatureType == typeof(AncientNecromancer))
                    {
                        creatureDetail.m_Name = "Ancient Necromancer";
                        creatureDetail.m_Title = "Eldritch Horror";

                        creatureDetail.m_IconItemID = 17050;
                        creatureDetail.m_IconHue = 0;
                        creatureDetail.m_IconOffsetX = 0;
                        creatureDetail.m_IconOffsetY = 0;
                    }
                break;

                #endregion

                #region Champions

                case MHSGroupType.Champion:
                    if (creatureType == typeof(Atraxis))
                    {
                    }                    
                break;

                #endregion

                #region LoH Boss

                case MHSGroupType.LoHBoss:
                    if (creatureType == typeof(SuperOgreLord))
                    {
                    }                    
                break;

                #endregion

                #region Rare

                case MHSGroupType.Rare:
                    if (creatureType == typeof(Dragon))
                    {
                    }                    
                break;

                #endregion

                #region Paragon

                case MHSGroupType.Paragon:
                    if (creatureType == typeof(Dragon))
                    {
                    }                    
                break;

                #endregion
            }

            //Assign Tasks
            List<MHSCreatureTask> m_Tasks = GetCreatureTasks(groupType);          
           
            foreach (MHSCreatureTask task in m_Tasks)
            {
                creatureDetail.m_Tasks.Add(new MHSCreatureTask(task.m_TaskType, task.m_Repeatable, task.m_PointsGranted));
            }            

            return creatureDetail;
        }

        public static MHSCreaturePlayerEntry GetCreaturePlayerEntry(PlayerMobile player, MHSGroupType groupType, Type creatureType)
        {
            MHSPersistance.CheckAndCreateMHSAccountEntry(player);

            foreach (MHSCreaturePlayerEntry creatureEntry in player.m_MHSPlayerEntry.m_CreatureEntries)
            {
                if (creatureEntry.m_CreatureType == creatureType && creatureEntry.m_GroupType == groupType)                
                    return creatureEntry;                
            }

            MHSCreaturePlayerEntry newEntry = new MHSCreaturePlayerEntry(creatureType, groupType);
            player.m_MHSPlayerEntry.m_CreatureEntries.Add(newEntry);

            return newEntry;
        }

        public static void CreatureKilled(BaseCreature creature, PlayerMobile player, double damagePercent, bool takenPoisonDamage, bool takenCreatureDamage)
        {
            if (creature == null || player == null)
                return;

            MHSPersistance.CheckAndCreateMHSAccountEntry(player);

            if (BossList.Contains(creature.GetType()))
                ResolveCreatureKilled(MHSGroupType.Boss, creature, player, damagePercent, takenPoisonDamage, takenCreatureDamage);

            if (ChampionList.Contains(creature.GetType()))
                ResolveCreatureKilled(MHSGroupType.Champion, creature, player, damagePercent, takenPoisonDamage, takenCreatureDamage);

            if (LoHBossList.Contains(creature.GetType()))
                ResolveCreatureKilled(MHSGroupType.LoHBoss, creature, player, damagePercent, takenPoisonDamage, takenCreatureDamage);

            if (creature.Rare && RareList.Contains(creature.GetType()))
                ResolveCreatureKilled(MHSGroupType.Rare, creature, player, damagePercent, takenPoisonDamage, takenCreatureDamage);

            if (creature.IsParagon && !creature.ConvertedParagon && RareList.Contains(creature.GetType()))
                ResolveCreatureKilled(MHSGroupType.Paragon, creature, player, damagePercent, takenPoisonDamage, takenCreatureDamage);
        }

        public static void ResolveCreatureKilled(MHSGroupType groupType, BaseCreature creature, PlayerMobile player, double damagePercent, bool takenPoisonDamage, bool takenCreatureDamage)
        {
            List<MHSCreatureTask> creatureTasks = MHSCreatures.GetCreatureTasks(groupType);
            MHSCreaturePlayerEntry creaturePlayerEntry = MHSCreatures.GetCreaturePlayerEntry(player, groupType, creature.GetType());

            MHSCreatureTaskPlayerEntry bestValidTask = null;
            int highestTaskPoints = 0;

            int creatureIndex = -1;
            int taskIndex = -1;

            foreach (MHSCreatureTask creatureTask in creatureTasks)
            {
                bool taskSuccessful = false;

                switch (creatureTask.m_TaskType)
                {
                    case MHSTaskType.Damage5PercentOfTotal:
                        if (damagePercent >= .05)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage5PercentOfTotalNoPoisonOrCreatureDamage:
                        if (damagePercent >= .05 && !takenPoisonDamage && !takenCreatureDamage)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage33PercentOfTotal:
                        if (damagePercent >= .33)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage33PercentOfTotalNoPoisonOrCreatureDamage:
                        if (damagePercent >= .33 && !takenPoisonDamage && !takenCreatureDamage)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage66PercentOfTotal:
                        if (damagePercent >= .66)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage66PercentOfTotalNoPoisonOrCreatureDamage:
                        if (damagePercent >= .66 && !takenPoisonDamage && !takenCreatureDamage)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage95PercentOfTotal:
                        if (damagePercent >= .95)
                            taskSuccessful = true;
                    break;

                    case MHSTaskType.Damage95PercentOfTotalNoPoisonOrCreatureDamage:
                        if (damagePercent >= .95 && !takenPoisonDamage && !takenCreatureDamage)
                            taskSuccessful = true;
                    break;
                }

                if (!taskSuccessful)
                    continue;

                MHSCreatureTaskPlayerEntry playerTaskEntry = null;

                int index = 0;

                for (int a = 0; a < creaturePlayerEntry.m_Tasks.Count; a++)
                {
                    if (creatureTask.m_TaskType == creaturePlayerEntry.m_Tasks[a].m_TaskType)
                    {
                        playerTaskEntry = creaturePlayerEntry.m_Tasks[a];

                        break;
                    }
                }

                if (playerTaskEntry == null)
                {
                    playerTaskEntry = new MHSCreatureTaskPlayerEntry(creatureTask.m_TaskType);
                    creaturePlayerEntry.m_Tasks.Add(playerTaskEntry);

                    index = creaturePlayerEntry.m_Tasks.Count - 1;
                }

                if (playerTaskEntry.m_TimesCompleted > 0 && !creatureTask.m_Repeatable)
                    continue;

                if (creatureTask.m_PointsGranted > highestTaskPoints)
                {                    
                    highestTaskPoints = creatureTask.m_PointsGranted;
                    bestValidTask = playerTaskEntry;
                }
            }

            if (highestTaskPoints > 0 && bestValidTask != null)
            {
                creaturePlayerEntry.m_TimesKilled++;
                creaturePlayerEntry.m_LastKilled = DateTime.UtcNow;

                bestValidTask.m_TimesCompleted++;
                bestValidTask.m_LastTimeCompleted = DateTime.UtcNow;

                player.m_MHSPlayerEntry.m_AvailablePoints += highestTaskPoints;
                player.m_MHSPlayerEntry.m_TotalPointsEarned += highestTaskPoints;

                MHSCreatureDetail creatureDetail = MHSCreatures.GetCreatureDetail(groupType, creature.GetType());

                string creatureName = creatureDetail.m_Name;
                string groupName = MHSCreatures.GetGroupTypeName(groupType);
                string[] taskDescription = MHSCreatures.GetTaskDescription(bestValidTask.m_TaskType);
                string taskName = "";

                int textHue = GetGroupTypeTextHue(groupType);

                for (int a = 0; a < taskDescription.Length; a++)
                {
                    taskName += taskDescription[a];

                    if (a != taskDescription.Length - 1)
                        taskName += " ";
                }

                string message = "You have earned " + highestTaskPoints.ToString() + " points in the Monster Hunter's Society for the task of: " + creatureName + " [" + groupName + "]" + " - " + taskName + ".";

                player.SendMessage(textHue, message);

                player.FixedEffect(0x373A, 10, 30, 0, 0);
                player.PlaySound(0x5A7);               
            }
        }

        public static int GetPlayerTaskCount(MHSGroupType groupType, PlayerMobile player)
        {
            int count = 0;

            if (player == null)
                return count;

            MHSPersistance.CheckAndCreateMHSAccountEntry(player);

            foreach (MHSCreaturePlayerEntry creatureEntry in player.m_MHSPlayerEntry.m_CreatureEntries)
            {
                if (creatureEntry.m_GroupType != groupType)
                    continue;

                foreach (MHSCreatureTaskPlayerEntry taskEntry in creatureEntry.m_Tasks)
                {
                    if (taskEntry.m_TimesCompleted > 0)
                        count++;
                }
            }            

            return count;
        }

        public static string GetGroupTypeName(MHSGroupType groupType)
        {
            string name = "";

            switch (groupType)
            {
                case MHSGroupType.Boss: name = "Boss"; break;
                case MHSGroupType.Champion: name = "Champion"; break;
                case MHSGroupType.LoHBoss: name = "League of Heroes"; break;
                case MHSGroupType.Rare: name = "Rare"; break;
                case MHSGroupType.Paragon: name = "Paragon"; break;
            }
            
            return name;
        }

        public static int GetGroupTypeTextHue(MHSGroupType groupType)
        {
            int hue = 0;

            switch (groupType)
            {
                case MHSGroupType.Boss: hue = BossTextHue; break;
                case MHSGroupType.Champion: hue = ChampionTextHue; break;
                case MHSGroupType.LoHBoss: hue = LoHBossTextHue; break;
                case MHSGroupType.Rare: hue = RareTextHue; break;
                case MHSGroupType.Paragon: hue = ParagonTextHue; break;
            }

            return hue;
        }

        public static string[] GetTaskDescription(MHSTaskType taskType)
        {
            string[] description = new string[] { };            

            switch (taskType)
            {
                case MHSTaskType.Damage5PercentOfTotal: description = new string[] { "Deal over 5% of total damage"}; break;
                case MHSTaskType.Damage33PercentOfTotal: description = new string[] { "Deal over 33% of total damage" }; break;
                case MHSTaskType.Damage66PercentOfTotal: description = new string[] { "Deal over 66% of total damage" }; break;
                case MHSTaskType.Damage95PercentOfTotal: description = new string[] { "Deal over 95% of total damage" }; break;

                case MHSTaskType.Damage5PercentOfTotalNoPoisonOrCreatureDamage: description = new string[] { "Deal over 5% of total damage", "without poison or creature help" }; break;
                case MHSTaskType.Damage33PercentOfTotalNoPoisonOrCreatureDamage: description = new string[] { "Deal over 33% of total damage", "without poison or creature help" }; break;
                case MHSTaskType.Damage66PercentOfTotalNoPoisonOrCreatureDamage: description = new string[] { "Deal over 66% of total damage", "without poison or creature help" }; break;
                case MHSTaskType.Damage95PercentOfTotalNoPoisonOrCreatureDamage: description = new string[] { "Deal over 95% of total damage", "without poison or creature help" }; break;
            }

            return description;
        }
    }

    public enum MHSTaskType
    {
        Damage5PercentOfTotal,
        Damage33PercentOfTotal,
        Damage66PercentOfTotal,
        Damage95PercentOfTotal,

        Damage5PercentOfTotalNoPoisonOrCreatureDamage,
        Damage33PercentOfTotalNoPoisonOrCreatureDamage,
        Damage66PercentOfTotalNoPoisonOrCreatureDamage,
        Damage95PercentOfTotalNoPoisonOrCreatureDamage,        
    }       

    public class MHSCreatureTask
    {
        public MHSTaskType m_TaskType = MHSTaskType.Damage5PercentOfTotal;  
        public bool m_Repeatable = false;
        public int m_PointsGranted = 1;

        public MHSCreatureTask(MHSTaskType taskType, bool repeatable, int pointsGranted)
        {
            m_TaskType = taskType;
            m_Repeatable = repeatable;
            m_PointsGranted = pointsGranted;
        }
    }

    public class MHSCreatureDetail
    {
        public string m_Name = "Creature Name";
        public string m_Title = "Creature Title";

        public int m_IconItemID = 8428;
        public int m_IconHue = 0;
        public int m_IconOffsetX = 0;
        public int m_IconOffsetY = 0;

        public List<MHSCreatureTask> m_Tasks = new List<MHSCreatureTask>();

        public MHSCreatureDetail()
        {
        }
    } 
}