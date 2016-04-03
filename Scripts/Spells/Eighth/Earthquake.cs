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

        public EarthquakeSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
        {
        }
        
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
                    Mobile mobile = targets[i];

                    double damage = (double)Utility.RandomMinMax(5, 10);
                    double damageBonus = 0;

                    CheckMagicResist(mobile);

                    if (mobile is PlayerMobile)                    
                        damage = (double)mobile.Hits * .6;                    

                    else
                    {
                        Boolean isTamedTarget = SpellHelper.IsTamedTarget(Caster, mobile);

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
                        }

                        mobile.FixedEffect(0x3779, 10, 20);

                        damage *= GetDamageScalar(mobile, damageBonus);
                    }                    

                    Caster.DoHarmful(mobile);

                    SpellHelper.Damage(this, Caster, mobile, damage);
                }                

                if (tremor)
                    CustomizationAbilities.Tremor(Caster, range);
            }

            FinishSequence();
        }
    }
}