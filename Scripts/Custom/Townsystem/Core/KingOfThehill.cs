using Server.Factions;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Townsystem.Core
{
    public class KingOfTheHillTimer : Timer
    {
        private Town m_Town;
        private Town m_Majority;
        public Dictionary<Town, int> TownCapturePoints = new Dictionary<Town, int>(Town.Towns.Count);

        private int m_Ticks = 0;
        private int m_Keys = 10;
        private double m_WindBonus = 2.0;
        private DateTime m_LastDouseCheck = DateTime.UtcNow;
        private static TimeSpan DouseCheckPeriod = TimeSpan.FromSeconds(30);

        public KingOfTheHillTimer(Town town)
            : base(TimeSpan.Zero, TimeSpan.FromSeconds(10.0))
        {
            m_Town = town;
        }

        public void AddPoints(Town town, int points)
        {
            if (!TownCapturePoints.ContainsKey(town))
                TownCapturePoints[town] = 0;

            TownCapturePoints[town] += points;
        }

        private Town CapturePointHolder()
        {
            return m_Town.CaptureBrazier.CapTown;
        }

        private List<PlayerMobile> MilitiaNearBraziers(int range = 12)
        {
            var players = new List<PlayerMobile>();
            foreach (TownBrazier b in m_Town.Braziers)
            {
                players.AddRange(MilitiaNearBrazier(b, range));
            }

            players.AddRange(MilitiaNearBrazier(m_Town.CaptureBrazier));

            return players;
        }

        private List<PlayerMobile> MilitiaNearBrazier(TownBrazier brazier, int range = 12)
        {
            var players = new List<PlayerMobile>();
            IPooledEnumerable eable = brazier.Map.GetMobilesInRange(brazier.Location, range);
            foreach (Mobile player in eable)
            {
                if (player is PlayerMobile && ValidMilitiaPlayer(player as PlayerMobile))
                {
                    players.Add(player as PlayerMobile);
                }
            }
            eable.Free();
            return players;
        }

        private bool ValidMilitiaPlayer(PlayerMobile p)
        {
            return p != null && p.Alive && !p.Hidden && p.IsInMilitia && p.NetState != null && p.Citizenship == CapturePointHolder(); 
        }

        // called every N ticks
        private void AwardKeys(Town majority)
        {
            if (majority == null) return;

            var players = MilitiaNearBraziers().Distinct();

            foreach (PlayerMobile player in players)
            {
                if (player == null || player.Citizenship != majority)
                    continue;
                if (ValidMilitiaPlayer(player))
                {
                    int keyReward = m_Keys + (BraziersControlledByTown(CapturePointHolder()) * 2);

                    if (player.Citizenship.HasActiveBuff(Custom.Townsystem.CitizenshipBuffs.Greed))
                    {
                        keyReward = (int)(keyReward * 1.1);
                    }

                    if (player.Citizenship == WindBattleground.Owner)
                    {
                        keyReward = (int)(keyReward * m_WindBonus);
                    }

                    player.TreasuryKeys += keyReward;
                    player.SendMessage(String.Format("You have been awarded {0} treasury keys.", keyReward));
                }
            }

            m_Ticks = 0;
        }

        private int BraziersControlledByTown(Town town)
        {
            int count = 0;

            foreach (TownBrazier b in m_Town.Braziers)
            {
                if (b.Captured && b.CapTown == town)
                    count++;
            }

            return count;
        }

        private void AutoDouseBraziers()
        {
            if (m_LastDouseCheck + DouseCheckPeriod < DateTime.UtcNow)
            {
                if (m_Town.CaptureBrazier.Captured && MilitiaNearBrazier(m_Town.CaptureBrazier).Count == 0)
                {
                    m_Town.CaptureBrazier.Captured = false;
                    Faction.BroadcastAll("The capture brazier has gone out!");
                }

                foreach (TownBrazier b in m_Town.Braziers)
                {
                    if (!b.Captured || MilitiaNearBrazier(b, 24).Count > 0) continue;

                    if (b.LastLight + TimeSpan.FromMinutes(45) < DateTime.UtcNow)
                    {
                        b.Captured = false;
                        Faction.BroadcastAll(String.Format("The {0} brazier has gone out!", b.BrazierLocationName.ToLower()));
                        continue;
                    }

                    if (b.LastLight + TimeSpan.FromMinutes(30) < DateTime.UtcNow && Utility.RandomDouble() < 0.10)
                    {
                        b.Captured = false;
                        Faction.BroadcastAll(String.Format("The {0} brazier has gone out!", b.BrazierLocationName.ToLower()));
                    }
                }

                m_LastDouseCheck = DateTime.UtcNow;
            }
        }

        protected override void OnTick()
        {
            if (!OCTimeSlots.IsActiveTown(m_Town))
            {
                m_Town.DouseBraziers();
                TownCapturePoints.Clear();
                Stop(); // stop timers from old towns from running
                return;
            }

            Town controller = CapturePointHolder();

            int totalBraziers = m_Town.Braziers.Count;

            if (controller != null)
            {
                if (m_Majority == null || controller != m_Majority)
                {
                    m_Majority = controller;
                }

                int points = 2 + BraziersControlledByTown(controller);

                if (controller == WindBattleground.Owner)
                    points = (int)(m_WindBonus * points);

                AddPoints(controller, points);
            }

            foreach (KeyValuePair<Town, int> entry in TownCapturePoints.OrderBy(key => key.Value))
            {
                int noContr = BraziersControlledByTown(entry.Key);
                foreach (TownBrazier tb in m_Town.Braziers)
                    tb.PublicOverheadMessage(Network.MessageType.Regular, entry.Key.HomeFaction.Definition.HuePrimary, false, String.Format("[{0}/{1}] {2}", noContr, totalBraziers, entry.Value));

                m_Town.CaptureBrazier.PublicOverheadMessage(Network.MessageType.Regular, entry.Key.HomeFaction.Definition.HuePrimary, false, String.Format("[{0}/{1}] {2}", noContr, totalBraziers, entry.Value));
            }

            if (controller != null && m_Town.CaptureBrazier.CapTown != null)
                m_Town.CaptureBrazier.PublicOverheadMessage(Network.MessageType.Regular, m_Town.CaptureBrazier.CapTown.HomeFaction.Definition.HuePrimary, false, controller.ToString());

            AutoDouseBraziers();

            m_Ticks++;
            if (m_Ticks > 5)
                AwardKeys(controller);
        }
    }
}
