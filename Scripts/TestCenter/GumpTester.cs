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
                //player.CloseGump(typeof(AchievementsGump));
                //player.SendGump(new AchievementsGump(player, AchievementsGump.PageType.Main, 0, AchievementCategory.Battle, 0, 0));

                //Guilds.SendGuildGump(player);

                player.CloseGump(typeof(TestGump));
                player.SendGump(new TestGump(player));
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
        public TestGump(Mobile from) : base(10, 10)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2655;

            AddPage(0);

            AddAlphaRegion(5, 5, 610, 340);

            AddImage(11, 128, 62);
            AddLabel(87, 121, 63, "Bid");
            AddButton(83, 146, 9721, 2151, 0, GumpButtonType.Reply, 0);
            AddImage(51, 9, 30077, 2599);
            AddImage(337, 140, 9801);
            AddImage(350, 152, 11285);
            AddLabel(334, 115, 2401, "Serathi");
            AddLabel(322, 152, textHue, "2");
            AddImage(445, 140, 9801);
            AddImage(458, 152, 11282);
            AddLabel(438, 115, 2401, "Fendrake");
            AddLabel(430, 152, textHue, "3");
            AddImage(547, 140, 9801);
            AddImage(560, 152, 11283);
            AddLabel(532, 115, 2401, "Tiderunner");
            AddLabel(532, 152, textHue, "3");
            AddImage(104, 235, 11284, 2587);
            AddImage(89, 205, 11281);
            AddImage(119, 205, 11282);
            AddImage(59, 205, 11283);
            AddImage(74, 235, 11284, 2587);
            AddButton(129, 84, 9906, 9906, 0, GumpButtonType.Reply, 0);
            AddButton(28, 60, 9900, 9900, 0, GumpButtonType.Reply, 0);
            AddButton(28, 84, 9906, 9906, 0, GumpButtonType.Reply, 0);
            AddLabel(72, 35, 63, "Luthius");
            AddImage(74, 60, 9801);
            AddImage(87, 72, 11284, 2587);
            AddLabel(59, 71, 2587, "3");
            AddImage(223, 140, 9801);
            AddImage(236, 152, 11284);
            AddLabel(205, 115, 2401, "Merrill Calder");
            AddLabel(208, 152, textHue, "2");
            AddBackground(205, 205, 20, 20, 3000);
            AddBackground(235, 205, 20, 20, 3000);
            AddBackground(265, 205, 20, 20, 3000);
            AddBackground(220, 235, 20, 20, 3000);
            AddBackground(250, 235, 20, 20, 3000);
            AddButton(128, 60, 9900, 9900, 0, GumpButtonType.Reply, 0);
            AddButton(220, 262, 4500, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(216, 315, 2550, "Call Liar!");
            AddButton(335, 262, 4500, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(332, 315, 2550, "Call Liar!");
            AddButton(442, 262, 4500, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(439, 315, 2550, "Call Liar!");
            AddButton(544, 262, 4500, 248, 0, GumpButtonType.Reply, 0);
            AddLabel(541, 315, 2550, "Call Liar!");
            AddButton(242, 15, 9809, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(297, 26, 149, "Liars Dice ");
            AddItem(238, 38, 3823);
            AddLabel(298, 46, textHue, "250,000 Pot");
            AddButton(220, 85, 2473, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(254, 88, 1256, "Exit Game");
            AddButton(205, 43, 2094, 2095, 0, GumpButtonType.Reply, 0);
            AddLabel(201, 27, 149, "Guide");
            AddItem(244, 28, 3823);
            AddItem(249, 39, 3823);
            AddLabel(211, 205, textHue, "?");
            AddLabel(241, 205, textHue, "?");
            AddLabel(271, 205, textHue, "?");
            AddLabel(225, 235, textHue, "?");
            AddLabel(256, 235, textHue, "?");
            AddBackground(319, 205, 20, 20, 3000);
            AddBackground(349, 205, 20, 20, 3000);
            AddBackground(379, 205, 20, 20, 3000);
            AddBackground(334, 235, 20, 20, 3000);
            AddLabel(325, 205, textHue, "?");
            AddLabel(355, 205, textHue, "?");
            AddLabel(385, 205, textHue, "?");
            AddLabel(339, 235, textHue, "?");
            AddBackground(427, 205, 20, 20, 3000);
            AddBackground(457, 205, 20, 20, 3000);
            AddBackground(487, 205, 20, 20, 3000);
            AddLabel(433, 205, textHue, "?");
            AddLabel(463, 205, textHue, "?");
            AddLabel(493, 205, textHue, "?");
            AddBackground(529, 205, 20, 20, 3000);
            AddBackground(559, 205, 20, 20, 3000);
            AddBackground(589, 205, 20, 20, 3000);
            AddBackground(544, 235, 20, 20, 3000);
            AddLabel(535, 205, textHue, "?");
            AddLabel(565, 205, textHue, "?");
            AddLabel(595, 205, textHue, "?");
            AddLabel(549, 235, textHue, "?");
        }
    }     
}