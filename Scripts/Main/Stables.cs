using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Custom;

namespace Server
{
    public class Stables
    {
        public static double GoldCostPerControlSlotPerDay = 10;
        public static double ResFeeMinTamingSkillDivisor = 50;

        public static int GetUsedStableSlots(PlayerMobile player)
        {
            int stableSlotsUsed = 0;

            for (int a = 0; a < player.Stabled.Count; a++)
            {
                BaseCreature creature = player.Stabled[a] as BaseCreature;

                if (creature == null)
                    continue;

                stableSlotsUsed += creature.ControlSlots;
            }

            return stableSlotsUsed;
        }

        public static int GetMaxStableSlots(PlayerMobile player)
        {
            if (player == null)
                return 0;

            int stableSlots = 10;

            //Animal Taming
            stableSlots += (int)(Math.Floor(player.Skills[SkillName.AnimalTaming].Value / 20));

            if (player.Skills[SkillName.AnimalTaming].Value == 120)
                stableSlots += 4;

            //Animal Lore
            stableSlots += (int)(Math.Floor(player.Skills[SkillName.AnimalLore].Value / 20));

            if (player.Skills[SkillName.AnimalLore].Value == 120)
                stableSlots += 4;

            if (PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.Herdsman))
                stableSlots += 5;

            if (PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.Rancher))
                stableSlots += 5;

