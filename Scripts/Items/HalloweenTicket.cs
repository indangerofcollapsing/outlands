using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;

namespace Server.Custom
{
    public class HalloweenTicket : Item
    {
        private bool m_Used = false;

        public override string DefaultName { get { return "Halloween Lottery Ticket"; } }

        [Constructable]
        public HalloweenTicket()
            : base(0x14ED)
        {
            LootType = Server.LootType.Regular;
            DonationItem = false;
            Hue = 143;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Use to receive a special Halloween prize!");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Used)
                return;

            from.PublicOverheadMessage(Network.MessageType.Regular, 0, true, "*Carefully unwraps a Halloween ticket...*");
            m_Used = true;

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (from == null || this.Deleted)
                    return;

                if (ReceivePrize(from, Utility.RandomDouble()))
                    Delete();
            });
        }

        public bool ReceivePrize(Mobile from, double no)
        {
            Item i = null;
            string text = "Sorry, try another ticket!";
            var ds = DonationState.Find(from);
            double commonMod = DonationState.GetModifier(from);
            double val = 0.0;

            if (no < (val += 0.07))
            {
                if (Utility.RandomDouble() < commonMod)
                {
                    text = "You win a piece of magical armor!";
                    BaseArmor armor = Loot.RandomArmor();
                    if (armor != null)
                    {
                        bool plusOne = Utility.RandomDouble() < 0.625;

                        armor.ProtectionLevel = (ArmorProtectionLevel)(plusOne ? 2 : 1);
                        armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                    }

                    i = armor;
                }
            }
            else if (no < (val += 0.07))
            {
                if (Utility.RandomDouble() < commonMod)
                {
                    text = "You win a magical weapon!";
                    BaseWeapon wep = Loot.RandomWeapon();
                    if (wep != null)
                    {
                        bool plusOne = Utility.RandomDouble() < 0.625;

                        wep.DamageLevel = (WeaponDamageLevel)(plusOne ? 2 : 1);
                        wep.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    }

                    i = wep;
                }
            }
            else if (no < (val += 0.085))
            {

                text = "You win a black feather! Happy Halloween!";
                var feather = new BlackFeather();
                i = feather;

            }
            else if (no < (val += 0.03))
            {
                text = "You win orange candy! Happy Halloween!";
                i = new OrangeCandy();
            }
            else if (no < (val += 0.003))
            {
                text = "You win orange candy! Happy Halloween!";
                i = new OrangeCandy();
            }
            else if (no < (val += 0.005))
            {
                text = "You win a black cat statue! Happy Halloween!";
                i = new BlackCatStatue();
            }
            else if (no < (val += 0.007))
            {
                text = "You win a ghoul statue! Happy Halloween!";
                i = new GhoulStatue();
            }
            else if (no < (val += 0.001))
            {
                text = "You win a spooky tree statue! Happy Halloween!";
                i = new SpookyTreeStatue();
            }
            else if (no < (val += 0.001))
            {
                text = "You win a runebook dyetub!";
                var runebook = new RunebookDyeTub();
                i = runebook;
            }
            else if (no < (val += 0.002))
            {
                text = "You win a furniture dyetub!";
                var runebook = new FurnitureDyeTub();
                runebook.UsesRemaining = Utility.RandomMinMax(10, 20);

                var rand = Utility.RandomDouble();
                if (rand < 0.55)
                    runebook.DyedHue = 1355;
                else if (rand < 0.85)
                    runebook.DyedHue = 2114;
                else
                    runebook.DyedHue = 2019;

                runebook.Redyable = false;

                i = runebook;
            }
            else if (no < (val += 0.005))
            {
                text = "You win a licorice candy! Happy Halloween!";
                i = new LicoriceCandy();
            }
            else if (no < (val += 0.007))
            {
                text = "You win a halloween cloak! Happy Halloween!";
                i = new Cloak();
                i.Name = "a halloween cloak";
                i.Hue = 2114;
            }
            else if (no < (val += 0.004))
            {
                text = "You win a green candy! Happy Halloween!";
                i = new GreenCandy();
            }
            else if (no < (val += 0.001))
            {
                text = "You win a bag of treats! Happy Halloween!";

                double rand = Utility.RandomDouble();

                if (rand < 0.33)
                {
                    i = new BagOfTreats1();
                }
                else if (rand < 0.66)
                {
                    i = new BagOfTreats2();
                }
                else
                {
                    i = new BagOfTreats3();
                }


            }
            else if (no < (val += 0.0001))
            {
                text = "You win a ghost mask! Happy Halloween!";
                i = new GhostMask();
                i.LootType = Server.LootType.Blessed;

            }
            else if (no < (val += 0.0002))
            {
                text = "You win a black cat statue! Happy Halloween!";
                i = new BlackCatStatueRare();
            }
            else if (no < (val += 0.0001))
            {
                text = "You win a spooky old tree! Happy Halloween!";
                i = new SpookyOldTree();
            }
            else if (no < (val += 0.0003))
            {
                text = "You win Halloween Sandals! Happy Halloween!";
                i = new Sandals();
                i.Name = "halloween sandals";
                i.Hue = 2114;
            }
            else if (no < (val += 0.00001))
            {
                text = "You win dull copper tower bricks! Happy Halloween!";
                i = new DullCopperTowerPlating();
            }

            else if (no < (val += 0.04))
            {
            }
            else if (no < (val += 0.0003))
            {
                text = "You win a horn of plenty! Happy Halloween!";
                i = new Item(0x46A1);
            }
            else if (no < (val += 0.0003))
            {
                text = "You win a spooky hedge! Happy Halloween!";
                i = new SpookyHedge();
            }
            else if (no < (val += 0.0001))
            {
                text = "You win a halloween scarecrow! Happy Halloween!";
                i = new Item(0x469B);
            }
            else if (no < (val += 0.05))
            {
                i = new YellowCandy();
                text = "You win a yellow candy! Happy Halloween!";
            }
            else if (no < (val += 0.08))
            {
                i = new PumpkinBomb();
                text = "You win a pumpkin bomb! Be very careful, it's explosive! Happy Halloween!";
            }

            //Costume and Gold Drops
            if (i == null)
            {
                if (Utility.RandomDouble() <= .80)
                {
                    switch (Utility.RandomMinMax(1, 2))
                    {
                        case 1:
                            text = "You win some gold! Happy Halloween!";

                            i = new Gold(Utility.RandomMinMax(10, 50));
                        break;

                        case 2:
                            text = "You win some reagents! Happy Halloween!";

                            switch (Utility.RandomMinMax(1, 8))
                            {
                                case 1: i = new BlackPearl(Utility.RandomMinMax(10, 20)); break;
                                case 2: i = new Bloodmoss(Utility.RandomMinMax(10, 20)); break;
                                case 3: i = new MandrakeRoot(Utility.RandomMinMax(10, 20)); break;
                                case 4: i = new Ginseng(Utility.RandomMinMax(10, 20)); break;
                                case 5: i = new Garlic(Utility.RandomMinMax(10, 20)); break;
                                case 6: i = new SpidersSilk(Utility.RandomMinMax(10, 20)); break;
                                case 7: i = new Nightshade(Utility.RandomMinMax(10, 20)); break;
                                case 8: i = new SulfurousAsh(Utility.RandomMinMax(10, 20)); break;
                            }                            
                        break;
                    }                   
                }

                else
                {
                    text = "You win a Halloween costume piece! Happy Halloween!";
                    i = Helpers.GetRandomHalloweenCostumePiece();
                }
            }

            if (text.Length > 0)
                from.PublicOverheadMessage(Network.MessageType.Regular, 0, false, text); 

            if (i != null)
            {
                if (!from.AddToBackpack(i))
                {
                    i.Delete();
                    from.SendMessage("You don't have enough room in your backpack. Please make room and try again.");
                    m_Used = false;
                    return false;
                }
            }
            return true;

        }

        public HalloweenTicket(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
