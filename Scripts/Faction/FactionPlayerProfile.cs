using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server
{
    public class FactionPlayerProfile : Item
    {
        public PlayerMobile m_Player;

        #region Persistant Properties

        private Faction m_Faction = null;
        [CommandProperty(AccessLevel.GameMaster)]
        public Faction Faction
        {
            get { return m_Faction; }
            set { m_Faction = value; }
        }

        #endregion

        #region Capture Event Values

        public double CurrentCaptureEventScore = 0;
        public DateTime BattleFatigueExpiration = DateTime.UtcNow;

        public List<TimeSpan> CapturePointContention = new List<TimeSpan>();

        #endregion

        [Constructable]
        public FactionPlayerProfile(PlayerMobile player): base(0x0)
        {
            m_Player = player;

            //-----

            Visible = false;
            Movable = false;
        }

        public FactionPlayerProfile(Serial serial): base(serial)
        {
        }

        public void ResetCaptureEventValues()
        {
            CurrentCaptureEventScore = 0;
            BattleFatigueExpiration = DateTime.UtcNow;
            CapturePointContention.Clear();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //Version

            //Version 0

            writer.Write(m_Player);
            writer.Write(m_Faction);

            writer.Write(CurrentCaptureEventScore);
            writer.Write(BattleFatigueExpiration);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Player = (PlayerMobile)reader.ReadMobile();
                Faction = (Faction)reader.ReadItem();

                CurrentCaptureEventScore = reader.ReadDouble();
                BattleFatigueExpiration = reader.ReadDateTime();
            }

            //-----
        }
    }
}