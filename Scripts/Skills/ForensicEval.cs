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

		public static TimeSpan OnUse( Mobile m )
		{
			m.Target = new ForensicTarget();
			m.RevealingAction();

            m.SendMessage("Show me the crime you wish to investigate or research materials / master scroll you wish to research.");			

			return TimeSpan.FromSeconds( 2.0 );
		}

		public class ForensicTarget : Target
		{
			public ForensicTarget() : base( 10, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object target )
			{
				if ( target is Mobile )
				{
					if ( from.CheckTargetSkill( SkillName.Forensics, target, 40.0, 100.0 ) )
					{
						if ( target is PlayerMobile && ((PlayerMobile)target).NpcGuild == NpcGuild.ThievesGuild )
							from.SendLocalizedMessage( 501004 );//That individual is a thief!

						else
							from.SendLocalizedMessage( 501003 );//You notice nothing unusual.
					}

					else					
						from.SendLocalizedMessage( 501001 );//You cannot determain anything useful.					
                                     
                    PlayerMobile pm = from as PlayerMobile;
                    PlayerMobile pm_Target = target as PlayerMobile;

                    if (pm != null && pm_Target != null)
                    {   
                        bool inVengeanceList = pm.FindVengeanceEntry(pm_Target);
                        
                        if (inVengeanceList)
                            pm.SendMessage("You may take vengeance against this player.");
                    }
				}

                else if (target is Head)
                {
                    Head head = target as Head;

                    string sVictim = head.PlayerName;
                    bool bVictimPaladin = false;
                    bool bVictimMurderer = false;

                    if (head.PlayerType == PlayerType.Paladin)
                        bVictimPaladin = true;

                    if (head.PlayerType == PlayerType.Murderer)
                        bVictimMurderer = true;

                    string sKiller = head.KillerName;
                    bool bKillerPaladin = false;
                    bool bKillerMurderer = false;

                    if (head.KillerType == PlayerType.Paladin)
                        bKillerPaladin = true;

                    if (head.KillerType == PlayerType.Murderer)
                        bKillerMurderer = true;

                    if (sKiller != "" && sKiller != null && sVictim != "" && sVictim != null)
                    {
                        string text = "It is the head of " + sVictim;

                        if (bVictimPaladin)
                            text += " the paladin.";

                        else if (bVictimMurderer)
                            text += " the murderer.";

                        else
                            text += ".";

                        text += " They would appear to have been slain by " + sKiller;

                        if (bKillerPaladin)
                            text += " the paladin.";

                        else if (bKillerMurderer)
                            text += " the murderer.";

                        else
                            text += ".";

                        from.SendMessage(text);
                    }

                    else
                        from.SendMessage("It appears to be a head, but you cannot discern the details of it's owner's demise.");
                }

                else if (target is Corpse)
                {
                    if (from.CheckTargetSkill(SkillName.Forensics, target, 0.0, 100.0))
                    {
                        Corpse c = (Corpse)target;

                        if (!Custom.Detective.OnTargetCorpse(from, c))//If the killer has not already been identified...
                            if (((Body)c.Amount).IsHuman)
                                c.LabelTo(from, 1042751, (c.Killer == null ? "no one" : c.Killer.Name));//This person was killed by ~1_KILLER_NAME~

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

                        if (from.CheckSkill(SkillName.Forensics, 95, 120))
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
