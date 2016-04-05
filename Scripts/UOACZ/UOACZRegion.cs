using System;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using System.Collections.Specialized;
using Server.Spells.Third;
using System.Net;


namespace Server
{
    public class UOACZRegion : Region
    {
        public static Rectangle2D UOACZRegionRectangle = new Rectangle2D(new Point2D(525, 0), new Point2D(2550, 2040));
        public static Map Facet = Map.Malas;
        
        public static void Initialize()
        {
            Region targetRegion = null;

            foreach (Region region in Region.Regions)
            {
                if (region is UOACZRegion)
                    targetRegion = region;
            }

            if (targetRegion == null)            
                UOACZPersistance.ChangeRegion(); 
        }

        public UOACZRegion(string name, Map map, int priority, Rectangle2D[] area): base(name, map, priority, area)
        {
        }

        public override TimeSpan GetLogoutDelay(Mobile mobile)
        { 
            return base.GetLogoutDelay(mobile);
        }      

        public static bool ContainsItem(Item item)
        {
            if (item == null)
                return false;

            Point3D location = item.Location;

            if (item.RootParentEntity != null)
                location = item.RootParentEntity.Location;

            return item.Map == Facet && UOACZRegionRectangle.Contains(location);
        } 
        
        public static bool ContainsMobile(Mobile mobile)
        {
            return mobile.Map == Facet && UOACZRegionRectangle.Contains(mobile);
        }         
		
		public override bool OnBeginSpellCast(Mobile mobile, ISpell spell)
		{			
			if (mobile.AccessLevel == AccessLevel.Player)
			{				
				if (spell is RecallSpell || spell is GateTravelSpell || spell is MarkSpell || spell is InvisibilitySpell ||
                    spell is PolymorphSpell ||spell is TeleportSpell)
				{
					
					mobile.SendMessage("That spell does not appear to work here.");
					return false;
				}

				return base.OnBeginSpellCast(mobile, spell);
			}

			return base.OnBeginSpellCast(mobile, spell);
		}
		
		public override bool OnSkillUse(Mobile from, int skill)
		{
            return UOACZSystem.CheckAllowSkillUse(from, skill);
		}

        public override bool OnBeforeDeath(Mobile mobile)
        {
            return base.OnBeforeDeath(mobile);
        }

        public override void OnDeath(Mobile mobile)
        {
            base.OnDeath(mobile);

            PlayerMobile player = mobile as PlayerMobile;

            if (player == null) return;
            if (player.AccessLevel > AccessLevel.Player) return;

            if (!UOACZPersistance.Active)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            player.Frozen = true;

            Queue m_Queue = new Queue();

            foreach (BaseCreature creature in player.AllFollowers)
            {
                if (UOACZRegion.ContainsMobile(creature))
                    m_Queue.Enqueue(creature);
            }

            while (m_Queue.Count > 0)
            {
                BaseCreature creature = (BaseCreature)m_Queue.Dequeue();

                int damage = (int)(Math.Round((double)creature.HitsMax * UOACZSystem.SwarmControllerDeathDamageScalar));

                new Blood().MoveToWorld(creature.Location, creature.Map);
                AOS.Damage(creature, damage, 0, 100, 0, 0, 0);

                if (UOACZSystem.IsUOACZValidMobile(creature))
                {
                    if (creature.AIObject != null && creature.Controlled)
                    {
                        creature.AIObject.DoOrderRelease();

                        if (creature is UOACZBaseUndead)
                        {
                            UOACZBaseUndead undeadCreature = creature as UOACZBaseUndead;

                            undeadCreature.m_LastActivity = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(3));
                            undeadCreature.m_NeedWaypoint = true;
                            undeadCreature.CanTeleportToBaseNode = true;
                            undeadCreature.InWilderness = true;
                        }
                    }
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (player == null) return;
                if (player.Deleted) return;                
                if (!UOACZRegion.ContainsMobile(player)) return;
                if (!UOACZPersistance.Active) return;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                player.m_UOACZAccountEntry.UndeadProfile.Deaths++;
            });

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (player == null) return;
                if (player.Deleted) return;
                if (!UOACZRegion.ContainsMobile(player)) return;
                if (!UOACZPersistance.Active) return;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                UOACZDestination destination;

                bool lostFoodWater = false;
                bool lostBrains = false;

                player.m_UOACZAccountEntry.FatigueExpiration = DateTime.UtcNow + UOACZSystem.FatigueDuration;

