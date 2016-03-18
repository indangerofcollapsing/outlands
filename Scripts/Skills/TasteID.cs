using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Mobiles;

namespace Server.SkillHandlers
{
	public class TasteID
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.TasteID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new InternalTarget();

			m.SendLocalizedMessage( 502807 ); // What would you like to taste?

			return TimeSpan.FromSeconds( 2.0 );
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( 2, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( targeted is Mobile )
				{
					from.SendLocalizedMessage( 502816 ); // You feel that such an action would be inappropriate.
				}

                else if (targeted is CustomAlchemyPotion)
                {
                    CustomAlchemyPotion customAlchemyPotion = targeted as CustomAlchemyPotion;
                    string name = customAlchemyPotion.GetPotionName();

                    switch (customAlchemyPotion.EffectPotency)
                    {
                        case CustomAlchemy.EffectPotencyType.Target:
                        break;

                        case CustomAlchemy.EffectPotencyType.SmallAoE:
                            if (from.Skills.TasteID.Value < 50)
                            {
                                from.SendMessage("You must have at least 50 Taste Identification skill to analyze that potion's composition.");
                                return;
                            }
                        break;

                        case CustomAlchemy.EffectPotencyType.MediumAoE:
                            if (from.Skills.TasteID.Value < 75)
                            {
                                from.SendMessage("You must have at least 75 Taste Identification skill to analyze that potion's composition.");
                                return;
                            }
                        break;

                        case CustomAlchemy.EffectPotencyType.LargeAoE:
                            if (from.Skills.TasteID.Value < 100)
                            {
                                from.SendMessage("You must have at least 100 Taste Identification skill to analyze that potion's composition.");
                                return;
                            }
                        break;
                    }

                    if (from.CheckTargetSkill(SkillName.TasteID, 0, 100))
                    {
                        customAlchemyPotion.Identified = true;
                        from.SendMessage("You analyze the potion and determine its composition: " + name + ".");

                        from.SendSound(0x5AF);
                    }

                    else
                    {
                        from.SendMessage("You are not certain of the potion's composition.");                        
                        from.SendSound(0x5AD);
                    }
                }

                else if (targeted is Food)
                {
                    Food food = (Food)targeted;

                    if (from.CheckTargetSkill(SkillName.TasteID, food, 0, 100))
                    {
                        if (food.Poison != null)
                        {
                            food.SendLocalizedMessageTo(from, 1038284); // It appears to have poison smeared on it.
                        }
                        else
                        {
                            // No poison on the food
                            food.SendLocalizedMessageTo(from, 1010600); // You detect nothing unusual about this substance.
                        }
                    }
                    else
                    {
                        // Skill check failed
                        food.SendLocalizedMessageTo(from, 502823); // You cannot discern anything about this substance.
                    }
                }
                else if (targeted is BasePotion)
                {
                    BasePotion potion = (BasePotion)targeted;

                    potion.SendLocalizedMessageTo(from, 502813); // You already know what kind of potion that is.
                    potion.SendLocalizedMessageTo(from, potion.LabelNumber);
                }
                else if (targeted is PotionKeg)
                {
                    PotionKeg keg = (PotionKeg)targeted;

                    if (keg.Held <= 0)
                    {
                        keg.SendLocalizedMessageTo(from, 502228); // There is nothing in the keg to taste!
                    }
                    else
                    {
                        keg.SendLocalizedMessageTo(from, 502229); // You are already familiar with this keg's contents.
                        keg.SendLocalizedMessageTo(from, keg.LabelNumber);
                    }
                }
                else
                {
                    // The target is not food or potion or potion keg.
                    from.SendLocalizedMessage(502820); // That's not something you can taste.
                }
			}

			protected override void OnTargetOutOfRange( Mobile from, object targeted )
			{
				from.SendLocalizedMessage( 502815 ); // You are too far away to taste that.
			}
		}
	}
}