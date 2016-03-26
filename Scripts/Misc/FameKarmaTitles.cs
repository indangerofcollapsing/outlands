using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Engines.CannedEvil;

namespace Server.Misc
{
    public class FameKarmaTitles
    {
        public const int MinFame = 0;
        public const int MaxFame = 15000;

        public static void AwardFame(Mobile m, int offset, bool message)
        {
            if (offset > 0)
            {
                if (m.Fame >= MaxFame)
                    return;

                offset -= m.Fame / 100;

                if (offset < 0)
                    offset = 0;
            }
            else if (offset < 0)
            {
                if (m.Fame <= MinFame)
                    return;

                offset -= m.Fame / 100;

                if (offset > 0)
                    offset = 0;
            }

            if ((m.Fame + offset) > MaxFame)
                offset = MaxFame - m.Fame;
            else if ((m.Fame + offset) < MinFame)
                offset = MinFame - m.Fame;

            m.Fame += offset;

            if (message)
            {
                if (offset > 40)
                    m.SendLocalizedMessage(1019054); // You have gained a lot of fame.
                else if (offset > 20)
                    m.SendLocalizedMessage(1019053); // You have gained a good amount of fame.
                else if (offset > 10)
                    m.SendLocalizedMessage(1019052); // You have gained some fame.
                else if (offset > 0)
                    m.SendLocalizedMessage(1019051); // You have gained a little fame.
                else if (offset < -40)
                    m.SendLocalizedMessage(1019058); // You have lost a lot of fame.
                else if (offset < -20)
                    m.SendLocalizedMessage(1019057); // You have lost a good amount of fame.
                else if (offset < -10)
                    m.SendLocalizedMessage(1019056); // You have lost some fame.
                else if (offset < 0)
                    m.SendLocalizedMessage(1019055); // You have lost a little fame.
            }
        }

        public const int MinKarma = -15000;
        public const int MaxKarma = 15000;

        public static void AwardKarma(Mobile m, int offset, bool message)
        {
            if (m.Region is UOACZRegion)
                return;

            if (offset > 0)
            {
                if (m is PlayerMobile && ((PlayerMobile)m).KarmaLocked)
                    return;

                if (m.Karma >= MaxKarma)
                    return;

                offset -= m.Karma / 100;

                if (offset < 0)
                    offset = 0;
            }

            else if (offset < 0)
            {
                if (m.Karma <= MinKarma)
                    return;

                offset -= m.Karma / 100;

                if (offset > 0)
                    offset = 0;
            }

            if ((m.Karma + offset) > MaxKarma)
                offset = MaxKarma - m.Karma;
            else if ((m.Karma + offset) < MinKarma)
                offset = MinKarma - m.Karma;

            bool wasPositiveKarma = (m.Karma >= 0);

            m.Karma += offset;

            if (message)
            {
                if (offset > 40)
                    m.SendLocalizedMessage(1019062); // You have gained a lot of karma.
                else if (offset > 20)
                    m.SendLocalizedMessage(1019061); // You have gained a good amount of karma.
                else if (offset > 10)
                    m.SendLocalizedMessage(1019060); // You have gained some karma.
                else if (offset > 0)
                    m.SendLocalizedMessage(1019059); // You have gained a little karma.
                else if (offset < -40)
                    m.SendLocalizedMessage(1019066); // You have lost a lot of karma.
                else if (offset < -20)
                    m.SendLocalizedMessage(1019065); // You have lost a good amount of karma.
                else if (offset < -10)
                    m.SendLocalizedMessage(1019064); // You have lost some karma.
                else if (offset < 0)
                    m.SendLocalizedMessage(1019063); // You have lost a little karma.
            }

            if (!Core.AOS && wasPositiveKarma && m.Karma < 0 && m is PlayerMobile && !((PlayerMobile)m).KarmaLocked)
            {
                ((PlayerMobile)m).KarmaLocked = true;
                m.SendLocalizedMessage(1042511, "", 0x22); // Karma is locked.  A mantra spoken at a shrine will unlock it again.
            }
        }

        public static string[] HarrowerTitles = new string[] { "Spite", "Opponent", "Hunter", "Venom", "Executioner", "Annihilator", "Champion", "Assailant", "Purifier", "Nullifier" };

