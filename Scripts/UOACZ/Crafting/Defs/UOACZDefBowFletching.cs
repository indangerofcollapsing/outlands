using System;
using Server.Items;
using Server.Custom;

namespace Server.Engines.Craft
{
    public class UOACZDefBowFletching : CraftSystem
    {
        #region Base

        public override SkillName MainSkill
        {
            get { return SkillName.Fletching; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044006; } // <CENTER>BOWCRAFT AND FLETCHING MENU</CENTER>
        }

        public static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new UOACZDefBowFletching();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.5; // 50%
        }

        public UOACZDefBowFletching()
            : base(1, 1, 1.25)// base( 1, 2, 1.7 )
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
            // no animation
            //if ( from.Body.Type == BodyType.Human && !from.Mounted )
            //	from.Animate( 33, 5, 1, true, false, 0 );

            from.PlaySound(0x55);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2)
                {
                    //Player Enhancement Customization: Artisan
                    if (PlayerEnhancementPersistance.IsCustomizationEntryActive(from, CustomizationType.Artisan))
                        CustomizationAbilities.Artisan(from, from.Location, from.Map);

                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                }

                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override CraftECA ECA { get { return CraftECA.FiftyPercentChanceMinusTenPercent; } }

        #endregion

        public override void InitCraftList()
        {
            int index = -1;

            //Utility
            AddCraft(typeof(Kindling), "Utility", "Kindling", 0, 0, typeof(Log), "Log", 1, "You do not have the logs neccessary in order to craft that.");
            
            //Ammunition
            index = AddCraft(typeof(Shaft), "Ammunition", "Shaft", 5, 30, typeof(Log), "Log", 1, "You do not have the logs neccessary in order to craft that.");
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Arrow), "Ammunition", "Arrow", 5, 30, typeof(Shaft), "Shaft", 1, "You do not have the shafts neccessary in order to craft that.");
            AddRes(index, typeof(Feather), "Feather", 1, "You do not have the feathers neccessary in order to craft that.");
            SetUseAllRes(index, true);

            index = AddCraft(typeof(Bolt), "Ammunition", "Bolt", 5, 30, typeof(Shaft), "Shaft", 1, "You do not have the shafts neccessary in order to craft that.");
            AddRes(index, typeof(Feather), "Feather", 1, "You do not have the feathers neccessary in order to craft that.");
            SetUseAllRes(index, true);
            
            //Snares
            index = AddCraft(typeof(HuntingSnare), "Snares", "Hunting Snare", 5, 30, typeof(UOACZRope), "Rope", 1, "You do not have the neccessary rope required to make this.");
            AddRes(index, typeof(Log), "Log", 5, "You do not have the neccessary logs required to make this.");

            // Weapons            
            AddCraft(typeof(Bow), "Weapons", "Bow", 15, 50, typeof(Log), "Log", 15, "You do not have the logs neccessary in order to craft that.");
            AddCraft(typeof(Crossbow), "Weapons", "Crossbow", 15, 50, typeof(Log), "Log", 15, "You do not have the logs neccessary in order to craft that.");
            AddCraft(typeof(HeavyCrossbow), "Weapons", "Heavy Crossbow", 25, 50, typeof(Log), "Log", 20, "You do not have the logs neccessary in order to craft that.");

            MarkOption = true;
            Repair = true;// Core.AOS;
        }
    }
}