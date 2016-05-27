using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
    public class ItemIdentification
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile from)
        {
            from.SendLocalizedMessage(500343); // What do you wish to appraise and identify?
            from.Target = new InternalTarget();

            return TimeSpan.FromSeconds(1.0);
        }

        [PlayerVendorTarget]
        private class InternalTarget : Target
        {
            public InternalTarget(): base(8, false, TargetFlags.None)
            {
                AllowNonlocal = true;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Container && from.Skills.ItemID.Base >= 100.0)
                {
                    var container = o as Container;

                    foreach (var item in container.Items)
                    {
                        if (item is BaseWeapon)
                            ((BaseWeapon)item).Identified = true;

                        else if (item is BaseArmor)
                            ((BaseArmor)item).Identified = true;
                    }

                    from.SendMessage("You identify the bag of goods.");
                }

                else if (o is Item)
                {
                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ItemIDCooldown * 1000);

                    if (from.CheckTargetSkill(SkillName.ItemID, o, 0, 100, 1.0))
                    {
                        if (o is BaseWeapon)
                            ((BaseWeapon)o).Identified = true;

                        else if (o is BaseArmor)
                            ((BaseArmor)o).Identified = true;

                        if (!Core.AOS)
                            ((Item)o).OnSingleClick(from);
                    }

                    else                    
                        from.SendLocalizedMessage(500353); // You are not certain...                    
                }

                else if (o is Mobile)                
                    ((Mobile)o).OnSingleClick(from);
                
                else                
                    from.SendLocalizedMessage(500353); // You are not certain...                
            }
        }
    }

    public class ItemIdGump : Gump
    {
        PlayerMobile m_Player;
        Item m_Item;

        public ItemIdGump(PlayerMobile player, Item item): base(50, 50)
        {
            if (player == null || item == null) return;
            if (item.Deleted) return;

            m_Player = player;
            m_Item = item;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddPage(0);
            AddImage(135, 12, 103, 2401);
            AddImage(7, 12, 103, 2401);
            AddBackground(19, 21, 246, 78, 9270);
            AddLabel(316, 18, 2401, @"Basic");
            AddLabel(160, 39, 0, @"Item Rarity");
            AddLabel(162, 59, 1259, @"Ultra Rare");
            AddLabel(71, 92, 0, @"Total in World:");
            AddLabel(171, 92, 149, @"50");
            AddLabel(69, 4, 149, @"Emperor Dragon Trophy");
            AddLabel(316, 43, 0, @"Common");
            AddLabel(315, 67, 169, @"Uncommon");
            AddLabel(316, 93, 2603, @"Rare");
            AddLabel(316, 118, 2594, @"Very Rare");
            AddLabel(316, 142, 1259, @"Ultra Rare");
            AddLabel(56, 39, 0, @"Item Type");
            AddLabel(63, 59, 149, @"Reward");
        }

        public override void OnResponse(Network.NetState sender, RelayInfo info)
        {           
        }
    }
}