using System;
using System.Collections;
using Server.Accounting;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Items;
using Server.Misc;

namespace Server.Bounty
{
	public class BountyHunter : BaseCreature
	{
		public static ArrayList bountyList = new ArrayList();
		
		[Constructable]
		public BountyHunter() : base( AIType.AI_Archer, FightMode.Aggressor, 14, 1, 0.8, 1.6 )
		{
			if ( Female = Utility.RandomBool() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
				Title = "the Bounty Huntress";
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
				Title = "the Bounty Hunter";
			}
			
			SetStr( 100 );
			SetDex( 100 );
			SetInt( 25 );
			
			SetDamage( 23, 42 );
			
			SetDamageType( ResistanceType.Physical, 125 );
			
			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 5, 15 );
			
			SetSkill( SkillName.Anatomy, 75.0, 98.0 );
			SetSkill( SkillName.EvalInt, 82.0, 100.0 );
			SetSkill( SkillName.Healing, 75.0, 98.0 );
			SetSkill( SkillName.Magery, 82.0, 100.0 );
			SetSkill( SkillName.MagicResist, 82.0, 100.0 );
			SetSkill( SkillName.Tactics, 82.0, 100.0 );
			
			Fame = 2000;
			Karma = 2000;
			
			AddItem( new Sandals() );
			AddItem( new Tunic( Utility.RandomRedHue() ) );
			AddItem( new LongPants( Utility.RandomRedHue() ) );
			AddItem( new Bow() );
			PackItem( new Arrow( Utility.RandomMinMax( 15, 30 ) ) );
			PackItem( new Bandage( 10 ) );
			PackItem( new Kindling( 3 ) );
			
			if( 0.5 < Utility.RandomDouble() )
				AddItem( new LongHair( Utility.RandomHairHue() ) );
			else
				AddItem( new PonyTail( Utility.RandomHairHue() ) );
		}
	
		public override void OnSpeech( SpeechEventArgs m ) 
		{
			Mobile from = m.Mobile as Mobile;
			PlayerMobile pm = from as PlayerMobile;
		
			if ( from.InRange( this, 4 ) && this.CanSee( from ) )
			{
				if ( m.Speech.ToLower().IndexOf( "bounty" ) >= 0 || m.Speech.ToLower().IndexOf( "hunt" ) >= 0 || m.Speech.ToLower().IndexOf( "job" ) >= 0 )
				{
					Direction = GetDirectionTo( from );
					GenerateBounty();
					
					if( bountyList.Count > 0 )
						Say( "Greetings, adventurer. Currently there doth exist a bounty? Doth thou wish to accept this bounty?" );
					else
						Say( "In these peaceful times, there are no bounties." );
				}
				
				if ( m.Speech.ToLower().IndexOf( "accept" ) >= 0 )
				{
					Direction = GetDirectionTo( from );
					GenerateBounty();
					
					if( bountyList.Count > 0 )
					{
						if( from.ShortTermMurders < 5 )
						{
							if( pm.NextBountyNote == TimeSpan.Zero )
							{
								Say( "I wish thee luck, adventurer. May thou bringest swift justice!" );
								
								from.AddToBackpack( new BountyNote( bountyList[Utility.Random(bountyList.Count)] as Mobile, from ) );
								pm.NextBountyNote = TimeSpan.FromDays( 7 );
							}
							else
								Say( "Thy previous bounty hath not expired nor hath it been completed." );
						}
						else
							Say( "Thou art an outcast and cannot enforce justice!" );
					}
					else
						Say( "In these peaceful times, there exist no bounties in all of Britannia." );
				}
			}
		}
		
		public static void GenerateBounty()
		{			
			foreach( NetState ns in NetState.Instances )
			{
				Mobile m = ns.Mobile;
				
				if( ns != null && m.ShortTermMurders >= 5 )
					bountyList.Add( m );
			}
		}
		
		public BountyHunter( Serial serial ) : base( serial )
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
	
