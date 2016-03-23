using System;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Achievements;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
	public enum RecycleResult
	{
		Success,
		Invalid,
		NoSkill
	}

	public class Recycle
	{
		public Recycle()
		{
		}

		public static void Do( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			int num = craftSystem.CanCraft( from, tool, null );

			if ( num > 0 && num != 1044267 )			
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );			

			else
			{
                CraftContext context = craftSystem.GetContext(from);

                if (context == null)
                    return;

				from.Target = new InternalTarget( craftSystem, tool );

                switch (context.RecycleOption)
                {
                    case CraftRecycleOption.RecycleItem: 
                        from.SendMessage("Target an item to recycle.");
                    break;

                    case CraftRecycleOption.RecycleAllNonExceptional:
                        from.SendMessage("Target the type of item you wish to recycle. All non-exceptional items of that type found in your pack will be processed.");
                    break;

                    case CraftRecycleOption.RecycleAll:
                        from.SendMessage("Target the type of item you wish to recycle. All items of that type found in your pack will be processed.");
                    break;
                }
			}
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

			private RecycleResult Recycle( Mobile from, Item item)
			{
                if (from == null || item == null || m_CraftSystem == null || m_Tool == null)
                    return RecycleResult.Invalid;

                //TEST: Need to Add and Match Recycling Types to The Different Crafting DEFs
                //TEST: Need to Have Items Made With Colored Ingots / Boards / Cloth Return Those Color Resources Back

                CraftContext craftContext = m_CraftSystem.GetContext(from);

                if (craftContext == null)
                    return RecycleResult.Invalid;

                Type itemType = item.GetType();

                CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(itemType); 

                if (craftItem == null || craftItem.Resources.Count == 0)
                    return RecycleResult.Invalid;
                
                Dictionary<Type, int> m_ValidRecipeResources = new Dictionary<Type, int>();

                CraftResCol craftResourceCollection = craftItem.Resources;

                for (int a = 0; a < craftResourceCollection.Count; a++)
                {
                    CraftRes craftResource = craftResourceCollection.GetAt(a);
                    
                    if (!(craftResource.ItemType == typeof(IronIngot) || craftResource.ItemType == typeof(Leather) || craftResource.ItemType == typeof(Board)))                    
                        continue;                    

                    if (craftResource.Amount < 2) 
                        continue;
                
                    if (!m_ValidRecipeResources.ContainsKey(craftResource.ItemType))                    
                        m_ValidRecipeResources.Add(craftResource.ItemType, craftResource.Amount);                    
                }

                if (m_ValidRecipeResources.Count == 0)
                    return RecycleResult.Invalid;

                if (from.Backpack == null)
                    return RecycleResult.Invalid;

                else if (from.Backpack.Deleted)
                    return RecycleResult.Invalid;   

                List<Item> m_ItemsToRecycle = new List<Item>();

                if (craftContext.RecycleOption == CraftRecycleOption.RecycleItem)
                    m_ItemsToRecycle.Add(item);

                Item[] m_MatchingItems = from.Backpack.FindItemsByType(itemType);
                
                for (int a = 0; a < m_MatchingItems.Length; a++)
                {
                    Item targetItem = m_MatchingItems[a];

                    if (craftContext.RecycleOption == CraftRecycleOption.RecycleItem)
                        continue;

                    if (craftContext.RecycleOption == CraftRecycleOption.RecycleAll)
                    {
                        m_ItemsToRecycle.Add(m_MatchingItems[a]);
                        continue;
                    }

                    if (craftContext.RecycleOption != CraftRecycleOption.RecycleAllNonExceptional)
                        continue;

                    BaseWeapon weapon = targetItem as BaseWeapon;
                    BaseArmor armor = targetItem as BaseArmor;
                    BaseClothing clothing = targetItem as BaseClothing;
                    
                    if (weapon != null)
                    {
                        if (weapon.Quality != WeaponQuality.Exceptional)
                            m_ItemsToRecycle.Add(weapon);
                    }

                    else if (armor != null)
                    {
                        if (armor.Quality != ArmorQuality.Exceptional)
                            m_ItemsToRecycle.Add(armor);
                    }

                    else if (clothing != null)
                    {
                        if (clothing.Quality != ClothingQuality.Exceptional)
                            m_ItemsToRecycle.Add(clothing);
                    }

                    else                    
                        m_ItemsToRecycle.Add(targetItem);                    
                }

                if (m_ItemsToRecycle.Count == 0)                
                    return RecycleResult.Invalid;                

                Queue m_Queue = new Queue();

                foreach (Item recycleItem in m_ItemsToRecycle)
                {
                    m_Queue.Enqueue(recycleItem);
                }

                while (m_Queue.Count > 0)
                {
                    Item recycleItem = (Item)m_Queue.Dequeue();

                    foreach (KeyValuePair<Type, int> pair in m_ValidRecipeResources)
                    {
                        Item newResource = (Item)Activator.CreateInstance(pair.Key);

                        if (newResource == null)
                            continue;

                        newResource.Amount = (int)(Math.Floor((double)pair.Value / 2));
                        from.AddToBackpack(newResource);
                    }

                    recycleItem.Delete();
                }

                from.PlaySound(0x2A);
                from.PlaySound(0x240);			

                return RecycleResult.Success;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				int num = m_CraftSystem.CanCraft( from, m_Tool, null );

				if ( num > 0 )
				{
					if ( num == 1044267 )
					{
						bool anvil, forge;
			
						DefBlacksmithy.CheckAnvilAndForge( from, 2, out anvil, out forge );

						if ( !anvil )
							num = 1044266; // You must be near an anvil

						else if ( !forge )
							num = 1044265; // You must be near a forge.
					}

					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, num ) );
				}

				else
				{
                    RecycleResult result = RecycleResult.Invalid;
                    int message;
                    Item item = targeted as Item;

                    if (item == null)
                        return;

                    else
                        result = Recycle(from, item);	

					switch ( result )
					{
						default:
						case RecycleResult.Invalid: message = 1044272; break; // You can't melt that down into ingots.
						case RecycleResult.NoSkill: message = 1044269; break; // You have no idea how to work this metal.
						case RecycleResult.Success: message = 1044270; break; // You melt the item down into ingots.
					}

					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, message ) );
				}
			}
		}
	}
}