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
                player.CloseGump(typeof(DamageTrackerGump));
                player.SendGump(new DamageTrackerGump(player));
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

            AddImage(13, 5, 206, 2419);
            AddImage(645, 5, 207, 2419);
            AddImage(13, 169, 202, 2419);
            AddImage(517, 361, 200, 2419);
            AddImage(389, 361, 200, 2419);
            AddImage(312, 361, 200, 2419);
            AddImage(184, 361, 200, 2419);
            AddImage(56, 361, 200, 2419);
            AddImage(518, 240, 200, 2419);
            AddImage(519, 170, 200, 2419);
            AddImage(518, 49, 200, 2419);
            AddImage(395, 240, 200, 2419);
            AddImage(312, 240, 200, 2419);
            AddImage(184, 240, 200, 2419);
            AddImage(56, 240, 200, 2419);
            AddImage(56, 170, 200, 2419);
            AddImage(184, 170, 200, 2419);
            AddImage(312, 170, 200, 2419);
            AddImage(398, 170, 200, 2419);
            AddImage(56, 49, 200, 2419);
            AddImage(184, 49, 200, 2419);
            AddImage(312, 49, 200, 2419);
            AddImage(390, 49, 200, 2419);
            AddImage(57, 485, 233, 2419);
            AddImage(56, 5, 201, 2419);
            AddImage(13, 49, 202, 2419);
            AddImage(646, 49, 203, 2419);
            AddImage(645, 485, 205, 2419);
            AddImage(13, 485, 204, 2419);
            AddImage(218, 5, 201, 2419);
            AddImage(218, 485, 233, 2419);
            AddImage(645, 169, 203, 2419);

            AddImage(31, 22, 9002, 2412);
            AddImage(223, 27, 1143, 2499);
            AddLabel(314, 29, 149, @"Donation Shop");

            AddButton(163, 475, 9724, 248, 0, GumpButtonType.Reply, 0);
            AddItem(119, 470, 5447);
            AddLabel(142, 445, 149, @"Masks");
            AddButton(77, 478, 4014, 248, 0, GumpButtonType.Reply, 0);
            AddButton(262, 475, 9721, 248, 0, GumpButtonType.Reply, 0);
            AddItem(225, 477, 5910);
            AddLabel(237, 445, textHue, @"Clothing");
            AddButton(609, 478, 4005, 248, 0, GumpButtonType.Reply, 0);
            AddButton(376, 475, 9721, 248, 0, GumpButtonType.Reply, 0);
            AddItem(329, 464, 3670);
            AddLabel(329, 445, textHue, @"Decorations");
            AddButton(468, 475, 9721, 248, 0, GumpButtonType.Reply, 0);
            AddItem(427, 476, 4009);
            AddLabel(428, 445, textHue, @"Clothing Dyes");
            AddButton(566, 475, 9721, 248, 0, GumpButtonType.Reply, 0);
            AddItem(524, 472, 5359);
            AddLabel(546, 449, textHue, @"Deeds");
            AddButton(29, 10, 2094, 2095, 0, GumpButtonType.Reply, 0);
            AddLabel(56, 19, textHue, @"Guide");
            AddButton(598, 409, 2151, 2151, 0, GumpButtonType.Reply, 0);
            AddLabel(570, 386, 169, @"Make Donation");
            AddLabel(473, 366, textHue, @"10,000");
            AddLabel(454, 386, 149, @"Silver in Bank");
            AddButton(436, 224, 9900, 248, 0, GumpButtonType.Reply, 0);
            AddButton(436, 265, 9906, 248, 0, GumpButtonType.Reply, 0);
            AddItem(474, 337, 3823, 2500);
            AddItem(546, 251, 2760);
            AddItem(524, 229, 2760);
            AddItem(545, 208, 2760);
            AddItem(502, 251, 2768);
            AddItem(523, 272, 2768);
            AddItem(566, 198, 2766);
            AddItem(525, 194, 2765);
            AddItem(630, 254, 2764);
            AddItem(590, 252, 2760);
            AddItem(568, 273, 2760);
            AddItem(568, 229, 2760);
            AddItem(589, 218, 2766);
            AddItem(544, 186, 2762);
            AddItem(502, 216, 2765);
            AddItem(608, 237, 2766);
            AddItem(544, 292, 2768);
            AddItem(481, 233, 2763);
            AddItem(611, 271, 2767);
            AddItem(590, 293, 2767);
            AddItem(566, 314, 2761);
            AddItem(505, 149, 3225);
            AddItem(618, 175, 3229);
            AddItem(482, 175, 3228);
            AddItem(475, 218, 555);
            AddItem(484, 166, 9);
            AddItem(605, 241, 3651);
            AddItem(491, 237, 554);
            AddItem(508, 254, 554);
            AddItem(556, 300, 554);
            AddItem(570, 313, 555, 2415);
            AddItem(586, 295, 555);
            AddItem(496, 196, 555);
            AddItem(517, 173, 555);
            AddItem(604, 274, 555);
            AddItem(525, 166, 555);
            AddItem(567, 168, 554);
            AddItem(584, 184, 554);
            AddItem(599, 198, 554);
            AddItem(616, 214, 554);
            AddItem(640, 237, 554);
            AddItem(484, 140, 9);
            AddItem(543, 111, 9);
            AddItem(543, 54, 9);
            AddItem(630, 186, 9);
            AddItem(630, 129, 9);
            AddItem(618, 259, 555);
            AddItem(538, 186, 3644);
            AddItem(540, 178, 3647);
            AddItem(506, 32, 1539, 149);
            AddItem(486, 50, 1561, 149);
            AddItem(467, 80, 1561, 149);
            AddItem(526, 23, 1536, 149);
            AddItem(530, 54, 1539, 2500);
            AddItem(510, 72, 1561, 2500);
            AddItem(491, 102, 1561, 2500);
            AddItem(550, 45, 1536, 2500);
            AddItem(552, 77, 1539, 149);
            AddItem(532, 95, 1561, 149);
            AddItem(513, 125, 1561, 149);
            AddItem(572, 68, 1536, 149);
            AddItem(573, 99, 1539, 2500);
            AddItem(553, 117, 1561, 2500);
            AddItem(534, 147, 1561, 2500);
            AddItem(593, 90, 1536, 2500);
            AddItem(594, 122, 1539, 149);
            AddItem(575, 139, 1561, 149);
            AddItem(614, 113, 1536, 149);
            AddItem(615, 143, 1539, 2500);
            AddItem(595, 161, 1561, 2500);
            AddItem(635, 134, 1536, 2500);
            AddItem(524, 215, 2818);
            AddItem(547, 236, 2817);
            AddItem(568, 258, 2816);
            AddItem(532, 215, 3656);
            AddItem(512, 226, 3628);
            AddItem(557, 279, 5452, 2500);
            AddItem(552, 222, 4644);
            AddItem(538, 238, 5357);
            AddItem(522, 234, 3838);
            AddItem(553, 256, 2886, 2606);
            AddItem(574, 261, 3629, 2500);
            AddItem(552, 244, 2586, 2587);
            AddItem(564, 245, 9);
            AddItem(564, 210, 9);
            AddItem(573, 314, 554);
            AddItem(582, 288, 3228);
            AddItem(555, 170, 1561, 149);
            AddItem(576, 190, 1561, 2500);


            AddImage(75, 83, 103, 2412);
            AddImage(206, 83, 103, 2412);
            AddImage(277, 83, 103, 2412);
            AddBackground(84, 91, 323, 81, 3000);

            AddItem(151, 163, 3823, 2500);
            AddItem(84, 85, 3805);
            AddLabel(192, 165, 2519, @"10,000");
            AddLabel(305, 164, 169, @"Purchase");
            AddButton(272, 160, 2152, 2154, 0, GumpButtonType.Reply, 0);
            AddLabel(146, 100, textHue, @"Terrify your neighbors with this ominous");
            AddLabel(152, 120, textHue, @"reminder of their impending mortality!");
            AddLabel(182, 140, textHue, @"Usable while locked down.");
            AddLabel(191, 71, 149, @"Ancient Gravestone");
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