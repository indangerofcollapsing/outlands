using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class LicoriceCandy : Food
    {
        public override string DefaultName {
            get 
	{
                return "licorice candy";
        }
        }

        private static readonly TimeSpan Duration = TimeSpan.FromMinutes(10);

        [Constructable]
        public LicoriceCandy()
            : base(0x469D) 
        {
            LootType = Server.LootType.Blessed;
            DonationItem = true;
            Hue = 2051;

        }

        public LicoriceCandy(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            bool ret = base.Eat(from);

            if (ret && Utility.RandomDouble() < 0.75) {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null) {
                    pm.BodyMod = 317;
		    pm.HueMod = 2106;
                    InternalTimer t = new InternalTimer(from);
                    t.Start();
                }
            }

            return ret;
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

        private class InternalTimer : Timer
        {
            private Mobile mob;

            public InternalTimer(Mobile m)
                : base(TimeSpan.FromSeconds(120)) {
                mob = m;
            }
            protected override void OnTick() {
                mob.BodyMod = 0;
		mob.HueMod = -1;
            }
        }
    }
}
