using System;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Spells;

namespace Server.Items
{
	public class StreamerFountainAddon : BaseAddon
	{
        private static int[,] m_AddOnSimpleComponents = new int[,] 
        {
            //Fountain
			{17317, 0, 0, 0, 0},
            {17298, 1, 0, 0, 0},
            {17294, 2, 0, 0, 0},
            {17298, 3, 0, 0, 0},
            {17302, 4, 0, 0, 0},
            {17310, 4, 1, 0, 0},

            //{17310, 4, 2, 0, 0},

            {17310, 4, 3, 0, 0},
            {17286, 4, 4, 0, 0},
            {17306, 3, 4, 0, 0},
            {17306, 2, 4, 0, 0},
            {17306, 1, 4, 0, 0},
            {17290, 0, 4, 0, 0},
            {17282, 0, 3, 0, 0},
            {17278, 0, 2, 0, 0},
            {17282, 0, 1, 0, 0},

            //Water
            {6711, 1, 1, 4, 2580},
            {6769, 2, 1, 4, 2580},
            {6723, 3, 1, 4, 2580},
            {6723, 1, 2, 4, 2580},
            {6769, 2, 2, 4, 2580},
            {6765, 3, 2, 4, 2580},
            {6719, 1, 3, 4, 2580},
            {6719, 2, 3, 4, 2580},
            {6723, 3, 3, 4, 2580},
		};
            
		public override BaseAddonDeed Deed { get { return new StreamerFountainAddonDeed(); }}

        public DateTime NextFountainSound = DateTime.UtcNow + FountainSoundInterval;
        public static TimeSpan FountainSoundInterval = TimeSpan.FromSeconds(8);
        
        private static Timer m_Timer;        

		[ Constructable ]
		public StreamerFountainAddon()
		{
            for (int i = 0; i < m_AddOnSimpleComponents.Length / 5; i++)
                AddComponent(new AddonComponent(m_AddOnSimpleComponents[i, 0], m_AddOnSimpleComponents[i, 4]), m_AddOnSimpleComponents[i, 1], m_AddOnSimpleComponents[i, 2], m_AddOnSimpleComponents[i, 3]);

            RestartFountainTimer();
        }

        public StreamerFountainAddon(Serial serial): base(serial)
		{
        }

        public override void OnComponentUsed(AddonComponent c, Mobile from)
        {
            base.OnComponentUsed(c, from);

            if (!from.Alive)
                return;

            if (!from.CanBeginAction(typeof(StreamerFountainAddon)))
            {
                from.SendMessage("You must wait a few moments before attempting to use that again.");
                return;
            }

            from.BeginAction(typeof(StreamerFountainAddon));

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (from != null)
                    from.EndAction(typeof(StreamerFountainAddon));
            });

            for (int a = 0; a < 3; a++)
            {
                TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 5);
                water.Name = "water";
                water.Hue = 2580;

                Point3D waterLocation = new Point3D(c.X + Utility.RandomList(-1, 1), c.Y + Utility.RandomList(-1, 1), c.Z);

                SpellHelper.AdjustField(ref waterLocation, Map, 12, false);
                waterLocation.Z++;

                water.MoveToWorld(waterLocation, Map);
            }

            Effects.PlaySound(c.Location, c.Map, Utility.RandomList(0x027, 0x026, 0x025));
        }

        public override void OnLocationChange(Point3D oldLoc)
        {
            base.OnLocationChange(oldLoc);

            RestartFountainTimer();
        }

        public override void OnMapChange()
        {
            base.OnMapChange();

            RestartFountainTimer();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            RestartFountainTimer();
        }

        public void FountainSound()
        {            
            IEntity startLocation = new Entity(Serial.Zero, new Point3D(Location.X + 2, Location.Y + 3, Location.Z + 1), Map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(Location.X + 2, Location.Y + 1, Location.Z + 1), Map);
            
            Effects.SendMovingParticles(startLocation, endLocation, 5364, 30, 50, false, false, 0, 0, 9501, 0, 0, 0);
            
            IPooledEnumerable nearbyMobiles = Map.GetMobilesInRange(Location, 8);           

            Queue m_Queue = new Queue();

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile is BaseCreature) continue;
                if (mobile == null) continue;
                if (mobile.Deleted) continue;
                if (mobile.Map != Map) continue;

                bool inLOS = false;
                bool above = false;
                bool below = false;

                if (Map.InLOS(mobile.Location, Location))
                    inLOS = true;

                if ((Location.Z - mobile.Z) >= 0 && (Location.Z - mobile.Z) <= 10)
                    below = true;

                if ((mobile.Z - Location.Z) >= 0 && (mobile.Z - Location.Z) <= 10)
                    above = true;
                
                if (inLOS || above || below)
                    m_Queue.Enqueue(mobile);               
            }

            nearbyMobiles.Free();

            while (m_Queue.Count > 0)
            {
                Mobile mobile = (Mobile)m_Queue.Dequeue();
                mobile.PlaySound(0x011);
            }
        }

        public void RestartFountainTimer()
        {
            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (!Deleted)
            {
                NextFountainSound = DateTime.UtcNow + FountainSoundInterval;

                m_Timer = new InternalTimer(this);
                m_Timer.Start();
            }
        }

        private class InternalTimer : Timer
        {
            private StreamerFountainAddon m_StreamerFountainAddon;

            public InternalTimer(StreamerFountainAddon streamerFountainAddon): base(TimeSpan.Zero, FountainSoundInterval)
            {
                Priority = TimerPriority.OneSecond;

                m_StreamerFountainAddon = streamerFountainAddon;
            }

            protected override void OnTick()
            {
                if (m_StreamerFountainAddon == null)
                {
                    Stop();
                    return;
                }

                if (m_StreamerFountainAddon.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_StreamerFountainAddon.NextFountainSound <= DateTime.UtcNow)
                {
                    m_StreamerFountainAddon.FountainSound();
                    m_StreamerFountainAddon.NextFountainSound = DateTime.UtcNow + FountainSoundInterval;
                }
            }
        }          

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            RestartFountainTimer();
		}
	}

	public class StreamerFountainAddonDeed : BaseAddonDeed
	{
        public override BaseAddon Addon { get { return new StreamerFountainAddon(); } }

		[Constructable]
		public StreamerFountainAddonDeed()
		{
            Name = "a streamer fountain";
		}

		public StreamerFountainAddonDeed( Serial serial ) : base( serial )
        {
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 ); // Version
		}

		public override void	Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}