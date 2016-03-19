using System;
using System.Collections;
using Server;
using Server.Network;
using Server.Targets;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
    public class ExplodingLlama : ExplodingAnimal
    {
        public override Type CreatureType { get { return typeof(Llama); }}
        public override int CreatureItemId { get { return 8438; } }
        public override String CreatureName { get { return "an exploding llama"; } }
        
        public override int Radius { get { return 5; } }

        public override int MinDamage { get { return 300; } }
        public override int MaxDamage { get { return 400; } }

        [Constructable]
        public ExplodingLlama(): base()
        {
        }

        public ExplodingLlama(Serial serial): base(serial)
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