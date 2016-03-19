using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class IcySpellHuePack3 : Bag
    {     
         [Constructable]
        public IcySpellHuePack3()
        {
            Name = "Icy Spell Hue Pack 3";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.FireElemental, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Flamestrike, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Harm, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.SummonCreature, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.MassCurse, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Clumsy, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Protection, SpellHueType.Icy));
        }

        public IcySpellHuePack3(Serial serial): base(serial)
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