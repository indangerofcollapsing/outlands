using System;
using System.Collections.Generic;
using System.Diagnostics;

using Server;
using Server.Gumps;
using Server.Commands;
using Server.Mobiles;

using Server.Custom;
using Server.Network;
using Server.Targeting;
using Server.Items;

namespace Server.Custom
{
    public static class Titles
    {

        #region Command Region
        public static void Initialize()
        {
            CommandSystem.Register("Titles", AccessLevel.Player, new CommandEventHandler(Titles_OnCommand));
            CommandSystem.Register("GiveTitle", AccessLevel.GameMaster, new CommandEventHandler(GiveTitle_OnCommand));
            CommandSystem.Register("RemoveTitle", AccessLevel.GameMaster, new CommandEventHandler(RemoveTitle_OnCommand));
        }

        private class GiveTitleTarget : Target
        {
            public string m_TitleToGive;
            public GiveTitleTarget() : base(-1, true, TargetFlags.None) { }
            protected override void OnTarget(Mobile from, object o)
            {
                PlayerMobile pm = o as PlayerMobile;
                if (pm != null && m_TitleToGive.Length > 0)
                {
                    if (!pm.TitlesPrefix.Contains(m_TitleToGive))
                    {
                        pm.TitlesPrefix.Add(m_TitleToGive);
                    }
                }
            }
        }

        private class RemoveTitleTarget : Target
        {
            public string m_TitleToRemove;

            public RemoveTitleTarget() : base(-1, true, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targeted)
            {
                PlayerMobile pm = targeted as PlayerMobile;
                if (pm != null && m_TitleToRemove.Length > 0)
                {
                    if (pm.TitlesPrefix.Contains(m_TitleToRemove))
                    {
                        pm.TitlesPrefix.Remove(m_TitleToRemove);
                    }
                }
            }
        }

        [Usage("GiveTitle")]
        [Description("Allows a player to access and change the titles associated with him/her.")]
        public static void GiveTitle_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length != 1)
                e.Mobile.SendMessage("Format: GiveTitle \"title\"");
            else if (e.Arguments[0].Length == 0)
                e.Mobile.SendMessage("Format: GiveTitle \"title\"");
            else
                e.Mobile.Target = new GiveTitleTarget() { m_TitleToGive = e.Arguments[0] };
        }

        [Usage("RemoveTitle")]
        [Description("Allows a game master to remove a title from a player.")]
        public static void RemoveTitle_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length != 1)
                e.Mobile.SendMessage("Format: RemoveTitle \"title\"");
            else if (e.Arguments[0].Length == 0)
                e.Mobile.SendMessage("Format: RemoveTitle \"title\"");
            else
                e.Mobile.Target = new RemoveTitleTarget() { m_TitleToRemove = e.Arguments[0] };
        }

        [Usage("Titles")]
        [Description("Allows a player to access and change the titles associated with him/her.")]
        public static void Titles_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                if (!from.HasGump(typeof(TitlesGump)))
                    from.SendGump(new TitlesGump(from, 0));
                else
                    from.SendMessage("You already have a titles gump open.");
            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: [Titles");
            }
        }
        #endregion
    }
}

namespace Server.Gumps
{
    public class TitlesGump : Gump
    {
        private List<string> m_Prefixes;
        private int m_PageNoPrefix;

