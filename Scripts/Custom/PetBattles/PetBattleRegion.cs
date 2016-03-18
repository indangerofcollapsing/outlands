using System;
using Server;
using Server.Spells.Second;
using Server.Custom;

namespace Server.Regions
{
    public class PetBattleRegion : DungeonRegion
    {
        public override bool IsBlessedRegion { get { return true; } }

        public PetBattleRegion(string name, Map map, int priority, Rectangle2D[] area)
            : base(name, map, priority, area) 
        {
            //Region = this;
        }

        public override void OnDeath(Mobile m)
        {
            base.OnDeath(m);
        }

        public override bool OnDamage(Mobile m, ref int Damage) 
        {   
            return base.OnDamage(m, ref Damage);
        }
    }
}