using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;
using Server;
using Server.Achievements;
using Server.Commands;
using Server.Mobiles;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Server.Misc
{
    public class NameVerification
    {
        public static readonly char[] SpaceDashPeriodQuote = new char[]
			{
				' ', '-', '.', '\''
			};

        public static readonly char[] Empty = new char[0];

        public static void Initialize()
        {
            CommandSystem.Register("ValidateName", AccessLevel.Administrator, new CommandEventHandler(ValidateName_OnCommand));
            CommandSystem.Register("ValidateAllNames", AccessLevel.Administrator, new CommandEventHandler(ValidateAllNames_OnCommand));
        }

        [Usage("ValidateName")]
        [Description("Checks the result of NameValidation on the specified name.")]
        public static void ValidateName_OnCommand(CommandEventArgs e)
        {
            if (Validate(e.ArgString, 2, 16, true, false, true, 1, SpaceDashPeriodQuote))
                e.Mobile.SendMessage(0x59, "That name is considered valid.");
            else
                e.Mobile.SendMessage(0x22, "That name is considered invalid.");
        }

        [Usage("ValidateAllNames")]
        [Description("Checks the result of NameValidation on the every world name.")]
        public static void ValidateAllNames_OnCommand(CommandEventArgs e)
        {
            Dictionary<string, List<PlayerMobile>> table = new Dictionary<string, List<PlayerMobile>>(World.Mobiles.Values.Count);
            foreach (Mobile m in World.Mobiles.Values)
            {
                if (!(m is PlayerMobile))
                    continue;

                string rawName = m.RawName.ToLower().Trim();

                if (rawName == "generic player")
                    continue;

                if (m.AccessLevel > AccessLevel.Player)
                    continue;

                m.RawName = "testtest1233";

                if (!NameVerification.Validate(rawName, 2, 16, true, true, true, 1, NameVerification.SpaceDashPeriodQuote))
                {
                    m.RawName = "testtest1234";
                    Console.WriteLine(rawName);
                    if (table.ContainsKey(rawName))
                    {
                        List<PlayerMobile> players = table[rawName];
                        players.Add(m as PlayerMobile);
                    }
                    else
                    {
                        List<PlayerMobile> newList = new List<PlayerMobile>();
                        newList.Add(m as PlayerMobile);
                        table.Add(rawName, newList);
                    }
                }
                else
                {
                    m.RawName = rawName;
                }

                Queue<PlayerMobile> toRename = new Queue<PlayerMobile>();

                foreach (KeyValuePair<string, List<PlayerMobile>> entry in table)
                {
                    var list = entry.Value;

                    if (list.Count == 0)
                        continue;
                    else if (list.Count == 1)
                        toRename.Enqueue(list[0]);
                    else
                    {
                        list.Sort(new Comparison<PlayerMobile>(SortByGameTime));

                        for (int i = 1; i < list.Count; i++)
                            toRename.Enqueue(list[i]);
                    }
                }

                while (toRename.Count > 0)
                {
                    PlayerMobile pm = toRename.Dequeue();
                    pm.RawName = "Generic Player";
                }

                toRename.Clear();
            }
        }

        public static int SortByGameTime(PlayerMobile m1, PlayerMobile m2)
        {
            return m2.GameTime.CompareTo(m1.GameTime);
        }

        public static bool Validate(string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, bool validateAgainstPlayers = true)
        {
            return Validate(name, minLength, maxLength, allowLetters, allowDigits, noExceptionsAtStart, maxExceptions, exceptions, m_Disallowed, m_StartDisallowed, validateAgainstPlayers);
        }

        public static bool Validate(string name, int minLength, int maxLength, bool allowLetters, bool allowDigits, bool noExceptionsAtStart, int maxExceptions, char[] exceptions, string[] disallowed, string[] startDisallowed, bool validateAgainstPlayers = true)
        {
            if (name == null || name.Length < minLength || name.Length > maxLength)
                return false;

            int exceptCount = 0;

            name = name.ToLower();

            if (!allowLetters || !allowDigits || (exceptions.Length > 0 && (noExceptionsAtStart || maxExceptions < int.MaxValue)))
            {
                for (int i = 0; i < name.Length; ++i)
                {
                    char c = name[i];

                    if (c >= 'a' && c <= 'z')
                    {
                        if (!allowLetters)
                            return false;

                        exceptCount = 0;
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        if (!allowDigits)
                            return false;

                        exceptCount = 0;
                    }
                    else
                    {
                        bool except = false;

                        for (int j = 0; !except && j < exceptions.Length; ++j)
                            if (c == exceptions[j])
                                except = true;

                        if (!except || (i == 0 && noExceptionsAtStart))
                            return false;

                        if (exceptCount++ == maxExceptions)
                            return false;
                    }
                }
            }

            //Check for reserved name
            var reservedNames = ReservedNames.ToArray();
            for (int i = 0; i < reservedNames.Length; i++)
            {
                int indexOf = name.IndexOf(reservedNames[i]);

                if (indexOf == -1)
                    continue;

                bool badPrefix = (indexOf == 0);

                for (int j = 0; !badPrefix && j < exceptions.Length; ++j)
                    badPrefix = (name[indexOf - 1] == exceptions[j]);

                if (!badPrefix)
                    continue;

                bool badSuffix = ((indexOf + reservedNames[i].Length) >= name.Length);

                for (int j = 0; !badSuffix && j < exceptions.Length; ++j)
                    badSuffix = (name[indexOf + reservedNames[i].Length] == exceptions[j]);

                if (badSuffix)
                    return false;
            }

            for (int i = 0; i < disallowed.Length; ++i)
            {
                int indexOf = name.IndexOf(disallowed[i]);

                if (indexOf == -1)
                    continue;

                bool badPrefix = (indexOf == 0);

                for (int j = 0; !badPrefix && j < exceptions.Length; ++j)
                    badPrefix = (name[indexOf - 1] == exceptions[j]);

                if (!badPrefix)
                    continue;

                bool badSuffix = ((indexOf + disallowed[i].Length) >= name.Length);

                for (int j = 0; !badSuffix && j < exceptions.Length; ++j)
                    badSuffix = (name[indexOf + disallowed[i].Length] == exceptions[j]);

                if (badSuffix)
                    return false;
            }

            for (int i = 0; i < startDisallowed.Length; ++i)
            {
                if (name.StartsWith(startDisallowed[i]))
                    return false;
            }

            string mname = null;
            string nname = name;

            if (validateAgainstPlayers)
            {
                foreach (Mobile m in World.Mobiles.Values)
                {
                    PlayerMobile pm = m as PlayerMobile;
                    if (pm != null && (nname != null && nname != "" && pm.RawName != null && pm.RawName != ""))
                    {
                        mname = pm.RawName.ToLower();
                        if (mname != null && mname != "") if (nname == mname) return false;
                    }
                }
            }


            return true;
        }

        public static string[] StartDisallowed { get { return m_StartDisallowed; } }
        public static string[] Disallowed { get { return m_Disallowed; } }

        private static readonly string[] m_StartDisallowed = new string[]
			{
				"seer",
				"counselor",
				"gm",
				"admin",
				"lady",
				"lord",
                "dev",
                "developer"
			};

        private static readonly string[] m_Disallowed = new string[]
			{
				"jigaboo",
				"chigaboo",
				"wop",
				"kyke",
				"kike",
				"tit",
				"spic",
				"prick",
				"piss",
				"lezbo",
				"lesbo",
				"felatio",
				"dyke",
				"dildo",
				"chinc",
				"chink",
				"cunnilingus",
				"cum",
				"cocksucker",
				"cock",
				"clitoris",
				"clit",
				"ass",
				"hitler",
				"penis",
				"nigga",
				"nigger",
				"klit",
				"kunt",
				"jiz",
				"jism",
				"jerkoff",
				"jackoff",
				"goddamn",
				"fag",
				"blowjob",
				"bitch",
				"asshole",
				"dick",
				"pussy",
				"snatch",
				"cunt",
				"twat",
				"shit",
				"fuck",
				"tailor",
				"smith",
				"scholar",
				"rogue",
				"novice",
				"neophyte",
				"merchant",
				"medium",
				"master",
				"mage",
				"journeyman",
				"grandmaster",
				"fisherman",
				"expert",
				"chef",
				"carpenter",
				"british",
				"blackthorne",
				"blackthorn",
				"beggar",
				"archer",
				"apprentice",
				"adept",
				"gamemaster",
				"frozen",
				"squelched",
				"invulnerable",
				"paladin",
				"vanquisher",
				"crusader of light",
				"defender of trinsic",
				"trinsic",
				"azaroth",
				"sean",
				"assholeroth",
				"azzholeroth",
				"azholeroth",
				"azsholeroth",
				"aszholeroth",
				"homo",
				"tarbaby",
				"jew",
				"king",
				"queen",
				"tilly",
				"avatar",
				"queef",
				"suicide",
				"cancer",
                "seneschal",
                "ipy",
                "in por ylem",
                "uoac",
                "uo an corp",
                "uo an corp",
				"puppz",
				"abigor",
			};
    }

    static class ReservedNames
    {
        public static HashSet<string> Names = new HashSet<string>();


        public static void Initialize()
        {
            new ReservedNamesPersistence();

            CommandSystem.Register("ReserveName", AccessLevel.Administrator, ReserveName);
            CommandSystem.Register("UnreserveName", AccessLevel.Administrator, UnreserveName);
            CommandSystem.Register("CheckName", AccessLevel.GameMaster, CheckName);

        }


        public static void ReserveName(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0 || string.IsNullOrEmpty(e.Arguments[0]))
            {
                e.Mobile.SendMessage(0x22, String.Format("Please specify the name."));
                return;
            }

            var key = e.Arguments[0].ToLower();

            if (!NameVerification.Validate(key, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote))
            {
                e.Mobile.SendMessage(0x22, String.Format("The name \"{0}\" is invalid.", e.Arguments[0]));
            }
            else if (e.Mobile is PlayerMobile && !Names.Contains(key))
            {
                Names.Add(key);
                e.Mobile.SendMessage(0x59, String.Format("The name \"{0}\" has been reserved.", e.Arguments[0]));
            }
            else
            {
                e.Mobile.SendMessage(0x22, String.Format("The name \"{0}\" has already been reserved.", e.Arguments[0]));
            }
        }

        public static void UnreserveName(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0 || string.IsNullOrEmpty(e.Arguments[0]))
            {
                e.Mobile.SendMessage(0x22, String.Format("Please specify the name."));
                return;
            }

            var key = e.Arguments[0].ToLower();
            if (Names.Contains(key))
            {
                Names.Remove(key);
                e.Mobile.SendMessage(0x59, String.Format("The name \"{0}\" has been unreserved.", e.Arguments[0]));
            }
            else
            {
                e.Mobile.SendMessage(0x22, String.Format("The name \"{0}\" is not reserved.", e.Arguments[0]));
            }
        }

        public static void CheckName(CommandEventArgs e)
        {
            if (e.Arguments.Length == 0 || string.IsNullOrEmpty(e.Arguments[0]))
            {
                e.Mobile.SendMessage(0x22, String.Format("Please specify the name."));
                return;
            }

            var key = e.Arguments[0].ToLower();
            if (IsThisNameReserved(key))
                e.Mobile.SendMessage(0x22, String.Format("The name \"{0}\" is reserved.", e.Arguments[0]));
            else
                e.Mobile.SendMessage(0x59, String.Format("The name \"{0}\" is available.", e.Arguments[0]));
        }

        public static bool IsThisNameReserved(string name)
        {
            return Names.Contains(name);
        }

        public static string[] ToArray()
        {
            return Names.ToArray();
        }
    }

    public class ReservedNamesPersistence : Item
    {
        public override string DefaultName { get { return "Reserved Names Persistence - Internal"; } }
        private static ReservedNamesPersistence m_Instance;

        public ReservedNamesPersistence()
            : base(0)
        {
            Movable = false;
            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public ReservedNamesPersistence(Serial serial)
            : base(serial)
        {

        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
            writer.Write(ReservedNames.Names.Count);
            foreach (var name in ReservedNames.Names)
            {
                writer.Write(name);
            }

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        int nameCount = reader.ReadInt();
                        for (var i = 0; i < nameCount; i++)
                        {
                            var name = reader.ReadString();
                            ReservedNames.Names.Add(name);
                        }
                    } break;
            }

            m_Instance = this;
        }
    }
}