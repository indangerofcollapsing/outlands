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
    public class ExplodingDragon : ExplodingAnimal
    {
        public override Type CreatureType { get { return typeof(Dragon); }}
        public override int CreatureItemId { get { return 9780; } }
        public override String CreatureName { get { return "an exploding dragon"; } }
        
        public override int Radius { get { return 9; } }

        public override int MinDamage { get { return 500; } }
        public override int MaxDamage { get { return 600; } }

        [Constructable]
        public ExplodingDragon(): base()
        {
        }

        public ExplodingDragon(Serial serial): base(serial)
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