using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Fourth
{
	public class CurseSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Curse", "Des Sanct",
				227,
				9031,
				Reagent.Nightshade,
				Reagent.Garlic,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

		public CurseSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

		private static Hashtable m_UnderEffect = new Hashtable();

		public static void RemoveEffect( object state )
		{
			Mobile m = (Mobile)state;

			m_UnderEffect.Remove( m );
			m.UpdateResistances();
		}

		public static bool UnderEffect( Mobile m )
		{
			return m_UnderEffect.Contains( m );
		}

        public static void ApplyEffect(Mobile Caster, Mobile mobile)
        {
            SpellHelper.AddStatCurse(Caster, mobile, StatType.Str); SpellHelper.DisableSkillCheck = true;
            SpellHelper.AddStatCurse(Caster, mobile, StatType.Dex);
            SpellHelper.AddStatCurse(Caster, mobile, StatType.Int); SpellHelper.DisableSkillCheck = false;

            Timer t = (Timer)m_UnderEffect[mobile];

            if (Caster.Player && mobile.Player && t == null)
            {
                TimeSpan duration = SpellHelper.GetDuration(Caster, mobile);
                m_UnderEffect[mobile] = t = Timer.DelayCall(duration, new TimerStateCallback(RemoveEffect), mobile);
                mobile.UpdateResistances();
            }

            if (mobile.Spell != null)
                mobile.Spell.OnCasterHurt();

            mobile.Paralyzed = false;

            int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.Curse);            

            mobile.FixedParticles(0x374A, 10, 15, 5028, spellHue, 0, EffectLayer.Waist);
            mobile.PlaySound(0x1E1);
        }

		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			
			else if ( CheckHSequence( mobile ) )
			{
				SpellHelper.Turn( Caster, mobile );
				SpellHelper.CheckReflect( (int)this.Circle, Caster, ref mobile );

                CheckMagicResist(mobile);

                ApplyEffect(Caster, mobile);
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private CurseSpell m_Owner;

			public InternalTarget( CurseSpell owner ) : base(12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}