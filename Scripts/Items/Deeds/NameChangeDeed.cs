using System;
using Server.Network;
using Server.Prompts;
using Server.Mobiles;
using Server.Misc;
using Server.Items;
using System.Collections.Generic;

namespace Server.Items
{
    public class NameChangeDeed : Item
    {
        [Constructable]
        public NameChangeDeed()
            : base(0x14F0)
        {
            base.Weight = 1.0;
            base.Name = "a name change deed";
        }

        public NameChangeDeed(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); writer.Write((int)0); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); int version = reader.ReadInt(); }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) from.SendLocalizedMessage(1042001);
            else
            {
                from.SendMessage("Enter your desired name.");
                from.Prompt = new RenamePrompt(from, this);
            }
        }

        private class RenamePrompt : Prompt
        {
            private Mobile m_from;
            private Item m_NameChangeDeed;

            public RenamePrompt(Mobile from, Item deed)
            {
                m_from = from;
                m_NameChangeDeed = deed;
            }

            public override void OnResponse(Mobile from, string text)
            {
                PlayerMobile pm = (PlayerMobile)from;
                text = text.Trim();
                if (!NameVerification.Validate(text, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote))
                {
                    pm.SendMessage("That name is either already taken or otherwise unnacceptable.");
                    return;
                }

                if (pm.PreviousNames == null)
                {
                    pm.PreviousNames = new List<string>();
                }

                pm.PreviousNames.Add(pm.RawName);

                pm.Name = text;
                pm.SendMessage("You will henceforth by known as {0}.", text);
                m_NameChangeDeed.Delete();
            }
        }
    }
}