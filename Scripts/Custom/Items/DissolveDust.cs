using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class DissolveDust : Item
    {
        [Constructable]
        public DissolveDust() : base(16954)
        {
            Name = "dissolve dust";

            Hue = 2635;
            Weight = .1;
        }

        public DissolveDust(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(double click to target a newbied item for deletion)");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the newbied item in your pack you wish to delete.");
            from.Target = new NewbieItemDeleteTarget(this);
        }

        public class NewbieItemDeleteTarget : Target
        {
            private DissolveDust m_DissolveDust;

            public NewbieItemDeleteTarget(DissolveDust DissolveDust): base(2, false, TargetFlags.None)
            {
                m_DissolveDust = DissolveDust;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_DissolveDust.Deleted || m_DissolveDust.RootParent != from) return;
                if (from == null) return;

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target an item in your backpack.");
                        return;
                    }

                    if (item.LootType != LootType.Newbied)
                    {
                        from.SendMessage("That item is not newbied.");
                        return;
                    }

                    from.CloseGump(typeof(NewbieItemDeleteGump));
                    from.SendGump(new NewbieItemDeleteGump(from, m_DissolveDust, item));
                }                

                else
                {
                    from.SendMessage("That is not a valid item to delete.");
                    return;
                }
            }
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

            //Version 0
            if (version >= 0)
            {               
            }
        }
    }

    public class NewbieItemDeleteGump : Gump
    {
        private Mobile m_Mobile;
        private DissolveDust m_DissolveDust;
        private Item m_Item;

        public NewbieItemDeleteGump(Mobile mobile, DissolveDust dissolveDust, Item item): base(110, 100)
        {
            m_Mobile = mobile;
            m_DissolveDust = dissolveDust;
            m_Item = item;                       

            Closable = false;

            AddPage(0);

            AddBackground(0, 0, 420, 280, 5054);

            AddImageTiled(10, 10, 400, 20, 2624);
            AddAlphaRegion(10, 10, 400, 20);

            AddHtmlLocalized(10, 10, 400, 20, 1060635, 30720, false, false); // <CENTER>WARNING</CENTER>

            AddImageTiled(10, 40, 400, 200, 2624);
            AddAlphaRegion(10, 40, 400, 200);

            AddHtml(10, 40, 400, 200, "You are about to delete this item permanently. Do you wish to proceed?", true, false);
            //AddHtmlLocalized( 10, 40, 400, 200, 1061795, 32512, false, true );

            AddImageTiled(10, 250, 400, 20, 2624);
            AddAlphaRegion(10, 250, 400, 20);

            AddButton(10, 250, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(40, 250, 170, 20, 1011036, 32767, false, false); // OKAY

            AddButton(210, 250, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddHtmlLocalized(240, 250, 170, 20, 1011012, 32767, false, false); // CANCEL
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (info.ButtonID == 1)
            {
                if (m_Mobile == null) return;
                if (m_Mobile.Deleted || !m_Mobile.Alive) return;

                bool dissolveDustMissing = false;

                if (m_DissolveDust == null)
                    dissolveDustMissing = true;

                if (m_DissolveDust.Deleted || !m_DissolveDust.IsChildOf(m_Mobile.Backpack))
                    dissolveDustMissing = true;

                if (dissolveDustMissing)
                {
                    m_Mobile.SendMessage("The dissolve dust you were using is no longer in your backpack.");
                    return;
                }

                bool itemInvalid = false;

                if (m_Item == null)
                    itemInvalid = true;

                if (m_Item.Deleted || !m_Item.IsChildOf(m_Mobile.Backpack) || m_Item.LootType != LootType.Newbied)
                    itemInvalid = true;

                if (itemInvalid)
                {
                    m_Mobile.SendMessage("The item you wish to delete is either no longer in your backpack or is no longer newbied.");
                    return;
                }

                m_Mobile.SendSound(0x5AE);
                m_Mobile.SendMessage("You delete the item.");

                m_Item.Delete();
                m_DissolveDust.Delete();
            }
        }
    }
}