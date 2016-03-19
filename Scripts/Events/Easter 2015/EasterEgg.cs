using Server.Multis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;
using Server.Misc;
using Server.Network;

namespace Server.Items
{
    class EasterEgg : Item
    {
        [Constructable]
        public EasterEgg(): base(0x9B5)
        {
            Name = "easter eggs";

            Weight = 1;
            Hue = Utility.RandomMinMax(2, 362);
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(this.GetWorldLocation(), 1))
            {
                Effects.PlaySound(from.Location, from.Map, Utility.RandomList(0x38D, 0x38E, 0x38F, 0x390));

                from.PublicOverheadMessage(Network.MessageType.Regular, 0, true, "*breaks open an easter egg*");                

                Timer.DelayCall(TimeSpan.FromSeconds(2), delegate
                {
                    if (from == null)
                        return;

                    ReceivePrize(from, Utility.RandomMinMax(1, 10000));
                });

                Delete();
            }

            else
                from.SendMessage("You are too far away to use that.");
        }

        public bool ReceivePrize(Mobile from, int seed)
        {
            Item i = null;
            string text = "";

            /*              Rewards
                * Sandals ~ Hued [2651, 2662]  :::: 0.5%
                * Spellbook ~ Hued [2651, 2662]  :::: 0.5%
                * UncutCloth ~ Hued [2651, 2662]  :::: 1%
                * Stuffed Easter Bunny Statue Hued [1150] :::: 1.5%
                * Dragon Coin ~ Amount [10-25] :::: 3%
                * Magic Weapon (Might + random others) 5%
                * RabbitBomb ~ 5%
                * Random Skill Manual ~ 5%
                * Another Easter Egg 10%
                * 
                * Explosion ~ (5-15 damage) :::: 25.5%
                * Gold ~ Amount [50-199] :::: 43%
                * 
            */

            if (seed <= 25)
            {
                text = "You receive Easter Sandals!";

                i = new Sandals();
                i.Hue = Utility.RandomList(2651, 2662);
                i.Name = "Easter Sandals";
            }

            else if (seed <= 75)
            {
                text = "You receive an Easter Spellbook!";

                i = new Spellbook();
                i.Hue = Utility.RandomList(2651, 2662);
                i.Name = "Easter Spellbook";
            }

            else if (seed <= 125)
            {
                text = "You receive some Easter Cloth!";

                i = new UncutCloth();
                i.Hue = Utility.RandomList(2651, 2662);
                i.Amount = 8;
            }

            else if (seed <= 225)
            {
                text = "You receive a stuffed Easter Bunny!";
                i = new StuffedEasterBunny();
            }

            else if (seed <= 600)
            {
                text = "You receive some Dragon Coins!";
                i = new DragonCoin();
                i.Amount = Utility.Random(25, 50);
            }

            else if (seed <= 850)
            {
                text = "You receive a magical weapon!";
                
                BaseWeapon wep = Loot.RandomWeapon();

                if (wep != null)
                {
                    wep.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(1, 5);
                    wep.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(1, 5);
                    wep.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(1, 5);
                }

                i = wep;
            }

            else if (seed <= 1650)
            {
                text = "You receive a rabbit bomb!";
                i = new RabbitBomb();
            }

            else if (seed <= 3150)
            {
                text = "You receive more easter eggs!";
                i = new EasterEgg();
            }

            else if (seed <= 5750)
            {
                Point3D loc = this.GetWorldLocation();
                Map facet = this.Map;
                
                text = "You receive remorse!";                

                if (from.CanBeDamaged())
                {
                    Effects.SendLocationEffect(loc, facet, 0x36BD, 15, 10);
                    Effects.PlaySound(from.Location, from.Map, 0x246);

                    from.PublicOverheadMessage(MessageType.Regular, 0, false, "*Looks remorseful*");

                    AOS.Damage(from, Utility.Random(10, 20), 0, 100, 0, 0, 0);

                    Blood blood = new Blood();
                    blood.MoveToWorld(from.Location, from.Map);
                }
            }

            else if (seed <= 6000)
            {
                text = "You receive a large amount of gold.";
                i = new Gold();
                i.Amount = Utility.RandomMinMax(750, 1250);

                from.SendSound(0x2e6);
            }

            else if (seed <= 6500)
            {
                text = "You receive a moderate amount of gold.";
                i = new Gold();
                i.Amount = Utility.RandomMinMax(400, 600);

                from.SendSound(0x2e5);
            }

            else
            {
                text = "You receive a small amount of gold.";
                i = new Gold();
                i.Amount = Utility.RandomMinMax(200, 300);

                from.SendSound(0x2e4);
            }

            if (text.Length > 0)
                from.SendMessage(text);

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

        public EasterEgg(Serial serial): base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);         
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            if (version == 0)
            {
                reader.ReadBool();
            }
        }
    }
}
