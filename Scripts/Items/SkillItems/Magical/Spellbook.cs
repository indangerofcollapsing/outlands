using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Engines.Craft;
using Server.Network;
using Server.Spells;
using Server.Targeting;
using Server.Mobiles;

namespace Server.Items
{
	public enum SpellbookType
	{
		Invalid = -1,
		Regular,
		Necromancer,
		Paladin,
		Ninja,
		Samurai,
		Arcanist
	}

	public enum BookQuality
	{
		Regular,
		Exceptional,
	}

	public class Spellbook : Item, ICraftable
	{
		private BookQuality m_Quality;
		[CommandProperty( AccessLevel.GameMaster )]		
		public BookQuality Quality
		{
			get{ return m_Quality; }
			set{ m_Quality = value; InvalidateProperties(); }
		}

        private SlayerGroupType m_SlayerGroup;
        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerGroupType SlayerGroup
        {
            get { return m_SlayerGroup; }
            set { m_SlayerGroup = value; InvalidateProperties(); }
        }

        public override bool DisplayWeight { get { return false; } }

        public virtual SpellbookType SpellbookType { get { return SpellbookType.Regular; } }
        public virtual int BookOffset { get { return 0; } }
        public virtual int BookCount { get { return 64; } }

        public override bool DisplayLootType { get { return false; } }

        private ulong m_Content;
        private int m_Count;

		public static void Initialize()
		{
			EventSink.OpenSpellbookRequest += new OpenSpellbookRequestEventHandler( EventSink_OpenSpellbookRequest );
			EventSink.CastSpellRequest += new CastSpellRequestEventHandler( EventSink_CastSpellRequest );

			CommandSystem.Register( "AllSpells", AccessLevel.GameMaster, new CommandEventHandler( AllSpells_OnCommand ) );
		}

		[Usage( "AllSpells" )]
		[Description( "Completely fills a targeted spellbook with scrolls." )]
		private static void AllSpells_OnCommand( CommandEventArgs e )
		{
			e.Mobile.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( AllSpells_OnTarget ) );
			e.Mobile.SendMessage( "Target the spellbook to fill." );
		}

		private static void AllSpells_OnTarget( Mobile from, object obj )
		{
			if ( obj is Spellbook )
			{
				Spellbook book = (Spellbook)obj;

				if ( book.BookCount == 64 )
					book.Content = ulong.MaxValue;
				else
					book.Content = (1ul << book.BookCount) - 1;

				from.SendMessage( "The spellbook has been filled." );

				CommandLogging.WriteLine( from, "{0} {1} filling spellbook {2}", from.AccessLevel, CommandLogging.Format( from ), CommandLogging.Format( book ) );
			}

			else
			{
				from.BeginTarget( -1, false, TargetFlags.None, new TargetCallback( AllSpells_OnTarget ) );
				from.SendMessage( "That is not a spellbook. Try again." );
			}
		}

		private static void EventSink_OpenSpellbookRequest( OpenSpellbookRequestEventArgs e )
		{
			Mobile from = e.Mobile;

			if ( !Multis.DesignContext.Check( from ) )
				return; // They are customizing

			SpellbookType type;

			switch ( e.Type )
			{
				default:
				case 1: type = SpellbookType.Regular; break;
				case 2: type = SpellbookType.Necromancer; break;
				case 3: type = SpellbookType.Paladin; break;
				case 4: type = SpellbookType.Ninja; break;
				case 5: type = SpellbookType.Samurai; break;
				case 6:	type = SpellbookType.Arcanist; break;
			}

			Spellbook book = Spellbook.Find( from, -1, type );

			if ( book != null )
				book.DisplayTo( from );
		}

		private static void EventSink_CastSpellRequest( CastSpellRequestEventArgs e )
		{
            Mobile from = e.Mobile;

            if (!Multis.DesignContext.Check(from))
                return; // They are customizing                     

            Spellbook book = e.Spellbook as Spellbook;

            int spellID = e.SpellID;

            if (book == null || !book.HasSpell(spellID))            
                book = Find(from, spellID);                       

            if (book != null && book.HasSpell(spellID))
            {                
                Spell spell = SpellRegistry.NewSpell(spellID, from, null);

                if (spell != null)
                    spell.Cast();

                else
                    from.SendLocalizedMessage(502345); // This spell has been temporarily disabled.                
            }

            else                          
                from.SendLocalizedMessage(500015); // You do not have that spell!    
		}

		private static Dictionary<Mobile, List<Spellbook>> m_Table = new Dictionary<Mobile, List<Spellbook>>();

