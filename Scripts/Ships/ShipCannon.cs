using System;
using Server;
using Server.Mobiles;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Spells;
using Server.Network;

namespace Server
{
    public class ShipCannon : Item
    {
        public enum CannonType
        {
            Small
        }

        public enum CannonPosition
        {
            Left,
            Right,
            Front,
            Rear
        }

        public BaseBoat m_Boat;

        public CannonType m_CannonType = CannonType.Small;
        public CannonPosition m_CannonPosition = CannonPosition.Left;

        public int m_xOffset;
        public int m_yOffset;
        public int m_zOffset;

        private int m_Ammunition;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Ammunition
        {
            get { return m_Ammunition; }
            set { m_Ammunition = value; }
        }

        private Direction m_Facing;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
        }       

        [Constructable]
        public ShipCannon(): base()
        {
            Movable = false;
        }

        public ShipCannon(Serial serial): base(serial)
        {
        }

        #region Place Ship Cannon

        public static void PlaceShipCannon(BaseBoat boat, Point3D point, CannonType cannonType, CannonPosition cannonPosition)
        {
            if (boat == null)
                return;

            ShipCannon shipCannon = new ShipCannon();

            shipCannon.Visible = false;

            shipCannon.m_Boat = boat;
            shipCannon.m_CannonType = cannonType;
            shipCannon.m_CannonPosition = cannonPosition;
            shipCannon.m_xOffset = point.X;
            shipCannon.m_yOffset = point.Y;
            shipCannon.m_zOffset = point.Z;

            Point3D cannonLocation = boat.GetRotatedLocation(point.X, point.Y, 0);

            shipCannon.MoveToWorld(new Point3D(boat.Location.X + cannonLocation.X, boat.Location.Y + cannonLocation.Y, boat.Location.Z + cannonLocation.Z), boat.Map);
            shipCannon.BoatFacingChange(boat.Facing);
            shipCannon.Z = boat.Location.Z + cannonLocation.Z + shipCannon.GetAdjustedCannonZOffset();

            shipCannon.Hue = boat.CannonHue;

            if (boat.MobileControlType != MobileControlType.Player)
                shipCannon.Ammunition = shipCannon.GetMaxAmmunition();

            shipCannon.Visible = true;

            boat.m_Cannons.Add(shipCannon);

            switch (cannonPosition)
            {
                case CannonPosition.Left: boat.m_LeftCannons.Add(shipCannon); break;
                case CannonPosition.Right: boat.m_RightCannons.Add(shipCannon); break;
                case CannonPosition.Front: boat.m_FrontCannons.Add(shipCannon); break;
                case CannonPosition.Rear: boat.m_RearCannons.Add(shipCannon); break;
            }
        }

        #endregion

        #region Boat Facing Change

        public void BoatFacingChange(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;
                    }
                break;

