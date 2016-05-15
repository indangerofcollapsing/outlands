using Server;
using Server.Items;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Custom;

namespace Server.Items
{
	public class BaseTreasureChest : LockableContainer
	{
		private TreasureLevel m_TreasureLevel;
		private short m_MaxSpawnTime = 60;
		private short m_MinSpawnTime = 30;
		private short s_AbsoluteMinSpawntime = 30;
		private TreasureResetTimer m_ResetTimer;
        private bool m_UsedCaptcha = false;

		[CommandProperty( AccessLevel.GameMaster )]
		public TreasureLevel Level
		{
			get
			{
				return m_TreasureLevel;
			}
			set
			{
                m_TreasureLevel = value; 
                SetLockLevel(); 
                Reset();
			}
		}


        [CommandProperty(AccessLevel.GameMaster)]
        public bool UsedCaptcha
        {
            get
            {
                return m_UsedCaptcha;
            }
            set
            {
                m_UsedCaptcha = value;
            }
        }

		[CommandProperty( AccessLevel.GameMaster )]
		public short MaxSpawnTime
		{
			get
			{
				return m_MaxSpawnTime;
			}
			set
			{
				m_MaxSpawnTime = value;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public short MinSpawnTime
		{
			get
			{
				return m_MinSpawnTime;
			}
			set
			{
				m_MinSpawnTime = Math.Max(value, s_AbsoluteMinSpawntime);
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public override bool Locked {
			get { return base.Locked; }
			set {
				if ( base.Locked != value ) {
					base.Locked = value;
					
					if ( !value )
						StartResetTimer();
				}
			}
		}

		public static void Initialize()
		{
			CommandSystem.Register("RespawnAllTreasureChest", AccessLevel.Administrator, new CommandEventHandler(RespawnAllTreasureChest_OnCommand));
		}

		private static void RespawnAllTreasureChest_OnCommand(CommandEventArgs e)
		{
			List<BaseTreasureChest> spawners = new List<BaseTreasureChest>();
			foreach (Item item in World.Items.Values)
			{
				BaseTreasureChest s = item as BaseTreasureChest;
				if (s != null)
					spawners.Add(s);
			}
			foreach (BaseTreasureChest c in spawners)
				c.Level = c.Level; // "reset"
		}

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (TrapPower != 0)
            {
                reject = LRReason.CannotLift;
                return false;
            }

            return base.CheckLift(from, item, ref reject);
        }

		public override bool IsDecoContainer
		{
			get{ return false; }
		}

		public BaseTreasureChest( int itemID ) : this( itemID, TreasureLevel.Level2 )
		{
		}

		public BaseTreasureChest( int itemID, TreasureLevel level ) : base( itemID )
		{
			m_TreasureLevel = level;
			Locked = true;
			Movable = false;

			SetLockLevel();
			GenerateTreasure();
		}

		public BaseTreasureChest( Serial serial ) : base( serial )
		{
		}

		public override string DefaultName
		{
			get
			{
				if ( this.Locked )
					return "a locked treasure chest";

				return "a treasure chest";
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
            // version 1
            writer.Write( m_UsedCaptcha );
            // version 0
			writer.Write( (byte) m_TreasureLevel );
			writer.Write( m_MinSpawnTime );
			writer.Write( m_MaxSpawnTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version > 0)
            {
                m_UsedCaptcha = reader.ReadBool();
            }

			m_TreasureLevel = (TreasureLevel)reader.ReadByte();
			m_MinSpawnTime = Math.Max(reader.ReadShort(), (short)s_AbsoluteMinSpawntime);
			m_MaxSpawnTime = reader.ReadShort();

			if( !Locked )
				StartResetTimer();
		}

        private void CaptchaDump(PlayerMobile from) 
        {
          if (!from.HarvestLockedout)
          {
              from.TempStashedHarvest = this;
              for(int i = 0; i < Items.Count; i++)
              {
                  Item item = Items[i];
                  item.Movable = false;
              }
          }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                base.OnDoubleClick(from);
                return;
            }

            if (!Locked && TrapPower == 0 && Items.Count > 0 && from is PlayerMobile && from.Alive && !UsedCaptcha)
            {
                CaptchaDump(from as PlayerMobile);
            }
            else
            {
                base.OnDoubleClick(from);
            }
        }

        public override void OnTelekinesis(Mobile from)
        {
            Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5022 );
			Effects.PlaySound( Location, Map, 0x1F5 );

            if (Level == TreasureLevel.Level4)
            {
                if (Utility.RandomBool())
                {
                    if (!ExecuteTrap(from))
                    {
                      if (Items.Count > 0 && from is PlayerMobile && from.Alive && !UsedCaptcha)
                      {
                          CaptchaDump(from as PlayerMobile);
                      }
                      else
                      {
                          base.DisplayTo(from);
                      }
                    }
                }
                else
                    from.SendMessage("Your magic fails to open the trap.");
            }
            else if (Level >= TreasureLevel.Level5) // no chance to pop trap
                from.SendMessage("Your magic fails to open a trap of this strength.");
            else if (!ExecuteTrap(from)) //!this.TrapOnOpen ||
            {
                if (Items.Count > 0 && from is PlayerMobile && from.Alive && !UsedCaptcha)
                {
                    CaptchaDump(from as PlayerMobile);
                }
                else
                {
                    base.DisplayTo(from);
                }
            }
        }

		protected virtual void SetLockLevel()
		{
			TrapType = TrapType.ExplosionTrap;
			switch( m_TreasureLevel )
			{
				case TreasureLevel.Level1:
					this.RequiredSkill = this.LockLevel = 28;
					TrapPower = Utility.Random(1, 10);
					break;

				case TreasureLevel.Level2:
					this.RequiredSkill = this.LockLevel = 36;
					TrapPower = Utility.Random(5, 20);
					break;

				case TreasureLevel.Level3:
					this.RequiredSkill = this.LockLevel = 81;
					TrapPower = Utility.Random(20, 60);					
					break;

				case TreasureLevel.Level4:
					this.RequiredSkill = this.LockLevel = 87;
					TrapPower = Utility.Random(40, 110);					
					break;

				case TreasureLevel.Level5:
					this.RequiredSkill = this.LockLevel = 92;
					TrapPower = Utility.Random(100, 220);
					break;

				case TreasureLevel.Level6:
					this.RequiredSkill = this.LockLevel = 100;
					TrapPower = Utility.Random(210, 350);
					break;
			}
		}

		private void StartResetTimer()
		{
			if( m_ResetTimer == null )
				m_ResetTimer = new TreasureResetTimer( this );
			else
				m_ResetTimer.Delay = TimeSpan.FromMinutes( Utility.Random( m_MinSpawnTime, m_MaxSpawnTime ));

			m_ResetTimer.Start();
		}

		protected virtual void GenerateTreasure()
		{
			int MinGold = 1;
			int MaxGold = 2;

			int minRegs = 1;
			int maxRegs = 2;

            double craftingComponentChance = .02;
            double prestigeScrollChance = .01;
            double researchMaterialsChance = .005;

			switch( m_TreasureLevel )
			{
				case TreasureLevel.Level1:
					minRegs = 1;
					maxRegs = 2;
					MinGold = 33;
					MaxGold = 100;

                    craftingComponentChance = .02;
                    prestigeScrollChance = .01;
                    researchMaterialsChance = .005;
				break;

				case TreasureLevel.Level2:
					minRegs = 1;
                    maxRegs = 3;
					MinGold = 75;
					MaxGold = 225;

                    craftingComponentChance = .04;
                    prestigeScrollChance = .02;
                    researchMaterialsChance = .01;
				break;

				case TreasureLevel.Level3:
					minRegs = 2;
                    maxRegs = 6;
					MinGold = 225;
					MaxGold = 335;

                    craftingComponentChance = .08;
                    prestigeScrollChance = .04;
                    researchMaterialsChance = .02;
				break;

				case TreasureLevel.Level4:
					minRegs = 4;
                    maxRegs = 12;
					MinGold = 600;
					MaxGold = 900;

                    craftingComponentChance = .10;
                    prestigeScrollChance = .05;
                    researchMaterialsChance = .025;
				break;

				case TreasureLevel.Level5:
					minRegs = 4;
                    maxRegs = 18;
					MinGold = 1000;
					MaxGold = 1500;

                    craftingComponentChance = .15;
                    prestigeScrollChance = .075;
                    researchMaterialsChance = .0375;
				break;

				case TreasureLevel.Level6:
					minRegs = 8;
                    maxRegs = 20;
					MinGold = 1600;
					MaxGold = 2200;

                    craftingComponentChance = .20;
                    prestigeScrollChance = .10;
                    researchMaterialsChance = .05;
				break;
			}

			//potion
            AddRare((int)m_TreasureLevel + 1, 0.75, Loot.PotionTypes); //0.75% chance per level for a potion

			// gold
			DropItem( new Gold( MinGold, MaxGold ) );

            // gems
            AddGems((int)m_TreasureLevel * 2);
            AddGems((int)m_TreasureLevel * 2);

			// reagents
			Item ReagentLoot = Loot.RandomReagent();
			ReagentLoot.Amount = Utility.Random(minRegs, maxRegs-minRegs);
			DropItem(ReagentLoot);

			// clothing
			for (int i = Utility.Random(1, 2); i > 1; i--)
				DropItem(Loot.RandomClothing());
         
            //Crafting Component Scroll
            if (Utility.RandomDouble() <= craftingComponentChance)
                DropItem(CraftingComponent.GetRandomCraftingComponent(1));

            //Prestige Scroll
            if (Utility.RandomDouble() <= prestigeScrollChance)
                DropItem(new PrestigeScroll());

            //Research Materials
            if (Utility.RandomDouble() <= researchMaterialsChance)
                DropItem(new ResearchMaterials());            

            // special cloth hues
            if (Level >= TreasureLevel.Level5 && Utility.Random(25) == 0)
                DropItem(new RareCloth());

			// lvl3+ has chance for weapon & armor
			if ((int)m_TreasureLevel >= (int)TreasureLevel.Level3 )
			{
				int level = (int)m_TreasureLevel;
				// Equipment
				for (int i = Utility.Random(1, level-1); i > 1; i--)
				{
                    Item item = Utility.RandomBool() ? Loot.RandomWeapon() as Item : Loot.RandomArmorOrShield() as Item;				
					
					if (item is BaseWeapon)
					{
						BaseWeapon weapon = (BaseWeapon)item;
						weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(level);
						weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(level);
						weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(level);
					}

					else if (item is BaseArmor)
					{
						BaseArmor armor = (BaseArmor)item;
						armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(level);
						armor.DurabilityLevel = (ArmorDurabilityLevel)Utility.Random(level);
                        armor.Quality = Quality.Regular;
					}

					DropItem(item);
				}
			}
		}

        public void AddGems(int amount)
        {
            if (amount <= 0)
                return;

            Item gem = Loot.RandomGem();

            gem.Amount = amount;

            DropItem(gem);
        }

        public void AddRare(int quantity, double chance, params Type[][] types)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (Utility.RandomDouble() < chance)
                {
                    Item item = Loot.Construct(types);
                    DropItem(item);
                }
            }
        }

