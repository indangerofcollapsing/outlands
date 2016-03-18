using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Items;
using Server.Items.Deeds;

namespace Server.ArenaSystem
{
	/// <summary>
	/// Baseclass, inherit
	/// </summary>
	public class ArenaSeasonRewardPackage
	{
		public Item[] CreateDefaultPrizes(EArenaMatchEra era, EArenaMatchRestrictions restrictions, int teamcount, int season_rank)
		{
			int rndrew = Utility.Random(4);
			Item price = null;
			if( rndrew == 0 )
				price = new ArenaRewardTotem_2HWeapons(47);
			else if (rndrew == 1)
				price = new ArenaRewardTotem_Headgear(47);
			else if (rndrew == 2)
				price = new ArenaRewardTotem_1HWeapons(47);
			else
				price = new ArenaRewardTotem_ArmorCombo(2603);

			price.Name = String.Format("A prize for ranking {0} in a {1}v{1} {2}/{3} arena ladder.", season_rank, teamcount, ArenaSystem.s_eraNames[(int)era], ArenaSystem.s_restrictionNames[(int)restrictions]);
			return new Item[]{price};
		}
		public virtual Item[] CreatePrizes(EArenaMatchEra era, EArenaMatchRestrictions restrictions, int teamcount, int season_rank)
		{
			return CreateDefaultPrizes(era, restrictions, teamcount, season_rank);
		}
	}

	public class BetaRewardPackage : ArenaRewardPackage
	{
		public BetaRewardPackage() : base(2213, 2213, 2213, 1779,1779,1779) { }
	}
    public class JanuaryRewardPackage : ArenaRewardPackage
    {
        public JanuaryRewardPackage() : base(2418, 2418, 2418, 2413, 2413, 2413) { }
    }
	public class FebruaryRewardPackage : ArenaRewardPackage
	{
        public FebruaryRewardPackage() : base(2213, 2213, 2213, 1779, 1779, 1779) { }
	}
	public class MarchRewardPackage : ArenaRewardPackage
	{
		public MarchRewardPackage() : base(2219, 2219, 2219, 2425, 2425, 2425) { }
	}
	public class AprilRewardPackage : ArenaRewardPackage
	{
        public AprilRewardPackage() : base(2418, 2418, 2418, 2413, 2413, 2413) { }
	}
	public class MayRewardPackage : ArenaRewardPackage
	{
		public MayRewardPackage() : base(2207, 2207, 2207, 2406, 2406, 2406) { }
	}
	public class JuneRewardPackage : ArenaRewardPackage
	{
        public JuneRewardPackage() : base(2213, 2213, 2213, 1779, 1779, 1779) { }
	}
	public class JulyRewardPackage : ArenaRewardPackage
	{
        public JulyRewardPackage() : base(2219, 2219, 2219, 2425, 2425, 2425) { }
	}
	public class AugustRewardPackage : ArenaRewardPackage
	{
        public AugustRewardPackage() : base(2418, 2418, 2418, 2413, 2413, 2413) { }
	}
	public class SeptemberRewardPackage : ArenaRewardPackage
	{
		public SeptemberRewardPackage() : base(2207, 2207, 2207, 2406, 2406, 2406) { }
	}
	public class OctoberRewardPackage : ArenaRewardPackage
	{
        public OctoberRewardPackage() : base(2213, 2213, 2213, 1779, 1779, 1779) { }
	}
    public class NovemberRewardPackage : ArenaRewardPackage
    {
        public NovemberRewardPackage() : base(2219, 2219, 2219, 2425, 2425, 2425) { }
    }
    public class DecemberRewardPackage : ArenaRewardPackage
    {
        public DecemberRewardPackage() : base(2207, 2207, 2207, 2406, 2406, 2406) { }
    }

	public class ArenaRewardPackage : ArenaSeasonRewardPackage
	{
		private int m_1v1hue_order;
		private int m_2v2hue_order;
		private int m_3v3hue_order;

		private int m_1v1hue_chaos;
		private int m_2v2hue_chaos;
		private int m_3v3hue_chaos;

		public ArenaRewardPackage(int hue1v1_o, int hue2v2_o, int hue3v3_o, int hue1v1_c, int hue2v2_c, int hue3v3_c)
		{
			m_1v1hue_order = hue1v1_o;
			m_2v2hue_order = hue2v2_o;
			m_3v3hue_order = hue3v3_o;

			m_1v1hue_chaos = hue1v1_c;
			m_2v2hue_chaos = hue2v2_c;
			m_3v3hue_chaos = hue3v3_c;
		}

