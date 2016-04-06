using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Misc;
using Server.Engines.BulkOrders;
using Server.Regions;
using Server.Factions;
using Server.Engines.Craft;
using Server;

namespace Server.Mobiles
{
	public enum VendorShoeType
	{
		None,
		Shoes,
		Boots,
		Sandals,
		ThighBoots
	}

	public class Budget
	{
		public static readonly TimeSpan TimeFrame = TimeSpan.FromDays( 1.0 );
		public static readonly long MaxBudget = 2000;

		public int CurrentBudget { get; set; }
		public DateTime ResetBudgetAt { get; set; }

		public Budget( int startBudget, DateTime resetBudgetAt )
		{
			CurrentBudget = startBudget;
			ResetBudgetAt = resetBudgetAt;
		}

		public static void WriteBudget( GenericWriter writer, Budget budget )
		{
			writer.Write( (int)budget.CurrentBudget );
			writer.Write( (DateTime)budget.ResetBudgetAt );
		}

		public static Budget ReadBudget( GenericReader reader )
		{
			Budget budget = new Budget( 0, DateTime.UtcNow );

			budget.CurrentBudget = reader.ReadInt();
			budget.ResetBudgetAt = reader.ReadDateTime();

			return budget;
		}
	}

	public abstract class BaseVendor : BaseCreature, IVendor
	{
		private const int MaxSell = 50;

		protected abstract List<SBInfo> SBInfos { get; }

        public override bool HasNormalLoot { get { return false; } }
        public override bool AllowParagon { get { return false; } }

		private Dictionary<Serial, Budget> m_Budgets = new Dictionary<Serial,Budget>();

		private ArrayList m_ArmorBuyInfo = new ArrayList();
		private ArrayList m_ArmorSellInfo = new ArrayList();

		private DateTime m_LastRestock;

		private DateTime m_NextTrickOrTreat;

		public override bool CanTeach { get { return true; } }

		public override bool BardImmune { get { return true; } }

		public override bool PlayerRangeSensitive { get { return true; } }

		public virtual bool IsActiveVendor { get { return true; } }
		public virtual bool IsActiveBuyer { get { return IsActiveVendor; } }
		public virtual bool IsActiveSeller { get { return IsActiveVendor; } }

		public virtual NpcGuild NpcGuild { get { return NpcGuild.None; } }

		public override bool IsInvulnerable { get { return true; } }

		public virtual DateTime NextTrickOrTreat { get { return m_NextTrickOrTreat; } set { m_NextTrickOrTreat = value; } }

		public override bool ShowFameTitle { get { return false; } }

		public virtual bool IsValidBulkOrder( Item item )
		{
			return false;
		}

		public virtual Item CreateBulkOrder( Mobile from, bool fromContextMenu )
		{
			return null;
		}

		public virtual bool SupportsBulkOrders( Mobile from )
		{
			return false;
		}

		public virtual TimeSpan GetNextBulkOrder( Mobile from )
		{
			return TimeSpan.Zero;
		}

		public virtual void OnSuccessfulBulkOrderReceive( Mobile from )
		{
		}

		private class BulkOrderInfoEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private BaseVendor m_Vendor;

			public BulkOrderInfoEntry( Mobile from, BaseVendor vendor ): base( 6152 )
			{
				m_From = from;
				m_Vendor = vendor;
			}

			public override void OnClick()
			{
				if ( m_Vendor.SupportsBulkOrders( m_From ) )
				{
					TimeSpan ts = m_Vendor.GetNextBulkOrder( m_From );

					int totalSeconds = (int)ts.TotalSeconds;
					int totalHours = (totalSeconds + 3599) / 3600;
					int totalMinutes = (totalSeconds + 59) / 60;

					if ( ((Core.SE ) ? totalMinutes == 0 : totalHours == 0) )					
						m_From.SendLocalizedMessage( 1049038 ); // You can get an order now.					

					else
					{
						int oldSpeechHue = m_Vendor.SpeechHue;
						m_Vendor.SpeechHue = 0x3B2;

						m_Vendor.SayTo( m_From, 1049039, totalHours.ToString() ); // An offer may be available in about ~1_hours~ hours.

						m_Vendor.SpeechHue = oldSpeechHue;
					}
				}
			}
		}

		public BaseVendor( string title ): base( AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2 )
		{
			LoadSBInfo();

			Title = title;
			InitBody();
			InitOutfit();

			Container pack;
			
			pack = new Backpack();
			pack.Layer = Layer.ShopBuy;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			pack = new Backpack();
			pack.Layer = Layer.ShopResale;
			pack.Movable = false;
			pack.Visible = false;
			AddItem( pack );

			m_LastRestock = DateTime.UtcNow;
		}

