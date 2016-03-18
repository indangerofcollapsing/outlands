using System;
using Server;
using Server.Items;
using Server.Gumps;
using System.Collections;
using Server.Commands;

namespace Server.Mobiles
{
	public enum QuestStep
	{
		None,
		ScavengerHunt,
		AirSlash,
		Ranch,
		Heavens,
		Jungle,
		Fights,
		Bandit,
		Skull,
		Hope,
		Iamubar,
		Completed
	}

	public class GenerateQuestNPC
	{
		public static void Initialize()
		{
			CommandSystem.Register( "QuestNPCGen", AccessLevel.GameMaster, new CommandEventHandler( QuestNPCGen_OnCommand ) );
			CommandSystem.Register( "QuestNPCRem", AccessLevel.GameMaster, new CommandEventHandler( QuestNPCRem_OnCommand ) );
		}

		[Usage( "QuestNPCGen" )]
		[Description( "Generates all QuestNPCs" )]
		private static void QuestNPCGen_OnCommand( CommandEventArgs e )
		{
			ArrayList list = new ArrayList();
			ArrayList list2 = new ArrayList();

			foreach( Mobile m in World.Mobiles.Values )
			{
				if ( m is QuestNPC )
					list.Add( m );

				if ( m is PlayerMobile )
					list2.Add( m );
			}

			for ( int i = 0; i < list.Count; i++ )
				((Mobile)list[i]).Delete();

			for ( int i = 0; i < list2.Count; i++ )
				((PlayerMobile)list2[i]).Step = QuestStep.None;
			
			MakeNPC( 1 );
			MakeNPC( 2 );
			MakeNPC( 3 );			
			MakeNPC( 4 );			
			MakeNPC( 5 );			
			MakeNPC( 6 );			
			MakeNPC( 7 );			
			MakeNPC( 8 );			
			MakeNPC( 9 );			
			MakeNPC( 10 );

			e.Mobile.SendMessage( "Quest NPC generation is complete." );
		}


		[Usage( "QuestNPCRem" )]
		[Description( "Removes all QuestNPCs" )]
		private static void QuestNPCRem_OnCommand( CommandEventArgs e )
		{
			ArrayList list = new ArrayList();
			ArrayList list2 = new ArrayList();

			foreach( Mobile m in World.Mobiles.Values )
			{
				if ( m is QuestNPC )
					list.Add( m );

				if ( m is PlayerMobile )
					list2.Add( m );
			}

			for ( int i = 0; i < list.Count; i++ )
				((Mobile)list[i]).Delete();

			for ( int i = 0; i < list2.Count; i++ )
				((PlayerMobile)list2[i]).Step = QuestStep.None;


			e.Mobile.SendMessage( "Quest NPC removal is complete." );
		}

		public static void MakeNPC( int i )
		{
			QuestNPC npc = new QuestNPC();
			npc.Step = (QuestStep)i;
		}
	}

	public class QuestEntry
	{
		private string m_Trigger;
		private string m_Reply;

		public string Trigger
		{
			get{ return m_Trigger; }
			set{ m_Trigger = value; }
		}

		public string Reply
		{
			get{ return m_Reply; }
			set{ m_Reply = value; }
		}

