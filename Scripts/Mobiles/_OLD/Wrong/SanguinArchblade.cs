using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a sanguin archeblade corpse")]
    public class SanguinArchblade : BaseSanguin
    {

        [Constructable]
        public SanguinArchblade(): base()
        {            
            Name = "a sanguin archblade";

            SetStr(75);
            SetDex(100);
            SetInt(2000);

            SetHits(2000);

            SetDamage(30, 45);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.MagicResist, 100);
            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);

            VirtualArmor = 50;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new Skirt() { Movable = false, Hue = 2051 });
            AddItem(new Sandals() { Movable = false, Hue = 1775 });
            AddItem(new BodySash() { Movable = false, Hue = 2051 });
            AddItem(new OverseerSunderedBlade() { Movable = false, Hue = 2051, Name = "a fanatic's blade" });
            AddItem(new ChainChest() { Movable = false, Hue = 1775 });
            AddItem(new WizardsHat() { Movable = false, Hue = 2051 });
            AddItem(new RingmailGloves() { Movable = false, Hue = 1775 });
            AddItem(new ChainLegs() { Movable = false, Hue = 1775 });

            SpellDelayMin = 3;
            SpellDelayMax = 4;
            SpellHue = 1775;
        }

        public SanguinArchblade(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}