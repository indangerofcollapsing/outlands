using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class Acid : Item
	{
        public int ticks = 0;
        public int maxTicks = 30;

        public int damage = 5;
        
		[Constructable]
        public Acid(): this(Utility.RandomList(4651, 4652, 4653, 4654))		
		{
            Hue = 1369;
            Name = "acid";
		}

		[Constructable]
		public Acid( int itemID ) : base( itemID )
		{
			Movable = false;

			new InternalTimer( this ).Start();
		}

        public Acid(Serial serial): base(serial)
		{
			new InternalTimer( this ).Start();
		}

        public override bool OnMoveOver(Mobile mobile)
        {   
            if (!mobile.CanBeDamaged())
                return true;

            if (mobile is Maggot || mobile is Entrail || mobile is DiseasedViscera || mobile is AcidElemental || mobile is ElderAcidElemental ||
                mobile is ToxicElemental || mobile is ElderToxicElemental || mobile is PoisonElemental || mobile is ElderPoisonElemental ||
                mobile is HalloweenPossessedPumpkin || mobile is CorrosiveSlime)          
                
            {
                return true;
            }
            
            AOS.Damage(mobile, damage, 0, 100, 0, 0, 0);

            Effects.PlaySound(mobile.Location, mobile.Map, 0x230);
            mobile.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);
            
            mobile.SendMessage("Acid burns your skin!");

            return true;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version

            writer.Write(ticks);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            ticks = reader.ReadInt();
		}

		private class InternalTimer : Timer
		{
			private Acid m_Acid;
            private static Queue m_Queue = new Queue();

			public InternalTimer( Acid acid ) : base( TimeSpan.FromSeconds(0), TimeSpan.FromSeconds( 0.5 ) )
			{
				Priority = TimerPriority.OneSecond;
                m_Acid = acid;
			}

			protected override void OnTick()
			{
                if (m_Acid == null)
                    Stop();

                if (m_Acid.Deleted || m_Acid.ticks >= m_Acid.maxTicks)
                {
                    m_Acid.Delete();
                    Stop();
                }

                else
                {
                    IPooledEnumerable eable = m_Acid.GetMobilesInRange(0);

                    foreach (Mobile mobile in eable)
                    {
                        if (!mobile.CanBeDamaged()) 
                            continue;

                        if (mobile is Maggot || mobile is Entrail || mobile is DiseasedViscera || mobile is AcidElemental || mobile is ElderAcidElemental ||
                            mobile is ToxicElemental || mobile is ElderToxicElemental || mobile is PoisonElemental || mobile is ElderPoisonElemental ||
                            mobile is HalloweenPossessedPumpkin || mobile is CorrosiveSlime)
                        {
                            continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    eable.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        AOS.Damage(mobile, m_Acid.damage, 0, 100, 0, 0, 0);

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x230);
                        mobile.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);

                        mobile.SendMessage("Acid burns your skin!");
                    }
                }

                m_Acid.ticks++;
			}
		}
	}
}