using System;
using Server;
using Server.Mobiles;
using System.Collections;

namespace Server.Items
{
	public class Creep : Item
	{
        public int ticks = 0;
        public int maxTicks = 60; //3.0 Seconds Per Tick        

        public Item m_Item;
                
		[Constructable]
        public Creep(): this(Utility.RandomList(4650, 4653, 7410, 7411, 7412, 7414, 7415, 7416, 7418, 7420, 7421, 7422, 7422, 7425, 7426, 7428, 7431, 7436, 7441, 7442, 4655))		
		{            
            Name = "creep";
            Hue = 2597;
		}

		[Constructable]
		public Creep( int itemID ) : base( itemID )
		{           
			Movable = false;

			new InternalTimer( this ).Start();
		}

        public Creep(Serial serial): base(serial)
		{
			new InternalTimer( this ).Start();
		}

        public override bool OnMoveOver(Mobile mobile)
        { 
            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player) 
                return true;

            bool validTarget = false;

            PlayerMobile pm_Target = mobile as PlayerMobile;
            BaseCreature bc_Target = mobile as BaseCreature;

            if (pm_Target != null)
                validTarget = true;

            if (bc_Target != null)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    validTarget = true;
            }

            if (!validTarget)
                return true;

            Effects.PlaySound(mobile.Location, mobile.Map, 0x62A);
            mobile.FixedParticles(0x374A, 10, 20, 5021, 2596, 0, EffectLayer.LeftFoot);

            SpecialAbilities.EntangleSpecialAbility(1.0, null, mobile, 1.0, 1.0, 0x5D9, false, "", "");
            mobile.SendMessage("Your feet sink into the creep...");

            return true;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Item != null)
            {
                if (!m_Item.Deleted)
                    m_Item.Delete();
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //version

            writer.Write(ticks);
            writer.Write(m_Item);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            ticks = reader.ReadInt();
            m_Item = reader.ReadItem();
		}

		private class InternalTimer : Timer
		{
			private Creep m_Creep;
            private static Queue m_Queue = new Queue();

            public InternalTimer(Creep Creep): base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(3.0))
			{
				Priority = TimerPriority.OneSecond;
                m_Creep = Creep;
			}

			protected override void OnTick()
			{
                if (m_Creep == null)
                    Stop();

                if (m_Creep.Deleted || m_Creep.ticks >= m_Creep.maxTicks)
                {
                    m_Creep.Delete();
                    Stop();
                }

                else
                {
                    IPooledEnumerable mobilesOnTile = m_Creep.GetMobilesInRange(0);

                    foreach (Mobile mobile in mobilesOnTile)
                    {
                        if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player) 
                            continue;

                        bool validTarget = false;

                        PlayerMobile pm_Target = mobile as PlayerMobile;
                        BaseCreature bc_Target = mobile as BaseCreature;

                        if (pm_Target != null)
                            validTarget = true;

                        if (bc_Target != null)
                        {
                            if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                                validTarget = true;
                        }

                        if (validTarget)
                            m_Queue.Enqueue(mobile);
                    }

                    mobilesOnTile.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double damage = Utility.RandomMinMax(5, 15);
            
                        if (mobile is BaseCreature)
                            damage *= 1.5;

                        AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);

                        Effects.PlaySound(mobile.Location, mobile.Map, 0x62A);                  
                        Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(0.5)), 6899, 10, 25, 2596, 0, 5029, 0);                        
                       
                        mobile.SendMessage("The creep lashes out at you!");
                    }
                }

                m_Creep.ticks++;
			}
		}
	}
}