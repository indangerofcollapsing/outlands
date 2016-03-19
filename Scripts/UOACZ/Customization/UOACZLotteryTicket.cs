using Server.Multis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;
using Server.Misc;
using Server.Network;
using Server.Custom;

namespace Server.Items
{
    class UOACZLotteryTicket : Item
    {  
        public bool inUse = false;

        [Constructable]
        public UOACZLotteryTicket(): base(0x14ED)
        {
            Name = "a UOACZ lottery ticket";

            Hue = 2408;
            Weight = 0.1;            
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (inUse)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            if (from.TotalWeight >= 375 && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are carrying too much weight to be able to accept any rewards.");
                return;
            }

            from.SendSound(0x249);
            from.SendMessage("You unfurl a UOACZ lottery ticket...");

            inUse = true;

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (from == null)
                    return;

                ReceiveReward(from);
            });
        }

        public void ReceiveReward(Mobile from)
        {
            int TotalValues = 0;   

            Item item = null;
            string rewardText = "";
            int rewardSound = 0x5AA;

            Dictionary<string, int> DictTemp = new Dictionary<string, int>();
            
            int ultraRareValue = 1;
            int rareValue = 2;
            int uncommonValue = 4;
            int commonValue = 8;

            //Ultra Rares
            DictTemp.Add("UltraRareHenchman", ultraRareValue);

            //Rares
            DictTemp.Add("RareHenchman", rareValue);

            //Uncommon
            DictTemp.Add("UncommonHenchman", uncommonValue);

            //Common
            DictTemp.Add("CommonHenchman", commonValue);
            
            foreach (KeyValuePair<string, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }
            
            double ItemCheck = Utility.RandomDouble();

            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine Reward                      
            foreach (KeyValuePair<string, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                {
                    Bag bag;

                    switch (pair.Key)
                    {
                        //Ultra Rares
                        case "UltraRareHenchman":
                            switch(Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.VampireCountess); break;
                                case 2: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.Mummy); break;
                            }
                            
                            rewardText = "You receive a henchman creation deed.";
                        break;

                        //Rares
                        case "RareHenchman":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.VampireThrall); break;
                                case 2: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.RottingCorpse); break;
                            }

                            rewardText = "You receive a henchman creation deed.";
                        break;

                        //Uncommon
                        case "UncommonHenchman":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.Lich); break;
                                case 2: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.Spectre); break;
                            }

                            rewardText = "You receive a henchman creation deed.";
                        break;

                        //Common
                        case "CommonHenchman":
                            switch (Utility.RandomMinMax(1, 2))
                            {
                                case 1: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.Zombie); break;
                                case 2: item = new HenchmanCreationDeed(HenchmanCreationDeed.CreatureType.Skeleton); break;
                            }

                            rewardText = "You receive a henchman creation deed.";
                        break;
                    }

                    break;
                }

                CumulativeAmount += AdditionalAmount;
            }
            
            if (item != null)
            {
                if (from.TotalWeight >= 390 && from.AccessLevel == AccessLevel.Player)
                {
                    item.Delete();
                    from.SendMessage("You are carrying too much weight to be able to accept any rewards.");

                    inUse = false;

                    return;
                }

                if (!from.AddToBackpack(item))
                {
                    item.Delete();
                    from.SendMessage("You don't have enough room in your backpack. Please make room and try again.");

                    inUse = false;

                    return;
                }

                else
                {
                    if (from.NetState != null)
                        from.PrivateOverheadMessage(MessageType.Regular, 0, false, rewardText, from.NetState);

                    if (rewardSound != -1)
                        from.SendSound(rewardSound);

                    Delete();
                }
            }

            else            
                inUse = false;            
        }

        public UOACZLotteryTicket(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);         
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
