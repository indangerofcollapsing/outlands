using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;

using Server;
using Server.Diagnostics;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;

using Server.Multis;
using Server.Accounting;
using Server.Gumps;

namespace Server.Commands
{
    public class PlayerWealth
    {
        public static void Initialize()
        {
            CommandSystem.Register("PlayerWealth", AccessLevel.Counselor, new CommandEventHandler(PlayerWealth_OnCommand));
            CommandSystem.Register("AllPlayerWealthGold", AccessLevel.Counselor, new CommandEventHandler(AllPlayerWealthGold_OnCommand));
        }

        private static void ParseAccount(Accounting.Account acc, ref int gold)
        {
            List<BaseHouse> checked_houses = new List<BaseHouse>();
            for (int i = 0; i < acc.accountMobiles.Length; ++i)
            {
                ParseCharacter(acc.accountMobiles[i] as PlayerMobile, ref gold, ref checked_houses);
            }
        }

        private static void ParseCharacter(PlayerMobile pm, ref int gold, ref List<BaseHouse> checked_houses)
        {
            if (pm == null)
                return;


            // bank, backpack and house secures
            ParseContainer(pm.Backpack, ref gold);
            ParseContainer(pm.BankBox, ref gold);

            List<BaseHouse> houses = BaseHouse.GetHouses(pm);
            foreach (BaseHouse h in houses)
            {
                if (h == null || checked_houses.Contains(h))
                    continue;
                foreach (SecureInfo info in h.Secures)
                {
                    if (info != null)
                        ParseContainer(info.Item, ref gold);
                }
                checked_houses.Add(h);
            }
        }

        private static void ParseContainer(Container c, ref int gold)
        {
            if (c != null)
            {
                List<Gold> bp_gold = c.FindItemsByType<Gold>();
                foreach (Gold bpg in bp_gold)
                    gold += bpg.Amount;

                List<BankCheck> bp_checks = c.FindItemsByType<BankCheck>();
                foreach (BankCheck bc in bp_checks)
                    gold += bc.Worth;
            }
        }

        [Usage("PlayerWealth")]
        [Description("Shows total gold for the targeted players account")]
        private static void PlayerWealth_OnCommand(CommandEventArgs e)
        {
            e.Mobile.Target = new PlayerWealthTarget();
            e.Mobile.SendMessage("Whom do you wish to inspect?");
        }


        internal class Entry : IComparable<Entry>
        {
            public int gold;
            public string account;
            public string characters;

            public int CompareTo(Entry other)
            {
                if (other == null)
                    return 1;
                else
                    return other.gold - gold;
            }
        };

        [Usage("AllPlayerWealthGold_OnCommand")]
        [Description("Shows total gold for the targeted players account")]
        private static void AllPlayerWealthGold_OnCommand(CommandEventArgs args)
        {
            List<Entry> entries = GetAllPlayerWealthGold();
            args.Mobile.SendGump(new WealthTrackerGump(entries, "Top GOLD owners"));
        }


        internal static List<Entry> GetAllPlayerWealthGold()
        {
            List<Entry> entries = new List<Entry>();
            foreach (Account a in Accounts.GetAccounts())
            {
                if (!a.Banned && a.AccessLevel == AccessLevel.Player)
                {
                    Entry e = new Entry();
                    ParseAccount(a, ref e.gold);
                    e.account = a.Username;
                    if (a.accountMobiles.Any())
                        e.characters = string.Join(", ", a.accountMobiles.Where(x => x != null).Select(x => x.Name));
                    entries.Add(e);
                }
            }

            entries.Sort();
            return entries;
        }

        private class WealthTrackerGump : Gump
        {
            public WealthTrackerGump(List<Entry> entries, string title)
                : base(0, 0)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = false;

                this.AddPage(0);
                this.AddImage(0, 44, 206);
                this.AddImageTiled(44, 85, 427, 318, 200);
                this.AddImage(44, 44, 201);
                this.AddImage(471, 44, 207);
                this.AddImage(471, 87, 203);

                this.AddImage(0, 88, 202);
                this.AddImage(0, 403, 204);
                this.AddImage(471, 403, 205);
                this.AddImage(44, 403, 233);
                this.AddImage(142, 22, 1419);
                this.AddButton(232, 403, 0x81c, 0x81b, 0, GumpButtonType.Reply, 0);
                this.AddImage(218, 4, 1417);
                this.AddImage(229, 13, 5608);


                this.AddImageTiled(51, 143, 400, 3, 0x238e);//horizontal bar

                int label_hue = 2036;
                this.AddLabel(205, 93, 53, title);
                this.AddLabel(90, 123, label_hue, @"ACCOUNT");
                this.AddLabel(240, 123, label_hue, @"GOLD");

                int entry_hue = 53;
                int y = 150;
                int delta = 22;
                int count = Math.Min(entries.Count, 10);
                for (int i = 0; i < count; ++i)
                {

                    AddLabel(90, y + (delta * i), entry_hue, entries[i].account);
                    AddLabel(240, y + (delta * i), entry_hue, entries[i].gold.ToString());
                }
            }
        }

        private class PlayerWealthTarget : Target
        {
            public PlayerWealthTarget()
                : base(15, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targ)
            {
                PlayerMobile pm = targ as PlayerMobile;
                if (pm == null)
                {
                    from.SendMessage("You must target a player");
                    return;
                }

                int total_gold = 0;

                // foreach character on account
                Accounting.Account acc = (Accounting.Account)pm.Account;
                ParseAccount(acc, ref total_gold);
                from.SendMessage(String.Format("Total for the Account \"{0}\":", acc.Username));
                from.SendMessage(String.Format("Gold: {0}", total_gold));
            }
        }
    }


}