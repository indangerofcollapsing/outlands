using Server.Misc;
using Server.Mobiles;
using Server.Prompts;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class ItemRenameDeed : Item
    {
        public override string DefaultName
        {
            get
            {
                return "Armor/Weapon Rename Deed";
            }
        }

        [Constructable]
        public ItemRenameDeed()
            : base(5360)
        {

        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1010095); //Must be on your person to use.
                return;
            }

            from.SendMessage("Target the item you wish to rename.");
            from.Target = new InternalTarget(this);

            base.OnDoubleClick(from);
        }

        public ItemRenameDeed(Serial serial)
            : base(serial)
        {

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        private class InternalTarget : Target
        {
            private ItemRenameDeed m_Deed;

            public InternalTarget(ItemRenameDeed deed)
                : base(12, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                // can't rename dungeon armor or armored hats
                if ((!(targeted is BaseArmor) && !(targeted is BaseWeapon)) || targeted is BaseDungeonArmor || !(targeted is Item))
                {
                    from.SendMessage("You cannot rename that.");
                    return;
                }

                from.SendMessage("Enter the desired name.");
                from.Prompt = new RenamePrompt(from, m_Deed, (Item)targeted);
            }
        }

        private class RenamePrompt : Prompt
        {
            private Mobile m_From;
            private ItemRenameDeed m_Deed;
            private Item m_Target;

            public RenamePrompt(Mobile from, ItemRenameDeed deed, Item target)
            {
                m_From = from;
                m_Deed = deed;
                m_Target = target;
            }

            public override void OnResponse(Mobile from, string text)
            {
                PlayerMobile pm = (PlayerMobile)from;
                text = text.Trim();
                if (!NameVerification.Validate(text, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote, false))
                {
                    pm.SendMessage("That name is unnacceptable.");
                    return;
                }
                m_Target.Name = text;
                pm.SendMessage("Your item will henceforth by known as {0}.", text);
                m_Deed.Delete();
            }
        }
    }
}
