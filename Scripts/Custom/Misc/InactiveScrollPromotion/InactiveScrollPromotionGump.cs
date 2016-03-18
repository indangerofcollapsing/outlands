using System;
using Server.Accounting;
using Server.Mobiles;
using Server.Items;
using System.Collections;
using Server.Custom;

namespace Server.Gumps
{
    public class InactiveScrollPromotionGump : Gump
    {
        public InactiveScrollPromotionGump(Mobile from)
            : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddBackground(0, 0, 750, 600, 9200);

            Account acct = from.Account as Account;

            string html = "Error";

            if (acct == null)
            {
                this.AddHtml(10, 10, 730, 100, html, (bool)true, (bool)true);

                return;
            }

            html = @"Greetings, friend! You’ve returned during our current skill scroll promotion for inactive accounts!  Perhaps you’d like to power some of your characters up?<br><br>Choose three skills to receive scrolls for:<br><br>You may cancel this menu and be prompted for it again at your next login.";

            this.AddHtml(10, 10, 730, 100, html, (bool)true, (bool)true);
                                     
            Mobile m_Character;

            int x, y;
                       
            //Determine Which Characters on Account Are Valid
            ArrayList aValidCharacters = new ArrayList();
                        
            for (int a = 0; a < acct.accountMobiles.Length; a++)
            {
                m_Character = acct.accountMobiles[a];  

                if (m_Character != null)
                {
                    aValidCharacters.Add(m_Character);
                }
            }           

            //Create Page For Each Valid Character        
            for (int a = 0; a < aValidCharacters.Count; a++)
            {
                m_Character = aValidCharacters[a] as Mobile;                

                //Valid Character
                if (m_Character != null)
                {
                    this.AddPage(a + 1);

                    for (int b = 0; b < 49; b++)
                    {
                        x = 10 + 225 * ((int)(b / 17));
                        y = 122 + (b % 17) * 20;

                        this.AddCheck(x, y, 210, 211, false, (a * 49) + b);
                        this.AddLabel(x + 22, y, 0, m_Character.Skills[b].Name + " (" + m_Character.Skills[b].Base.ToString() + ")");
                    }

                    //Character Label
                    this.AddLabel(300, 475, 0, @"Account:");
                    this.AddLabel(375, 475, 0, acct.Username);

                    this.AddLabel(300, 490, 0, @"Character:");
                    this.AddLabel(375, 490, 0, m_Character.RawName);

                    //Add Previous Character Button                
                    if (a > 0)
                    {
                        this.AddButton(310, 510, 4506, 4506, 0, GumpButtonType.Page, a);
                    }

                    //Add Next Character Button
                    if (a < (aValidCharacters.Count - 1))
                    {
                        this.AddButton(370, 510, 4502, 4502, 0, GumpButtonType.Page, a + 2);
                    }

                    //Cancel Button
                    this.AddButton(600, 560, 241, 242, 0, GumpButtonType.Reply, 0);

                    //Apply Button
                    this.AddButton(675, 560, 238, 239, 1, GumpButtonType.Reply, 0);                   
                }
            }        
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {           
            Account acct = sender.Account as Account;
            PlayerMobile pm_Sender = sender.Mobile as PlayerMobile;
            Mobile m_Character;

            //Determine Which Characters on Account Are Valid
            ArrayList aValidCharacters = new ArrayList();

            for (int a = 0; a < acct.accountMobiles.Length; a++)
            {
                m_Character = acct.accountMobiles[a];

                if (m_Character != null)
                {
                    aValidCharacters.Add(m_Character);
                }
            } 
            
            //Result (Canel = 0 / Apply = 1)
            if (info.ButtonID == 0)
            {
                return;
            }           

            if (pm_Sender == null || pm_Sender.LoginElapsedTime < InactiveScrollPromotion.MinimumDelay)
            {
                return;
            }          

            if (info.Switches.Length < 1 || info.Switches.Length > 3)
            {
                pm_Sender.SendMessage("You must select at least one skill and no more than three skills.");
                pm_Sender.SendGump(new InactiveScrollPromotionGump(pm_Sender));

                return;
            }           

            PlayerMobile pm_Character;
            ArrayList aSkills = new ArrayList();

            int iWeeks = (int)((pm_Sender.LoginElapsedTime - InactiveScrollPromotion.MinimumDelay).TotalDays / 7);
              
            for (int a = 0; a < info.Switches.Length; a++)
            {   
                int iCharacter = (int)(Math.Floor((double)info.Switches[a] / 49));                    
                int iSkill = info.Switches[a] % 49;               

                pm_Character = aValidCharacters[iCharacter] as PlayerMobile;                       

                if (pm_Character == null)
                {                    
                    return;
                }                 

                Skill skill = pm_Character.Skills[iSkill];               

                if (skill != null)
                {
                    int iScrolls = 0;

                    int iSkillValue = skill.BaseFixedPoint;                    

                    if (iSkillValue < 500)
                    {
                        iScrolls = 8 + (iWeeks * 4);
                    }

                    else if (iSkillValue < 600)
                    {
                        iScrolls = 7 + (iWeeks * 4);
                    }

                    else if (iSkillValue < 800)
                    {
                        iScrolls = 6 + (iWeeks * 3);
                    }

                    else if (iSkillValue < 900)
                    {
                        iScrolls = 5 + (iWeeks * 3);
                    }

                    else if (iSkillValue < 950)
                    {
                        iScrolls = 4 + (iWeeks * 2);
                    }

                    else if (iSkillValue < 990)
                    {
                        iScrolls = 3 + (iWeeks * 2);
                    }

                    else
                    {
                        iScrolls = 2 + (iWeeks * 2);
                    }

                    //Normalize Previous Distribution Amounts for Individual Scrolls
                    iScrolls = iScrolls / 3;
                    
                    //Max Scrolls Allowed for a Skill
                    if (iScrolls > 30)
                    {
                        iScrolls = 30;
                    }

                    //Add Scrolls to Character's Backpack
                    for (int b = 0; b < iScrolls; b++)
                    {
                        SkillScroll scroll = SkillScroll.Generate(pm_Character, 100.0, GetSkillScrollRarity(), iSkill);

                        if (scroll != null)
                        {
                            scroll.MaxCapOffset = 1;

                            pm_Character.Backpack.AddItem(scroll);
                        }
                    }
                }              
            }                       

            InactiveScrollPromotion.LogUse(pm_Sender);

            pm_Sender.LoginElapsedTime = TimeSpan.Zero;
            pm_Sender.SendMessage("Thanks for returning! Your skill scrolls have been placed in your selected character's backpacks.");
        }

        public static int GetSkillScrollRarity()
        {
            var rand = Utility.RandomDouble();

            if (rand < 0.1)
            {
                return 0;
            }

            else if (rand < 0.25)
            {
                return 1;
            }

            else
            {
                return 2;
            }
        }
    }
}