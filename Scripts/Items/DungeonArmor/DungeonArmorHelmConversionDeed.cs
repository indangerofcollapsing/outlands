using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonArmorHelmConversionDeed : Item
    {
        [Constructable]
        public DungeonArmorHelmConversionDeed(): base(0x14F0)
        {
            Name = "a dungeon armor helm conversion deed";
            Hue = 2952; 
        }

        public DungeonArmorHelmConversionDeed(Serial serial): base(serial)
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

            from.SendMessage("Target the dungeon armor helm you wish to convert.");
            from.Target = new DungeonArmorHelmTarget(this);
        }

        public class DungeonArmorHelmTarget : Target
        {
            private DungeonArmorHelmConversionDeed m_DungeonArmorHelmConversionDeed;

            public DungeonArmorHelmTarget(DungeonArmorHelmConversionDeed dungeonArmorHelmConversionDeed): base(18, false, TargetFlags.None)
            {
                m_DungeonArmorHelmConversionDeed = dungeonArmorHelmConversionDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (m_DungeonArmorHelmConversionDeed == null) return;
                if (m_DungeonArmorHelmConversionDeed.Deleted) return;
                
                if (!m_DungeonArmorHelmConversionDeed.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                BaseDungeonArmor dungeonArmorHelm = target as BaseDungeonArmor;

                if (dungeonArmorHelm == null)
                {
                    player.SendMessage("That is not a dungeon armor helm.");
                    return;
                }

                if (dungeonArmorHelm.Layer != Layer.Helm)
                {
                    player.SendMessage("That is not a dungeon armor helm.");
                    return;
                }

                if (!dungeonArmorHelm.IsChildOf(player.Backpack))
                {
                    from.SendMessage("The target item must be in your pack.");
                    return;
                }

                from.SendMessage("Target a hat, helm, or mask you wish this dungeon armor to resemble. This will delete the targeted item.");
                from.Target = new NormalHelmTarget(m_DungeonArmorHelmConversionDeed, dungeonArmorHelm);
            }                    
        }

        public class NormalHelmTarget : Target
        {
            private DungeonArmorHelmConversionDeed m_DungeonArmorHelmConversionDeed;
            private BaseDungeonArmor m_DungeonArmorHelmet;

            public NormalHelmTarget(DungeonArmorHelmConversionDeed dungeonArmorHelmConversionDeed, BaseDungeonArmor dungeonArmorHelmet): base(18, false, TargetFlags.None)
            {
                m_DungeonArmorHelmConversionDeed = dungeonArmorHelmConversionDeed;
                m_DungeonArmorHelmet = dungeonArmorHelmet;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                if (m_DungeonArmorHelmConversionDeed == null) return;
                if (m_DungeonArmorHelmConversionDeed.Deleted) return;
                if (m_DungeonArmorHelmet == null) return;
                if (m_DungeonArmorHelmet.Deleted) return;

                if (!m_DungeonArmorHelmConversionDeed.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }

                if (!m_DungeonArmorHelmet.IsChildOf(player.Backpack))
                {
                    from.SendMessage("That item must be in your pack in order to use it.");
                    return;
                }
                
                Item targetHelm = target as Item;

                if (targetHelm == null)
                {
                    from.SendMessage("That is not a valid target");
                    return;
                }

                if (!targetHelm.IsChildOf(player.Backpack))
                {
                    from.SendMessage("The targeted helm, hat, or mask must be in your pack.");
                    return;
                }

                if (targetHelm.Layer != Layer.Helm)
                {
                    from.SendMessage("That is not a helm, hat, or mask.");
                    return;
                }

                m_DungeonArmorHelmet.ItemID = targetHelm.ItemID;

                player.SendSound(0x64E);

                from.SendMessage("You change the appearance of your dungeon armor helm.");

                targetHelm.Delete();
                m_DungeonArmorHelmConversionDeed.Delete();                
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