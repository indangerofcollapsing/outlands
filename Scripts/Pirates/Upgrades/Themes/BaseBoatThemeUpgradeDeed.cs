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
    public enum Theme
    {
        None,
        BritainNavy,
        Pirate,
        Fisherman,
        GhostShip,
        Derelict,
        Treasure
    }
    
    public class BaseBoatThemeUpgradeDeed : BaseBoatUpgradeDeed
    {
        public override UpgradeType UpgradeType { get { return UpgradeType.Theme; } } 
        public override int DoubloonCost { get { return 500; } }

        public Theme Theme;

        [Constructable]
        public BaseBoatThemeUpgradeDeed(): base()
        {
            Name = "a ship theme upgrade deed";

            Theme = Theme.None;
        }

        public BaseBoatThemeUpgradeDeed(Serial serial): base(serial)
		{
		}

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return;

            from.CloseAllGumps();

            from.SendMessage("Target the ship deed you wish to install this upgrade onto.");
            from.Target = new UpgradeDeedTarget(this);
        }

        public override void InstallUpgrade(BaseBoatDeed boatDeed, BaseBoatUpgradeDeed upgradeDeed, Mobile mobile)
        {
            base.InstallUpgrade(boatDeed, upgradeDeed, mobile);

            BaseBoatThemeUpgradeDeed deed = upgradeDeed as BaseBoatThemeUpgradeDeed;

            if (deed != null)
            {
                for (int a = 0; a < boatDeed.m_Upgrades.Count; a++)
                {
                    if (boatDeed.m_Upgrades[a] is BaseBoatThemeUpgradeDeed)
                    {
                        boatDeed.m_Upgrades[a].Delete();
                        boatDeed.m_Upgrades.RemoveAt(a);
                        break;
                    }
                }

                BaseBoatUpgradeDeed newDeed = (BaseBoatUpgradeDeed)Activator.CreateInstance(upgradeDeed.GetType());

                boatDeed.m_Upgrades.Add(newDeed);

                upgradeDeed.Delete();
            }
        }

        private class UpgradeDeedTarget : Target
        {
            BaseBoatThemeUpgradeDeed m_UpgradeDeed;

            public UpgradeDeedTarget(BaseBoatThemeUpgradeDeed upgradeDeed): base(1, false, TargetFlags.None)
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

                if (!(item is BaseBoatDeed))
                {
                    from.SendMessage("That is not a ship deed.");
                    return;
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
			writer.Write( (int) 1 ); // version

            //Version 1
            writer.Write((int)Theme); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version
            if (version >= 1)
            {
                Theme = (Theme)reader.ReadInt();
            }
		}
    }
}