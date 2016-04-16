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
    public enum PassiveAbilityType
    {
        None,
        SecureHold,
        ExpandedHold,
        FishingTrawler,
        CrowsNest
    }
    
    public class BaseBoatPassiveAbilityUpgradeDeed : BaseBoatUpgradeDeed
    {
        public override UpgradeType UpgradeType { get { return UpgradeType.PassiveAbility; } }
        public override int DoubloonCost { get { return 100; } }

        public virtual PassiveAbilityType PassiveAbility { get { return PassiveAbilityType.None; } }         

        public PassiveAbilityType m_PassiveAbility; //No Longer used (kept for old serialization)

        [Constructable]
        public BaseBoatPassiveAbilityUpgradeDeed(): base()
        {
            Name = "a ship passive ability upgrade deed";
        }

        public BaseBoatPassiveAbilityUpgradeDeed(Serial serial): base(serial)
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

            BaseBoatPassiveAbilityUpgradeDeed deed = upgradeDeed as BaseBoatPassiveAbilityUpgradeDeed;

            if (deed != null)
            {
                BaseBoatUpgradeDeed newDeed = (BaseBoatUpgradeDeed)Activator.CreateInstance(upgradeDeed.GetType());

                boatDeed.m_Upgrades.Add(newDeed);

                upgradeDeed.Delete();
            }
        }

        private class UpgradeDeedTarget : Target
        {
            BaseBoatPassiveAbilityUpgradeDeed m_UpgradeDeed;

            public UpgradeDeedTarget(BaseBoatPassiveAbilityUpgradeDeed upgradeDeed): base(1, false, TargetFlags.None)
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

                bool foundUpgrade = boatDeed.CheckForUpgrade(m_UpgradeDeed.GetType());

                if (foundUpgrade)
                {
                    from.SendMessage("That upgrade is already present on that ship.");
                    return;
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
			writer.Write( (int) 1); // version

            writer.Write((int)m_PassiveAbility);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version
            if (version >= 1)
            {
                m_PassiveAbility = (PassiveAbilityType)reader.ReadInt();
            }
		}
    }
}