using Server.Engines.Craft;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    class DungeonArmorRepairDeed : Item
    {
        public override string DefaultName
        {
            get
            {
                return "a dungeon armor repair deed";
            }
        }

        [Constructable]
        public DungeonArmorRepairDeed()
            : base(0x14F0)
        {
            Hue = 2410;
        }

        public DungeonArmorRepairDeed(Serial serial)
            : base(serial)
        {

        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Check(from))
            {
                from.Target = new InternalTarget(from, this);
                from.SendLocalizedMessage(1044276); // Target an item to repair.
            }
        }

        private class InternalTarget : Target
        {
            private Mobile m_From;
            private DungeonArmorRepairDeed m_Deed;

            public InternalTarget(Mobile from, DungeonArmorRepairDeed deed)
                : base(2, false, TargetFlags.None)
            {
                m_From = from;
                m_Deed = deed;
            }

            private bool CheckDeed(Mobile from)
            {
                if (m_Deed != null)
                {
                    return m_Deed.Check(from);
                }

                return true;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int number = 1044277;

                if (!CheckDeed(from))
                    return;

                bool toDelete = false;

                if (targeted is BaseDungeonArmor)
                {
                    BaseArmor armor = (BaseArmor)targeted;

                    if (0.75 > Utility.RandomDouble()) // 75% chance to succeed
                    {
                        number = 1044279; // You repair the item.
                        DefBlacksmithy.CraftSystem.PlayCraftEffect(from);
                        armor.HitPoints = armor.MaxHitPoints;
                    }
                    else
                    {
                        number = 1061137; // You fail to repair the item. [And the contract is destroyed]
                        DefBlacksmithy.CraftSystem.PlayCraftEffect(from);
                    }

                    toDelete = true;

                }


                from.SendLocalizedMessage(number);

                if (toDelete)
                    m_Deed.Delete();
            }
        }

        public bool Check(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                from.SendLocalizedMessage(1047012); // The contract must be in your backpack to use it.
            else if (!VerifyRegion(from))
                TextDefinition.SendMessageTo(from, 1047013);
            else
                return true;

            return false;
        }

        public bool VerifyRegion(Mobile m)
        {
            if (!m.Region.IsPartOf(typeof(TownRegion)))
                return false;

            return Server.Factions.Faction.IsNearType(m, typeof(Blacksmith), 6);
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

            switch (version)
            {
                case 0:
                    {
                        break;
                    }
            }
        }
    }
}
