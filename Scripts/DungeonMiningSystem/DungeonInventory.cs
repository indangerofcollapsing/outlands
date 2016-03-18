using System;
using System.Collections.Generic;
using System.Diagnostics;

using Server;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Targeting;
using Server.Accounting;
using Server.Commands;
namespace Server.PortalSystem
{
   
    // See Doors.cs for DoorFacing
    //public class VirtualDungeonDoor : IControlSelectable
    //{
    //    public int m_graphicId;
    //    public string m_name;
    //    public int m_quantity;
    //    public EDoorType m_doorType;
    //    public DoorFacing m_doorFacing;
    //    public static string m_category = "Door";

    //    public VirtualDungeonDoor(int graphicId, string name, int quantity,
    //        EDoorType type, DoorFacing facing)
    //    {
    //        m_graphicId = graphicId;
    //        m_name = name;
    //        m_quantity = quantity;
    //        m_doorType = type;
    //        m_doorFacing = facing;
    //    }
    //    public int AdjustQuantity(short quantity)
    //    {
    //        m_quantity += quantity;
    //        return m_quantity;
    //    }
    //    public int GetGraphicId()
    //    {
    //        return m_graphicId;
    //    }
    //    public string GetName()
    //    {
    //        return m_name;
    //    }
    //}

    //public class VirtualDungeonStatic : IControlSelectable
    //{
    //    public int m_quantity { get; private set; }// Stacks
    //    public int m_graphicId { get; private set; }// Also an internal id.
    //    public string m_name { get; private set; }// The static name (e.g. Marble Floor)
    //    public string m_category { get; private set; }// The archetype of the item (e.g. Wall, Decor, Floor).

    //    public VirtualDungeonStatic(int quantity, int graphicId, string name, string category)
    //    {
    //        m_quantity = quantity;
    //        m_graphicId = graphicId;
    //        m_name = name;
    //        m_category = category;
    //    }
    //    //public DungeonSingleStaticInjector ConvertToInjector(int quantity, bool adjustQuantity = false)
    //    //{
    //    //    // Inventory is now authoritative over quantity changes.
    //    //    //m_quantity -= quantity;
    //    //    return new DungeonSingleStaticInjector(quantity, m_graphicId, m_name, m_category);
    //    //}
    //    public int AdjustQuantity(int quantity)
    //    {
    //        m_quantity += quantity;
    //        return m_quantity;
    //    }
    //    public int GetGraphicId()
    //    {
    //        return m_graphicId;
    //    }
    //    public string GetName()
    //    {
    //        return m_name;
    //    }
    //}
    //public class InjectorInspector : Item
    //{
    //    public static int s_gid = 0x14F5;
    //    [Constructable]
    //    public InjectorInspector()
    //        : base(s_gid)
    //    {
    //    }
    //    public InjectorInspector(Serial serial)
    //        : base(serial)
    //    {
    //    }
    //    public override void Serialize(GenericWriter writer)
    //    {
    //        base.Serialize(writer);
    //    }
    //    public override void Deserialize(GenericReader reader)
    //    {
    //        base.Deserialize(reader);
    //    }
    //    public override string DefaultName
    //    {
    //        get
    //        {
    //            return "A dungeon element inspector";
    //        }
    //    }
    //    public override void OnDoubleClick(Mobile from)
    //    {
    //        from.SendMessage("Select a dungeon injector to inspect.");
    //        from.Target = new InspectInjectorTarget();
    //    }
    //}

    //class InspectInjectorTarget : Target
    //{
    //    public InspectInjectorTarget()
    //        : base(-1, true, TargetFlags.None, false)
    //    {
    //    }
    //    protected override void OnTarget(Mobile from, object targeted)
    //    {
    //        if (targeted is DungeonInjector)
    //        {
    //            from.SendGump(new InjectorPreviewGump(from, targeted as DungeonInjector));
    //        }
    //        else
    //        {
    //            from.SendMessage("That is an invalid target.");
    //        }
    //    }
    //}

