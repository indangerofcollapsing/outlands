using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class EarthySpellHuePack5 : Bag
    {     
         [Constructable]
        public EarthySpellHuePack5()
        {
            Name = "Earthy Spell Hue Pack 5";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.EnergyVortex, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.EarthElemental, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.WallOfStone, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.MagicReflect, SpellHueType.Earthy));            
            DropItem(new SpellHueDeed(HueableSpell.Fireball, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.MindBlast, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Weaken, SpellHueType.Earthy));
        }

        public EarthySpellHuePack5(Serial serial): base(serial)
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