using System;
using Server.Targeting;
using Server.Custom.Items;
using Server.Custom.Townsystem.Items;
using Server.Custom.Donations.Items;

namespace Server.Items
{
    public class HatArmorPatch : Item
    {
        public override string DefaultName { get { return "a mask armor patch"; } }

        public static Type[] MaskTypes = new Type[] { 
            typeof(BearMask), typeof(ArmoredBearMask),
            typeof(DeerMask), typeof(ArmoredDeerMask),
            typeof(TribalMask), typeof(ArmoredTribalMask),
            typeof(Bandana), typeof(ReinforcedBandana),
            typeof(Bonnet), typeof(ReinforcedBonnet),
            typeof(Cap), typeof(ReinforcedCap),
            typeof(FeatheredHat), typeof(ReinforcedFeatheredHat),
            typeof(FloppyHat), typeof(ReinforcedFloppyHat),
            typeof(JesterHat), typeof(ReinforcedJesterHat),
            typeof(SkullCap), typeof(ReinforcedSkullcap),
            typeof(StrawHat), typeof(ReinforcedStrawHat),
            typeof(TallStrawHat), typeof(ReinforcedTallStrawHat),
            typeof(TricorneHat), typeof(ReinforcedTricorneHat),
            typeof(WideBrimHat), typeof(ReinforcedWideBrimHat),
            typeof(WizardsHat), typeof(ReinforcedWizardsHat),
            typeof(OrcMask), typeof(ArmoredOrcMask),
            typeof(OrcishKinMask), typeof(ArmoredOrcishKinMask),
            typeof(OgreLordMask), typeof(ArmoredOgreLordMask),
            typeof(MilitiaHeadband), typeof(ArmoredMilitiaHeadband),           
            typeof(MilitiaWearable), typeof(ArmoredMilitiaWearable),
            typeof(SilverStagMask), typeof(ArmoredSilverStagMask),
            typeof(HornedTribalMask), typeof(ArmoredHornedTribalMask),
            typeof(PirateBandana), typeof(ArmoredPirateBandana),
            typeof(PirateSkullcap), typeof(ArmoredPirateSkullcap),
            typeof(PirateTricorneHat), typeof(ArmoredPirateTricorneHat),
        };

        public static bool IsArmorable(object o)
        {
            if (o == null)
                return false;

            int res, index = Array.IndexOf(MaskTypes, o.GetType());

            Math.DivRem(index, 2, out res);

            // special case for militia wearable
            // ensure only hat slot items can be armored
            if (o is MilitiaWearable)
            {
                return ((MilitiaWearable)o).Layer == Layer.Helm;
            }

            return (index != -1 && res == 0);
        }

        public void ApplyArmor(Mobile from, Item item)
        {
            int index = Array.IndexOf(MaskTypes, item.GetType());
            Item newMask;

            if (item is MilitiaWearable)
            {
                newMask = new ArmoredMilitiaWearable();
                newMask.ItemID = item.ItemID;
                newMask.Layer = item.Layer;
            }
            else if (item is MilitiaHeadband)
                newMask = new ArmoredMilitiaHeadband(((MilitiaHeadband)item).Town);
            else
                newMask = Activator.CreateInstance(MaskTypes[index + 1]) as Item;

            newMask.DonationItem = item.DonationItem;
            newMask.LootType = item.LootType;
            newMask.Hue = item.Hue;

            if (item.PlayerClassRestricted)
                newMask.PlayerClassOwner = item.PlayerClassOwner;

            from.Backpack.DropItem(newMask);
            item.Delete();

            from.SendMessage("You apply the armor patch to your mask.");
            Delete();
        }

        [Constructable]
        public HatArmorPatch()
            : base(0x0376)
        {
        }

        public HatArmorPatch(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack)) {
                from.SendMessage("That must be in your backpack to use it.");
                    return;
            }

            from.Target = new InternalTarget(this);
            from.SendMessage("Which mask would you like to apply the armor to?");
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }

        private class InternalTarget : Target
        {
            private HatArmorPatch m_Sender;

            public InternalTarget(HatArmorPatch sender)
                : base(1, false, TargetFlags.None)
            {
                m_Sender = sender;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_Sender == null || m_Sender.Deleted)
                    return;
                else if (!HatArmorPatch.IsArmorable(targeted))
                {
                    from.SendMessage("You cannot apply armor to that item.");
                    return;
                }
                
                Item item = targeted as Item;

                if (item == null)
                    return;

                if (!item.IsChildOf(from.Backpack))
                {
                    from.SendMessage("That must be in your backpack to apply the armor.");
                }
                else
                {
                    m_Sender.ApplyArmor(from, item);
                }
            }
        }
    }
}