		public QuestEntry( QuestStep step )
		{
			switch ( step )
			{
				case QuestStep.None: break;
				case QuestStep.ScavengerHunt:
				{
					m_Trigger = "scavenger hunt";
					m_Reply = "<p>Hello adventurer! Welcome to my scavenger hunt, there are 10 NPCs on this hunt, and you have only 2 hours to complete it. Each NPC will give you a riddle or hint to find the next one, and so forth. The first 5 players to arrive at the final NPC will recieve random prize bags. The remaining participant will recieve misc gold/gems. Good luck adventurer!</p><p>Here is your first clue:<br>Where elementals blow through the tunnels like dust in the wind, and scorpions poison all who come near! When you find Calvin, tell him the secret passphrase: [airslash].</p>";
					break;
				}
				case QuestStep.AirSlash:
				{
					m_Trigger = "airslash";
					m_Reply = "Ahh you've found me! You'll find the next NPC in the hidden valley outside of Trinsic. Speak the passphrase: [ranch]";
					break;
				}
				case QuestStep.Ranch:
				{
					m_Trigger = "ranch";
					m_Reply = "Ahh you've made it this far have you? Well let us see if you can find the Kala, hidden where she can view the most distant heavenly bodies. Ask her about the [heavens] when you see her.";
					break;
				}
				case QuestStep.Heavens: 
				{
					m_Trigger = "heavens";
					m_Reply = "Very good! That was a hard one. Now that you've found me I expect you want your next clue. Alright, here it is, you'll find your next quarry at the shrine who's symbol is a sword. Once you get there speak the passphrase [jungle].";
					break;
				}
				case QuestStep.Jungle:
				{
					m_Trigger = "jungle";
					m_Reply = "The next step of this journey will take you to an island town, inside that town there will be a place where fighters duel until the death! In this arena you will find Candy, ask her about the [fights].";
					break;
				}
				case QuestStep.Fights:
				{
					m_Trigger = "fights";
					m_Reply = "<p>The fights? Oh yes, when we get some good fighters in there it can be quite a show. However I'm asusming you're not here to watch any fights. You probably want your next clue.</p><p>Well here it is:<br>Your next contact will be found consorting with thieves between Britain and Yew. When you find him speak the passphrase [bandit].</p>";
					break;
				}
				case QuestStep.Bandit:
				{
					m_Trigger = "bandit";
					m_Reply = "Ahh hello adventurer! Your next quarry is hidden deep on a frosty island. He's in a room not often visited with a lich keeping him company. When you seek him out, speak the passphrase [skull].";
					break;
				}
				case QuestStep.Skull:
				{
					m_Trigger = "skull";
					m_Reply = "Excellent! You are nearing the end, your next quarry never made it to his post! He was kidnapped by BloodClan orcs on his way to Empath Abbey! Quick adventurers, go seek out Oscar and tell him there is [hope] of escape!";
					break;
				}
				case QuestStep.Hope:
				{
					m_Trigger = "hope";
					m_Reply = "Well hello there adventurer! Yes tis true the orcs kidnapped me, but I rather like it here. You're likely here for your final clue aren't you? Well this one will not be easy, but here goes. You're looking for the  \"Britain Public Library\". Go there and speak the mantra [iamubar] and you will recieve your prize!";
					break;
				}
				case QuestStep.Iamubar:
				{
					m_Trigger = "iamubar";
					m_Reply = "Congratulations! You've won, if you are one of the first 5 you will be recieving a prize shortly! Thanks for participating!";
					break;
				}
			}
		}
	}

	public class QuestNPC : BaseCreature
	{
		private int m_PeopleCompleted;
		private QuestStep m_Step;

