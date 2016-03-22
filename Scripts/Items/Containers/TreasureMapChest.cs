using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Engines.PartySystem;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.Custom;


namespace Server.Items
{
	public class TreasureMapChest : LockableContainer
	{
		public override int LabelNumber{ get{ return 3000541; } }

		public static Type[] Artifacts { get { return m_Artifacts; } }

		private static Type[] m_Artifacts = new Type[]
		{
			/* typeof( CandelabraOfSouls ), typeof( GoldBricks ), typeof( PhillipsWoodenSteed ),
			typeof( ArcticDeathDealer ), typeof( BlazeOfDeath ), typeof( BurglarsBandana ),
			typeof( CavortingClub ), typeof( DreadPirateHat ),
			typeof( EnchantedTitanLegBone ), typeof( GwennosHarp ), typeof( IolosLute ),
			typeof( LunaLance ), typeof( NightsKiss ), typeof( NoxRangersHeavyCrossbow ),
			typeof( PolarBearMask ), typeof( VioletCourage ), typeof( HeartOfTheLion ),
			typeof( ColdBlood ), typeof( AlchemistsBauble ) */
		};

		private int m_Level;
		private DateTime m_DeleteTime;
		private Timer m_Timer;
		private Mobile m_Owner;
		private bool m_Temporary;

