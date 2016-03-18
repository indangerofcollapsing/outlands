using Server.Achievements;
using Server.Custom.Battlegrounds.Items;
using Server.Custom.Battlegrounds.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Games
{
    public class SanctuaryBattleground : SiegeBattleground
    {
        public override int MaximumPlayers { get { return 20; } }
        public override int MinimumPlayers { get { return 8; } }

        public SanctuaryBattleground()
            : base()
        {
            Offense = new Team("Offense", new Point3D(6244, 6, 15), GoldColor);
            Defense = new Team("Defense", new Point3D(6228, 68, 0), RedColor);
            
            Teams = new List<Team>()
            {
                Offense,
                Defense,
            };

            Name = "Sanctuary";
            KingLocation = new Point3D(6269, 132, 2);
            SpectatePoint = new Point3D(6266, 65, -10);
        }

        protected override void SpawnKing()
        {
            base.SpawnKing();
            King = new SanctuaryKing();
            King.MoveToWorld(KingLocation, Map);
        }

        protected override void EnableBarriers()
        {
            base.EnableBarriers();

            var points = new List<Point3D>()
            {
                // defense
                new Point3D(6224, 70, 0),
                new Point3D(6225, 70, 0),
                new Point3D(6226, 70, 0),
                new Point3D(6227, 70, 0),
                new Point3D(6228, 70, 0),
                new Point3D(6229, 70, 0),
                new Point3D(6230, 70, 0),

                new Point3D(6230, 68, 0),
                new Point3D(6230, 67, 0),
                new Point3D(6230, 66, 0),
                new Point3D(6230, 65, 0),
                new Point3D(6230, 64, 0),
                new Point3D(6230, 63, 0),

                // offense
                new Point3D(6247, 6, 10),
                new Point3D(6243, 7, 15),
                new Point3D(6244, 7, 15),
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
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_SanctuaryBattleground);
            }
            base.OnKingsDeath();
        }

        public override void OnTimeout()
        {
            foreach (var player in Defense.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_SanctuaryBattleground);
            }
            base.OnTimeout();
        }
    }
}
