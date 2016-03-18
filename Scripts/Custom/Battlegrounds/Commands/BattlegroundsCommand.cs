using Server.Commands;
using Server.Custom.Battlegrounds.Gumps;
using Server.Custom.Battlegrounds.Regions;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Commands
{
    public class BattlegroundsCommand
    {

        public static void Initialize()
        {
            CommandSystem.Register("battlegrounds", AccessLevel.Player, new CommandEventHandler(Battlegrounds_OnCommand));
            CommandSystem.Register("scoreboard", AccessLevel.Player, new CommandEventHandler(Scoreboard_OnCommand));
            CommandSystem.Register("leave", AccessLevel.Player, new CommandEventHandler(Leave_OnCommand));
            CommandSystem.Register("teamswitch", AccessLevel.Player, new CommandEventHandler(TeamSwitch_OnCommand));
            CommandSystem.Register("votekick", AccessLevel.Player, new CommandEventHandler(VoteKick_OnCommand));
            CommandSystem.Register("bgadmin", AccessLevel.Seer, new CommandEventHandler(BGAdmin_OnCommand));
        }

        private static void BGAdmin_OnCommand(CommandEventArgs e)
        {
            e.Mobile.CloseAllGumps();
            e.Mobile.SendGump(new BattlegroundAdminGump());
        }

        private static void VoteKick_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.Region is BattlegroundRegion)
            {
                var pm = e.Mobile as PlayerMobile;
                if (pm == null) return;

                pm.Target = new VoteKickTarget();
                pm.SendMessage("Please select the player you wish to eject from the game.");
            }
        }

        private class VoteKickTarget : Target
        {
            public VoteKickTarget()
                : base(30, false, TargetFlags.None, false)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                base.OnTarget(from, targeted);
                if (targeted is PlayerMobile && from.Region is BattlegroundRegion && from != targeted)
                {
                    Battleground bg = ((BattlegroundRegion)from.Region).Battleground;
                    bg.VoteEject(from as PlayerMobile, targeted as PlayerMobile);
                }
                else
                {
                    from.SendMessage("You cannot vote to eject that!");
                }
            }
        }

        private static void TeamSwitch_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.Region is SiegeBattlegroundRegion)
            {
                SiegeBattleground bg = ((BattlegroundRegion)e.Mobile.Region).Battleground as SiegeBattleground;
                bool result = bg.TrySwitchTeams(e.Mobile as PlayerMobile);
                if (!result)
                    e.Mobile.SendMessage("You are unable to switch teams.");
            }
        }

        private static void Leave_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.Region is BattlegroundRegion)
            {
                Battleground bg = ((BattlegroundRegion)e.Mobile.Region).Battleground;
                bg.Leave(e.Mobile as PlayerMobile);
            }
        }

        private static Scoreboard GetScoreboard(PlayerMobile player)
        {
            Battleground battleground;

            if (player.Region is BattlegroundRegion)
            {
                return ((BattlegroundRegion)player.Region).Battleground.CurrentScoreboard;
            }
            return null;
        }

        private static void Scoreboard_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.HasGump(typeof(ScoreboardGump)))
                e.Mobile.CloseGump(typeof(ScoreboardGump));
            Scoreboard scoreboard = GetScoreboard(e.Mobile as PlayerMobile);
            if (scoreboard == null)
            {
                e.Mobile.SendMessage("You must be in a battleground to view the score.");
                return;
            }
            e.Mobile.SendGump(new ScoreboardGump(e.Mobile, scoreboard));
        }

        private static void Battlegrounds_OnCommand(CommandEventArgs e)
        {
            if (e.Mobile.HasGump(typeof(BattlegroundGump)))
                e.Mobile.CloseGump(typeof(BattlegroundGump));

            var pm = e.Mobile as PlayerMobile;
            if (pm == null) return;

            if (pm.NextBattlegroundTime != DateTime.MinValue && pm.NextBattlegroundTime < DateTime.UtcNow + Battleground.EjectTimeout)
            {
                int diff = (int)(pm.NextBattlegroundTime - DateTime.UtcNow).TotalMinutes;
                pm.SendMessage(string.Format("You must wait {0} minutes before entering another battleground.", diff));
                return;
            }

            e.Mobile.SendGump(new BattlegroundGump(e.Mobile));
        }
    }
}
