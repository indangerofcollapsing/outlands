using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using Server.Items;
using Server.Spells;
using System.Collections.Generic;
using Server.Network;

namespace Server.Custom.Pirates
{
    public abstract class BaseCannon : Item
    {
        public BaseBoat m_Boat;
        public Direction m_facing;

        public DateTime m_CooldownTime;

        public abstract BaseCannonDeed GetDeed { get; }

        public virtual double Accuracy { get { return .8; } }
        public virtual int DamageMin { get { return 20; } }
        public virtual int DamageMax { get { return 30; } }
        public virtual int CooldownTime { get { return 10; } }
        public virtual int ReloadTime { get { return 2; } }

        public virtual int NorthID { get { return 0; } }
        public virtual int EastID { get { return 0; } }
        public virtual int SouthID { get { return 0; } }
        public virtual int WestID { get { return 0; } }

        public int m_xOffset = 0;
        public int m_yOffset = 0;
        public int m_zOffset = 0;

        public int m_CurrentCharges;

        public static int MaxCharges = 10;
        public static int Range = 12;

        public const int ExplosionRange = 1; //How large is the cannon blast radius

        public const double OceanCreatureDamageMultiplier = .75; //Damage scalar for creatures (on Water)
        public const double MobileDamageMultiplier = .5; //Damage scalar for creatures & NPCs (on Land or Ships)
        public const double PlayerDamageMultiplier = .5; //Damage scalar for players (on Land or Ships)
        public const double IndirectHitDamageMultiplier = .25; //If explosion range is larger than 1, damage modifier for Mobiles outside of target location
        public const double ShipIndirectHitDamageMultiplier = .75; //Ship damage modifier if player directly hit instead of the ship

        public double m_MovementMaxAccuracyPenalty = 0.2; //Maximum Accuracy Penalty for Ship Moving or Having Recently Moved
        public double m_TargetMovementMaxAccuracyPenalty = 0.2; //Maximum Accuracy Penalty for Opponent's Moving or Having Recently Moved
        public double m_MovementAccuracyCooldown = 10.0; //Seconds after stopping ship movement before no penalty to accuracy exists: scales from 0 to this number of seconds

        public double m_MaxMisfireChance = 0.40;

