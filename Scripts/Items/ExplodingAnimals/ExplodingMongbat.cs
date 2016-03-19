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
    public class ExplodingMongbat : ExplodingAnimal
    {
        public override Type CreatureType { get { return typeof(Mongbat); }}
        public override int CreatureItemId { get { return 8441; } }
        public override String CreatureName { get { return "an exploding mongbat"; } }
        
        public override int Radius { get { return 4; } }

        public override int MinDamage { get { return 300; } }
        public override int MaxDamage { get { return 400; } }

        [Constructable]
        public ExplodingMongbat(): base()
        {
        }

        public ExplodingMongbat(Serial serial): base(serial)
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