using System;
using Server.Items;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "the demonweb queen's corpse" )]
	public class DemonwebQueen : BaseCreature
	{
        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30));

        public int damageIntervalThreshold = 1000;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 20;

        public int minSpiders = 5;
        public int maxSpiders = 15;

        public int minWebs = 30;
        public int maxWebs = 60;

        public List<Mobile> m_Creatures = new List<Mobile>();
        public List<Point3D> m_BackupLocations = new List<Point3D>();

        public string[] idleSpeech
        { 
            get { return new string[] {"*stalks*"}; }
        }

        public string[] combatSpeech
        {
            get { return new string[] {""}; }
        }
        
        [Constructable]
		public DemonwebQueen() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Demonweb Queen";
			Body = 735;
            BaseSoundID = 0x388;            

			SetStr(100);
			SetDex(75);
			SetInt(25);

			SetHits(15000);
            SetStam(10000);

			SetDamage(20, 40);            

            SetSkill(SkillName.Wrestling, 95);
			SetSkill(SkillName.Tactics, 100);			

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Poisoning, 33);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

			Fame = 20000;
			Karma = -20000;

			VirtualArmor = 25;

            SetBackupLocations();
		}

        public void SetBackupLocations()
        {
            m_BackupLocations = new List<Point3D>();

            #region Locations

            m_BackupLocations.Add(new Point3D(5443, 1816, 0));
            m_BackupLocations.Add(new Point3D(5437, 1822, 0));
            m_BackupLocations.Add(new Point3D(5428, 1821, 0));
            m_BackupLocations.Add(new Point3D(5424, 1815, 0));
            m_BackupLocations.Add(new Point3D(5426, 1810, 0));
            m_BackupLocations.Add(new Point3D(5428, 1803, 0));
            m_BackupLocations.Add(new Point3D(5437, 1799, 0));
            m_BackupLocations.Add(new Point3D(5443, 1801, 0));
            m_BackupLocations.Add(new Point3D(5446, 1808, 0));

            #endregion
        }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;

            UniqueCreatureDifficultyScalar = 1.5;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.33, this, defender, DamageMax, 8.0, -1, true, "", "Their attack causes you to bleed!");

            if (Utility.RandomDouble() < .10)
            {
                Effects.PlaySound(defender.Location, defender.Map, 0x580);
                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(5.0)), Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308), 0, 125, 0, 0, 5029, 0);

                defender.SendMessage("You have been wrapped in a web!");

                SpecialAbilities.HinderSpecialAbility(1.0, this, defender, 1.0, Utility.RandomMinMax(4, 6), false, -1, false, "", "");
            }
        }

        public void SpawnTrees()
        {
            List<Point3D> m_Locations = new List<Point3D>();

            #region Spawn Locations

            m_Locations.Add(new Point3D(5464, 1803, 0));
            m_Locations.Add(new Point3D(5462, 1809, 0));
            m_Locations.Add(new Point3D(5463, 1816, 0));
            m_Locations.Add(new Point3D(5460, 1821, 0));
            m_Locations.Add(new Point3D(5454, 1822, 0));
            m_Locations.Add(new Point3D(5456, 1813, 0));
            m_Locations.Add(new Point3D(5460, 1799, 0));
            m_Locations.Add(new Point3D(5456, 1803, 0));
            m_Locations.Add(new Point3D(5455, 1797, 0));
            m_Locations.Add(new Point3D(5452, 1807, 0));
            m_Locations.Add(new Point3D(5450, 1815, 0));
            m_Locations.Add(new Point3D(5446, 1822, 0));
            m_Locations.Add(new Point3D(5445, 1817, 0));
            m_Locations.Add(new Point3D(5440, 1823, 0));
            m_Locations.Add(new Point3D(5440, 1816, 0));
            m_Locations.Add(new Point3D(5444, 1811, 0));
            m_Locations.Add(new Point3D(5447, 1804, 0));
            m_Locations.Add(new Point3D(5449, 1798, 0));
            m_Locations.Add(new Point3D(5441, 1799, 0));
            m_Locations.Add(new Point3D(5439, 1806, 0));
            m_Locations.Add(new Point3D(5436, 1811, 0));
            m_Locations.Add(new Point3D(5433, 1817, 0));
            m_Locations.Add(new Point3D(5434, 1823, 0));
            m_Locations.Add(new Point3D(5426, 1822, 0));
            m_Locations.Add(new Point3D(5426, 1814, 0));
            m_Locations.Add(new Point3D(5431, 1807, 0));
            m_Locations.Add(new Point3D(5434, 1801, 0));
            m_Locations.Add(new Point3D(5429, 1803, 0));
            m_Locations.Add(new Point3D(5425, 1808, 0));
            m_Locations.Add(new Point3D(5420, 1817, 0));
            m_Locations.Add(new Point3D(5417, 1821, 0));
            m_Locations.Add(new Point3D(5417, 1812, 0));
            m_Locations.Add(new Point3D(5419, 1805, 0));
            m_Locations.Add(new Point3D(5422, 1801, 0));
            m_Locations.Add(new Point3D(5411, 1817, 0));
            m_Locations.Add(new Point3D(5411, 1810, 0));
            m_Locations.Add(new Point3D(5412, 1804, 0));
            m_Locations.Add(new Point3D(5415, 1800, 0));

            #endregion

            foreach (Point3D location in m_Locations)
            {
                BaseCreature tree = (BaseCreature)Activator.CreateInstance(typeof(NightbarkTree));
                tree.MoveToWorld(location, Map);
                m_Creatures.Add(tree);
            }            
        }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override int AttackRange { get { return 2; } }

        public override bool RevealImmune { get { return true; } }

        public override bool AlwaysBoss { get { return true; } }
        public override string BossSpawnMessage { get { return "The Demonweb Queen has arisen and stirs within Covetous Dungeon..."; } }
        public override string TitleReward { get { return "Slayer of the Demonweb Queen"; } }
        public override bool AlwaysMurderer { get { return true; } }         

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    Point3D originalLocation = Location;                        
                        
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, 0x21D, 5, 10, true, m_BackupLocations))
                    {
                        for (int a = 0; a < 10; a++)
                        {
                            if (Utility.RandomDouble() <= .50)
                            {
                                //Rocks
                                Blood rocks = new Blood();
                                rocks.Name = "rocks";
                                rocks.ItemID = Utility.RandomList(4967, 4970, 4973);

                                Point3D rockLocation = new Point3D(originalLocation.X + Utility.RandomMinMax(-2, 2), originalLocation.Y + Utility.RandomMinMax(-2, 2), originalLocation.Z);

                                rocks.MoveToWorld(rockLocation, Map);
                            }

                            else
                            {
                                //Dirt
                                Blood dirt = new Blood();
                                dirt.Name = "dirt";
                                dirt.ItemID = Utility.RandomList(7681, 7682);

                                Point3D dirtLocation = new Point3D(originalLocation.X + Utility.RandomMinMax(-2, 2), originalLocation.Y + Utility.RandomMinMax(-2, 2), Z);

                                dirt.MoveToWorld(dirtLocation, Map);
                            }
                        }

                        PublicOverheadMessage(MessageType.Regular, 0, false, "*burrows*");

                        Combatant = null;

                        Effects.PlaySound(Location, Map, GetIdleSound());
                    }                     

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }

            if (!Hidden && Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (!Hidden && Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*shrieks*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*hisses*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*spits*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*gnashes teeth*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                        break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*salivates*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                        break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;
                        
                    SpawnSpiders(this);

                    if (intervalCount % 2 == 0)
                        ShootWebs(this);
                }
            }            

            base.OnDamage(amount, from, willKill);
        }

        public void ShootWebs(BaseCreature creature)
        {
            if (creature == null) return;
            if (!creature.Alive || creature.Deleted) return;

            double minWebs = 25;
            double maxWebs = 50;

            int minRadius = 1;
            int maxRadius = 20;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;
            int websToAdd = (int)(minWebs + ((maxWebs - minWebs) * spawnPercent));

            for (int a = 0; a < websToAdd; a++)
            {
                List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(Location, false, true, creature.Location, creature.Map, 1, 15, minRadius, maxRadius, true);

                if (m_ValidLocations.Count > 0)
                {
                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    int webId = Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z), creature.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z), creature.Map);

                    Effects.PlaySound(newLocation, creature.Map, 0x4F1);
                    Effects.SendMovingEffect(startLocation, endLocation, webId, 5, 0, false, false, 0, 0);

                    double distance = creature.GetDistanceToSqrt(newLocation);
                    double destinationDelay = (double)distance * .08;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        if (creature == null) return;
                        if (!creature.Alive || creature.Deleted) return;

                        Effects.SendLocationParticles(EffectItem.Create(newLocation, creature.Map, TimeSpan.FromSeconds(15)), webId, 5, 4000, 0, 0, 5029, 0);
                    });
                }            
            }

            IPooledEnumerable eable = creature.Map.GetObjectsInRange(creature.Location, 20);

            foreach (object obj in eable)
            {
                if (obj is PlayerMobile)
                {
                    Mobile mobile = obj as Mobile;
                    PlayerMobile player = obj as PlayerMobile;
                    BaseCreature bc_Creature = obj as BaseCreature;

                    if (mobile == null) continue;
                    if (!mobile.Alive) continue;

                    bool validTarget = false;

                    if (player != null)
                    {
                        if (player.AccessLevel == AccessLevel.Player)
                            validTarget = true;
                    }

                    if (bc_Creature != null)
                    {
                        if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                            validTarget = true;
                    }

                    double mobileWebChanceMin = .4;
                    double mobileWebChanceMax = .8;

                    double chance = mobileWebChanceMin + ((mobileWebChanceMax - mobileWebChanceMin) * spawnPercent);

                    if (creature.CanBeHarmful(mobile) && validTarget && !mobile.Hidden && Utility.RandomDouble() <= chance)
                    {
                        Effects.PlaySound(mobile.Location, player.Map, 0x580);
                        SpecialAbilities.HinderSpecialAbility(1.0, creature, mobile, 1.0, Utility.RandomMinMax(4, 8), false, -1, false, "", "You have been wrapped in a web!");

                        int webId = Utility.RandomList(3811, 3812, 3813, 3814, 4306, 4307, 4308, 4308);

                        IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z), creature.Map);
                        IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobile.X, mobile.Y, mobile.Z), creature.Map);

                        Effects.SendMovingEffect(startLocation, endLocation, webId, 5, 0, false, false, 0, 0);

                        double distance = creature.GetDistanceToSqrt(mobile.Location);
                        double destinationDelay = (double)distance * .08;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            if (mobile == null || creature == null) return;
                            if (!mobile.Alive || mobile.Deleted || !creature.Alive || creature.Deleted) return;

                            Effects.SendLocationParticles(EffectItem.Create(mobile.Location, creature.Map, TimeSpan.FromSeconds(10)), webId, 0, 125, 0, 0, 5029, 0);
                        });
                    }
                }
            }

            eable.Free();
        }

        public void SpawnSpiders(BaseCreature creature)
        {
            if (creature == null) return;
            if (!creature.Alive) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int spiderCount = minSpiders + (int)Math.Ceiling(((double)maxSpiders - (double)minSpiders) * spawnPercent);

            int minRadius = 2;
            int maxRadius = 10;

            for (int a = 0; a < spiderCount; a++)
            {
                List<Point3D> locations = SpecialAbilities.GetSpawnableTiles(creature.Location, false, true, creature.Location, creature.Map, 1, 15, minRadius, maxRadius, true);

                Point3D newLocation = new Point3D();

                if (locations.Count > 0)
                    newLocation = locations[0];                

                else
                {
                    bool validBackups = false;

                    if (m_BackupLocations != null)
                    {
                        if (m_BackupLocations.Count > 0)
                            validBackups = true;
                    }

                    if (Utility.RandomDouble() < .5 && validBackups)
                        newLocation = m_BackupLocations[Utility.RandomMinMax(0, m_BackupLocations.Count - 1)];
                    else
                        newLocation = Location;
                }

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z), creature.Map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z), creature.Map);

                Effects.PlaySound(newLocation, creature.Map, 0x4F1);

                if (newLocation != Location)
                    Effects.SendMovingEffect(startLocation, endLocation, 4313, 5, 0, false, false, 0, 0);

                double distance = creature.GetDistanceToSqrt(newLocation);

                double destinationDelay = (double)distance * .08;

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (creature == null) return;
                    if (!creature.Alive || creature.Deleted) return;

                    Effects.SendLocationParticles(EffectItem.Create(newLocation, creature.Map, TimeSpan.FromSeconds(1.5)), 4313, 5, 60, 0, 0, 5029, 0);
                });

                Timer.DelayCall(TimeSpan.FromSeconds(3.0), delegate
                {
                    if (creature == null) return;
                    if (!creature.Alive || creature.Deleted) return;

                    Effects.PlaySound(newLocation, creature.Map, 0x4F1);
                    Effects.SendLocationParticles(EffectItem.Create(newLocation, creature.Map, TimeSpan.FromSeconds(2.0)), 0x11A6, 5, 25, 1149, 0, 5029, 0);

                    switch (Utility.RandomMinMax(1, 2))
                    {
                        case 1:
                            CorruptSpiderling corruptSpiderling = new CorruptSpiderling();
                            corruptSpiderling.MoveToWorld(newLocation, creature.Map);
                            m_Creatures.Add(corruptSpiderling);
                        break;

                        case 2:
                            Deathspinner deathSpinner = new Deathspinner();
                            deathSpinner.MoveToWorld(newLocation, creature.Map);
                            m_Creatures.Add(deathSpinner);
                        break;
                    }
                }); 
            }
        }        

        public override bool IsHighSeasBodyType { get { return true; } }        
        public override bool HasAlternateHighSeasHurtAnimation { get { return true; } }

        public override int GetAngerSound() { return 0x4FF; }
        public override int GetIdleSound() { return 0x4FD; }
        public override int GetAttackSound() { return 0x4EE; }
        public override int GetHurtSound() { return 0x4EF; }
        public override int GetDeathSound() { return 0x4EB; }

        public override void OnAfterSpawn()
        {
            base.OnAfterSpawn();

            if (Global_AllowAbilities)            
                SpawnTrees();            
        }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, 0x350);

            return base.OnMove(d);           
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);

            BossPersistance.PersistanceItem.CovetousBossLastStatusChange = DateTime.UtcNow;
        }

        public override bool OnBeforeDeath()
        {
            return base.OnBeforeDeath();            
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 10) == 1)
                c.AddItem(new DemonwebQueenStatue());

            if (Utility.RandomMinMax(1, 20) == 1)
                c.AddItem(new DemonwebCocoon());

            if (Global_AllowAbilities)
            {
                for (int a = 0; a < m_Creatures.Count; ++a)
                {
                    if (m_Creatures[a] != null)
                    {
                        if (m_Creatures[a].Alive)
                            m_Creatures[a].Kill();
                    }
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (Global_AllowAbilities)
            {
                for (int a = 0; a < m_Creatures.Count; ++a)
                {
                    if (m_Creatures[a] != null)
                    {
                        if (m_Creatures[a].Alive)
                            m_Creatures[a].Kill();
                    }
                }
            }
        }

        public DemonwebQueen(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize(writer);
			writer.Write((int)1);

            writer.Write(damageIntervalThreshold);
            writer.Write(damageProgress);
            writer.Write(intervalCount);
            writer.Write(totalIntervals);

            writer.Write(m_Creatures.Count);
            for (int a = 0; a < m_Creatures.Count; a++)
            {
                writer.Write(m_Creatures[a]);
            }
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            m_Creatures = new List<Mobile>();

            if (version >= 1)
            {
                int creaturesCount = reader.ReadInt();
                for (int a = 0; a < creaturesCount; a++)
                {
                    Mobile creature = reader.ReadMobile();

                    m_Creatures.Add(creature);
                }
            }

            SetBackupLocations();
		}
	}
}
