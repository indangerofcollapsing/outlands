using Server.Spells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items.Misc
{
	class PouchWithManyPockets : Pouch
	{
		private static int s_MaxTrapCharges = 10;

		private int m_Charges;
        private int m_Hue = 14;
        public static int TrappedHue = 25;

		public override string DefaultName
		{
			get
			{
				return "a pouch with many pockets";
			}
		}

		public override bool DisplaysContent
		{
			get
			{
				return false;
			}
		}

		public override bool DisplayLootType
		{
			get
			{
				return true;
			}
		}
		[Constructable]
		public PouchWithManyPockets() : base()
		{
			Weight = 5.0;
			LootType = LootType.Blessed;
            Hue = m_Hue;
		}

		public PouchWithManyPockets(Serial serial)
			: base(serial)
		{
		}

        public override int GetTotal(TotalType type)
        {
            int total = base.GetTotal(type);

            if (type == TotalType.Items)
            {
                return 9;          // RunUO seems to treat TotalItems as a 0-based count for some reason
            }

            return total;
        }

        public override bool CheckTarget(Mobile from, Targeting.Target targ, object targeted)
        {
            // is there a better way to do this?
            if (targ.GetType().FullName == "Server.Spells.Second.MagicTrapSpell+InternalTarget")
            {
                if (SpellHelper.CheckCombat(from, true))
                {
                    // send no combat message
                    from.SendMessage("You cannot trap this while in combat.");
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return base.CheckTarget(from, targ, targeted);
        }

		public override void OnSingleClick(Mobile from)
		{
			base.OnSingleClick(from);
			LabelTo(from, String.Format("[{0} pockets are trapped]", m_Charges));
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
            Hue = m_Hue;
            if (this.TrapType == TrapType.MagicTrap)
            {
                m_Charges = s_MaxTrapCharges;
                Hue = TrappedHue;
            }
		}

		///////////// OVERRIDES 
		[CommandProperty( AccessLevel.GameMaster )]
		public override TrapType TrapType
		{
			get
			{
				return base.TrapType;
			}
			set
			{
				if (value == TrapType.MagicTrap)
					m_Charges = s_MaxTrapCharges;
				base.TrapType = value;
			}
		}

		public override bool ExecuteTrap(Mobile from)
		{
			if ( TrapType == TrapType.MagicTrap )
			{
				if (from.AccessLevel >= AccessLevel.GameMaster)
				{
					SendMessageTo(from, "That is trapped, but you open it with your godly powers.", 0x3B2);
					return false;
				}

				Point3D loc = this.GetWorldLocation();
				Map facet = this.Map;

				if (from.InRange(loc, 1))
					from.Damage(TrapPower);

				Effects.PlaySound(loc, Map, 0x307);

				Effects.SendLocationEffect(new Point3D(loc.X - 1, loc.Y, loc.Z), Map, 0x36BD, 15);
				Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y, loc.Z), Map, 0x36BD, 15);

				Effects.SendLocationEffect(new Point3D(loc.X, loc.Y - 1, loc.Z), Map, 0x36BD, 15);
				Effects.SendLocationEffect(new Point3D(loc.X, loc.Y + 1, loc.Z), Map, 0x36BD, 15);

				Effects.SendLocationEffect(new Point3D(loc.X + 1, loc.Y + 1, loc.Z + 11), Map, 0x36BD, 15);

				// IPY - Restore pouch color after use of magic trap
				--m_Charges;
				if (m_Charges == 0)
				{
					Hue = m_Hue;
					TrapType = TrapType.None;
					TrapPower = 0;
					TrapLevel = 0;
					Trapper = null;
				}
				// IPY - Restore pouch color after use of magic trap
			}
			return true; // never allow opening
		}


		public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
		{
			return false; // no storage
		}
	}
}
