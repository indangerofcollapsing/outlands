using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguin hunter corpse")]
    public class SanguinHunter : BaseSanguin
    {
        [Constructable]
        public SanguinHunter(): base()
        {           
            Name = "a sanguin hunter";

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(12, 24);

            SetSkill(SkillName.Archery, 95);
            SetSkill(SkillName.Tactics, 100);            

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new LeatherGorget() { Movable = false, Hue = weaponHue });
            AddItem(new ChainmailChest() { Movable = false, Hue = itemHue });
            AddItem(new LeatherArms() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = weaponHue });
            AddItem(new LeatherLegs() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new Crossbow() { Movable = false, Hue = 0 }); break;
                case 2: AddItem(new HeavyCrossbow() { Movable = false, Hue = 0 }); break;
            }
        }   
        
        public SanguinHunter(Serial serial): base(serial)
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