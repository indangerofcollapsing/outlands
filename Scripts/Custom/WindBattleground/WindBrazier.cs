using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Network;

namespace Server.Custom.Townsystem
{
	public class WindBrazier : Brazier
	{
        private class CaptureFlame : Item
        {
            public WindBrazier Brazier;

            public CaptureFlame(WindBrazier b)
                : base(6571)
            {
                Brazier = b;
                Movable = false;
            }

            public CaptureFlame(Serial serial)
                : base(serial)
            {
            }

            public override void OnSingleClick(Mobile from)
            {
                if (Brazier != null)
                    Brazier.OnSingleClick(from);
            }

            public override void OnDoubleClick(Mobile from)
            {
                if (Brazier != null)
                    Brazier.OnDoubleClick(from);
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.WriteEncodedInt(0);
                writer.WriteItem(Brazier);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();
                Brazier = (WindBrazier)reader.ReadItem();
            }

        }
        private bool m_Captured;
        private Town m_CapTown;
        private string m_LocationIdentifier;
        private Item m_EffectItem;
        private static readonly TimeSpan m_TimeBetweenHits = TimeSpan.FromSeconds(0.75);
        private InternalTimer m_Timer;
        private int m_SparkHits;

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public string BrazierLocationName
        {
            get { return m_LocationIdentifier; }
            set { m_LocationIdentifier = value;}
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town CapTown
        {
            get { return m_CapTown; }
            set { m_CapTown = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public bool Captured
        {
            get { return m_Captured; }
            set 
            {
                if (m_Captured == value)
                    return;

                m_Captured = value;
                if (!value)
                {
                    if (m_EffectItem != null && !m_EffectItem.Deleted)
                        m_EffectItem.Delete();

                    Hue = 0;
                    m_CapTown = null;
                }
                else
                {
                    if (m_EffectItem == null || m_EffectItem.Deleted)
                    {
                        m_EffectItem = new CaptureFlame(this);
                        if (m_CapTown != null)
                            Hue = m_CapTown.HomeFaction.Definition.HuePrimary;
                        m_EffectItem.MoveToWorld(new Point3D(X, Y, Z + 7),Map);
                    }
                }
            }
        }

        [Constructable]
		public WindBrazier() : base()
		{
            Movable = false;
            m_Captured = false;
            m_LocationIdentifier = "(default)";

            WindBattleground.RegisterBrazier(this);
        }

        public WindBrazier(Serial serial)
            : base(serial)
		{
		}

        public override void OnDelete()
        {
            WindBattleground.Braziers.Remove(this);

            if (m_EffectItem != null && !m_EffectItem.Deleted)
                m_EffectItem.Delete();

            base.OnDelete();
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Captured && m_CapTown != null)
            {
                LabelTo(from, "[" + m_CapTown.HomeFaction.Definition.FriendlyName + "]");
            }

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from) {
            if (!WindBattleground.IsActive()) {
                from.SendMessage("The brazier ignition clicks without lighting.");
                return;
            }

            if (!from.InRange(this, 3)) {
                SendLocalizedMessageTo(from, 500295); //You are too far away to do that.
                return;
            }

            var fromTown = Town.Find(from);

            if (!m_Captured) {
                from.SendMessage("You begin lighting the flame.");
                new BrazierLightSpell(this, from).Cast();
            } else if (fromTown != null && m_CapTown != null && fromTown != CapTown) {
                from.SendMessage("You begin dousing the flame.");
                new BrazierDouseSpell(this, from).Cast();
            } else
                from.SendMessage("You cannot douse this.");
        }


        public bool OnLight(Mobile m)
        {
            return WindBattleground.TryCapture(this, m);
        }

        public void OnDouse()
        {
            WindBattleground.OnDouse(this);
            Captured = false;
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 ); //version

            Town.WriteReference(writer, m_CapTown);
            writer.WriteItem(m_EffectItem);
            writer.Write((bool)m_Captured);
            writer.Write((string)m_LocationIdentifier);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_CapTown = Town.ReadReference(reader);
                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            Faction.ReadReference(reader);
                        m_EffectItem = reader.ReadItem();
                        m_Captured = reader.ReadBool();
                        m_LocationIdentifier = reader.ReadString();
                        
                    }break;
            }
            

            if (!WindBattleground.Braziers.Contains(this))
                WindBattleground.Braziers.Add(this);
		}

        private class BrazierSpell : Spell
        {
            private static SpellInfo m_Info = new SpellInfo("Wind Brazier Spell", "" );

            protected WindBrazier m_Brazier;
            protected Mobile m_Caster;

            public BrazierSpell(WindBrazier brazier, Mobile caster)
                : base(caster, null, m_Info)
            {
                m_Caster = caster;
                m_Brazier = brazier;
            }

            public override bool ClearHandsOnCast { get { return true; } }
            public override bool RevealOnCast { get { return true; } }

            public override TimeSpan GetCastRecovery()
            {
                return TimeSpan.FromSeconds(5);
            }

            public override double CastDelayFastScalar { get { return 0; } }

            public override TimeSpan CastDelayBase
            {
                get
                {
                    return TimeSpan.FromSeconds(10);
                }
            }

            public override int GetMana(){return 0;}

            public override bool ConsumeReagents(){return false;}

            public override bool CheckFizzle() { return true;}

            private bool m_Stop;

            public void Stop()
            {
                m_Stop = true;
                Disturb(DisturbType.Hurt, false, false);
            }

            public override bool CheckDisturb(DisturbType type, bool checkFirst, bool resistable)
            {
                if (type == DisturbType.EquipRequest || type == DisturbType.UseRequest/* || type == DisturbType.Hurt*/ )
                    return false;

                return true;
            }

            public override void DoHurtFizzle()
            {
                if (!m_Stop)
                    base.DoHurtFizzle();
            }

            public override void DoFizzle()
            {
                if (!m_Stop)
                    base.DoFizzle();
            }

            public override void OnDisturb(DisturbType type, bool message)
            {
                if (message && !m_Stop)
                    Caster.SendMessage("You have been disrupted!"); // You have been disrupted while attempting to summon your pet!
            }

            public override void OnCast()
            {
                throw new NotImplementedException();
            }

        }

        private class BrazierDouseSpell : BrazierSpell
        {

            public BrazierDouseSpell(WindBrazier brazier, Mobile caster)
                : base(brazier, caster)
            {
            }

            public override void OnCast()
            {
                if (m_Brazier.Captured)
                {
                    m_Brazier.Captured = false;
                    Caster.SendMessage("You have successfully doused the brazier!");
                }
                FinishSequence();
            }
        }

        private class BrazierLightSpell : BrazierSpell
        {
            public BrazierLightSpell(WindBrazier brazier, Mobile caster) : base(brazier, caster) { }

            public override void OnCast()
            {
                if (!m_Brazier.Captured)
                {
                    m_Brazier.OnLight(m_Caster);
                    Caster.SendMessage("You have successfully lit the brazier!");
                }
                FinishSequence();
            }
        }

        private class InternalTimer : Timer
        {
            private WindBrazier m_Brazier;

            public InternalTimer(WindBrazier brazier)
                : base(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(2))
            {
                m_Brazier = brazier;
            }

            protected override void OnTick()
            {
                if (--m_Brazier.m_SparkHits <= 0)
                    Stop();
            }
        }
	}


}