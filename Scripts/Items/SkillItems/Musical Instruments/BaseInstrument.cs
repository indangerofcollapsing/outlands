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

    public enum InstrumentQuality
    {
        Low,
        Regular,
        Exceptional
    }

    public abstract class BaseInstrument : Item, ICraftable
    {
        public virtual int InitMinUses { get { return 350; } }
        public virtual int InitMaxUses { get { return 450; } }

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

        private InstrumentQuality m_Quality;   
        [CommandProperty(AccessLevel.GameMaster)]
        public InstrumentQuality Quality
        {
            get { return m_Quality; }
            set { UnscaleUses(); m_Quality = value; InvalidateProperties(); ScaleUses(); }
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
            UsesRemaining = (UsesRemaining * GetUsesScalar()) / 100;
        }

        public void UnscaleUses()
        {
            UsesRemaining = (UsesRemaining * 100) / GetUsesScalar();
        }

        public int GetUsesScalar()
        {
            if (m_Quality == InstrumentQuality.Exceptional)
                return 200;

            return 100;
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
            return 8 + (int)(bard.Skills[skill].Value / 15);
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

            if (m_Quality == InstrumentQuality.Exceptional)
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

            if (m_Quality == InstrumentQuality.Exceptional)
                attrs.Add(new EquipInfoAttribute(1018305 - (int)m_Quality));          

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
            m.CheckSkill(SkillName.Musicianship, 0.0, 120.0);

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

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(6.0))
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
            Quality = (InstrumentQuality)quality;

            if (makersMark)
                CraftedBy = from;

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

            writer.WriteEncodedInt((int)m_Quality);
            writer.WriteEncodedInt((int)m_SlayerGroup);
            writer.WriteEncodedInt((int)UsesRemaining);
            writer.WriteEncodedInt((int)m_SuccessSound);
            writer.WriteEncodedInt((int)m_FailureSound);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Quality = (InstrumentQuality)reader.ReadInt();
                m_SlayerGroup = (SlayerGroupType)reader.ReadInt();
                m_UsesRemaining = reader.ReadInt();
                m_SuccessSound = reader.ReadInt();
                m_FailureSound = reader.ReadInt();
            }
        }        
    }
}
