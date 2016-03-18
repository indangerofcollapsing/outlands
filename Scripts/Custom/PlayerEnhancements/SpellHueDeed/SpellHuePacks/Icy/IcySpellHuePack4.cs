using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class IcySpellHuePack4 : Bag
    {     
         [Constructable]
        public IcySpellHuePack4()
        {
            Name = "Icy Spell Hue Pack 4";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.SummonDaemon, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Teleport, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Cure, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.Bless, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.MassDispel, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.ArchProtection, SpellHueType.Icy));
            DropItem(new SpellHueDeed(HueableSpell.ReactiveArmor, SpellHueType.Icy));
        }

        public IcySpellHuePack4(Serial serial): base(serial)
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