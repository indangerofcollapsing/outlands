using System;
using Server;
using Server.Targeting;
using Server.Mobiles;

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
}