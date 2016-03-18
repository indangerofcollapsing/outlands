using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an orghereim bow maiden corpse")]
    public class OrghereimBowMaiden : BaseOrghereim
    {
        [Constructable]
        public OrghereimBowMaiden(): base()
        {            
            Name = "an orghereim bow maiden";           

            Body = 0x191;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(10, 20);            

            SetSkill(SkillName.Archery, 100);
            SetSkill(SkillName.Tactics, 100);           

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new PlateGorget() { Movable = false, Hue = itemHue });
            AddItem(new FemalePlateChest() { Movable = false, Hue = itemHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });
            AddItem(new ThighBoots() { Movable = false, Hue = itemHue });            

            AddItem(new Yumi() { Movable = false, Hue = 0, Speed = 30, MaxRange = 14, Name = "an orghereim longbow" });
            PackItem(new Arrow(15));
        }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.15;
            
            DictCombatTargeting[CombatTargeting.Predator] = 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 8; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.EntangleSpecialAbility(0.15, this, defender, 1.0, 5, -1, true, "", "Their arrow pins you in place!");
        }

        public OrghereimBowMaiden(Serial serial): base(serial)
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