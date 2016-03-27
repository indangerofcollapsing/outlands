using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Network;

namespace Server.SkillHandlers
{
	class SpiritSpeak
	{
		public static void Initialize()
		{
            SkillInfo.Table[(int)SkillName.SpiritSpeak].Callback = new SkillUseCallback(OnUse);
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.RevealingAction();

            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.SpiritSpeakCooldown * 1000);

			if ( from.CheckSkill( SkillName.SpiritSpeak, 0, 120, 1.0 ) )
			{	
				if ( !from.CanHearGhosts )
				{
					Timer t = new SpiritSpeakTimer( from );

					double secs = from.Skills[SkillName.SpiritSpeak].Base / 50;
					secs *= 90;

					if ( secs < 15 )
						secs = 15;

					t.Delay = TimeSpan.FromSeconds( secs );//15seconds to 3 minutes
					t.Start();
					from.CanHearGhosts = true;
				}

				from.PlaySound( 0x24A );
				from.SendLocalizedMessage( 502444 );//You contact the neitherworld.
			}

			else
			{
				from.SendLocalizedMessage( 502443 );//You fail to contact the neitherworld.
				from.CanHearGhosts = false;
			}

			return TimeSpan.FromSeconds( 1.0 );
		}

		private class SpiritSpeakTimer : Timer
		{
			private Mobile m_Owner;
			public SpiritSpeakTimer( Mobile m ) : base( TimeSpan.FromMinutes( 2.0 ) )
			{
				m_Owner = m;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				m_Owner.CanHearGhosts = false;
				m_Owner.SendLocalizedMessage( 502445 );//You feel your contact with the neitherworld fading.
			}
		}
	}
}