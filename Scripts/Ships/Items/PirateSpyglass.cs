using System;
using Server;
using Server.Network;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Gumps;
using Server.Mobiles;
using Server.Custom;

namespace Server.Items
{
    public enum SpyglassAction
    {
        None,
        ShipsShort,
        ShipsMedium,
        ShipsLong,
        FishingShort,
        FishingMedium,
        FishingLong
    }

    public class PirateSpyglass : SpyglassMapViewItem
    {
        public override int PlayerClassCurrencyValue { get { return 50; } }

        public int m_ItemID = 5365;
        
        public int m_ViewDistance = 50;
        public int m_Width = 300;
        public int m_Height = 300;
        
        public DateTime m_NextSpyglassActionAllowed = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(30));

        public Timer m_SpyglassSearchTimer;
        public int m_SearchIntervalsCompleted = 0;

        public bool trackShips = true;
        public int rangeType = 0;

        [Constructable]
        public PirateSpyglass(): base()
        {
            Name = "A Pirate Spyglass";

            ItemID = m_ItemID;   

            m_Protected = false;
            m_Editable = false;

            LootType = LootType.Blessed;

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = 0;

            SetDisplay(0, 0, 5119, 4095, m_Width, m_Height);
        }

        public PirateSpyglass(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            //PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            /*
            if (player.SpyglassAction != SpyglassAction.None)
            {
                player.SendMessage("You are already using a spyglass at the moment.");
                return;
            }

            if (DateTime.UtcNow < player.NextSpyglassActionAllowed)
            {
                player.SendMessage("You have used a spyglass too recently to use one again.");
                return;
            }

            if (DateTime.UtcNow < m_NextSpyglassActionAllowed)
            {
                player.SendMessage("That spyglass has been used to recently to be used again.");
                return;
            }       
             */

            StartSpyglassAction(player);
        }

        public void StartSpyglassAction(PlayerMobile player)
        {
            player.CloseGump(typeof(PirateSpyglassGump));
            player.SendGump(new PirateSpyglassGump(this, player, trackShips, rangeType));
            player.SendMessage("Select how you will use the spyglass.");
        }

        public void StartSearching(PlayerMobile player, bool searchForShips)
        {
            m_SpyglassSearchTimer = new PirateSpyglassSearchTimer(this, player);
            m_SpyglassSearchTimer.Start();

            if (searchForShips)
                player.Emote("*begins scanning the horizon for ships*");
            else
                player.Emote("*begins scanning the water for activity*");
        }

