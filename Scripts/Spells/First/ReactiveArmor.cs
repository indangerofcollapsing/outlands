using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.First
{
	public class ReactiveArmorSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Reactive Armor", "Flam Sanct",
				236,
				9011,
				Reagent.Garlic,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.First; } }

		public ReactiveArmorSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			return true;
		}

		private static Hashtable m_Table = new Hashtable();

        public static bool IsUnderProtection(Mobile m)
        {
            object[] mods = (object[])m_Table[m];
            return mods != null;
        }

        public static void Dispel(Mobile m)
        {
            m_Table.Remove(m);

            ResistanceMod[] mods = (ResistanceMod[])m_Table[m];
            for (int i = 0; i < mods.Length; ++i)
                m.RemoveResistanceMod(mods[i]);
        }
		public override void OnCast()
        {
            #region AOS - NOT USED
            if (Core.AOS)
            {
                /* The reactive armor spell increases the caster's physical resistance, while lowering the caster's elemental resistances.
                 * 15 + (Inscription/20) Physcial bonus
                 * -5 Elemental
                 * The reactive armor spell has an indefinite duration, becoming active when cast, and deactivated when re-cast. 
                 * Reactive Armor, Protection, and Magic Reflection will stay on—even after logging out, even after dying—until you “turn them off” by casting them again. 
                 * (+20 physical -5 elemental at 100 Inscription)
                 */

                if (CheckSequence())
                {
                    Mobile targ = Caster;

                    ResistanceMod[] mods = (ResistanceMod[])m_Table[targ];

                    if (mods == null)
                    {
                        targ.PlaySound(0x1E9);
                        targ.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);

                        mods = new ResistanceMod[5]
							{
								new ResistanceMod( ResistanceType.Physical, 15 + (int)(targ.Skills[SkillName.Inscribe].Value / 20) ),
								new ResistanceMod( ResistanceType.Fire, -5 ),
								new ResistanceMod( ResistanceType.Cold, -5 ),
								new ResistanceMod( ResistanceType.Poison, -5 ),
								new ResistanceMod( ResistanceType.Energy, -5 )
							};

                        m_Table[targ] = mods;

                        for (int i = 0; i < mods.Length; ++i)
                            targ.AddResistanceMod(mods[i]);
                    }
                    else
                    {
                        targ.PlaySound(0x1ED);
                        targ.FixedParticles(0x376A, 9, 32, 5008, EffectLayer.Waist);

                        m_Table.Remove(targ);

                        for (int i = 0; i < mods.Length; ++i)
                            targ.RemoveResistanceMod(mods[i]);
                    }
                }

                FinishSequence();
            }
            #endregion
            else
            {
                if (Caster is BaseCreature)
                {
                    this.Target(Caster);
                }

                else
                {
                    Caster.Target = new InternalTarget(this);
                }
            }
		}

		public void Target( Mobile m )
		{
            if (!m.CanSee(m) || m.Hidden)
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}

            if (!m.CanBeginAction(typeof(ReactiveArmorSpell)))
            {
                Caster.SendMessage("The spell will not adhere to them at this time.");
            }

            else if (CheckBSequence(m))
            {
                int value = (int)(m.Skills[SkillName.Magery].Value);

                value /= 3;

                if (value < 0)
                    value = 1;
                else if (value > 20)
                    value = 20;
                                
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Wizard, false, true);

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(Caster, HueableSpell.ReactiveArmor);    

                //Spirit Speak Bonus
                value += (int)(20 * (Caster.Skills[SkillName.SpiritSpeak].Value / 100));
                                
                if (enhancedSpellcast)
                {
                    value += 20;

                    m.FixedParticles(0x376A, 9, 64, 5008, spellHue, 0, EffectLayer.Waist);
                    m.PlaySound(0x1F2);
                }

                else
                {
                    m.FixedParticles(0x376A, 9, 32, 5008, spellHue, 0, EffectLayer.Waist);
                    m.PlaySound(0x1F2);
                }

                m.MeleeDamageAbsorb = value;
               
                m.BeginAction(typeof(ReactiveArmorSpell));
                Timer.DelayCall(TimeSpan.FromMinutes(0.5), delegate { m.EndAction(typeof(ReactiveArmorSpell)); });
            }

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ReactiveArmorSpell m_Owner;

			public InternalTarget( ReactiveArmorSpell owner ) : base( 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			} 

			protected override void OnTarget( Mobile from, object o )
			{
				if (  o is Mobile )
				{	
					if ( ((Mobile)o).MeleeDamageAbsorb > 0 )
						from.SendLocalizedMessage( 1005559 ); // This spell is already in effect.
					else
						m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}