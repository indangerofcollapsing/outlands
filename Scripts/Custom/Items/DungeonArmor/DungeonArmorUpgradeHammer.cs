using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class DungeonArmorUpgradeHammer : Item
    {
        [Constructable]
        public DungeonArmorUpgradeHammer() : base(4020)
        {
            Name = "a dungeon armor upgrade hammer";
            Hue = 2568; 
        }

        public DungeonArmorUpgradeHammer(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "charges remaining: 1");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the dungeon armor or dungeon shield you wish to upgrade.");
            from.Target = new DungeonArmorUpgradeTarget(this);
        }

        public class DungeonArmorUpgradeTarget : Target
        {
            private DungeonArmorUpgradeHammer m_DungeonArmorUpgradeHammer;

            public DungeonArmorUpgradeTarget(DungeonArmorUpgradeHammer DungeonArmorUpgradeHammer): base(2, false, TargetFlags.None)
            {
                m_DungeonArmorUpgradeHammer = DungeonArmorUpgradeHammer;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_DungeonArmorUpgradeHammer.Deleted || m_DungeonArmorUpgradeHammer.RootParent != from)     
                if (from == null) return;

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target dungeon armor or a dungeon shield in your backpack.");
                        return;
                    }
                }
                
                if (target is BaseDungeonArmor)
                {
                    BaseDungeonArmor armor = target as BaseDungeonArmor;

                    if (armor.Tier != BaseDungeonArmor.ArmorTierEnum.Tier4)
                    {
                        armor.Tier++;
                        armor.MaxBlessedCharges--;

                        from.PlaySound(0X64e);
                        Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, armor.Hue - 1, 0, 5029, 0);

                        from.SendMessage("You upgrade the dungeon armor's tier. Its maximum number of blessed charges has been reduced by one.");
                        m_DungeonArmorUpgradeHammer.Delete();                        
                    }

                    else
                    {
                        from.SendMessage("That dungeon armor cannot be upgraded any further.");
                        return;
                    }
                }

                else if (target is BaseDungeonShield)
                {
                    BaseDungeonShield shield = target as BaseDungeonShield;

                    if (shield.Tier != BaseDungeonArmor.ArmorTierEnum.Tier4)
                    {
                        shield.Tier++;
                        shield.MaxBlessedCharges--;

                        from.PlaySound(0X64e);
                        Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, shield.Hue - 1, 0, 5029, 0);

                        from.SendMessage("You upgrade the dungeon shield's tier. It's maximum number of blessed charges has been reduced by one.");
                        m_DungeonArmorUpgradeHammer.Delete();
                    }

                    else
                    {
                        from.SendMessage("That dungeon shield cannot be upgraded any further.");
                        return;
                    }
                }

                else
                {
                    from.SendMessage("That is neither dungeon armor or a dungeon shield.");
                    return;
                }
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

            //Version 0
            if (version >= 0)
            {               
            }
        }
    }
}