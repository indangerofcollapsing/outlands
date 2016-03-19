using System;
using Server.Network;
using Server;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
    public class GreaterRepelUndeadPotion : BaseRepelPotion
    {
        public override double Delay { get { return 300; } }
        public override SlayerName RepelSlayerName { get { return SlayerName.Silver; } }

        [Constructable]
        public GreaterRepelUndeadPotion()
            : base(PotionEffect.Custom)
        {
            Weight = 1.0;
            Movable = true;
            Hue = 0x3CB;
            Name = "a violently swirling white potion";
        }
        public GreaterRepelUndeadPotion(Serial serial)
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