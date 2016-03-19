using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class UOACZBile : Item
	{
        public Mobile m_Owner;

        public DateTime m_Expiration = DateTime.MaxValue;
        public TimeSpan Duration = TimeSpan.FromSeconds(30);
        
        public int m_MinDamage = 3;
        public int m_MaxDamage = 5;

        public int m_HitSound = 0x230;
        
        public Timer m_Timer;

        public bool m_UndeadBased = true;
        
		[Constructable]
        public UOACZBile(Mobile owner): base(Utility.RandomList(4651, 4652, 4653, 4654))		
		{            
            Name = "bile";

            Hue = 1369;

            Movable = false;

            m_Owner = owner;
            m_Expiration = DateTime.UtcNow + Duration;

            PlayerMobile playerOwner = m_Owner as PlayerMobile;

            Effects.PlaySound(Location, Map, m_HitSound);

            m_Timer = new InternalTimer(this, m_Owner);
            m_Timer.Start();
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public UOACZBile(Serial serial): base(serial)
		{	
		}

        public override bool OnMoveOver(Mobile mobile)
        {
            if (!UOACZSystem.IsUOACZValidMobile(mobile))
                return true;

            mobile.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);
            mobile.PlaySound(m_HitSound);

            Poison poison = Poison.GetPoison(Utility.RandomMinMax(0, 1));
            mobile.ApplyPoison(m_Owner, poison);
           
            return true;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
                m_Timer.Stop();
        }    

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version   

            writer.Write(m_Owner);
            writer.Write(m_Expiration);
            writer.Write(m_UndeadBased);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            if (version >= 0)
            {
                m_Owner = reader.ReadMobile();
                m_Expiration = reader.ReadDateTime();
                m_UndeadBased = reader.ReadBool();
            }

            m_Timer = new InternalTimer(this, m_Owner);
            m_Timer.Start();
		}

		private class InternalTimer : Timer
		{
            public UOACZBile m_UOACZBile;
            public Mobile m_Owner;

            public static Queue m_Queue = new Queue();

            public InternalTimer(UOACZBile UOACZBile, Mobile owner): base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
			{
				Priority = TimerPriority.TwoFiftyMS;

                m_UOACZBile = UOACZBile;
                m_Owner = owner;
			}

			protected override void OnTick()
			{
                if (m_UOACZBile == null)
                {
                    Stop();
                    return;
                }

                if (m_UOACZBile.Deleted)
                {
                    Stop();
                    return;
                }

                if (DateTime.UtcNow >= m_UOACZBile.m_Expiration)
                {
                    Stop();
                    m_UOACZBile.Delete();

                    return;
                }

                bool foundAnother = false;

                IPooledEnumerable itemsOnTile = m_UOACZBile.GetItemsInRange(0);

                foreach (Item item in itemsOnTile)
                {
                    if (item == m_UOACZBile)
                        continue;

                    if (item is UOACZBile)
                    {
                        foundAnother = true;
                        break;
                    }
                }

                itemsOnTile.Free();

                if (foundAnother)
                {
                    Stop();
                    m_UOACZBile.Delete();

                    return;
                }
                
                IPooledEnumerable mobilesOnTile = m_UOACZBile.GetMobilesInRange(0);

                foreach (Mobile mobile in mobilesOnTile)
                {
                    if (!UOACZSystem.IsUOACZValidMobile(mobile))
                        continue;                    
                }

                mobilesOnTile.Free();

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    mobile.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);
                    mobile.PlaySound(m_UOACZBile.m_HitSound);

                    Poison poison = Poison.GetPoison(Utility.RandomMinMax(0, 1));
                    mobile.ApplyPoison(m_Owner, poison);
                }         
			}
		}
	}
}