using Server.Custom.Battlegrounds.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds
{
    public class Team
    {
        public string Name { get; private set; }
        public List<PlayerMobile> Players { get; private set; }
        public Point3D SpawnPoint { get; private set; }
        public Point3D FlagLocation { get; private set; }

        public CTFFlag Flag { get; private set; }

        protected Map Map { get; private set; }

        public int Color { get; private set; }
        public int CarrierHue { get; private set; }

        private int m_FlagID;

        // ctf constructor
        public Team(string name, Point3D spawnPoint, int color, Point3D flagLocation, Map map, int carrierHue, int flagId = 5525)
            : this(name, spawnPoint, color)
        {
            FlagLocation = flagLocation;
            Map = map;
            m_FlagID = flagId;
            CarrierHue = carrierHue;
        }

        // siege constructor
        public Team(string name, Point3D spawnPoint, int color)
        {
            Name = name;
            Players = new List<PlayerMobile>();
            SpawnPoint = spawnPoint;
            Color = color;
        }

        public int Count { get { return Players.Count; } }

        public void SetupFlag() 
        {
            Flag = new CTFFlag(m_FlagID, this);
            Flag.MoveToWorld(FlagLocation, Map);
        }

        public void RemoveFlag()
        {
            Flag.Delete();
        }

        public bool Contains(PlayerMobile player)
        {
            return Players.Contains(player);
        }

        public void Add(PlayerMobile player)
        {
            if (!Players.Contains(player))
            {
                Players.Add(player);
            }
        }

        public void Remove(PlayerMobile player)
        {
            if (Players.Contains(player))
            {
                Players.Remove(player);
            }
        }

        public void AddRobe(PlayerMobile player)
        {
            var robe = player.FindItemOnLayer(Layer.OuterTorso);
            if (robe != null)
            {
                if (!player.AddToBackpack(robe))
                {
                    player.BankBox.DropItem(robe);
                }
            }

            robe = new BattlegroundRobe(Color);

            if (!player.EquipItem(robe))
                robe.Delete();
        }

        public void RemoveRobe(PlayerMobile player)
        {
            var robe = player.FindItemOnLayer(Layer.OuterTorso);
            if (robe == null || !(robe is BattlegroundRobe))
            {
                robe = player.Backpack.FindItemByType<BattlegroundRobe>();
                if (robe != null && !robe.Deleted)
                    robe.Delete();
            }
            else if (robe is BattlegroundRobe)
                robe.Delete();
        }
    }
}
