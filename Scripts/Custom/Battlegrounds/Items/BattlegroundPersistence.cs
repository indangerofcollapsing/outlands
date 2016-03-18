using Server.Custom.Battlegrounds.Mobiles;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{

    public class BattlegroundSettings 
    {
        public bool Enabled {get; set;}
        public bool FreeConsume {get; set;}

        public BattlegroundSettings(bool enabled, bool free)
        {
            Enabled = enabled;
            FreeConsume = free;
        }
    }

    public class BattlegroundSettingsPersistence : Item
    {
        public override string DefaultName { get { return "Battleground Settings Persistence - Internal"; } }
        private static BattlegroundSettingsPersistence m_Instance;

        public static BattlegroundSettingsPersistence Instance
        {
            get
            {
                if (m_Instance == null || m_Instance.Deleted)
                    new BattlegroundSettingsPersistence();

                return m_Instance;
            }
        }

        public Dictionary<string, BattlegroundSettings> Settings { get; private set; }

        public BattlegroundSettingsPersistence()
            : base(0)
        {
            Movable = false;
            if (m_Instance == null || m_Instance.Deleted)
            {
                Settings = new Dictionary<string, BattlegroundSettings>();
                m_Instance = this;
            }
            else
                base.Delete();
        }

        public BattlegroundSettingsPersistence(Serial serial)
            : base(serial)
        {
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(Settings.Count);
            foreach(var kvp in Settings)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.Enabled);
                writer.Write(kvp.Value.FreeConsume);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int count = reader.ReadInt();
            Settings = new Dictionary<string, BattlegroundSettings>();
            for(int i = 0; i < count; i++)
            {
                string key = reader.ReadString();
                bool enabled = reader.ReadBool();
                bool free = reader.ReadBool();
                Settings.Add(key, new BattlegroundSettings(enabled, free));
            }
            m_Instance = this;
        }
    }

    public class SiegeBattlegroundPersistence : Item
    {
        public override string DefaultName { get { return "Siege Battleground Persistence - Internal"; } }
        protected SiegeBattleground m_Battleground;


        public SiegeBattlegroundPersistence(SiegeBattleground battleground)
            : base(0)
        {
            Movable = false;
            m_Battleground = battleground;
        }

        public SiegeBattlegroundPersistence(Serial serial)
            : base(serial)
        {

        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Battleground.Name);

            writer.Write(m_Battleground.StartTime);

            m_Battleground.CurrentScoreboard.Serialize(writer);

            writer.Write(m_Battleground.Spectators.Count);
            foreach(var player in m_Battleground.Spectators)
            {
                writer.Write(player);
            }

            writer.Write(m_Battleground.Teams.Count);

            foreach (var team in m_Battleground.Teams)
            {
                writer.Write(team.Count);
                foreach (var player in team.Players)
                {
                    writer.Write(player);
                }
            }

            writer.Write(m_Battleground.King);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            string name = reader.ReadString();

            m_Battleground = (SiegeBattleground)Battleground.Instances.Find(bg => bg.Name == name);

            m_Battleground.Persistence = this;

            m_Battleground.State = BattlegroundState.Active;
            m_Battleground.Ended = false;

            m_Battleground.StartTime = reader.ReadDateTime();

            m_Battleground.CurrentScoreboard = new Scoreboard(m_Battleground);
            m_Battleground.CurrentScoreboard.Deserialize(reader);

            int specCount = reader.ReadInt();
            for (int i = 0; i < specCount; i++)
            {
                var player = reader.ReadMobile() as PlayerMobile;
                if (player != null)
                    m_Battleground.Spectators.Add(player);
            }

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int teamSize = reader.ReadInt();
                for (int x = 0; x < teamSize; x++)
                {
                    var mob = reader.ReadMobile() as PlayerMobile;
                    if (mob != null)
                    {
                        m_Battleground.Teams[i].Add(mob);
                        m_Battleground.Teams[i].AddRobe(mob);
                    }
                }

            }

            m_Battleground.King = reader.ReadMobile() as BattlegroundKing;

            m_Battleground.Resume();
        }
    }

    public class CTFBattlegroundPersistence : Item
    {

        public override string DefaultName { get { return "CTF Battleground Persistence - Internal"; } }

        protected CTFBattleground m_Battleground;

        public CTFBattlegroundPersistence(CTFBattleground battleground)
            : base(0)
        {
            Movable = false;
            m_Battleground = battleground;
        }

        public CTFBattlegroundPersistence(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Battleground.Name);

            writer.Write(m_Battleground.StartTime);

            m_Battleground.CurrentScoreboard.Serialize(writer);

            writer.Write(m_Battleground.Spectators.Count);
            foreach (var player in m_Battleground.Spectators)
            {
                writer.Write(player);
            }

            writer.Write(m_Battleground.Teams.Count);

            foreach (var team in m_Battleground.Teams)
            {
                writer.Write(team.Count);
                foreach (var player in team.Players)
                {
                    writer.Write(player);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            string name = reader.ReadString();

            m_Battleground = (CTFBattleground)Battleground.Instances.Find(bg => bg.Name == name);

            m_Battleground.Persistence = this;

            m_Battleground.State = BattlegroundState.Active;
            m_Battleground.Ended = false;

            m_Battleground.StartTime = reader.ReadDateTime();

            m_Battleground.CurrentScoreboard = new Scoreboard(m_Battleground);
            m_Battleground.CurrentScoreboard.Deserialize(reader);

            int specCount = reader.ReadInt();
            for (int i = 0; i < specCount; i++)
            {
                var player = reader.ReadMobile() as PlayerMobile;
                if (player != null)
                    m_Battleground.Spectators.Add(player);
            }

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int teamSize = reader.ReadInt();
                for (int x = 0; x < teamSize; x++)
                {
                    var mob = reader.ReadMobile() as PlayerMobile;
                    if (mob != null)
                    {
                        m_Battleground.Teams[i].Add(mob);
                        m_Battleground.Teams[i].AddRobe(mob);
                    }
                }

            }

            m_Battleground.Resume();
        }
    }
}
