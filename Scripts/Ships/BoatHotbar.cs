using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Multis.Deeds;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public class BoatHotbarGump : Gump
    {
        public enum MovementMode
        {
            Single,
            Full
        }

        public enum ShipAction
        {
            AddCoOwner,
            AddFriend,
            ClearDeck,       
            Disembark,
            DisembarkFollowers,
            DividePlunder,
            Dock,
            Embark,
            EmbarkFollowers,
            LowerAnchor,              
            RaiseAnchor,
            ThrowTargetOverboard
        }

        public class ShipPlayerControlSettings
        {
            public bool m_CollapseMode = false;

            //public ActiveAbilityType m_ActiveAbility = ActiveAbilityType.None;
            //public EpicAbilityType m_EpicAbility = EpicAbilityType.None;

            public MovementMode m_MovementMode = MovementMode.Full;
            public ShipAction m_ShipAction = ShipAction.Embark;

            public ShipPlayerControlSettings()
            {
            }
        }

        public PlayerMobile m_Player;        

        public BoatHotbarGump(PlayerMobile player): base(10, 10)
        {
            if (player == null) 
                return;

            m_Player = player;

            if (m_Player.m_ShipControlSettings == null)
                m_Player.m_ShipControlSettings = new ShipPlayerControlSettings();

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            int textHue = 2036;

            //-----
            
            BaseBoat m_Boat = m_Player.BoatOccupied;

            if (m_Boat != null)
            {
                if (m_Boat.Deleted || m_Boat.m_SinkTimer != null)
                    m_Boat = null;
            }

            if (m_Player.m_ShipControlSettings.m_CollapseMode)
            {
                //Gump Controls
                AddAlphaRegion(0, 0, 30, 85);
                AddButton(3, 0, 9906, 9906, 27, GumpButtonType.Reply, 0); //Collapse
                AddItem(-8, 29, 5363);
                AddButton(6, 62, 1210, 1209, 28, GumpButtonType.Reply, 0); //Ship Selection
            }

            else
            {
                int hullPoints = -1;
                int maxHullPoints = -1;
                double hullPercent = 0;

                int sailPoints = -1;
                int maxSailPoints = -1;
                double sailPercent = 0;

                int gunPoints = -1;
                int maxGunPoints = -1;
                double gunPercent = 0;

                int leftCannons = -1;
                bool leftCannonsReady = false;

                int rightCannons = -1;
                bool rightCannonsReady = false;

                string targetingMode = "-";
                string movementMode = "Full";

                switch (m_Player.m_ShipControlSettings.m_MovementMode)
                {
                    case MovementMode.Single: movementMode = "Single"; break;
                    case MovementMode.Full: movementMode = "Full"; break;
                }

                string minorAbilityText1 = "";
                string minorAbilityText2 = "";
                string minorAbilityTimeRemaining = "";
                bool minorAbilityReady = false;

                string epicAbilityText1 = "";
                string epicAbilityText2 = "";
                string epicAbilityTimeRemaining = "";
                bool epicAbilityReady = false;

                if (m_Boat != null)
                {
                    hullPoints = m_Boat.HitPoints;
                    maxHullPoints = m_Boat.MaxHitPoints;
                    hullPercent = (double)hullPoints / (double)maxHullPoints;

                    sailPoints = m_Boat.SailPoints;
                    maxSailPoints = m_Boat.MaxSailPoints;
                    sailPercent = (double)sailPoints / (double)maxSailPoints;

                    gunPoints = m_Boat.GunPoints;
                    maxGunPoints = m_Boat.MaxGunPoints;
                    gunPercent = (double)gunPoints / (double)maxGunPoints;

                    leftCannons = 10;
                    leftCannonsReady = true;

                    rightCannons = 10;
                    rightCannonsReady = false;

                    //TEST: Add Ability Display Names and Descriptions
                    //public ActiveAbilityType m_ActiveAbility = ActiveAbilityType.None;
                    minorAbilityText1 = "Mastercraft";
                    minorAbilityText2 = "Cannons";
                    minorAbilityTimeRemaining = "Ready";
                    minorAbilityReady = true;

                    //TEST: Add Ability Display Names and Descriptions
                    //public EpicAbilityType m_EpicAbility = EpicAbilityType.None;
                    epicAbilityText1 = "Hellfire";
                    epicAbilityText2 = "Ammunition";
                    epicAbilityTimeRemaining = "4m 30s";
                    epicAbilityReady = false;

                    switch (m_Boat.TargetingMode)
                    {
                        case TargetingMode.Random: targetingMode = "Random"; break;
                        case TargetingMode.Hull: targetingMode = "Hull"; break;
                        case TargetingMode.Sails: targetingMode = "Sails"; break;
                        case TargetingMode.Guns: targetingMode = "Guns"; break;
                    }
                }
            
                AddAlphaRegion(0, 0, 304, 392);

                //Gump Controls
                AddAlphaRegion(0, 0, 30, 85);
                AddButton(3, 0, 9900, 9900, 27, GumpButtonType.Reply, 0);
                AddItem(-8, 29, 5363);
                AddButton(6, 62, 1210, 1209, 28, GumpButtonType.Reply, 0);

                //Directions            
                AddButton(129, 269, 4500, 4500, 1, GumpButtonType.Reply, 0); //Forward
                AddButton(189, 269, 4501, 4501, 2, GumpButtonType.Reply, 0); //Forward Right
                AddButton(236, 312, 4502, 4502, 3, GumpButtonType.Reply, 0); //Right
                AddButton(236, 357, 4503, 4503, 4, GumpButtonType.Reply, 0); //Backwards Right //x=189
                AddButton(129, 357, 4504, 4504, 5, GumpButtonType.Reply, 0); //Backwards
                AddButton(24, 357, 4505, 4505, 6, GumpButtonType.Reply, 0); //Backwards Left //x=69
                AddButton(24, 312, 4506, 4506, 7, GumpButtonType.Reply, 0); //Left
                AddButton(69, 269, 4507, 4507, 8, GumpButtonType.Reply, 0); //Forward Right

                //Center Controls
                AddButton(83, 328, 4014, 4016, 9, GumpButtonType.Reply, 0);
                AddButton(140, 328, 4017, 4019, 10, GumpButtonType.Reply, 0);
                AddButton(196, 328, 4005, 4007, 11, GumpButtonType.Reply, 0);
                
                //Movement Mode
                AddLabel(34, 167, 187, "Movement Mode");
                AddLabel(Utility.CenteredTextOffset(85, movementMode), 189, textHue, movementMode);
                AddButton(33, 194, 2223, 2223, 12, GumpButtonType.Reply, 0);
                AddButton(109, 194, 2224, 2224, 13, GumpButtonType.Reply, 0);

                //Action
                string actionText = "Embark/Disembark";
                int buttonID = 4029;
                int buttonPressedID = 4029;

                switch (m_Player.m_ShipControlSettings.m_ShipAction)
                {
                    case ShipAction.RaiseAnchor:
                        actionText = "Raise Anchor";
                        buttonID = 4014;
                        buttonPressedID = 4016;
                    break;

                    case ShipAction.LowerAnchor:
                        actionText = "Lower Anchor";
                        buttonID = 4005;
                        buttonPressedID = 4007;
                    break;

                    case ShipAction.Embark:
                        actionText = "Embark";
                        buttonID = 4002;
                        buttonPressedID = 4004;
                    break;

                    case ShipAction.EmbarkFollowers:
                        actionText = "Embark Followers";
                        buttonID = 4008;
                        buttonPressedID = 4010;
                    break;

                    case ShipAction.Disembark:
                        actionText = "Disembark";
                        buttonID = 4002;
                        buttonPressedID = 4004;
                    break;

                    case ShipAction.DisembarkFollowers:
                        actionText = "Disembark Followers";
                        buttonID = 4008;
                        buttonPressedID = 4010;
                    break;

                    case ShipAction.Dock:
                        actionText = "Dock The Ship";
                        buttonID = 4017;
                        buttonPressedID = 4019;
                    break;

                    case ShipAction.ClearDeck:
                        actionText = "Clear The Deck";
                        buttonID = 4020;
                        buttonPressedID = 4022;
                    break;                    

                    case ShipAction.DividePlunder:
                        actionText = "Divide The Plunder";
                        buttonID = 4029;
                        buttonPressedID = 4031;
                    break;

                    case ShipAction.AddFriend:
                        actionText = "Add Friend";
                        buttonID = 4003;
                        buttonPressedID = 4002;
                    break;

                    case ShipAction.AddCoOwner:
                        actionText = "Add Co-Owner";
                        buttonID = 4003;
                        buttonPressedID = 4002;
                    break;

                    case ShipAction.ThrowTargetOverboard:
                        actionText = "Throw Target Overboard";
                        buttonID = 4014;
                        buttonPressedID = 4016;
                    break;
                }
                
                AddLabel(Utility.CenteredTextOffset(232, actionText), 167, 169, actionText);
                AddButton(181, 194, 2223, 2223, 14, GumpButtonType.Reply, 0);
                AddButton(252, 194, 2224, 2224, 15, GumpButtonType.Reply, 0);
                AddButton(212, 189, buttonID, buttonPressedID, 16, GumpButtonType.Reply, 0);

                //Left Cannon
                AddItem(2, 206, 733);

                if (leftCannons > -1)
                {
                    if (leftCannonsReady)
                        AddButton(28, 273, 2152, 2151, 17, GumpButtonType.Reply, 0);
                    else
                        AddButton(28, 273, 9720, 9722, 17, GumpButtonType.Reply, 0);

                    AddLabel(10, 245, textHue, leftCannons.ToString());
                }

                else
                    AddLabel(10, 245, textHue, "-");

                //Right Cannon
                AddItem(253, 209, 709);

                if (rightCannons > 1)
                {
                    if (rightCannonsReady)
                        AddButton(256, 275, 2152, 2151, 18, GumpButtonType.Reply, 0);
                    else
                        AddButton(256, 275, 9720, 9722, 18, GumpButtonType.Reply, 0);


                    AddLabel(285, 248, textHue, rightCannons.ToString());
                }

                else
                    AddLabel(285, 248, textHue, "-");

                //Targeting Mode
                AddLabel(105, 222, 2115, "Targeting Mode");
                AddLabel(Utility.CenteredTextOffset(157, targetingMode), 241, textHue, targetingMode);

                if (m_Boat != null)
                {
                    AddButton(96, 245, 2223, 2223, 19, GumpButtonType.Reply, 0);
                    AddButton(190, 245, 2224, 2224, 20, GumpButtonType.Reply, 0);
                }

                //Minor Ability
                AddLabel(33, 75, 2603, "Ship Minor Ability");

                if (m_Boat != null)
                {
                    if (minorAbilityText2 != "")
                    {
                        AddLabel(Utility.CenteredTextOffset(89, minorAbilityText1), 94, textHue, minorAbilityText1);
                        AddLabel(Utility.CenteredTextOffset(89, minorAbilityText2), 109, textHue, minorAbilityText2);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(89, minorAbilityText1), 101, textHue, minorAbilityText1);

                    if (minorAbilityReady)
                        AddLabel(21, 123, 169, "Ready");
                    else
                        AddLabel(21, 123, 2562, minorAbilityTimeRemaining);
                }

                else
                    AddLabel(Utility.CenteredTextOffset(89, "-"), 101, textHue, "-");

                if (m_Boat != null)
                {
                    AddButton(19, 104, 2223, 2223, 21, GumpButtonType.Reply, 0);
                    AddButton(132, 104, 2224, 2224, 22, GumpButtonType.Reply, 0);

                    if (minorAbilityReady)
                        AddButton(68, 130, 2152, 2151, 23, GumpButtonType.Reply, 0);
                    else
                        AddButton(68, 130, 9720, 9722, 23, GumpButtonType.Reply, 0);
                }

                //Epic Ability
                AddLabel(176, 76, 2606, "Ship Epic Ability");
                if (m_Boat != null)
                {
                    if (epicAbilityText2 != "")
                    {
                        AddLabel(Utility.CenteredTextOffset(232, epicAbilityText1), 94, textHue, epicAbilityText1);
                        AddLabel(Utility.CenteredTextOffset(232, epicAbilityText2), 109, textHue, epicAbilityText2);
                    }

                    else
                        AddLabel(Utility.CenteredTextOffset(232, epicAbilityText1), 101, textHue, epicAbilityText1);

                    if (epicAbilityReady)
                        AddLabel(244, 127, 169, "Ready");
                    else
                        AddLabel(244, 127, 2562, epicAbilityTimeRemaining);
                }

                else
                    AddLabel(Utility.CenteredTextOffset(230, "-"), 101, textHue, "-");

                if (m_Boat != null)
                {
                    AddButton(162, 105, 2223, 2223, 24, GumpButtonType.Reply, 0);
                    AddButton(270, 105, 2224, 2224, 25, GumpButtonType.Reply, 0);

                    if (epicAbilityReady)
                        AddButton(211, 131, 2152, 2151, 26, GumpButtonType.Reply, 0);    
                    else
                        AddButton(211, 131, 9720, 9722, 26, GumpButtonType.Reply, 0);  
                }

                //Stats
                AddLabel(48, 7, 149, "Hull");
                AddImage(79, 11, 2057);
                AddImageTiled(79 + Utility.ProgressBarX(hullPercent), 14, Utility.ProgressBarWidth(hullPercent), 7, 2488);
                if (maxHullPoints > -1)
                    AddLabel(196, 7, textHue, hullPoints.ToString() + "/" + maxHullPoints.ToString());

                AddLabel(43, 27, 187, "Sails");
                AddImage(79, 32, 2054);
                AddImageTiled(79 + Utility.ProgressBarX(sailPercent), 34, Utility.ProgressBarWidth(sailPercent), 7, 2488);
                if (maxSailPoints > -1)
                    AddLabel(196, 27, textHue, sailPoints.ToString() + "/" + maxSailPoints.ToString());

                AddLabel(43, 46, textHue, "Guns");
                AddImage(79, 51, 2057, 2499);
                AddImageTiled(79 + Utility.ProgressBarX(gunPercent), 54, Utility.ProgressBarWidth(gunPercent), 7, 2488);
                if (maxGunPoints > -1)
                    AddLabel(196, 47, textHue, gunPoints.ToString() + "/" + maxGunPoints.ToString());
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_Player == null)
                return;

            BaseBoat m_Boat = m_Player.BoatOccupied;

            if (m_Boat != null)
            {
                if (m_Boat.Deleted || m_Boat.m_SinkTimer != null)
                    m_Boat = null;
            }

            bool closeGump = true;

            int minorActionCount = 0;
            int epicActionCount = 0;

            int currentMovementMode;
            int movementModeCount = Enum.GetNames(typeof(MovementMode)).Length;

            int currentShipAction;
            int shipActionCount = Enum.GetNames(typeof(ShipAction)).Length;

            switch (info.ButtonID)
            {
                //Forward
                case 1:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveForward(m_Player);

                        else
                            BaseBoat.OneMoveForward(m_Player);
                    }

                    closeGump = false;
                break;

                //Forward Right
                case 2:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveForwardRight(m_Player);

                        else
                            BaseBoat.OneMoveForwardRight(m_Player);
                    }

                    closeGump = false;
                break;

                //Right
                case 3:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveRight(m_Player);

                        else
                            BaseBoat.OneMoveRight(m_Player);
                    }

                    closeGump = false;
                break;

                //Backwards Right
                case 4:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveBackwardRight(m_Player);

                        else
                            BaseBoat.OneMoveBackwardRight(m_Player);
                    }

                    closeGump = false;
                break;

                //Backward
                case 5:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveBackward(m_Player);

                        else
                            BaseBoat.OneMoveBackward(m_Player);
                    }

                    closeGump = false;
                break;

                //Backwards Left
                case 6:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveBackwardLeft(m_Player);

                        else
                            BaseBoat.OneMoveBackwardLeft(m_Player);
                    }

                    closeGump = false;
                break;

                //Left
                case 7:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveLeft(m_Player);

                        else
                            BaseBoat.OneMoveLeft(m_Player);
                    }

                    closeGump = false;
                break;

                //Forward Left
                case 8:
                    if (m_Boat != null)
                    {
                        if (m_Player.m_ShipControlSettings.m_MovementMode == MovementMode.Full)
                            BaseBoat.StartMoveForwardLeft(m_Player);

                        else
                            BaseBoat.OneMoveForwardLeft(m_Player);

                    }

                    closeGump = false;
                break;

                //Turn Left
                case 9:
                    if (m_Boat != null)
                        BaseBoat.StartTurnLeft(m_Player);

                    closeGump = false;
                break;

                //Stop
                case 10:
                    if (m_Boat != null)
                        BaseBoat.Stop(m_Player);

                    closeGump = false;
                break;

                //Turn Right
                case 11:
                    if (m_Boat != null)
                        BaseBoat.StartTurnRight(m_Player);

                    closeGump = false;
                break;

                //Previous Movement Mode
                case 12:
                    currentMovementMode = (int)m_Player.m_ShipControlSettings.m_MovementMode;
                    currentMovementMode--;

                    if (currentMovementMode < 0)
                        currentMovementMode = movementModeCount - 1;

                    m_Player.m_ShipControlSettings.m_MovementMode = (MovementMode)currentMovementMode;

                    closeGump = false;
                break;

                //Next Movement Mode
                case 13:
                    currentMovementMode = (int)m_Player.m_ShipControlSettings.m_MovementMode;
                    currentMovementMode++;

                    if (currentMovementMode > movementModeCount - 1)
                        currentMovementMode = 0;

                    m_Player.m_ShipControlSettings.m_MovementMode = (MovementMode)currentMovementMode;

                    closeGump = false;
                break;

                //Previous Ship Action
                case 14:
                    currentShipAction = (int)m_Player.m_ShipControlSettings.m_ShipAction;
                    currentShipAction--;

                    if (currentShipAction < 0)
                        currentShipAction = shipActionCount - 1;

                    m_Player.m_ShipControlSettings.m_ShipAction = (ShipAction)currentShipAction;

                    closeGump = false;
                break;

                //Next Ship Action
                case 15:
                    currentShipAction = (int)m_Player.m_ShipControlSettings.m_ShipAction;
                    currentShipAction++;

                    if (currentShipAction > shipActionCount - 1)
                        currentShipAction = 0;

                    m_Player.m_ShipControlSettings.m_ShipAction = (ShipAction)currentShipAction;

                    closeGump = false;
                break;

                //Activate Ship Action
                case 16:
                    switch (m_Player.m_ShipControlSettings.m_ShipAction)
                    {
                        case ShipAction.RaiseAnchor:
                            if (m_Boat != null)
                            {
                                if (m_Boat.IsCoOwner(m_Player) || m_Boat.IsOwner(m_Player))
                                    BaseBoat.RaiseAnchor(m_Player);
                            }
                        break;

                        case ShipAction.LowerAnchor:
                            if (m_Boat != null)
                            {
                                if (m_Boat.IsCoOwner(m_Player) || m_Boat.IsOwner(m_Player))
                                    BaseBoat.LowerAnchor(m_Player);
                            }
                        break;

                        case ShipAction.Embark:
                            BaseBoat.TargetedEmbark(m_Player);                           
                        break;

                        case ShipAction.EmbarkFollowers:
                            BaseBoat.TargetedEmbarkFollowers(m_Player);
                        break;

                        case ShipAction.Disembark:
                            if (m_Boat != null)
                                m_Boat.Disembark(m_Player);
                        break;

                        case ShipAction.DisembarkFollowers:
                            if (m_Boat != null)
                                m_Boat.DisembarkFollowers(m_Player);
                        break;

                        case ShipAction.Dock:
                            if (m_Boat != null)
                            {
                                if (m_Boat.IsOwner(m_Player))
                                    m_Boat.BeginDryDock(m_Player);
                            }
                        break;

                        case ShipAction.ClearDeck:
                            //TEST: Finish
                        break;

                        case ShipAction.DividePlunder:
                            if (m_Boat != null)
                            {
                                if (m_Boat.IsOwner(m_Player))
                                    m_Boat.BeginDivideThePlunder(m_Player);
                            }
                        break;

                        case ShipAction.AddFriend:                        
                            if (m_Boat != null)
                                m_Boat.AddFriendCommand(m_Player);                            
                        break;

                        case ShipAction.AddCoOwner:
                            if (m_Boat != null)
                                m_Boat.AddCoOwnerCommand(m_Player);      
                        break;

                        case ShipAction.ThrowTargetOverboard:
                            if (m_Boat != null)
                                m_Boat.ThrowOverboardCommand(m_Player);
                        break;
                    }

                    closeGump = false;
                break;

                //Fire Left Cannons
                case 17:
                    if (m_Boat != null)
                    {
                        if (m_Boat.IsCoOwner(m_Player) || m_Boat.IsOwner(m_Player))
                            BaseBoat.FireCannons(m_Player, true);
                    }

                    closeGump = false;
                break;

                //Fire Right Cannons
                case 18:
                    if (m_Boat != null)
                    {
                        if (m_Boat.IsCoOwner(m_Player) || m_Boat.IsOwner(m_Player))
                            BaseBoat.FireCannons(m_Player, false);
                    }

                    closeGump = false;
                break;

                //Targeting Mode: Previous
                case 19:
                    if (m_Boat != null)
                    {
                        if (m_Boat.IsCoOwner(m_Player) || m_Boat.IsOwner(m_Player))
                        {
                            switch (m_Boat.TargetingMode)
                            {
                                case TargetingMode.Random: m_Boat.SetTargetingMode(TargetingMode.Guns); break;
                                case TargetingMode.Hull: m_Boat.SetTargetingMode(TargetingMode.Random); break;
                                case TargetingMode.Sails: m_Boat.SetTargetingMode(TargetingMode.Hull); break;
                                case TargetingMode.Guns: m_Boat.SetTargetingMode(TargetingMode.Sails); break;
                            }
                        }   
                    }

                    closeGump = false;
                break;

                //Targeting Mode: Next
                case 20:
                    if (m_Boat != null)
                    {
                        if (m_Boat.IsCoOwner(m_Player) || m_Boat.IsOwner(m_Player))
                        {
                            switch (m_Boat.TargetingMode)
                            {
                                case TargetingMode.Random: m_Boat.SetTargetingMode(TargetingMode.Hull); break;
                                case TargetingMode.Hull: m_Boat.SetTargetingMode(TargetingMode.Sails); break;
                                case TargetingMode.Sails: m_Boat.SetTargetingMode(TargetingMode.Guns); break;
                                case TargetingMode.Guns: m_Boat.SetTargetingMode(TargetingMode.Random); break;
                            }
                        }  
                    }

                    closeGump = false;
                break;

                //Minor Ability: Previous
                case 21:
                    if (m_Boat != null)
                    {
                    }

                    closeGump = false;
                break;

                //Minor Ability: Next
                case 22:
                    if (m_Boat != null)
                    {
                    }

                    closeGump = false;
                break;

                //Minor Ability: Activate
                case 23:
                    if (m_Boat != null)
                    {
                    }

                    closeGump = false;
                break;

                //Epic Ability: Previous
                case 24:
                    if (m_Boat != null)
                    {
                    }

                    closeGump = false;
                break;

                //Epic Ability: Next
                case 25:
                    if (m_Boat != null)
                    {
                    }

                    closeGump = false;
                break;

                //Epic Ability: Activate
                case 26:
                    if (m_Boat != null)
                    {
                    }

                    closeGump = false;
                break;

                //Collapse + Expand
                case 27:
                    m_Player.m_ShipControlSettings.m_CollapseMode = !m_Player.m_ShipControlSettings.m_CollapseMode;

                    closeGump = false;
                break;

                //Ship Selection
                case 28:
                    BaseBoat.ShipSelection(m_Player);

                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_Player.CloseGump(typeof(BoatHotbarGump));
                m_Player.SendGump(new BoatHotbarGump(m_Player));
            }
        }
    }
}