                case Direction.South:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;
                    }
                break;

                case Direction.East:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;
                    }
                break;

                case Direction.West:
                    switch (m_CannonPosition)
                    {
                        case CannonPosition.Left:
                            Facing = Direction.South;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dc; break;
                            }
                        break;

                        case CannonPosition.Right:
                            Facing = Direction.North;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c5; break;
                            }
                        break;

                        case CannonPosition.Front:
                            Facing = Direction.West;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2dd; break;
                            }
                        break;

                        case CannonPosition.Rear:
                            Facing = Direction.East;

                            switch (m_CannonType)
                            {
                                case CannonType.Small: ItemID = 0x2c6; break;
                            }
                        break;
                    }
                break;
            }
        }

        #endregion

        #region GetAdjustedCannonZOffset()

        public int GetAdjustedCannonZOffset()
        {
            if (m_Boat == null)
                return 0;

            int adjustZ = 0;

            switch (m_Boat.Facing)
            {
                case Direction.North:
                    if (Facing == Direction.West)
                        adjustZ = -1;

                    else
                        adjustZ = 1;
                break;

                case Direction.East:
                    if (Facing == Direction.North)
                        adjustZ = -2;

                    else
                        adjustZ = 2;
                break;

                case Direction.South:
                    if (Facing == Direction.West)
                        adjustZ = -1;

                    else
                        adjustZ = 1;
                break;

                case Direction.West:
                    if (Facing == Direction.North)
                        adjustZ = -2;

                    else
                        adjustZ = 2;
                break;
            }
          
            return adjustZ;
        }

        #endregion

        public bool InAngle(Point3D point)
        {
            Point3D loc = this.Location;
            int x = point.X - loc.X;
            int y = point.Y - loc.Y;

            if (x == 0) x = 1;
            if (y == 0) y = 1;

            switch (Facing)
            {
                case Direction.North: { if (y < 0 && Math.Abs(Math.Atan(y / x)) > 0.52) return true; } break;
                case Direction.East: { if (x > 0 && Math.Abs(Math.Atan(x / y)) > 0.52) return true; } break;
                case Direction.South: { if (y > 0 && Math.Abs(Math.Atan(y / x)) > 0.52) return true; } break;
                case Direction.West: { if (x < 0 && Math.Abs(Math.Atan(x / y)) > 0.52) return true; } break;
            }

            return false;
        }

        public int GetMaxAmmunition()
        {
            int maxAmmunition = BaseBoat.CannonMaxAmmunition;

            return maxAmmunition;
        }

        public override void OnSingleClick(Mobile from)
        {    
            base.OnSingleClick(from);

            if (m_Boat == null)
                return;

            LabelTo(from, "[Ammunition: {0}/{1}]", Ammunition, GetMaxAmmunition());

            if (m_Boat.CannonCooldown <= DateTime.UtcNow)
            {
                if (Ammunition > 0)                    
                    LabelTo(from, "Ready to fire");                    

                else                    
                    LabelTo(from, "Needs ammunition");                    
            }

            else
            {
                if (Ammunition > 0)
                {
                    int secondsToFire = (int)(Math.Ceiling((m_Boat.CannonCooldown - DateTime.UtcNow).TotalSeconds));

                    if (secondsToFire == 0)                        
                        LabelTo(from, "Ready to fire");                        

                    else                        
                        LabelTo(from, "Fireable in " + secondsToFire.ToString() + " seconds");                        
                }

                else                    
                    LabelTo(from, "Needs ammunition");                    
            }            

            base.OnSingleClick(from);
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

            else if (Ammunition == 0)
            {
                from.SendMessage("Those cannons are out of ammunition.");
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
            private ShipCannon m_ShipCannon;

            public CannonTarget(ShipCannon shipCannon): base(25, true, TargetFlags.Harmful)
            {
                m_ShipCannon = shipCannon;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_ShipCannon == null) return;
                if (m_ShipCannon.Deleted) return;

                IPoint3D location = targeted as IPoint3D;

                if (location != null)
                {
                    if (location is Item)
                        location = ((Item)location).GetWorldTop();

                    else if (location is Mobile)
                        location = ((Mobile)location).Location;

                    m_ShipCannon.OnTarget(from, new Point3D(location.X, location.Y, location.Z), false, false, false, false);
                }
            }
        }

        public void OnTarget(Mobile from, Point3D point, bool IsNPCShip, bool canHitCenter, bool canHitHold, bool canHitTillerman)
        {
            if (m_Boat == null)
                return;

            Map map = from.Map;
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

                else if (Ammunition == 0)
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

            double cannonDelayTotal = 0;
            int cannonsFiring = 0;

            //Need At Least One Cannon With LOS to Target and In Range of Target For Volley To Be Valid
            foreach (ShipCannon shipCannon in m_Boat.m_Cannons)
            {
                //Cannon Has Ammunition and is on Correct Ship Side for Volley
                if (shipCannon.Ammunition > 0 && shipCannon.Facing == Facing)
                {
                    cannonDelayTotal += BaseBoat.CannonCooldownTime;
                    cannonsFiring++;

                    double modifiedRange = (double)BaseBoat.CannonMaxRange * m_Boat.CannonRangeScalar;

                    //Already Deterined to Be Valid Shot: NPC AI Ship
                    if (IsNPCShip)
                    {
                        volleyValid = true;
                        break;
                    }

                    //Cannon is in LOS and Within Range
                    if (shipCannon.InAngle(point) && Utility.GetDistanceToSqrt(shipCannon.Location, point) <= modifiedRange)
                        volleyValid = true;

                    //Cannon is too close
                    if (Utility.GetDistanceToSqrt(shipCannon.Location, point) < 2)
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

                m_Boat.LastCombatTime = DateTime.UtcNow;

                //Ship Cooldown Time (Average of Delay for Each Cannon Type that is Firing)
                double cooldown = cannonDelayTotal / cannonsFiring;

                m_Boat.CannonCooldown = DateTime.UtcNow + TimeSpan.FromSeconds(cooldown);
                m_Boat.StartCannonCooldown();

                List<ShipCannon> cannonsToFire = new List<ShipCannon>();
                
                foreach (ShipCannon shipCannon in m_Boat.m_Cannons)
                {
                    if (shipCannon.Ammunition > 0 && shipCannon.Facing == Facing)                    
                        cannonsToFire.Add(shipCannon);
                }

                int firingLoops = BaseBoat.CannonFiringLoops;

                int cannonCount = cannonsToFire.Count;

                for (int a = 0; a < firingLoops; a++)
                {
                    for (int b = 0; b < cannonCount; b++)
                    {
                        bool showSmoke = false;
                        bool lastCannon = false;

                        int cannonIndex = Utility.RandomMinMax(0, cannonsToFire.Count - 1);
                        ShipCannon shipCannon = cannonsToFire[cannonIndex];

                        if (a == 0)
                            showSmoke = true;

                        if (a == (firingLoops - 1))
                        {
                            shipCannon.Ammunition--;
                            cannonsToFire.RemoveAt(cannonIndex);

                            if (b == cannonCount - 1)
                                lastCannon = true;
                        }

                        //Check Accuracy
                        double cannonAccuracy = BaseBoat.CannonAccuracy * m_Boat.CannonAccuracyModifer;

                        double opponentMovementPenalty = 0;
                        double movementAccuracyPenalty = 0;

                        //Own Ship Movement Penalty
                        TimeSpan timeStationary = DateTime.UtcNow - m_Boat.TimeLastMoved;
                        double secondsStationary = (double)timeStationary.TotalSeconds;

                        if (secondsStationary > BaseBoat.CannonMovementAccuracyCooldown)
                            secondsStationary = BaseBoat.CannonMovementAccuracyCooldown;

                        if (targetBoat != null)
                        {
                            TimeSpan timeTargetStationary = DateTime.UtcNow - targetBoat.TimeLastMoved;
                            double secondsOpponentStationary = (double)timeStationary.TotalSeconds;

                            if (secondsOpponentStationary > BaseBoat.CannonMovementAccuracyCooldown)
                                secondsOpponentStationary = BaseBoat.CannonMovementAccuracyCooldown;

                            opponentMovementPenalty = 1 - (BaseBoat.CannonTargetMovementMaxAccuracyPenalty * (1 - (secondsOpponentStationary / BaseBoat.CannonMovementAccuracyCooldown)));

                            //No Movement Penalty to Shoot a Ship That is in Reduced Speed Mode
                            if (targetBoat.ReducedSpeedMode)
                                opponentMovementPenalty = 1;
                        }

                        movementAccuracyPenalty = 1 - (BaseBoat.CannonMovementMaxAccuracyPenalty * (1 - (secondsStationary / BaseBoat.CannonMovementAccuracyCooldown)));

                        double finalAccuracy = cannonAccuracy * movementAccuracyPenalty * opponentMovementPenalty;

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

                        double delay = (BaseBoat.CannonLoopDelay * (a + 1) / (double)firingLoops) * b;
                        
                        Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                        {
                            FireCannon(shipCannon, from, cannonEndLocation, map, hit, showSmoke);
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

        public void FireCannon(ShipCannon shipCannon, Mobile from, Point3D targetLocation, Map map, bool hit, bool showSmoke)
        {
            if (shipCannon == null)
                return;

            int cannonballItemID = 0xE73;
            int cannonballHue = 0;
            int smokeHue = 0;

            bool fixedDirection = false;

            double shotDelay = .04;
            int shotSpeed = 6;

            Point3D smokeLocation = shipCannon.Location;

            switch (shipCannon.Facing)
            {
                case Direction.North: { } break;
                case Direction.East: { smokeLocation.X++; } break;
                case Direction.South: { smokeLocation.Y++; } break;
                case Direction.West: { smokeLocation.X--; } break;
            }

            if (m_Boat != null)
            {
                double gunsPercent = (double)((float)m_Boat.GunPoints / (float)m_Boat.MaxGunPoints);
                double misfireChance = BaseBoat.CannonMaxMisfireChance * (1 - gunsPercent);

                double chance = Utility.RandomDouble();

                double distance = Utility.GetDistanceToSqrt(shipCannon.Location, targetLocation);
                double flatDistance = Utility.GetDistance(shipCannon.Location, targetLocation);

                //Misfire
                if (chance < misfireChance)
                {
                    List<Mobile> m_MobilesOnBoat = m_Boat.GetMobilesOnBoat(true, true);

                    foreach (Mobile mobile in m_MobilesOnBoat)
                    {
                        if (m_Boat.IsOwner(mobile) || m_Boat.IsCoOwner(mobile) || m_Boat.IsFriend(mobile))
                            mobile.SendMessage("Misfire!");
                    }

                    Effects.SendLocationEffect(shipCannon.Location, map, 0x3735, 10);
                    Effects.PlaySound(shipCannon.Location, map, 0x475);

                    return;
                }

                if (m_Boat.MobileFactionType == MobileFactionType.Undead)
                {
                    cannonballItemID = Utility.RandomList(6880, 6881, 6882, 6883, 6884);
                    smokeHue = 2630;
                }

                //Hit
                if (hit)
                {
                    m_Boat.LastCombatTime = DateTime.UtcNow;

                    Effects.PlaySound(shipCannon.Location, map, 0x664);

                    if (showSmoke)
                        Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

                    SpellHelper.AdjustField(ref targetLocation, map, 12, false);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(shipCannon.Location.X, shipCannon.Location.Y, shipCannon.Location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), map);

                    Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);
                    double effectDelay = distance * shotDelay;

                    Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
                    {
                        ResolveCannon(shipCannon, from, targetLocation, map, hit);
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

                    Effects.PlaySound(shipCannon.Location, map, 0x664);
                    Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(shipCannon.Location.X, shipCannon.Location.Y, shipCannon.Location.Z + 10), map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, targetLocation.Z + 5), map);

                    Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);

                    Point3D splashLocation = new Point3D(targetLocation.X + xOffset, targetLocation.Y + yOffset, +targetLocation.Z);

                    double newDistance = from.GetDistanceToSqrt(splashLocation);
                    double effectDelay = newDistance * shotDelay;

                    Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
                    {
                        ResolveCannon(shipCannon, from, splashLocation, map, hit);
                    });
                }
            }
        }

        public void ResolveCannon(ShipCannon shipCannon, Mobile from, Point3D targetLocation, Map map, bool hit)
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

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(targetLocation, BaseBoat.CannonExplosionRange);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (!validTargets.Contains(mobile))
                    validTargets.Add(mobile);
            }

            nearbyMobiles.Free();

            List<Mobile> m_MobilesOnSourceBoat = new List<Mobile>();
            List<Mobile> m_Targets = new List<Mobile>();

            double baseCannonDamage = (double)(Utility.RandomMinMax(BaseBoat.CannonDamageMin, BaseBoat.CannonDamageMax));
            
            if (m_Boat == null)
                m_MobilesOnSourceBoat.Add(from);

            else
            {
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
                            damageDealt *= BaseBoat.CannonPlayerDamageMultiplier;

                        //Creature
                        if (bc_Target != null)
                        {
                            if (bc_Target.IsOceanCreature)
                                damageDealt *= BaseBoat.CannonOceanCreatureDamageMultiplier;

                            else
                                damageDealt *= BaseBoat.CannonMobileDamageMultiplier;
                        }

                        if (!directHit)
                            damageDealt *= BaseBoat.CannonIndirectHitDamageMultiplier;
                        
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

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, from, finalDamage, 100, 0, 0, 0, 0);                            
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

                        if (dealDamage)
                        {
                            DamageType damageType = boatTarget.GetDamageTypeByTargetingMode(m_Boat.TargetingMode);

                            int finalDamage = (int)(Math.Round(damageDealt));

                            boatTarget.ReceiveDamage(from, m_Boat, finalDamage, damageType);
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

        public override void OnDelete()
        {
            if (m_Boat != null)
            {
                if (m_Boat.m_Cannons.Contains(this))
                    m_Boat.m_Cannons.Remove(this);
            }

            base.OnDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            //Version 0
            writer.Write(m_Boat);
            writer.Write((int)m_CannonType);
            writer.Write((int)m_CannonPosition);
            writer.Write(m_Ammunition);
            writer.Write(m_xOffset);
            writer.Write(m_yOffset);
            writer.Write(m_zOffset);
            writer.Write((int)m_Facing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Boat = (BaseBoat)reader.ReadItem();
                m_CannonType = (CannonType)reader.ReadInt();
                m_CannonPosition = (CannonPosition)reader.ReadInt();
                m_Ammunition = reader.ReadInt();
                m_xOffset = reader.ReadInt();
                m_yOffset = reader.ReadInt();
                m_zOffset = reader.ReadInt();
                Facing = (Direction)reader.ReadInt();
            }

            //-----

            Movable = false;

            if (m_Boat != null)
            {
                m_Boat.m_Cannons.Add(this);

                switch (m_CannonPosition)
                {
                    case CannonPosition.Left: m_Boat.m_LeftCannons.Add(this); break;
                    case CannonPosition.Right: m_Boat.m_RightCannons.Add(this); break;
                    case CannonPosition.Front: m_Boat.m_FrontCannons.Add(this); break;
                    case CannonPosition.Rear: m_Boat.m_RearCannons.Add(this); break;
                }
            }
        }
    }    
}