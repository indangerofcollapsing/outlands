using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Townsystem.Items
{
    class ArmoredMilitiaWearable : BaseArmoredHat
    {
        public override string DefaultName { get { return "an armored militia hat"; } }

        [Constructable]
        public ArmoredMilitiaWearable()
            : base(1)
        {
            LootType = Server.LootType.Newbied;
        }

        public ArmoredMilitiaWearable(Serial serial)
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

    class MilitiaWearable : Item
    {
        public override string DefaultName { get { return "a militia wearable"; } }        

        public new bool Scissor(Mobile from, Scissors scissors)
        {
            from.SendLocalizedMessage(502440); // Scissors can not be used on that to produce anything.
            return false;
        }

        public override bool CanEquip(Mobile from)
        {
            var pm = from as PlayerMobile;

            return pm != null && pm.IsInMilitia && base.CanEquip(from);
        }

        [Constructable]
        public MilitiaWearable()
            : base(0x1F03)
        {
            LootType = Server.LootType.Newbied;
        }

        public MilitiaWearable(Serial serial)
            : base(serial)
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


        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player)
            {
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

    class MilitiaRobe : MilitiaWearable
    {
        public MilitiaRobe()
            : base()
        {
            ItemID = 7939;
            Layer = Server.Layer.OuterTorso;
            Name = "a militia robe";
        }

        public MilitiaRobe(Serial serial) : base(serial) { }

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

    class MilitiaCloak : MilitiaWearable
    {
        public MilitiaCloak()
            : base()
        {
            ItemID = 5397;
            Layer = Server.Layer.Cloak;
            Name = "a militia cloak";
        }

        public MilitiaCloak(Serial serial) : base(serial) { }

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

    class MilitiaSkirt : MilitiaWearable
    {
        public MilitiaSkirt()
            : base()
        {
            ItemID = 5398;
            Layer = Server.Layer.OuterLegs;
            Name = "a militia skirt";
        }

        public MilitiaSkirt(Serial serial) : base(serial) { }

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

    class MilitiaDoublet : MilitiaWearable
    {
        public MilitiaDoublet()
            : base()
        {
            ItemID = 8059;
            Layer = Server.Layer.MiddleTorso;
            Name = "a militia doublet";
        }

        public MilitiaDoublet(Serial serial) : base(serial) { }

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

    class MilitiaHeadband : ArmoredMilitiaWearable
    {
        public MilitiaHeadband()
            : base()
        {
            ItemID = 5440;
            Layer = Server.Layer.Helm;
            Name = "a militia headband";
        }

        public MilitiaHeadband(Serial serial) : base(serial) { }

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

    class MilitiaSash : MilitiaWearable
    {
        public MilitiaSash()
            : base()
        {
            ItemID = 5441;
            Layer = Server.Layer.MiddleTorso;
            Name = "a militia sash";
        }

        public MilitiaSash(Serial serial) : base(serial) { }

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

    class MilitiaHalfApron : MilitiaWearable
    {
        public MilitiaHalfApron()
            : base()
        {
            ItemID = 5435;
            Layer = Server.Layer.Waist;
            Name = "a militia half apron";
        }

        public MilitiaHalfApron(Serial serial) : base(serial) { }

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

    class MilitiaKilt : MilitiaWearable
    {
        public MilitiaKilt()
            : base()
        {
            ItemID = 5431;
            Layer = Server.Layer.OuterLegs;
            Name = "a militia kilt";
        }

        public MilitiaKilt(Serial serial) : base(serial) { }

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

    class MilitiaWizardHat : ArmoredMilitiaWearable
    {
        public MilitiaWizardHat()
            : base()
        {
            ItemID = 5912;
            Layer = Server.Layer.Helm;
            Name = "a militia wizard's hat";
        }

        public MilitiaWizardHat(Serial serial) : base(serial) { }

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

    class MilitiaTunic : MilitiaWearable
    {
        public MilitiaTunic()
            : base()
        {
            ItemID = 8097;
            Layer = Server.Layer.MiddleTorso;
            Name = "a militia tunic";
        }

        public MilitiaTunic(Serial serial) : base(serial) { }

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

    class MilitiaLantern : Lantern
    {
        public MilitiaLantern()
            : base()
        {
            Layer = Server.Layer.Earrings;
            LootType = Server.LootType.Blessed;
            Name = "a militia lantern";
        }

        public MilitiaLantern(Serial serial) : base(serial) { }

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
