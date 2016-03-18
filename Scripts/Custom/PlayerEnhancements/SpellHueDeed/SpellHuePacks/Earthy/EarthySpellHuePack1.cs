using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class EarthySpellHuePack1 : Bag
    {     
         [Constructable]
        public EarthySpellHuePack1()
        {
            Name = "Earthy Spell Hue Pack 1";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.AirElemental, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.EnergyBolt, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.MeteorSwarm, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.EnergyField, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Curse, SpellHueType.Earthy));            
            DropItem(new SpellHueDeed(HueableSpell.MagicArrow, SpellHueType.Earthy));
        }

        public EarthySpellHuePack1(Serial serial): base(serial)
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