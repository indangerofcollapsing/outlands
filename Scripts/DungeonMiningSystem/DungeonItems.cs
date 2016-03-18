using System;
using System.Collections.Generic;
using System.Text;

using Server.Items;
using Server.Misc;
using Server.Mobiles;
using Server.Network;
namespace Server.PortalSystem
{
    public class DungeonInjector : Item
    {
        protected string m_desc;
        public static readonly int s_gid = 0x2831;

        public DungeonInjector(string desc)
            : base(0x2831)
        {
            m_desc = desc;
        }
        public DungeonInjector(Serial serial)
            : base(serial)
		{
		}
        public override string DefaultName
        {
            get { return m_desc; }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
            writer.Write(m_desc);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        m_desc = reader.ReadString();
                        break;
                    }
            }

        }
        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Select the dungeon controller to receive this.");
            from.Target = new InjectControllerTarget(this);
        }
    }

    public interface IDungeonItem
    {
        bool IsVisibleTo(Mobile m);
    }
    public interface IPortalElement
    {
        int GetGid();
        int GetEntryKey();
        string GetName();
        string GetCategory();

        Point3D GetLocation();
        void MoveToWorldWrapper(Point3D point);
        void MoveToWorldWrapper(Point3D point, Map map);
        Point3D GetSurfaceTopWrapper();

        void SetVisibility(bool isVisible);

        void Delete();
    }

    public class PortalElement : Static, IPortalElement
    {
        public int m_gid;
        public int m_entryKey;
        public string m_name; // The static name (e.g. Marble Floor)
        public string m_category; // The archetype of the item (e.g. Wall, Decor, Floor).
        public DateTime m_placementTime;
        public PortalElement(int gid, int entryKey, string name, string category, DateTime placementTime)
            : base(gid, 1)
        {
            m_gid = gid;
            m_entryKey = entryKey;
            m_name = name;
            m_category = category;
            m_placementTime = placementTime;
        }
        public PortalElement(Serial serial)
            : base(serial)
		{
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            int version = 0;
            writer.Write(version);

            // Version 0
            writer.Write(m_gid);
            writer.Write(m_entryKey);
            writer.Write(m_name);
            writer.Write(m_category);
            writer.Write(m_placementTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_gid = reader.ReadInt();
                m_entryKey = reader.ReadInt();
                m_name = reader.ReadString();
                m_category = reader.ReadString();
                m_placementTime = reader.ReadDateTime();
            }
        }

        public override string DefaultName
        {
            get
            {
                return m_name;
            }
        }

        // IPortalElement
        public int GetGid()
        {
            return m_gid;
        }
        public int GetEntryKey()
        {
            return m_entryKey;
        }
        public string GetName()
        {
            return m_name;
        }
        public string GetCategory()
        {
            return m_category;
        }
        public Point3D GetLocation()
        {
            return this.Location;
        }
        public void MoveToWorldWrapper(Point3D point)
        {
            base.MoveToWorld(point);
        }
        public void MoveToWorldWrapper(Point3D point, Map map)
        {
            base.MoveToWorld(point, map);
        }
        public Point3D GetSurfaceTopWrapper()
        {
            return base.GetSurfaceTop();
        }
        public void SetVisibility(bool isVisible)
        {
            base.Visible = isVisible;
        }
        // ~IPortalElement
    }
    public class PortalStatic : PortalElement
    {
        [Constructable]
        public PortalStatic(int gid, int entryKey, string name, string category)
            : base(gid, entryKey, name, category, DateTime.UtcNow)
		{
		}
        [Constructable]
        public PortalStatic(short quantity, int gid, int entryKey, string name, string category, DateTime placementTime)
            : base(gid, entryKey, name, category, placementTime)
        {
        }
        public PortalStatic(Serial serial)
            : base(serial)
        {
        }
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
		}
    }

    // Members are duplicated because we can't use multiple inheritance here.
    public class PortalDoor : BaseDoor, IPortalElement
    {
        public enum EDoorType
        {
            EDoorType_Invalid,
            EDoorType_IronGateShort,
            EDoorType_IronGate,
            EDoorType_LightWoodGate,
            EDoorType_DarkWoodGate,
            EDoorType_MetalDoor,
            EDoorType_BarredMetalDoor,
            EDoorType_BarredMetalDoor2,
            EDoorType_RattanDoor,
            EDoorType_DarkWoodDoor,
            EDoorType_MediumWoodDoor,
            EDoorType_MetalDoor2,
            EDoorType_LightWoodDoor,
            EDoorType_StrongWoodDoor,
            EDoorType_SecretDungeonDoor
        }

        public int m_gid;
        public int m_entryKey;
        public string m_name;
        public string m_category;
        public EDoorType m_doorType;
        public DoorFacing m_doorFacing;
        public DateTime m_placementTime;

        [Constructable]
        public PortalDoor(int gid, int entryKey, string name, string category, EDoorType type, DoorFacing facing)
            : base(GetClosedId(type,facing)
            , GetOpenedId(type,facing)
            , GetOpenedSound(type,facing)
            , GetClosedSound(type,facing)
            , GetOffset(facing))
		{
            m_gid = gid;
            m_entryKey = entryKey;
            m_name = name;
            m_category = category;
            m_doorType = type;
            m_doorFacing = facing;
            m_placementTime = DateTime.UtcNow;
		}
        [Constructable]
        public PortalDoor(int gid, int entryKey, string name, string category, EDoorType type, DoorFacing facing, DateTime placementTime)
            : base(GetClosedId(type, facing)
            , GetOpenedId(type, facing)
            , GetOpenedSound(type, facing)
            , GetClosedSound(type, facing)
            , GetOffset(facing))
        {
            m_gid = gid;
            m_entryKey = entryKey;
            m_name = name;
            m_category = category;
            m_doorType = type;
            m_doorFacing = facing;
            m_placementTime = placementTime;
        }
        public PortalDoor(Serial serial)
            : base(serial)
        {
        }

        /// <summary>
        /// The base gid is the closed version.
        /// </summary>
        /// <param name="type"></param>
        public static int GetDoorBaseGid(EDoorType type)
        {
            switch (type)
            {
                case EDoorType.EDoorType_IronGateShort: return 0x84c;
                case EDoorType.EDoorType_IronGate: return 0x824;
                case EDoorType.EDoorType_MetalDoor: return 0x675;
                case EDoorType.EDoorType_MetalDoor2: return 0x6C5;
                case EDoorType.EDoorType_BarredMetalDoor: return 0x685;
                case EDoorType.EDoorType_BarredMetalDoor2: return 0x1FED;
                case EDoorType.EDoorType_LightWoodGate: return  0x839;
                case EDoorType.EDoorType_DarkWoodGate: return  0x866;
                case EDoorType.EDoorType_RattanDoor: return  0x695;
                case EDoorType.EDoorType_DarkWoodDoor: return  0x6A5;
                case EDoorType.EDoorType_MediumWoodDoor: return  0x6B5;
                case EDoorType.EDoorType_LightWoodDoor: return  0x6D5;
                case EDoorType.EDoorType_StrongWoodDoor: return  0x6E5;
                case EDoorType.EDoorType_SecretDungeonDoor: return 0x314;
            }
            return 0;
        }
        public static EDoorType GetTypeFromGid(int gid)
        {
            if (gid >= 0x84c && gid <= 0x85A)
                return EDoorType.EDoorType_IronGateShort;
            else if (gid >= 0x824 && gid <= 0x832)
                return EDoorType.EDoorType_IronGate;
            else if (gid >= 0x675 && gid <= 0x683)
                return EDoorType.EDoorType_MetalDoor;
            else if (gid >= 0x84c && gid <= 0x85A)
                return EDoorType.EDoorType_MetalDoor2;
            else if (gid >= 0x685 && gid <= 0x693)
                return EDoorType.EDoorType_BarredMetalDoor;
            else if (gid >= 0x1FED && gid <= 0x1FFB)
                return EDoorType.EDoorType_BarredMetalDoor2;
            else if (gid >= 0x839 && gid <= 0x847)
                return EDoorType.EDoorType_LightWoodGate;
            else if (gid >= 0x866 && gid <= 0x874)
                return EDoorType.EDoorType_DarkWoodGate;
            else if (gid >= 0x695 && gid <= 0x6A3)
                return EDoorType.EDoorType_RattanDoor;
            else if (gid >= 0x6A5 && gid <= 0x6B3)
                return EDoorType.EDoorType_DarkWoodDoor;
            else if (gid >= 0x6B5 && gid <= 0x6C3)
                return EDoorType.EDoorType_MediumWoodDoor;
            else if (gid >= 0x6D5 && gid <= 0x6E3)
                return EDoorType.EDoorType_LightWoodDoor;
            else if (gid >= 0x6E5 && gid <= 0x6F3)
                return EDoorType.EDoorType_StrongWoodDoor;
            else if (gid >= 0x314 && gid <= 0x322)
                return EDoorType.EDoorType_SecretDungeonDoor;

            return EDoorType.EDoorType_Invalid;
        }
        public static DoorFacing GetFacingFromGid(int gid)
        {
            EDoorType type = GetTypeFromGid(gid);
            int baseGid = GetDoorBaseGid(type);
            // Unique facing is staggered by 2.
            // Invalid enum at index 0 requires offset by 1.
            int delta = gid - baseGid;
            //return (DoorFacing)((delta / 2) + 1);
            return (DoorFacing)(delta / 2);
        }
        // deprecated - name is now provided in itemization xml
        //public static string GetDoorName(EDoorType type)
        //{
        //    switch (type)
        //    {
        //        case EDoorType.EDoorType_IronGateShort: return "Short Iron Gate";
        //        case EDoorType.EDoorType_IronGate: return "Iron Gate";
        //        case EDoorType.EDoorType_MetalDoor: return "Metal Door";
        //        case EDoorType.EDoorType_MetalDoor2: return "Metal Door (v2)";
        //        case EDoorType.EDoorType_BarredMetalDoor: return "Barred Metal Door";
        //        case EDoorType.EDoorType_BarredMetalDoor2: return "Barred Metal Door (v2)";
        //        case EDoorType.EDoorType_LightWoodGate: return "Light Wood Gate";
        //        case EDoorType.EDoorType_DarkWoodGate: return "Dark Wood Gate";
        //        case EDoorType.EDoorType_RattanDoor: return "Rattan Door";
        //        case EDoorType.EDoorType_DarkWoodDoor: return "Dark Wood Door";
        //        case EDoorType.EDoorType_MediumWoodDoor: return "Medium Wood Door";
        //        case EDoorType.EDoorType_LightWoodDoor: return "Light Wood Door";
        //        case EDoorType.EDoorType_StrongWoodDoor: return "Strong Wood Door";
        //        case EDoorType.EDoorType_SecretDungeonDoor: return "Dungeon Wall";
        //    }
        //    return "";
        //}
        /// <summary>
        /// The dungeon partition system requires the gid of the closed door
        /// to resolve the selection request from the dungeon controller gump.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="facing"></param>
        /// <returns></returns>
        public static int GetClosedId(EDoorType type, DoorFacing facing)
        {
            int gid = 0;
            switch (type)
            {
                case EDoorType.EDoorType_IronGateShort: { gid = 0x84c; break; }
                case EDoorType.EDoorType_IronGate: { gid = 0x824; break; }
                case EDoorType.EDoorType_MetalDoor: { gid = 0x675; break; }
                case EDoorType.EDoorType_MetalDoor2: { gid = 0x6C5; break; }
                case EDoorType.EDoorType_BarredMetalDoor: { gid = 0x685; break; }
                case EDoorType.EDoorType_BarredMetalDoor2: { gid = 0x1FED; break; }
                case EDoorType.EDoorType_LightWoodGate: { gid = 0x839; break; }
                case EDoorType.EDoorType_DarkWoodGate: { gid = 0x866; break; }
                case EDoorType.EDoorType_RattanDoor: { gid = 0x695; break; }
                case EDoorType.EDoorType_DarkWoodDoor: { gid = 0x6A5; break; }
                case EDoorType.EDoorType_MediumWoodDoor: { gid = 0x6B5; break; }
                case EDoorType.EDoorType_LightWoodDoor: { gid = 0x6D5; break; }
                case EDoorType.EDoorType_StrongWoodDoor: { gid = 0x6E5; break; }
                case EDoorType.EDoorType_SecretDungeonDoor: { gid = 0x314; break; }
            }

            return gid + (2 * (int)facing);
        }
        private static int GetOpenedId(EDoorType type, DoorFacing facing)
        {
            int gid = 0;
            switch (type)
            {
                case EDoorType.EDoorType_IronGateShort: { gid = 0x84d; break; }
                case EDoorType.EDoorType_IronGate: { gid = 0x825; break; }
                case EDoorType.EDoorType_MetalDoor: { gid = 0x676; break; }
                case EDoorType.EDoorType_MetalDoor2: { gid = 0x6C6; break; }
                case EDoorType.EDoorType_BarredMetalDoor: { gid = 0x686; break; }
                case EDoorType.EDoorType_BarredMetalDoor2: { gid = 0x1FEE; break; }
                case EDoorType.EDoorType_LightWoodGate: { gid = 0x83A; break; }
                case EDoorType.EDoorType_DarkWoodGate: { gid = 0x867; break; }
                case EDoorType.EDoorType_RattanDoor: { gid = 0x696; break; }
                case EDoorType.EDoorType_DarkWoodDoor: { gid = 0x6A6; break; }
                case EDoorType.EDoorType_MediumWoodDoor: { gid = 0x6B6; break; }
                case EDoorType.EDoorType_LightWoodDoor: { gid = 0x6D6; break; }
                case EDoorType.EDoorType_StrongWoodDoor: { gid = 0x6E6; break; }
                case EDoorType.EDoorType_SecretDungeonDoor: { gid = 0x315; break; }
            }
            return gid + (2 * (int)facing);
        }
        private static int GetOpenedSound(EDoorType doorType, DoorFacing facing)
        {
            switch (doorType)
            {
                case EDoorType.EDoorType_IronGateShort:
                case EDoorType.EDoorType_IronGate:
                case EDoorType.EDoorType_MetalDoor:
                case EDoorType.EDoorType_MetalDoor2:
                case EDoorType.EDoorType_BarredMetalDoor:
                case EDoorType.EDoorType_BarredMetalDoor2:
                case EDoorType.EDoorType_SecretDungeonDoor:
                    return 0xEC;
                case EDoorType.EDoorType_LightWoodGate:
                case EDoorType.EDoorType_DarkWoodGate:
                case EDoorType.EDoorType_RattanDoor:
                    return 0xEB;
                case EDoorType.EDoorType_DarkWoodDoor:
                case EDoorType.EDoorType_MediumWoodDoor:
                case EDoorType.EDoorType_LightWoodDoor:
                case EDoorType.EDoorType_StrongWoodDoor:
                    return 0xEA;
            }

            return 0xEC;
        }
        private static int GetClosedSound(EDoorType doorType, DoorFacing facing)
        {
            switch (doorType)
            {
                case EDoorType.EDoorType_IronGateShort:
                case EDoorType.EDoorType_IronGate:
                case EDoorType.EDoorType_MetalDoor:
                case EDoorType.EDoorType_MetalDoor2:
                case EDoorType.EDoorType_BarredMetalDoor:
                case EDoorType.EDoorType_BarredMetalDoor2:
                case EDoorType.EDoorType_SecretDungeonDoor:
                    return 0xF3;
                case EDoorType.EDoorType_LightWoodGate:
                case EDoorType.EDoorType_DarkWoodGate:
                case EDoorType.EDoorType_RattanDoor:
                    return 0xF2;
                case EDoorType.EDoorType_DarkWoodDoor:
                case EDoorType.EDoorType_MediumWoodDoor:
                case EDoorType.EDoorType_LightWoodDoor:
                case EDoorType.EDoorType_StrongWoodDoor:
                    return 0xF1;
            }
            return 0xEC;
        }
        
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

            int version = 0;
			writer.Write(version);

            writer.Write(m_gid);
            writer.Write(m_entryKey);
            writer.Write(m_name);
            writer.Write(m_category);
            writer.Write(m_placementTime);
            writer.Write((int)m_doorType);
            writer.Write((int)m_doorFacing);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_gid = reader.ReadInt();
                m_entryKey = reader.ReadInt();
                m_name = reader.ReadString();
                m_category = reader.ReadString();
                m_placementTime = reader.ReadDateTime();
                m_doorType = (EDoorType)reader.ReadInt();
                m_doorFacing = (DoorFacing)reader.ReadInt();
            }
        }
        // IPortalElement
        public int GetGid()
        {
            return m_gid;
        }
        public int GetEntryKey()
        {
            return m_entryKey;
        }
        public string GetName()
        {
            return m_name;
        }
        public string GetCategory()
        {
            return m_category;
        }
        public Point3D GetLocation()
        {
            return this.Location;
        }
        public void MoveToWorldWrapper(Point3D point)
        {
            base.MoveToWorld(point);
        }
        public void MoveToWorldWrapper(Point3D point, Map map)
        {
            base.MoveToWorld(point, map);
        }
        public Point3D GetSurfaceTopWrapper()
        {
            return base.GetSurfaceTop();
        }
        public void SetVisibility(bool isVisible)
        {
            base.Visible = isVisible;
        }
        // ~IPortalElement
    }
   
    /// <summary>
    /// Remove during beta.
    /// </summary>
    public class DungeonTestInjector : DungeonInjector
    {
         public DungeonTestInjector()
            : base("A complete inventory injector")
        {
        }
        public DungeonTestInjector(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    }

    public class PortalRecycleBin : BaseContainer
    {
        public PortalRecycleBin()
            : base(0xE41)
        {
            Direction = Direction.South;
            Movable = false;
            MaxItems = Int32.MaxValue;
            Weight = -1;
        }
        public PortalRecycleBin(Serial serial)
            : base(serial)
        {
        }

        public override bool Decays
        {
            get
            {
                return false;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            PortalPartition partition = PortalsSystem.GetPartitionAtPoint(this.Location);
            if (partition != null)
            {
                if (partition.m_username == from.Account.Username)
                {
                    base.OnDoubleClick(from);
                }
            }
        }
        public override bool TryDropItem(Mobile from, Item dropped, bool sendFullMessage)
        {
            return false;
        }

        public override bool OnDragDropInto(Mobile from, Item item, Point3D p)
        {
            return false;
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
        public override string DefaultName
        {
            get
            {
                return "Dungeon Bin";
            }
        }
    }
}
