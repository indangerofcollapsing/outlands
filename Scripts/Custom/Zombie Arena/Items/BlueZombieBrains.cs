using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Custom;

namespace Server.Items
{
    public class BlueZombieBrains : BaseZombieBrains
    {
        public override string DefaultName { get { return "a blue brain"; } }

        [Constructable]
        public BlueZombieBrains()
            : base()
        {
            Hue = 6;
        }

        public override void Eat(Mobile from)
        {
            if (from.Hits == from.HitsMax)
            {
                from.SendMessage("You can't eat another one of these so soon!");
            }
            else 
            {
                from.FixedParticles(0x376A, 9, 32, 5007, EffectLayer.Waist);
                from.PlaySound(0x03C);
                from.SendMessage("You devour the bluish brains and immediately feel your body regenerate.");
                from.Hits = from.HitsMax;
                Delete();
            }
        }

        public BlueZombieBrains(Serial serial)
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
