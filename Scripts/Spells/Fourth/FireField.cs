using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Multis;

namespace Server.Spells.Fourth
{
    public class FireFieldSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Fire Field", "In Flam Grav",
                215,
                9041,
                false,
                Reagent.BlackPearl,
                Reagent.SpidersSilk,
                Reagent.SulfurousAsh
            );

        public override SpellCircle Circle { get { return SpellCircle.Fourth; } }

        public FireFieldSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            BaseCreature casterCreature = Caster as BaseCreature;

            if (casterCreature != null)
            {
                if (casterCreature.SpellTarget != null)                
                    this.Target(casterCreature.SpellTarget);                
            }

            else            
                Caster.Target = new InternalTarget(this);            
        }

        public void Target(IPoint3D p)
        {
            if (!Caster.CanSee(p))            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.       

            else if (BaseBoat.FindBoatAt(p, Caster.Map) != null)
                Caster.SendMessage("That location is blocked.");       

            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                int dx = Caster.Location.X - p.X;
                int dy = Caster.Location.Y - p.Y;
                int rx = (dx - dy) * 44;
                int ry = (dx + dy) * 44;

                bool eastToWest;

                if (rx >= 0 && ry >= 0)
                    eastToWest = false;

                else if (rx >= 0)
                    eastToWest = true;

                else if (ry >= 0)
                    eastToWest = true;

                else
                    eastToWest = false;

                Effects.PlaySound(p, Caster.Map, 0x20C);

                int itemID = eastToWest ? 0x398C : 0x3996;

                TimeSpan duration;

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Fire, false, true);

                duration = TimeSpan.FromSeconds(4.0 + (Caster.Skills[SkillName.Magery].Value * 0.5));

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);

                    FireFieldItem fireField = new FireFieldItem(itemID, loc, Caster, Caster.Map, duration, i);

                    if (enhancedSpellcast)
                        fireField.m_Enhanced = true;
                }
            }

            FinishSequence();
        }

        [DispellableField]
        public class FireFieldItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;
            private Mobile m_Caster;
            private int m_Damage;
            private bool m_ResistCheck = true;
            private double MaxResist = 50.0;

            public Boolean m_Enhanced = false;

            public override bool BlocksFit { get { return true; } }

            public FireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val): this(itemID, loc, caster, map, duration, val, 2)
            {
            }

            public FireFieldItem(int itemID, Point3D loc, Mobile caster, Map map, TimeSpan duration, int val, int damage, bool resistCheck = true): base(itemID)
            {
                bool canFit = SpellHelper.AdjustField(ref loc, map, 12, false);

                Visible = false;
                Movable = false;
                Light = LightType.Circle300;
                m_ResistCheck = resistCheck;

                MoveToWorld(loc, map);

                m_Caster = caster;

                m_Damage = damage;

                m_End = DateTime.UtcNow + duration;                 

                m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(Math.Abs(val) * 0.2), caster.InLOS(this), canFit, resistCheck);
                m_Timer.Start();
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            public FireFieldItem(Serial serial): base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version
               
                writer.Write(m_Caster);
                writer.WriteDeltaTime(m_End);
                writer.Write((Boolean)m_Enhanced);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                //Version 0
                if (version >= 0)
                {
                    m_Caster = reader.ReadMobile();
                    m_End = reader.ReadDeltaTime();
                    m_Enhanced = reader.ReadBool();
                }

                //-----

                m_Timer = new InternalTimer(this, TimeSpan.Zero, true, true);
                m_Timer.Start();                
            }

            public override bool OnMoveOver(Mobile m)
            {
                if (Visible && m_Caster != null && SpellHelper.ValidIndirectTarget(m_Caster, m) && m_Caster.CanBeHarmful(m, false))
                {
                    m_Caster.DoHarmful(m);

                    double damage = (double)m_Damage;

                    if (m_ResistCheck && m.CheckSkill(SkillName.MagicResist, 0.0, MaxResist, 1.0))
                    {
                        damage *= .5;
                        m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    }

                    if (m_Enhanced && m is BaseCreature)
                        damage *= 1.5;

                    int finalDamage = (int)Math.Round(damage);

                    if (finalDamage < 1)
                        finalDamage = 1;

                    AOS.Damage(m, m_Caster, finalDamage, 0, 100, 0, 0, 0);
                    m.PlaySound(0x208);

                    if (m is BaseCreature)
                        ((BaseCreature)m).OnHarmfulSpell(m_Caster);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private FireFieldItem m_Item;
                private bool m_InLOS, m_CanFit, m_ResistCheck;

                private static Queue m_Queue = new Queue();

                public InternalTimer(FireFieldItem item, TimeSpan delay, bool inLOS, bool canFit, bool resistCheck = true): base(delay, TimeSpan.FromSeconds(1.0))
                {
                    m_Item = item;
                    m_InLOS = inLOS;
                    m_CanFit = canFit;
                    m_ResistCheck = resistCheck;

                    Priority = TimerPriority.FiftyMS;
                }

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
                            IPooledEnumerable eable = m_Item.GetMobilesInRange(0);

                            foreach (Mobile m in eable)
                            {
                                if ((m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && SpellHelper.ValidIndirectTarget(caster, m) && caster.CanBeHarmful(m, false))
                                    m_Queue.Enqueue(m);
                            }

                            eable.Free();

                            while (m_Queue.Count > 0)
                            {
                                Mobile m = (Mobile)m_Queue.Dequeue();

                                caster.DoHarmful(m);

                                double damage = (double)m_Item.m_Damage;

                                if (m_ResistCheck && m.CheckSkill(SkillName.MagicResist, 0.0, m_Item.MaxResist, 1.0))                                
                                    damage *= .5;                                

                                if (m_Item.m_Enhanced && m is BaseCreature)
                                    damage *= .5;
                              
                                int finalDamage = (int)Math.Round(damage);

                                if (finalDamage < 1)
                                    finalDamage = 1;

                                AOS.Damage(m, caster, finalDamage, 0, 100, 0, 0, 0);
                                m.PlaySound(0x208);

                                if (m is BaseCreature)
                                    ((BaseCreature)m).OnHarmfulSpell(caster);
                            }
                        }
                    }
                }
            }
        }

        private class InternalTarget : Target
        {
            private FireFieldSpell m_Owner;

            public InternalTarget(FireFieldSpell owner): base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}