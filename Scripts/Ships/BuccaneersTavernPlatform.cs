/***************************************************************************
 *                         BuccaneersTavernPlatform.cs
 *                            ------------------
 *   begin                : August 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Custom.Pirates;
using Server.Guilds;

namespace Server.Multis
{
    public class BuccaneersDenPlatform : BaseMulti
    {
        public int EventRange { get { return 4; } }

        [Constructable]
        public BuccaneersDenPlatform()
            : base(0x48)
        {
            BuccaneersTavernControl.m_BuccaneersDenPlatform = this;
        }

        public void OnEnter(Mobile m)
        {
            ItemID = 0x49;
            BuccaneersTavernControl.PlatformOnEnter(m);
        }

        public void OnExit(Mobile m)
        {
            ItemID = 0x48;
        }

        public override bool HandlesOnMovement { get { return true; } }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            bool inOldRange = Utility.InRange(oldLocation, Location, EventRange);
            bool inNewRange = Utility.InRange(m.Location, Location, EventRange);

            if (inNewRange && !inOldRange)
                OnEnter(m);
            else if (inOldRange && !inNewRange)
                OnExit(m);
        }
        public BuccaneersDenPlatform(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((DateTime)BuccaneersTavernControl.m_ControlStartTime);
            writer.Write((bool)BuccaneersTavernControl.m_Controlled);
            writer.WriteGuild((Guild)BuccaneersTavernControl.m_ControllingGuild);
            writer.Write((int)BuccaneersTavernControl.m_CurrentControlLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            BuccaneersTavernControl.m_ControlStartTime = reader.ReadDateTime();
            BuccaneersTavernControl.m_Controlled = reader.ReadBool();
            BuccaneersTavernControl.m_ControllingGuild = (Guild)reader.ReadGuild();
            BuccaneersTavernControl.m_CurrentControlLevel = reader.ReadInt();

            if (BuccaneersTavernControl.m_Controlled)
                BuccaneersTavernControl.StartTimer();

            BuccaneersTavernControl.m_BuccaneersDenPlatform = this;
        }
    }
}