    //public class InjectorPreviewGump : Gump
    //{
    //    enum GumpActions
    //    {
    //        GA_SubmitVote,
    //    };

    //    public InjectorPreviewGump(Mobile from, DungeonInjector inj)
    //        : base(100, 100)
    //    {
    //        // Create an identifier that is unique to the injector's contents. We can't use the serial
    //        // because two different injectors with identical contents will appear to be different.
    //        int seed = 0x1A5D6F32;
    //        //if (inj is DungeonSingleStaticInjector)
    //        //{
    //        //    DungeonSingleStaticInjector injector = inj as DungeonSingleStaticInjector;
    //        //    seed ^= injector.m_graphicId;
    //        //    seed *= 75 * injector.m_quantity;
    //        //    seed += 65650;
    //        //    seed ^= 0xC36D13B;
    //        //}
    //        //else if (inj is DungeonGroupStaticInjector)
    //        //{
    //        //    DungeonGroupStaticInjector injector = inj as DungeonGroupStaticInjector;
    //        //    int len = injector.m_graphicIds.Length;
    //        //    int sum = 0;
    //        //    for (int i = 0; i < len; ++i)
    //        //    {
    //        //        sum += injector.m_graphicIds[i];
    //        //    }
    //        //    seed ^= injector.m_graphicIds[0];
    //        //    seed *= sum * injector.m_quantity * injector.m_graphicIds.Length;
    //        //    seed += 29534;
    //        //    seed ^= 0xA5D113A;
    //        //}
    //        //else if (inj is DungeonDoorInjector)
    //        //{
    //        //    DungeonDoorInjector injector = inj as DungeonDoorInjector;
    //        //    seed ^= injector.m_graphicId;
    //        //    seed *= 105 * injector.m_quantity;
    //        //    seed += 32164;
    //        //    seed ^= 0xC41D36A;
    //        //}

    //        //AddBackground(5, 5, 40, 400, 2620); // Inventory patch
    //        //AddBackground(5, 5, 350, 400, 2620); // Inventory
    //        //string message = "Here are the contents. The unique code for this collection is ";
    //        //message += seed.ToString("X");
    //        //message += ". If this code is different, the contents are not the same.";
    //        //AddHtml(20, 20, 320, 60, message, true, false);

    //        //int page = 1;
    //        //AddPage(page);

    //        //int x = 20;
    //        //int y = 100;

    //        //if (inj is DungeonSingleStaticInjector)
    //        //{
    //        //    DungeonSingleStaticInjector injector = inj as DungeonSingleStaticInjector;
    //        //    AddItem(x, y + 20, injector.m_graphicId);
    //        //    AddLabel(x + 10, y - 5, 0xFF, injector.m_quantity.ToString());
    //        //    AddLabel(x, y - 20, 0xFF, "0x" + injector.m_graphicId.ToString("X"));
    //        //}
    //        //else if (inj is DungeonGroupStaticInjector)
    //        //{
    //        //    DungeonGroupStaticInjector injector = inj as DungeonGroupStaticInjector;

    //        //    int len = injector.m_graphicIds.Length;
    //        //    int entry = 0;
    //        //    for (int i = 0; i < len; ++i)
    //        //    {
    //        //        if (entry >= 12)
    //        //        {
    //        //            AddButton(15, 370, 4005, 4007, 0, GumpButtonType.Page, page + 1); // Next page.
    //        //            AddLabel(50, 370, 0xFF, "Next");

    //        //            AddPage(++page);

    //        //            AddButton(320, 370, 4014, 4016, 0, GumpButtonType.Page, page - 1);
    //        //            AddLabel(288, 370, 0xFF, "Back");

    //        //            x = 20;
    //        //            y = 100;

    //        //            entry = 0;
    //        //        }

