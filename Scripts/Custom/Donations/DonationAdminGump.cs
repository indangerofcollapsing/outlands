using System;
using System.Collections.Generic;
using Server.Custom.Townsystem;
using Server.Items;
using Server.Commands;
using System.IO;

namespace Server.Gumps
{
    public class DonationAdminGump : Server.Gumps.Gump
    {
        public static void Initialize()
        {
            Server.Commands.CommandSystem.Register("DonationAdmin", AccessLevel.Administrator, new CommandEventHandler(DonationAdmin_OnCommand));
        }

        [Usage("DonationAdmin")]
        [Description("DonationAdmin")]
        public static void DonationAdmin_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.SendGump(new DonationAdminGump());
        }

        public DonationAdminGump()
            : base(100, 100)
        {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);
            this.AddBackground(5, 13, 247, 189, 9200);
            this.AddImage(20, 95, 2501);
            this.AddImage(171, 94, 2444);
            this.AddLabel(30, 78, 0, @"Character Name");
            this.AddLabel(176, 78, 0, @"Amount");

            AddGroup(1);

            this.AddHtml(14, 33, 228, 27, @"<center>Donation Admin Gump</center>", true, false);
            this.AddButton(175, 170, 239, 240, (int)Buttons.Apply, GumpButtonType.Reply, 0);
            this.AddTextEntry(25, 96, 130, 15, 0, (int)Buttons.Name, @"");
            this.AddTextEntry(179, 97, 47, 17, 0, (int)Buttons.Amount, @"");
        }

        public enum Buttons
        {
            Cancel,
            Apply,
            Name,
            Amount,
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (info.ButtonID == 0)
                return;

            Mobile from = sender.Mobile;

            if (from == null)
                return;

            TextRelay name = info.GetTextEntry((int)Buttons.Name);

            if (name.Text == null || name.Text.Length == 0)
            {
                from.SendGump(new DonationAdminGump());
                return;
            }

            string donator = name.Text.Trim();

            Mobile toGive = Outcasts.FromName(donator);

            if (toGive == null)
            {
                from.SendMessage("No player by the name of {0} can be found.", donator);
                from.SendGump(new DonationAdminGump());
                return;
            }

            int amount;
            Int32.TryParse(info.GetTextEntry((int)Buttons.Amount).Text, out amount);

            if (amount == 0)
            {
                from.SendMessage("Invalid amount.", donator);
                from.SendGump(new DonationAdminGump());
                return;
            }

            DragonCoin coin = new DragonCoin(amount);

            if (toGive.AddToBackpack(coin))
            {
                from.SendMessage("{0} dragon coin has been awarded to {1}.", amount, donator);
                using (StreamWriter op = new StreamWriter("donation_currency.log", true))
                {
                    op.Write("{0}\t : {1} gave {2} {3} dragon coins.", DateTime.UtcNow.ToShortDateString(), from.Name.ToUpper(), toGive.Name.ToUpper(), amount);
                    op.WriteLine();
                }
            }
            else
            {
                from.SendMessage("{0}'s backpack is currently full and cannot hold these coins. Try again later.", donator);
                coin.Delete();
            }

            from.SendGump(new DonationAdminGump());
        }
    }
}