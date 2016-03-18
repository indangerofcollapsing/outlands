using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Custom;
using Server.Spells;
using Server.Network;

namespace Server.Items
{
    public class UOACZCorruptionSourcestone : UOACZBreakableStatic
    {
        public static List<UOACZCorruptionSourcestone> m_Instances = new List<UOACZCorruptionSourcestone>();

        public override bool AllowHumanDamage { get { return true; } }

        public override int OverrideNormalItemId { get { return 730; } }
        public override int OverrideNormalHue { get { return 2411; } }

        public override int OverrideLightlyDamagedItemId { get { return 730; } }
        public override int OverrideLightlyDamagedHue { get { return 2411; } }

        public override int OverrideHeavilyDamagedItemId { get { return 730; } }
        public override int OverrideHeavilyDamagedHue { get { return 2411; } }

        public override int OverrideBrokenItemId { get { return 730; } }
        public override int OverrideBrokenHue { get { return 2411; } }

        public static int DefaultMaxHitPoints = 20000;

        private double m_NextDamageStateNotification = .9;
        [CommandProperty(AccessLevel.Counselor)]
        public double NextDamageStateNotification
        {
            get { return m_NextDamageStateNotification; }
            set { m_NextDamageStateNotification = value; }
        }

        public double NotificationInterval = .1;

        [Constructable]
        public UOACZCorruptionSourcestone(): base()
        {
            Name = "corruption sourcestone";
            ItemID = 730;

            DeleteOnBreak = true;

            HitSound = 0x38E;
            LightlyDamagedSound = 0x38E;
            HeavilyDamagedSound = 0x38E;
            BrokenSound = 0x665;

            MaxHitPoints = DefaultMaxHitPoints;
            HitPoints = DefaultMaxHitPoints;

            m_Instances.Add(this);
        }

        public static List<UOACZCorruptionSourcestone> GetActiveInstances()
        {
            List<UOACZCorruptionSourcestone> m_ActiveInstances = new List<UOACZCorruptionSourcestone>();

            for (int a = 0; a < m_Instances.Count; a++)
            {
                UOACZCorruptionSourcestone instance = m_Instances[a];

                if (instance.Deleted)
                    continue;

                if (UOACZRegion.ContainsItem(instance))
                    m_ActiveInstances.Add(instance);
            }

            return m_ActiveInstances;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Corruption Sourcestone");
            LabelTo(from, "[Durability: " + HitPoints.ToString() + "/" + MaxHitPoints.ToString() + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            if (player.IsUOACZUndead)
                return;

            if (player.IsUOACZHuman)
            {
                if (!from.InRange(GetWorldLocation(), InteractionRange))
                {
                    from.SendMessage("That is too far away.");
                    return;
                }

                base.OnDoubleClick(from);

                return;
            }
        }

        public override void ReceiveDamage(Mobile from, int damage, BreakableStatic.InteractionType interactionType)
        {
            base.ReceiveDamage(from, damage, interactionType);

            double hitPointsPercent = (double)HitPoints / (double)MaxHitPoints;

            if (m_NextDamageStateNotification > 0.05 && hitPointsPercent < m_NextDamageStateNotification)
            {
                foreach (NetState state in NetState.Instances)
                {
                    Mobile mobile = state.Mobile;
                    PlayerMobile player = mobile as PlayerMobile;

                    if (player == null)
                        continue;

                    if (UOACZRegion.ContainsMobile(player))
                        player.SendMessage(UOACZSystem.yellowTextHue, "The Corruption Sourcestone is under attack and at " + Utility.CreatePercentageString(m_NextDamageStateNotification) + " durability.");
                }

                m_NextDamageStateNotification -= NotificationInterval;
            }
        }

        public override void BeforeBreak(Mobile from, InteractionType interactionType)
        {
            UOACZEvents.SourceOfCorruptionDamaged(true);
        }

        public UOACZCorruptionSourcestone(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_NextDamageStateNotification);        
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextDamageStateNotification = reader.ReadDouble();

            //--------

            m_Instances.Add(this);
        }
    }
}