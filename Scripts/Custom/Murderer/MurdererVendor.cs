using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Custom.Items.Totem;

namespace Server
{
    public class MurdererVendor : PlayerClassVendor
    {
        public override PlayerClass m_PlayerClass { get { return PlayerClass.Murderer; } }

        public override string m_PlayerClassName { get { return PlayerClassPersistance.MurdererClassName; } }

        public override Type m_CurrencyType { get { return PlayerClassPersistance.MurdererCurrency; } }
        public virtual string m_CurrencyName { get { return "dread coin"; } }
        public override int m_CurrencyItemId { get { return PlayerClassPersistance.MurdererCurrencyItemId; } }
        public override int m_CurrencyItemHue { get { return PlayerClassPersistance.MurdererCurrencyItemHue; } }

        public override int m_GumpBackgroundId { get { return PlayerClassPersistance.MurdererGumpBackgroundId; } }

        [Constructable]
        public MurdererVendor(): base()
        {
            SpeechHue = PlayerClassPersistance.MurdererCurrencyItemHue;
            Hue = Utility.RandomSkinHue();

            Title = "the murderous goods dealer";
           
            AddItem(new DreadCloak());
            AddItem(new DreadSash());
            AddItem(new DreadPlateArms());
            AddItem(new DreadPlateChest());
            AddItem(new DreadPlateGloves());
            AddItem(new DreadPlateGorget());
            AddItem(new DreadPlateLegs());

            AddItem(new Kryss() { Movable = false, Hue = PlayerClassPersistance.MurdererItemHue });
            AddItem(new DreadLightShield());

            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);
        }

        public MurdererVendor(Serial serial): base(serial)
        {
        }

        public override List<PlayerClassItemVendorEntry> GetVendorItems(Mobile playerFrom)
        {
            List<PlayerClassItemVendorEntry> m_VendorItems = new List<PlayerClassItemVendorEntry>();

            //Ransomed Items
            PlayerMobile player = playerFrom as PlayerMobile;

            if (player == null)
                return null;

            foreach (PlayerClassItemRansomEntry ransomItemEntry in player.m_PlayerClassRansomedItemsAvailable)
            {
                if (ransomItemEntry == null)
                    continue;

                if (ransomItemEntry.m_ItemType == null)
                    continue;

                Item item = (Item)Activator.CreateInstance(ransomItemEntry.m_ItemType);

                if (item.PlayerClass == m_PlayerClass)
                {
                    PlayerClassItemVendorEntry vendorItemEntry = new PlayerClassItemVendorEntry(item.GetType(), true);
                    vendorItemEntry.m_Cost = ransomItemEntry.m_GoldCost;

                    m_VendorItems.Add(vendorItemEntry);
                }

                item.Delete();
            }
            
            //Non-Blessed
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(OrbOfDreadSight), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadWeaponOil), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadSpellbookDye), false));  
          
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadPlateChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadFemalePlateChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadChainChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadRingmailChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadStuddedChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadFemaleMageChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageBustier), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadPlateLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadChainLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadRingmailLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadStuddedLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageShorts), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageSkirt), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadPlateArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadRingmailArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadStuddedArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageArms), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadPlateHelm), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadChainCoif), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageHat), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadPlateGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadRingmailGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadStuddedGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageGloves), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadPlateGorget), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadStuddedGorget), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadMageGorget), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadHeavyShield), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadLightShield), false));

            //Blessed
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadNecklaceOfWoe), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadRingOfMalevolence), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadTotem), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadDoublet), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadFancyDress), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadLongPants), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadShirt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadShortPants), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadSurcoat), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadTunic), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadCloak), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadSash), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadKilt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadSkirt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadSandals), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadBoots), false));            

            //Titles
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(MurdererRank1TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(MurdererRank2TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(MurdererRank3TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(MurdererRank4TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(MurdererRank5TitleDeed), false));

            //MISC
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadTrophyStone), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadRunebookDyeTub), false)); 

            //Luthius Expansion
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadDresser), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(ShadowBannerEastAddonDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(ShadowBannerNorthAddonDeed), false)); 

            //-----

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(DreadCarpet), false)); 

            return m_VendorItems;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}