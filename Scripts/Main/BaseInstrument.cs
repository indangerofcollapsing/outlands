using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Craft;

using Server.ContextMenus;
using Server.Items;

namespace Server.Items
{
    public delegate void InstrumentPickedCallback(Mobile from, BaseInstrument instrument);

    public abstract class BaseInstrument : Item, ICraftable
    {
        public virtual int InitMinUses { get { return 200; } }
        public virtual int InitMaxUses { get { return 300; } }

        public static double DiscordanceModifier = .25;

        public static double ExceptionalQualitySkillBonus = 5;
        public static double SlayerSkillBonus = 15;

        public static int PacifiedTextHue = 2599;
        public static int ProvokedTextHue = 149;
        public static int DiscordedTextHue = 2601;

        private InstrumentDurabilityLevel m_DurabilityLevel;
        [CommandProperty(AccessLevel.GameMaster)]
        public InstrumentDurabilityLevel DurabilityLevel
        {
            get { return m_DurabilityLevel; }
            set
            {
                bool changeOcurred = (m_DurabilityLevel == value);

                m_DurabilityLevel = value;

                if (changeOcurred)
                {
                    UnscaleUses();
                    ScaleUses();
                }
            }
        }

        private InstrumentArtistryLevel m_ArtistryLevel;
        [CommandProperty(AccessLevel.GameMaster)]
        public InstrumentArtistryLevel ArtistryLevel
        {
            get { return m_ArtistryLevel; }
            set { m_ArtistryLevel = value; }
        }

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

        public override void QualityChange()
        {
            UnscaleUses();
            ScaleUses();
        }

        public override void ResourceChange()
        {
            UnscaleUses();
            ScaleUses();
        }

        public void UnscaleUses()
        {
            UsesRemaining = Utility.RandomMinMax(InitMinUses, InitMaxUses);
        }

        public void ScaleUses()
        {
            double baseUsesRemaining = (double)(UsesRemaining = Utility.RandomMinMax(InitMinUses, InitMaxUses));

            switch (Quality)
            {
                case Quality.Low: baseUsesRemaining -= 50; break;
                case Quality.Regular: baseUsesRemaining += 0; break;
                case Quality.Exceptional: baseUsesRemaining += 100; break;
            }

            switch (Resource)
            {
                case CraftResource.RegularWood: baseUsesRemaining += 0; break;
                case CraftResource.OakWood: baseUsesRemaining += 100; break;
                case CraftResource.AshWood: baseUsesRemaining += 200; break;
                case CraftResource.YewWood: baseUsesRemaining += 300; break;
                case CraftResource.Heartwood: baseUsesRemaining += 400; break;
                case CraftResource.Bloodwood: baseUsesRemaining += 500; break;
                case CraftResource.Frostwood: baseUsesRemaining += 600; break;
            }

            switch (m_DurabilityLevel)
            {
                case InstrumentDurabilityLevel.Durable: baseUsesRemaining += 100; break;
                case InstrumentDurabilityLevel.Substantial: baseUsesRemaining += 150; break;
                case InstrumentDurabilityLevel.Massive: baseUsesRemaining += 200; break;
                case InstrumentDurabilityLevel.Fortified: baseUsesRemaining += 250; break;
                case InstrumentDurabilityLevel.Indestructible: baseUsesRemaining += 300; break;
            }

            UsesRemaining = (int)(Math.Ceiling((double)baseUsesRemaining));
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

        public static bool CheckSkillGain(double successChance)
        {
            if (successChance > 0 && successChance < 1.0)
                return true;

            return false;
        }

        public static string GetFailureMessage(double successChance, SkillName skill)
        {
            string failureMessage = "";

            switch (skill)
            {
                case SkillName.Discordance:
                    failureMessage = "You fail to disrupt your opponent. You estimate the task to be beyond your skill.";

                    if (successChance > 0)
                        failureMessage = "You fail to disrupt your opponent. You estimate the task to be near impossible.";

                    if (successChance >= .05)
                        failureMessage = "You fail to disrupt your opponent. You estimate the task to be very difficult.";

                    if (successChance >= .25)
                        failureMessage = "You fail to disrupt your opponent. You estimate the task to be somewhat challenging.";

                    if (successChance >= .50)
                        failureMessage = "You fail to disrupt your opponent. You estimate the task to be fairly reasonable.";

                    if (successChance >= .75)
                        failureMessage = "You fail to disrupt your opponent. You estimate the task to be easy.";

                    if (successChance >= .95)
                        failureMessage = "You fail to disrupt your opponent. You estimate the task to be trivial.";
                    break;

                case SkillName.Peacemaking:
                    failureMessage = "You fail to pacify your opponent. You estimate the task to be beyond your skill.";

                    if (successChance > 0)
                        failureMessage = "You fail to pacify your opponent. You estimate the task to be near impossible.";

                    if (successChance >= .05)
                        failureMessage = "You fail to pacify your opponent. You estimate the task to be very difficult.";

                    if (successChance >= .25)
                        failureMessage = "You fail to pacify your opponent. You estimate the task to be somewhat challenging.";

                    if (successChance >= .50)
                        failureMessage = "You fail to pacify your opponent. You estimate the task to be fairly reasonable.";

                    if (successChance >= .75)
                        failureMessage = "You fail to pacify your opponent. You estimate the task to be easy.";

                    if (successChance >= .95)
                        failureMessage = "You fail to pacify your opponent. You estimate the task to be trivial.";
                    break;

                case SkillName.Provocation:
                    failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be beyond your skill.";

                    if (successChance > 0)
                        failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be near impossible.";

                    if (successChance >= .05)
                        failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be very difficult.";

                    if (successChance >= .25)
                        failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be somewhat challenging.";

                    if (successChance >= .50)
                        failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be fairly reasonable.";

                    if (successChance >= .75)
                        failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be easy.";

                    if (successChance >= .95)
                        failureMessage = "You fail to incite anger amongst your opponents. You estimate the task to be trivial.";
                    break;
            }

            return failureMessage;
        }

        public static double GetBardBonusSkill(Mobile from, Mobile target, BaseInstrument instrument)
        {
            double bonusBardSkill = 0;

            if (instrument.Quality == Quality.Low)
                bonusBardSkill -= 10;

            if (instrument.Quality == Quality.Exceptional)
                bonusBardSkill += 10;

            switch (instrument.ArtistryLevel)
            {
                case InstrumentArtistryLevel.Melodist: bonusBardSkill += 5; break;
                case InstrumentArtistryLevel.Jongleur: bonusBardSkill += 10; break;
                case InstrumentArtistryLevel.Minstrel: bonusBardSkill += 15; break;
                case InstrumentArtistryLevel.Troubadour: bonusBardSkill += 20; break;
                case InstrumentArtistryLevel.Balladeer: bonusBardSkill += 25; break;
            }

            BaseCreature bc_Target = target as BaseCreature;

            if (bc_Target != null)
            {
                if (bc_Target.SlayerGroup == instrument.SlayerGroup)
                    bonusBardSkill += 15;
            }

            return bonusBardSkill;
        }

        public static double GetBardSuccessChance(double effectiveBardSkill, double targetDifficulty)
        {
            double baseSuccessChance = (effectiveBardSkill - (targetDifficulty * 1.5)) * .02;
            double minimumSuccessChance = effectiveBardSkill * .0015;

            if (baseSuccessChance < minimumSuccessChance)
                return minimumSuccessChance;

            return baseSuccessChance;
        }

        public static TimeSpan GetBardDuration(Mobile target, double targetDifficulty)
        {
            BaseCreature bc_Target = target as BaseCreature;

            double duration = 60;

            double effectScalar = 1.0;
            double minimumEffectScalar = .20;
            double tamedEffectScalar = .5;
            double difficultyScalar = .01;

            if (bc_Target != null)
            {
                effectScalar = 1 - (targetDifficulty * difficultyScalar);

                if (effectScalar < minimumEffectScalar)
                    effectScalar = minimumEffectScalar;                

                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    effectScalar *= tamedEffectScalar;
            }

            else
                duration = 1;

            return TimeSpan.FromSeconds(duration * effectScalar);
        }

        public static void SetInstrument(Mobile from, BaseInstrument item)
        {
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From != null && item != null)
                pm_From.LastInstrument = item;
        }

