using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class BaseBoatCannonMetalUpgradeDeed : BaseBoatUpgradeDeed
    {        
        public override UpgradeType UpgradeType { get { return UpgradeType.CannonMetal; } }
        public override int DoubloonCost { get { return 750; } }        

        public virtual int CannonHue { get { return 0; } }  

        [Constructable]
        public BaseBoatCannonMetalUpgradeDeed(): base()
        {
            Name = "a ship cannon metal upgrade deed";
        }

        public BaseBoatCannonMetalUpgradeDeed(Serial serial): base(serial)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            from.CloseAllGumps();

            from.SendMessage("Target the ship deed or land cannon you wish to install this upgrade onto.");
            from.Target = new UpgradeDeedTarget(this);
        }

        public override void InstallUpgrade(BaseBoatDeed boatDeed, BaseBoatUpgradeDeed upgradeDeed, Mobile mobile)
        {
            base.InstallUpgrade(boatDeed, upgradeDeed, mobile);

            BaseBoatCannonMetalUpgradeDeed deed = upgradeDeed as BaseBoatCannonMetalUpgradeDeed;

            if (deed != null && boatDeed != null)
            {
                for (int a = 0; a < boatDeed.m_Upgrades.Count; a++)
                {
                    if (boatDeed.m_Upgrades[a] is BaseBoatCannonMetalUpgradeDeed)
                    {
                        boatDeed.m_Upgrades[a].Delete();
                        boatDeed.m_Upgrades.RemoveAt(a);
                        break;
                    }
                }                
                
                BaseBoatUpgradeDeed newDeed = (BaseBoatUpgradeDeed)Activator.CreateInstance(upgradeDeed.GetType());

                boatDeed.m_CannonHue = deed.CannonHue;
                boatDeed.m_Upgrades.Add(newDeed);

                upgradeDeed.Delete();
            }
        }

        private class UpgradeDeedTarget : Target
        {
            BaseBoatCannonMetalUpgradeDeed m_UpgradeDeed;

            public UpgradeDeedTarget(BaseBoatCannonMetalUpgradeDeed upgradeDeed): base(1, false, TargetFlags.None)
            {
                m_UpgradeDeed = upgradeDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                Item item = target as Item;

                if (item == null)
                    return;

                if (item.Deleted || !from.Alive)
                    return;

                if (!(item is BaseBoatDeed || item is PlayerLandCannon))
                {
                    from.SendMessage("That is not a ship deed or salvaged cannon.");
                    return;
                }

                if (item is PlayerLandCannon)
                {
                    PlayerLandCannon landCannon = item as PlayerLandCannon;

                    if (!landCannon.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("That must be in your backpack in order to upgrade it.");
                        return;
                    }

                    else
                    {
                        if (landCannon.Hue == m_UpgradeDeed.CannonHue)
                        {
                            from.SendMessage("That cannon is already that hue.");
                            return;
                        }

                        else
                        {
                            landCannon.Hue = m_UpgradeDeed.CannonHue;
                            from.SendMessage("You apply the cannon metal hue upgrade to the cannon.");

                            m_UpgradeDeed.Delete();

                            return;
                        }
                    }
                }

                BaseBoatDeed boatDeed = item as BaseBoatDeed;

                if (boatDeed.PlayerClassRestricted)
                {
                    if (boatDeed.PlayerClassOwner != from)
                    {
                        from.SendMessage("That ship is not bound to you.");
                        return;
                    }
                }

                bool foundUpgrade = boatDeed.CheckForUpgradeType(m_UpgradeDeed);

                if (foundUpgrade)
                {                   
                    from.CloseAllGumps();
                    from.SendGump(new BindBaseBoatUpgradeDeedGump(boatDeed, m_UpgradeDeed, (PlayerMobile)from, true));
                }

                else
                {
                    from.CloseAllGumps();
                    from.SendGump(new BindBaseBoatUpgradeDeedGump(boatDeed, m_UpgradeDeed, (PlayerMobile)from, false));
                }
            }
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