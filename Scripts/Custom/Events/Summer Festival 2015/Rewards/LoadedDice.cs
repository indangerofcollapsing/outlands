using System;
using Server.Network;

namespace Server.Items
{
    public class LoadedDice : Item, ITelekinesisable
    {
        public override bool AlwaysAllowDoubleClick { get { return true; } }

        [Constructable]
        public LoadedDice() : base(0xFA7)
        {
            Name = "loaded dice";
            Weight = 1.0;
        }

        public LoadedDice(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 2))
                return;

            Roll(from);
        }

        public void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles(EffectItem.Create(Location, Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5022);
            Effects.PlaySound(Location, Map, 0x1F5);

            Roll(from);
        }

        public void Roll(Mobile from)
        {
            PublicOverheadMessage(MessageType.Regular, 0, false, string.Format("*{0} rolls {1}, {2}*", from.Name, Utility.RandomMinMax(4, 6), Utility.RandomMinMax(4, 6)));
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