        public BaseInstrument(int itemID, int wellSound, int badlySound)
            : base(itemID)
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

        public override void DisplayLabelName(Mobile from)
        {
            if (from == null)
                return;

            bool isMagical = SlayerGroup != SlayerGroupType.None || m_DurabilityLevel != InstrumentDurabilityLevel.Regular || m_ArtistryLevel != InstrumentArtistryLevel.Regular;

            string displayName = "";

            if (isMagical && !Identified && from.AccessLevel == AccessLevel.Player)
                LabelTo(from, "unidentified " + Name);

            else
            {
                if (Quality == Quality.Exceptional)
                    displayName += "exceptional ";

                if (DurabilityLevel != InstrumentDurabilityLevel.Regular)
                    displayName += DurabilityLevel.ToString().ToLower() + " ";

                switch (ArtistryLevel)
                {
                    case InstrumentArtistryLevel.Melodist: displayName += "melodist "; break;
                    case InstrumentArtistryLevel.Jongleur: displayName += "jongleur "; break;
                    case InstrumentArtistryLevel.Minstrel: displayName += "minstrel "; break;
                    case InstrumentArtistryLevel.Troubadour: displayName += "troubadour "; break;
                    case InstrumentArtistryLevel.Balladeer: displayName += "balladeer "; break;
                }

                displayName += Name;

                if (SlayerGroup != SlayerGroupType.None)
                    displayName += " of " + SlayerGroup.ToString().ToLower() + " enticement";

                LabelTo(from, displayName);
            }

            LabelTo(from, UsesRemaining.ToString() + " uses remaining");
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
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
            Quality = (Quality)quality;

            if (makersMark)
                DisplayCrafter = true;

            Type resourceType = typeRes;

            if (resourceType == null)
                resourceType = craftItem.Resources.GetAt(0).ItemType;

            Resource = CraftResources.GetFromType(resourceType);

            return quality;
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
        }

        public BaseInstrument(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            //Version 0
            writer.Write((int)m_DurabilityLevel);
            writer.Write((int)m_ArtistryLevel);
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
                m_DurabilityLevel = (InstrumentDurabilityLevel)reader.ReadInt();
                m_ArtistryLevel = (InstrumentArtistryLevel)reader.ReadInt();
                m_SlayerGroup = (SlayerGroupType)reader.ReadInt();
                m_UsesRemaining = reader.ReadInt();
                m_SuccessSound = reader.ReadInt();
                m_FailureSound = reader.ReadInt();
            }
        }
    }

    public enum InstrumentDurabilityLevel
    {
        Regular,
        Durable,
        Substantial,
        Massive,
        Fortified,
        Indestructible
    }

    public enum InstrumentArtistryLevel
    {
        Regular,
        Melodist,
        Jongleur,
        Minstrel,
        Troubadour,
        Balladeer
    }
}