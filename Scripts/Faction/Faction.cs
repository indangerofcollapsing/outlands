using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class Faction : Item
    {
        public enum FactionTypeValue
        {
            Freedom,
            Unity
        }

        public static List<Type> FactionList = new List<Type>() 
        {
            typeof(Freedom), 
            typeof(Unity), 
        };

        public static List<Faction> Factions = new List<Faction>();

        public virtual bool Active { get { return true; } }

        public virtual FactionTypeValue FactionType { get { return FactionTypeValue.Freedom; } }
        public virtual string FactionName { get { return "Faction Name"; } }

        public virtual int TextHue { get { return 2550; } }

        public virtual int SymbolIconId { get { return 16140; } }
        public virtual int SymbolIconHue { get { return 2550; } }
        public virtual int SymbolIconOffsetX { get { return 0; } }
        public virtual int SymbolIconOffsetY { get { return 0; } }

        public virtual int FlagIconId { get { return 6326; } }
        public virtual int FlagIconHue { get { return 2550; } }
        public virtual int FlagIconOffsetX { get { return 0; } }
        public virtual int FlagIconOffsetY { get { return 0; } }

        public List<PlayerMobile> Players = new List<PlayerMobile>();

        //-----

        public static int whiteTextHue = 2655;
        public static int greenTextHue = 0x3F;
        public static int lightGreenTextHue = 2599;
        public static int blueTextHue = 2603;
        public static int purpleTextHue = 2606;
        public static int lightPurpleTextHue = 2628;
        public static int yellowTextHue = 2550;
        public static int redTextHue = 2115;
        public static int greyTextHue = 2401;
        public static int orangeTextHue = 149;

        [Constructable]
        public Faction()
        {
            Factions.Add(this);
        }

        public Faction(Serial serial): base(serial)
        {
        }

        public static void OnLogin(PlayerMobile player)
        {
            CheckCreateFactionPlayerProfile(player);
        }

        public static void CheckCreateFactionPlayerProfile(PlayerMobile player)
        {
            if (player == null)
                return;

            if (player.m_FactionPlayerProfile == null)
                player.m_FactionPlayerProfile = new FactionPlayerProfile(player);

            if (player.m_FactionPlayerProfile.Deleted)
                player.m_FactionPlayerProfile = new FactionPlayerProfile(player);
        }

        public void Audit()
        {
            if (!Active)
                Players.Clear();
        }

        public static Faction GetFaction(FactionTypeValue factionType)
        {
            foreach(Faction faction in Factions)
            {
                if (faction.FactionType == factionType)
                    return faction;
            }

            return null;
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            //Version 0
            writer.Write(Players.Count);
            for (int a = 0; a < Players.Count; a++)
            {
                writer.Write(Players[a]);
            }
        }        

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                int playerCount = reader.ReadInt();
                for (int a = 0; a < playerCount; a++)
                {
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();

                    if (player != null)
                        Players.Add(player);
                }
            }

            //-----

            Factions.Add(this);
        }
    }
}