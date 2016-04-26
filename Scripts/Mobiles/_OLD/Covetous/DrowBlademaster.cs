using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a drow blademaster corpse")]
    public class DrowBlademaster : BaseDrow
    {   
        [Constructable]
        public DrowBlademaster(): base()
        {            
            Name = "a drow blademaster";                            

            SetStr(75);
            SetDex(75);
            SetInt(75);

            SetHits(1000);
            SetMana(1000);

            SetDamage(18, 26);            

            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Fencing, 100);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 50;

            Fame = 1500;
            Karma = -1500;

            AddItem(new PlateGorget() { Movable = false, Hue = itemHue });
            AddItem(new PlateChest() { Movable = false, Hue = itemHue });
            AddItem(new PlateArms() { Movable = false, Hue = itemHue });
            AddItem(new PlateGloves() { Movable = false, Hue = itemHue });

            AddItem(new ChainmailLegs() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = itemHue });

            Utility.AssignRandomHair(this, hairHue);

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new WarCleaver() { Movable = false, Speed = 40, Hue = weaponHue, Name = "a drow reaverblade" }); break;
                case 2: AddItem(new RadiantScimitar() { Movable = false, Speed = 40, Hue = weaponHue, Name = "a drow greatsword" }); break;
                case 3: AddItem(new NoDachi() { Movable = false, Speed = 40, Hue = weaponHue, Name = "a drow warblade" }); break;
            }
        }

        public override void SetUniqueAI()
        {            
            UniqueCreatureDifficultyScalar = 1.2;

            CombatEpicActionMinDelay = 15;
            CombatEpicActionMaxDelay = 30;

            DictCombatAction[CombatAction.CombatHealSelf] = 2;
            DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
            DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;

            DictCombatAction[CombatAction.CombatEpicAction] = 1;
            DictCombatEpicAction[CombatEpicAction.MeleeBleedAoE] = 25;

            CombatHealActionMinDelay = 20;
            CombatHealActionMaxDelay = 40;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.2, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!", "-1");
        }

        public DrowBlademaster(Serial serial): base(serial)
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