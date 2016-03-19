using Server.Achievements;
using Server.Custom.Battlegrounds.Items;
using Server.Custom.Battlegrounds.Mobiles;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Games
{
    public class ThroneSiegeBattleground : SiegeBattleground
    {
        public override int MaximumPlayers { get { return 20; } }
        public override int MinimumPlayers { get { return 8; } }

        public ThroneSiegeBattleground()
            : base()
        {
            Offense = new Team("Offense", new Point3D(414, 138, 0), GoldColor);
            Defense = new Team("Defense", new Point3D(344, 138, 1), GreenColor);
            
            Teams = new List<Team>()
            {
                Offense,
                Defense,
            };

            Name = "Throne Siege";
            Map = Map.TerMur;
            KingLocation = new Point3D(327, 159, 20);
            SpectatePoint = new Point3D(388, 158, 0);
        }

        protected override void SpawnKing()
        {
            base.SpawnKing();
            King = new ThroneRoomKing();
            King.MoveToWorld(KingLocation, Map);
        }

        protected override void EnableBarriers()
        {
            base.EnableBarriers();

            var points = new List<Point3D>()
            {
                // offense
                new Point3D(419, 149, 0),
                new Point3D(420, 149, 0),
                // defense
                new Point3D(340, 140, 0),
                new Point3D(341, 140, 0),
                new Point3D(348, 140, 0),
                new Point3D(449, 140, 0),

            };

            foreach (var point in points)
            {
                var barrier = new Barrier();
                barrier.MoveToWorld(point, Map);
                Barriers.Add(barrier);
            }
        }

        public override void OnKingsDeath()
        {
            foreach (var player in Offense.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_TreasuryBattleground);
            }
            base.OnKingsDeath();
        }

        public override void OnTimeout()
        {
            foreach (var player in Defense.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_TreasuryBattleground);
            }
            base.OnTimeout();
        }
    }
}
