using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Custom;

namespace Server.Items
{
    public class PlantGump : Gump
    {
        Mobile m_From;
        PlantBowl m_PlantBowl;

        public PlantGump(Mobile from, PlantBowl plantBowl): base(10, 10)
        {
            if (from == null || plantBowl == null) return;
            if (from.Deleted || !from.Alive || plantBowl.Deleted) return;

            m_From = from;
            m_PlantBowl = plantBowl;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            int textHue = 2036;

            bool showBowl = true;

            Item plantItem = null;

            int plantItemId = 3238;
            int plantItemHue = 0;
            int plantItemOffsetX = 0;
            int plantItemOffsetY = 0;

            int seedItemId = 22326;
            int seedItemHue = 0;
            int seedItemOffsetX = 0;
            int seedItemOffsetY = 0;

            string plantName = "Unknown Plant";

            double growthPercent = 0;
            double waterPercent = 0;
            double soilPercent = 0;
            double heatPercent = 0;

            double targetWaterPercent = 0;
            double targetSoilPercent = 0;
            double targetHeatPercent = 0;

            string dailyGrowthText = "";
            string growthText = "";
            string waterText = "";
            string soilText = "";
            string heatText = "";
            string growthEfficiencyText = "";
            int growthEfficiencyHue = textHue;

            string seedText = "";
            int seedTextHue = textHue;

            string actionText = "Add Seed";
            int actionTextHue = textHue;

            if (plantBowl.PlantSeed != null)
            {
                plantBowl.DetermineHeatLevel();

                seedText = plantBowl.PlantSeed.SeedGumpName;

                targetWaterPercent = plantBowl.PlantSeed.TargetWater / PlantPersistance.MaxWater;
                targetSoilPercent = plantBowl.PlantSeed.TargetSoilQuality / PlantPersistance.MaxSoilQuality;
                targetHeatPercent = plantBowl.PlantSeed.TargetHeat / PlantPersistance.MaxHeat;

                //TEST: Change this based on rarity
                seedTextHue = 2605;

                seedItemId = plantBowl.PlantSeed.SeedItemID;
                seedItemHue = plantBowl.PlantSeed.SeedHue;
                seedItemOffsetX = plantBowl.PlantSeed.SeedGumpOffsetX;
                seedItemOffsetY = plantBowl.PlantSeed.SeedGumpOffsetY;

                if (plantBowl.ReadyForHarvest)
                {
                    growthText = plantBowl.GrowthValue.ToString();
                    growthPercent = 100;

                    actionTextHue = 267;

                    actionText = "Harvest Plant";

                    showBowl = false;

                    plantItem = (Item)Activator.CreateInstance(plantBowl.PlantSeed.PlantType);

                    //Plant Item
                    if (plantItem is Plant)
                    {
                        Plant plant = plantItem as Plant;

                        plantItemId = plant.PlantItemID;
                        plantItemHue = plant.PlantItemHue;
                        plantItemOffsetX = plant.PlantItemOffsetX;
                        plantItemOffsetY = plant.PlantItemOffsetY;

                        plantName = plant.PlantName;

                        dailyGrowthText = "Ready to Harvest";
                    }

                    //Non-Plant Items
                    else
                    {
                    }
                }

                else
                {
                    actionText = "Add Ingredient";

                    double progressPercent = plantBowl.GrowthValue / plantBowl.PlantSeed.TargetGrowth;
                    double dailyGrowthValue = plantBowl.GetDailyGrowthScalar() * PlantPersistance.GrowthPerDay;

                    dailyGrowthText = Utility.CreateDecimalString(dailyGrowthValue, 1);

                    if (dailyGrowthText.IndexOf(".") == -1)
                        dailyGrowthText = dailyGrowthText + ".0";

                    dailyGrowthText += " Daily Growth";

                    growthText = Utility.CreateDecimalString(plantBowl.GrowthValue, 1);
                    growthText = growthText + " / " + plantBowl.PlantSeed.TargetGrowth.ToString();

                    waterText = Utility.CreateDecimalString(plantBowl.WaterValue, 0);
                    if (plantBowl.WaterValue != plantBowl.PlantSeed.TargetWater)
                        waterText = waterText + " (-" + Utility.CreateDecimalPercentageString(plantBowl.GetWaterPenalty(), 0) + ")";

                    soilText = Utility.CreateDecimalString(plantBowl.SoilQualityValue, 0);
                    if (plantBowl.SoilQualityValue != plantBowl.PlantSeed.TargetSoilQuality)
                        soilText = soilText + " (-" + Utility.CreateDecimalPercentageString(plantBowl.GetSoilPenalty(), 0) + ")";

                    heatText = Utility.CreateDecimalString(plantBowl.HeatValue, 0);
                    if (plantBowl.HeatValue != plantBowl.PlantSeed.TargetHeat)
                        heatText = heatText + " (-" + Utility.CreateDecimalPercentageString(plantBowl.GetHeatPenalty(), 0) + ")";

                    growthPercent = progressPercent;
                    waterPercent = plantBowl.WaterValue / PlantPersistance.MaxWater;
                    soilPercent = plantBowl.SoilQualityValue / PlantPersistance.MaxSoilQuality;
                    heatPercent = plantBowl.HeatValue / PlantPersistance.MaxHeat;

                    #region Plant Images

                    switch (plantBowl.PlantSeed.PlantGroup)
                    {
                        case PlantGroupType.Crop:
                            plantName = "Unknown Crop";

                            if (progressPercent < .33)
                            {
                                plantItemId = 6818;
                                plantItemHue = 0;
                                plantItemOffsetX = -1;
                                plantItemOffsetY = 18;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 731;
                                plantItemHue = 2208;
                                plantItemOffsetX = 6;
                                plantItemOffsetY = 2;
                            }

                            else
                            {
                                plantItemId = 3155;
                                plantItemHue = 2208;
                                plantItemOffsetX = 4;
                                plantItemOffsetY = -1;
                            }
                        break;

                        case PlantGroupType.Fern:
                            plantName = "Unknown Fern";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3169;
                                plantItemHue = 0;
                                plantItemOffsetX = 3;
                                plantItemOffsetY = 16;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3267;
                                plantItemHue = 0;
                                plantItemOffsetX = 1;
                                plantItemOffsetY = -9;
                            }

                            else
                            {
                                plantItemId = 3269;
                                plantItemHue = 0;
                                plantItemOffsetX = 6;
                                plantItemOffsetY = -8;
                            }
                        break;

                        case PlantGroupType.Flower:
                            plantName = "Unknown Flower";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3176;
                                plantItemHue = 0;
                                plantItemOffsetX = -2;
                                plantItemOffsetY = 4;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3177;
                                plantItemHue = 0;
                                plantItemOffsetX = -2;
                                plantItemOffsetY = 5;
                            }

                            else
                            {
                                plantItemId = 3183;
                                plantItemHue = 0;
                                plantItemOffsetX = -1;
                                plantItemOffsetY = 3;
                            }
                        break;

                        case PlantGroupType.Grass:
                            plantName = "Unknown Grass";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3253;
                                plantItemHue = 0;
                                plantItemOffsetX = 2;
                                plantItemOffsetY = 7;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3254;
                                plantItemHue = 0;
                                plantItemOffsetX = 0;
                                plantItemOffsetY = 6;
                            }

                            else
                            {
                                plantItemId = 3219;
                                plantItemHue = 0;
                                plantItemOffsetX = 2;
                                plantItemOffsetY = 6;
                            }
                        break;

                        case PlantGroupType.Tree:
                            plantName = "Unknown Tree";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3198;
                                plantItemHue = 0;
                                plantItemOffsetX = 0;
                                plantItemOffsetY = 0;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3306;
                                plantItemHue = 0;
                                plantItemOffsetX = 7;
                                plantItemOffsetY = -28;
                            }

