using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class EarthySpellHuePack2 : Bag
    {     
         [Constructable]
        public EarthySpellHuePack2()
        {
            Name = "Earthy Spell Hue Pack 2";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.WaterElemental, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Explosion, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.BladeSpirits, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.ArchCure, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Dispel, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Feeblemind, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.ParalyzeField, SpellHueType.Earthy));  
        }

        public EarthySpellHuePack2(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}