		public static SpellbookType GetTypeForSpell( int spellID )
		{
			if ( spellID >= 0 && spellID < 64 )
				return SpellbookType.Regular;

			else if ( spellID >= 100 && spellID < 117 )
				return SpellbookType.Necromancer;

			else if ( spellID >= 200 && spellID < 210 )
				return SpellbookType.Paladin;

			else if( spellID >= 400 && spellID < 406 )
				return SpellbookType.Samurai;

			else if( spellID >= 500 && spellID < 508 )
				return SpellbookType.Ninja;

			else if ( spellID >= 600 && spellID < 617 )
				return SpellbookType.Arcanist;

			return SpellbookType.Invalid;
		}

		public static Spellbook FindRegular( Mobile from )
		{
			return Find( from, -1, SpellbookType.Regular );
		}

		public static Spellbook FindNecromancer( Mobile from )
		{
			return Find( from, -1, SpellbookType.Necromancer );
		}

		public static Spellbook FindPaladin( Mobile from )
		{
			return Find( from, -1, SpellbookType.Paladin );
		}

		public static Spellbook FindSamurai( Mobile from )
		{
			return Find( from, -1, SpellbookType.Samurai );
		}

		public static Spellbook FindNinja( Mobile from )
		{
			return Find( from, -1, SpellbookType.Ninja );
		}

		public static Spellbook FindArcanist( Mobile from )
		{
			return Find( from, -1, SpellbookType.Arcanist );
		}

		public static Spellbook Find( Mobile from, int spellID )
		{
			return Find( from, spellID, GetTypeForSpell( spellID ) );
		}

		public static Spellbook Find( Mobile from, int spellID, SpellbookType type )
		{
			if ( from == null )
				return null;

			if ( from.Deleted )
			{
				m_Table.Remove( from );
				return null;
			}

			List<Spellbook> list = null;

			m_Table.TryGetValue( from, out list );

			bool searchAgain = false;

			if ( list == null )
				m_Table[from] = list = FindAllSpellbooks( from );

			else
				searchAgain = true;

			Spellbook book = FindSpellbookInList( list, from, spellID, type );

			if ( book == null && searchAgain )
			{
				m_Table[from] = list = FindAllSpellbooks( from );

				book = FindSpellbookInList( list, from, spellID, type );
			}

			return book;
		}

		public static Spellbook FindSpellbookInList( List<Spellbook> list, Mobile from, int spellID, SpellbookType type )
		{
			Container pack = from.Backpack;

			for ( int i = list.Count - 1; i >= 0; --i )
			{
				if ( i >= list.Count )
					continue;

				Spellbook book = list[i];
                
				if (!(book is EnhancedSpellbook) && !book.Deleted && (book.Parent == from || (pack != null && book.Parent == pack)) && ValidateSpellbook( book, spellID, type ) )
					return book;

				list.RemoveAt( i );
			}

			return null;
		}

		public static List<Spellbook> FindAllSpellbooks( Mobile from )
		{
			List<Spellbook> list = new List<Spellbook>();

			Item item = from.FindItemOnLayer( Layer.OneHanded );

			if ( item is Spellbook )
				list.Add( (Spellbook)item );

			Container pack = from.Backpack;

			if ( pack == null )
				return list;

			for ( int i = 0; i < pack.Items.Count; ++i )
			{
				item = pack.Items[i];

				if ( item is Spellbook )
					list.Add( (Spellbook)item );
			}

			return list;
		}

		public static Spellbook FindEquippedSpellbook( Mobile from )
		{
			return (from.FindItemOnLayer( Layer.OneHanded ) as Spellbook);
		}

		public static bool ValidateSpellbook( Spellbook book, int spellID, SpellbookType type )
		{
			return ( book.SpellbookType == type && ( spellID == -1 || book.HasSpell( spellID ) ) );
		}

        public void AddScroll(Spellbook spellbook, SpellScroll scroll)
        {
            if (spellbook == null || scroll == null)            
                return;            

            int val = scroll.SpellID - BookOffset;                      

            if (val >= 0 && val < BookCount)
            {
                spellbook.Content |= (ulong)1 << val;
                ++m_Count;

                InvalidateProperties();

                scroll.Delete();
            }
        }

		public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			if ( !Ethics.Ethic.CheckTrade( from, to, newOwner, this ) )
				return false;

