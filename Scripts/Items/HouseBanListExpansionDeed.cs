using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Multis;

namespace Server.Items
{
    public class HouseBanListExpansionDeed : Item
    {
        public static int BanLimitIncreaseAmount = 25;

        [Constructable]
        public HouseBanListExpansionDeed(): base(0x14F0)
        {
            Name = "a housing ban list expansion deed";
            Hue = 2635;
        }

        public HouseBanListExpansionDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "(" + BanLimitIncreaseAmount.ToString() + " bans)");            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            BaseHouse house = BaseHouse.FindHouseAt(from.Location, from.Map, 64);

            if (house == null)
            {
                from.SendMessage("You must be inside a house in order to use this deed.");
                return;
            }

            if (house.Owner != from)
            {
                from.SendMessage("Only the owner of this house may use this deed.");
                return;
            }

            house.MaxBans += BanLimitIncreaseAmount;

            from.SendMessage("You increase the maximum number of bans available to the house by " + BanLimitIncreaseAmount.ToString() + ".");

            from.PlaySound(0x0F7);
            from.FixedParticles(0x373A, 10, 15, 5012, 2587, 0, EffectLayer.Waist);

            Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}