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
                    this.Target(casterCreature.SpellTarget);
            }

            else
                Caster.Target = new InternalTarget(this);
        }

        public static void ApplyEffect(Mobile Caster, Mobile target)
        {
            if (target.Spell != null)
                target.Spell.OnCasterHurt();

            double magerySkill = Caster.Skills[SkillName.Magery].Value;
            double poisoningSkill = Caster.Skills[SkillName.Poisoning].Value;

            if (poisoningSkill > magerySkill)
            {
                if (!(poisoningSkill > 100 && magerySkill == 100))
                    poisoningSkill = magerySkill;
            }

            if (poisoningSkill > 100 && Caster is PlayerMobile && target is PlayerMobile)
                poisoningSkill = 100;

            int poisonLevel = 0;

            if (Caster is PlayerMobile)
                poisonLevel = 1;

            double greaterPoisonChance = (poisoningSkill / 100) * .5;
            double deadlyPoisonChance = (poisoningSkill / 100) * .2;

            double creaturePoisonUpgradeChanceScalar = 1.0;
            double playerPoisonUpgradeChanceScalar = 1.0;

            double greaterPoisonSkillMinimum = 50;
            double deadlyPoisonSkillMinimum = 75;

            double enhancedChanceScalar = 1.5;
            bool enhanceChargeUsed = false;

            //Against Players            
            if (target is PlayerMobile)
            {
                if (poisoningSkill >= greaterPoisonSkillMinimum)
                {
                    if (Utility.RandomDouble() <= greaterPoisonChance * playerPoisonUpgradeChanceScalar)
                        poisonLevel = 2;
                }

                if (poisoningSkill >= deadlyPoisonSkillMinimum)
                {
                    if (Utility.RandomDouble() <= deadlyPoisonChance * playerPoisonUpgradeChanceScalar)
                        poisonLevel = 3;
                }
            }

            //Against Creatures
            if (target is BaseCreature)
            {
                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Warlock, true, false);

                if (enhancedSpellcast)
                {
                    enhanceChargeUsed = true;

                    greaterPoisonChance *= enhancedChanceScalar;
                    deadlyPoisonChance *= enhancedChanceScalar;
                }

                if (poisoningSkill >= greaterPoisonSkillMinimum)
                {
                    if (Utility.RandomDouble() <= greaterPoisonChance * creaturePoisonUpgradeChanceScalar)
                        poisonLevel = 2;
                }

                if (poisoningSkill >= deadlyPoisonSkillMinimum)
                {
                    if (Utility.RandomDouble() <= deadlyPoisonChance * creaturePoisonUpgradeChanceScalar)
                        poisonLevel = 3;
                }

                if (enhanceChargeUsed && poisonLevel > 1)
                {
                    if (Caster.FindItemOnLayer(Layer.OneHanded) is EnhancedSpellbook)
                    {
                        EnhancedSpellbook spellbook = Caster.FindItemOnLayer(Layer.OneHanded) as EnhancedSpellbook;

                        if (spellbook != null)
                            spellbook.OnSpellCast(Caster);
                    }
                }
            }            

            target.ApplyPoison(Caster, Poison.GetPoison(poisonLevel));
        }

        public void Target(Mobile mobile)
        {
            if (!Caster.CanSee(mobile) || mobile.Hidden)
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (CheckHSequence(mobile))
            {
                SpellHelper.Turn(Caster, mobile);
                SpellHelper.CheckReflect((int)this.Circle, Caster, ref mobile);

                if (mobile.Spell != null)
                    mobile.Spell.OnCasterHurt();

                mobile.Paralyzed = false;

                bool success = false;

                if (CheckMagicResist(mobile) && mobile is PlayerMobile)
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

                DungeonArmor.PlayerDungeonArmorProfile casterDungeonArmor = new DungeonArmor.PlayerDungeonArmorProfile(Caster, null);

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

            public InternalTarget(PoisonSpell owner)
                : base(12, false, TargetFlags.Harmful)
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