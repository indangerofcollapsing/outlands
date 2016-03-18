using System;
using Server.Custom.Townsystem;

namespace Server.Items
{
    public class FlimsyBaronsRing : GoldRing
    {
        public DateTime CreationTime { get; set; }
        public override string DefaultName { get { return "Flimsy Baron's Ring"; } }

        [Constructable]
        public FlimsyBaronsRing()
        {
            LootType = Server.LootType.Newbied;
            CreationTime = DateTime.Now;
        }

        public FlimsyBaronsRing(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(CreationTime);

            if (CreationTime + TimeSpan.FromDays(7) < DateTime.Now)
                Timer.DelayCall(TimeSpan.FromTicks(1), Delete);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            CreationTime = reader.ReadDateTime();
        }

        public override bool Nontransferable { get { return true; } }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player) {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }
            return base.OnDroppedToWorld(from, p);
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && from != target)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && target != from.Backpack && target != from.BankBox)
            {
                from.SendMessage("Militia items are non-transferrable");
                return false;
            }

            return base.DropToItem(from, target, p);
        }
    }
}