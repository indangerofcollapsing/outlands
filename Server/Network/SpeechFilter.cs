using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Server.Network
{
    static public class SpeechFilter
    {
        private static List<WordPair> WordPairs;
        private const String fileName = "SpeechFilter.csv";
		private static bool m_Disabled = true;

        public struct WordPair: IComparable<WordPair>
        {
            public String Replace;
            public String With;

            public WordPair(String replace, String with)
            {
                Replace = replace.ToLower();
                With = with.ToLower();
            }

            public int CompareTo(WordPair wp)
            {
                return wp.Replace.Length.CompareTo(this.Replace.Length);
            }
                
        }

        public static void Filter(ref StringBuilder str)
        {
			if (m_Disabled)
				return;

            int r;
            if (WordPairs != null)
            {
                foreach (WordPair wp in WordPairs)
                {
                    int i = 0;
                    r = 0;

                    int spacecnt = 0;

                    while (r < str.Length)
                    {
                        char repl = wp.Replace[i];
                        char eval = str[r];

                        if (eval == ' ')
                        {
                            r++;
                            if (i != 0)
                                spacecnt++;
                            continue;
                        }

                        if ((int)eval > 64 && (int)eval < 91)
                            eval = (char)((int)eval + 32);

                        if (repl == eval ||
                            repl == 'i' && eval == 'l' ||
                            repl == 'i' && eval == '1' ||
                            repl == 'o' && eval == '0' ||
                            repl == 's' && eval == '$' ||
                            repl == 'i' && eval == '!' ||
                            repl == 'i' && eval == '|' ||
                            repl == 'l' && eval == '|' ||
                            repl == 't' && eval == '+' ||
                            repl == 's' && eval == '5' ||
                            repl == 'c' && eval == '(' ||
                            repl == 'c' && eval == '[' ||
                            repl == 'g' && eval == '6' ||
                            repl == 'e' && eval == '3' ||
                            repl == 'a' && eval == '4' ||
                            repl == 'l' && eval == '1')
                        {
                            if (i == 0)
                                spacecnt = 0;

                            if (i == wp.Replace.Length - 1)
                            {
                                //r = r - wp.Replace.Length + 1;
                                r -= (wp.Replace.Length + spacecnt - 1);
                                for (i = 0; i < wp.Replace.Length && i < wp.With.Length; i++)
                                    str[r + i] = wp.With[i];

                                i = 0;
                                r += wp.With.Length;
                                int remove = wp.Replace.Length - wp.With.Length + spacecnt;
                                if (remove > 0)
                                    str.Remove(r, remove);
                                //if (spacecnt!=0)
                                //    str.Remove(r-- - spacecnt, spacecnt);
                            }
                            else
                                i++;
                        }
                        else
                            i = 0;
                        r++;
                    }
                }
            }

            r = 0;
            while (r < str.Length - 2)
            {
                if (str[r++] == (char)32)
                    if (str[r] == (char)32)
                        str.Remove(--r, 1);
            }

            if (str.Length > 0)
                if (str[0] == (char)32)
                    str.Remove(0, 1);

            if (str.Length > 0)
                if (str[str.Length - 1] == (char)32)
                    str.Remove(str.Length - 1, 1);
        }

        public static void Initialize()
        {

            if (!File.Exists(fileName))
            {
                Console.WriteLine("SpeechFilter: Could not find '{0}'.", fileName);
                WordPairs = new List<WordPair>(0);
                return;
            }

            StreamReader sr = new StreamReader(fileName);

            WordPairs = new List<WordPair>();

            while (!sr.EndOfStream)
            {
                String[] split = sr.ReadLine().Split(',');

                if (split.Length != 2)
                    continue;

                if (split[0].Replace(" ", "").Length == 0)
                    continue;

                String replace = split[0].Replace(" ", "");
                String with = split[1].Replace(" ", "");

                if (replace.Length < with.Length)
                    with = with.Substring(0, replace.Length);

                WordPairs.Add(new WordPair(replace, with));

            }

            sr.Close();
            sr.Dispose();

            Console.WriteLine("SpeechFilter: Loaded {0} word pairs from '{1}'.", WordPairs.Count, fileName);

            WordPairs.Sort();

        }
    }
}
