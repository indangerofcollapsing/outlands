using System;
using Server.Targeting;
using Server.Items;
using Server.Network;
using Server.Misc;
using Server.Mobiles;
using Server.Multis;
using Server.Items;
using Server.Custom;

namespace Server.Spells.Sixth
{
    public class ParalyzeFieldSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Paralyze Field", "In Ex Grav",
                230,
                9012,
                false,
                Reagent.BlackPearl,
                Reagent.Ginseng,
                Reagent.SpidersSilk
            );

        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

        public ParalyzeFieldSpell(Mobile caster, Item scroll): base(caster, scroll, m_Info)
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

                Effects.PlaySound(p, Caster.Map, 0x20B);

                int itemID = eastToWest ? 0x3967 : 0x3979;

                TimeSpan duration = TimeSpan.FromSeconds(3.0 + (Caster.Skills[SkillName.Magery].Value / 3.0));

                bool enhancedSpellcast = SpellHelper.IsEnhancedSpell(Caster, null, EnhancedSpellbookType.Warlock, false, true);

                for (int i = -2; i <= 2; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);
                    bool canFit = SpellHelper.AdjustField(ref loc, Caster.Map, 12, false);

                    if (!canFit)
                        continue;

                    InternalItem item = new InternalItem(Caster, itemID, loc, Caster.Map, duration);

                    if (enhancedSpellcast)
                        item.m_Enhanced = true;

                    item.ProcessDelta();

                    Effects.SendLocationParticles(EffectItem.Create(loc, Caster.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 5048);
                }
            }

            FinishSequence();
        }

        [DispellableField]
        public class InternalItem : Item
        {
            private Timer m_Timer;
            private Mobile m_Caster;
            private DateTime m_End;
            public Boolean m_Enhanced = false;

            public override bool BlocksFit { get { return true; } }

            public InternalItem(Mobile caster, int itemID, Point3D loc, Map map, TimeSpan duration): base(itemID)
            {
                Visible = false;
                Movable = false;
                Light = LightType.Circle300;

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(caster, HueableSpell.ParalyzeField);

                Hue = spellHue;

                MoveToWorld(loc, map);

                if (caster.InLOS(this))
                    Visible = true;
                else
                    Delete();

                if (Deleted)
                    return;

                m_Caster = caster;               

                m_Timer = new InternalTimer(this, duration);
                m_Timer.Start();

                m_End = DateTime.UtcNow + duration;
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            public InternalItem(Serial serial): base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); //version

                //Version 0
                writer.Write(m_Enhanced);
                writer.Write(m_Caster);
                writer.WriteDeltaTime(m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadInt();

                //Version 0
                if (version >= 0)
                {
                    m_Enhanced = reader.ReadBool();
                    m_Caster = reader.ReadMobile();
                    m_End = reader.ReadDeltaTime();
                }

                //-----

                m_Timer = new InternalTimer(this, m_End - DateTime.UtcNow);
                m_Timer.Start();
            }

            public override bool OnMoveOver(Mobile m)
            {                
                if (Visible && m_Caster != null && SpellHelper.ValidIndirectTarget(m_Caster, m) && m_Caster.CanBeHarmful(m, false))
                {
                    m_Caster.DoHarmful(m);

                    double duration; 
                
                    duration = 5.0 + (m_Caster.Skills[SkillName.Magery].Value * 0.05);
                    
                    //Enhanced Spellbook: Warlock
                    if (m_Enhanced && m is BaseCreature)
                    {
                        duration *= 2;

                        if (m.Paralyze(m_Caster, duration))
                        {
                            m.FixedEffect(0x376A, 10, 32);
                            m.PlaySound(0x204);     
                        }

                        else if (m is PlayerMobile)
                        {
                            m.FixedEffect(0x376A, 10, 32);
                            m.PlaySound(0x204); 
                        }
                    }

                    else
                    {
                        if (m.Paralyze(m_Caster, duration))
                        {
                            m.FixedEffect(0x376A, 10, 16);
                            m.PlaySound(0x204);
                        }

                        else if (m is PlayerMobile)
                        {
                            m.FixedEffect(0x376A, 10, 32);
                            m.PlaySound(0x204); 
                        }
                    }

                    if (m is BaseCreature)
                        ((BaseCreature)m).OnHarmfulSpell(m_Caster);
                }

                return true;
            }

            private class InternalTimer : Timer
            {
                private Item m_Item;

                public InternalTimer(Item item, TimeSpan duration): base(duration)
                {
                    m_Item = item;
                }

                protected override void OnTick()
                {
                    m_Item.Delete();
                }
            }
        }

        private class InternalTarget : Target
        {
            private ParalyzeFieldSpell m_Owner;

            public InternalTarget(ParalyzeFieldSpell owner): base(Core.ML ? 10 : 12, true, TargetFlags.None)
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