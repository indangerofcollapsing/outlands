using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Custom;

namespace Server.SkillHandlers
{
	public class RemoveTrap
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.RemoveTrap].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
            if (m.Region is UOACZRegion)
            {
                m.Target = new InternalTarget();
                m.SendMessage("What will you search for traps?");

                m.Target = new InternalTarget();

                return TimeSpan.FromSeconds(SkillCooldown.RemoveTrapCooldown);
            }

			if ( m.Skills[SkillName.Lockpicking].Value < 50 )			
				m.SendLocalizedMessage( 502366 ); // You do not know enough about locks.  Become better at picking locks.
						
			else
			{
				m.Target = new InternalTarget();
				m.SendLocalizedMessage( 502368 ); // Wich trap will you attempt to disarm?
			}

			return TimeSpan.FromSeconds( SkillCooldown.RemoveTrapCooldown );
		}

		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
			}

            private void RemoveTrap(Mobile from, TrapableContainer targ)
            {
                targ.TrapPower = 0;
                targ.TrapLevel = 0;
                targ.TrapType = TrapType.None;
                targ.Trapper = null;

                from.PlaySound(0x5AC);

                from.SendLocalizedMessage(502377); // You successfully render the trap harmless
            }

			protected override void OnTarget( Mobile from, object targeted )
			{
                if (from.Region is UOACZRegion)
                {
                    if (targeted is UOACZScavengeContainer || targeted is UOACZScavengeDebris)
                    {
                        UOACZBaseScavengeObject scavengeObject = targeted as UOACZBaseScavengeObject;

                        scavengeObject.RemoveTrap(from);                       
                    }

                    else                    
                        from.SendMessage("That cannot be trapped.");

                    return;
                }

				if ( targeted is Mobile )				
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate
				
				else if ( targeted is TrapableContainer )
				{
					TrapableContainer targ = (TrapableContainer)targeted;

					from.Direction = from.GetDirectionTo( targ );

					if ( targ.TrapType == TrapType.None )
					{
						from.SendLocalizedMessage( 502373 ); // That doesn't appear to be trapped
						return;
					}

                    from.PlaySound(0x4D3); //0x241

                    if (targ is BaseTreasureChest)
                    {
                        BaseTreasureChest tchest = targ as BaseTreasureChest;

                        double minSkill = 35.0 + ((int)tchest.Level * 10.0); // 45, 55, 65, 75, 85, 95
                        double maxSkill = 60.0 + ((int)tchest.Level * 10.0); // 70, 80, 90, 100, 110, 120

                        // chance to open l5 at gm: 60%
                        // chance to open l6 at gm: 20%

                        if (from.Skills.RemoveTrap.Value < minSkill && (int)tchest.Level == 1)
                        {
                            from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.RemoveTrapCooldown * 1000);

                            from.CheckTargetSkill(SkillName.RemoveTrap, targ, 0, 100, 1.0);
                            from.SendMessage("You fail to safely remove the trap.");

                            return;
                        }
                        
                        else if (from.Skills.RemoveTrap.Value < minSkill)
                        {
                            from.SendMessage("You do not have sufficient skill to attempt the removal of that trap.");
                            return;
                        }

                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.RemoveTrapCooldown * 1000);

                        if (from.CheckTargetSkill(SkillName.RemoveTrap, targ, minSkill, maxSkill, 1.0))                        
                            RemoveTrap(from, targ);    
                    
                        else                 
						    from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off					
                    }


                    else if (targ is TreasureMapChest)
                    {
                        TreasureMapChest tchest = targ as TreasureMapChest;

                        double minSkill = TreasureMapChest.RemoveTrapSkillRequiredPerLevel[tchest.Level];
                        double maxSkill = TreasureMapChest.RemoveTrapSkillMaxPerLevel[tchest.Level];

                        if (from.Skills.RemoveTrap.Value < minSkill)
                        {
                            from.SendMessage("You do not have sufficient skill to attempt the removal of that trap.");
                            return;
                        }

                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.RemoveTrapCooldown * 1000);

                        if (from.CheckTargetSkill(SkillName.RemoveTrap, targ, minSkill, maxSkill, 1.0))                        
                            RemoveTrap(from, targ); 
                       
                        else                 
						    from.SendLocalizedMessage( 502372 ); // You fail to disarm the trap... but you don't set it off		
                    }

                    else if (from.CheckTargetSkill(SkillName.RemoveTrap, targ, targ.TrapPower, targ.TrapPower + 40, 1.0))
                    {
                        RemoveTrap(from, targ);
                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.RemoveTrapCooldown * 1000);
                    }

                    else
                    {
                        from.SendLocalizedMessage(502372); // You fail to disarm the trap... but you don't set it off		
                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.RemoveTrapCooldown * 1000);
                    }
				}
				
				else				
					from.SendLocalizedMessage( 502373 ); // That does'nt appear to be trapped				
			}
		}
	}
}