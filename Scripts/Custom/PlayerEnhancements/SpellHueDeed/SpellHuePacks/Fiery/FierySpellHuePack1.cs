using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class FierySpellHuePack1 : Bag
    {     
         [Constructable]
        public FierySpellHuePack1()
        {
            Name = "Fiery Spell Hue Pack 1";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.AirElemental, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.EnergyBolt, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.MeteorSwarm, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.EnergyField, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Curse, SpellHueType.Fiery));            
            DropItem(new SpellHueDeed(HueableSpell.MagicArrow, SpellHueType.Fiery));
        }

        public FierySpellHuePack1(Serial serial): base(serial)
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