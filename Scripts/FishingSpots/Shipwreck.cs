using System;
using Server;
using System.Collections.Generic;
using Server.Network;
using Server.Prompts;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;
using Server.Gumps;
using Server.Items;
using Server.Spells;

namespace Server.Custom
{
    public class Shipwreck : FishingSpot
    {
        public enum ShipwreckType
        {
            Raft,
            Debris
            //SmallBoat,            
        }

        [Constructable]
        public Shipwreck(): base()
        {
            Name = "a shipwreck";
        }

        public override void AddComponents()
        {
            base.AddComponents();

            ShipwreckType shipwreckType = (ShipwreckType)Utility.RandomMinMax(0, 1);

            switch (shipwreckType)
            {
                case ShipwreckType.Raft:
                    //Raft
                    GroupItem(new Static(1221), 0, -1, -1, 0, "a lifeless raft"); //Raft
                    GroupItem(new Static(1221), 0, 0, -1, 0, "a lifeless raft");
                    GroupItem(new Static(1221), 0, 1, -1, 0, "a lifeless raft");
                    GroupItem(new Static(1217), 0, -1, 0, 0, "a lifeless raft");
                    GroupItem(new Static(1217), 0, 0, 0, 0, "a lifeless raft");
                    GroupItem(new Static(1217), 0, 1, 0, 0, "a lifeless raft");
                    GroupItem(new Static(1220), 0, -1, 1, 0, "a lifeless raft");
                    GroupItem(new Static(1220), 0, 0, 1, 0, "a lifeless raft");
                    GroupItem(new Static(1220), 0, 1, 1, 0, "a lifeless raft");

                    GroupItem(new Static(563), 0, 0, 0, 1, "a lifeless raft"); //Sail Pole

                    GroupItem(new Static(2646), 0, 1, 0, -2, "a lifeless raft"); //Mattress
                    GroupItem(new Static(3520), 0, -1, 1, 1, "a lifeless raft"); //Fishing Pole
                    GroupItem(new Static(3792), 0, 0, 1, 1, "a lifeless raft"); //Skeleton
                    GroupItem(new Static(3619), 2503, -1, 0, 1, "a lifeless raft"); //Bowl
                    GroupItem(new Static(2581), 0, 1, 1, 1, "a lifeless raft"); //Lantern
                    GroupItem(new Static(4103), 2213, 0, 1, 2, "a jar of yellow liquid"); //Pee Jar
                    GroupItem(new Static(7132), 0, -1, -1, 1, "a lifeless raft"); //Lumberpile
                    GroupItem(new Static(15116), 0, -1, 0, 2, "a lifeless raft"); //Fishbones
                break;

                case ShipwreckType.Debris:
                    for (int a = -2; a < 3; a++)
                    {
                        for (int b = -2; b < 3; b++)
                        {
                            if (a == 0 && b == 0)
                                continue;

                            if (Utility.RandomDouble() <= .66)
                            {
                                int itemId = 0;
                                int hue = 0;
                                int offsetZ = 0;

                                switch (Utility.RandomMinMax(1, 173))
                                {
                                    case 1: itemId = 3310; hue = 0; break;
                                    case 2: itemId = 3314; hue = 0; break;
                                    case 3: itemId = 3332; hue = 0; break;
                                    case 4: itemId = 3378; hue = 0; break;
                                    case 5: itemId = 3392; hue = 2208; break;
                                    case 6: itemId = 3376; hue = 2208; offsetZ = -3; break;
                                    case 7: itemId = 3377; hue = 2208; offsetZ = -3; break;
                                    case 8: itemId = 3514; hue = 2208; break;
                                    case 9: itemId = 3514; hue = 2208; break;
                                    case 10: itemId = 3515; hue = 2208; break;
                                    case 11: itemId = 3515; hue = 2208; break;
                                    case 12: itemId = 3582; hue = 2208; break;
                                    case 13: itemId = 3707; hue = 0; break;
                                    case 14: itemId = 3711; hue = 0; break;
                                    case 15: itemId = 3892; hue = 1845; break;
                                    case 16: itemId = 4150; hue = 0; break;
                                    case 17: itemId = 4151; hue = 0; break;
                                    case 18: itemId = 4152; hue = 0; break;
                                    case 19: itemId = 6001; hue = 0; break;
                                    case 20: itemId = 6002; hue = 0; break;
                                    case 21: itemId = 6003; hue = 0; break;
                                    case 22: itemId = 6004; hue = 0; break;
                                    case 23: itemId = 6005; hue = 0; break;
                                    case 24: itemId = 6006; hue = 0; break;
                                    case 25: itemId = 6007; hue = 0; break;
                                    case 26: itemId = 6008; hue = 0; break;
                                    case 27: itemId = 6804; hue = 2212; break;
                                    case 28: itemId = 6805; hue = 2212; break;
                                    case 29: itemId = 6806; hue = 2212; break;
                                    case 30: itemId = 6943; hue = 0; break;
                                    case 31: itemId = 6944; hue = 0; break;
                                    case 32: itemId = 6945; hue = 0; break;
                                    case 33: itemId = 6946; hue = 0; break;
                                    case 34: itemId = 7041; hue = 0; break;
                                    case 35: itemId = 7044; hue = 0; break;
                                    case 36: itemId = 7053; hue = 0; break;
                                    case 37: itemId = 7067; hue = 0; break;
                                    case 38: itemId = 7068; hue = 0; break;
                                    case 39: itemId = 7069; hue = 0; break;
                                    case 40: itemId = 7070; hue = 0; break;
                                    case 41: itemId = 7681; hue = 0; break;
                                    case 42: itemId = 7682; hue = 0; break;
                                    case 43: itemId = 7857; hue = 0; break;
                                    case 44: itemId = 7858; hue = 0; break;
                                    case 45: itemId = 7859; hue = 0; break;
                                    case 46: itemId = 7860; hue = 0; break;
                                    case 47: itemId = 7861; hue = 0; break;
                                    case 48: itemId = 8762; hue = 2311; break;
                                    case 49: itemId = 8766; hue = 2311; break;
                                    case 50: itemId = 8769; hue = 2311; break;
                                    case 51: itemId = 8772; hue = 2311; break;
                                    case 52: itemId = 8777; hue = 2311; break;
                                    case 53: itemId = 12320; hue = 2210; break;
                                    case 54: itemId = 12321; hue = 2210; break;
                                    case 55: itemId = 12322; hue = 2210; break;
                                    case 56: itemId = 12323; hue = 2210; break;
                                    case 57: itemId = 12324; hue = 2210; break;

                                    case 58: itemId = 13345; hue = 0; break;
                                    case 59: itemId = 13345; hue = 0; break;
                                    case 60: itemId = 13345; hue = 0; break;
                                    case 61: itemId = 13345; hue = 0; break;
                                    case 62: itemId = 13345; hue = 0; break;

                                    case 63: itemId = 13351; hue = 0; break;
                                    case 64: itemId = 13351; hue = 0; break;
                                    case 65: itemId = 13351; hue = 0; break;
                                    case 66: itemId = 13351; hue = 0; break;
                                    case 67: itemId = 13351; hue = 0; break;

                                    case 68: itemId = 13484; hue = 0; break;
                                    case 69: itemId = 13484; hue = 0; break;
                                    case 70: itemId = 13484; hue = 0; break;
                                    case 71: itemId = 13484; hue = 0; break;
                                    case 72: itemId = 13484; hue = 0; break;

                                    case 73: itemId = 13488; hue = 0; break;
                                    case 74: itemId = 13488; hue = 0; break;
                                    case 75: itemId = 13488; hue = 0; break;
                                    case 76: itemId = 13488; hue = 0; break;
                                    case 77: itemId = 13488; hue = 0; break;

                                    case 78: itemId = 3117; hue = 0; break;
                                    case 79: itemId = 3118; hue = 0; break;
                                    case 80: itemId = 3119; hue = 0; break;
                                    case 81: itemId = 3120; hue = 0; break;
                                    case 82: itemId = 3117; hue = 0; break;
                                    case 83: itemId = 3118; hue = 0; break;
                                    case 84: itemId = 3119; hue = 0; break;
                                    case 85: itemId = 3120; hue = 0; break;
                                    case 86: itemId = 3117; hue = 0; break;
                                    case 87: itemId = 3118; hue = 0; break;
                                    case 88: itemId = 3119; hue = 0; break;
                                    case 89: itemId = 3120; hue = 0; break;
                                    case 90: itemId = 3553; hue = 0; break;
                                    case 91: itemId = 3553; hue = 0; break;
                                    case 92: itemId = 3553; hue = 0; break;
                                    case 93: itemId = 7607; hue = 0; break;
                                    case 94: itemId = 7127; hue = 0; break;
                                    case 95: itemId = 7128; hue = 0; break;
                                    case 96: itemId = 7130; hue = 0; break;
                                    case 97: itemId = 7131; hue = 0; break;
                                    case 98: itemId = 3117; hue = 0; break;
                                    case 99: itemId = 3118; hue = 0; break;
                                    case 100: itemId = 3119; hue = 0; break;
                                    case 101: itemId = 3120; hue = 0; break;
                                    case 102: itemId = 3117; hue = 0; break;
                                    case 103: itemId = 3118; hue = 0; break;
                                    case 104: itemId = 3119; hue = 0; break;
                                    case 105: itemId = 3120; hue = 0; break;
                                    case 106: itemId = 3117; hue = 0; break;
                                    case 107: itemId = 3118; hue = 0; break;
                                    case 108: itemId = 3119; hue = 0; break;
                                    case 109: itemId = 3120; hue = 0; break;
                                    case 110: itemId = 3553; hue = 0; break;
                                    case 111: itemId = 3553; hue = 0; break;
                                    case 112: itemId = 3553; hue = 0; break;
                                    case 113: itemId = 7607; hue = 0; break;
                                    case 114: itemId = 7127; hue = 0; break;
                                    case 115: itemId = 7128; hue = 0; break;
                                    case 116: itemId = 7130; hue = 0; break;
                                    case 117: itemId = 7131; hue = 0; break;                                        
                                    case 118: itemId = 3117; hue = 0; break;
                                    case 119: itemId = 3118; hue = 0; break;
                                    case 120: itemId = 3119; hue = 0; break;
                                    case 121: itemId = 3120; hue = 0; break;
                                    case 122: itemId = 3117; hue = 0; break;
                                    case 123: itemId = 3118; hue = 0; break;
                                    case 124: itemId = 3119; hue = 0; break;
                                    case 125: itemId = 3120; hue = 0; break;
                                    case 126: itemId = 3117; hue = 0; break;
                                    case 127: itemId = 3118; hue = 0; break;
                                    case 128: itemId = 3119; hue = 0; break;
                                    case 129: itemId = 3120; hue = 0; break;
                                    case 130: itemId = 3553; hue = 0; break;
                                    case 131: itemId = 3553; hue = 0; break;
                                    case 132: itemId = 3553; hue = 0; break;
                                    case 133: itemId = 7607; hue = 0; break;
                                    case 134: itemId = 7127; hue = 0; break;
                                    case 135: itemId = 7128; hue = 0; break;
                                    case 136: itemId = 7130; hue = 0; break;
                                    case 137: itemId = 7131; hue = 0; break;
                                    case 138: itemId = 3117; hue = 0; break;
                                    case 139: itemId = 3118; hue = 0; break;
                                    case 140: itemId = 3119; hue = 0; break;
                                    case 141: itemId = 3120; hue = 0; break;
                                    case 142: itemId = 3117; hue = 0; break;
                                    case 143: itemId = 3118; hue = 0; break;
                                    case 144: itemId = 3119; hue = 0; break;
                                    case 145: itemId = 3120; hue = 0; break;
                                    case 146: itemId = 3117; hue = 0; break;
                                    case 147: itemId = 3118; hue = 0; break;
                                    case 148: itemId = 3119; hue = 0; break;
                                    case 149: itemId = 3120; hue = 0; break;
                                    case 150: itemId = 3553; hue = 0; break;
                                    case 151: itemId = 3553; hue = 0; break;
                                    case 152: itemId = 3553; hue = 0; break;
                                    case 153: itemId = 7607; hue = 0; break;
                                    case 154: itemId = 7127; hue = 0; break;
                                    case 155: itemId = 7128; hue = 0; break;
                                    case 156: itemId = 7130; hue = 0; break;
                                    case 157: itemId = 7131; hue = 0; break;

                                    case 158: itemId = 3339; hue = 0; break;
                                    case 159: itemId = 3374; hue = 2209; break;
                                    case 160: itemId = 3337; hue = 0; break;
                                    case 161: itemId = 3392; hue = 2209; break;
                                    case 162: itemId = 3515; hue = 2209; break;
                                    case 163: itemId = 3514; hue = 2209; break;
                                    case 164: itemId = 3336; hue = 0; break;
                                    case 165: itemId = 3332; hue = 0; break;
                                    case 166: itemId = 3338; hue = 0; break;
                                    case 167: itemId = 3310; hue = 2209; break;
                                    case 168: itemId = 3314; hue = 2209; break;
                                    case 169: itemId = 3372; hue = 2209; break;
                                    case 170: itemId = 3370; hue = 2209; break;
                                    case 171: itemId = 3367; hue = 2209; break;
                                    case 172: itemId = 3378; hue = 2209; break;
                                    case 173: itemId = 3379; hue = 2209; break;
                                }

                                GroupItem(new Static(itemId), hue, a, b, offsetZ, "a shipwreck");
                            }
                        }
                    }
                break;

                    /*
                case ShipwreckType.SmallBoat:
                    GroupItem(new Static(0), 0, 0, 0, 0, "a shipwreck");
                break;
                */                
            }
        }

