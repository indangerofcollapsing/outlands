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
    public class LavaOrb : BreakableStatic
    {
        public Mobile m_Owner;

        public Timer m_DetonationTimer;
        public DateTime m_DetonationTime = DateTime.UtcNow + TimeSpan.FromSeconds(15);

        private int m_Radius = 3;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }

        private int m_MinDamage = 20;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int MinDamage
        {
            get { return m_MinDamage; }
            set { m_MinDamage = value; }
        }

        private int m_MaxDamage = 40;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int MaxDamage
        {
            get { return m_MaxDamage; }
            set { m_MaxDamage = value; }
        }

        private int m_LavaDuration = 600;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int LavaDuration
        {
            get { return m_LavaDuration; }
            set { m_LavaDuration = value; }
        }

        private int m_LavaMinDamage = 3;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int LavaMinDamage
        {
            get { return m_LavaMinDamage; }
            set { m_LavaMinDamage = value; }
        }

        private int m_LavaMaxDamage = 5;
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.GameMaster)]
        public int LavaMaxDamage
        {
            get { return m_LavaMaxDamage; }
            set { m_LavaMaxDamage = value; }
        }
        
        [Constructable]
        public LavaOrb(): base()
        {
            Name = "a lava orb";

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

            MaxHitPoints = 500;
            HitPoints = 500;

            AddLOSBlocker = false;

            HitSound = 0x38E;

            ItemID = 13935;
            Hue = 1358;

            NormalItemId = 13935;
            NormalHue = 1358;

            LightlyDamagedPercent = .666;
            LightlyDamagedSound = 0x38E;
            LightlyDamagedItemId = 13935;
            LightlyDamagedHue = 1358;

            HeavilyDamagedPercent = .333;
            HeavilyDamagedSound = 0x38E;
            HeavilyDamagedItemId = 13935;
            HeavilyDamagedHue = 1358;

            BrokenSound = 0x665;
            BrokenItemId = 6001;
            BrokenHue = 2603;

            DeleteOnBreak = true;
            CreateTimedStaticAfterBreak = false;

            RevealNearbyHiddenItemsOnBreak = false;
            RevealNearbyHiddenItemsOnBreakRadius = 0;
            RefreshNearbyMovables = false;
            RefreshNearbyMovablesRadius = 0;
            
            m_DetonationTimer = new DetonationTimer(this);
            m_DetonationTimer.Start();
        }

        private class DetonationTimer : Timer
        {
            public LavaOrb m_LavaOrb;

            public DetonationTimer(LavaOrb lavaOrb): base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_LavaOrb = lavaOrb;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_LavaOrb == null)
                {
                    Stop();
                    return;
                }

                if (m_LavaOrb.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_LavaOrb.m_DetonationTime <= DateTime.UtcNow)
                {
                    m_LavaOrb.Detonate();

                    Stop();
                    return;
                }
            }
        }

        public void Detonate()
        {
            Point3D location = Location;
            Map map = Map;

            if (!SpecialAbilities.Exists(m_Owner))
                return;

            Mobile owner = m_Owner;

            Effects.PlaySound(Location, Map, 0x306);

            int radius = m_Radius;
            int minRange = radius * -1;
            int maxRange = radius;

            int minDamage = m_MinDamage;
            int maxDamage = m_MaxDamage;

            for (int a = minRange; a < maxRange + 1; a++)
            {
                for (int b = minRange; b < maxRange + 1; b++)
                {
                    Point3D newPoint = new Point3D(location.X + a, location.Y + b, location.Z);
                    newPoint.Z = map.GetSurfaceZ(newPoint, 30);

                    int distance = Utility.GetDistance(location, newPoint);

                    double effectChance = .95 - ((double)distance * .05);

                    if (Utility.RandomDouble() > effectChance)
                        continue;

                    Timer.DelayCall(TimeSpan.FromSeconds(distance * .10), delegate
                    {
                        int impactItemId = 0x3709;
                        int impactHue = 2074;

                        if (Utility.RandomDouble() <= .75)
                        {
                            LavaField lavaField = new LavaField(null, 0, 1, m_LavaDuration, m_LavaMinDamage, m_LavaMaxDamage, false, false, true, -1, true);
                            lavaField.MoveToWorld(newPoint, map);
                        }

                        else
                        {
                            Effects.PlaySound(newPoint, map, 0x5CF);
                            Effects.SendLocationParticles(EffectItem.Create(newPoint, map, EffectItem.DefaultDuration), impactItemId, 20, 20, impactHue, 0, 0, 0);

                            IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newPoint, 0);

                            if (!SpecialAbilities.Exists(owner))
                                return;

                            Queue m_Queue = new Queue();

                            foreach (Mobile mobile in mobilesOnTile)
                            {
                                if (m_Owner == mobile) continue;
                                if (!SpecialAbilities.MonsterCanDamage(m_Owner, mobile)) continue;

                                m_Queue.Enqueue(mobile);
                            }

                            mobilesOnTile.Free();

                            while (m_Queue.Count > 0)
                            {
                                double damage = Utility.RandomMinMax(minDamage, maxDamage);

                                Mobile mobile = (Mobile)m_Queue.Dequeue();

                                if (mobile is BaseCreature)
                                    damage *= 2;

                                int finalDamage = (int)(Math.Round((double)damage));

                                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                                AOS.Damage(mobile, m_Owner, finalDamage, 100, 0, 0, 0, 0);
                            }
                        }
                    });
                }
            }

            Delete();
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
                TimedStatic fragments = new TimedStatic(22328, 3);
                fragments.Name = "orb fragments";
                fragments.Hue = 1358;

                Point3D oreLocation = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                oreLocation.Z = Map.GetSurfaceZ(oreLocation, 30);

                SpellHelper.AdjustField(ref oreLocation, Map, 12, false);
                fragments.MoveToWorld(oreLocation, Map);
            }
        }

        public override void BeforeBreak(Mobile from, InteractionType interactionType)
        {
            int fragments = 20;

            for (int a = 0; a < fragments; a++)
            {
                TimedStatic ore = new TimedStatic(22328, 3);
                ore.Name = "orb fragments";
                ore.Hue = 1358;

                Point3D oreLocation = new Point3D(Location.X + Utility.RandomList(-2, 2), Location.Y + Utility.RandomList(-2, 2), Location.Z);
                oreLocation.Z = Map.GetSurfaceZ(oreLocation, 30);

                ore.MoveToWorld(oreLocation, Map);
            }

            Point3D location = Location;
            Map map = Map;

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {              
            });

            base.BeforeBreak(from, interactionType);
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();           

            Delete();
        }

        public LavaOrb(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            //Version 0
            writer.Write(m_Owner);

            writer.Write(m_Radius);
            writer.Write(m_MinDamage);
            writer.Write(m_MaxDamage);
            writer.Write(m_LavaDuration);
            writer.Write(m_LavaMinDamage);
            writer.Write(m_LavaMaxDamage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Owner = reader.ReadMobile();

                m_Radius = reader.ReadInt();
                m_MinDamage = reader.ReadInt();
                m_MaxDamage = reader.ReadInt();
                m_LavaDuration = reader.ReadInt();
                m_LavaMinDamage = reader.ReadInt();
                m_LavaMaxDamage = reader.ReadInt();
            }

            //---------    

            m_DetonationTimer = new DetonationTimer(this);
            m_DetonationTimer.Start();
        }
    }
}
