using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom;

namespace Server.SkillHandlers
{
	public class ArmsLore
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ArmsLore].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse(Mobile m)
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 500349 ); // What item do you wish to get information about?

			return TimeSpan.FromSeconds( 2.5 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() : base( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if (targeted is BaseWeapon)
				{
                    BaseWeapon weapon = (BaseWeapon)targeted;

                    if (weapon.Dungeon != DungeonEnum.None && weapon.TierLevel > 0)
                    {
                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

                        bool skillSuccess = from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 120, 1.0 );

                        from.SendGump(new DungeonWeaponGump(from, weapon, skillSuccess));

                        return;
                    }

                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

					if (from.CheckTargetSkill( SkillName.ArmsLore, targeted, 0, 120, 1.0 ) )					
						from.SendMessage("You estimate this weapon is at " + weapon.HitPoints.ToString() + " of " + weapon.MaxHitPoints.ToString() + " durability.");                    

					else					
						from.SendLocalizedMessage( 500353 ); // You are not certain...					
				}

				else if (targeted is BaseArmor)
				{
                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ArmsLoreCooldown * 1000);

					if (from.CheckTargetSkill(SkillName.ArmsLore, targeted, 0, 120, 1.0))
					{
						BaseArmor arm = (BaseArmor)targeted;
                        from.SendMessage("You estimate this armor is at " + arm.HitPoints.ToString() + " of " + arm.MaxHitPoints.ToString() + " durability.");
                    }

					else					
						from.SendLocalizedMessage( 500353 ); // You are not certain...					
				}

				else				
					from.SendLocalizedMessage( 500352 ); // This is neither weapon nor armor.				
			}
		}
	}
}