using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;
using Server.Spells;


namespace Server.Mobiles
{
	public class UOACZBaseUndead : BaseCreature
	{
        public static List<UOACZBaseUndead> m_Creatures = new List<UOACZBaseUndead>();

        public enum BonePileType
        {
            Small,
            Medium,
            Large
        }

        private bool m_InWilderness = false;
        [CommandProperty(AccessLevel.Counselor)]
        public bool InWilderness
        {
            get { return m_InWilderness; }
            set { m_InWilderness = value; }
        }

        private bool m_CanTeleportToBaseNode = true;
        [CommandProperty(AccessLevel.Counselor)]
        public bool CanTeleportToBaseNode
        {
            get { return m_CanTeleportToBaseNode; }
            set { m_CanTeleportToBaseNode = value; }
        }

        private bool m_UseMoveToWorldHandling = true;
        [CommandProperty(AccessLevel.Counselor)]
        public bool UseMoveToWorldHandling
        {
            get { return m_UseMoveToWorldHandling; }
            set { m_UseMoveToWorldHandling = value; }
        }

        public virtual int DifficultyValue { get { return 1; } }

        public override bool AllowParagon { get { return false; } }
        public override bool CanRummageCorpses { get { return false; } }

        public virtual string[] idleSpeech { get{ return new string[0];} }
        public virtual string[] combatSpeech { get { return new string[0]; } }

        public override bool PlayerRangeSensitive { get { return false; } }
        public override int WaypointCompletionProximityNeeded { get { return 3; } }

        //For Guarding Locations: If GuardsHome is Toggled
        public override int MaxDistanceAllowedFromHome { get { return 50; } }

        public bool m_NeedWaypoint = false;

        public DateTime m_LastActivity = DateTime.Now;
        public static TimeSpan InactivityThreshold = TimeSpan.FromMinutes(5);
        
        public BreakableStatic m_LastBreakableStaticAttacked;

        public DateTime m_NextBreakableStaticCheckAllowed = DateTime.UtcNow;
        public TimeSpan BreakableStaticCheckDelay = TimeSpan.FromSeconds(2);

        public DateTime m_NextBreakableStaticAttackAllowed = DateTime.UtcNow;
        public TimeSpan BreakableStaticAttackDelay = TimeSpan.FromSeconds(5);

