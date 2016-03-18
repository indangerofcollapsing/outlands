using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Custom.Pirates;
using Server.Guilds;

namespace Server.Multis
{
    public abstract class BaseGuildDock : BaseMulti
    {
        private List<Item> m_Items;
        private List<Mobile> m_Mobiles;
        public Guild m_Guild;

        public static Dictionary<Guild, BaseGuildDock> m_GuildDockDictionary = new Dictionary<Guild, BaseGuildDock>();

        public virtual int EventRange { get { return 10; } }

        public BaseGuildDock(int multiID, Guild guild): base(multiID)
        {
            m_Items = new List<Item>();
            m_Mobiles = new List<Mobile>();

            if (guild != null)
            {
                m_Guild = guild;

                if (!m_GuildDockDictionary.ContainsKey(guild))
                    m_GuildDockDictionary.Add(guild, this);

                GuildDockPersistance.CheckCreateGuildDockGuildInfo(guild);
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(CheckAddComponents));
        }

        public void CheckAddComponents()
        {
            if (Deleted)
                return;

            AddComponents();
        }

        public virtual void AddComponents()
        {

        }

        public virtual void AddItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            m_Items.Add(item);

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
        }

        public virtual void AddMobile(Mobile m, int wanderRange, int xOffset, int yOffset, int zOffset)
        {
            m_Mobiles.Add(m);

            Point3D loc = new Point3D(X + xOffset, Y + yOffset, Z + zOffset);
            BaseCreature bc = m as BaseCreature;

            if (bc != null)
            {
                bc.RangeHome = wanderRange;
                bc.Home = loc;
            }

            if (m is BaseVendor || m is Banker)
                m.Direction = Direction.South;

            m.MoveToWorld(loc, this.Map);
        }

        public virtual void OnEnter(Mobile m)
        {
        }

        public virtual void OnExit(Mobile m)
        {           
        }

        public override bool HandlesOnMovement { get { return false; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            bool inOldRange = Utility.InRange(oldLocation, Location, EventRange);
            bool inNewRange = Utility.InRange(m.Location, Location, EventRange);

            if (inNewRange && !inOldRange)
                OnEnter(m);

            else if (inOldRange && !inNewRange)
                OnExit(m);
        }

        public static bool FindGuildDockAt(Point3D point, Map map)
        {
            bool foundGuildDock = false;

            List<BaseMulti> m_Multis = BaseMulti.GetMultisAt(point, map);

            foreach (BaseMulti multi in m_Multis)
            {
                if (multi is BaseGuildDock)
                {
                    foundGuildDock = true;
                    break;
                }
            }

            return foundGuildDock;
        }

        public static BaseGuildDock GetGuildDockAt(Point3D point, Map map)
        {
            BaseGuildDock guildDock = null; 

            List<BaseMulti> m_Multis = BaseMulti.GetMultisAt(point, map);

            foreach (BaseMulti multi in m_Multis)
            {
                if (multi is BaseGuildDock)                
                    return multi as BaseGuildDock;
            }

            return guildDock;
        }

        public override void OnAfterDelete()
        {
            if (m_Guild != null)
            {
                if (m_GuildDockDictionary.ContainsKey(m_Guild))
                    m_GuildDockDictionary.Remove(m_Guild);
            }

            if (m_Items != null)
            {
                for (int i = 0; i < m_Items.Count; ++i)
                {
                    if (m_Items[i] != null && !m_Items[i].Deleted)
                        m_Items[i].Delete();
                }

                m_Items.Clear();
            }

            if (m_Mobiles != null)
            {
                for (int i = 0; i < m_Mobiles.Count; ++i)
                {
                    if (m_Mobiles[i] != null && !m_Mobiles[i].Deleted)
                        m_Mobiles[i].Delete();
                }

                m_Mobiles.Clear();
            }

            base.OnAfterDelete();
        }

        public BaseGuildDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0);

            writer.Write(m_Items, true);
            writer.Write(m_Mobiles, true);
            writer.Write(m_Guild);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Items = reader.ReadStrongItemList();
                m_Mobiles = reader.ReadStrongMobileList();
                m_Guild = (Guild)reader.ReadGuild();
            }

            //------

            if (m_Guild != null)
            {
                if (!m_GuildDockDictionary.ContainsKey(m_Guild))
                    m_GuildDockDictionary.Add(m_Guild, this);

                GuildDockPersistance.CheckCreateGuildDockGuildInfo(m_Guild);
            }           
        }
    }

    #region Guild Dock Directions

    public class SouthGuildDock : BaseGuildDock
    {
        [Constructable]
        public SouthGuildDock(Guild guild): base(0x004E, guild)
        {
        }

        public override void AddComponents()
        {
            AddMobile(new Dockmaster(this), 5, 0, 4, 5);
        }

        public SouthGuildDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class EastGuildDock : BaseGuildDock
    {
        [Constructable]
        public EastGuildDock(Guild guild): base(0x004F, guild)
        {
        }

        public override void AddComponents()
        {
            AddMobile(new Dockmaster(this), 5, 4, 0, 5);
        }

        public EastGuildDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class NorthGuildDock : BaseGuildDock
    {
        [Constructable]
        public NorthGuildDock(Guild guild): base(0x004A, guild)
        {
        }

        public override void AddComponents()
        {
            AddMobile(new Dockmaster(this), 5, 0, -4, 5);
        }

        public NorthGuildDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WestGuildDock : BaseGuildDock
    {
        [Constructable]
        public WestGuildDock(Guild guild): base(0x004B, guild)
        {
        }

        public override void AddComponents()
        {
            AddMobile(new Dockmaster(this), 5, -4, 0, 5);
        }

        public WestGuildDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Verseion 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    #endregion
    
    #region Buccaneer's Den Docks

    public class SouthBuccsDock : BaseGuildDock
    {
        [Constructable]
        public SouthBuccsDock(): base(0x004E, null)
        {
        }

        public override void AddComponents()
        {
        }

        public SouthBuccsDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //-----

            DoubloonDocks.RegisterDock(DoubloonDock.South, this);
        }
    }

    public class NorthBuccsDock : BaseGuildDock
    {
        [Constructable]
        public NorthBuccsDock(): base(0x004A, null)
        {
        }

        public override void AddComponents()
        {
        }

        public NorthBuccsDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //-----

            DoubloonDocks.RegisterDock(DoubloonDock.North, this);
        }
    }

    public class WestBuccsDock : BaseGuildDock
    {
        [Constructable]
        public WestBuccsDock(): base(0x004B, null)
        {
        }

        public override void AddComponents()
        {
        }

        public WestBuccsDock(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            //Version 0
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //-----

            DoubloonDocks.RegisterDock(DoubloonDock.West, this);
        }
    }

    #endregion
}