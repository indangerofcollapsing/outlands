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
    public enum OutfittingType
    {
        None,
        Runner,
        Merchant,
        Hunter,
        Destroyer,
        Dreadnought
    }

    public class BaseBoatOutfittingUpgradeDeed : BaseBoatUpgradeDeed
    {
        public override UpgradeType UpgradeType { get { return UpgradeType.OutfittingType; } }
        public virtual OutfittingType Outfitting { get { return OutfittingType.None; } } 

        public override int DoubloonCost { get { return 2000; } }

        [Constructable]
        public BaseBoatOutfittingUpgradeDeed(): base()
        {
            Name = "a ship outfitting upgrade deed";
        }

        public BaseBoatOutfittingUpgradeDeed(Serial serial): base(serial)
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

        private class UpgradeDeedTarget : Target
        {
            BaseBoatOutfittingUpgradeDeed m_UpgradeDeed;

            public UpgradeDeedTarget(BaseBoatOutfittingUpgradeDeed upgradeDeed): base(1, false, TargetFlags.None)
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
        public override void InstallUpgrade(BaseBoatDeed boatDeed, BaseBoatUpgradeDeed upgradeDeed, Mobile mobile)
        {
            base.InstallUpgrade(boatDeed, upgradeDeed, mobile);

            BaseBoatOutfittingUpgradeDeed deed = upgradeDeed as BaseBoatOutfittingUpgradeDeed;

            if (deed != null && boatDeed != null)
            {
                for (int a = 0; a < boatDeed.m_Upgrades.Count; a++)
                {
                    if (boatDeed.m_Upgrades[a] is BaseBoatOutfittingUpgradeDeed)
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

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}