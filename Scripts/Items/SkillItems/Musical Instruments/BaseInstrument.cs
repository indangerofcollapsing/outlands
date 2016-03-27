using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Achievements;
using Server.ContextMenus;
using Server.Items;

namespace Server.Items
{
    public delegate void InstrumentPickedCallback(Mobile from, BaseInstrument instrument);
    
    public abstract class BaseInstrument : Item, ICraftable
    {
        public virtual int InitMinUses { get { return 200; } }
        public virtual int InitMaxUses { get { return 300; } }

        public static double ExceptionalQualitySkillBonus = 5;
        public static double SlayerSkillBonus = 15;

        private int m_SuccessSound;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessSound
        {
            get { return m_SuccessSound; }
            set { m_SuccessSound = value; }
        }

        private int m_FailureSound;
        [CommandProperty(AccessLevel.GameMaster)]
        public int FailureSound
        {
            get { return m_FailureSound; }
            set { m_FailureSound = value; }
        }

        private SlayerGroupType m_SlayerGroup;
        [CommandProperty(AccessLevel.GameMaster)]
        public SlayerGroupType SlayerGroup
        {
            get { return m_SlayerGroup; }
            set { m_SlayerGroup = value; InvalidateProperties(); }
        }
        
        private int m_UsesRemaining;
        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public void ScaleUses()
        {
            UsesRemaining = (int)((double)UsesRemaining * GetUsesScalar());
        }

        public void UnscaleUses()
        {
            UsesRemaining = (int)((double)UsesRemaining * GetUsesScalar());
        }

        public double GetUsesScalar()
        {
            if (Quality == Quality.Exceptional)
                return 1.5;

            return 1;
        }

        public void ConsumeUse(Mobile from)
        {
            if (UsesRemaining > 1)
                --UsesRemaining;

            else
            {
                if (from != null)
                    from.SendLocalizedMessage(502079); // The instrument played its last tune.

                Delete();
            }
        }

        private static Hashtable m_Instruments = new Hashtable();

        public static BaseInstrument GetInstrument(Mobile from)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)
                return null;

            BaseInstrument item = pm_From.LastInstrument;

            if (item == null)
                return null;

            if (item.Deleted)
                return null;

            if (!item.IsChildOf(from.Backpack))
                return null;

            return item;
        }

        public static int GetBardRange(Mobile bard, SkillName skill)
        {
            return 12;
        }

        public static void PickInstrument(Mobile from, InstrumentPickedCallback callback)
        {
            BaseInstrument instrument = GetInstrument(from);

            if (instrument != null)
            {
                if (callback != null)
                    callback(from, instrument);
            }

            else
            {
                from.SendLocalizedMessage(500617); // What instrument shall you play?
                from.BeginTarget(1, false, TargetFlags.None, new TargetStateCallback(OnPickedInstrument), callback);
            }
        }

        public static void OnPickedInstrument(Mobile from, object targeted, object state)
        {
            BaseInstrument instrument = targeted as BaseInstrument;

            if (instrument == null)
                from.SendLocalizedMessage(500619); // That is not a musical instrument.

            else
            {
                SetInstrument(from, instrument);

                InstrumentPickedCallback callback = state as InstrumentPickedCallback;

                if (callback != null)
                    callback(from, instrument);
            }
        }

        public static void SetInstrument(Mobile from, BaseInstrument item)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From != null && item != null)
                pm_From.LastInstrument = item;
        }

        public BaseInstrument(int itemID, int wellSound, int badlySound): base(itemID)
        {
            m_SuccessSound = wellSound;
            m_FailureSound = badlySound;
            UsesRemaining = Utility.RandomMinMax(InitMinUses, InitMaxUses);
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            int oldUses = m_UsesRemaining;

            base.GetProperties(list);

            if (Quality == Quality.Exceptional)
                list.Add(1060636); // exceptional

            list.Add(1060584, m_UsesRemaining.ToString()); // uses remaining: ~1_val~

            if (m_UsesRemaining != oldUses)
                Timer.DelayCall(TimeSpan.Zero, new TimerCallback(InvalidateProperties));            
        }

        public override void OnSingleClick(Mobile from)
        {            
            ArrayList attrs = new ArrayList();

            if (DisplayLootType)
            {
                if (LootType == LootType.Blessed)
                    attrs.Add(new EquipInfoAttribute(1038021)); // blessed
                else if (LootType == LootType.Cursed)
                    attrs.Add(new EquipInfoAttribute(1049643)); // cursed
            }

            if (Quality == Quality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)Quality));

            int number;

            if (Name == null)
                number = LabelNumber;

            else
            {
                this.LabelTo(from, Name);
                number = 1041000;
            }

            //if (attrs.Count == 0 && CraftedBy == null  Name != null)
            //return;

            EquipmentInfo eqInfo = new EquipmentInfo(number, CraftedBy, false, (EquipInfoAttribute[])attrs.ToArray(typeof(EquipInfoAttribute)));

            from.Send(new DisplayEquipmentInfo(this, eqInfo));            
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.InRange(GetWorldLocation(), 1))
                from.SendLocalizedMessage(500446); // That is too far away.

            else if (from.BeginAction(typeof(BaseInstrument)))
            {
                SetInstrument(from, this);

                new InternalTimer(from).Start();

                if (CheckMusicianship(from))
                    PlayInstrumentWell(from);
                else
                    PlayInstrumentBadly(from);
            }

            else
                from.SendLocalizedMessage(500119); // You must wait to perform another action            
        }

        public static bool CheckMusicianship(Mobile m)
        {
            m.CheckSkill(SkillName.Musicianship, 0.0, 120.0, 1.0);

            return ((m.Skills[SkillName.Musicianship].Value / 100) > Utility.RandomDouble());
        }

        public virtual void PlayInstrumentWell(Mobile from)
        {
            from.PlaySound(m_SuccessSound);
        }

        public void PlayInstrumentBadly(Mobile from)
        {
            from.PlaySound(m_FailureSound);
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;

            public InternalTimer(Mobile from): base(TimeSpan.FromSeconds(6.0))
            {
                m_From = from;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                m_From.EndAction(typeof(BaseInstrument));
            }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Quality = (Quality)quality;

            if (makersMark)
                DisplayCrafter = true;

            return quality;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
        }

        public BaseInstrument(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            
            //Version 0
            writer.Write((int)m_SlayerGroup);
            writer.Write(UsesRemaining);
            writer.Write(m_SuccessSound);
            writer.Write(m_FailureSound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_SlayerGroup = (SlayerGroupType)reader.ReadInt();
                m_UsesRemaining = reader.ReadInt();
                m_SuccessSound = reader.ReadInt();
                m_FailureSound = reader.ReadInt();
            }
        }
    }
}