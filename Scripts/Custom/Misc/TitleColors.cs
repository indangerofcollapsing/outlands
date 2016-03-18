using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom
{
	public enum EColorRarity
	{
		Common = 0,
		Uncommon,
		Rare,
		VeryRare,
	}

	public class PlayerTitleColors
	{
		public static string[] ColorRarityNames = new string[((int)EColorRarity.VeryRare) + 1]
		{
			"Common hue",
			"Uncommon hue",
			"Rare hue",
			"Very Rare hue"
		};

		public static void Initialize()
		{
			// count number of colors in each category.
			foreach (int hue in m_CommonColors) {
				if (hue == 0) break;
				++NumCommonHues;
			}
			foreach (int hue in m_UncommonColors){
				if (hue == 0) break;
				++NumUncommonHues;
			}
			foreach (int hue in m_RareColors){
				if (hue == 0) break;
				++NumRareHues;
			}
			foreach (int hue in m_VeryRareColors){
				if (hue == 0) break;
				++NumUniqueHues;
			}
		}

		public readonly static int NUM_COLORS_PER_CATEGORY = 48;

		public static int NumCommonHues { get; set; }
		public static int NumUncommonHues { get; set; }
		public static int NumRareHues { get; set; }
		public static int NumUniqueHues { get; set; }

		private long[] m_ColorStates = new long[((int)EColorRarity.VeryRare) + 1];
		public PlayerTitleColors()
		{
			UnlockColor(0, EColorRarity.Common);
		}
		public bool IsColorUnlocked(int coloridx, EColorRarity color_rarity)
		{
			long color_state = m_ColorStates[(int)color_rarity];
			return (color_state & (1L << coloridx)) != 0;
		}

		public void UnlockColor(int coloridx, EColorRarity color_rarity)
		{
			m_ColorStates[(int)color_rarity] |= (1L << coloridx);
		}
		
		public void UnlockAllColors()
		{
			m_ColorStates[0] = ~0;
			m_ColorStates[1] = ~0;
			m_ColorStates[2] = ~0;
			m_ColorStates[3] = ~0;
		}

		public long GetNumUnlockedColors(EColorRarity color_rarity)
		{
			// http://stackoverflow.com/questions/2709430/count-number-of-bits-in-a-64-bit-long-big-integer
			long i = m_ColorStates[(int)color_rarity];
			
			i = i - ((i >> 1) & 0x5555555555555555);
			i = (i & 0x3333333333333333) + ((i >> 2) & 0x3333333333333333);
			return (((i + (i >> 4)) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56;
		}

		public void Serialize(GenericWriter writer)
		{
			writer.Write(m_ColorStates[0]);
			writer.Write(m_ColorStates[1]);
			writer.Write(m_ColorStates[2]);
			writer.Write(m_ColorStates[3]);

		}

		public void Deserialize(GenericReader reader)
		{
			m_ColorStates[0] = reader.ReadLong();
			m_ColorStates[1] = reader.ReadLong();
			m_ColorStates[2] = reader.ReadLong();
			m_ColorStates[3] = reader.ReadLong();

			UnlockColor(0, EColorRarity.Common);	// make sure the first common color is unlocked
		}

		public static int GetLabelColorValue(int coloridx, EColorRarity color_rarity)
		{
			switch (color_rarity)
			{
				case EColorRarity.Common:
					return coloridx < m_CommonColors.Length ? m_CommonColors[coloridx] : 52;
				case EColorRarity.Uncommon:
					return coloridx < m_UncommonColors.Length ? m_UncommonColors[coloridx] : 52;
				case EColorRarity.Rare:
					return coloridx < m_RareColors.Length ? m_RareColors[coloridx] : 52;
				case EColorRarity.VeryRare:
					return coloridx < m_VeryRareColors.Length ? m_VeryRareColors[coloridx] : 52;
				default:
					return 0;
			}
		}

		public static int GetSpokenColorValue(int coloridx, EColorRarity color_rarity)
		{
			// NOTE: The plus 1 is due to a difference in the UO client renderer.
			// Label hues are offset by 1 compared to "spoken"/message hues 
			switch (color_rarity)
			{
				case EColorRarity.Common:
					return coloridx < m_CommonColors.Length ? m_CommonColors[coloridx] +1 : 52;
				case EColorRarity.Uncommon:
					return coloridx < m_UncommonColors.Length ? m_UncommonColors[coloridx] +1 : 52;
				case EColorRarity.Rare:
					return coloridx < m_RareColors.Length ? m_RareColors[coloridx] +1 : 52;
				case EColorRarity.VeryRare:
					return coloridx < m_VeryRareColors.Length ? m_VeryRareColors[coloridx] +1 : 52;
				default:
					return 0;
			}
		}
		public static int GetNumColorsForRarity(EColorRarity rarity)
		{
			switch (rarity)
			{
				case EColorRarity.Common: return NumCommonHues;
				case EColorRarity.Uncommon: return NumUncommonHues;
				case EColorRarity.Rare: return NumRareHues;
				case EColorRarity.VeryRare: return NumUniqueHues;
				default:
					return 0;
			}
		}

		//---------
		// Common - Basic variations of yellow, green, blue and gray
		//---------
		private static int[] m_CommonColors = new int[48]
		{
			52, 53, 152, 5, 94, 95,
			80, 99, 100, 87, 82,
			149, 190, 290, 490,
			500,0,0,0,0,
			0,0,0,0,0,0,0,0,0,0, // 21-30
			0,0,0,0,0,0,0,0,0,0, // 31-40
			0,0,0,0,0,0,0,0 // 41-50
		};

		//---------
		// Uncommon - Basic variations of red, pink and basic purple
		//---------
		private static int[] m_UncommonColors = new int[48]
		{
			83, 72, 73, 75, 64,
			65, 67, 57, 60, 78, 
			81, 172, 175, 177, 362, 
			367, 375, 383, 480, 481, 
			757,
			0,0,0,0,0,0,0,0,0, // 21-30
			0,0,0,0,0,0,0,0,0,0, // 31-40
			0,0,0,0,0,0,0,0, // 41-50
		};

		//---------
		// Rare - Basic variations of orange, purple and elemental colors
		//---------
		private static int[] m_RareColors = new int[48]
		{
			36, 37, 38, 21, 22, 
			23, 24, 28, 41, 42,
			43, 130, 142, 143, 121, 
			122, 127, 132, 324, 427,
			1375,0,0,0,0,
			0,0,0,0,0,
			0,0,0,0,0,0,0,0,0,0, // 31-40
			0,0,0,0,0,0,0,0, // 41-50
		};

		//---------
		// Very rare - Pure white(s) and strong accented colors. 
		//				These should be truly rare and explicitly dropped (i.e never randomized) - that's what the enum is for.
		//---------
		public enum EVeryRareColorTypes
		{
			SnowWhiteTitleHue = 0,	// Rotting corpses
			LightGrey1TitleHue,
			LightGrey2TitleHue,
			DarkGreyTitleHue,		// Brigands in dungeons
			StrongOrangeTitleHue,	// AW
			ClearBlueTitleHue,		// AL
			VeryPurpleTitleHue,		
			MurdererHue,			// ChaosDragon and Ancient Red Wyrm
		};
		private static int[] m_VeryRareColors = new int[48]
		{
			2048, 
			2036, 
			2038, 
			2425, 
			1259, 
			1282,
			1088, 
			Notoriety.GetHue(Notoriety.Murderer)-1,	// Exactly the same as murderer
			0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0, // 21-30
			0,0,0,0,0,0,0,0,0,0, // 31-40
			0,0,0,0,0,0,0,0, // 41-50
		};
	}
}
