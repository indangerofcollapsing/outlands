using Server.ContextMenus;
using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items.Misc
{
    public class RestockStone : BaseContainer
    {
        public static TimeSpan CoolDown = TimeSpan.FromMinutes(15);
        public static List<Type> AllowedTypes = new List<Type>() {
            typeof(BlackPearl),
            typeof(Bloodmoss),
            typeof(MandrakeRoot),
            typeof(Garlic),
            typeof(Ginseng),
            typeof(SulfurousAsh),
            typeof(SpidersSilk),
            typeof(Nightshade),
            typeof(Bandage),
            typeof(Arrow),
            typeof(Bolt),
            typeof(TotalRefreshPotion),
            typeof(GreaterAgilityPotion),
            typeof(GreaterStrengthPotion),
            typeof(GreaterCurePotion),
            typeof(GreaterHealPotion),
        };
        public override string DefaultName { get { return "restock stone"; } }

        private Dictionary<String, int> m_Stock;
        private DateTime m_LastStock = DateTime.MinValue;

        [Constructable]
        public RestockStone()
            : base(0x1f13)
        {
            Hue = 1175;
            InitializeStorage();
        }

        private void InitializeStorage()
        {
            m_Stock = new Dictionary<String, int>();
            for (int i = 0; i < AllowedTypes.Count; i++)
            {
                m_Stock[AllowedTypes[i].ToString()] = 0;
            }
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenus.ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            list.Add(new EditRestockStoneEntry(from, this));
        }

        public RestockStone(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(m_Stock.Count);
            foreach (var kvp in m_Stock)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            InitializeStorage();

            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                m_Stock[reader.ReadString()] = reader.ReadInt();
            }
        }

        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            return Add(dropped);
        }

        public bool Add(Item target)
        {
            var type = target.GetType();
            if (AllowedTypes.Contains(type))
            {
                m_Stock[type.ToString()] += target.Amount;
                target.Delete();
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_LastStock + CoolDown > DateTime.UtcNow)
            {
                from.SendMessage("You cannot restock again so soon.");
                return;
            }
        }
    }

    public class EditRestockStoneEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private RestockStone m_Stone;

        public EditRestockStoneEntry(Mobile from, RestockStone stone)
            : base(5101, 1)
        {
            m_From = from;
            m_Stone = stone;
        }

        public override void OnClick()
        {
            //m_From.SendGump(new PlayListGump(m_From, m_Box));
        }
    }
}
