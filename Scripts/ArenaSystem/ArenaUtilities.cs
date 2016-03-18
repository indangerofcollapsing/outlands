using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    public class Pair<T1, T2>
    {
        public Pair(T1 p1, T2 p2)
        {
            First = p1;
            Second = p2;
        }
        public T1 First { get; set; }
        public T2 Second { get; set; }
    }
    public class Trip<T1, T2, T3>
    {
        public Trip(T1 p1, T2 p2, T3 p3)
        {
            First = p1;
            Second = p2;
            Third = p3;
        }
        public T1 First { get; set; }
        public T2 Second { get; set; }
        public T3 Third { get; set; }
    }
}
namespace Server.ArenaSystem
{
    public class ArenaUtilities
    {
        /// <summary>
        /// For every queue type combination, a queue exists. GetIndexForCriterion converts a combination into the correct index.
        /// </summary>
        /// <param name="restrictions"></param>
        /// <param name="eras"></param>
        /// <param name="templated"></param>
        /// <returns></returns>
        public static int GetIndexForCriterion(EArenaMatchRestrictions restrictions, EArenaMatchEra eras, bool templated)
        {
            // 11/16/13 - Expanded for convenience of adding additional enums.
            int enumVariations = (int)EArenaMatchEra.eAMR_NumEras * (int)EArenaMatchRestrictions.eAMC_NumRestrictions;
            if (restrictions == EArenaMatchRestrictions.eAMC_Order)
            {
                if (eras == EArenaMatchEra.eAMR_IPY)
                    return 0 + (templated ? enumVariations : 0);
                if (eras == EArenaMatchEra.eAMR_Pub16)
                    return 1 + (templated ? enumVariations : 0);
                if (eras == EArenaMatchEra.eAMR_T2A)
                    return 2 + (templated ? enumVariations : 0);
            }
            else
            {
                if (eras == EArenaMatchEra.eAMR_IPY)
                    return 3 + (templated ? enumVariations : 0);
                if (eras == EArenaMatchEra.eAMR_Pub16)
                    return 4 + (templated ? enumVariations : 0);
                if (eras == EArenaMatchEra.eAMR_T2A)
                    return 5 + (templated ? enumVariations : 0);
            }

            return -1;
        }
        /// <summary>
        /// For every queue type combination, a queue exists. GetQueueCriterionCount retrieves the maximum queue types.
        /// </summary>
        /// <returns></returns>
        public static int GetQueueCriterionCount()
        {
			//int enumVariations = Enum.GetNames(typeof(EArenaMatchEra)).Length + Enum.GetNames(typeof(EArenaMatchEra)).Length;
			int enumVariations = (int)EArenaMatchEra.eAMR_NumEras * (int)EArenaMatchRestrictions.eAMC_NumRestrictions;

            // Variations * 2 due to template versions.
            return enumVariations * 2;
        }
        public static void WriteLine(string format, params object[] args)
        {
            Console.WriteLine(String.Concat("AS: ", format), args);
        }
        public static void WriteError(Exception exc, string message, params object[] args)
        {
            Utility.PushColor(ConsoleColor.Red);
            WriteLine(message, args);
            WriteLine(exc.Message);
            WriteLine(exc.StackTrace);
            Utility.PopColor();
        }
    }
}
