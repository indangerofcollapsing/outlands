using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System.Collections;

namespace Server.Items
{
	public class Teleporter : Item
	{
		private bool m_Active, m_Creatures, m_CombatCheck;
		private Point3D m_PointDest;
		private Map m_MapDest;
		private bool m_SourceEffect;
		private bool m_DestEffect;
		private int m_SoundID;
		private TimeSpan m_Delay;
        private bool m_OverrideT2AAccessCheck;
        private bool m_CriminalCheck;

		[CommandProperty( AccessLevel.GameMaster )]
		public bool SourceEffect
		{
			get{ return m_SourceEffect; }
			set{ m_SourceEffect = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool DestEffect
		{
			get{ return m_DestEffect; }
			set{ m_DestEffect = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SoundID
		{
			get{ return m_SoundID; }
			set{ m_SoundID = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public TimeSpan Delay
		{
			get{ return m_Delay; }
			set{ m_Delay = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set { m_Active = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D PointDest
		{
			get { return m_PointDest; }
			set { m_PointDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest
		{
			get { return m_MapDest; }
			set { m_MapDest = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Creatures
		{
			get { return m_Creatures; }
			set { m_Creatures = value; InvalidateProperties(); }
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public bool CombatCheck
		{
			get { return m_CombatCheck; }
			set { m_CombatCheck = value; InvalidateProperties(); }
		}

        [CommandProperty(AccessLevel.GameMaster)]
        public bool OverrideT2ACheck
        {
            get { return m_OverrideT2AAccessCheck; }
            set { m_OverrideT2AAccessCheck = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool CriminalCheck
        {
            get { return m_CriminalCheck; }
            set { m_CriminalCheck = value; }
        }

		public override int LabelNumber{ get{ return 1026095; } } // teleporter

		[Constructable]
		public Teleporter()
			: this(new Point3D(0, 0, 0), null, false)
		{
		}

		[Constructable]
		public Teleporter(Point3D pointDest, Map mapDest)
			: this(pointDest, mapDest, false)
		{
		}

		[Constructable]
		public Teleporter(Point3D pointDest, Map mapDest, bool creatures)
			: base(0x1BC3)
		{
			Movable = false;
			Visible = false;

			m_Active = true;
			m_PointDest = pointDest;
			m_MapDest = mapDest;
			m_Creatures = creatures;

			m_CombatCheck = false;
            m_CriminalCheck = false;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Active )
				list.Add( 1060742 ); // active
			else
				list.Add( 1060743 ); // inactive

			if ( m_MapDest != null )
				list.Add( 1060658, "Map\t{0}", m_MapDest );

			if ( m_PointDest != Point3D.Zero )
				list.Add( 1060659, "Coords\t{0}", m_PointDest );

			list.Add( 1060660, "Creatures\t{0}", m_Creatures ? "Yes" : "No" );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );

			if ( m_Active )
			{
				if ( m_MapDest != null && m_PointDest != Point3D.Zero )
					LabelTo( from, "{0} [{1}]", m_PointDest, m_MapDest );
				else if ( m_MapDest != null )
					LabelTo( from, "[{0}]", m_MapDest );
				else if ( m_PointDest != Point3D.Zero )
					LabelTo( from, m_PointDest.ToString() );
			}
			else
			{
				LabelTo( from, "(inactive)" );
			}
		}

		public virtual bool CanTeleport(Mobile m)
		{
			if (!m_Creatures && !m.Player)
			{
				return false;
			}
			else if (m_CombatCheck && SpellHelper.CheckCombat(m, true))
			{
				m.SendLocalizedMessage(1005564, "", 0x22); // Wouldst thou flee during the heat of battle??
				return false;
			}
            else if (m_CriminalCheck && m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
                
                return false;
            }

			return true;
		}

		public virtual void StartTeleport( Mobile m )
		{
			if ( m_Delay == TimeSpan.Zero )
				DoTeleport( m );
			else
				Timer.DelayCall( m_Delay, new TimerStateCallback( DoTeleport_Callback ), m );
		}

		private void DoTeleport_Callback( object state )
		{
			DoTeleport( (Mobile) state );
		}

		public virtual void DoTeleport( Mobile m )
		{
            if (!m_OverrideT2AAccessCheck && (T2AAccess.IsT2A(m_PointDest, m_MapDest) && !T2AAccess.HasAccess(m)))
            {
                m.SendMessage("T2A access has been disabled at this time.");
                return;
            }

			Map map = m_MapDest;

			if ( map == null || map == Map.Internal )
				map = m.Map;

			Point3D p = m_PointDest;

			if ( p == Point3D.Zero )
				p = m.Location;

			Server.Mobiles.BaseCreature.TeleportPets( m, p, map );

			bool sendEffect = ( !m.Hidden || m.AccessLevel == AccessLevel.Player );

			if ( m_SourceEffect && sendEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			m.MoveToWorld( p, map );

			if ( m_DestEffect && sendEffect )
				Effects.SendLocationEffect( m.Location, m.Map, 0x3728, 10, 10 );

			if ( m_SoundID > 0 && sendEffect )
				Effects.PlaySound( m.Location, m.Map, m_SoundID );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if (m_Active && CanTeleport(m))
			{
				StartTeleport( m );
				return false;
			}

			return true;
		}

		public Teleporter(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 5 ); // version
			
            //version 5
            writer.Write(m_CriminalCheck);

			//version 4
			
			writer.Write((bool)m_OverrideT2AAccessCheck);
			
			//version 3

			writer.Write( (bool) m_CombatCheck );

			writer.Write( (bool) m_SourceEffect );
			writer.Write( (bool) m_DestEffect );
			writer.Write( (TimeSpan) m_Delay );
			writer.WriteEncodedInt( (int) m_SoundID );

			writer.Write( m_Creatures );

			writer.Write( m_Active );
			writer.Write( m_PointDest );
			writer.Write( m_MapDest );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
                case 5:
                    {
                        m_CriminalCheck = reader.ReadBool();
                        goto case 4;
                    }
				case 4:
				{
					m_OverrideT2AAccessCheck = reader.ReadBool();
					goto case 3;
				}
				case 3:
				{
					m_CombatCheck = reader.ReadBool();
					goto case 2;
				}
				case 2:
				{
					m_SourceEffect = reader.ReadBool();
					m_DestEffect = reader.ReadBool();
					m_Delay = reader.ReadTimeSpan();
					m_SoundID = reader.ReadEncodedInt();

					goto case 1;
				}
				case 1:
				{
					m_Creatures = reader.ReadBool();

					goto case 0;
				}
				case 0:
				{
					m_Active = reader.ReadBool();
					m_PointDest = reader.ReadPoint3D();
					m_MapDest = reader.ReadMap();

					break;
				}
			}
            if (version < 5)
                m_CriminalCheck = false;
		}
	}

	public class SkillTeleporter : Teleporter
	{
		private SkillName m_Skill;
		private double m_Required;
		private string m_MessageString;
		private int m_MessageNumber;

		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get{ return m_Skill; }
			set{ m_Skill = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double Required
		{
			get{ return m_Required; }
			set{ m_Required = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string MessageString
		{
			get{ return m_MessageString; }
			set{ m_MessageString = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MessageNumber
		{
			get{ return m_MessageNumber; }
			set{ m_MessageNumber = value; InvalidateProperties(); }
		}

		private void EndMessageLock( object state )
		{
			((Mobile)state).EndAction( this );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( Active )
			{
				if ( !Creatures && !m.Player )
					return true;

				Skill sk = m.Skills[m_Skill];

				if ( sk == null || sk.Base < m_Required )
				{
					if ( m.BeginAction( this ) )
					{
						if ( m_MessageString != null )
							m.Send( new UnicodeMessage( Serial, ItemID, MessageType.Regular, 0x3B2, 3, "ENU", null, m_MessageString ) );
						else if ( m_MessageNumber != 0 )
							m.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, m_MessageNumber, null, "" ) );

						Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
					}

					return false;
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int skillIndex = (int)m_Skill;
			string skillName;

			if ( skillIndex >= 0 && skillIndex < SkillInfo.Table.Length )
				skillName = SkillInfo.Table[skillIndex].Name;
			else
				skillName = "(Invalid)";

			list.Add( 1060661, "{0}\t{1:F1}", skillName, m_Required );

			if ( m_MessageString != null )
				list.Add( 1060662, "Message\t{0}", m_MessageString );
			else if ( m_MessageNumber != 0 )
				list.Add( 1060662, "Message\t#{0}", m_MessageNumber );
		}

		[Constructable]
		public SkillTeleporter()
		{
		}

		public SkillTeleporter(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (int) m_Skill );
			writer.Write( (double) m_Required );
			writer.Write( (string) m_MessageString );
			writer.Write( (int) m_MessageNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Skill = (SkillName)reader.ReadInt();
					m_Required = reader.ReadDouble();
					m_MessageString = reader.ReadString();
					m_MessageNumber = reader.ReadInt();

					break;
				}
			}
		}
	}

	/// <summary>
	/// This teleporter allows you to set certain skill thresholds that will prevent players from
	/// being teleported should any of their skills exceed the maximum skill values set.
	/// </summary>
	public class MultiSkillTeleporter : Teleporter
	{
		// This list stores the skills that have had their values set. We'll iterate through this
		// later on rather than *all* 52 skills.
		private List<SkillName> m_SkillsActive = new List<SkillName>();
		
		private double m_AlchemyMaximum;
		private double m_AnatomyMaximum;
		private double m_AnimalLoreMaximum;
		private double m_ItemIDMaximum;
		private double m_ArmsLoreMaximum;
		private double m_ParryMaximum;
		private double m_BeggingMaximum;
		private double m_BlacksmithMaximum;
		private double m_FletchingMaximum;
		private double m_PeacemakingMaximum;
		private double m_CampingMaximum;
		private double m_CarpentryMaximum;
		private double m_CartographyMaximum;
		private double m_CookingMaximum;
		private double m_DetectHiddenMaximum;
		private double m_DiscordanceMaximum;
		private double m_EvalIntMaximum;
		private double m_HealingMaximum;
		private double m_FishingMaximum;
		private double m_ForensicsMaximum;
		private double m_HerdingMaximum;
		private double m_HidingMaximum;
		private double m_ProvocationMaximum;
		private double m_InscribeMaximum;
		private double m_LockpickingMaximum;
		private double m_MageryMaximum;
		private double m_MagicResistMaximum;
		private double m_TacticsMaximum;
		private double m_SnoopingMaximum;
		private double m_MusicianshipMaximum;
		private double m_PoisoningMaximum;
		private double m_ArcheryMaximum;
		private double m_SpiritSpeakMaximum;
		private double m_StealingMaximum;
		private double m_TailoringMaximum;
		private double m_AnimalTamingMaximum;
		private double m_TasteIDMaximum;
		private double m_TinkeringMaximum;
		private double m_TrackingMaximum;
		private double m_VeterinaryMaximum;
		private double m_SwordsMaximum;
		private double m_MacingMaximum;
		private double m_FencingMaximum;
		private double m_WrestlingMaximum;
		private double m_LumberjackingMaximum;
		private double m_MiningMaximum;
		private double m_MeditationMaximum;
		private double m_StealthMaximum;
		private double m_RemoveTrapMaximum;
		private double m_NecromancyMaximum;
		private double m_FocusMaximum;
		private double m_ChivalryMaximum;
		
		private string m_MessageString;
		private int m_MessageNumber;

		#region Properties

		[CommandProperty( AccessLevel.GameMaster )]
		public double AlchemyMaximum
		{
			get{ return m_AlchemyMaximum; }
			set{ m_AlchemyMaximum = value; m_SkillsActive.Add( SkillName.Alchemy ); InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public double AnatomyMaximum
		{
			get{ return m_AnatomyMaximum; }
			set{ m_AnatomyMaximum = value; m_SkillsActive.Add( SkillName.Anatomy ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double AnimalLoreMaximum
		{
			get{ return m_AnimalLoreMaximum; }
			set{ m_AnimalLoreMaximum = value; m_SkillsActive.Add( SkillName.AnimalLore ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ItemIDMaximum
		{
			get{ return m_ItemIDMaximum; }
			set{ m_ItemIDMaximum = value; m_SkillsActive.Add( SkillName.ItemID ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ArmsLoreMaximum
		{
			get{ return m_ArmsLoreMaximum; }
			set{ m_ArmsLoreMaximum = value; m_SkillsActive.Add( SkillName.ArmsLore ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ParryMaximum
		{
			get{ return m_ParryMaximum; }
			set{ m_ParryMaximum = value; m_SkillsActive.Add( SkillName.Parry ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double BeggingMaximum
		{
			get{ return m_BeggingMaximum; }
			set{ m_BeggingMaximum = value; m_SkillsActive.Add( SkillName.Begging ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double BlacksmithMaximum
		{
			get{ return m_BlacksmithMaximum; }
			set{ m_BlacksmithMaximum = value; m_SkillsActive.Add( SkillName.Blacksmith ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double FletchingMaximum
		{
			get{ return m_FletchingMaximum; }
			set{ m_FletchingMaximum = value; m_SkillsActive.Add( SkillName.Fletching ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double PeacemakingMaximum
		{
			get{ return m_PeacemakingMaximum; }
			set{ m_PeacemakingMaximum = value; m_SkillsActive.Add( SkillName.Peacemaking ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double CampingMaximum
		{
			get{ return m_CampingMaximum; }
			set{ m_CampingMaximum = value; m_SkillsActive.Add( SkillName.Camping ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double CarpentryMaximum
		{
			get{ return m_CarpentryMaximum; }
			set{ m_CarpentryMaximum = value; m_SkillsActive.Add( SkillName.Carpentry ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double CartographyMaximum
		{
			get{ return m_CartographyMaximum; }
			set{ m_CartographyMaximum = value; m_SkillsActive.Add( SkillName.Cartography ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double CookingMaximum
		{
			get{ return m_CookingMaximum; }
			set{ m_CookingMaximum = value; m_SkillsActive.Add( SkillName.Cooking ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double DetectHiddenMaximum
		{
			get{ return m_DetectHiddenMaximum; }
			set{ m_DetectHiddenMaximum = value; m_SkillsActive.Add( SkillName.DetectHidden ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double DiscordanceMaximum
		{
			get{ return m_DiscordanceMaximum; }
			set{ m_DiscordanceMaximum = value; m_SkillsActive.Add( SkillName.Discordance ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double EvalIntMaximum
		{
			get{ return m_EvalIntMaximum; }
			set{ m_EvalIntMaximum = value; m_SkillsActive.Add( SkillName.EvalInt ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double HealingMaximum
		{
			get{ return  m_HealingMaximum; }
			set{ m_HealingMaximum = value; m_SkillsActive.Add( SkillName.Healing ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double FishingMaximum
		{
			get{ return m_FishingMaximum; }
			set{ m_FishingMaximum = value; m_SkillsActive.Add( SkillName.Fishing ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ForensicsMaximum
		{
			get{ return m_ForensicsMaximum; }
			set{ m_ForensicsMaximum = value; m_SkillsActive.Add( SkillName.Forensics ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double HerdingMaximum
		{
			get{ return m_HerdingMaximum; }
			set{ m_HerdingMaximum = value; m_SkillsActive.Add( SkillName.Herding ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double HidingMaximum
		{
			get{ return m_HidingMaximum; }
			set{ m_HidingMaximum = value; m_SkillsActive.Add( SkillName.Hiding ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ProvocationMaximum
		{
			get{ return m_ProvocationMaximum; }
			set{ m_ProvocationMaximum = value; m_SkillsActive.Add( SkillName.Provocation ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double InscribeMaximum
		{
			get{ return m_InscribeMaximum; }
			set{ m_InscribeMaximum = value; m_SkillsActive.Add( SkillName.Inscribe ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double LockpickingMaximum
		{
			get{ return m_LockpickingMaximum; }
			set{ m_LockpickingMaximum = value; m_SkillsActive.Add( SkillName.Lockpicking ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double MageryMaximum
		{
			get{ return m_MageryMaximum; }
			set{ m_MageryMaximum = value; m_SkillsActive.Add( SkillName.Magery ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double MagicResistMaximum
		{
			get{ return m_MagicResistMaximum; }
			set{ m_MagicResistMaximum = value; m_SkillsActive.Add( SkillName.MagicResist ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double TacticsMaximum
		{
			get{ return m_TacticsMaximum; }
			set{ m_TacticsMaximum = value; m_SkillsActive.Add( SkillName.Tactics ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double SnoopingMaximum
		{
			get{ return m_SnoopingMaximum; }
			set{ m_SnoopingMaximum = value; m_SkillsActive.Add( SkillName.Snooping ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double MusicianshipMaximum
		{
			get{ return m_MusicianshipMaximum; }
			set{ m_MusicianshipMaximum = value; m_SkillsActive.Add( SkillName.Musicianship ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double PoisoningMaximum
		{
			get{ return m_PoisoningMaximum; }
			set{ m_PoisoningMaximum = value; m_SkillsActive.Add( SkillName.Poisoning ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ArcheryMaximum
		{
			get{ return m_ArcheryMaximum; }
			set{ m_ArcheryMaximum = value; m_SkillsActive.Add( SkillName.Archery ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double SpiritSpeakMaximum
		{
			get{ return m_SpiritSpeakMaximum; }
			set{ m_SpiritSpeakMaximum = value; m_SkillsActive.Add( SkillName.SpiritSpeak ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double StealingMaximum
		{
			get{ return m_StealingMaximum; }
			set{ m_StealingMaximum = value; m_SkillsActive.Add( SkillName.Stealing ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double TailoringMaximum
		{
			get{ return m_TailoringMaximum; }
			set{ m_TailoringMaximum = value; m_SkillsActive.Add( SkillName.Tailoring ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double AnimalTamingMaximum
		{
			get{ return m_AnimalTamingMaximum; }
			set{ m_AnimalTamingMaximum = value; m_SkillsActive.Add( SkillName.AnimalTaming ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double TasteIDMaximum
		{
			get{ return m_TasteIDMaximum; }
			set{ m_TasteIDMaximum = value; m_SkillsActive.Add( SkillName.TasteID ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double TinkeringMaximum
		{
			get{ return m_TinkeringMaximum; }
			set{ m_TinkeringMaximum = value; m_SkillsActive.Add( SkillName.Tinkering ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double TrackingMaximum
		{
			get{ return m_TrackingMaximum; }
			set{ m_TrackingMaximum = value; m_SkillsActive.Add( SkillName.Tracking ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double VeterinaryMaximum
		{
			get{ return m_VeterinaryMaximum; }
			set{ m_VeterinaryMaximum = value; m_SkillsActive.Add( SkillName.Veterinary ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double SwordsMaximum
		{
			get{ return m_SwordsMaximum; }
			set{ m_SwordsMaximum = value; m_SkillsActive.Add( SkillName.Swords ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double MacingMaximum
		{
			get{ return m_MacingMaximum; }
			set{ m_MacingMaximum = value; m_SkillsActive.Add( SkillName.Macing ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double FencingMaximum
		{
			get{ return m_FencingMaximum; }
			set{ m_FencingMaximum = value; m_SkillsActive.Add( SkillName.Fencing ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double WrestlingMaximum
		{
			get{ return m_WrestlingMaximum; }
			set{ m_WrestlingMaximum = value; m_SkillsActive.Add( SkillName.Wrestling ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double LumberjackingMaximum
		{
			get{ return m_LumberjackingMaximum; }
			set{ m_LumberjackingMaximum = value; m_SkillsActive.Add( SkillName.Lumberjacking ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double MiningMaximum
		{
			get{ return m_MiningMaximum; }
			set{ m_MiningMaximum = value; m_SkillsActive.Add( SkillName.Mining ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double MeditationMaximum
		{
			get{ return m_MeditationMaximum; }
			set{ m_MeditationMaximum = value; m_SkillsActive.Add( SkillName.Meditation ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double StealthMaximum
		{
			get{ return m_StealthMaximum; }
			set{ m_StealthMaximum = value; m_SkillsActive.Add( SkillName.Stealth ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double RemoveTrapMaximum
		{
			get{ return m_RemoveTrapMaximum; }
			set{ m_RemoveTrapMaximum = value; m_SkillsActive.Add( SkillName.RemoveTrap ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double NecromancyMaximum
		{
			get{ return m_NecromancyMaximum; }
			set{ m_NecromancyMaximum = value; m_SkillsActive.Add( SkillName.Necromancy ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double FocusMaximum
		{
			get{ return m_FocusMaximum; }
			set{ m_FocusMaximum = value; m_SkillsActive.Add( SkillName.Focus ); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public double ChivalryMaximum
		{
			get{ return m_ChivalryMaximum; }
			set{ m_ChivalryMaximum = value; m_SkillsActive.Add( SkillName.Chivalry ); InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public string MessageString
		{
			get{ return m_MessageString; }
			set{ m_MessageString = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MessageNumber
		{
			get{ return m_MessageNumber; }
			set{ m_MessageNumber = value; InvalidateProperties(); }
		}
		
		#endregion

		private void EndMessageLock( object state )
		{
			((Mobile)state).EndAction( this );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( Active )
			{
				if ( !Creatures && !m.Player )
					return true;

				foreach ( SkillName skill in m_SkillsActive )
				{
					Skill sk = m.Skills[skill];
					if ( sk == null || !IsValidSkillLevel( skill, sk.Base ) )
					{
						if ( m.BeginAction( this ) )
						{
							if ( m_MessageString != null )
								m.Send( new UnicodeMessage( Serial, ItemID, MessageType.Regular, 0x3B2, 3, "ENU", null, m_MessageString ) );
							else if ( m_MessageNumber != 0 )
								m.Send( new MessageLocalized( Serial, ItemID, MessageType.Regular, 0x3B2, 3, m_MessageNumber, null, "" ) );

							Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerStateCallback( EndMessageLock ), m );
						}

						return false;
					}
				}

				StartTeleport( m );
				return false;
			}

			return true;
		}
		
		/// <summary>
		/// This method checks that the skill's value does not exceed the maximum skill threshold.
		/// </summary>
		/// <param name="skill">The skill to check.</param>
		/// <param name="value">The skill value to check.</param>
		private bool IsValidSkillLevel( SkillName skill, double value )
		{
			switch ( skill )
			{
				case SkillName.Alchemy:       return value <= m_AlchemyMaximum;
				case SkillName.Anatomy:       return value <= m_AnatomyMaximum;
				case SkillName.AnimalLore:    return value <= m_AnimalLoreMaximum;
				case SkillName.ItemID:        return value <= m_ItemIDMaximum;
				case SkillName.ArmsLore:      return value <= m_ArmsLoreMaximum;
				case SkillName.Parry:         return value <= m_ParryMaximum;
				case SkillName.Begging:       return value <= m_BeggingMaximum;
				case SkillName.Blacksmith:    return value <= m_BlacksmithMaximum;
				case SkillName.Fletching:     return value <= m_FletchingMaximum;
				case SkillName.Peacemaking:   return value <= m_PeacemakingMaximum;
				case SkillName.Camping:       return value <= m_CampingMaximum;
				case SkillName.Carpentry:     return value <= m_CarpentryMaximum;
				case SkillName.Cartography:   return value <= m_CartographyMaximum;
				case SkillName.Cooking:       return value <= m_CookingMaximum;
				case SkillName.DetectHidden:  return value <= m_DetectHiddenMaximum;
				case SkillName.Discordance:   return value <= m_DiscordanceMaximum;
				case SkillName.EvalInt:       return value <= m_EvalIntMaximum;
				case SkillName.Healing:       return value <= m_HealingMaximum;
				case SkillName.Fishing:       return value <= m_FishingMaximum;
				case SkillName.Forensics:     return value <= m_ForensicsMaximum;
				case SkillName.Herding:       return value <= m_HerdingMaximum;
				case SkillName.Hiding:        return value <= m_HidingMaximum;
				case SkillName.Provocation:   return value <= m_ProvocationMaximum;
				case SkillName.Inscribe:      return value <= m_InscribeMaximum;
				case SkillName.Lockpicking:   return value <= m_LockpickingMaximum;
				case SkillName.Magery:        return value <= m_MageryMaximum;
				case SkillName.MagicResist:   return value <= m_MagicResistMaximum;
				case SkillName.Tactics:       return value <= m_TacticsMaximum;
				case SkillName.Snooping:      return value <= m_SnoopingMaximum;
				case SkillName.Musicianship:  return value <= m_MusicianshipMaximum;
				case SkillName.Poisoning:     return value <= m_PoisoningMaximum;
				case SkillName.Archery:       return value <= m_ArcheryMaximum;
				case SkillName.SpiritSpeak:   return value <= m_SpiritSpeakMaximum;
				case SkillName.Stealing:      return value <= m_StealingMaximum;
				case SkillName.Tailoring:     return value <= m_TailoringMaximum;
				case SkillName.AnimalTaming:  return value <= m_AnimalTamingMaximum;
				case SkillName.TasteID:       return value <= m_TasteIDMaximum;
				case SkillName.Tinkering:     return value <= m_TinkeringMaximum;
				case SkillName.Tracking:      return value <= m_TrackingMaximum;
				case SkillName.Veterinary:    return value <= m_VeterinaryMaximum;
				case SkillName.Swords:        return value <= m_SwordsMaximum;
				case SkillName.Macing:        return value <= m_MacingMaximum;
				case SkillName.Fencing:       return value <= m_FencingMaximum;
				case SkillName.Wrestling:     return value <= m_WrestlingMaximum;
				case SkillName.Lumberjacking: return value <= m_LumberjackingMaximum;
				case SkillName.Mining:        return value <= m_MiningMaximum;
				case SkillName.Meditation:    return value <= m_MeditationMaximum;
				case SkillName.Stealth:       return value <= m_StealthMaximum;
				case SkillName.RemoveTrap:    return value <= m_RemoveTrapMaximum;
				case SkillName.Necromancy:    return value <= m_NecromancyMaximum;
				case SkillName.Focus:         return value <= m_FocusMaximum;
				case SkillName.Chivalry:      return value <= m_ChivalryMaximum;
				
				default: return false;
			}
		}
		
		/// <summary>
		/// Returns the skill value of the requested skill.
		/// </summary>
		/// <param name="skill">The skill value to check.</param>
		private double GetSkillValue( SkillName skill )
		{
			switch ( skill )
			{
				case SkillName.Alchemy:       return m_AlchemyMaximum;
				case SkillName.Anatomy:       return m_AnatomyMaximum;
				case SkillName.AnimalLore:    return m_AnimalLoreMaximum;
				case SkillName.ItemID:        return m_ItemIDMaximum;
				case SkillName.ArmsLore:      return m_ArmsLoreMaximum;
				case SkillName.Parry:         return m_ParryMaximum;
				case SkillName.Begging:       return m_BeggingMaximum;
				case SkillName.Blacksmith:    return m_BlacksmithMaximum;
				case SkillName.Fletching:     return m_FletchingMaximum;
				case SkillName.Peacemaking:   return m_PeacemakingMaximum;
				case SkillName.Camping:       return m_CampingMaximum;
				case SkillName.Carpentry:     return m_CarpentryMaximum;
				case SkillName.Cartography:   return m_CartographyMaximum;
				case SkillName.Cooking:       return m_CookingMaximum;
				case SkillName.DetectHidden:  return m_DetectHiddenMaximum;
				case SkillName.Discordance:   return m_DiscordanceMaximum;
				case SkillName.EvalInt:       return m_EvalIntMaximum;
				case SkillName.Healing:       return m_HealingMaximum;
				case SkillName.Fishing:       return m_FishingMaximum;
				case SkillName.Forensics:     return m_ForensicsMaximum;
				case SkillName.Herding:       return m_HerdingMaximum;
				case SkillName.Hiding:        return m_HidingMaximum;
				case SkillName.Provocation:   return m_ProvocationMaximum;
				case SkillName.Inscribe:      return m_InscribeMaximum;
				case SkillName.Lockpicking:   return m_LockpickingMaximum;
				case SkillName.Magery:        return m_MageryMaximum;
				case SkillName.MagicResist:   return m_MagicResistMaximum;
				case SkillName.Tactics:       return m_TacticsMaximum;
				case SkillName.Snooping:      return m_SnoopingMaximum;
				case SkillName.Musicianship:  return m_MusicianshipMaximum;
				case SkillName.Poisoning:     return m_PoisoningMaximum;
				case SkillName.Archery:       return m_ArcheryMaximum;
				case SkillName.SpiritSpeak:   return m_SpiritSpeakMaximum;
				case SkillName.Stealing:      return m_StealingMaximum;
				case SkillName.Tailoring:     return m_TailoringMaximum;
				case SkillName.AnimalTaming:  return m_AnimalTamingMaximum;
				case SkillName.TasteID:       return m_TasteIDMaximum;
				case SkillName.Tinkering:     return m_TinkeringMaximum;
				case SkillName.Tracking:      return m_TrackingMaximum;
				case SkillName.Veterinary:    return m_VeterinaryMaximum;
				case SkillName.Swords:        return m_SwordsMaximum;
				case SkillName.Macing:        return m_MacingMaximum;
				case SkillName.Fencing:       return m_FencingMaximum;
				case SkillName.Wrestling:     return m_WrestlingMaximum;
				case SkillName.Lumberjacking: return m_LumberjackingMaximum;
				case SkillName.Mining:        return m_MiningMaximum;
				case SkillName.Meditation:    return m_MeditationMaximum;
				case SkillName.Stealth:       return m_StealthMaximum;
				case SkillName.RemoveTrap:    return m_RemoveTrapMaximum;
				case SkillName.Necromancy:    return m_NecromancyMaximum;
				case SkillName.Focus:         return m_FocusMaximum;
				case SkillName.Chivalry:      return m_ChivalryMaximum;
				
				default: return 0.0;
			}
		}
		
		/// <summary>
		/// Sets the skill value of the requested skill.
		/// </summary>
		/// <param name="skill">The skill to set.</param>
		/// <param name="value">The skill value to set.</param>
		private void SetSkillValue( SkillName skill, double value )
		{
			switch ( skill )
			{
				case SkillName.Alchemy:       m_AlchemyMaximum       = value; break;
				case SkillName.Anatomy:       m_AnatomyMaximum       = value; break;
				case SkillName.AnimalLore:    m_AnimalLoreMaximum    = value; break;
				case SkillName.ItemID:        m_ItemIDMaximum        = value; break;
				case SkillName.ArmsLore:      m_ArmsLoreMaximum      = value; break;
				case SkillName.Parry:         m_ParryMaximum         = value; break;
				case SkillName.Begging:       m_BeggingMaximum       = value; break;
				case SkillName.Blacksmith:    m_BlacksmithMaximum    = value; break;
				case SkillName.Fletching:     m_FletchingMaximum     = value; break;
				case SkillName.Peacemaking:   m_PeacemakingMaximum   = value; break;
				case SkillName.Camping:       m_CampingMaximum       = value; break;
				case SkillName.Carpentry:     m_CarpentryMaximum     = value; break;
				case SkillName.Cartography:   m_CartographyMaximum   = value; break;
				case SkillName.Cooking:       m_CookingMaximum       = value; break;
				case SkillName.DetectHidden:  m_DetectHiddenMaximum  = value; break;
				case SkillName.Discordance:   m_DiscordanceMaximum   = value; break;
				case SkillName.EvalInt:       m_EvalIntMaximum       = value; break;
				case SkillName.Healing:       m_HealingMaximum       = value; break;
				case SkillName.Fishing:       m_FishingMaximum       = value; break;
				case SkillName.Forensics:     m_ForensicsMaximum     = value; break;
				case SkillName.Herding:       m_HerdingMaximum       = value; break;
				case SkillName.Hiding:        m_HidingMaximum        = value; break;
				case SkillName.Provocation:   m_ProvocationMaximum   = value; break;
				case SkillName.Inscribe:      m_InscribeMaximum      = value; break;
				case SkillName.Lockpicking:   m_LockpickingMaximum   = value; break;
				case SkillName.Magery:        m_MageryMaximum        = value; break;
				case SkillName.MagicResist:   m_MagicResistMaximum   = value; break;
				case SkillName.Tactics:       m_TacticsMaximum       = value; break;
				case SkillName.Snooping:      m_SnoopingMaximum      = value; break;
				case SkillName.Musicianship:  m_MusicianshipMaximum  = value; break;
				case SkillName.Poisoning:     m_PoisoningMaximum     = value; break;
				case SkillName.Archery:       m_ArcheryMaximum       = value; break;
				case SkillName.SpiritSpeak:   m_SpiritSpeakMaximum   = value; break;
				case SkillName.Stealing:      m_StealingMaximum      = value; break;
				case SkillName.Tailoring:     m_TailoringMaximum     = value; break;
				case SkillName.AnimalTaming:  m_AnimalTamingMaximum  = value; break;
				case SkillName.TasteID:       m_TasteIDMaximum       = value; break;
				case SkillName.Tinkering:     m_TinkeringMaximum     = value; break;
				case SkillName.Tracking:      m_TrackingMaximum      = value; break;
				case SkillName.Veterinary:    m_VeterinaryMaximum    = value; break;
				case SkillName.Swords:        m_SwordsMaximum        = value; break;
				case SkillName.Macing:        m_MacingMaximum        = value; break;
				case SkillName.Fencing:       m_FencingMaximum       = value; break;
				case SkillName.Wrestling:     m_WrestlingMaximum     = value; break;
				case SkillName.Lumberjacking: m_LumberjackingMaximum = value; break;
				case SkillName.Mining:        m_MiningMaximum        = value; break;
				case SkillName.Meditation:    m_MeditationMaximum    = value; break;
				case SkillName.Stealth:       m_StealthMaximum       = value; break;
				case SkillName.RemoveTrap:    m_RemoveTrapMaximum    = value; break;
				case SkillName.Necromancy:    m_NecromancyMaximum    = value; break;
				case SkillName.Focus:         m_FocusMaximum         = value; break;
				case SkillName.Chivalry:      m_ChivalryMaximum      = value; break;
				
				default: return;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_MessageString != null )
				list.Add( 1060714, "Message\t{0}", m_MessageString );
			else if ( m_MessageNumber != 0 )
				list.Add( 1060714, "Message\t#{0}", m_MessageNumber );
		}

		[Constructable]
		public MultiSkillTeleporter()
		{
		}

		public MultiSkillTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			
			writer.Write( (int) m_SkillsActive.Count );
			
			foreach ( SkillName skill in m_SkillsActive )
			{
				writer.Write( (int) skill );
				writer.Write( (double) GetSkillValue( skill ) );
			}
/*
			writer.Write( (double) m_AlchemyMaximum );
			writer.Write( (double) m_AnatomyMaximum );
			writer.Write( (double) m_AnimalLoreMaximum );
			writer.Write( (double) m_ItemIDMaximum );
			writer.Write( (double) m_ArmsLoreMaximum );
			writer.Write( (double) m_ParryMaximum );
			writer.Write( (double) m_BeggingMaximum );
			writer.Write( (double) m_BlacksmithMaximum );
			writer.Write( (double) m_FletchingMaximum );
			writer.Write( (double) m_PeacemakingMaximum );
			writer.Write( (double) m_CampingMaximum );
			writer.Write( (double) m_CarpentryMaximum );
			writer.Write( (double) m_CartographyMaximum );
			writer.Write( (double) m_CookingMaximum );
			writer.Write( (double) m_DetectHiddenMaximum );
			writer.Write( (double) m_DiscordanceMaximum );
			writer.Write( (double) m_EvalIntMaximum );
			writer.Write( (double) m_HealingMaximum );
			writer.Write( (double) m_FishingMaximum );
			writer.Write( (double) m_ForensicsMaximum );
			writer.Write( (double) m_HerdingMaximum );
			writer.Write( (double) m_HidingMaximum );
			writer.Write( (double) m_ProvocationMaximum );
			writer.Write( (double) m_InscribeMaximum );
			writer.Write( (double) m_LockpickingMaximum );
			writer.Write( (double) m_MageryMaximum );
			writer.Write( (double) m_MagicResistMaximum );
			writer.Write( (double) m_TacticsMaximum );
			writer.Write( (double) m_SnoopingMaximum );
			writer.Write( (double) m_MusicianshipMaximum );
			writer.Write( (double) m_PoisoningMaximum );
			writer.Write( (double) m_ArcheryMaximum );
			writer.Write( (double) m_SpiritSpeakMaximum );
			writer.Write( (double) m_StealingMaximum );
			writer.Write( (double) m_TailoringMaximum );
			writer.Write( (double) m_AnimalTamingMaximum );
			writer.Write( (double) m_TasteIDMaximum );
			writer.Write( (double) m_TinkeringMaximum );
			writer.Write( (double) m_TrackingMaximum );
			writer.Write( (double) m_VeterinaryMaximum );
			writer.Write( (double) m_SwordsMaximum );
			writer.Write( (double) m_MacingMaximum );
			writer.Write( (double) m_FencingMaximum );
			writer.Write( (double) m_WrestlingMaximum );
			writer.Write( (double) m_LumberjackingMaximum );
			writer.Write( (double) m_MiningMaximum );
			writer.Write( (double) m_MeditationMaximum );
			writer.Write( (double) m_StealthMaximum );
			writer.Write( (double) m_RemoveTrapMaximum );
			writer.Write( (double) m_NecromancyMaximum );
			writer.Write( (double) m_FocusMaximum );
			writer.Write( (double) m_ChivalryMaximum );
*/
			writer.Write( (string) m_MessageString );
			writer.Write( (int) m_MessageNumber );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					int count = reader.ReadInt();
					
					for ( int i = 0; i < count; ++i )
					{
						SkillName skill = (SkillName)reader.ReadInt();
						SetSkillValue( skill, reader.ReadDouble() );
						m_SkillsActive.Add( skill );
					}
/*
					m_AlchemyMaximum = reader.ReadDouble();
					m_AnatomyMaximum = reader.ReadDouble();
					m_AnimalLoreMaximum = reader.ReadDouble();
					m_ItemIDMaximum = reader.ReadDouble();
					m_ArmsLoreMaximum = reader.ReadDouble();
					m_ParryMaximum = reader.ReadDouble();
					m_BeggingMaximum = reader.ReadDouble();
					m_BlacksmithMaximum = reader.ReadDouble();
					m_FletchingMaximum = reader.ReadDouble();
					m_PeacemakingMaximum = reader.ReadDouble();
					m_CampingMaximum = reader.ReadDouble();
					m_CarpentryMaximum = reader.ReadDouble();
					m_CartographyMaximum = reader.ReadDouble();
					m_CookingMaximum = reader.ReadDouble();
					m_DetectHiddenMaximum = reader.ReadDouble();
					m_DiscordanceMaximum = reader.ReadDouble();
					m_EvalIntMaximum = reader.ReadDouble();
					m_HealingMaximum = reader.ReadDouble();
					m_FishingMaximum = reader.ReadDouble();
					m_ForensicsMaximum = reader.ReadDouble();
					m_HerdingMaximum = reader.ReadDouble();
					m_HidingMaximum = reader.ReadDouble();
					m_ProvocationMaximum = reader.ReadDouble();
					m_InscribeMaximum = reader.ReadDouble();
					m_LockpickingMaximum = reader.ReadDouble();
					m_MageryMaximum = reader.ReadDouble();
					m_MagicResistMaximum = reader.ReadDouble();
					m_TacticsMaximum = reader.ReadDouble();
					m_SnoopingMaximum = reader.ReadDouble();
					m_MusicianshipMaximum = reader.ReadDouble();
					m_PoisoningMaximum = reader.ReadDouble();
					m_ArcheryMaximum = reader.ReadDouble();
					m_SpiritSpeakMaximum = reader.ReadDouble();
					m_StealingMaximum = reader.ReadDouble();
					m_TailoringMaximum = reader.ReadDouble();
					m_AnimalTamingMaximum = reader.ReadDouble();
					m_TasteIDMaximum = reader.ReadDouble();
					m_TinkeringMaximum = reader.ReadDouble();
					m_TrackingMaximum = reader.ReadDouble();
					m_VeterinaryMaximum = reader.ReadDouble();
					m_SwordsMaximum = reader.ReadDouble();
					m_MacingMaximum = reader.ReadDouble();
					m_FencingMaximum = reader.ReadDouble();
					m_WrestlingMaximum = reader.ReadDouble();
					m_LumberjackingMaximum = reader.ReadDouble();
					m_MiningMaximum = reader.ReadDouble();
					m_MeditationMaximum = reader.ReadDouble();
					m_StealthMaximum = reader.ReadDouble();
					m_RemoveTrapMaximum = reader.ReadDouble();
					m_NecromancyMaximum = reader.ReadDouble();
					m_FocusMaximum = reader.ReadDouble();
					m_ChivalryMaximum = reader.ReadDouble();
*/
					m_MessageString = reader.ReadString();
					m_MessageNumber = reader.ReadInt();

					break;
				}
			}
		}
	}

	public class KeywordTeleporter : Teleporter
	{
		private string m_Substring;
		private int m_Keyword;
		private int m_Range;

		[CommandProperty( AccessLevel.GameMaster )]
		public string Substring
		{
			get{ return m_Substring; }
			set{ m_Substring = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Keyword
		{
			get{ return m_Keyword; }
			set{ m_Keyword = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Range
		{
			get{ return m_Range; }
			set{ m_Range = value; InvalidateProperties(); }
		}

		public override bool HandlesOnSpeech{ get{ return true; } }

		public override void OnSpeech( SpeechEventArgs e )
		{
			if ( !e.Handled && Active )
			{
				Mobile m = e.Mobile;

				if ( !Creatures && !m.Player )
					return;

				if ( !m.InRange( GetWorldLocation(), m_Range ) )
					return;

				bool isMatch = false;

				if ( m_Keyword >= 0 && e.HasKeyword( m_Keyword ) )
					isMatch = true;
				else if ( m_Substring != null && e.Speech.ToLower().IndexOf( m_Substring.ToLower() ) >= 0 )
					isMatch = true;

				if ( !isMatch )
					return;

				e.Handled = true;
				StartTeleport( m );
			}
		}

		public override bool OnMoveOver( Mobile m )
		{
			return true;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060661, "Range\t{0}", m_Range );

			if ( m_Keyword >= 0 )
				list.Add( 1060662, "Keyword\t{0}", m_Keyword );

			if ( m_Substring != null )
				list.Add( 1060663, "Substring\t{0}", m_Substring );
		}

		[Constructable]
		public KeywordTeleporter()
		{
			m_Keyword = -1;
			m_Substring = null;
		}

		public KeywordTeleporter( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Substring );
			writer.Write( m_Keyword );
			writer.Write( m_Range );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Substring = reader.ReadString();
					m_Keyword = reader.ReadInt();
					m_Range = reader.ReadInt();

					break;
				}
			}
		}
	}
}