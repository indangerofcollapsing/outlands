using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public class PrestigeScroll : Item
    {
        private IndexedRegionName m_Region = IndexedRegionName.Britain;
        [CommandProperty(AccessLevel.Administrator)]
        public IndexedRegionName Region
        {
            get { return m_Region; }
            set { m_Region = value; }
        }

        private int m_InfluenceAmount = 1;
        [CommandProperty(AccessLevel.Administrator)]
        public int InfluenceAmount
        {
            get { return m_InfluenceAmount; }
            set { m_InfluenceAmount = value; }
        }

        [Constructable]
        public PrestigeScroll(IndexedRegionName region, int influenceAmount): this()
        {
            Hue = 2615;

            m_Region = region;
            m_InfluenceAmount = influenceAmount;

            Name = "a prestige scroll";
        }

        [Constructable]
        public PrestigeScroll(IndexedRegionName region): this()
        {
            Name = "a prestige scroll";

            Hue = 2615;

            m_Region = region;
            m_InfluenceAmount = GetRandomInfluenceAmount();
        }

        [Constructable]
        public PrestigeScroll(int influenceAmount): this()
        {
            Name = "a prestige scroll";

            Hue = 2615;

            m_Region = InfluencePersistance.GetRandomRegion();
            m_InfluenceAmount = influenceAmount;
        }

        [Constructable]
        public PrestigeScroll(): base(0x1F65)
        {
            Name = "a prestige scroll";

            Hue = 2615;

            m_Region = InfluencePersistance.GetRandomRegion();
            m_InfluenceAmount = GetRandomInfluenceAmount();
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
            {
                LabelTo(from, "a prestige scroll for " + InfluencePersistance.GetRegionName(Region));                                 
                LabelTo(from, "(" + m_InfluenceAmount.ToString() + " Prestige)");  
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;
            
            InfluencePersistance.CheckCreateInfluenceAccountEntry(player);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            InfluencePersistance.UpdatePlayerPrestigeAvailable(player, m_Region, m_InfluenceAmount);
            player.SendMessage("Your available Prestige to spend in " + InfluencePersistance.GetRegionName(m_Region) + " has increased by " + m_InfluenceAmount.ToString() + ".");

            player.SendSound(0x5A7);
            Delete();
        }

        public int GetRandomInfluenceAmount()
        {
            int influenceValue = 0;

            double randomValue = Utility.RandomDouble();

            if (randomValue >= 0)
                influenceValue = 1;

            if (randomValue >= .80)
                influenceValue = 2;

            if (randomValue >= .99)
                influenceValue = 3;

            return influenceValue;
        }

        public PrestigeScroll(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write((int)m_Region);
            writer.Write(m_InfluenceAmount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Region = (IndexedRegionName)reader.ReadInt();
                m_InfluenceAmount = reader.ReadInt();
            }
        }
    }
}