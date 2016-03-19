using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a sanguine mender corpse")]
    public class SanguinMender : BaseSanguin
    {
        [Constructable]
        public SanguinMender(): base()
        {
            Name = "a sanguine mender";

            SetStr(75);
            SetDex(25);
            SetInt(100);

            SetHits(300);
            SetMana(1000);

            SetDamage(7, 14);

            SetSkill(SkillName.Macing, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 50;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new Helmet() { Movable = false, Hue = itemHue });
            AddItem(new LeatherGorget() { Movable = false, Hue = itemHue });
            AddItem(new PlateArms() { Movable = false, Hue = weaponHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = itemHue });
            AddItem(new LeatherChest() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new WarMace() { Movable = false, Hue = weaponHue });
            AddItem(new WoodenKiteShield() { Movable = false, Hue = weaponHue });
        }

        public SanguinMender(Serial serial): base(serial)
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