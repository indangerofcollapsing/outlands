using System;
using Server.Engines.Plants;
using Server.Targeting;

namespace Server.Items
{
    public class DormantSpringSeed : Item
    {
        int seeds;
        int dirt;
        int water;
        int fertilizerCharges;
        bool essenceOfNature;
        bool isFilled;

        [CommandProperty(AccessLevel.GameMaster)]
        public int SeedCount { get { return seeds; } set { seeds = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int DirtCount { get { return dirt; } set { dirt = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int waterCount { get { return water; } set { water = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AccelerantCount { get { return fertilizerCharges; } set { fertilizerCharges = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool EssenceOfNature { get { return essenceOfNature; } set { essenceOfNature = value; } }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ItemFilled { get { return isFilled; } set { isFilled = value; Hue = 1167; } }


        [Constructable]
        public DormantSpringSeed()
            : base(0x1600)
        {
            Name = ("dormant springseed");
            Weight = 1.0;
            Hue = 2001;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                return;
            }

            if (isFilled)
            {
                SpringSeed springseed = new SpringSeed();
                if (!from.AddToBackpack(springseed))
                {
                    springseed.Delete();
                    from.SendMessage("You don't have enough room in your backpack. Please make room and try again.");
                }
                else
                {
                    from.SendMessage("You reap a vibrant spring seed from the pot! Something tells you it belongs in the forest.");
                    from.SendSound(0x000);
                    Delete();
                }
                return;
            }
            if (seeds < 25)
                from.SendMessage("Looks like you still need more plant seeds.");
            if (dirt < 250)
                from.SendMessage("Looks like you still need more fertile dirt.");
            if (water < 5)
                from.SendMessage("Looks like you still need more water.");
            if (fertilizerCharges < 10)
                from.SendMessage("Looks like you still need more plant accelerants.");
            if (!(essenceOfNature))
                from.SendMessage("Looks like you still need an essence of nature.");
            from.Target = new InternalTarget(this);
        }

        public DormantSpringSeed(Serial serial) : base(serial) { }

        private class InternalTarget : Target
        {
            private readonly DormantSpringSeed m_DormantSpringSeed;

            public InternalTarget(DormantSpringSeed dormantspringseed)
                : base(3, true, TargetFlags.None)
            {
                this.m_DormantSpringSeed = dormantspringseed;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_DormantSpringSeed == null || m_DormantSpringSeed.Deleted)
                    return;

                if (!this.m_DormantSpringSeed.IsChildOf(from.Backpack))
                {
                    from.SendLocalizedMessage(1042664); // You must have the object in your backpack to use it.
                    return;
                }

                if (targeted == null)
                    return;

                // Seeds
                if (targeted is Seed && m_DormantSpringSeed.seeds < 25)
                {
                    Seed seed = (Seed)targeted;
                    if (seed == null)
                        return;

                    int maxChargesToAdd = 25 - m_DormantSpringSeed.seeds;
                    int chargesAdded = seed.Amount;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    m_DormantSpringSeed.seeds += chargesAdded;
                    seed.Amount -= chargesAdded;

                    if (seed.Amount <= 0)
                        seed.Delete();

                    from.SendMessage("You plant some seed into the soil.");
                    from.SendSound(0x12D);
                }
                // Dirt
                else if (targeted is FertileDirt && m_DormantSpringSeed.dirt < 250)
                {
                    FertileDirt fertileDirt = (FertileDirt)targeted;
                    if (fertileDirt == null)
                        return;

                    int maxChargesToAdd = 250 - m_DormantSpringSeed.dirt;
                    int chargesAdded = fertileDirt.Amount;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    m_DormantSpringSeed.dirt += chargesAdded;
                    fertileDirt.Amount -= chargesAdded;

                    if (fertileDirt.Amount <= 0)
                        fertileDirt.Delete();

                    from.SendMessage("You lay some fertilized soil into the container.");
                    from.SendSound(0x12E);
                }
                // Water
                else if (isWaterContainer(targeted) && m_DormantSpringSeed.water < 5)
                {
                    fillWithWater(from, targeted);
                }
                // Accelerant
                else if (targeted is PlantGrowthAccelerant && m_DormantSpringSeed.fertilizerCharges < 10)
                {
                    PlantGrowthAccelerant accelerant = (PlantGrowthAccelerant)targeted;
                    if (accelerant == null)
                        return;

                    int maxChargesToAdd = 10 - m_DormantSpringSeed.fertilizerCharges;
                    int chargesAdded = accelerant.Charges;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    m_DormantSpringSeed.fertilizerCharges += chargesAdded;
                    accelerant.Charges -= chargesAdded;

                    if (accelerant.Charges <= 0)
                        accelerant.Delete();

                    from.SendMessage("You mix some accelerants into the plants soil.");
                    from.SendSound(0x59);
                }
                // Essence of Nature
                else if (targeted is EssenceOfNature && m_DormantSpringSeed.essenceOfNature == false)
                {
                    EssenceOfNature essence = (EssenceOfNature)targeted;
                    if (essence == null)
                        return;
                    m_DormantSpringSeed.essenceOfNature = true;
                    essence.Consume();
                    from.SendSound(0x1EB);
                }
                else
                {
                    from.SendMessage("You don't need to use that!");
                }
                if (m_DormantSpringSeed.seeds > 24 &&
                    m_DormantSpringSeed.dirt > 249 &&
                    m_DormantSpringSeed.water > 4 &&
                    m_DormantSpringSeed.fertilizerCharges > 9 &&
                    m_DormantSpringSeed.essenceOfNature == true
                    )
                {
                    m_DormantSpringSeed.isFilled = true;
                    m_DormantSpringSeed.Hue = 1167;
                    from.SendSound(0x28);
                }
            }

            public bool isWaterContainer(object targ)
            {
                if (targ is BaseBeverage)
                {
                    return true;
                }
                else if (targ is BaseWaterContainer)
                {
                    return true;
                }
                else if (targ is Item)
                {
                    Item item = (Item)targ;
                    IWaterSource src;

                    src = (item as IWaterSource);

                    if (src == null && item is AddonComponent)
                        src = (((AddonComponent)item).Addon as IWaterSource);

                    if (!(src == null || src.Quantity <= 0))
                    {
                        return true;
                    }
                }
                return false;
            }

            public void fillWithWater(Mobile from, object targ)
            {
                if (targ is BaseBeverage)
                {
                    BaseBeverage bev = (BaseBeverage)targ;

                    if (bev.IsEmpty || !bev.ValidateUse(from, true))
                    {
                        from.SendMessage("Hmm seems like you can't use this to water your plant.");
                        return;
                    }
                    int maxChargesToAdd = 5 - m_DormantSpringSeed.water;
                    int chargesAdded = bev.Quantity;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    m_DormantSpringSeed.water += chargesAdded;
                    bev.Quantity -= chargesAdded;

                    from.PlaySound(0x4E);
                    from.SendLocalizedMessage(1010089); // You fill the container with water.

                }
                else if (targ is BaseWaterContainer)
                {
                    BaseWaterContainer bwc = targ as BaseWaterContainer;

                    if (bwc == null || bwc.Quantity < 1)
                    {
                        from.SendMessage("Hmm seems like you can't use this to water your plant.");
                        return;
                    }

                    int maxChargesToAdd = 5 - m_DormantSpringSeed.water;
                    int chargesAdded = bwc.Quantity;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    m_DormantSpringSeed.water += chargesAdded;
                    bwc.Quantity -= chargesAdded;

                    from.PlaySound(0x4E);
                    from.SendLocalizedMessage(1010089); // You fill the container with water.
                }
                else if (targ is Item)
                {
                    Item item = (Item)targ;
                    IWaterSource src;

                    src = (item as IWaterSource);

                    if (src == null && item is AddonComponent)
                        src = (((AddonComponent)item).Addon as IWaterSource);

                    if (src == null || src.Quantity <= 0)
                    {
                        from.SendMessage("Hmm seems like you can't use this to water your plant.");
                        return;
                    }

                    if (from.Map != item.Map || !from.InRange(item.GetWorldLocation(), 2) || !from.InLOS(item))
                    {
                        from.SendMessage("I can't reach that.");
                        return;
                    }
                    int maxChargesToAdd = 5 - m_DormantSpringSeed.water;
                    int chargesAdded = src.Quantity;

                    if (chargesAdded > maxChargesToAdd)
                        chargesAdded = maxChargesToAdd;

                    m_DormantSpringSeed.water += chargesAdded;
                    src.Quantity -= chargesAdded;

                    from.PlaySound(0x4E);
                    from.SendLocalizedMessage(1010089); // You fill the container with water.
                }
            }
        }


        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(seeds);
            writer.Write(dirt);
            writer.Write(water);
            writer.Write(fertilizerCharges);
            writer.Write((bool)essenceOfNature);
            writer.Write((bool)isFilled);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            seeds = reader.ReadInt();
            dirt = reader.ReadInt();
            water = reader.ReadInt();
            fertilizerCharges = reader.ReadInt();
            essenceOfNature = reader.ReadBool();
            isFilled = reader.ReadBool();
        }
    }
}