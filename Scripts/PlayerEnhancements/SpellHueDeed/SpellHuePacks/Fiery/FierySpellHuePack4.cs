using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class FierySpellHuePack4 : Bag
    {     
         [Constructable]
        public FierySpellHuePack4()
        {
            Name = "Fiery Spell Hue Pack 4";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.SummonDaemon, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Teleport, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Cure, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.Bless, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.MassDispel, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.ArchProtection, SpellHueType.Fiery));
            DropItem(new SpellHueDeed(HueableSpell.ReactiveArmor, SpellHueType.Fiery));
        }

        public FierySpellHuePack4(Serial serial): base(serial)
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