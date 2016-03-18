using System;
using Server.Targeting;
using Server.HuePickers;

namespace Server.Items
{
    public class Dyes : Item
    {
        [Constructable]
        public Dyes()
            : base(0xFA9)
        {
            Weight = 3.0;
        }

        public Dyes(Serial serial)
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

            if (Weight == 0.0)
                Weight = 3.0;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(500856); // Select the dye tub to use the dyes on.
            from.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(1, false, TargetFlags.None)
            {
            }

            private class InternalPicker : HuePicker
            {
                private DyeTub m_Tub;

                public InternalPicker(DyeTub tub)
                    : base(tub.ItemID)
                {
                    m_Tub = tub;
                }

                public override void OnResponse(int hue)
                {
                    m_Tub.DyedHue = hue;
                }
            }

            private class InternalRunebookPicker : HuePicker
            {
                private RunebookDyeTub m_RunebookDyeTub;

                public InternalRunebookPicker(RunebookDyeTub runebookDyeTub)
                    : base(runebookDyeTub.ItemID)
                {
                    m_RunebookDyeTub = runebookDyeTub;
                }

                public override void OnResponse(int hue)
                {
                    m_RunebookDyeTub.DyeColor = hue;
                }
            }

            private class InternalBulkOrderBulkPicker : HuePicker
            {
                private BulkOrderBookDyeTub m_BulkOrderBookDyeTub;

                public InternalBulkOrderBulkPicker(BulkOrderBookDyeTub bulkOrderBookDyeTub)
                    : base(bulkOrderBookDyeTub.ItemID)
                {
                    m_BulkOrderBookDyeTub = bulkOrderBookDyeTub;
                }

                public override void OnResponse(int hue)
                {
                    m_BulkOrderBookDyeTub.DyeColor = hue;
                }
            }

            public virtual void SetTubHue(Mobile from, object state, int hue)
            {
                if (state is RunebookDyeTub)
                {
                    RunebookDyeTub runebookDyeTub = state as RunebookDyeTub;

                    runebookDyeTub.DyeColor = hue;
                }

                if (state is BulkOrderBookDyeTub)
                {
                    BulkOrderBookDyeTub bulkOrderBookDyeTub = state as BulkOrderBookDyeTub;

                    bulkOrderBookDyeTub.DyeColor = hue;
                }

                if (state is DyeTub)
                {
                    DyeTub tub = state as DyeTub;

                    tub.DyedHue = hue;
                }
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is RunebookDyeTub)
                {
                    RunebookDyeTub runebookDyeTub = targeted as RunebookDyeTub;

                    if (runebookDyeTub.Redyable)
                    {
                        if (runebookDyeTub.CustomHuePicker != null)
                            from.SendGump(new CustomHuePickerGump(from, runebookDyeTub.CustomHuePicker, new CustomHuePickerCallback(SetTubHue), runebookDyeTub));

                        else
                            from.SendHuePicker(new InternalRunebookPicker(runebookDyeTub));
                    }

                    else
                        from.SendMessage("That dye tube cannot be redyed.");
                }
                else if (targeted is BulkOrderBookDyeTub)
                {
                    BulkOrderBookDyeTub bulkOrderBookDyeTub = targeted as BulkOrderBookDyeTub;

                    if (bulkOrderBookDyeTub.Redyable)
                    {
                        if (bulkOrderBookDyeTub.CustomHuePicker != null)
                            from.SendGump(new CustomHuePickerGump(from, bulkOrderBookDyeTub.CustomHuePicker, new CustomHuePickerCallback(SetTubHue), bulkOrderBookDyeTub));

                        else
                            from.SendHuePicker(new InternalBulkOrderBulkPicker(bulkOrderBookDyeTub));
                    }

                    else
                        from.SendMessage("That dye tube cannot be redyed.");
                }
                else if (targeted is DyeTub)
                {
                    DyeTub tub = (DyeTub)targeted;

                    if (tub.Redyable)
                    {
                        if (tub.MetallicHues)
                            from.SendGump(new MetallicHuePicker(from, new MetallicHuePicker.MetallicHuePickerCallback(SetTubHue), tub));

                        else if (tub.CustomHuePicker != null)
                            from.SendGump(new CustomHuePickerGump(from, tub.CustomHuePicker, new CustomHuePickerCallback(SetTubHue), tub));

                        else
                            from.SendHuePicker(new InternalPicker(tub));
                    }

                    else if (tub is BlackDyeTub)
                        from.SendLocalizedMessage(1010092); // You can not use this on a black dye tub.

                    else
                        from.SendMessage("That dye tub may not be redyed.");
                }

                else
                    from.SendLocalizedMessage(500857); // Use this on a dye tub.				
            }
        }
    }
}