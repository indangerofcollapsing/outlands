using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class TransmutationEssence : Item
    {
        [Constructable]
        public TransmutationEssence() : base(6204)
        {
            Name = "a vial of transmutation essence";

            Hue = 2603;
            Weight = 5;
        }

        public TransmutationEssence(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.SendMessage("Transmutation essence can be distilled with the alchemy skill into a transmutation stone, which transforms dungeon armor from one type to another.");
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version          
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}