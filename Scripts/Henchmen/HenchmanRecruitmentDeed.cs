using System;
using Server;
using Server.Regions;
using Server.Targeting;
using Server.Engines.CannedEvil;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Multis;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom
{    
    public class HenchmanRecruitmentDeed : Item
    {
        private int m_Experience;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Experience
        {
            get { return m_Experience; }
            set { m_Experience = value; }
        }

        private int m_SlotsRemaining = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SlotsRemaining
        {
            get { return m_SlotsRemaining; }
            set { m_SlotsRemaining = value; }
        }

        public int MaxSlots = 5;

        [Constructable]
        public HenchmanRecruitmentDeed(): base(0x14F0)
		{
            Name = "a henchman deed";

            Weight = 0.1;
            Hue = 2587;

            m_Experience = Utility.Random(1, 250);
		}

        public HenchmanRecruitmentDeed(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "(bonus exp: " + m_Experience.ToString() + ")");
            LabelTo(from, "[control slots left: " + m_SlotsRemaining.ToString() + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That must be in your backpack for you to use it.");
                return;
            }

            from.SendMessage("Which individual would you like to recruit?");
            from.Target = new HenchmanTarget(this);
        }

        public class HenchmanTarget : Target
        {
            private HenchmanRecruitmentDeed m_Deed;

            public HenchmanTarget(HenchmanRecruitmentDeed deed): base(8, false, TargetFlags.None)
            {
                m_Deed = deed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_Deed.Deleted || m_Deed.RootParent != from)
                    return;

                PlayerMobile pm_From = from as PlayerMobile;

                if (pm_From == null)
                    return;

                if (!m_Deed.IsChildOf(pm_From.Backpack))
                {
                    pm_From.SendMessage("The henchman recruitment deed is no longer in your backpack.");
                    return;
                }

                BaseHenchman henchman = target as BaseHenchman;

                if (henchman != null)
                {
                    if (henchman.Deleted || !henchman.Alive)
                        return;

                    if (!henchman.Recruitable)
                    {
                        pm_From.SendMessage("That individual can not be recruited in this manner.");
                        return;
                    }

                    if (henchman.Controlled)
                    {
                        pm_From.SendMessage("That individual already has an employer.");
                        return;
                    }

                    if (pm_From.Skills.Begging.Value < henchman.MinTameSkill)
                    {
                        pm_From.SendMessage("You do not have the neccessary Begging skill required to recruit and command that individual.");
                        return;
                    }

                    if (pm_From.Skills.Camping.Value < henchman.MinTameSkill)
                    {
                        pm_From.SendMessage("You do not have the neccessary Camping skill required to recruit and command that individual.");
                        return;
                    }

                    if (pm_From.Followers + henchman.ControlSlots > pm_From.FollowersMax)
                    {
                        pm_From.SendMessage("You do not have enough control slots available to recruit that individual.");
                        return;
                    }

                    if (henchman.ControlSlots > m_Deed.SlotsRemaining)
                    {
                        pm_From.SendMessage("This deed does not have enough control slots remaining to recruit that individual.");
                        return;
                    }

                    if (henchman.m_XMLSpawner != null)
                    {
                        if (!henchman.m_XMLSpawner.Deleted && henchman.m_XMLSpawner.InfluenceMarker != null)
                        {
                            if (!henchman.m_XMLSpawner.InfluenceMarker.Deleted)
                            {
                                InfluenceMarker influenceMarker = henchman.m_XMLSpawner.InfluenceMarker;

                                int prestigeLevel = 0;
                                int investmentLevel = 0;

                                influenceMarker.GetPlayerLevels(pm_From, out prestigeLevel, out investmentLevel);

                                if (prestigeLevel + investmentLevel < 5)
                                {
                                    pm_From.SendMessage("You must have at least 5 total Influence at this building in order to recruit individuals here. You may increase your Influence in this building through accessing the building's influence marker.");
                                    return;
                                }                                
                            }
                        }
                    }

                    henchman.Blessed = false;

                    henchman.Owners.Add(pm_From);
                    henchman.TimesTamed++;
                    henchman.SetControlMaster(pm_From);
                    henchman.IsBonded = true;
                    henchman.OwnerAbandonTime = DateTime.UtcNow + henchman.AbandonDelay;
                    henchman.ResurrectionsRemaining = 2;

                    henchman.Experience = m_Deed.m_Experience;

                    henchman.Animate(33, 5, 1, true, false, 0);

                    if (henchman.recruitSpeech.Length > 0)
                        henchman.Say(henchman.recruitSpeech[Utility.Random(henchman.recruitSpeech.Length - 1)]);

                    henchman.Hits = henchman.HitsMax;
                    henchman.Stam = henchman.StamMax;
                    henchman.Mana = henchman.ManaMax;

                    henchman.Animate(33, 5, 1, true, false, 0);

                    pm_From.SendMessage("You recruit the individual and they bond to you. They may be resurrected " + henchman.ResurrectionsRemaining.ToString() + " times before they fade from creation.");

                    m_Deed.m_SlotsRemaining -= henchman.ControlSlots;   

                    if (m_Deed.m_SlotsRemaining <= 0)
                        m_Deed.Delete();
                }

                else
                {
                    pm_From.SendMessage("That is not a recruitable individual.");
                    return;
                }
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            writer.Write(m_Experience);
            writer.Write(m_SlotsRemaining);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Experience = reader.ReadInt();
                m_SlotsRemaining = reader.ReadInt();
            }
		}
    }
}