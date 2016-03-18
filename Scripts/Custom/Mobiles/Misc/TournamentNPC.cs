using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles
{
	public class TournamentNPC : Mobile
	{
		public static int TourneyPot;
		public const int ContestantLimit = 64;
		public static int CurrentParticipants;

		[Constructable]
		public TournamentNPC()
		{
			Str = 100;
			Dex = 100;
			Int = 100;

			Hits = 100;
			Mana = 100;
			Stam = 100;

			Blessed = true;

			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 401;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 400;
				Name = NameList.RandomName( "male" );
			}

			Title = "the Ticketseller";

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
			if ( from.InRange( this.Location, 12 ) )
				return true;

			return base.HandlesOnSpeech( from );
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );

			if ( e.Mobile.InRange( this.Location, 3 ) )
			{
				if ( e.Speech.ToLower().IndexOf( "i wish to participate" ) >= 0 )
				{
					if ( CurrentParticipants >= ContestantLimit )
					{
						this.Say( "I'm sorry, but the contestant limit as been reached." );
					}
					else
					{
						e.Mobile.CloseGump( typeof ( DuelGump ) );
						e.Mobile.SendGump( new DuelGump( e.Mobile ) );
					}
				}
				else if ( e.Speech.ToLower().IndexOf( "i wish to spectate" ) >= 0 )
				{
					e.Mobile.CloseGump( typeof ( SpectateGump ) );
					e.Mobile.SendGump( new SpectateGump( e.Mobile ) );
				}
				else if ( e.Speech.ToLower().IndexOf( "pot" ) >= 0 && e.Mobile.AccessLevel == AccessLevel.Administrator )
				{
					e.Mobile.SendMessage( String.Format( "The current tourney prize is: {0} gp.", TourneyPot ) );
				}
				else if ( e.Speech.ToLower().IndexOf( "collect" ) >= 0 && e.Mobile.AccessLevel == AccessLevel.Administrator )
				{
					Container pack = e.Mobile.Backpack;

					if ( pack != null )
					{
						pack.DropItem( new BankCheck( TourneyPot ) );
						TourneyPot = 0;
					}
					else
					{
						e.Mobile.SendMessage( "You do not have a backpack..." );
					}
				}
			}
		}

		public TournamentNPC( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}