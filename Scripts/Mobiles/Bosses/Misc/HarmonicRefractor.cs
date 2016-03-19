using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Commands;
using Server.Custom;

namespace Server.Items
{
    public class HarmonicRefractor : BreakableStatic
    {
        public Mobile m_Owner;
        public InternalTimer m_Timer;

        public virtual int MainHue { get { return 2588; } }
        public virtual int AltHue { get { return 2589; } }

        public TimeSpan HueChangeDelay = TimeSpan.FromSeconds(.33);

        public static List<HarmonicRefractor> m_Instances = new List<HarmonicRefractor>();

        [Constructable]
        public HarmonicRefractor(): base()
        {
            Name = "large harmonic refractor";

            int itemStyle = Utility.RandomList(12262, 12263);

            InteractionRange = 1;
            InteractionDelay = 5;
            MinInteractDamage = 5;
            MaxInteractDamage = 10;

            InteractDamageScalar = 1.0;
            WeaponDamageScalar = 1.0;
            LockpickDamageScalar = 0;
            ObjectBreakingDeviceDamageScalar = 1.0;
            MiningDamageScalar = 0.5;
            LumberjackingDamageScalar = 0.5;

            MaxHitPoints = 1000;
            HitPoints = 1000;

            AddLOSBlocker = false;

            HitSound = 0x38E;

            ItemID = itemStyle;
            Hue = 2588;

            NormalItemId = itemStyle;
            NormalHue = 2588;

            LightlyDamagedPercent = .666;
            LightlyDamagedSound = 0x38E;
            LightlyDamagedItemId = itemStyle;
            LightlyDamagedHue = 2588;

            HeavilyDamagedPercent = .333;
            HeavilyDamagedSound = 0x38E;
            HeavilyDamagedItemId = itemStyle;
            HeavilyDamagedHue = 2588;

            BrokenSound = 0x665;
            BrokenItemId = itemStyle;
            BrokenHue = 2588;

            DeleteOnBreak = true;
            CreateTimedStaticAfterBreak = false;

            RevealNearbyHiddenItemsOnBreak = false;
            RevealNearbyHiddenItemsOnBreakRadius = 0;
            RefreshNearbyMovables = false;
            RefreshNearbyMovablesRadius = 0;

            //-----

            m_Instances.Add(this);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }

        public class InternalTimer : Timer
        {
            public HarmonicRefractor m_HarmonicRefractor;

            public InternalTimer(HarmonicRefractor harmonicRefractor): base(harmonicRefractor.HueChangeDelay, harmonicRefractor.HueChangeDelay)
            {
                m_HarmonicRefractor = harmonicRefractor;

                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_HarmonicRefractor == null)
                {
                    Stop();
                    return;
                }

                if (m_HarmonicRefractor.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_HarmonicRefractor.Hue == m_HarmonicRefractor.MainHue)
                    m_HarmonicRefractor.Hue = m_HarmonicRefractor.AltHue;
                else
                    m_HarmonicRefractor.Hue = m_HarmonicRefractor.MainHue;
            }
        }

        public override void Interact(Mobile from, BreakableStatic.InteractionType interactionType)
        {
            base.Interact(from, interactionType);
        }

        public override void AfterInteract(Mobile from, InteractionType interactionType)
        {
            if (from == null) return;
            if (this == null) return;
            if (Deleted) return;

            for (int a = 0; a < 3; a++)
            {
                TimedStatic crystalShards = new TimedStatic(22328, 3);
                crystalShards.Name = "harmonic shards";
                crystalShards.Hue = 2588;

                Point3D shardLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                shardLocation.Z = Map.GetSurfaceZ(shardLocation, 30);

                SpellHelper.AdjustField(ref shardLocation, Map, 12, false);
                crystalShards.MoveToWorld(shardLocation, Map);
            }
        }

        public override void BeforeBreak(Mobile from, InteractionType interactionType)
        {
            int fragments = 20;

            for (int a = 0; a < fragments; a++)
            {
                TimedStatic crystalShards = new TimedStatic(22328, 3);
                crystalShards.Name = "harmonic shards";
                crystalShards.Hue = 2588;

                Point3D shardLocation = new Point3D(Location.X + Utility.RandomList(-2, 2), Location.Y + Utility.RandomList(-2, 2), Location.Z);
                shardLocation.Z = Map.GetSurfaceZ(shardLocation, 30);

                crystalShards.MoveToWorld(shardLocation, Map);
            }

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {              
            });

            base.BeforeBreak(from, interactionType);
        }

        public override void OnDelete()
        {
            if (m_Instances.Contains(this))
                m_Instances.Remove(this);

            base.OnDelete();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();   

            Delete();
        }

        public HarmonicRefractor(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Owner = reader.ReadMobile();
            }

            //---------  

            m_Instances.Add(this);

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}
