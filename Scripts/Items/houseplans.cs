using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Commands;
using Server.Gumps;
using System.Reflection;


namespace Server.Items
{
    public abstract class UnfilledHousePlans : Item
    {
        public abstract int RequiredBoards { get; }
        public int CurrentBoards { get; set; }
        public abstract int RequiredIngots { get; }
        public int CurrentIngots { get; set; }
        public abstract BaseHousePlans FilledPlans { get; }

        public UnfilledHousePlans()
            : base(5359)
        {
            Weight = 1.0;
        }

        public static void Create(Mobile from, Type type)
        {
            UnfilledHousePlans ubp = GetUnfilledItem(type);
            double toDelete = 0.035 * Utility.RandomDouble();

            ubp.CurrentBoards = (int)((double)ubp.RequiredBoards * (1 - toDelete));
            ubp.CurrentIngots = (int)((double)ubp.RequiredIngots * (1 - toDelete*2));
            from.AddToBackpack(ubp);
            from.SendMessage("You have destroyed some of the boards required to make this House.");
        }

        public static UnfilledHousePlans GetUnfilledItem(Type type)
        {
            if (type == typeof(SmallStoneTempleHousePlans))
                return new UnfilledSmallStoneTempleHousePlans();
            else if (type == typeof(ArbitersEstateHousePlans))
                return new UnfilledArbitersEstateHousePlans();
            else if (type == typeof(SandstoneSpaHousePlans))
                return new UnfilledSandstoneSpaHousePlans();
            else if (type == typeof(MagistratesHousePlans))
                return new UnfilledMagistratesHousePlans();
            else if (type == typeof(BalconyHousePlans))
                return new UnfilledBalconyHousePlans();
            else
                return null;
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            LabelTo(from, "Boards: [{0}/{1}] Ingots: [{2}/{3}] ", CurrentBoards, RequiredBoards, CurrentIngots, RequiredIngots);

        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is BaseResourceBoard && from.InRange(GetWorldLocation(), 2))
                FillWith(from, (BaseResourceBoard)dropped);

            else if (dropped is IronIngot && from.InRange(GetWorldLocation(), 2))
                FillWith(from, (IronIngot)dropped);

            return false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
                return;

            from.SendMessage("Target the materials you would like to fill the plan with.");
            from.Target = new InternalTarget(this);
        }

        public void FillWith(Mobile from, BaseResourceBoard board)
        {
            int need = RequiredBoards - CurrentBoards;

            int toAdd = Math.Min(need, board.Amount);

            CurrentBoards += toAdd;

            board.Amount -= need;

            if (board.Amount <= 0)
                board.Delete();

            if (CurrentBoards < RequiredBoards || CurrentIngots < RequiredIngots)
                from.SendMessage("You have added {0} boards to the plan.", toAdd);
            else
            {
                from.SendMessage("The filled plans have been added to your backpack.");
                from.AddToBackpack(FilledPlans);
                Delete();
            }
        }

        public void FillWith(Mobile from, IronIngot ironingot)
        {
            int need = RequiredIngots - CurrentIngots;

            int toAdd = Math.Min(need, ironingot.Amount);

            CurrentIngots += toAdd;

            ironingot.Amount -= need;

            if (ironingot.Amount <= 0)
                ironingot.Delete();

            if (CurrentIngots < RequiredIngots || CurrentBoards < RequiredBoards)
                from.SendMessage("You have added {0} ingots to the plan.", toAdd);
            else
            {
                from.SendMessage("The filled plans have been added to your backpack.");
                from.AddToBackpack(FilledPlans);
                Delete();
            }
        }

        public UnfilledHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(CurrentIngots);
            writer.Write(CurrentBoards);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            CurrentIngots = reader.ReadInt();

