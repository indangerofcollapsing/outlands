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
	public class PirateWeaponOil : Item
	{
        public override int PlayerClassCurrencyValue { get { return 200; } }

		[Constructable] 
		public PirateWeaponOil() : base( 0x0EFE )
		{           
			Weight = 1.0;
            Name = "Pirate Weapon Oil";

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
            Hue = PlayerClassPersistance.PirateItemHue;
		}

        public PirateWeaponOil(Serial serial): base(serial)
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

            if (!pm_From.Pirate || PlayerClassOwner != pm_From)
            {
                from.SendMessage("Only the Pirate owner of this item may use it");
                return;
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
            private PirateWeaponOil m_PirateWeaponOil;

            public InternalTarget(PirateWeaponOil pirateWeaponOil): base(1, false, TargetFlags.None)
            {
                m_PirateWeaponOil = pirateWeaponOil;
			}

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_PirateWeaponOil == null)
                    return;

                if (m_PirateWeaponOil.Deleted)
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
                    from.SendMessage("You may only target weapons in your bank, in your pack, or ones currently equipped.");
                    return;
                }

                if (!(item is BaseWeapon))
                {
                    from.SendMessage("That is not a weapon.");
                    return;
                }

                BaseWeapon weapon = item as BaseWeapon;

                if (weapon.LootType == LootType.Blessed || weapon.LootType == LootType.Newbied)
                {
                    from.SendMessage("You may not dye a blessed weapon.");
                    return;
                }

                weapon.PlayerClass = PlayerClass.Pirate;
                weapon.PlayerClassOwner = from;
                weapon.PlayerClassRestricted = true;
                weapon.Hue = PlayerClassPersistance.PirateItemHue;

                from.PlaySound(0x23E);
                m_PirateWeaponOil.Delete();
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