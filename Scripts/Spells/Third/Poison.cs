using System;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Third
{
    public class PoisonSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Poison", "In Nox",
                203,
                9051,
                Reagent.Nightshade
            );

        public override SpellCircle Circle { get { return SpellCircle.Third; } }

        public PoisonSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
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

        public static void ApplyEffect(Mobile Caster, Mobile m)
        {
            if (m.Spell != null)
                m.Spell.OnCasterHurt();

            double magerySkill = Caster.Skills[SkillName.Magery].Value;
            double poisoningSkill = Caster.Skills[SkillName.Poisoning].Value;

            //Poisoning Skill is Capped by Magery
            if (poisoningSkill > magerySkill)
                poisoningSkill = magerySkill;

            //Defaults to Regular Poison
            int poisonLevel = 0;

            //Player Caster
            if (Caster is PlayerMobile)
                poisonLevel = 1;

            //Against Non-Players
            if (m is BaseCreature)
            {
                double poisonResult = Utility.RandomDouble();

                double greaterUpgradeChance = 1.0 * (poisoningSkill / 100);
                double deadlyUpgradeChance = .30 * (poisoningSkill / 100);
                double lethalUpgradeChance = .10 * (poisoningSkill / 100);

                bool chargeUsed = false;

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Warlock, true, false);

                if (enhancedSpellcast)
                {
                    chargeUsed = true;

                    greaterUpgradeChance *= 1.5;
                    deadlyUpgradeChance *= 1.5;
                    lethalUpgradeChance *= 1.5;
                }

                if (poisonResult <= greaterUpgradeChance && poisoningSkill >= 25)
                {
                    if (poisonLevel < 2)
                        poisonLevel = 2;
                }

                if (poisonResult <= deadlyUpgradeChance && poisoningSkill >= 50)
                {
                    if (poisonLevel < 3)
                        poisonLevel = 3;
                }

                if (poisonResult <= lethalUpgradeChance && poisoningSkill >= 75)
                {
                    if (poisonLevel < 4)
                        poisonLevel = 4;
                }

                if (chargeUsed && poisonLevel > 1)
                {
                    if (Caster.FindItemOnLayer(Layer.OneHanded) is EnhancedSpellbook)
                    {
                        EnhancedSpellbook spellbook = Caster.FindItemOnLayer(Layer.OneHanded) as EnhancedSpellbook;

                        if (spellbook != null)
                            spellbook.OnSpellCast(Caster);
                    }
                }
            }

            m.ApplyPoison(Caster, Poison.GetPoison(poisonLevel));
        }

        public void Target(Mobile mobile)
        {
            if (!Caster.CanSee(mobile) || mobile.Hidden)
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }

            else if (CheckHSequence(mobile))
            {
                SpellHelper.Turn(Caster, mobile);
                
                SpellHelper.CheckReflect((int)this.Circle, Caster, ref mobile);

                mobile.Paralyzed = false;

                bool success = false;

                if (CheckResisted(mobile) && mobile is PlayerMobile)
                    mobile.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.                

                else
                {
                    ApplyEffect(Caster, mobile);
                    success = true;
                }

                //Player Enhancement Customization: Venomous
                bool venomous = PlayerEnhancementPersistance.IsCustomizationEntryActive(Caster, CustomizationType.Venomous);

                if (venomous)
                {
                    if (success)                    
                        CustomizationAbilities.Venomous(mobile);                     

                    else
                    {
                        mobile.FixedParticles(0x374A, 10, 15, 5021, 0, 0, EffectLayer.Waist);
                        mobile.PlaySound(0x474);
                    }

                }

                else
                {
                    mobile.FixedParticles(0x374A, 10, 15, 5021, 0, 0, EffectLayer.Waist);
                    mobile.PlaySound(0x474);
                }

                BaseDungeonArmor.PlayerDungeonArmorProfile casterDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(Caster, null);

                if (casterDungeonArmor.MatchingSet && !casterDungeonArmor.InPlayerCombat && mobile is BaseCreature)
                {
                    if (Utility.RandomDouble() <= casterDungeonArmor.DungeonArmorDetail.PoisonSpellNoManaCostChance)
                    {
                        Caster.Mana += 9;
                        Caster.SendMessage("You feel a rush of energy from your armor, fueling mana into the spell.");
                        
                        Effects.PlaySound(Caster.Location, Caster.Map, 0x64B);
                        Effects.SendLocationParticles(EffectItem.Create(Caster.Location, Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, casterDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);                        
                    }
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private PoisonSpell m_Owner;

            public InternalTarget(PoisonSpell owner): base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}