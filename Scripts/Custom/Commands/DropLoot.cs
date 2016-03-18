using System;
using System.Reflection;
using Server.Items;
using Server.Targeting;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells;
using Server.Custom;

namespace Server.Commands
{
    public class DropLoot
    {
        public static void Initialize()
        {
        }

        public class DropLootTarget : Target
        {
            private int m_Amount;
            private Mobile m_Mob;
            private int m_Flag;

            public DropLootTarget(Mobile m, int flag)
                : base(15, false, TargetFlags.None)
            {
                m_Mob = m;
                m_Flag = flag;

            }


            public Mobile SelectWinner(Mobile m, int flag)
            {
                Mobile winner = null;
                if (m == null)
                    return winner;

                int wintype;
                int winner_index;

                if (m_Flag == 3) //if m_Flag is 3, then no arguments were selected (or random was manually selected).
                    wintype = Utility.Random(3);
                else
                    wintype = flag;

                if (wintype == 0)
                {
                    return Utilities.FindPlayerDamagerFromKiller(m.FindMostRecentDamager(false));
                }

                else if (wintype == 1)
                {
                    return m.FindMostTotalDamger(false);
                }

                else
                {
                    for (int i = 0; i < 10; i++)
                    {
                        winner_index = Utility.RandomMinMax(0, m.DamageEntries.Count - 1);
                        if (winner_index >= 0 && m.DamageEntries.Count > 0)
                        {
                            var player = Utilities.FindPlayerDamagerFromKiller(m.DamageEntries[winner_index].Damager);
                            if (player.AccessLevel == AccessLevel.Player)
                                return player;
                        }
                    }
                    

                }
                return winner;

            }


            protected override void OnTarget(Mobile from, object targ)
            {
                bool done = false;
                if (!(targ is Item))
                {
                    from.SendMessage("You can only gift items.");
                    return;
                }

                CommandLogging.WriteLine(from, "{0} {1} gifting {2})", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targ));

                Item copy = (Item)targ;
                Container pack;
                Mobile m_Winner = null;

               
                    m_Winner = SelectWinner(m_Mob, m_Flag);
              
                    if (m_Winner == null)
                {
                    from.SendMessage("No mobile to dop");
                    return;
                }

                if (m_Winner != null)
                    pack = m_Winner.Backpack;

                else
                    pack = from.Backpack;


                Type t = copy.GetType();


                ConstructorInfo c = t.GetConstructor(Type.EmptyTypes);

                if (c != null)
                {
                    try
                    {
                        from.SendMessage("Gifting");
                        object o = c.Invoke(null);
                        if (o != null && o is Item)
                        {
                            Item newItem = (Item)o;
                            CopyProperties(newItem, copy);
                            copy.OnAfterDuped(newItem);
                            newItem.Parent = null;


                            if (pack != null)
                                pack.DropItem(newItem);
                            else
                            {
                                from.SendMessage("Pack is null");
                            }
                            newItem.InvalidateProperties();

                            CommandLogging.WriteLine(from, "{0} {1} Gifted {2} creating {3}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targ), CommandLogging.Format(newItem));
                        }

                        from.SendMessage("Done");
                        done = true;
                        if (pack != null)
                        {
                            World.Broadcast(256, true, "{0} has received a special event reward!", ((Mobile)pack.Parent).Name);
                         }
                    }
                    catch
                    {
                        from.SendMessage("Error!");
                        return;
                    }
                }

                if (!done)
                {
                    from.SendMessage("Unable to Drop Loot.  Item must have a 0 parameter constructor.");
                }
            }
        }



        public static void CopyProperties(Item dest, Item src)
        {
            PropertyInfo[] props = src.GetType().GetProperties();

            for (int i = 0; i < props.Length; i++)
            {
                try
                {
                    if (props[i].CanRead && props[i].CanWrite)
                    {
                        //Console.WriteLine( "Setting {0} = {1}", props[i].Name, props[i].GetValue( src, null ) );
                        props[i].SetValue(dest, props[i].GetValue(src, null), null);
                    }
                }
                catch
                {
                    //Console.WriteLine( "Denied" );
                }
            }
        }
    }
}
