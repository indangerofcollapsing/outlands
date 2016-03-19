using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an elemental seer corpse")]
    public class ElementalSeer : BaseAtlantian
    {
        [Constructable]
        public ElementalSeer(): base()
        {
            Name = "an elemental seer";

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(300);
            SetMana(2000);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 50;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, Utility.RandomList(1072));

            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = 1072 });

            AddItem(new Runebook() { Movable = false, Hue = 1072, Name = "tome of healing" });
        }       

        public ElementalSeer(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
