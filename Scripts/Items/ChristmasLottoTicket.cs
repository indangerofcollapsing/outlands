using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;

namespace Server.Custom
{
    public class ChristmasLottoTicket : Item
    {
        private bool m_Used = false;

        public override string  DefaultName { get  {  return "Christmas Lottery Ticket"; } }

        [Constructable]
        public ChristmasLottoTicket()
            : base(0x14ED)
        {
            LootType = Server.LootType.Blessed;
            DonationItem = false;
            Hue = 133;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Use to receive a special prize!");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Used)
                return;

            from.PublicOverheadMessage(Network.MessageType.Regular, 0, true, "*Carefully unwraps a lottery ticket...*");
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

            if (no < (val += 0.07)) {
                if (Utility.RandomDouble() < commonMod) {
                    text = "You win a piece of magical armor!";
                    BaseArmor armor = Loot.RandomArmor();
                    if (armor != null) {
                        bool plusOne = Utility.RandomDouble() < 0.625;

                        armor.ProtectionLevel = (ArmorProtectionLevel)(plusOne ? 2 : 1);
                        armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                    }

                    i = armor;
                }
            } else if (no < (val += 0.07)) {
                if (Utility.RandomDouble() < commonMod) {
                    text = "You win a magical weapon!";
                    BaseWeapon wep = Loot.RandomWeapon();
                    if (wep != null) {
                        bool plusOne = Utility.RandomDouble() < 0.625;

                        wep.DamageLevel = (WeaponDamageLevel)(plusOne ? 2 : 1);
                        wep.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    }

                    i = wep;
                }
            } else if (no < (val += 0.05))
            {
                text = "You win a glass of milk for Santa!!";
                i = new MilkForSanta();
            //} else if (no < (val += 0.002)) {
             //   text = "You win festive boat paint!";
              //  i = new ChristmasBoatPaint();
            } else if (no < (val += 0.005)) {
                text = "You win a gargoyle statue!";
                i = new MonsterStatuette(MonsterStatuetteType.Gargoyle);
            } else if (no < (val += 0.007)) {
                text = "You win a troll statue!";
                i = new MonsterStatuette(MonsterStatuetteType.Troll);
            } else if (no < (val += 0.001)) {
                text = "You win a gorilla statue!";
                i = new MonsterStatuette(MonsterStatuetteType.Gorilla);
            } else if (no < (val += 0.001)) {
                text = "You win a runebook dyetub!";
                var runebook = new RunebookDyeTub();
                i = runebook;
            } else if (no < (val += 0.001)) {
                text = "You win a furniture dyetub!";
                var runebook = new FurnitureDyeTub();
                runebook.UsesRemaining = Utility.RandomMinMax(20, 30);

                var rand = Utility.RandomDouble();
                if (rand < 0.55)
                    runebook.DyedHue = 133;
                else if (rand < 0.85)
                    runebook.DyedHue = 2117;
                else
                    runebook.DyedHue = 2118;

                runebook.Redyable = false;

                i = runebook;
            } else if (no < (val += 0.05)) {
                text = "You win reindeer fruit!!";
                i = new ReindeerFruit();
            } else if (no < (val += 0.001)) {
                text = "You win Santa's Cloak!";
                i = new Cloak();
                i.Name = "santa's cloak";
                i.Hue = 2118;
            } else if (no < (val += 0.03)) {
                text = "Uh oh! Coal!";
                i = new Coal();
            } else if (no < (val += 0.001)) {
                text = "You win a Christmas decoration!";

                double rand = Utility.RandomDouble();

                if (rand < 0.33) {
                    i = new Item(0x17CE); //snow
                } else if (rand < 0.66) {
                    i = new Item(0x46FB); //angel
                } else {
                    i = new Item(0x46FA); //angel
                }
            
            } else if (no < (val += 0.0007)) {
                text = "You win a reindeer mask!";
                i = new ReindeerMask();
                i.LootType = Server.LootType.Blessed;
            } else if (no < (val += 0.0001)) {
                text = "You win polar bear mask!";
                i = new PolarMask();
                i.LootType = Server.LootType.Blessed;

            } else if (no < (val += 0.001)) {
                text = "You win a ranger gorget!";
                i = new RangerGorget();
            } else if (no < (val += 0.001)) {
                text = "You win ranger arms!";
                i = new RangerArms();
            } else if (no < (val += 0.0005)) {
                text = "You win ranger legs!";
                i = new RangerLegs();
            } else if (no < (val += 0.0005)) {
                text = "You win a ranger chest!";
                i = new RangerChest();
            } else if (no < (val += 0.001)) {
                text = "You win ranger gloves!";
                i = new RangerGloves();

            } else if (no < (val += 0.0002)) {
                text = "You win a gingerbread cookie!";
                i = new GingerBreadCookie();
                i.LootType = Server.LootType.Blessed;
            } else if (no < (val += 0.0001)) {
                text = "You win a North Pole Mini House Deed!";
                i = new MiniHouseDeed(MiniHouseType.ChurchAtNight);
            } else if (no < (val += 0.0004)) {
                text = "You win a dollhouse!";
                i = new MiniHouseDeed(MiniHouseType.StoneAndPlaster);
		i.Hue = 2595;
            } else if (no < (val += 0.0002)) {
                text = "You win a dollhouse!";
                i = new MiniHouseDeed(MiniHouseType.TwoStoryStoneAndPlaster);
		        i.Hue = 2595;
           // } else if (no < (val += 0.00003)) {
           //     text = "You win a house deed! It's SANTA'S WORKSHOP!!";
             //   i = new SantasWorkshop();
            } 
            else if (no < (val += 0.04)) 
            {               

            } else if (no < (val += 0.00005)) {
                text = "You win a snow tile!";
                i = new Item(0x17BD);
            } else if (no < (val += 0.005)) {
                i = new SnowPile();
                text = "You win a pile of snow!";
            } else if (no < (val += 0.05)) {
                i = new ReindeerCarrot();
                text = "You win a reindeer carrot!";
            } else if (no < (val += 0.004)) {
                i = new FireworksWand(Utility.RandomMinMax(10, 100), 185);
                text = "You win a blue fireworks wand!";
            } else if (no < (val += 0.005)) {
                i = new FireworksWand(Utility.RandomMinMax(10, 100), 73);
                text = "You win a green fireworks wand!";
            } else if (no < (val += 0.005)) {
                i = new FireworksWand(Utility.RandomMinMax(10, 100), 1150);
                text = "You win a white fireworks wand!";
            } else if (no < (val += 0.005)) {
                i = new FireworksWand(Utility.RandomMinMax(10, 100), 32);
                text = "You win a red fireworks wand!";
            } else if (no < (val += 0.0003)) {
                i = new ChristmasPack(ChristmasPack.ChristmasPackType.Green);
                text = "You win a green Christmas pack!";
            } else if (no < (val += 0.0002)) {
                i = new ChristmasPack(ChristmasPack.ChristmasPackType.Red);
                text = "You win a red Christmas pack!";
            } else if (no < (val += 0.0001)) {
                i = new ChristmasPack(ChristmasPack.ChristmasPackType.White);
                text = "You win a white Christmas pack!";
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

        public ChristmasLottoTicket(Serial serial)
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
