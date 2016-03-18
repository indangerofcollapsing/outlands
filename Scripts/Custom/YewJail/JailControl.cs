/***************************************************************************
 *                              YewJailControl.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Targeting;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Commands;

namespace Server.YewJail
{
    public static class YewJailControl
    {

        public static List<Point3D> m_GuardSpawnLocs = new List<Point3D>
        {
            new Point3D(300, 761, 20), new Point3D(312, 770, 20), new Point3D(312, 787, 20),
            new Point3D(338, 793, 20), new Point3D(338, 818, 20), new Point3D(355, 836, 20),
            new Point3D(374, 883, 0) };
        
        public static List<Point3D> m_HelperFinalLoc = new List<Point3D>
        {
            new Point3D(286, 770, 0), new Point3D(280, 770, 0), new Point3D(274, 770, 0),
            new Point3D(268, 770, 0), new Point3D(262, 770, 0), new Point3D(262, 772, 0),
            new Point3D(268, 772, 0), new Point3D(274, 772, 0), new Point3D(280, 772, 0),
            new Point3D(286, 772, 0), 

	        new Point3D(286, 770, 20), new Point3D(280, 770, 20), new Point3D(274, 770, 20),
            new Point3D(268, 770, 20), new Point3D(262, 770, 20), new Point3D(262, 772, 20),
            new Point3D(268, 772, 20), new Point3D(274, 772, 20), new Point3D(280, 772, 20),
            new Point3D(286, 772, 20) };

        public static List<Point3D> m_doorBombLocation = new List<Point3D>
        {
            new Point3D(285, 768, 0), new Point3D(279, 768, 0), new Point3D(273, 768, 0),
            new Point3D(267, 768, 0), new Point3D(261, 768, 0), new Point3D(262, 774, 0),
            new Point3D(268, 774, 0), new Point3D(274, 774, 0), new Point3D(280, 774, 0),
            new Point3D(286, 774, 0),

	        new Point3D(285, 768, 20), new Point3D(279, 768, 20), new Point3D(273, 768, 20),
            new Point3D(267, 768, 20), new Point3D(261, 768, 20), new Point3D(262, 774, 20),
            new Point3D(268, 774, 20), new Point3D(274, 774, 20), new Point3D(280, 774, 20),
            new Point3D(286, 774, 20) };

        public static List<Point3D> m_bagSpawnLoc = new List<Point3D>
        {
            new Point3D(286, 767, 0), new Point3D(280, 767, 0), new Point3D(274, 767, 0),
            new Point3D(268, 767, 0), new Point3D(262, 767, 0), new Point3D(262, 774, 0),
            new Point3D(268, 774, 0), new Point3D(274, 774, 0), new Point3D(280, 774, 0),
            new Point3D(286, 774, 0), 

	        new Point3D(286, 767, 20), new Point3D(280, 767, 20), new Point3D(274, 767, 20),
            new Point3D(268, 767, 20), new Point3D(262, 767, 20), new Point3D(262, 774, 20),
            new Point3D(268, 774, 20), new Point3D(274, 774, 20), new Point3D(280, 774, 20),
            new Point3D(286, 774, 20) };

        public static Point3D[] m_JailLocations = new Point3D[]
        {
            new Point3D(286, 766, 0),new Point3D(280, 766, 0),new Point3D(274, 766, 0),
            new Point3D(268, 766, 0),new Point3D(262, 766, 0),new Point3D(262, 776, 0),
            new Point3D(268, 776, 0),new Point3D(274, 776, 0),new Point3D(280, 776, 0),
            new Point3D(286, 776, 0),

            new Point3D(286, 766, 20),new Point3D(280, 766, 20),new Point3D(274, 766, 20),
            new Point3D(268, 766, 20),new Point3D(262, 766, 20),new Point3D(262, 776, 20),
            new Point3D(268, 776, 20),new Point3D(274, 776, 20),new Point3D(280, 776, 20),
            new Point3D(286, 776, 20) };

        public static Point3D[] m_BombHelperLocations = new Point3D[]
        {
            new Point3D(284, 764, 0),new Point3D(278, 764, 0),new Point3D(272, 764, 0),
            new Point3D(266, 764, 0),new Point3D(260, 764, 0),new Point3D(260, 774, 0),
            new Point3D(266, 774, 0),new Point3D(272, 774, 0),new Point3D(278, 774, 0),
            new Point3D(284, 774, 0),

            new Point3D(284, 764, 20),new Point3D(278, 764, 20),new Point3D(272, 764, 20),
            new Point3D(266, 764, 20),new Point3D(260, 764, 20),new Point3D(260, 774, 20),
            new Point3D(266, 774, 20),new Point3D(272, 774, 20),new Point3D(278, 774, 20),
            new Point3D(284, 774, 20) };

        public static List<Point3D> m_doorSpawnLocs = new List<Point3D>
        {
            new Point3D(286, 769, 0),new Point3D(280, 769, 0),new Point3D(274, 769, 0),new Point3D(268, 769, 0),
            new Point3D(262, 769, 0),new Point3D(262, 773, 0),new Point3D(268, 773, 0),new Point3D(274, 773, 0),
            new Point3D(280, 773, 0),new Point3D(286, 773, 0),

            new Point3D(286, 769, 20),new Point3D(280, 769, 20),new Point3D(274, 769, 20),new Point3D(268, 769, 20),
            new Point3D(262, 769, 20),new Point3D(262, 773, 20),new Point3D(268, 773, 20),new Point3D(274, 773, 20),
            new Point3D(280, 773, 20),new Point3D(286, 773, 20) };

        public static Point3D exitLoc = new Point3D(371,871,0);

        private static Layer[] m_Layers = new Layer[]
		{
			Layer.Cloak, Layer.Bracelet, Layer.Ring, Layer.Shirt, Layer.Pants, Layer.InnerLegs,
			Layer.Shoes, Layer.Arms, Layer.InnerTorso, Layer.MiddleTorso, Layer.OuterLegs, 
            Layer.Neck, Layer.Waist, Layer.Gloves, Layer.OuterTorso, Layer.OneHanded,
			Layer.TwoHanded, Layer.FacialHair, Layer.Hair, Layer.Helm, Layer.Talisman
		};

        public static MetalJailChest m_ChestItem;
        public static Point3D m_chestLocation = new Point3D(290, 769, 20);
        public static Point3D m_JailCellEntranceLocation = new Point3D(289, 771, 0);
        public static Point3D m_JailCellEntranceLocation2 = new Point3D(289, 771, 20);
        public static JailCellEntranceDoor m_JailCellEntranceDoor;
        public static JailCellEntranceDoor2 m_JailCellEntranceDoor2;
        public static Dictionary<Int32, YewJailItem> m_YewJailItems = new Dictionary<Int32, YewJailItem>();
        public static ExitItem m_YewJailExitItem;
        public static Dictionary<Int32, YewJailDoor> m_YewJailDoorItems = new Dictionary<Int32, YewJailDoor>();
        public static Dictionary<Int32, YewJailFlintGuard> m_YewJailFlintGuards = new Dictionary<Int32, YewJailFlintGuard>();
        public static Point3D m_HelperSpawnLoc = new Point3D(291, 764, 20);
        public static Point3D m_HelperSpawnLoc2 = new Point3D(300, 771, 20);
        public static List<Int32> m_OccupiedCell = new List<Int32>();
        public static bool m_Doors = false;
        public static bool m_Chest = false;
        private static Queue m_Queue = new Queue();

        public static void Initialize()
        {
            CommandSystem.Register("YewJail", AccessLevel.Counselor, new CommandEventHandler(YewJail_OnCommand));
            Server.Commands.CommandSystem.Register("ClearJails", AccessLevel.Counselor, new CommandEventHandler(ClearJails));
        }

        [Usage("YewJail")]
        [Description("Moves a player into the Yew jail cell and activates the Jail Quest.")]
        public static void YewJail_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            if (e.Length == 0)
            {
                from.SendMessage(String.Format("Who would you like to send to the Yew Jail?"));
                from.Target = new YewJailTarget();
            }
            else
            {
                e.Mobile.SendMessage(0x25, "Bad Format: YewJail");
            }
        }

        public static void ClearJails(CommandEventArgs e)
        {
            DeleteDoors();
            DeleteAllJailItems();

            if (m_JailCellEntranceDoor != null)
                m_JailCellEntranceDoor.Delete();

            if (m_JailCellEntranceDoor2 != null)
                m_JailCellEntranceDoor2.Delete();

            if (m_ChestItem != null)
                m_ChestItem.Delete();

            m_OccupiedCell.Clear();

            m_Doors = false;
            m_Chest = false;

            if (m_YewJailExitItem != null)
                m_YewJailExitItem.Delete();

        }


        private class YewJailTarget : Target
        {
            public YewJailTarget()
                : base(10, false, TargetFlags.None) { }

            protected override void OnTarget(Mobile from, object targ)
            {
                if (targ != null && targ is PlayerMobile)
                {
                    PlayerMobile m = (PlayerMobile)targ;
                    Server.YewJail.YewJailControl.NewJailedTarget(((Mobile)m));
                }
            }
        }

        public static void NewJailedTarget(Mobile from)
        {
            if (m_Doors == false)
                CreateDoors();

            if (m_Chest == false)
                CreateChest();

            if (m_YewJailExitItem == null)
            {
                m_YewJailExitItem = new ExitItem();
                m_YewJailExitItem.MoveToWorld(exitLoc, Map.Felucca);
            }

            bool m_found = false;
            Int32 i = 1;
            while (m_found == false && i <= 20)
            {
                if (!m_OccupiedCell.Contains(i))
                {
                    if (!from.Alive)
                        from.Resurrect();

                    YewJailItem jailItem = new YewJailItem(from, TimeSpan.FromMinutes(10 * from.ShortTermMurders), i);
                    m_found = true;
                    RegisterJailCell(jailItem, i);
                    from.MoveToWorld(m_JailLocations[i - 1], Map.Felucca);
                    m_YewJailDoorItems[i].Locked = true;
                }
                i++;
            }

            if (m_found)
            {
                MoveItemsToBank(from);
                (from).AddItem(new Items.Robe());
            }
            else
            {
                from.SendMessage("The jail is full. Lucky you.");
            }
        }

        private static void MoveItemsToBank(Mobile from)
        {
            m_Queue.Clear();

            foreach (Layer l in m_Layers)
            {
                Item i = from.FindItemOnLayer(l);
                if (i != null)
                    from.BankBox.AddItem(i);
            }
            foreach (Item i in from.Backpack.Items)
                if (i != null && !(i is YewJailItem))
                    if (i.GetType() != typeof(Items.Spellbook))
                        m_Queue.Enqueue(i);

            while (m_Queue.Count > 0)
            {
                Item j = (Item)m_Queue.Dequeue();
                from.BankBox.AddItem(j);
            }
            from.LocalOverheadMessage(Network.MessageType.Regular, from.EmoteHue, true, "You have been sent to Jail! Your items have been placed in your Bank Box.");
        }

        public static void RegisterJailCell(YewJailItem jItem, Int32 jailCell)
        {
            if (m_OccupiedCell.Contains(jailCell))
                m_OccupiedCell.Remove(jailCell);

            m_OccupiedCell.Add(jailCell);
            m_YewJailItems.Add(jailCell, jItem);

        }

        public static void RegisterExitItem(ExitItem jItem)
        {
            m_YewJailExitItem = jItem;
        }

        public static void UnregisterJailCell(Int32 jailCell)
        {
            m_OccupiedCell.Remove(jailCell);
            m_YewJailItems.Remove(jailCell);
        }

        public static void DeleteAllJailItems()
        {
            m_Queue.Clear();

            foreach (YewJailItem i in m_YewJailItems.Values)
                if (i != null)
                    m_Queue.Enqueue(i);

            while (m_Queue.Count > 0)
            {
                YewJailItem j = m_Queue.Dequeue() as YewJailItem;
                j.Delete();
            }

            m_YewJailItems.Clear();
        }

        public static void RegisterJailDoor(YewJailDoor jItem, Int32 jailCell)
        {
            if (m_YewJailDoorItems.ContainsKey(jailCell))
                m_YewJailDoorItems.Remove(jailCell);

            m_YewJailDoorItems.Add(jailCell, jItem);
        }

        public static void UnRegisterJailDoor(Int32 jailCell)
        {
            if (m_YewJailDoorItems.ContainsKey(jailCell))
                m_YewJailDoorItems.Remove(jailCell);
        }

        public static void RegisterFlintGuard(YewJailFlintGuard helper, Int32 jailCell)
        {
            if (m_YewJailFlintGuards.ContainsKey(jailCell))
                m_YewJailFlintGuards.Remove(jailCell);

            m_YewJailFlintGuards.Add(jailCell, helper);
        }

        public static void RegisterEntraceDoor(JailCellEntranceDoor door) { m_JailCellEntranceDoor = door; }
        public static void RegisterEntraceDoor2(JailCellEntranceDoor2 door) { m_JailCellEntranceDoor2 = door; }
        public static void OpenEntranceDoor() { m_JailCellEntranceDoor.Open = true; }
        public static void OpenEntranceDoor2() { m_JailCellEntranceDoor2.Open = true; }

        private static void CreateDoors()
        {
            Int32 i = 1;
            if (m_Doors == false)
            {
                YewJailDoor jDoor;
                foreach (Point3D p in m_doorSpawnLocs)
                {
                    if (p.Y == 769)
                        jDoor = new YewJailDoor(DoorFacing.WestCW, i);
                    else
                        jDoor = new YewJailDoor(DoorFacing.WestCCW, i);

                    jDoor.Locked = false;
                    jDoor.MoveToWorld(p, Map.Felucca);

                    if (m_YewJailDoorItems.ContainsKey(i))
                        m_YewJailDoorItems.Remove(i);

                    m_YewJailDoorItems.Add(i, jDoor);
                    i++;
                }
                m_Doors = true;
            }
            if (m_JailCellEntranceDoor == null)
            {
                m_JailCellEntranceDoor = new JailCellEntranceDoor(DoorFacing.SouthCW);
                m_JailCellEntranceDoor.Locked = false;
                m_JailCellEntranceDoor.MoveToWorld(m_JailCellEntranceLocation, Map.Felucca);
            }
            if (m_JailCellEntranceDoor2 == null)
            {
                m_JailCellEntranceDoor2 = new JailCellEntranceDoor2(DoorFacing.SouthCW);
                m_JailCellEntranceDoor2.Locked = false;
                m_JailCellEntranceDoor2.MoveToWorld(m_JailCellEntranceLocation2, Map.Felucca);
            }
        }

        private static void CreateChest()
        {
            if (m_Chest == false)
            {
                MetalJailChest chest = new MetalJailChest();
                chest.MoveToWorld(m_chestLocation, Map.Felucca);
                m_Chest = true;
                m_ChestItem = chest;
            }
        }

        public static void FillChest()
        {
            if (m_ChestItem != null)
            {
                m_Queue.Clear();

                foreach (Item item in m_ChestItem.Items)
                    if (item != null)
                        if (item.GetType() != typeof(Items.Spellbook))
                            m_Queue.Enqueue(item);

                while (m_Queue.Count > 0)
                {
                    Item j = (Item)m_Queue.Dequeue();
                    j.Delete();
                }

                m_ChestItem.AddItem(new Bandage(5));
                m_ChestItem.AddItem(new BagOfReagents(5));
                m_ChestItem.AddItem(new Dagger());
                m_ChestItem.AddItem(new Longsword());
                m_ChestItem.AddItem(new QuarterStaff());
            }
        }

        public static void DeleteDoors()
        {
            foreach (YewJailDoor j in m_YewJailDoorItems.Values)
                m_Queue.Enqueue(j);

            while (m_Queue.Count > 0)
            {
                YewJailDoor j = m_Queue.Dequeue() as YewJailDoor;
                j.Delete();
            }

            m_YewJailDoorItems.Clear();
        }
    }

    public class MetalJailChest : FillableContainer
    {
        public MetalJailChest() : base(0xE7C) { }

        public MetalJailChest(Serial serial) : base(serial) { }

        public void RegisterChest()
        {
            YewJailControl.m_ChestItem = this;
            YewJailControl.m_Chest = true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && Weight == 25)
                Weight = -1;

            RegisterChest();
        }
    }

    public class JailCellEntranceDoor : BaseDoor
    {
        [Constructable]
        public JailCellEntranceDoor(Items.DoorFacing facing) : base(0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing)) { }

        public JailCellEntranceDoor(Serial serial) : base(serial) { }

        public void RegisterJailDoor() { YewJailControl.RegisterEntraceDoor(this); }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            RegisterJailDoor();
        }
    }
    public class JailCellEntranceDoor2 : BaseDoor
    {
        [Constructable]
        public JailCellEntranceDoor2(Items.DoorFacing facing) : base(0x675 + (2 * (int)facing), 0x676 + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing)) { }

        public JailCellEntranceDoor2(Serial serial) : base(serial) { }
        
        public void RegisterJailDoor2() { YewJailControl.RegisterEntraceDoor2(this); }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }
        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            RegisterJailDoor2();
        }
    }
}

