using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class SlayerGroup
    {
        private static SlayerEntry[] m_TotalEntries;
        private static SlayerGroup[] m_Groups;

        public static SlayerEntry[] TotalEntries
        {
            get { return m_TotalEntries; }
        }

        public static SlayerGroup[] Groups
        {
            get { return m_Groups; }
        }

        public static SlayerEntry GetEntryByName(SlayerName name)
        {
            int v = (int)name;

            if (v >= 0 && v < m_TotalEntries.Length)
                return m_TotalEntries[v];

            return null;
        }

        public static SlayerName GetLootSlayerType(Type type)
        {
            for (int i = 0; i < m_Groups.Length; ++i)
            {
                SlayerGroup group = m_Groups[i];
                Type[] foundOn = group.FoundOn;

                bool inGroup = false;

                for (int j = 0; foundOn != null && !inGroup && j < foundOn.Length; ++j)
                    inGroup = (foundOn[j] == type);

                if (inGroup)
                {
                    int index = Utility.Random(1 + group.Entries.Length);

                    if (index == 0)
                        return group.m_Super.Name;

                    return group.Entries[index - 1].Name;
                }
            }

            return SlayerName.Silver;
        }

        public static SlayerName? GetSlayerForMobType(Type type)
        {
            for (int i = 0; i < m_Groups.Length; ++i)
            {
                SlayerGroup group = m_Groups[i];
                Type[] foundOn = group.Super.Types;

                bool inGroup = false;

                for (int j = 0; foundOn != null && !inGroup && j < foundOn.Length; ++j)
                    inGroup = (foundOn[j] == type);

                if (inGroup)
                {
                    int index = Utility.Random(1 + group.Entries.Length);

                    if (index == 0)
                        return group.m_Super.Name;

                    return group.Entries[index - 1].Name;
                }
            }

            return null;
        }

        static SlayerGroup()
        {
            SlayerGroup humanoid = new SlayerGroup();
            SlayerGroup undead = new SlayerGroup();
            SlayerGroup elemental = new SlayerGroup();
            SlayerGroup abyss = new SlayerGroup();
            SlayerGroup arachnid = new SlayerGroup();
            SlayerGroup reptilian = new SlayerGroup();
            SlayerGroup fey = new SlayerGroup();
            
            m_Groups = new SlayerGroup[]
                {
                    humanoid,
                    undead,
                    elemental,
                    abyss,
                    arachnid,
                    reptilian,
                    fey
                };

            m_TotalEntries = CompileEntries(m_Groups);
        }

        private static SlayerEntry[] CompileEntries(SlayerGroup[] groups)
        {
            SlayerEntry[] entries = new SlayerEntry[28];

            for (int i = 0; i < groups.Length; ++i)
            {
                SlayerGroup g = groups[i];

                g.Super.Group = g;

                entries[(int)g.Super.Name] = g.Super;

                for (int j = 0; j < g.Entries.Length; ++j)
                {
                    g.Entries[j].Group = g;
                    entries[(int)g.Entries[j].Name] = g.Entries[j];
                }
            }

            return entries;
        }

        private SlayerGroup[] m_Opposition;
        private SlayerEntry m_Super;
        private SlayerEntry[] m_Entries;
        private Type[] m_FoundOn;

        public SlayerGroup[] Opposition { get { return m_Opposition; } set { m_Opposition = value; } }
        public SlayerEntry Super { get { return m_Super; } set { m_Super = value; } }
        public SlayerEntry[] Entries { get { return m_Entries; } set { m_Entries = value; } }
        public Type[] FoundOn { get { return m_FoundOn; } set { m_FoundOn = value; } }

        public bool OppositionSuperSlays(Mobile m)
        {
            for (int i = 0; i < Opposition.Length; i++)
            {
                if (Opposition[i].Super.Slays(m))
                    return true;
            }

            return false;
        }

        public SlayerGroup()
        {
        }
    }
}