        public static string ComputeTitle(Mobile beholder, Mobile beheld, bool include_ipy_prefix)
        {
            StringBuilder title = new StringBuilder();

            int fame = beheld.Fame;
            int karma = beheld.Karma;

            bool showSkillTitle = beheld.ShowFameTitle && ((beholder == beheld) || (fame >= 5000));

            if (beheld.ShowFameTitle || (beholder == beheld))
            {
                for (int i = 0; i < m_FameEntries.Length; ++i)
                {
                    FameEntry fe = m_FameEntries[i];

                    if (fame <= fe.m_Fame || i == (m_FameEntries.Length - 1))
                    {
                        KarmaEntry[] karmaEntries = fe.m_Karma;

                        for (int j = 0; j < karmaEntries.Length; ++j)
                        {
                            KarmaEntry ke = karmaEntries[j];

                            if (karma <= ke.m_Karma || j == (karmaEntries.Length - 1))
                            {
                                //title.AppendFormat(ke.m_Title, beheld.Name, beheld.Female ? "Lady" : "Lord");
                                title.Append(ke.m_Title);
                                break;
                            }
                        }

                        break;
                    }
                }
            }

            if (beheld.Fame >= 10000)
            {
                title.AppendFormat(title.Length > 0 ? " {0}" : "{0}", beheld.Female ? "Lady" : "Lord");
            }

            title.AppendFormat(title.Length > 0 ? " {0}" : "{0}", beheld.Name);

            string customTitle = beheld.Title;
            if (customTitle != null && customTitle.Trim().Length > 0)
            {
                title.AppendFormat(customTitle.IndexOf("the", 0, 5) == -1 ? ", {0}" : " {0}", customTitle.Trim());
            }            

            if (customTitle == null || customTitle.Trim().Length == 0)
            {
                if (showSkillTitle && beheld.Player)
                {
                    string skillTitle = GetSkillTitle(beheld);

                    if (skillTitle != null)
                    {
                        title.Append(", ").Append(skillTitle);
                    }
                }
            }

            return title.ToString();
        }

        public static string GetSkillTitle(Mobile mob)
        {
            Skill highest = GetHighestSkill(mob);// beheld.Skills.Highest;

            if (highest != null && highest.BaseFixedPoint >= 300)
            {
                string skillLevel = GetSkillLevel(highest);
                string skillTitle = highest.Info.Title;

                if (mob.Female && skillTitle.EndsWith("man"))
                    skillTitle = skillTitle.Substring(0, skillTitle.Length - 3) + "woman";

                return String.Concat(skillLevel, " ", skillTitle);
            }

            return null;
        }

        private static Skill GetHighestSkill(Mobile m)
        {
            Skills skills = m.Skills;

            Skill highest = null;

            for (int i = 0; i < m.Skills.Length; ++i)
            {
                Skill check = m.Skills[i];

                if (highest == null || check.BaseFixedPoint > highest.BaseFixedPoint)
                    highest = check;
                else if (highest != null && highest.Lock != SkillLock.Up && check.Lock == SkillLock.Up && check.BaseFixedPoint == highest.BaseFixedPoint)
                    highest = check;
            }

            //Just make sure that the method won't return null for whatever odd reason
            if (highest == null)
                highest = skills.Highest;

            return highest;
        }

        private static string[,] m_Levels = new string[,]
			{
				{ "Neophyte",		"Neophyte",		"Neophyte"		},
				{ "Novice",			"Novice",		"Novice"		},
				{ "Apprentice",		"Apprentice",	"Apprentice"	},
				{ "Journeyman",		"Journeyman",	"Journeyman"	},
				{ "Expert",			"Expert",		"Expert"		},
				{ "Adept",			"Adept",		"Adept"			},
				{ "Master",			"Master",		"Master"		},
				{ "Grandmaster",	"Grandmaster",	"Grandmaster"	},
				{ "Elder",			"Tatsujin",		"Shinobi"		},
				{ "Legendary",		"Kengo",		"Ka-ge"			}
			};

        private static string GetSkillLevel(Skill skill)
        {
            return m_Levels[GetTableIndex(skill), GetTableType(skill)];
        }

        public static string GetSkillLevelName(int value)
        {
            int fp = Math.Min(Math.Max(300, value), 1200); // titles between 300 and 1200
            int index = (fp - 300) / 100;
            return m_Levels[index, 0];
        }

