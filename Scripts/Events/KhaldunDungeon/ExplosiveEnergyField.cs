using System;
using Server;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
    public class ExplosiveEnergyField : Item
    {
        public int randomSparkle { get { return Utility.RandomList(0x1153, 0x373A, 0x375A, 0x376A, 0x3779); } }
        public int randomUnhuedExplosion { get { return Utility.RandomList(0x36B0, 0x36CA); } }

        public int randomSparkleSound { get { return Utility.RandomList(0x0F8, 0x0F7, 0x0FE, 0x103, 0x104, 0x105); } }
        public int randomExplosionSound { get { return Utility.RandomList(0x11B, 0x11D, 0x307); } }

        private bool inUse = false;

        private AccessLevel m_RequiredAccessLevel = AccessLevel.GameMaster;
        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel RequiredAccessLevel
        {
            get { return m_RequiredAccessLevel; }
            set { m_RequiredAccessLevel = value; }
        }

        private bool m_SameHueCondition = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SameHueCondition
        {
            get { return m_SameHueCondition; }
            set { m_SameHueCondition = value; }
        }

        private bool m_DefaultExplosionHue = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DefaultExplosionHue
        {
            get { return m_DefaultExplosionHue; }
            set { m_DefaultExplosionHue = value; }
        }

        private int m_EffectsHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectsHue
        {
            get { return m_EffectsHue; }
            set { m_EffectsHue = value; }
        }

        private int m_RequiredDistanceToPlayer = 20;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RequiredDistanceToPlayer
        {
            get { return m_RequiredDistanceToPlayer; }
            set { m_RequiredDistanceToPlayer = value; }
        }

        private int m_NumberOfEffects = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NumberOfEffects
        {
            get { return m_NumberOfEffects; }
            set { m_NumberOfEffects = value; }
        }

        private TimeSpan m_EffectDelay = TimeSpan.FromSeconds(.25);
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan EffectDelay
        {
            get { return m_EffectDelay; }
            set { m_EffectDelay = value; }
        }

        private TimeSpan m_FuseTime = TimeSpan.FromSeconds(.1);
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan FuseTime
        {
            get { return m_FuseTime; }
            set { m_FuseTime = value; }
        }

        private int m_EffectDuration = 20;
        [CommandProperty(AccessLevel.GameMaster)]
        public int EffectDuration
        {
            get { return m_EffectDuration; }
            set { m_EffectDuration = value; }
        }

        [Constructable]
        public ExplosiveEnergyField(): base(0x3946)
        {
            Name = "field of energy";
            Movable = false;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (Deleted) return;
            if (from == null) return;

            if (from.AccessLevel < m_RequiredAccessLevel)
            {
                from.SendMessage("You are repelled by the wall!");
                return;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use that.");
                return;
            }

            if (!from.InRange(Location, m_RequiredDistanceToPlayer))
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            if (!inUse)
            {
                from.SendMessage("You trigger the wall, causing a chain reaction!");
                ChainExplosion();
            }
        }

        public void ChainExplosion()
        {
            if (Deleted || inUse)
                return;

            inUse = true;

            Point3D location = GetWorldLocation();
            Map map = Map;

            TimeSpan explosionDelay = m_EffectDelay;
            int effectDuration = m_EffectDuration;

            int effectColor = 0;

            if (m_DefaultExplosionHue)
                effectColor = this.Hue;
            else
                effectColor = this.m_EffectsHue;

            Effects.SendLocationEffect(location, map, 0x3709, 30, effectColor, 0);

            Queue m_Queue = new Queue();
            IPooledEnumerable nearbyItems = map.GetItemsInRange(location, 1);

            foreach (Item item in nearbyItems)
            {
                if (item is ExplosiveEnergyField)
                {
                    if (m_SameHueCondition && (item.Hue != Hue))
                        continue;

                    m_Queue.Enqueue(item);
                }
            }

            nearbyItems.Free();

            while (m_Queue.Count > 0)
            {
                ExplosiveEnergyField explosiveEnergyField = (ExplosiveEnergyField)m_Queue.Dequeue();

                Timer.DelayCall(explosiveEnergyField.m_FuseTime, delegate
                {
                    if (explosiveEnergyField == null) return;
                    if (explosiveEnergyField.Deleted) return;

                    explosiveEnergyField.ChainExplosion();
                });
            }

            for (int a = 0; a < m_NumberOfEffects; a++)
            {
                int curRandSparkle = randomSparkle;
                int curRandExplosion = randomUnhuedExplosion;

                Timer.DelayCall(TimeSpan.FromSeconds(a) + explosionDelay, delegate
                {
                    Point3D sparkleLocation = new Point3D(location.X + Utility.RandomMinMax(-2, 2), location.Y + Utility.RandomMinMax(-2, 2), location.Z);
                    Effects.SendLocationEffect(sparkleLocation, map, curRandSparkle, effectDuration, effectColor, 0);
                    Effects.PlaySound(location, map, randomSparkleSound);

                    Point3D explosionLocation = new Point3D(location.X + Utility.RandomMinMax(-2, 2), location.Y + Utility.RandomMinMax(-2, 2), location.Z);
                    Effects.SendLocationEffect(explosionLocation, map, curRandExplosion, effectDuration, effectColor, 0);
                    Effects.PlaySound(location, map, randomExplosionSound);
                });

                if (!Deleted)
                    Delete();
            }
        }

        public ExplosiveEnergyField(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}