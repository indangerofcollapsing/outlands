using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;
using Server.Targeting;

namespace Server.ArenaSystem
{
    public interface IArenaItem
    {
        Serial GetArenaSerial();
    }
    public class EjectionPoint : Item, IArenaItem
    {
        public int m_parentSerialId;
        public EjectionPoint(Point3D p, Map map, int parentSerialId)
            : base(0x1808)
        {
            m_parentSerialId = parentSerialId;
            Location = p;
            Map = map;
            Visible = false;
            Movable = false;
        }
        public EjectionPoint(Serial serial)
            : base(serial)
        {
            Visible = false;
            Movable = false;
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        public override string DefaultName
        {
            get { return "Eject Location"; }
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem
    }
    public class SpectatorPoint : Item, IArenaItem
    {
        public int m_parentSerialId;
        public SpectatorPoint(Point3D p, Map map, int parentSerialId)
            : base(0x1808)
        {
            m_parentSerialId = parentSerialId;
            Location = p;
            Map = map;
            Visible = false;
            Movable = false;
        }
        public SpectatorPoint(Serial serial)
            : base(serial)
        {
            Visible = false;
            Movable = false;
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        public override string DefaultName
        {
            get { return "Spectator Location"; }
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem
    }
    public class AnnouncerPoint : Item, IArenaItem
    {
        public int m_parentSerialId;
        public AnnouncerPoint(Point3D p, Map map, int parentSerialId)
            : base(0x1809)
        {
            m_parentSerialId = parentSerialId;
            Location = p;
            Map = map;
            Visible = false;
            Movable = false;
        }
        public AnnouncerPoint(Serial serial)
            : base(serial)
        {
            Visible = false;
            Movable = false;
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        public override string DefaultName
        {
            get { return "Announcer Location"; }
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem
    }
    public class TeamPoint : Item, IArenaItem
    {
        public int m_parentSerialId;
        public bool m_team1;
        public TeamPoint(Point3D p, Map map, bool team1, int parentSerialId)
            : base(team1 ? 0x17D3 : 0x180B)
        {
            m_parentSerialId = parentSerialId;
            m_team1 = team1;
            Location = p;
            Map = map;
            Visible = false;
            Movable = false;
        }
        public TeamPoint(Serial serial)
            : base(serial)
        {
            Visible = false;
            Movable = false;
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_team1);
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_team1 = reader.ReadBool();
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        public override string DefaultName
        {
            get { return "Team " + (m_team1 ? "1" : "2") + " Location"; }
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem
    }
    // 0x2199 - LOSBlocker.cs: Line of sight and pathing blocker
    // 0x21A4 - Blocker.cs: Pathing blocker
    public class ArenaBoundary : Blocker, IArenaItem
    {
        public int m_parentSerialId;
        public ArenaBoundary(int parentSerialId)
        {
            //TileData.ItemTable[0x21A4].Flags = TileFlag.Wall | TileFlag.NoShoot;
            //TileData.ItemTable[0x21A4].Height = 50;
            m_parentSerialId = parentSerialId;
        }
        public ArenaBoundary(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem
    }

    /// <summary>
    /// Represents a physical component of the arena.
    /// </summary>
    public class ArenaStatic : Static, IArenaItem
    {
        public int m_parentSerialId;
        public ArenaStatic(int staticId, int parentSerialId)
            : base(staticId)
        {
            m_parentSerialId = parentSerialId;
        }
        public ArenaStatic(Serial serial)
            : base(serial)
        {
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write(m_parentSerialId);
            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            m_parentSerialId = reader.ReadInt();
            base.Deserialize(reader);
        }
        // IArenaItem
        public Serial GetArenaSerial() { return m_parentSerialId; }
        // ~IArenaItem
    }
    public class Create1v1ArenaTarget : Target
    {
        public Create1v1ArenaTarget()
                : base(-1, true, TargetFlags.None, false)
        {
        }
        protected override void OnTarget(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;
			if (p != null)
			{
				Arena1v1 arena = new Arena1v1(new Point3D(p.X, p.Y, p.Z), from.Map);
			}
			from.SendGump(new ArenaAdminGump(from, ArenaAdminGump.ListType.Arenas));
        }
    }
    public class Create2v2ArenaTarget : Target
    {
        public Create2v2ArenaTarget()
                : base(-1, true, TargetFlags.None, false)
        {
        }
		protected override void OnTarget(Mobile from, object targeted)
		{
			IPoint3D p = targeted as IPoint3D;
			if (p != null)
			{
				Arena2v2 arena = new Arena2v2(new Point3D(p.X, p.Y, p.Z), from.Map);
			}
			from.SendGump(new ArenaAdminGump(from, ArenaAdminGump.ListType.Arenas));
		}
    }
    public class Create3v3ArenaTarget : Target
    {
        public Create3v3ArenaTarget()
                : base(-1, true, TargetFlags.None, false)
        {
        }
		protected override void OnTarget(Mobile from, object targeted)
		{
			IPoint3D p = targeted as IPoint3D;
			if (p != null)
			{
				Arena3v3 arena = new Arena3v3(new Point3D(p.X, p.Y, p.Z), from.Map);
			}
			from.SendGump(new ArenaAdminGump(from, ArenaAdminGump.ListType.Arenas));
		}
    }
    public class Arena1v1 : Arena
    {
        [Constructable]
        public Arena1v1(Point3D anchor, Map map)
            : base(anchor, map, EArenaSize.eAS_1v1)
        {
            // Generate the arena.
            Construct();
        }
        public Arena1v1(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        private void Construct()
        {
            Point3D anchorPoint = new Point3D(Location.X, Location.Y - 1, Location.Z);

            #region Definition
            List<Pair<int, Point3D>> arenaPieces = new List<Pair<int, Point3D>>();

            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(12, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(10, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(8, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(6, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(4, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(12, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(11, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(10, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(9, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(8, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(7, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(6, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(5, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(4, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(1, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(3, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(2, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -15, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(1, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(2, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -16, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(3, -16, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20705, new Point3D(3, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -13, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -5, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -7, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -9, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -11, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(4, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(12, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(12, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(11, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(10, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(9, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(8, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(7, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(6, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(5, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(4, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(12, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(11, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(10, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(9, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(8, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(7, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(6, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(5, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(4, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(8, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(10, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(9, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(8, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(7, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(6, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(5, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(6, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(4, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(10, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20703, new Point3D(3, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(11, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(3, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(1, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(2, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5568, new Point3D(3, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(5568, new Point3D(0, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(12, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(3, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -3, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(1, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(2, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -1, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -2, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -14, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -13, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(13, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(14, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(5569, new Point3D(17, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -16, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(13, -16, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(14, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(15, -16, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -15, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20702, new Point3D(13, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(15, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(15, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(5564, new Point3D(13, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -11, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -9, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -7, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -5, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20704, new Point3D(13, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5565, new Point3D(17, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(13, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(14, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -13, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5569, new Point3D(17, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -3, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -13, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(13, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -14, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(14, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(15, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -1, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -2, 15)));

            foreach (Pair<int, Point3D> pair in arenaPieces)
            {
                ArenaStatic s = new ArenaStatic(pair.First, this.Serial);
                Point3D localPoint = pair.Second;
                Point3D worldPoint = new Point3D(localPoint.X + anchorPoint.X, localPoint.Y + anchorPoint.Y, localPoint.Z + anchorPoint.Z);
                s.MoveToWorld(worldPoint, Map);
            }
            #endregion // Definition

            ArenaStatic sign1 = new ArenaStatic(3030, this.Serial);
            sign1.Name = "A 1v1 Arena";
            Point3D signLocalPoint = new Point3D(17, -1, 0);
            Point3D signWorldPoint = new Point3D(signLocalPoint.X + anchorPoint.X, signLocalPoint.Y + anchorPoint.Y, signLocalPoint.Z + anchorPoint.Z);
            sign1.MoveToWorld(signWorldPoint, Map);

            ArenaStatic sign2 = new ArenaStatic(3029, this.Serial);
            sign2.Name = "A 1v1 Arena";
            signLocalPoint = new Point3D(2, 1, 0);
            signWorldPoint = new Point3D(signLocalPoint.X + anchorPoint.X, signLocalPoint.Y + anchorPoint.Y, signLocalPoint.Z + anchorPoint.Z);
            sign2.MoveToWorld(signWorldPoint, Map);

            m_team1Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 5, anchorPoint.Z + 10), Map, true, (int)Serial));
            m_team1EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y + 2, anchorPoint.Z), Map, (int)Serial));

            m_team2Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 11, anchorPoint.Z + 10), Map, false, (int)Serial));
            m_team2EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 19, anchorPoint.Z), Map, (int)Serial));

            m_announcerPoint = new AnnouncerPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 8, anchorPoint.Z + 10), Map, (int)Serial);

            Point2D start = new Point2D(anchorPoint.X + 3, anchorPoint.Y - 13);
            Point2D end = new Point2D(anchorPoint.X + 13, anchorPoint.Y - 3);
            m_boundary = new Rectangle2D(start, end);

            Rectangle2D interior = new Rectangle2D(start.X + 1, start.Y + 1, end.X - start.X - 1, end.Y - start.Y - 1);
            for (int x = start.X; x <= end.X; ++x)
            {
                for (int y = start.Y; y <= end.Y; ++y)
                {
                    if (interior.Contains(new Point2D(x, y)))
                        continue;

                    ArenaBoundary boundary = new ArenaBoundary((int)Serial);
                    boundary.MoveToWorld(new Point3D(x, y, anchorPoint.Z + 10), Map);
                }
            }

            CreateRegion(start, end);

            // Create an arena board.
            Point3D worldPoint1 = new Point3D(anchorPoint.X + 15, anchorPoint.Y + 1, Location.Z);
            ArenaBoard board1 = new ArenaBoard(true, Serial);
            board1.MoveToWorld(worldPoint1, Map);

            Point3D worldPoint2 = new Point3D(anchorPoint.X + 18, anchorPoint.Y - 14, Location.Z + 10);
            ArenaBoard board2 = new ArenaBoard(false, Serial);
            board2.MoveToWorld(worldPoint2, Map);

            // Create an arena vendor.
            Point3D worldPoint3 = new Point3D(anchorPoint.X + 18, anchorPoint.Y + 1, Location.Z + 10);
            ArenaVendor vendor = new ArenaVendor();
            vendor.CantWalk = true;
            vendor.Frozen = true;
            vendor.Direction = Direction.East;
            vendor.MoveToWorld(worldPoint3, Map);
        }
    }
    public class Arena2v2 : Arena
    {
        [Constructable]
        public Arena2v2(Point3D anchor, Map map)
            : base(anchor, map, EArenaSize.eAS_2v2)
        {
            // Generate the arena.
            Construct();
        }
        public Arena2v2(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        private void Construct()
        {
            Point3D anchorPoint = new Point3D(Location.X, Location.Y - 1, Location.Z);

            #region Definition
            List<Pair<int, Point3D>> arenaPieces = new List<Pair<int, Point3D>>();

            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(12, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(10, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(8, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(6, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(4, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(12, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(11, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(10, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(9, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(8, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(7, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(6, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(5, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(4, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(1, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(3, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(2, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -19, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(1, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(2, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -20, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(3, -20, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(1, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -18, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20705, new Point3D(3, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -17, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(12, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(11, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(10, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(9, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(8, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(7, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(6, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(5, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(4, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -17, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -5, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -7, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -9, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -11, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(4, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(12, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(12, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(11, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(10, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(9, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(8, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(7, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(6, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(5, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(4, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(8, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(10, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(9, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(8, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(7, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(6, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(5, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(6, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(4, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(10, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20703, new Point3D(3, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(11, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(3, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(2, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5568, new Point3D(3, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(5568, new Point3D(0, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(12, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(3, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -3, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(1, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(2, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -1, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -2, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(13, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(14, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -20, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(13, -20, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(14, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(15, -20, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -19, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(15, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -18, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20704, new Point3D(13, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -17, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -17, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20702, new Point3D(13, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(15, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(5564, new Point3D(13, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -11, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -9, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -7, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -5, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(13, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(14, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -3, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(13, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(14, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(15, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -1, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -2, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -13, 10)));

            foreach (Pair<int, Point3D> pair in arenaPieces)
            {
                ArenaStatic s = new ArenaStatic(pair.First, this.Serial);
                Point3D localPoint = pair.Second;
                Point3D worldPoint = new Point3D(localPoint.X + anchorPoint.X, localPoint.Y + anchorPoint.Y, localPoint.Z + anchorPoint.Z);
                s.MoveToWorld(worldPoint, Map);
            }
            #endregion // Definition

            ArenaStatic sign1 = new ArenaStatic(3030, this.Serial);
            sign1.Name = "A 2v2 Arena";
            Point3D signLocalPoint = new Point3D(17, -1, 0);
            Point3D signWorldPoint = new Point3D(signLocalPoint.X + anchorPoint.X, signLocalPoint.Y + anchorPoint.Y, signLocalPoint.Z + anchorPoint.Z);
            sign1.MoveToWorld(signWorldPoint, Map);

            ArenaStatic sign2 = new ArenaStatic(3029, this.Serial);
            sign2.Name = "A 2v2 Arena";
            signLocalPoint = new Point3D(2, 1, 0);
            signWorldPoint = new Point3D(signLocalPoint.X + anchorPoint.X, signLocalPoint.Y + anchorPoint.Y, signLocalPoint.Z + anchorPoint.Z);
            sign2.MoveToWorld(signWorldPoint, Map);

            m_team1Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y - 5, anchorPoint.Z + 10), Map, true, (int)Serial));
            m_team1EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y + 2, anchorPoint.Z), Map, (int)Serial));
            m_team1Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y - 5, anchorPoint.Z + 10), Map, true, (int)Serial));
            m_team1EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y + 2, anchorPoint.Z), Map, (int)Serial));

            m_team2Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y - 15, anchorPoint.Z + 10), Map, false, (int)Serial));
            m_team2EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y - 23, anchorPoint.Z), Map, (int)Serial));
            m_team2Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y - 15, anchorPoint.Z + 10), Map, false, (int)Serial));
            m_team2EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y - 23, anchorPoint.Z), Map, (int)Serial));

            m_announcerPoint = new AnnouncerPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 10, anchorPoint.Z + 10), Map, (int)Serial);

            Point2D start = new Point2D(anchorPoint.X + 3, anchorPoint.Y - 17);
            Point2D end = new Point2D(anchorPoint.X + 13, anchorPoint.Y - 3);
            m_boundary = new Rectangle2D(start, end);

            Rectangle2D interior = new Rectangle2D(start.X + 1, start.Y + 1, end.X - start.X - 1, end.Y - start.Y - 1);
            for (int x = start.X; x <= end.X; ++x)
            {
                for (int y = start.Y; y <= end.Y; ++y)
                {
                    if (interior.Contains(new Point2D(x, y)))
                        continue;

                    ArenaBoundary boundary = new ArenaBoundary((int)Serial);
                    boundary.MoveToWorld(new Point3D(x, y, anchorPoint.Z + 10), Map);
                }
            }

            CreateRegion(start,end);
            
            // Create an arena board.
            Point3D worldPoint1 = new Point3D(anchorPoint.X + 15, anchorPoint.Y + 1, Location.Z);
            ArenaBoard board1 = new ArenaBoard(true, Serial);
            board1.MoveToWorld(worldPoint1, Map);

            Point3D worldPoint2 = new Point3D(anchorPoint.X + 18, anchorPoint.Y - 18, Location.Z + 10);
            ArenaBoard board2 = new ArenaBoard(false, Serial);
            board2.MoveToWorld(worldPoint2, Map);
            
            // Create an arena vendor.
            Point3D worldPoint3 = new Point3D(anchorPoint.X + 18, anchorPoint.Y + 1, Location.Z + 10);
            ArenaVendor vendor = new ArenaVendor();
            vendor.CantWalk = true;
            vendor.Frozen = true;
            vendor.Direction = Direction.East;
            vendor.MoveToWorld(worldPoint3, Map);
        }
    }
    public class Arena3v3 : Arena
    {
        [Constructable]
        public Arena3v3(Point3D anchor, Map map)
            : base(anchor, map, EArenaSize.eAS_3v3)
        {
            // Generate the arena.
            Construct();
        }
        public Arena3v3(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
        public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }
        private void Construct()
        {
            Point3D anchorPoint = new Point3D(Location.X, Location.Y - 1, Location.Z);

            #region Definition
            List<Pair<int, Point3D>> arenaPieces = new List<Pair<int, Point3D>>();

            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(12, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(10, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(8, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(6, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(4, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(12, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(11, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(10, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(9, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(8, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(7, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(6, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(5, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1828, new Point3D(4, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -23, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(1, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(3, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(2, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -23, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(1, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(2, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -24, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(3, -24, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -22, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(1, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -22, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20705, new Point3D(3, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -21, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -21, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -21, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -21, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(12, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(11, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(10, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(9, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(8, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(7, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(6, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(5, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20717, new Point3D(4, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -21, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -3, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -5, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -7, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -9, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -11, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(4, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(12, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(12, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(11, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(10, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(9, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(8, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(7, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(6, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(5, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20712, new Point3D(4, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(12, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(10, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(9, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(8, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(7, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(6, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(5, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(4, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(8, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(10, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(9, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(8, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(7, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(6, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(5, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(6, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(4, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(10, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20703, new Point3D(3, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(11, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(11, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(0, -1, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(3, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(2, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(1, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(3, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5568, new Point3D(3, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(5568, new Point3D(0, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1826, new Point3D(12, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(3, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, -3, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(0, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(1, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(2, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -1, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(0, -2, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(1, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(2, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(0, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1829, new Point3D(1, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20715, new Point3D(3, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(4, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(5, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(6, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(7, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(8, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(9, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(10, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(11, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(12, -13, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -23, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(13, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -24, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(14, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -24, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -23, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -23, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(5569, new Point3D(17, -24, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -24, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(13, -24, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(14, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(15, -24, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -23, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -22, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(15, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -22, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -22, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -22, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -21, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -21, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20704, new Point3D(13, -21, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -21, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -21, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5569, new Point3D(17, -21, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -21, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -15, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -15, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -16, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -16, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -16, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -15, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -17, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -19, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -18, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -20, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -20, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -19, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -18, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -17, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -17, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -18, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -19, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -20, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -4, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20702, new Point3D(13, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(15, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(5564, new Point3D(13, 1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -12, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -12, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -12, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -1, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -11, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -10, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -9, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -8, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -7, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -6, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -5, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -3, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -11, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -10, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -7, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -6, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -5, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -4, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -8, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -9, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -2, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(5565, new Point3D(17, -3, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -11, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -10, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -9, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -8, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -7, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -6, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -5, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, 0, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, 0, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3215, new Point3D(13, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(14, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(13, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(16, -3, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -1, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(3216, new Point3D(15, -2, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, -3, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(16, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(20707, new Point3D(13, 0, 20)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -4, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(14, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(15, 0, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -1, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(20706, new Point3D(16, -2, 15)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(16, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(15, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -14, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -13, 0)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -13, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1825, new Point3D(14, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(1827, new Point3D(15, -14, 5)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -14, 10)));
            arenaPieces.Add(new Pair<int, Point3D>(20714, new Point3D(13, -13, 10)));

            foreach (Pair<int, Point3D> pair in arenaPieces)
            {
                ArenaStatic s = new ArenaStatic(pair.First, this.Serial);
                Point3D localPoint = pair.Second;
                Point3D worldPoint = new Point3D(localPoint.X + anchorPoint.X, localPoint.Y + anchorPoint.Y, localPoint.Z + anchorPoint.Z);
                s.MoveToWorld(worldPoint, Map);
            }

#endregion // Definition

            ArenaStatic sign1 = new ArenaStatic(3030, this.Serial);
            sign1.Name = "A 3v3 Arena";
            Point3D signLocalPoint = new Point3D(17, -1, 0);
            Point3D signWorldPoint = new Point3D(signLocalPoint.X + anchorPoint.X, signLocalPoint.Y + anchorPoint.Y, signLocalPoint.Z + anchorPoint.Z);
            sign1.MoveToWorld(signWorldPoint, Map);

            ArenaStatic sign2 = new ArenaStatic(3029, this.Serial);
            sign2.Name = "A 3v3 Arena";
            signLocalPoint = new Point3D(2, 1, 0);
            signWorldPoint = new Point3D(signLocalPoint.X + anchorPoint.X, signLocalPoint.Y + anchorPoint.Y, signLocalPoint.Z + anchorPoint.Z);
            sign2.MoveToWorld(signWorldPoint, Map);

            m_team1Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y - 5, anchorPoint.Z + 10), Map, true, (int)Serial));
            m_team1EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y + 2, anchorPoint.Z), Map, (int)Serial));
            m_team1Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 5, anchorPoint.Z + 10), Map, true, (int)Serial));
            m_team1EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y + 2, anchorPoint.Z), Map, (int)Serial));
            m_team1Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y - 5, anchorPoint.Z + 10), Map, true, (int)Serial));
            m_team1EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y + 2, anchorPoint.Z), Map, (int)Serial));

            m_team2Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y - 19, anchorPoint.Z + 10), Map, false, (int)Serial));
            m_team2EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 6, anchorPoint.Y - 27, anchorPoint.Z), Map, (int)Serial));
            m_team2Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 19, anchorPoint.Z + 10), Map, false, (int)Serial));
            m_team2EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 27, anchorPoint.Z), Map, (int)Serial));
            m_team2Points.Add(new TeamPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y - 19, anchorPoint.Z + 10), Map, false, (int)Serial));
            m_team2EjectionPoints.Add(new EjectionPoint(new Point3D(anchorPoint.X + 10, anchorPoint.Y - 27, anchorPoint.Z), Map, (int)Serial));

            m_announcerPoint = new AnnouncerPoint(new Point3D(anchorPoint.X + 8, anchorPoint.Y - 12, anchorPoint.Z + 10), Map, (int)Serial);

            Point2D start = new Point2D(anchorPoint.X + 3, anchorPoint.Y - 21);
            Point2D end = new Point2D(anchorPoint.X + 13, anchorPoint.Y - 3);
            m_boundary = new Rectangle2D(start, end);

            Rectangle2D interior = new Rectangle2D(start.X + 1, start.Y + 1, end.X - start.X - 1, end.Y - start.Y - 1);
            for (int x = start.X; x <= end.X; ++x)
            {
                for (int y = start.Y; y <= end.Y; ++y)
                {
                    if (interior.Contains(new Point2D(x, y)))
                        continue;

                    ArenaBoundary boundary = new ArenaBoundary((int)Serial);
                    boundary.MoveToWorld(new Point3D(x, y, anchorPoint.Z + 10), Map);
                }
            }

            CreateRegion(start,end);

            // Create an arena board.
            Point3D worldPoint1 = new Point3D(anchorPoint.X + 15, anchorPoint.Y + 1, Location.Z);
            ArenaBoard board1 = new ArenaBoard(true, Serial);
            board1.MoveToWorld(worldPoint1, Map);

            Point3D worldPoint2 = new Point3D(anchorPoint.X + 18, anchorPoint.Y - 22, Location.Z + 10);
            ArenaBoard board2 = new ArenaBoard(false, Serial);
            board2.MoveToWorld(worldPoint2, Map);

            // Create an arena vendor.
            Point3D worldPoint3 = new Point3D(anchorPoint.X + 18, anchorPoint.Y + 1, Location.Z + 10);
            ArenaVendor vendor = new ArenaVendor();
            vendor.CantWalk = true;
            vendor.Frozen = true;
            vendor.Direction = Direction.East;
            vendor.MoveToWorld(worldPoint3, Map);
        }
    }

    public abstract class Arena : Item, IArenaItem
    {
        // IArenaItem
        public Serial GetArenaSerial() { return Serial; }
        // ~IArenaItem

        public enum EArenaCustomizationTypes
        {
            eACT_None,
            eACT_VariableHeight,
            eACT_FixedObstacles,
            eACT_DynamicObstacles,
        }
        public enum EArenaSize
        {
            eAS_1v1,
            eAS_2v2,
            eAS_3v3,
        }

        public int Size
        {
            get
            {
                switch (m_size)
                {
                    case EArenaSize.eAS_1v1: return 1;
                    case EArenaSize.eAS_2v2: return 2;
                    case EArenaSize.eAS_3v3: return 3;
                    default: return -1;
                }
            }
        }
        protected EArenaSize m_size;
        protected List<TeamPoint> m_team1Points;
        protected List<TeamPoint> m_team2Points;
        protected List<EjectionPoint> m_team1EjectionPoints;
        protected List<EjectionPoint> m_team2EjectionPoints;
        protected AnnouncerPoint m_announcerPoint;
        protected Rectangle2D m_boundary;

        public List<Point3D> Team1EjectionLocations
        {
            get
            {
                List<Point3D> list = new List<Point3D>();
                foreach (EjectionPoint point in m_team1EjectionPoints)
                {
                    list.Add(point.Location);
                }
                return list;
            }
        }
        public List<Point3D> Team2EjectionLocations
        {
            get
            {
                List<Point3D> list = new List<Point3D>();
                foreach (EjectionPoint point in m_team2EjectionPoints)
                {
                    list.Add(point.Location);
                }
                return list;
            }
        }
        public Point3D AnnouncerLocation { get { return m_announcerPoint.Location; } }
        public List<Point3D> Team1Locations
        {
            get
            {
                List<Point3D> list = new List<Point3D>();
                foreach(TeamPoint point in m_team1Points)
                {
                    list.Add(point.Location);
                }
                return list;
            }
        }
        public List<Point3D> Team2Locations
        {
            get
            {
                List<Point3D> list = new List<Point3D>();
                foreach (TeamPoint point in m_team2Points)
                {
                    list.Add(point.Location);
                }
                return list;
            }
        }

        protected ArenaCombatRegion m_arenaCombatRegion;

        public ArenaMatch MatchInProgress { get; set; }

        public Arena(Point3D anchor, Map map, EArenaSize size)
			: base(0xED4)
        {
            Location = anchor;
            Map = map;
            Visible = false;
            Movable = false;

            m_size = size;
            m_team1Points = new List<TeamPoint>();
            m_team2Points = new List<TeamPoint>();
            m_team1EjectionPoints = new List<EjectionPoint>();
            m_team2EjectionPoints = new List<EjectionPoint>();

            MatchInProgress = null;

            ArenaSystem.RegisterArena(this);
        }
        public Arena(Serial serial)
            : base(serial)
        {
            Visible = false;
            Movable = false;
            m_team1Points = new List<TeamPoint>();
            m_team2Points = new List<TeamPoint>();
            m_team1EjectionPoints = new List<EjectionPoint>();
            m_team2EjectionPoints = new List<EjectionPoint>();
        }
        public override string DefaultName
        {
            get
            {
                return "An arena node";
            }
        }

        public override void OnDelete()
        {
            // Remove any items associated with the arena.
            List<Item> itemList = new List<Item>();
			IPooledEnumerable penum = Map.Felucca.GetItemsInRange(Location, 50);
            foreach (Item item in penum)
            {
                if (!(item is Arena) && (item is IArenaItem && ((IArenaItem)item).GetArenaSerial() == Serial))
                {
                    itemList.Add(item as Item);
                }
            }
            penum.Free();

            foreach (Item item in itemList)
            {
                item.Delete();
            }

            ArenaSystem.UnregisterArena(this);

			if (m_arenaCombatRegion != null)
            {
				m_arenaCombatRegion.Unregister();
            }

            base.OnDelete();
        }
        public override void Serialize(GenericWriter writer)
        {
            writer.Write((int)4);

            writer.Write((int)m_size);
            // Team points
            writer.Write(m_team1Points.Count);
            foreach (TeamPoint point in m_team1Points)
            {
                writer.Write((int)point.Serial);
            }
            foreach (TeamPoint point in m_team2Points)
            {
                writer.Write((int)point.Serial);
            }
            // Ejection points
            foreach (EjectionPoint point in m_team1EjectionPoints)
            {
                writer.Write((int)point.Serial);
            }
            foreach (EjectionPoint point in m_team2EjectionPoints)
            {
                writer.Write((int)point.Serial);
            }
            // Announcer
            writer.Write(m_announcerPoint.Serial);

            writer.Write(m_boundary);

            writer.Write(m_arenaCombatRegion.Area[0]);
            writer.Write(m_arenaCombatRegion.Map);
            writer.Write(m_arenaCombatRegion.Name);

            base.Serialize(writer);
        }
        public override void Deserialize(GenericReader reader)
        {
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_size = (EArenaSize)reader.ReadInt();
                // Team points
                int teamPointsCount = reader.ReadInt();
                for (int i = 0; i < teamPointsCount; ++i)
                {
                    Serial ser = reader.ReadInt();
                    m_team1Points.Add((TeamPoint)World.FindItem(ser));
                }
                for (int i = 0; i < teamPointsCount; ++i)
                {
                    Serial ser = reader.ReadInt();
                    m_team2Points.Add((TeamPoint)World.FindItem(ser));
                }
                // Ejection points
                for (int i = 0; i < teamPointsCount; ++i)
                {
                    Serial ser = reader.ReadInt();
                    m_team1EjectionPoints.Add((EjectionPoint)World.FindItem(ser));
                }
                for (int i = 0; i < teamPointsCount; ++i)
                {
                    Serial ser = reader.ReadInt();
                    m_team2EjectionPoints.Add((EjectionPoint)World.FindItem(ser));
                }
                // Announcer
                Serial announcerPointSerial = reader.ReadInt();
                m_announcerPoint = (AnnouncerPoint)World.FindItem(announcerPointSerial);

            }
            if (version >= 1)
            {
                m_boundary = reader.ReadRect2D();
            }
            if (version >= 2)
            {
                if (version < 4)
                {
                    // Region
                    Rectangle3D area = reader.ReadRect3D();
                    Map map = reader.ReadMap();
                    string name = reader.ReadString();
                }
            }
            if (version >= 3)
            {
                Rectangle3D area = reader.ReadRect3D();
                Map map = reader.ReadMap();
                string name = reader.ReadString();
                m_arenaCombatRegion = new ArenaCombatRegion(this, name, map, area);
                m_arenaCombatRegion.Register();
            }
            
            base.Deserialize(reader);
        }
        protected void CreateRegion(Point2D start, Point2D end)
        {
            Rectangle3D arenaCombatRect = new Rectangle3D(new Point3D(start.X, start.Y, -150),
                 new Point3D(end.X, end.Y, 150));
            m_arenaCombatRegion = new ArenaCombatRegion(this, "ArenaCombatRegion" + Serial.ToString(), Map, arenaCombatRect);
            m_arenaCombatRegion.Register();
        }
        private void CreateObstacles(EArenaCustomizationTypes types)
        {
            switch (types)
            {
                case EArenaCustomizationTypes.eACT_None:
                    {
                        break;
                    }
                case EArenaCustomizationTypes.eACT_VariableHeight:
                    {
                        break;
                    }
                case EArenaCustomizationTypes.eACT_FixedObstacles:
                    {
                        break;
                    }
                case EArenaCustomizationTypes.eACT_DynamicObstacles:
                    {
                        break;
                    }
            }
        }
        public void Setup(ArenaMatch match)
        {
            MatchInProgress = match;
            PositionPlayers(match.Team1.Players, match.Team2.Players);

            // Todo: Find compatable arena type.
            //CreateObstacles();
        }
        public void Shutdown()
        {
            EjectPlayersAndClean(MatchInProgress.Team1Players, MatchInProgress.Team2Players);
            MatchInProgress = null;
        }
        private void PositionPlayers(List<PlayerMobile> team1Players, List<PlayerMobile> team2Players)
        {
            int count = team1Players.Count;
            for (int i = 0; i < count; ++i)
            {
                TeamPoint point1 = m_team1Points[i];
                PlayerMobile pm = team1Players[i];
                pm.MoveToWorld(point1.Location, Map.Felucca);
                pm.Direction = pm.GetDirectionTo(m_team2Points[i]);
                
                TeamPoint point2 = m_team2Points[i];
                pm = team2Players[i];
                pm.MoveToWorld(point2.Location, Map.Felucca);
                pm.Direction = pm.GetDirectionTo(m_team1Points[i]);
            }
        }
        public void EjectPlayersAndClean(List<PlayerMobile> team1Players, List<PlayerMobile> team2Players)
        {
            List<Item> items = new List<Item>();
            IPooledEnumerable penum = Map.Felucca.GetItemsInBounds(m_boundary);
            foreach (Item item in penum)
            {
                //if (item is IArenaItem || item is Corpse)
                if (item is IArenaItem)
                    continue;
                items.Add(item);
            }
            penum.Free();

            int playerCount = team1Players.Count;
            for (int i = 0; i < playerCount; ++i)
            {
                PlayerMobile pm = team1Players[i];
                Point3D loc = m_team1EjectionPoints[i].Location;

                var removals = new List<Item>();
                // item dropped at player's corpse/feet on death?
                var location = pm.Location;
                if (pm.Corpse != null)
                    location = pm.Corpse.Location;

                foreach (var item in items.Where(it => it.Location == location))
                {
                    if (!(item is Corpse))
                    {
                        removals.Add(item);
                        item.MoveToWorld(loc);
                    }
                }

                pm.Location = loc;

                foreach (var item in removals) 
                    items.Remove(item);
            }

            for (int i = 0; i < playerCount; ++i)
            {
                Point3D loc = m_team2EjectionPoints[i].Location;
                PlayerMobile pm = team2Players[i];

                var removals = new List<Item>();
                // item dropped at player's corpse/feet on death?
                var location = pm.Location;
                if (pm.Corpse != null)
                    location = pm.Corpse.Location;

                foreach (var item in items.Where(it => it.Location == location))
                {
                    if (!(item is Corpse))
                    {
                        removals.Add(item);
                        item.MoveToWorld(loc);
                    }
                }

                pm.Location = loc;

                foreach (var item in removals)
                    items.Remove(item);
            }

            foreach (var item in items)
                item.Delete();

            Clean();
        }
        private void Clean()
        {
            List<Mobile> mobiles = new List<Mobile>();
			var penum = Map.Felucca.GetMobilesInBounds(m_boundary);
            foreach (Mobile mobile in penum)
            {
                if (mobile is PlayerMobile || mobile is ArenaAnnouncer)
                    continue;
                mobiles.Add(mobile);
            }
            penum.Free();

            foreach (Mobile mobile in mobiles)
            {
                mobile.Delete();
            }
        }
    }
}
