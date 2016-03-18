using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Custom;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    class CommonTitleDye : TitleDye
    {
        public CommonTitleDye() : base()
        {
			ColorIndex = Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.Common));
			ColorRarity = EColorRarity.Common;
        }

        public CommonTitleDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    class UncommonTitleDye : TitleDye
    {
        public UncommonTitleDye()
            : base()
        {
            ColorIndex = Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.Uncommon));
            ColorRarity = EColorRarity.Uncommon;
        }

        public UncommonTitleDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    class RareTitleDye : TitleDye
    {
        public RareTitleDye()
            : base()
        {
            ColorIndex = Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.Rare));
            ColorRarity = EColorRarity.Rare;
        }

        public RareTitleDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    class VeryRareTitleDye : TitleDye
    {
        public VeryRareTitleDye()
            : base()
        {
            ColorIndex = Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.VeryRare));
            ColorRarity = EColorRarity.VeryRare;
        }

        public VeryRareTitleDye(Serial serial) : base(serial) { }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

	class TitleDye : Item
	{
		/// <summary>
		/// Helpers
		/// </summary>
		public static Item RandomCommonTitleDye()
		{
			return new TitleDye(Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.Common)), EColorRarity.Common);
		}
		public static Item RandomUncommonTitleDye()
		{
			return new TitleDye(Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.Uncommon)), EColorRarity.Uncommon);
		}
		public static Item RandomRareTitleDye()
		{
			return new TitleDye(Utility.Random(PlayerTitleColors.GetNumColorsForRarity(EColorRarity.Rare)), EColorRarity.Rare);
		}
		public static Item VeryRareTitleDye(PlayerTitleColors.EVeryRareColorTypes unique_color)
		{
			return new TitleDye((int)unique_color, EColorRarity.VeryRare);
		}


		/// <summary>
		/// TitleDye item
		/// </summary>
		private int m_ColorIndex;
		EColorRarity m_ColorRarity;

		[CommandProperty(AccessLevel.GameMaster)]
		public int ColorIndex 
		{
			get { return m_ColorIndex; }
			set {
				m_ColorIndex = Math.Max(0, value);
				m_ColorIndex = Math.Min(PlayerTitleColors.GetNumColorsForRarity(ColorRarity) - 1, ColorIndex);
				Hue = PlayerTitleColors.GetLabelColorValue(m_ColorIndex, m_ColorRarity);
			}
		}
		[CommandProperty(AccessLevel.GameMaster)]
		public EColorRarity ColorRarity
		{
			get { return m_ColorRarity; }
			set { 
				m_ColorRarity = value;
				Hue = PlayerTitleColors.GetLabelColorValue(m_ColorIndex, m_ColorRarity);
			}
		}

		public override string DefaultName
		{
			get {
				switch (m_ColorRarity)
				{
					case EColorRarity.Common: return string.Format("an illusionary title dye (common {0:D4})", Hue);
					case EColorRarity.Uncommon: return string.Format("an illusionary title dye (uncommon {0:D4})", Hue);
					case EColorRarity.Rare: return string.Format("an illusionary title dye (rare {0:D4})", Hue);
					case EColorRarity.VeryRare: return string.Format("an illusionary title dye (very rare {0:D4})", Hue);
					default: return string.Format("an illusionary title dye {0:D4}", Hue);
				} 
			}
		}

		[Constructable]
		public TitleDye()
			: base(0xE26)
		{
			ColorIndex = 0;
			ColorRarity = EColorRarity.Common;
			Weight = 1.0;
		}

		[Constructable]
		public TitleDye(int titlecolor_index, EColorRarity rarity)
			: base(0xE26)
		{
			ColorIndex = titlecolor_index;
			ColorRarity = rarity;
			Weight = 1.0;
		}


		public TitleDye(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version

			// version 0
			writer.Write(ColorIndex);
			writer.Write((int)ColorRarity);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			switch (version)
			{
				case 0:
				{
					m_ColorIndex = reader.ReadInt();
					m_ColorRarity = (EColorRarity)reader.ReadInt();

					break;
				}
			}
		}

		public override void OnDoubleClick(Mobile from)
		{
			if (!IsChildOf(from.Backpack)) // Make sure its in their pack
			{
				from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
				return;
			}
			if (!from.InRange(this.GetWorldLocation(), 1))
			{
				from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that.
				return;
			}
			
			PlayerMobile pm = from as PlayerMobile;
			if( pm == null )
				return;

			if (pm.TitleColorState.IsColorUnlocked(ColorIndex, ColorRarity))
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, false, "I already know this hue...");
			}
			else
			{
				int hue = PlayerTitleColors.GetSpokenColorValue(ColorIndex, ColorRarity);
				from.LocalOverheadMessage(MessageType.Regular, hue, true, "That's a pretty hue...");
				from.SendMessage("You have unlocked a new title color");

				pm.TitleColorState.UnlockColor(ColorIndex, ColorRarity);
				this.Delete();
			}
		}
	}
}
