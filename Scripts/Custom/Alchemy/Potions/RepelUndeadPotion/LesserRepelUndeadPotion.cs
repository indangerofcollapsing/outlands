using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class LesserRepelUndeadPotion : BaseRepelPotion
    {
        public override double Delay { get { return 60; } }
        public override SlayerName RepelSlayerName { get { return SlayerName.Silver; } }

        [Constructable]
        public LesserRepelUndeadPotion()
            : base(PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = 0x3CB;
            Name = "a swirling white potion";
        }
        public LesserRepelUndeadPotion(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
	}
}