using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Custom
{
    public class EarthySpellHuePack4 : Bag
    {     
         [Constructable]
        public EarthySpellHuePack4()
        {
            Name = "Earthy Spell Hue Pack 4";
            PackItems();
        }

        public void PackItems()
        {
            DropItem(new SpellHueDeed(HueableSpell.SummonDaemon, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Teleport, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Cure, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.Bless, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.MassDispel, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.ArchProtection, SpellHueType.Earthy));
            DropItem(new SpellHueDeed(HueableSpell.ReactiveArmor, SpellHueType.Earthy));
        }

        public EarthySpellHuePack4(Serial serial): base(serial)
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