        public DateTime m_NextMoveSoundAllowed = DateTime.UtcNow;
        public TimeSpan MoveSoundDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 15));

        public UOACZUndeadSpawner m_Spawner;

        public override int PoisonResistance { get { return 5; } }

        public virtual int DeathAnimation { get { return Utility.RandomMinMax(2, 3); } }
        public virtual int DeathAnimationFrames { get { return 4; } }

        public virtual BonePileType BonePile { get { return BonePileType.Small; } }

        private bool m_GuardsHome = false;
        [CommandProperty(AccessLevel.Counselor)]
        public bool GuardsHome
        {
            get { return m_GuardsHome; }
            set { m_GuardsHome = value; }
        }

        public virtual string ReturnHomeString { get { return "*returns back to it's lair*"; } }
        
        [Constructable]
		public UOACZBaseUndead() : base( AIType.AI_Archer, FightMode.Closest, 15, 1, 0.2, 0.4 )
		{
            m_LastActivity = DateTime.UtcNow;

            m_Creatures.Add(this);
		}

        public override void SetUniqueAI()
        {
            DictCombatTargeting[CombatTargeting.PlayerAny] = 0;

            DictCombatTargeting[CombatTargeting.UOACZIgnoreHumanSentry] = 100;

            DictCombatTargeting[CombatTargeting.UOACZWildlife] = 1;
            DictCombatTargeting[CombatTargeting.UOACZHuman] = 3;
            DictCombatTargeting[CombatTargeting.UOACZHumanPlayer] = 5;            

            DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 4;

            DictCombatFlee[CombatFlee.Flee50] = 0;
            DictCombatFlee[CombatFlee.Flee25] = 0;
            DictCombatFlee[CombatFlee.Flee10] = 0;
            DictCombatFlee[CombatFlee.Flee5] = 0;

            ActiveSpeed = 0.4;
            PassiveSpeed = 0.5;

            RevealImmune = true;

            ResolveAcquireTargetDelay = 2;

            RangePerception = 12;
            DefaultPerceptionRange = 12;

            Fame = 1000;
            Karma = -1000;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();

            Point3D location = Location;
            Map map = Map;

            //Outside of Guard Area
            if (GuardsHome && !(ControlMaster is PlayerMobile))
            {                
                if (Utility.GetDistance(Home, location) > MaxDistanceAllowedFromHome)
                {
                    TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                    dirt.Name = "dirt";
                    dirt.MoveToWorld(location, map);

                    dirt.PublicOverheadMessage(MessageType.Regular, 0, false, ReturnHomeString);

                    Effects.PlaySound(location, map, 0x657);

                    int projectiles = 6;
                    int particleSpeed = 4;

                    for (int a = 0; a < projectiles; a++)
                    {
                        Point3D newLocation = new Point3D(location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), location.Z);
                        SpellHelper.AdjustField(ref newLocation, map, 12, false);

                        IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(location.X, location.Y, location.Z + 5), map);
                        IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                        Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
                    }

                    Location = Home;
                    Combatant = null;

                    return;
                }
            }

            if (m_NeedWaypoint && m_CanTeleportToBaseNode && !(ControlMaster is PlayerMobile))
            {
                Dictionary<UOACZWayPoint, int> DictWaypoints = new Dictionary<UOACZWayPoint, int>();
            
                foreach (UOACZWayPoint waypoint in UOACZWayPoint.m_UOACZWaypoints)
                {
                    if (!UOACZRegion.ContainsItem(waypoint)) continue;
                    if (!waypoint.IsBaseNode) continue;
                    
                    int distanceToWaypoint = (Utility.GetDistance(Location, waypoint.Location));

                    if (distanceToWaypoint >= UOACZSystem.UndeadMaxDynamicWaypointRange) continue;

                    int distanceWeight = (int)(Math.Round(((double)UOACZSystem.UndeadMaxDynamicWaypointRange - (double)distanceToWaypoint) * .05));

                    if (distanceWeight < 1)
                        distanceWeight = 1;

                    DictWaypoints.Add(waypoint, distanceWeight);
                }

                if (DictWaypoints.Count > 0)
                {
                    int TotalValues = 0;

                    foreach (KeyValuePair<UOACZWayPoint, int> pair in DictWaypoints)
                    {
                        TotalValues += pair.Value;
                    }

                    double ItemCheck = Utility.RandomDouble();

                    double CumulativeAmount = 0.0;
                    double AdditionalAmount = 0.0;

                    foreach (KeyValuePair<UOACZWayPoint, int> pair in DictWaypoints)
                    {
                        AdditionalAmount = (double)pair.Value / (double)TotalValues;

                        if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                        {
                            UOACZWayPoint waypoint = pair.Key;

                            if (waypoint != null)
                            {
                                if (!waypoint.Deleted)
                                {
                                    CurrentWaypoint = pair.Key;
                                    m_NeedWaypoint = false;

                                    return;
                                }
                            }

                            break;
                        }

                        CumulativeAmount += AdditionalAmount;
                    }
                }

                else                
                    m_NeedWaypoint = false;                
            }

            if ((DateTime.UtcNow >= m_LastActivity + InactivityThreshold) && !(ControlMaster is PlayerMobile))
            {
                //Send to Random Town Waypoint BaseNode
                if (m_CanTeleportToBaseNode)
                {
                    List<UOACZWayPoint> m_ValidBaseNodes = new List<UOACZWayPoint>();

                    foreach (UOACZWayPoint waypoint in UOACZWayPoint.m_UOACZWaypoints)
                    {
                        if (waypoint == null) continue;
                        if (waypoint.Deleted) continue;
                        if (!UOACZRegion.ContainsItem(waypoint)) continue;
                        if (!waypoint.IsBaseNode) continue;
                        if (waypoint.WaypointType == UOACZWayPoint.UOACZWaypointType.UndeadWilderness) continue;

                        m_ValidBaseNodes.Add(waypoint);                       
                    }

                    if (m_ValidBaseNodes.Count > 0)
                    {
                        UOACZWayPoint targetWaypoint = m_ValidBaseNodes[Utility.RandomMinMax(0, m_ValidBaseNodes.Count - 1)];

                        MoveToWorld(targetWaypoint.Location, targetWaypoint.Map);
                        CurrentWaypoint = targetWaypoint;
                        m_LastActivity = DateTime.UtcNow;

                        m_NeedWaypoint = false;
                        return;
                    }
                }                

                m_NeedWaypoint = true;
                return;               
            }

            if (Utility.RandomDouble() < 0.005)
            {
                if (Combatant == null)
                    Say(idleSpeech[Utility.Random(idleSpeech.Length - 1)]);

                else
                    Say(combatSpeech[Utility.Random(combatSpeech.Length - 1)]);
            }
            
            bool ignoreBreakableStatics = false;

            if (Hidden && ControlMaster is PlayerMobile)
                ignoreBreakableStatics = true;

            if (DateTime.UtcNow >= m_NextBreakableStaticCheckAllowed && DateTime.UtcNow >= m_NextBreakableStaticAttackAllowed && !ignoreBreakableStatics)
            {
                m_NextBreakableStaticCheckAllowed = DateTime.UtcNow + BreakableStaticCheckDelay;

                BreakableStatic targetBreakableStatic = null;
                Dictionary<BreakableStatic, int> DictBreakableStatics = new Dictionary<BreakableStatic, int>();

                IPooledEnumerable nearbyItems = Map.GetItemsInRange(Location, 1);

                foreach (Item item in nearbyItems)
                {
                    if (item.Deleted) continue;                   

                    if (item is BreakableStatic)
                    {
                        if (!Map.InLOS(Location, item.Location)) continue;                        

                        BreakableStatic breakableStatic = item as BreakableStatic;

                        if (breakableStatic is UOACZBreakableStatic)
                        {
                            UOACZBreakableStatic uoaczBreakableStatic = breakableStatic as UOACZBreakableStatic;

                            if (uoaczBreakableStatic.AllowHumanDamage)
                                continue;
                        }
                        
                        if (breakableStatic.DamageState == BreakableStatic.DamageStateType.Broken)
                            continue;
                        
                        int weight = 5;

                        if (breakableStatic == m_LastBreakableStaticAttacked)
                            weight += 10;

                        int damageBonus = (int)Math.Round((10 * (1 - (double)breakableStatic.HitPoints / (double)breakableStatic.MaxHitPoints)));

                        weight += damageBonus;
                        
                        DictBreakableStatics.Add(breakableStatic, weight);                        
                    }
                }

                nearbyItems.Free();

                int TotalValues = 0;

                foreach (KeyValuePair<BreakableStatic, int> pair in DictBreakableStatics)
                {
                    TotalValues += pair.Value;
                }

                double ItemCheck = Utility.RandomDouble();

                double CumulativeAmount = 0.0;
                double AdditionalAmount = 0.0;
                  
                foreach (KeyValuePair<BreakableStatic, int> pair in DictBreakableStatics)
                {
                    AdditionalAmount = (double)pair.Value / (double)TotalValues;

                    if (ItemCheck >= CumulativeAmount && ItemCheck < (CumulativeAmount + AdditionalAmount))
                    {
                        targetBreakableStatic = pair.Key;
                        break;
                    }

                    CumulativeAmount += AdditionalAmount;
                }

                if (targetBreakableStatic != null)
                {
                    double minHinder = 1;
                    double maxHinder = 1;

                    double weaponUsageDelay = 3;

                    BaseWeapon weapon = Weapon as BaseWeapon;

                    if (weapon != null)
                        weaponUsageDelay = weapon.GetDelay(this, false).TotalSeconds;

                    bool stockpileNearby = false;

                    if (targetBreakableStatic is UOACZStockpile)                    
                        stockpileNearby = true;                    

                    else
                    {
                        switch (targetBreakableStatic.DamageState)
                        {
                            case BreakableStatic.DamageStateType.Normal:
                                minHinder = weaponUsageDelay * .5;
                                maxHinder = weaponUsageDelay;
                            break;

                            case BreakableStatic.DamageStateType.LightlyDamaged:
                                minHinder = weaponUsageDelay * .65;
                                maxHinder = weaponUsageDelay;
                            break;

                            case BreakableStatic.DamageStateType.HeavilyDamaged:
                                minHinder = weaponUsageDelay * .8;
                                maxHinder = weaponUsageDelay;
                            break;
                        }
                    }

                    double delay = minHinder + ((maxHinder - minHinder) * Utility.RandomDouble());

                    if (ControlMaster is PlayerMobile)
                    {
                        delay = 1;
                        SpecialAbilities.EntangleSpecialAbility(1.0, null, this, 1, delay, 0, false, "", "", "-1");
                    }

                    else
                    {

                        if (stockpileNearby)
                        {
                            delay = 10;

                            SpecialAbilities.EntangleSpecialAbility(1.0, null, this, 1, delay, 0, false, "", "", "-1");
                            m_NextBreakableStaticAttackAllowed = DateTime.UtcNow + BreakableStaticAttackDelay;
                        }

                        else
                        {
                            if (Utility.RandomDouble() <= .5)
                                SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, delay, true, 0, false, "", "", "-1");

                            else
                                SpecialAbilities.EntangleSpecialAbility(1.0, null, this, 1, delay, 0, false, "", "", "-1");

                            m_NextBreakableStaticAttackAllowed = DateTime.UtcNow + BreakableStaticAttackDelay;
                        }
                    }

                    targetBreakableStatic.Interact(this, BreakableStatic.InteractionType.Weapon);                                     
                }                
            }
        }

        public override void MoveToWorld(Point3D newLocation, Map map)
        {
            base.MoveToWorld(newLocation, map);

            if (!UseMoveToWorldHandling)
                return;

            int animation = Utility.RandomMinMax(2, 3);
            int frameCount = 4;

            int bonesItemID = 6937;            

            int radius = 1;
            double smallBonesChance = .2;

            switch (BonePile)
            {
                case BonePileType.Small:
                    bonesItemID = Utility.RandomList(6921, 6925);
                    smallBonesChance = .25;
                break;

                case BonePileType.Medium:
                    bonesItemID = Utility.RandomList(6922, 6923, 6924);
                    smallBonesChance = .33;
                break;

                case BonePileType.Large:
                    bonesItemID = Utility.RandomList(6927, 6928);
                    radius = 2;
                    smallBonesChance = .66;
                break;                  
            }

            TimedStatic bones = new TimedStatic(bonesItemID, 10);
            bones.Name = "bones";
            bones.MoveToWorld(newLocation, map);

            for (int a = -1 * radius; a < radius + 1; a++)
            {
                for (int b = -1 * radius; b < radius + 1; b++)
                {
                    Point3D newPoint = new Point3D(newLocation.X + a, newLocation.Y + b, newLocation.Z);
                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                    double distanceFromCenter = Utility.GetDistanceToSqrt(newLocation, newPoint);

                    double extraBonesChance = 1;

                    if (distanceFromCenter >= 1)
                        extraBonesChance = (1 / (distanceFromCenter)) * smallBonesChance;
                    
                    if (Utility.RandomDouble() <= extraBonesChance)                    
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(distanceFromCenter * .25), delegate
                        {
                            TimedStatic smallBones = new TimedStatic(Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6882, 6883), 5);
                            smallBones.Name = "bones";
                            smallBones.MoveToWorld(newPoint, map);
                        });
                    }
                }
            }                        

            PlaySound(GetIdleSound());

            Hidden = true;
            Frozen = true;

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (this == null) return;
                if (Deleted || !Alive) return;

                Hidden = false;
                Frozen = false;

                PlaySound(GetAngerSound());
                Animate(animation, frameCount, 1, false, false, 1);                
            });
        }

        protected override bool OnMove(Direction d)
        {
            if (!Hidden && DateTime.UtcNow >= m_NextMoveSoundAllowed)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(GetAngerSound(), GetIdleSound()));

                m_NextMoveSoundAllowed = DateTime.UtcNow + MoveSoundDelay;
            }

            return base.OnMove(d);
        }

        public static Type GetRandomUndeadType(int minThreat, int maxThreat)
        {
            if (minThreat < 0)
                minThreat = 0;

            if (maxThreat < 0)
                maxThreat = 0;

            if (maxThreat < minThreat)
                maxThreat = minThreat;

            int threatResult = Utility.RandomMinMax(minThreat, maxThreat);

            Type creatureType = null;

            if (threatResult <= 100)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: creatureType = typeof(UOACZZombie); break;
                    case 2: creatureType = typeof(UOACZSkeleton); break;
                    case 3: creatureType = typeof(UOACZGhoul); break;
                    case 4: creatureType = typeof(UOACZSkeletalCritter); break;
                }
            }

            else if (threatResult <= 110)
            {
                switch (Utility.RandomMinMax(1, 8))
                {
                    case 1: creatureType = typeof(UOACZZombieMagi); break;
                    case 2: creatureType = typeof(UOACZSpectre); break;
                    case 3: creatureType = typeof(UOACZPatchworkSkeleton); break;
                    case 4: creatureType = typeof(UOACZNecromancer); break;
                    case 5: creatureType = typeof(UOACZVoidSlime); break;
                    case 6: creatureType = typeof(UOACZCorpseEater); break;
                    case 7: creatureType = typeof(UOACZSkeletalHorse); break;
                    case 8: creatureType = typeof(UOACZGiantBat); break;
                }
            }

            else if (threatResult <= 120)
            {
                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: creatureType = typeof(UOACZDecayedZombie); break;
                    case 2: creatureType = typeof(UOACZGhost); break;
                    case 3: creatureType = typeof(UOACZPitTentacle); break;
                    case 4: creatureType = typeof(UOACZSkeletalMage); break;
                    case 5: creatureType = typeof(UOACZFailedExperiment); break;
                }
            }

            else if (threatResult <= 130)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: creatureType = typeof(UOACZBloodyZombie); break;
                    case 2: creatureType = typeof(UOACZCorpseBride); break;
                    case 3: creatureType = typeof(UOACZVampireBat); break;
                    case 4: creatureType = typeof(UOACZSkeletalKnight); break;
                }
            }

            else if (threatResult <= 140)
            {
                switch (Utility.RandomMinMax(1, 5))
                {
                    case 1: creatureType = typeof(UOACZDarkWisp); break;
                    case 2: creatureType = typeof(UOACZNightmare); break;
                    case 3: creatureType = typeof(UOACZVampireThrall); break;
                    case 4: creatureType = typeof(UOACZFlamingZombie); break;
                    case 5: creatureType = typeof(UOACZShadowFiend); break;
                }
            }

            else if (threatResult <= 150)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: creatureType = typeof(UOACZLich); break;
                    case 2: creatureType = typeof(UOACZMummy); break;
                    case 3: creatureType = typeof(UOACZPhantom); break;
                    case 4: creatureType = typeof(UOACZPlagueRat); break;
                }
            }

            else if (threatResult <= 160)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: creatureType = typeof(UOACZVampireCountess); break;
                    case 2: creatureType = typeof(UOACZSkeletalDrake); break;
                    case 3: creatureType = typeof(UOACZFountainOfEvil); break;
                    case 4: creatureType = typeof(UOACZRagWitch); break;
                }
            }

            else if (threatResult <= 170)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: creatureType = typeof(UOACZFleshGolem); break;
                    case 2: creatureType = typeof(UOACZLichLord); break;
                    case 3: creatureType = typeof(UOACZRottingCorpse); break;
                    case 4: creatureType = typeof(UOACZGiantSkeleton); break;
                }
            }

            else
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: creatureType = typeof(UOACZShadowKnight); break;
                    case 2: creatureType = typeof(UOACZEnvelopingDarkness); break;
                    case 3: creatureType = typeof(UOACZShadowDragon); break;
                }
            }

            return creatureType;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            //Player Swarm Followers
            if (TimesTamed > 0)
                return;

            int scoreValue = 1;

            int intestines = 0;
            int bones = 0;

            double intestineChance = 0;
            double normalItemChance = 0;
            double magicItemChance = 0;
            double survivalStoneChance = 0;
            double upgradeTokenChance = 0;

            #region Upgrade Chances
            
            switch (DifficultyValue)
            {
                case 1:
                    scoreValue = 1;
                    bones = 1;
                    intestines = 1;                   

                    intestineChance = .20;

                    normalItemChance = .1;
                    magicItemChance = .02;                   
                    survivalStoneChance = .04;
                    upgradeTokenChance = .02;
                break;

                case 2:
                    scoreValue = 1;
                    bones = 1;
                    intestines = 1;                    

                    intestineChance = .20;

                    normalItemChance = 0.125;
                    magicItemChance = 0.025;
                    survivalStoneChance = .05;
                    upgradeTokenChance = .025;
                break;

                case 3:
                    scoreValue = 2;
                    bones = 2;
                    intestines = 1;                   

                    intestineChance = .30;

                    normalItemChance = .15;
                    magicItemChance = .03;
                    survivalStoneChance = .06;
                    upgradeTokenChance = .03;
                break;

                case 4:
                    scoreValue = 2;
                    bones = 2;
                    intestines = 1;                    

                    intestineChance = .4;

                    normalItemChance = .2;
                    magicItemChance = .04;
                    survivalStoneChance = .08;
                    upgradeTokenChance = .04;
                break;

                case 5:
                    scoreValue = 3;
                    bones = 3;
                    intestines = 1;                    

                    intestineChance = .5;

                    normalItemChance = .25;
                    magicItemChance = .05;
                    survivalStoneChance = .1;
                    upgradeTokenChance = .05;
                break;

                case 6:
                    scoreValue = 3;
                    bones = 3;
                    intestines = 1;                    

                    intestineChance = .6;

                    normalItemChance = .3;
                    magicItemChance = .06;
                    survivalStoneChance = .12;
                    upgradeTokenChance = .06;
                break;

                case 7:
                    scoreValue = 4;
                    bones = 4;
                    intestines = 1;                    

                    intestineChance = .7;

                    normalItemChance = .4;
                    magicItemChance = .08;
                    survivalStoneChance = .15;
                    upgradeTokenChance = 0.075;
                break;

                case 8:
                    scoreValue = 5;
                    bones = 5;
                    intestines = 1;                    

                    intestineChance = .8;

                    normalItemChance = .5;
                    magicItemChance = .1;
                    survivalStoneChance = .2;
                    upgradeTokenChance = .1;
                break;

                case 9:
                    scoreValue = 6;
                    bones = 6;
                    intestines = 1;                   

                    intestineChance = .9;

                    normalItemChance = 1;
                    magicItemChance = .2;
                    survivalStoneChance = .25;
                    upgradeTokenChance = .125;
                break;

                case 10:
                    scoreValue = 40;
                    bones = 30;
                    intestines = 1;                    

                    intestineChance = 1;

                    normalItemChance = 2;
                    magicItemChance = 2;
                    survivalStoneChance = 2;
                    upgradeTokenChance = 2;
                break;

                case 11:
                    scoreValue = 80;
                    bones = 50;
                    intestines = 1;                    

                    intestineChance = 1;

                    normalItemChance = 2;
                    magicItemChance = 2;
                    survivalStoneChance = 2;
                    upgradeTokenChance = 2;
                break;                
            }            

            #endregion

            Dictionary<PlayerMobile, int> m_PlayerDamageDealt = new Dictionary<PlayerMobile, int>();
            List<PlayerMobile> m_PotentialPlayers = new List<PlayerMobile>();

            bool playerThresholdReached = false;

            int totalDamage = 0;
            int totalPlayerDamage = 0;

            //Determine Total Damaged Inflicted and Per Player
            foreach (DamageEntry entry in DamageEntries)
            {
                if (!entry.HasExpired)
                {
                    Mobile damager = entry.Damager;

                    if (damager == null) continue;

                    totalDamage += entry.DamageGiven;

                    PlayerMobile playerDamager = damager as PlayerMobile;

                    if (playerDamager != null)
                        totalPlayerDamage += entry.DamageGiven;

                    BaseCreature creatureDamager = damager as BaseCreature;

                    if (creatureDamager != null)
                    {
                        if (creatureDamager.ControlMaster is PlayerMobile)
                            totalPlayerDamage += entry.DamageGiven;
                    }
                }
            }

            foreach (DamageEntry entry in DamageEntries)
            {
                if (!entry.HasExpired && entry.DamageGiven > 0)
                {
                    PlayerMobile player = null;

                    Mobile damager = entry.Damager;

                    if (damager == null) continue;
                    if (damager.Deleted) continue;

                    PlayerMobile pm_Damager = damager as PlayerMobile;
                    BaseCreature bc_Damager = damager as BaseCreature;

                    if (pm_Damager != null)
                        player = pm_Damager;

                    if (bc_Damager != null)
                    {
                        if (bc_Damager.Controlled && bc_Damager.ControlMaster is PlayerMobile)
                        {
                            if (!bc_Damager.ControlMaster.Deleted)
                                player = bc_Damager.ControlMaster as PlayerMobile;
                        }
                    }

                    if (player != null)
                    {
                        if (m_PlayerDamageDealt.ContainsKey(player))
                            m_PlayerDamageDealt[player] += entry.DamageGiven;

                        else
                            m_PlayerDamageDealt.Add(player, entry.DamageGiven);
                    }
                }
            }

            Queue m_Queue = new Queue();

            foreach (KeyValuePair<PlayerMobile, int> playerEntry in m_PlayerDamageDealt)
            {
                PlayerMobile player = playerEntry.Key;
                int damage = playerEntry.Value;

                if (player.IsUOACZHuman)
                {
                    double damagePercentOfTotal = (double)damage / totalDamage;

                    if (damage >= 100 || damagePercentOfTotal > .10)
                    {
                        UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
                        player.m_UOACZAccountEntry.UndeadKilledAsHuman++;                        

                        UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanScore, scoreValue, true);

                        m_PotentialPlayers.Add(player);
                    }
                }
            }

            if (totalDamage == 0)
                totalDamage = 1;

            double playerDamageRatio = (double)totalPlayerDamage / (double)totalDamage;
            double npcHelpScalar = playerDamageRatio;

            intestineChance *= UOACZPersistance.HumanBalanceScalar;
            normalItemChance *= UOACZPersistance.HumanBalanceScalar;
            magicItemChance *= UOACZPersistance.HumanBalanceScalar;
            survivalStoneChance *= UOACZPersistance.HumanBalanceScalar;
            upgradeTokenChance *= UOACZPersistance.HumanBalanceScalar;

            if (playerDamageRatio >= .25)
            {
                if (bones > 0)
                    c.AddItem(new Bone(bones));

                if (Utility.RandomDouble() <= (intestineChance * npcHelpScalar))
                {
                    for (int a = 0; a < intestines; a++)
                    {
                        c.AddItem(new UOACZIntestines());
                    }
                }
            }

            if (Utility.RandomDouble() <= (normalItemChance * npcHelpScalar))
            {
                if (Utility.RandomDouble() <= .4)
                {
                    BaseWeapon weapon = UOACZSystem.GetRandomCrudeBoneWeapon();

                    if (weapon != null)
                        c.AddItem(weapon);
                }

                else
                {
                    BaseArmor armor = UOACZSystem.GetRandomCrudeBoneArmor();

                    if (armor != null)
                        c.AddItem(armor);
                }
            }

            if (Utility.RandomDouble() <= (magicItemChance * npcHelpScalar))
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1:
                        BaseWeapon weapon = Loot.RandomWeapon();

                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.RandomMinMax(1, 2);
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.RandomMinMax(1, 2);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.RandomMinMax(1, 2);
                        
                        weapon.Identified = true;

                        c.AddItem(weapon);
                    break;

                    case 2:
                        BaseArmor armor = Loot.RandomArmorOrShield();

                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.RandomMinMax(1, 2);
                        armor.DurabilityLevel = (ArmorDurabilityLevel)Utility.RandomMinMax(1, 2);

                        armor.Identified = true;

                        c.AddItem(armor);
                    break;
                }
            }
            
            bool dropSurvivalStone = false;
            bool dropUpgradeToken = false;

            if (m_PotentialPlayers.Count >= 3)
            {
                survivalStoneChance *= .75;
                upgradeTokenChance *= .75;
            }

            if (m_PotentialPlayers.Count >= 5)
            {
                survivalStoneChance *= .75;
                upgradeTokenChance *= .75;
            }

            if (Utility.RandomDouble() <= (survivalStoneChance * npcHelpScalar))
                dropSurvivalStone = true;

            if (Utility.RandomDouble() <= (upgradeTokenChance * npcHelpScalar))
                dropUpgradeToken = true;

            foreach (PlayerMobile player in m_PotentialPlayers)
            {
               if (dropSurvivalStone)
                    c.AddItem(new UOACZSurvivalStone(player));

                if (dropUpgradeToken)
                    c.AddItem(new UOACZHumanUpgradeToken(player));                
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Spawner != null)            
                m_Spawner.m_Mobiles.Remove(this);            
        }

        public UOACZBaseUndead(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version

            //Version 0
            writer.Write(m_NextBreakableStaticCheckAllowed);
            writer.Write(m_NextBreakableStaticAttackAllowed);
            writer.Write(m_LastActivity);
            writer.Write(m_NeedWaypoint);
            writer.Write(m_Spawner);
            writer.Write(m_InWilderness);           
            writer.Write(m_CanTeleportToBaseNode);
            writer.Write(m_GuardsHome);
            writer.Write(m_UseMoveToWorldHandling);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_NextBreakableStaticCheckAllowed = reader.ReadDateTime();
                m_NextBreakableStaticAttackAllowed = reader.ReadDateTime();
                m_LastActivity = reader.ReadDateTime();
                m_NeedWaypoint = reader.ReadBool();
                m_Spawner = (UOACZUndeadSpawner)reader.ReadItem();
                m_InWilderness = reader.ReadBool();                
                m_CanTeleportToBaseNode = reader.ReadBool();
                m_GuardsHome = reader.ReadBool();
                m_UseMoveToWorldHandling = reader.ReadBool();
            }

            //------------

            m_Creatures.Add(this);
		}
	}
}
