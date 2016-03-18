using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
	public class MilitiaStone : Item
	{
		private Town m_Town;

		[CommandProperty( AccessLevel.Counselor, AccessLevel.Administrator )]
		public Town Town
		{
			get{ return m_Town; }
			set
			{
                m_Town = value;
			}
		}

		public override string DefaultName { get { return "Town Militia Stone"; } }

		[Constructable]
		public MilitiaStone() : this( null )
		{
		}

		[Constructable]
		public MilitiaStone( Town Town ) : base( 0xED4 )
		{
			Movable = false;
			m_Town = Town;
		}

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;
            Town = Town.FromRegion(Region.Find(Location, Map));
            if (Town == null)
            {
                Delete();
                return;
            }
            Name = String.Format("{0} Militia Stone", m_Town.Definition.FriendlyName);
            Town.MilitiaStone = this;
        }

		public override void OnDoubleClick( Mobile from )
		{
            Town town = Town.CheckCitizenship(from);

            if (town == null)
            {
                from.SendGump(new CitizenshipGump(m_Town));
                return;
            }
            else if (town != m_Town)
            {
                //if (!from.HasGump(typeof(AllianceGump)))
                //    from.SendGump(new AllianceGump(from, m_Town));
                return;
            }
			else if (m_Town.HomeFaction == null)
            {
                from.SendMessage("The town currently has no joinable Militia until the King chooses his allegiance.");
                return;
            }
            else if (Faction.Find(from) != null)
            {
                if (!from.HasGump(typeof(AllianceGump)))
                    from.SendGump(new AllianceGump(from, m_Town));

                return;
            }

            if (from.HasGump(typeof(ConfirmMilitiaGump)))
                from.CloseGump(typeof(ConfirmMilitiaGump));

            from.SendGump(new ConfirmMilitiaGump(from, m_Town));
            //m_Town.Decreed.OnJoinAccepted(from);
		}

		public MilitiaStone( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			Town.WriteReference( writer, m_Town );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					Town = Town.ReadReference( reader );
					break;
				}
			}
		}
	}
}