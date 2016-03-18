using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;

namespace Server.Items
{
    public class MHSGrimoire : Item
    {
        public static int GumpSound = 0x055;

        [Constructable]
        public MHSGrimoire(): base(8787)
        {
            Name = "a monster hunter society grimoire";
            Hue = 2635;
        }

        public MHSGrimoire(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            MHSPersistance.CheckAndCreateMHSAccountEntry(player);

            player.SendSound(MHSGrimoire.GumpSound);

            if (player.m_MHSPlayerEntry.m_GrimoirePage == MHSPlayerEntry.GrimoirePage.Main)
            {
                player.CloseGump(typeof(MHSTableOfContentsGump));
                player.SendGump(new MHSTableOfContentsGump(player));
            }

            else
            {
                player.CloseGump(typeof(MHSCreatureEntryGump));
                player.SendGump(new MHSCreatureEntryGump(player));
            }           
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class MHSTableOfContentsGump : Gump
    {   
        public PlayerMobile player;

        public MHSTableOfContentsGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) return;
            if (player.Deleted) return;

            MHSPersistance.CheckAndCreateMHSAccountEntry(player);        

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddImage(205, 193, 11010);
            AddImage(204, 1, 11010);
            AddImage(3, 192, 11010);
            AddImage(3, 1, 11010);
            AddImage(301, 265, 2081);
            AddImage(301, 12, 2081);
            AddImage(301, 79, 2081);
            AddImage(301, 142, 2081);
            AddImage(301, 207, 2081);
            AddImage(299, 335, 2081);
            AddImage(43, 266, 2081);
            AddImage(41, 14, 2081);
            AddImage(43, 83, 2081);
            AddImage(43, 145, 2081);
            AddImage(43, 209, 2081);
            AddImage(41, 335, 2081);
            AddImage(43, 334, 2081);
            AddItem(46, 300, 8758);
            AddItem(125, 221, 2095);
            AddItem(129, 253, 11650);
            AddItem(95, 196, 2081);
            AddItem(161, 173, 2083);
            AddItem(118, 173, 2081);
            AddItem(139, 242, 2081);
            AddItem(163, 220, 2081);
            AddItem(184, 193, 2083);
            AddItem(147, 242, 2097);
            AddItem(507, 368, 6921);
            AddItem(502, 356, 6937);

            AddItem(32, 365, 7111, 2500);

            AddItem(322, 61, 9780); //Lyth
            AddItem(337, 134, 9778); //Dark Sentinel
            AddItem(319, 191, 9736); //Enveloping Darkness
            AddItem(329, 251, 8491, 2500); //Drider
            AddItem(324, 301, 9743, 2125); //Burrow Beetle

            //------------

            int whiteTextHue = 2499;
            int greenTextHue = 0x3F;
            int blueTextHue = 2603;
            int yellowTextHue = 2550;
            int lightGreenTextHue = 2599;
            int orangeTextHue = 149;
            int purpleTextHue = 1164;
            int lightPurpleTextHue = 2628;            

            //Left Page----------

            string playerName = player.RawName;
            string totalPointsEarned = player.m_MHSPlayerEntry.m_TotalPointsEarned.ToString();
            string globalRanking = MHSPersistance.GetGlobalRanking(player).ToString() + " of " + MHSPersistance.GetGlobalEntryCount().ToString();
            string pointsAvailable = player.m_MHSPlayerEntry.m_AvailablePoints.ToString() + " Points To Spend";

            int playerBossTasksCompleted = MHSCreatures.GetPlayerTaskCount(MHSGroupType.Boss, player);
            int playerChampionTasksCompleted = MHSCreatures.GetPlayerTaskCount(MHSGroupType.Champion, player);
            int playerLoHBossTasksCompleted = MHSCreatures.GetPlayerTaskCount(MHSGroupType.LoHBoss, player);
            int playerRareTasksCompleted = MHSCreatures.GetPlayerTaskCount(MHSGroupType.Rare, player);
            int playerParagonTasksCompleted = MHSCreatures.GetPlayerTaskCount(MHSGroupType.Paragon, player); 
  
            int totalBossTasksAvailable = MHSCreatures.GetCreatureTasks(MHSGroupType.Boss).Count * MHSCreatures.BossList.Count;
            int totalChampionTasksAvailable = MHSCreatures.GetCreatureTasks(MHSGroupType.Champion).Count * MHSCreatures.ChampionList.Count;
            int totalLoHBossTasksAvailable = MHSCreatures.GetCreatureTasks(MHSGroupType.LoHBoss).Count * MHSCreatures.LoHBossList.Count;
            int totalRareTasksAvailable = MHSCreatures.GetCreatureTasks(MHSGroupType.Rare).Count * MHSCreatures.RareList.Count;
            int totalParagonTasksAvailable = MHSCreatures.GetCreatureTasks(MHSGroupType.Paragon).Count * MHSCreatures.ParagonList.Count;

            string bossTaskProgress = playerBossTasksCompleted.ToString() + "/" + totalBossTasksAvailable.ToString();
            string championTaskProgress = playerChampionTasksCompleted.ToString() + "/" + totalChampionTasksAvailable.ToString();
            string LoHCreatureTaskProgress = playerLoHBossTasksCompleted.ToString() + "/" + totalLoHBossTasksAvailable.ToString();
            string rareCreatureTaskProgress = playerRareTasksCompleted.ToString() + "/" + totalRareTasksAvailable.ToString();
            string paragonTaskProgress = playerParagonTasksCompleted.ToString() + "/" + totalParagonTasksAvailable.ToString();              
              
            AddLabel(97, 20, blueTextHue, "Monster Hunter Society");
            AddLabel(97, 40, yellowTextHue, "Grimoire of Achivements");
            AddLabel(149, 63, lightPurpleTextHue, playerName);

            AddLabel(66, 104, whiteTextHue, "Lifetime Points");
            AddItem(155, 99, 7782);
            AddLabel(200, 106, greenTextHue, totalPointsEarned);

            AddLabel(74, 139, whiteTextHue, "Global Ranking");
            AddItem(153, 138, 7779);
            AddLabel(200, 141, greenTextHue, globalRanking);

            AddLabel(114, 348, whiteTextHue, pointsAvailable);
            AddButton(102, 372, 2151, 2154, 1, GumpButtonType.Reply, 0);
            AddLabel(135, 376, greenTextHue, "Redeem Rewards");

            //Right Page-------------
            AddLabel(390, 30, orangeTextHue, "Task Progress");

            AddButton(400, 85, 2151, 2154, 6, GumpButtonType.Reply, 0);
            AddLabel(440, 81, MHSCreatures.BossTextHue, "Bosses");
            AddLabel(440, 101, whiteTextHue, bossTaskProgress);

            AddButton(400, 143, 2151, 2154, 7, GumpButtonType.Reply, 0);
            AddLabel(440, 139, MHSCreatures.ChampionTextHue, "Champions");
            AddLabel(440, 159, whiteTextHue, championTaskProgress);

            AddButton(400, 199, 2151, 2154, 8, GumpButtonType.Reply, 0);
            AddLabel(440, 195, MHSCreatures.LoHBossTextHue, "League of Heroes");
            AddLabel(440, 215, whiteTextHue, LoHCreatureTaskProgress);

            AddButton(400, 255, 2151, 2154, 9, GumpButtonType.Reply, 0);
            AddLabel(440, 250, MHSCreatures.RareTextHue, "Rare Creatures");
            AddLabel(440, 270, whiteTextHue, rareCreatureTaskProgress);

            AddButton(400, 307, 2151, 2154, 10, GumpButtonType.Reply, 0);
            AddLabel(440, 303, MHSCreatures.ParagonTextHue, "Paragons");
            AddLabel(440, 322, whiteTextHue, paragonTaskProgress);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            MHSPersistance.CheckAndCreateMHSAccountEntry(player);

            switch (info.ButtonID)
            {
                //Redeem Rewards
                case 1:
                    //player.CloseGump(typeof(MHSTableOfContentsGump));
                    //player.SendGump(new MHSCreatureEntryGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
                break;

                //Boss Entries
                case 6:
                    player.m_MHSPlayerEntry.m_GrimoirePage = MHSPlayerEntry.GrimoirePage.Boss;

                    player.CloseGump(typeof(MHSTableOfContentsGump));
                    player.SendGump(new MHSCreatureEntryGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
                break;

                //Champion Entries
                case 7:
                    player.m_MHSPlayerEntry.m_GrimoirePage = MHSPlayerEntry.GrimoirePage.Champion;

                    player.CloseGump(typeof(MHSTableOfContentsGump));
                    player.SendGump(new MHSCreatureEntryGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
                break;

                //LoH Creature Entries
                case 8:
                    player.m_MHSPlayerEntry.m_GrimoirePage = MHSPlayerEntry.GrimoirePage.LoHBoss;

                    player.CloseGump(typeof(MHSTableOfContentsGump));
                    player.SendGump(new MHSCreatureEntryGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
                break;

                //Rare Creature Entries
                case 9:
                    player.m_MHSPlayerEntry.m_GrimoirePage = MHSPlayerEntry.GrimoirePage.Rare;

                    player.CloseGump(typeof(MHSTableOfContentsGump));
                    player.SendGump(new MHSCreatureEntryGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
                break;

                //Paragon Entries
                case 10:
                    player.m_MHSPlayerEntry.m_GrimoirePage = MHSPlayerEntry.GrimoirePage.Paragon;

                    player.CloseGump(typeof(MHSTableOfContentsGump));
                    player.SendGump(new MHSCreatureEntryGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
                break;
            }
        }
    }

    public class MHSCreatureEntryGump : Gump
    {        
        public PlayerMobile player;

        public int m_PageNumber = 1;
        public int m_MaxPageCount = 1;

        public MHSCreatureEntryGump(PlayerMobile player): base(7, 7)
        {
            if (player == null) return;
            if (player.Deleted) return;

            MHSPersistance.CheckAndCreateMHSAccountEntry(player);         

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            
            AddImage(205, 193, 11010);
            AddImage(204, 1, 11010);
            AddImage(3, 192, 11010);
            AddImage(3, 1, 11010);
            AddImage(301, 265, 2081);
            AddImage(300, 12, 2081);
            AddImage(301, 79, 2081);
            AddImage(302, 142, 2081);
            AddImage(301, 207, 2081);
            AddImage(299, 335, 2081);
            AddImage(43, 266, 2081);
            AddImage(41, 14, 2081);
            AddImage(43, 83, 2081);
            AddImage(43, 145, 2081);
            AddImage(43, 209, 2081);
            AddImage(41, 335, 2081);
            AddImage(43, 334, 2081);
            AddImage(220, 13, 2440);
            AddItem(131, 297, 6873);
            AddItem(154, 319, 6872);
            AddItem(124, 277, 6874);
            AddItem(149, 277, 6875);
            AddItem(177, 297, 6876);
            AddItem(194, 292, 6877);
            AddItem(203, 301, 6922);
            AddItem(162, 315, 6923);
            AddItem(104, 300, 6924);
            AddItem(143, 330, 6925);
            AddItem(192, 323, 6938);

            //----------------------------

            int whiteTextHue = 2499;
            int greenTextHue = 0x3F;
            int greyTextHue = 2401;
            int blueTextHue = 2603;
            int yellowTextHue = 2550;
            int lightGreenTextHue = 2599;
            int orangeTextHue = 149;
            int purpleTextHue = 1164;
            int lightPurpleTextHue = 2628;

            int bossTextHue = 2115;
            int championTextHue = 2603;
            int LoHBossTextHue = 2607;
            int rareTextHue = 1101;
            int paragonTextHue = 2124;

            int pageNumber = 1;
            int maxPageCount = 1;

            string creatureTypeText = "";
            int creatureTypeHue = bossTextHue;

            string creatureNameText = "";
            int creatureNameHue = bossTextHue;

            string creatureTitleText = "";
            int creatureTitleHue = yellowTextHue;

            int creatureIconItemId = 9780;
            int creatureIconHue = 0;
            int creatureIconOffsetX = 0;
            int creatureIconOffsetY = 0;

            int timesKilled = 0;

            string lastKilled = "";

            MHSGroupType groupType = MHSGroupType.Boss;
            Type creatureType = null;            

            switch (player.m_MHSPlayerEntry.m_GrimoirePage)
            {
                case MHSPlayerEntry.GrimoirePage.Boss:
                    groupType = MHSGroupType.Boss;

                    pageNumber = player.m_MHSPlayerEntry.m_BossPageNumber;
                    maxPageCount = MHSCreatures.BossList.Count; 

                    creatureType = MHSCreatures.BossList[pageNumber - 1];                    

                    creatureTypeText = "Bosses";
                    creatureTypeHue = bossTextHue;
                    creatureNameHue = bossTextHue;
                break;

                case MHSPlayerEntry.GrimoirePage.Champion:
                    groupType = MHSGroupType.Champion;

                    pageNumber = player.m_MHSPlayerEntry.m_ChampionPageNumber;
                    maxPageCount = MHSCreatures.ChampionList.Count; 

                    creatureType = MHSCreatures.ChampionList[pageNumber - 1];                   

                    creatureTypeText = "Champions";
                    creatureTypeHue = championTextHue;
                    creatureNameHue = championTextHue;
                break;

                case MHSPlayerEntry.GrimoirePage.LoHBoss:
                    groupType = MHSGroupType.LoHBoss;

                    pageNumber = player.m_MHSPlayerEntry.m_LoHBossPageNumber;
                    maxPageCount = MHSCreatures.LoHBossList.Count;

                    creatureType = MHSCreatures.LoHBossList[pageNumber - 1];                   

                    creatureTypeText = "League of Heroes";
                    creatureTypeHue = LoHBossTextHue;
                    creatureNameHue = LoHBossTextHue;
                break;

                case MHSPlayerEntry.GrimoirePage.Rare:
                    groupType = MHSGroupType.Rare;

                    pageNumber = player.m_MHSPlayerEntry.m_RarePageNumber;
                    maxPageCount = MHSCreatures.RareList.Count;

                    creatureType = MHSCreatures.RareList[pageNumber - 1];                    

                    creatureTypeText = "Rare Creatures";
                    creatureTypeHue = rareTextHue;
                    creatureNameHue = rareTextHue;
                break;

                case MHSPlayerEntry.GrimoirePage.Paragon:
                    groupType = MHSGroupType.Paragon;

                    pageNumber = player.m_MHSPlayerEntry.m_ParagonPageNumber;
                    maxPageCount = MHSCreatures.ParagonList.Count; 

                    creatureType = MHSCreatures.ParagonList[pageNumber - 1];                    

                    creatureTypeText = "Paragons";
                    creatureTypeHue = paragonTextHue;
                    creatureNameHue = paragonTextHue;
                break;
            }

            if (creatureType == null)
            {
                player.CloseGump(typeof(MHSCreatureEntryGump));
                return;
            }

            MHSCreatureDetail creatureDetail = MHSCreatures.GetCreatureDetail(groupType, creatureType);

            creatureNameText = creatureDetail.m_Name;
            creatureTitleText = creatureDetail.m_Title;

            creatureIconItemId = creatureDetail.m_IconItemID;
            creatureIconHue = creatureDetail.m_IconHue;
            creatureIconOffsetX = creatureDetail.m_IconOffsetX;
            creatureIconOffsetY = creatureDetail.m_IconOffsetY;

            MHSCreaturePlayerEntry creaturePlayerEntry = MHSCreatures.GetCreaturePlayerEntry(player, groupType, creatureType);

            timesKilled = creaturePlayerEntry.m_TimesKilled;

            if (creaturePlayerEntry.m_LastKilled != DateTime.MaxValue)
                lastKilled = creaturePlayerEntry.m_LastKilled.ToShortDateString();

            //Left Page
            AddLabel(Utility.CenteredTextOffset(305, creatureTypeText), 14, creatureTypeHue, creatureTypeText); //Creature Type

            AddLabel(Utility.CenteredTextOffset(180, creatureNameText), 40, creatureNameHue, creatureNameText); //Creature Name
            AddLabel(Utility.CenteredTextOffset(180, creatureTitleText), 60, creatureTitleHue, creatureTitleText); //Creature Title

            AddItem(148 + creatureIconOffsetX, 99 + creatureIconOffsetY, creatureIconItemId, creatureIconHue); //Creature Icon

            AddLabel(90, 195, whiteTextHue, "Times Killed");
            AddItem(158, 199, 6884);
            AddLabel(198, 195, greenTextHue, timesKilled.ToString());

            AddLabel(78, 225, whiteTextHue, "Last Kill Date");
            AddItem(158, 221, 6160);
            AddLabel(195, 225, greenTextHue, lastKilled);

            AddButton(83, 372, 2151, 2154, 1, GumpButtonType.Reply, 0);
            AddLabel(117, 376, greenTextHue, "Go to Table of Contents");

            //Right Page
            AddLabel(405, 40, orangeTextHue, "Kill Tasks");

            int startX = 310;
            int startY = 65;

            for (int a = 0; a < creatureDetail.m_Tasks.Count; a++)
            {
                MHSCreatureTask creatureTask = creatureDetail.m_Tasks[a];

                string[] taskDescription = MHSCreatures.GetTaskDescription(creatureTask.m_TaskType);
                bool repeatable = creatureTask.m_Repeatable;
                int pointsGranted = creatureTask.m_PointsGranted;

                int timesCompleted = 0;
                DateTime lastCompleted = DateTime.MaxValue;

                foreach (MHSCreatureTaskPlayerEntry creaturePlayerTask in creaturePlayerEntry.m_Tasks)
                {
                    if (creaturePlayerTask.m_TaskType == creatureTask.m_TaskType)
                    {
                        timesCompleted = creaturePlayerTask.m_TimesCompleted;
                        lastCompleted = creaturePlayerTask.m_LastTimeCompleted;

                        break;
                    }
                }

                if (timesCompleted > 0 && !repeatable)
                    AddImage(startX, startY, 9730, 0);               

                else
                    AddImage(startX, startY, 9727, 0);                   

                startY += 3;

                for (int b = 0; b < taskDescription.Length; b++)
                {
                    string descriptionText = taskDescription[b];                    

                    if (timesCompleted > 0 && !repeatable)
                        AddLabel(startX + 40, startY, greyTextHue, descriptionText);

                    else
                        AddLabel(startX + 40, startY, whiteTextHue, descriptionText);

                    startY += 18;
                }

                int pointsX = 438;

                string text = pointsGranted.ToString() + " Points";

                AddLabel(Utility.CenteredTextOffset(pointsX, text), startY, lightGreenTextHue, text);

                if (repeatable)
                {
                    text = "(Repeatable Task)";

                    startY += 18;
                    AddLabel(Utility.CenteredTextOffset(pointsX + 5, text), startY, blueTextHue, text);
                }

                startY += 25;
            }

            //Store Values
            m_PageNumber = pageNumber;
            m_MaxPageCount = maxPageCount;

            if (pageNumber <= maxPageCount && pageNumber > 1)
                AddButton(345, 378, 4014, 4014, 10, GumpButtonType.Reply, 0); //Previous

            string pagePosition = "Page " + pageNumber.ToString() + " of " + maxPageCount.ToString();
            AddLabel(Utility.CenteredTextOffset(430, pagePosition), 378, 2499, pagePosition);

            if (pageNumber < maxPageCount)
                AddButton(490, 378, 4007, 4007, 11, GumpButtonType.Reply, 0); //Next
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            bool reloadPage = false;

            switch (info.ButtonID)
            {
                //Table of Contents
                case 1:
                    player.m_MHSPlayerEntry.m_GrimoirePage = MHSPlayerEntry.GrimoirePage.Main;
            
                    player.CloseGump(typeof(MHSTableOfContentsGump));
                    player.SendGump(new MHSTableOfContentsGump(player));

                    player.SendSound(MHSGrimoire.GumpSound);
            
                    return;
                break;

                //Previous Page
                case 10:
                    if (m_PageNumber <= m_MaxPageCount && m_PageNumber > 1)
                    {
                        switch (player.m_MHSPlayerEntry.m_GrimoirePage)
                        {
                            case MHSPlayerEntry.GrimoirePage.Boss: player.m_MHSPlayerEntry.m_BossPageNumber--; break;
                            case MHSPlayerEntry.GrimoirePage.Champion: player.m_MHSPlayerEntry.m_ChampionPageNumber--; break;
                            case MHSPlayerEntry.GrimoirePage.LoHBoss: player.m_MHSPlayerEntry.m_LoHBossPageNumber--; break;
                            case MHSPlayerEntry.GrimoirePage.Rare: player.m_MHSPlayerEntry.m_RarePageNumber--; break;
                            case MHSPlayerEntry.GrimoirePage.Paragon: player.m_MHSPlayerEntry.m_ParagonPageNumber--; break;
                        }
                    }

                    reloadPage = true;
                break;

                //Next Page
                case 11:
                    if (m_PageNumber < m_MaxPageCount)
                    {
                        switch (player.m_MHSPlayerEntry.m_GrimoirePage)
                        {
                            case MHSPlayerEntry.GrimoirePage.Boss: player.m_MHSPlayerEntry.m_BossPageNumber++; break;
                            case MHSPlayerEntry.GrimoirePage.Champion: player.m_MHSPlayerEntry.m_ChampionPageNumber++; break;
                            case MHSPlayerEntry.GrimoirePage.LoHBoss: player.m_MHSPlayerEntry.m_LoHBossPageNumber++; break;
                            case MHSPlayerEntry.GrimoirePage.Rare: player.m_MHSPlayerEntry.m_RarePageNumber++; break;
                            case MHSPlayerEntry.GrimoirePage.Paragon: player.m_MHSPlayerEntry.m_ParagonPageNumber++; break;
                        }
                    }

                    reloadPage = true;
                break;
            }

            if (reloadPage)
            {
                player.CloseGump(typeof(MHSCreatureEntryGump));
                player.SendGump(new MHSCreatureEntryGump(player));

                player.SendSound(MHSGrimoire.GumpSound);
            }
        }
    }
}