using System;
using Server;
using Server.Mobiles;
using System.Collections;
using Server.Achievements;
using Server.Network;

namespace Server.Items
{
	public class UOACZFirefield : Item
	{
        public Mobile m_Owner;

        public DateTime m_Expiration = DateTime.MaxValue;
        public TimeSpan Duration = TimeSpan.FromSeconds(30);
        
        public int m_HitSound = 0x208;
        
        public Timer m_Timer;

        public bool m_UndeadBased = false;
        
		[Constructable]
        public UOACZFirefield(Mobile owner): base(0x398C)		
		{            
            Name = "fire field";

            Movable = false;

            m_Owner = owner;
            m_Expiration = DateTime.UtcNow + Duration;

            PlayerMobile playerOwner = m_Owner as PlayerMobile;

            if (playerOwner != null)
            {
                if (playerOwner.IsUOACZUndead)
                    m_UndeadBased = true;
            }

            if (m_Owner is UOACZBaseUndead)
                m_UndeadBased = true;

            if (!m_UndeadBased)
                Hue = 2515;

            Effects.PlaySound(Location, Map, m_HitSound);

            m_Timer = new InternalTimer(this, m_Owner);
            m_Timer.Start();
		}

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);            
        }

        public UOACZFirefield(Serial serial): base(serial)
		{	
		}

        public override bool OnMoveOver(Mobile mobile)
        {
            if (mobile == null) return true;
            if (mobile.Deleted || !mobile.Alive) return true;

            if (!UOACZSystem.IsUOACZValidMobile(mobile))
                return true;

            PlayerMobile player = mobile as PlayerMobile;

            if (player != null)
            {
                if (player.IsUOACZUndead)
                {
                    if (player.m_UOACZAccountEntry.UndeadProfile.ActiveForm == UOACZUndeadUpgradeType.FlamingZombie)
                        return true;
                }
            }

            if (mobile is UOACZFlamingZombie)
                return true;

            double damageScalar = 1;

            if (m_UndeadBased)
            {
                if (mobile is UOACZBaseUndead)
                    return true;

                if (player != null)
                {
                    if (player.IsUOACZUndead)
                        return true;
                }

                if (mobile is BaseCreature)
                    damageScalar = 2;
            }

            else
            {
                if (mobile is UOACZBaseHuman)
                    return true;

                if (player != null)
                {
                    if (player.IsUOACZHuman)
                        return true;
                }

                if (mobile is BaseCreature)
                    damageScalar = 2;
            }            

            int minDamage = 3;
            int maxDamage = 5;

            double damage = (double)Utility.RandomMinMax(minDamage, maxDamage) * damageScalar;

            if (damage > 0)
            {
                int finalDamage = (int)(Math.Round(damage));

                if (finalDamage >= mobile.Hits && mobile is UOACZBaseUndead)
                {
                    if (m_Owner is PlayerMobile && UOACZSystem.IsUOACZValidMobile(m_Owner))
                    {
                        PlayerMobile pm_Owner = m_Owner as PlayerMobile;

                        if (pm_Owner.IsUOACZHuman)
                            AchievementSystemImpl.Instance.TickProgressMulti(player, AchievementTriggers.Trigger_UOACZKillCreaturesWithFirefields, 1);
                    }
                }

                AOS.Damage(mobile, finalDamage, 0, 100, 0, 0, 0);
                Effects.PlaySound(mobile.Location, mobile.Map, m_HitSound);
            }
           
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
            public UOACZFirefield m_UOACZFirefield;
            public Mobile m_Owner;

            public static Queue m_Queue = new Queue();

            public InternalTimer(UOACZFirefield UOACZFirefield, Mobile owner): base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1))
			{
				Priority = TimerPriority.TwoFiftyMS;

                m_UOACZFirefield = UOACZFirefield;
                m_Owner = owner;
			}

			protected override void OnTick()
			{
                if (m_UOACZFirefield == null)
                {
                    Stop();
                    return;
                }

                if (m_UOACZFirefield.Deleted)
                {
                    Stop();
                    return;
                }
                
                if (DateTime.UtcNow >= m_UOACZFirefield.m_Expiration)
                {
                    Stop();  
                    m_UOACZFirefield.Delete();                                    

                    return;
                }
           
                if (m_UOACZFirefield.Map == Map.Internal || m_UOACZFirefield.Map == null)
                    return;

                bool foundAnother = false;

                IPooledEnumerable itemsOnTile = m_UOACZFirefield.Map.GetItemsInRange(m_UOACZFirefield.Location, 0);
                
                foreach (Item item in itemsOnTile)
                {
                    if (item == null) continue;
                    if (item.Deleted) continue;
                    if (item == m_UOACZFirefield) continue;

                    if (item is UOACZFirefield)
                    {
                        foundAnother = true;
                        break;
                    }
                }

                itemsOnTile.Free();

                if (foundAnother)
                {
                    Stop();
                    m_UOACZFirefield.Delete();

                    return;
                }

                if (m_UOACZFirefield.Map == Map.Internal || m_UOACZFirefield.Map == null)
                    return;

                m_Queue = new Queue();

                IPooledEnumerable itemsOnTileB = m_UOACZFirefield.Map.GetItemsInRange(m_UOACZFirefield.Location, 0);

                foreach (Item item in itemsOnTileB)
                {
                    if (item == null) continue;
                    if (item.Deleted) continue;

                    if (item is UOACZOilLocation)                    
                        m_Queue.Enqueue(item);                    
                }

                itemsOnTileB.Free();

                while (m_Queue.Count > 0)
                {
                    UOACZOilLocation oilLocation = (UOACZOilLocation)m_Queue.Dequeue();

                    if (oilLocation == null) continue;
                    if (oilLocation.Deleted) continue;
                    if (oilLocation.Burning) continue;

                    oilLocation.Ignite(null);
                }

                //Possible Result of Igniting Other Oil
                if (m_UOACZFirefield == null)
                {
                    Stop();
                    return;
                }

                if (m_UOACZFirefield.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_UOACZFirefield.Map == Map.Internal || m_UOACZFirefield.Map == null)
                    return;
                
                m_Queue = new Queue();

                IPooledEnumerable nearbyMobiles = m_UOACZFirefield.Map.GetMobilesInRange(m_UOACZFirefield.Location, 0);

                foreach (Mobile mobile in nearbyMobiles)
                {
                    if (m_UOACZFirefield == null) continue;
                    if (m_UOACZFirefield.Deleted) continue;
                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;

                    PlayerMobile player = mobile as PlayerMobile;

                    if (player != null)
                    {
                        if (player.IsUOACZUndead)
                        {                            
                            if (player.m_UOACZAccountEntry.UndeadProfile.ActiveForm == UOACZUndeadUpgradeType.FlamingZombie)
                                continue;
                        }
                    }

                    if (mobile is UOACZFlamingZombie)
                        continue;

                    if (m_UOACZFirefield.m_UndeadBased)
                    {
                        if (mobile is UOACZBaseUndead)
                            continue;

                        if (player != null)
                        {
                            if (player.IsUOACZUndead)
                                continue;
                        }
                    }

                    else
                    {
                        if (mobile is UOACZBaseHuman)
                            continue;

                        if (player != null)
                        {
                            if (player.IsUOACZHuman)
                               continue;
                        }
                    }

                    m_Queue.Enqueue(mobile);
                }

                nearbyMobiles.Free();                

                while (m_Queue.Count > 0)
                {
                    Mobile mobile = (Mobile)m_Queue.Dequeue();

                    if (m_UOACZFirefield == null) continue;
                    if (m_UOACZFirefield.Deleted) continue;
                    if (!UOACZSystem.IsUOACZValidMobile(mobile)) continue;

                    int minDamage = 3;
                    int maxDamage = 5;

                    double damageScalar = 1;

                    if (mobile is BaseCreature)
                        damageScalar = 2;

                    double damage = (double)Utility.RandomMinMax(minDamage, maxDamage) * damageScalar;

                    if (damage > 0)
                    {
                        int finalDamage = (int)(Math.Round(damage));

                        if (finalDamage >= mobile.Hits && mobile is UOACZBaseUndead)
                        {
                            if (UOACZSystem.IsUOACZValidMobile(m_Owner))
                            {
                                PlayerMobile pm_Owner = m_Owner as PlayerMobile;

                                if (pm_Owner != null)
                                {
                                    if (pm_Owner.IsUOACZHuman)
                                        AchievementSystemImpl.Instance.TickProgressMulti(pm_Owner, AchievementTriggers.Trigger_UOACZKillCreaturesWithFirefields, 1);
                                }
                            }
                        }

                        AOS.Damage(mobile, finalDamage, 0, 100, 0, 0, 0);
                        Effects.PlaySound(mobile.Location, mobile.Map, m_UOACZFirefield.m_HitSound);
                    }
                }                
			}
		}
	}
}