        public void ResolveSpyglassAction(PlayerMobile player)
        {
            if (player == null)
                return;

            /*
            if (!player.Alive)
            {
                player.SpyglassAction = SpyglassAction.None;
                return;
            }

            if (player.Deleted)
                return;

            bool searchForShips = true;
            
            double spyglassActionRangeModifer = 1;            

            switch (player.SpyglassAction)
            {
                case SpyglassAction.None:
                    return;
                break;

                case SpyglassAction.ShipsShort:
                    spyglassActionRangeModifer = 1.0;
                break;

                case SpyglassAction.FishingShort:
                    spyglassActionRangeModifer = 1.0;
                    searchForShips = false;
                break;

                case SpyglassAction.ShipsMedium:
                    spyglassActionRangeModifer = 2.0;
                break;

                case SpyglassAction.FishingMedium:
                    spyglassActionRangeModifer = 2.0;
                    searchForShips = false;
                break;

                case SpyglassAction.ShipsLong:
                    spyglassActionRangeModifer = 3.0;
                break;

                case SpyglassAction.FishingLong:
                    spyglassActionRangeModifer = 3.0;
                    searchForShips = false;
                break;
            }   
             * */

            double trackingSkill = player.Skills[SkillName.Tracking].Value;
            double cartographySkill = player.Skills[SkillName.Cartography].Value;
            double rangeBonus = 1 + (0.25 * trackingSkill / 100) + (0.25 * cartographySkill / 100);

            int modifiedViewDistance = (int)((double)m_ViewDistance * rangeBonus * 1);

            Map = player.Map;

            SetDisplay(player.X - modifiedViewDistance, player.Y - modifiedViewDistance, player.X + modifiedViewDistance, player.Y + modifiedViewDistance, m_Width, m_Height);

            ClearPins();

            //Player Location
            AddWorldPin(player.Location.X, player.Location.Y);

            Dictionary<Point3D, int> m_Locations = new Dictionary<Point3D, int>();

            /*
            if (searchForShips)
            {
                foreach (BaseBoat foundBoat in BaseBoat.m_Instances)
                {
                    if (foundBoat == null)
                        continue;

                    if (foundBoat.Deleted || foundBoat.Map != player.Map)
                        continue;

                    if (player.BoatOccupied != null && player.BoatOccupied == foundBoat)
                        continue;

                    int distance = foundBoat.GetBoatToLocationDistance(foundBoat, player.Location);

                    if (distance <= modifiedViewDistance)
                        m_Locations.Add(foundBoat.Location, distance);
                }
            }

            else
            {                  
                foreach (FishingSpot fishingSpot in FishingSpot.AllFishingSpotInstances)
                {
                    if (fishingSpot == null)
                        continue;

                    if (fishingSpot.Deleted || fishingSpot.Map != player.Map)
                        continue;

                    int distance = (int)Utility.GetDistanceToSqrt(player.Location, fishingSpot.Location);

                    if (distance <= modifiedViewDistance)
                        m_Locations.Add(fishingSpot.Location, distance);
                }                   
            }
            */

            player.SendMessage("You are at " + player.Location.X.ToString() + ", " + player.Location.Y.ToString());

            if (m_Locations.Count > 0)
            {
                var items = from pair in m_Locations orderby pair.Value ascending select pair;

                int pinCount = 1;

                foreach (KeyValuePair<Point3D, int> pair in items)
                {
                    int x = pair.Key.X;
                    int y = pair.Key.Y;

                    AddWorldPin(x, y);

                    string objectType = "Ship ";

                    //if (!searchForShips)
                        //objectType = "Fishing spot ";

                    player.SendMessage(objectType + (pinCount + 1).ToString() + " located at " + x.ToString() + ", " + y.ToString());

                    pinCount++;
                }
            }

            else
            {
                /*
                if (searchForShips)
                    player.SendMessage("You are unable to locate any ships.");
                else
                    player.SendMessage("You are unable to locate any fishing spots.");
                 * */
            }            
            
            player.Send(new MapDetails(this));
            player.Send(new MapDisplay(this));

            for (int i = 0; i < m_Pins.Count; ++i)
                player.Send(new MapAddPin(this, m_Pins[i]));

            player.Send(new MapSetEditable(this, ValidateEdit(player)));            

            //player.SpyglassAction = SpyglassAction.None;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
           
            //Version 0
            writer.Write(m_NextSpyglassActionAllowed);     
       
            //Version 1
            writer.Write(trackShips);
            writer.Write(rangeType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_NextSpyglassActionAllowed = reader.ReadDateTime();
            }

            //Version 1
            if (version >= 1)
            {
                trackShips = reader.ReadBool();
                rangeType = reader.ReadInt();        
            }

            //-------------

            LootType = LootType.Blessed;
        }
    }

    public class PirateSpyglassSearchTimer : Timer
    {
        private PirateSpyglass m_PirateSpyglass;
        private PlayerMobile m_Player;
        private int m_Intervals = 0;

        public PirateSpyglassSearchTimer(PirateSpyglass pirateSpyglass, PlayerMobile player)
            : base(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5))
        {
            m_PirateSpyglass = pirateSpyglass;
            m_Player = player;

            Priority = TimerPriority.TwoFiftyMS;
        }

        protected override void OnTick()
        {
            m_Intervals++;

            m_PirateSpyglass.ResolveSpyglassAction(m_Player);
            Stop();

            /*
            switch (m_Player.SpyglassAction)
            {
                case SpyglassAction.ShipsShort:
                    if (m_Intervals >= 1)
                    {
                        m_PirateSpyglass.ResolveSpyglassAction(m_Player);
                        Stop();
                    }
                break;

                case SpyglassAction.FishingShort:
                    if (m_Intervals >= 1)
                    {
                        m_PirateSpyglass.ResolveSpyglassAction(m_Player);
                        Stop();
                    }
                break;

                case SpyglassAction.ShipsMedium:
                    if (m_Intervals >= 3)
                    {
                        m_PirateSpyglass.ResolveSpyglassAction(m_Player);
                        Stop();
                    }
                    else
                        m_Player.SendMessage("You continue to scan the horizon for ships...");
                break;

                case SpyglassAction.FishingMedium:
                if (m_Intervals >= 3)
                {
                    m_PirateSpyglass.ResolveSpyglassAction(m_Player);
                    Stop();
                }
                else
                    m_Player.SendMessage("You continue to scan the water for activity...");
                break;

                case SpyglassAction.ShipsLong:
                    if (m_Intervals >= 6)
                    {
                        m_PirateSpyglass.ResolveSpyglassAction(m_Player);
                        Stop();
                    }
                    else
                        m_Player.SendMessage("You continue to scan the horizon for ships...");
                break;

                case SpyglassAction.FishingLong:
                    if (m_Intervals >= 6)
                    {
                        m_PirateSpyglass.ResolveSpyglassAction(m_Player);
                        Stop();
                    }
                    else
                        m_Player.SendMessage("You continue to scan the water for activity...");
                break;
            }
            */
        }
    }

    public class PirateSpyglassGump : Gump
    {
        PirateSpyglass m_PirateSpyglass;
        PlayerMobile m_Player;

        bool m_Ships = true;
        int m_Range = 0;

        public PirateSpyglassGump(PirateSpyglass pirateSpyglass, PlayerMobile player, bool ships, int range): base(10, 10)
        {
            m_PirateSpyglass = pirateSpyglass;
            m_Player = player;

            m_Ships = ships;
            m_Range = range;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;
            int boldHue = 149;

            AddImage(0, 0, 1247);

            AddLabel(163, 25, boldHue, "Search Type");

            AddLabel(105, 45, textHue, "Ships");
            AddItem(82, 72, 5364);

            if (m_Ships)
                AddButton(122, 70, 2154, 2151, 1, GumpButtonType.Reply, 0);
            else
                AddButton(122, 70, 2151, 2154, 1, GumpButtonType.Reply, 0);

            AddLabel(264, 45, textHue, "Fishing");
            AddItem(255, 65, 5367);

            if (!m_Ships)
                AddButton(287, 70, 2154, 2151, 2, GumpButtonType.Reply, 0);
            else
                AddButton(287, 70, 2151, 2154, 2, GumpButtonType.Reply, 0);
                                 
            if (m_Range == 0)
                AddButton(241, 133, 2154, 2151, 3, GumpButtonType.Reply, 0); //Short
            else
                AddButton(241, 133, 2151, 2154, 3, GumpButtonType.Reply, 0); //Short

            if (m_Range == 1)
                AddButton(241, 164, 2154, 2151, 4, GumpButtonType.Reply, 0); //Medium
            else
                AddButton(241, 164, 2151, 2154, 4, GumpButtonType.Reply, 0); //Medium

            if (m_Range == 2)
                AddButton(241, 196, 2154, 2151, 5, GumpButtonType.Reply, 0); //Long
            else
                AddButton(241, 196, 2151, 2154, 5, GumpButtonType.Reply, 0); //Long

            AddButton(161, 255, 247, 248, 6, GumpButtonType.Reply, 0); //Okay

            AddItem(90, 201, 5365);                  
            AddItem(140, 201, 5365);
            AddItem(115, 201, 5365);
            AddItem(116, 169, 5365);
            AddItem(141, 169, 5365);
            AddItem(141, 138, 5365);

            AddLabel(160, 112, boldHue, "Search Range");  
   
            AddLabel(193, 137, textHue, "Short");
            AddLabel(184, 167, textHue, "Medium");
            AddLabel(196, 199, textHue, "Long");

            AddLabel(153, 235, boldHue, "Begin Search");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null)
                return;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            switch (info.ButtonID)
            {
                case 1:
                    m_Ships = true;

                    from.CloseGump(typeof(PirateSpyglassGump));
                    player.SendGump(new PirateSpyglassGump(m_PirateSpyglass, player, m_Ships, m_Range));
                break;

                case 2:
                    m_Ships = false;

                    from.CloseGump(typeof(PirateSpyglassGump));
                    player.SendGump(new PirateSpyglassGump(m_PirateSpyglass, player, m_Ships, m_Range));
                break;

                case 3:
                    m_Range = 0;

                    from.CloseGump(typeof(PirateSpyglassGump));
                    player.SendGump(new PirateSpyglassGump(m_PirateSpyglass, player, m_Ships, m_Range));
                break;

                case 4:
                    m_Range = 1;

                    from.CloseGump(typeof(PirateSpyglassGump));
                    player.SendGump(new PirateSpyglassGump(m_PirateSpyglass, player, m_Ships, m_Range));
                break;

                case 5:
                    m_Range = 2;

                    from.CloseGump(typeof(PirateSpyglassGump));
                    player.SendGump(new PirateSpyglassGump(m_PirateSpyglass, player, m_Ships, m_Range));
                break;

                case 6:
                    int duration = 5;

                    //Medium
                    if (m_Range == 1)
                        duration = 15;

                    //Long
                    else if (m_Range == 2)
                        duration = 30;

                    m_PirateSpyglass.trackShips = m_Ships;
                    m_PirateSpyglass.rangeType = m_Range;

                    m_PirateSpyglass.m_NextSpyglassActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(duration);

                    m_PirateSpyglass.StartSearching(m_Player, m_Ships);
                break;
            }
        }
    }
}