		private List<Mobile> m_Guardians;

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level{ get{ return m_Level; } set{ m_Level = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner{ get{ return m_Owner; } set{ m_Owner = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime DeleteTime{ get{ return m_DeleteTime; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Temporary{ get{ return m_Temporary; } set{ m_Temporary = value; } }

		public List<Mobile> Guardians { get { return m_Guardians; } }

		[Constructable]
		public TreasureMapChest( int level ) : this( null, level, false )
		{
		}

		public TreasureMapChest( Mobile owner, int level, bool temporary ) : base( 0xE40 )
		{
			m_Owner = owner;
			m_Level = level;
			m_DeleteTime = DateTime.UtcNow + TimeSpan.FromHours( 3.0 );

			m_Temporary = temporary;
			m_Guardians = new List<Mobile>();

			m_Timer = new DeleteTimer( this, m_DeleteTime );
			m_Timer.Start();

			Fill( this, level );
		}

        private static int[] RegsPerLevel = new int[7] 
        {
            4,
            4,
            7,
            10,
            14,
            17,
            20
        };

        private static int[] ItemsPerLevel = new int[7]
		{
			3,
			5,
			10,
			15,
			20,
			25,
			30,
		};

        private static int[] MaxModPerLevel = new int[7]
        {
            1,
            1,
            2,
            3,
            4,
            5,
            5
        };

        private static int[] GemsPerLevel = new int[7]
        {
            5,
            5,
            5,
            5,
            6,
            7,
            7
        };

        private static double[] SuperSlayerChancePerLevel = new double[7]
        {
            0.00,
            0.00,
            0.00,
            0.00,
            0.05,
            0.10,
            0.10,
        };

        public static double[] RemoveTrapSkillRequiredPerLevel = new double[7]
        {
            25.0,
            35.0,
            50.0,
            75.0,
            90.0,
            98.0,
            99.0
        };

        public static double[] RemoveTrapSkillMaxPerLevel = new double[7]
        {
            35.0,
            55.0,
            80.0,
            90.0,
            95.0,
            100.0,
            110.0
        };       

        private static double[] RareChancePerLevel = new double[7]
        {
            0.05,
            0.10,
            0.15,
            0.20,
            0.25,
            0.30,
            0.35
        };

		public static void Fill( LockableContainer cont, int level )
		{
			cont.Movable = false;
			cont.Locked = true;

			if ( level == 0 )
			{
				cont.LockLevel = 0; // Can't be unlocked

				cont.DropItem( new Gold( Utility.RandomMinMax( 200, 300 ) ) );

				if ( Utility.RandomDouble() < 0.75 )
					cont.DropItem( new TreasureMap( 0, Map.Felucca ) );
			}
			else
			{
				cont.TrapType = TrapType.ExplosionTrap;
				cont.TrapPower = level * 25;
				cont.TrapLevel = level;

				switch ( level )
				{
					case 1: cont.RequiredSkill = 35; break;
					case 2: cont.RequiredSkill = 50; break;
					case 3: cont.RequiredSkill = 75; break;
					case 4: cont.RequiredSkill = 90; break;
					case 5: cont.RequiredSkill = 98; break;
					case 6: cont.RequiredSkill = 100; break;
				}

				cont.LockLevel = cont.RequiredSkill - 10;
				cont.MaxLockLevel = cont.RequiredSkill + 40;

                var gold = new Gold(level * 500);
                if (level == 6)
                    gold.Amount = 5000;

                cont.DropItem(gold);

                int reagents = level == 0 ? 4 : 8;

                for (int i = 0; i < reagents; i++)
                {
                    Item item = Loot.RandomPossibleReagent();
                    item.Amount = RegsPerLevel[level];
                    cont.DropItem(item);
                }

                for (int i = 0; i < level * 3; ++i)
                    cont.DropItem(Loot.RandomScroll(0, Math.Min(47, (level + 1) * 8 - 1), SpellbookType.Regular));

                int minimumMod = 1;
                int maximumMod = MaxModPerLevel[level];

                //Luthius Expansion                
                double craftingComponentLoops = 4;
                double prestigeScrollLoops = 3;
                double spellHueDeedLoops = 1;

                double craftingComponentChance = .02;
                double prestigeScrollChance = .1;
                double spellHueDeedChance = .005;

                switch (level)
                {
                    case 1:
                        craftingComponentChance = .10;
                        prestigeScrollChance = .1;
                        spellHueDeedChance = .005;
                    break;

                    case 2:  
                        craftingComponentChance = .15;
                        prestigeScrollChance = .2;
                        spellHueDeedChance = .01;
                    break;

                    case 3:
                        craftingComponentChance = .20;
                        prestigeScrollChance = .3;
                        spellHueDeedChance = .015;
                    break;

                    case 4:
                        craftingComponentChance = .25;
                        prestigeScrollChance = .4;
                        spellHueDeedChance = .02;
                    break;

                    case 5:
                        craftingComponentChance = .30;
                        prestigeScrollChance = .5;
                        spellHueDeedChance = .025;
                    break;

                    case 6:
                        craftingComponentChance = .40;
                        prestigeScrollChance = .6;
                        spellHueDeedChance = .03;
                    break;
                }                

                for (int a = 0; a < craftingComponentLoops; a++)
                {
                    if (Utility.RandomDouble() <= craftingComponentChance)
                        cont.DropItem(CraftingComponent.GetRandomCraftingComponent(1));
                }

                for (int a = 0; a < prestigeScrollLoops; a++)
                {
                    if (Utility.RandomDouble() <= prestigeScrollChance)
                        cont.DropItem(new PrestigeScroll());
                }

                for (int a = 0; a < spellHueDeedLoops; a++)
                {
                    if (Utility.RandomDouble() <= spellHueDeedChance)
                        cont.DropItem(new SpellHueDeed());
                }                

				for ( int i = 0; i < ItemsPerLevel[level]; ++i )
				{
                    if (Utility.RandomDouble() < 0.30)
                    {
                        BaseWeapon weapon = Loot.RandomWeapon();
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(minimumMod, maximumMod);
                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(minimumMod, maximumMod);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(minimumMod, maximumMod);
                        
                        cont.DropItem(weapon);
                    }
                    else
                    {
                        BaseArmor armor = Loot.RandomArmorOrShield();
                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(minimumMod, maximumMod);
                        armor.Durability = (ArmorDurabilityLevel)Utility.RandomMinMax(minimumMod, maximumMod);

                        cont.DropItem(armor);
                    }
				}
			}

            for (int i = 0; i < level; i++)
            {
                Item item = Loot.RandomGem();
                item.Amount = GemsPerLevel[level];
                cont.DropItem(item);
            }

            // rares
            if (level == 6 && Utility.Random(50) == 0)
                cont.DropItem(new SpecialHairDye());

            switch(level)
            {
                case 6:
                    if (Utility.RandomDouble() < 0.09)
                        cont.DropItem(PowerScroll.CreateRandom(5, 10));
                    break;
                case 5:
                    if (Utility.RandomDouble() < 0.06)
                        cont.DropItem(new RareSandals());
                    break;
                case 4:
                    if (Utility.RandomDouble() < 0.03)
                        cont.DropItem(new BossCloth());
                    break;
                default:
                    if (Utility.RandomDouble() < RareChancePerLevel[level])
                    {
                        int index = Utility.Random(LootPack.RareCraftingIngredients.Length);
                        var rare = LootPack.RareCraftingIngredients[index].Construct();
                        cont.DropItem(rare);
                    }
                    break;

            }
            
            int chance = 610 - (100 * level);
            if (Utility.Random(chance) == 0)
                cont.DropItem(new RareCloth());
		}

		public override bool CheckLocked( Mobile from )
		{
			if ( !this.Locked )
				return false;

			if ( this.Level == 0 && from.AccessLevel < AccessLevel.GameMaster )
			{
				foreach ( Mobile m in this.Guardians )
				{
					if ( m.Alive )
					{
						from.SendLocalizedMessage( 1046448 ); // You must first kill the guardians before you may open this chest.
						return true;
					}
				}

				LockPick( from );
				return false;
			}
			else
			{
				return base.CheckLocked( from );
			}
		}

		private List<Item> m_Lifted = new List<Item>();

		private bool CheckLoot( Mobile m, bool criminalAction )
		{
			if ( m_Temporary )
				return false;

			if ( m.AccessLevel >= AccessLevel.GameMaster || m_Owner == null || m == m_Owner )
				return true;

			Party p = Party.Get( m_Owner );

			if ( p != null && p.Contains( m ) )
				return true;

			Map map = this.Map;

			if ( map != null && (map.Rules & MapRules.HarmfulRestrictions) == 0 )
			{
				if ( criminalAction )
					m.CriminalAction( true );
				else
					m.SendLocalizedMessage( 1010630 ); // Taking someone else's treasure is a criminal offense!

				return true;
			}

			m.SendLocalizedMessage( 1010631 ); // You did not discover this chest!
			return false;
		}

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		public override bool CheckItemUse( Mobile from, Item item )
		{
			return CheckLoot( from, item != this ) && base.CheckItemUse( from, item );
		}

		public override bool CheckLift( Mobile from, Item item, ref LRReason reject )
		{
			return CheckLoot( from, true ) && base.CheckLift( from, item, ref reject );
		}

		public override void OnItemLifted( Mobile from, Item item )
		{
			bool notYetLifted = !m_Lifted.Contains( item );

			from.RevealingAction();

			if ( notYetLifted && !(item is Gold && item.Amount < 300))
			{
				m_Lifted.Add( item );

				if ( 0.1 >= Utility.RandomDouble() ) // 10% chance to spawn a new monster
					TreasureMap.Spawn( m_Level, GetWorldLocation(), Map, from, false );
			}

			base.OnItemLifted( from, item );
		}

		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( m.AccessLevel < AccessLevel.GameMaster )
			{
				m.SendLocalizedMessage( 1048122, "", 0x8A5 ); // The chest refuses to be filled with treasure again.
				return false;
			}

			return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
		}

		public TreasureMapChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( m_Guardians, true );
			writer.Write( (bool) m_Temporary );

			writer.Write( m_Owner );

			writer.Write( (int) m_Level );
			writer.WriteDeltaTime( m_DeleteTime );
			writer.Write( m_Lifted, true );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					m_Guardians = reader.ReadStrongMobileList();
					m_Temporary = reader.ReadBool();

					goto case 1;
				}
				case 1:
				{
					m_Owner = reader.ReadMobile();

					goto case 0;
				}
				case 0:
				{
					m_Level = reader.ReadInt();
					m_DeleteTime = reader.ReadDeltaTime();
					m_Lifted = reader.ReadStrongItemList();

					if ( version < 2 )
						m_Guardians = new List<Mobile>();

					break;
				}
			}

			if ( !m_Temporary )
			{
				m_Timer = new DeleteTimer( this, m_DeleteTime );
				m_Timer.Start();
			}
			else
			{
				Delete();
			}
		}

		public override void OnAfterDelete()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;

			base.OnAfterDelete();
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			if ( from.Alive )
				list.Add( new RemoveEntry( from, this ) );
		}

		public void BeginRemove( Mobile from )
		{
			if ( !from.Alive )
				return;

			from.CloseGump( typeof( RemoveGump ) );
			from.SendGump( new RemoveGump( from, this ) );
		}

		public void EndRemove( Mobile from )
		{
			if ( Deleted || from != m_Owner || !from.InRange( GetWorldLocation(), 3 ) )
				return;

			from.SendLocalizedMessage( 1048124, "", 0x8A5 ); // The old, rusted chest crumbles when you hit it.
			this.Delete();
		}

		private class RemoveGump : Gump
		{
			private Mobile m_From;
			private TreasureMapChest m_Chest;

			public RemoveGump( Mobile from, TreasureMapChest chest ) : base( 15, 15 )
			{
				m_From = from;
				m_Chest = chest;

				Closable = false;
				Disposable = false;

				AddPage( 0 );

				AddBackground( 30, 0, 240, 240, 2620 );

				AddHtmlLocalized( 45, 15, 200, 80, 1048125, 0xFFFFFF, false, false ); // When this treasure chest is removed, any items still inside of it will be lost.
				AddHtmlLocalized( 45, 95, 200, 60, 1048126, 0xFFFFFF, false, false ); // Are you certain you're ready to remove this chest?

				AddButton( 40, 153, 4005, 4007, 1, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 155, 180, 40, 1048127, 0xFFFFFF, false, false ); // Remove the Treasure Chest

				AddButton( 40, 195, 4005, 4007, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 75, 197, 180, 35, 1006045, 0xFFFFFF, false, false ); // Cancel
			}

			public override void OnResponse( NetState sender, RelayInfo info )
			{
				if ( info.ButtonID == 1 )
					m_Chest.EndRemove( m_From );
			}
		}

		private class RemoveEntry : ContextMenuEntry
		{
			private Mobile m_From;
			private TreasureMapChest m_Chest;

			public RemoveEntry( Mobile from, TreasureMapChest chest ) : base( 6149, 3 )
			{
				m_From = from;
				m_Chest = chest;

				Enabled = ( from == chest.Owner );
			}

			public override void OnClick()
			{
				if ( m_Chest.Deleted || m_From != m_Chest.Owner || !m_From.CheckAlive() )
					return;

				m_Chest.BeginRemove( m_From );
			}
		}

		private class DeleteTimer : Timer
		{
			private Item m_Item;

			public DeleteTimer( Item item, DateTime time ) : base( time - DateTime.UtcNow )
			{
				m_Item = item;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Item.Delete();
			}
		}
	}
}
