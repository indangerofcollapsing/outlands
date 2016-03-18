using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Commands;

namespace Server.Custom.Townsystem
{
    public class Crown : BaseHat
    {
        public static void Initialize()
        {
            CommandSystem.Register("DistributeKingCrowns", AccessLevel.Administrator, DistributeCrowns);
        }

        [Usage("DonationShopLog")]
        [Description("Views the donation shop log for IPY.")]
        public static void DistributeCrowns(CommandEventArgs e)
        {
            while (m_Instances.Count > 0)
                m_Instances[0].Delete();

            foreach (Town town in Town.Towns)
                if (town.King != null)
                    town.King.Backpack.DropItem(new Crown(town));
        }

        public override bool Nontransferable { get { return true; } }

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 5; } }
        public override int BaseColdResistance { get { return 9; } }
        public override int BasePoisonResistance { get { return 5; } }
        public override int BaseEnergyResistance { get { return 5; } }

        public override int InitMinHits { get { return 20; } }
        public override int InitMaxHits { get { return 30; } }

        public override bool Dye(Mobile from, DyeTub sender) { return false; }

        private Town m_Town;

        [CommandProperty(AccessLevel.Administrator)]
        public Town Town 
        { 
            get { return m_Town; } 
            set { 
                m_Town = value; 
                Name = String.Format("{0}King's Crown", m_Town == null ? "" : (m_Town.Definition.FriendlyName + " "));
            } 
        }

        private static List<Crown> m_Instances = new List<Crown>();

        public static List<Crown> Instances { get { return m_Instances; } }


        [Constructable]
        public Crown()
            : this(null, 0x28B, 0x501)
        {
        }

        [Constructable]
        public Crown(Town town)
            : this(town, 0x28B, 0x501)
        {
        }

        public Crown(Town town, int id, int hue)
            : base(id, hue)
        {
            LootType = Server.LootType.Blessed;
            Name = "king's crown";
            Weight = 1.0;
            Town = town;
            m_Instances.Add(this);
        }

        public override void OnAfterDelete()
        {
            m_Instances.Remove(this);

            base.OnAfterDelete();
        }

        public Crown(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            Town.WriteReference(writer, Town);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version > 0)
                m_Town = Town.ReadReference(reader);

            m_Instances.Add(this);
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You decide against throwing away your King's Crown.");
                return false;
            }

            return base.DropToWorld(from, p);
        }

        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && from != target)
            {
                from.SendMessage("You decide against giving away your King's Crown.");
                return false;
            }

            return base.DropToMobile(from, target, p);
        }

        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            if (from.AccessLevel == AccessLevel.Player && target != from.BankBox && target != from.Backpack)
            {
                from.SendMessage("The King's Crown must remain in either your bankbox or top level of your backpack.");
                return false;
            }
            
            return base.DropToItem(from, target, p);
        }

        public override bool OnEquip(Mobile from)
        {
            if (Town != null && !Town.IsKing(from))
            {
                Delete();
                from.SendMessage("The crown vanishes as you attempt to equip it.");
                return true;
            }

            return base.OnEquip(from);
        }
    }
}
