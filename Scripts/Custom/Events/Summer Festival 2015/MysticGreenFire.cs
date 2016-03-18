using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Misc;
using Server.Network;
using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class MysticGreenFire : Item
    {
        [Constructable]
        public MysticGreenFire(): base(6571)
        {
            Name = "a mystical green fire";
                        
            Hue = 2003;
            Weight = 1.0;

            Z += 5;

            Movable = false;
        }

        public void ReceiveReward(Mobile from, MagicSpringwood magicSpringwood)
        {
            if (from == null || magicSpringwood == null)
                return;

            magicSpringwood.inUse = true;

            int TotalValues = 0;

            Item item = null;
            string rewardText = "";

            Dictionary<string, int> DictTemp = new Dictionary<string, int>();

            //Used to Determine Chance of Last Reward Item (GoldSmallAmount)
            int maxRandomValue = 10000;

            //Add Rewards (Chance = Value / Sum of All Item Values Combined)            
            DictTemp.Add("Seed", 1000);

            // Vines (14 variants * 1.25 = 17.5% for a random vine) 
            DictTemp.Add("HangingPinkFlowersVinesEast", 225);
            DictTemp.Add("HangingPinkFlowersVinesSouth", 225);
            DictTemp.Add("HangingPurpleFlowersVinesEast", 225);
            DictTemp.Add("HangingPurpleFlowersVinesSouth", 225);
            DictTemp.Add("HangingVinesLeft", 225);
            DictTemp.Add("HangingVinesLeftCenter", 225);
            DictTemp.Add("HangingVinesRight", 225);
            DictTemp.Add("HangingVinesRightCenter", 225);
            DictTemp.Add("VinesBramblyEast", 225);
            DictTemp.Add("VinesBramblySouth", 225);
            DictTemp.Add("VinesLeafyEast", 225);
            DictTemp.Add("VinesLeafySouth", 225);
            DictTemp.Add("VinesRegularEast", 225);
            DictTemp.Add("VinesRegularSouth", 225);

            foreach (KeyValuePair<string, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            int remainderItemChance = maxRandomValue - TotalValues;

            if (remainderItemChance < 1)
                remainderItemChance = 1;

            //Last Reward Item (Chance = maxRandomValue - Sum of All Item Values Combined)
            DictTemp.Add("FertileDirt", remainderItemChance);

            TotalValues += remainderItemChance;

            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine Reward                      
            foreach (KeyValuePair<string, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    switch (pair.Key)
                    {
                        case "FertileDirt":
                            item = new FertileDirt(Utility.RandomMinMax(8, 12));
                            rewardText = "You receive some fertile dirt!";
                        break;

                        case "Seed":
                            item = new Seed();
                            rewardText = "You receive a plant seed!";
                        break;

                        case "HangingPinkFlowersVinesEast":
                            item = new HangingPinkFlowersVinesEast();
                            rewardText = "You receive hanging vines with pink flowers!";
                        break;

                        case "HangingPinkFlowersVinesSouth":
                            item = new HangingPinkFlowersVinesSouth();
                            rewardText = "You receive hanging vines with pink flowers!";
                        break;

                        case "HangingPurpleFlowersVinesEast":
                            item = new HangingPurpleFlowersVinesEast();
                            rewardText = "You receive hanging vines with purple flowers!";
                            break;

                        case "HangingPurpleFlowersVinesSouth":
                            item = new HangingPurpleFlowersVinesSouth();
                            rewardText = "You receive hanging vines with purple flowers!";
                        break;

                        case "HangingVinesLeft":
                            item = new HangingVinesLeft();
                            rewardText = "You receive hanging vines!";
                        break;

                        case "HangingVinesLeftCenter":
                            item = new HangingVinesLeftCenter();
                            rewardText = "You receive hanging vines!";
                        break;

                        case "HangingVinesRight":
                            item = new HangingVinesRight();
                            rewardText = "You receive hanging vines!";
                            break;

                        case "HangingVinesRightCenter":
                            item = new HangingVinesRightCenter();
                            rewardText = "You receive hanging vines!";
                        break;

                        case "VinesBramblyEast":
                            item = new VinesBramblyEast();
                            rewardText = "You receive brambly vines!";
                            break;

                        case "VinesBramblySouth":
                            item = new VinesBramblySouth();
                            rewardText = "You receive brambly vines!";
                        break;

                        case "VinesLeafyEast":
                            item = new VinesLeafyEast();
                            rewardText = "You receive leafy vines!";
                        break;

                        case "VinesLeafySouth":
                            item = new VinesLeafySouth();
                            rewardText = "You receive leafy vines!";
                        break;

                        case "VinesRegularEast":
                            item = new VinesRegularEast();
                            rewardText = "You receive vines!";
                        break;

                        case "VinesRegularSouth":
                            item = new VinesRegularSouth();
                            rewardText = "You receive vines!";
                        break;
                    }

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }

            if (item != null)
            {
                if (!from.AddToBackpack(item))
                {
                    item.Delete();
                    from.SendMessage("You don't have enough room in your backpack. Please make room and try again.");

                    return;
                }

                else
                {
                    if (from.NetState != null)
                        from.PrivateOverheadMessage(MessageType.Regular, 0, false, rewardText, from.NetState);
                    
                    Effects.SendLocationEffect(Location, Map, 0x3709, 30, 2002, 0);
                    Effects.PlaySound(Location, Map, 0x591);

                    magicSpringwood.Delete();
                }
            }
        }

        public MysticGreenFire(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}