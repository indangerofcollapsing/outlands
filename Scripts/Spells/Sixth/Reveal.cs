using System;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Sixth
{
    public class RevealSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Reveal", "Wis Quas",
                206,
                9002,
                Reagent.Bloodmoss,
                Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

        public RevealSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {   
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)                
                    this.Target(casterCreature.SpellTarget);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                List<Mobile> targets = new List<Mobile>();

                Map map = Caster.Map;

                if (map != null)
                {                    
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 1 + (int)(Caster.Skills[SkillName.Magery].Value / 50.0));

                    foreach (Mobile m in eable)
                    {
                        if (m.Hidden && !m.RevealImmune && (m.AccessLevel == AccessLevel.Player || Caster.AccessLevel > m.AccessLevel) && Caster.CanBeHarmful(m, false, true))
                            targets.Add(m);
                    }

                    eable.Free();
                }

                int spellHue = 0; //PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Reveal);

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];

                    m.RevealingAction();

                    m.FixedParticles(0x375A, 9, 20, 5049, spellHue, 0, EffectLayer.Head);
                    m.PlaySound(0x1FD);
                }
            }

            FinishSequence();
        }        

        public class InternalTarget : Target
        {
            private RevealSpell m_Owner;

            public InternalTarget(RevealSpell owner): base(12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}