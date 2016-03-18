using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom.Townsystem.AI
{
	public enum ReactionType
	{
		Ignore,
		Warn,
		Attack
	}

	public enum MovementType
	{
		Stand,
		Patrol,
		Follow
	}

	public class Reaction
	{
		private Town m_Town;
		private ReactionType m_Type;

		public Town Town{ get{ return m_Town; } }
		public ReactionType Type{ get{ return m_Type; } set{ m_Type = value; } }

		public Reaction( Town town, ReactionType type )
		{
			m_Town = town;
			m_Type = type;
		}

		public Reaction( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();

			switch ( version )
			{
                case 1:
                    m_Town = Town.ReadReference(reader);
                    goto case 0;
				case 0:
				{
                    if (version < 1)
					    Faction.ReadReference( reader );
					m_Type = (ReactionType) reader.ReadEncodedInt();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 1 ); // version

			Town.WriteReference( writer, m_Town );
			writer.WriteEncodedInt( (int) m_Type );
		}
	}

	public class Orders
	{
		private BaseFactionGuard m_Guard;

		private List<Reaction> m_Reactions;
		private MovementType m_Movement;
		private Mobile m_Follow;

		public BaseFactionGuard Guard{ get{ return m_Guard; } }

		public MovementType Movement
		{ 
			get{ return m_Movement; } 
			set { m_Movement = value; } 
		}

		public Mobile Follow{ get{ return m_Follow; } set{ m_Follow = value; } }

		public Reaction GetReaction( Town town )
		{
			Reaction reaction;

			for ( int i = 0; i < m_Reactions.Count; ++i )
			{
				reaction = m_Reactions[i];

				if ( reaction.Town == town )
					return reaction;
			}

			reaction = new Reaction( town, ( town == null || town == m_Guard.Town ) ? ReactionType.Ignore : ReactionType.Attack );
			m_Reactions.Add( reaction );

			return reaction;
		}

		public void SetReaction( Town town, ReactionType type )
		{
			Reaction reaction = GetReaction( town );

			reaction.Type = type;
		}

		public Orders( BaseFactionGuard guard )
		{
			m_Guard = guard;
			m_Reactions = new List<Reaction>();
			Movement = MovementType.Patrol;
		}

		public Orders( BaseFactionGuard guard, GenericReader reader )
		{
			m_Guard = guard;

			int version = reader.ReadEncodedInt();

			switch ( version )
			{
				case 1:
				{
					m_Follow = reader.ReadMobile();
					goto case 0;
				}

				case 0:
				{
					int count = reader.ReadEncodedInt();
					m_Reactions = new List<Reaction>( count );

					for ( int i = 0; i < count; ++i )
						m_Reactions.Add( new Reaction( reader ) );

					Movement = (MovementType)reader.ReadEncodedInt();

					break;
				}
			}
		}

		public void Serialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 1 ); // version

			writer.Write( (Mobile) m_Follow );

			writer.WriteEncodedInt( (int) m_Reactions.Count );

			for ( int i = 0; i < m_Reactions.Count; ++i )
				m_Reactions[i].Serialize( writer );

			writer.WriteEncodedInt( (int) Movement );
		}
	}
}