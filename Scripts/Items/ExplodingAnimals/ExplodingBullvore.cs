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
    public class ExplodingBullvore : ExplodingAnimal
    {
        public override Type CreatureType { get { return typeof(Bullvore); }}
        public override int CreatureItemId { get { return 8464; } }
        public override String CreatureName { get { return "an exploding bullvore"; } }
        
        public override int Radius { get { return 7; } }

        public override int MinDamage { get { return 400; } }
        public override int MaxDamage { get { return 500; } }

        [Constructable]
        public ExplodingBullvore(): base()
        {
        }

        public ExplodingBullvore(Serial serial): base(serial)
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