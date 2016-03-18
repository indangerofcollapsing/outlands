using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;

namespace Scripts.Custom
{
    public class FireDungeonPlayerState
    {
        public int DousingPoints { get; set; }
        public bool PurchasedFireElementalStatue { get; set; }
        public bool PurchasedShameLightGreyGloves { get; set; }

        public FireDungeonPlayerState()
        {
        }

        public static FireDungeonPlayerState Find(PlayerMobile pm)
        {
            if (pm == null)
                return null;

            return pm.FireDungeonState;
        }
        public static FireDungeonPlayerState FindOrCreate(PlayerMobile pm)
        {
            if (pm == null)
                return new FireDungeonPlayerState();

            if (pm.FireDungeonState == null)
                pm.FireDungeonState = new FireDungeonPlayerState();

            return pm.FireDungeonState;
        }

        public static void AddDousingPoints(PlayerMobile pm, int amount)
        {
            if (pm == null)
                return;

            var state = FindOrCreate(pm);
            state.DousingPoints += amount;
        }
        public static bool ConsumeFirePoints(PlayerMobile pm, int amount)
        {
            var state = Find(pm);

            if (state == null)
                return false;
            else if (state.DousingPoints < amount)
                return false;
            else
            {
                state.DousingPoints -= amount;
                return true;
            }
        }
        public static int GetDousingPoints(PlayerMobile pm)
        {
            var state = Find(pm);
            return state == null ? 0 : state.DousingPoints;
        }

        public static void Serialize(GenericWriter writer, PlayerMobile pm)
        {
            bool toWrite = pm.FireDungeonState != null;

            writer.Write(toWrite);

            if (toWrite)
            {
                writer.Write((int)0); //version
                writer.Write(pm.FireDungeonState.DousingPoints);
                writer.Write(pm.FireDungeonState.PurchasedShameLightGreyGloves);
                writer.Write(pm.FireDungeonState.PurchasedFireElementalStatue);
            }
        }

        public static FireDungeonPlayerState Deserialize(GenericReader reader)
        {
            bool toRead = reader.ReadBool();

            if (toRead)
            {
                var state = new FireDungeonPlayerState();
                int version = reader.ReadInt();

                state.DousingPoints = reader.ReadInt();
                state.PurchasedShameLightGreyGloves = reader.ReadBool();
                state.PurchasedFireElementalStatue = reader.ReadBool();

                return state;
            }
            else
            {
                return null;
            }
        }
    }
}
