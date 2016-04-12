using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;

namespace Server.Mobiles
{
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

			if ( from.Alive && animal.Controlled && !animal.IsDeadPet && ( from == animal.ControlMaster || from == animal.SummonMaster ) )
				return true;

			return false;
		}

		public static void CombineBackpacks( BaseCreature animal )
		{
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