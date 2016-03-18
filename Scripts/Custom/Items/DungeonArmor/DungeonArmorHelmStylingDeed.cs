using Server.Commands;
using Server.Engines.Craft;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class DungeonArmorHelmStylingDeed : Item
    {
        public override string DefaultName
        {
            get
            {
                return "a dungeon armor helm styling deed";
            }
        }

        [Constructable]
        public DungeonArmorHelmStylingDeed()
            : base(0x14F0)
        {
            Hue = 2410;
        }

        public DungeonArmorHelmStylingDeed(Serial serial)
            : base(serial)
        {

        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Check(from))
            {
                from.Target = new InternalTarget(from, this);
                from.SendMessage("Select a slayer helm you would like to restyle.");
            }
        }

        private class InternalTarget : Target
        {
            private Mobile m_From;
            private DungeonArmorHelmStylingDeed m_Deed;

            public InternalTarget(Mobile from, DungeonArmorHelmStylingDeed deed)
                : base(2, false, TargetFlags.None)
            {
                m_From = from;
                m_Deed = deed;
            }

            private bool CheckDeed(Mobile from)
            {
                if (m_Deed != null)
                {
                    return m_Deed.Check(from);
                }

                return true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!CheckDeed(from))
                    return;
                
                // This is to ensure that players can't convert the helm to unexpected ID
                var possibleHelmIds = new int[] { 5129, 5131, 5133, 5138, 5051, 5135, 5912 }; 

                //Make sure that taget is the slayer helm
                if (targeted is BaseDungeonArmor && targeted is BaseArmor && possibleHelmIds.Contains(((BaseArmor)targeted).ItemID) && ((Item)targeted).IsChildOf(from))
                {
                    if (from.HasGump(typeof(InternalGump)))
                        from.CloseGump(typeof(InternalGump));
                    from.SendGump(new InternalGump(from, m_Deed, targeted as BaseArmor));
                    from.SendMessage("Warning: this change cannot be undone!");
                }
                else
                {
                    from.SendMessage("The deed can only be used on a slayer helm, which you are wearing or in your backpack.");
                }

            }
        }

        public bool Check(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1047012); // The contract must be in your backpack to use it.
            else
                return true;

            return false;
        }

        public bool VerifyRegion(Mobile m)
        {
            if (!m.Region.IsPartOf(typeof(TownRegion)))
                return false;

            return Server.Factions.Faction.IsNearType(m, typeof(Blacksmith), 6);
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }
        }

        private class InternalGump : Gump
        {
            private Mobile _from;
            private DungeonArmorHelmStylingDeed _deed;
            private BaseArmor _targeted;

            public InternalGump(Mobile from, DungeonArmorHelmStylingDeed deed, BaseArmor targeted)
                : base(50, 50)
            {
                _from = from;
                _deed = deed;
                _targeted = targeted;

                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                AddPage(0);
                AddBackground(0, 0, 231, 375, 9270);
                AddLabel(34, 23, 53, @"Please select the new style");
                AddImage(200, 200, 10410); //dragon
                AddButton(42, 320, 247, 248, 1, GumpButtonType.Reply, 0);
                AddButton(129, 320, 241, 242, 0, GumpButtonType.Reply, 0);
                AddItem(33, 66, 5129);
                AddItem(89, 65, 5131);
                AddItem(146, 63, 5133);
                AddItem(32, 149, 5138);
                AddItem(84, 151, 5051);
                AddItem(154, 148, 5135);
                AddItem(90, 240, 5912);
                AddRadio(46, 100, 209, 208, true, 5129);
                AddRadio(104, 100, 209, 208, false, 5131);
                AddRadio(163, 100, 209, 208, false, 5133);
                AddRadio(45, 186, 209, 208, false, 5138);
                AddRadio(104, 186, 209, 208, false, 5051);
                AddRadio(163, 186, 209, 208, false, 5135);
                AddRadio(104, 272, 209, 208, false, 5912);
            }



            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                switch (info.ButtonID)
                {
                    case 1:
                        {
                            (_targeted).ItemID = info.Switches[0];
                            _deed.Delete();
                            from.SendMessage("The dungeon armor helm has morphed into a new shape.");
                            break;
                        }

                }
            }
        }
    }
}
