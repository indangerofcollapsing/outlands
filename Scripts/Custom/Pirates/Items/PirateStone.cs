using System;
using System.Collections.Generic;
using Server.Guilds;
using Server.Items;
using Server.Multis;

namespace Server.Custom.Pirates
{
    public class PirateStone : Item, IChopable
    {
        private Guild m_Guild;

        public static Dictionary<Guild, PirateStone> m_PirateStoneDictionary = new Dictionary<Guild, PirateStone>();

        public override string DefaultName
        {
            get { return "Pirate Stone"; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Guild Guild
        {
            get { return m_Guild; }
        }

        [Constructable]
        public PirateStone(Guild guild)
            : base(0xED4)
        {

            Movable = false;
            Hue = 0x455;


            if (guild != null)
            {
                m_Guild = guild;
                if (!m_PirateStoneDictionary.ContainsKey(guild))
                {
                    m_PirateStoneDictionary.Add(guild, this);
                }
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Guild != null && !m_Guild.Disbanded)
            {
                string name;

                if ((name = m_Guild.Name) == null || (name = name.Trim()).Length <= 0)
                {
                    name = "(unnamed)";
                }

                this.LabelTo(from, name);
            }
            else if (m_Guild == null || m_Guild.Disbanded)
            {
                string name;
                name = "(This Stone Does not belong to a guild.)";
                this.LabelTo(from, name);
            }

            else if (m_Guild.Name != null)
            {
                this.LabelTo(from, m_Guild.Name);
            }
        }

        public override void OnAfterDelete()
        {
            if (m_Guild != null)
            {
                if (m_PirateStoneDictionary.ContainsKey(m_Guild))
                {
                    m_PirateStoneDictionary.Remove(m_Guild);
                }
            }
        }

        #region IChopable Members

        public void OnChop(Mobile from)
        {
            BaseHouse house = BaseHouse.FindHouseAt(this);

            if (house == null || house != null && (house.IsCoOwner(from)))
            {
                Effects.PlaySound(GetWorldLocation(), Map, 0x3B3);
                from.SendLocalizedMessage(500461); //You destroy the item.

                Delete();

                from.AddToBackpack(new PirateStoneDeed());
            }
        }

        #endregion

        public PirateStone(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(m_Guild);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Guild = (Guild)reader.ReadGuild();
                        break;
                    }
            }

            if (m_Guild != null)
            {
                if (!m_PirateStoneDictionary.ContainsKey(m_Guild))
                {
                    m_PirateStoneDictionary.Add(m_Guild, this);
                }
            }
        }
    }
}