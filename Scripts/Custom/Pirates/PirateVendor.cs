using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Custom.Items.Totem;

namespace Server.Mobiles
{
    public class PirateVendor : PlayerClassVendor
    {
        public override PlayerClass m_PlayerClass { get { return PlayerClass.Pirate; } }

        public override string m_PlayerClassName { get { return PlayerClassPersistance.PirateClassName; } }

        public override Type m_CurrencyType { get { return PlayerClassPersistance.PirateCurrency; } }
        public virtual string m_CurrencyName { get { return "doubloons"; } }
        public override int m_CurrencyItemId { get { return PlayerClassPersistance.PirateCurrencyItemId; } }
        public override int m_CurrencyItemHue { get { return PlayerClassPersistance.PirateCurrencyItemHue; } }

        public override int m_GumpBackgroundId { get { return PlayerClassPersistance.PirateGumpBackgroundId; } }

        [Constructable]
        public PirateVendor(): base()
        {
            SpeechHue = PlayerClassPersistance.PirateCurrencyItemHue;
            Hue = Utility.RandomSkinHue();

            Title = "the pirate goods dealer";

            AddItem(new PirateTricorneHat());
            AddItem(new PirateCloak());
            AddItem(new PirateSash());   
            AddItem(new PirateMageChest());
            AddItem(new PirateMageArms());
            AddItem(new PirateMageGloves());
            AddItem(new PirateMageGorget());
            AddItem(new PirateMageLegs());
            AddItem(new PirateBoots());

            AddItem(new Cutlass() { Movable = false, Hue = PlayerClassPersistance.PirateItemHue });
            AddItem(new PirateLightShield());

            int hairHue = GetHairHue();

            Utility.AssignRandomHair(this, hairHue);
            Utility.AssignRandomFacialHair(this, hairHue);
        }

        public PirateVendor(Serial serial): base(serial)
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
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSpyglass), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateWeaponOil),false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSpellbookDye), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PiratePlateChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateFemalePlateChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateChainChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRingmailChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateStuddedChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateFemaleMageChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageBustier), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PiratePlateLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateChainLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRingmailLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateStuddedLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageLegs),false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageShorts), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageSkirt), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PiratePlateArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRingmailArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateStuddedArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageArms), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PiratePlateHelm), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateChainCoif), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageHat), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PiratePlateGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRingmailGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateStuddedGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageGloves), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PiratePlateGorget), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateStuddedGorget), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateMageGorget), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateHeavyShield), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateLightShield), false));            

            //Blessed            
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateNecklaceOfAmbition), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRingofDaring), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateTotem), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(OceansBountyFishingPole), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateDoublet), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateFancyDress), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateLongPants), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateShirt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateShortPants), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSurcoat), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateTunic), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateCloak), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSash), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateKilt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSkirt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSandals),false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateBoots),  false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateTricorneHat),  false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateSkullcap),  false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateBandana), false));

            //Buildables
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(Custom.Pirates.PirateStoneDeed), false));

            //Titles
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRank1TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRank2TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRank3TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRank4TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRank5TitleDeed), false));   
         
            //Misc
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateRunebookDyeTub), false));
            
            //Luthius Expansion            
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PlayerLandCannon), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateFootlocker), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(JollyRogerAddonDeed), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PirateCarpet), false)); 
  
            //-----

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