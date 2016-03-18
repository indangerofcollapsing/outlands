using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Items;
using Server.Targeting;

namespace Server.Targets
{
    public class FoldClothesTarget : Target
    {
        private Item m_Item;

        public FoldClothesTarget(Item item)
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
            //Make sure tool still exists
            if (m_Item.Deleted)
                return;

            var targetItem = targeted as Item;

            if (targetItem != null && targetItem.IsLockedDown)
            {
                from.SendMessage("You cannot fold the locked down clothes!");
                return;
            }

            if (targeted is Cloth || targeted is UncutCloth)
            {
                from.PlaySound(0x248);
                from.SendMessage("You have folded the clothes neatly with the contraption.");
                var cloth = targeted as Item;
                var hue = cloth.Hue;
                var amount = cloth.Amount;
                cloth.Delete();
                from.AddToBackpack(new UncutCloth()
                {
                    Hue = hue,
                    Amount = amount
                });
            }
            else
            {
                from.SendMessage("Only clothes can be folded!");
            }

        }
    }
}
