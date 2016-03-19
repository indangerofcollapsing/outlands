using System;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class ZombieQuestStone : Item
	{
		// Zombie Quest Stone
		[Constructable]
		public ZombieQuestStone()	: base(0xED4) 
		{
			Movable = false;
			Hue = 0x489; //Fire Red
			Name = "Zombie Quest";
		}

		public ZombieQuestStone(Serial serial)
			: base(serial)
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			PlayerMobile pm = from as PlayerMobile;

            if (!pm.InRange(GetWorldLocation(), 2)) // Are they in range of the Stone?
			{
                pm.SendLocalizedMessage(500446);  // That is too far away.
			}
            else if (!ZombieQuestRegion.CanJoin(from))
            {
                from.SendMessage("You cannot join again so soon.");
            }
            else // Valid Double Click
            {
                pm.DropHolding();

                if (xStrip(pm)) // Remove all Items and put into a bag in the bank
                {
                    ZombieQuestRegion.Join(from);
                }
            }
		}

		// Method to put all playe items into a bag and in their bank
		public bool xStrip(PlayerMobile m)
		{
			Backpack l_bag = new Backpack();
			//l_bag.Hue = 1;
			//l_bag.Movable = false;

			ArrayList equipedItems = new ArrayList(m.Items);

			foreach (Item item in equipedItems)
			{
				if (!(item.Layer == Layer.Bank ||
					  item.Layer == Layer.Backpack ||
					  item.Layer == Layer.Hair ||
					  item.Layer == Layer.FacialHair ||
					  item is DeathShroud ||
                      item is Spellbook))
				{
                    if (item.DonationItem || item.QuestItem || item.Nontransferable)
                        m.BankBox.DropItem(item);
                    else
					    l_bag.DropItem(item);
				}
			}

			ArrayList backpackItems = new ArrayList(m.Backpack.Items);

			foreach (Item item in backpackItems)
			{
				// Non movable pack items must remain
				if (item.Movable)
				{
                    if (item is Spellbook)
                    {
                        continue;
                    }
                    else if (item.DonationItem || item.QuestItem || item.Nontransferable)
                        m.BankBox.DropItem(item);
                    else
					    l_bag.DropItem(item);
				}
			}
			// Put the bag into their Bank
			if (m.BankBox.TryDropItem(m, l_bag, false))
			{
				m.SendMessage("Your items have been stored in your bank box.");
				return true;
			}
			else
			{
				ArrayList backToPlayerBackPack = new ArrayList(l_bag.Items);

				foreach (Item item in backToPlayerBackPack)
				{
					m.Backpack.DropItem(item);
				}
				l_bag.Delete();
				m.SendMessage("There is not enough room in your bank box.");
				return false;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);

            bool toWrite = !ZombieQuestRegion.Saved;
            writer.Write(toWrite);
            if (toWrite)
                ZombieQuestRegion.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        if (reader.ReadBool())
                            ZombieQuestRegion.Deserialize(reader);

                        break;
                    }
            }
		}
	}
}