        public override void FishingResult(Mobile from)
        {
            if (from == null)
                return;
            
            double chance = .20 * (from.Skills.Fishing.Value / 120);

            if (Utility.RandomDouble() <= chance)
            {
                double eventResult = Utility.RandomDouble();

                from.PlaySound(0x025);
                
                if (eventResult <= .60)
                    FindJunk(from);

                else if (eventResult <= .75)
                    FindResources(from);

                else if (eventResult <= .90)
                    FindDoubloons(from);

                else if (eventResult <= .95)
                    FindPrestigeScroll(from);                

                else if (eventResult <= .97)
                    FindCreature(from, 1);

                else if (eventResult <= .98)
                    FindCreature(from, 2);

                else if (eventResult <= .99)
                    FindCreature(from, 3);

                else if (eventResult <= .995)
                    FindResearchMaterials(from);

                else
                    FindCreature(from, 4);                
            }

            else
                from.SendMessage("You fail to find anything.");

            base.FishingResult(from);
        }               

        public void FindJunk(Mobile from)
        {
            if (from == null)
                return;

            Item item = null;

            switch (Utility.RandomMinMax(1, 10))
            {
                case 1: item = new Torch(); break;
                case 2: item = new Lantern(); break;
                case 3: item = new Skillet(); break;
                case 4: item = new Beeswax(); break;
                case 5: item = new Bedroll(); break;
                case 6: item = new Dices(); break;
                case 7: item = new Kindling(); break;
                case 8: item = new Bottle(); break;
                case 9: item = new BeverageBottle(BeverageType.Ale); break;
                case 10: item = new Jug(BeverageType.Cider); break;
            }

            if (item != null)
            {
                if (from.AddToBackpack(item))
                    from.SendMessage("You retrieve some junk from the shipwreck and place it in your backpack.");

                else
                {
                    from.SendMessage("You retrieve some junk from the shipwreck, and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }
        }

        public void FindResources(Mobile from)
        {
            if (from == null)
                return;

            int minAmount = 0;
            int maxAmount = 0;

            Item item = null;
            string itemName = "";

            switch (Utility.RandomMinMax(1, 5))
            {
                //Gold
                case 1:
                    minAmount = ((int)m_Rarity + 1) * 150;
                    maxAmount = ((int)m_Rarity + 1) * 250;

                    item = new Gold(Utility.RandomMinMax(minAmount, maxAmount));
                    itemName = "some gold";
                break;

                //Iron Ingots
                case 2:
                    minAmount = ((int)m_Rarity + 1) * 25;
                    maxAmount = ((int)m_Rarity + 1) * 50;

                    item = new IronIngot(Utility.RandomMinMax(minAmount, maxAmount));
                    itemName = "some iron ingots";
                break;

                //Boards
                case 3:
                    minAmount = ((int)m_Rarity + 1) * 25;
                    maxAmount = ((int)m_Rarity + 1) * 50;

                    item = new Board(Utility.RandomMinMax(minAmount, maxAmount));
                    itemName = "some boards";
                break;

                //Cloth
                case 4:
                    minAmount = ((int)m_Rarity + 1) * 25;
                    maxAmount = ((int)m_Rarity + 1) * 50;

                    item = new Cloth(Utility.RandomMinMax(minAmount, maxAmount));
                    itemName = "some cloth";
                break;

                //Cut Leather
                case 5:
                    minAmount = ((int)m_Rarity + 1) * 25;
                    maxAmount = ((int)m_Rarity + 1) * 50;

                    item = new Leather(Utility.RandomMinMax(minAmount, maxAmount));
                    itemName = "some leather";
                break;
            }

            if (item != null)
            {
                if (from.AddToBackpack(item))
                    from.SendMessage("You retrieve " + itemName + " from the shipwreck and place it in your backpack.");

                else
                {
                    from.SendMessage("You retrieve " + itemName + " from the shipwreck, and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }
        }

        public void FindDoubloons(Mobile from)
        {
            if (from == null)
                return;

            int minDoubloons = 5;
            int maxDoubloons = 10;

            switch (m_Rarity)
            {
                case AquariumItem.Rarity.Common:
                    minDoubloons = 25;
                    maxDoubloons = 50;
                break;

                case AquariumItem.Rarity.Uncommon:
                    minDoubloons = 50;
                    maxDoubloons = 100;
                break;

                case AquariumItem.Rarity.Rare:
                    minDoubloons = 100;
                    maxDoubloons = 250;
                break;

                case AquariumItem.Rarity.UltraRare:
                    minDoubloons = 250;
                    maxDoubloons = 500;
                break;
            }

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (player != null)
            {
                if (!player.Deleted && player.BoatOccupied != null)
                {
                    int doubloonValue = Utility.RandomMinMax(minDoubloons, maxDoubloons);
                    
                    if (player.BoatOccupied.DepositDoubloons(doubloonValue))
                    {
                        Doubloon doubloonPile = new Doubloon(doubloonValue);
                        player.SendSound(doubloonPile.GetDropSound());
                        doubloonPile.Delete();

                        player.BoatOccupied.doubloonsEarned += doubloonValue;

                        player.SendMessage("You have recovered " + doubloonValue.ToString() + " doubloons from the cargo! The coins have been placed in your ship's hold.");

                    }

                    else
                        player.SendMessage("You have earned doubloons, but alas there was not enough room in your ship's hold to place them all!");
                }
            }
        }

        public void FindResearchMaterials(Mobile from)
        {
            Item item = new ResearchMaterials();

            if (item != null)
            {
                if (from.AddToBackpack(item))
                    from.SendMessage("You retrieve some Research Materials safely stashed away within the shipwreck and place it in your backpack.");

                else
                {
                    from.SendMessage("You retrieve some Research Materials safely stashed away within the shipwreck, and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }
        }  

        public void FindPrestigeScroll(Mobile from)
        {
            Item item = new PrestigeScroll();            

            if (item != null)
            {
                if (from.AddToBackpack(item))
                    from.SendMessage("You retrieve a Prestige Scroll from the shipwreck and place it in your backpack.");

                else
                {
                    from.SendMessage("You retrieve a Prestige Scroll from the shipwreck, and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }
        }        

        public void FindCreature(Mobile from, int creatureLevel)
        {
            if (from == null)
                return;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;           
         
            int waterLocationChecks = 20;

            int minSpawnRadius = 3;
            int maxSpawnRadius = 6;

            bool foundWaterSpot = false;
            bool spawnedCreatures = false;

            Point3D spawnLocation = Location;
            Point3D newLocation = new Point3D();

            for (int a = 0; a < waterLocationChecks; a++)
            {
                int x = X;

                int xOffset = Utility.RandomMinMax(minSpawnRadius, maxSpawnRadius);
                if (Utility.RandomDouble() >= .5)
                    xOffset *= -1;

                x += xOffset;

                int y = Y;

                int yOffset = Utility.RandomMinMax(minSpawnRadius, maxSpawnRadius);
                if (Utility.RandomDouble() >= .5)
                    yOffset *= -1;

                y += yOffset;

                newLocation.X = x;
                newLocation.Y = y;
                newLocation.Z = -5;

                bool waterTile = BaseBoat.IsWaterTile(newLocation, Map);

                if (waterTile)
                {
                    if (BaseBoat.FindBoatAt(newLocation, Map) != null)
                        continue;

                    SpellHelper.AdjustField(ref spawnLocation, Map, 12, false);
                   
                    foundWaterSpot = true;
                    break;                    
                }                          
            }

            if (!foundWaterSpot)            
                return;            

            int count = 0;

            switch (creatureLevel)
            {
                case 1: 
                    count = Utility.RandomMinMax(1, 2);

                    for (int a = 0; a < count; a++)
                    {
                        BaseCreature bc_Creature = new Puddle();

                        bc_Creature.m_WasFishedUp = true;
                        bc_Creature.MoveToWorld(spawnLocation, from.Map);
                        spawnedCreatures = true;
                    }

                    if (spawnedCreatures)
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something was hiding in the wreckage!*");
                break;

                case 2:
                    if (player.BoatOccupied != null)
                    {
                        if (!player.BoatOccupied.Deleted && player.BoatOccupied.m_SinkTimer == null)
                        {
                            count = Utility.RandomMinMax(2, 4);

                            for (int a = 0; a < count; a++)
                            {
                                BaseCreature bc_Creature = new ColossusTermite();

                                bc_Creature.m_WasFishedUp = true;
                                bc_Creature.MoveToWorld(player.BoatOccupied.GetRandomEmbarkLocation(false), from.Map);
                                spawnedCreatures = true;
                            }
                        }
                    }

                    if (spawnedCreatures)
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*the wreckage was full of termites!*");
                break;

                case 3:
                    count = Utility.RandomMinMax(1, 2);

                    for (int a = 0; a < count; a++)
                    {
                        BaseCreature bc_Creature = new DeepSeaSerpent();

                        bc_Creature.m_WasFishedUp = true;
                        bc_Creature.MoveToWorld(spawnLocation, from.Map);
                        spawnedCreatures = true;
                    }

                    if (spawnedCreatures)
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something was hiding in the wreckage!*");
                break;

                case 4:
                    count = Utility.RandomMinMax(1, 2);

                    for (int a = 0; a < count; a++)
                    {
                        BaseCreature bc_Creature = new Kraken();

                        bc_Creature.m_WasFishedUp = true;
                        bc_Creature.MoveToWorld(spawnLocation, from.Map);
                        spawnedCreatures = true;
                    }

                    if (spawnedCreatures)
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something was hiding in the wreckage!*");
                break;
            }
        }

        public override void FishingFinalResult(Mobile from)
        {
            if (Utility.RandomDouble() < .33)
            {
                AquariumItem item = AquariumItem.GetRandomDecoration(m_Rarity);

                if (item == null)
                    return;

                from.PlaySound(0x026);

                if (from.AddToBackpack(item))
                    from.SendMessage("You retrieve some sunken cargo from the shipwreck and place it in your backpack.");

                else
                {
                    from.SendMessage("You retrieve some sunken cargo from the shipwreck, and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }

            else            
                from.SendMessage("Alas, you find the shipwreck to be devoid of any further items of value.");

            base.FishingFinalResult(from);
        } 

        public Shipwreck(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);                
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}