    //        //        AddItem(x, y + 20, injector.m_graphicIds[i]);
    //        //        AddLabel(x + 10, y - 5, 0xFF, injector.m_quantity.ToString());
    //        //        AddLabel(x, y - 20, 0xFF, "0x" + injector.m_graphicIds[i].ToString("X"));

    //        //        x += 50;
    //        //        if (x > 300)
    //        //        {
    //        //            x = 20;
    //        //            y += 160;
    //        //        }

    //        //        entry++;
    //        //    }
    //        //}
    //        //else if (inj is DungeonDoorInjector)
    //        //{
    //        //    DungeonDoorInjector injector = inj as DungeonDoorInjector;
    //        //    AddItem(x, y + 20, injector.m_graphicId);
    //        //    AddLabel(x + 10, y - 5, 0xFF, injector.m_quantity.ToString());
    //        //    string facingStr = injector.m_doorFacing.ToString();
    //        //    AddLabel(x + 10, y + 20, 0xFF, facingStr.Contains("CC") ? "CCW" : "CW");
    //        //    AddLabel(x, y - 20, 0xFF, "0x" + injector.m_graphicId.ToString("X"));
    //        //}
    //    }
    //}

    //public class DungeonInventory
    //{
    //    // All of these collections refer to the same static in memory.
    //    // For efficient access of a specific unique static; the graphicId is guaranteed to be unique.
    //    public Dictionary<int, VirtualDungeonStatic> m_graphicIdToStaticMap { get; private set; }
    //    public Dictionary<string, LinkedList<VirtualDungeonStatic>> m_categoryToStaticsMap { get; private set; }
    //    public List<VirtualDungeonDoor> m_doorList { get; private set; }
    //    public DungeonInventory()
    //    {
    //        m_categoryToStaticsMap = new Dictionary<string, LinkedList<VirtualDungeonStatic>>();
    //        m_graphicIdToStaticMap = new Dictionary<int, VirtualDungeonStatic>();
    //        m_doorList = new List<VirtualDungeonDoor>();
    //    }
    //    public int GetCompleteStaticsCount()
    //    {
    //        int count = 0;
    //        foreach (KeyValuePair<int, VirtualDungeonStatic> kvp in m_graphicIdToStaticMap)
    //        {
    //            count += kvp.Value.m_quantity;
    //        }
    //        foreach (VirtualDungeonDoor door in m_doorList)
    //        {
    //            count += door.m_quantity;
    //        }
    //        return count;
    //    }
    //    public int GetUniqueStaticsCount()
    //    {
    //        int count = m_graphicIdToStaticMap.Count;
    //        count += m_doorList.Count;
    //        return count;
    //    }

    //    public VirtualDungeonStatic GetStaticByGraphicId(int graphicId)
    //    {
    //        if (m_graphicIdToStaticMap.ContainsKey(graphicId))
    //        {
    //            return m_graphicIdToStaticMap[graphicId];
    //        }
    //        return null;
    //    }
    //    public VirtualDungeonDoor GetDoorByGraphicId(int graphicId)
    //    {
    //        foreach (VirtualDungeonDoor door in m_doorList)
    //        {
    //            if (door.m_graphicId == graphicId)
    //            {
    //                return door;
    //            }
    //        }
    //        return null;
    //    }
    //    public int GetCategoryCount()
    //    {
    //        // Add 1 for the spawners category.
    //        return m_categoryToStaticsMap.Count
    //            + (m_doorList.Count > 0 ? 1 : 0);
    //    }
    //    public bool ContainsCategory(string category)
    //    {
    //        return m_categoryToStaticsMap.ContainsKey(category);
    //    }
    //    public bool ContainsStatic(VirtualDungeonStatic vStatic)
    //    {
    //        return m_graphicIdToStaticMap.ContainsKey(vStatic.m_graphicId) ? true : false;
    //    }
    //    public LinkedList<VirtualDungeonStatic> GetStaticsByCategory(string category)
    //    {
    //        if (ContainsCategory(category))
    //        {
    //            return m_categoryToStaticsMap[category];
    //        }
    //        return null;
    //    }
    //    public void OnStaticInjection(DungeonInjector injector)
    //    {
    //        // The graphicId's use as an entryId is intentional; guarantees a unique
    //        // mapping in the inventory.
    //        //if (injector is DungeonSingleStaticInjector)
    //        //{
    //        //    DungeonSingleStaticInjector inj = injector as DungeonSingleStaticInjector;

