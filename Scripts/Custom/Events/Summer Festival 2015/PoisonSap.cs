using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class PoisonousSap : Item
	{
        public int ticks = 0;
        public int maxTicks = 60;

        public int damage = 5;
        
		[Constructable]
        public PoisonousSap(): this(Utility.RandomList(4651, 4652, 4653, 4654))		
		{            
            Name = "poisonous sap";
            Hue = 2542;
		}

		[Constructable]
		public PoisonousSap( int itemID ) : base( itemID )
		{
			Movable = false;

			new InternalTimer( this ).Start();
		}

        public PoisonousSap(Serial serial): base(serial)
		{
			new InternalTimer( this ).Start();
		}

        public override bool OnMoveOver(Mobile mobile)
        {   
            if (!mobile.CanBeDamaged() || !mobile.Alive)
                return true;

            if (mobile is WoodlandSprite || mobile is ElderWoodlandSprite || mobile is EarthlyTendril || mobile is WildOne || mobile is ArborealMyconid ||
                mobile is TreeStalker || mobile is Ent || mobile is TreeOfLife || mobile is Myconid || mobile is MyconidTallstalk)
            {
                return true;
            }

            if (mobile is BaseCreature)
                damage *= 2;
            
            AOS.Damage(mobile, damage, 0, 100, 0, 0, 0);           

            Effects.PlaySound(mobile.Location, mobile.Map, 0x230);
            mobile.FixedParticles(0x374A, 10, 20, 5021, 2003, 0, EffectLayer.Head);

            mobile.SendMessage("Poisonous sap eats away at your skin!");

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
			private PoisonousSap m_PoisonousSap;
            private static Queue m_Queue = new Queue();

            public InternalTimer(PoisonousSap poisonousSap): base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0.5))
			{
				Priority = TimerPriority.OneSecond;
                m_PoisonousSap = poisonousSap;
			}

			protected override void OnTick()
			{
                if (m_PoisonousSap == null)
                    Stop();

                if (m_PoisonousSap.Deleted || m_PoisonousSap.ticks >= m_PoisonousSap.maxTicks)
                {
                    m_PoisonousSap.Delete();
                    Stop();
                }

                else
                {
                    var mobilesOnTile = m_PoisonousSap.GetMobilesInRange(0);

                    foreach (Mobile mobile in mobilesOnTile)
                    {
                        if (!mobile.CanBeDamaged() || !mobile.Alive) 
                            continue;

                        if (mobile is WoodlandSprite || mobile is ElderWoodlandSprite || mobile is EarthlyTendril || mobile is WildOne || mobile is ArborealMyconid ||
                            mobile is TreeStalker || mobile is Ent || mobile is TreeOfLife || mobile is Myconid || mobile is MyconidTallstalk)
                        {
                            continue;
                        }

                        m_Queue.Enqueue(mobile);
                    }

                    mobilesOnTile.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        int damage = m_PoisonousSap.damage;

                        if (mobile is BaseCreature)
                            damage  *= 2;

                        AOS.Damage(mobile, damage, 0, 100, 0, 0, 0);

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x230);
                        mobile.FixedParticles(0x374A, 10, 20, 5021, 2003, 0, EffectLayer.Head);

                        mobile.SendMessage("Poisonous sap eats away at your skin!");
                    }
                }

                m_PoisonousSap.ticks++;
			}
		}
	}
}