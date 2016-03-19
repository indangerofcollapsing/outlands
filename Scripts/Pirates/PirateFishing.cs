using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Targeting;
using Server.Multis;
using Server.Mobiles;
using System;

namespace Server.Custom.Pirates
{
    public static class PirateFishing
    {
        #region Commands
        public static void Initialize()
        {
            CommandSystem.Register("AddToFishlist", AccessLevel.GameMaster, new CommandEventHandler(AddToFishlist_OnCommand));
        }

        [Usage("AddToFishlist")]
        [Description("AddToFishlist")]
        public static void AddToFishlist_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
                from.Target = new AddTarget();
            else
                e.Mobile.SendMessage(0x25, "Bad Format: [AddToFishlist");
        }

        private class AddTarget : Target
        {
            public AddTarget()
                : base(10, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object o)
            {

                if (o != null)
                {
                    if (o is Container)
                    {

                        if (((Container)o).Items.Count > 0)
                        {
                            PirateFishing.AddToFishlist((Container)o);
                            ((Container)o).Delete();
                            from.SendMessage("The selected container has been added to the sunken ship fishing list.");
                        }
                        else
                            from.SendMessage("There is nothing in that container.");
                    }
                    else
                    {
                        from.SendMessage("That is not a container.");
                    }
                }
            }
        }

        #endregion

        private static PirateFishingPersistance m_Item;

        public static TimeSpan m_SunkenShipDecayTime = TimeSpan.FromDays(14);
        private const int m_TilesPerBlock = 190;
        private const int m_MaxTileX = 5119;
        private const int m_MaxTileY = 4095;
        private static int m_BlocksX = (int)Math.Ceiling((double)m_MaxTileX / (double)m_TilesPerBlock);
        private static int m_BlocksY = (int)Math.Ceiling((double)m_MaxTileY / (double)m_TilesPerBlock);

        public static Dictionary<int, List<SunkenShipContainer>> m_Table = new Dictionary<int, List<SunkenShipContainer>>();
        private static Queue m_Queue = new Queue();

        public static int GetBlockNumber(Point3D loc)
        {
            int BlockX = loc.X / m_TilesPerBlock; //only want integer part...
            int BlockY = (int)Math.Ceiling((double)loc.Y / (double)m_TilesPerBlock); //round up!

            return (BlockX * m_BlocksY + BlockY);
        }

        public static void SunkenShip(BaseBoat b)
        {
            if (b == null)
                return;

            if (b.Hold == null)
                return;

            AddToFishlist(b.Hold);
        }

        public static SunkenShipContainer OnFish(Point3D loc)
        {
            int numBlock = GetBlockNumber(loc);
            List<SunkenShipContainer> m_List;

            m_Table.TryGetValue(numBlock, out m_List);

            if (m_List != null)
            {
                if (m_List.Count > 0)
                {
                    SunkenShipContainer c = m_List[Utility.Random(m_List.Count)];
                    if (c != null)
                    {
                        if (c.Items.Count > 0)
                        {
                            m_List.Remove(c);
                            return c;
                        }
                    }
                    m_List.Remove(c);
                }
            }

            return null;
        }

        public static void AddToFishlist(Container cont)
        {
            if (cont.TotalItems <= 0)
                return;

            if (m_Item == null)
                new PirateFishingPersistance();

            else if (m_Item.Deleted)
                new PirateFishingPersistance();

            SunkenShipContainer c = new SunkenShipContainer();

            foreach (Item item in cont.Items)
            {
                if (item is Doubloon)
                    continue;

                m_Queue.Enqueue(item);
            }

            while (m_Queue.Count > 0)
            {
                c.AddItem((Item)m_Queue.Dequeue());
            }

            m_Queue.Clear();
            int numBlock = GetBlockNumber(cont.GetWorldLocation());

            List<SunkenShipContainer> m_List;

            m_Table.TryGetValue(numBlock, out m_List);

            if (m_List != null)
            {
                m_Table.Remove(numBlock);
                m_List.Add(c);
            }

            else
            {
                m_List = new List<SunkenShipContainer>();
                m_List.Add(c);
            }

            m_Table.Add(numBlock, m_List);
        }

        public class PirateFishingPersistance : Item
        {
            public PirateFishingPersistance()
                : base(0x0)
            {
                if (m_Item != null)
                    if (!m_Item.Deleted)
                        Delete();
                m_Item = this;

            }

            public PirateFishingPersistance(Serial serial)
                : base(serial)
            {
            }


            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version
                writer.Write(m_Table.Count);
                foreach (KeyValuePair<int, List<SunkenShipContainer>> kvp in m_Table)
                {
                    writer.Write(kvp.Key);
                    writer.Write(kvp.Value.Count);
                    foreach (SunkenShipContainer m in kvp.Value)
                        writer.WriteItem(m);
                }
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                int count = reader.ReadInt();
                for (int i = 0; i < count ; i++)
                {
                    int key = reader.ReadInt();
                    int count2 = reader.ReadInt();

                    List<SunkenShipContainer> cList = new List<SunkenShipContainer>();
                    for (int j = 0; j < count2 ; j++)
                    {
                        SunkenShipContainer c = (SunkenShipContainer)reader.ReadItem();
                        if (c != null)
                            cList.Add(c);

                    }

                    if (/*key != null &&*/ cList.Count > 0)
                    {
                        m_Table.Add(key, cList);
                    }
                }

                m_Item = this;
            }
        } //SAVE DATA
    }
}