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
	public class MassCurseSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Mass Curse", "Vas Des Sanct",
				218,
				9031,
				false,
				Reagent.Garlic,
				Reagent.Nightshade,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public MassCurseSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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

		public void Target( IPoint3D p )
		{
			if ( !Caster.CanSee( p ) )			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			
			else if ( SpellHelper.CheckTown( p, Caster ) && CheckSequence() )
			{
				SpellHelper.Turn( Caster, p );
				SpellHelper.GetSurfaceTop( ref p );

				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
				{
					IPooledEnumerable eable = map.GetMobilesInRange( new Point3D( p ), 3 );

					foreach ( Mobile m in eable )
					{
						if ( SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanSee( m ) && Caster.CanBeHarmful( m, false ) )
							targets.Add( m );
					}

					eable.Free();
				}

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.MassCurse);

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile mobile = targets[i];

                    CheckMagicResist(mobile);                   

					Caster.DoHarmful( mobile );
                   
					SpellHelper.AddStatCurse( Caster, mobile, StatType.Str );
					SpellHelper.AddStatCurse( Caster, mobile, StatType.Dex );
					SpellHelper.AddStatCurse( Caster, mobile, StatType.Int );

                    if (mobile.Spell != null)
                        mobile.Spell.OnCasterHurt();

                    mobile.Paralyzed = false;

                    mobile.FixedParticles(0x374A, 10, 15, 5028, spellHue, 0, EffectLayer.Waist);
					mobile.PlaySound( 0x1FB );
				}
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private MassCurseSpell m_Owner;

			public InternalTarget( MassCurseSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				IPoint3D p = o as IPoint3D;

				if ( p != null )
					m_Owner.Target( p );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}