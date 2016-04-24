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
    [CorpseName("maggot's corpse")]
    public class Maggot : BaseCreature
    {
        public override string TitleReward { get { return "Slayer of Maggot"; } }

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSpeechAllowed;
        public TimeSpan NextSpeechDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextSwallowAllowed;
        public TimeSpan NextSwallowDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextAbilityAllowed;
        public TimeSpan NextAbilityDelay = TimeSpan.FromSeconds(5);

        public int damageIntervalThreshold = 625;
        public int damageProgress = 0;

        public int intervalCount = 0;
        public int totalIntervals = 40;

        public bool AbilityInProgress = false;
        public bool DamageIntervalInProgress = false;

        public List<Mobile> m_Creatures = new List<Mobile>();
        List<Mobile> m_SwallowedMobiles = new List<Mobile>();

        public string[] idleSpeech { get { return new string[] { "*breathes heavily*" }; } }
        public string[] combatSpeech { get { return new string[] { "" }; } }

        [Constructable]
        public Maggot(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Maggot";

            Body = 256;
            BaseSoundID = 0x388;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(25000);
            SetStam(5000);

            SetDamage(30, 50);            

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);            

            SetSkill(SkillName.MagicResist, 25);

            Fame = 20000;
            Karma = -20000;

            VirtualArmor = 25;

            CantWalk = true;

            PackItem(new Bone(100));
        }

        public override int PoisonResistance { get { return 5; } }

        public override int AttackRange { get { return 2; } }

        public override bool IgnoreHinderForSwings { get { return true; } }

        public override bool AlwaysBoss { get { return true; } }
        public override string BossSpawnMessage { get { return "Maggot has arisen and stirs within Despise Dungeon..."; } }
        public override bool AlwaysMurderer { get { return true; } }        

        public override void SetUniqueAI()
        {   
            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
           
            UniqueCreatureDifficultyScalar = 30;

            CantWalk = true;

            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));
        }
                
        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            damageIntervalThreshold = (int)(Math.Round((double)HitsMax / (double)totalIntervals));
            intervalCount = (int)(Math.Floor((1 - (double)Hits / (double)HitsMax) * (double)totalIntervals));

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            if (!willKill)
            {
                damageProgress += amount;

                if (damageProgress >= damageIntervalThreshold)
                {
                    Effects.PlaySound(Location, Map, GetAngerSound());

                    damageProgress = 0;

                    int lurchCount = (int)(Math.Ceiling(.25 * (double)totalIntervals));

                    if (intervalCount == lurchCount)
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*lurches forward!*");

                    if (intervalCount >= lurchCount)
                    {
                        CantWalk = false;

                        double minSpeed = 1.5;
                        double maxSpeed = 0.35;

                        double speed = minSpeed - ((minSpeed - maxSpeed) * spawnPercent);

                        ActiveSpeed = speed;
                        PassiveSpeed = speed;
                        CurrentSpeed = speed;
                    }

                    if (intervalCount % 4 == 0)
                        SpawnCreatures();

                    else
                    {
                        switch (Utility.RandomMinMax(1, 3))
                        {
                            case 1: MassiveCorpseExplosion(); break;
                            case 2: MassiveCorpseExplosion(); break;
                            case 3: MassiveCorpseExplosion(); break;
                        }
                    }

                    /*
                    Effects.PlaySound(Location, Map, 0x5A1);

                    PublicOverheadMessage(MessageType.Regular, 0, false, "*violently erupts*");

                    int bileAmount = minEventBile + (int)Math.Ceiling(((double)maxEventBile - (double)minEventBile) * spawnPercent);

                    for (int a = 0; a < bileAmount; a++)
                    {
                        if (this == null) continue;

                        Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                        Map newMap = Map;
                        Point3D newLocation = new Point3D(X + Utility.RandomMinMax(-6, 6), Y + Utility.RandomMinMax(-6, 6), Z + 1);

                        IEntity effectStartLocation = new Entity(Serial.Zero, Location, Map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, newLocation, newMap);

                        Bile bile = new Bile();

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, bile.ItemID, 5, 0, false, false, 0, 0);

                        double distance = Utility.GetDistanceToSqrt(Location, newLocation);

                        double destinationDelay = (double)distance * .16;
                        double explosionDelay = ((double)distance * .16) + 1;

                        Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                        {
                            Effects.PlaySound(newLocation, newMap, 0x230);

                            bile.MoveToWorld(newLocation, newMap);
                        });
                    }

                    if (Utility.RandomDouble() <= .5)
                    {
                        int corpseParts = minEventCorpses + (int)Math.Ceiling(((double)maxEventCorpses - (double)minEventCorpses) * spawnPercent);

                        MassiveCorpseExplosion(null, Location, Location, false, true, Map, 0, 15, corpseParts, corpseParts);
                    }

                    else
                    {
                        int creatures = minEventCreatures + (int)Math.Ceiling(((double)maxEventCreatures - (double)minEventCreatures) * spawnPercent);

                        SpawnCreatures(creatures);
                    }
                    */
                }

                if (from != null && amount > 10 && !AbilityInProgress && !DamageIntervalInProgress)
                {
                    BaseWeapon weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        //Ranged Weapon
                        if (weapon is BaseRanged)
                        {
                            if (Utility.RandomDouble() < .1)
                                DamageEffect(from);
                        }

                        //Melee Weapon
                        else if (weapon is BaseMeleeWeapon || weapon is Fists)
                        {
                            if (Utility.RandomDouble() < .05)
                                DamageEffect(from);
                        }
                    }

                    else
                    {
                        //Spell or Special Effect
                        if (Utility.RandomDouble() < .15)
                            DamageEffect(from);
                    }
                }
            }
            

            base.OnDamage(amount, from, willKill);
        }

        public void DamageEffect(Mobile from)
        {
            if (AbilityInProgress || DamageIntervalInProgress)
                return;

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: CorpseExplosion(from); break;
                case 2: CorpseExplosion(from); break;
            }
        }

        //4 16 1 true false 0 LARGE PUNCH
        //5 15 1 true false 0 SLAP
        //6 15 1 true false 0 BITE
        //11 15 1 true false 0 EAT GROUND
        //17 12 1 true false 0 BURP
        //18 12 1 true false 0 JUMP BITE

        public void CorpseExplosion(Mobile from)
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(from)) return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int maxExtraParts = 4;
            int parts = 4 + (int)Math.Ceiling((double)maxExtraParts * spawnPercent);

            SpecialAbilities.CorpseExplosionAbility(this, Location, from.Location, false, true, Map, 0, 2, parts, parts);                    
        }

        public void BileAttack()
        {
            /*
            Bile bile = new Bile();
            bile.Hue = 2075;
            bile.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z + 1), Map);

            int bileAmount = Utility.RandomMinMax(2, 4);

            for (int a = 0; a < bileAmount; a++)
            {
                Bile extraBile = new Bile();
                extraBile.Hue = 2075;
                extraBile.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z + 1), Map);
            }

            Effects.PlaySound(defender.Location, defender.Map, 0x4F1);
            defender.FixedParticles(0x374A, 10, 20, 5021, 1107, 0, EffectLayer.Head);

            defender.SendMessage("You have been covered in foul, black bile!");

            SpecialAbilities.EntangleSpecialAbility(1.0, this, defender, 1.0, 1, -1, false, "", "", "-1");
            SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8, -1, false, "", "", "-1");
            SpecialAbilities.CrippleSpecialAbility(1.0, this, defender, .25, 15, -1, false, "", "", "-1");
            SpecialAbilities.DisorientSpecialAbility(1.0, this, defender, .15, 15, -1, false, "", "", "-1");
            */
        }

        public void Swallow()
        {
            if (!SpecialAbilities.Exists(this)) return;
            if (!SpecialAbilities.Exists(Combatant)) return;
            if (m_SwallowedMobiles.Contains(Combatant)) return;

            Mobile mobileTarget = Combatant;

            Combatant = null;

            double directionDelay = .25;
            double initialDelay = 1;
            double totalDelay = 1 + directionDelay + initialDelay;

            Direction direction = Utility.GetDirection(Location, mobileTarget.Location);

            PlaySound(0x5DA);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, totalDelay, true, 0, false, "", "", "-1");

            m_NextSwallowAllowed = DateTime.UtcNow + NextSwallowDelay + TimeSpan.FromSeconds(totalDelay);
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(totalDelay);

            AbilityInProgress = true;

            Timer.DelayCall(TimeSpan.FromSeconds(totalDelay), delegate
            {
                if (SpecialAbilities.Exists(this))
                    AbilityInProgress = false;
            });

            Timer.DelayCall(TimeSpan.FromSeconds(directionDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                Animate(11, 15, 1, true, false, 0);
                PlaySound(0x5DA);

                double spawnPercent = (double)intervalCount / (double)totalIntervals;

                int maxExtraSwallowDuration = 10;
                double swallowDuration = 10 + (Math.Ceiling((double)maxExtraSwallowDuration * spawnPercent));

                PublicOverheadMessage(MessageType.Regular, 0, false, "*swallows " + mobileTarget.Name + "*");
                m_SwallowedMobiles.Add(mobileTarget);

                mobileTarget.Location = Location;

                double extraDamage = DamageMax;
                double damage = (double)DamageMin + (extraDamage * spawnPercent);

                if (mobileTarget is BaseCreature)
                    damage *= 1.5;

                if (mobileTarget is PlayerMobile)
                    damage *= .5;

                SpecialAbilities.BleedSpecialAbility(1.0, this, mobileTarget, damage, swallowDuration, 0, true, "", "", "-1");
                SpecialAbilities.HinderSpecialAbility(1.0, null, mobileTarget, 1.0, swallowDuration, false, -1, false, "", "You have been 'engulfed' and cannot move or speak!", "-1");

                mobileTarget.Squelched = true;
                mobileTarget.Hidden = true;

                Timer.DelayCall(TimeSpan.FromSeconds(swallowDuration), delegate
                {
                    if (mobileTarget == null) return;
                    if (mobileTarget.Deleted) return;

                    mobileTarget.Squelched = false;
                    mobileTarget.Hidden = false;

                    if (!SpecialAbilities.Exists(this))
                        return;

                    if (m_SwallowedMobiles.Contains(mobileTarget))
                        m_SwallowedMobiles.Remove(mobileTarget);

                    if (!mobileTarget.Alive)
                        return;

                    PlaySound(0x56C);

                    PublicOverheadMessage(MessageType.Regular, 0, false, "*spits out " + mobileTarget.Name + "*");

                    int knockbackDistance = 10;
                    damage = 10;

                    SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, mobileTarget, damage, knockbackDistance, -1, "", "Maggot spits you out!");

                    for (int a = 0; a < 6; a++)
                    {
                        TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                        ichor.Hue = 2051;
                        ichor.Name = "ichor";

                        Point3D newPoint = new Point3D(mobileTarget.Location.X + Utility.RandomList(-1, 1), mobileTarget.Location.Y + Utility.RandomList(-1, 1), mobileTarget.Location.Z);
                        SpellHelper.AdjustField(ref newPoint, mobileTarget.Map, 12, false);

                        ichor.MoveToWorld(newPoint, mobileTarget.Map);
                    }
                });
            });  
        }

        public void SpawnCreatures()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int maxExtraCreatures = 8;
            int creatures = 2 + (int)Math.Ceiling((double)maxExtraCreatures * spawnPercent);

            PublicOverheadMessage(MessageType.Regular, 0, false, "*creatures burst from maggot*");

            double stationaryDelay = 3;

            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(.5);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;
            });

            for (int a = 0; a < creatures; a++)
            {
                List<Point3D> m_Locations = SpecialAbilities.GetSpawnableTiles(Location, false, true, Location, Map, 1, 10, 1, 3, true);

                Point3D newLocation = new Point3D();

                if (m_Locations.Count > 0)
                    newLocation = m_Locations[0];

                else
                    newLocation = Location;

                //TEST
                //ADD PLAGUE BEAST

                if (Utility.RandomDouble() >= .2)
                {
                    Entrail entrail = new Entrail();
                    entrail.MoveToWorld(newLocation, Map);
                    m_Creatures.Add(entrail);

                    new Blood().MoveToWorld(entrail.Location, Map);
                }

                else
                {
                    DiseasedViscera diseasedViscera = new DiseasedViscera();
                    diseasedViscera.MoveToWorld(newLocation, Map);
                    m_Creatures.Add(diseasedViscera);

                    new Blood().MoveToWorld(diseasedViscera.Location, Map);
                }
            }
        }

        public void MassiveCorpseExplosion()
        {
            if (!SpecialAbilities.Exists(this))
                return;

            PublicOverheadMessage(MessageType.Regular, 0, false, "*violently erupts*");

            double spawnPercent = (double)intervalCount / (double)totalIntervals;

            int maxExtraParts = 80;
            int parts = 40 + (int)(Math.Ceiling((double)maxExtraParts * spawnPercent));

            double stationaryDelay = 5;

            AbilityInProgress = true;
            DamageIntervalInProgress = true;
            m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay + TimeSpan.FromSeconds(stationaryDelay);

            SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, stationaryDelay, true, 0, false, "", "", "-1");

            Timer.DelayCall(TimeSpan.FromSeconds(stationaryDelay), delegate
            {
                if (!SpecialAbilities.Exists(this))
                    return;

                AbilityInProgress = false;
                DamageIntervalInProgress = false;
                m_NextAbilityAllowed = DateTime.UtcNow + NextAbilityDelay;
            });

            PlaySound(GetAngerSound());

            SpecialAbilities.CorpseExplosionAbility(this, Location, Location, false, true, Map, 0, 18, parts, parts);
        }    

        public override void OnThink()
        {
            base.OnThink();
           
            if (Utility.RandomDouble() < 0.01 && !Hidden && DateTime.UtcNow > m_NextSpeechAllowed)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                m_NextSpeechAllowed = DateTime.UtcNow + NextSpeechDelay;
            }

            if (SpecialAbilities.MonsterCanDamage(this, Combatant) && DateTime.UtcNow >= m_NextAbilityAllowed && !Frozen && !AbilityInProgress && !DamageIntervalInProgress)
            {                
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1:
                        if (DateTime.UtcNow >= m_NextSwallowAllowed && Utility.GetDistance(Location, Combatant.Location) <= AttackRange)
                        {
                            Swallow();
                            return;
                        }
                    break;

                    case 2:
                        //if (DateTime.UtcNow >= m_NextMassiveBreathAllowed)
                        //{
                            //MassiveBoneBreath();
                            //return;
                        //}
                    break;
                }                
            }

            if (Utility.RandomDouble() < .01 && DateTime.UtcNow > m_NextAIChangeAllowed && CantWalk == false)
            {
                Effects.PlaySound(Location, Map, GetAngerSound());

                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*groans*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                        break;

                    case 2:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*gurgle*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                    break;

                    case 3:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*grumble*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                    break;

                    case 4:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*gulp*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 10;
                        DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                    break;

                    case 5:
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*grimaces*");

                        DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.WeakToPoison] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 0;
                        DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 10;
                    break;
                }

                m_NextAIChangeAllowed = DateTime.UtcNow + NextAIChangeDelay;
            }            
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetAngerSound() { return 0x59F; }
        public override int GetIdleSound() { return 0x571; }
        public override int GetAttackSound() { return 0x573; }
        public override int GetHurtSound() { return 0x570; }
        public override int GetDeathSound() { return 0x59D; }        

        protected override bool OnMove(Direction d)
        {
            foreach (Mobile mobile in m_SwallowedMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;
                if (Utility.GetDistance(mobile.Location, Location) > 6) continue;
                
                mobile.Location = Location;
            }

            for (int a = 0; a < 2; a++)
            {
                new Blood().MoveToWorld(new Point3D(this.X + Utility.RandomMinMax(-1, 1), this.Y + Utility.RandomMinMax(-1, 1), this.Z), this.Map);
            }

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);                       
        }

        public override void OnBeforeSpawn(Point3D location, Map m)
        {
            base.OnBeforeSpawn(location, m);

            BossPersistance.PersistanceItem.DespiseBossLastStatusChange = DateTime.UtcNow;
        }

        public override bool OnBeforeDeath()
        {
            foreach (Mobile mobile in m_SwallowedMobiles)
            {
                if (mobile == null) continue;
                if (mobile.Deleted) continue;

                mobile.Squelched = false;
                mobile.Hidden = false;

                Point3D ichorLocation = new Point3D(mobile.Location.X + Utility.RandomMinMax(-2, 2), mobile.Location.Y + Utility.RandomMinMax(-2, 2), mobile.Location.Z);
                SpellHelper.AdjustField(ref ichorLocation, mobile.Map, 12, false);

                TimedStatic ichor = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                ichor.Hue = 2051;
                ichor.Name = "ichor";
                ichor.MoveToWorld(ichorLocation, mobile.Map);

                Effects.PlaySound(mobile.Location, mobile.Map, Utility.RandomList(0x101));
            }

            MassiveCorpseExplosion();

            CantWalk = true;
            ActiveSpeed = 1.5;
            PassiveSpeed = 1.5;
            CurrentSpeed = 1.5;            

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 10) == 1)
                c.AddItem(new MaggotStatue());

            if (Utility.RandomMinMax(1, 20) == 1)
                c.AddItem(new MaggotsMeat());
            
            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }            
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
           
            for (int a = 0; a < m_Creatures.Count; ++a)
            {
                if (m_Creatures[a] != null)
                {
                    if (m_Creatures[a].Alive)
                        m_Creatures[a].Kill();
                }
            }            
        }

        public Maggot(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
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

            //Version 0
            writer.Write(m_SwallowedMobiles.Count);

            for (int a = 0; a < m_SwallowedMobiles.Count; a++)
            {
                writer.Write(m_SwallowedMobiles[a]);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            damageIntervalThreshold = reader.ReadInt();
            damageProgress = reader.ReadInt();
            intervalCount = reader.ReadInt();
            totalIntervals = reader.ReadInt();

            m_Creatures = new List<Mobile>();
            
            int creaturesCount = reader.ReadInt();
            for (int a = 0; a < creaturesCount; a++)
            {
                Mobile creature = reader.ReadMobile();

                m_Creatures.Add(creature);
            }            

            int swalledMobilesCount = reader.ReadInt();

            for (int a = 0; a < swalledMobilesCount; a++)
            {
                m_SwallowedMobiles.Add((Mobile)reader.ReadMobile());
            }
        }
    }
}
