using Server.Guilds;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    public class FakeGuildstoneDeed : Item
    {
        public override int LabelNumber{ get{ return 1041055; } } // a guild deed

        [Constructable]
        public FakeGuildstoneDeed()
            : base(0x14F0)
        {
            Weight = 1;
        }

        public override void OnDoubleClick(Mobile from)
        {
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}
			else
			{
				BaseHouse house = BaseHouse.FindHouseAt( from );

				if ( house == null )
				{
					from.SendLocalizedMessage( 501138 ); // You can only place a guildstone in a house.
				}
				else if ( house.FindFakeGuildstone() != null )
				{
					from.SendLocalizedMessage( 501142 );//Only one guildstone may reside in a given house.
				}
				else if ( !house.IsOwner( from ) )
				{
					from.SendLocalizedMessage( 501141 ); // You can only place a guildstone in a house you own!
				}
                else if (from.Guild == null) 
                {
                    from.SendMessage("You must be in a guild to place this item!");
                }
				else
				{
					FakeGuildstone guildstone = new FakeGuildstone(from.Guild as Guild);
                    guildstone.MoveToWorld(from.Location, from.Map);
                    house.Addons.Add(guildstone);
                    Delete();
				}
			}
        }

        public FakeGuildstoneDeed(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    public class FakeGuildstone : Item, IAddon, IChopable
    {
        private Guild m_Guild;

        public override int LabelNumber { get { return 1041429; } } // a guildstone

        public FakeGuildstone(Guild guild)
            : base(0xED6)
        {
            Movable = false;
            m_Guild = guild;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Guild != null && !m_Guild.Disbanded)
            {
                string name;
                string abbr;

                if ((name = m_Guild.Name) == null || (name = name.Trim()).Length <= 0)
                    name = "(unnamed)";

                if ((abbr = m_Guild.Abbreviation) == null || (abbr = abbr.Trim()).Length <= 0)
                    abbr = "";

                list.Add(1060802, String.Format("{0} [{1}]", Utility.FixHtml(name), Utility.FixHtml(abbr)));
            }
            else if (m_Guild.Name != null && m_Guild.Abbreviation != null)
            {
                list.Add(1060802, String.Format("{0} [{1}]", Utility.FixHtml(m_Guild.Name), Utility.FixHtml(m_Guild.Abbreviation)));
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Guild != null && !m_Guild.Disbanded)
            {
                string name;

                if ((name = m_Guild.Name) == null || (name = name.Trim()).Length <= 0)
                    name = "(unnamed)";

                this.LabelTo(from, name);
            }
            else if (m_Guild.Name != null)
            {
                this.LabelTo(from, m_Guild.Name);
            }
        }

        public Item Deed
        {
            get { return new FakeGuildstoneDeed(); }
        }

        public bool CouldFit(IPoint3D p, Map map)
        {
            return map.CanFit(p.X, p.Y, p.Z, this.ItemData.Height);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Guild.IsMember(from))
            {
                from.CloseGump(typeof(GuildInfoGump));
                from.SendGump(new GuildInfoGump(from as PlayerMobile, m_Guild));
            }
            else
                from.SendMessage("You are not a member of this guild.");
        }

        public void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house != null && house.IsOwner(from) && house.Addons.Contains(this))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); // You destroy the item.

                Delete();

                if (house != null && house.Addons.Contains(this))
                    house.Addons.Remove(this);

                Item deed = Deed;

                if (deed != null)
                {
                    from.AddToBackpack(deed);
                }
            }
        }

        public FakeGuildstone(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
            writer.Write(m_Guild);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_Guild = reader.ReadGuild() as Guild;
        }
    }
}
