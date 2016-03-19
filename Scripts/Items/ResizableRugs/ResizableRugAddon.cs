using Server.Multis;

namespace Server.Items
{
    public enum CarpetType
    {
        Plain1,
        Plain2,
        Red1,
        Red2,
        Red3,
        Blue1,
        Blue2,
        Blue3,
        Fancy1,
        Fancy2,
        Fancy3,
        Fancy4,
        Fancy5
    }

    public class ResizableRugAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new ResizableRugDeed(m_whichCarpet); } }
        private CarpetType m_whichCarpet;

        private enum RugPiece
        {
            Top,
            Bottom,
            Right,
            Left,
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft,
            Center
        }

        private int RugPieceId(CarpetType whichCarpet, RugPiece whichPiece)
        {
            switch (whichCarpet)
            {
                case CarpetType.Plain1:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAAA;
                        case RugPiece.Bottom: return 0xAAC;
                        case RugPiece.Left: return 0xAAD;
                        case RugPiece.Right: return 0xAAB;
                        case RugPiece.TopLeft: return 0xAAE;
                        case RugPiece.TopRight: return 0xAAF;
                        case RugPiece.BottomLeft: return 0xAB1;
                        case RugPiece.BottomRight: return 0xAB0;
                        default: return 0xAA9;
                    }
                case CarpetType.Plain2:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAB4;
                        case RugPiece.Bottom: return 0xAB6;
                        case RugPiece.Left: return 0xAB7;
                        case RugPiece.Right: return 0xAB5;
                        case RugPiece.TopLeft: return 0xAB8;
                        case RugPiece.TopRight: return 0xAB9;
                        case RugPiece.BottomLeft: return 0xABB;
                        case RugPiece.BottomRight: return 0xABA;
                        default: return 0xAB3;
                    }
                case CarpetType.Red1:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xACE;
                        case RugPiece.Bottom: return 0xAD0;
                        case RugPiece.Left: return 0xACD;
                        case RugPiece.Right: return 0xACF;
                        case RugPiece.TopLeft: return 0xACA;
                        case RugPiece.TopRight: return 0xACC;
                        case RugPiece.BottomLeft: return 0xAcB;
                        case RugPiece.BottomRight: return 0xAC9;
                        default: return 0xAC8;
                    }
                case CarpetType.Red2:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xACE;
                        case RugPiece.Bottom: return 0xAD0;
                        case RugPiece.Left: return 0xACD;
                        case RugPiece.Right: return 0xACF;
                        case RugPiece.TopLeft: return 0xACA;
                        case RugPiece.TopRight: return 0xACC;
                        case RugPiece.BottomLeft: return 0xAcB;
                        case RugPiece.BottomRight: return 0xAC9;
                        default: return 0xAC6;
                    }
                case CarpetType.Red3:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xACE;
                        case RugPiece.Bottom: return 0xAD0;
                        case RugPiece.Left: return 0xACD;
                        case RugPiece.Right: return 0xACF;
                        case RugPiece.TopLeft: return 0xACA;
                        case RugPiece.TopRight: return 0xACC;
                        case RugPiece.BottomLeft: return 0xAcB;
                        case RugPiece.BottomRight: return 0xAC9;
                        default: return 0xAC7;
                    }
                case CarpetType.Blue1:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAF7;
                        case RugPiece.Bottom: return 0xAF9;
                        case RugPiece.Left: return 0xAF6;
                        case RugPiece.Right: return 0xAF8;
                        case RugPiece.TopLeft: return 0xAC3;
                        case RugPiece.TopRight: return 0xAC5;
                        case RugPiece.BottomLeft: return 0xAC4;
                        case RugPiece.BottomRight: return 0xAC2;
                        default: return 0xABE;
                    }
                case CarpetType.Blue2:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAF7;
                        case RugPiece.Bottom: return 0xAF9;
                        case RugPiece.Left: return 0xAF6;
                        case RugPiece.Right: return 0xAF8;
                        case RugPiece.TopLeft: return 0xAC3;
                        case RugPiece.TopRight: return 0xAC5;
                        case RugPiece.BottomLeft: return 0xAC4;
                        case RugPiece.BottomRight: return 0xAC2;
                        default: return 0xABD;
                    }
                case CarpetType.Blue3:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAF7;
                        case RugPiece.Bottom: return 0xAF9;
                        case RugPiece.Left: return 0xAF6;
                        case RugPiece.Right: return 0xAF8;
                        case RugPiece.TopLeft: return 0xAC3;
                        case RugPiece.TopRight: return 0xAC5;
                        case RugPiece.BottomLeft: return 0xAC4;
                        case RugPiece.BottomRight: return 0xAC2;
                        default: return 0xABF;
                    }
                case CarpetType.Fancy1:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAD7;
                        case RugPiece.Bottom: return 0xAD9;
                        case RugPiece.Left: return 0xAD6;
                        case RugPiece.Right: return 0xAD8;
                        case RugPiece.TopLeft: return 0xAD3;
                        case RugPiece.TopRight: return 0xAD5;
                        case RugPiece.BottomLeft: return 0xAD4;
                        case RugPiece.BottomRight: return 0xAD2;
                        default: return 0xAD1;
                    }
                case CarpetType.Fancy2:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAE0;
                        case RugPiece.Bottom: return 0xAE2;
                        case RugPiece.Left: return 0xADF;
                        case RugPiece.Right: return 0xAE1;
                        case RugPiece.TopLeft: return 0xADC;
                        case RugPiece.TopRight: return 0xADE;
                        case RugPiece.BottomLeft: return 0xADD;
                        case RugPiece.BottomRight: return 0xADB;
                        default: return 0xADA;
                    }
                case CarpetType.Fancy3:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAE8;
                        case RugPiece.Bottom: return 0xAEA;
                        case RugPiece.Left: return 0xAE7;
                        case RugPiece.Right: return 0xAE9;
                        case RugPiece.TopLeft: return 0xAE4;
                        case RugPiece.TopRight: return 0xAE6;
                        case RugPiece.BottomLeft: return 0xAE5;
                        case RugPiece.BottomRight: return 0xAE3;
                        default: return 0xAEB;
                    }
                case CarpetType.Fancy4:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAF3;
                        case RugPiece.Bottom: return 0xAF5;
                        case RugPiece.Left: return 0xAF2;
                        case RugPiece.Right: return 0xAF4;
                        case RugPiece.TopLeft: return 0xAEF;
                        case RugPiece.TopRight: return 0xAF1;
                        case RugPiece.BottomLeft: return 0xAF0;
                        case RugPiece.BottomRight: return 0xAEE;
                        default: return 0xAEC;
                    }
                case CarpetType.Fancy5:
                    switch (whichPiece)
                    {
                        case RugPiece.Top: return 0xAF3;
                        case RugPiece.Bottom: return 0xAF5;
                        case RugPiece.Left: return 0xAF2;
                        case RugPiece.Right: return 0xAF4;
                        case RugPiece.TopLeft: return 0xAEF;
                        case RugPiece.TopRight: return 0xAF1;
                        case RugPiece.BottomLeft: return 0xAF0;
                        case RugPiece.BottomRight: return 0xAEE;
                        default: return 0xAED;
                    }
                default: return 1;
            }
        }

        [Constructable]
        public ResizableRugAddon(Rectangle2D rect, CarpetType whichCarpet)
        {
            m_whichCarpet = whichCarpet;

            for (int x = 0; x < rect.Width; x++)
                for (int y = 0; y < rect.Height; y++)
                {
                    if (y == 0 && x != 0 && x != rect.Width - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.Top)), x, y, 0);
                    if (y == rect.Height - 1 && x != 0 && x != rect.Width - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.Bottom)), x, y, 0);
                    if (x == 0 && y != 0 && y != rect.Height - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.Left)), x, y, 0);
                    if (x == rect.Width - 1 && y != 0 && y != rect.Height - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.Right)), x, y, 0);
                    if (y == 0 && x == 0)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.TopLeft)), x, y, 0);
                    if (y == 0 && x == rect.Width - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.TopRight)), x, y, 0);
                    if (y == rect.Height - 1 && x == 0)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.BottomLeft)), x, y, 0);
                    if (y == rect.Height - 1 && x == rect.Width - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.BottomRight)), x, y, 0);
                    if (y != 0 && x != 0 && x != rect.Width - 1 && y != rect.Height - 1)
                        AddComponent(new AddonComponent(RugPieceId(whichCarpet, RugPiece.Center)), x, y, 0);
                }
        }

        public ResizableRugAddon(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)m_whichCarpet);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_whichCarpet = (CarpetType)reader.ReadInt();
        }
    }

    public class ResizableRugDeed : BaseAddonDeed
    {
        public override BaseAddon Addon { get { return null; } }
        public CarpetType m_whichCarpet;

        [Constructable]
        public ResizableRugDeed(CarpetType whichCarpet)
        {
            Name = GetDisplayName(whichCarpet);
            m_whichCarpet = whichCarpet;
        }

        public ResizableRugDeed(Serial serial) : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
                BoundingBoxPicker.Begin(from, new BoundingBoxCallback(BuildRugBox_Callback), this);
            else
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
        }

        private static string GetDisplayName(CarpetType whichCarpet)
        {
            switch (whichCarpet)
            {
                case CarpetType.Plain1:
                case CarpetType.Plain2:
                case CarpetType.Red1:
                case CarpetType.Red2:
                case CarpetType.Red3:
                case CarpetType.Blue1:
                case CarpetType.Blue2:
                case CarpetType.Blue3:
                case CarpetType.Fancy1:
                case CarpetType.Fancy2:
                case CarpetType.Fancy3:
                case CarpetType.Fancy4:
                case CarpetType.Fancy5:
                default:
                    return string.Format("Rug ({0})", whichCarpet.ToString());

            }
        }

        private static void BuildRugBox_Callback(Mobile from, Map map, Point3D start, Point3D end, object state)
        {
            ResizableRugDeed m_Deed = state as ResizableRugDeed;

            if (m_Deed == null || m_Deed.Deleted)
                return;

            if (m_Deed.IsChildOf(from.Backpack))
            {
                Rectangle2D rect = new Rectangle2D(start.X, start.Y, end.X - start.X + 1, end.Y - start.Y + 1);

                if (rect.Width < 3 || rect.Height < 3)
                {
                    from.SendMessage("The carpet is too small. It should be longer or wider than that.");
                    return;
                }

                BaseAddon addon = new ResizableRugAddon(rect, m_Deed.m_whichCarpet);

                BaseHouse house = null;

                AddonFitResult res = addon.CouldFit(start, map, from, ref house);

                if (res == AddonFitResult.Valid)
                    addon.MoveToWorld(start, map);
                else if (res == AddonFitResult.Blocked)
                    from.SendLocalizedMessage(500269); // You cannot build that there.
                else if (res == AddonFitResult.NotInHouse)
                    from.SendLocalizedMessage(500274); // You can only place this in a house that you own!
                else if (res == AddonFitResult.DoorsNotClosed)
                    from.SendMessage("You must close all house doors before placing this.");
                else if (res == AddonFitResult.DoorTooClose)
                    from.SendLocalizedMessage(500271); // You cannot build near the door.

                if (res == AddonFitResult.Valid)
                {
                    m_Deed.Delete();
                    if (house != null)
                        house.Addons.Add(addon);
                }
                else
                {
                    addon.Delete();
                }
            }
            else
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)m_whichCarpet);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            m_whichCarpet = (CarpetType)reader.ReadInt();
        }
    }
}