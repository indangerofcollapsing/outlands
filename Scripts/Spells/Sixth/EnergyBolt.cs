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
                {
                    this.Target(casterCreature.SpellTarget);
                }
            }

            else
            {
                Caster.Target = new InternalTarget(this);
            }
		}

		public override bool DelayedDamage{ get{ return true; } }

		public void Target( Mobile m )
		{
            if (!Caster.CanSee(m) || m.Hidden)
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) )
            {
                Mobile source = Caster;

                SpellHelper.Turn(Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, ref source, ref m);

                double damage;
                
                damage = Utility.Random(18, 17);

                if (m.Region is UOACZRegion)
                    damage = Utility.RandomMinMax(12, 24);

                if (CheckResisted(m))
                {
                    damage *= 0.70;

                    m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }

                // Scale damage based on evalint and resist
                damage *= GetDamageScalar(m);                

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, m, EnhancedSpellbookType.Energy, true, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, m, true, Scroll != null);
                Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.EnergyBolt);

                if (enhancedSpellcast)
                {
                    if (isTamedTarget)
                        damage *= SpellHelper.enhancedTamedCreatureMultiplier;

                    else
                        damage *= SpellHelper.enhancedMultiplier;
                }

                if (chargedSpellcast)
                {
                    if (isTamedTarget)
                        damage *= SpellHelper.chargedTamedCreatureMultiplier;

                    else
                        damage *= SpellHelper.chargedMultiplier;

                    if (spellHue != 0)
                    {
                        source.MovingParticles(m, 0x22C7, 4, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                        source.MovingParticles(m, 0x22C7, 10, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                    }

                    else
                    {
                        source.MovingParticles(m, 0x379F, 4, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                        source.MovingParticles(m, 0x379F, 10, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);
                    }

                    source.PlaySound(0x20A);
                }

                else
                {
                    if (spellHue != 0)
                        source.MovingParticles(m, 0x22C7, 4, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);

                    else
                        source.MovingParticles(m, 0x379F, 7, 0, false, true, spellHue, 0, 3043, 4043, 0x211, 0);

                    source.PlaySound(0x20A);
                }

                // Deal the damage
                SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 100);
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyBoltSpell m_Owner;

			public InternalTarget( EnergyBoltSpell owner ) : base( Core.ML ? 10 : 12, false, TargetFlags.Harmful )
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