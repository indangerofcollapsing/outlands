using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonArmorMaleChestDeed : Item
    {
        [Constructable]
        public DungeonArmorMaleChestDeed(): base(0x14F0)
        {
            Name = "a dungeon armor male chest conversion deed";
            Hue = 2952; 
        }

        public DungeonArmorMaleChestDeed(Serial serial): base(serial)
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

            from.SendMessage("Target the dungeon armor chest piece you wish to convert back to a male version.");
            from.Target = new DungeonArmorChestTarget(this);
        }

        public class DungeonArmorChestTarget : Target
        {
            private DungeonArmorMaleChestDeed m_DungeonArmorMaleChestDeed;

            public DungeonArmorChestTarget(DungeonArmorMaleChestDeed DungeonArmorMaleChestDeed): base(18, false, TargetFlags.None)
            {
                m_DungeonArmorMaleChestDeed = DungeonArmorMaleChestDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (m_DungeonArmorMaleChestDeed == null) return;
                if (m_DungeonArmorMaleChestDeed.Deleted) return;
                
                if (!m_DungeonArmorMaleChestDeed.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                BaseDungeonArmor dungeonArmorChest = target as BaseDungeonArmor;

                if (dungeonArmorChest == null)
                {
                    player.SendMessage("That is not a dungeon armor chest piece.");
                    return;
                }

                if (dungeonArmorChest.Layer != Layer.InnerTorso)
                {
                    player.SendMessage("That is not a dungeon armor chest piece.");
                    return;
                }

                if (!dungeonArmorChest.IsChildOf(player.Backpack))
                {
                    from.SendMessage("The target item must be in your pack.");
                    return;
                }

                dungeonArmorChest.ItemID = 0x1415;

                player.SendSound(0x64E);

                from.SendMessage("You change the appearance of your dungeon armor chest piece.");
                
                m_DungeonArmorMaleChestDeed.Delete();  
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