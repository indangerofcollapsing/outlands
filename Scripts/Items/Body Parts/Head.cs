using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public enum HeadType
	{
		Regular,
		Duel,
		Tournament
	}

    public enum PlayerType
    {
        None,
        Paladin,
        Murderer        
    }

	public class Head : Item
	{
        private HeadType m_HeadType;
		[CommandProperty( AccessLevel.GameMaster )]
		public HeadType HeadType
		{
			get { return m_HeadType; }
			set { m_HeadType = value; }
		}

        public Mobile m_Owner;
        [CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; }
		}

        private string m_PlayerName;
        [CommandProperty(AccessLevel.GameMaster)]
        public string PlayerName
        {
            get { return m_PlayerName; }
            set { m_PlayerName = value; }
        }

        private PlayerType m_PlayerType = PlayerType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerType PlayerType
        {
            get { return m_PlayerType; }
            set { m_PlayerType = value; }
        }

        public Mobile m_Killer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Killer
        {
            get { return m_Killer; }
            set { m_Killer = value; }
        }

        private string m_KillerName;
        [CommandProperty(AccessLevel.GameMaster)]
        public string KillerName
        {
            get { return m_KillerName; }
            set { m_KillerName = value; }
        }

        private PlayerType m_KillerType;
        [CommandProperty(AccessLevel.GameMaster)]
        public PlayerType KillerType
        {
            get { return m_KillerType; }
            set { m_KillerType = value; }
        }

		[Constructable]
		public Head( HeadType headType, Mobile owner): base( 0x1DA0 )
		{
            m_HeadType = headType;
            m_Owner = owner;
            
            PlayerMobile pm_Owner = owner as PlayerMobile;

            if (pm_Owner != null)
            {
                PlayerName = pm_Owner.Name;

                switch (m_HeadType)
                {
                    case Server.Items.HeadType.Regular: Name = "the head of " + pm_Owner.Name; break;
                    case Server.Items.HeadType.Duel: Name = "the head of " + pm_Owner.Name + ", taken in a duel"; break;
                    case Server.Items.HeadType.Tournament: Name = "the head of " + pm_Owner.Name + ", taken in a tournament"; break;
                }
            }

            else
                Name = "a head";
            
            Weight = 1.0;
		}

		public Head( Serial serial ): base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);           
			writer.Write((int)1);
            
            //Version 0
            writer.WriteEncodedInt((int)m_HeadType);
			writer.Write((Mobile)m_Owner);
            writer.Write((string)m_PlayerName); 
          
            //Version 1
            writer.Write((int)m_PlayerType);

            writer.Write((Mobile)m_Killer);
            writer.Write((string)m_KillerName);

            writer.Write((int)m_KillerType);
        }
            
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            m_HeadType = (HeadType)reader.ReadEncodedInt();
            m_Owner = (PlayerMobile)reader.ReadMobile();
            m_PlayerName = reader.ReadString();

            //Version 1
            if (version >= 1)
            {
                m_PlayerType = (PlayerType)reader.ReadInt();

                m_Killer = (PlayerMobile)reader.ReadMobile();
                m_KillerName = reader.ReadString();

                m_KillerType = (PlayerType)reader.ReadInt();
            }
		}
	}
}