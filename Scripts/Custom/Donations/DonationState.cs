using System;
using System.Collections.Generic;
using Server.Mobiles;

namespace Server.Custom
{
    public class DonationState
    {
        private static readonly TimeSpan twentyFourHours = TimeSpan.FromDays(1);
        public enum GazerStatuetteState { Gazer, Beholder, MercuryGazer }

        public DateTime LastCounterReset { get; set; }
        public int TicketsOpenedSinceLastReset { get; set; }
        public int TotalOgre2TicketsOpened { get; set; }
        public GazerStatuetteState GazerState { get; set; }

        public DonationState() {
			LastCounterReset = DateTime.UtcNow;
            TicketsOpenedSinceLastReset = 0;
            TotalOgre2TicketsOpened = 0;
        }

        public static DonationState Find(Mobile m) {
            PlayerMobile pm = m as PlayerMobile;

            if (pm == null)
                return null;

            if (pm.DonationPlayerState == null)
                pm.DonationPlayerState = new DonationState();

            return pm.DonationPlayerState;
        }

        public static double GetModifier(Mobile m) {
            var ds = Find(m);
            double mod = 1.0;

            if (ds != null) {
				if (ds.LastCounterReset + twentyFourHours < DateTime.UtcNow)
				{
					ds.LastCounterReset = DateTime.UtcNow;
                    ds.TicketsOpenedSinceLastReset = 0;
                }

                ds.TicketsOpenedSinceLastReset++;
                ds.TotalOgre2TicketsOpened++;

                if (ds.TicketsOpenedSinceLastReset <= 20)
                    mod = 1.0;
                else if (ds.TicketsOpenedSinceLastReset <= 50)
                    mod = 0.5;
                else
                    mod = 0.25;
                
            }

            return mod;
        }

        public static void Serialize(GenericWriter writer, PlayerMobile pm) {
            writer.Write((int)1);
            
            //version 0
            var ds = pm.DonationPlayerState;
            bool toWrite = ds != null;
            
            writer.Write(toWrite);

            if (toWrite) {
                writer.Write(ds.TotalOgre2TicketsOpened); //version 1
                writer.Write(ds.LastCounterReset);
                writer.Write(ds.TicketsOpenedSinceLastReset);
                writer.Write((byte)ds.GazerState);
            }
        }

        public static DonationState Deserialize(GenericReader reader) {
            int version = reader.ReadInt();
            DonationState ds = null;
            
            bool toRead = reader.ReadBool();

            if (toRead) {
                ds = new DonationState();

                switch (version) {
                    case 1: {
                            ds.TotalOgre2TicketsOpened = reader.ReadInt();
                            goto case 0;
                        }
                    case 0: {
                            ds.LastCounterReset = reader.ReadDateTime();
                            ds.TicketsOpenedSinceLastReset = reader.ReadInt();
                            ds.GazerState = (GazerStatuetteState)reader.ReadByte();
                            break;
                        }
                }
            }

            return ds;
        }
    }
}
