using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an orghereim ice carl corpse")]
    public class OrghereimIceCarl : BaseOrghereim
    {
        public DateTime m_NextThrowingAxeAllowed;
        public TimeSpan NextThrowingAxeDelay = TimeSpan.FromSeconds(5);
                
        [Constructable]
        public OrghereimIceCarl(): base()
        {           
            Name = "an orghereim ice carl";
            
            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(20, 30);

            SetSkill(SkillName.Archery, 85);
            SetSkill(SkillName.Swords, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new DragonHelm() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = itemHue });
            AddItem(new Cloak() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });

            AddItem(new StuddedChest() { Movable = false, Hue = 0 });
            AddItem(new LeatherArms() { Movable = false, Hue = 0 });
            AddItem(new LeatherLegs() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });
            AddItem(new LeatherGorget() { Movable = false, Hue = 0 });            

            AddItem(new HeavyOrnateAxe() { Movable = false, Hue = itemHue, Layer = Layer.FirstValid, Name = "an orghereim throwing axe" });
            AddItem(new WoodenShield() { Movable = false, Hue = 0 });

            PackItem(new Arrow(10));
        }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.75;
                        
            DictCombatRange[CombatRange.WeaponAttackRange] = 0;
            DictCombatRange[CombatRange.SpellRange] = 8;
            DictCombatRange[CombatRange.Withdraw] = 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int DoubloonValue { get { return 8; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            m_NextThrowingAxeAllowed = DateTime.UtcNow + NextThrowingAxeDelay;
        }

        public override void OnThink()
        {
            base.OnThink();
            
            if (Utility.RandomDouble() < 1 && DateTime.UtcNow > m_NextThrowingAxeAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
            {
                Mobile combatant = this.Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 8)
                    {
                        //Backstab
                        if (Hidden)
                        {
                            int minDamage = (int)((double)DamageMin * 1.5);
                            int maxDamage = (int)((double)DamageMax * 1.5);

                            SpecialAbilities.ThrowObjectAbility(this, combatant, 1.5, 5, .95, minDamage, maxDamage, 3912, 3911, itemHue, -1, 0x237, 1);

                            Timer.DelayCall(TimeSpan.FromSeconds(0.4), delegate
                            {
                                if (this == null) return;
                                if (!this.Alive || this.Deleted) return;

                                if (Hidden)
                                {
                                    Effects.PlaySound(Location, Map, 0x51D);
                                    RevealingAction();
                                }
                            });
                        }

                        else
                        {
                            int minDamage = (int)((double)DamageMin * 1.0);
                            int maxDamage = (int)((double)DamageMax * 1.0);

                            SpecialAbilities.ThrowObjectAbility(this, combatant, 1.5, 5, .75, minDamage, maxDamage, 3912, 3911, itemHue, -1, 0x237, 1);
                        }

                        m_NextThrowingAxeAllowed = DateTime.UtcNow + NextThrowingAxeDelay;
                    }
                }
            }            
        }

        public OrghereimIceCarl(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}