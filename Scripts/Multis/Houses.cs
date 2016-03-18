using System;
using System.Collections;
using System.Linq;
using Server;
using System.Linq.Expressions;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
    public class SmallOldHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7), new Rectangle2D(-1, 4, 3, 1) };

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(2, 4, 0); } }

        public override int DefaultPrice { get { return 72500; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[0]; } }

        public SmallOldHouse(Mobile owner, int id)
            : base(id, owner, 538, 3)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoor(0, 3, 7, keyValue);

            SetSign(2, 4, 5);
        }

        public SmallOldHouse(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed()
        {
            switch (ItemID)
            {
                case 0x64: return new StonePlasterHouseDeed();
                case 0x66: return new FieldStoneHouseDeed();
                case 0x68: return new SmallBrickHouseDeed();
                case 0x6A: return new WoodHouseDeed();
                case 0x6C: return new WoodPlasterHouseDeed();
                case 0x6E:
                default: return new ThatchedRoofCottageDeed();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 3;
                MaxLockDowns = 538;
            }
        }
    }

    public class GuildHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 14, 14), new Rectangle2D(-2, 7, 4, 1) };

        public override int DefaultPrice { get { return 495000; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.ThreeStoryFoundations[20]; } }
        public override int ConvertOffsetX { get { return -1; } }
        public override int ConvertOffsetY { get { return -1; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(4, 8, 0); } }

        public GuildHouse(Mobile owner)
            : base(0x74, owner, 1725, 11)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(-1, 6, 7, keyValue);

            SetSign(4, 8, 16);

            AddSouthDoor(-3, -1, 7);
            AddSouthDoor(3, -1, 7);
        }

        public GuildHouse(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new BrickHouseDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 11;
                MaxLockDowns = 1725;
            }

        }
    }

    public class TwoStoryHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, 0, 14, 7), new Rectangle2D(-7, -7, 9, 7), new Rectangle2D(-4, 7, 4, 1) };

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(2, 8, 0); } }

        public override int DefaultPrice { get { return 650000; } }

        public TwoStoryHouse(Mobile owner, int id)
            : base(id, owner, 2100, 13)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(-3, 6, 7, keyValue);

            SetSign(2, 8, 16);

            AddSouthDoor(-3, 0, 7);
            AddSouthDoor(id == 0x76 ? -2 : -3, 0, 27);
        }

        public TwoStoryHouse(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed()
        {
            switch (ItemID)
            {
                case 0x76: return new TwoStoryWoodPlasterHouseDeed();
                case 0x78:
                default: return new TwoStoryStonePlasterHouseDeed();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 13;
                MaxLockDowns = 2100;
            }

        }
    }

    public class Tower : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 16, 14), new Rectangle2D(-1, 7, 4, 2), new Rectangle2D(-11, 0, 4, 7), new Rectangle2D(9, 0, 4, 7) };

        public override int DefaultPrice { get { return 1250000; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.ThreeStoryFoundations[37]; } }
        public override int ConvertOffsetY { get { return -1; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(5, 8, 0); } }

        public Tower(Mobile owner)
            : base(0x7A, owner, 2975, 16)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(false, 0, 6, 6, keyValue);

            SetSign(5, 8, 16);

            AddSouthDoor(false, 3, -2, 6);
            AddEastDoor(false, 1, 4, 26);
            AddEastDoor(false, 1, 4, 46);
        }

        public Tower(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new TowerDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 16;
                MaxLockDowns = 2975;
            }
        }
    }

    public class Keep : BaseHouse//warning: ODD shape!
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-11, -11, 7, 8), new Rectangle2D(-11, 5, 7, 8), new Rectangle2D(6, -11, 7, 8), new Rectangle2D(6, 5, 7, 8), new Rectangle2D(-9, -3, 5, 8), new Rectangle2D(6, -3, 5, 8), new Rectangle2D(-4, -9, 10, 20), new Rectangle2D(-1, 11, 4, 1) };

        public override int DefaultPrice { get { return 2550000; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(5, 13, 0); } }

        public Keep(Mobile owner)
            : base(0x7C, owner, 3975, 20)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(false, 0, 10, 6, keyValue);

            SetSign(5, 12, 16);
        }

        public Keep(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new KeepDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 20;
                MaxLockDowns = 3975;
            }
        }
    }

    public class Castle : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-15, -15, 31, 31), new Rectangle2D(-1, 16, 4, 1) };

        public override int DefaultPrice { get { return 7420000; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(5, 17, 0); } }

        public Castle(Mobile owner)
            : base(0x7E, owner, 4975, 28)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(false, 0, 15, 6, keyValue);

            SetSign(5, 17, 16);

            AddSouthDoors(false, 0, 11, 6, keyValue);
            AddSouthDoors(false, 0, 5, 6, keyValue);
            AddSouthDoors(false, -1, -11, 6, keyValue);
        }

        public Castle(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new CastleDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            if (version < 2)
            {
                //Find the existing keyvalue
                var doors = this.Doors.Cast<BaseDoor>().ToList();
                var keyValue = doors.Select(x => x.KeyValue).First(x => x > 0);
                foreach (BaseDoor door in this.Doors)
                {
                    door.KeyValue = keyValue;
                    door.Locked = true;
                }
            }
            if (version < 1)
            {
                MaxSecures = 28;
                MaxLockDowns = 4975;
            }
        }
    }

    public class LargePatioHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 15, 14), new Rectangle2D(-5, 7, 4, 1) };

        public override int DefaultPrice { get { return 425000; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.ThreeStoryFoundations[29]; } }
        public override int ConvertOffsetY { get { return -1; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(1, 8, 0); } }

        public LargePatioHouse(Mobile owner)
            : base(0x8C, owner, 1475, 10)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(-4, 6, 7, keyValue);

            SetSign(1, 8, 16);

            AddEastDoor(1, 4, 7);
            AddEastDoor(1, -4, 7);
            AddSouthDoor(4, -1, 7);
        }

        public LargePatioHouse(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new LargePatioDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 10;
                MaxLockDowns = 1475;
            }
        }
    }

    public class LargeMarbleHouse : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-7, -7, 15, 14), new Rectangle2D(-6, 7, 6, 1) };

        public override int DefaultPrice { get { return 780000; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.ThreeStoryFoundations[29]; } }
        public override int ConvertOffsetY { get { return -1; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(1, 8, 0); } }

        public LargeMarbleHouse(Mobile owner)
            : base(0x96, owner, 2450, 14)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(false, -4, 3, 4, keyValue);

            SetSign(1, 8, 11);
        }

        public LargeMarbleHouse(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new LargeMarbleDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 14;
                MaxLockDowns = 2450;
            }
        }
    }

    public class SmallTower : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -3, 8, 7), new Rectangle2D(2, 4, 3, 1) };

        public override int DefaultPrice { get { return 175500; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[6]; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(1, 4, 0); } }

        public SmallTower(Mobile owner)
            : base(0x98, owner, 884, 5)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoor(false, 3, 3, 6, keyValue);

            SetSign(1, 4, 5);
        }

        public SmallTower(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new SmallTowerDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 5;
                MaxLockDowns = 884;
            }
        }
    }

    public class LogCabin : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-3, -6, 8, 13) };

        public override int DefaultPrice { get { return 295500; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[12]; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(5, 8, 0); } }

        public LogCabin(Mobile owner)
            : base(0x9A, owner, 1178, 8)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoor(1, 4, 8, keyValue);

            SetSign(5, 8, 20);

            AddSouthDoor(1, 0, 29);
        }

        public LogCabin(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new LogCabinDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SandStonePatio : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-5, -4, 12, 8), new Rectangle2D(-2, 4, 3, 1) };

        public override int DefaultPrice { get { return 195900; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[35]; } }
        public override int ConvertOffsetY { get { return -1; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(4, 6, 0); } }

        public SandStonePatio(Mobile owner)
            : base(0x9C, owner, 903, 6)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoor(-1, 3, 6, keyValue);

            SetSign(4, 6, 24);
        }

        public SandStonePatio(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new SandstonePatioDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version < 1)
            {
                MaxSecures = 6;
                MaxLockDowns = 903;
            }
        }
    }

    public class TwoStoryVilla : BaseHouse
    {
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-5, -5, 11, 11), new Rectangle2D(2, 6, 4, 1) };

        public override int DefaultPrice { get { return 276500; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[31]; } }

        public override Rectangle2D[] Area { get { return AreaArray; } }
        public override Point3D BaseBanLocation { get { return new Point3D(3, 8, 0); } }

        public TwoStoryVilla(Mobile owner)
            : base(0x9E, owner, 1100, 8)
        {
            uint keyValue = CreateKeys(owner);

            AddSouthDoors(3, 1, 5, keyValue);

            SetSign(3, 8, 24);

            AddEastDoor(1, 0, 25);
            AddSouthDoor(-3, -1, 25);
        }

        public TwoStoryVilla(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed() { return new VillaDeed(); }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class SmallShop : BaseHouse
    {
        public override Rectangle2D[] Area { get { return (ItemID == 0x40A2 ? AreaArray1 : AreaArray2); } }
        public override Point3D BaseBanLocation { get { return new Point3D(3, 4, 0); } }

        public override int DefaultPrice { get { return 150000; } }

        public override HousePlacementEntry ConvertEntry { get { return HousePlacementEntry.TwoStoryFoundations[0]; } }

        public static Rectangle2D[] AreaArray1 = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7), new Rectangle2D(-1, 4, 4, 1) };
        public static Rectangle2D[] AreaArray2 = new Rectangle2D[] { new Rectangle2D(-3, -3, 7, 7), new Rectangle2D(-2, 4, 3, 1) };

        public SmallShop(Mobile owner, int id)
            : base(id, owner, 846, 4)
        {
            uint keyValue = CreateKeys(owner);

            BaseDoor door = MakeDoor(false, DoorFacing.EastCW);

            door.Locked = true;
            door.KeyValue = keyValue;

            if (door is BaseHouseDoor)
                ((BaseHouseDoor)door).Facing = DoorFacing.EastCCW;

            AddDoor(door, -2, 0, id == 0xA2 ? 24 : 27);

            //AddSouthDoor( false, -2, 0, 27 - (id == 0xA2 ? 3 : 0), keyValue );

            SetSign(3, 4, 7 - (id == 0xA2 ? 2 : 0));
        }

        public SmallShop(Serial serial)
            : base(serial)
        {
        }

        public override HouseDeed GetDeed()
        {
            switch (ItemID)
            {
                case 0xA0: return new StoneWorkshopDeed();
                case 0xA2:
                default: return new MarbleWorkshopDeed();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}