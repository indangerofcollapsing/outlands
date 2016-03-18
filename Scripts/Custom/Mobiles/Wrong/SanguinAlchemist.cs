using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguine alchemist corpse")]
    public class SanguinAlchemist : BaseSanguin
    {
        public DateTime m_NextPotionThrowAllowed;
        public TimeSpan NextPotionThrowDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(4, 6));

        [Constructable]
        public SanguinAlchemist(): base()
        {            
            Name = "a sanguine alchemist";

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(400);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);            

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new StuddedGorget() { Movable = false, Hue = weaponHue });
            AddItem(new StuddedArms() { Movable = false, Hue = weaponHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = weaponHue });
            AddItem(new FullApron() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            PackItem(new MortarPestle());
            PackItem(new Garlic(4));
            PackItem(new Ginseng(4));
            PackItem(new SpidersSilk(4));
            PackItem(new MandrakeRoot(4));
        }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 6.2;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextPotionThrowAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
                {
                    Mobile combatant = this.Combatant;

                    if (combatant != null && !BardPacified)
                    {
                        if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 8)
                        {
                            int potionType = Utility.RandomMinMax(1, 5);

                            switch (potionType)
                            {
                                case 1: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Explosion, 1, 10, 20, 1, 0, false, true); break;
                                case 2: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Paralyze, 1, 5, 10, 1, 5, false, true); break;
                                case 3: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Poison, 1, 5, 10, Utility.RandomMinMax(1, 2), 1, false, true); break;
                                case 4: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Frost, 1, 5, 10, .2, 10, false, true); break;
                                case 5: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Shrapnel, 2, 7, 15, 1, 0, true, true); break;
                            }

                            m_NextPotionThrowAllowed = DateTime.UtcNow + NextPotionThrowDelay;
                        }
                    }
                }
            }
        }

        public SanguinAlchemist(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}