		public BaseVendor( Serial serial ): base( serial )
		{
		}

		public DateTime LastRestock
		{
			get
			{
				return m_LastRestock;
			}
			set
			{
				m_LastRestock = value;
			}
		}

		public virtual TimeSpan RestockDelay
		{
			get
			{				
				return TimeSpan.FromMinutes( 45 );
			}
		}

		public Container BuyPack
		{
			get
			{
				Container pack = FindItemOnLayer( Layer.ShopBuy ) as Container;

				if ( pack == null )
				{
					pack = new Backpack();
					pack.Layer = Layer.ShopBuy;
					pack.Visible = false;
					AddItem( pack );
				}

				return pack;
			}
		}

		public abstract void InitSBInfo();

		public virtual bool IsTokunoVendor { get { return ( Map == Map.Tokuno ); } }

		protected void LoadSBInfo()
		{
			m_LastRestock = DateTime.UtcNow;

			for ( int i = 0; i < m_ArmorBuyInfo.Count; ++i )
			{
				GenericBuyInfo buy = m_ArmorBuyInfo[i] as GenericBuyInfo;

				if ( buy != null )
					buy.DeleteDisplayEntity();
			}

			SBInfos.Clear();

			InitSBInfo();

			m_ArmorBuyInfo.Clear();
			m_ArmorSellInfo.Clear();

			for ( int i = 0; i < SBInfos.Count; i++ )
			{
				SBInfo sbInfo = (SBInfo)SBInfos[i];
				m_ArmorBuyInfo.AddRange( sbInfo.BuyInfo );
				m_ArmorSellInfo.Add( sbInfo.SellInfo );
			}
		}

		public virtual bool GetGender()
		{
			return Utility.RandomBool();
		}

		public virtual void InitBody()
		{
			InitStats( 100, 100, 25 );

			SpeechHue = Utility.RandomDyedHue();
			Hue = Utility.RandomSkinHue();

			if ( IsInvulnerable && !Core.AOS )
				NameHue = 0x35;

			if ( Female = GetGender() )
			{
				Body = 0x191;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 0x190;
				Name = NameList.RandomName( "male" );
			}
		}

		public virtual int GetRandomHue()
		{
			switch ( Utility.Random( 5 ) )
			{
				default:
				case 0: return Utility.RandomBlueHue();
				case 1: return Utility.RandomGreenHue();
				case 2: return Utility.RandomRedHue();
				case 3: return Utility.RandomYellowHue();
				case 4: return Utility.RandomNeutralHue();
			}
		}

		public virtual int GetShoeHue()
		{
			if ( 0.1 > Utility.RandomDouble() )
				return 0;

			return Utility.RandomNeutralHue();
		}

		public virtual VendorShoeType ShoeType
		{
			get { return VendorShoeType.Shoes; }
		}

		public override void OnAfterSpawn()
		{
		}

		protected override void OnMapChange( Map oldMap )
		{
			base.OnMapChange( oldMap );

			LoadSBInfo();
		}

		public virtual void CapitalizeTitle()
		{
			string title = this.Title;

			if ( title == null )
				return;

			string[] split = title.Split( ' ' );

			for ( int i = 0; i < split.Length; ++i )
			{
				if ( Insensitive.Equals( split[i], "the" ) )
					continue;

				if ( split[i].Length > 1 )
					split[i] = Char.ToUpper( split[i][0] ) + split[i].Substring( 1 );

				else if ( split[i].Length > 0 )
					split[i] = Char.ToUpper( split[i][0] ).ToString();
			}

			this.Title = String.Join( " ", split );
		}

		public virtual int GetHairHue()
		{
			return Utility.RandomHairHue();
		}

		public virtual void InitOutfit()
		{
			switch ( Utility.Random( 3 ) )
			{
				case 0: AddItem( new FancyShirt( GetRandomHue() ) ); break;
				case 1: AddItem( new Doublet( GetRandomHue() ) ); break;
				case 2: AddItem( new Shirt( GetRandomHue() ) ); break;
			}

			switch ( ShoeType )
			{
				case VendorShoeType.Shoes: AddItem( new Shoes( GetShoeHue() ) ); break;
				case VendorShoeType.Boots: AddItem( new Boots( GetShoeHue() ) ); break;
				case VendorShoeType.Sandals: AddItem( new Sandals( GetShoeHue() ) ); break;
				case VendorShoeType.ThighBoots: AddItem( new ThighBoots( GetShoeHue() ) ); break;
			}

			int hairHue = GetHairHue();

			Utility.AssignRandomHair( this, hairHue );
			Utility.AssignRandomFacialHair( this, hairHue );

			if ( Female )
			{
				switch ( Utility.Random( 6 ) )
				{
					case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
					case 1:
					case 2: AddItem( new Kilt( GetRandomHue() ) ); break;
					case 3:
					case 4:
					case 5: AddItem( new Skirt( GetRandomHue() ) ); break;
				}
			}

			else
			{
				switch ( Utility.Random( 2 ) )
				{
					case 0: AddItem( new LongPants( GetRandomHue() ) ); break;
					case 1: AddItem( new ShortPants( GetRandomHue() ) ); break;
				}
			}

			PackGold( 100, 200 );
		}

