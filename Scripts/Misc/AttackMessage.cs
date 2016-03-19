using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;

namespace Server.Misc
{
	public class AttackMessage
	{
		private const string AggressorFormat = "You are attacking {0}!";
		private const string AggressedFormat = "{0} is attacking you!";
		private const int Hue = 0x22;

		private static TimeSpan Delay = TimeSpan.FromMinutes( 1.0 );

		public static void Initialize()
		{
			EventSink.AggressiveAction += new AggressiveActionEventHandler( EventSink_AggressiveAction );
		}

		public static void EventSink_AggressiveAction( AggressiveActionEventArgs e )
		{
            PlayerMobile pm_From = e.Aggressor as PlayerMobile;
            PlayerMobile pm_Target = e.Aggressed as PlayerMobile;

            BaseCreature bc_Target = e.Aggressed as BaseCreature;

            //UOACZ
            if (pm_From != null && bc_Target != null)
            {
                if (pm_From.IsUOACZHuman && bc_Target is UOACZBaseHuman)
                {
                    if (!CheckAggressions(pm_From, bc_Target) && pm_From.AccessLevel == AccessLevel.Player)
                        UOACZSystem.ChangeStat(pm_From, UOACZSystem.UOACZStatType.Honor, UOACZSystem.HumanAttackCivilianHonorLoss, true); 
                }  
            }

            if (pm_From == null || pm_Target == null)
                return;

            if (!CheckAggressions(pm_From, pm_Target) && pm_From.AccessLevel == AccessLevel.Player)
			{
                pm_From.LocalOverheadMessage(MessageType.Regular, Hue, true, String.Format(AggressorFormat, pm_Target.Name));
                pm_Target.LocalOverheadMessage(MessageType.Regular, Hue, true, String.Format(AggressedFormat, pm_From.Name));

                //UOACZ
                if (pm_From.IsUOACZHuman && pm_Target.IsUOACZHuman)
                {
                    if (Notoriety.Compute(pm_From, pm_Target) == Notoriety.Innocent)                    
                        UOACZSystem.ChangeStat(pm_From, UOACZSystem.UOACZStatType.Honor, UOACZSystem.HumanAttackPlayerHonorLoss, true);                    
                }               
			}           
		}

		public static bool CheckAggressions( Mobile m1, Mobile m2 )
		{
			List<AggressorInfo> list = m1.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( info.Attacker == m2 && DateTime.UtcNow < (info.LastCombatTime + Delay) )
					return true;
			}

			list = m2.Aggressors;

			for ( int i = 0; i < list.Count; ++i )
			{
				AggressorInfo info = list[i];

				if ( info.Attacker == m1 && DateTime.UtcNow < (info.LastCombatTime + Delay) )
					return true;
			}

			return false;
		}
	}
}