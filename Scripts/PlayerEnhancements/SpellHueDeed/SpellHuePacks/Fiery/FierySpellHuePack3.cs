using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class FierySpellHuePack3 : Bag
    {     
         [Constructable]
        public FierySpellHuePack3()
        {
            Name = "Fiery Spell Hue Pack 3";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.FireElemental, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Flamestrike, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Harm, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.SummonCreature, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.MassCurse, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Clumsy, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Protection, SpellHueType.Fiery));
        }

        public FierySpellHuePack3(Serial serial): base(serial)
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