using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public enum UpgradeType
    {
        PassiveAbility,
        ActiveAbility,
        Paint,
        Theme,
        EpicAbility,
        OutfittingType,
        CannonMetal
    }
    
    public class BaseBoatUpgradeDeed : Item
    {
        public virtual UpgradeType UpgradeType { get { return UpgradeType.PassiveAbility; } }      
        public virtual int DoubloonCost { get { return 0; } }  
        public virtual string DisplayName { get { return ""; } }  

        [Constructable]
        public BaseBoatUpgradeDeed(): base(0x14F0)
		{
            Name = "a boat upgrade deed";
            Weight = 1.0;			
		}

        public BaseBoatUpgradeDeed(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", Name));

                if (DoubloonCost > 0)
                    ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, 0, 3, "ENU", "", "[Base Cost: " + DoubloonCost.ToString() + " doubloons]"));               
            }
        }

        public virtual void InstallUpgrade(BaseBoatDeed boatDeed, BaseBoatUpgradeDeed upgradeDeed, Mobile mobile)
        {
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

    public class BindBaseBoatUpgradeDeedGump : Gump
    {
        BaseBoatDeed m_BaseBoatDeed;
        BaseBoatUpgradeDeed m_BaseBoatUpgradeDeed;
        PlayerMobile m_Player;

        public BindBaseBoatUpgradeDeedGump(BaseBoatDeed baseBoatDeed, BaseBoatUpgradeDeed baseBoatUpgradeDeed, PlayerMobile player, bool installOver): base(50, 50)
        {
            m_BaseBoatDeed = baseBoatDeed;
            m_BaseBoatUpgradeDeed = baseBoatUpgradeDeed;
            m_Player = player;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            AddPage(0);

            AddBackground(90, 150, 380, 215, 5054);
            AddBackground(100, 160, 360, 195, 3000);

            int upgradeCost = (int)((double)m_BaseBoatUpgradeDeed.DoubloonCost * m_BaseBoatDeed.DoubloonMultiplier);
            string cost = m_BaseBoatUpgradeDeed.DoubloonCost.ToString() + " x " + m_BaseBoatDeed.DoubloonMultiplier.ToString() + " = " + upgradeCost.ToString();

            if (upgradeCost > 0)
            {                
                AddHtml(110, 175, 330, 17, @"<center>" + "The following number of doubloons will be removed" + "</center>", (bool)false, (bool)false);
                AddHtml(110, 197, 330, 17, @"<center>" + "from your bank box and the ship will be upgraded." + "</center>", (bool)false, (bool)false);
                AddHtml(110, 230, 330, 17, @"<center>" + cost + "</center>", (bool)false, (bool)false);

                AddItem(255, 255, 2539);
            }

            else
            {
                AddHtml(110, 230, 330, 17, @"<center>" + "Install this upgrade?" + "</center>", (bool)false, (bool)false);
            }

            if (installOver)            
                AddHtml(110, 285, 330, 17, @"<center>" + "*This will remove the previous upgrade of this type*" + "</center>", (bool)false, (bool)false);
                
            AddButton(170, 315, 247, 248, 1, GumpButtonType.Reply, 0); //Okay
            AddButton(310, 315, 243, 248, 2, GumpButtonType.Reply, 0); //Cancel
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            int upgradeCost = (int)((double)m_BaseBoatUpgradeDeed.DoubloonCost * m_BaseBoatDeed.DoubloonMultiplier);

            switch (info.ButtonID)
            {
                case 1:
                    int doubloonsInBank = Banker.GetUniqueCurrencyBalance(from, typeof(Doubloon));

                    if (doubloonsInBank >= upgradeCost)
                    {
                        if (Banker.WithdrawUniqueCurrency(m_Player, typeof(Doubloon), upgradeCost))
                        {
                            Doubloon doubloonPile = new Doubloon(upgradeCost);
                            from.SendSound(doubloonPile.GetDropSound());
                            doubloonPile.Delete();

                            m_BaseBoatUpgradeDeed.InstallUpgrade(m_BaseBoatDeed, m_BaseBoatUpgradeDeed, from);

                            m_BaseBoatDeed.PlayerClassOwner = m_Player;
                            m_Player.SendMessage("You install the upgrade on the ship.");
                        }

                        else
                            m_Player.SendMessage("You do not have the required doubloons in your bankbox to install this upgrade.");
                    }

                    else
                        m_Player.SendMessage("You do not have the required doubloons in your bankbox to install this upgrade.");
                    
                break;
            }
        }        
    }
}