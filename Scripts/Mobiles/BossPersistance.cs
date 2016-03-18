using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Accounting;
using Server.Misc;
using Server.Spells;
using Server.Gumps;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;
using Server.Regions;
using Server.Multis;

namespace Server
{
    public static class BossPersistance
    {       
        public static BossPersistanceTracker PersistanceItem;

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new BossPersistanceTracker();
            });
        }

        public static void ResetSequentialSpawner(XmlSpawner xmlSpawner)
        {
            if (xmlSpawner == null)
                return;

            if (xmlSpawner == BossPersistance.PersistanceItem.CovetousSequential)            
                BossPersistance.PersistanceItem.CovetousSequentialLastStatusChange = DateTime.UtcNow;            

            if (xmlSpawner == BossPersistance.PersistanceItem.DeceitSequential)
                BossPersistance.PersistanceItem.DeceitSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.DespiseSequential)
                BossPersistance.PersistanceItem.DespiseSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.DestardSequential)
                BossPersistance.PersistanceItem.DestardSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.FireDungeonSequential)
                BossPersistance.PersistanceItem.FireDungeonSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.HythlothSequential)
                BossPersistance.PersistanceItem.HythlothSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.IceDungeonSequential)
                BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.ShameSequential)
                BossPersistance.PersistanceItem.ShameSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.WrongSequential)
                BossPersistance.PersistanceItem.WrongSequentialLastStatusChange = DateTime.UtcNow;

            if (xmlSpawner == BossPersistance.PersistanceItem.OceanSequential)
                BossPersistance.PersistanceItem.OceanSequentialLastStatusChange = DateTime.UtcNow;
        }

        public static string GetBossStatus(SpeechEventArgs e)
        {
            string bossResponse = "";

            if (BossPersistance.PersistanceItem == null) return bossResponse;
            if (BossPersistance.PersistanceItem.Deleted) return bossResponse;

            TimeSpan BossStatusUpdateDelay = TimeSpan.FromMinutes(120); //2 Hour Delay
            TimeSpan SequentialStatusUpdateDelay = TimeSpan.FromMinutes(480); //6 Hour Timer + 2 Hour Delay

            bool bossExists = false;
            bool sequentialExists = false;

            DateTime AllowableTime = DateTime.MaxValue;

            if (e.Speech.ToLower().IndexOf("rumors") >= 0)
            {
                #region Dungeon Response Text

                //Covetous
                if (e.Speech.ToLower().IndexOf("covetous") >= 0)
                {                    
                    if (BossPersistance.PersistanceItem.CovetousBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.CovetousBoss.Deleted)
                            bossExists = true;  
     
                        if (BossPersistance.PersistanceItem.CovetousBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.CovetousBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.CovetousBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that the Demonweb Queen has awakened within Covetous Dungeon";
                        else
                            bossResponse = "Word has it that Demonweb Queen lies dormant within Covetous Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.CovetousSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.CovetousSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.CovetousSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.CovetousSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and her minions are ready to unleash their fury.";
                            else
                                bossResponse += " and her minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.CovetousSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.CovetousSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.CovetousSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.CovetousSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Covetous Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Covetous Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += "."; 
                }

                //Deceit
                if (e.Speech.ToLower().IndexOf("deceit") >= 0)
                {
                    if (BossPersistance.PersistanceItem.DeceitBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.DeceitBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.DeceitBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.DeceitBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.DeceitBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that the Ancient Necromancer has awakened within Deceit Dungeon";
                        else
                            bossResponse = "Word has it that Ancient Necromancer lies dormant within Deceit Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.DeceitSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.DeceitSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.DeceitSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.DeceitSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and his minions are ready to unleash their fury.";
                            else
                                bossResponse += " and his minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.DeceitSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.DeceitSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.DeceitSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.DeceitSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Deceit Dungeonare ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Deceit Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                //Despise
                if (e.Speech.ToLower().IndexOf("despise") >= 0)
                {
                    if (BossPersistance.PersistanceItem.DespiseBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.DespiseBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.DespiseBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.DespiseBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.DespiseBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that Maggot has awakened within Despise Dungeon";
                        else
                            bossResponse = "Word has it that Maggot lies dormant within Despise Dungeon";
                    }


                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.DespiseSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.DespiseSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.DespiseSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.DespiseSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and his minions are ready to unleash their fury.";
                            else
                                bossResponse += " and his minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.DespiseSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.DespiseSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.DespiseSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.DespiseSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Despise Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Despise Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                //Destard
                if (e.Speech.ToLower().IndexOf("destard") >= 0)
                {
                    if (BossPersistance.PersistanceItem.DestardBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.DestardBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.DestardBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.DestardBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.DestardBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that Lyth the Destroyer has awakened within Destard Dungeon";
                        else
                            bossResponse = "Word has it that Lyth the Destroyer lies dormant within Destard Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.DestardSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.DestardSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.DestardSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.DestardSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and her minions are ready to unleash their fury.";
                            else
                                bossResponse += " and her minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.DestardSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.DestardSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.DestardSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.DestardSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Destard Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Destard Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                //Fire Dungeon
                if (e.Speech.ToLower().IndexOf("fire") >= 0)
                {
                    if (BossPersistance.PersistanceItem.FireDungeonBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.FireDungeonBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.FireDungeonBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.FireDungeonBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.FireDungeonBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "";
                        else
                            bossResponse = "";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.FireDungeonSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.FireDungeonSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.FireDungeonSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.FireDungeonSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "";
                            else
                                bossResponse += "";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.FireDungeonSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.FireDungeonSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.FireDungeonSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.FireDungeonSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Fire Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Fire Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }
                    
                //Hythloth
                if (e.Speech.ToLower().IndexOf("hythloth") >= 0)
                {
                    if (BossPersistance.PersistanceItem.HythlothBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.HythlothBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.HythlothBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.HythlothBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.HythlothBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that General Peradun has awakened within Hythloth Dungeon";
                        else
                            bossResponse = "Word has it that General Peradun lies dormant within Hythloth Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.HythlothSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.HythlothSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.HythlothSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.HythlothSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and his minions are ready to unleash their fury.";
                            else
                                bossResponse += " and his minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.HythlothSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.HythlothSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.HythlothSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.HythlothSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Hythloth Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Hythloth Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                //Ice Dungeon
                if (e.Speech.ToLower().IndexOf("ice") >= 0)
                {
                    if (BossPersistance.PersistanceItem.IceDungeonBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.IceDungeonBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.IceDungeonBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.IceDungeonBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.IceDungeonBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that The Voidbringer has awakened within Ice Dungeon";
                        else
                            bossResponse = "Word has it that The Voidbringer Frost lies dormant within Ice Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.IceDungeonSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.IceDungeonSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and its minions are ready to unleash their fury.";
                            else
                                bossResponse += " and its minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.IceDungeonSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.IceDungeonSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Ice Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Ice Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }
                
                //Shame
                if (e.Speech.ToLower().IndexOf("shame") >= 0)
                {
                    if (BossPersistance.PersistanceItem.ShameBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.ShameBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.ShameBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.ShameBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.ShameBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that The Elder Stone has awakened within Shame Dungeon";
                        else
                            bossResponse = "Word has it that The Elder Stone lies dormant within Shame Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.ShameSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.ShameSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and its minions are ready to unleash their fury.";
                            else
                                bossResponse += " and its minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.ShameSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.ShameSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Shame Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Shame Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                //Wrong
                if (e.Speech.ToLower().IndexOf("wrong") >= 0)
                {
                    if (BossPersistance.PersistanceItem.WrongBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.WrongBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.WrongBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.WrongBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.WrongBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that Atraxis has awakened within Wrong Dungeon";
                        else
                            bossResponse = "Word has it that Atraxis Peradun lies dormant within Wrong Dungeon";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.WrongSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.WrongSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.WrongSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.WrongSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and her minions are ready to unleash their fury.";
                            else
                                bossResponse += " and her minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.WrongSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.WrongSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.WrongSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.WrongSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of Wrong Dungeon are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of Wrong Dungeon are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                //Ocean
                if (e.Speech.ToLower().IndexOf("ocean") >= 0)
                {                   
                    if (BossPersistance.PersistanceItem.OceanBoss != null)
                    {
                        if (!BossPersistance.PersistanceItem.OceanBoss.Deleted)
                            bossExists = true;

                        if (BossPersistance.PersistanceItem.OceanBossLastStatusChange != DateTime.MaxValue)
                            AllowableTime = BossPersistance.PersistanceItem.OceanBossLastStatusChange.Add(BossStatusUpdateDelay);

                        if (BossPersistance.PersistanceItem.OceanBoss.CurrentCount > 0 && bossExists && AllowableTime <= DateTime.UtcNow)
                            bossResponse = "Word has it that The Deep One has awakened within it's ocean lair";
                        else
                            bossResponse = "Word has it that The Deep One lies dormant within it's ocean lair";
                    }

                    if (bossExists)
                    {
                        if (BossPersistance.PersistanceItem.OceanSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.OceanSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.OceanSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.OceanSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += " and it's minions are ready to unleash their fury.";
                            else
                                bossResponse += " and it's minions are still gathering their strength.";
                        }
                    }

                    else
                    {
                        if (BossPersistance.PersistanceItem.OceanSequential != null)
                        {
                            if (!BossPersistance.PersistanceItem.OceanSequential.Deleted)
                                sequentialExists = true;

                            if (BossPersistance.PersistanceItem.OceanSequentialLastStatusChange != DateTime.MaxValue)
                                AllowableTime = BossPersistance.PersistanceItem.OceanSequentialLastStatusChange.Add(SequentialStatusUpdateDelay);

                            if (sequentialExists && AllowableTime <= DateTime.UtcNow)
                                bossResponse += "The minions of The Deep One are ready to unleash their fury.";
                            else
                                bossResponse += "The minions of The Deep One are still gathering their strength.";
                        }
                    }

                    if (bossExists && !sequentialExists)
                        bossResponse += ".";
                }

                #endregion
            }

            if (!bossExists && !sequentialExists)
                bossResponse += "Word has not reached my ears of the evils that dwell there.";

            return bossResponse;
        }
        
        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version      
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();
        }
    }
       
    public class BossPersistanceTrackerBook : Item
    {
        [Constructable]
        public BossPersistanceTrackerBook(): base(0x2253)
        {
            Hue = 1107;
            Name = "Boss Tracker Book";

            LootType = LootType.Blessed;
        }

        public BossPersistanceTrackerBook(Serial serial): base(serial)
        {
        }

        #region Dungeon Bosses / Sequential Properties

        //Covetous
        private XmlSpawner m_CovetousBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner CovetousBoss
        {
            get { return m_CovetousBoss; }
            set
            {
                m_CovetousBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.CovetousBoss = value;
            }
        }

        private DateTime m_CovetousBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CovetousBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.CovetousBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            }            
        }

        private XmlSpawner m_CovetousSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner CovetousSequential
        {
            get { return m_CovetousSequential; }
            set
            {
                m_CovetousSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.CovetousSequential = value;
            }
        }

        private DateTime m_CovetousSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CovetousSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.CovetousSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            }  
        }

        //Deceit
        private XmlSpawner m_DeceitBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DeceitBoss
        {
            get { return m_DeceitBoss; }
            set
            {
                m_DeceitBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.DeceitBoss = value;
            }
        }

        private DateTime m_DeceitBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeceitBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.DeceitBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_DeceitSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DeceitSequential
        {
            get { return m_DeceitSequential; }
            set
            {
                m_DeceitSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.DeceitSequential = value;
            }
        }

        private DateTime m_DeceitSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeceitSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.DeceitSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Despise
        private XmlSpawner m_DespiseBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DespiseBoss
        {
            get { return m_DespiseBoss; }
            set
            {
                m_DespiseBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.DespiseBoss = value;
            }
        }

        private DateTime m_DespiseBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DespiseBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.DespiseBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_DespiseSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DespiseSequential
        {
            get { return m_DespiseSequential; }
            set
            {
                m_DespiseSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.DespiseSequential = value;
            }
        }

        private DateTime m_DespiseSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DespiseSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.DespiseSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Destard
        private XmlSpawner m_DestardBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DestardBoss
        {
            get { return m_DestardBoss; }
            set
            {
                m_DestardBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.DestardBoss = value;
            }
        }

        private DateTime m_DestardBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DestardBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.DestardBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_DestardSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DestardSequential
        {
            get { return m_DestardSequential; }
            set
            {
                m_DestardSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.DestardSequential = value;
            }
        }

        private DateTime m_DestardSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DestardSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.DestardSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Fire Dungeon
        private XmlSpawner m_FireDungeonBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner FireDungeonBoss
        {
            get { return m_FireDungeonBoss; }
            set
            {
                m_FireDungeonBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.FireDungeonBoss = value;
            }
        }

        private DateTime m_FireDungeonBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FireDungeonBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.FireDungeonBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_FireDungeonSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner FireDungeonSequential
        {
            get { return m_FireDungeonSequential; }
            set
            {
                m_FireDungeonSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.FireDungeonSequential = value;
            }
        }

        private DateTime m_FireDungeonSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FireDungeonSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.FireDungeonSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Hythloth
        private XmlSpawner m_HythlothBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner HythlothBoss
        {
            get { return m_HythlothBoss; }
            set
            {
                m_HythlothBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.HythlothBoss = value;
            }
        }

        private DateTime m_HythlothBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime HythlothBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.HythlothBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_HythlothSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner HythlothSequential
        {
            get { return m_HythlothSequential; }
            set
            {
                m_HythlothSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.HythlothSequential = value;
            }
        }

        private DateTime m_HythlothSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime HythlothSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.HythlothSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Ice Dungeon
        private XmlSpawner m_IceDungeonBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner IceDungeonBoss
        {
            get { return m_IceDungeonBoss; }
            set
            {
                m_IceDungeonBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.IceDungeonBoss = value;
            }
        }

        private DateTime m_IceDungeonBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime IceDungeonBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.IceDungeonBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_IceDungeonSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner IceDungeonSequential
        {
            get { return m_IceDungeonSequential; }
            set
            {
                m_IceDungeonSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.IceDungeonSequential = value;
            }
        }

        private DateTime m_IceDungeonSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime IceDungeonSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.IceDungeonSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Shame
        private XmlSpawner m_ShameBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner ShameBoss
        {
            get { return m_ShameBoss; }
            set
            {
                m_ShameBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.ShameBoss = value;
            }
        }

        private DateTime m_ShameBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ShameBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.ShameBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_ShameSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner ShameSequential
        {
            get { return m_ShameSequential; }
            set
            {
                m_ShameSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.ShameSequential = value;
            }
        }

        private DateTime m_ShameSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ShameSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.ShameSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Wrong
        private XmlSpawner m_WrongBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner WrongBoss
        {
            get { return m_WrongBoss; }
            set
            {
                m_WrongBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.WrongBoss = value;
            }
        }

        private DateTime m_WrongBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime WrongBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.WrongBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        private XmlSpawner m_WrongSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner WrongSequential
        {
            get { return m_WrongSequential; }
            set
            {
                m_WrongSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.WrongSequential = value;
            }
        }

        private DateTime m_WrongSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime WrongSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.WrongSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            } 
        }

        //Ocean
        private XmlSpawner m_OceanBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner OceanBoss
        {
            get { return m_OceanBoss; }
            set
            {
                m_OceanBoss = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.OceanBoss = value;
            }
        }

        private DateTime m_OceanBossLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime OceanBossLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.OceanBossLastStatusChange;
                else
                    return DateTime.MaxValue;
            }
        }

        private XmlSpawner m_OceanSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner OceanSequential
        {
            get { return m_OceanSequential; }
            set
            {
                m_OceanSequential = value;

                if (BossPersistance.PersistanceItem != null)
                    BossPersistance.PersistanceItem.OceanSequential = value;
            }
        }

        private DateTime m_OceanSequentialLastStatusChange;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime OceanSequentialLastStatusChange
        {
            get
            {
                if (BossPersistance.PersistanceItem != null)
                    return BossPersistance.PersistanceItem.OceanSequentialLastStatusChange;
                else
                    return DateTime.MaxValue;
            }
        }

        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version 

            //Version 0
            writer.Write(CovetousBoss);
            writer.Write(CovetousSequential);

            writer.Write(DeceitBoss);
            writer.Write(DeceitSequential);

            writer.Write(DespiseBoss);
            writer.Write(DespiseSequential);

            writer.Write(DestardBoss);
            writer.Write(DestardSequential);

            writer.Write(FireDungeonBoss);
            writer.Write(FireDungeonSequential);

            writer.Write(HythlothBoss);
            writer.Write(HythlothSequential);

            writer.Write(IceDungeonBoss);
            writer.Write(IceDungeonSequential);

            writer.Write(ShameBoss);
            writer.Write(ShameSequential);

            writer.Write(WrongBoss);
            writer.Write(WrongSequential);

            //Version 1
            writer.Write(OceanBoss);
            writer.Write(OceanSequential);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                CovetousBoss = (XmlSpawner)reader.ReadItem();
                CovetousSequential = (XmlSpawner)reader.ReadItem();

                DeceitBoss = (XmlSpawner)reader.ReadItem();
                DeceitSequential = (XmlSpawner)reader.ReadItem();

                DespiseBoss = (XmlSpawner)reader.ReadItem();
                DespiseSequential = (XmlSpawner)reader.ReadItem();

                DestardBoss = (XmlSpawner)reader.ReadItem();
                DestardSequential = (XmlSpawner)reader.ReadItem();

                FireDungeonBoss = (XmlSpawner)reader.ReadItem();
                FireDungeonSequential = (XmlSpawner)reader.ReadItem();

                HythlothBoss = (XmlSpawner)reader.ReadItem();
                HythlothSequential = (XmlSpawner)reader.ReadItem();

                IceDungeonBoss = (XmlSpawner)reader.ReadItem();
                IceDungeonSequential = (XmlSpawner)reader.ReadItem();                

                ShameBoss = (XmlSpawner)reader.ReadItem();
                ShameSequential = (XmlSpawner)reader.ReadItem();

                WrongBoss = (XmlSpawner)reader.ReadItem();
                WrongSequential = (XmlSpawner)reader.ReadItem();
            }

            //Version 1
            if (version >= 1)
            {
                OceanBoss = (XmlSpawner)reader.ReadItem();
                OceanSequential = (XmlSpawner)reader.ReadItem();
            }
        } 
    }    

    public class BossPersistanceTracker : Item
    {
        public override string DefaultName { get { return "Boss Persistance"; } }
        

        public BossPersistanceTracker(): base(0x0)
        {
        }

        public BossPersistanceTracker(Serial serial): base(serial)
        {
        }

        #region Dungeon Bosses / Sequential Properties

        //Covetous
        private XmlSpawner m_CovetousBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner CovetousBoss
        {
            get { return m_CovetousBoss; }
            set{m_CovetousBoss = value; }
        }

        private DateTime m_CovetousBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CovetousBossLastStatusChange
        {
            get { return m_CovetousBossLastStatusChange; }
            set { m_CovetousBossLastStatusChange = value; }
        }

        private XmlSpawner m_CovetousSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner CovetousSequential
        {
            get { return m_CovetousSequential; }
            set { m_CovetousSequential = value; }
        }

        private DateTime m_CovetousSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime CovetousSequentialLastStatusChange
        {
            get { return m_CovetousSequentialLastStatusChange; }
            set { m_CovetousSequentialLastStatusChange = value; }
        }

        //Deceit
        private XmlSpawner m_DeceitBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DeceitBoss
        {
            get { return m_DeceitBoss; }
            set{m_DeceitBoss = value;}
        }

        private DateTime m_DeceitBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeceitBossLastStatusChange
        {
            get { return m_DeceitBossLastStatusChange; }
            set { m_DeceitBossLastStatusChange = value; }
        }

        private XmlSpawner m_DeceitSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DeceitSequential
        {
            get { return m_DeceitSequential; }
            set { m_DeceitSequential = value; }
        }

        private DateTime m_DeceitSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DeceitSequentialLastStatusChange
        {
            get { return m_DeceitSequentialLastStatusChange; }
            set { m_DeceitSequentialLastStatusChange = value; }
        }

        //Despise
        private XmlSpawner m_DespiseBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DespiseBoss
        {
            get { return m_DespiseBoss; }
            set{ m_DespiseBoss = value;}
        }

        private DateTime m_DespiseBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DespiseBossLastStatusChange
        {
            get { return m_DespiseBossLastStatusChange; }
            set { m_DespiseBossLastStatusChange = value; }
        }

        private XmlSpawner m_DespiseSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DespiseSequential
        {
            get { return m_DespiseSequential; }
            set { m_DespiseSequential = value;}
        }

        private DateTime m_DespiseSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DespiseSequentialLastStatusChange
        {
            get { return m_DespiseSequentialLastStatusChange; }
            set { m_DespiseSequentialLastStatusChange = value; }
        }

        //Destard
        private XmlSpawner m_DestardBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DestardBoss
        {
            get { return m_DestardBoss; }
            set { m_DestardBoss = value; }
        }

        private DateTime m_DestardBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DestardBossLastStatusChange
        {
            get { return m_DestardBossLastStatusChange; }
            set { m_DestardBossLastStatusChange = value; }
        }

        private XmlSpawner m_DestardSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner DestardSequential
        {
            get { return m_DestardSequential; }
            set {m_DestardSequential = value;}
        }

        private DateTime m_DestardSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DestardSequentialLastStatusChange
        {
            get { return m_DestardSequentialLastStatusChange; }
            set { m_DestardSequentialLastStatusChange = value; }
        }

        //Fire Dungeon
        private XmlSpawner m_FireDungeonBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner FireDungeonBoss
        {
            get { return m_FireDungeonBoss; }
            set { m_FireDungeonBoss = value; }
        }

        private DateTime m_FireDungeonBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FireDungeonBossLastStatusChange
        {
            get { return m_FireDungeonBossLastStatusChange; }
            set { m_FireDungeonBossLastStatusChange = value; }
        }

        private XmlSpawner m_FireDungeonSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner FireDungeonSequential
        {
            get { return m_FireDungeonSequential; }
            set{m_FireDungeonSequential = value; }
        }

        private DateTime m_FireDungeonSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime FireDungeonSequentialLastStatusChange
        {
            get { return m_FireDungeonSequentialLastStatusChange; }
            set { m_FireDungeonSequentialLastStatusChange = value; }
        }

        //Hythloth
        private XmlSpawner m_HythlothBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner HythlothBoss
        {
            get { return m_HythlothBoss; }
            set{  m_HythlothBoss = value;}
        }

        private DateTime m_HythlothBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime HythlothBossLastStatusChange
        {
            get { return m_HythlothBossLastStatusChange; }
            set { m_HythlothBossLastStatusChange = value; }
        }

        private XmlSpawner m_HythlothSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner HythlothSequential
        {
            get { return m_HythlothSequential; }
            set{m_HythlothSequential = value; }
        }

        private DateTime m_HythlothSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime HythlothSequentialLastStatusChange
        {
            get { return m_HythlothSequentialLastStatusChange; }
            set { m_HythlothSequentialLastStatusChange = value; }
        }

        //Ice Dungeon
        private XmlSpawner m_IceDungeonBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner IceDungeonBoss
        {
            get { return m_IceDungeonBoss; }
            set {m_IceDungeonBoss = value;}
        }

        private DateTime m_IceDungeonBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime IceDungeonBossLastStatusChange
        {
            get { return m_IceDungeonBossLastStatusChange; }
            set { m_IceDungeonBossLastStatusChange = value; }
        }

        private XmlSpawner m_IceDungeonSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner IceDungeonSequential
        {
            get { return m_IceDungeonSequential; }
            set{m_IceDungeonSequential = value;}
        }

        private DateTime m_IceDungeonSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime IceDungeonSequentialLastStatusChange
        {
            get { return m_IceDungeonSequentialLastStatusChange; }
            set { m_IceDungeonSequentialLastStatusChange = value; }
        }

        //Shame
        private XmlSpawner m_ShameBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner ShameBoss
        {
            get { return m_ShameBoss; }
            set{m_ShameBoss = value;}
        }

        private DateTime m_ShameBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ShameBossLastStatusChange
        {
            get { return m_ShameBossLastStatusChange; }
            set { m_ShameBossLastStatusChange = value; }
        }

        private XmlSpawner m_ShameSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner ShameSequential
        {
            get { return m_ShameSequential; }
            set { m_ShameSequential = value; }
        }

        private DateTime m_ShameSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime ShameSequentialLastStatusChange
        {
            get { return m_ShameSequentialLastStatusChange; }
            set { m_ShameSequentialLastStatusChange = value; }
        }

        //Wrong
        private XmlSpawner m_WrongBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner WrongBoss
        {
            get { return m_WrongBoss; }
            set{ m_WrongBoss = value;}
        }

        private DateTime m_WrongBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime WrongBossLastStatusChange
        {
            get { return m_WrongBossLastStatusChange; }
            set { m_WrongBossLastStatusChange = value; }
        }

        private XmlSpawner m_WrongSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner WrongSequential
        {
            get { return m_WrongSequential; }
            set {m_WrongSequential = value;}
        }

        private DateTime m_WrongSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime WrongSequentialLastStatusChange
        {
            get { return m_WrongSequentialLastStatusChange; }
            set { m_WrongSequentialLastStatusChange = value; }
        }        

        //Ocean Boss
        private XmlSpawner m_OceanBoss;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner OceanBoss
        {
            get { return m_OceanBoss; }
            set { m_OceanBoss = value; }
        }

        private DateTime m_OceanBossLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime OceanBossLastStatusChange
        {
            get { return m_OceanBossLastStatusChange; }
            set { m_OceanBossLastStatusChange = value; }
        }

        private XmlSpawner m_OceanSequential;
        [CommandProperty(AccessLevel.GameMaster)]
        public XmlSpawner OceanSequential
        {
            get { return m_OceanSequential; }
            set { m_OceanSequential = value; }
        }

        private DateTime m_OceanSequentialLastStatusChange = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime OceanSequentialLastStatusChange
        {
            get { return m_OceanSequentialLastStatusChange; }
            set { m_OceanSequentialLastStatusChange = value; }
        }

        #endregion

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            BossPersistance.Serialize(writer);

            writer.Write(CovetousBoss);
            writer.Write(CovetousSequential);

            writer.Write(DeceitBoss);
            writer.Write(DeceitSequential);

            writer.Write(DespiseBoss);
            writer.Write(DespiseSequential);

            writer.Write(DestardBoss);
            writer.Write(DestardSequential);

            writer.Write(FireDungeonBoss);
            writer.Write(FireDungeonSequential);

            writer.Write(HythlothBoss);
            writer.Write(HythlothSequential);

            writer.Write(IceDungeonBoss);
            writer.Write(IceDungeonSequential);

            writer.Write(ShameBoss);
            writer.Write(ShameSequential);

            writer.Write(WrongBoss);
            writer.Write(WrongSequential);
  
            //Version 1
            writer.Write(OceanBoss);
            writer.Write(OceanSequential);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            BossPersistance.PersistanceItem = this;
            BossPersistance.Deserialize(reader);

            if (version >= 0)
            {
                CovetousBoss = (XmlSpawner)reader.ReadItem();
                CovetousSequential = (XmlSpawner)reader.ReadItem();

                DeceitBoss = (XmlSpawner)reader.ReadItem();
                DeceitSequential = (XmlSpawner)reader.ReadItem();

                DespiseBoss = (XmlSpawner)reader.ReadItem();
                DespiseSequential = (XmlSpawner)reader.ReadItem();

                DestardBoss = (XmlSpawner)reader.ReadItem();
                DestardSequential = (XmlSpawner)reader.ReadItem();

                FireDungeonBoss = (XmlSpawner)reader.ReadItem();
                FireDungeonSequential = (XmlSpawner)reader.ReadItem();

                HythlothBoss = (XmlSpawner)reader.ReadItem();
                HythlothSequential = (XmlSpawner)reader.ReadItem();

                IceDungeonBoss = (XmlSpawner)reader.ReadItem();
                IceDungeonSequential = (XmlSpawner)reader.ReadItem();

                ShameBoss = (XmlSpawner)reader.ReadItem();
                ShameSequential = (XmlSpawner)reader.ReadItem();

                WrongBoss = (XmlSpawner)reader.ReadItem();
                WrongSequential = (XmlSpawner)reader.ReadItem();
            }

            //Version 1
            if (version >= 1)
            {
                OceanBoss = (XmlSpawner)reader.ReadItem();
                OceanSequential = (XmlSpawner)reader.ReadItem();
            }
        }        
    }
}
