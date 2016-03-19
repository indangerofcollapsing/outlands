using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;

namespace Server.Items
{
    public class WoodHuePlating : Item
    {
        private CraftResource m_ResourceType = CraftResource.RegularWood; 
        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource ResourceType
        {
            get { return m_ResourceType; }
            set
            {
                m_ResourceType = value;

                CraftResourceInfo resourceInfo = CraftResources.GetInfo(m_ResourceType);

                Hue = resourceInfo.Hue;
            }
        }

        [Constructable]
        public WoodHuePlating(): base(1532)
        {
            Name = "wood hue plating";
        }

        public WoodHuePlating(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            CraftResourceInfo info = CraftResources.GetInfo(m_ResourceType);

            LabelTo(from, Name);
            LabelTo(from, "(" + info.Name.ToLower() + ")" );            
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target a wood-based weapon or shield you wish to change to this hue.");
            from.Target = new HuePlatingTarget(this);
        }

        public class HuePlatingTarget : Target
        {
            private WoodHuePlating m_HuePlating;

            public HuePlatingTarget(WoodHuePlating HuePlating): base(2, false, TargetFlags.None)
            {
                m_HuePlating = HuePlating;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_HuePlating.Deleted || m_HuePlating.RootParent != from)     
                if (from == null) return;

                CraftResourceInfo resourceInfo = CraftResources.GetInfo(m_HuePlating.ResourceType);

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target a wood-based weapon or shield in your backpack.");
                        return;
                    }                    

                    if (item is BaseWeapon || item is BaseArmor || item is BaseShield)
                    {
                        if (item.PlayerClass != PlayerClass.None)
                        {
                            from.SendMessage("Playerclass items may not be hued with this item.");
                            return;
                        }

                        if (item.LootType == LootType.Newbied || item.LootType == LootType.Blessed)
                        {
                            from.SendMessage("Newbied or blessed items may not be hued with this item.");
                            return;
                        }

                        if (item is BaseWeapon)
                        {
                            BaseWeapon weapon = item as BaseWeapon;

                            if (!(weapon is BaseStaff || weapon is BaseRanged || weapon is Club))
                            {
                                from.SendMessage("Only wood-based weapons may be hued with this item.");
                                return;
                            }
                        }

                        if (item is BaseShield)
                        {
                            BaseShield shield = item as BaseShield;

                            if (!(shield is WoodenShield || shield is WoodenKiteShield))
                            {
                                from.SendMessage("Only wood-based shields may be hued with this item.");
                                return;
                            }
                        }

                        if (item.Hue == resourceInfo.Hue)
                        {
                            from.SendMessage("That item is already that hue.");
                            return;
                        }

                        else
                        {
                            item.Hue = resourceInfo.Hue;
                            from.PlaySound(0x23E);

                            from.SendMessage("You alter the hue of the item.");

                            m_HuePlating.Delete();                            
                        }
                    }

                    else
                    {
                        from.SendMessage("You must target a wood-based weapon or piece or armor in your backpack.");
                        return;
                    }
                }

                else
                {
                    from.SendMessage("You must target a wood-based weapon or piece or armor in your backpack.");
                    return;
                }  
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write((int)m_ResourceType);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_ResourceType = (CraftResource)reader.ReadInt();
            }
        }
    }
}