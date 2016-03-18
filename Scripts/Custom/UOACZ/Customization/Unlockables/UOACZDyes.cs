using System;
using Server.Targeting;
using Server.HuePickers;

namespace Server.Items
{
    public class UOACZDyes : Item
    {
        [Constructable]
        public UOACZDyes(): base(0xFA9)
        {
            Weight = 1.0;
        }

        public UOACZDyes(Serial serial): base(serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            from.SendLocalizedMessage(500856);
            from.Target = new InternalTarget();
        }

        private class InternalTarget : Target
        {
            public InternalTarget(): base(1, false, TargetFlags.None)
            {
            }

            private class InternalPicker : HuePicker
            {
                private UOACZDyeTub m_Tub;

                public InternalPicker(UOACZDyeTub tub): base(tub.ItemID)
                {
                    m_Tub = tub;
                }

                public override void OnResponse(int hue)
                {
                    m_Tub.DyedHue = hue;
                }
            }            

            public virtual void SetTubHue(Mobile from, object state, int hue)
            {
                if (state is UOACZDyeTub)
                {
                    UOACZDyeTub tub = state as UOACZDyeTub;

                    tub.DyedHue = hue;
                }
            }

            protected override void OnTarget(Mobile from, object targeted)
            {                
                if (targeted is UOACZDyeTub)
                {
                    UOACZDyeTub tub = (UOACZDyeTub)targeted;

                    if (tub.Redyable)
                    {
                        if (tub.MetallicHues)
                            from.SendGump(new MetallicHuePicker(from, new MetallicHuePicker.MetallicHuePickerCallback(SetTubHue), tub));

                        else if (tub.CustomHuePicker != null)
                            from.SendGump(new CustomHuePickerGump(from, tub.CustomHuePicker, new CustomHuePickerCallback(SetTubHue), tub));

                        else
                            from.SendHuePicker(new InternalPicker(tub));
                    }                   

                    else
                        from.SendMessage("That dye tub may not be redyed.");
                }

                else
                    from.SendLocalizedMessage(500857); // Use this on a dye tub.				
            }
        }
    }
}