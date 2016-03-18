using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Mobiles;

namespace Server.Custom
{
    static class Utilities
    {
        public static Mobile FindPlayerDamagerFromKiller(Mobile killer)
        {
            if (killer is BaseCreature)
            {
                BaseCreature creaturekiller = killer as BaseCreature;

                if (creaturekiller.BardProvoked)
                    return creaturekiller.BardMaster;

                else
                    return creaturekiller.GetMaster();
            }

            return killer;
        }
    }

	public abstract class WinnerSelector
	{
		public abstract Mobile SelectWinner(BaseCreature monster_killed);
		public abstract string GetShortDescription();
	}

    public class LoHWinnerSelectors_HighestDamage : WinnerSelector
    {
        public override Mobile SelectWinner(BaseCreature monster_killed)
        {
            return monster_killed.FindMostTotalDamger(false);
        }

		public override string GetShortDescription()
		{
			return "Most Damage Done";
		}
    }

    public class LoHWinnerSelectors_Killshot : WinnerSelector
    {
        public override Mobile SelectWinner(BaseCreature monster_killed)
        {
            return Utilities.FindPlayerDamagerFromKiller(monster_killed.FindMostRecentDamager(false));
        }

		public override string GetShortDescription()
		{
			return "Killing Blow";
		}
    }

    public class LoHWinnerSelectors_RandomDamager : WinnerSelector
    {       
        public override Mobile SelectWinner(BaseCreature monster_killed)
        {
            int winner_index = Utility.Random(monster_killed.DamageEntries.Count);
            return Utilities.FindPlayerDamagerFromKiller(monster_killed.DamageEntries[winner_index].Damager);
        }

		public override string GetShortDescription()
		{
			return "Random Aggressor";
		}
    }
}
