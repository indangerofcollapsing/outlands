using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class IcySpellHuePack5 : Bag
    {     
         [Constructable]
        public IcySpellHuePack5()
        {
            Name = "Icy Spell Hue Pack 5";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.EnergyVortex, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.EarthElemental, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.WallOfStone, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.MagicReflect, SpellHueType.Icy));            
            DropItem(new SpellHueDeed(HueableSpell.Fireball, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.MindBlast, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Weaken, SpellHueType.Icy));
        }

        public IcySpellHuePack5(Serial serial): base(serial)
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