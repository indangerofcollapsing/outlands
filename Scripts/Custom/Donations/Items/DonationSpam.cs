using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Items
{
    public class DonationSpam : Spam
    {
        public static readonly int[] Hues = new int[] { 758, 658, 558, 458, 358, 258, 158, 58 };

        [Constructable]
        public DonationSpam()
        {
        }

        public DonationSpam(Serial serial)
            : base(serial)
        {
        }

        public override bool Eat(Mobile from)
        {
            bool ret = base.Eat(from);

            if (ret) {
                PlayerMobile pm = from as PlayerMobile;
                if (pm != null) {
                    if (pm.HueMod > -1)
                    {
                        int hueIndex = Array.IndexOf(Hues, pm.HueMod);
                        if (hueIndex < 0) {
                            pm.HueMod = Hues[0];
                        }
                        else if (hueIndex < Hues.Length - 1) {
                            pm.HueMod = Hues[hueIndex + 1];
                        }
                    }
                    else
                    {
                        pm.HueMod = Hues[0];
                    }

					if (pm.HueModEnd < DateTime.UtcNow)
						pm.HueModEnd = DateTime.UtcNow;

                    pm.HueModEnd += TimeSpan.FromMinutes(5);

                    pm.SendMessage("You feel terribly sick after eating the spam.");
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
    }
}