		public virtual void Restock()
		{
			m_LastRestock = DateTime.UtcNow;

			IBuyItemInfo[] buyInfo = this.GetBuyInfo();

			foreach ( IBuyItemInfo bii in buyInfo )
				bii.OnRestock();
		}

		private static TimeSpan InventoryDecayTime = TimeSpan.FromHours( 1.0 );

		public virtual void VendorBuy( Mobile from )
		{
			if ( !IsActiveSeller )
				return;

			if ( !from.CheckAlive() )
				return;

			if ( !CheckVendorAccess( from ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return;
			}

			if ( DateTime.UtcNow - m_LastRestock > RestockDelay )
				Restock();
            
			int count = 0;

            List<BuyItemState> list;
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] sellInfo = this.GetSellInfo();

            list = new List<BuyItemState>(buyInfo.Length);
			Container cont = this.BuyPack;

			List<ObjectPropertyList> opls = null;

			for ( int idx = 0; idx < buyInfo.Length; idx++ )
			{
				IBuyItemInfo buyItem = (IBuyItemInfo)buyInfo[idx];

                if ( buyItem.Amount <= 0 || list.Count >= 250 )
					continue;

				GenericBuyInfo gbi = (GenericBuyInfo)buyItem;
				IEntity disp = gbi.GetDisplayEntity();

				list.Add( new BuyItemState( buyItem.Name, cont.Serial, disp == null ? (Serial)0x7FC0FFEE : disp.Serial, buyItem.Price, buyItem.Amount, buyItem.ItemID, buyItem.Hue ) );
				count++;

				if ( opls == null ) {
					opls = new List<ObjectPropertyList>();
				}

				if ( disp is Item )                 
					opls.Add( ( ( Item ) disp ).PropertyList );
				
                else if ( disp is Mobile )                
					opls.Add( ( ( Mobile ) disp ).PropertyList );				
			}

            if (list.Count > 0)
            {
                list.Sort(new BuyItemStateComparer());

                SendPacksTo(from);

                NetState ns = from.NetState;

                if (ns == null)
                    return;

                if (ns.ContainerGridLines)
                    from.Send(new VendorBuyContent6017(list));

                else
                    from.Send(new VendorBuyContent(list));

                from.Send(new VendorBuyList(this, list));

                if (ns.HighSeas)
                    from.Send(new DisplayBuyListHS(this));

                else
                    from.Send(new DisplayBuyList(this));

                from.Send(new MobileStatusExtended(from));

				if ( opls != null )
                {
					for ( int i = 0; i < opls.Count; ++i ) 
                    {
                        from.Send(opls[i]);
                    }
                }

                SayTo(from, 500186); // Greetings.  Have a look around.
            }
		}

		public virtual void SendPacksTo( Mobile from )
		{
			Item pack = FindItemOnLayer( Layer.ShopBuy );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopBuy;
				pack.Movable = false;
				pack.Visible = false;
				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopSell );

			if ( pack != null )
				from.Send( new EquipUpdate( pack ) );

			pack = FindItemOnLayer( Layer.ShopResale );

			if ( pack == null )
			{
				pack = new Backpack();
				pack.Layer = Layer.ShopResale;
				pack.Movable = false;
				pack.Visible = false;

				AddItem( pack );
			}

