using Server.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Games
{
    public class LavaCTF : RandomTeamCTFBattleground
    {
        public override int MinimumPlayers { get { return 12; } } // 12
        public override int MaximumPlayers { get { return 32; } } // 32
        public override int ScoreRequiredToWin { get { return 8; } } // 8
        
        public LavaCTF()
            : base()
        {
            Name = "Volcano";
            Teams = new List<Team>()
            {
                new Team("Gold", new Point3D(681, 1356, 25), GoldColor, new Point3D(688, 1363, 0), Map, GoldFlagCarrierColor),
                new Team("Blue", new Point3D(724, 1356, 25), BlueColor, new Point3D(717, 1363, 0), Map, BlueFlagCarrierColor),
                new Team("Red", new Point3D(724, 1399, 25), RedColor, new Point3D(717, 1392, 0), Map, RedFlagCarrierColor),
                new Team("Green", new Point3D(681, 1399, 25), GreenColor, new Point3D(688, 1392, 0), Map, GreenFlagCarrierColor),
            };
            SpectatePoint = new Point3D(704, 1378, 35);
        }

        protected override void AwardWinners(Team winners)
        {
            base.AwardWinners(winners);
            foreach (var player in winners.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_VolcanoBattleground);
            }
        }
    }

    public class StarryCTF : RandomTeamCTFBattleground
    {
        public override int MinimumPlayers { get { return 6; } } // 6
        public override int MaximumPlayers { get { return 12; } } // 12
        public override int ScoreRequiredToWin { get { return 8; } } // 8

        public StarryCTF()
            : base()
        {
            Name = "Starry Expanse";
            Teams = new List<Team>()
            {
                new Team("Gold", new Point3D(882, 1407, 0), GoldColor, new Point3D(877, 1405, 0), Map, GoldFlagCarrierColor),
                new Team("Green", new Point3D(910, 1380, 0), GreenColor, new Point3D(915, 1382, 0), Map, GreenFlagCarrierColor),
            };
            SpectatePoint = new Point3D(896, 1393, 0);
        }

        protected override void AwardWinners(Team winners)
        {
            base.AwardWinners(winners);
            foreach (var player in winners.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_StarryExpanseBattleground);
            }
        }

    }

    public class MushroomCTF : PickedTeamCTFBattleground
    {
        public override int MinimumPlayersPerTeam { get { return 3; } } // 3
        public override int ScoreRequiredToWin { get { return 5; } } // 5

        public override int MaximumPlayers { get { return 32; } }

        public MushroomCTF()
            : base()
        {
            Name = "Fungal Fancies";
            Teams = new List<Team>()
            {
                new Team("Gold", new Point3D(779, 1352, 55), GoldColor, new Point3D(781, 1363, 60), Map, GoldFlagCarrierColor),
                new Team("Blue", new Point3D(815, 1331, 55), BlueColor, new Point3D(804, 1333, 60), Map, BlueFlagCarrierColor),
                new Team("Red", new Point3D(835, 1367, 25), RedColor, new Point3D(833, 1356, 30), Map, RedFlagCarrierColor),
                new Team("Green", new Point3D(800, 1387, 25), GreenColor, new Point3D(811, 1385, 30), Map, GreenFlagCarrierColor),
            };
            SpectatePoint = new Point3D(800, 1361, 40);
        }

        protected override void AwardWinners(Team winners)
        {
            base.AwardWinners(winners);
            foreach (var player in winners.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_FungalBattleground);
            }
        }
    }

    public class JungleCTF : PickedTeamCTFBattleground
    {
        public override int MinimumPlayersPerTeam { get { return 3; } }
        public override int ScoreRequiredToWin { get { return 5; } } // 5
        public override int MaximumPlayers { get { return 20; } }

        public JungleCTF()
            : base()
        {
            Name = "Oasis";
            Teams = new List<Team>()
            {
                new Team("Blue", new Point3D(937, 1264, 5), BlueColor, new Point3D(935, 1268, 66), Map, BlueFlagCarrierColor),
                new Team("Red", new Point3D(937, 1309, 5), RedColor, new Point3D(935, 1307, 65), Map, RedFlagCarrierColor),
            };
            SpectatePoint = new Point3D(924, 1286, 5);
        }

        protected override void AwardWinners(Team winners)
        {
            base.AwardWinners(winners);
            foreach (var player in winners.Players)
            {
                if (player.NetState != null)
                    AchievementSystem.Instance.TickProgress(player, AchievementTriggers.Trigger_OasisBattleground);
            }
        }

    }
}