        public TitlesGump(Mobile from, int PageNoPrefix)
            : base(200, 50)
        {
            m_PageNoPrefix = PageNoPrefix;

            #region ANTI NULLIFY.. KABOOOOOM
            if (((PlayerMobile)from).TitlesPrefix == null)
                ((PlayerMobile)from).TitlesPrefix = new List<String>();
            if (from.Title == null)
                from.Title = "";
            if (((PlayerMobile)from).CurrentPrefix == null)
                ((PlayerMobile)from).CurrentPrefix = "";
            #endregion

            #region Static Content

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddImage(0, 44, 206);
            this.AddImageTiled(44, 85, 427, 318, 200);
            this.AddImage(44, 44, 201);
            this.AddImage(471, 44, 207);
            this.AddImage(0, 88, 202);
            this.AddLabel(71, 123, 0x0, @"Earned Titles");
            this.AddImage(471, 87, 203);
            this.AddImage(0, 403, 204);
            this.AddImage(471, 403, 205);
            this.AddImage(293, 143, 96);
            this.AddImage(38, 143, 96);
            this.AddImage(44, 403, 233);
            this.AddImage(142, 22, 1419);
            this.AddCheck(68, 72, 210, 211, !((PlayerMobile)from).m_UserOptHideFameTitles, (int)Buttons.chkFameTitle);
            this.AddCheck(68, 92, 210, 211, ((PlayerMobile)from).CurrentPrefix.Length > 0 ? true : false, (int)Buttons.chkPrefix);
            this.AddButton(227, 403, 238, 240, (int)Buttons.btnApply, GumpButtonType.Reply, 0);
            this.AddImage(218, 4, 1417);
            this.AddImage(229, 13, 5608);
            this.AddLabel(89, 72, 0x0, @"Display Fame");
            this.AddLabel(89, 92, 0x0, @"Display Title");

            // title color section
            PlayerMobile pm = from as PlayerMobile;

            this.AddLabel(324, 123, 0x0, @"Title Color");
            int current_hue = PlayerTitleColors.GetLabelColorValue(pm.SelectedTitleColorIndex, pm.SelectedTitleColorRarity);
            this.AddLabel(320, 155, current_hue, string.Format("{0:d4}: {1}", current_hue, pm.CurrentPrefix.Length > 0 ? pm.CurrentPrefix : "********"));
            this.AddButton(285, 153, 0xfa5, 0xfa7, (int)Buttons.btnSelectColor, GumpButtonType.Reply, 0);

            //Prefix buttons
            if (m_PageNoPrefix > 0)
                this.AddButton(23, 154, 250, 251, (int)Buttons.btnUpPrefix, GumpButtonType.Reply, 0);

            #endregion

            m_Prefixes = new List<string>();
            
            m_Prefixes.AddRange(((PlayerMobile)from).TitlesPrefix);

            int countPrefix = m_Prefixes.Count - m_PageNoPrefix * 10;

            if (countPrefix > 10)
            {
                this.AddButton(23, 379, 252, 253, (int)Buttons.btnDownPrefix, GumpButtonType.Reply, 0);
                countPrefix = 10;
            }

            AddGroup(0);
            int buttonID = 11;

            for (int i = 0; i < countPrefix; i++)
            {
                int index = i + m_PageNoPrefix * 10;

                if (index < 0 || index >= m_Prefixes.Count)
                    break;
                
                this.AddRadio(41, 155 + i * 25, 208, 209, pm.CurrentPrefix == m_Prefixes[index] ? true : false, buttonID++);
                this.AddLabel(64, 155 + i * 25, 0x0, String.Format("{0}. {1}", i + 1 + m_PageNoPrefix * 10, m_Prefixes[index]));                
            }
        }

        public enum Buttons
        {
            btnCancel,
            btnApply,
            chkPrefix,
            btnDownPrefix,
            btnUpPrefix,
            chkFameTitle,
            btnSelectColor,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            base.OnResponse(sender, info);
            Mobile from = sender.Mobile;
            int prefixID = -1;

