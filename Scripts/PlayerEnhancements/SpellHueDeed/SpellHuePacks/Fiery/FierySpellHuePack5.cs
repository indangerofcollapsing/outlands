using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class FierySpellHuePack5 : Bag
    {     
         [Constructable]
        public FierySpellHuePack5()
        {
            Name = "Fiery Spell Hue Pack 5";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.EnergyVortex, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.EarthElemental, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.WallOfStone, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.MagicReflect, SpellHueType.Fiery));            
            DropItem(new SpellHueDeed(HueableSpell.Fireball, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.MindBlast, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Weaken, SpellHueType.Fiery));
        }

        public FierySpellHuePack5(Serial serial): base(serial)
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