using System;
using Server.Items;

using Server.Mobiles;

namespace Server.Engines.Craft
{
    public class DefAlchemy : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Alchemy; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044001; } // <CENTER>ALCHEMY MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefAlchemy();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        private DefAlchemy(): base(1, 1, 1.25)
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!

            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x242);
        }

        private static Type typeofPotion = typeof(BasePotion);

        public static bool IsPotion(Type type)
        {
            return typeofPotion.IsAssignableFrom(type);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (IsPotion(item.ItemType))
                {
                    from.AddToBackpack(new Bottle());
                    return 500287; // You fail to create a useful potion.
                }

                else
                    return 1044043; // You failed to create the item, and some of your materials are lost.				
            }

            else
            {
                from.PlaySound(0x240); // Sound of a filling bottle
                
                if (IsPotion(item.ItemType))
                {
                    if (quality == -1)
                        return 1048136; // You create the potion and pour it into a keg.

                    else
                        return 500279; // You pour the potion into a bottle...
                }

                else
                    return 1044154; // You create the item.				
            }
        }

        public override void InitCraftList()
        {
            int index = -1;

            //Heal
            index = AddCraft(1, typeof(LesserHealPotion), "Heal Potions", "Lesser Heal Potion", -25.0, 25.0, typeof(Ginseng), 1044356, 1, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(HealPotion), "Heal Potions", "Heal Potion", 15.0, 65.0, typeof(Ginseng), 1044356, 3, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterHealPotion), "Heal Potions", "Greater Heal Potion", 55.0, 105.0, typeof(Ginseng), 1044356, 7, 1044364);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Cure Potion
            index = AddCraft(1, typeof(LesserCurePotion), "Cure Potions", "Lesser Cure Potion", -10.0, 40.0, typeof(Garlic), 1044355, 1, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(CurePotion), "Cure Potions", "Cure Potion", 25.0, 75.0, typeof(Garlic), 1044355, 3, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterCurePotion), "Cure Potions", "Greater Cure Potion", 65.0, 115.0, typeof(Garlic), 1044355, 6, 1044363);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Refresh Potion
            index = AddCraft(1, typeof(RefreshPotion), "Refresh Potions", "Refresh Potion", -25, 25.0, typeof(BlackPearl), 1044353, 1, 1044361);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(TotalRefreshPotion), "Refresh Potions", "Total Refresh Potion", 25.0, 75.0, typeof(BlackPearl), 1044353, 5, 1044361);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Strength Potion
            index = AddCraft(1, typeof(StrengthPotion), "Strength Potions", "Strength Potion", 25.0, 75.0, typeof(MandrakeRoot), 1044357, 2, 1044365);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterStrengthPotion), "Strength Potions", "Greater Strength Potion", 45.0, 95.0, typeof(MandrakeRoot), 1044357, 5, 1044365);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Agility Potion
            index = AddCraft(1, typeof(AgilityPotion), "Agility Potions", "Agility Potion", 15.0, 65.0, typeof(Bloodmoss), 1044354, 1, 1044362);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterAgilityPotion), "Agility Potions", "Greater Agility Potion", 35.0, 85.0, typeof(Bloodmoss), 1044354, 3, 1044362);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Magic Resist Potion
            index = AddCraft(1, typeof(LesserMagicResistPotion), "Magic Resist Potions", "Lesser Magic Resist Potion", 25.0, 75.0, typeof(SpidersSilk), 1044360, 3, 1044368);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(MagicResistPotion), "Magic Resist Potions", "Magic Resist Potion", 50.0, 100.0, typeof(SpidersSilk), 1044360, 6, 1044368);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterMagicResistPotion), "Magic Resist Potions", "Greater Magic Resist Potion", 75.0, 125.0, typeof(SpidersSilk), 1044360, 9, 1044368);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Poison Potion
            index = AddCraft(1, typeof(LesserPoisonPotion), "Poison Potions", "Lesser Poison Potion", -5.0, 45.0, typeof(Nightshade), 1044358, 1, 1044366);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(PoisonPotion), "Poison Potions", "Poison Potion", 15.0, 65.0, typeof(Nightshade), 1044358, 2, 1044366);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterPoisonPotion), "Poison Potions", "Greater Poison Potion", 55.0, 105.0, typeof(Nightshade), 1044358, 4, 1044366);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(DeadlyPoisonPotion), "Poison Potions", "Deadly Poison Potion", 90.0, 140.0, typeof(Nightshade), 1044358, 8, 1044366);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            //Explosion Potion
            index = AddCraft(1, typeof(LesserExplosionPotion), "Explosion Potions", "Lesser Explosion Potion", 5.0, 55.0, typeof(SulfurousAsh), 1044359, 3, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(ExplosionPotion), "Explosion Potions", "Explosion Potion", 35.0, 85.0, typeof(SulfurousAsh), 1044359, 5, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);

            index = AddCraft(1, typeof(GreaterExplosionPotion), "Explosion Potions", "Greater Explosion Potion", 65.0, 115.0, typeof(SulfurousAsh), 1044359, 10, 1044367);
            AddRes(index, typeof(Bottle), 1044529, 1, 500315);
        }
    }
}