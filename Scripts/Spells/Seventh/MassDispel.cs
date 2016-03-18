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

        public MassDispelSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            if (PlayerMobile.CheckAccountForStatloss(Caster))
            {
                Caster.SendMessage("You are not allowed to cast that spell while there is a character with temporary statloss active on your account.");
                return;
            }

            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)
                {
                    this.Target(casterCreature.SpellTarget, casterCreature);
                }
            }

            else
            {
                Caster.Target = new InternalTarget(this);
            }
        }

        public void Target(IPoint3D p, Mobile from)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
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

                    //This check will make Blue turn orange if he/she tries to attack/dispel enemy OCB player's summoned creature, except EV and BS.
                    if (from is PlayerMobile && bc_Creature.SummonMaster != null && bc_Creature.SummonMaster is PlayerMobile && !(bc_Creature is EnergyVortex) && !(bc_Creature is BladeSpirits))
                    {
                        var caster = from as PlayerMobile;
                        var summonMaster = bc_Creature.SummonMaster as PlayerMobile;

                        if (!caster.Aggressors.Select(x => x.Attacker).Contains(summonMaster)//Make sure that this is not an act of self-defense (check if summonmaster is in caster aggressor list)
                            && !caster.IsInMilitia //The blue player is not in the militita
                            && summonMaster.IsInMilitia //This player is an OCB
                            && (caster.Citizenship == null || caster.Citizenship != summonMaster.Citizenship) // and they are from different faction (enemies)
                          )
                        {
                            caster.AssistedOwnMilitia = true;
                        }
                    }
                   
                    Caster.DoHarmful(m);

                    if (Caster is PlayerMobile)
                    {
                        if (Utility.RandomDouble() < 0.2)
                            m.FixedEffect(0x3779, 10, 20, spellHue, 0);

                        else                            
                            bc_Creature.ResolveDispel(Caster, true, spellHue);                            
                    }

                    else
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