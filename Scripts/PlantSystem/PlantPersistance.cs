using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Items
{
    public class PlantPersistance
    {
        public static List<PlantBowl> m_Instances = new List<PlantBowl>();
        
        public static double GrowthPerDay = 10.0;
        public static double SoilQualityLostPerDay = 5.0;
        public static double WaterLostPerDay = 10.0;

        public static double MaxWater = 100;
        public static double MaxSoilQuality = 100;
        public static double MaxHeat = 100;

        public static double WaterAddedPerUse = 10;

        public static Timer m_Timer;

        public static void Initialize()
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                m_Timer = new PlantTimer();
                m_Timer.Start();
            });
        }

        public class PlantTimer : Timer
        {
            public PlantTimer(): base(TimeSpan.Zero, TimeSpan.FromHours(1))
            {
                Priority = TimerPriority.OneMinute;
            }

            protected override void OnTick()
            {
                foreach (PlantBowl plantBowl in m_Instances)
                {
                    if (plantBowl == null) continue;
                    if (plantBowl.Deleted || plantBowl.Map == Map.Internal) continue;

                    if (plantBowl.m_PlantSeed == null) continue;
                    if (plantBowl.m_PlantSeed.Deleted) continue;

                    if (plantBowl.ReadyForHarvest) continue;

                    double targetGrowth = plantBowl.m_PlantSeed.TargetGrowth;

                    plantBowl.DetermineHeatLevel();

                    double growthAmount = (GrowthPerDay / 24.0) * plantBowl.GetDailyGrowthScalar();

                    plantBowl.GrowthValue += growthAmount;

                    if (plantBowl.GrowthValue >= targetGrowth)
                    {
                        plantBowl.GrowthValue = targetGrowth;
                        plantBowl.ReadyForHarvest = true;
                    }

                    plantBowl.WaterValue -= (WaterLostPerDay / 24.0);

                    if (plantBowl.WaterValue < 0)
                        plantBowl.WaterValue = 0;

                    plantBowl.SoilQualityValue -= (SoilQualityLostPerDay / 24.0);

                    if (plantBowl.SoilQualityValue < 0)
                        plantBowl.SoilQualityValue = 0;
                }
            }
        }
    }
}