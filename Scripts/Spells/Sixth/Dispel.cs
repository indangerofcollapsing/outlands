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
            if (PlayerMobile.CheckAccountForStatloss(Caster))
            {
                Caster.SendMessage("You are not allowed to cast that spell while there is a character with temporary statloss active on your account.");
                return;
            }

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

                //This check will make Blue turn orange if he/she tries to attack/dispel enemy OCB player's summoned creature, except EV and BS.
                if (Caster is PlayerMobile && bc_Creature.SummonMaster != null && bc_Creature.SummonMaster is PlayerMobile && !(bc_Creature is EnergyVortex) && !(bc_Creature is BladeSpirits))
                {
                    var caster = Caster as PlayerMobile;
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