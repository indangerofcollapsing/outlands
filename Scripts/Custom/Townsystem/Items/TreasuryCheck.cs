using System;
using System.Collections.Generic;
using Server.Custom.Townsystem;

namespace Server.Items
{
    public class TreasuryCheck : Item
    {
        private static string[] m_Names = new string[] { "a treasury voucher", "a treasury food stamp", "a treasury bond", "a treasury claim ticket", "a treasury check", "a treasury cheque", "treasury monies", "treasury billz"};

        [CommandProperty(AccessLevel.GameMaster)]
        public int GoldWorth { get; set; }

		[Constructable]
        public TreasuryCheck(int worth = 100)
            : base(5360)
        {
            Name = m_Names[Utility.Random(m_Names.Length)];
            GoldWorth = worth;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, String.Format("[Worth: {0}]", GoldWorth));
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Drop this on the town's treasury of your choice to deposit the funds.");
        }


        public TreasuryCheck(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(GoldWorth);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            GoldWorth = reader.ReadInt();
        }
    }
}
