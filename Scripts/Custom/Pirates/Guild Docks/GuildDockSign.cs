using System;
using System.Collections;
using System.Collections.Generic;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class GuildDockSign : Item
    {
        private Guild m_Guild;
        [CommandProperty(AccessLevel.GameMaster)]
        public Guild Guild
        {
            get { return m_Guild; }
            set { m_Guild = value; }
        }

        private BaseGuildDock m_GuildDock;
        [CommandProperty(AccessLevel.GameMaster)]
        public BaseGuildDock GuildDock
        {
            get { return m_GuildDock; }
            set { m_GuildDock = value; }
        }

        public List<Item> m_Items = new List<Item>();

        [Constructable]
        public GuildDockSign(): base(7977)
        {
            Name = "guild docks";
            Movable = false;

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        [Constructable]
        public GuildDockSign(Guild guild, BaseGuildDock guildDock): base(7977)
        {
            Name = "guild docks";           
            Movable = false;

            m_Guild = guild;
            m_GuildDock = guildDock;

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        public void AddComponents()
        {
            if (Deleted)
                return;

            GroupItem(new Static(4758), 0, 0, 0);

            Z += 14;
        }

        public virtual void GroupItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            m_Items.Add(item);
            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
        }

        public GuildDockSign(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (Guild != null)            
                LabelTo(from, "guild docks for " + Guild.Name + " [" + Guild.Abbreviation + "]");

            else
                LabelTo(from, "an abandoned guild docks");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            //TEST: TEMPORARY HANDLING
            if (player.AccessLevel > AccessLevel.Player)
            {
                if (m_Guild == null)
                {                
                    if (player.Guild != null)
                        m_Guild = (Guild)player.Guild;                    
                }   
 
                if (m_GuildDock == null)
                {
                    BaseGuildDock guildDock = BaseGuildDock.GetGuildDockAt(player.Location, player.Map);

                    if (guildDock != null)
                        m_GuildDock = guildDock;
                }
            }

            bool guildMatch = false;

            if (player.Guild != null)
            {
                if (player.Guild == m_Guild)
                    guildMatch = true;
            }

            if (guildMatch)
            {
                player.CloseGump(typeof(GuildDockGump));
                player.SendGump(new GuildDockGump(player, 1));
            }

            else            
                player.SendMessage("You are not a member of that guild.");
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Guild);
            writer.Write(m_GuildDock);

            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Guild = (Guild)reader.ReadGuild();
                m_GuildDock = (BaseGuildDock)reader.ReadItem();

                int itemsCount = reader.ReadInt();
                for (int i = 0; i < itemsCount; ++i)
                {
                    m_Items.Add(reader.ReadItem());
                } 
            }
        }
    }    
}