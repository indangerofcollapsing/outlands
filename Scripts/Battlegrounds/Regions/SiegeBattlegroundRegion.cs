using Server.Items;
using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Server.Custom.Battlegrounds.Games;
using Server.Custom.Battlegrounds.Mobiles;

namespace Server.Custom.Battlegrounds.Regions
{
    class SiegeBattlegroundRegion : BattlegroundRegion
    {
        public SiegeBattlegroundRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
        }

        

        public override bool AllowBeneficial(Mobile from, Mobile target)
        {
            if (target is BattlegroundKing)
                return false;

            return base.AllowBeneficial(from, target);
        }

        private PlayerMobile FindPlayerOwner(BaseCreature mob)
        {
            PlayerMobile player = null;

            if (mob.Summoned && mob.SummonMaster != null)
            {
                if (mob.SummonMaster is PlayerMobile)
                    player = mob.SummonMaster as PlayerMobile;
            }

            else if (mob.Controlled && mob.ControlMaster != null)
            {
                if (mob.ControlMaster is PlayerMobile)
                    player = mob.ControlMaster as PlayerMobile;
            }

            else if (mob.BardProvoked && mob.BardMaster != null)
            {
                if (mob.BardMaster is PlayerMobile)
                    player = mob.BardMaster as PlayerMobile;
            }

            return player;
        }

        public override bool AllowHarmful(Mobile from, Mobile target)
        {
            var creature = from as BaseCreature;

            PlayerMobile player;

            if (creature != null)
                player = FindPlayerOwner(creature);
            else
                player = from as PlayerMobile;

            if (player == null)
                return true;

            // prevent defense from attacking king
            if (((SiegeBattleground)Battleground).Defense.Contains(player) && target is BattlegroundKing)
                return false;

            return base.AllowHarmful(from, target);
        }
    }
}
