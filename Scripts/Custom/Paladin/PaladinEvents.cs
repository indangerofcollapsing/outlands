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
    public static class PaladinEvents
    {
        public static List<DeathEventEntry> m_DeathEventEntries = new List<DeathEventEntry>();
        public static DeathEventPersistance PersistanceItem;

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (PersistanceItem == null)
                    PersistanceItem = new DeathEventPersistance();
            });
        }

        public static Rectangle2D MurdererSafeZone = new Rectangle2D(new Point2D(1449, 779), new Point2D(1578, 895));

        public const int RestitutionFeeCostPerCount = 500;
        public const double RestitutionFeeDistributionPercent = .80;
        
        public static int minimumPaladinPenanceDuration = 60; //Minimum Statloss Timer in Minutes For Murders Killed By Paladins        
        public static int penancePerMurderCount = 10; //Additional Statloss Timer in Minutes Per Murder Count Above 5
        public static int maximumPenanceDuration = 720; //Maximum Statloss Timer Amount in Minutes
        
        public static int dishonorDuration = 30; //Minimum Statloss Timer in Minutes For Paladins killed by Murderer  

        public static int recallRestrictionVictimTimeout = 120; //Number of Seconds Until Victim Has to Punish Extra Killers Involved in Murder
        public static int recallRestrictionMinimum = 120; //Automatic Number of Seconds of Recall Restriction After Any Kill That Can Be Reported        
        public static int recallRestrictionDurationPerKiller = 0; //Duration in Seconds for No Recall/Gate Usage on a Murder Per Additional Killer Involved        

        public static double StatLossStatScalar = 1.0; //Scalar for Stat Loss on Temp Statloss
        public static double StatLossSkillScalar = 0.5; //Scalar for Skill Loss on Temp Statloss

        public static int paladinDismissalTextHue = 0x22;

        public static int PaladinCriminalPunishmentHours = 24;
        public static int PaladinAidingMurdererDays = 3;
        public static int PaladinCommitMurderDays = 5;

        public static double platinumValuePerDreadCoin = 2.0;

        public static TimeSpan uniqueMurderInterval = TimeSpan.FromHours(4);
        public static TimeSpan resKillInterval = TimeSpan.FromMinutes(30);

        public static TimeSpan DeathEntryExpiration = TimeSpan.FromMinutes(10); //Expand to 24 Hour Eventually for Admin Viewing Purposes
        
        public static bool AddDeathEventEntry(DeathEventEntry entry)
        {
            AuditDeathEntries();

            bool foundDuplicate = false;

            foreach (DeathEventEntry existingEntry in m_DeathEventEntries)
            {
                if (entry.m_Victim == existingEntry.m_Victim && entry.m_EventTime == existingEntry.m_EventTime)
                {
                    foundDuplicate = true;
                    break;
                }                
            }

            if (!foundDuplicate)
                m_DeathEventEntries.Add(entry);

            BaseOrb.HandleDeath(entry.m_DeathEventType);

            return true;
        }

        public static void AuditDeathEntries()
        {
            List<DeathEventEntry> m_EntriesToDelete = new List<DeathEventEntry>();

            foreach (DeathEventEntry entry in m_DeathEventEntries)
            {
                if (entry.m_EventTime + DeathEntryExpiration < DateTime.UtcNow)
                    m_EntriesToDelete.Add(entry);
            }

            int entriesToDelete = m_EntriesToDelete.Count;

            for (int a = 0; a < entriesToDelete; a++)
            {
                m_DeathEventEntries.Remove(m_EntriesToDelete[a]);
            }
        }

        public static void PaladinKillMurdererResult(PlayerMobile paladin, PlayerMobile murderer, bool isHighestDamager, Point3D location, Map map)
        {
            bool shareCommonGuild = paladin.CheckPlayerAccountsForCommonGuild(murderer);
            bool dungeonDeath = Region.Find(location, map) is DungeonRegion;
            bool houseDeath = BaseHouse.FindHouseAt(location, map, location.Z) != null;
           
            double platinumValue = (double)murderer.PaladinCurrencyValue;
            
            if (shareCommonGuild)
                platinumValue = 0;

            if (platinumValue > 0)
            {
                double playerClassGearBonus = 1 + (PlayerClassPersistance.PlayerClassCurrencyBonusPerItem * (double)PlayerClassPersistance.GetPlayerClassArmorItemCount(paladin, PlayerClass.Paladin));

                platinumValue *= playerClassGearBonus;

                if (paladin.Citizenship != null && paladin.Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Greed))
                    platinumValue *= 1.1;

                if (platinumValue < 1)
                    platinumValue = 1;

                if (isHighestDamager)
                {
                    int fullPlatinum = (int)platinumValue;

                    Banker.DepositUniqueCurrency(paladin, typeof(Platinum), fullPlatinum);
                    paladin.SendMessage("You have been awarded " + fullPlatinum.ToString() + " platinum for the apprehension of a vile murderer. The coins have been deposited in your bank and the Order of the Silver Serpent thanks you for your efforts.");

                    paladin.PaladinScore += fullPlatinum;

                    Platinum platinumSoundPile = new Platinum(fullPlatinum);
                    paladin.PlaySound(platinumSoundPile.GetDropSound());
                    platinumSoundPile.Delete();
                }

                else
                {
                    int halfPlatinum = (int)(platinumValue / 2);

                    Banker.DepositUniqueCurrency(paladin, typeof(Platinum), halfPlatinum);
                    paladin.SendMessage("You have been awarded " + halfPlatinum.ToString() + " platinum for contributing to the apprehension of a vile murderer. The coins have been deposited in your bank and the Order of the Silver Serpent thanks you for your efforts.");

                    paladin.PaladinScore += halfPlatinum;

                    Platinum platinumSoundPile = new Platinum(halfPlatinum);
                    paladin.PlaySound(platinumSoundPile.GetDropSound());
                    platinumSoundPile.Delete();
                }
            }

            else            
                paladin.SendMessage("The Order of the Silver Serpent regretfully cannot offer a reward for the apprehension of that individual.");            
        }

        public static void ReportMurder(PlayerMobile killer, PlayerMobile victim, int killerCount, Point3D location, Map map)
        {
            if (killer == null || victim == null)
                return;

            if (killer.Deleted || victim.Deleted)
                return;
           
            bool shareCommonGuild = killer.CheckPlayerAccountsForCommonGuild(victim);
            bool dungeonMurder = Region.Find(location, map) is DungeonRegion;
            bool houseMurder = BaseHouse.FindHouseAt(location, map, location.Z) != null;
            bool resKill = false;
            bool uniqueMurder = false;
            bool multipleKillers = killerCount > 1;
            
            killer.Kills++;
            killer.ShortTermMurders++;
            killer.m_ShortTermMurders.Add(victim);  

            if (killer.PaladinRejoinAllowed < DateTime.UtcNow + TimeSpan.FromDays(PaladinEvents.PaladinCommitMurderDays))
                killer.PaladinRejoinAllowed = DateTime.UtcNow + TimeSpan.FromDays(PaladinEvents.PaladinCommitMurderDays);

            bool foundVictimInMurderList = false;
            
            foreach (KeyValuePair<PlayerMobile, DateTime> pair in killer.DictUniqueMurderEntries)
            {
                PlayerMobile oldVictim = pair.Key;
                DateTime oldMurderDate = pair.Value;

                if (oldVictim == victim)
                {
                    foundVictimInMurderList = true;

                    //Murderer Killed Victim Again Too Soon: Considered Res Kill
                    if (oldMurderDate + resKillInterval > DateTime.UtcNow)                        
                        resKill = true;
                        
                    //Uniqueness Interval Expired: Kill Qualifies for Advanced Dread Coin Value
                    if (oldMurderDate + uniqueMurderInterval <= DateTime.UtcNow)
                    {
                        uniqueMurder = true;
                        killer.UniqueMurders++;
                    }

                    killer.DictUniqueMurderEntries.Remove(pair.Key);
                    break;
                }
            }

            if (!foundVictimInMurderList)
            {
                uniqueMurder = true;
                killer.UniqueMurders++;
            }

            killer.DictUniqueMurderEntries.Add(victim, DateTime.UtcNow);
            killer.m_ShortTermMurders.Add(victim);

            //Vengeance List Entry                           
            victim.AddVengeanceEntry(killer, DateTime.UtcNow, VengeanceEntry.PointsRemainingDefault);

            //Killer Loses All Progress on Current Kill Count Decay
            killer.ResetKillTime();

            killer.SendLocalizedMessage(1049067); //You have been reported for murder!

            bool removeAllPaladins = false;
            bool removeAllOtherPaladins = false;

            if (killer.ShortTermMurders == 5 && !killer.Paladin)
            {
                killer.SendLocalizedMessage(502134); //You are now known as a murderer!
                removeAllOtherPaladins = true;
            }

            if (killer.ShortTermMurders == 5 && killer.Paladin)
            {
                killer.SendLocalizedMessage(502134); //You are now known as a murderer!
                removeAllPaladins = true;
            }

            if (killer.ShortTermMurders > 5 && !killer.Paladin)
                removeAllOtherPaladins = true;

            if (killer.ShortTermMurders > 5 && killer.Paladin)
                removeAllPaladins = true;

            if ((removeAllPaladins || removeAllOtherPaladins) && PaladinEvents.RemoveAllPaladinsOnAccount(killer, PaladinEvents.PaladinCommitMurderDays))
            {
                if (removeAllPaladins)
                {
                    killer.SendMessage(paladinDismissalTextHue, "Your act of murder goes against the very principles of the Order. You are no longer a Paladin of Trinsic, but may seek reinstatement in " + PaladinEvents.PaladinCommitMurderDays.ToString() + " days time.");
                    killer.SendMessage(paladinDismissalTextHue, "All of your paladin associates have been dismissed from the guild.");
                }
            }            

            else if (SkillHandlers.Stealing.SuspendOnMurder && killer.ShortTermMurders >= 5 && killer.NpcGuild == NpcGuild.ThievesGuild)
                killer.SendMessage(paladinDismissalTextHue, "You have been suspended by the Thieves Guild");

            else if (killer.ShortTermMurders >= 5 && killer.NpcGuild == NpcGuild.DetectivesGuild)
                killer.SendMessage(paladinDismissalTextHue, "You have been suspended by the Detective's Guild");

            //Award Dread Coin
            if (killer.ShortTermMurders > 5)
            {
                double dreadCoinValue = 4;

                if (uniqueMurder)
                    dreadCoinValue *= 10;

                if (dungeonMurder)
                    dreadCoinValue *= 2;

                if (dreadCoinValue < 1)
                    dreadCoinValue = 1;

                if (shareCommonGuild || resKill || houseMurder)
                    dreadCoinValue = 0;                

                if (dreadCoinValue > 0)
                {
                    //Increase the Worth of the Murderer to Paladins
                    killer.PaladinCurrencyValue += (int)(dreadCoinValue * platinumValuePerDreadCoin);

                    double playerClassGearBonus = 1 + (PlayerClassPersistance.PlayerClassCurrencyBonusPerItem * (double)PlayerClassPersistance.GetPlayerClassArmorItemCount(killer, PlayerClass.Murderer));

                    dreadCoinValue *= playerClassGearBonus;

                    if (killer.Citizenship != null && killer.Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Greed))
                        dreadCoinValue *= 1.1;

                    int finalDreadCoinValue = (int)dreadCoinValue;

                    Banker.DepositUniqueCurrency(killer, typeof(DreadCoin), finalDreadCoinValue);
                    killer.SendMessage("You have been awarded " + finalDreadCoinValue.ToString() + " dread coin for committing murder. The coins have been deposited in your bankbox.");

                    killer.MurdererScore += (int)dreadCoinValue;

                    DreadCoin dreadCoinSoundPile = new DreadCoin(finalDreadCoinValue);
                    killer.PlaySound(dreadCoinSoundPile.GetDropSound());
                    dreadCoinSoundPile.Delete();
                }
            }

            // IPY ACHIEVEMENT (accumulate murder counts)
            AchievementSystem.Instance.TickProgress(killer, AchievementTriggers.Trigger_GetMurderCount);
        }

        public static void DisburseRestitutionFees(PlayerMobile pm_Murderer)
        {
            if (pm_Murderer == null)
                return;

            Dictionary<Mobile, int> DictMurderInstances = new Dictionary<Mobile, int>();

            int disbursements = 0;

            foreach (Mobile mobile in pm_Murderer.m_ShortTermMurders)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;

                disbursements++;

                if (DictMurderInstances.ContainsKey(mobile))
                    DictMurderInstances[mobile] += 1;

                else                
                    DictMurderInstances.Add(mobile, 1);                
            }

            int totalFeesToDistribute = (int)((double)pm_Murderer.RestitutionFeesToDistribute * RestitutionFeeDistributionPercent);

            if (totalFeesToDistribute > 0 && disbursements > 0)
            {
                foreach (KeyValuePair<Mobile, int> entry in DictMurderInstances)
                {
                    PlayerMobile pm_Victim = entry.Key as PlayerMobile;

                    if (pm_Victim == null)
                        continue;

                    int timesMurdered = entry.Value;
                    int feeDistributed = (int)(((double)timesMurdered / (double)disbursements) * (double)totalFeesToDistribute);

                    if (feeDistributed < 1)
                        feeDistributed = 1;                    

                    pm_Victim.SendMessage("The vile murderer " + pm_Murderer.RawName + " has been brought to justice by the Paladins of the Order. They have seen fit to grant you the sum of " + feeDistributed.ToString() + " gold as part of the criminal's restitution.");
                    
                    BankBox bankBox = pm_Victim.BankBox;

                    if (bankBox != null)
                    {
                        Gold coinPile = new Gold(feeDistributed);
                        pm_Victim.SendSound(coinPile.GetDropSound());
                        coinPile.Delete();

                        if (Banker.Deposit(pm_Victim, feeDistributed))
                        {
                            if (bankBox.TotalItems < bankBox.MaxItems)
                                bankBox.AddItem(new VictimNote(pm_Murderer.LastPlayerKilledBy, pm_Murderer, pm_Victim, feeDistributed));
                        }
                    }
                }
            }

            MurdererPunishmentResolved(pm_Murderer);
        }

        public static void MurdererPunishmentResolved(PlayerMobile pm_Murderer)
        {
            pm_Murderer.m_ShortTermMurders.Clear();
            pm_Murderer.m_PaladinsKilled.Clear();
            pm_Murderer.LastPlayerKilledBy = null;
            pm_Murderer.KilledByPaladin = false;

            pm_Murderer.RestitutionFee = 0;
            pm_Murderer.RestitutionFeesToDistribute = 0;

            if (pm_Murderer.ShortTermMurders > 5)
                pm_Murderer.ShortTermMurders = 5;

            pm_Murderer.UniqueMurders = 0;
            pm_Murderer.PaladinCurrencyValue = 0;

            pm_Murderer.MurdererDeathGumpNeeded = false;
        }

        public static bool CheckAccountForMurderers(Mobile from)
        {
            bool hasMurderer = false;

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;

            Account acc = pm_From.Account as Account;

            for (int i = 0; i < (acc.Length - 1); i++)
            {
                Mobile m = acc.accountMobiles[i] as Mobile;

                if (m != null)
                {
                    if (m.ShortTermMurders >= 5)
                    {
                        hasMurderer = true;
                        break;
                    }
                }
            }

            return hasMurderer;
        }

        public static bool CheckAccountForPaladins(Mobile from)
        {
            bool hasPaladin = false;

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;

            Account acc = pm_From.Account as Account;

            for (int i = 0; i < (acc.Length - 1); i++)
            {
                Mobile m = acc.accountMobiles[i] as Mobile;

                if (m != null)
                {
                    if (((PlayerMobile)m).Paladin)
                    {
                        hasPaladin = true;
                        break;
                    }
                }
            }

            return hasPaladin;
        }

        public static bool RemoveAllPaladinsOnAccount(Mobile from, int days)
        {
            bool hasPaladin = false;

            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;

            Account acc = pm_From.Account as Account;

            for (int i = 0; i < (acc.Length - 1); i++)
            {
                Mobile mobile = acc.accountMobiles[i] as Mobile;

                if (mobile != null)
                {
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player != null)
                    {
                        bool message = true;

                        if (from == player)
                            message = false;

                        if (player.Paladin)
                        {
                            pm_From.PlaySound(0x5CE);                         
                            Effects.SendLocationParticles(EffectItem.Create(pm_From.Location, pm_From.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2500, 0, 5029, 0);

                            PaladinDismissal(mobile, days, message);
                            hasPaladin = true;
                        }
                    }
                }
            }

            return hasPaladin;
        }

        public static void PaladinDismissal(Mobile m, int days, bool message)
        {
            if (message)
                m.SendMessage(paladinDismissalTextHue, "Your actions are counterproductive towards the goal of The Order. You are no longer a Paladin of Trinsic.");

            RemovePaladinStatus(m, days);
        }

        public static void RemovePaladinStatus(Mobile m, int days)
        {
            PlayerMobile pm_From = m as PlayerMobile;

            if (pm_From == null)
                return;

            pm_From.Paladin = false;

            if (pm_From.PaladinRejoinAllowed < DateTime.UtcNow + TimeSpan.FromDays(days))
                pm_From.PaladinRejoinAllowed = DateTime.UtcNow + TimeSpan.FromDays(days);
            
            PlayerClassPersistance.RemoveTitles(pm_From, PlayerClassPersistance.PaladinTitles);
            PlayerClassPersistance.RemovePlayerClassEquipment(pm_From, PlayerClass.Paladin);
        }               

        public static void PaladinOnHit(int amount, Mobile from, PlayerMobile to, bool willKill)
        {
        }

        public static void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0
            writer.Write((int)m_DeathEventEntries.Count);

            foreach (DeathEventEntry entry in m_DeathEventEntries)
            {
                writer.Write(entry.m_Victim);
                writer.Write((int)entry.m_DeathEventType);

                writer.Write((int)entry.m_Killers.Count);
                foreach (Mobile killer in entry.m_Killers)
                {
                    writer.Write(killer);
                }

                writer.Write(entry.m_EventTime);
                writer.Write(entry.m_Location);
                writer.Write(entry.m_RandomLocation);
                writer.Write(entry.m_Map);
            }
        }

        public static void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            if (version >= 0)
            {
                m_DeathEventEntries = new List<DeathEventEntry>();

                int entriesCount = reader.ReadInt();
                for (int a = 0; a < entriesCount; ++a)
                {
                    Mobile m_Victim = reader.ReadMobile();
                    DeathEventType m_DeathEventType = (DeathEventType)reader.ReadInt();

                    List<Mobile> m_Killers = new List<Mobile>();

                    int killers = reader.ReadInt(); 
                    for (int b = 0; b < killers; ++b)
                    {
                        m_Killers.Add(reader.ReadMobile());
                    }

                    DateTime m_EventTime = reader.ReadDateTime();
                    Point3D m_Location = reader.ReadPoint3D();
                    Point3D m_RandomLocation = reader.ReadPoint3D();
                    Map m_Map = reader.ReadMap();

                    DeathEventEntry entry = new DeathEventEntry(m_Victim, m_DeathEventType, m_Killers, m_EventTime, m_Location, m_RandomLocation, m_Map);

                    m_DeathEventEntries.Add(entry);
                }
            }
        }
    }

    public enum DeathEventType
    {
        Player,
        Murderer,
        Paladin
    }

    public class DeathEventEntry
    {
        public Mobile m_Victim;
        public DeathEventType m_DeathEventType;
        public List<Mobile> m_Killers;
        public DateTime m_EventTime;
        public Point3D m_Location;
        public Point3D m_RandomLocation;
        public Map m_Map;

        public DeathEventEntry(Mobile victim, DeathEventType deathEventType, List<Mobile> killers, DateTime eventTime, Point3D location, Point3D randomLocation, Map map)
        {
            m_Victim = victim;
            m_DeathEventType = deathEventType;
            m_Killers = killers;
            m_EventTime = eventTime;
            m_Location = location;
            m_RandomLocation = randomLocation;

            m_Map = map;                    
        }
    }

    public class DeathEventPersistance : Item
    {
        public override string DefaultName { get { return "Death Event Persistance"; } }

        public DeathEventPersistance(): base(0x0)
        {
        }

        public DeathEventPersistance(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            PaladinEvents.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            PaladinEvents.PersistanceItem = this;
            PaladinEvents.Deserialize(reader);
        }
    }
}
