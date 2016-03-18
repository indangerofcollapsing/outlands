using System;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Commands;
using System.IO;
using System.Text;
using System.Collections;
using System.Net;
using Server.Accounting;
using Server.Items;
using Server.Menus;
using Server.Menus.Questions;
using Server.Menus.ItemLists;
using Server.Spells;
using Server.Targeting;
using Server.Targets;
using Server.Gumps;
using System.Collections.Generic;

namespace Server.Commands
{
    public class RegionParagonMod
    {
        private static Dictionary<Region, double> m_Modifiers = new Dictionary<Region, double>();

        public static void Initialize()
        {
            CommandSystem.Register("RegionParagonMod", AccessLevel.Seer, new CommandEventHandler(RegionParagonMod_OnCommand));
        }

        public static double GetModifier(Region region)
        {
            if (m_Modifiers.ContainsKey(region))            
                return m_Modifiers[region];
            
            else            
                return 0.0;            
        }

        public static void SetModifier(Region region, double mod)
        {
            mod = Math.Min(mod, 1.0);
            mod = Math.Max(mod, 0.0);

            if (mod == 0.0)            
                m_Modifiers.Remove(region);
            
            else            
                m_Modifiers[region] = mod;            
        }

        [Usage("RegionParagonMod (optional:<region> <mod>)")]
        [Description("Sets a paragon modifier for a specific region.")]
        public static void RegionParagonMod_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Length == 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<BASEFONT COLOR=WHITE>MODIFIED PARAGON REGIONS<br><br>");

                foreach (var kvp in m_Modifiers)
                {
                    sb.Append(kvp.Key.Name);
                    sb.Append(": ");
                    sb.Append(kvp.Value);
                    sb.Append("<br>");
                }

                from.SendGump(new TextGump(sb.ToString()));

                return;
            }

            if (e.Length == 2)
            {
                try
                {
                    string name = e.GetString(0);
                    double mod = e.GetDouble(1);

                    var region = FindRegion(from, name);

                    if (region == null)
                    {
                        from.SendMessage("No region with that name was found.");
                    }
                    else
                    {
                        SetModifier(region, mod);
                        from.SendMessage(String.Format("The paragon modifier for \"{0}\" has been set to {1}", region.Name, mod));
                    }
                }
                catch
                {
                    from.SendMessage("Error in arguments. Usage: [RegionParagonMod \"name\" 0.5 (50% paragons)");
                }
            }
        }

        public static Region FindRegion(Mobile from, string name)
        {
            Dictionary<string, Region> list = from.Map.Regions;

            foreach (KeyValuePair<string, Region> kvp in list)
            {
                Region r = kvp.Value;

                if (Insensitive.Equals(r.Name, name))
                {
                    return r;
                }
            }

            for (int i = 0; i < Map.AllMaps.Count; ++i)
            {
                Map m = Map.AllMaps[i];

                if (m.MapIndex == 0x7F || m.MapIndex == 0xFF || from.Map == m)
                    continue;

                foreach (Region r in m.Regions.Values)
                {
                    if (Insensitive.Equals(r.Name, name))
                    {
                        return r;
                    }
                }
            }

            return null;
        }

        private class TextGump : Server.Gumps.Gump
        {
            public TextGump(string strHTML)
                : base(100, 100)
            {
                this.Closable = true;
                this.Disposable = true;
                this.Dragable = true;
                this.Resizable = true;
                this.AddPage(0);
                AddBackground(0, 0, 260, 500, 0xE10);
                AddAlphaRegion(15, 15, 230, 470);
                this.AddHtml(20, 20, 220, 460, strHTML, (bool)false, (bool)true);
            }
        }
    }
}
