using Server.Custom.Battlegrounds.Items;
using Server.Custom.Battlegrounds.Mobiles;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds
{
    public class SiegeBattleground : Battleground
    {
        public Point3D KingLocation { get; protected set; }

        public BattlegroundKing King { get; set; }

        public List<Barrier> Barriers { get; protected set; }

        public List<SiegeWeaponSpawner> SiegeSpawners { get; protected set; }
        public List<ISiegeable> SiegeItems { get; protected set; }

        public Team Offense { get; protected set; }
        public Team Defense { get; protected set; }
        public SiegeAutobalancer Autobalancer { get; protected set; }
        public SiegeBattlegroundPersistence Persistence { get; set; }

        private static TimeSpan TeamSwitchCooldown = TimeSpan.FromMinutes(5);

        public SiegeBattleground()
            : base()
        {
            Barriers = new List<Barrier>();
            SiegeSpawners = new List<SiegeWeaponSpawner>();
            SiegeItems = new List<ISiegeable>();
        }

        protected override void Start()
        {
            base.Start();

            if (Persistence != null && !Persistence.Deleted)
                Persistence.Delete();

            Persistence = new SiegeBattlegroundPersistence(this);

            EnableBarriers();
            SpawnKing();
            RepairOrDeleteItems();
            foreach (var siegeSpawner in SiegeSpawners)
            {
                if (siegeSpawner != null && !siegeSpawner.Deleted)
                    siegeSpawner.Respawn();
            }
        }

        protected virtual void SpawnKing()
        {
        }

        protected override void StartTimers()
        {
            base.StartTimers();

            if (Autobalancer != null && Autobalancer.Running)
                Autobalancer.Stop();

            Autobalancer = new SiegeAutobalancer(this);
            Autobalancer.Start();
        }

        protected override void StopTimers()
        {
            base.StopTimers();

            if (Autobalancer != null && Autobalancer.Running)
                Autobalancer.Stop();
        }

        public override void EnterBattleground(PlayerMobile player)
        {
            base.EnterBattleground(player);

            Point3D spawn;
            if (Offense.Count >= Defense.Count)
            {
                CurrentScoreboard.AddPlayer(player, Defense.Color);
                Defense.Add(player);
                Defense.AddRobe(player);
                spawn = Defense.SpawnPoint;
            }
            else
            {
                CurrentScoreboard.AddPlayer(player, Offense.Color);
                Offense.Add(player);
                Offense.AddRobe(player);
                spawn = Offense.SpawnPoint;
            }
            player.LastLocation = player.Location;
            player.MoveToWorld(spawn, Map);
        }

        protected virtual void DeleteKing()
        {
            if (King != null && !King.Deleted)
                King.Delete();
        }

        public virtual void BalanceTeams(int difference)
        {
            int toMove = difference / 2;
            for (int i = 0; i < toMove; i++)
            {
                if (Offense.Count > Defense.Count)
                    SwitchTeams(Offense.Players.Last());
                else
                    SwitchTeams(Defense.Players.Last());

            }
            Broadcast(1153, "The teams have been autobalanced.");
        }

        protected virtual void SwitchTeams(PlayerMobile player)
        {
            if (Offense.Contains(player))
            {
                Offense.Remove(player);
                Offense.RemoveRobe(player);

                Defense.Add(player);
                Defense.AddRobe(player);
                CurrentScoreboard.ChangeTeams(player, Defense.Color);
            }
            else
            {
                Defense.Remove(player);
                Defense.RemoveRobe(player);

                Offense.Add(player);
                Offense.AddRobe(player);
                CurrentScoreboard.ChangeTeams(player, Offense.Color);
            }
            MoveToSpawn(player);
            player.SendMessage("You have switched teams");
            player.LastTeamSwitch = DateTime.UtcNow;
        }

        public bool TrySwitchTeams(PlayerMobile player)
        {
            if ((player.LastTeamSwitch + TeamSwitchCooldown > DateTime.UtcNow) || player.IsDragging)
                return false;

            if (Offense.Contains(player))
            {
                if (Defense.Count > Offense.Count)
                    return false;
                else
                {
                    SwitchTeams(player);
                    return true;
                }
            }
            else if (Defense.Contains(player))
            {
                if (Offense.Count > Defense.Count)
                    return false;
                else
                {
                    SwitchTeams(player);
                    return true;
                }
            }

            return false;
        }

        public virtual void OnKingsDeath()
        {
            // offense wins
            AwardWinners(Offense);
            Broadcast(Offense.Color, "Offense has won!");
            CurrentScoreboard.Winner = Offense;
            Finish();
        }

        public override void OnTimeout()
        {
            // defense wins
            AwardWinners(Defense);
            Broadcast(Defense.Color, "Defense has won!");
            CurrentScoreboard.Winner = Defense;
            Finish();
        }

        protected override void CleanUp()
        {
            base.CleanUp();
            DeleteKing();

            if (Persistence != null && !Persistence.Deleted)
                Persistence.Delete();

            RepairOrDeleteItems();
        }

        protected virtual void EnableBarriers()
        {
            Timer.DelayCall(GameStartGracePeriod, DisableBarriers);
        }

        protected virtual void DisableBarriers()
        {
            Broadcast(1153, "The battle has begun!");
            Barriers.ForEach((Barrier barrier) => { barrier.Delete(); });
            Barriers.Clear();
        }

        protected virtual void RepairOrDeleteItems()
        {
            foreach (var item in SiegeItems)
            {
                if (item != null && !item.Deleted)
                    item.RepairOrDelete();
            }
        }
    }
}
