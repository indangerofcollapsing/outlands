using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class FierySpellHuePack2 : Bag
    {     
         [Constructable]
        public FierySpellHuePack2()
        {
            Name = "Fiery Spell Hue Pack 2";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.WaterElemental, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Explosion, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.BladeSpirits, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.ArchCure, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Dispel, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Feeblemind, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.ParalyzeField, SpellHueType.Fiery));  
        }

        public FierySpellHuePack2(Serial serial): base(serial)
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