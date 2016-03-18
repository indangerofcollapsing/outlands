using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
	public class ElectionStone : Item
	{
		private Town m_Town;
        private Item m_switch;  

		[CommandProperty( AccessLevel.Counselor, AccessLevel.Administrator )]
		public Town Town
		{
			get{ return m_Town; }
			set
			{
				m_Town = value;

				//AssignName( m_Town == null ? null : m_Town.Definition.TownStoneName );
			}
		}

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public TimeSpan NextStateTime
        {
            get { return m_Town.Election.NextStateTime; }
            set { m_Town.Election.NextStateTime = value; }
        }

		public override string DefaultName { get { return "Election stone"; } }

		[Constructable]
		public ElectionStone() : this( null )
		{
		}

		[Constructable]
		public ElectionStone( Town Town ) : base( 0xEDC )
		{
			Movable = false;
			m_Town = Town;
		}

        public void CreateSwitch()
        {
            if (m_switch != null)
                return;
            m_switch = new Item(4242);
            m_switch.Name = this.Name;
            m_switch.MoveToWorld(new Point3D(Location.X, Location.Y + 1, Location.Z + 10),Map);
            m_switch.Movable = false;
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
            Name = String.Format("{0} Election Stone", m_Town.Definition.FriendlyName);
            Town.Election.ElectionStone = this;
            CreateSwitch();
        }

		public override void OnDoubleClick( Mobile from )
		{
            if (m_Town == null)
                return;

			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
			else if ( from.HasGump(typeof(ElectionGump) ))
			{
				from.SendLocalizedMessage( 1042160 ); // You already have a Town menu open.
			}
			else if ( from is PlayerMobile )
			{
				Town existingTown = Town.CheckCitizenship( from );
                if (from.AccessLevel >= AccessLevel.GameMaster )
                {
                    from.SendGump(new ElectionManagementGump(Town.Election));
                }

                if ( existingTown == m_Town)
				{
				    from.SendGump( new ElectionGump( (PlayerMobile) from, m_Town.Election ) );
				}
				else if ( existingTown != null )
				{
					// TODO: Validate
					from.SendMessage( "This is not your Town!" ); // This is not your Town stone!
				}
				else
				{
					from.SendGump( new CitizenshipGump( m_Town ) );
				}
			}
		}

		public ElectionStone( Serial serial ) : base( serial )
		{
		}

        public override void OnAfterDelete()
        {
            m_switch.Delete();
            base.OnAfterDelete();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			Town.WriteReference( writer, m_Town );
            writer.WriteItem(m_switch);
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
                    m_switch = reader.ReadItem();
					break;
				}
			}
		}
	}
}