                switch (player.m_UOACZAccountEntry.ActiveProfile)
                {
                    case UOACZAccountEntry.ActiveProfileType.Human:
                        if (player.m_UOACZAccountEntry.HumanProfile.HonorPoints <= UOACZSystem.HonorAggressionThreshold)
                        {
                            destination = UOACZDestination.GetRandomEntrance(false);

                            if (destination != null)
                                player.MoveToWorld(destination.Location, destination.Map);

                            else
                                player.MoveToWorld(UOACZPersistance.DefaultUndeadLocation, UOACZPersistance.DefaultMap);
                        }

                        else
                        {
                            destination = UOACZDestination.GetRandomEntrance(true);

                            if (destination != null)
                                player.MoveToWorld(destination.Location, destination.Map);

                            else
                                player.MoveToWorld(UOACZPersistance.DefaultHumanLocation, UOACZPersistance.DefaultMap);
                        }                        

                        player.Resurrect();
                        player.RevealingAction();
                        player.Frozen = false;
                        
                        #region Auto-Reequip Blessed Gear

                        if (player.Backpack != null)
                        {
                            if (!player.Backpack.Deleted)
                            {
                                Item deathRobe = player.FindItemOnLayer(Layer.OuterTorso);

                                if (!(deathRobe is DeathRobe))
                                    deathRobe = null;

                                UOACZSurvivalMachete survivalMachete = null;
                                UOACZSurvivalLantern survivalLantern = null;

                                List<Item> m_LayerShirt = new List<Item>();
                                List<Item> m_MiddleTorso = new List<Item>();
                                List<Item> m_OuterLegs = new List<Item>();
                                List<Item> m_Pants = new List<Item>();
                                List<Item> m_Shoes = new List<Item>();

                                List<Item> m_BackpackItems = player.Backpack.Items;

                                bool foundPants = false;

                                foreach (Item item in m_BackpackItems)
                                {
                                    if (item is UOACZSurvivalMachete)
                                        survivalMachete = item as UOACZSurvivalMachete;

                                    //if (item is UOACZSurvivalLantern)
                                        //survivalLantern = item as UOACZSurvivalLantern;

                                    if (item.Layer == Layer.Shirt)
                                        m_LayerShirt.Add(item);

                                    if (item.Layer == Layer.MiddleTorso)
                                        m_MiddleTorso.Add(item);

                                    if (item.Layer == Layer.OuterLegs)
                                    {
                                        m_OuterLegs.Add(item);
                                        foundPants = true;
                                    }

                                    if (item.Layer == Layer.Pants)
                                    {
                                        m_Pants.Add(item);
                                        foundPants = true;
                                    }

                                    if (item.Layer == Layer.Shoes)
                                        m_Shoes.Add(item);
                                }

                                if (survivalMachete != null)
                                    player.AddItem(survivalMachete);

                                //if (survivalLantern != null)
                                    //player.AddItem(survivalLantern);

                                if (foundPants && deathRobe != null)
                                    deathRobe.Delete();

                                if (m_LayerShirt.Count > 0)
                                    player.AddItem(m_LayerShirt[0]);

                                if (m_MiddleTorso.Count > 0)
                                    player.AddItem(m_MiddleTorso[0]);

                                if (m_OuterLegs.Count > 0)
                                    player.AddItem(m_OuterLegs[0]);

                                if (m_Pants.Count > 0)
                                    player.AddItem(m_Pants[0]);

                                if (m_Shoes.Count > 0)
                                    player.AddItem(m_Shoes[0]);
                            }
                        }  
                      
                        #endregion

                        UOACZSystem.ApplyActiveProfile(player);

                        player.Hits = (int)Math.Ceiling(UOACZSystem.HumanRessStatsPercent * (double)player.HitsMax);
                        player.Stam = (int)Math.Ceiling(UOACZSystem.HumanRessStatsPercent * (double)player.StamMax);
                        player.Mana = (int)Math.Ceiling(UOACZSystem.HumanRessStatsPercent * (double)player.ManaMax);

                        if (player.Backpack != null)
                        {
                            Item[] consumptionItem = player.Backpack.FindItemsByType(typeof(UOACZConsumptionItem));

                            m_Queue = new Queue();

                            for (int a = 0; a < consumptionItem.Length; a++)
                            {
                                UOACZConsumptionItem foodItem = consumptionItem[a] as UOACZConsumptionItem;

                                if (foodItem == null)
                                    continue;

                                if (Utility.RandomDouble() <= UOACZSystem.HumanDeathFoodWaterLossChance)
                                {
                                    lostFoodWater = true;

                                    if (foodItem.Charges > 1)                                    
                                        foodItem.Charges = (int)Math.Floor((double)foodItem.Charges / 2);

                                    else
                                        m_Queue.Enqueue(foodItem);
                                }
                            }

                            while (m_Queue.Count > 0)
                            {
                                Item item = (Item)m_Queue.Dequeue();
                                item.Delete();
                            }
                        }
                        
                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (!UOACZSystem.IsUOACZValidMobile(player))
                                return; 
                            
                            player.m_UOACZAccountEntry.HumanProfile.CauseOfDeath = UOACZAccountEntry.HumanProfileEntry.CauseOfDeathType.Misc;
                            player.m_UOACZAccountEntry.HumanDeaths++;

                            if (player.IsUOACZHuman)
                            {
                                if (player.m_UOACZAccountEntry.HumanAbilitiesHotbarDisplayed)
                                {
                                    player.CloseGump(typeof(HumanProfileAbilitiesHotbarGump));
                                    player.SendGump(new HumanProfileAbilitiesHotbarGump(player));
                                }

                                if (player.m_UOACZAccountEntry.HumanStatsHotbarDisplayed)
                                {
                                    player.CloseGump(typeof(HumanProfileStatsHotbarGump));
                                    player.SendGump(new HumanProfileStatsHotbarGump(player));
                                }

                                if (player.m_UOACZAccountEntry.ObjectivesDisplayed)
                                {
                                    player.CloseGump(typeof(ObjectivesHotbarGump));
                                    player.SendGump(new ObjectivesHotbarGump(player));
                                }
                            }
                        });
                    break;

