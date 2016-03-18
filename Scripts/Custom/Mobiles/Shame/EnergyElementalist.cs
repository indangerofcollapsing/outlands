using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("an energy elementalist corpse")]
    public class EnergyElementalist : BaseAtlantian
    {
        [Constructable]
        public EnergyElementalist(): base()
        {
            Name = "an energy elementalist";

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

            Utility.AssignRandomHair(this, Utility.RandomList(2588));

            AddItem(new Sandals() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = 2588 });

            AddItem(new Runebook() { Movable = false, Hue = 2588, Name = "tome of energy dispersion" });
        }

        public override void SetUniqueAI()
        {
            CastOnlyEnergySpells = true;
        }

        public EnergyElementalist(Serial serial): base(serial)
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
