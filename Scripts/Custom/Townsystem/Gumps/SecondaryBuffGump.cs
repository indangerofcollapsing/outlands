using Server.Gumps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Townsystem.Gumps
{
    class SecondaryBuffGump : Gump
    {
        private Town m_Town;
        private Mobile m_From;
        private static CitizenshipBuffs[] values = (CitizenshipBuffs[])Enum.GetValues(typeof(CitizenshipBuffs));
        enum Buttons
        {
            OK,
            Cancel
        }

        public SecondaryBuffGump(Mobile from, Town town)
            : base(50, 50)
        {
            m_Town = town;
            m_From = from;

            if (from.AccessLevel == AccessLevel.Player && !m_Town.IsKing(from))
            {
                from.SendMessage("Only the King is allowed to make changes to the King's Stone.");
                return;
            }
            if (!m_Town.CanUpdateSecondaryBuffs)
                return;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(56, 33, 320, 435, 9270);
            AddLabel(140, 50, m_Town.HomeFaction.Definition.HuePrimary-1, string.Format("{0}'s Secondary Buffs", m_Town.Definition.FriendlyName));
            AddButton(225, 425, 238, 240, (int)Buttons.OK, GumpButtonType.Reply, 0);
            AddButton(295, 425, 242, 241, (int)Buttons.Cancel, GumpButtonType.Reply, 0);

            AddLabel(155, 75, 1153, string.Format("{0} days until upkeep", ((m_Town.LastBuffUpkeep + Town.BuffUpkeepPeriod) - DateTime.UtcNow).Days));
            AddLabel(120, 95, 1153, string.Format("{0} prorated amount per buff", m_Town.ProrateSecondaryBuffAmount));

            int y = 125;
            // skip none value
            for(int i = 1; i < values.Length; i++) 
            {
                var buff = values[i];
                // don't show primary buff
                if (buff == m_Town.PrimaryCitizenshipBuff && !m_Town.SecondaryCitizenshipBuffs.Contains(buff)) continue;

                AddGroup(i);
                AddCheck(85, y, 210, 211, m_Town.HasActiveBuff(buff), i);
                AddLabel(115, y, 1153, TownBuff.GetBuffName(buff));

                y += 25;
            }
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {
            if (sender.Mobile.AccessLevel == AccessLevel.Player && !m_Town.IsKing(sender.Mobile))
            {
                sender.Mobile.SendMessage("Only the King is allowed to make changes to the King's Stone.");
                return;
            }

            if (!m_Town.CanUpdateSecondaryBuffs)
                return;

            switch ((Buttons)info.ButtonID)
            {
                case Buttons.Cancel:
                    break;
                case Buttons.OK:
                    UpdateBuffs(info);
                    break;
            }

        }

        private void UpdateBuffs(RelayInfo info)
        {
            var removals = new List<CitizenshipBuffs>();
            var added = new List<CitizenshipBuffs>();

            for (int i = 1; i < values.Length; i++)
            {
                var buff = values[i];
                if (buff == m_Town.PrimaryCitizenshipBuff) continue;

                bool marked = info.IsSwitched(i);
                if (m_Town.HasActiveBuff(buff))
                {
                    if (!marked)
                        removals.Add(buff);
                }
                else if (marked)
                    added.Add(buff);
            }

            m_Town.AdjustSecondaryBuffs(m_From, added, removals);
        }
    }

    class SecondaryBuffViewGump : Gump
    {
        private Town m_Town;
        private static CitizenshipBuffs[] values = (CitizenshipBuffs[])Enum.GetValues(typeof(CitizenshipBuffs));

        public SecondaryBuffViewGump(Town town)
            : base(50, 50)
        {
            m_Town = town;

            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            AddPage(0);
            AddBackground(56, 33, 320, 435, 9270);
            AddLabel(138, 52, m_Town.HomeFaction.Definition.HuePrimary - 1, string.Format("{0}'s Secondary Buffs", m_Town.Definition.FriendlyName));
            AddButton(295, 425, 242, 241, 0, GumpButtonType.Reply, 0);

            int y = 125;
            // skip none value
            for (int i = 1; i < values.Length; i++)
            {
                var buff = values[i];
                // don't show primary buff
                if (buff == m_Town.PrimaryCitizenshipBuff) continue;

                AddGroup(i);
                AddCheck(85, y, 210, 211, m_Town.HasActiveBuff(buff), i);
                AddLabel(115, y, 1153, TownBuff.GetBuffName(buff));

                y += 25;
            }
        }
    }
}
