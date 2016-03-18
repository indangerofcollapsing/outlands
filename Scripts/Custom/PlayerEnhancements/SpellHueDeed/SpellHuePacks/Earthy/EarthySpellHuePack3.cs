using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class EarthySpellHuePack3 : Bag
    {     
         [Constructable]
        public EarthySpellHuePack3()
        {
            Name = "Earthy Spell Hue Pack 3";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.FireElemental, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Flamestrike, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Harm, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.SummonCreature, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.MassCurse, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Clumsy, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Protection, SpellHueType.Earthy));
        }

        public EarthySpellHuePack3(Serial serial): base(serial)
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