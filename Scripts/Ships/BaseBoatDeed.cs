using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;

using Server.Guilds;

namespace Server.Multis
{
    public abstract class BaseBoatDeed : Item
    {
        private int m_MultiID;
        private Point3D m_Offset;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MultiID { get { return m_MultiID; } set { m_MultiID = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D Offset { get { return m_Offset; } set { m_Offset = value; } }

        public abstract BaseBoat Boat { get; }

        public virtual int DoubloonCost { get { return 0; } }
        public virtual double DoubloonMultiplier { get { return 1.0; } }

        public Mobile m_Owner;
        public List<Mobile> m_CoOwners = new List<Mobile>();
        public List<Mobile> m_Friends = new List<Mobile>();

        public bool GuildAsFriends = true;

        public int HitPoints;
        public int SailPoints;
        public int GunPoints;

        public string m_ShipName;

        public TargetingMode m_TargetingMode = TargetingMode.Random;

        public DateTime m_TimeLastRepaired;
        public DateTime m_NextTimeRepairable;

        [Constructable]
        public BaseBoatDeed(int id, Point3D offset): base(0x14F2)
        {
            Weight = 1.0;

            HitPoints = Boat.MaxHitPoints;
            SailPoints = Boat.MaxSailPoints;
            GunPoints = Boat.MaxGunPoints;

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;

            m_MultiID = id;
            m_Offset = offset;
        }

        public BaseBoatDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", Name));

                if (PlayerClassRestricted)
                {
                    if (PlayerClassOwner == null)
                    {
                        if (DoubloonCost > 0)
                            ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "[Requires " + DoubloonCost.ToString() + " doubloons]"));
                    }

                    else
                        ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "[Bound to " + PlayerClassOwner.RawName + "]"));
                }
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            if (PlayerClassRestricted)
            {
                if (PlayerClassOwner == null)
                {
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(from, typeof(Doubloon));

                    if (doubloonsInBank >= DoubloonCost)
                    {
                        from.CloseAllGumps();
                        from.SendGump(new BindBaseBoatDeedGump(this, pm_From));
                    }

                    else
                        from.SendMessage("You must have at least " + DoubloonCost.ToString() + " doubloons in your bank to claim this ship as your own.");
                }

                else
                {
                    if (PlayerClassOwner == from)
                    {
                        if (!IsChildOf(from.Backpack))
                            from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.			

                        else if (from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas))
                            from.SendLocalizedMessage(1010567, null, 0x25); // You may not place a boat from this location.			

                        else
                        {
                            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
                            from.Target = new InternalTarget(this);
                        }
                    }

                    else
                    {
                        PlayerClassOwner = from;
                        from.SendMessage("You claim the ship deed as your own.");
                    }                        
                }
            }

            else
            {
                if (!IsChildOf(from.Backpack))
                    from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.			

                else if (from.AccessLevel < AccessLevel.GameMaster && (from.Map == Map.Ilshenar || from.Map == Map.Malas))
                    from.SendLocalizedMessage(1010567, null, 0x25); // You may not place a boat from this location.			

                else
                {
                    from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 502482); // Where do you wish to place the ship?
                    from.Target = new InternalTarget(this);
                }
            }
        }

        public void OnPlacement(Mobile from, Point3D p)
        {
            PlayerMobile player = from as PlayerMobile;

            if (Deleted)
                return;

            else if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.			

            else
            {
                Map map = from.Map;

                if (map == null)
                    return;

                if (from.AccessLevel < AccessLevel.GameMaster && map != Map.Felucca)
                {
                    from.SendLocalizedMessage(1043284); // A ship can not be created here.
                    return;
                }

                if (from.Region.IsPartOf(typeof(HouseRegion)) || BaseBoat.FindBoatAt(from, from.Map) != null)
                {
                    from.SendLocalizedMessage(1010568, null, 0x25); // You may not place a ship while on another ship or inside a house.
                    return;
                }

                if (from.GetDistanceToSqrt(p) > 10)
                {
                    from.SendMessage("You cannot place a ship that far away from land.");
                    return;
                }

                foreach(BaseBoat boatInstance in BaseBoat.m_Instances)
                {
                    if (boatInstance.Owner == from)
                    {
                        from.SendMessage("You already have a boat at sea.");
                        return;
                    }
                }

                BaseBoat boat = Boat;

                if (boat == null)
                    return;

                p = new Point3D(p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z);

                Direction newDirection = Direction.North;
                int shipFacingItemID = -1;

                switch (from.Direction)
                {
                    case Direction.North:
                        newDirection = Direction.North;
                        shipFacingItemID = boat.NorthID;
                    break;

                    case Direction.Up:
                        newDirection = Direction.North;
                        shipFacingItemID = boat.NorthID;
                    break;

                    case Direction.East:
                        newDirection = Direction.East;
                        shipFacingItemID = boat.EastID;
                    break;

                    case Direction.Right:
                        newDirection = Direction.East;
                        shipFacingItemID = boat.EastID;
                    break;

                    case Direction.South:
                        newDirection = Direction.South;
                        shipFacingItemID = boat.SouthID;
                    break;

                    case Direction.Down:
                        newDirection = Direction.South;
                        shipFacingItemID = boat.SouthID;
                    break;

                    case Direction.West:
                        newDirection = Direction.West;
                        shipFacingItemID = boat.WestID;
                    break;

                    case Direction.Left:
                        newDirection = Direction.West;
                        shipFacingItemID = boat.WestID;
                    break;

                    default:
                        newDirection = Direction.North;
                        shipFacingItemID = boat.NorthID;
                    break; 
                }

                if (BaseBoat.IsValidLocation(p, map) && boat.CanFit(p, map, shipFacingItemID))
                {   
                    //Set Boat Properties Stored in Deed
                    boat.CoOwners = m_CoOwners;
                    boat.Friends = m_Friends;
                    boat.GuildAsFriends = GuildAsFriends;

                    boat.Owner = from;
                    boat.ShipName = m_ShipName;

                    boat.TargetingMode = m_TargetingMode;
                    boat.TimeLastRepaired = m_TimeLastRepaired;
                    boat.NextTimeRepairable = m_NextTimeRepairable;                   

                    boat.DecayTime = DateTime.UtcNow + boat.BoatDecayDelay;                        
                        
                    boat.Anchored = true;

                    ShipUniqueness.GenerateShipUniqueness(boat);                       

                    boat.HitPoints = HitPoints;
                    boat.SailPoints = SailPoints;
                    boat.GunPoints = GunPoints;

                    bool fullSailPoints = (boat.SailPoints == boat.BaseMaxSailPoints);
                    bool fullGunPoints = (boat.GunPoints == boat.BaseMaxGunPoints);
                    bool fullHitPoints = (boat.HitPoints == boat.BaseMaxHitPoints);   
                    
                    boat.SetFacing(newDirection);

                    boat.MoveToWorld(p, map);                       

                    Delete();

                    BoatRune boatRune = new BoatRune(boat, from);
                    boat.BoatRune = boatRune;

                    BoatRune boatBankRune = new BoatRune(boat, from);
                    boat.BoatBankRune = boatBankRune;                        

                    bool addedToPack = false;
                    bool addedToBank = false;

                    if (from.AddToBackpack(boatRune))
                        addedToPack = true;

                    BankBox bankBox = from.FindBankNoCreate();

                    if (bankBox != null)
                    {
                        if (bankBox.Items.Count < bankBox.MaxItems)
                        {
                            bankBox.AddItem(boatBankRune);
                            addedToBank = true;
                        }
                    }

                    string message = "You place the ship at sea. A boat rune has been placed both in your bankbox and your backpack.";

                    if (!addedToPack && !addedToBank)
                        message = "You place the ship at sea. However, there was no room in neither your bankbox nor your backpack to place boat runes.";

                    else if (!addedToPack)
                        message = "You place the ship at sea. A boat rune was placed in your bankbox, however, there was no room in your backpack to place a boat rune.";

                    else if (!addedToBank)
                        message = "You place the ship at sea. A boat rune was placed in your backpack, however, there was no room in your bankbox to place a boat rune.";

                    from.SendMessage(message);
                }

                else
                {
                    boat.Delete();
                    from.SendMessage("A boat cannot be placed there. You may change your facing to change the direction of the boat placement.");
                }
            }
        }

        private class InternalTarget : MultiTarget
        {
            private BaseBoatDeed m_Deed;

            public InternalTarget(BaseBoatDeed deed): base(deed.MultiID, deed.Offset)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D ip = o as IPoint3D;

                if (ip != null)
                {
                    if (ip is Item)
                        ip = ((Item)ip).GetWorldTop();

                    Point3D p = new Point3D(ip);

                    Region region = Region.Find(p, from.Map);

                    if (region.IsPartOf(typeof(DungeonRegion)))
                        from.SendLocalizedMessage(502488); // You can not place a ship inside a dungeon.

                    else if (region.IsPartOf(typeof(HouseRegion)) || region.IsPartOf(typeof(ChampionSpawnRegion)))
                        from.SendLocalizedMessage(1042549); // A boat may not be placed in this area.

                    else
                        m_Deed.OnPlacement(from, p);
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //Version

            //Version 0
            writer.Write(m_MultiID);
            writer.Write(m_Offset);

            writer.Write(m_CoOwners.Count);
            for (int a = 0; a < m_CoOwners.Count; a++)
            {
                writer.Write(m_CoOwners[a]);
            }

            writer.Write(m_Friends.Count);
            for (int a = 0; a < m_Friends.Count; a++)
            {
                writer.Write(m_Friends[a]);
            }

            writer.Write(GuildAsFriends);

            writer.Write(HitPoints);
            writer.Write(SailPoints);
            writer.Write(GunPoints);

            writer.Write(m_Owner);
            writer.Write(m_ShipName);

            writer.Write((int)m_TargetingMode);
            writer.Write(m_TimeLastRepaired);
            writer.Write(m_NextTimeRepairable);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            
            //Version 0
            if (version >= 0)
            {
                m_MultiID = reader.ReadInt();
                m_Offset = reader.ReadPoint3D();

                int coOwnerCount = reader.ReadInt();
                for (int a = 0; a < coOwnerCount; a++)
                {
                    m_CoOwners.Add(reader.ReadMobile());
                }

                int friendCount = reader.ReadInt();
                for (int a = 0; a < friendCount; a++)
                {
                    m_Friends.Add(reader.ReadMobile());
                }

                GuildAsFriends = reader.ReadBool();

                HitPoints = reader.ReadInt();
                SailPoints = reader.ReadInt();
                GunPoints = reader.ReadInt();

                m_Owner = reader.ReadMobile();

                m_ShipName = reader.ReadString();

                m_TargetingMode = (TargetingMode)reader.ReadInt();

                m_TimeLastRepaired = reader.ReadDateTime();
                m_NextTimeRepairable = reader.ReadDateTime();
            }
        }
    }

    public class BindBaseBoatDeedGump : Gump
    {
        BaseBoatDeed m_BaseBoatDeed;
        PlayerMobile m_Player;

        public BindBaseBoatDeedGump(BaseBoatDeed baseBoatDeed, PlayerMobile player): base(50, 50)
        {
            m_BaseBoatDeed = baseBoatDeed;
            m_Player = player;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddBackground(90, 150, 380, 185, 5054);
            AddBackground(100, 160, 360, 165, 3000);

            AddLabel(110, 175, 0, @"The following number of doubloons will be removed");
            AddLabel(110, 197, 0, @"from your bank box and the ship will be bound to you");
            AddHtml(118, 230, 320, 17, @"<center>" + m_BaseBoatDeed.DoubloonCost.ToString() + "</center>", (bool)false, (bool)false);
            AddItem(255, 255, 2539);

            AddButton(170, 285, 247, 248, 1, GumpButtonType.Reply, 0); //Okay
            AddButton(310, 285, 243, 248, 2, GumpButtonType.Reply, 0); //Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch (info.ButtonID)
            {
                case 1:
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(from, typeof(Doubloon));

                    if (doubloonsInBank >= m_BaseBoatDeed.DoubloonCost)
                    {
                        if (Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), m_BaseBoatDeed.DoubloonCost))
                        {
                            Doubloon doubloonPile = new Doubloon(m_BaseBoatDeed.DoubloonCost);
                            from.SendSound(doubloonPile.GetDropSound());
                            doubloonPile.Delete();
                            
                            m_BaseBoatDeed.PlayerClassOwner = m_Player;
                            m_Player.SendMessage("You claim the boat as your own.");
                        }

                        else
                            m_Player.SendMessage("You no longer have the required doubloons in your bank box to claim this boat.");
                    }

                    else
                        m_Player.SendMessage("You no longer have the required doubloons in your bank box to claim this boat.");
                    break;
            }
        }
    }
}