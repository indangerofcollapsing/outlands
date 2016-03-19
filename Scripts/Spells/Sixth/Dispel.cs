using System;
using System.Linq;
using Server.Misc;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Sixth
{
    public class DispelSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Dispel", "An Ort",
                218,
                9002,
                Reagent.Garlic,
                Reagent.MandrakeRoot,
                Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

        public DispelSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
        {
        }

        public override bool DelayedDamage { get { return true; } }

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

        public void Target(Mobile mobile)
        {
            BaseCreature bc_Creature = mobile as BaseCreature;

            if (!Caster.CanSee(mobile) || mobile.Hidden)
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (bc_Creature == null || !bc_Creature.IsDispellable)
                Caster.SendLocalizedMessage(1005049); // That cannot be dispelled.            

            else if (CheckHSequence(mobile))
            {
                SpellHelper.Turn(Caster, mobile);
                
                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Dispel);

                Caster.DoHarmful(bc_Creature);

                if (Caster is PlayerMobile)
                    bc_Creature.ResolveDispel(Caster, true, spellHue);

                else
                    bc_Creature.ResolveDispel(Caster, false, spellHue);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private DispelSpell m_Owner;

            public InternalTarget(DispelSpell owner): base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}