        private Timer m_DelayedCannonFireTimer;
        private Timer m_ExplosionTimer;
        private Timer m_SplashTimer;        

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Direction Facing
        {
            get { return m_facing; }
            set { SetFacing(value); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int xOffset
        {
            get { return m_xOffset; }
            set { m_xOffset = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int yOffset
        {
            get { return m_yOffset; }
            set { m_yOffset = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int zOffset
        {
            get { return m_zOffset; }
            set { m_zOffset = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int CurrentCharges { get { return m_CurrentCharges; } set { m_CurrentCharges = value; } }

        public BaseCannon(Serial serial): base(serial)
        {
        }

        public BaseCannon(int itemID): base(itemID)
        {
            m_CurrentCharges = 0;
            Movable = false;
            m_CooldownTime = DateTime.UtcNow + TimeSpan.FromSeconds(CooldownTime);
        }

        public BaseCannon(int itemID, BaseBoat b)
        {
            m_CurrentCharges = 0;
            Movable = false;
            m_Boat = b;
            m_CooldownTime = DateTime.UtcNow + TimeSpan.FromSeconds(CooldownTime);
        }

        public override bool OnMoveOver(Mobile m)
        {
            return false;
        }

        private bool VolleyLoaded()
        {
            Boolean anyCannonsLoaded = false;

            if (m_Boat == null)
                return false;

            foreach (BaseCannon cannon in m_Boat.Cannons)
            {
                if (cannon.CurrentCharges > 0 && cannon.Facing == this.Facing)
                {
                    anyCannonsLoaded = true;
                    break;
                }
            }

            return anyCannonsLoaded;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (m_Boat == null)
                return;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to use this.");
                return;
            }

            else if (!(m_Boat.IsOwner(from) || m_Boat.IsCoOwner(from)))
            {
                from.SendMessage("You do not have permission to use this.");
                return;
            }

            else if (!m_Boat.Contains(from))
            {
                from.SendMessage("You cannot reach that.");
                return;
            }

            else if (VolleyLoaded() == false)
            {
                from.SendMessage("At least one of your cannons must be loaded to fire a volley.");
                return;
            }

            else if (DateTime.UtcNow < m_Boat.CannonCooldown)
            {
                from.SendMessage("You must wait before firing another cannon volley.");
                return;
            }

            else
            {
                from.SendMessage("Where would you like to fire the cannon volley?");
                from.Target = new CannonTarget(this);
                from.RevealingAction();
            }
        }

        private class CannonTarget : Target
        {
            private BaseCannon m_Cannon;

            public CannonTarget(BaseCannon b): base(25, true, TargetFlags.Harmful)
            {
                m_Cannon = b;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                {
                    if (p is Item)
                        p = ((Item)p).GetWorldTop();

                    else if (p is Mobile)
                        p = ((Mobile)p).Location;

                    m_Cannon.OnTarget(from, new Point3D(p.X, p.Y, p.Z), false, false, false, false);
                }
            }
        }

        public void OnTarget(Mobile from, Point3D point, bool IsNPCShip, bool canHitCenter, bool canHitHold, bool canHitTillerman)
        {
            //No Valid Parent Boat
            if (m_Boat == null)
                return;  
 
            bool highExplosiveAmmunitionActive = false;
            bool grapeshotAmmunitionActive = false;
            bool chainshotAmmunitionActive = false;
            bool powdersoakAmmunitionActive = false;
            bool smokeshotAmmunitionActive = false;
            bool hellfireAmmunitionActive = false; 

            Map map = from.Map;

            //Epic Ability: High-Explosive Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.HighExplosiveAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                highExplosiveAmmunitionActive = true;

            //Epic Ability: Grapeshot Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.GrapeshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                grapeshotAmmunitionActive = true;

            //Epic Ability: Chainshot Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.ChainshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                chainshotAmmunitionActive = true;

            //Epic Ability: Powersoak Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.PowdersoakAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                powdersoakAmmunitionActive = true;

            //Epic Ability: Smokeshot Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.SmokeshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                smokeshotAmmunitionActive = true;

            //Epic Ability: Hellfire Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.HellfireAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                hellfireAmmunitionActive = true;

            BaseBoat targetBoat = BaseBoat.FindBoatAt(point, map);            
            
            //For Player Ships
            if (m_Boat.MobileControlType == MobileControlType.Player)
            {
                if (!from.Player)
                    return;

                else if (!from.Alive)
                {
                    from.SendMessage("You must be alive to use this.");
                    return;
                }

                else if (!m_Boat.Contains(from))
                {
                    from.SendMessage("You are no longer on the boat.");
                    return;
                }

                else if (targetBoat == m_Boat)
                {
                    from.SendMessage("You may not fire onto your own ship!");
                    return;
                }

                else if (VolleyLoaded() == false)
                {
                    from.SendMessage("At least one of your cannons must be loaded to fire a volley.");
                    return;
                }

                else if (DateTime.UtcNow < m_Boat.CannonCooldown)
                {
                    from.SendMessage("You must wait before firing another cannon volley.");
                    return;
                }
            }
            
            bool volleyValid = false;
            bool tooClose = false;

            int cannonDelayTotal = 0;
            int cannonsFiring = 0;            

            //Need At Least One Cannon With LOS to Target and In Range of Target For Volley To Be Valid
            foreach (BaseCannon cannon in m_Boat.Cannons)
            {
                //Cannon Has Ammunition and is on Correct Ship Side for Volley
                if (cannon.CurrentCharges > 0 && cannon.Facing == Facing)
                {
                    cannonDelayTotal += cannon.CooldownTime;
                    cannonsFiring++;

                    double modifiedRange = (int)((double)Range * m_Boat.CannonRangeScalar);

                    //Already Deterined to Be Valid Shot: NPC AI Ship
                    if (IsNPCShip)
                    {
                        volleyValid = true;
                        break;
                    }

                    //Cannon is in LOS and Within Range
                    if (cannon.InAngle(point) && GetDistanceToSqrt(cannon, point) <= modifiedRange)
                        volleyValid = true;
                    
                    //Cannon is too close
                    if (GetDistanceToSqrt(cannon, point) < 2) 
                        tooClose = true;                    
                }
            }

            //At Least One Cannon Was Too Close to Fire
            if (tooClose)
                volleyValid = false;

            //Can Fire Cannon Volley
            if (volleyValid)
            {
                if (m_Boat.TillerMan != null)
                    m_Boat.TillerMan.Say("Firing cannons!");

                //Epic Ability: Deactivate After First Volley
                if (highExplosiveAmmunitionActive || grapeshotAmmunitionActive || chainshotAmmunitionActive || powdersoakAmmunitionActive || smokeshotAmmunitionActive || hellfireAmmunitionActive)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds(8), delegate
                    {
                        if (m_Boat != null)
                        {
                            m_Boat.m_EpicAbilityExpiration = DateTime.MinValue;
                            m_Boat.m_EpicAbility = EpicAbilityType.None;
                        }
                    });
                }

                m_Boat.LastCombatTime = DateTime.UtcNow;

                //Ship Cooldown Time (Average of Delay for Each Cannon Type that is Firing)
                double cooldown = cannonDelayTotal / cannonsFiring;

                m_Boat.CannonCooldown = DateTime.UtcNow + TimeSpan.FromSeconds(cooldown);
                m_Boat.StartCannonCooldown();

                int totalCannons = m_Boat.Cannons.Count;

                List<BaseCannon> cannonsToFire = new List<BaseCannon>();

                for (int a = 0; a < m_Boat.Cannons.Count; a++)
                {
                    if (m_Boat.Cannons[a].CurrentCharges > 0 && m_Boat.Cannons[a].Facing == Facing)
                        cannonsToFire.Add(m_Boat.Cannons[a]);
                }

                int firingLoops = 1;

                 //Epic Ability: High-Explosive Ammunition
                if (highExplosiveAmmunitionActive)
                    firingLoops = 3;

                //Epic Ability: Grapeshot Ammunition
                if (grapeshotAmmunitionActive)
                    firingLoops = 6;  

                //Epic Ability: Chainshot Ammunition
                if (chainshotAmmunitionActive)
                    firingLoops = 3;

                //Epic Ability: Powdersoak Ammunition
                if (powdersoakAmmunitionActive)
                    firingLoops = 3;

                //Epic Ability: Smokeshot Ammunition
                if (smokeshotAmmunitionActive)
                    firingLoops = 3;

                int cannonCount = cannonsToFire.Count;

                for (int a = 0; a < firingLoops; a++)
                {     
                    for (int b = 0; b < cannonCount; b++)
                    {
                        bool showSmoke = false;
                        bool lastCannon = false;

                        int cannonIndex = Utility.RandomMinMax(0, cannonsToFire.Count - 1);
                        BaseCannon cannon = cannonsToFire[cannonIndex];

                        if (a == 0)
                            showSmoke = true;

                        if (a == (firingLoops - 1))
                        {
                            cannon.CurrentCharges--;
                            cannonsToFire.RemoveAt(cannonIndex);

                            if (b == cannonCount - 1);
                                lastCannon = true;
                        }  
                  
                        //Epic Ability: Hellfire Ammunition
                        if (hellfireAmmunitionActive)
                        {
                            ShipEpicAbilities.HellfireAmmunition(from, m_Boat, cannon);
                            continue;
                        } 

                        //Check Accuracy
                        double cannonAccuracy = Accuracy * m_Boat.CannonAccuracyModifer;

                        double opponentMovementPenalty = 0;
                        double movementAccuracyPenalty = 0;

                        //Own Ship Movement Penalty
                        TimeSpan timeStationary = DateTime.UtcNow - m_Boat.TimeLastMoved;
                        double secondsStationary = (double)timeStationary.TotalSeconds;

                        if (secondsStationary > m_MovementAccuracyCooldown)
                            secondsStationary = m_MovementAccuracyCooldown;

                        if (targetBoat != null)
                        {
                            TimeSpan timeTargetStationary = DateTime.UtcNow - targetBoat.TimeLastMoved;
                            double secondsOpponentStationary = (double)timeStationary.TotalSeconds;

                            if (secondsOpponentStationary > m_MovementAccuracyCooldown)
                                secondsOpponentStationary = m_MovementAccuracyCooldown;

                            opponentMovementPenalty = 1 - (m_TargetMovementMaxAccuracyPenalty * (1 - (secondsOpponentStationary / m_MovementAccuracyCooldown)));

                            //No Movement Penalty to Shoot a Ship That's in Reduced Speed Mode
                            if (targetBoat.ReducedSpeedMode)
                                opponentMovementPenalty = 1;
                        }

                        movementAccuracyPenalty = 1 - (m_MovementMaxAccuracyPenalty * (1 - (secondsStationary / m_MovementAccuracyCooldown)));

                        double upgradesScalar = 1;

                        //Active Ability: Mastercraft Cannons
                        if (m_Boat.m_ActiveAbility == ActiveAbilityType.MastercraftCannons && m_Boat.m_ActiveAbilityExpiration > DateTime.UtcNow)
                            upgradesScalar = BaseBoat.MastercraftCannonsBonus;

                        double finalAccuracy = cannonAccuracy * movementAccuracyPenalty * opponentMovementPenalty * upgradesScalar;

                        double chance = Utility.RandomDouble();

                        bool hit = false;

                        //Hit Target
                        if (chance <= finalAccuracy)
                            hit = true;

                        Point3D cannonEndLocation = point;

                        if (IsNPCShip && targetBoat != null)
                        {
                            if (canHitCenter)
                                cannonEndLocation = targetBoat.GetRandomEmbarkLocation(true);

                            else if (canHitHold && canHitTillerman)
                            {
                                if (Utility.RandomDouble() < .5)
                                    cannonEndLocation = targetBoat.Hold.Location;
                                else
                                    cannonEndLocation = targetBoat.TillerMan.Location;
                            }

                            else if (canHitHold && !canHitTillerman)
                                cannonEndLocation = targetBoat.Hold.Location;

                            else if (!canHitHold && canHitTillerman)
                                cannonEndLocation = targetBoat.TillerMan.Location;
                        }

                        double delay = (.25 * (a + 1) / (double)firingLoops) * b;

                        //Epic Ability: Grapeshot Ammunition
                        if (grapeshotAmmunitionActive)
                            delay = (.05 * b);

                        Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                        {
                            FireCannon(cannon, from, cannonEndLocation, map, hit, showSmoke);
                        });                        
                    }
                }
            }

            else
            {
                if (tooClose)
                    from.SendMessage("Your target is too close to the ship to be fired upon.");

                else
                    from.SendMessage("At least one of your cannons must be within range of and in line of sight of your target in order to fire a cannon volley.");
            }
        }

        public void FireCannon(Custom.Pirates.BaseCannon cannon, Mobile from, Point3D targetLocation, Map map, bool hit, bool showSmoke)
        {
            int cannonballItemID = 0xE73;
            int cannonballHue = 0;
            int smokeHue = 0;

            bool fixedDirection = false;

            double shotDelay = .04;
            int shotSpeed = 6;

            bool highExplosiveAmmunitionActive = false;
            bool grapeshotAmmunitionActive = false;
            bool chainshotAmmunitionActive = false;
            bool powdersoakAmmunitionActive = false;
            bool smokeshotAmmunitionActive = false;
            bool hellfireAmmunitionActive = false;

            //Epic Ability: High-Explosive Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.HighExplosiveAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
            {
                cannonballItemID = 12693;
                cannonballHue = 0;

                highExplosiveAmmunitionActive = true;
            }
            
            //Epic Ability: Grapeshot Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.GrapeshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
            {
                cannonballItemID = 2514;
                cannonballHue = 2412;

                grapeshotAmmunitionActive = true;
            }

            //Epic Ability: Chainshot Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.ChainshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
            {
                cannonballItemID = Utility.RandomList(5178, 5179);
                cannonballHue = 0;

                shotDelay = .03;
                shotSpeed = 8;

                chainshotAmmunitionActive = true;
            }

            //Epic Ability: Powdersoak Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.PowdersoakAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
            {
                cannonballItemID = 12691;
                cannonballHue = 0;

                powdersoakAmmunitionActive = true;
            }

            //Epic Ability: Smokeshot Ammunition
            if (m_Boat.m_EpicAbility == EpicAbilityType.SmokeshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
            {
                cannonballItemID = 12694;
                cannonballHue = 0;

                smokeshotAmmunitionActive = true;
            }

            Point3D smokeLocation = cannon.Location;

            switch (cannon.m_facing)
            {
                case Direction.North: { } break;
                case Direction.East: { smokeLocation.X++; } break;
                case Direction.South: { smokeLocation.Y++; } break;
                case Direction.West: { smokeLocation.X--; } break;
            }

            if (m_Boat != null)
            {
                double gunsPercent = (double)((float)m_Boat.GunPoints / (float)m_Boat.MaxGunPoints);
                double misfireChance = m_MaxMisfireChance * (1 - gunsPercent);

                double chance = Utility.RandomDouble();

                double distance = GetDistanceToSqrt(cannon, targetLocation);
                double flatDistance = Utility.GetDistance(cannon.Location, targetLocation);

                //Misfire
                if (chance < misfireChance && m_Boat.m_EpicAbility == EpicAbilityType.None)
                {
                    List<Mobile> m_MobilesOnBoat = m_Boat.GetMobilesOnBoat(true, true);

                    foreach (Mobile mobile in m_MobilesOnBoat)
                    {
                        if (m_Boat.IsOwner(mobile) || m_Boat.IsCoOwner(mobile) || m_Boat.IsFriend(mobile))
                            mobile.SendMessage("Misfire!");
                    }

                    Effects.SendLocationEffect(cannon.Location, map, 0x3735, 10);
                    Effects.PlaySound(cannon.Location, map, 0x475);

                    return;
                }
                
                if (m_Boat.MobileFactionType == MobileFactionType.Undead)
                {
                    cannonballItemID = Utility.RandomList(6880, 6881, 6882, 6883, 6884);
                    smokeHue = 2630;
                }

                //Epic Abilities That Scatter
                if (highExplosiveAmmunitionActive || grapeshotAmmunitionActive || chainshotAmmunitionActive || powdersoakAmmunitionActive || smokeshotAmmunitionActive)
                    hit = true;

                //Hit
                if (hit)
                {
                    //Epic Abilities that Scatter
                    if (highExplosiveAmmunitionActive || grapeshotAmmunitionActive || chainshotAmmunitionActive || powdersoakAmmunitionActive || smokeshotAmmunitionActive)
                    {
                        int xOffset = 0;
                        int yOffset = 0;

                        double effectiveDistance = (double)flatDistance;
                        
                        //Epic Ability: High-Explosive Ammunition
                        if (highExplosiveAmmunitionActive)
                            effectiveDistance *= .33;

                        //Epic Ability: Grapeshot Ammunition
                        if (grapeshotAmmunitionActive)
                            effectiveDistance *= .5;

                        //Epic Ability: Chainshot Ammunition
                        if (chainshotAmmunitionActive)
                            effectiveDistance *= .33;

                        //Epic Ability: Powdersoak Ammunition
                        if (powdersoakAmmunitionActive)
                            effectiveDistance *= .33;

                        //Epic Ability: Smokeshot Ammunition
                        if (smokeshotAmmunitionActive)
                            effectiveDistance *= .33;

                        int distanceOffset = (int)(Math.Round(effectiveDistance));

                        if (distanceOffset < 1)
                            distanceOffset = 1;

                        xOffset = Utility.RandomMinMax(0, distanceOffset);

                        if (Utility.RandomDouble() > .5)
                            xOffset *= -1;

                        yOffset = Utility.RandomMinMax(0, distanceOffset);

                        if (Utility.RandomDouble() > .5)
                            yOffset *= -1;

                        double forcedMissChance = .3 - (.05 * flatDistance);

                        if (forcedMissChance < .05)
                            forcedMissChance = .05;

                        if (forcedMissChance > .2)
                            forcedMissChance = .2;

                        if (Utility.RandomDouble() <= forcedMissChance)
                            hit = false;

                        targetLocation.X += xOffset;
                        targetLocation.Y += yOffset;
                    }

                    m_Boat.LastCombatTime = DateTime.UtcNow;

                    Effects.PlaySound(cannon.Location, map, 0x664);

                    if (showSmoke)
                        Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

                    SpellHelper.AdjustField(ref targetLocation, map, 12, false);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(cannon.Location.X, cannon.Location.Y, cannon.Location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), map);
                    
                    Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);
                    double effectDelay = distance * shotDelay;

                    Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
                    {
                        ResolveCannon(cannon, from, targetLocation, map, hit);
                    });
                }

                //Miss
                else
                {
                    int xOffset = 0;
                    int yOffset = 0;

                    double effectiveDistance = distance;                    

                    int distanceOffset = (int)(Math.Floor(effectiveDistance / 2));

                    if (distance >= 2)
                    {
                        xOffset = Utility.RandomMinMax(0, distanceOffset);

                        if (Utility.RandomDouble() > .5)
                            xOffset *= -1;

                        yOffset = Utility.RandomMinMax(0, distanceOffset);

                        if (Utility.RandomDouble() > .5)
                            yOffset *= -1;
                    }

                    Effects.PlaySound(cannon.Location, map, 0x664);
                    Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(cannon.Location.X, cannon.Location.Y, cannon.Location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z + 5), map);

                    Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);

                    Point3D splashLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, + targetLocation.Z);

                    double newDistance = from.GetDistanceToSqrt(splashLocation);
                    double effectDelay = newDistance * shotDelay;

                    Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
                    {
                        ResolveCannon(cannon, from, splashLocation, map, hit);
                    });
                }
            }
        }

        public void ResolveCannon(Custom.Pirates.BaseCannon cannon, Mobile from, Point3D targetLocation, Map map, bool hit)
        {
            if (hit)            
                ResolveCannonHit(from, targetLocation);            

            else            
                Splash(targetLocation, map);            
        }

        public void ResolveCannonHit(Mobile from, Point3D targetLocation)
        {        
            ArrayList validTargets = new ArrayList();

            Map map = Map;            

            BaseBoat boatFrom = BaseBoat.FindBoatAt(from.Location, map);
            BaseBoat targetBoat = BaseBoat.FindBoatAt(targetLocation, map);

            bool hitObject = false;
            bool hitBoat = false;
            bool showExplosion = true;

            IPooledEnumerable eable = map.GetMobilesInRange(targetLocation, ExplosionRange);

            foreach (Mobile mobile in eable)
            {
                if (!validTargets.Contains(mobile))                
                    validTargets.Add(mobile);                
            }

            eable.Free();

            List<Mobile> m_MobilesOnSourceBoat = new List<Mobile>();
            List<Mobile> m_Targets = new List<Mobile>();

            double baseCannonDamage = (double)(Utility.RandomMinMax(DamageMin, DamageMax));

            bool highExplosiveAmmunitionActive = false;
            bool grapeshotAmmunitionActive = false;
            bool chainshotAmmunitionActive = false;
            bool powdersoakAmmunitionActive = false;
            bool smokeshotAmmunitionActive = false;

            if (m_Boat == null)
                m_MobilesOnSourceBoat.Add(from);            

            else
            {
                //Epic Ability: High-Explosive Ammunition
                if (m_Boat.m_EpicAbility == EpicAbilityType.HighExplosiveAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                {
                    highExplosiveAmmunitionActive = true;
                    showExplosion = true;
                }

                //Epic Ability: Grapeshot Ammunition
                if (m_Boat.m_EpicAbility == EpicAbilityType.GrapeshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                {
                    grapeshotAmmunitionActive = true;
                    showExplosion = false;
                }

                //Epic Ability: Chainshot Ammunition
                if (m_Boat.m_EpicAbility == EpicAbilityType.ChainshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                {
                    chainshotAmmunitionActive = true;
                    showExplosion = false;
                }

                //Epic Ability: Powdersoak Ammunition
                if (m_Boat.m_EpicAbility == EpicAbilityType.PowdersoakAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                {
                    powdersoakAmmunitionActive = true;
                    showExplosion = false;
                }

                //Epic Ability: Smokeshot Ammunition
                if (m_Boat.m_EpicAbility == EpicAbilityType.SmokeshotAmmunition && m_Boat.m_EpicAbilityExpiration > DateTime.UtcNow)
                {
                    smokeshotAmmunitionActive = true;
                    showExplosion = false;
                }

                baseCannonDamage = m_Boat.CannonDamageScalar * baseCannonDamage;
                m_MobilesOnSourceBoat = m_Boat.GetMobilesOnBoat(false, false);
            }

            bool targetLocationIsBoat = false;            
            
            if (targetBoat != null)
            {
                targetLocationIsBoat = true;
                m_Targets = targetBoat.GetMobilesOnBoat(false, false);
                validTargets.Add(targetBoat);
            }

            else
                m_Targets = new List<Mobile>();

            double damageDealt;

            for (int a = 0; a < validTargets.Count; ++a)
            {
                damageDealt = baseCannonDamage;

                object target = validTargets[a];

                int d = 0;
                int damage = 0;

                bool largeCreatureHit = false;

                PlayerMobile pm_Target;
                BaseCreature bc_Target;

                //Large Boss-Size Creature Hit: Don't Deal Damage to Boat Underneath it
                if (target is Mobile)
                {
                    bc_Target = target as BaseCreature;

                    if (bc_Target != null)
                    {
                        if (bc_Target.IsChamp() || bc_Target.IsBoss() || bc_Target.IsLoHBoss() || bc_Target.IsEventBoss())
                            largeCreatureHit = true;
                    }
                }

                if (target is Mobile)
                {
                    Mobile mobile = target as Mobile;

                    pm_Target = mobile as PlayerMobile;
                    bc_Target = mobile as BaseCreature;

                    if (!mobile.Alive)
                        continue;                    

                    //Mobile is somehow on boat that cannon is shooting from
                    BaseBoat mobileBoat = BaseBoat.FindBoatAt(mobile.Location, mobile.Map);

                    if (m_Boat != null && mobileBoat != null)
                    {
                        if (m_Boat == mobileBoat)
                            continue;
                    }                       

                    hitObject = true;

                    bool dealDamage = true;
                    bool directHit = false;

                    if (mobile.InRange(targetLocation, 0))
                        directHit = true;

                    bool isOnWater = BaseBoat.IsWaterTile(mobile.Location, mobile.Map);                    

                    if (from != null || (SpellHelper.ValidIndirectTarget(from, mobile) && from.CanBeHarmful(mobile, false)))
                    {
                        //Player
                        if (pm_Target != null)
                            damageDealt *= PlayerDamageMultiplier;

                        //Creature
                        if (bc_Target != null)
                        {
                            if (bc_Target.IsOceanCreature)
                                damageDealt *= OceanCreatureDamageMultiplier;

                            else
                                damageDealt *= MobileDamageMultiplier;  
                        }

                        if (!directHit)
                            damageDealt *= IndirectHitDamageMultiplier;

                        //Epic Ability: High-Explosive Ammunition
                        if (highExplosiveAmmunitionActive)
                        {
                            if (mobile is PlayerMobile)
                                damageDealt *= .5;

                            if (mobile is BaseCreature)
                                damageDealt *= .5;
                        } 

                        //Epic Ability: Grapeshot Ammunition
                        if (grapeshotAmmunitionActive)
                        {
                            if (mobile is PlayerMobile)
                                damageDealt *= .5;

                            if (mobile is BaseCreature)
                                damageDealt *= 4;
                        }                        

                        //Epic Ability: Chainshot Ammunition
                        if (chainshotAmmunitionActive)
                        {
                            if (mobile is PlayerMobile)
                                damageDealt *= .5;

                            if (mobile is BaseCreature)
                                damageDealt *= .5;
                        }

                        //Epic Ability: Powdersoak Ammunition
                        if (powdersoakAmmunitionActive)
                        {
                            if (mobile is PlayerMobile)
                                damageDealt *= .5;

                            if (mobile is BaseCreature)
                                damageDealt *= .5;
                        }

                        //Epic Ability: Smokeshot Ammunition
                        if (smokeshotAmmunitionActive)
                        {
                            dealDamage = false;

                            BaseCreature bc_Creature = mobile as BaseCreature;
                            PlayerMobile pm_Player = mobile as PlayerMobile;

                            int duration = 5;

                            if (bc_Creature != null)
                            {
                                if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                                {
                                    duration = Utility.RandomMinMax(5, 10);

                                    bc_Creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*looks disoriented*");
                                    SpecialAbilities.HinderSpecialAbility(1.0, null, bc_Creature, 1.0, duration, true, 0, false, "", "");
                                }

                                else
                                {
                                    duration = Utility.RandomMinMax(10, 20);

                                    if (bc_Creature.IsChamp() || bc_Creature.IsBoss() || bc_Creature.IsLoHBoss() || bc_Creature.IsEventBoss())
                                        duration = Utility.RandomMinMax(5, 10);

                                    bc_Creature.PublicOverheadMessage(MessageType.Regular, 0, false, "*looks disoriented*");
                                    SpecialAbilities.HinderSpecialAbility(1.0, null, bc_Creature, 1.0, duration, true, 0, false, "", "");
                                }
                            }

                            if (pm_Player != null)
                            {
                                duration = Utility.RandomMinMax(1, 2);

                                pm_Player.PublicOverheadMessage(MessageType.Regular, 0, false, "*looks disoriented*");
                                SpecialAbilities.HinderSpecialAbility(1.0, null, pm_Player, 1.0, duration, true, 0, false, "", "You have been hit with disorienting smoke and cannot act.");
                            }                            
                        }
                        
                        if (dealDamage)
                        {
                            from.DoHarmful(mobile);

                            int finalDamage = (int)Math.Round(damageDealt); 
                          
                            BaseCreature bc_Creature = mobile as BaseCreature;       

                            if (bc_Creature != null)
                            {
                                bool willKill = false;

                                if (bc_Creature.Hits - finalDamage <= 0)
                                    willKill = true;

                                bc_Creature.OnGotCannonHit(finalDamage, from, willKill);
                            }

                            AOS.Damage(mobile, from, finalDamage, 100, 0, 0, 0, 0);
                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        }
                    }
                }

                else if (target is DerelictCargo)
                {
                    DerelictCargo crate = target as DerelictCargo;
                    crate.TakeDamage(from, (int)damageDealt);              
                }

                else if (target is BaseBoat && !largeCreatureHit)
                {
                    BaseBoat boatTarget = target as BaseBoat;

                    if (from != null && m_Boat != null && boatTarget != null)
                    {
                        //Somehow Hitting Own Boat
                        if (m_Boat == boatTarget)
                            continue;                         

                        CannonDoHarmful(from, m_MobilesOnSourceBoat, m_Targets);

                        hitObject = true;
                        hitBoat = true;

                        bool dealDamage = true;

                        //Epic Ability: Smokeshot Ammunition
                        if (smokeshotAmmunitionActive)                        
                            dealDamage = false;                        

                        if (dealDamage)
                        {
                            DamageType damageType = boatTarget.GetDamageTypeByTargetingMode(m_Boat.TargetingMode);

                            //Epic Ability: High-Explosive Ammunition
                            if (highExplosiveAmmunitionActive)
                            {                                
                                damageType = DamageType.Hull;
                                damageDealt *= .33; 
                            }

                            //Epic Ability: Grapeshot Ammunition
                            if (grapeshotAmmunitionActive)
                                damageDealt *= .1;

                            //Epic Ability: Chainshot Ammunition
                            if (chainshotAmmunitionActive)
                            {
                                if (boatTarget.SailPoints > 0)
                                {
                                    damageType = DamageType.Sails;
                                    damageDealt *= .5;
                                }

                                else
                                    damageDealt *= .25;                                
                            }

                            //Epic Ability: Powdersoak Ammunition
                            if (powdersoakAmmunitionActive)
                            {
                                if (boatTarget.GunPoints > 0)
                                {   
                                    damageType = DamageType.Guns;
                                    damageDealt *= .5;
                                }

                                else
                                    damageDealt *= .25;
                            }

                            int finalDamage = (int)(Math.Round(damageDealt));

                            boatTarget.ReceiveDamage(from, m_Boat, finalDamage , damageType);
                        }
                    }                    
                }                
            }

            if (hitObject)
            {
                IEntity explosionLocationEntity = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z - 1), map);

                int explosionHue = 0;
                int explosionSound = 0x307;

                if (m_Boat.MobileFactionType == MobileFactionType.Undead)
                {
                    explosionHue = 2630;
                    explosionSound = 0x56E;
                }

                //Epic Ability: High-Explosion Ammunition
                else if (highExplosiveAmmunitionActive && hitBoat)
                {
                    explosionHue = 2634;
                    explosionSound = 0x306;

                    Point3D targetPoint = explosionLocationEntity.Location;

                    Effects.PlaySound(targetPoint, map, 0x56D);

                    int particleSpeed = 8;

                    if (Utility.RandomDouble() <= .5)
                    {
                        if (Utility.RandomDouble() <= .33)
                        {
                            int projectiles = Utility.RandomMinMax(2, 3);

                            for (int a = 0; a < projectiles; a++)
                            {
                                int debrisOffsetX = 0;
                                int debrisOffsetY = 0;

                                switch (Utility.RandomMinMax(1, 11))
                                {
                                    case 1: debrisOffsetX = -5; debrisOffsetY = 5; break;
                                    case 2: debrisOffsetX = -4; debrisOffsetY = 4; break;
                                    case 3: debrisOffsetX = -3; debrisOffsetY = 3; break;
                                    case 4: debrisOffsetX = -2; debrisOffsetY = 2; break;
                                    case 5: debrisOffsetX = -1; debrisOffsetY = 1; break;
                                    case 6: debrisOffsetX = 0; debrisOffsetY = 0; break;
                                    case 7: debrisOffsetX = 1; debrisOffsetY = -1; break;
                                    case 8: debrisOffsetX = 2; debrisOffsetY = -2; break;
                                    case 9: debrisOffsetX = 3; debrisOffsetY = -3; break;
                                    case 10: debrisOffsetX = 4; debrisOffsetY = -4; break;
                                    case 11: debrisOffsetX = 5; debrisOffsetY = -5; break;
                                }

                                Point3D newLocation = new Point3D(targetPoint.X + debrisOffsetX, targetPoint.Y + debrisOffsetY, targetPoint.Z);

                                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(targetPoint.X, targetPoint.Y, targetPoint.Z + 2), map);
                                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 30), map);

                                newLocation.Z += 5;

                                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3117, 3118, 3119, 3120, 3553, 7127, 7130, 7128, 7131), particleSpeed, 0, false, false, 0, 0);
                            }
                        }

                        else
                        {
                            int projectiles = Utility.RandomMinMax(1, 2);

                            int xOffset = Utility.RandomMinMax(-1, 1);
                            int yOffset = Utility.RandomMinMax(-1, 1);

                            Point3D newLocation = new Point3D(targetPoint.X + xOffset, targetPoint.Y + yOffset, targetPoint.Z);

                            SpellHelper.AdjustField(ref newLocation, map, 12, false);

                            TimedStatic debris = new TimedStatic(Utility.RandomList(3118, 3119, 3120, 3553, 7127, 7130), 10);
                            debris.Name = "debris";
                            debris.MoveToWorld(newLocation, map);
                        }
                    }
                }

                //Epic Ability: Grapeshot Ammunition
                else if (grapeshotAmmunitionActive)                
                    Effects.PlaySound(explosionLocationEntity.Location, map, Utility.RandomList(0x536, 0x537, 0x3BA, 0x3BB));                

                //Epic Ability: Chainshot Ammunition
                else if (chainshotAmmunitionActive && hitBoat)
                {
                    Effects.PlaySound(explosionLocationEntity.Location, map, 0x235);

                    int debrisCount = Utility.RandomMinMax(1, 2);

                    for (int a = 0; a < debrisCount; a++)
                    {
                        Blood debris = new Blood();
                        debris.Name = "debris";
                        debris.ItemID = Utility.RandomList(8766, 8767, 8768, 8769, 8770, 8771, 8772, 8773, 8774, 8775, 8776, 8777);
                        debris.Hue = 2498;

                        Point3D debrisLocation = new Point3D(explosionLocationEntity.Location.X + Utility.RandomMinMax(-1, 1), explosionLocationEntity.Location.Y + Utility.RandomMinMax(-1, 1), explosionLocationEntity.Location.Z);

                        SpellHelper.AdjustField(ref debrisLocation, map, 12, false);
                        debris.MoveToWorld(debrisLocation, map);
                    }
                }

                //Epic Ability: Powdersoak Ammunition
                else if (powdersoakAmmunitionActive && hitBoat)
                {
                    Effects.PlaySound(explosionLocationEntity.Location, map, Utility.RandomList(0x025, 0x026)); //0x382, 0x383, 0x384

                    int debrisCount = Utility.RandomMinMax(2, 3);

                    for (int a = 0; a < debrisCount; a++)
                    {

                        TimedStatic water = new TimedStatic(Utility.RandomList(4650, 4651, 4653, 4654, 4655), 30);
                        water.Name = "water";
                        water.Hue = 2120;

                        Point3D waterLocation = new Point3D(explosionLocationEntity.Location.X + Utility.RandomMinMax(-2, 2), explosionLocationEntity.Location.Y + Utility.RandomMinMax(-2, 2), explosionLocationEntity.Location.Z);

                        SpellHelper.AdjustField(ref waterLocation, map, 12, false);
                        water.MoveToWorld(waterLocation, map);
                    }
                }

                //Epic Ability: Smokeshot Ammunition
                else if (smokeshotAmmunitionActive)
                {
                    switch (Utility.RandomMinMax(1, 3))
                    {
                        case 1: Effects.SendLocationParticles(explosionLocationEntity, 0x37B9, 10, 6, 0, 0, 5044, 0); break;
                        case 2: Effects.SendLocationParticles(explosionLocationEntity, 0x372A, 10, 20, 0, 0, 5029, 0); break;
                        case 3: Effects.SendLocationParticles(explosionLocationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2499, 0, 5044, 0); break;
                    }

                    Effects.SendLocationParticles(explosionLocationEntity, 0x3709, 10, 30, 2499, 0, 5029, 0);
                    Effects.PlaySound(explosionLocationEntity.Location, map, Utility.RandomList(0x22A, 0x229));
                }

                if (showExplosion)
                {
                    Effects.SendLocationParticles(explosionLocationEntity, Utility.RandomList(14013, 14015, 14027, 14012), 30, 7, explosionHue, 0, 5044, 0);
                    Effects.PlaySound(explosionLocationEntity.Location, map, explosionSound);
                }                
            }

            else
                Splash(targetLocation, map);
        }
        
        public void CannonDoHarmful(Mobile from, List<Mobile> m_BoatAllies, List<Mobile> m_BoatTargets)
        {
            //Whoever shoots the cannon from the source boat attacks everyone on target boat
            foreach (Mobile mobileTarget in m_BoatTargets)
            {
                if (from.CanBeHarmful(mobileTarget, false))
                    from.DoHarmful(mobileTarget);
            }

            //All mobiles on boat attack everyone on target boat
            foreach (Mobile boatAlly in m_BoatAllies)
            {
                if (boatAlly == from)
                    continue;

                //NPC Boat: Everyone Attacks Everyone on Target Boat
                if (m_Boat.MobileControlType != MobileControlType.Player)
                {
                    //Player Currently on an NPC Boat
                    if (boatAlly is PlayerMobile)
                        continue;

                    foreach (Mobile mobileTarget in m_BoatTargets)
                    {
                        if (boatAlly.CanBeHarmful(mobileTarget, false))
                            boatAlly.DoHarmful(mobileTarget);
                    }
                }

                //Player Boat: Everyone on Boat Makes Criminal Checks Against Target Boat Inhabitants
                else
                {
                    bool isCriminal = false;

                    foreach (Mobile mobileTarget in m_BoatTargets)
                    {
                        if (Notoriety.Compute(boatAlly, mobileTarget) == Notoriety.Innocent && mobileTarget.Alive)
                        {
                            isCriminal = true;
                            break;
                        }
                    }

                    if (isCriminal)
                        boatAlly.CriminalAction(false);
                }
            }
        }
                
        public static void Splash(Point3D point, Map map)
        {  
            BaseBoat boatCheck = BaseBoat.FindBoatAt(point, map);

            bool foundAnyItem = false;

            IPooledEnumerable itemsInRange = map.GetItemsInRange(point, 1);
            
            foreach (Item item in itemsInRange)
            {
                if (item != null)
                {
                    foundAnyItem = true;
                    break;
                }
            }

            itemsInRange.Free();            

            //Boat in Location
            if (boatCheck != null)
                Effects.PlaySound(point, map, 0x148);

            //Water
            else if (BaseBoat.IsWaterTile(point, map))
            {
                if (!foundAnyItem)
                    Effects.SendLocationEffect(point, map, 0x352D, 7);

                Effects.PlaySound(point, map, 0x027);
            }

            //Anything Else
            else
                Effects.PlaySound(point, map, 0x148);
        }        

        public bool CanHitTargetShip(BaseBoat targetBoat, bool considerRange)
        {
            if (targetBoat == null)
                return false;

            if (targetBoat.Deleted)
                return false;

            double cannonScalar = 1;

            if (m_Boat != null)
                cannonScalar = m_Boat.CannonRangeScalar;

            int modifiedRange = (int)((double)Custom.Pirates.BaseCannon.Range * cannonScalar);

            if (considerRange && targetBoat.GetBoatToLocationDistance(targetBoat, Location) > modifiedRange)
                return false;

            if (InAngle(targetBoat.Location))
                return true;

            if (targetBoat.TillerMan != null)
            {
                if (InAngle(targetBoat.TillerMan.Location))
                    return true;
            }

            if (targetBoat.Hold != null)
            {
                if (InAngle(targetBoat.Hold.Location))
                    return true;
            }

            return false;
        }

        public bool InAngle(Point3D point)
        {
            Point3D loc = this.Location;
            int x = point.X - loc.X;
            int y = point.Y - loc.Y;

            if (x == 0) x = 1;
            if (y == 0) y = 1;

            switch (m_facing)
            {
                case Direction.North: { if (y < 0 && Math.Abs(Math.Atan(y / x)) > 0.52) return true; } break;
                case Direction.East: { if (x > 0 && Math.Abs(Math.Atan(x / y)) > 0.52) return true; } break;
                case Direction.South: { if (y > 0 && Math.Abs(Math.Atan(y / x)) > 0.52) return true; } break;
                case Direction.West: { if (x < 0 && Math.Abs(Math.Atan(x / y)) > 0.52) return true; } break;
            }

            return false;
        }

        public void SetFacing(Direction facing)
        {
            if (StarboardSide(facing & Direction.Mask))
            {
                switch (facing & Direction.Mask)
                {
                    case Direction.North: ItemID = EastID; m_facing = Direction.East; break;
                    case Direction.East: ItemID = SouthID; m_facing = Direction.South; break;
                    case Direction.South: ItemID = WestID; m_facing = Direction.West; break;
                    case Direction.West: ItemID = NorthID; m_facing = Direction.North; break;
                }
            }

            else
            {
                switch (facing & Direction.Mask)
                {
                    case Direction.North: ItemID = WestID; m_facing = Direction.West; break;
                    case Direction.East: ItemID = NorthID; m_facing = Direction.North; break;
                    case Direction.South: ItemID = EastID; m_facing = Direction.East; break;
                    case Direction.West: ItemID = SouthID; m_facing = Direction.South; break;
                }
            }
        }

        private bool StarboardSide(Direction facing)
        {
            if (m_Boat != null)
            {
                Point3D loc = m_Boat.Location;
                Point3D locThis = this.Location;

                switch (facing)
                {
                    case Direction.North: return (locThis.X > loc.X);
                    case Direction.South: return (locThis.X < loc.X);
                    case Direction.East: return (locThis.Y > loc.Y);
                    case Direction.West: return (locThis.Y < loc.Y);
                }
            }

            Delete();

            return true;
        }

        public bool PotionKegFill(int amount)
        {
            return true;
        }        

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((BaseBoat)m_Boat);
            writer.Write((int)m_CurrentCharges);
            writer.Write((int)m_facing);

            writer.Write((int)m_xOffset);
            writer.Write((int)m_yOffset);
            writer.Write((int)m_zOffset);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Boat = (BaseBoat)reader.ReadItem();
                        m_CurrentCharges = reader.ReadInt();
                        m_facing = (Direction)reader.ReadInt();

                        m_xOffset = (int)reader.ReadInt();
                        m_yOffset = (int)reader.ReadInt();
                        m_zOffset = (int)reader.ReadInt();
                    }
                    break;
            }

            if (m_Boat == null || m_Boat.Deleted)
                Delete();
        }

        public double GetDistanceToSqrt(Item from, Point3D to)
        {
            if (from == null || !(to is Point3D))
                return -1;

            int xDelta = from.Location.X - to.X;
            int yDelta = from.Location.Y - to.Y;

            return Math.Sqrt((xDelta * xDelta) + (yDelta * yDelta));
        }
    }
}