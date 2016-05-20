using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an orghereim shield mother corpse")]
    public class OrghereimShieldMother : BaseOrghereim
    {        
        [Constructable]
        public OrghereimShieldMother() : base()
        {
            Name = "an orghereim shield mother";

            Body = 0x191;            

            SetStr( 75 );
            SetDex( 25 );
            SetInt( 25 );

            SetHits( 550 );

            SetDamage(20, 30);

            SetSkill(SkillName.Archery, 100);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Parry, 25);

            SetSkill(SkillName.Healing, 75);

            VirtualArmor = 50;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new PlateGorget() { Movable = false, Hue = itemHue });
            AddItem(new FemalePlateChest() { Movable = false, Hue = itemHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });
            AddItem(new ThighBoots() { Movable = false, Hue = itemHue });

            AddItem(new Bardiche() { Movable = false, Hue = itemHue, Layer = Layer.FirstValid });
            AddItem(new BronzeShield() { Movable = false, Hue = itemHue });
        }

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int DoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public OrghereimShieldMother(Serial serial): base(serial)
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