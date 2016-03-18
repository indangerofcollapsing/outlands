using Server;
using Server.Gumps;
using Server.Commands;
using Server.Mobiles;
using Server.Accounting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom.Paladin
{
    public class MurdererDeathGump : Gump
    {
        public PlayerMobile m_PaladinKilledBy;
        public PlayerMobile m_Murderer;
        public int m_PunishableCounts;
        public int m_BaseRestitutionFee;
        public DateTime m_PenanceExpiration;

        public double m_ReprieveDiscount;
        public double m_KillCountDiscount;
        public double m_PaladinDiscount;
        public double m_SkillLossSelectionDiscount;

        public int m_DiscountedRestitutionFee;

        public double skillLossAmount = 0;

        public int m_GoldPayment = 0;

        public int m_SkillLossButtonSelected = 1;
        
        public MurdererDeathGump(PlayerMobile murderer, int skillLossSelection): base(0, 0)
        {               
            if (murderer == null)
                return;

            m_Murderer = murderer;
            m_PaladinKilledBy = murderer.LastPlayerKilledBy;            
            m_PunishableCounts = murderer.ShortTermMurders - 5;
            m_BaseRestitutionFee = m_PunishableCounts * PaladinEvents.RestitutionFeeCostPerCount;            
            m_PenanceExpiration = murderer.PenanceExpiration;

            m_ReprieveDiscount = 0;
            m_KillCountDiscount = 0;
            m_PaladinDiscount = 0;
            m_SkillLossSelectionDiscount = 0;
            
            m_SkillLossButtonSelected = skillLossSelection;

            m_GoldPayment = 0;

            //Reprieve
            if (murderer.Citizenship != null && murderer.Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Reprieve))
                m_ReprieveDiscount = .10;

            string killStreakText = "Kill Streak (None): ";

            //Kill Counts
            if (m_PunishableCounts >= 25)
            {
                m_KillCountDiscount = .05;
                killStreakText = "Kill Streak (25+): ";
            }

            if (m_PunishableCounts >= 50)
            {
                m_KillCountDiscount = .10;
                killStreakText = "Kill Streak (50+): ";
            }

            if (m_PunishableCounts >= 75)
            {
                m_KillCountDiscount = .15;
                killStreakText = "Kill Streak (75+): ";
            }

            if (m_PunishableCounts >= 100)
            {
                m_KillCountDiscount = .20;
                killStreakText = "Kill Streak (100+): ";
            }

            if (m_PunishableCounts >= 150)
            {
                m_KillCountDiscount = .25;
                killStreakText = "Kill Streak (150+): ";
            }

            if (m_PunishableCounts >= 250)
            {
                m_KillCountDiscount = .30;
                killStreakText = "Kill Streak (250+): ";

            }

            //Paladins
            int paladinInstances = 0;
            int maxPaladinInstances = 1;

            if (m_PaladinKilledBy != null)
            {
                foreach (Mobile mobile in murderer.m_PaladinsKilled)
                {
                    if (mobile != null)
                    {
                        if (mobile == m_PaladinKilledBy && !murderer.CheckPlayerAccountsForCommonGuild(m_PaladinKilledBy))
                            paladinInstances++;
                    }
                }
            }

            if (paladinInstances > maxPaladinInstances)
                paladinInstances = maxPaladinInstances;

            m_PaladinDiscount = .20 * paladinInstances;

            //Skill Loss Selection
            switch (m_SkillLossButtonSelected)
            {
                case 1: m_SkillLossSelectionDiscount = 0; skillLossAmount = 0; break;
                case 2: m_SkillLossSelectionDiscount = .10; skillLossAmount = .05; break;
                case 3: m_SkillLossSelectionDiscount = .20; skillLossAmount = .10;  break;
                case 4: m_SkillLossSelectionDiscount = .40; skillLossAmount = .20; break;
            }
           
            double finalDiscount = m_ReprieveDiscount + m_KillCountDiscount + m_PaladinDiscount + m_SkillLossSelectionDiscount;

            if (finalDiscount > 1)
                finalDiscount = 1;

            if (finalDiscount < 0)
                finalDiscount = 0;

            m_DiscountedRestitutionFee = (int)((double)m_BaseRestitutionFee * (1 - finalDiscount));

            if (m_DiscountedRestitutionFee < 0)
                m_DiscountedRestitutionFee = 0;
                        
            this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

            int textHue = 2036;
            int goldHue = 149;

            AddPage(0);

            AddBackground(9, 7, 500, 400, 9200);

            AddLabel(115, 14, 2500, @"You have been slain by the paladin " + m_PaladinKilledBy.RawName + "!");

            AddLabel(24, 55, textHue, @"Punishable Murder Counts:");
            AddItem(190, 46, 4653); //Blood
            AddLabel(232, 55, textHue, m_PunishableCounts.ToString());

            AddLabel(304, 55, textHue, @"Base Fees:");
            AddItem(370, 51, 3823); //Gold
            AddLabel(414, 55, textHue, m_BaseRestitutionFee.ToString());

            AddLabel(74, 100, textHue, @"Fee Discounts");
            AddLabel(55, 125, textHue, "Town of Reprieve: " + (m_ReprieveDiscount * 100).ToString() + "%");
            AddLabel(55, 155, textHue, killStreakText + (m_KillCountDiscount * 100).ToString() + "%");
            AddLabel(55, 185, textHue, "Paladin Shame: "+ (m_PaladinDiscount * 100).ToString() + "%");
            AddLabel(55, 215, textHue, "Optional Skill Loss: " + (m_SkillLossSelectionDiscount * 100).ToString() + "%");          

            AddItem(11, 116, 2844); //Candle
            AddItem(6, 148, 7392); //Corpse
            AddItem(6, 179, 7108); //Shield
            AddItem(-1, 212, 3997); //Sewing Kit
            AddItem(21, 222, 4028); //Tongs
            AddItem(18, 228, 4031); //Scribes Pen

            AddLabel(61, 245, 2527, @"Total Discount: " + (finalDiscount * 100).ToString() + "%");    
            
            AddImage(222, 238, 2083); //Scroll
            AddImage(219, 168, 2081); //Scroll
            AddImage(218, 126, 2081); //Scroll
            AddImage(202, 93, 2080); //Scroll

            AddLabel(301, 101, textHue, @"Optional Skill Loss");
            AddLabel(326, 125, 2500, @"Skill Loss");
            AddLabel(289, 145, 2500, @"0%");
            AddLabel(325, 145, 2500, @"5%");
            AddLabel(363, 145, 2500, @"10%");
            AddLabel(404, 145, 2500, @"20%");

            AddItem(216, 133, 4214); //Dummy

            if (m_SkillLossButtonSelected == 1)
                AddButton(279, 167, 2154, 2152, 3, GumpButtonType.Reply, 0);
            else
                AddButton(279, 167, 2152, 2154, 3, GumpButtonType.Reply, 0);

            if (m_SkillLossButtonSelected == 2)
                AddButton(319, 167, 2154, 2152, 4, GumpButtonType.Reply, 0);
            else
                AddButton(319, 167, 2152, 2154, 4, GumpButtonType.Reply, 0);

            if (m_SkillLossButtonSelected == 3)
                AddButton(358, 167, 2154, 2152, 5, GumpButtonType.Reply, 0);
            else
                AddButton(358, 167, 2152, 2154, 5, GumpButtonType.Reply, 0);

            if (m_SkillLossButtonSelected == 4)
                AddButton(398, 167, 2154, 2152, 6, GumpButtonType.Reply, 0);
            else
                AddButton(398, 167, 2152, 2154, 6, GumpButtonType.Reply, 0);

            AddLabel(288, 200, 2527, @"0%");
            AddLabel(322, 200, 2527, @"10%");
            AddLabel(363, 200, 2527, @"20%");
            AddLabel(404, 200, 2527, @"40%");
            AddLabel(318, 220, 2527, @"Fee Discount");

            AddLabel(40, 285, 149, @"Adjusted Restitution Fees");
            AddItem(75, 315, 3823); //Gold
            AddLabel(122, 318, 149, m_DiscountedRestitutionFee.ToString());

            AddLabel(39, 345, textHue, @"Make Immediate Payment of:");
           
            AddImage(25, 374, 2501); //Text Bar
            AddTextEntry(33, 374, 127, 20, textHue, 2, m_GoldPayment.ToString()); //Text Entry
            AddButton(167, 372, 247, 248, 1, GumpButtonType.Reply, 0); //Okay

            AddLabel(244, 285, textHue, @"Restitution Fees must be paid");
            AddLabel(244, 305, textHue, @"for this character before it can be");
            AddLabel(244, 325, textHue, @"ressed. Any player may make a payment");
            AddLabel(244, 345, textHue, @"towards this character's restitution");
            AddLabel(244, 365, textHue, @"at a Restitution Board at any town bank.");

            AddItem(468, 254, 6660, 139); //Skeleton
        }       

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            PlayerMobile player = sender.Mobile as PlayerMobile;

            if (player == null) return;
            if (player.Deleted) return;

            bool reloadGump = true;
           
            DateTime longestPenance = DateTime.UtcNow;

            Account account = player.Account as Account;

            if (account == null) return;
            if (account.accountMobiles == null) return;

            for (int a = 0; a < account.accountMobiles.Length; a++)
            {
                PlayerMobile pm_Mobile = account.accountMobiles[a] as PlayerMobile;

                if (pm_Mobile == null)
                    continue;
                
                if (pm_Mobile.PenanceExpiration > DateTime.UtcNow && pm_Mobile.PenanceExpiration > longestPenance)
                    longestPenance = pm_Mobile.PenanceExpiration;

                break;                             
            }

            string timeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, longestPenance, false, true, true, true, false);

            switch(info.ButtonID)
            {
                case 1:
				{
                    TextRelay textRelayGoldPayment = info.GetTextEntry(2);
                    string textGold = textRelayGoldPayment.Text.Trim();

                    int goldPayment = 0;

                    try { goldPayment = Convert.ToInt32(textGold); }
                    catch (Exception e) { goldPayment = -1; }

                    if (goldPayment < 0)
                        player.SendMessage("That is not a valid amount of gold for a payment.");                    

                    else if (goldPayment > m_DiscountedRestitutionFee)
                        player.SendMessage("That payment is more than the total restitution fee required for your character.");                    

                    else
                    {
                        bool hasEnoughGold = Banker.GetBalance(player) >= goldPayment;

                        if (hasEnoughGold)
                        {
                            //Paying Full Amount
                            if (goldPayment >= m_DiscountedRestitutionFee)
                            {
                                if (goldPayment > 0)
                                    Banker.Withdraw(player, m_DiscountedRestitutionFee);

                                player.RestitutionFee = 0;
                                player.RestitutionFeesToDistribute = m_DiscountedRestitutionFee;

                                reloadGump = false;

                                player.SendMessage("Your Restitution Fee has been paid in full! You may now be freely resurrected.");
                                
                                if (longestPenance > DateTime.UtcNow)
                                    player.SendMessage("Your account is under the risk of temporary statloss for another " + timeRemaining + " if you enter any dungeon or contested area.");
                                
                                if (skillLossAmount > 0)
                                    player.ApplyPermanentSkillLoss(skillLossAmount);

                                player.MurdererDeathGumpNeeded = false;

                                //Disburse Restitution Fees to Victims
                                PaladinEvents.DisburseRestitutionFees(player);
                            }

                            //Partial Payment
                            else
                            {
                                Banker.Withdraw(player, goldPayment);

                                player.RestitutionFeesToDistribute = m_DiscountedRestitutionFee;
                                player.RestitutionFee = m_DiscountedRestitutionFee - goldPayment;

                                RestitutionEntry entry = new RestitutionEntry(player, player.RestitutionFee);
                                Restitution.UpdatePublicRestitutionEntry(entry);

                                player.SendMessage("You make a payment of " + goldPayment.ToString() + " gold towards your Restitution Fee. You will need to have the remainder paid off before you may be freely resurrectable.");
                                                               
                                if (longestPenance > DateTime.UtcNow)
                                    player.SendMessage("Your account is under the risk of temporary statloss for another " + timeRemaining + " if you enter any dungeon or contested area.");
                                
                                if (skillLossAmount > 0)
                                    player.ApplyPermanentSkillLoss(skillLossAmount);

                                reloadGump = false;
                                player.MurdererDeathGumpNeeded = false;
                            }
                        }

                        else
                            player.SendMessage("You do not have that much gold in your bank.");
                    }
				}

                break;

                case 3:
                {
                    m_SkillLossButtonSelected = 1;                  
                }
                break;

                case 4:
                {
                    m_SkillLossButtonSelected = 2;
                }
                break;

                case 5:
                {
                    m_SkillLossButtonSelected = 3;
                }
                break;

                case 6:
                {
                    m_SkillLossButtonSelected = 4;
                }
                break;
            }

            if (reloadGump)
            {
                player.CloseAllGumps();
                player.SendGump(new Custom.Paladin.MurdererDeathGump(player, m_SkillLossButtonSelected));  
            }
        }        
    }
}