using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguin medic corpse")]
    public class SanguinMedic : BaseSanguin
    {
        [Constructable]
        public SanguinMedic(): base()
        {
            Name = "a sanguin medic";

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(10, 20);

            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 25);

            SetSkill(SkillName.Healing, 75);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 50;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new NorseHelm() { Movable = false, Hue = itemHue });
            AddItem(new Doublet() { Movable = false, Hue = itemHue });
            AddItem(new StuddedArms() { Movable = false, Hue = weaponHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = weaponHue });
            AddItem(new Kilt() { Movable = false, Hue = weaponHue });
            AddItem(new ChainmailLegs() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            AddItem(new Cloak() { Movable = false, Hue = weaponHue });

            AddItem(new Mace() { Movable = false, Hue = weaponHue });
            AddItem(new MetalShield() { Movable = false, Hue = weaponHue });
        }

        public SanguinMedic(Serial serial): base(serial)
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