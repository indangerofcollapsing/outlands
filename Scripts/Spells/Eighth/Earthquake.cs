using System;
using System.Collections.Generic;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using Server.Custom;

namespace Server.Spells.Eighth
{
    public class EarthquakeSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Earthquake", "In Vas Por",
                233,
                9012,
                false,
                Reagent.Bloodmoss,
                Reagent.Ginseng,
                Reagent.MandrakeRoot,
                Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }


        public override TimeSpan GetCastDelay()
        {
            return TimeSpan.FromSeconds(4.0);
        }


        public EarthquakeSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool DelayedDamage { get { return !Core.AOS; } }

        public override void OnCast()
        {
            if (SpellHelper.CheckTown(Caster, Caster) && CheckSequence())
            {
                List<Mobile> targets = new List<Mobile>();

                Map map = Caster.Map;

                //Player Enhancement Customization: Tremor
                bool tremor = PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.Tremors);

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Wizard, false, true);
                Boolean chargedSpellcast = SpellHelper.IsChargedSpell(Caster, null, false, Scroll != null);

                int range = 1 + (int)(Caster.Skills[SkillName.Magery].Value / 15.0);

                bool useHeightCheck = true;
                int maxDifferenceZ = 20;
                
                if (map != null)
                {
                    foreach (Mobile m in Caster.GetMobilesInRange(range))
                    {
                        int differenceZ = Math.Abs(Caster.Location.Z - m.Location.Z);

                        if (useHeightCheck && (differenceZ >= maxDifferenceZ))
                            continue;

                        if (Caster != m && SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(m, false) && (!Core.AOS || Caster.InLOS(m)))
                            targets.Add(m);
                    }
                }

                Caster.PlaySound(0x2f3);

                int baseDamage = Utility.RandomMinMax(20, 25);

                for (int i = 0; i < targets.Count; ++i)
                {
                    Mobile m = targets[i];

                    int damage = baseDamage;

                    if (m is PlayerMobile)                    
                        damage = (int)((double)m.Hits * .6);                    

                    else
                    {
                        damage = (int)((double)damage * GetDamageScalar(m));

                        Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, m);

                        if (enhancedSpellcast)
                        {
                            if (isTamedTarget)
                                damage = (int)((double)damage * SpellHelper.enhancedTamedCreatureMultiplier);
                            else
                                damage = (int)((double)damage * SpellHelper.enhancedMultiplier);
                        }

                        if (chargedSpellcast)
                        {
                            if (isTamedTarget)
                                damage = (int)((double)damage * SpellHelper.chargedTamedCreatureMultiplier);

                            else
                                damage = (int)((double)damage * SpellHelper.chargedMultiplier);
                        }

                        m.FixedEffect(0x3779, 10, 20);
                    }

                    if (damage < 10 && !(m is PlayerMobile))
                        damage = 10;

                    Caster.DoHarmful(m);
                    SpellHelper.Damage(TimeSpan.Zero, m, Caster, damage, 100, 0, 0, 0, 0);
                }

                

                if (tremor)
                    CustomizationAbilities.Tremor(Caster, range);
            }

            FinishSequence();
        }
    }
}