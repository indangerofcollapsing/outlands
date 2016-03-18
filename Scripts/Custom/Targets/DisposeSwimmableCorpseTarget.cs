/***************************************************************************
 *                               DisposeSwimmableCorpseTarget.cs
 *                            -------------------
 *   begin                : May 23, 2014
 *   author               : Patt Rojanasthien
 *   email                : frostshoxx@gmail.com
 *
 *
 ***************************************************************************/

using Server.Engines.Harvest;
using Server.Engines.Quests;
using Server.Engines.Quests.Hag;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Targets
{
    public class DisposeSwimmableCorpseTarget : Target
    {
        private Item m_Item;

        public DisposeSwimmableCorpseTarget(Item item)
            : base(2, false, TargetFlags.None)
        {
            m_Item = item;
        }

        protected override void OnTargetOutOfRange(Mobile from, object targeted)
        {
            base.OnTargetOutOfRange(from, targeted);
        }

        protected override void OnTarget(Mobile from, object targeted)
        {
            if (m_Item.Deleted)
                return;

            //Check if the target is a corpse and also container (to avoid casting error exception)
            if (targeted is Corpse && targeted is Container)
            {
                Corpse corpse = (Corpse)targeted;
                Container container = (Container)targeted;
                //Make sure they can swim (sea creatures).
                if (corpse.Owner != null && corpse.Owner.CanSwim)
                {
                    //Make sure their backpack is emptied
                    if (container.TotalItems == 0)
                    {
                        corpse.BeginDecay(TimeSpan.FromSeconds(1.0));
                        from.SendMessage("You start chopping the corpse into small disposable pieces.");
                        from.PlaySound(0x13E);

                        //Remove the durability for each successful use
                        var cutterItem = (IUsesRemaining)m_Item;
                        cutterItem.UsesRemaining--;
                        
                        //Destroy the tool if the durability is less than 1
                        if (cutterItem.UsesRemaining < 1)
                        {
                            m_Item.Delete();
                            from.SendMessage("Your tool falls apart while you are chopping the corpse.");
                        }

                    }
                    else
                    {
                        from.SendMessage("You found something inside the corpse and decide not to chop it.");
                    }
                }
                else
                {
                    from.SendMessage("You decide to keep the tool for only chopping sea creatures instead.");
                }
            }
            else
            {
                from.SendMessage("You cannot do that!");
            }

        }
    }
}