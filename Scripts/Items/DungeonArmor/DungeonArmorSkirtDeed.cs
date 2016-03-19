using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonArmorSkirtDeed : Item
    {
        [Constructable]
        public DungeonArmorSkirtDeed(): base(0x14F0)
        {
            Name = "a dungeon armor skirt conversion deed";
            Hue = 2952; 
        }

        public DungeonArmorSkirtDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the dungeon armor leggings you wish to convert.");
            from.Target = new DungeonArmorChestTarget(this);
        }

        public class DungeonArmorChestTarget : Target
        {
            private DungeonArmorSkirtDeed m_DungeonArmorSkirtDeed;

            public DungeonArmorChestTarget(DungeonArmorSkirtDeed DungeonArmorSkirtDeed): base(18, false, TargetFlags.None)
            {
                m_DungeonArmorSkirtDeed = DungeonArmorSkirtDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (m_DungeonArmorSkirtDeed == null) return;
                if (m_DungeonArmorSkirtDeed.Deleted) return;
                
                if (!m_DungeonArmorSkirtDeed.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                BaseDungeonArmor dungeonArmorChest = target as BaseDungeonArmor;

                if (dungeonArmorChest == null)
                {
                    player.SendMessage("That is not dungeon armor leggings.");
                    return;
                }

                if (dungeonArmorChest.Layer != Layer.Pants)
                {
                    player.SendMessage("That is not dungeon armor leggings.");
                    return;
                }

                if (!dungeonArmorChest.IsChildOf(player.Backpack))
                {
                    from.SendMessage("The target item must be in your pack.");
                    return;
                }

                dungeonArmorChest.ItemID = 0x1c08;

                player.SendSound(0x1c08);

                from.SendMessage("You change the appearance of your dungeon armor leggings.");
                
                m_DungeonArmorSkirtDeed.Delete();  
            }                    
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