using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Regions;
using Server.Mobiles;
using Server.Items;
using Server.YewJail;
using System.Collections;

namespace Server.Custom.YewJail
{
    static class DaemonCavern
    {
        public class DaemonCavernRegion : BaseRegion
        {
            public DaemonCavernRegion(String name, Map map, int priority, Rectangle2D area)
                : base(name, map, priority, area)
            {
            }

            public override bool OnBeforeDeath(Mobile m)
            {
                if (!(m is PlayerMobile))
                    return true;

                YewJailItem i = (YewJailItem)m.Backpack.FindItemByType(typeof(YewJailItem));

                if (i == null)
                {
                    m.MoveToWorld(m_ExitLocation, Map.Felucca);
                    return true;
                }

                i.OnDeath();

                return false;
            }
        }

        public static void Initialize()
        {
            if (m_Region == null)
            {
                m_Region = new DaemonCavernRegion("DaemonCavernRegion", Map.Felucca, 55, m_Area);
                m_Region.Register();
            }

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate
            {
                if (m_Guardian == null || m_Guardian.Deleted)
                {
                    m_Guardian = new DaemonGuardian();
                    m_Guardian.MoveToWorld(m_ExitPoint1, Map.Tokuno);
                }
            });
        }

        private static DaemonCavernRegion m_Region;
        private static DaemonGuardian m_Guardian;
        private static readonly Rectangle2D m_Area = new Rectangle2D(new Point2D(335, 400), new Point2D(450, 550));
        private static readonly Point3D m_PlayerSpawn = new Point3D(398, 548, 15);
        private static readonly Point3D m_ExitPoint1 = new Point3D(423, 411, 0);
        private static readonly Point3D m_ExitLocation = new Point3D(2611, 2227, -20);


        public static void Begin(Mobile m)
        {
            m.MoveToWorld(m_PlayerSpawn, Map.Tokuno);
        }

        public static void RegisterDaemon(Mobile m)
        {
            if (m_Guardian == null || m_Guardian.Deleted)
                m_Guardian = (DaemonGuardian)m;
            else
                m.Delete();
        }
    }


	public class DaemonGuardian : BaseVendor
	{
        private static readonly string m_ExitString = @"In this particular lair of torment, you pay for your sins... quite literally. Collect for me 10 gold pieces for every innocent on your conscience, and I will let you pass.";
        private static readonly Point3D m_ExitLocation = new Point3D(2611, 2227, -20);

		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

		[Constructable]
		public DaemonGuardian() : base( "the Guardian" )
		{
            Name = "daemon guardian";
			SpeechHue = Utility.RandomDyedHue();
            Body = 9;
            BaseSoundID = 357;
            Direction = Server.Direction.South;
            CantWalk = true;
		}

        public override void InitSBInfo()
        {
        }

        public DaemonGuardian(Serial serial)
            : base(serial)
		{
		}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(dropped is Gold))
                return false;

            Gold g = dropped as Gold;

            int amount = g.Amount;
            int req = from.ShortTermMurders * 10;

            if (amount < req)
            {
                Say("This isn't enough! Don't come back until you have 10 gold per murder!");
                return false;
            }

            g.Delete();

            var i = (YewJailItem)from.Backpack.FindItemByType(typeof(YewJailItem));

            if (i != null)
                i.Delete();

            from.MoveToWorld(m_ExitLocation, Map.Felucca);

            return false;
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (InRange(m, 5) && !InRange(oldLocation, 5))
                Say(m_ExitString);

            base.OnMovement(m, oldLocation);
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

            CantWalk = true;
            DaemonCavern.RegisterDaemon(this);
		}
	}
}
