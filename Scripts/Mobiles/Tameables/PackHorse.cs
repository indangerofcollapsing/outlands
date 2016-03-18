using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;

namespace Server.Mobiles
{
	[CorpseName( "a horse corpse" )]
	public class PackHorse : BaseCreature
	{
		public override bool DropsGold { get { return false; } }
		public override double MaxSkillScrollWorth { get { return 0.0; } }
		[Constructable]
		public PackHorse() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a pack horse";
			Body = 291;
			BaseSoundID = 0xA8;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(2, 4);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 0;
			Karma = 200;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 0.0;

			Container pack = Backpack;

			if ( pack != null )
				pack.Delete();

			pack = new StrongBackpack();
			pack.Movable = false;

			AddItem( pack );
		}

        public override int MaxExperience { get { return 50; } }

        public override int Meat { get { return 3; } }
        public override int Hides { get { return 10; } }

        //Animal Lore Display Info
        public override int TamedItemId { get { return 8486; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 10; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 150; } }
        public override int TamedBaseMinDamage { get { return 4; } }
        public override int TamedBaseMaxDamage { get { return 6; } }
        public override double TamedBaseWrestling { get { return 50; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 25; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 50; } }
        public override int TamedBaseVirtualArmor { get { return 20; } }	
		
		public PackHorse( Serial serial ) : base( serial )
		{
		}

		#region Pack Animal Methods
		public override bool OnBeforeDeath()
		{
			if ( !base.OnBeforeDeath() )
				return false;

			PackAnimal.CombineBackpacks( this );

			return true;
		}

		public override DeathMoveResult GetInventoryMoveResultFor( Item item )
		{
			return DeathMoveResult.MoveToCorpse;
		}

		public override bool IsSnoop( Mobile from )
		{
			if ( PackAnimal.CheckAccess( this, from ) )
				return false;

			return base.IsSnoop( from );
		}

		public override bool OnDragDrop( Mobile from, Item item )
		{
			if ( CheckFeed( from, item ) )
				return true;

			if ( PackAnimal.CheckAccess( this, from ) )
			{
				AddToBackpack( item );
				return true;
			}

			return base.OnDragDrop( from, item );
		}

		public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override bool CheckNonlocalLift( Mobile from, Item item )
		{
			return PackAnimal.CheckAccess( this, from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			PackAnimal.TryPackOpen( this, from );
		}

		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );

			PackAnimal.GetContextMenuEntries( this, from, list );
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}

	public class PackAnimalBackpackEntry : ContextMenuEntry
	{
		private BaseCreature m_Animal;
		private Mobile m_From;

		public PackAnimalBackpackEntry( BaseCreature animal, Mobile from ) : base( 6145, 3 )
		{
			m_Animal = animal;
			m_From = from;

			if ( animal.IsDeadPet )
				Enabled = false;
		}

		public override void OnClick()
		{
			PackAnimal.TryPackOpen( m_Animal, m_From );
		}
	}

	public class PackAnimal
	{
		public static void GetContextMenuEntries( BaseCreature animal, Mobile from, List<ContextMenuEntry> list )
		{
			if ( CheckAccess( animal, from ) )
				list.Add( new PackAnimalBackpackEntry( animal, from ) );
		}

		public static bool CheckAccess( BaseCreature animal, Mobile from )
		{
			if ( from == animal || from.AccessLevel >= AccessLevel.GameMaster )
				return true;

			if ( from.Alive && animal.Controlled && !animal.IsDeadPet && ( from == animal.ControlMaster || from == animal.SummonMaster || animal.IsPetFriend( from ) ) )
				return true;

			return false;
		}

		public static void CombineBackpacks( BaseCreature animal )
		{
			if ( Core.AOS )
				return;

			//if ( animal.IsBonded || animal.IsDeadPet )
			//	return;

			Container pack = animal.Backpack;

			if ( pack != null )
			{
				Container newPack = new Backpack();

				for ( int i = pack.Items.Count - 1; i >= 0; --i )
				{
					if ( i >= pack.Items.Count )
						continue;

					newPack.DropItem( pack.Items[i] );
				}

				pack.DropItem( newPack );
			}
		}

		public static void TryPackOpen( BaseCreature animal, Mobile from )
		{
			if ( animal.IsDeadPet )
				return;

			Container item = animal.Backpack;

			if ( item != null )
				from.Use( item );
		}
	}
}