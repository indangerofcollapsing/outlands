using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("a flame elementalist corpse")]
    public class FlameElementalist : BaseAtlantian
    {
        [Constructable]
        public FlameElementalist(): base()
        {
            Name = "a flame elementalist";

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(300);
            SetMana(1000);

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

            Utility.AssignRandomHair(this, Utility.RandomList(1358));

            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = 1358 });

            AddItem(new Runebook() { Movable = false, Hue = 1358, Name = "tome of destruction" });
        }

        public override void SetUniqueAI()
        {
            CastOnlyFireSpells = true;
        }

        public FlameElementalist(Serial serial): base(serial)
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
