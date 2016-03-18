using System;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;


namespace Server.Mobiles
{
    public class BaronVonGeddon : AncientWyrm
    {
        private InternalTimer m_Timer;
        private IgniteTimer m_IgniteTimer;
       
        public override bool AlwaysBoss { get { return true; } }

        [Constructable]
        public BaronVonGeddon(double difficultyMultiplier = 1.0)
        {
            Name = "Baron Von Geddon";

            SetHits((int)Math.Ceiling(3000 * difficultyMultiplier));
            SetDamage((int)Math.Ceiling(29.0 * difficultyMultiplier), (int)Math.Ceiling(39.0 * difficultyMultiplier));

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public BaronVonGeddon(Serial serial)
            : base(serial)
        {
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

        public void Ignite(Mobile target)
        {
            if (target == null || Deleted || !CanBeHarmful(target))
                return;

            int X = target.X;
            int Y = target.Y;
            int Z = target.Z;

            for (int x = -4; x <= 4; x++)
            {
                for (int y = -4; y <= 4; y++)
                {
                    Effects.SendLocationParticles(new Entity(Serial.Zero, new Point3D(X+x, Y+y, Z), target.Map), 0x36BD, 20, 10, 5044);
                }
            }

            IPooledEnumerable eable = Map.GetMobilesInRange(target.Location, 4);

            foreach (Mobile m in eable)
            {
                if (m != this && CanBeHarmful(m))
                {
                    DoHarmful(m);
                    m.PlaySound(0x307);
                    SpellHelper.Damage(TimeSpan.FromTicks(1), m, 95);
                }
            }

            eable.Free();
        }

        public void BeginIgnite()
        {
            var targets = AcquireSpellTargets(10, 10);
            if (targets != null && targets.Count > 0)
            {
                var target = targets[Utility.RandomMinMax(0, targets.Count - 1)];
                m_IgniteTimer = new IgniteTimer(this, target);
                m_IgniteTimer.Start();

                target.SendMessage("Someone Set Up Us The Bomb!");
            }
        }

        public override bool CanBeHarmful(Mobile target)
        {
            if (target is BaronVonGeddon)
                return false;

            return base.CanBeHarmful(target);
        }

		private static Point2D NewLocation(Point3D oldLoc, Direction d)
		{
			int x = oldLoc.X;
			int y = oldLoc.Y;

			switch (d & Direction.Mask)
			{
				case Direction.North:
					--y;
					break;
				case Direction.Right:
					++x;
					--y;
					break;
				case Direction.East:
					++x;
					break;
				case Direction.Down:
					++x;
					++y;
					break;
				case Direction.South:
					++y;
					break;
				case Direction.Left:
					--x;
					++y;
					break;
				case Direction.West:
					--x;
					break;
				case Direction.Up:
					--x;
					--y;
					break;
			}

			return new Point2D(x, y);
		}

        protected override bool OnMove(Direction d)
        {
            bool inHome = true;

            if (Home != Point3D.Zero)
            {
                Point3D newLoc = new Point3D(NewLocation(Location, d), Z);
                //if (!Utility.InRange(Home, newLoc, RangeHome))
                //{
                //    inHome = false;
                //}
            }

            return base.OnMove(d) && inHome;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private DateTime m_NextTarget;
            private BaronVonGeddon m_Baron;

            public InternalTimer(BaronVonGeddon baron)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(8))
            {
				m_NextTarget = DateTime.UtcNow + TimeSpan.FromSeconds(8);
                m_Baron = baron;
            }
                
            protected override void OnTick()
            {
                if (m_Baron == null || m_Baron.Deleted)
                {
                    Stop();
                    return;
                }

				if (m_NextTarget < DateTime.UtcNow)
                {
                    m_Baron.BeginIgnite();
					m_NextTarget = DateTime.UtcNow + TimeSpan.FromSeconds(8);
                }
            }
        }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

    		switch( Utility.RandomMinMax( 0, 3 ) )
    		{
         		case 0: { c.AddItem( SpellScroll.MakeMaster( new MassDispelScroll() ) ); } break;
         		case 1: { c.AddItem( new ForgedMetal( ) ); } break;
                case 2: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Helmet)); } break;
                case 3: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gloves)); } break;
         	}
		}

        private class IgniteTimer : Timer
        {
            private int m_Countdown;
            private BaronVonGeddon m_Baron;
            private Mobile m_Target;

            public IgniteTimer(BaronVonGeddon baron, Mobile target)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Countdown = 6;
                m_Baron = baron;
                m_Target = target;
            }

            protected override void OnTick()
            {
                if (m_Baron == null || m_Baron.Deleted || m_Target == null || m_Target.Deleted)
                {
                    Stop();
                    return;
                }

                m_Target.PublicOverheadMessage(Network.MessageType.Regular, 32, true, String.Format("0:{0:D2}", m_Countdown--));

                if (m_Countdown < 0)
                {
                    m_Baron.Ignite(m_Target);
                    Stop();
                }
            }
        }
    }
}
