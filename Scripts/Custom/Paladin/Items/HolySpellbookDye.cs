using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class HolySpellbookDye : Item
	{
        public override int PlayerClassCurrencyValue { get { return 200; } }

		[Constructable]
        public HolySpellbookDye(): base(0x0F00)
		{           
			Weight = 1.0;
            Name = "Holy Spellbook Dye";

            PlayerClass = PlayerClass.Paladin;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PaladinItemHue;
		}

        public HolySpellbookDye(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

		public override void OnDoubleClick( Mobile from )
		{
            PlayerMobile pm_From = from as PlayerMobile;

            if (!pm_From.Paladin || this.PlayerClassOwner != pm_From)
            {
                from.SendMessage("Only the Paladin owner of this item may use it");
                return;
            }

            else if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("Target the weapon you wish to dye.");
                from.Target = new InternalTarget(this);
            }

            else if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("Target the weapon you wish to dye.");
                from.Target = new InternalTarget(this);
            }

            else
            {
                from.SendMessage("That is too far away to use.");
                return;
            }
		}

		private class InternalTarget : Target
		{
            private HolySpellbookDye m_HolySpellbookDye;

            public InternalTarget(HolySpellbookDye HolySpellbookDye): base(1, false, TargetFlags.None)
            {
                m_HolySpellbookDye = HolySpellbookDye;
			}

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_HolySpellbookDye == null || m_HolySpellbookDye.Deleted)
                    return;

                if (!(targeted is Item))
                {
                    from.SendMessage("You cannot dye that with this.");
                    return;
                }

                Item item = targeted as Item;
                BankBox bankBox = from.FindBankNoCreate();

                bool itemInBank = false;
                bool itemOnPerson = false;

                if (bankBox == null)
                {
                    if (item.RootParent == bankBox)
                        itemInBank = true;
                }

                if (item.Parent == from || item.RootParent == from)
                    itemOnPerson = true;

                if (!(itemOnPerson || itemInBank))
                {
                    from.SendMessage("You may only target spellbooks in your bank, in your pack, or ones currently equipped.");
                    return;
                }

                if (!(item is Spellbook))
                {
                    from.SendMessage("That is not a spellbook.");
                    return;
                }

                item.Hue = PlayerClassPersistance.PaladinItemHue;
                item.PlayerClassOwner = from;

                from.PlaySound(0x23E);
                m_HolySpellbookDye.Delete();
            }
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0); //Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}