			from.Send( new EquipUpdate( pack ) );
		}

		public virtual void VendorSell( Mobile from )
		{
			if ( !IsActiveBuyer )
				return;

			if ( !from.CheckAlive() )
				return;

			if ( !CheckVendorAccess( from ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return;
			}
            
            //TEST: Figure Out Budgets Concept
			if ( m_Budgets.ContainsKey( from.Serial ) )
			{
                /*
				if ( from.AccessLevel >= AccessLevel.GameMaster )
				{
					SayTo( from, true, "Your current budget is {0}, which resets at {1}",
						m_Budgets[from.Serial].CurrentBudget, m_Budgets[from.Serial].ResetBudgetAt );
				}
                */
				
				if ( DateTime.UtcNow < m_Budgets[from.Serial].ResetBudgetAt )
				{
				}
				
				else
				{
					m_Budgets[from.Serial].CurrentBudget = 0;
					m_Budgets[from.Serial].ResetBudgetAt = DateTime.UtcNow + Budget.TimeFrame;
				}
			}

			else
				m_Budgets.Add( from.Serial, new Budget( 0, DateTime.UtcNow + Budget.TimeFrame ) );

			Container pack = from.Backpack;

			if ( pack != null )
			{
				IShopSellInfo[] info = GetSellInfo();
                IBuyItemInfo[] buyinfo = GetBuyInfo();

				Hashtable table = new Hashtable();

                int numItemsNotCraftedBySeller = 0;

				foreach ( IShopSellInfo ssi in info )
				{
					Item[] items = pack.FindItemsByType( ssi.Types );

					foreach ( Item item in items )
					{

                        if (from is PlayerMobile)
                        {
                            PlayerMobile pMobile = (PlayerMobile)from;

                            if (pMobile.ResetItemsNotCraftedByDateTime <= DateTime.UtcNow)                            
                                pMobile.ItemsNotCraftedBySold = 0;
                            
                            if (item.Acquisition == Item.AcquisitionType.Crafted && item.AcquisitionData != from.Serial.Value && !item.Stackable)
                            {
                                if (numItemsNotCraftedBySeller + pMobile.ItemsNotCraftedBySold > 15)
                                    continue;

                                else                                
                                    numItemsNotCraftedBySeller++;
                                
                            }
                        }

						if ( item is Container && ( (Container)item ).Items.Count != 0 )
							continue;

						if ( item.IsStandardLoot() && item.Movable && ssi.IsSellable( item ) )
                        {
                            int price = ssi.GetSellPriceFor( item );

                            for ( int idx = 0; idx < buyinfo.Length; idx++ )
			                {
				                IBuyItemInfo buyItem = (IBuyItemInfo)buyinfo[idx];
                                GenericBuyInfo gbi = (GenericBuyInfo)buyItem;
                                
                                if (item.GetType().IsAssignableFrom(gbi.Type) && !(item is BaseWeapon) && !(item is BaseArmor))
                                {
                                    if (price >= buyItem.Price)                                    
                                        price = buyItem.Price - 1;                                    
                                }
                            }
                            
							table[item] = new SellItemState( item, price, ssi.GetNameFor( item ) );
                        }
					}
				}

				if ( table.Count > 0 )
				{
					SendPacksTo( from );

					from.Send( new VendorSellList( this, table ) );
				}

				else				
					Say( true, "You have nothing I would be interested in." );				
			}
		}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
			if ( dropped is SmallBOD || dropped is LargeBOD )
            {
				PlayerMobile pm = from as PlayerMobile;

                if ( !IsValidBulkOrder( dropped ) )
				{
					SayTo( from, 1045130 ); // That order is for some other shopkeeper.
					return false;
				}

				else if ( (dropped is SmallBOD && !((SmallBOD)dropped).Complete) || (dropped is LargeBOD && !((LargeBOD)dropped).Complete) )
				{
					SayTo( from, 1045131 ); // You have not completed the order yet.
					return false;
				}

				Item reward;
				int gold, fame;

				if ( dropped is SmallBOD )
					((SmallBOD)dropped).GetRewards( out reward, out gold, out fame );

				else
					((LargeBOD)dropped).GetRewards( out reward, out gold, out fame );

				from.SendSound( 0x3D );

				SayTo( from, 1045132 ); // Thank you so much!  Here is a reward for your effort.

				if ( reward != null )
					from.AddToBackpack( reward );

				if ( gold > 1000 )
					from.AddToBackpack( new BankCheck( gold ) );

				else if ( gold > 0 )
					from.AddToBackpack( new Gold( gold ) );

				FameKarmaTitles.AwardFame( from, fame, true );

				OnSuccessfulBulkOrderReceive( from );

				if ( Core.ML && pm != null )
					pm.NextBODTurnInTime = DateTime.UtcNow + TimeSpan.FromSeconds( 10.0 );

				dropped.Delete();

				return true;
			}

			return base.OnDragDrop( from, dropped );
		}

		private GenericBuyInfo LookupDisplayObject( object obj )
		{
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();

			for ( int i = 0; i < buyInfo.Length; ++i ) {
				GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[i];

				if ( gbi.GetDisplayEntity() == obj )
					return gbi;
			}

			return null;
		}

		private void ProcessSinglePurchase( BuyItemResponse buy, IBuyItemInfo bii, List<BuyItemResponse> validBuy, ref int controlSlots, ref bool fullPurchase, ref int totalCost )
		{
			int amount = buy.Amount;

			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount <= 0 )
				return;

			int slots = bii.ControlSlots * amount;

			if ( controlSlots >= slots )			
				controlSlots -= slots;
			
			else
			{
				fullPurchase = false;
				return;
			}

			totalCost += bii.Price * amount;
			validBuy.Add( buy );
		}

		private void ProcessValidPurchase( int amount, IBuyItemInfo bii, Mobile buyer, Container cont, Container bankbox )
		{
			if ( amount > bii.Amount )
				amount = bii.Amount;

			if ( amount < 1 )
				return;

			bii.Amount -= amount;

			IEntity o = bii.GetEntity();

			if ( o is Item )
			{
				Item item = (Item)o;

				if ( item.Stackable )
				{
					item.Amount = amount;

					if (cont == null || !cont.TryDropItem(buyer, item, false))
					{
						if (bankbox.TryDropItem(buyer, item, false))
							buyer.PrivateOverheadMessage(MessageType.Regular, 0x22, true, "Because of overweight your purchase was delivered to your bankbox.", buyer.NetState);
						
                        else
							item.MoveToWorld(buyer.Location, buyer.Map);
					}
				}

				else
				{
					item.Amount = 1;
                    item.Acquisition = Item.AcquisitionType.VendorBought;
                    item.AcquisitionData = bii.Price;

					if (cont == null || !cont.TryDropItem(buyer, item, false))
					{
						if (bankbox.TryDropItem(buyer, item, false))
							buyer.PrivateOverheadMessage(MessageType.Regular, 0x22, true, "Because of overweight your purchase was delivered to your bankbox.", buyer.NetState);
						else
							item.MoveToWorld(buyer.Location, buyer.Map);
					}

					for ( int i = 1; i < amount; i++ )
					{
						if ( item != null )
						{
                            item = bii.GetEntity() as Item;
                            item.Acquisition = Item.AcquisitionType.VendorBought;
                            item.AcquisitionData = bii.Price;

							item.Amount = 1;

							if (cont == null || !cont.TryDropItem(buyer, item, false))
							{
								if (bankbox.TryDropItem(buyer, item, false))
									buyer.PrivateOverheadMessage(MessageType.Regular, 0x22, true, "Because of overweight your purchase was delivered to your bankbox.", buyer.NetState);
								else
									item.MoveToWorld(buyer.Location, buyer.Map);
							}
						}
					}
				}
			}

			else if ( o is Mobile )
			{
				Mobile m = (Mobile)o;

				m.Direction = (Direction)Utility.Random( 8 );
				m.MoveToWorld( buyer.Location, buyer.Map );
				m.PlaySound( m.GetIdleSound() );

				if ( m is BaseCreature )
					( (BaseCreature)m ).SetControlMaster( buyer );

				for ( int i = 1; i < amount; ++i )
				{
					m = bii.GetEntity() as Mobile;

					if ( m != null )
					{
						m.Direction = (Direction)Utility.Random( 8 );
						m.MoveToWorld( buyer.Location, buyer.Map );

						if ( m is BaseCreature )
							( (BaseCreature)m ).SetControlMaster( buyer );
					}
				}
			}
		}

        public virtual bool OnBuyItems(Mobile buyer, List<BuyItemResponse> list)
		{
			if ( !IsActiveSeller )
				return false;

			if ( !buyer.CheckAlive() )
				return false;

			if ( !CheckVendorAccess( buyer ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return false;
			}
            
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			IShopSellInfo[] info = GetSellInfo();
			int totalCost = 0;
            List<BuyItemResponse> validBuy = new List<BuyItemResponse>(list.Count);
			Container cont;
			bool bought = false;
			bool fromBank = false;
			bool fullPurchase = true;
			int controlSlots = buyer.FollowersMax - buyer.Followers;

			foreach ( BuyItemResponse buy in list )
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if ( ser.IsItem )
				{
					Item item = World.FindItem( ser );

					if ( item == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( item );

					if ( gbi != null )					
						ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
					
					else if ( item != this.BuyPack && item.IsChildOf( this.BuyPack ) )
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						if ( amount <= 0 )
							continue;

						foreach ( IShopSellInfo ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									totalCost += ssi.GetBuyPriceFor( item ) * amount;
									validBuy.Add( buy );

									break;
								}
							}
						}
					}
				}

				else if ( ser.IsMobile )
				{
					Mobile mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
				}
			}

			if ( fullPurchase && validBuy.Count == 0 )
				SayTo( buyer, 500190 ); // Thou hast bought nothing!

			else if ( validBuy.Count == 0 )
				SayTo( buyer, 500187 ); // Your order cannot be fulfilled, please try again.

			if ( validBuy.Count == 0 )
				return false;

			bought = ( buyer.AccessLevel >= AccessLevel.GameMaster );

			cont = buyer.Backpack;

			if ( !bought && cont != null )
			{
				if ( cont.ConsumeTotal( typeof( Gold ), totalCost ) )
					bought = true;				
			}

			if ( !bought)
			{
				cont = buyer.FindBankNoCreate();

				if ( cont != null && cont.ConsumeTotal( typeof( Gold ), totalCost ) )
				{
					bought = true;
					fromBank = true;
				}

				else				
					SayTo( buyer, 500191 ); //Begging thy pardon, but thy bank account lacks these funds.				
			}

			if ( !bought )
				return false;

			else
				buyer.PlaySound( 0x32 );

			cont = buyer.Backpack;

			if ( cont == null )
				cont = buyer.BankBox;

			Container bankbox = buyer.BankBox;

			foreach ( BuyItemResponse buy in validBuy )
			{
				Serial ser = buy.Serial;
				int amount = buy.Amount;

				if ( amount < 1 )
					continue;

				if ( ser.IsItem )
				{
					Item item = World.FindItem( ser );

					if ( item == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( item );

					if ( gbi != null )					
						ProcessValidPurchase( amount, gbi, buyer, cont, bankbox );
					
					else
					{
						if ( amount > item.Amount )
							amount = item.Amount;

						foreach ( IShopSellInfo ssi in info )
						{
							if ( ssi.IsSellable( item ) )
							{
								if ( ssi.IsResellable( item ) )
								{
									Item buyItem;

									if ( amount >= item.Amount )									
										buyItem = item;
									
									else
									{
										buyItem = Mobile.LiftItemDupe( item, item.Amount - amount );

										if ( buyItem == null )
											buyItem = item;
									}

									if (cont == null || !cont.TryDropItem(buyer, buyItem, false))
									{
										if (bankbox.TryDropItem(buyer, buyItem, false))
											buyer.PrivateOverheadMessage(MessageType.Regular, 0x22, true, "Because of overweight your purchase was delivered to your bankbox.", buyer.NetState);
										
                                        else
											buyItem.MoveToWorld(buyer.Location, buyer.Map);
									}

									break;
								}
							}
						}
					}
				}

				else if ( ser.IsMobile )
				{
					Mobile mob = World.FindMobile( ser );

					if ( mob == null )
						continue;

					GenericBuyInfo gbi = LookupDisplayObject( mob );

					if ( gbi != null )
						ProcessValidPurchase( amount, gbi, buyer, cont, bankbox );
				}
			}

			if ( fullPurchase )
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Here are the goods you requested." );
				
                else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.", totalCost );
				
                else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.", totalCost );
            }

			else
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, true, "I would not presume to charge thee anything.  Unfortunately, I could not sell you all the goods you requested." );
				
                else if ( fromBank )
					SayTo( buyer, true, "The total of thy purchase is {0} gold, which has been withdrawn from your bank account.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
				
                else
					SayTo( buyer, true, "The total of thy purchase is {0} gold.  My thanks for the patronage.  Unfortunately, I could not sell you all the goods you requested.", totalCost );
			}

            //Player Enhancement Customization: Customer Loyalty
            bool customerLoyalty = PlayerEnhancementPersistance.IsCustomizationEntryActive(buyer, Custom.CustomizationType.CustomerLoyalty);

            if (customerLoyalty)
                CustomizationAbilities.CustomerLoyalty(this);

			return true;
		}

		public virtual bool CheckVendorAccess( Mobile from )
		{
			GuardedRegion reg = (GuardedRegion)this.Region.GetRegion( typeof( GuardedRegion ) );

			if ( reg != null && !reg.CheckVendorAccess( this, from ) )
				return false;

			if ( this.Region != from.Region )
			{
				reg = (GuardedRegion)from.Region.GetRegion( typeof( GuardedRegion ) );

				if ( reg != null && !reg.CheckVendorAccess( this, from ) )
					return false;
			}

			return true;
		}

        public virtual bool OnSellItems(Mobile seller, List<SellItemResponse> list)
		{
			if ( !IsActiveBuyer )
				return false;

			if ( !seller.CheckAlive() )
				return false;

			if ( !CheckVendorAccess( seller ) )
			{
				Say( 501522 ); // I shall not treat with scum like thee!
				return false;
			}

			seller.PlaySound( 0x32 );

			IShopSellInfo[] info = GetSellInfo();
			IBuyItemInfo[] buyInfo = this.GetBuyInfo();
			int GiveGold = 0;
			int Sold = 0;
			Container cont;
			ArrayList delete = new ArrayList();
			ArrayList drop = new ArrayList();

			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				foreach ( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						Sold++;
						break;
					}
				}
			}

			if ( Sold > MaxSell )
			{
				SayTo( seller, true, "You may only sell {0} items at a time!", MaxSell );
				return false;
			}

			else if ( Sold == 0 )			
				return true;			

            int TotalPrice = 0;
			int TotalNonLootedItemPrice = 0;

            foreach ( SellItemResponse resp in list )
			{
				bool crafted_or_bought = (resp.Item.Acquisition == Item.AcquisitionType.Crafted || resp.Item.Acquisition == Item.AcquisitionType.VendorBought);
				
                if ( !crafted_or_bought || resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				foreach (IShopSellInfo ssi in info)
				{
					if (ssi.IsSellable(resp.Item))					
						TotalPrice += ssi.GetSellPriceFor(resp.Item) * resp.Amount;
				}
            }

            if (!Custom.SellingPersistance.PlayerRequestSell(seller, TotalPrice))
            {
                SayTo(seller, true, "I can't afford anything else right now!");
                return false;
            }
            
			foreach ( SellItemResponse resp in list )
			{
				if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !resp.Item.IsStandardLoot() || !resp.Item.Movable || ( resp.Item is Container && ( (Container)resp.Item ).Items.Count != 0 ) )
					continue;

				foreach ( IShopSellInfo ssi in info )
				{
					if ( ssi.IsSellable( resp.Item ) )
					{
						int amount = resp.Amount;

						if ( amount > resp.Item.Amount )
							amount = resp.Item.Amount;

						if ( ssi.IsResellable( resp.Item ) )
						{
							bool found = false;

							foreach ( IBuyItemInfo bii in buyInfo )
							{
								if ( bii.Restock( resp.Item, amount ) )
								{
									resp.Item.Consume( amount );
									found = true;

									break;
								}
							}

							if ( !found )
							{
								cont = this.BuyPack;

								if ( amount < resp.Item.Amount )
								{
									Item item = Mobile.LiftItemDupe( resp.Item, resp.Item.Amount - amount );

									if ( item != null )
									{
										item.SetLastMoved();
										cont.DropItem( item );
									}

									else
									{
										resp.Item.SetLastMoved();
										cont.DropItem( resp.Item );
									}
								}

								else
								{
									resp.Item.SetLastMoved();
									cont.DropItem( resp.Item );
								}
							}
						}

						else
						{
							if ( amount < resp.Item.Amount )
								resp.Item.Amount -= amount;

							else
								resp.Item.Delete();
						}

						if (ssi.IsSellable(resp.Item))
						{
							if (resp.Item.Acquisition == Item.AcquisitionType.VendorBought && !resp.Item.Stackable)
								GiveGold += (int)(resp.Item.AcquisitionData * .40) * resp.Amount;

							else
								GiveGold += ssi.GetSellPriceFor(resp.Item) * resp.Amount;
						}

                        if (seller is PlayerMobile && !resp.Item.Stackable)
                            if (resp.Item.Acquisition == Item.AcquisitionType.Crafted && resp.Item.AcquisitionData != seller.Serial.Value)
                            {
                                PlayerMobile pMobile = (PlayerMobile)seller;

                                if (pMobile.ItemsNotCraftedBySold == 0)
                                    pMobile.ResetItemsNotCraftedByDateTime = DateTime.UtcNow + TimeSpan.FromDays(1);
                                
                                pMobile.ItemsNotCraftedBySold++;
                            }
                        
						break;
					}
				}
			}

			if ( GiveGold > 0 )
			{
                if (m_Budgets.ContainsKey(seller.Serial))
                    m_Budgets[seller.Serial].CurrentBudget += GiveGold;

                else
                    m_Budgets.Add(seller.Serial, new Budget(GiveGold, DateTime.UtcNow + Budget.TimeFrame));

				while ( GiveGold > 1000000 )
				{
					seller.AddToBackpack( new BankCheck( 1000000 ) );
					GiveGold -= 1000000;
				}

                if ( GiveGold > 10000 )
                    seller.AddToBackpack( new BankCheck( GiveGold ) );

                else
				    seller.AddToBackpack( new Gold( GiveGold ) );

				seller.PlaySound( 0x0037 );

				if ( SupportsBulkOrders( seller ) )
				{
					Item bulkOrder = CreateBulkOrder( seller, false );

                    var pmSeller = seller as PlayerMobile;

                    if (!pmSeller.HarvestLockedout)                    
                        pmSeller.TempStashedHarvest = bulkOrder;                                        
				}
			}

			return true;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)2 ); // version

			writer.Write( (int)m_Budgets.Count );
			foreach ( var keyValue in m_Budgets )
			{
				writer.Write( (int)keyValue.Key );
				Budget.WriteBudget( writer, keyValue.Value );
			}

			List<SBInfo> sbInfos = this.SBInfos;

			for ( int i = 0; sbInfos != null && i < sbInfos.Count; ++i )
			{
				SBInfo sbInfo = sbInfos[i];
				List<GenericBuyInfo> buyInfo = sbInfo.BuyInfo;

				for ( int j = 0; buyInfo != null && j < buyInfo.Count; ++j )
				{
					GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[j];

					int maxAmount = gbi.MaxAmount;
					int doubled = 0;

					switch ( maxAmount )
					{
						case  40: doubled = 1; break;
						case  80: doubled = 2; break;
						case 160: doubled = 3; break;
						case 320: doubled = 4; break;
						case 640: doubled = 5; break;
						case 999: doubled = 6; break;
					}

					if ( doubled > 0 )
					{
						writer.WriteEncodedInt( 1 + ( ( j * sbInfos.Count ) + i ) );
						writer.WriteEncodedInt( doubled );
					}
				}
			}

			writer.WriteEncodedInt( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			LoadSBInfo();

			List<SBInfo> sbInfos = this.SBInfos;

			switch ( version )
			{				
				case 2:
				{
					int count = reader.ReadInt();
					for ( int i = 0; i < count; ++i )
					{
						Serial serial = (Serial)reader.ReadInt();
						Budget budget = Budget.ReadBudget( reader );

						m_Budgets.Add( serial, budget );
					}

					goto case 1;
				}

				case 1:
				{
					int index;

					while ( ( index = reader.ReadEncodedInt() ) > 0 )
					{
						int doubled = reader.ReadEncodedInt();

						if ( sbInfos != null )
						{
							index -= 1;
							int sbInfoIndex = index % sbInfos.Count;
							int buyInfoIndex = index / sbInfos.Count;

							if ( sbInfoIndex >= 0 && sbInfoIndex < sbInfos.Count )
							{
								SBInfo sbInfo = sbInfos[sbInfoIndex];
								List<GenericBuyInfo> buyInfo = sbInfo.BuyInfo;

								if ( buyInfo != null && buyInfoIndex >= 0 && buyInfoIndex < buyInfo.Count )
								{
									GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[buyInfoIndex];

									int amount = 20;

									switch ( doubled )
									{
										case 1: amount = 40; break;
										case 2: amount = 80; break;
										case 3: amount = 160; break;
										case 4: amount = 320; break;
										case 5: amount = 640; break;
										case 6: amount = 999; break;
									}

									gbi.Amount = gbi.MaxAmount = amount;
								}
							}
						}
					}

					break;
				}
			}

			if ( IsParagon )
				IsParagon = false;
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && IsActiveVendor )
			{				
				if ( IsActiveSeller )
					list.Add( new VendorBuyEntry( from, this ) );

				if ( IsActiveBuyer )
					list.Add( new VendorSellEntry( from, this ) );
			}

			base.AddCustomContextEntries( from, list );
		}

		public virtual IShopSellInfo[] GetSellInfo()
		{
			return (IShopSellInfo[])m_ArmorSellInfo.ToArray( typeof( IShopSellInfo ) );
		}

		public virtual IBuyItemInfo[] GetBuyInfo()
		{
			return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );
		}

		public override bool CanBeDamaged()
		{
			return !IsInvulnerable;
		}
	}
}

