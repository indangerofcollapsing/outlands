using System;
using System.Collections;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Targeting;
using Server.Items;
using Server.Custom;

namespace Server.SkillHandlers
{
	public class ForensicEvaluation
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Forensics].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.Target = new ForensicTarget();
			from.RevealingAction();

            from.SendMessage("What do you wish to carve, investigate, or research?");

			return TimeSpan.FromSeconds( 2.0 );
		}

		public class ForensicTarget : Target
		{
			public ForensicTarget() : base( 10, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object target )
			{
                if (from == null)
                    return;

				if (target is Mobile)
				{
                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ForensicsCooldown * 1000);

					if ( from.CheckTargetSkill( SkillName.Forensics, target, 50.0, 120.0, 1.0 ) )
					{
						if (target is PlayerMobile && ((PlayerMobile)target).NpcGuild == NpcGuild.ThievesGuild )
							from.SendLocalizedMessage( 501004 ); //That individual is a thief!

						else
							from.SendLocalizedMessage( 501003 ); //You notice nothing unusual.
					}

					else					
						from.SendLocalizedMessage( 501001 ); //You cannot determain anything useful.	
				}

                else if (target is Head)
                {
                }

                else if (target is Corpse)
                {
                    Corpse corpse = target as Corpse;                    

                    if (corpse.Owner is BaseCreature && !corpse.Carved && from.Backpack != null)
                    {
                        BaseCreature bc_Creature = corpse.Owner as BaseCreature;

                        bool foundBladed = false;

                        Item oneHanded = from.FindItemOnLayer(Layer.OneHanded);
                        Item twoHanded = from.FindItemOnLayer(Layer.TwoHanded);

                        if (oneHanded is BaseSword || oneHanded is BaseKnife)
                            foundBladed = true;

                        if (twoHanded is BaseSword || twoHanded is BaseKnife)
                            foundBladed = true;

                        if (!foundBladed)
                        {
                            Item[] items = from.Backpack.FindItemsByType(typeof(BaseMeleeWeapon));

                            for (int a = 0; a < items.Length; a++)
                            {
                                if (items[a] is BaseSword || items[a] is BaseKnife)
                                {
                                    foundBladed = true;
                                    break;
                                }
                            }
                        }

                        if (!foundBladed)
                        {
                            from.SendMessage("You must have a cutting weapon equipped or in your pack in order to carve this.");
                            return;
                        }                        

                        bc_Creature.OnCarve(from, corpse);

                        return;
                    }
                    
                    if (from.CheckTargetSkill(SkillName.Forensics, target, 0.0, 120.0, 1.0))
                    {
                        Corpse c = (Corpse)target;

                        if (c.Looters.Count > 0)
                        {
                            StringBuilder sb = new StringBuilder();

                            for (int i = 0; i < c.Looters.Count; i++)
                            {
                                if (i > 0)
                                    sb.Append(", ");
                                sb.Append(((Mobile)c.Looters[i]).Name);
                            }

                            c.LabelTo(from, 1042752, sb.ToString());//This body has been distrubed by ~1_PLAYER_NAMES~
                        }

                        else
                            c.LabelTo(from, 501002);//The corpse has not be desecrated.						
                    }

                    else
                        from.SendLocalizedMessage(501001);//You cannot determain anything useful.					
                }

                else if (target is ILockpickable)
                {
                    ILockpickable p = (ILockpickable)target;

                    if (p.Picker != null)
                        from.SendLocalizedMessage(1042749, p.Picker.Name);//This lock was opened by ~1_PICKER_NAME~

                    else
                        from.SendLocalizedMessage(501003);//You notice nothing unusual.
                }

                else if (target is ResearchMaterials)
                {
                    ResearchMaterials researchMaterials = target as ResearchMaterials;
                    researchMaterials.AttemptResearch(from);
                }

                else if (target is SpellScroll)
                {
                    SpellScroll spellScroll = target as SpellScroll;

                    if (spellScroll.MasterStatus == 1 || spellScroll.MasterStatus == 2)
                    {
                        if (from.Skills.Forensics.Value < 95)
                        {
                            from.SendMessage("You do not have enough forensics evaluation skill to make an appropriate research attempt on that.");
                            return;
                        }

                        from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.ForensicsCooldown * 1000);

                        if (from.CheckSkill(SkillName.Forensics, 95, 120, 1.0))
                        {
                            from.SendSound(0x652);
                            from.SendMessage("You learn many a great secret whilst scrying the words held within. You may now apply this researched master scroll to an ancient mystery scroll.");

                            spellScroll.Delete();
                            from.AddToBackpack(new ResearchedMasterScroll());

                            return;
                        }

                        else
                        {
                            if (Utility.RandomDouble() <= .33)
                            {
                                from.SendSound(0x5AE);
                                from.SendMessage("The scroll crumbles to dust before your very eyes and is lost!");

                                spellScroll.Delete();
                            }

                            else
                            {
                                from.SendSound(0x055);
                                from.SendMessage("You diligently sift through the words within the scroll, but are unable to glean any secrets held within.");
                            }

                            return;
                        }
                    }    
                }    
			}
		}
	}
}
