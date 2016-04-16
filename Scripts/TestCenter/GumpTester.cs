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

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                player.CloseGump(typeof(BoatHotbarGump));
                player.SendGump(new BoatHotbarGump(player));
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

            AddAlphaRegion(0, 0, 304, 392);

            //Directions            
            AddButton(129, 206, 4500, 248, 1, GumpButtonType.Reply, 0);
            AddButton(189, 206, 4501, 248, 2, GumpButtonType.Reply, 0);
            AddButton(236, 249, 4502, 248, 3, GumpButtonType.Reply, 0);
            AddButton(189, 294, 4503, 248, 4, GumpButtonType.Reply, 0);
            AddButton(129, 293, 4504, 248, 5, GumpButtonType.Reply, 0);
            AddButton(69, 293, 4505, 248, 6, GumpButtonType.Reply, 0);
            AddButton(24, 249, 4506, 248, 7, GumpButtonType.Reply, 0);
            AddButton(69, 206, 4507, 248, 8, GumpButtonType.Reply, 0);
           
            //Center Controls
            AddButton(83, 264, 4014, 248, 9, GumpButtonType.Reply, 0);
            AddButton(140, 264, 4017, 248, 10, GumpButtonType.Reply, 0);
            AddButton(196, 264, 4007, 248, 11, GumpButtonType.Reply, 0);

            //Movement Mode
            AddLabel(44, 345, 187, "Movement Mode");
            AddLabel(72, 368, textHue, "Single");
            AddButton(43, 372, 2223, 248, 12, GumpButtonType.Reply, 0);
            AddButton(119, 372, 2224, 248, 13, GumpButtonType.Reply, 0);
            
            //Action
            AddLabel(164, 345, 169, "Embark/Disembark");
            AddButton(171, 372, 2223, 248, 14, GumpButtonType.Reply, 0);
            AddButton(242, 372, 2224, 248, 15, GumpButtonType.Reply, 0);
            AddButton(202, 367, 4029, 248, 16, GumpButtonType.Reply, 0);

            //Left Cannon
            AddItem(2, 143, 733);
            AddButton(28, 210, 2151, 2151, 17, GumpButtonType.Reply, 0);
            AddLabel(10, 182, textHue, "10");

            //Right Cannon
            AddItem(253, 146, 709);
            AddButton(256, 212, 2151, 2151, 18, GumpButtonType.Reply, 0);
            AddLabel(285, 184, textHue, "10");

           //Targeting Mode
            AddLabel(105, 159, 2115, "Targeting Mode");
            AddLabel(131, 178, textHue, "Random");
            AddButton(96, 182, 2223, 248, 19, GumpButtonType.Reply, 0);
            AddButton(190, 182, 2224, 248, 20, GumpButtonType.Reply, 0);
           
            //Minor Ability
            AddLabel(33, 76, 2603, "Ship Minor Ability");
            AddLabel(46, 94, textHue, "Mastercraft");
            AddLabel(61, 109, textHue, "Cannons");
            AddButton(19, 105, 2223, 248, 21, GumpButtonType.Reply, 0);
            AddButton(132, 105, 2224, 248, 22, GumpButtonType.Reply, 0);
            AddButton(68, 131, 9721, 2151, 23, GumpButtonType.Reply, 0);
            AddLabel(21, 124, 2562, "4m 30s");

            //Epic Ability
            AddLabel(176, 76, 2606, "Ship Epic Ability");
            AddLabel(202, 94, textHue, "Hellfire");
            AddLabel(189, 109, textHue, "Ammunition");
            AddButton(162, 105, 2223, 248, 24, GumpButtonType.Reply, 0);
            AddButton(270, 105, 2224, 248, 25, GumpButtonType.Reply, 0);
            AddButton(211, 131, 9721, 2151, 26, GumpButtonType.Reply, 0);
            AddLabel(244, 127, 2600, "4m 30s");
           
            //Ship Toggle
            AddItem(-1, 6, 5363);
            AddButton(13, 35, 1210, 248, 27, GumpButtonType.Reply, 0);
            
            //Stats
            AddLabel(48, 7, 149, "Hull");
            AddImage(79, 11, 2057);
            AddImageTiled(173, 14, 12, 7, 2488);
            AddLabel(196, 7, textHue, "9500/10000");

            AddLabel(43, 27, 187, "Sails");
            AddImage(79, 32, 2054);            
            AddImageTiled(104, 34, 80, 7, 2488);
            AddLabel(196, 27, textHue, "125/1500");

            AddLabel(43, 46, textHue, "Guns");
            AddImage(79, 51, 2057, 2499);            
            AddImageTiled(164, 54, 21, 7, 2488);
            AddLabel(196, 47, textHue, "1250/1500");
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            bool closeGump = true;

            from.Say(info.ButtonID.ToString());

            if (info.ButtonID >= 1 && info.ButtonID <= 24)
                closeGump = false;

            if (!closeGump)
            {
                from.CloseGump(typeof(TestGump));
                from.SendGump(new TestGump(from));
            }
        }
    }
}