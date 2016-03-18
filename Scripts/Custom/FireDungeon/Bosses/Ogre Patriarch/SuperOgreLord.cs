using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Targeting;
using Server.Factions;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "an ogre patriarch corpse" )]
	public class SuperOgreLord : BaseCreature
	{
        public static readonly int StompSize = 30; //make this an even number greater than zero
        public override bool AlwaysBoss { get { return true; } }

		public override Faction FactionAllegiance { get { return Minax.Instance; } }
		public override Ethics.Ethic EthicAllegiance { get { return Ethics.Ethic.Evil; } }
		
		public virtual bool DoEarthquakeSpecial { get { return true; } }

        public override bool Paralyzed
        {
            get
            {
                return base.Paralyzed;
            }
            set
            {
                base.Paralyzed = false;
            }
        }

		private StompTimer m_StompTimer;
        private bool goingHome = false;
        private int cracksMaxDistance = 6;
        private int cracksMinDistance = 0;

		[Constructable]
		public SuperOgreLord(double difficultyMultiplier = 1.0) : base( AIType.AI_Melee, FightMode.Weakest, 10, 1, 0.1, 0.2 )
		{
			Name = "Ogre Patriarch";
			Body = 83;
			BaseSoundID = 427;
			Hue = 2014;

			SetStr( 767, 945 );
			SetDex( 156, 165 );
			SetInt( 46, 70 );
			
			SetStam( 100 );

			SetHits( 4760, 5520 );
      SetHits((int)Math.Ceiling(Hits * difficultyMultiplier));

      SetDamage((int)Math.Ceiling(29.0 * difficultyMultiplier), (int)Math.Ceiling(35.0 * difficultyMultiplier));

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 125.1, 140.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 50;

			PackItem( new Club() );

			m_StompTimer = new StompTimer(this);
			m_StompTimer.Start();
		}

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            UniqueCreatureDifficultyScalar = 2.0;
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath(c);

			switch (Utility.RandomMinMax(0, 4))
			{
				case 0: { c.AddItem(new Diamond()); } break;
				case 1: { c.AddItem(new ForgedMetal()); } break;
                case 2: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Arms)); } break;
				case 3: { c.AddItem(new PatchOfPoppies()); } break;
                case 4: { c.AddItem(SpellScroll.MakeMaster(new SummonDaemonScroll())); } break;
			}
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int Meat{ get{ return 2; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CracksMaxDistance
        {
          get { return cracksMaxDistance; }
          set { cracksMaxDistance = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int CracksMinDistance
        {
          get { return cracksMinDistance; }
          set { cracksMinDistance = value; }
        }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 1 );
		}


		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( Utility.RandomBool() )
				Earthquake();
		}

        public override bool CanBeHarmful(Mobile target)
        {
            if (target is SuperOgreLord)
                return false;

            return base.CanBeHarmful(target);
        }

		public void Earthquake()
		{
			Map map = this.Map;

			if ( map == null || DoEarthquakeSpecial == false )
				return;

			ArrayList targets = new ArrayList();

            var mobiles = this.GetMobilesInRange(12);
			foreach ( Mobile m in mobiles )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

				if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
					targets.Add( m );
				else if ( m.Player )
					targets.Add( m );
			}

            mobiles.Free();

			PlaySound( 0x2F3 );

			for ( int i = 0; i < targets.Count; ++i )
			{
				Mobile m = (Mobile)targets[i];

				double damage = m.Hits * 0.6;

                if ( damage > 75.0 )
					damage = 75.0;

				DoHarmful( m );

                m.Damage((int)damage, this);

				if ( m.Alive && m.Body.IsHuman && !m.Mounted )
					m.Animate( 20, 7, 1, true, false, 0 ); // take hit
			}
		}

        public Direction Opposite(Direction d)
        {
            switch (d & Direction.Mask)
            {
                case (Direction.Up): return Direction.Down;
                case (Direction.North): return Direction.South;
                case (Direction.Right): return Direction.Left;
                case (Direction.East): return Direction.West;
                case (Direction.Down): return Direction.Up;
                case (Direction.South): return Direction.North;
                case (Direction.Left): return Direction.Right;
                case (Direction.West): return Direction.East;
                default: return Direction.Left;
            }
        }

		private void Stomp()
		{
			CantWalk = true;
			
			PublicOverheadMessage( Network.MessageType.Regular, 0x3F, true, "ROAAARRRR!");
			Animate(11, 10, 1, true, false, 1);

            Rectangle2D stompRect;

            Direction facing = Direction & Direction.Mask;
            if (Utility.RandomBool())
                facing = Opposite(facing);

            int width = 15;
            int height = 15;
            
            switch (facing)
            {
                case (Direction.Up):
                case (Direction.North): stompRect = new Rectangle2D(new Point2D(Location.X - width, Location.Y-height), new Point2D(Location.X+width, Location.Y)); break;
                case (Direction.Right):
                case (Direction.East): stompRect = new Rectangle2D(new Point2D(Location.X, Location.Y - height), new Point2D(Location.X + width, Location.Y+height)); break;
                case (Direction.Down):
                case (Direction.South): stompRect = new Rectangle2D(new Point2D(Location.X - width, Location.Y), new Point2D(Location.X + width, Location.Y+height)); break;
                case (Direction.Left):
                case (Direction.West): stompRect = new Rectangle2D(new Point2D(Location.X-width, Location.Y - height), new Point2D(Location.X, Location.Y+height)); break;
                default: stompRect = new Rectangle2D(); break;
            }

            IPoint3D Itemp;
            Point3D temp;

            for (int x = 0; x <= stompRect.Width; x++)
            {
                for (int y = 0; y <= stompRect.Height; y++)
                {
                    Itemp = new Point3D(stompRect.Start.X + x, stompRect.Start.Y + y, 0);
                    temp = new Point3D(Itemp.X, Itemp.Y, Map.GetAverageZ(Itemp.X, Itemp.Y));
                    if (Map.CanFit(temp, 0, true, false))
                    {
                        new CrackedFloor().MoveToWorld(temp, Map);
                        Effects.SendLocationEffect(temp, Map, 0x3728, 13);
                    }
                }
            }

            Timer.DelayCall(TimeSpan.FromSeconds(4), delegate { CantWalk = false; });
		}
			                                
		private Point3D RandomLocation()
		{
            int xOffset = Utility.RandomMinMax(-1 * CracksMaxDistance, CracksMaxDistance);
            int yOffset = Utility.RandomMinMax(-1 * CracksMaxDistance, CracksMaxDistance);

            IPoint3D target = new Point3D(X + xOffset, Y + yOffset, Z);
            SpellHelper.GetSurfaceTop(ref target);
            return (Point3D)target;
		}

		public SuperOgreLord( Serial serial ) : base( serial )
		{
		}
		
		public override void OnAfterDelete()
		{
			if ( m_StompTimer != null )
				m_StompTimer.Stop();

			m_StompTimer = null;

			base.OnAfterDelete();
		}		
		
		public override void OnThink()
		{
            if (goingHome == true && GetHomeDistance() == 0)
            {
                Hits = HitsMax;
                goingHome = false;
            }

			if(GetHomeDistance() > RangeHome)
			{
				TargetLocation = new Point2D(Home.X, Home.Y);
                goingHome = true;
			}
			
			base.OnThink();
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
			
			switch ( version )
			{
				case 0:
				{
					
					break;
				}
			}

            m_StompTimer = new StompTimer(this);
            m_StompTimer.Start();

		}
		
		private class StompTimer : Timer
		{
			private SuperOgreLord m_Owner;
			
			public StompTimer( SuperOgreLord owner ) : base( TimeSpan.FromSeconds( 2.0 ), TimeSpan.FromSeconds( 10.0 ) )
			{
				m_Owner = owner;
				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				if ( m_Owner == null || m_Owner.Deleted )
				{
					Stop();
					return;
				}
				
				if(m_Owner.Hits < (m_Owner.HitsMax * .9))
				{
					m_Owner.Stomp();
				}
			}
		}		
	}
}
