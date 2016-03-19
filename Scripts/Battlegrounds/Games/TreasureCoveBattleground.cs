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
    public class TreasureCoveBattleground : SiegeBattleground
    {

        public override int MaximumPlayers { get { return 20; } } // 20
        public override int MinimumPlayers { get { return 8; } } // 8

        public TreasureCoveBattleground()
            : base()
        {
            Defense = new Team("Defense", new Point3D(318, 474, -2), BlueColor);
            Offense = new Team("Offense", new Point3D(207, 493, -5), RedColor);
            Teams = new List<Team>()
            {
               Offense,
               Defense,
            };

            Name = "Treasure Cove";
            Map = Map.Tokuno;

            KingLocation = new Point3D(288, 464, 47);
            SpectatePoint = new Point3D(243, 475, 2);
        }
        protected override void SpawnKing()
        {
            base.SpawnKing();
            King = new TreasureCoveKing();
            King.MoveToWorld(KingLocation, Map);
        }

        protected override void EnableBarriers()
        {
            base.EnableBarriers();
            var points = new List<Point3D>()
            {
                //offense
                new Point3D(251, 472, -1),
                new Point3D(251, 473, 0),
                new Point3D(251, 474, 0),
                new Point3D(251, 475, 0),
                new Point3D(251, 476, 0),
                new Point3D(251, 477, 0),
                new Point3D(251, 478, 0),
                new Point3D(251, 479, -3),

                // defense
                new Point3D(207, 463, 2),
                new Point3D(208, 463, 2),
                new Point3D(209, 463, 2),
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
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_CaveLakeBattleground);
            }
            base.OnKingsDeath();
        }

        public override void OnTimeout()
        {
            foreach (var player in Defense.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_CaveLakeBattleground);
            }
            base.OnTimeout();
        }
    }
}
