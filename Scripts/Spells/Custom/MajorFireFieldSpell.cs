using System;
using Server;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Misc;

namespace Server.Spells.Custom
{

    /// <summary>
    /// -- Elbows
    /// Major Fire Field is a custom spell used by boss mobs which extends the FireField spell.
    /// It overrides OnTarget to make use of the MajorFireFieldItem class which does the bulk
    /// of the customization.  It also allows the caster to explicitly specify how the fire field
    /// should be oriented through the Direction property.
    /// </summary>
    class MajorFireFieldSpell : FireFieldSpell
    {
        private bool m_Direction;

        /// <summary>
        /// direction property is a boolean used to orient the fire field.
        /// </summary>
        public bool Direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = value;
            }
        }

        private static SpellInfo m_Info = new SpellInfo(
				"Major Fire Field", "In Flam Grav",
				215,
				9041,
				false,
				Reagent.BlackPearl,
				Reagent.SpidersSilk,
				Reagent.SulfurousAsh
		);

        public MajorFireFieldSpell( Mobile caster, Item scroll ) : base( caster, scroll )
		{
            m_Direction = false;
		}

        /// <summary>
        /// Target is overridden to use the MajorFireFieldItem and also to make
        /// use of the Direction property
        /// </summary>
        /// <param name="p"></param>
        public new void Target(IPoint3D p) {
            
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);


                Effects.PlaySound(p, Caster.Map, 0x20C);

                int itemID = m_Direction ? 0x3915 : 0x3922;

                TimeSpan duration = TimeSpan.FromSeconds(4.0 + (Caster.Skills[SkillName.Magery].Value * 0.5));

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(m_Direction ? p.X + i : p.X, m_Direction ? p.Y : p.Y + i, p.Z);

                    new MajorFireFieldItem(itemID, loc, Caster, Caster.Map, duration, i);
                }
            }

            FinishSequence();
        }

        /// <summary>
        /// MajorFireFieldItem does not implement fire field item because there is too much that needed to be
        /// overridden and the base member variables were hidden by declarations in this class, so we extend item
        /// just like FireFieldItem does.
        /// </summary>
        [DispellableField]
        public class MajorFireFieldItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;
            private Mobile m_Caster;
            private int m_Damage;

            public override bool BlocksFit { get { return true; } }

            public MajorFireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val)
                : this(itemID, loc, caster, map, duration, val, 10)
            {
            }

            /// <summary>
            /// CanFit is explicitly set to true so the full X Shows when the spell is cast
            /// I was getting some funky blocking.
            /// </summary>
            /// <param name="itemID"></param>
            /// <param name="loc"></param>
            /// <param name="caster"></param>
            /// <param name="map"></param>
            /// <param name="duration"></param>
            /// <param name="val"></param>
            /// <param name="damage"></param>
            public MajorFireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val, int damage)
                : base(itemID)
            {
                //bool canFit = SpellHelper.AdjustField( ref loc, map, 12, false );
                bool canFit = true;

                Visible = false;
                Movable = false;
                Light = LightType.Circle300;

                MoveToWorld(loc, map);

                m_Caster = caster;

                m_Damage = damage;

                m_End = DateTime.UtcNow + duration;

                m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(Math.Abs(val) * 0.2), caster.InLOS(this), canFit);
                m_Timer.Start();
            }


            /// <summary>
            /// Unchagned from FireFieldItem
            /// </summary>
            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            /// <summary>
            /// Unchagned from FireFieldItem
            /// </summary>
            public MajorFireFieldItem(Serial serial)
                : base(serial)
            {
            }

            /// <summary>
            /// Unchagned from FireFieldItem
            /// </summary>
            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)1); // version

                //Commented by IPY
                //writer.Write( m_Damage );
                writer.Write(m_Caster);
                writer.WriteDeltaTime(m_End);
            }

            /// <summary>
            /// m_Damage explicitly set to 10
            /// </summary>
            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch (version)
                {
                    //Commented by IPY
                    /*case 2:
					{
						m_Damage = reader.ReadInt();
						goto case 1;
					}*/
                    case 1:
                        {
                            m_Caster = reader.ReadMobile();

                            goto case 0;
                        }
                    case 0:
                        {
                            m_End = reader.ReadDeltaTime();

                            m_Timer = new InternalTimer(this, TimeSpan.Zero, true, true);
                            m_Timer.Start();

                            break;
                        }
                }
                m_Damage = 10;
            }

            /// <summary>
            /// This function was changed to include m.Player in the if statement.
            /// This prevents BawwSarre and his summons from being hit by the spell.
            /// </summary>
            /// <param name="m"></param>
            /// <returns></returns>
            public override bool OnMoveOver(Mobile m)
            {
                if (m.Player && Visible && m_Caster != null && SpellHelper.ValidIndirectTarget(m_Caster, m) && m_Caster.CanBeHarmful(m, false))
                {
                    m_Caster.DoHarmful(m);

                    AOS.Damage(m, m_Caster, m_Damage, 0, 100, 0, 0, 0);
                    m.PlaySound(0x208);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private MajorFireFieldItem m_Item;
                private bool m_InLOS, m_CanFit;

                private static Queue m_Queue = new Queue();

                public InternalTimer(MajorFireFieldItem item, TimeSpan delay, bool inLOS, bool canFit)
                    : base(delay, TimeSpan.FromSeconds(1.0))
                {
                    m_Item = item;
                    m_InLOS = inLOS;
                    m_CanFit = canFit;

                    Priority = TimerPriority.FiftyMS;
                }

                /// <summary>
                /// This function was also changed to include a  check for m.Player
                /// it also increases the damage per tick over a normal fire field to 5
                /// </summary>
                protected override void OnTick()
                {
                    if (m_Item.Deleted)
                        return;

                    if (!m_Item.Visible)
                    {
                        if (m_InLOS && m_CanFit)
                            m_Item.Visible = true;
                        else
                            m_Item.Delete();

                        if (!m_Item.Deleted)
                        {
                            m_Item.ProcessDelta();
                            Effects.SendLocationParticles(EffectItem.Create(m_Item.Location, m_Item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5029);
                        }
                    }
                    else if (DateTime.UtcNow > m_Item.m_End)
                    {
                        m_Item.Delete();
                        Stop();
                    }
                    else
                    {
                        Map map = m_Item.Map;
                        Mobile caster = m_Item.m_Caster;

                        if (map != null && caster != null)
                        {
                            foreach (Mobile m in m_Item.GetMobilesInRange(0))
                            {
                                if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && SpellHelper.ValidIndirectTarget(caster, m) && caster.CanBeHarmful(m, false))
                                    m_Queue.Enqueue(m);
                            }

                            while (m_Queue.Count > 0)
                            {
                                Mobile m = (Mobile)m_Queue.Dequeue();
                                if (m.Player)
                                {
                                    caster.DoHarmful(m);

                                    int damage = 5;

                                    AOS.Damage(m, caster, damage, 0, 100, 0, 0, 0);
                                    m.PlaySound(0x208);
                                }
                            }
                        }
                    }
                }
            }
        }


    }
}
