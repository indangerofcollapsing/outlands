using System;
using Server.Network;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class TrapTriggerTile : Item
    {
        public TinkerTrapPlaced m_TinkerTrapPlaced;

        [Constructable]
        public TrapTriggerTile(): base(1173)
        {
            Movable = false; 
        }

        public TrapTriggerTile(Serial serial) : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_TinkerTrapPlaced == null) return;
            if (m_TinkerTrapPlaced.Deleted) return;
           
            int minutes;
            int seconds;

            string sTimeRemaining = Utility.CreateTimeRemainingString(DateTime.UtcNow, m_TinkerTrapPlaced.Expiration, true, true, true, true, true);
            
            if (from.NetState != null)
            {
                LabelTo(from, Name);

                if (m_TinkerTrapPlaced.Owner != null)
                    LabelTo(from, "(placed by " + m_TinkerTrapPlaced.Owner.RawName + ")");

                LabelTo(from, "[expires in " + sTimeRemaining + "]");
            }
        }

        public override bool OnMoveOver(Mobile mobile)
        {
            if (m_TinkerTrapPlaced == null) return true;
            if (m_TinkerTrapPlaced.Deleted || m_TinkerTrapPlaced.IsResolving) return true;

            BaseCreature bc_Creature = mobile as BaseCreature;
            PlayerMobile player = mobile as PlayerMobile;

            if (m_TinkerTrapPlaced.TrapReady > DateTime.UtcNow)
                return true;

            if (UOACZSystem.IsUOACZValidMobile(mobile))
            {
                bool activate = false;

                if (mobile is UOACZBaseWildlife)
                    activate = true;

                if (mobile is UOACZBaseUndead)
                    activate = true;

                if (player != null)
                {
                    if (player.IsUOACZUndead)
                        activate = true;
                }

                if (activate)                
                    m_TinkerTrapPlaced.Activate();

                return true;
            }

            if (bc_Creature != null)
            {
                if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                    return true;               
               
                m_TinkerTrapPlaced.Activate();                
            }

            return true;
        }

        public override bool CanBeSeenBy(Mobile from)
        {
            if (m_TinkerTrapPlaced == null) return false;
            if (m_TinkerTrapPlaced.Deleted) return false;

            bool canBeSeen = false;

            if (from.AccessLevel > AccessLevel.Player)
                return true;

            PlayerMobile player = from as PlayerMobile;

            if (from.Region is UOACZRegion)
            {
                if (player != null)
                {
                    if (player.IsUOACZUndead)
                        return false;

                    if (player.IsUOACZHuman)
                        return true;
                }
            }

            if (from == m_TinkerTrapPlaced.Owner)
                canBeSeen = true;           

            if (from.Party != null && m_TinkerTrapPlaced.Owner != null)
            {
                if (from.Party == m_TinkerTrapPlaced.Owner.Party)
                    return true;
            }            

            return canBeSeen;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_TinkerTrapPlaced);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_TinkerTrapPlaced = reader.ReadItem() as TinkerTrapPlaced;
            }
        }
    }
}
