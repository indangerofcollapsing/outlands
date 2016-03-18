using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an orghereim tracker corpse")]
    public class OrghereimTracker : BaseOrghereim
    {
        [Constructable]
        public OrghereimTracker(): base()
        {
            Name = "an orghereim tracker";

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(300);

            SetDamage(7, 14);           

            SetSkill(SkillName.Archery, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.DetectHidden, 75);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new Bandana() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });

            AddItem(new LeatherChest() { Movable = false, Hue = 0 });
            AddItem(new LeatherArms() { Movable = false, Hue = 0 });
            AddItem(new LeatherLegs() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });
            AddItem(new LeatherGorget() { Movable = false, Hue = 0 });

            AddItem(new Bow() { Movable = false, Hue = 0 });
            PackItem(new Arrow(10));
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.05;
            
            DictCombatTargeting[CombatTargeting.Predator] = 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 6; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.EntangleSpecialAbility(0.05, this, defender, 1.0, 5, -1, true, "", "Their arrow pins you in place!");
        }

        public OrghereimTracker(Serial serial): base(serial)
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