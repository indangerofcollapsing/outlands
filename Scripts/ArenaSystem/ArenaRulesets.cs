using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.ArenaSystem
{
    public abstract class ArenaRuleset
    {
    }
    public class IPY3ArenaRuleset : ArenaRuleset
    {
        public IPY3ArenaRuleset() { }
    }
    public class Pub16ArenaRuleset : ArenaRuleset
    {
        public Pub16ArenaRuleset() { }
    }
    public class T2AArenaRuleset : ArenaRuleset
    {
        public T2AArenaRuleset() { }
    }
}
