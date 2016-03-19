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
    public enum ActiveAbilityType
    {
        None,
        Ram,       
        ReinforcedHull,
        ExceptionalRigging,
        MastercraftCannons,
        BoardingHooks
    }
    
    public class BaseBoatActiveAbilityUpgradeDeed : BaseBoatUpgradeDeed
    {
        public override UpgradeType UpgradeType { get { return UpgradeType.ActiveAbility; } }        
        public override int DoubloonCost { get { return 200; } }

        public virtual ActiveAbilityType ActiveAbility { get { return ActiveAbilityType.None; } }  

        public ActiveAbilityType m_ActiveAbility; //No Longer used (kept for old serialization)

        [Constructable]
        public BaseBoatActiveAbilityUpgradeDeed(): base()
        {
            Name = "a ship active ability upgrade deed";
        }

        public BaseBoatActiveAbilityUpgradeDeed(Serial serial): base(serial)
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

            BaseBoatActiveAbilityUpgradeDeed deed = upgradeDeed as BaseBoatActiveAbilityUpgradeDeed;

            if (deed != null)
            {
                BaseBoatUpgradeDeed newDeed = (BaseBoatUpgradeDeed)Activator.CreateInstance(upgradeDeed.GetType());

                boatDeed.m_Upgrades.Add(newDeed);

                upgradeDeed.Delete();
            }
        }

        private class UpgradeDeedTarget : Target
        {
            BaseBoatActiveAbilityUpgradeDeed m_UpgradeDeed;

            public UpgradeDeedTarget(BaseBoatActiveAbilityUpgradeDeed upgradeDeed): base(1, false, TargetFlags.None)
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
			writer.Write( (int) 1 ); // version

            writer.Write((int)ActiveAbility);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version
            if (version >= 1)
            {
                m_ActiveAbility = (ActiveAbilityType)reader.ReadInt();
            }
		}
    }
}