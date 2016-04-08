using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class GumpTester : Item
    {
        [Constructable]
        public GumpTester(): base(0x26BC)
        {
            Name = "a gump tester"; 
        }

        public GumpTester(Serial serial): base(serial)
        {
        }


        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            from.CloseGump(typeof(TestGump));
            from.SendGump(new TestGump(from));
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

    public class TestGump : Gump
    {
        public TestGump(Mobile from): base(10, 10)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;

            AddPage(0);

            AddPage(0);
            AddImage(133, 371, 103);
            AddImage(2, 0, 103);
            AddImage(131, 0, 103);
            AddImage(264, 0, 103);
            AddImage(264, 88, 103);
            AddImage(264, 181, 103);
            AddImage(264, 276, 103);
            AddImage(2, 86, 103);
            AddImage(2, 180, 103);
            AddImage(2, 277, 103);
            AddImage(3, 372, 103);
            AddImage(264, 371, 103);
            AddBackground(16, 10, 381, 455, 3000);
            AddImage(134, 3, 100);
            AddLabel(192, 30, textHue, @"The");
            AddLabel(175, 50, textHue, @"Rebellion");
            AddButton(15, 11, 2094, 2095, 0, GumpButtonType.Reply, 0);
            AddLabel(43, 14, 149, @"Guide");
            AddLabel(28, 50, 2600, @"Stats");
            AddButton(31, 76, 4018, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(81, 50, textHue, @"Options");
            AddButton(90, 76, 4017, 248, 0, GumpButtonType.Reply, 0);
            AddButton(23, 401, 4002, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(60, 402, textHue, @"Embark/Disembark");
            AddButton(23, 429, 4008, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(60, 430, textHue, @"Embark/Disembark Followers");
            AddButton(261, 398, 4029, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(298, 399, textHue, @"Divide Plunder");
            AddButton(261, 427, 4017, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(298, 428, textHue, @"Dock The Ship");
            AddItem(296, 33, 2539);
            AddLabel(334, 30, 149, @"500");
            AddButton(241, 358, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(228, 249, 2606, @"Ship Epic Ability");
            AddButton(214, 316, 2223, 248, 0, GumpButtonType.Reply, 0);
            AddButton(323, 317, 2224, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(220, 268, textHue, @"Hellfire Ammunition");
            AddLabel(278, 362, 2600, @"Ready");
            AddImage(240, 292, 2328);
            AddItem(251, 275, 710);
            AddButton(84, 358, 9721, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(67, 249, 2603, @"Ship Minor Ability");
            AddButton(56, 316, 2223, 248, 0, GumpButtonType.Reply, 0);
            AddButton(165, 317, 2224, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(76, 268, textHue, @"Boarding Ropes");
            AddLabel(118, 356, 2562, @"Usable in");
            AddImage(82, 292, 2328);
            AddItem(98, 309, 5368);
            AddLabel(121, 372, 2562, @"4m 30s");
            AddImage(64, 125, 103);
            AddImage(201, 125, 103);
            AddImageTiled(78, 139, 257, 77, 2624);
            AddLabel(87, 140, 149, @"Hull");
            AddImage(118, 144, 2057);
            AddLabel(235, 140, textHue, @"10000/10000");
            AddLabel(82, 167, 187, @"Sails");
            AddImage(118, 172, 2054);
            AddLabel(235, 167, textHue, @"125/1500");
            AddLabel(82, 194, textHue, @"Guns");
            AddImage(118, 199, 2057, 2499);
            AddLabel(235, 194, textHue, @"1250/1500");
            AddImageTiled(203, 202, 21, 7, 2488);
            AddImageTiled(143, 174, 80, 7, 2488);
            AddImageTiled(212, 147, 12, 7, 2488);
            AddLabel(306, 50, 149, @"Doubloons");
            AddLabel(317, 69, 149, @"In Hold");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
        }
    }
}