    //        //    // The graphicId's use as an entryId is intentional; guarantees a unique
    //        //    // mapping in the virtual inventory.

    //        //    // Verify the integrity of the injector.
    //        //    if (DungeonMiningSystem.m_regTypeMap.ContainsKey(inj.m_graphicId) &&
    //        //        DungeonMiningSystem.m_regTypeMap[inj.m_graphicId] == DungeonMiningSystem.ERegistrationType.ERT_Static)
    //        //    {
    //        //        AddStaticToInventory(inj.m_quantity, inj.m_graphicId, inj.m_name, inj.m_category);
    //        //    }

    //        //}
    //        //else if (injector is DungeonGroupStaticInjector)
    //        //{
    //        //    DungeonGroupStaticInjector inj = injector as DungeonGroupStaticInjector;

    //        //    // Verify the integrity of the injector.
    //        //    if (DungeonMiningSystem.m_regTypeMap.ContainsKey(inj.m_graphicIds[0]) &&
    //        //        DungeonMiningSystem.m_regTypeMap[inj.m_graphicIds[0]] == DungeonMiningSystem.ERegistrationType.ERT_Static)
    //        //    {
    //        //        int count = inj.m_graphicIds.Length;
    //        //        for (int i = 0; i < count; i++)
    //        //        {
    //        //            AddStaticToInventory(inj.m_quantity, inj.m_graphicIds[i], inj.m_names[i], inj.m_categories[i]);
    //        //        }
    //        //    }
    //        //}
    //    }
    //    //public void OnDoorInjection(DungeonDoorInjector inj)
    //    //{
    //    //    // Verify the integrity of the injector.
    //    //    if (DungeonMiningSystem.m_regTypeMap.ContainsKey(inj.m_graphicId) &&
    //    //        DungeonMiningSystem.m_regTypeMap[inj.m_graphicId] == DungeonMiningSystem.ERegistrationType.ERT_Door)
    //    //    {
    //    //        AddDoorToInventory(inj.m_graphicId, inj.m_name, inj.m_quantity, inj.m_doorType, inj.m_doorFacing);
    //    //    }
    //    //}
    //    public DungeonInjector ConvertToInjector(VirtualDungeonStatic vStatic, int quantity)
    //    {
    //        int gid = vStatic.m_graphicId;
    //        if (m_graphicIdToStaticMap.ContainsKey(gid))
    //        {
    //            short adjustment = (short)(quantity * -1);
    //            AdjustStaticQuantity(vStatic, adjustment);
    //            //return new DungeonSingleStaticInjector(quantity, gid, vStatic.m_name, vStatic.m_category);
    //        }
    //        return null;
    //    }
    //    public DungeonInjector ConvertToInjector(VirtualDungeonDoor vDoor)
    //    {
    //        if (RemoveDoorFromInventory(vDoor))
    //        {
    //            return new DungeonDoorInjector(vDoor.m_doorType, vDoor.m_doorFacing, vDoor.m_graphicId, vDoor.m_name, vDoor.m_quantity);
    //        }
    //        return null;
    //    }
    //    public void AddToInventory(PortalContentEntry entry)
    //    {
    //        bool isStatic = entry.m_behavior == "door";
    //        if (entry.m_span == "single")
    //        {
    //            PortalSingleContentEntry singleEntry = entry as PortalSingleContentEntry;
    //            if (isStatic)
    //            {
    //                AddStaticToInventory(singleEntry.m_vendorQuantity, singleEntry.m_key, singleEntry.m_name, singleEntry.m_category);
    //            }
    //            else
    //            {
    //               AddDoorToInventory(singleEntry.m_key, singleEntry.m_name, singleEntry.m_vendorQuantity, DungeonDoor.GetTypeFromGid(singleEntry.m_key), DungeonDoor.GetFacingFromGid(singleEntry.m_key));
    //            }
    //        }
    //        else if (entry.m_span == "ranged")
    //        {
    //            PortalRangedContentEntry rangedEntry = entry as PortalRangedContentEntry;
    //            int end = rangedEntry.m_gidEnd;
    //            for (int gid = rangedEntry.m_gidStart; gid <= end; ++gid)
    //            {
    //                if (isStatic)
    //                {
    //                    AddStaticToInventory(rangedEntry.m_vendorQuantity, gid, rangedEntry.m_name, rangedEntry.m_category);
    //                }
    //                else
    //                {
    //                    AddDoorToInventory(gid, rangedEntry.m_name, rangedEntry.m_vendorQuantity, DungeonDoor.GetTypeFromGid(gid), DungeonDoor.GetFacingFromGid(gid));
    //                }
    //            }
    //        }
    //    }
    //    public void AddStaticToInventory(int quantity, int graphicId, string name, string category)
    //    {
    //        if (m_graphicIdToStaticMap.ContainsKey(graphicId))
    //        {
    //            // This unique exists, simply increase quantity. No change required to
    //            // category list.
    //            m_graphicIdToStaticMap[graphicId].AdjustQuantity(quantity);
    //        }
    //        else
    //        {
    //            // This is the first entry of this type, change required to category list.
    //            VirtualDungeonStatic vStatic = new VirtualDungeonStatic(quantity, graphicId, name, category);
    //            m_graphicIdToStaticMap.Add(graphicId, vStatic);

