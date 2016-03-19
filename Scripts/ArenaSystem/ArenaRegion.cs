using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Spells;
using Server.Items;
using System.Xml;

namespace Server.ArenaSystem
{
    /// <summary>
    /// Encapsulates the match area with an arena. Intercepts important events and propogates them to
    /// the Arena's ruleset or restriction.
    /// </summary>
    public class ArenaCombatRegion : Region
    {
        public Arena Arena { get; set; }

        public ArenaCombatRegion(Arena arena, string name, Map map, Rectangle3D area)
            : base(name, map, 2000, area)
        {
            Arena = arena;
        }
        public override bool AllowHousing(Mobile from, Point3D p)
        {
            // Housing is never allowed.
            return false;
        }
        public override void OnSpeech(SpeechEventArgs args)
        {
            // Any speech from a player in a match ignores the los requirement. This allows spectators
            // to observe speech occuring in the arena, but not vice-versa.
            args.IgnoreLos = true;
        }
        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = LightCycle.DayLevel;
        }
        public override bool AcceptsSpawnsFrom(Region region)
        {
            return false;
        }
        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (Arena.MatchInProgress != null)
            {
                return Arena.MatchInProgress.Restriction.OnBeginSpellCast(m, s);
            }
            return false;
        }
        public override bool OnSkillUse(Mobile m, int Skill)
        {
            if (Arena.MatchInProgress != null)
            {
                return Arena.MatchInProgress.Restriction.OnSkillUse(m, Skill);
            }
            return false;
        }
        public override void OnDeath(Mobile m)
        {
            if (Arena.MatchInProgress != null)
            {
                Arena.MatchInProgress.Restriction.OnDeath(m);
            }
        }
        public override void OnEnter(Mobile m)
        {
            if (m is PlayerMobile)
            {
                ArenaSystem.VerifyPlayerLocation((PlayerMobile)m);
            }

            if (m.Target != null)
            {
                m.Target.Cancel(m, Targeting.TargetCancelType.Canceled);
                m.ClearTarget();
            }
        }
    }

    public class ArenaSpectatorRegion : Region
    {
        public ArenaSpectatorRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent) 
        {
        }

        public override void MakeGuard(Mobile focus)
        {
            // No action
        }

        public override void AlterLightLevel(Mobile m, ref int global, ref int personal)
        {
            global = LightCycle.DayLevel;
        }

        public override bool AllowBeneficial(Mobile from, Mobile target)
        {
            from.SendMessage("You can not perform this action in an arena zone.");
            return false;
        }

        public override bool AllowHarmful(Mobile from, Mobile target)
        {
            from.SendMessage("You can not perform this action in an arena zone.");
            return false;
        }

        public override bool OnSkillUse(Mobile from, int Skill)
        {
            from.SendMessage("You can not perform this action in an arena zone.");
            return false;
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (!(s is ArenaTransportSpell))
            {
                m.SendMessage("You can not perform this action in an arena zone.");
                return false;
            }
            return true;
        }

        public override void OnEnter(Mobile m)
        {
            if (m.Target != null)
            {
                m.Target.Cancel(m, Targeting.TargetCancelType.Canceled);
                m.ClearTarget();
            }
            if (m is PlayerMobile && !(m.Region is ArenaCombatRegion) && m.AccessLevel == AccessLevel.Player)
            {
                //m.SendMessage("Your abilities fade away as you enter an arena zone!");
                Effects.SendLocationParticles(m, 0x376A, 9, 10, 9502);
            }

            // hue any equipped special arena items if an arena reward talisman is equipped
            Item italisman = m.FindItemOnLayer(Layer.Talisman);
            ArenaRewardTotem arena_talisman = italisman as ArenaRewardTotem;
            if (arena_talisman != null)
            {
                foreach (Item i in m.Items)
                    arena_talisman.HueItem(i);
            }
        }

        public static bool TryEnter(Mobile m)
        {
            PlayerMobile player = m as PlayerMobile;
            if (player == null)
                return true;

            if (player.Criminal)
            {
                player.SendMessage(0x22, "Thou'rt a criminal and are not allowed into the arena area at the moment.");
                return false;
            }
            else if (SpellHelper.CheckCombat(player, true))
            {
                player.SendMessage(0x22, "You have recently been in combat and are not allowed into the arena area at the moment.");
                return false;
            }
            return true;
        }

        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            // pretty much the same checks as for recall/gate travel
            return TryEnter(m);
        }
        public override bool OnDoubleClick(Mobile m, object o)
        {
            if (o is BasePotion)
                return false;
            return true;
        }

        public override void OnExit(Mobile m)
        {
            if (m is PlayerMobile && m.AccessLevel == AccessLevel.Player)
            {
                Region region = Region.Find(m.Location, m.Map);
                if (!(region is ArenaCombatRegion))
                {
                    //m.SendMessage("Your abilities are restored as you exit an arena zone!");
                    Effects.SendLocationParticles(m, 0x376A, 9, 10, 9502);

                    foreach (Item i in m.Items)
                        i.Hue = i.OriginalHue;
                }
            }
        }
        public override bool AllowHousing(Mobile from, Point3D p)
        {
            // Housing is never allowed.
            return false;
        }
        public override bool AcceptsSpawnsFrom(Region region)
        {
            return false;
        }
    }
}
