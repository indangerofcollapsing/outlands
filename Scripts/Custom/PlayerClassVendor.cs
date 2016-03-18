using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Custom.Paladin;

namespace Server.Mobiles
{
	public class PlayerClassVendor : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; }}
        public override bool HandlesOnSpeech(Mobile from) { return true; }

        public virtual PlayerClass m_PlayerClass { get { return PlayerClass.None; } }
        public virtual string m_PlayerClassName { get { return PlayerClassPersistance.NoClassName; ; } }

        public virtual Type m_CurrencyType { get { return PlayerClassPersistance.NoClassCurrency; } }
        public virtual string m_CurrencyName { get { return "gold"; } }
        public virtual int m_CurrencyItemId { get { return PlayerClassPersistance.NoClassCurrencyItemId; } }
        public virtual int m_CurrencyItemHue { get { return PlayerClassPersistance.NoClassCurrencyItemHue; } }

        public virtual int m_GumpBackgroundId { get { return PlayerClassPersistance.NoClassGumpBackgroundId; } }

		[Constructable]
		public PlayerClassVendor() : base("")
		{   
            SpeechHue = Utility.RandomDyedHue();            
            Hue = Utility.RandomSkinHue();            

            Title = "the playerclass vendor";

            Item legs = FindItemOnLayer(Layer.InnerLegs);

            if (legs != null)
                legs.Delete();

            Item shoes = FindItemOnLayer(Layer.Shoes);

            if (shoes != null)
                shoes.Delete();  

            Blessed = true;
            Frozen = true;
            Direction = Direction.South;

            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);
		}

        public virtual List<PlayerClassItemVendorEntry> GetVendorItems(Mobile playerFor)
        {
            List<PlayerClassItemVendorEntry> m_VendorItems = new List<PlayerClassItemVendorEntry>();  

            return m_VendorItems;            
        }

        public class PlayerClassItemVendorEntry
        {
            public Type m_Type;

            public string m_Name;
            public int m_Cost;
            public int m_ItemID;
            public int m_ItemHue;
            public bool m_IsRansomItem;

            [Constructable]
            public PlayerClassItemVendorEntry(Type type, bool isRansomItem)
            {
                Item item = (Item)Activator.CreateInstance(type);

                m_Type = type;

                m_Name = item.Name;
                m_Cost = item.PlayerClassCurrencyValue;
                m_ItemID = item.ItemID;
                m_ItemHue = item.Hue;
                m_IsRansomItem = isRansomItem;

                item.Delete();
            }
        }

        public class BuyEntry: ContextMenuEntry
        {
            private PlayerClassVendor m_Vendor;
            private Mobile m_from;

            public BuyEntry(Mobile vendor, Mobile from): base(6103, 8)
            {
                m_Vendor = vendor as PlayerClassVendor;               
                m_from = from;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (m_Vendor == null)
                    return;

                if (from.CheckAlive() && from is PlayerMobile)
                {
                    if (PlayerClassPersistance.IsPlayerClass(from, m_Vendor.m_PlayerClass))
                    {
                        from.CloseGump(typeof(PlayerClassVendorGump));
                        from.SendGump(new PlayerClassVendorGump(m_Vendor, 1, m_Vendor.m_GumpBackgroundId, m_Vendor.m_PlayerClass, m_Vendor.m_CurrencyType, m_Vendor.m_CurrencyItemId, m_Vendor.m_CurrencyItemHue, m_Vendor.GetVendorItems(from)));
                    }
                    else
                        from.SendMessage("You must be a " + m_Vendor.m_PlayerClassName + " to purchase these goods.");
                }
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            if (e.Handled || !from.Alive || from.GetDistanceToSqrt(this) > 3)
                return;

            if (e.HasKeyword(0x3C) || (e.HasKeyword(0x171) && WasNamed(e.Speech))) // vendor buy, *buy*
            {
                if (PlayerClassPersistance.IsPlayerClass(from, m_PlayerClass))
                {
                    PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, 503208); //Have a look at my goods.
                    from.SendGump(new PlayerClassVendorGump(this, 1, m_GumpBackgroundId, m_PlayerClass, m_CurrencyType, m_CurrencyItemId, m_CurrencyItemHue, GetVendorItems(from)));
                }

                else
                    PublicOverheadMessage(Network.MessageType.Regular, SpeechHue, true, "You must be a " + m_PlayerClassName + " to purchase these goods.");
            }

            base.OnSpeech(e);
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;

            if (dropped.PlayerClass == m_PlayerClass)
            {
                //Refund
                if (dropped.PlayerClassOwner == from)
                {
                    int itemValue = dropped.PlayerClassCurrencyValue;

                    if (Banker.DepositUniqueCurrency(pm_From, m_CurrencyType, itemValue))
                    {
                        Platinum coinPile = new Platinum(itemValue);
                        pm_From.SendSound(coinPile.GetDropSound());
                        coinPile.Delete();

                        Say("We shall find another owner for this then.");
                        pm_From.SendMessage("You have been refunded " + itemValue.ToString() + " " + m_CurrencyName + " for this item and it has been placed in your bankbox.");

                        dropped.Delete();

                        return true;
                    }

                    else
                    {
                        pm_From.SendMessage("You would not have space in your bankbox for the " + m_CurrencyName + " this item would be worth.");
                        return false;
                    }
                }

                //Ransom
                else if (dropped.PlayerClassOwner != null)
                {
                    PlayerMobile pm_ItemOwner = dropped.PlayerClassOwner as PlayerMobile;

                    if (pm_ItemOwner == null)
                    {
                        pm_From.SendMessage("The original owner of this item can no longer be found in these lands.");
                        return false;
                    }

                    if (pm_ItemOwner.Deleted)
                    {
                        pm_From.SendMessage("The original owner of this item can no longer be found in these lands.");
                        return false;
                    }

                    pm_From.CloseAllGumps();
                    pm_From.SendGump(new RansomEntryGump(this, pm_From, dropped));
                }
            }

            else if (dropped.PlayerClass != PlayerClass.None)
                Say("I have no interest in that particular item. Perhaps you should seek another individual who specializes in that particular line of work.");

            return base.OnDragDrop(from, dropped);
        }

        public override void InitOutfit()
        {
        }

        public override void AddCustomContextEntries(Mobile from, System.Collections.Generic.List<ContextMenus.ContextMenuEntry> list)
        {
            list.Add(new BuyEntry(this, from));
        }

		public override void InitSBInfo()
		{
		}

		public PlayerClassVendor( Serial serial ) : base( serial )
		{
		}

        public bool WasNamed( string speech )
		{
			return this.Name != null && Insensitive.StartsWith( speech, this.Name );
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