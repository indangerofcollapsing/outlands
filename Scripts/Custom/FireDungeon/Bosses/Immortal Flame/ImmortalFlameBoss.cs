using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using System.Collections.Generic;

namespace Server.Mobiles
{
    [CorpseName("an immortal flame corpse")]
    public class ImmortalFlameBoss : BaseCreature
    {
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            switch (Utility.RandomMinMax(0, 3))
            {
                case 0: { c.AddItem(SpellScroll.MakeMaster(new EarthquakeScroll())); } break;
                case 1: { c.AddItem(new ForgedMetal()); } break;
                case 2: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Chest)); } break;
                case 3: { c.AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Unspecified, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Gorget)); } break;

            }
        }

        public override bool AlwaysBoss { get { return true; } }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.SuperBoss, 3);
        }

        public override void SetUniqueAI()
        {
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            UniqueCreatureDifficultyScalar = 2;
        }        

        public override int Hides { get { return 20; } }
        public override int Meat { get { return 19; } }        
        
        public override int GetIdleSound() { return 0x2D3; }
        public override int GetHurtSound() { return 0x2D1; }

        private static readonly Point3D BossSpawnLocation = new Point3D(5907, 1409, 44);
        public DateTime FreezeTimer { get; set; }
        private InternalTimer m_Timer;
        private List<Mobile> m_SpawnedMobiles;
        public List<ImmortalFlameWaterBucket> Buckets { get; set; }
        private Point3D[] m_BucketLocations;

        [Constructable]
        public ImmortalFlameBoss(double difficultyMultiplier, params Point3D[] bucketLocs)
            : base(AIType.AI_Mage, FightMode.Weakest, 10, 1, 0.2, 0.4)
        {
            CantWalk = true;
            m_BucketLocations = bucketLocs;

            Name = "Immortal Flame";
            BodyValue = 780;
            Hue = 0x54E;
            BaseSoundID = 362;

            SetStr(1096, 1185);
            SetDex(86, 175);
            SetInt(686, 775);

            SetHits(18000, 19001);
            SetHits((int)Math.Ceiling(Hits * difficultyMultiplier));

            SetDamage((int)Math.Ceiling(29.0 * difficultyMultiplier), (int)Math.Ceiling(39.0 * difficultyMultiplier));

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Fire, 25);

            SetResistance(ResistanceType.Physical, 65, 75);
            SetResistance(ResistanceType.Fire, 80, 90);
            SetResistance(ResistanceType.Cold, 70, 80);
            SetResistance(ResistanceType.Poison, 60, 70);
            SetResistance(ResistanceType.Energy, 60, 70);

            SetSkill(SkillName.EvalInt, 80.1, 100.0);
            SetSkill(SkillName.Magery, 80.1, 120.0);
            SetSkill(SkillName.Meditation, 52.5, 75.0);
            SetSkill(SkillName.MagicResist, 200, 201);
            SetSkill(SkillName.Tactics, 97.6, 100.0);
            SetSkill(SkillName.Wrestling, 70, 70.1);

            SetSkill(SkillName.Poisoning, 25);

            Fame = 25500;
            Karma = -22500;

            VirtualArmor = 80;

            m_Timer = new InternalTimer(this);
            m_Timer.Start();

            Buckets = new List<ImmortalFlameWaterBucket>();
            m_SpawnedMobiles = new List<Mobile>();

            Timer.DelayCall(TimeSpan.FromTicks(1), SpawnBuckets);
        }

        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Utility.RandomBool() ? Poison.Deadly : Poison.Lethal; } }

        public virtual void SpawnBuckets()
        {
            if (Deleted)
                return;

            ImmortalFlameWaterBucket bucket;

            foreach (Point3D p in m_BucketLocations)
            {
                if (!CheckExistance(p, Map, typeof(ImmortalFlameWaterBucket)))
                {
                    bucket = new ImmortalFlameWaterBucket(this);
                    Buckets.Add(bucket);
                    bucket.MoveToWorld(p, Map);
                    bucket.Home = p;
                }
            }
        }

        private static bool CheckExistance(Point3D loc, Map facet, Type type)
        {
            foreach (Item item in facet.GetItemsInRange(loc, 0))
            {
                if (type.IsAssignableFrom(item.GetType()))
                    return true;
            }

            return false;
        }

        public virtual void SpawnFireElemental()
        {
            Point3D loc = GenerateSpawnLoc();
            FireElemental ele = new FireElemental();
            ele.BardImmune = true;
            ele.MoveToWorld(loc, Map);
            m_SpawnedMobiles.Add(ele);
        }

        private Point3D GenerateSpawnLoc()
        {
            int newX = X + Utility.Random(12) - 6;
            int newY = Y + Utility.Random(12) - 6;
            IPoint3D spawnLoc = new Point3D(newX, newY, Z);
            SpellHelper.GetSurfaceTop(ref spawnLoc);
            return (Point3D)spawnLoc;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (FreezeTimer > DateTime.UtcNow)
                amount *= 2;

            base.OnDamage(amount, from, willKill);
        }

        public override void GenerateLoot(bool spawning)
        {
            if (!spawning)
            {
                var chest = new TreasureMapChest(5);
                chest.Locked = false;
                chest.TrapType = TrapType.None;
                AddItem(chest);

                if (Utility.RandomBool())
                    AddItem(BaseDungeonArmor.CreateDungeonArmor(BaseDungeonArmor.DungeonEnum.Shame, BaseDungeonArmor.ArmorTierEnum.Tier1, BaseDungeonArmor.ArmorLocation.Legs));
            }
            base.GenerateLoot(spawning);
        }

        public void BucketUsed(ImmortalFlameWaterBucket bucket)
        {
            Buckets.Remove(bucket);
            FreezeTimer = DateTime.UtcNow + TimeSpan.FromSeconds(8);
            Hue = 0x83A;
            Timer.DelayCall(TimeSpan.FromSeconds(8), delegate
            {
                Hue = 0x54E;
                SpawnBuckets();
            });
        }

        public ImmortalFlameBoss(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(m_SpawnedMobiles.Count);
            for (int i = 0; i < m_SpawnedMobiles.Count; i++)
                writer.Write(m_SpawnedMobiles[i]);

            writer.Write(Buckets.Count);
            for (int i = 0; i < Buckets.Count; i++)
                writer.Write(Buckets[i]);

            writer.Write(m_BucketLocations.Length);
            for (int i = 0; i < m_BucketLocations.Length; i++)
                writer.Write(m_BucketLocations[i]);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        int count = reader.ReadInt();
                        m_SpawnedMobiles = new List<Mobile>();
                        for (int i = 0; i < count; i++)
                        {
                            Mobile m = reader.ReadMobile();
                            if (m != null)
                                m_SpawnedMobiles.Add(m);
                        }

                        count = reader.ReadInt();
                        Buckets = new List<ImmortalFlameWaterBucket>();
                        for (int i = 0; i < count; i++)
                        {
                            ImmortalFlameWaterBucket m = reader.ReadItem() as ImmortalFlameWaterBucket;
                            if (m != null)
                                Buckets.Add(m);
                        }

                        count = reader.ReadInt();
                        m_BucketLocations = new Point3D[count];
                        for (int i = 0; i < count; i++)
                            m_BucketLocations[i] = reader.ReadPoint3D();

                        m_Timer = new InternalTimer(this);
                        m_Timer.Start();

                        break;
                    }

            }
        }

        public override void OnAfterDelete()
        {
            if (Buckets != null)
            {
                while (Buckets.Count > 0)
                {
                    var bucket = Buckets[0];
                    Buckets.RemoveAt(0);
                    bucket.Delete();
                }
            }

            if (m_SpawnedMobiles != null)
            {
                while (m_SpawnedMobiles.Count > 0)
                {
                    var mob = m_SpawnedMobiles[0];
                    m_SpawnedMobiles.RemoveAt(0);
                    mob.Delete();
                }
            }


            base.OnAfterDelete();
        }

        private class FireElementalTimer : Timer
        {
            private ImmortalFlameBoss m_Boss;

            public FireElementalTimer(ImmortalFlameBoss boss)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(10))
            {
                m_Boss = boss;
            }

            protected override void OnTick()
            {
                if (m_Boss == null || m_Boss.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_Boss.Combatant != null)
                    m_Boss.SpawnFireElemental();
            }
        }

        private class InternalTimer : Timer
        {
            private ImmortalFlameBoss m_Boss;
            private int m_Countdown;
            private FireElementalTimer m_Timer;

            public InternalTimer(ImmortalFlameBoss boss)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(1))
            {
                m_Boss = boss;
                m_Countdown = 10;

                m_Timer = new FireElementalTimer(m_Boss);
                m_Timer.Start();
            }

            protected override void OnTick()
            {
                if (m_Boss == null || m_Boss.Deleted)
                {
                    Stop();
                    return;
                }
                else if (m_Boss.Combatant == null) //dont reset timer here
                {
                    return;
                }
                else if (DateTime.UtcNow < m_Boss.FreezeTimer) //reset countdown when the boss gets doused
                {
                    if (m_Countdown < 10)
                        m_Countdown = 10;
                }

                m_Boss.PublicOverheadMessage(Network.MessageType.Regular, 32, true, String.Format("0:{0:D2}", m_Countdown));

                if (m_Countdown-- <= 0)
                {
                    m_Boss.BreathStart(m_Boss.Combatant);
                    m_Countdown = 10;
                }
            }
        }
    }
}
