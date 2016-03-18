using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Network;
using Server.ContextMenus;
using Server.Engines.PartySystem;

namespace Server.Items
{
	[Flipable]
	public class ParagonChest : LockableContainer
	{
		public override int LabelNumber{ get{ return 3000541; } }

		private static int[] m_ItemIDs = new int[]
		{
			0x9AB, 0xE40, 0xE41, 0xE7C
		};

		private static int[] m_Hues = new int[]
		{
			0x0, 0x455, 0x47E, 0x89F, 0x8A5, 0x8AB, 
			0x966, 0x96D, 0x972, 0x973, 0x979,   
		};

		private string m_Name;

		[Constructable]
		public ParagonChest( string name, int level ) : base( Utility.RandomList( m_ItemIDs ) )
		{
			m_Name = name;
			Hue = Utility.RandomList( m_Hues );
			Fill( level );
		}

		public override void OnSingleClick( Mobile from )
		{
			base.OnSingleClick(from);
			LabelTo( from, 1063449, m_Name );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1063449, m_Name );
		}

		public void Flip()
		{
			switch ( ItemID )
			{
				case 0x9AB : ItemID = 0xE7C; break;
				case 0xE7C : ItemID = 0x9AB; break;
				
				case 0xE40 : ItemID = 0xE41; break;
				case 0xE41 : ItemID = 0xE40; break;
			}
		}

		private void Fill( int level )
		{
			TrapType = TrapType.ExplosionTrap;
			TrapPower = level * 25;
			TrapLevel = level;
			Locked = true;

			switch ( level )
			{
				case 1: RequiredSkill = 36; break;
				case 2: RequiredSkill = 76; break;
				case 3: RequiredSkill = 84; break;
				case 4: RequiredSkill = 92; break;
				case 5: RequiredSkill = 100; break;
			}

			LockLevel = RequiredSkill - 10;
			MaxLockLevel = RequiredSkill + 40;

			DropItem( new Gold( level * 200 ) );

            int chance = 610 - (100 * level);
            if (Utility.Random(chance) == 0)
                DropItem(new RareCloth());

			for ( int i = 0; i < level; ++i )
                DropItem(Loot.RandomScroll(0, Math.Min(47, (level + 1) * 8 - 1), SpellbookType.Regular));

			for ( int i = 0; i < level * 2; ++i )
			{
                int maxMod = Math.Min(5, level + (Utility.RandomDouble() < 0.33 ? 1 : 0));

                if (Utility.RandomDouble() < 0.60)
                {
                    BaseWeapon weapon = Loot.RandomWeapon();
                    weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(1, maxMod);
                    weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);

                    DropItem(weapon);
                }
                else
                {
                    BaseArmor armor = Loot.RandomArmorOrShield();
                    armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(1, maxMod);
                    armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);

                    DropItem(armor);
                }
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomPossibleReagent();
				item.Amount = Utility.RandomMinMax( 10, 20 );
				DropItem( item );
			}

			for ( int i = 0; i < level; i++ )
			{
				Item item = Loot.RandomGem();
				DropItem( item );
			}

			DropItem( new TreasureMap( Math.Min(6, level + 1), ( Map.Felucca ) ) );
		}

		public ParagonChest( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_Name );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			m_Name = Utility.Intern( reader.ReadString() );
		}
	}
}