			return base.AllowSecureTrade( from, to, newOwner, accepted );
		}

		public override bool CanEquip( Mobile from )
		{
			if ( !Ethics.Ethic.CheckEquip( from, this ) )			
				return false;
			
			else if ( !from.CanBeginAction( typeof( BaseWeapon ) ) )			
				return false;			

			return base.CanEquip( from );
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			return true;
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is SpellScroll && dropped.Amount == 1)
			{
				SpellScroll scroll = (SpellScroll)dropped;

                if (scroll.MasterStatus > 0)
                    return false;

				SpellbookType type = GetTypeForSpell( scroll.SpellID );

				if ( type != this.SpellbookType )				
					return false;				

                else if (scroll.MasterStatus > 0)
                {
                    from.SendMessage("That is a Master Scroll.");
                    return false;
                }

                else if (HasSpell(scroll.SpellID))
                {
                    from.SendLocalizedMessage(500179); // That spell is already present in that spellbook.
                    return false;
                }

                else
                {
                    int val = scroll.SpellID - BookOffset;

                    if (val >= 0 && val < BookCount)
                    {
                        m_Content |= (ulong)1 << val;
                        ++m_Count;

                        InvalidateProperties();

                        scroll.Delete();

                        from.Send(new PlaySound(0x249, GetWorldLocation()));

                        return true;
                    }

                    return false;
                }
			}

			else			
				return false;			
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ulong Content
		{
			get
			{
				return m_Content;
			}

			set
			{
				if ( m_Content != value )
				{
					m_Content = value;

					m_Count = 0;

					while ( value > 0 )
					{
						m_Count += (int)(value & 0x1);
						value >>= 1;
					}

					InvalidateProperties();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int SpellCount
		{
			get
			{
				return m_Count;
			}
		}

		[Constructable]
		public Spellbook() : this( (ulong)0 )
		{
		}

		[Constructable]
		public Spellbook( ulong content ) : this( content, 0xEFA )
		{
		}

		public Spellbook( ulong content, int itemID ) : base( itemID )
		{	
			Layer = Layer.OneHanded;
			LootType = LootType.Blessed;

			Content = content;

            Weight = 2.0;
		}

		public override void OnAfterDuped( Item newItem )
		{
			Spellbook book = newItem as Spellbook;

			if ( book == null )
				return;
		}

		public override void OnAdded( object parent )
		{			
		}

		public override void OnRemoved( object parent )
		{			
		}

		public bool HasSpell( int spellID )
		{
			spellID -= BookOffset;

			return ( spellID >= 0 && spellID < BookCount && (m_Content & ((ulong)1 << spellID)) != 0 );
		}

		public Spellbook( Serial serial ) : base( serial )
		{
		}

        public void DisplayTo(Mobile to)
        {
            NetState ns = to.NetState;

            if (ns == null)
                return;

            if (Parent == null)            
                to.Send(this.WorldPacket);
            
            else if (Parent is Item)
            {
                if (ns.ContainerGridLines)
                    to.Send(new ContainerContentUpdate6017(this));

                else
                    to.Send(new ContainerContentUpdate(this));
            }

            else if (Parent is Mobile)
                to.Send(new EquipUpdate(this));            

            if (ns.HighSeas)
                to.Send(new DisplaySpellbookHS(this));

            else
                to.Send(new DisplaySpellbook(this));

			if ( ns.ContainerGridLines )             
                to.Send(new SpellbookContent6017(m_Count, BookOffset + 1, m_Content, this));	
		
            else            
                to.Send(new SpellbookContent(m_Count, BookOffset + 1, m_Content, this));
        }

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			if ( m_Quality == BookQuality.Exceptional )
				list.Add( 1063341 ); // exceptional

			list.Add( 1042886, m_Count.ToString() ); // ~1_NUMBERS_OF_SPELLS~ Spells
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );
            
			LabelTo( from, 1042886, m_Count.ToString() );
		}

		public override void OnDoubleClick( Mobile from )
		{
			Container pack = from.Backpack;

			if ( Parent == from || ( pack != null && Parent == pack ) )
				DisplayTo( from );

			else
				from.SendLocalizedMessage( 500207 ); // The spellbook must be in your backpack (and not in a container within) to open.
		}

        public virtual int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            if (makersMark)
                DisplayCrafter = true;

            m_Quality = (BookQuality)(quality - 1);

            return quality;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
			writer.Write((int)m_Quality);
            writer.Write((int)m_SlayerGroup);
			writer.Write(m_Content);
			writer.Write(m_Count);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Quality = (BookQuality)reader.ReadInt();
                m_SlayerGroup = (SlayerGroupType)reader.ReadInt();
                m_Content = reader.ReadULong();
                m_Count = reader.ReadInt();
            }
		}
	}
}