namespace Server.ContextMenus
{
	public class VendorBuyEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorBuyEntry( Mobile from, BaseVendor vendor ): base( 6103, 8 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorBuy( this.Owner.From );
		}
	}

	public class VendorSellEntry : ContextMenuEntry
	{
		private BaseVendor m_Vendor;

		public VendorSellEntry( Mobile from, BaseVendor vendor ): base( 6104, 8 )
		{
			m_Vendor = vendor;
			Enabled = vendor.CheckVendorAccess( from );
		}

		public override void OnClick()
		{
			m_Vendor.VendorSell( this.Owner.From );
		}
	}
}

namespace Server
{
	public interface IShopSellInfo
	{
		string GetNameFor( Item item );
		int GetSellPriceFor( Item item );
		int GetBuyPriceFor( Item item );
		bool IsSellable( Item item );

		Type[] Types { get; }

		bool IsResellable( Item item );
	}

	public interface IBuyItemInfo
	{
		IEntity GetEntity();
		int ControlSlots { get; }
		int PriceScalar { get; set; }
		int Price { get; }
		string Name { get; }
		int Hue { get; }
		int ItemID { get; }
		int Amount { get; set; }
		int MaxAmount { get; }
		bool Restock( Item item, int amount );

		void OnRestock();
	}
}