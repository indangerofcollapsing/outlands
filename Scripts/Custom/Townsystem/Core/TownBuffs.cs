/***************************************************************************
 *                               TownBuffs.cs
 *                            ------------------
 *   begin                : December 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using System.Reflection;
using System.Collections.Generic;


namespace Server.Custom.Townsystem
{
    /// <summary>
    /// These are the current available town buffs for the Citizenship program.
    /// *important* Explicit integer casting ensures removal of a buff will not shift the remaining town buffs by one.
    /// </summary>
    public enum CitizenshipBuffs
    {
        None = 0,   //None
        Gather = 1,     // gathering bonus
        Crafting = 2,   // crafting skill gain / less resource consumption

        Adventurer = 4, // increased gold drops
        Luck = 5,       // 5% luck
        Hunter = 6,     // 5% damage to monsters
        Greed = 7,      // 10% plat/dread/doubloons/treasury keys
        Reprieve = 8,   // 10% less statloss duration (ocb/penance)
        Taming = 9,     // 10% chance to tame creatures
    }

    public class TownBuff
    {
        public string BuffName { get; set; }

        public TownBuff(string buffName)
        {
            BuffName = buffName;
        }

        public static Dictionary<int, TownBuff> Table = new Dictionary<int, TownBuff>
            {
                {(int)CitizenshipBuffs.Gather,     new TownBuff("Town of Gatherers")},
                {(int)CitizenshipBuffs.Crafting,   new TownBuff("Town of Crafters")},
                {(int)CitizenshipBuffs.Adventurer, new TownBuff("Town of Adventurers")},
                {(int)CitizenshipBuffs.Luck,       new TownBuff("Town of Luck")},
                {(int)CitizenshipBuffs.Hunter,     new TownBuff("Town of Hunters")},
                {(int)CitizenshipBuffs.Greed,      new TownBuff("Town of Greed")},
                {(int)CitizenshipBuffs.Reprieve,   new TownBuff("Town of Reprieve")},
                {(int)CitizenshipBuffs.Taming,     new TownBuff("Town of Tamers")},
            };

        public static int NumberOfPrimaryBuffs { get { return Table.Count; } }

        public static string GetBuffName(CitizenshipBuffs value)
        {
            TownBuff tb;
            Table.TryGetValue((int)value, out tb);
            return tb == null ? "" : tb.BuffName;
        }
    }
}