                            else
                            {
                                plantItemId = 3305;
                                plantItemHue = 0;
                                plantItemOffsetX = 3;
                                plantItemOffsetY = -39;
                            }
                        break;

                        case PlantGroupType.Vine:
                            plantName = "Unknown Vine";

                            if (progressPercent < .33)
                            {
                                plantItemId = 3251;
                                plantItemHue = 0;
                                plantItemOffsetX = 18;
                                plantItemOffsetY = 0;
                            }

                            else if (progressPercent < .66)
                            {
                                plantItemId = 3166;
                                plantItemHue = 0;
                                plantItemOffsetX = 6;
                                plantItemOffsetY = 3;
                            }

                            else
                            {
                                plantItemId = 3167;
                                plantItemHue = 0;
                                plantItemOffsetX = 5;
                                plantItemOffsetY = 3;
                            }
                        break;
                    }

                    #endregion
                }
            }

            else            
                plantName = "No Seed Planted";

            //Bowl
            if (showBowl)
                AddItem(105, 160, 4551);

            //Plant Image
            if (plantBowl.PlantSeed != null)
                AddItem(105 + plantItemOffsetX, 145 + plantItemOffsetY, plantItemId, plantItemHue);   

            //Plant Name
            AddLabel(Utility.CenteredTextOffset(125, plantName), 199, 2610, plantName);
      
            //Daily Growth
            AddLabel(Utility.CenteredTextOffset(125, dailyGrowthText), 220, 267, dailyGrowthText);

            //Growth
            AddLabel(19, 242, 267, "Growth");
            AddImage(73, 248, 2056);
            AddImageTiled(73 + Utility.ProgressBarX(growthPercent), 250, Utility.ProgressBarWidth(growthPercent), 7, 2488);
            AddLabel(186, 242, 267, growthText);

            //Water
            AddLabel(24, 268, 187, "Water");
            AddImage(73, 275, 2054);
            AddImageTiled(73 + Utility.ProgressBarX(waterPercent), 277, Utility.ProgressBarWidth(waterPercent), 7, 2488);
            AddLabel(186, 268, 187, waterText);
            if (plantBowl.PlantSeed != null)
                AddImage(73 + Utility.ProgressBarX(targetWaterPercent), 287, 2436, 187);           

            //Soil
            AddLabel(38, 298, 542, "Soil");
            if (soilPercent > 0)
                AddImage(73, 303, 2057, 542);
            else
                AddImage(73, 303, 2057);
            AddImageTiled(73 + Utility.ProgressBarX(soilPercent), 305, Utility.ProgressBarWidth(soilPercent), 7, 2488);
            AddLabel(186, 298, 542, soilText);
            if (plantBowl.PlantSeed != null)
                AddImage(73 + Utility.ProgressBarX(targetSoilPercent), 315, 2436, 542);

            //Heat
            AddLabel(33, 327, 52, "Heat");
            AddImage(73, 332, 2057);
            AddImageTiled(73 + Utility.ProgressBarX(heatPercent), 334, Utility.ProgressBarWidth(heatPercent), 7, 2488);
            AddLabel(186, 327, 52, heatText);
            if (plantBowl.PlantSeed != null)
                AddImage(73 + Utility.ProgressBarX(targetHeatPercent), 345, 2436, 52);

            //Growth Efficiency
            if (plantBowl.PlantSeed != null && !plantBowl.ReadyForHarvest)
            {
                double dailyGrowthScalar = plantBowl.GetDailyGrowthScalar();

                growthEfficiencyText = "Growth Efficiency: " + Utility.CreateDecimalPercentageString(dailyGrowthScalar, 0);

                AddLabel(Utility.CenteredTextOffset(125, growthEfficiencyText), 359, growthEfficiencyHue, growthEfficiencyText);
            }

            AddImage(52, 385, 103);

            //Seed
            if (plantBowl.PlantSeed != null)
            {               
                AddLabel(108, 397, 2515, "Seed");

                AddItem(107 + seedItemOffsetX, 431 + seedItemOffsetY, seedItemId, seedItemHue);
                AddLabel(Utility.CenteredTextOffset(125, seedText), 453, seedTextHue, seedText);
            }

            //Guide
            AddLabel(15, 410, 149, "Guide");
            AddButton(20, 428, 2094, 2095, 2, GumpButtonType.Reply, 0);

            //Add Seed / Ingredient / Harvest
            AddLabel(Utility.CenteredTextOffset(125, actionText), 493, actionTextHue, actionText);
            AddButton(85, 518, 1147, 1148, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            if (m_From == null || m_PlantBowl == null) return;
            if (m_From.Deleted || !m_From.Alive || m_PlantBowl.Deleted) return;
            
            if (!m_From.InRange(m_PlantBowl.GetWorldLocation(), 2))
            {
                m_From.SendMessage("You are too far away to access that.");
                return;
            }

            bool closeGump = true;

            switch (info.ButtonID)
            {
                //Harvest or Add Ingredient
                case 1:
                    if (m_PlantBowl.ReadyForHarvest)                    
                        m_PlantBowl.Harvest(m_From);                    

                    else
                        m_PlantBowl.AddIngredient(m_From);

                    closeGump = false;
                break;

                //Launch Guide
                case 2:
                    closeGump = false;
                break;
            }

            if (!closeGump)
            {
                m_From.CloseGump(typeof(PlantGump));
                m_From.SendGump(new PlantGump(m_From, m_PlantBowl));
            }
        }
    }
}