            return stableSlots;
        }

        public static int GetClaimCost(Mobile vendor, PlayerMobile player, BaseCreature creature)
        {
            double cost = 0;

            TimeSpan timeStabled = DateTime.UtcNow - creature.TimeStabled;

            int daysStabled = (int)(Math.Ceiling((DateTime.UtcNow - creature.TimeStabled).TotalDays));

            if (daysStabled <= 0)
                daysStabled = 1;

            cost += ((double)daysStabled * GoldCostPerControlSlotPerDay * (double)creature.ControlSlots);

            if (!creature.Alive || creature.IsDeadBondedPet)
            {
                double resFee = ((creature.MinTameSkill * creature.MinTameSkill) / ResFeeMinTamingSkillDivisor) * (double)creature.ControlSlots;

                cost += resFee;
            }

            int finalCost = (int)(Math.Round(cost));

            return finalCost;
        }
        
        public static void StableNewFollower(Mobile vendor, PlayerMobile player, int page)
        {
            if (player == null || vendor == null)
                return;

            if (!player.Alive)
            {
                player.SendMessage("You must be alive to do that.");

                player.CloseGump(typeof(StableGump));
                player.SendGump(new StableGump(vendor, player, page));
                
                return;
            }

            if (player.AllFollowers.Count == 0)
            {
                player.SendMessage("You do not have any followers currently.");

                player.CloseGump(typeof(StableGump));
                player.SendGump(new StableGump(vendor, player, page));

                return;
            }

            player.SendMessage("Which follower do you wish to stable?");
            player.Target = new StableFollowerTarget(vendor, player, page);
        }

        private class StableFollowerTarget : Target
        {
            public Mobile m_Vendor;
            public PlayerMobile m_Player;

            public int m_Page;

            public StableFollowerTarget(Mobile vendor, PlayerMobile player, int page): base(100, false, TargetFlags.None)
            {
                m_Vendor = vendor;
                m_Player = player;

                m_Page = page;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Player == null)
                    return;

                if (m_Vendor == null)
                    return;                

                if (m_Vendor.Deleted || !m_Vendor.Alive)
                    return;

                if (!m_Player.Alive)
                {
                    m_Player.SendMessage("You must be alive to do that.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (Utility.GetDistance(m_Vendor.Location, m_Player.Location) >= 15)
                {
                    m_Player.SendMessage("You are too far away to continue that.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (!(target is BaseCreature))
                {
                    m_Player.SendMessage("That is not a valid follower.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                BaseCreature bc_Target = target as BaseCreature;

                if (Utility.GetDistance(m_Player.Location, bc_Target.Location) >= 8)
                {
                    m_Player.SendMessage("That follower is too far away.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (!m_Player.InLOS(bc_Target) || bc_Target.Hidden)
                {
                    m_Player.SendMessage("That follower cannot be seen.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (bc_Target.Summoned)
                {
                    m_Player.SendMessage("That type of follower cannot be stabled.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (!(bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile))
                {
                    m_Player.SendMessage("That follower does not belong to you.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (bc_Target.RecentlyInCombat)
                {
                    m_Player.SendMessage("That follower has been in combat too recently to be stabled.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }
                
                if (bc_Target.RecentlyInPlayerCombat)
                {
                    m_Player.SendMessage("You have been in combat with another player too recently for that follower to be stabled.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                if (bc_Target.Backpack is StrongBackpack)
                {
                    if (bc_Target.Backpack.Items.Count > 0)
                    {
                        m_Player.SendMessage("You must clear that follower's backpack of items before it may be stabled.");

                        m_Player.CloseGump(typeof(StableGump));
                        m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                        return;
                    }
                }

                if (bc_Target.ControlSlots + Stables.GetUsedStableSlots(m_Player) > Stables.GetMaxStableSlots(m_Player))
                {
                    m_Player.SendMessage("You have too many followers stabled already.");

                    m_Player.CloseGump(typeof(StableGump));
                    m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                    return;
                }

                StableFollower(m_Vendor, m_Player, bc_Target, m_Page);
            }
        }

        public static void StableFollower(Mobile vendor, PlayerMobile player, BaseCreature creature, int page)
        {
            if (player == null || creature == null) return;
            if (creature.Deleted) return;

            creature.ControlTarget = null;
            creature.ControlOrder = OrderType.Stay;
            creature.Internalize();

            creature.SetControlMaster(null);

            creature.IsStabled = true;
            creature.TimeStabled = DateTime.UtcNow;

            creature.OwnerAbandonTime = DateTime.UtcNow + TimeSpan.FromDays(1000);

            player.Stabled.Add(creature);
            
            player.CloseGump(typeof(StableGump));
            player.SendGump(new StableGump(vendor, player, page));            
        }

        public static void ClaimFollower(Mobile vendor, PlayerMobile player, BaseCreature creature, int page)
        {
            if (player == null || creature == null)
                return;

            if (player.Stabled.Contains(creature))
                player.Stabled.Remove(creature);

            creature.SetControlMaster(player);

            creature.ControlTarget = player;
            creature.ControlOrder = OrderType.Follow;

            creature.OwnerAbandonTime = DateTime.UtcNow + creature.AbandonDelay;

            creature.MoveToWorld(player.Location, player.Map);

            creature.AnimateIdle();
            creature.PlaySound(creature.GetIdleSound());

            creature.IsStabled = false;

            if (!creature.Alive || creature.IsDeadBondedPet)
                creature.ResurrectPet();

            creature.ApplyExperience();

            creature.Hits = creature.HitsMax;
            creature.Stam = creature.StamMax;
            creature.Mana = creature.ManaMax;
           
            player.CloseGump(typeof(StableGump));
            player.SendGump(new StableGump(vendor, player, page));

            player.SendMessage("You claim your follower.");
        }
    }

    public class StableGump : Gump
    {
        public Mobile m_Vendor;
        public PlayerMobile m_Player;

        public int FollowersPerPage = 7;
        public int m_Page;

        public int openGumpSound = 0x055;
        public int changeGumpSound = 0x057;
        public int closeGumpSound = 0x058;

        public StableGump(Mobile vendor, PlayerMobile player, int page): base(10, 10)
        {
            if (vendor == null) return;
            if (player == null) return;

            m_Vendor = vendor;
            m_Player = player;

            m_Page = page;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            #region Images

            AddPage(0);

            AddImage(3, 369, 103);
            AddImage(2, 262, 103);
            AddImage(2, 3, 103);
            AddImage(131, 3, 103);
            AddImage(2, 97, 103);
            AddImage(134, 97, 103);
            AddImage(264, 288, 103);
            AddImage(133, 368, 103);
            AddImage(265, 3, 103);
            AddImage(265, 98, 103);
            AddImage(265, 193, 103);
            AddImage(2, 194, 103);
            AddImage(265, 368, 103);
            AddImage(394, 288, 103);
            AddImage(395, 3, 103);
            AddImage(395, 98, 103);
            AddImage(395, 193, 103);
            AddImage(395, 368, 103);
            AddImage(527, 288, 103);
            AddImage(528, 3, 103);
            AddImage(528, 98, 103);
            AddImage(528, 193, 103);
            AddImage(528, 368, 103);

            AddBackground(11, 106, 647, 352, 3000);
            AddBackground(11, 14, 647, 84, 3000);

            AddItem(133, 10, 3892);
            AddItem(142, 43, 3892);
            AddItem(142, 43, 3892);
            AddItem(113, 49, 3892);
            AddItem(101, 15, 3892);
            AddItem(71, 47, 3892);
            AddItem(71, 47, 3892);
            AddItem(96, 29, 3892);
            AddItem(177, 60, 3899);
            AddItem(114, 12, 3896);
            AddItem(177, 19, 3894);
            AddItem(120, 15, 3898);
            AddItem(100, 84, 3899);
            AddItem(77, 32, 3894);
            AddItem(139, 28, 5368);
            AddItem(580, 30, 2604);
            AddItem(586, 29, 4030);
            AddItem(599, 36, 4031);
            AddItem(479, 48, 2602);
            AddItem(550, 15, 2651);
            AddItem(534, 42, 2650);
            AddItem(537, 35, 2707, 2118);
            AddItem(496, 17, 2868);
            AddItem(499, 21, 2479);
            AddItem(485, 24, 2542);
            AddItem(493, 28, 2575);

            #endregion

            int usedStableSlots = Stables.GetUsedStableSlots(m_Player);
            int maxStableSlots = Stables.GetMaxStableSlots(m_Player);

            int totalFollowers = m_Player.Stabled.Count;
            int totalPages = (int)(Math.Ceiling((double)totalFollowers / (double)FollowersPerPage));

            if (m_Page >= totalPages)
                m_Page = 0;

            if (m_Page < 0)
                m_Page = 0;

            int creatureStartIndex = m_Page * FollowersPerPage;
            int creatureEndIndex = (m_Page * FollowersPerPage) + (FollowersPerPage - 1);

            if (creatureEndIndex >= totalFollowers)
                creatureEndIndex = totalFollowers - 1;

            int WhiteTextHue = 2655; //2036

            AddLabel(24, 112, 2599, "Claim");
            AddLabel(75, 112, 2599, "Slots");
            AddLabel(126, 112, 2599, "Claim Cost");
            AddLabel(310, 112, 2599, "Follower");
            AddLabel(448, 112, 2599, "Status");
            AddLabel(505, 112, 2599, "Level");
            AddLabel(560, 112, 2599, "Exp");
            AddLabel(602, 112, 2599, "Dismiss");

            //Guide
            AddButton(11, 4, 2094, 2095, 1, GumpButtonType.Reply, 0);
            AddLabel(38, 13, 149, "Guide");

            AddLabel(304, 27, 2201, "Claim Followers");
            AddLabel(282, 62, 2610, "Stable Slots Used:");
            AddLabel(402, 62, WhiteTextHue, usedStableSlots.ToString() + " / " + maxStableSlots.ToString());

            //Loop

            int startY = 142;
            int itemSpacing = 40;

            int creatureCount = creatureEndIndex - creatureStartIndex;

            for (int a = 0; a < creatureCount + 1; a++)
            {
                int creatureIndex = creatureStartIndex + a;
                int creatureButtonIndex = (10 * (creatureIndex + 1));

                if (creatureIndex >= m_Player.Stabled.Count)
                    continue;

                BaseCreature bc_Creature = m_Player.Stabled[creatureIndex] as BaseCreature;

                if (bc_Creature == null)
                    continue;

                string controlSlots = bc_Creature.ControlSlots.ToString();
                string claimCost = Stables.GetClaimCost(m_Vendor, m_Player, bc_Creature).ToString();
                string followerName = bc_Creature.GetTamedDisplayName();

                string followerStatus = "Alive";
                int aliveTextHue = WhiteTextHue;
                int claimCostTextHue = WhiteTextHue;

                if (!bc_Creature.Alive || bc_Creature.IsDeadBondedPet)
                {
                    followerStatus = "Dead";
                    aliveTextHue = 2201;
                    claimCostTextHue = 2201;
                }

                string experienceLevel = bc_Creature.ExperienceLevel.ToString();
                string experience = bc_Creature.Experience.ToString();

                AddButton(24, startY - 5, 2151, 2152, creatureButtonIndex, GumpButtonType.Reply, 0); //Claim Button
                AddLabel(Utility.CenteredTextOffset(90, controlSlots), startY, WhiteTextHue, controlSlots); //Slots
                AddItem(117, startY - 5, 3823); //Gold Image
                AddLabel(160, startY, claimCostTextHue, claimCost.ToString()); //Claim Cost
                AddLabel(Utility.CenteredTextOffset(335, followerName), startY, 2603, followerName); //Follower Name
                AddLabel(453, startY, aliveTextHue, followerStatus);
                AddLabel(Utility.CenteredTextOffset(520, experienceLevel), startY, WhiteTextHue, experienceLevel);
                AddLabel(Utility.CenteredTextOffset(568, experience), startY, WhiteTextHue, experience);
                AddButton(611, startY - 5, 2472, 2473, creatureButtonIndex + 1, GumpButtonType.Reply, 0);

                startY += itemSpacing;
            }

            //Previous Page
            if (m_Page > 0)               
            {
                AddButton(24, 408, 4014, 4016, 2, GumpButtonType.Reply, 0);
                AddLabel(61, 409, WhiteTextHue, "Previous Page");
            }

            //Stable New Folloer
            AddButton(276, 408, 4002, 4004, 3, GumpButtonType.Reply, 0);
            AddLabel(313, 409, 0x3F, "Stable New Follower");

            //Next Page
            if (m_Page < totalPages - 1)
            {
                AddLabel(547, 408, WhiteTextHue, "Next Page");
                AddButton(616, 409, 4005, 4007, 4, GumpButtonType.Reply, 0);
            }

            string claimMessage = "Claim Cost: 10 Gold Per Control Slot Each Day (paid when claimed)";

            AddLabel(Utility.CenteredTextOffset(350, claimMessage), 433, 2550, claimMessage);
        }
        
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)  return;
            if (m_Vendor == null)  return;

            if (m_Vendor.Deleted || !m_Vendor.Alive)
                return;

            if (!m_Player.Alive)
            {
                m_Player.SendMessage("You must be alive to do that.");
                return;
            }

            if (Utility.GetDistance(m_Vendor.Location, m_Player.Location) >= 15)
            {
                m_Player.SendMessage("You are too far away to continue that.");

                m_Player.CloseGump(typeof(StableGump));
                m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                return;
            }

            bool closeGump = true;

            int usedStableSlots = Stables.GetUsedStableSlots(m_Player);
            int maxStableSlots = Stables.GetMaxStableSlots(m_Player);

            int totalFollowers = m_Player.Stabled.Count;
            int totalPages = (int)(Math.Ceiling((double)totalFollowers / (double)FollowersPerPage));

            if (m_Page >= totalPages)
                m_Page = 0;

            if (m_Page < 0)
                m_Page = 0;

            int creatureStartIndex = m_Page * FollowersPerPage;
            int creatureEndIndex = (m_Page * FollowersPerPage) + (FollowersPerPage - 1);

            if (creatureEndIndex >= totalFollowers)
                creatureEndIndex = totalFollowers - 1;

            switch (info.ButtonID)
            {
                //Guide
                case 1:
                    closeGump = false;
                break;

                //Previous Page
                case 2:
                    if (m_Page > 0)
                    {
                        m_Page--;
                        m_Player.SendSound(changeGumpSound);
                    }

                    closeGump = false;
                break;

                //Stable New Follower
                case 3:
                    Stables.StableNewFollower(m_Vendor, m_Player, m_Page);
                break;

                //Next Page
                case 4:
                    if (m_Page < totalPages - 1)
                    {
                        m_Page++;
                        m_Player.SendSound(changeGumpSound);
                    }

                    closeGump = false;
                break;
            }

            //Follower
            if (info.ButtonID >= 10)
            {
                int creatureIndex = (int)(Math.Floor((double)info.ButtonID / 10)) - 1;
                int buttonPressed = info.ButtonID % 10;

                if (creatureIndex < m_Player.Stabled.Count)
                {
                    BaseCreature bc_Creature = m_Player.Stabled[creatureIndex] as BaseCreature;

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.IsStabled)
                        {
                            switch (buttonPressed)
                            {
                                //Claim Follower
                                case 0:
                                    if (m_Player.RecentlyInCombat)                                    
                                        m_Player.SendMessage("You have been in combat too recently to claim a follower.");

                                    else if (m_Player.RecentlyInPlayerCombat)
                                        m_Player.SendMessage("You have been in combat with another player too recently claim a follower.");

                                    else if (bc_Creature.ControlSlots + m_Player.Followers > m_Player.FollowersMax)                                    
                                        m_Player.SendMessage("You do not have enough control slots available to claim that follower.");                                    

                                    else
                                    {
                                        int goldCost = Stables.GetClaimCost(m_Vendor, m_Player, bc_Creature);

                                        if (Banker.GetBalance(m_Player) < goldCost)
                                            m_Player.SendMessage("You do not have enough gold in your bank to claim that follower.");

                                        else
                                        {
                                            Banker.Withdraw(m_Player, goldCost);
                                            Stables.ClaimFollower(m_Vendor, m_Player, bc_Creature, m_Page);

                                            m_Player.SendSound(0x2E6);
                                        }
                                    }
                                break;

                                //Dismiss Follower
                                case 1:                                   

                                    m_Player.CloseGump(typeof(StableDismissFollowerGump));
                                    m_Player.SendGump(new StableDismissFollowerGump(m_Vendor, m_Player, bc_Creature, m_Page));

                                    m_Player.SendSound(openGumpSound);

                                    return;
                                break;
                            }
                        }

                        else
                        {
                            //If Somehow Still in Stabled List but Not Set as Stabled
                            if (m_Player.Stabled.Contains(bc_Creature))
                                m_Player.Stabled.Remove(bc_Creature);
                        }
                    }
                }

                closeGump = false;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(StableGump));
                m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));
            }
        }
    }

    public class StableDismissFollowerGump : Gump
    {
        public Mobile m_Vendor;
        public PlayerMobile m_Player;
        public BaseCreature m_Creature;

        public int m_Page;

        public StableDismissFollowerGump(Mobile vendor, PlayerMobile player, BaseCreature creature, int page): base(200, 200)
        {
            if (vendor == null) return;
            if (player == null) return;
            if (creature == null) return;

            m_Vendor = vendor;
            m_Player = player;
            m_Creature = creature;

            m_Page = page;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int WhiteTextHue = 2655; //2036

            AddImage(10, 10, 103);
			AddImage(125, 10, 103);
			AddImage(10, 100, 103);
			AddImage(125, 100, 103);
			AddImage(20, 61, 200);
			AddImage(126, 61, 200);
			AddImage(20, 20, 200);
			AddImage(126, 20, 200);

            AddLabel(110, 25, 34, "Warning");
			AddLabel(87, 55, 149, "Dismiss Follower");					
			AddLabel(39, 85, WhiteTextHue, "This will permanently delete the");
			AddLabel(46, 105, WhiteTextHue, "follower and cannot be undone.");
            AddLabel(107, 161, 2208, "Proceed?");

            AddButton(31, 161, 247, 248, 1, GumpButtonType.Reply, 0);
            AddButton(179, 160, 242, 248, 2, GumpButtonType.Reply, 0);			
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
 	        if (m_Player == null)  return;
            if (m_Vendor == null)  return;
            if (m_Creature == null) return;

            if (m_Vendor.Deleted || !m_Vendor.Alive)
                return;

            if (m_Creature.Deleted) 
                return;

            if (!m_Player.Alive)
            {
                m_Player.SendMessage("You must be alive to do that.");

                m_Player.CloseGump(typeof(StableGump));
                m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                return;
            }

            if (Utility.GetDistance(m_Vendor.Location, m_Player.Location) >= 15)
            {
                m_Player.SendMessage("You are too far away to continue that.");

                m_Player.CloseGump(typeof(StableGump));
                m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));

                return;
            }

            if (info.ButtonID == 1)
            {
                if (m_Player.Stabled.Contains(m_Creature))
                    m_Player.Stabled.Remove(m_Creature);

                m_Player.SendSound(m_Creature.GetDeathSound());

                m_Creature.Delete();

                m_Player.SendMessage("You permanently dismiss the follower.");
            }

            m_Player.CloseGump(typeof(StableGump));
            m_Player.SendGump(new StableGump(m_Vendor, m_Player, m_Page));
        }
    }
}
