using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Scripts.Custom;

namespace Server.Custom
{
    public static class DousingGuild
    {
        public static Dictionary<Type, int> PointValues = new Dictionary<Type, int>()
        {
            {typeof(CoreHound), 1},
            {typeof(LavaSurger), 1},
            {typeof(CharredProtector), 1},
            {typeof(SuperOgreLord), 5},
            {typeof(RubyDragon), 5},
            {typeof(BaronVonGeddon), 7},
            {typeof(ImmortalFlameBoss), 15},
        };

        public static void RegionDeath(Mobile m)
        {
            if (!FireDungeon.Active)
                return;

            int pointValue;

            if (!PointValues.TryGetValue(m.GetType(), out pointValue))
                return;

            var players = m.Map == Map.Felucca ? FireDungeon.InstanceOne.Players : FireDungeon.InstanceTwo.Players;
  
            foreach (Mobile mob in players)
            {
                FireDungeonPlayerState.AddDousingPoints(mob as PlayerMobile, pointValue);
                mob.SendMessage(String.Format("Your group has been awarded {0} dousing points for slaying {1}.", pointValue, m.Name));
            }
        }

        public class DousingPointEntry
        {
            public Type Type { get; set; }
            public int Value { get; set; }
            public string Description { get; set; }

        }
    }

}