    //            AddStaticToCategory(vStatic);
    //        }
    //    }
    //    public void AddDoorToInventory(int graphicId, string name, int quantity,
    //        EDoorType doorType, DoorFacing doorFacing)
    //    {
    //        int len = m_doorList.Count;
    //        for (int i = 0; i < len; i++)
    //        {
    //            VirtualDungeonDoor vDoor = m_doorList[i];
    //            if (vDoor.m_graphicId == graphicId)
    //            {
    //                vDoor.AdjustQuantity((short)quantity);
    //                return;
    //            }
    //        }

    //        VirtualDungeonDoor vDungeonDoor = new VirtualDungeonDoor(graphicId, name, quantity, doorType, doorFacing);
    //        m_doorList.Add(vDungeonDoor);
    //    }
    //    private void AddStaticToCategory(VirtualDungeonStatic vStatic)
    //    {
    //        string categoryKey = vStatic.m_category;
    //        if (ContainsCategory(categoryKey))
    //        {
    //            // This category exists.
    //            LinkedList<VirtualDungeonStatic> staticsLinkedList = m_categoryToStaticsMap[categoryKey];
    //            staticsLinkedList.AddLast(vStatic);
    //        }
    //        else
    //        {
    //            // This is the first entry of this category, create a new category. Since
    //            // this category is new, the static can't already be in the map.
    //            LinkedList<VirtualDungeonStatic> staticsLinkedList = new LinkedList<VirtualDungeonStatic>();
    //            staticsLinkedList.AddLast(vStatic);
    //            m_categoryToStaticsMap.Add(categoryKey, staticsLinkedList);
    //        }
    //    }
    //    public int AdjustStaticQuantity(VirtualDungeonStatic vStatic, short quantity)
    //    {
    //        int newQuantity = vStatic.AdjustQuantity(quantity);
    //        if (newQuantity <= 0)
    //        {
    //            RemoveStaticFromInventory(vStatic);
    //            return 0;
    //        }
    //        return vStatic.m_quantity;
    //    }
    //    public int AdjustDoorQuantity(VirtualDungeonDoor vDoor, short quantity)
    //    {
    //        int newQuantity = vDoor.AdjustQuantity(quantity);
    //        if (newQuantity <= 0)
    //        {
    //            RemoveDoorFromInventory(vDoor);
    //            return 0;
    //        }
    //        return vDoor.m_quantity;
    //    }
    //    public void RemoveStaticFromInventory(VirtualDungeonStatic vStatic)
    //    {
    //        // Remote the static from any collections in which it exists.
    //        m_graphicIdToStaticMap.Remove(vStatic.m_graphicId);

