using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Regions;
using Server.Targets;

namespace Server.Items
{
    [FlipableAttribute(0x13E3, 0x13E4)]
    public class ArcaneHammer : Item, IUsesRemaining
    {
        //Move this to feature list in the future
        public static readonly int DefaultUsesRemaining = 25;

        protected int m_UsesRemaining;
        protected bool m_ShowUsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowUsesRemaining
        {
            get { return m_ShowUsesRemaining; }
            set { m_ShowUsesRemaining = value; InvalidateProperties(); }
        }

        public override string DefaultName
        {
            get
            {
                return "an arcane hammer";
            }
        }

        [Constructable]
        public ArcaneHammer()
            : base(0x13E3)
        {
            Weight = 8.0;
            Layer = Layer.OneHanded;
            UsesRemaining = DefaultUsesRemaining;
            ShowUsesRemaining = true;
            Hue = 2410;
        }

        public ArcaneHammer(int itemId) : base(itemId)
        {
        }

        public ArcaneHammer(Serial serial)
            : base(serial)
        {
        }


        public bool CheckLocation(Mobile from)
        {
            if (!(IsChildOf(from.Backpack) || Parent == from))
            {
                from.SendMessage("The arcane hammer must be in your backpack to use it.");
                return false;
            }

            bool anvil, forge;
            CheckAnvilAndForge(from, 2, out anvil, out forge);

            if (anvil && forge)
                return true;
            else
            {
                from.SendMessage("You must be near an anvil and a forge to reforge this item.");
                return false;
            }
        }


        private static readonly Type typeofAnvil = typeof(AnvilAttribute);
        private static readonly Type typeofForge = typeof(ForgeAttribute);

        public static void CheckAnvilAndForge(Mobile from, int range, out bool anvil, out bool forge)
        {
            anvil = false;
            forge = false;

            Map map = from.Map;

            if (map == null)
                return;

            IPooledEnumerable eable = map.GetItemsInRange(from.Location, range);

            foreach (Item item in eable)
            {
                Type type = item.GetType();

                bool isAnvil = (type.IsDefined(typeofAnvil, false) || item.ItemID == 4015 || item.ItemID == 4016 || item.ItemID == 0x2DD5 || item.ItemID == 0x2DD6);
                bool isForge = (type.IsDefined(typeofForge, false) || item.ItemID == 4017 || (item.ItemID >= 6522 && item.ItemID <= 6569) || item.ItemID == 0x2DD8);

                if (isAnvil || isForge)
                {
                    if ((from.Z + 16) < item.Z || (item.Z + 16) < from.Z || !from.InLOS(item))
                        continue;

                    anvil = anvil || isAnvil;
                    forge = forge || isForge;

                    if (anvil && forge)
                        break;
                }
            }

            eable.Free();

            for (int x = -range; (!anvil || !forge) && x <= range; ++x)
            {
                for (int y = -range; (!anvil || !forge) && y <= range; ++y)
                {
                    StaticTile[] tiles = map.Tiles.GetStaticTiles(from.X + x, from.Y + y, true);

                    for (int i = 0; (!anvil || !forge) && i < tiles.Length; ++i)
                    {
                        int id = tiles[i].ID;

                        bool isAnvil = (id == 4015 || id == 4016 || id == 0x2DD5 || id == 0x2DD6);
                        bool isForge = (id == 4017 || (id >= 6522 && id <= 6569) || id == 0x2DD8);

                        if (isAnvil || isForge)
                        {
                            if ((from.Z + 16) < tiles[i].Z || (tiles[i].Z + 16) < from.Z || !from.InLOS(new Point3D(from.X + x, from.Y + y, tiles[i].Z + (tiles[i].Height / 2) + 1)))
                                continue;

                            anvil = anvil || isAnvil;
                            forge = forge || isForge;
                        }
                    }
                }
            }
        }

        public bool VerifyRegion(Mobile m)
        {
            if (!m.Region.IsPartOf(typeof(TownRegion)))
                return false;

            return Server.Factions.Faction.IsNearType(m, typeof(Blacksmith), 6);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((bool)m_ShowUsesRemaining);

            writer.Write((int)m_UsesRemaining);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Add the serialization versioning just in case we expand this after it goes live.
            switch (version)
            {
                case 0:
                    {
                        m_ShowUsesRemaining = reader.ReadBool();
                        m_UsesRemaining = reader.ReadInt();

                        if (m_UsesRemaining < 1)
                            m_UsesRemaining = 1;
                        break;
                    }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Durability : {0}", this.UsesRemaining);
            LabelTo(from, "{0}", this.DefaultName);
        }

        public override void OnDoubleClick(Mobile from)
        {
            //Make sure the hammer is use at blacksmithy
            if (!this.CheckLocation(from))
                return;

            from.SendLocalizedMessage(1010018); // What do you want to use this item on?

            from.Target = new ItemReforgingTarget(this);
        }
    }
}