                    case UOACZAccountEntry.ActiveProfileType.Undead:                        
                        destination = UOACZDestination.GetRandomEntrance(false);

                        player.m_UOACZAccountEntry.UndeadDeaths++;

                        if (destination != null)
                            player.MoveToWorld(destination.Location, destination.Map);

                        else
                            player.MoveToWorld(UOACZPersistance.DefaultUndeadLocation, UOACZPersistance.DefaultMap);

                        player.Resurrect();
                        player.RevealingAction();
                        player.Frozen = false;

                        if (player.Backpack != null)
                        {
                            Item[] brainItems = player.Backpack.FindItemsByType(typeof(UOACZBrains));

                            m_Queue = new Queue();

                            for (int a = 0; a < brainItems.Length; a++)
                            {
                                UOACZBrains brainItem = brainItems[a] as UOACZBrains;

                                if (brainItem == null)
                                    continue;

                                if (Utility.RandomDouble() <= UOACZSystem.UndeadDeathBrainLossChance)
                                {
                                    lostBrains = true;

                                    m_Queue.Enqueue(brainItem);
                                }
                            }

                            while (m_Queue.Count > 0)
                            {
                                Item item = (Item)m_Queue.Dequeue();
                                item.Delete();
                            }
                        }

                        UOACZSystem.ApplyActiveProfile(player);

                        player.Hits = (int)Math.Ceiling(UOACZSystem.UndeadRessStatsPercent * (double)player.HitsMax);
                        player.Stam = (int)Math.Ceiling(UOACZSystem.UndeadRessStatsPercent * (double)player.StamMax);
                        player.Mana = (int)Math.Ceiling(UOACZSystem.UndeadRessStatsPercent * (double)player.ManaMax);  
                        
                        if (player.m_UOACZAccountEntry.UndeadAbilitiesHotbarDisplayed)
                        {
                            player.CloseGump(typeof(UndeadProfileAbilitiesHotbarGump));
                            player.SendGump(new UndeadProfileAbilitiesHotbarGump(player));
                        }

                        if (player.m_UOACZAccountEntry.UndeadStatsHotbarDisplayed)
                        {
                            player.CloseGump(typeof(UndeadProfileStatsHotbarGump));
                            player.SendGump(new UndeadProfileStatsHotbarGump(player));
                        }

                        if (player.m_UOACZAccountEntry.ObjectivesDisplayed)
                        {
                            player.CloseGump(typeof(ObjectivesHotbarGump));
                            player.SendGump(new ObjectivesHotbarGump(player));
                        }
                    break;
                }

                string fatigueDuration = Utility.CreateTimeRemainingString(DateTime.UtcNow, DateTime.UtcNow + UOACZSystem.FatigueDuration, false, true, true, true, true);
                string fatiguePercentText = ((1.0 - UOACZSystem.FatigueActiveScalar) * 100).ToString() + "%";

                player.SendMessage(UOACZSystem.orangeTextHue, "You have died and will be subject to a -" + fatiguePercentText + " PvP penalty for " + fatigueDuration + ".");

                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (player == null)
                        return;

                    if (player.IsUOACZHuman && lostFoodWater)
                        player.SendMessage(UOACZSystem.orangeTextHue, "As a result of your death, some of your food and water has been been lost.");

                    if (player.IsUOACZUndead && lostBrains)
                        player.SendMessage(UOACZSystem.orangeTextHue, "As a result of your death, some of your brains have been been lost.");
                });
            });
        }

        public override bool OnResurrect(Mobile mobile)
        {
            return base.OnResurrect(mobile);                        
        }
	}
}