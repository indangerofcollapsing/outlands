using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Sixth
{
	public class EnergyBoltSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Energy Bolt", "Corp Por",
				230,
				9022,
				Reagent.BlackPearl,
				Reagent.Nightshade
			);

		public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		public EnergyBoltSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
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
        
		public void Target( Mobile mobile )
		{
            if (!Caster.CanSee(mobile) || mobile.Hidden)			
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			
			else if ( CheckHSequence( mobile ) )
            {
                Mobile source = Caster;
                SpellHelper.Turn(Caster, mobile);
                SpellHelper.CheckReflect((int)this.Circle, ref source, ref mobile);

                double damage = (double)Utility.RandomMinMax(20, 35);
                double damageBonus = 0;
                
                CheckMagicResist(mobile);	  

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, mobile, EnhancedSpellbookType.Energy, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, mobile, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.EnergyBolt);

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.EnhancedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.EnhancedSpellBonus;
                }

                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damageBonus += SpellHelper.ChargedSpellTamedCreatureBonus;

                    else
                        damageBonus += SpellHelper.ChargedSpellBonus;

                    if (spellHue != 0)
                    {
                        source.MovingParticles(mobile, 0x22C7, 4, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                        source.MovingParticles(mobile, 0x22C7, 10, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                    }

                    else
                    {
                        source.MovingParticles(mobile, 0x379F, 4, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                        source.MovingParticles(mobile, 0x379F, 10, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                    }

                    source.PlaySound(0x20A);
                }

                else
                {
                    if (spellHue != 0)
                        source.MovingParticles(mobile, 0x22C7, 4, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);

                    else
                        source.MovingParticles(mobile, 0x379F, 7, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);

                    source.PlaySound(0x20A);
                }

                damage *= GetDamageScalar(mobile, damageBonus);

                SpellHelper.Damage(this, Caster, mobile, damage);
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyBoltSpell m_Owner;

			public InternalTarget( EnergyBoltSpell owner ) : base(12, false, TargetFlags.Harmful )
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