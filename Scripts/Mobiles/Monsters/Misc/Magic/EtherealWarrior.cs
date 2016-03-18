using System;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Mobiles 
{ 
	[CorpseName( "an ethereal warrior corpse" )] 
	public class EtherealWarrior : BaseCreature 
	{ 
		public override bool InitialInnocent{ get{ return true; } }

		[Constructable] 
		public EtherealWarrior() : base( AIType.AI_Mage, FightMode.Evil, 10, 1, 0.2, 0.4 ) 
		{ 
			Name = NameList.RandomName( "ethereal warrior" );
			Body = 123;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 100);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

			Fame = 7000;
			Karma = 7000;
		}

        public override int Feathers { get { return 250; } }
        
		private DateTime m_NextResurrect;
		private static TimeSpan ResurrectDelay = TimeSpan.FromSeconds(2.0);

		public override void OnMovement(Mobile from, Point3D oldLocation)
		{
			if (!from.Alive && (from is PlayerMobile))
			{
				if (!from.Frozen && (DateTime.UtcNow >= m_NextResurrect) && InRange(from, 4) && !InRange(oldLocation, 4) && InLOS(from))
				{
					m_NextResurrect = DateTime.UtcNow + ResurrectDelay;
					if (!from.Criminal && (from.ShortTermMurders < 5) && (from.Karma > 0))
					{
						if (from.Map != null && from.Map.CanFit(from.Location, 16, false, false))
						{
							Direction = GetDirectionTo(from);
							from.PlaySound(0x1F2);
							from.FixedEffect(0x376A, 10, 16);
							from.CloseGump(typeof(ResurrectGump));
							from.SendGump(new ResurrectGump(from, ResurrectMessage.Healer));
						}
					}
				}
			}
		}

		public override int GetAngerSound()
		{
			return 0x2F8;
		}

		public override int GetIdleSound()
		{
			return 0x2F8;
		}

		public override int GetAttackSound()
		{
			return Utility.Random( 0x2F5, 2 );
		}

		public override int GetHurtSound()
		{
			return 0x2F9;
		}

		public override int GetDeathSound()
		{
			return 0x2F7;
		}	

		public EtherealWarrior( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	}
}
