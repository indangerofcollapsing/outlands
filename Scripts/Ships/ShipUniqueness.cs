using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server.Items
{
    public static class ShipUniqueness
    {
        public static void GenerateShipUniqueness(BaseBoat boat)
        {
            if (boat == null) return;
            if (boat.Deleted) return;

            string factionName = "";

            int shipLevel = 1;

            int BaseMaxHitPoints = 1000;
            int BaseMaxSailPoints = 500;
            int BaseMaxGunPoints = 500;

            double BaseFastInterval = 0.2;
            double BaseFastDriftInterval = 0.4;
            double BaseSlowInterval = 0.4;
            double BaseSlowDriftInterval = 0.8;

            double CannonAccuracyModifer = -1;
            double CannonRangeScalar = -1;
            double CannonDamageScalar = -1;
            double CannonReloadTimeScalar = -1;
            double DamageFromPlayerBoatScalar = -1;

            if (boat is SmallBoat || boat is SmallDragonBoat)
            {
                shipLevel = 1;

                boat.BaseDoubloonValue = 15;              

                BaseMaxHitPoints = 1000;
                BaseMaxSailPoints = 500;
                BaseMaxGunPoints = 500;

                BaseFastInterval = 0.2;
                BaseFastDriftInterval = 0.4;
                BaseSlowInterval = 0.4;
                BaseSlowDriftInterval = 0.8;                             
            }

            if (boat is MediumBoat || boat is MediumDragonBoat)
            {
                shipLevel = 2;

                boat.BaseDoubloonValue = 25;

                BaseMaxHitPoints = 1250;
                BaseMaxSailPoints = 625;
                BaseMaxGunPoints = 625;

                BaseFastInterval = 0.225;
                BaseFastDriftInterval = 0.45;
                BaseSlowInterval = 0.45;
                BaseSlowDriftInterval = 0.9;                               
            }

            if (boat is LargeBoat || boat is LargeDragonBoat)
            {
                shipLevel = 3;

                boat.BaseDoubloonValue = 100;

                BaseMaxHitPoints = 1500;
                BaseMaxSailPoints = 750;
                BaseMaxGunPoints = 750;

                BaseFastInterval = 0.25;
                BaseFastDriftInterval = 0.5;
                BaseSlowInterval = 0.5;
                BaseSlowDriftInterval = 1.0;
            }

            if (boat is CarrackBoat)
            {
                shipLevel = 4;

                boat.BaseDoubloonValue = 400;

                BaseMaxHitPoints = 2000;
                BaseMaxSailPoints = 1000;
                BaseMaxGunPoints = 1000;

                BaseFastInterval = 0.275;
                BaseFastDriftInterval = 0.55;
                BaseSlowInterval = 0.55;
                BaseSlowDriftInterval = 1.1;
            }

            if (boat is GalleonBoat)
            {
                shipLevel = 5;
                
                boat.BaseDoubloonValue = 1000;

                BaseMaxHitPoints = 3000;
                BaseMaxSailPoints = 1500;
                BaseMaxGunPoints = 1500;

                BaseFastInterval = 0.3;
                BaseFastDriftInterval = 0.6;
                BaseSlowInterval = 0.6;
                BaseSlowDriftInterval = 1.2;
            }

            //NPC Boat Modifications
            if (boat is SmallFishingBoat || boat is MediumFishingBoat || boat is LargeFishingBoat || boat is CarrackFishingBoat || boat is GalleonFishingBoat)
            {
                boat.MobileControlType = MobileControlType.Innocent;
                boat.MobileFactionType = MobileFactionType.Fishing;
                boat.Hue = 2076;

                factionName = "a fishing";

                DamageFromPlayerBoatScalar = 2.0;                

                boat.DoubloonValue = 25 + (Utility.RandomMinMax(20 * (shipLevel - 1), 30 * (shipLevel - 1)));    
            }

            if (boat is SmallBritainNavyBoat || boat is MediumBritainNavyBoat || boat is LargeBritainNavyBoat || boat is CarrackBritainNavyBoat || boat is GalleonBritainNavyBoat)
            {
                boat.MobileControlType = MobileControlType.Good;
                boat.MobileFactionType = MobileFactionType.Britain;
                boat.Hue = 1102;

                factionName = "a britain navy";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerBoatScalar = 2.0;                

                boat.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (boat is SmallPirateBoat || boat is MediumPirateBoat || boat is LargePirateBoat || boat is CarrackPirateBoat || boat is GalleonPirateBoat)
            {
                boat.MobileControlType = MobileControlType.Evil;
                boat.MobileFactionType = MobileFactionType.Pirate;
                boat.Hue = 1898;

                factionName = "a pirate";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerBoatScalar = 2.0;

                boat.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (boat is SmallOrcBoat || boat is MediumOrcBoat || boat is LargeOrcBoat || boat is CarrackOrcBoat || boat is GalleonOrcBoat)
            {
                boat.MobileControlType = MobileControlType.Evil;
                boat.MobileFactionType = MobileFactionType.Orc;
                boat.Hue = 1164;

                factionName = "an orc";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerBoatScalar = 2.0;

                boat.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (boat is SmallOrghereimBoat || boat is MediumOrghereimBoat || boat is LargeOrghereimBoat || boat is CarrackOrghereimBoat || boat is GalleonOrghereimBoat)
            {
                boat.MobileControlType = MobileControlType.Evil;
                boat.MobileFactionType = MobileFactionType.Orghereim;
                boat.Hue = 1154;

                factionName = "an orghereim";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerBoatScalar = 2.0;

                boat.DoubloonValue = 100 + (Utility.RandomMinMax(40 * (shipLevel - 1), 60 * (shipLevel - 1)));    
            }

            if (boat is SmallUndeadBoat || boat is MediumUndeadBoat || boat is LargeUndeadBoat || boat is CarrackUndeadBoat || boat is GalleonUndeadBoat)
            {
                boat.MobileControlType = MobileControlType.Evil;
                boat.MobileFactionType = MobileFactionType.Undead;
                boat.Hue = 1072;
                boat.CannonHue = 1072;

                factionName = "an undead";

                CannonAccuracyModifer = .9;
                CannonRangeScalar = 1.5;
                DamageFromPlayerBoatScalar = 2.0;

                boat.DoubloonValue = 125 + (Utility.RandomMinMax(50 * (shipLevel - 1), 75 * (shipLevel - 1)));             
            }

            //Outfitting
            BaseBoatOutfittingUpgradeDeed outfitting = boat.GetOutfittingUpgrade();

            if (outfitting != null)
            {
                switch (outfitting.Outfitting)
                {
                    case OutfittingType.Runner:
                        BaseMaxSailPoints = (int)(Math.Round((double)BaseMaxSailPoints * 1.5));

                        BaseFastInterval *= 0.8;
                        BaseFastDriftInterval *= 0.8;
                        BaseSlowInterval *= 0.8;
                        BaseSlowDriftInterval *= 0.8;
                    break;

                    case OutfittingType.Merchant:
                        BaseMaxHitPoints = (int)(Math.Round((double)BaseMaxHitPoints * 1.1));
                        BaseMaxSailPoints = (int)(Math.Round((double)BaseMaxSailPoints * 1.5));

                        BaseFastInterval *= 0.9;
                        BaseFastDriftInterval *= 0.9;
                        BaseSlowInterval *= 0.9;
                        BaseSlowDriftInterval *= 0.9;
                    break;

                    case OutfittingType.Hunter:
                        BaseFastInterval *= 0.9;
                        BaseFastDriftInterval *= 0.9;
                        BaseSlowInterval *= 0.9;
                        BaseSlowDriftInterval *= 0.9;

                        CannonReloadTimeScalar *= 0.5;
                        CannonAccuracyModifer *= 1.1; 
                    break;

                    case OutfittingType.Destroyer:
                        BaseMaxGunPoints = (int)(Math.Round((double)BaseMaxGunPoints * 1.5));

                        CannonReloadTimeScalar *= 0.75;
                        CannonRangeScalar *= 1.25;
                        CannonDamageScalar *= 1.1;
                    break;

                    case OutfittingType.Dreadnought:
                        BaseMaxHitPoints = (int)(Math.Round((double)BaseMaxHitPoints * 1.2));
                        BaseMaxSailPoints = (int)(Math.Round((double)BaseMaxSailPoints * 1.5));
                        BaseMaxGunPoints = (int)(Math.Round((double)BaseMaxGunPoints * 1.5));
                    break;
                }
            }

            boat.BaseMaxHitPoints = BaseMaxHitPoints;
            boat.BaseMaxSailPoints = BaseMaxSailPoints;
            boat.BaseMaxGunPoints = BaseMaxGunPoints;

            boat.BaseFastInterval = BaseFastInterval;
            boat.BaseFastDriftInterval = BaseFastDriftInterval;
            boat.BaseSlowInterval = BaseSlowInterval;
            boat.BaseSlowDriftInterval = BaseSlowDriftInterval;

            if (CannonAccuracyModifer != -1)
                boat.CannonAccuracyModifer = CannonAccuracyModifer;

            if (CannonRangeScalar != -1)
                boat.CannonRangeScalar = CannonRangeScalar;

            if (CannonDamageScalar != -1)
                boat.CannonDamageScalar = CannonDamageScalar;

            if (CannonReloadTimeScalar != -1)
                boat.CannonReloadTimeScalar = CannonReloadTimeScalar;

            if (DamageFromPlayerBoatScalar != -1)
                boat.DamageFromPlayerBoatScalar = DamageFromPlayerBoatScalar;
            
            if (boat.MobileControlType == MobileControlType.Player)
                return;

            boat.HitPoints = BaseMaxHitPoints;
            boat.SailPoints = BaseMaxSailPoints;
            boat.GunPoints = BaseMaxGunPoints;        

            string boatType = "";

            switch (shipLevel)
            {
                case 1: boatType = "small boat"; break;
                case 2: boatType = "medium boat"; break;
                case 3: boatType = "large boat"; break;
                case 4: boatType = "carrack"; break;
                case 5: boatType = "galleon"; break;
            }            

            boat.Name = factionName + " " + boatType;

            boat.m_TargetingMode = TargetingMode.Random;            
        }
    }
}
