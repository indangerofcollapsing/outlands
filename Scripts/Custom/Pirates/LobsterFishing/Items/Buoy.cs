/***************************************************************************
 *                                 Buoy.cs
 *                            ------------------
 *   begin                : February 2011
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Network;

namespace Server.Items
{
	public class LobsterBuoy : Item, ITelekinesisable
	{
        public override string DefaultName { get { return "buoy"; } }
        public LobsterTrap Trap { get; private set; }
        public Mobile Trapper { get; set; }

        public static readonly int AnimationID = 0x44CB;
        public static readonly int RegularID = 0x44CC;
        public static readonly TimeSpan AnimationTime = TimeSpan.FromSeconds(3);

        public LobsterBuoy(LobsterTrap trap) : base(RegularID)
		{
            Movable = false;
            Trap = trap;
		}

        public void Bob()
        {
            PublicOverheadMessage(Network.MessageType.Regular, 0x0, false, "**bob**");

            ItemID = AnimationID;
            Timer.DelayCall(AnimationTime, EndAnimation);
        }

        public void EndAnimation()
        {
            if (Deleted)
                return;

            ItemID = RegularID;
        }

        void ITelekinesisable.OnTelekinesis(Mobile from)
        {
            Use(from);
        }

		public override void OnDoubleClick( Mobile from )
		{
            if (!from.InRange(Location, 3))
                from.SendLocalizedMessage(500618); //That is too far away.
            else
                Use(from);
		}

        public void Use(Mobile from)
        {
            Trap.EndHarvest(from);
        }

		public LobsterBuoy( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromTicks(1), Delete);
		}
	}
}