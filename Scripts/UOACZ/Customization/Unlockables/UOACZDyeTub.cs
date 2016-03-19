using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;

namespace Server.Items
{
	public class UOACZDyeTub : Item
	{
		public virtual CustomHuePicker CustomHuePicker{ get{ return null; } }
        public virtual bool MetallicHues { get { return false; } }

        private bool m_Redyable = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Redyable
        {
            get { return m_Redyable; }
            set { m_Redyable = value; }
        }

        private int m_DyedHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DyedHue
        {
            get { return m_DyedHue; }
            set
            { 
                m_DyedHue = value;
                Hue = m_DyedHue;
            }
        }

        public static int StartingCharges = 5;

        private int m_UsesRemaining;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; }
        }        

        [Constructable]
        public UOACZDyeTub(): base(4011)
        {
            Name = "a dye tub";
            Weight = 10;

            m_UsesRemaining = StartingCharges;
            m_Redyable = true;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(Hue: " + DyedHue.ToString() + ")");
            LabelTo(from, "[" + UsesRemaining.ToString() + " Uses Remaining]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That dye tub must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("What would you like to dye?");
            from.Target = new InternalTarget(this);
        }

        public static bool IsDyableItem(Item item)
        {
            if (item is BaseClothing)
                return true;

            if (item is BaseArmor)
            {
                BaseArmor armor = item as BaseArmor;

                if (armor.MaterialType == ArmorMaterialType.Leather)
                    return true;

                if (armor.MaterialType == ArmorMaterialType.Studded)
                    return true;
            }

            return false;
        }

        public void UseCharge(Mobile from)
        {
            UsesRemaining--;

            from.PlaySound(0x23E);

            if (UsesRemaining <= 0)
            {
                from.SendMessage("You use the last dye in the tub.");
                Delete();
            }

            else
            {
                from.SendMessage("You use dye the item.");
            }
        }

        private class InternalTarget : Target
        {
            private UOACZDyeTub m_Tub;

            public InternalTarget(UOACZDyeTub tub)
                : base(1, false, TargetFlags.None)
            {
                m_Tub = tub;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {

                if (m_Tub == null || from == null) return;
                if (m_Tub.Deleted || from.Deleted || !from.Alive) return;

                if (!m_Tub.IsChildOf(from.Backpack))
                {
                    from.SendMessage("That dye tub must be in your pack in order to use it.");
                    return;
                }

                Item item = null;

                if (targeted is Item)
                    item = targeted as Item;

                else
                {
                    from.SendMessage("You cannot dye that.");
                    return;
                }

                if (!item.IsChildOf(from.Backpack))
                {
                    from.SendMessage("The item you wish to dye must be in your pack.");
                    return;
                }

                bool canBeDyed = IsDyableItem(item);

                if (!canBeDyed)
                {
                    from.SendMessage("You cannot dye that.");
                    return;
                }

                if (item.Hue == m_Tub.DyedHue)
                {
                    from.SendMessage("That item is already that hue.");
                    return;
                }

                m_Tub.UseCharge(from);

                item.Hue = m_Tub.DyedHue;
            }
        }

        public UOACZDyeTub(Serial serial): base(serial)
        {
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
            writer.Write((int)0); // version

            writer.Write(m_DyedHue);
            writer.Write(m_Redyable);
            writer.Write(m_UsesRemaining);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_DyedHue = reader.ReadInt();
                m_Redyable = reader.ReadBool();
                m_UsesRemaining = reader.ReadInt();
            }
		}	
	}
}