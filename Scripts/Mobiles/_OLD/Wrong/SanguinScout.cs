using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguin scout corpse")]
    public class SanguinScout : BaseSanguin
    {
        [Constructable]
        public SanguinScout(): base()
        {            
            Name = "a sanguin scout";

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Archery, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.DetectHidden, 75);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new LeatherGorget() { Movable = false, Hue = weaponHue });
            AddItem(new LeatherChest() { Movable = false, Hue = 0 });
            AddItem(new LeatherArms() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = weaponHue });
            AddItem(new Kilt() { Movable = false, Hue = weaponHue });
            AddItem(new LeatherLegs() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new Bow() { Movable = false, Hue = 0 });    
        }
        
        public SanguinScout(Serial serial): base(serial)
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