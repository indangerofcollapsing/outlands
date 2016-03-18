using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests.Paladin
{
	public class PaladinInitiationQuest : QuestSystem
	{
		private static Type[] m_TypeReferenceTable = new Type[]
			{
				typeof( Paladin.DontOfferConversation ),
			};

		public override Type[] TypeReferenceTable{ get{ return m_TypeReferenceTable; } }

		public override object Name
		{
			get
			{
				return "Paladin Initiation Quest";
			}
		}

		public override object OfferMessage
		{
			get
			{
				return @"Greetings!<br><br>Thine interest in joining the ranks of the Paladins is flattering, as tales of " +
                "thy deeds hath certainly been told within the walls of Trinsic. The Order of the Shining Serpent hath " +
                "existed since the dark ages of this world, and joining our ranks is no small feat.<br><br>If your bravery you " +
                "intend to prove, seek the great silver weapon of Paladins long past from the depths of Deceit.<br><br>Return " +
                "to us this stolen artifact and the Paladins shall surely take note of thy heroism...<br><br><br>Will you accept the offer?";
			}
		}

		public override TimeSpan RestartDelay{ get{ return TimeSpan.Zero; } }
		public override bool IsTutorial{ get{ return false; } }

		public override int Picture{ get{ return 0x15C9; } }

		public PaladinInitiationQuest( PlayerMobile from ) : base( from )
		{
		}

		// Serialization
        public PaladinInitiationQuest()
		{
		}

		public override void ChildDeserialize( GenericReader reader )
		{
			int version = reader.ReadEncodedInt();
		}

		public override void ChildSerialize( GenericWriter writer )
		{
			writer.WriteEncodedInt( (int) 0 ); // version

		}

		public override void Accept()
		{
			base.Accept();
            ReturnDeceitItem qo = new ReturnDeceitItem();
            AddObjective(qo);
            qo.GenerateDeceitItem(From);
		}
	}
}