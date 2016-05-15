using System;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;

namespace Server.Engines.Craft
{
	public class Repair
	{
		public Repair()
		{
		}

		public static void Do( Mobile from, CraftSystem craftSystem, BaseTool tool )
		{
			from.Target = new InternalTarget( craftSystem, tool );
			from.SendLocalizedMessage( 1044276 ); // Target an item to repair.
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

			private static void EndGolemRepair( object state )
			{
				((Mobile)state).EndAction( typeof( Golem ) );
			}

            public static bool ReduceDurabilityOnSuccess(Item item)
            {

                if (item is BaseWeapon)
                {
                    BaseWeapon weapon = item as BaseWeapon;

                    if (weapon.Quality == Quality.Exceptional && weapon.DisplayCrafter)
                        return false;
                }

                if (item is BaseArmor)
                {
                    BaseArmor armor = item as BaseArmor;

                    if (armor.Quality == Quality.Exceptional && armor.DisplayCrafter)
                        return false;
                }
                
                return true;
            }
                                    
			protected override void OnTarget( Mobile from, object targeted )
			{
				int number;
                
				if ( m_CraftSystem.CanCraft( from, m_Tool, targeted.GetType() ) == 1044267 )				
					number = 1044282; // You must be near a forge and and anvil to repair items.				
				
				else if ( targeted is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon)targeted;
					SkillName skill = m_CraftSystem.MainSkill;

                    double repairChance = (from.Skills[skill].Value / 100);

                    double missingHitPointsScalar = ((double)weapon.MaxHitPoints - (double)weapon.HitPoints) / (double)weapon.MaxHitPoints;

                    int repairDurabilityDamage = (int)(Math.Round((missingHitPointsScalar * .1) * (double)weapon.MaxHitPoints));

                    if (repairDurabilityDamage < 1)
                        repairDurabilityDamage = 1;                   

					if ( m_CraftSystem.CraftItems.SearchForSubclass( weapon.GetType() ) == null)					
						number = 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
					
					else if ( !weapon.IsChildOf( from.Backpack ) && ( !Core.ML || weapon.Parent != from ) )					
						number = 1044275; // The item must be in your backpack to repair it.
					
					else if ( weapon.PoisonCharges != 0 )					
						number = 1005012; // You cannot repair an item while a caustic substance is on it.					

					else if ( weapon.MaxHitPoints <= 0 || weapon.HitPoints == weapon.MaxHitPoints )					
						number = 1044281; // That item is in full repair

                    else if (weapon.MaxHitPoints < 10)					
						number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
					
					else
					{
                        if (Utility.RandomDouble() <= repairChance)
                        {
                            number = 1044279; // You repair the item.                            

                            if (ReduceDurabilityOnSuccess(weapon))
                                weapon.MaxHitPoints -= repairDurabilityDamage;

                            weapon.HitPoints = weapon.MaxHitPoints;

                            m_CraftSystem.PlayCraftEffect(from);
                        }

                        else
                        {
                            number = 044280; // You fail to repair the item. [And the contract is destroyed]

                            m_CraftSystem.PlayCraftEffect(from);
                        }
					}
				}

				else if ( targeted is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)targeted;
					SkillName skill = m_CraftSystem.MainSkill;

                    double repairChance = (from.Skills[skill].Value / 100);

                    double missingHitPointsScalar = ((double)armor.MaxHitPoints - (double)armor.HitPoints) / (double)armor.MaxHitPoints;

                    int repairDurabilityDamage = (int)(Math.Round((missingHitPointsScalar * .1) * (double)armor.MaxHitPoints));

                    if (repairDurabilityDamage < 1)
                        repairDurabilityDamage = 1;

					if ( m_CraftSystem.CraftItems.SearchForSubclass( armor.GetType() ) == null )					
						number = 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
					
					else if ( !armor.IsChildOf( from.Backpack ) && (armor.Parent != from ) )					
						number = 1044275; // The item must be in your backpack to repair it.
					
					else if ( armor.MaxHitPoints <= 0 || armor.HitPoints == armor.MaxHitPoints )					
						number = 1044281; // That item is in full repair

                    else if (armor.MaxHitPoints < 10)					
						number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.					

					else
					{
                        if (Utility.RandomDouble() <= repairChance)
                        {
                            number = 1044279; // You repair the item.                            

                            if (ReduceDurabilityOnSuccess(armor))
                                armor.MaxHitPoints -= repairDurabilityDamage;

                            armor.HitPoints = armor.MaxHitPoints;

                            m_CraftSystem.PlayCraftEffect(from);
                        }

                        else
                        {
                            number = 044280; // You fail to repair the item. [And the contract is destroyed]

                            m_CraftSystem.PlayCraftEffect(from);
                        }
					}
				}

                /*
				else if ( targeted is BaseClothing )
				{
					BaseClothing clothing = (BaseClothing)targeted;
					SkillName skill = m_CraftSystem.MainSkill;
					int toWeaken = 0;

					if ( Core.AOS )
					{
						toWeaken = 1;
					}

					else if ( skill != SkillName.Tailoring )
					{
						double skillLevel = from.Skills[skill].Base;

						if ( skillLevel >= 90.0 )
							toWeaken = 1;
						else if ( skillLevel >= 70.0 )
							toWeaken = 2;
						else
							toWeaken = 3;
					}

 					if (m_CraftSystem.CraftItems.SearchForSubclass(clothing.GetType()) == null && !((targeted is TribalMask) || (targeted is HornedTribalMask)) )
 					{
						number = 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
					}

					else if ( !clothing.IsChildOf( from.Backpack ) && ( !Core.ML || clothing.Parent != from ) )
					{
						number = 1044275; // The item must be in your backpack to repair it.
					}

					else if ( clothing.MaxHitPoints <= 0 || clothing.HitPoints == clothing.MaxHitPoints )
					{
						number = 1044281; // That item is in full repair
					}

					else if ( clothing.MaxHitPoints <= toWeaken )
					{
						number = 1044278; // That item has been repaired many times, and will break if repairs are attempted again.
					}

					else
					{
						if ( CheckWeaken( from, skill, clothing.HitPoints, clothing.MaxHitPoints ) )
						{
							clothing.MaxHitPoints -= toWeaken;
							clothing.HitPoints = Math.Max( 0, clothing.HitPoints - toWeaken );
						}

						if ( CheckRepairDifficulty( from, skill, clothing.HitPoints, clothing.MaxHitPoints ) )
						{
							number = 1044279; // You repair the item.
							m_CraftSystem.PlayCraftEffect( from );
							clothing.HitPoints = clothing.MaxHitPoints;
						}

						else
						{
							number = 1044280; // You fail to repair the item. [And the contract is destroyed]
							m_CraftSystem.PlayCraftEffect( from );
						}
					}
				}
                */

				else if ( targeted is Item )				
					number = 1044277; // That item cannot be repaired. // You cannot repair that item with this type of repair contract.
				
				else				
					number = 500426; // You can't repair that.				
				
				CraftContext context = m_CraftSystem.GetContext( from );
				from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, number ) );
			}
		}
	}
}