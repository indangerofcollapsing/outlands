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
        public enum AchivementCategory
        {
            Warfare,
            Seafaring,
            AnimalTaming,
            Crafting,

            Adventuring,
            Luxury,
            Harvesting,
            SkillMastery,

            Slaying,
            Competition,
            Virtue,
            Vice
        }

        public TestGump(Mobile from): base(10, 10)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;            

            AddPage(0);

            int iStartX = 0;
            int iStartY = 0;

            int rowSpacing = 140;
            int columnSpacing = 125;            

            int rows = 3;
            int columns = 4;

            int categoryIndex = 0;

            for (int a = 0; a < rows; a++)
            {
                for (int b = 0; b < columns; b++)
                {
                    AchivementCategory category = (AchivementCategory)categoryIndex;

                    #region Images

                    switch (category)
                    {
                        case AchivementCategory.Warfare:
                            //Warfare
                            AddItem(iStartX + 19, iStartY + 34, 18210, 2500);
                            AddItem(iStartX + 21, iStartY + 33, 5049, 2500);
                            AddItem(iStartX + 14, iStartY + 22, 5138, 2500);
                            AddItem(iStartX + 4, iStartY + 40, 7028, 2500);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 1256);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 1256);
                            AddLabel(iStartX + 41, iStartY + 5, 1256, "Warfare");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);
                        break;

                        case AchivementCategory.Adventuring:
                            //Adventuring
                            AddItem(iStartX + 4, iStartY + 19, 3226);
                            AddItem(iStartX + 19, iStartY + 44, 4967);
                            AddItem(iStartX + 11, iStartY + 55, 4970);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 149);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 149);
                            AddItem(iStartX + 26, iStartY + 69, 2648);
                            AddItem(iStartX + -4, iStartY + 58, 5356);
                            AddItem(iStartX + 2, iStartY + 65, 3922);
                            AddItem(iStartX + -37, iStartY + 50, 3898);
                            AddLabel(iStartX + 27, iStartY + 5, 148, "Adventuring");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);     
                        break;

                        case AchivementCategory.Slaying:
                            //Slaying
                            AddItem(iStartX + 3, iStartY + 31, 7433);
                            AddItem(iStartX + 8, iStartY + 56, 4655);
                            AddItem(iStartX + 21, iStartY + 20, 7438);
                            AddItem(iStartX + 45, iStartY + 34, 7419);
                            AddItem(iStartX + 68, iStartY + 33, 7418);
                            AddItem(iStartX + 7, iStartY + 54, 7782);
                            AddItem(iStartX + 43, iStartY + 68, 7430);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2116);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2116);
                            AddItem(iStartX + 23, iStartY + 52, 3910);
                            AddLabel(iStartX + 44, iStartY + 5, 2116, "Slaying");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);
                        break;

                        case AchivementCategory.Seafaring:
                            //Seafaring
                            AddItem(iStartX + 5, iStartY + 50, 5363);
                            AddItem(iStartX + 23, iStartY + 60, 5365);
                            AddItem(iStartX + 24, iStartY + 37, 5370);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2602);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2602);
                            AddLabel(iStartX + 36, iStartY + 5, 2603, "Seafaring");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);  
                        break;

                        case AchivementCategory.Luxury:
                            //Luxury
                            AddItem(iStartX + -3, iStartY + 68, 2448, 2425);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2618);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2618);
                            AddItem(iStartX + 19, iStartY + 30, 16508);
                            AddItem(iStartX + 1, iStartY + 63, 2459, 2562);
                            AddItem(iStartX + -8, iStartY + 63, 2459, 2600);
                            AddItem(iStartX + -4, iStartY + 67, 2459, 2606);
                            AddLabel(iStartX + 43, iStartY + 5, 2618, "Luxury");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0); 
                        break;

                        case AchivementCategory.Competition:
                            //Competition
                            AddItem(iStartX + 7, iStartY + 40, 16434);
                            AddItem(iStartX + 18, iStartY + 26, 16433);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2963);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2963);
                            AddItem(iStartX + 8, iStartY + 35, 4006);
                            AddItem(iStartX + 0, iStartY + 48, 4008, 2500);
                            AddItem(iStartX + 10, iStartY + 34, 4008, 1107);
                            AddLabel(iStartX + 26, iStartY + 5, 2962, "Competition");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);  
                        break;

                        case AchivementCategory.AnimalTaming:
                            //Animal Taming           
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2208);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2208);
                            AddItem(iStartX + 13, iStartY + 49, 2476);
                            AddItem(iStartX + 10, iStartY + 40, 3191);
                            AddItem(iStartX + 14, iStartY + 38, 3191);
                            AddItem(iStartX + 13, iStartY + 39, 3713);
                            AddLabel(iStartX + 23, iStartY + 5, 2599, "Animal Taming");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);
                        break;

                        case AchivementCategory.Harvesting:
                            //Harvesting
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2417);
                            AddItem(iStartX + 27, iStartY + 44, 3346, 2208);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2417);
                            AddItem(iStartX + 6, iStartY + 33, 3670);
                            AddItem(iStartX + 36, iStartY + 29, 3351, 2208);
                            AddItem(iStartX + -15, iStartY + 79, 3352, 2208);
                            AddItem(iStartX + 2, iStartY + 65, 3344, 2208);
                            AddItem(iStartX + 18, iStartY + 62, 7137);
                            AddItem(iStartX + 27, iStartY + 57, 3908);
                            AddItem(iStartX + 28, iStartY + 71, 2482, 2500);
                            AddLabel(iStartX + 28, iStartY + 5, 2417, "Harvesting");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);
                        break;

                        case AchivementCategory.Virtue:
                            //Virtue
                            AddItem(iStartX + -5, iStartY + 16, 2);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2589);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2589);
                            AddItem(iStartX + 15, iStartY + 18, 3);
                            AddItem(iStartX + 23, iStartY + 79, 3618);
                            AddItem(iStartX + 28, iStartY + 60, 3619);
                            AddLabel(iStartX + 48, iStartY + 5, 2590, "Virtue");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);
                        break;

                        case AchivementCategory.Crafting:
                            //Crafting
                            AddItem(iStartX + -10, iStartY + 41, 4142);
                            AddItem(iStartX + 1, iStartY + 41, 4150);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2401);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2401);
                            AddItem(iStartX + 22, iStartY + 49, 2920);
                            AddItem(iStartX + 10, iStartY + 39, 2921);
                            AddItem(iStartX + 40, iStartY + 77, 4148);
                            AddItem(iStartX + 9, iStartY + 45, 4189);
                            AddItem(iStartX + 22, iStartY + 61, 4179);
                            AddItem(iStartX + 3, iStartY + 53, 4139);
                            AddItem(iStartX + 21, iStartY + 43, 2581);
                            AddItem(iStartX + 3, iStartY + 29, 2503);
                            AddItem(iStartX + 11, iStartY + 22, 4172);
                            AddLabel(iStartX + 36, iStartY + 5, 2036, "Crafting");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);
                        break;

                        case AchivementCategory.SkillMastery:
                            //Skill Mastery            
                            AddImage(iStartX + 60, iStartY + 32, 10550, 2652);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 2652);
                            AddItem(iStartX + 21, iStartY + 42, 2942);
                            AddItem(iStartX + -4, iStartY + 28, 2943);
                            AddItem(iStartX + 16, iStartY + 33, 2507);
                            AddItem(iStartX + 15, iStartY + 49, 4030);
                            AddItem(iStartX + 28, iStartY + 28, 7716);
                            AddItem(iStartX + -4, iStartY + 20, 7717, 2652);
                            AddItem(iStartX + 18, iStartY + 39, 4031);
                            AddLabel(iStartX + 21, iStartY + 5, 2652, "Skill Mastery");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);  
                        break;

                        case AchivementCategory.Vice:
                            //Vice           
                            AddItem(iStartX + 27, iStartY + 71, 6872);
                            AddItem(iStartX + 6, iStartY + 51, 6873);
                            AddItem(iStartX + -10, iStartY + 42, 6874);
                            AddItem(iStartX + 25, iStartY + 32, 6875);
                            AddItem(iStartX + 51, iStartY + 52, 6876);
                            AddItem(iStartX + 69, iStartY + 46, 6877);
                            AddItem(iStartX + 14, iStartY + 84, 6880);
                            AddImage(iStartX + 60, iStartY + 32, 10550, 1106);
                            AddImage(iStartX + 88, iStartY + 32, 10552, 1106);
                            AddItem(iStartX + -17, iStartY + 83, 6883);
                            AddLabel(iStartX + 48, iStartY + 5, 1106, "Vice");
                            AddButton(iStartX + 74, iStartY + 46, 9721, 2151, 0, GumpButtonType.Reply, 0);  
                        break;
                    }

                    #endregion

                    categoryIndex++;
                    iStartX += columnSpacing;

                    if (b == (columns - 1))
                    {
                        iStartX = 0;
                        iStartY += rowSpacing;
                    }
                }   
            }         
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