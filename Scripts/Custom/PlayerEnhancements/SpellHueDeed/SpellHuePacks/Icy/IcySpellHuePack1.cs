using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class IcySpellHuePack1 : Bag
    {     
         [Constructable]
        public IcySpellHuePack1()
        {
            Name = "Icy Spell Hue Pack 1";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.AirElemental, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.EnergyBolt, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.MeteorSwarm, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.EnergyField, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Curse, SpellHueType.Icy));            
            DropItem(new SpellHueDeed(HueableSpell.MagicArrow, SpellHueType.Icy));
        }

        public IcySpellHuePack1(Serial serial): base(serial)
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