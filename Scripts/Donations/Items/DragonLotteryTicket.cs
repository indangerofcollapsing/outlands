
using Server.Items.Misc;
using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class DragonLotteryTicket : Item
    {

        private bool m_Used = false;

        [Constructable]
        public DragonLotteryTicket()
            : base(0x14ED)
        {
            LootType = Server.LootType.Blessed;
            Name = "Dragon Lottery Ticket";
            DonationItem = true;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Use to receive a special prize!");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Used)
                return;

            from.PublicOverheadMessage(Network.MessageType.Regular, 0, true, "*Opens a lottery ticket*");
            m_Used = true;

            Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
            {
                if (from == null || this.Deleted)
                    return;

                if (ReceivePrize(from, Utility.RandomMinMax(1, 1000)))
                    Delete();
                else
                    m_Used = false;
            });
        }

        public bool ReceivePrize(Mobile from, int seed)
        {
            Item i = null;
            string text = "";

            if (seed <= 5)
            {
                text = "You win a brown bear rug!";
                i = new BrownBearRugSouthDeed();
            }
            else if (seed < 10)
            {
                text = "You win a brown bear rug!";
                i = new BrownBearRugEastDeed();
            }
            else if (seed < 25)
            {
                text = "You win pet dye!";
                i = new PetDye();
                i.Hue = Utility.RandomList(2419, 2418, 2413, 2406, 2215, 2425, 2219, 2207);
            }
            else if (seed < 30)
            {
                text = "You win a Monster Statuette!";
                i = new MonsterStatuette(MonsterStatuetteType.Dragon);
            }
            else if (seed < 35)
            {
                text = "You win a rare artifact!";
                i = new FullJars3();
            }
            else if (seed < 40)
            {
                text = "You win a rare artifact!";
                i = new TarotCardsArtifact();
            }
            else if (seed < 45)
            {
                text = "You win a rare artifact!";
                i = new PottedTree();
            }
            else if (seed < 55)
            {
                text = "You win a pouch with many pockets!";
                i = new PouchWithManyPockets();
            }
            else if (seed < 60)
            {
                text = "You win an item rename deed!";
                i = new ItemRenameDeed();
            }
            else if (seed < 65)
            {
                text = "You win a magical weapon!";
                BaseWeapon wep = Loot.RandomWeapon();
                if (wep != null)
                {
                    wep.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                    wep.DamageLevel = WeaponDamageLevel.Might;
                    wep.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                }

                i = wep;
            }
            else if (seed < 70)
            {
                text = "You win Town Sandals!";
                i = new Sandals() { Hue = Utility.RandomList(TownCloth.Hues) };
            }
            else if (seed < 75)
            {
                text = "You win a rare artifact!";
                i = new LongFlask();
            }
            else if (seed < 80)
            {
                text = "You win a rare artifact!";
                i = new FullJars2();
            }
            else if (seed < 85)
            {
                text = "You win a rare title dye!";
                i = TitleDye.RandomRareTitleDye();
            }
            else if (seed < 95)
            {
                text = "You win an uncommon title dye!";
                i = TitleDye.RandomUncommonTitleDye();
            }
            else if (seed < 100)
            {
                text = "You win another Dragon Ticket!";
                i = new DragonLotteryTicket();
            }
            else if (seed < 105)
            {
                text = "You win some rare cloth!";
                i = new RareCloth();
            }
            else
            {
                text = "Sorry! Try another ticket.";
            }


            if (text.Length > 0)
                from.PublicOverheadMessage(Network.MessageType.Regular, 0, false, text);

            if (i != null)
            {
                if (!from.AddToBackpack(i))
                {
                    i.Delete();
                    from.SendMessage("You don't have enough room in your backpack. Please make room and try again.");
                    return false;
                }
            }

            return true;
        }

        public DragonLotteryTicket(Serial serial)
            : base(serial)
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