		public override Item[] CreatePrizes(EArenaMatchEra era, EArenaMatchRestrictions restrictions, int teamcount, int season_rank)
		{
			Item price = null;
			if (teamcount == 1)
			{
				price = new ArenaRewardTotem_2HWeapons(restrictions == EArenaMatchRestrictions.eAMC_Chaos ? m_1v1hue_chaos : m_1v1hue_order);
			}
			else if (teamcount == 2)
			{
				price = new ArenaRewardTotem_1HWeapons(restrictions == EArenaMatchRestrictions.eAMC_Chaos ? m_2v2hue_chaos : m_2v2hue_order);
			}
			else if (teamcount == 3)
			{
				price = new ArenaRewardTotem_ArmorCombo(restrictions == EArenaMatchRestrictions.eAMC_Chaos ? m_3v3hue_chaos : m_3v3hue_order);
			}
			price.Name = String.Format("UOAC {0} league - {1}v{1}", ArenaSystem.s_restrictionNames[(int)restrictions], teamcount);
			return new Item[]{price};
		}
	}

	public class DefaultRewardPackage : ArenaSeasonRewardPackage
	{
		public virtual Item[] CreatePrize(EArenaMatchEra era, EArenaMatchRestrictions restrictions, int teamcount, int season_rank)
		{
			return CreateDefaultPrizes(era, restrictions, teamcount, season_rank);
		}
	}



	////////////////////////////////////////////////////////////////
	// Arena reward totem, special item that set the hue of equipped items in the arena 
	////////////////////////////////////////////////////////////////
	public class ArenaRewardTotem_ArmorCombo : ArenaRewardTotem
	{
		[Constructable]
		public ArenaRewardTotem_ArmorCombo(int hue) : base(hue) { }
		public ArenaRewardTotem_ArmorCombo(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }

		public override void HueItem(Item item)
		{
			if (item.Layer == Layer.Helm || item.Layer == Layer.MiddleTorso || item.Layer == Layer.Shoes || item.Layer == Layer.Gloves)
			{
				SetNewAndOriginalHue(item, m_Hue);
			}
		}
	}
	public class ArenaRewardTotem_Headgear : ArenaRewardTotem
	{
		[Constructable]
		public ArenaRewardTotem_Headgear(int hue) : base(hue) { }
		public ArenaRewardTotem_Headgear(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }

		public override void HueItem(Item item)
		{
			if (item.Layer == Layer.Helm)
			{
				SetNewAndOriginalHue(item, m_Hue);
			}
		}
	}
	public class ArenaRewardTotem_2HWeapons : ArenaRewardTotem
	{
		[Constructable]
		public ArenaRewardTotem_2HWeapons(int hue) : base(hue) { }
		public ArenaRewardTotem_2HWeapons(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }

		public override void HueItem(Item item)
		{
			if (item.Layer == Layer.TwoHanded)
			{
				SetNewAndOriginalHue(item, m_Hue);
			}
		}
	}
	public class ArenaRewardTotem_1HWeapons : ArenaRewardTotem
	{
		[Constructable]
		public ArenaRewardTotem_1HWeapons(int hue) : base(hue) { }
		public ArenaRewardTotem_1HWeapons(Serial serial) : base(serial) { }
		public override void Serialize(GenericWriter writer) { base.Serialize(writer); }
		public override void Deserialize(GenericReader reader) { base.Deserialize(reader); }

		public override void HueItem(Item item)
		{
			if (item.Layer == Layer.OneHanded)
			{
				SetNewAndOriginalHue(item, m_Hue);
			}
		}
	}

	public abstract class ArenaRewardTotem : Item
	{
		protected int m_Hue {get; set;}
		public int Charges { get; set; }
		public Type RequiredTargetType;
		public string ItemNameAfterDyed;

		[Constructable]
		public ArenaRewardTotem(int hue)
			: base(0x2F5B)
		{
			Weight = 1.0;
			LootType = LootType.Blessed;
			Layer = Layer.Talisman;
			m_Hue = hue;
			Hue = hue;	// Item.Hue
		}
		public ArenaRewardTotem(Serial serial)
			: base(serial)
		{
		}

		public override void LabelLootTypeTo(Mobile to)
		{
			// blessed to specific person
			Mobile bfor = BlessedFor;
			if (bfor != null)
			{
				LabelTo(to, String.Format("(blessed for {0})", bfor.Name));
			}
			else
			{
				base.LabelLootTypeTo(to);
			}
		}

		public abstract void HueItem(Item item);


		protected void SetNewAndOriginalHue(Item item, int new_hue)
		{
			// why this? Setting item.Hue also sets item.OriginalHue
			// when we do this we want the Hue to be the new Hue and OriginalHue to be whatever was before.
			// this is the ONLY place we should have to take this into consideration.
			if (item.Hue != new_hue)
			{
				int old_hue = item.Hue;
				item.Hue = m_Hue;
				item.OriginalHue = old_hue;
			}
		}

		public override void OnRemoved(object parent)
		{
			base.OnRemoved(parent);
			Mobile from = parent as Mobile;
			if (from != null)
			{
				foreach (Item i in from.Items)
					i.Hue = i.OriginalHue;
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
			writer.Write(m_Hue);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			m_Hue = reader.ReadInt();
		}
	}
}
