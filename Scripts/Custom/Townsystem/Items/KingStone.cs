using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
	public class KingStone : Item
	{
		private Town m_Town;

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public Town Town
		{
			get{ return m_Town; }
			set
			{
                m_Town = value;
			}
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Mobile King
        {
            get { return m_Town.King; }
            set { m_Town.King = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public Faction HomeFaction
        {
			get { return m_Town.HomeFaction; }
			set { m_Town.HomeFaction = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public CitizenshipBuffs PrimaryBuff
        {
            get { return m_Town.PrimaryCitizenshipBuff; }
            set { m_Town.PrimaryCitizenshipBuff = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public double SalesTax
        {
            get { return m_Town.SalesTax; }
            set { m_Town.SalesTax = Math.Max(0.0, value); }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public TreasuryWallTypes TreasuryWallType
        {
            get { return m_Town.TreasuryWallType; }
            set { m_Town.SetTreasuryWalls(value); }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public int AvailableBombPlans
        {
            get { return m_Town.AvailableBombPlans; }
            set { m_Town.AvailableBombPlans = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public bool ExilesDisabled {
            get { return m_Town.ExilesDisabled; }
            set { m_Town.ExilesDisabled = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public AllianceInfo TownAlliance
		{
			get { return HomeFaction.Alliance; }
			set { value.AddFaction(HomeFaction, false); }
		}


		public override string DefaultName { get { return "King's stone"; } }

		[Constructable]
		public KingStone() : this( null )
		{
		}

		[Constructable]
		public KingStone( Town Town ) : base( 0xED4 )
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
            Name = String.Format("{0} King's Stone", m_Town.Definition.FriendlyName);
            Town.KingStone = this;
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Town == null )
				return;

			if ( !from.InRange( GetWorldLocation(), 2 ) )
			{
				from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
			}
			else if ( from.HasGump(typeof(KingsGump) ))
			{
				from.SendMessage("You already have a King's menu open.");
			}
			else if ( from is PlayerMobile )
			{
				Town existingTown = Town.CheckCitizenship( from );
                bool isKing = existingTown == null ? false : existingTown.IsKing(from);

                if (from.AccessLevel >= AccessLevel.GameMaster )
                {
                    from.SendGump(new KingsGump(m_Town));
                }
                else if ( existingTown == null )
				{
                    from.SendGump(new CitizenshipGump(m_Town));
				}
                else if (existingTown != m_Town)
                {
                    from.SendMessage("This is not your Town!");
                }
                else if (!isKing)
                {
                    if (!from.HasGump(typeof(AllianceGump)))
                        from.SendGump(new AllianceGump(from, m_Town));
                }
                else
                {
                    from.SendGump(new KingsGump(m_Town));
                }
			}
		}

		public KingStone( Serial serial ) : base( serial )
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