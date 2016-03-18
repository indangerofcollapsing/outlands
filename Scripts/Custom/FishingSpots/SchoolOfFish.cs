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
    public class SchoolOfFish : FishingSpot
    {
        [Constructable]
        public SchoolOfFish(): base()
        {
            Name = "a school of fish";
        }

        public override void AddComponents()
        {
            base.AddComponents();

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

                        switch (Utility.RandomMinMax(1, 33))
                        {
                            case 1: itemId = 3339; hue = 0; break;
                            case 2: itemId = 3374; hue = 2209; break;
                            case 3: itemId = 3337; hue = 0; break;
                            case 4: itemId = 3392; hue = 2209; break;
                            case 5: itemId = 3515; hue = 2209; break;
                            case 6: itemId = 3514; hue = 2209; break;
                            case 7: itemId = 3336; hue = 0; break;
                            case 8: itemId = 3332; hue = 0; break;
                            case 9: itemId = 3338; hue = 0; break;
                            case 10: itemId = 3310; hue = 2209; break;
                            case 11: itemId = 3314; hue = 2209; break;
                            case 12: itemId = 3372; hue = 2209; break;
                            case 13: itemId = 3370; hue = 2209; break;
                            case 14: itemId = 3367; hue = 2209; break;
                            case 15: itemId = 3378; hue = 2209; break;
                            case 16: itemId = 3379; hue = 2209; break;
                            case 17: itemId = 3376; hue = 2209; offsetZ = -3; break;
                            case 18: itemId = 3377; hue = 2209; offsetZ = -3; break;
                            case 19: itemId = 3381; hue = 2209; break;
                            case 20: itemId = 3302; hue = 2209; break;
                            case 21: itemId = 3518; hue = 0; break;
                            case 22: itemId = 3516; hue = 0; break;
                            case 23: itemId = 6001; hue = 0; break;
                            case 24: itemId = 6002; hue = 0; break;
                            case 25: itemId = 6003; hue = 0; break;
                            case 26: itemId = 6004; hue = 0; break;
                            case 27: itemId = 6005; hue = 0; break;
                            case 28: itemId = 6006; hue = 0; break;
                            case 29: itemId = 6007; hue = 0; break;
                            case 30: itemId = 6008; hue = 0; break;
                            case 31: itemId = 6809; hue = 2208; break;
                            case 32: itemId = 6810; hue = 2208; break;
                            case 33: itemId = 6811; hue = 2208; break;
                        }

                        GroupItem(new Static(itemId), hue, a, b, offsetZ, "a fishing spot");
                    }
                }
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

                if (eventResult <= 60)
                    FindFish(from, 1);

                else if (eventResult <= .75)
                    FindFish(from, 2);

                else if (eventResult <= .90)
                    FindFish(from, 3);

                else if (eventResult <= .95)
                    FindCraftingComponent(from);

                else if (eventResult <= .97)
                    FindCreature(from, 1);

                else if (eventResult <= .98)
                    FindCreature(from, 2);

                else if (eventResult <= .99)
                    FindCreature(from, 3);

                else
                    FindCreature(from, 4);
            }

            else
                from.SendMessage("You fail to find anything.");

            base.FishingResult(from);
        }

        public void FindFish(Mobile from, int level)
        {
            if (from == null)
                return;

            Item item = null;
            string itemName = "";

            switch (level)
            {
                case 1:
                    //Normal Fish
                    item = new Fish(Utility.RandomMinMax(10, 20));
                    itemName = "a pile of fish"; 
                break;
                    
                case 2:
                    //Magic Fish
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1:
                            item = new WondrousFish();
                            itemName = "a wonderous fish";
                        break;

                        case 2:
                            item = new PeculiarFish();
                            itemName = "a perculiar fish";
                        break;

                        case 3:
                            item = new TrulyRareFish();
                            itemName = "a truly rare fish";
                        break;
                    }               
                break;

                case 3:
                    //Fish Parts
                    switch (Utility.RandomMinMax(1, 2))
                    {
                        case 1:
                            item = new FishHeads();
                            itemName = "some fish heads";
                        break;

                        case 2:
                            item = new FishBones();
                            itemName = "some fishbones";
                        break;
                    }
                break;
            }

            if (item != null)
            {
                if (from.AddToBackpack(item))
                    from.SendMessage("You reel in " + itemName + " from the water and place them in your backpack.");

                else
                {
                    from.SendMessage("You reel in " + itemName + " from the water and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }
        }

        public void FindCraftingComponent(Mobile from)
        {
            Item item = CraftingComponent.GetRandomCraftingComponent(1);

            if (item != null)
            {
                if (from.AddToBackpack(item))
                    from.SendMessage("You retrieve " + item.Name + " from the shipwreck and place it in your backpack.");

                else
                {
                    from.SendMessage("You retrieve " + item.Name + " from the shipwreck, and place it at your feet.");
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
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something rises from the water*");
                break;

                case 2:
                    count = Utility.RandomMinMax(1, 2);

                    for (int a = 0; a < count; a++)
                    {
                        BaseCreature bc_Creature = new WaterElemental();

                        bc_Creature.m_WasFishedUp = true;
                        bc_Creature.MoveToWorld(spawnLocation, from.Map);
                        spawnedCreatures = true;
                    }

                    if (spawnedCreatures)
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something rises from the water*");
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
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something rises from the water*");
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
                        from.PublicOverheadMessage(MessageType.Regular, 0, false, "*something rises from the water*");
                break;
            }
        }

        public override void FishingFinalResult(Mobile from)
        {
            if (Utility.RandomDouble() < .33)
            {
                AquariumItem item = AquariumItem.GetRandomFish(m_Rarity);

                if (item == null)
                    return;

                from.PlaySound(0x026);

                if (from.AddToBackpack(item))
                    from.SendMessage("You reel in a creature place it in your backpack.");

                else
                {
                    from.SendMessage("You reel in a creature place and place it at your feet.");
                    item.MoveToWorld(from.Location, from.Map);
                }
            }

            base.FishingFinalResult(from);
        }

        public SchoolOfFish(Serial serial): base(serial)
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
