using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class Bile : Item
	{
        public int ticks = 0;
        public int maxTicks = 60;

        public int damage = 10;
        
		[Constructable]
        public Bile(): this(Utility.RandomList(4651, 4652, 4653, 4654))		
		{
            Hue = 1369;
            Name = "diseased bile";
		}

		[Constructable]
		public Bile( int itemID ) : base( itemID )
		{
			Movable = false;

			new InternalTimer( this ).Start();
		}

        public Bile(Serial serial): base(serial)
		{
			new InternalTimer( this ).Start();
		}

        public override bool OnMoveOver(Mobile mobile)
        {   
            if (!mobile.CanBeDamaged() || !mobile.Alive)
                return true;

            if (mobile is Maggot || mobile is Entrail || mobile is DiseasedViscera || mobile is AcidElemental || mobile is ElderAcidElemental ||
                mobile is ToxicElemental || mobile is ElderToxicElemental || mobile is PoisonElemental || mobile is ElderPoisonElemental)
            {
                return true;
            }
            
            AOS.Damage(mobile, damage, 0, 100, 0, 0, 0);

            Effects.PlaySound(mobile.Location, mobile.Map, 0x230);
            mobile.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);
            
            mobile.SendMessage("Foul bile eats away at your flesh!");

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
			private Bile m_Bile;
            private static Queue m_Queue = new Queue();

			public InternalTimer( Bile bile ) : base( TimeSpan.FromSeconds(0), TimeSpan.FromSeconds( 0.5 ) )
			{
				Priority = TimerPriority.OneSecond;
                m_Bile = bile;
			}

			protected override void OnTick()
			{
                if (m_Bile == null)
                    Stop();

                if (m_Bile.Deleted || m_Bile.ticks >= m_Bile.maxTicks)
                {
                    m_Bile.Delete();
                    Stop();
                }

                else
                {
                    var mobilesOnTile = m_Bile.GetMobilesInRange(0);

                    foreach (Mobile mobile in mobilesOnTile)
                    {
                        if (!mobile.CanBeDamaged() || !mobile.Alive) 
                            continue;

                        if (mobile is Maggot || mobile is Entrail || mobile is DiseasedViscera || mobile is AcidElemental || mobile is ElderAcidElemental ||
                            mobile is ToxicElemental || mobile is ElderToxicElemental || mobile is PoisonElemental || mobile is ElderPoisonElemental)
                        {
                            continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    mobilesOnTile.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        AOS.Damage(mobile, m_Bile.damage, 0, 100, 0, 0, 0);

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x230);
                        mobile.FixedParticles(0x374A, 10, 20, 5021, 0x110, 0, EffectLayer.Head);

                        mobile.SendMessage("Foul bile eats away at your flesh!");
                    }
                }

                m_Bile.ticks++;
			}
		}
	}
}