        public void AddRare(int quantity, double chance, params Type[] type)
        {
            for (int i = 0; i < quantity; i++)
            {
                if (Utility.RandomDouble() < chance)
                {
                    Item item = Loot.Construct(type);
                    DropItem(item);
                }
            }
        }

		public void ClearContents()
		{
			for ( int i = Items.Count - 1; i >= 0; --i )
			{
				if ( i < Items.Count )
					Items[i].Delete();
			}
		}

		public void Reset()
		{
			if( m_ResetTimer != null )
			{
				if( m_ResetTimer.Running )
					m_ResetTimer.Stop();
			}

			SetLockLevel();
			Locked = true;
			ClearContents();
			GenerateTreasure();
            UsedCaptcha = false;
		}

		public enum TreasureLevel
		{
			Level1 = 1, 
			Level2, 
			Level3, 
			Level4, 
			Level5,
			Level6,
		}; 

		private class TreasureResetTimer : Timer
		{
			private BaseTreasureChest m_Chest;

			public TreasureResetTimer( BaseTreasureChest chest ) : base ( TimeSpan.FromMinutes( Utility.Random( chest.MinSpawnTime, chest.MaxSpawnTime ) ) )
			{
				m_Chest = chest;
				Priority = TimerPriority.OneMinute;
			}

			protected override void OnTick()
			{
				m_Chest.Reset();
			}
		}
	}
}
