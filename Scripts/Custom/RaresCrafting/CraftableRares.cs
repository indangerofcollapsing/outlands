using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server;
using Server.Custom.RaresCrafting;
using Server.Mobiles;

namespace Server.Custom.RaresCrafting
{
	////////////////////////////////////////////////////////////////
	// All craftable rares
	////////////////////////////////////////////////////////////////
	public class RareDefinitions
	{
		////////////////////////////////////////////////////////////////
		// Alchemy
		public static ICraftableRare AlchemyFlask1()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Alchemy,
				Result = new CraftableEntry() { m_Name = "flask", m_ItemId = 0x182A },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "vial", m_AmountRequired = 1, m_ItemId = 0x21FE },
					new CraftableEntry(){ m_Name = "flask", m_AmountRequired = 1, m_ItemId = 0x182D },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 },
				},
			};
		}
		public static ICraftableRare AlchemyFlask2()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Alchemy,
				Result = new CraftableEntry() { m_Name = "flask", m_ItemId = 0x182B },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "vial", m_AmountRequired = 1, m_ItemId = 0x21FE },
					new CraftableEntry(){ m_Name = "flask", m_AmountRequired = 1, m_ItemId = 0x182D },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		public static ICraftableRare AlchemyFlask3()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Alchemy,
				Result = new CraftableEntry() { m_Name = "flask", m_ItemId = 0x182C },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "vial", m_AmountRequired = 1, m_ItemId = 0x21FE },
					new CraftableEntry(){ m_Name = "flask", m_AmountRequired = 1, m_ItemId = 0x182D },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		////////////////////////////////////////////////////////////////
		// Bowcrafting
		public static ICraftableRare DecorativeBowAndArrows()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Fletching,
				Result = new CraftableEntry() { m_Name = "decorative weapons", m_ItemId = 0x155c },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "arrows", m_AmountRequired = 5, m_ItemId = 0x0f40 },
					new CraftableEntry(){ m_Name = "peg board", m_AmountRequired = 1, m_ItemId = 0x0c39 },
					new CraftableEntry(){ m_Name = "bow", m_AmountRequired = 1, m_ItemId = 0x13B2 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 2, m_ItemId = 0x5745 }
				},
				DispOffsetY = -25,
				DispOffsetX = -20,
			};
		}
		public static ICraftableRare BundleOfArrows()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Fletching,
				Result = new CraftableEntry() { m_Name = "bundle of arrows", m_ItemId = 0x0f41 },
				Ingredients = new CraftableEntry[2]
				{	
					new CraftableEntry(){ m_Name = "arrows", m_AmountRequired = 3, m_ItemId = 0x0f40 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		public static ICraftableRare BundleOfBolts()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Fletching,
				Result = new CraftableEntry() { m_Name = "bundle of bolts", m_ItemId = 0x1bfd },
				Ingredients = new CraftableEntry[2]
				{	
					new CraftableEntry(){ m_Name = "crossbow bolts", m_AmountRequired = 3, m_ItemId = 0x1bfc },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		////////////////////////////////////////////////////////////////
		// Blacksmithing
		public static ICraftableRare DecorativeHalberd()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Blacksmith,
				Result = new CraftableEntry() { m_Name = "decorative weapons", m_ItemId = 0x1560 },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "rope", m_AmountRequired = 2, m_ItemId = 0x14f8 },
					new CraftableEntry(){ m_Name = "peg board", m_AmountRequired = 1, m_ItemId = 0x0c39 },
					new CraftableEntry(){ m_Name = "halberd", m_AmountRequired = 1, m_ItemId = 0x143E },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 2, m_ItemId = 0x5745 }
				},
				DispOffsetY = -25,
				DispOffsetX = -20,
			};
		}
		public static ICraftableRare HangingChainmailLeggings()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Blacksmith,
				Result = new CraftableEntry() { m_Name = "chainmail leggings", m_ItemId = 0x13BC },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "chains", m_AmountRequired = 1, m_ItemId = 0x1a07 },
					new CraftableEntry(){ m_Name = "peg board", m_AmountRequired = 1, m_ItemId = 0x0c39 },
					new CraftableEntry(){ m_Name = "chainmail leggings", m_AmountRequired = 1, m_ItemId = 0x13BE },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 3, m_ItemId = 0x5745 }
				},
				DispOffsetY = -10,
			};
		}
		public static ICraftableRare GoldIngots()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Blacksmith,
				Result = new CraftableEntry() { m_Name = "gold ingots", m_ItemId = 0x1BEE },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "gold ingots", m_AmountRequired = 5, m_ItemId = 0x1BEA },
					new CraftableEntry(){ m_Name = "beeswax", m_AmountRequired = 1, m_ItemId = 0x1426 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		public static ICraftableRare CopperIngots()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Blacksmith,
				Result = new CraftableEntry() { m_Name = "copper ingots", m_ItemId = 0x1BE5 },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "copper ingots", m_AmountRequired = 5, m_ItemId = 0x1BE4 },
					new CraftableEntry(){ m_Name = "beeswax", m_AmountRequired = 1, m_ItemId = 0x1426 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		////////////////////////////////////////////////////////////////
		// Carpentry
		public static ICraftableRare DartboardWithAxe()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Carpentry,
				SecondReqSkillId = SkillName.Lumberjacking,
				Result = new CraftableEntry() { m_Name = "dartboard", m_ItemId = 0x1E30 },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "dartboard", m_AmountRequired = 1, m_ItemId = 0x1E2E },
					new CraftableEntry(){ m_Name = "axe", m_AmountRequired = 2, m_ItemId = 0xf49 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 2, m_ItemId = 0x5745 }
				},
				DispOffsetX = -10,
			};
		}
		public static ICraftableRare RuinedBookcase()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Carpentry,
				Result = new CraftableEntry() { m_Name = "ruined bookcase", m_ItemId = 0x0c15 },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "book", m_AmountRequired = 10, m_ItemId = 0x0FBD },
					new CraftableEntry(){ m_Name = "sledgehammer", m_AmountRequired = 2, m_ItemId = 0xfb5 },
					new CraftableEntry(){ m_Name = "armoire", m_AmountRequired = 1, m_ItemId = 0xa4f },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 3, m_ItemId = 0x5745 }
				},
				DispOffsetY = -25,
			};
		}
		public static ICraftableRare CoveredChair()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Carpentry,
				SecondReqSkillId = SkillName.Tailoring,
				Result = new CraftableEntry() { m_Name = "covered chair", m_ItemId = 0x0c17 },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "folded sheet", m_AmountRequired = 4, m_ItemId = 0x0A92 },
					new CraftableEntry(){ m_Name = "broken chair", m_AmountRequired = 2, m_ItemId = 0x0C1C },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 4, m_ItemId = 0x5745 }

				},
				DispOffsetY = -15,
				DispOffsetX = -40,
			};
		}
		public static ICraftableRare LogPileLarge()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Carpentry,
				SecondReqSkillId = SkillName.Lumberjacking,
				Result = new CraftableEntry() { m_Name = "pile of logs", m_ItemId = 0x1BE2 },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "logs", m_AmountRequired = 3, m_ItemId = 0x1BE1 },
					new CraftableEntry(){ m_Name = "logs", m_AmountRequired = 20, m_ItemId = 0x1BDD },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
				DispOffsetY = -10,
			};
		}
		////////////////////////////////////////////////////////////////
		// Cooking
		public static ICraftableRare PotOfWax()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Cooking,
				Result = new CraftableEntry() { m_Name = "pot of wax", m_ItemId = 0x142B },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "pot", m_AmountRequired = 1, m_ItemId = 0x09E0 },
					new CraftableEntry(){ m_Name = "beeswax", m_AmountRequired = 2, m_ItemId = 0x1426 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		public static ICraftableRare KettleOfWax()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Cooking,
				Result = new CraftableEntry() { m_Name = "kettle of wax", m_ItemId = 0x142A },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "kettle", m_AmountRequired = 1, m_ItemId = 0x9ED },
					new CraftableEntry(){ m_Name = "beeswax", m_AmountRequired = 5, m_ItemId = 0x1426 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 2, m_ItemId = 0x5745 }
				},
			};
		}
		public static ICraftableRare DirtyPan()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Cooking,
				Result = new CraftableEntry() { m_Name = "dirty pan", m_ItemId = 0x9DE },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "skillet", m_AmountRequired = 1, m_ItemId = 0x97f },
					new CraftableEntry(){ m_Name = "horse dung", m_AmountRequired = 2, m_ItemId = 0x0F3B },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		////////////////////////////////////////////////////////////////
		// Inscription
		public static ICraftableRare DamagedBooks()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Inscribe,
				Result = new CraftableEntry() { m_Name = "damaged books", m_ItemId = 0x0C16 },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "book", m_AmountRequired = 10, m_ItemId = 0x0FBD },
					new CraftableEntry(){ m_Name = "scissors", m_AmountRequired = 2, m_ItemId = 0xf9f },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 2, m_ItemId = 0x5745 }
				},
				DispOffsetY = -10,
				DispOffsetX = -30,
			};
		}
		public static ICraftableRare BookPile1()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Inscribe,
				Result = new CraftableEntry() { m_Name = "books", m_ItemId = 0x1E21 },
				Ingredients = new CraftableEntry[2]
				{	
					new CraftableEntry(){ m_Name = "book", m_AmountRequired = 2, m_ItemId = 0x0FF4 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
				DispOffsetX = -10,
			};
		}
		public static ICraftableRare BookPile2()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Inscribe,
				Result = new CraftableEntry() { m_Name = "books", m_ItemId = 0x1E25 },
				Ingredients = new CraftableEntry[2]
				{	
					new CraftableEntry(){ m_Name = "book", m_AmountRequired = 5, m_ItemId = 0x0FF4 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
			};
		}
		public static ICraftableRare ForbiddenWritings()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Inscribe,
				SecondReqSkillId = SkillName.Magery,
				Result = new CraftableEntry() { m_Name = "forbidden writings", m_ItemId = 0x2253 },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "forbidden words", m_AmountRequired = 10, m_ItemId = 0x2265 },
					new CraftableEntry(){ m_Name = "spellbook", m_AmountRequired = 1, m_ItemId = 0x0EFA },
					new CraftableEntry(){ m_Name = "torch", m_AmountRequired = 1, m_ItemId = 0xf6b },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 2, m_ItemId = 0x5745 }
				},
			};
		}
		////////////////////////////////////////////////////////////////
		// Tailoring
		public static ICraftableRare LargeFishingNet()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Tailoring,
				Result = new CraftableEntry() { m_Name = "net", m_ItemId = 0x1EA5 },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "fishing net", m_AmountRequired = 10, m_ItemId = 0x0DCA },
					new CraftableEntry(){ m_Name = "fishing net", m_AmountRequired = 10, m_ItemId = 0x0DCB },
					new CraftableEntry(){ m_Name = "scissors", m_AmountRequired = 2, m_ItemId = 0xf9f },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 3, m_ItemId = 0x5745 }
				},
				DispOffsetY = -40,
				DispOffsetX = -40,
			};
		}
		public static ICraftableRare DyeableCurtainEast()
		{
			return new CraftableCurtainEast()
			{
				FirstReqSkillId = SkillName.Tailoring,
				Result = new CraftableEntry() { m_Name = "curtain (dyable)", m_ItemId = 0x160D },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "folded sheet", m_AmountRequired = 4, m_ItemId = 0x0A92 },
					new CraftableEntry(){ m_Name = "iron wire", m_AmountRequired = 1, m_ItemId = 0x1876 },
					new CraftableEntry(){ m_Name = "sewing kit", m_AmountRequired = 1, m_ItemId = 0x0F9D },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 4, m_ItemId = 0x5745 }
				},
				DispOffsetY = -40,
				DispOffsetX = -40,
			};
		}
		public static ICraftableRare DyeableCurtainSouth()
		{
			return new CraftableCurtainSouth()
			{
				FirstReqSkillId = SkillName.Tailoring,
				Result = new CraftableEntry() { m_Name = "curtain (dyable)", m_ItemId = 0x160E },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "folded sheet", m_AmountRequired = 4, m_ItemId = 0x0A92 },
					new CraftableEntry(){ m_Name = "iron wire", m_AmountRequired = 1, m_ItemId = 0x1876 },
					new CraftableEntry(){ m_Name = "sewing kit", m_AmountRequired = 1, m_ItemId = 0x0F9D },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 4, m_ItemId = 0x5745 }
				},
				DispOffsetY = -40,
				DispOffsetX = -40,
			};
		}
		////////////////////////////////////////////////////////////////
		// Tinkering
		public static ICraftableRare Anchor()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Tinkering,
				Result = new CraftableEntry() { m_Name = "anchor", m_ItemId = 0x14F7 },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "rope", m_AmountRequired = 2, m_ItemId = 0x14f8 },
					new CraftableEntry(){ m_Name = "iron ingots", m_AmountRequired = 5, m_ItemId = 0x1BF2 },
					new CraftableEntry(){ m_Name = "iron wire", m_AmountRequired = 2, m_ItemId = 0x1876 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 3, m_ItemId = 0x5745 }
				},
				DispOffsetY = -20,
			};
		}
		public static ICraftableRare HangingSkeleton1()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Tinkering,
				Result = new CraftableEntry() { m_Name = "skeleton", m_ItemId = 0x1B7F },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "chains", m_AmountRequired = 4, m_ItemId = 0x1a07 },
					new CraftableEntry(){ m_Name = "peg board", m_AmountRequired = 1, m_ItemId = 0x0c39 },
					new CraftableEntry(){ m_Name = "skeleton", m_AmountRequired = 1, m_ItemId = 0x1D8F },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 5, m_ItemId = 0x5745 }
				},
				DispOffsetY = -30,
			};
		}
		public static ICraftableRare Hook()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Tinkering,
				Result = new CraftableEntry() { m_Name = "hook", m_ItemId = 0x1E9A },
				Ingredients = new CraftableEntry[3]
				{	
					new CraftableEntry(){ m_Name = "rope", m_AmountRequired = 2, m_ItemId = 0x14f8 },
					new CraftableEntry(){ m_Name = "iron ingots", m_AmountRequired = 2, m_ItemId = 0x1BF2 },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 1, m_ItemId = 0x5745 }
				},
				DispOffsetY = -10,
				DispOffsetX = -40,
			};
		}
		public static ICraftableRare HangingCauldron()
		{
			return new CraftableRare()
			{
				FirstReqSkillId = SkillName.Tinkering,
				SecondReqSkillId = SkillName.Blacksmith,
				Result = new CraftableEntry() { m_Name = "cauldron", m_ItemId = 0x0975 },
				Ingredients = new CraftableEntry[4]
				{	
					new CraftableEntry(){ m_Name = "chains", m_AmountRequired = 2, m_ItemId = 0x1a07 },
					new CraftableEntry(){ m_Name = "iron ingots", m_AmountRequired = 5, m_ItemId = 0x1BF2 },
					new CraftableEntry(){ m_Name = "kettle", m_AmountRequired = 1, m_ItemId = 0x9ED },
					new CraftableEntry(){ m_Name = "transformation dust", m_AmountRequired = 5, m_ItemId = 0x5745 }
				},
				DispOffsetY = -35,
			};
		}
	}

	/// <summary>
	/// Common definition for all craftable rares
	/// </summary>
	class CraftableRare : ICraftableRare
	{
		// total hack, using ninjutsu as the "not set" value. Because ninjas are cool.
		private static SkillName NO_SKILL_REQUIREMENT = SkillName.Ninjitsu;
		private SkillName m_FirstRequiredSkillId;
		private SkillName m_SecondRequiredSkillId;
		private int m_NumDustsRequired;

		public SkillName FirstReqSkillId
		{
			set
			{
				m_FirstRequiredSkillId = value;
				m_FirstRequiredSkill = value != NO_SKILL_REQUIREMENT ? SkillInfo.Table[(int)value].Name : "";
				// hax, real name is too long for the gump
				if (m_FirstRequiredSkillId == SkillName.Fletching)
					m_FirstRequiredSkill = "Bowcraft";
			}
			get
			{
				return m_FirstRequiredSkillId;
			}
		}
		public SkillName SecondReqSkillId
		{
			set
			{
				m_SecondRequiredSkillId = value;
				m_SecondRequiredSkill = value != NO_SKILL_REQUIREMENT ? SkillInfo.Table[(int)value].Name : "";
				// hax, real name is too long for the gump
				if (m_FirstRequiredSkillId == SkillName.Fletching)
					m_FirstRequiredSkill = "Bowcraft";
			}
			get
			{
				return m_SecondRequiredSkillId;
			}
		}

		public CraftableEntry[] Ingredients;
		public CraftableEntry Result;

		public CraftableRare()
		{
			FirstReqSkillId = NO_SKILL_REQUIREMENT;
			SecondReqSkillId = NO_SKILL_REQUIREMENT;
		}
		public override bool MeetsRequiredSkillLevel_1(Mobile mob)
		{
			return FirstReqSkillId == NO_SKILL_REQUIREMENT ? true : mob.Skills[FirstReqSkillId].BaseFixedPoint >= 1000;
		}
		public override bool MeetsRequiredSkillLevel_2(Mobile mob)
		{
			return FirstReqSkillId == NO_SKILL_REQUIREMENT ? true : mob.Skills[SecondReqSkillId].BaseFixedPoint >= 1000;
		}

		public override CraftableEntry[] GetIngredients()
		{
			return Ingredients;
		}
		public override CraftableEntry GetResult()
		{
			return Result;
		}
		public override Item GenerateCraftedItem()
		{
			Item crafted_item = new Item(Result.m_ItemId);
			crafted_item.Name = Result.m_Name;
			return crafted_item;
		}
	}

	class CraftableCurtainEast : CraftableRare
	{
		public override Item GenerateCraftedItem()
		{
			Item crafted_item = new Server.Items.DyeableCurtainEast();
			crafted_item.Name = Result.m_Name;
			return crafted_item;
		}
	}

	class CraftableCurtainSouth : CraftableRare
	{
		public override Item GenerateCraftedItem()
		{
			Item crafted_item = new Server.Items.DyeableCurtainSouth();
			crafted_item.Name = Result.m_Name;
			return crafted_item;
		}
	}

}