            CurrentBoards = reader.ReadInt();
        }

        private class InternalTarget : Target
        {
            public UnfilledHousePlans m_Plans;

            public InternalTarget(UnfilledHousePlans plans)
                : base(2, false, TargetFlags.None)
            {
                m_Plans = plans;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is BaseResourceBoard || targeted is IronIngot))
                {
                    from.SendLocalizedMessage(1149667); //invalid target
                    return;
                }

                Item target = targeted as Item;
                if (!target.IsChildOf(from.Backpack) && !target.IsChildOf(from.BankBox))
                {
                    from.SendLocalizedMessage(1042010); //must be in your pack
                    return;
                }

                if (m_Plans != null && !m_Plans.Deleted)
                {
                    if (target is BaseResourceBoard)
                        m_Plans.FillWith(from, (BaseResourceBoard)target);
                    if (target is IronIngot)
                        m_Plans.FillWith(from, (IronIngot)target);
                }
            }
        }

    }

    public class UnfilledSmallStoneTempleHousePlans : UnfilledHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] small stone temple plans"; } }
        public override int RequiredBoards { get { return 10000; } }
        public override int RequiredIngots { get { return 1600; } }
        public override BaseHousePlans FilledPlans { get { return new SmallStoneTempleHousePlans(); } }

        [Constructable]
        public UnfilledSmallStoneTempleHousePlans()
        {
        }

        public UnfilledSmallStoneTempleHousePlans(Serial serial)
            : base(serial)
        {
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
    }
    public class UnfilledArbitersEstateHousePlans : UnfilledHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] Arbiter's Estate House Plans"; } }
        public override int RequiredBoards { get { return 171500; } }
        public override int RequiredIngots { get { return 27500; } }
        public override BaseHousePlans FilledPlans { get { return new ArbitersEstateHousePlans(); } }

        [Constructable]
        public UnfilledArbitersEstateHousePlans()
        {
        }

        public UnfilledArbitersEstateHousePlans(Serial serial)
            : base(serial)
        {
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
    }
    public class UnfilledSandstoneSpaHousePlans : UnfilledHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] sandstone spa house plans"; } }
        public override int RequiredBoards { get { return 40625; } }
        public override int RequiredIngots { get { return 6500; } }
        public override BaseHousePlans FilledPlans { get { return new SandstoneSpaHousePlans(); } }

        [Constructable]
        public UnfilledSandstoneSpaHousePlans()
        {
        }

        public UnfilledSandstoneSpaHousePlans(Serial serial)
            : base(serial)
        {
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
    }
    public class UnfilledMagistratesHousePlans : UnfilledHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] magistrate's house plans"; } }
        public override int RequiredBoards { get { return 40625; } }
        public override int RequiredIngots { get { return 6500; } }
        public override BaseHousePlans FilledPlans { get { return new MagistratesHousePlans(); } }

        [Constructable]
        public UnfilledMagistratesHousePlans()
        {
        }

        public UnfilledMagistratesHousePlans(Serial serial)
            : base(serial)
        {
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
    }
    public class UnfilledBalconyHousePlans : UnfilledHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] Two Story House with Balcony Plans"; } }
        public override int RequiredBoards { get { return 89375; } }
        public override int RequiredIngots { get { return 14300; } }
        public override BaseHousePlans FilledPlans { get { return new BalconyHousePlans(); } }

        [Constructable]
        public UnfilledBalconyHousePlans()
        {
        }

        public UnfilledBalconyHousePlans(Serial serial)
            : base(serial)
        {
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
    }
    public abstract class BaseHousePlans : Item
    {
        public abstract UnfilledHousePlans UnfilledPlans { get; }

        public BaseHousePlans()
            : base(5359)
        {
            Weight = 1.0;
        }

        public void Fail(Mobile from)
        {
            Delete();
            UnfilledHousePlans plans = UnfilledPlans;
            plans.CurrentBoards = (int)((double)plans.RequiredBoards * (Utility.RandomDouble() * 0.25 + 0.75));
            plans.CurrentIngots = (int)((double)plans.RequiredIngots * (Utility.RandomDouble() * 0.05 + 0.5));
        }

        public BaseHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

        }
    }

    public class SmallStoneTempleHousePlans : BaseHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] small stone temple plans"; } }
        public override UnfilledHousePlans UnfilledPlans { get { return new UnfilledSmallStoneTempleHousePlans(); } }

        [Constructable]
        public SmallStoneTempleHousePlans()
        {
        }

        public SmallStoneTempleHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
    public class ArbitersEstateHousePlans : BaseHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] arbiters estate house plans"; } }
        public override UnfilledHousePlans UnfilledPlans { get { return new UnfilledArbitersEstateHousePlans(); } }

        [Constructable]
        public ArbitersEstateHousePlans()
        {
        }

        public ArbitersEstateHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

    }
    public class SandstoneSpaHousePlans : BaseHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] sandstone spa house plans"; } }
        public override UnfilledHousePlans UnfilledPlans { get { return new UnfilledSandstoneSpaHousePlans(); } }

        [Constructable]
        public SandstoneSpaHousePlans()
        {
        }

        public SandstoneSpaHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

    }
    public class MagistratesHousePlans : BaseHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] magistrates house plans"; } }
        public override UnfilledHousePlans UnfilledPlans { get { return new UnfilledMagistratesHousePlans(); } }

        [Constructable]
        public MagistratesHousePlans()
        {
        }

        public MagistratesHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

    }
    public class BalconyHousePlans : BaseHousePlans
    {
        public override string DefaultName { get { return "[Carpentry Plan] two story house with balcony plans"; } }
        public override UnfilledHousePlans UnfilledPlans { get { return new UnfilledBalconyHousePlans(); } }

        [Constructable]
        public BalconyHousePlans()
        {
        }

        public BalconyHousePlans(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

}