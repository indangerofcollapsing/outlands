using System;
using Server;
using Server.Targeting;
using Server.Items;

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

        public static bool IsRecycleResource(Type type)
        {
            bool recycleable = false;

            if (type == typeof(IronIngot)) return true;
            if (type == typeof(Leather)) return true;
            if (type == typeof(Board)) return true;
            if (type == typeof(Cloth)) return true;

            return recycleable;
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

                    if (!IsRecycleResource(craftResource.ItemType))
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

                List<Item> m_Items = new List<Item>();
                List<Item> m_ItemsToRecycle = new List<Item>();                

                Item[] m_MatchingItems = from.Backpack.FindItemsByType(itemType);
                
                for (int a = 0; a < m_MatchingItems.Length; a++)
                {
                    Item targetItem = m_MatchingItems[a];

                    if (craftContext.RecycleOption == CraftRecycleOption.RecycleItem && targetItem == item)
                    {
                        m_ItemsToRecycle.Add(targetItem);
                        continue;
                    }

                    if (craftContext.RecycleOption == CraftRecycleOption.RecycleAllNonExceptional && targetItem.Quality != Quality.Exceptional)
                    {
                        m_Items.Add(targetItem);
                        continue;
                    }  

                    if (craftContext.RecycleOption == CraftRecycleOption.RecycleAll)
                    {
                        m_ItemsToRecycle.Add(targetItem);
                        continue;
                    }                                                       
                }

                foreach (Item recycleItem in m_Items)
                {
                    if (recycleItem.LootType != LootType.Regular) continue;
                    if (recycleItem.PlayerClassCurrencyValue > 0) continue;
                    if (recycleItem.QuestItem) continue;
                    if (recycleItem.Nontransferable) continue;
                    if (recycleItem.DonationItem) continue;
                    if (recycleItem.DecorativeEquipment) continue;

                    m_ItemsToRecycle.Add(recycleItem);
                }

                if (m_ItemsToRecycle.Count == 0)                
                    return RecycleResult.Invalid;                

                Queue m_Queue = new Queue();

                foreach (Item recycleItem in m_ItemsToRecycle)
                {
                    m_Queue.Enqueue(recycleItem);
                }

                int deletedCount = 0;

                List<int> m_RecycleSounds = new List<int>();
                
                while (m_Queue.Count > 0)
                {
                    Item recycleItem = (Item)m_Queue.Dequeue();

                    bool deleteItem = false;
                                                           
                    foreach (KeyValuePair<Type, int> pair in m_ValidRecipeResources)
                    {
                        Type resourceType = pair.Key;
                        int totalResourceAmount = pair.Value * recycleItem.Amount;

                        if (totalResourceAmount < 2)
                            continue;

                        //Ingot
                        if (resourceType == typeof(IronIngot))
                        {
                            if (!m_RecycleSounds.Contains(0x2A))
                                m_RecycleSounds.Add(0x2A);

                            if (!m_RecycleSounds.Contains(0x240))
                                m_RecycleSounds.Add(0x240);

                            if (recycleItem.Resource != CraftResource.Iron)
                            {
                                resourceType = CraftResources.GetCraftResourceType(recycleItem.Resource);

                                if (resourceType == null)
                                    resourceType = typeof(IronIngot);
                            }
                        }

                        //Leather
                        if (resourceType == typeof(Leather))
                        {
                            if (!m_RecycleSounds.Contains(0x3E3))
                                m_RecycleSounds.Add(0x3E3);

                            if (recycleItem.Resource != CraftResource.RegularLeather)
                            {
                                resourceType = CraftResources.GetCraftResourceType(recycleItem.Resource);

                                if (resourceType == null)
                                    resourceType = typeof(Leather);
                            }
                        }

                        //Wood
                        if (resourceType == typeof(Board))
                        {
                            if (!m_RecycleSounds.Contains(0x23D))
                                m_RecycleSounds.Add(0x23D);

                            if (recycleItem.Resource != CraftResource.RegularWood)
                            {
                                resourceType = CraftResources.GetCraftResourceType(recycleItem.Resource);

                                if (resourceType == null)
                                    resourceType = typeof(Board);
                            }
                        }

                        Item newResource = (Item)Activator.CreateInstance(resourceType);

                        if (newResource == null)
                            continue;

                        //Cloth
                        if (resourceType == typeof(Cloth))
                        {
                            if (!m_RecycleSounds.Contains(0x248))
                                m_RecycleSounds.Add(0x248);

                            newResource.Hue = recycleItem.Hue;
                        }

                        deleteItem = true;
                        deletedCount++;

                        newResource.Amount = (int)(Math.Floor((double)totalResourceAmount / 2));
                        from.AddToBackpack(newResource);
                    }

                    if (deleteItem)
                        recycleItem.Delete();
                }
                
                if (deletedCount > 0)
                {
                    foreach (int sound in m_RecycleSounds)
                    {
                        from.PlaySound(sound);
                    }

                    return RecycleResult.Success;
                }

                else                
                    return RecycleResult.Invalid;                
			}

			protected override void OnTarget( Mobile from, object targeted )
			{                
                int num = m_CraftSystem.CanCraft( from, m_Tool, null );

                if (num > 0)
                {
                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, num));
                    return;
                }                 

                RecycleResult result = RecycleResult.Invalid;
                string message;
                Item item = targeted as Item;

                if (item == null)
                    return;

                else
                    result = Recycle(from, item);

                switch (result)
                {
                    default:
                    case RecycleResult.Invalid: message = "That cannot be recycled."; break;
                    case RecycleResult.NoSkill: message = "You do not know how to recycle this material."; break;
                    case RecycleResult.Success: message = "You recycle the item."; break;
                }

                from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, message));
			}
		}
	}
}