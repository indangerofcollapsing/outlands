using System;
using System.Collections.Generic;
using System.Linq;
using Server.Misc;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Seventh
{
    public class MassDispelSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Mass Dispel", "Vas An Ort",
                263,
                9002,
                Reagent.Garlic,
                Reagent.MandrakeRoot,
                Reagent.BlackPearl,
                Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.Seventh; } }

        public MassDispelSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)                
                    this.Target(casterCreature.SpellTarget, casterCreature);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
        }

        public void Target(IPoint3D p, Mobile from)
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
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(p), 8);

                    foreach (Mobile m in eable)
                    {
                        if (m is BaseCreature && ((BaseCreature)m).Summoned && Caster.CanBeHarmful(m, false))
                            targets.Add(m);
                    }

                    eable.Free();
                }

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.MassDispel);

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];
                    BaseCreature bc_Creature = m as BaseCreature;

                    if (bc_Creature == null)
                        continue;
                                       
                    Caster.DoHarmful(m);

                    bc_Creature.ResolveDispel(Caster, false, spellHue);                    
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private MassDispelSpell m_Owner;

            public InternalTarget(MassDispelSpell owner): base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p, from);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}