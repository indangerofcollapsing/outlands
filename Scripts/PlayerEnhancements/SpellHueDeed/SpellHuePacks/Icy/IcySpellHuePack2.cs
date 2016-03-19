using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class IcySpellHuePack2 : Bag
    {     
         [Constructable]
        public IcySpellHuePack2()
        {
            Name = "Icy Spell Hue Pack 2";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.WaterElemental, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Explosion, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.BladeSpirits, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.ArchCure, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Dispel, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Feeblemind, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.ParalyzeField, SpellHueType.Icy));  
        }

        public IcySpellHuePack2(Serial serial): base(serial)
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