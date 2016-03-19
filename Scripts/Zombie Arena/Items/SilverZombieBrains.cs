using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class SilverZombieBrains : BaseZombieBrains
    {
        public override string DefaultName { get { return "a silver brain"; } }

        [Constructable]
        public SilverZombieBrains()
            : base()
        {
            Hue = 2101;
        }

        public override void Eat(Mobile from)
        {
            bool mounted = (from.FindItemOnLayer(Layer.Mount) as IMountItem) != null;

            if (mounted)
            {
                from.SendMessage("You can't eat another one of these so soon!");
            }
            else
            {
				from.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
				from.PlaySound(0x03C);
				from.SendMessage("You devour the silvery brains and immediately feel your legs begin to pulse.");
                from.AddItem(new ZombieMountItem());
                Delete();
            }
        }

        public SilverZombieBrains(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public class ZombieMountItem : MountItem
        {
            public ZombieMountItem()
                : base(null, 0)
            {
                Timer.DelayCall(TimeSpan.FromMinutes(5), Delete );
            }

            public ZombieMountItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)1);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();
            }
        }
    }
}
