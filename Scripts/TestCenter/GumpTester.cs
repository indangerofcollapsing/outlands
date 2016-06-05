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
                player.CloseGump(typeof(AchievementsGump));
                player.SendGump(new AchievementsGump(player, AchievementsGump.PageType.Main, 0, AchievementCategory.Warfare, 0, 0));

                //player.CloseGump(typeof(TestGump));
                //player.SendGump(new TestGump(player));
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

            int textHue = 2036;

            AddPage(0);
        }
    }     
}