            if (info.Switches.Length > 0)
            {
                for (int i = 0; i < info.Switches.Length; i++)
                {
                    if (info.Switches[i] > 10 && info.Switches[i] < 21)
                        prefixID = info.Switches[i];
                }

            }
            if (info.ButtonID == (int)Buttons.btnApply)
            {
                if (info.IsSwitched((int)Buttons.chkPrefix))
                {
                    if (prefixID > 0)
                    {
                        int index = prefixID - 11 + m_PageNoPrefix * 10;
                        if (index >= 0 && index < m_Prefixes.Count)
                            ((PlayerMobile)from).CurrentPrefix = m_Prefixes[index];
                    }
                }
                else
                {
                    ((PlayerMobile)from).CurrentPrefix = "";
                }

                from.Title = "";

                ((PlayerMobile)from).m_UserOptHideFameTitles = !info.IsSwitched((int)Buttons.chkFameTitle);
                from.InvalidateProperties();

            }
            else if (info.ButtonID == (int)Buttons.btnDownPrefix)
            {
                from.SendGump(new TitlesGump(from, ++m_PageNoPrefix));
            }
            else if (info.ButtonID == (int)Buttons.btnUpPrefix)
            {
                from.SendGump(new TitlesGump(from, --m_PageNoPrefix));
            }
            else if (info.ButtonID == (int)Buttons.btnSelectColor)
            {
                from.SendGump(new TitlesColorsGump(from, Server.Custom.EColorRarity.Common));
            }
        }
    }


    public class TitlesColorsGump : Gump
    {
        private Server.Custom.EColorRarity m_ColorRarity;

        enum Buttons
        {
            btnCommon = 1,
            btnUncommon,
            btnRare,
            btnUnique,
            btnUnlockAll,

            btnSelectionRangeStart = 1000,
        }


