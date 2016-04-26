using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server
{
    public class Faction
    {
        public virtual bool Active { get { return true; } }
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

        public Faction()
        {
            FactionPersistance.Factions.Add(this);
        }

        public void Audit()
        {
            if (!Active)
                Players.Clear();
        }
        
        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); //Version

            //Version 0
            writer.Write(Players.Count);
            for (int a = 0; a < Players.Count; a++)
            {
                writer.Write(Players[a]);
            }
        }        

        public void Deserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

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

            FactionPersistance.Factions.Add(this);
        }
    }
}