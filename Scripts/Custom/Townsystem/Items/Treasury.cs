using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using System.Collections.Generic;
using Server.Targeting;
using Server.Regions;
using Server.Custom.Townsystem;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
	public class TreasuryChest : Item
	{ 
        public override bool Decays { get { return false; } }
        public override double DefaultWeight { get { return 20.0; } }
        public static readonly TimeSpan TreasuryCaptureTime = TimeSpan.FromHours(24);
		public static List<TreasuryChest> s_AllTreasuryChests = new List<TreasuryChest>();
		public HashSet<Serial> m_Thieves;

		private Town m_Town;

		[CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
		public bool ClearThievesList
		{
			get { return false; }
			set { if(value == true) m_Thieves.Clear(); }
		}

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public double TreasuryValue
        {
            get { return m_Town.Treasury; }
            set { m_Town.Treasury = value; }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
        public TreasuryWallTypes TreasuryWallType
        {
            get { return m_Town.TreasuryWallType; }
            set { m_Town.SetTreasuryWalls(value); }
        }

		[CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
		public Town Town
		{
			get{ return m_Town; }
			set{ m_Town = value; Update(); }
		}

		public void Update()
		{
            if (m_Town == null)
                Name = "a town treasury";
            else
                Name = String.Format("{0} Treasury", m_Town.Definition.FriendlyName);

			InvalidateProperties();
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick( from );
			if (m_Town != null)
				LabelTo(from, "{0} gold coin", (int)m_Town.Treasury);
		}

        public override void OnDoubleClick(Mobile m)
        {
            if (Town == null || m == null)
                return;

			PlayerMobile pm = m as PlayerMobile;

            if (pm == null || !pm.IsInMilitia)
            {
				return;
            }

			double dist_to_chest = Math.Pow(pm.X - X, 2) + Math.Pow(pm.Y - Y, 2);
			if (dist_to_chest > 4.0)
			{
				pm.SendLocalizedMessage(500446); // That is too far away
			}
			else
			{
				pm.SendGump(new ConfirmUseKeyGump(pm, this));
			}
        }

		private class ConfirmUseKeyGump : Gump
		{
			private PlayerMobile m_From;
			TreasuryChest m_Chest;
			public ConfirmUseKeyGump(PlayerMobile from, TreasuryChest chest)
				: base(50, 50)
			{
				m_From = from;
				m_Chest = chest;
				m_From.CloseGump(typeof(ConfirmUseKeyGump));

				AddPage(0);

				AddBackground(0, 0, 270, 120, 5054);
				AddBackground(10, 10, 250, 100, 3000);

				AddHtml(20, 15, 230, 60, String.Format("Do you want to use 100 treasury keys to open this chest?<br>You own {0} treasury keys.", from.TreasuryKeys), true, true);

				AddButton(20, 80, 4005, 4007, 2, GumpButtonType.Reply, 0);
				AddHtmlLocalized(55, 80, 75, 20, 1011011, false, false); // CONTINUE

				AddButton(135, 80, 4005, 4007, 1, GumpButtonType.Reply, 0);
				AddHtmlLocalized(170, 80, 75, 20, 1011012, false, false); // CANCEL
			}

			public override void OnResponse(Server.Network.NetState sender, RelayInfo info)
			{
				if (info.ButtonID == 2)
				{
					Mobile m = m_From;
					PlayerMobile pm = m as PlayerMobile;

					if (pm == null || m_Chest == null || m_Chest.Town == null || !pm.IsInMilitia)
					{
						return;
					}

					double dist_to_chest = Math.Pow(pm.X - m_Chest.X, 2) + Math.Pow(pm.Y - m_Chest.Y, 2);
					if (dist_to_chest > 4.0)
					{
						pm.SendLocalizedMessage(500446); // That is too far away
					}
					else if (pm.Citizenship == m_Chest.Town)
					{
						m.SendMessage("You decide against looting your own treasury.");
					}
                    else if (m_Chest.Town.ControllingTown != pm.Citizenship)
                    {
                        m.SendMessage("Your town must control the town in order to loot its treasury!");
                    }
					else if (m_Chest.Town.TreasuryWallType != TreasuryWallTypes.None)
					{
						m.SendMessage("The strong magic invested in the treasury walls protects the treasury.");
					}
					else if (pm.TownsystemPlayerState != null && pm.TownsystemPlayerState.JoinedWithinMinutes(5))
					{
						m.SendMessage("You can not do that yet");
					}
					else if (m_Chest.m_Thieves.Contains(pm.Serial))
					{
						m.SendMessage("You can only loot a towns treasury once per day.");
					}
					else if (pm.TreasuryKeys < 100)
					{
						m.SendMessage("You need 100 treasury keys to open this chest");
					}
					else
					{
						pm.TreasuryKeys -= 100;
						LootRandomItem(pm);
						pm.SendMessage("You quickly open the chest and take an item");
						if( pm.AccessLevel < AccessLevel.GameMaster)
							m_Chest.m_Thieves.Add(pm.Serial);

						// IPY ACHIEVEMENT
						Server.Achievements.AchievementSystem.Instance.TickProgress(m, Server.Achievements.AchievementTriggers.Trigger_StealEnemyTreasury);
						// IPY ACHIEVEMENT
					}
				}
			}
		}

		private static void LootRandomItem(PlayerMobile pm)
		{
			Item item = null;
			switch (Utility.Random(50))
			{
				case 0: item = new TreasureMap(2+Utility.Random(4), Map.Felucca);
					break;
				case 1: item = new RareFoldedSheets();
					break;
				case 2: item = new RareNecroScroll();
					break;
				case 3: item = new Item(0x3B10) { Name = "a toy" }; // sea horse "statue"
					break;
				case 4: item = new MetalGoldenChest() { Hue = 1755 };
					break;
				case 5: item = new RunebookDyeTub() { UsesRemaining = Utility.Random(1, 3) };
					break;
				case 6: 
					item = new Sandals() { Hue = Utility.RandomBool() ? Utility.RandomMetalHue() : Utility.RandomSlimeHue() };
					if (Utility.Random(50) == 0) // one in 2500
						item.Hue = 1109; // muy populare
					break;
				case 7:		/* 1 in 5 for weapon */
				case 8:		/* 1 in 5 for weapon */
				case 9:		/* 1 in 5 for weapon */
				case 10:	/* 1 in 5 for weapon */
				case 11:	/* 1 in 5 for weapon */
				case 12:	/* 1 in 5 for weapon */
				case 13:	/* 1 in 5 for weapon */
				case 14:	/* 1 in 5 for weapon */
				case 15:	/* 1 in 5 for weapon */
				case 16:	/* 1 in 5 for weapon */
					item = Loot.RandomWeapon();
					((BaseWeapon)item).DamageLevel = (WeaponDamageLevel)Utility.Random(6); // up to vanq
					((BaseWeapon)item).AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
					((BaseWeapon)item).DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
					if (Utility.Random(100) == 0) // 1 in 3000 silver vanq
						((BaseWeapon)item).Slayer = SlayerName.Silver;
					break;
				default:
					item = Loot.RandomArmorOrShield();
					((BaseArmor)item).ProtectionLevel = (ArmorProtectionLevel)Utility.Random(6);
					((BaseArmor)item).Durability = (ArmorDurabilityLevel)Utility.Random(6);
					break;
			}
			if (item != null && pm.Backpack != null)
			{
				pm.Backpack.AddItem(item);
			}
		}

        public static bool TreasuryRegionContains(Town town, Point3D p)
        {
            if (town == null)
                return false;

            for (int i = 0; i < town.Definition.TreasuryRects.Length; i++)
            {
                if (town.Definition.TreasuryRects[i].Contains(p))
                    return true;
            }

            return false;
        }

        public TreasuryChest( Town town ) 
			: base( 0xE41 )
		{
			Movable = false;
			Town = town;
            ItemID = Town.Definition.TreasuryID;
			s_AllTreasuryChests.Add(this);
			m_Thieves = new HashSet<Serial>();
		}

        public TreasuryChest(Serial serial)
            : base(serial)
		{
			s_AllTreasuryChests.Add(this);
			m_Thieves = new HashSet<Serial>();
		}

		public override void OnDelete()
		{
			s_AllTreasuryChests.Remove(this);
			base.OnDelete();
		}

		public static void ClearThiefList()
		{
			foreach (TreasuryChest chest in s_AllTreasuryChests)
			{
				chest.m_Thieves.Clear();
			}
		}

		public override bool OnDragDrop(Mobile from, Item dropped)
		{
			if (dropped is TreasuryCheck || dropped is Gold || dropped is BankCheck)
			{
				int amount = 0;
				if (from.InRange(Location, 3))
				{
					if (dropped is TreasuryCheck)
					{
						var check = dropped as TreasuryCheck;
						amount = check.GoldWorth;
					}
					else if (dropped is Gold)
					{
						var gold = dropped as Gold;
						amount = gold.Amount;
					}
					else if (dropped is BankCheck)
					{
						var check = dropped as BankCheck;
						amount = check.Worth;
					}

					TreasuryValue += amount;
					from.SendMessage(String.Format("You have deposited {0} gold into the {1} treasury.", amount, Town.Definition.FriendlyName));
					Town.WithdrawLog.AddLogEntry(from, amount, TreasuryLogType.CheckDeposit);
					dropped.Delete();
				}
			}

			return base.OnDragDrop(from, dropped);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version

			// version 3
			writer.Write((int)m_Thieves.Count);
			foreach (Serial s in m_Thieves)
				writer.Write(s.Value);

            // version 0
			Town.WriteReference( writer, m_Town );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			if (version >= 2)
			{
				m_Thieves = new HashSet<Serial>();
				switch (version)
				{
					case 3:
					{
						int num_thieves = reader.ReadInt();
						for (int i = 0; i < num_thieves; ++i)
						{
							Serial s = reader.ReadInt();
							m_Thieves.Add(s);
						}
						goto case 2;
					}
					case 2:
					{
						m_Town = Town.ReadReference(reader);
						break;
					}

				}
			}
			else
			{	// pre version 2
				switch (version)
				{
					case 1:
					{
						reader.ReadMobile();
						goto case 0;
					}
					case 0:
					{
						m_Town = Town.ReadReference(reader);
						reader.ReadDateTime();

						break;
					}
				}
			}
		}
	}
}