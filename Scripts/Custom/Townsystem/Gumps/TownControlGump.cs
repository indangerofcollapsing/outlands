using System;
using Server;
using Server.Gumps;
using Server.Custom.Townsystem;
using Server.Commands;
using System.Collections.Generic;
using Server.Custom;

namespace Server.Gumps
{
    public class TownControlGump : Gump
    {
        [Usage("[ControlMap")]
        [Description("Views the control map.")]
        public static void ControlMap_OnCommand(CommandEventArgs e)
        {
            Mobile caller = e.Mobile;

            if (caller.HasGump(typeof(TownControlGump)))
                caller.CloseGump(typeof(TownControlGump));

            caller.SendGump(new TownControlGump());
        }

        public static Town Britain;
        public static Town Minoc;
        public static Town Vesper;
        public static Town Cove;
        public static Town Yew;
        public static Town Ocllo;
        public static Town Magincia;
        public static Town SerpentsHold;
        public static Town Nujelm;
        public static Town Moonglow;
        public static Town SkaraBrae;
        public static Town Jhelom;
        public static Town Trinsic;


        public static void Initialize()
        {
            CommandSystem.Register("ControlMap", AccessLevel.Player, new CommandEventHandler(ControlMap_OnCommand));

            Britain = Town.Parse("Britain");
            Minoc = Town.Parse("Minoc");
            Vesper = Town.Parse("Vesper");
            Cove = Town.Parse("Cove");
            Yew = Town.Parse("Yew");
            Ocllo = Town.Parse("Ocllo");
            Magincia = Town.Parse("Magincia");
            SerpentsHold = Town.Parse("Serpent's Hold");
            Nujelm = Town.Parse("Nujel'm");
            Moonglow = Town.Parse("Moonglow");
            SkaraBrae = Town.Parse("Skara Brae");
            Jhelom = Town.Parse("Jhelom");
            Trinsic = Town.Parse("Trinsic");
        }


        public TownControlGump()
            : base(0, 0)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            #region static content
            AddPage(0);
            AddImage(264, -1, 3501);
            AddImage(-2, 23, 3503);
            AddImage(24, 0, 3501);
            AddImage(270, 353, 3507);
            AddImageTiled(8, 10, 182, 131, 3504);
            AddImage(-4, -3, 5528);
            AddImage(-2, 0, 3500);
            AddImage(368, 22, 3503);
            AddImageTiled(394, 9, 116, 359, 3504);
            AddImage(368, -1, 3500);
            AddImage(368, 130, 3503);
            AddImage(506, -1, 3502);
            AddImage(506, 25, 3505);
            AddImage(506, 129, 3505);
            AddImage(506, 353, 3508);
            AddImage(368, 353, 3506);
            #endregion

            int spacer = 0;

            ++spacer;

            for (int i = 0; i < Town.Towns.Count; i++)
            {
                // side bar of towns
                var town = Town.Towns[i];
				AddLabel(390, 15 + 20 * ++spacer, town.HomeFaction.Definition.HuePrimary - 1, town.Definition.FriendlyName);
            }

			if (Yew != null)
			{
				int hue = Yew.ControllingTown.HomeFaction.Definition.HuePrimary -1;
				AddImage(44, 59, 22228, hue);
			}

            if (Minoc != null)
            {
                int hue = Minoc.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(185, 17, 22228, hue);
            }

            if (Vesper != null)
            {
                int hue = Vesper.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(216, 56, 22228, hue);
            }

            if (Britain != null)
            {
                int hue = Britain.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(110, 126, 22228, hue);
            }

            if (Nujelm != null)
            {
                int hue = Nujelm.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(275, 85, 22228, hue);
            }

            if (Moonglow != null)
            {
                int hue = Moonglow.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(335, 87, 22228, hue);
            }

            if (SkaraBrae != null)
            {
                int hue = SkaraBrae.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(42, 172, 22228, hue);
            }

            if (Magincia != null)
            {
                int hue = Magincia.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(278, 165, 22228, hue);
            }

            if (Ocllo != null)
            {
                int hue = Ocllo.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(276, 212, 22228, hue);
            }

            if (Trinsic != null)
            {
                int hue = Trinsic.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(141, 229, 22228, hue);
            }

            if (Jhelom != null)
            {
                int hue = Jhelom.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(99, 316, 22228, hue);
            }

            if (SerpentsHold != null)
            {
                int hue = SerpentsHold.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(222, 287, 22228, hue);
            }

            if (Cove != null)
            {
                int hue = Cove.ControllingTown.HomeFaction.Definition.HuePrimary - 1;
                AddImage(165, 84, 22228, hue);
            }

            if (WindBattleground.Owner != null)
            {
                AddLabel(390, 15 + 20 * ++spacer, 1153, "Wind Owner");
                AddLabel(410, 15 + 20 * ++spacer, WindBattleground.Owner.HomeFaction.Definition.HuePrimary - 1, WindBattleground.Owner.HomeFaction.Definition.FriendlyName);
            }
        }
    }
}