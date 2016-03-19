using Server.Custom.Battlegrounds.Regions;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    public class CTFFlag : Item
    {
        private class DamageIncreaseTimer : Timer
        {
            CTFFlag m_Flag;
            public DamageIncreaseTimer(CTFFlag flag)
                : base(TimeSpan.FromMinutes(3))
            {
                Priority = TimerPriority.FiveSeconds;
                m_Flag = flag;
            }

            protected override void OnTick()
            {
                if (m_Flag != null && !m_Flag.Deleted)
                {
                    m_Flag.CarrierIsVulnerable = true;
                    if (m_Flag.Carrier != null)
                    {
                        // flag carries is now vulnerable
                        var bg = m_Flag.Carrier.Region as BattlegroundRegion;
                        if (bg != null)
                        {
                            var team = bg.Battleground.Teams.Find(t => t.Contains(m_Flag.Carrier));
                            bg.Battleground.FeedBroadcast(string.Format("The {0} team flag carrier has become more vulnerable to damage!", m_Flag.Team.Name), hue: team.Color);
                            m_Flag.Carrier.DamageVulnerable = true;
                        }
                    }
                }
                else
                {
                    Stop();
                }
            }
        }

        private class PassTarget : Target
        {
            CTFFlag m_Flag;
            public PassTarget(CTFFlag flag)
                : base(2, true, TargetFlags.None, true)
            {
                m_Flag = flag;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is PlayerMobile && from is PlayerMobile)
                {
                    var targetPlayer = targeted as PlayerMobile;
                    var fromPlayer = from as PlayerMobile;

                    if (m_Flag.CanPassTo(fromPlayer, targetPlayer))
                    {
                        if (!targetPlayer.PlaceInBackpack(m_Flag))
                        {
                            from.SendMessage("Their backpack is full.");
                        }
                        else
                        {
                            var bg = ((CTFBattlegroundRegion)targetPlayer.Region).Battleground as CTFBattleground;
                            m_Flag.Carrier = targetPlayer;
                            bg.OnFlagPickedUp(targetPlayer, m_Flag);
                        }
                    }
                    else
                    {
                        from.SendMessage("You can only pass to teammates.");
                    }
                }
                else if (targeted is StaticTarget)
                {
                    m_Flag.MoveToWorld(((StaticTarget)targeted).Location);
                }
                else
                {
                    from.SendMessage("You can't pass the flag to that!");
                }
            }
        }

        public static bool ExistsOn(Mobile mob)
        {
            Container pack = mob.Backpack;

            return (pack != null && pack.FindItemByType(typeof(CTFFlag)) != null);
        }

        public Team Team { get; private set; }
        public PlayerMobile Carrier { get; private set; }
        private DamageIncreaseTimer CaptureTimer { get; set; }

        public bool CarrierIsVulnerable = false;

        public CTFFlag(int itemId, Team team)
            : base(itemId)
        {
            Hue = team.Color;
            LootType = LootType.Newbied;
            Team = team;
            Name = string.Format("{0}'s Flag", team.Name);
        }

        private void StartCaptureTimer()
        {
            if (CaptureTimer != null && CaptureTimer.Running)
            {
                return;
            }

            CaptureTimer = new DamageIncreaseTimer(this);
            CaptureTimer.Start();
        }

        private void StopCaptureTimer()
        {
            if (CaptureTimer != null)
                CaptureTimer.Stop();
        }

        public CTFFlag(Serial serial)
            : base(serial)
        {

        }

        public override bool OnDragLift(Mobile from)
        {
            if (Carrier != null && Carrier == from)
            {
                from.Target = new PassTarget(this);
            }
            else
            {
                OnDoubleClick(from);
            }
            return false;
        }

        private Mobile FindOwner(object parent)
        {
            if (parent is Item)
                return ((Item)parent).RootParent as Mobile;

            if (parent is Mobile)
                return (Mobile)parent;

            return null;
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            var player = FindOwner(parent) as PlayerMobile;
            Carrier = null;
            if (player != null)
            {
                player.SolidHueOverride = -1;
                player.DamageVulnerable = false;
            }
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            PlayerMobile player = FindOwner(parent) as PlayerMobile;

            if (player != null)
            {
                var bg = ((CTFBattlegroundRegion)player.Region).Battleground as CTFBattleground;
                var team = bg.Teams.Find(t => t.Players.Contains(player));
                player.SolidHueOverride = team.CarrierHue;
                if (CarrierIsVulnerable)
                    player.DamageVulnerable = true;
            }
        }

        public bool CanPassTo(PlayerMobile from, PlayerMobile target)
        {
            if (!(target.Region is CTFBattlegroundRegion))
                return false;
                
            var bg = ((CTFBattlegroundRegion)target.Region).Battleground as CTFBattleground;

            var targetTeam = bg.Teams.Find(t => t.Players.Contains(target));
            var fromTeam = bg.Teams.Find(t => t.Players.Contains(from));
            return targetTeam != null && fromTeam != null &&
                targetTeam == fromTeam;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!from.InRange(this, 1) || !from.InLOS(this))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3b2, 1019045); // I can't reach that
                return;
            }

            var player = from as PlayerMobile;
            var bg = ((CTFBattlegroundRegion)player.Region).Battleground as CTFBattleground;
            var theirTeam = bg.Teams.Find(t => t.Players.Contains(player));
            if (theirTeam == null) return;

            var theirFlag = theirTeam.Flag;

            var carriedFlag = from.Backpack.FindItemByType<CTFFlag>();

            if (carriedFlag != null) 
            {
                // capping a flag
                if (Team == theirTeam && this.Location == Team.FlagLocation) 
                {
                    bg.OnFlagCaptured(player, carriedFlag);
                    carriedFlag.SendHome();
                    StopCaptureTimer();
                }
                else if (Team == theirTeam && Location != Team.FlagLocation)
                {
                    bg.OnFlagReturned(player);
                    SendHome();
                    StopCaptureTimer();
                }
            } 
            else if (Team == theirTeam)
            {
                if (this.Location != Team.FlagLocation)
                {
                    bg.OnFlagReturned(player);
                    SendHome();
                    StopCaptureTimer();
                }
            }
            else if (!from.PlaceInBackpack(this))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x26, false, "I can't hold that.");
            }
            else
            {
                from.RevealingAction();
                bg.OnFlagPickedUp(player, this);
                Carrier = player;
                Carrier.DamageVulnerable = CarrierIsVulnerable;
                StartCaptureTimer();
            }
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { if (!Deleted) MoveToWorld(GetWorldLocation(), Map); });
            return DeathMoveResult.RemainEquiped;
        }

        public void SendHome()
        {
            if (!Deleted)
            {
                CarrierIsVulnerable = false;
                MoveToWorld(Team.FlagLocation, Map);
            }
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { if (!Deleted) MoveToWorld(GetWorldLocation(), Map); });
            return DeathMoveResult.RemainEquiped;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            Delete(); // doesn't need to stick around
        }
    }
}
