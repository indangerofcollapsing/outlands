using System;
using Server.Targeting;
using Server.Network;
using Server.Gumps;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Eighth
{
    public class ResurrectionSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Resurrection", "An Corp",
                245,
                9062,
                Reagent.Bloodmoss,
                Reagent.Garlic,
                Reagent.Ginseng
            );

        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }


        public override TimeSpan GetCastDelay()
        {
            return TimeSpan.FromSeconds(5.0);
        }


        public ResurrectionSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(Caster))
            {
                Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            bool validTarget = true;

            PlayerMobile playerTarget = m as PlayerMobile;

            if (!Caster.CanSee(m) || m.Hidden)
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
                validTarget = false;
            }

            else if (m == Caster)
            {
                Caster.SendLocalizedMessage(501039); // Thou can not resurrect thyself.
                validTarget = false;
            }

            else if (!Caster.Alive)
            {
                Caster.SendLocalizedMessage(501040); // The resurrecter must be alive.
                validTarget = false;
            }

            else if (m.Alive)
            {
                Caster.SendLocalizedMessage(501041); // Target is not dead.
                validTarget = false;
            }

            else if (!Caster.InRange(m, 1))
            {
                Caster.SendLocalizedMessage(501042); // Target is not close enough.
                validTarget = false;
            }

            else if (!m.Player)
            {
                Caster.SendLocalizedMessage(501043); // Target is not a being.
                validTarget = false;
            }   

            else if (m.Map == null || !m.Map.CanFit(m.Location, 16, false, false))
            {
                Caster.SendLocalizedMessage(501042); // Target can not be resurrected at that location.
                m.SendLocalizedMessage(502391); // Thou can not be resurrected there!
                validTarget = false;
            }

            if (playerTarget != null)
            {
                if (playerTarget.RestitutionFee > 0 || playerTarget.MurdererDeathGumpNeeded)
                {
                    Caster.SendMessage("That player may not be ressurrected while they have unpaid restitution fees.");
                    validTarget = false;
                }                
            }

            if (validTarget)
            {
                if (CheckBSequence(m, true))
                {
                    SpellHelper.Turn(Caster, m);

                    int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.WaterElemental);

                    m.PlaySound(0x214);
                    m.FixedEffect(0x376A, 10, 16, spellHue, 0);

                    m.SendGump(new ResurrectGump(m, Caster));
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private ResurrectionSpell m_Owner;

            public InternalTarget(ResurrectionSpell owner)
                : base(1, false, TargetFlags.Beneficial)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}