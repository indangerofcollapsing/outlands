using Server.Engines.XmlSpawner2;
using Server.Guilds;
using Server.Gumps;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Guilds
{
    public static class Abilities
    {

        public static void Initialize()
        {
            EventSink.Speech += new SpeechEventHandler(EventSink_Speech);
        }

        private static void EventSink_Speech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;
            int[] keywords = e.Keywords;

            if (from.Guild == null || !(from.Guild is Guild))
                return;

            var playerGuild = from.Guild as Guild;

            string text = e.Speech.Trim().ToLower();

            if (text.IndexOf("vas an corp") != -1) // area res
            {
                AreaResurrect(from);
            }
            else if (text.IndexOf("an corp") != -1) // self/target res
            {
                if (from.Alive)
                {
                    from.Target = new ResurrectionTarget();
                }
                else
                {
                    SelfResurrect(from);
                }
            }
            else if (text.IndexOf("kal vas xen") != -1) // challenge dungeon boss
            {
                if (!playerGuild.CanUseAbility(from, GuildBonus.SequentialRestart))
                {
                    from.SendMessage("You cannot use this ability yet");
                    return;
                }

                from.Target = new ChallengeTarget();
                from.SendMessage("Target the altar you wish to challenge!");
            }
        }
        private class ChallengeTarget : Target
        {
            public ChallengeTarget()
                : base(8, false, TargetFlags.None)
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				ChallengeDungeon( from, targeted );
			}
        }

        private static void ChallengeDungeon(Mobile from, object targeted)
        {
            if (targeted is Item && from.Guild != null && from.Guild is Guild)
            {
                Item target = targeted as Item;
                XmlUse use = XmlAttach.FindAttachment(target, typeof(XmlUse)) as XmlUse;
                var guild = from.Guild as Guild;
                if (use != null && guild.CanUseAbility(from, GuildBonus.SequentialRestart))
                {
                    use.m_EndTime = DateTime.UtcNow - TimeSpan.FromMinutes(1); // reset cooldown
                    use.ExecuteActions(from, targeted, use.SuccessAction);
                    guild.AbilityUsed(from, GuildBonus.SequentialRestart);
                }
            }
        }

        private static void SelfResurrect(Mobile from)
        {
            if (!from.Alive)
            {
                var player = from as PlayerMobile;
                if (player == null) return;
                var guild = player.Guild as Guild;
                if (guild == null) return;

                if (BaseHouse.FindHouseAt(from) != null)
                {
                    from.SendMessage("You may not resurrect inside a house.");
                    return;
                }

                if (player.MurdererDeathGumpNeeded == false && player.RestitutionFee == 0 && guild.CanUseAbility(from, GuildBonus.SelfRez))
                {
                    from.CloseGump(typeof(ResurrectGump));
                    from.SendGump(new ResurrectGump(from));
                    guild.AbilityUsed(from, GuildBonus.SelfRez);
                }
            }
        }

        private static void Resurrect(Mobile from, object target)
        {
            if (from.Guild == null || !(from.Guild is Guild))
                return;

            if (!(target is PlayerMobile))
            {
                from.SendMessage("you cannot resurrect that!");
                return;
            }
            var targetPlayer = target as PlayerMobile;
            var guild = from.Guild as Guild;

            if (from.Guild == targetPlayer.Guild && !targetPlayer.Alive && from.Map.LineOfSight(from, targetPlayer))
            {
                if (targetPlayer.RestitutionFee > 0 || targetPlayer.MurdererDeathGumpNeeded)
                {
                    from.SendMessage("That player may not be ressurrected while they have unpaid restitution fees.");
                    return;
                }

                if (BaseHouse.FindHouseAt(targetPlayer) != null)
                {
                    from.SendMessage("You may not resurrect inside a house.");
                    return;
                }

                if (guild.CanUseAbility(from, GuildBonus.RezGuildmate))
                {
                    targetPlayer.CloseGump(typeof(ResurrectGump));
                    targetPlayer.SendGump(new ResurrectGump(targetPlayer));
                    guild.AbilityUsed(from, GuildBonus.RezGuildmate);
                }
                else
                {
                    from.SendMessage("You cannot use that ability yet.");
                }
            }
        }

        private static void AreaResurrect(Mobile from)
        {
            var guild = from.Guild as Guild;
            if (guild == null) return;

            if (!guild.CanUseAbility(from, GuildBonus.AoeRez))
            {
                from.SendMessage("You cannot use that ability yet.");
                return;
            }

            var guildmates = new List<PlayerMobile>();

            IPooledEnumerable eable = from.Map.GetMobilesInRange(from.Location, 4);

            foreach (Mobile mobile in eable)
            {
                if (!(mobile is PlayerMobile) || mobile.Guild != guild || mobile.Alive || !from.Map.LineOfSight(from, mobile))
                    continue;
                guildmates.Add(mobile as PlayerMobile);
            }
            eable.Free();

            int maximum = 4;
            int count = 0;
            foreach (var guildmate in guildmates)
            {
                if (guildmate == null || guildmate.RestitutionFee > 0 || guildmate.MurdererDeathGumpNeeded || count > maximum)
                    continue;

                if (BaseHouse.FindHouseAt(guildmate) != null)
                    continue;

                guildmate.CloseGump(typeof(ResurrectGump));
                guildmate.SendGump(new ResurrectGump(from));
                count++;
            }

            guild.AbilityUsed(from, GuildBonus.AoeRez);
        }

        private class ResurrectionTarget : Target
        {
            public ResurrectionTarget()
                : base(8, false, TargetFlags.None)
			{
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				Resurrect( from, targeted );
			}
        }
    }
}
