using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Custom;

namespace Server.Mobiles
{
	public class DetectiveGuildmaster : BaseGuildmaster
	{
		public override NpcGuild NpcGuild{ get{ return NpcGuild.DetectivesGuild; } }

		public override TimeSpan JoinAge{ get{ return TimeSpan.FromDays( 0.1 ); } }

		[Constructable]
		public DetectiveGuildmaster() : base( "detective" )
		{
			SetSkill( SkillName.Forensics, 75.0, 98.0 );
		}

		/*public override void InitOutfit()
		{
			base.InitOutfit();
		}*/

		public override bool CheckCustomReqs( PlayerMobile pm )
		{
			if ( pm.Young )
			{
				SayTo( pm, "You cannot be a member of the Detective's Guild while you are Young."); // You cannot be a member of the Thieves' Guild while you are Young.
				return false;
			}
			else if ( pm.ShortTermMurders > 0 )
			{
				SayTo( pm, "This guild despises people like you. Now get out of here." ); // This guild is for cunning thieves, not oafish cutthroats.
				return false;
			}
			else if ( pm.Skills[SkillName.Forensics].Base < 60.0 )
			{
				SayTo( pm, "You must be at least a journeyman forensic evaluator to join this elite organization." ); // You must be at least a journeyman pickpocket to join this elite organization.
				return false;
			}
            else if (pm.Paladin)
            {
                SayTo(pm, "This guild is for detectives, not paladins.");
                return false;
            }

            // Disabled for now - Jimmy
			return false;
		}

		public override void SayWelcomeTo( Mobile m )
		{
			SayTo( m, "Welcome to the guild! May your skill put many behind bars." ); // Welcome to the guild! Stay to the shadows, friend.
		}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is WantedNote)
            {
                WantedNote wn = (WantedNote)dropped;
                if (wn.Killer != null && wn.Detective != null)
                {
					if (wn.TimeCreated + TimeSpan.FromDays(3) < DateTime.UtcNow)
                    {
                        PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, "There is nothing we can do about this crime now. The murderer is probably long gone.");
                        return false;
                    }
                    else if (wn.Killer.Map == Map.Internal)
                    {
                        from.SendMessage("The murderer is not online. Please turn in the wanted poster later.");
                        return false;
                    }
                    else if (Detective.KillerInDatabase(wn.Killer))
                    {
                        PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, "Bounty hunters are already out looking for this criminal.");
                        return false;
                    }
                    else
                    {
                        PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, String.Format("Thank you for your hard work, Detective {0}. I will send bounty hunters out for {1} immediately.", from.Name, wn.Killer.Name));
                        Detective.AddDetectiveMurdererPair(wn.Killer, wn.Detective);
                        JailBountyHunterControl.SpawnBountyHunters(wn.Killer, 0.01 * (double)wn.Quality);
                        wn.Delete();
                        return true;
                    }
                }
                else
                {
                    PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, String.Format("Thank you for your hard work, Detective {0}.", from.Name));
                }

                wn.Delete();
                return true;
            }
            
            
            return base.OnDragDrop(from, dropped);
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (from.InRange(this.Location, 2))
                return true;

            return base.HandlesOnSpeech(from);
        }

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile from = e.Mobile;

            string text = e.Speech;

			if ( !e.Handled && from is PlayerMobile && from.InRange( this.Location, 2 ) && text.IndexOf("book") != -1 )
			{
				PlayerMobile pm = (PlayerMobile)from;

				if ( pm.NpcGuild == NpcGuild.DetectivesGuild )
					SayTo(from, "That particular item costs 300 gold pieces." ); // That particular item costs 700 gold pieces.
				else
					SayTo( from, "Only detectives may purchase a clue book."); // I don't know what you're talking about.

				e.Handled = true;
			}

			base.OnSpeech( e );
		}

		public override bool OnGoldGiven( Mobile from, Gold dropped )
		{
			if ( from is PlayerMobile && dropped.Amount == 300 )
			{
				PlayerMobile pm = (PlayerMobile)from;

				if ( pm.NpcGuild == NpcGuild.DetectivesGuild )
				{
					from.AddToBackpack( new ClueBook() );

					dropped.Delete();
					return true;
				}
			}

			return base.OnGoldGiven( from, dropped );
		}

		public DetectiveGuildmaster( Serial serial ) : base( serial )
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