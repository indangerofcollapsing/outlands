using System;
using Server.Targeting;
using Server.Network;
using Server.Misc;
using Server.Mobiles;

using Server.Multis;
using Server.Items;
using Server.Custom;
using Server.Regions;

namespace Server.Spells.Third
{
    public class WallOfStoneSpell : MagerySpell
    {
        private static SpellInfo m_Info = new SpellInfo(
                "Wall of Stone", "In Sanct Ylem",
                227,
                9011,
                false,
                Reagent.Bloodmoss,
                Reagent.Garlic
            );

        public override SpellCircle Circle { get { return SpellCircle.Third; } }

        public WallOfStoneSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override bool CheckCast()
        {
            if (!Caster.CanBeginAction(typeof(WallOfStoneSpell)))
            {
                Caster.SendMessage("You cannot cast that spell again so soon.");
                return false;
            }

            if (Caster.Region is NewbieDungeonRegion)
            {
                Caster.SendMessage("You cannot cast that here.");
                return false;
            }

            return base.CheckCast();
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            Point3D point = new Point3D(p.X, p.Y, p.Z);            

            if (!Caster.CanSee(p))            
                Caster.SendLocalizedMessage(500237); // Target can not be seen.            

            else if (BaseBoat.FindBoatAt(p, Caster.Map) != null)
                Caster.SendMessage("That location is blocked.");

            else if (SpellHelper.CheckTown(point, Caster) && CheckSequence())
            {
                SpellHelper.Turn(Caster, p);

                SpellHelper.GetSurfaceTop(ref p);

                int dx = Caster.Location.X - p.X;
                int dy = Caster.Location.Y - p.Y;
                int rx = (dx - dy) * 44;
                int ry = (dx + dy) * 44;

                bool eastToWest;

                if (rx >= 0 && ry >= 0)
                {
                    eastToWest = false;
                }
                else if (rx >= 0)
                {
                    eastToWest = true;
                }
                else if (ry >= 0)
                {
                    eastToWest = true;
                }
                else
                {
                    eastToWest = false;
                }

                Effects.PlaySound(p, Caster.Map, 0x1F6);

                for (int i = -1; i <= 1; ++i)
                {
                    Point3D loc = new Point3D(eastToWest ? p.X + i : p.X, eastToWest ? p.Y : p.Y + i, p.Z);
                    bool canFit = SpellHelper.AdjustField(ref loc, Caster.Map, 22, true);

                    if (!canFit)
                        continue;

                    Item item = new InternalItem(loc, Caster.Map, Caster);
                    Effects.SendLocationParticles(item, 0x376A, 9, 10, 5025);
                }
            }

            FinishSequence();

            Caster.BeginAction(typeof(WallOfStoneSpell));
            Timer.DelayCall(TimeSpan.FromSeconds(16), delegate { Caster.EndAction(typeof(WallOfStoneSpell)); });
        }

        [DispellableField]
        private class InternalItem : Item
        {
            private Timer m_Timer;
            private DateTime m_End;

            public override bool BlocksFit { get { return true; } }

            public InternalItem(Point3D loc, Map map, Mobile caster): base(128)
            {
                Visible = false;
                Movable = false;

                //Player Enhancement Customization: Geomancer
                if (PlayerEnhancementPersistance.IsCustomizationEntryActive(caster, CustomizationType.Geomancer))
                    ItemID = 376;

                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(caster, HueableSpell.WallOfStone);

                Hue = spellHue;

                MoveToWorld(loc, map);

                if (caster.InLOS(this))
                    Visible = true;
                else
                    Delete();

                if (Deleted)
                    return;

                m_Timer = new InternalTimer(this, TimeSpan.FromSeconds(10.0));
                m_Timer.Start();

                m_End = DateTime.UtcNow + TimeSpan.FromSeconds(10.0);
            }

            public InternalItem(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.Write((int)1); // version

                writer.WriteDeltaTime(m_End);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();

                switch (version)
                {
                    case 1:
                        {
                            m_End = reader.ReadDeltaTime();

                            m_Timer = new InternalTimer(this, m_End - DateTime.UtcNow);
                            m_Timer.Start();

                            break;
                        }
                    case 0:
                        {
                            TimeSpan duration = TimeSpan.FromSeconds(10.0);

                            m_Timer = new InternalTimer(this, duration);
                            m_Timer.Start();

                            m_End = DateTime.UtcNow + duration;

                            break;
                        }
                }
            }

            public override void OnAfterDelete()
            {
                base.OnAfterDelete();

                if (m_Timer != null)
                    m_Timer.Stop();
            }

            private class InternalTimer : Timer
            {
                private InternalItem m_Item;

                public InternalTimer(InternalItem item, TimeSpan duration): base(duration)
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
            private WallOfStoneSpell m_Owner;

            public InternalTarget(WallOfStoneSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
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