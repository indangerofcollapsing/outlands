using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Custom.Items.Totem;

namespace Server.Mobiles
{
    public class PaladinVendor : PlayerClassVendor
    {
        public override PlayerClass m_PlayerClass { get { return PlayerClass.Paladin; } }

        public override string m_PlayerClassName { get { return PlayerClassPersistance.PaladinClassName; } }

        public override Type m_CurrencyType { get { return PlayerClassPersistance.PaladinCurrency; } }
        public virtual string m_CurrencyName { get { return "platinum"; } }
        public override int m_CurrencyItemId { get { return PlayerClassPersistance.PaladinCurrencyItemId; } }
        public override int m_CurrencyItemHue { get { return PlayerClassPersistance.PaladinCurrencyItemHue; } }

        public override int m_GumpBackgroundId { get { return PlayerClassPersistance.PaladinGumpBackgroundId; } }

        [Constructable]
        public PaladinVendor(): base()
        {
            SpeechHue = PlayerClassPersistance.PaladinCurrencyItemHue;
            Hue = Utility.RandomSkinHue();

            Title = "the paladin quartermaster";                      
            
            AddItem(new PaladinCloak());
            AddItem(new PaladinSash());
            AddItem(new PaladinPlateArms());
            AddItem(new PaladinPlateChest());
            AddItem(new PaladinPlateGloves());
            AddItem(new PaladinPlateGorget());
            AddItem(new PaladinPlateLegs());

            AddItem(new VikingSword() { Movable = false, Hue = PlayerClassPersistance.PaladinItemHue });
            AddItem(new PaladinHeavyShield());

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: AddItem(new Skirt()); break;
                case 2: AddItem(new Kilt()); break;
                case 3: break;
            }
        }

        public PaladinVendor(Serial serial): base(serial)
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
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(OrbOfHolySight), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(HolyWeaponOil), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(HolySpellbookDye), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinPlateChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinFemalePlateChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinChainChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRingmailChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinStuddedChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinFemaleMageChest), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageBustier), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinPlateLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinChainLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRingmailLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinStuddedLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageLegs), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageShorts), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageSkirt), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinPlateArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRingmailArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinStuddedArms), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageArms), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinPlateHelm), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinChainCoif), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageHat), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinPlateGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRingmailGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinStuddedGloves), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageGloves), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinPlateGorget), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinStuddedGorget), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinMageGorget), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinHeavyShield), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinLightShield), false));
            
            //Blessed
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinNecklaceOfHonor), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRingOfCourage), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinTotem), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinDoublet), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinFancyDress), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinLongPants), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinShirt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinShortPants), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinSurcoat), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinTunic), false));

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinCloak), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinSash), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinKilt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinSkirt), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinSandals), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinBoots), false));

            //Titles
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRank1TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRank2TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRank3TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRank4TitleDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRank5TitleDeed), false));            

            //Misc
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinTrophyStone), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinRunebookDyeTub), false));

            //Luthius Expansion            
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinDresser), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(HorseBardingEastAddonDeed), false));
            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(HorseBardingNorthAddonDeed), false)); 

            //-----

            m_VendorItems.Add(new PlayerClassItemVendorEntry(typeof(PaladinCarpet), false)); 

            return m_VendorItems;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return false;                        

            if (dropped is Head)
            {
                Head head = dropped as Head;

                if (head.Owner != null)
                {
                    PlayerMobile pm_Head = head.Owner as PlayerMobile;

                    if (pm_Head != null)
                    {
                        if (pm_Head.Murderer)
                        {
                            if (pm_From.Paladin)
                            {
                                Say("Thou art already a paladin amongst our ranks.");
                                return false;
                            }

                            if (pm_From.Murderer)
                            {
                                Say("We do not allow murderers among our ranks!");
                                return false;
                            }

                            if (pm_From.NpcGuild == NpcGuild.DetectivesGuild)
                            {
                                Say("Thou must resign from thy Detective's duty first.");
                                return false;
                            }

                            if (pm_From.NpcGuild == NpcGuild.ThievesGuild)
                            {
                                Say("We do not allow members of the Thieves Guild among our ranks.");
                                return false;
                            }

                            if (pm_From.PaladinRejoinAllowed > DateTime.UtcNow)
                            {
                                Say("Words of your unlawful actions have reached our ears and we will not accept thee into our ranks at this time.");

                                int days;
                                int hours;
                                int minutes;
                                int seconds;

                                string sTime = "";

                                days = pm_From.PaladinRejoinAllowed.Subtract(DateTime.UtcNow).Days;
                                hours = pm_From.PaladinRejoinAllowed.Subtract(DateTime.UtcNow).Hours;
                                minutes = pm_From.PaladinRejoinAllowed.Subtract(DateTime.UtcNow).Minutes;

                                if (minutes >= 60)
                                    hours++;

                                if (hours >= 24)
                                    days++;

                                sTime = "";

                                if (days > 1)
                                    sTime += days.ToString() + " days ";
                                else if (days == 1)
                                    sTime += days.ToString() + " day ";

                                if (hours > 1)
                                    sTime += hours.ToString() + " hours ";
                                else if (hours == 1)
                                    sTime += hours.ToString() + " hour ";

                                if (minutes > 1)
                                    sTime += minutes.ToString() + " minutes ";
                                else if (minutes == 1)
                                    sTime += minutes.ToString() + " minute ";

                                sTime = sTime.Trim();

                                if (sTime != "")
                                    SendMessage("You may rejoin the order in " + sTime + ".");

                                return false;
                            }

                            Say("We welcome thee with open arms into the Order of the Shining Serpent! Fight with bravery and do not bring dishonor to our name.");

                            pm_From.Paladin = true;
                            pm_From.FixedEffect(0x373A, 10, 30);
                            pm_From.PlaySound(0x5A7);

                            head.Delete();

                            Direction = GetDirectionTo(pm_From);

                            Timer.DelayCall(TimeSpan.FromSeconds(1.0), delegate
                            {
                                if (this != null && pm_From != null)
                                {
                                    if (!Deleted && !pm_From.Deleted)
                                    {
                                        Animate(33, 5, 1, true, false, 0);

                                        pm_From.PlaySound(0x5CA);
                                        Effects.SendLocationParticles(EffectItem.Create(pm_From.Location, pm_From.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 1150, 0, 5029, 0);
                                    }
                                }
                            });
                        }

                        else
                        {
                            Say("That individual does not appear to be a murderer. Best dispose of it quickly lest the guards catch thee with it.");

                            return false;
                        }
                    }

                    else
                    {
                        Say("That individual does not appear to be a murderer. Best dispose of it quickly lest the guards catch thee with it.");

                        return false;
                    }
                }

                else
                {
                    Say("That individual does not appear to be a murderer. Best dispose of it quickly lest the guards catch thee with it.");

                    return false;
                }
            }            

            return base.OnDragDrop(from, dropped);
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