        // Nummmnut - SkillScroll
        // Made this public so GetSkillLevel may be seen and used with the SkillScrolls
        // This allows the PackSkillScroll to see what level the player's title is for the given skill
        public static string GetSkillLevelName(Skill skill)
        {
            string defaultSkillLevel = "Neophyte";
            if (skill != null)
            {
                if (skill.BaseFixedPoint >= 300)
                {
                    return GetSkillLevel(skill);
                }
                else
                {
                    return defaultSkillLevel;
                }
            }
            else
            {
                return "null";
            }
        }
        // Nummmnut -SkillScroll end

        private static int GetTableType(Skill skill)
        {
            switch (skill.SkillName)
            {
                default: return 0;
            }
        }

        private static int GetTableIndex(Skill skill)
        {
            int fp = Math.Min(skill.BaseFixedPoint, 1200);

            return (fp - 300) / 100;
        }

        private static FameEntry[] m_FameEntries = new FameEntry[]
			{
				new FameEntry( 1249, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "The Outcast" ),
					new KarmaEntry( -5000, "The Despicable" ),
					new KarmaEntry( -2500, "The Scoundrel" ),
					new KarmaEntry( -1250, "The Unsavory" ),
					new KarmaEntry( -625, "The Rude" ),
					new KarmaEntry( 624, "" ),
					new KarmaEntry( 1249, "The Fair" ),
					new KarmaEntry( 2499, "The Kind" ),
					new KarmaEntry( 4999, "The Good" ),
					new KarmaEntry( 9999, "The Honest" ),
					new KarmaEntry( 10000, "The Trustworthy" )
				} ),
				new FameEntry( 2499, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "The Wretched" ),
					new KarmaEntry( -5000, "The Dastardly" ),
					new KarmaEntry( -2500, "The Malicious" ),
					new KarmaEntry( -1250, "The Dishonorable" ),
					new KarmaEntry( -625, "The Disreputable" ),
					new KarmaEntry( 624, "The Notable" ),
					new KarmaEntry( 1249, "The Upstanding" ),
					new KarmaEntry( 2499, "The Respectable" ),
					new KarmaEntry( 4999, "The Honorable" ),
					new KarmaEntry( 9999, "The Commendable" ),
					new KarmaEntry( 10000, "The Estimable" )
				} ),
				new FameEntry( 4999, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "The Nefarious" ),
					new KarmaEntry( -5000, "The Wicked" ),
					new KarmaEntry( -2500, "The Vile" ),
					new KarmaEntry( -1250, "The Ignoble" ),
					new KarmaEntry( -625, "The Notorious" ),
					new KarmaEntry( 624, "The Prominent" ),
					new KarmaEntry( 1249, "The Reputable" ),
					new KarmaEntry( 2499, "The Proper" ),
					new KarmaEntry( 4999, "The Admirable" ),
					new KarmaEntry( 9999, "The Famed" ),
					new KarmaEntry( 10000, "The Great" )
				} ),
				new FameEntry( 9999, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "The Dread" ),
					new KarmaEntry( -5000, "The Evil" ),
					new KarmaEntry( -2500, "The Villainous" ),
					new KarmaEntry( -1250, "The Sinister" ),
					new KarmaEntry( -625, "The Infamous" ),
					new KarmaEntry( 624, "The Renowned" ),
					new KarmaEntry( 1249, "The Distinguished" ),
					new KarmaEntry( 2499, "The Eminent" ),
					new KarmaEntry( 4999, "The Noble" ),
					new KarmaEntry( 9999, "The Illustrious" ),
					new KarmaEntry( 10000, "The Glorious" )
				} ),
				new FameEntry( 10000, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "The Dread" ),
					new KarmaEntry( -5000, "The Evil" ),
					new KarmaEntry( -2500, "The Dark" ),
					new KarmaEntry( -1250, "The Sinister" ),
					new KarmaEntry( -625, "The Dishonored" ),
					new KarmaEntry( 624, "" ),
					new KarmaEntry( 1249, "The Distinguished" ),
					new KarmaEntry( 2499, "The Eminent" ),
					new KarmaEntry( 4999, "The Noble" ),
					new KarmaEntry( 9999, "The Illustrious" ),
					new KarmaEntry( 10000, "The Glorious" )
				} )
			};
    }

    public class FameEntry
    {
        public int m_Fame;
        public KarmaEntry[] m_Karma;

        public FameEntry(int fame, KarmaEntry[] karma)
        {
            m_Fame = fame;
            m_Karma = karma;
        }
    }

    public class KarmaEntry
    {
        public int m_Karma;
        public string m_Title;

        public KarmaEntry(int karma, string title)
        {
            m_Karma = karma;
            m_Title = title;
        }
    }
}