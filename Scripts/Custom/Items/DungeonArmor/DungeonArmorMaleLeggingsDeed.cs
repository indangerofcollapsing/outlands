using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonArmorMaleLeggingsDeed : Item
    {
        [Constructable]
        public DungeonArmorMaleLeggingsDeed(): base(0x14F0)
        {
            Name = "a dungeon armor male leggings conversion deed";
            Hue = 2952; 
        }

        public DungeonArmorMaleLeggingsDeed(Serial serial): base(serial)
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

            from.SendMessage("Target the dungeon armor leggings you wish to convert back to a male version.");
            from.Target = new DungeonArmorLeggingsTarget(this);
        }

        public class DungeonArmorLeggingsTarget : Target
        {
            private DungeonArmorMaleLeggingsDeed m_DungeonArmorMaleLeggingsDeed;

            public DungeonArmorLeggingsTarget(DungeonArmorMaleLeggingsDeed DungeonArmorMaleLeggingsDeed)
                : base(18, false, TargetFlags.None)
            {
                m_DungeonArmorMaleLeggingsDeed = DungeonArmorMaleLeggingsDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (m_DungeonArmorMaleLeggingsDeed == null) return;
                if (m_DungeonArmorMaleLeggingsDeed.Deleted) return;
                
                if (!m_DungeonArmorMaleLeggingsDeed.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                BaseDungeonArmor dungeonArmorChest = target as BaseDungeonArmor;

                if (dungeonArmorChest == null)
                {
                    player.SendMessage("That is not a dungeon armor leggings.");
                    return;
                }

                if (dungeonArmorChest.Layer != Layer.Pants)
                {
                    player.SendMessage("That is not a dungeon armor leggings.");
                    return;
                }

                if (!dungeonArmorChest.IsChildOf(player.Backpack))
                {
                    from.SendMessage("The target item must be in your pack.");
                    return;
                }

                dungeonArmorChest.ItemID = 0x1415;

                player.SendSound(0x64E);

                from.SendMessage("You change the appearance of your dungeon armor leggings.");
                
                m_DungeonArmorMaleLeggingsDeed.Delete();  
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