	[FlipableAttribute( 0x14EE, 0x14EF )]
	public class BountyNote : Item
	{
		private static Mobile m_Hunted, m_Hunter;
		private static bool m_Completed;
		private int reward;
		private DateTime m_Decompose;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public static Mobile Hunted
		{
			get{ return m_Hunted; }
			set{ m_Hunted = value; }
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public static Mobile Hunter
		{
			get{ return m_Hunter; }
			set{ m_Hunter = value; }
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public static bool Completed
		{
			get{ return m_Completed; }
			set{ m_Completed = value; }
		}
		
		[CommandProperty(AccessLevel.GameMaster)]
		public TimeSpan Decompose
		{
			get
			{
				TimeSpan ts = m_Decompose - DateTime.UtcNow;

				if( ts < TimeSpan.Zero )
					ts = TimeSpan.Zero;

				return ts;
			}
			set
			{
				try { m_Decompose = DateTime.UtcNow + value; }
				catch{}
			}
		}
		
		public BountyNote( Mobile hunted, Mobile hunter ) : base( 0x14EF )
		{
			Name = "a bounty note";
			Hue = 0x21;
			Hunter = hunter;
			Hunted = hunted;
			LootType = LootType.Blessed;
			reward = hunted.ShortTermMurders * 100;
			Completed = false;
			this.Decompose = TimeSpan.FromDays( 7 );
		}
		
		public BountyNote( Serial serial ) : base( serial )
		{
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if( Hunted != null )
			{
				if( this.Decompose > TimeSpan.Zero )
				{
					LabelTo( from, "[ " + Hunted.Name + " : " + reward + " ]" );
					
					if( Completed == false )
						from.Target = new HeadTarget( this );
				}
				else
					from.SendMessage( "The bounty note is scrawled, you can no longer read the writing." );
			}
		}
		
		public override bool OnDroppedToMobile( Mobile from, Mobile target )
		{
			PlayerMobile pm = from as PlayerMobile;
			
			if( from.ShortTermMurders >= 5 && target is BountyHunter ) // Murderers can't be bounty hunters.
			{
				target.Say( "Thou wicked soul cannot enforce justice." );
				return false;
			}
			
			if( target is BountyHunter )
			{			
				if( pm.NextBountyNote < pm.NextBountyNote + TimeSpan.FromDays( 7 ) )
				{
					if( Hunter == from )
					{
						if( Completed == true )
						{
							if( reward <= 1000 )
								from.AddToBackpack( new Gold( reward ) );
							else
								from.AddToBackpack( new BankCheck( reward ) );
						
							target.Say( "Well done, noble hunter. Here is thy reward for thine valiant efforts." );						
							pm.NextBountyNote = TimeSpan.Zero;
							this.Consume();
							return true;
						}
						else
							target.Say( "Thy task hath not been completed." );
					}
					else						
						target.Say( "Dost thou try to deceive me? This is not your bounty!" );
				}
				else
					target.Say( "Thou bounty note is no longer readable." );
			}
			else
				from.SendMessage( "Only a bounty hunter can accept this note." );
			
			return true;
		}
		
		public class HeadTarget : Target
		{
			private Item m_Item;
			
			public HeadTarget( Item item ) : base( 0, false, TargetFlags.None )
			{
				m_Item = item;
			}
			
			protected override void OnTarget( Mobile from, object targeted )
			{
				if( m_Item == null || m_Item.Deleted )
					return;
				
				if( targeted is Head )
				{					
					//if( !((Head)targeted).Decomposed )
					//{
						if( ((Head)targeted).Serial == BountyNote.Hunted.Serial )
						{
							if( from == BountyNote.Hunter )
							{
								BountyNote.Completed = true;
								((Head)targeted).Consume();
							}
							else
								from.SendMessage( "This is not your bounty note." );
						}
						else
							from.SendMessage( "This head does not match the offender." );
					//}
					//else
					//	from.SendMessage( "The head has already decomposed and cannot be verified as the offender." );
				}
				else
					from.SendMessage( "That is not a head." );
			}
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			
			writer.Write( Decompose );
			writer.Write( (int) reward );
			writer.Write( (bool) m_Completed );
			writer.Write( (Mobile) m_Hunter );
			writer.Write( (Mobile) m_Hunted );
		}
		
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			Decompose = reader.ReadTimeSpan();
			int reward = reader.ReadInt();
			bool m_Completed = reader.ReadBool();
			m_Hunter = (PlayerMobile)reader.ReadMobile();
			m_Hunted = (PlayerMobile)reader.ReadMobile();
		}
	}
}