    //        string categoryKey = vStatic.m_category;
    //        m_categoryToStaticsMap[categoryKey].Remove(vStatic);

    //        if (m_categoryToStaticsMap[categoryKey].Count == 0)
    //        {
    //            m_categoryToStaticsMap.Remove(categoryKey);
    //        }
    //    }
    //    public bool RemoveDoorFromInventory(VirtualDungeonDoor vDoor)
    //    {
    //        int len = m_doorList.Count;
    //        for (int i = 0; i < len; i++)
    //        {
    //            VirtualDungeonDoor door = m_doorList[i];
    //            if (door.m_graphicId == vDoor.m_graphicId)
    //            {
    //                m_doorList.RemoveAt(i);
    //                return true;
    //            }
    //        }
    //        return false;
    //    }

    //    public void FlushInventory()
    //    {
    //        m_graphicIdToStaticMap.Clear();
    //        m_categoryToStaticsMap.Clear();
    //        m_doorList.Clear();
    //    }
    //    public void Serialize(GenericWriter writer)
    //    {
    //        writer.Write((int)0);

    //        int count = m_graphicIdToStaticMap.Count;
    //        writer.Write(count);

    //        foreach (KeyValuePair<int, VirtualDungeonStatic> kvp in m_graphicIdToStaticMap)
    //        {
    //            VirtualDungeonStatic vStatic = kvp.Value;
    //            writer.Write(vStatic.m_quantity);
    //            writer.Write(vStatic.m_graphicId);
    //            writer.Write(vStatic.m_name);
    //            writer.Write(vStatic.m_category);
    //        }

    //        count = m_doorList.Count;
    //        writer.Write(count);
    //        foreach (VirtualDungeonDoor vDoor in m_doorList)
    //        {
    //            writer.Write(vDoor.m_graphicId);
    //            writer.Write(vDoor.m_name);
    //            writer.Write(vDoor.m_quantity);
    //            writer.Write((int)vDoor.m_doorType);
    //            writer.Write((int)vDoor.m_doorFacing);
    //        }
    //    }
    //    public void Deserialize(GenericReader reader)
    //    {
    //        int version = reader.ReadInt();
    //        switch (version)
    //        {
    //            case 0:
    //                {
    //                    int count = reader.ReadInt();
    //                    for (var i = 0; i < count; i++)
    //                    {
    //                        int quantity = reader.ReadInt();
    //                        int graphicId = reader.ReadInt();
    //                        string name = reader.ReadString();
    //                        string category = reader.ReadString();
    //                        AddStaticToInventory(quantity, graphicId, name, category);
    //                    }

    //                    count = reader.ReadInt();
    //                    for (var i = 0; i < count; i++)
    //                    {
    //                        int graphicId = reader.ReadInt();
    //                        string name = reader.ReadString();
    //                        int quantity = reader.ReadInt();
    //                        EDoorType doorType = (EDoorType)reader.ReadInt();
    //                        DoorFacing doorFacing = (DoorFacing)reader.ReadInt();
    //                        AddDoorToInventory(graphicId, name, quantity, doorType, doorFacing);
    //                    }
    //                    break;
    //                }
    //        }
    //    }
    //}
}
