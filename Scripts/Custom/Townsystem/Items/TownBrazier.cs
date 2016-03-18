using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Network;
using Server.Mobiles;
using Server.Achievements;

namespace Server.Custom.Townsystem
{
    public class TownBrazier : Brazier
    {
        private class CaptureFlame : Item
        {
            public TownBrazier Brazier;

            public CaptureFlame(TownBrazier b)
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
                Brazier = (TownBrazier)reader.ReadItem();
            }

        }
        private Town m_Town;
        private bool m_Captured;
        private Town m_CapTown;
        private string m_LocationIdentifier;
        private Item m_EffectItem;
        private BaseFactionGuard m_Guard;
        public static TimeSpan TravelRestrictionDuration = TimeSpan.FromMinutes(2);
        public DateTime LastLight = DateTime.UtcNow;

        public override bool ForceShowProperties { get { return true; } }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public string BrazierLocationName
        {
            get { return m_LocationIdentifier; }
            set { m_LocationIdentifier = value; }
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town Town
        {
            get { return m_Town; }
            set { m_Town = value; Invalidate(); }
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
                        m_EffectItem.MoveToWorld(new Point3D(X, Y, Z + 7), Map);
                    }
                }
            }
        }

        [Constructable]
        public TownBrazier()
            : base()
        {
            Name = "a town brazier";
            Movable = false;
            m_Captured = false;
            m_LocationIdentifier = "(default)";
        }

        public override void OnMapChange()
        {
            base.OnMapChange();
            if (Location == Point3D.Zero) return;
            Town = Town.FromRegion(Region.Find(Location, Map));
            if (Town == null)
            {
                Delete();
                return;
            }
            Town.Braziers.Add(this);
        }

        public TownBrazier(Town town)
            : base()
        {
            Movable = false;
            Town = town;
        }

        public TownBrazier(Serial serial)
            : base(serial)
        {
        }

        public override void OnDelete()
        {
            if (m_Town != null && m_Town.Braziers.Contains(this))
                m_Town.Braziers.Remove(this);

            if (m_EffectItem != null && !m_EffectItem.Deleted)
            {
                m_EffectItem.Delete();
            }

            base.OnDelete();
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Captured && m_CapTown != null)
            {
                LabelTo(from, "[" + m_CapTown.Definition.FriendlyName + "]");
            }

            base.OnSingleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(this, 3))
            {
                SendLocalizedMessageTo(from, 500295); //You are too far away to do that.
                return;
            }
            var player = from as PlayerMobile;
            var playerTown = Town.Find(from);

            if (player == null || playerTown == null)
                return;

            if (!OCTimeSlots.IsActiveTown(this.Town))
            {
                from.SendMessage("This town is not currently active for town militia wars.");
            }
            else if (!m_Captured)
            {
                if (!player.IsInStatLoss)
                    new BrazierLightSpell(this, from).Cast();
                else
                    from.SendMessage("You cannot light a brazier while suffering from stat loss.");
            }
            else if (m_CapTown != null && playerTown != m_CapTown)
            {
                if (!player.IsInStatLoss)
                    new BrazierDouseSpell(this, from).Cast();
                else
                    from.SendMessage("You cannot douse a brazier while suffering from stat loss.");
            }
            else
                from.SendMessage("You cannot douse this.");
        }

        public bool OnLight(Mobile m)
        {
            if (Town == null) { Delete(); return false; }

            if (Town.TryCapture(this, m))
            {
                var player = m as PlayerMobile;
                if (player != null)
                {
                    player.RecallRestrictionExpiration = DateTime.UtcNow + TravelRestrictionDuration;
                    player.HideRestrictionExpiration = DateTime.UtcNow + TravelRestrictionDuration;
                }
                LastLight = DateTime.UtcNow;
                return true;
            }
            else
            return false;
        }

        public void Invalidate()
        {
            Name = m_Town == null ? "a town brazier" : m_Town.Definition.TownBrazierName;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)4); //version

            //version 4 - guard reference
            writer.Write(m_Guard);

            //version 3 - changed from version 2
            Town.WriteReference(writer, m_CapTown);

            //version 1
            writer.WriteItem(m_EffectItem);
            Town.WriteReference(writer, m_Town);
            writer.Write((bool)m_Captured);
            writer.Write((string)m_LocationIdentifier);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4:
                    {
                        m_Guard = reader.ReadMobile() as FactionHenchman;
                        goto case 3;
                    }
                case 3:
                    {
                        m_CapTown = Town.ReadReference(reader);
                        goto case 2;
                    }
                case 2:
                    {
                        if (version < 3)
                            Faction.ReadReference(reader);
                        goto case 1;
                    }
                case 1:
                    {
                        m_EffectItem = reader.ReadItem();
                        goto case 0;
                    }
                case 0:
                    {
                        Town = Town.ReadReference(reader);
                        m_Captured = reader.ReadBool();
                        m_LocationIdentifier = reader.ReadString();
                    } break;
            }
        }

        private class BrazierLightSpell : Spell
        {
            private static SpellInfo m_Info = new SpellInfo("Town Brazier Douse", "", Reagent.GraveDust);

            private TownBrazier m_Brazier;
            private Mobile m_Caster;

            public BrazierLightSpell(TownBrazier brazier, Mobile caster)
                : base(caster, null, m_Info)
            {
                m_Caster = caster;
                m_Brazier = brazier;
                m_Caster.SendMessage("You begin lighting the flame.");
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

            public override int GetMana() { return 0; }

            public override bool ConsumeReagents() { return false; }

            public override bool CheckFizzle() { return true; }

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
                    Caster.SendMessage("You have been disrupted while attempting to light the brazier!"); // You have been disrupted while attempting to summon your pet!
            }

            public override void OnCast()
            {
                if (!m_Brazier.Captured)
                {
                    Container pack = m_Caster.Backpack;
                    if (pack == null)
                        return;

                    if (pack.ConsumeTotal(typeof(BrazierDust), 1))
                        m_Brazier.OnLight(m_Caster);
                    else
                        m_Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 502630); // More reagents are needed for this spell.
                }

                FinishSequence();
            }
        }

        private class BrazierDouseSpell : Spell
        {
            private static SpellInfo m_Info = new SpellInfo("Town Brazier Douse", "");

            private TownBrazier m_Brazier;
            private Mobile m_Caster;

            public BrazierDouseSpell(TownBrazier brazier, Mobile caster)
                : base(caster, null, m_Info)
            {
                m_Caster = caster;
                m_Brazier = brazier;
                caster.SendMessage("You begin to douse the brazier.");
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

            public override int GetMana() { return 0; }

            public override bool ConsumeReagents() { return false; }

            public override bool CheckFizzle() { return true; }

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
                    Caster.SendMessage("You have been disrupted while attempting to douse the brazier!"); // You have been disrupted while attempting to summon your pet!
            }

            public override void OnCast()
            {
                if (m_Brazier.Captured)
                {
                    m_Brazier.Captured = false;
                    Caster.SendMessage("You have successfully doused the brazier!");

                    // IPY ACHIEVEMENT
                    AchievementSystem.Instance.TickProgress(Caster, AchievementTriggers.Trigger_DouseBrazier);
                    DailyAchievement.TickProgress(Category.PvP, (PlayerMobile)Caster, PvPCategory.DouseBraziers);
                    // IPY ACHIEVEMENT
                    OCLeaderboard.RegisterBrazier(Caster);

                    if (Caster is PlayerMobile)
                    {
                        var player = Caster as PlayerMobile;
                        player.RecallRestrictionExpiration = DateTime.UtcNow + TravelRestrictionDuration;
                    }
                }
                FinishSequence(); // always finish the sequence, fix issue where multiple dousers would be unable to cast following dousing.
            }
        }

    }


}