		[CommandProperty( AccessLevel.GameMaster )]
		public int PeopleCompleted
		{ 
			get{ return m_PeopleCompleted;  }
			set{ m_PeopleCompleted = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public QuestStep Step
		{ 
			get{ return m_Step;  }
			set{ m_Step = value; UpdateProperties(); } 
		}

		[Constructable]
		public QuestNPC() : base( AIType.AI_Melee, FightMode.None, 22, 1, 0.2, 1.0 )
		{
			Str = 100;
			Dex = 100;
			Int = 100;

			Hits = 100;
			Mana = 100;
			Stam = 100;

			Blessed = true;
			CantWalk = true;
			Frozen = true;

			Hue = Utility.RandomSkinHue();
			Body = 400;
			Female = false;
			Name = "[Debug]: " + this.Serial.ToString();
			AddItem( new FancyShirt( GetRandomHue() ) );

			int lowHue = GetRandomHue();

			AddItem( new ShortPants( lowHue ) );
			AddItem( new Boots( lowHue ) );
			AddItem( new BodySash( lowHue ) );
			AddItem( new Doublet( GetRandomHue() ) );

			switch ( Utility.Random( 4 ) )
			{
				case 0: AddItem( new ShortHair( Utility.RandomHairHue() ) ); break;
				case 1: AddItem( new TwoPigTails( Utility.RandomHairHue() ) ); break;
				case 2: AddItem( new ReceedingHair( Utility.RandomHairHue() ) ); break;
				case 3: AddItem( new KrisnaHair( Utility.RandomHairHue() ) ); break;
			}
		}

		public override bool ClickTitle{ get{ return false; } }

		public void UpdateProperties()
		{
			switch( m_Step )
			{
				case QuestStep.ScavengerHunt:	Name = "Vahn";					break;
				case QuestStep.AirSlash:		Name = "Calvin";				break;
				case QuestStep.Ranch:			Name = "Raul";					break;
				case QuestStep.Heavens:			Name = "Kala";		IsGirl();	break;
				case QuestStep.Jungle:			Name = "Branson";				break;
				case QuestStep.Fights:			Name = "Candy";		IsGirl();	break;
				case QuestStep.Bandit:			Name = "Lyle";					break;
				case QuestStep.Skull:			Name = "Nancy";		IsGirl();	break;
				case QuestStep.Hope:			Name = "Oscar";					break;
				case QuestStep.Iamubar:			Name = "Tim";					break;
			}

			MoveQuestNPC();
		}

		public void IsGirl()
		{
			Body = 401; 
			Female = true;
		}

		public void MoveQuestNPC()
		{			
			this.Map = Map.Felucca;

			switch( m_Step )
			{
				case QuestStep.ScavengerHunt:		this.Location = new Point3D( 1496, 1517, 40 );		Direction = Direction.East;		break;
				case QuestStep.AirSlash:		this.Location = new Point3D( 5620, 16, 10 );		Direction = Direction.South;	break;
				case QuestStep.Ranch:			this.Location = new Point3D( 1671, 2986, 20 );		Direction = Direction.East;		break;
				case QuestStep.Heavens:			this.Location = new Point3D( 4714, 1115, 0 );		Direction = Direction.South;	break;
				case QuestStep.Jungle:			this.Location = new Point3D( 2490, 3930, 2 );		Direction = Direction.East;		break;
				case QuestStep.Fights:			this.Location = new Point3D( 1390, 3729, -21 );		Direction = Direction.South;	break;
				case QuestStep.Bandit:			this.Location = new Point3D( 840, 1681, 0 );		Direction = Direction.East;		break;
				case QuestStep.Skull:			this.Location = new Point3D( 5147, 745, 0 );		Direction = Direction.East;	break;
				case QuestStep.Hope:			this.Location = new Point3D( 631, 1477, 15 );		Direction = Direction.East;		break;
				case QuestStep.Iamubar:			this.Location = new Point3D( 1408, 1598, 30 );		Direction = Direction.East;		break;
			}
		}

		private static int GetRandomHue()
		{
			switch ( Utility.Random( 6 ) )
			{
				default:
				case 0: return 0;
				case 1: return Utility.RandomBlueHue();
				case 2: return Utility.RandomGreenHue();
				case 3: return Utility.RandomRedHue();
				case 4: return Utility.RandomYellowHue();
				case 5: return Utility.RandomNeutralHue();
			}
		}

		public override bool HandlesOnSpeech( Mobile from )
		{
			if ( !from.Alive )
				return false;

			if ( from.InRange( this.Location, 12 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );

			if ( e.Mobile.InRange( this.Location, 12 ) && e.Mobile.InLOS( this ) )
			{
				if ( !( e.Mobile is PlayerMobile ) )
					return;

				PlayerMobile from = (PlayerMobile)e.Mobile;
				string speech = e.Speech.ToLower();
				QuestEntry entry = new QuestEntry( m_Step );
				
				if ( speech == entry.Trigger && from.Step != QuestStep.Completed && ( ( from.Step == QuestStep.None && m_Step == QuestStep.ScavengerHunt ) || from.Step == m_Step ) )
				{
					++m_PeopleCompleted;

					if ( m_Step == QuestStep.Iamubar )
					{
						from.Step = QuestStep.Completed;
						this.Say( String.Format( "Congratulations! You are player number {0} to complete the scavenger hunt! {1}", m_PeopleCompleted, m_PeopleCompleted <= 5 ? "Please wait now for your prize. Thanks for participating!" : "Thanks for participating!" ) );
					}
					else
					{
						if ( from.Step == QuestStep.None && m_Step == QuestStep.ScavengerHunt )
							from.Step = (QuestStep)((int)from.Step + 2 );
						else
							from.Step = (QuestStep)((int)from.Step + 1 );

						from.CloseGump( typeof( QuestConversationGump ) );
						from.SendGump( new QuestConversationGump( entry.Reply ) );
					}
				}
			}
		}

		public QuestNPC( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
			writer.Write( (int) m_Step );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			m_Step = (QuestStep)reader.ReadInt();
		}
	}
}