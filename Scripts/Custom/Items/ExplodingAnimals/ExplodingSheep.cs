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
    public class ExplodingSheep : ExplodingAnimal
    {
        public override Type CreatureType { get { return typeof(Sheep); }}
        public override int CreatureItemId { get { return 8427; } }
        public override String CreatureName { get { return "an exploding sheep"; } }
        
        public override int Radius { get { return 3; } }

        public override int MinDamage { get { return 200; } }
        public override int MaxDamage { get { return 300; } }

        [Constructable]
        public ExplodingSheep(): base()
        {
        }

        public ExplodingSheep(Serial serial): base(serial)
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