        public TitlesColorsGump(Mobile from, Server.Custom.EColorRarity color_rarity)
            : base(200, 50)
        {
            from.CloseGump(typeof(TitlesColorsGump));
            m_ColorRarity = color_rarity;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddImage(0, 44, 206);
            this.AddImageTiled(44, 85, 427, 318, 200);
            this.AddImage(44, 44, 201);
            this.AddImage(471, 44, 207);
            this.AddImage(0, 88, 202);
            this.AddImage(471, 87, 203);
            this.AddImage(0, 403, 204);
            this.AddImage(471, 403, 205);
            this.AddImage(44, 403, 233);


            int buttx = 100;
            this.AddButton(buttx, 85, 0xfa5, 0xfa7, (int)Buttons.btnCommon, GumpButtonType.Reply, 0);
            this.AddButton(buttx, 110, 0xfa5, 0xfa7, (int)Buttons.btnUncommon, GumpButtonType.Reply, 0);
            this.AddButton(buttx, 135, 0xfa5, 0xfa7, (int)Buttons.btnRare, GumpButtonType.Reply, 0);
            this.AddButton(buttx, 160, 0xfa5, 0xfa7, (int)Buttons.btnUnique, GumpButtonType.Reply, 0);

            int textx = 35;
            this.AddLabel(textx, 85, 53, "Common");
            this.AddLabel(textx, 110, 53, "Uncommon");
            this.AddLabel(textx, 135, 53, "Rare");
            this.AddLabel(textx, 160, 53, "Very Rare");

            if (from.AccessLevel > AccessLevel.Player)
            {
                this.AddButton(buttx, 220, 0xfa5, 0xfa7, (int)Buttons.btnUnlockAll, GumpButtonType.Reply, 0);
                this.AddLabel(textx, 220, 53, "Unlock All");
            }

            string demo_string = "*****";

            Debug.Assert(Server.Custom.PlayerTitleColors.GetNumColorsForRarity(m_ColorRarity) <= 48); // if this hits we have changed the max number and this gump needs some layout-love

            PlayerMobile pm = from as PlayerMobile;
            if (pm != null)
            {
                int total_unlocked = (int)pm.TitleColorState.GetNumUnlockedColors(color_rarity);
                total_unlocked = Math.Min(total_unlocked, PlayerTitleColors.GetNumColorsForRarity(color_rarity));
                this.AddLabel(190, 58, 53, String.Format("{0}s - {1}/{2} unlocked.", Server.Custom.PlayerTitleColors.ColorRarityNames[(int)m_ColorRarity], total_unlocked, Server.Custom.PlayerTitleColors.GetNumColorsForRarity(m_ColorRarity)));

                int xcolor = 160;
                int ycolor = 85;
                for (int i = 0; i < 16; ++i)
                {
                    int hue = PlayerTitleColors.GetLabelColorValue(i, color_rarity);
                    if (hue == 0)
                        continue;

                    AddLabel(xcolor, ycolor, hue, string.Format("{0:D4}: {1}", hue, demo_string));
                    if (pm.TitleColorState.IsColorUnlocked(i, color_rarity))
                        AddButton(xcolor + 105, ycolor, 0xfb7, 0xfb9, (int)Buttons.btnSelectionRangeStart + i, GumpButtonType.Reply, 0);
                    ycolor += 21;

                }

                xcolor = 310;
                ycolor = 85;
                for (int i = 16; i < 32; ++i)
                {
                    int hue = PlayerTitleColors.GetLabelColorValue(i, color_rarity);
                    if (hue == 0)
                        continue;

                    AddLabel(xcolor, ycolor, hue, string.Format("{0:D4}: {1}", hue, demo_string));
                    if (pm.TitleColorState.IsColorUnlocked(i, color_rarity))
                        AddButton(xcolor + 105, ycolor, 0xfb7, 0xfb9, (int)Buttons.btnSelectionRangeStart + i, GumpButtonType.Reply, 0);
                    ycolor += 21;

                }

                xcolor = 380;
                ycolor = 85;
                for (int i = 32; i < 48; ++i)
                {
                    int hue = PlayerTitleColors.GetLabelColorValue(i, color_rarity);
                    if (hue == 0)
                        continue;

                    AddLabel(xcolor, ycolor, hue, string.Format("{0:D4}: {1}", hue, demo_string));
                    if (pm.TitleColorState.IsColorUnlocked(i, color_rarity))
                        AddButton(xcolor + 105, ycolor, 0xfb7, 0xfb9, (int)Buttons.btnSelectionRangeStart + i, GumpButtonType.Reply, 0);
                    ycolor += 21;
                }
            }
        }
        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.btnCommon)
            {
                sender.Mobile.SendGump(new TitlesColorsGump(sender.Mobile, Server.Custom.EColorRarity.Common));
            }
            else if (info.ButtonID == (int)Buttons.btnUncommon)
            {
                sender.Mobile.SendGump(new TitlesColorsGump(sender.Mobile, Server.Custom.EColorRarity.Uncommon));
            }
            else if (info.ButtonID == (int)Buttons.btnRare)
            {
                sender.Mobile.SendGump(new TitlesColorsGump(sender.Mobile, Server.Custom.EColorRarity.Rare));
            }
            else if (info.ButtonID == (int)Buttons.btnUnique)
            {
                sender.Mobile.SendGump(new TitlesColorsGump(sender.Mobile, Server.Custom.EColorRarity.VeryRare));
            }
            else if (info.ButtonID == (int)Buttons.btnUnlockAll)
            {
                if (sender.Mobile.AccessLevel > AccessLevel.Player)
                {
                    PlayerMobile pm = sender.Mobile as PlayerMobile;
                    if (pm != null)
                        pm.TitleColorState.UnlockAllColors();
                }
            }
            else if (info.ButtonID >= (int)Buttons.btnSelectionRangeStart)
            {
                int which_color_idx = info.ButtonID - (int)Buttons.btnSelectionRangeStart;
                PlayerMobile pm = sender.Mobile as PlayerMobile;
                if (pm != null)
                {
                    if (PlayerTitleColors.GetNumColorsForRarity(m_ColorRarity) > which_color_idx && pm.TitleColorState.IsColorUnlocked(which_color_idx, m_ColorRarity))
                    {
                        pm.SelectedTitleColorIndex = which_color_idx;
                        pm.SelectedTitleColorRarity = m_ColorRarity;
                        sender.Mobile.SendGump(new TitlesColorsGump(sender.Mobile, m_ColorRarity));
                    }
                }
            }
        }
    }




    //////////////////////////////////////////////////////////////////////////
    // TEST GUMP
    //////////////////////////////////////////////////////////////////////////

    public class TestTitleColorsGump : Gump
    {
        private int m_StartColorIdx;

        enum Buttons
        {
            next = 1,
            prev,
            btnSelectionRangeStart = 1000,
        }


        public TestTitleColorsGump(Mobile from, int start_color_idx)
            : base(200, 50)
        {
            m_StartColorIdx = Math.Max(start_color_idx, 0);

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            this.AddPage(0);
            this.AddImage(0, 44, 206);
            this.AddImageTiled(44, 85, 427, 318, 200);
            this.AddImage(44, 44, 201);
            this.AddImage(471, 44, 207);
            this.AddImage(0, 88, 202);
            this.AddImage(471, 87, 203);
            this.AddImage(0, 403, 204);
            this.AddImage(471, 403, 205);
            this.AddImage(44, 403, 233);


            int buttx = 100;
            this.AddButton(buttx, 85, 0xfa5, 0xfa7, (int)Buttons.next, GumpButtonType.Reply, 0);
            this.AddButton(buttx, 110, 0xfa5, 0xfa7, (int)Buttons.prev, GumpButtonType.Reply, 0);

            int textx = 35;
            this.AddLabel(textx, 85, 53, "next");
            this.AddLabel(textx, 110, 53, "prev");

            string demo_string = "*** {0}";

            PlayerMobile pm = from as PlayerMobile;
            int start_index = m_StartColorIdx;
            if (pm != null)
            {
                int xcolor = 140;
                int ycolor = 85;
                for (int i = start_index; i < start_index + 16; ++i)
                {
                    AddLabel(xcolor, ycolor, i, String.Format(demo_string, i));
                    AddButton(xcolor + 70, ycolor, 0xfb7, 0xfb9, (int)Buttons.btnSelectionRangeStart + i, GumpButtonType.Reply, 0);
                    ycolor += 21;

                }

                xcolor = 250;
                ycolor = 85;
                for (int i = start_index + 16; i < start_index + 32; ++i)
                {
                    AddLabel(xcolor, ycolor, i, String.Format(demo_string, i));
                    AddButton(xcolor + 70, ycolor, 0xfb7, 0xfb9, (int)Buttons.btnSelectionRangeStart + i, GumpButtonType.Reply, 0);
                    ycolor += 21;

                }

                xcolor = 360;
                ycolor = 85;
                for (int i = start_index + 32; i < start_index + 48; ++i)
                {
                    AddLabel(xcolor, ycolor, i, String.Format(demo_string, i));
                    AddButton(xcolor + 70, ycolor, 0xfb7, 0xfb9, (int)Buttons.btnSelectionRangeStart + i, GumpButtonType.Reply, 0);
                    ycolor += 21;
                }
            }
        }
        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == (int)Buttons.prev)
            {
                sender.Mobile.SendGump(new TestTitleColorsGump(sender.Mobile, m_StartColorIdx - 48));
            }
            else if (info.ButtonID == (int)Buttons.next)
            {
                sender.Mobile.SendGump(new TestTitleColorsGump(sender.Mobile, m_StartColorIdx + 48));
            }
            else if (info.ButtonID >= (int)Buttons.btnSelectionRangeStart)
            {
                int which_color = info.ButtonID - (int)Buttons.btnSelectionRangeStart;

                // THE TRICK:
                // Label colors are always +1 compared to spoken hues for some reason.
                // Meaning label color 53 is spoken color 54
                PlayerMobile pm = sender.Mobile as PlayerMobile;
                if (pm != null)
                {
                    pm.PrivateOverheadMessage(MessageType.Label, which_color + 1, true, "This is the hue when spoken!!", pm.NetState);
                    sender.Mobile.SendGump(new TestTitleColorsGump(sender.Mobile, m_StartColorIdx));
                }
            }
        }

        public static void Initialize()
        {
            CommandSystem.Register("TitleColorTest", AccessLevel.GameMaster, new CommandEventHandler(TitleColorTest_OnCommand));
        }
        [Usage("TitleColorTest")]
        [Description("List and test all possible title colors.")]
        public static void TitleColorTest_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendGump(new TestTitleColorsGump(from, 0));
        }
    }
}