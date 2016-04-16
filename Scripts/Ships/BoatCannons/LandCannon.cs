using System;
using Server;
using Server.Multis;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Network;
using Server.Custom;

namespace Server
{
    public class LandCannon : Item
    {
        public enum CannonSize
        {
            Light,
            Heavy
        }

        public override bool AlwaysAllowDoubleClick { get { return true; } }
        public override bool Decays { get { return true; } }

        private CannonSize m_CannonType = CannonSize.Light;
        [CommandProperty(AccessLevel.GameMaster)]
        public CannonSize CannonType
        {
            get { return m_CannonType; }
            set {
                m_CannonType = value;
                SetFacing(m_Facing);
            }
        }        

        private AccessLevel m_UsageAccessLevel = AccessLevel.Seer;
        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel UsageAccessLevel
        {
            get { return m_UsageAccessLevel; }
            set { m_UsageAccessLevel = value; }
        }

        private AccessLevel m_MoveAccessLevel = AccessLevel.Seer;
        [CommandProperty(AccessLevel.GameMaster)]
        public AccessLevel MoveAccessLevel
        {
            get { return m_MoveAccessLevel; }
            set { m_MoveAccessLevel = value; }
        }

        private int m_Charges = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        private int m_MaxCharges = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxCharges
        {
            get { return m_MaxCharges; }
            set { m_MaxCharges = value; }
        }

        private int m_DamageMin = 20;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageMin
        {
            get { return m_DamageMin; }
            set { m_DamageMin = value; }
        }

        private int m_DamageMax = 30;
        [CommandProperty(AccessLevel.GameMaster)]
        public int DamageMax
        {
            get { return m_DamageMax; }
            set { m_DamageMax = value; }
        }

        private double m_CreatureDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double CreatureDamageScalar
        {
            get { return m_CreatureDamageScalar; }
            set { m_CreatureDamageScalar = value; }
        }

        private double m_TamedCreatureDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TamedCreatureDamageScalar
        {
            get { return m_TamedCreatureDamageScalar; }
            set { m_TamedCreatureDamageScalar = value; }
        }

        private double m_PlayerDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double PlayerDamageScalar
        {
            get { return m_PlayerDamageScalar; }
            set { m_PlayerDamageScalar = value; }
        }

        private double m_BoatDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double BoatDamageScalar
        {
            get { return m_BoatDamageScalar; }
            set { m_BoatDamageScalar = value; }
        }

        private double m_BreakableObjectDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double BreakableObjectDamageScalar
        {
            get { return m_BreakableObjectDamageScalar; }
            set { m_BreakableObjectDamageScalar = value; }
        }

        private double m_CooldownDelay = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public double CooldownDelay
        {
            get { return m_CooldownDelay; }
            set { m_CooldownDelay = value; }
        }

        private double m_ReloadDelay = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double ReloadDelay
        {
            get { return m_ReloadDelay; }
            set { m_ReloadDelay = value; }
        }

        private int m_Range = 12;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Range
        {
            get { return m_Range; }
            set { m_Range = value; }
        }

        private int m_ExplosionRadius = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplosionRadius
        {
            get { return m_ExplosionRadius; }
            set { m_ExplosionRadius = value; }
        }

        private Direction m_Facing = Direction.North;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Direction Facing
        {
            get { return m_Facing; }
            set { SetFacing(value); }
        }

        private DateTime m_NextUsageAllowed = DateTime.UtcNow;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextUsageAllowed
        {
            get { return m_NextUsageAllowed; }
            set { m_NextUsageAllowed = value; }
        }

        private int m_CannonballItemId = 3699;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CannonballItemId
        {
            get { return m_CannonballItemId; }
            set { m_CannonballItemId = value; }
        }

        private int m_CannonballHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int CannonballHue
        {
            get { return m_CannonballHue; }
            set { m_CannonballHue = value; }
        }

        private int m_SmokeHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int SmokeHue
        {
            get { return m_SmokeHue; }
            set { m_SmokeHue = value; }
        }

        private int m_ExplosionHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ExplosionHue
        {
            get { return m_ExplosionHue; }
            set { m_ExplosionHue = value; }
        }

        [Constructable]
        public LandCannon(): base(0x2c5)
        {
            SetFacing(Direction.East);
        }

        public LandCannon(Serial serial): base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            return false;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, Name);
            LabelTo(from, "[Ammunition: {0}/{1}]", Charges, MaxCharges);

            if (m_NextUsageAllowed <= DateTime.UtcNow)
            {
                if (Charges > 0)                    
                    LabelTo(from, "Ready to fire");                    

                else                    
                    LabelTo(from, "Needs ammunition");                    
            }

            else
            {
                if (Charges > 0)
                {
                    int secondsToFire = (int)(Math.Ceiling((m_NextUsageAllowed - DateTime.UtcNow).TotalSeconds));

                    if (secondsToFire == 0)                        
                        LabelTo(from, "Ready to fire");                        

                    else                        
                        LabelTo(from, "Fireable in " + secondsToFire.ToString() + " seconds");                        
                }

                else                    
                    LabelTo(from, "Needs ammunition");                    
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);
            
            if (from.AccessLevel < UsageAccessLevel)
            {
                from.SendMessage("You are not allowed to access this.");
                return;
            }

            if (!from.Alive && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You must be alive to use this.");
                return;
            }

            if (!from.InRange(Location, 1) && from.AccessLevel == AccessLevel.Player)
            {
                from.SendMessage("You are too far away to activate this.");
                return;
            }

            else if (Charges == 0)
            {
                from.SendMessage("The cannon is out of ammunition.");
                return;
            }

            else if (DateTime.UtcNow < NextUsageAllowed)
            {
                from.SendMessage("You must wait before firing again.");
                return;
            }

            else
            {
                from.SendMessage("Where would you like to fire the cannon?");
                from.Target = new LandCannonTarget(this);

                from.RevealingAction();
            }
        }

        private class LandCannonTarget : Target
        {
            private LandCannon m_LandCannon;

            public LandCannonTarget(LandCannon landCannon): base(25, true, TargetFlags.None)
            {
                m_LandCannon = landCannon;
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

                    m_LandCannon.OnTarget(from, new Point3D(p.X, p.Y, p.Z));
                }
            }
        }

        public void OnTarget(Mobile from, Point3D targetLocation)
        {
            Map map = from.Map;
                        
            bool volleyValid = false;
            bool tooClose = false;

            double modifiedRange = (double)Range;

            //Not in Firing Angle
            if (!InAngle(targetLocation))
            {
                from.SendMessage("That location is not within the cannon's firing angle.");
                return;
            }

            //Not in Line of Sight
            if (!map.InLOS(Location, targetLocation))
            {
                from.SendMessage("That location is not within the cannon's line-of-sight.");
                return;
            }

            //Too Far
            if (Utility.GetDistance(Location, targetLocation) > modifiedRange)
            {
                from.SendMessage("That location is out of the range of the cannon.");
                return;
            }

            //Too Close
            if (Utility.GetDistance(Location, targetLocation) <= 2)
            {
                from.SendMessage("That location is too close to the cannon to fire there.");
                return;
            }            

            m_NextUsageAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(CooldownDelay);

            Timer.DelayCall(TimeSpan.FromSeconds(CooldownDelay), delegate
            {
                if (this == null) return;
                if (Deleted) return;

                if (Charges > 0)
                    PublicOverheadMessage(MessageType.Regular, 0, false, "*ready to fire*");
            }); 

            int firingLoops = 1;

            for (int a = 0; a < firingLoops; a++)
            {
                bool hit = true;
                bool showSmoke = false;

                if (a == 0)
                    showSmoke = true;

                if (a == (firingLoops - 1))                        
                    Charges--;   

                double delay = (.25 * a);

                Timer.DelayCall(TimeSpan.FromSeconds(delay), delegate
                {
                    if (this == null) return;
                    if (Deleted) return;

                    FireCannon(from, targetLocation, map, hit, showSmoke);
                });

                if (Charges == 0)
                {
                    Timer.DelayCall(TimeSpan.FromSeconds((.25 * a) + .5), delegate
                    {
                        if (this == null) return;
                        if (Deleted) return;

                        PublicOverheadMessage(MessageType.Regular, 0, false, "*out of ammunition*");
                    });
                }
            }
        }

        public void FireCannon(Mobile from, Point3D targetLocation, Map map, bool hit, bool showSmoke)
        {
            int cannonballItemID = m_CannonballItemId;
            int cannonballHue = m_CannonballHue;
            int smokeHue = m_SmokeHue;

            bool fixedDirection = false;

            double shotDelay = .035;
            int shotSpeed = 10;
                        
            Point3D smokeLocation = Location;            

            smokeLocation.Z -= 3;

            switch (m_Facing)
            {
                case Direction.North: { } break;
                case Direction.East: { smokeLocation.X++; } break;
                case Direction.South: { smokeLocation.Y++; } break;
                case Direction.West: { smokeLocation.X--; } break;
            }

            Effects.PlaySound(Location, map, 0x664);

            if (showSmoke)
                Effects.SendLocationEffect(smokeLocation, map, 0x36CB, 10, smokeHue, 0);

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 4), map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 4), map);

            double distance = Utility.GetDistanceToSqrt(Location, targetLocation);

            Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, shotSpeed, 0, fixedDirection, false, cannonballHue, 0);
            double effectDelay = distance * shotDelay;

            Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
            {
                ResolveCannon(from, targetLocation, map, hit);
            });
        }

        public void ResolveCannon(Mobile from, Point3D targetLocation, Map map, bool hit)
        {
            if (hit)            
                ResolveCannonHit(from, targetLocation);            

            else            
                Splash(targetLocation, map);            
        }

        public void ResolveCannonHit(Mobile from, Point3D targetLocation)
        {
            Map map = Map;

            int explosionOverrideZ = -1;

            bool hitObject = false;
            bool hitMobile = false;
            bool hitBoat = false;
            bool hitBreakableObject = false;

            bool showExplosion = true;

            ArrayList validTargets = new ArrayList();

            IPooledEnumerable eable = map.GetObjectsInRange(targetLocation, ExplosionRadius);

            foreach (Object targetObject in eable)
            {
                if (!validTargets.Contains(targetObject))
                    validTargets.Add(targetObject);    
            }

            eable.Free();

            for (int a = 0; a < validTargets.Count; ++a)
            {
                double damageScalar = 1.0;

                if (validTargets[a] is Mobile)
                {
                    Mobile mobile = validTargets[a] as Mobile;                 
                    
                    BaseCreature bc_Target = mobile as BaseCreature;
                    PlayerMobile pm_Target = mobile as PlayerMobile;

                    if (!from.CanBeHarmful(mobile)) continue;
                    if (!map.InLOS(targetLocation, mobile.Location)) continue;

                    if (bc_Target != null)
                    {
                        if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                        {
                            if (TamedCreatureDamageScalar == 0)
                                continue;
                            else
                                damageScalar = TamedCreatureDamageScalar;
                        }

                        else
                        {
                            if (CreatureDamageScalar == 0)
                                continue;
                            else
                                damageScalar = CreatureDamageScalar;
                        }
                    }

                    if (pm_Target != null)
                    {
                        if (PlayerDamageScalar == 0)
                            continue;
                        else
                            damageScalar = PlayerDamageScalar;
                    }

                    hitMobile = true;

                    from.DoHarmful(mobile);

                    double damage = (double)Utility.RandomMinMax(DamageMin, DamageMax) * damageScalar;
                    int finalDamage = (int)(Math.Round(damage));

                    new Blood().MoveToWorld(mobile.Location, mobile.Map);

                    for (int b = 0; b < 3; b++)
                    {
                        Point3D newPoint = new Point3D(mobile.Location.X + Utility.RandomList(-1, 1), mobile.Location.Y + Utility.RandomList(-1, 1), mobile.Location.Z);
                        SpellHelper.AdjustField(ref newPoint, map, 12, false);
                        new Blood().MoveToWorld(newPoint, mobile.Map);
                    }

                    Explode(from, mobile.Location, map, hitBreakableObject, showExplosion, explosionOverrideZ);                    

                    AOS.Damage(mobile, from, finalDamage, 100, 0, 0, 0, 0);                    
                }

                else if (validTargets[a] is BaseBoat)
                {
                    BaseBoat boat = validTargets[a] as BaseBoat;

                    hitObject = true;
                    CannonDoHarmful(from, boat.GetMobilesOnBoat(false, false));

                    if (BoatDamageScalar == 0)
                        continue;
                    else
                        damageScalar = BoatDamageScalar;

                    Explode(from, targetLocation, map, hitBreakableObject, showExplosion, explosionOverrideZ);

                    double damage = (double)Utility.RandomMinMax(DamageMin, DamageMax) * damageScalar;
                    int finalDamage = (int)(Math.Round(damage));

                    boat.ReceiveDamage(from, null, finalDamage, DamageType.Hull);
                }

                else if (validTargets[a] is BreakableContainer)
                {
                    BreakableContainer breakableContainer = validTargets[a] as BreakableContainer;

                    if (!map.InLOS(targetLocation, breakableContainer.Location)) continue;

                    if (BreakableObjectDamageScalar == 0 || breakableContainer.ObjectBreakingDeviceDamageScalar == 0)
                        continue;
                    else
                        damageScalar = BreakableObjectDamageScalar;

                    hitObject = true;
                    hitBreakableObject = true;
                    explosionOverrideZ = breakableContainer.Z;
                    
                    Explode(from, breakableContainer.Location, map, hitBreakableObject, showExplosion, explosionOverrideZ);
                    Effects.PlaySound(breakableContainer.Location, breakableContainer.Map, breakableContainer.HitSound);

                    double damage = (double)Utility.RandomMinMax(DamageMin, DamageMax) * damageScalar;
                    int finalDamage = (int)(Math.Round(damage));

                    breakableContainer.ReceiveDamage(from, finalDamage, BreakableContainer.InteractionType.ObjectBreakingDevice);
                }

                else if (validTargets[a] is BreakableStatic)
                {
                    BreakableStatic breakableStatic = validTargets[a] as BreakableStatic;

                    if (!map.InLOS(targetLocation, breakableStatic.Location)) continue;

                    if (BreakableObjectDamageScalar == 0 || breakableStatic.ObjectBreakingDeviceDamageScalar == 0)
                        continue;
                    else
                        damageScalar = BreakableObjectDamageScalar * breakableStatic.ObjectBreakingDeviceDamageScalar;
                    
                    hitObject = true;
                    hitBreakableObject = true;
                    explosionOverrideZ = breakableStatic.Z;

                    Explode(from, breakableStatic.Location, map, hitBreakableObject, showExplosion, explosionOverrideZ);
                    Effects.PlaySound(breakableStatic.Location, breakableStatic.Map, breakableStatic.HitSound);

                    double damage = (double)Utility.RandomMinMax(DamageMin, DamageMax) * damageScalar;
                    int finalDamage = (int)(Math.Round(damage));

                    breakableStatic.ReceiveDamage(from, finalDamage, BreakableStatic.InteractionType.ObjectBreakingDevice);
                }
            }

            if (!(hitObject || hitMobile))
                Splash(targetLocation, map);
        }

        public void Explode(Mobile from, Point3D targetLocation, Map map, bool hitBreakableObject, bool showExplosion, int explosionOverrideZ)
        {
            int explosionHue = m_ExplosionHue;
            int explosionSound = 0x307;

            if (hitBreakableObject)
            {                
                targetLocation = SpecialAbilities.GetPointByDirection(targetLocation, Utility.GetDirection(targetLocation, Location));

                if (explosionOverrideZ != -1)
                    targetLocation.Z = explosionOverrideZ;
            }

            IEntity explosionLocationEntity = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);

            if (showExplosion)
            {
                Effects.SendLocationParticles(explosionLocationEntity, Utility.RandomList(14013, 14015, 14027, 14012), 30, 7, explosionHue, 0, 5044, 0);
                Effects.PlaySound(explosionLocationEntity.Location, map, explosionSound);
            }
        }
        
        public void CannonDoHarmful(Mobile from, List<Mobile> m_BoatTargets)
        {
            foreach (Mobile mobileTarget in m_BoatTargets)
            {
                if (from.CanBeHarmful(mobileTarget, false))
                    from.DoHarmful(mobileTarget);
            }
        }
                
        public void Splash(Point3D point, Map map)
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
            {
                Point3D newLocation = new Point3D(point.X, point.Y, point.Z);
                SpellHelper.AdjustField(ref newLocation, map, 12, false);
                
                TimedStatic timedStatic = new TimedStatic(0x3779, .5);
                timedStatic.Hue = 2406;
                timedStatic.Name = "";
                timedStatic.MoveToWorld(newLocation, map);

                int minRange = m_ExplosionRadius * -1;
                int maxRange = m_ExplosionRadius;

                for (int a = minRange; a < maxRange + 1; a++)
                {
                    for (int b = minRange; b < maxRange + 1; b++)
                    {
                        if (Utility.RandomDouble() <= .5 )
                        {
                            if (a == 0 && b == 0)
                                continue;

                            Point3D newPoint = new Point3D(point.X + a, point.Y + b, point.Z);
                            SpellHelper.AdjustField(ref newPoint, map, 12, false);

                            TimedStatic timedStaticB = new TimedStatic(0x3779, .5);
                            timedStaticB.Hue = 2406;
                            timedStaticB.Name = "";
                            timedStaticB.MoveToWorld(newLocation, map);
                        }
                    }
                }

                Effects.PlaySound(point, map, 0x140);
            }
        }        
        
        public bool InAngle(Point3D point)
        {           
            Point3D loc = this.Location;

            int x = point.X - loc.X;
            int y = point.Y - loc.Y;

            if (x == 0) x = 1;
            if (y == 0) y = 1;

            switch (m_Facing)
            {
                case Direction.North: { if (y < 0 && Math.Abs(Math.Atan(y / x)) > 0.30) return true; } break;
                case Direction.East: { if (x > 0 && Math.Abs(Math.Atan(x / y)) > 0.30) return true; } break;
                case Direction.South: { if (y > 0 && Math.Abs(Math.Atan(y / x)) > 0.30) return true; } break;
                case Direction.West: { if (x < 0 && Math.Abs(Math.Atan(x / y)) > 0.30) return true; } break;
            }            

            return false;
        }

        public void SetFacing(Direction facing)
        {
            switch (Direction)
            {
                case Direction.Up: facing = Direction.North; break;
                case Direction.Right: facing = Direction.East; break;
                case Direction.Down: facing = Direction.South; break;
                case Direction.Left: facing = Direction.West; break;
            }

            switch (m_CannonType)
            {
                case CannonSize.Light:
                    switch (facing & Direction.Mask)
                    {
                        case Direction.North: ItemID = 0x2c5; m_Facing = Direction.North; break;
                        case Direction.East: ItemID = 0x2c6; m_Facing = Direction.East; break;
                        case Direction.South: ItemID = 0x2dc; m_Facing = Direction.South; break;
                        case Direction.West: ItemID = 0x2dd; m_Facing = Direction.West; break;
                    }
                break;

                case CannonSize.Heavy:
                    switch (facing & Direction.Mask)
                    {
                        case Direction.North: ItemID = 0x2c1; m_Facing = Direction.North; break;
                        case Direction.East: ItemID = 0x2c2; m_Facing = Direction.East; break;
                        case Direction.South: ItemID = 0x2c3; m_Facing = Direction.South; break;
                        case Direction.West: ItemID = 0x2c4; m_Facing = Direction.West; break;
                    }
                break;
            }
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            base.OnDroppedToWorld(from, p);

            Direction = from.Direction;
            SetFacing(Direction);

            return true;
        }

        public override bool OnDragLift(Mobile from)
        {
            if (from.AccessLevel < MoveAccessLevel)
            {
                from.SendMessage("You are not allowed to access that.");
                return false;
            }

            return base.OnDragLift(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_CannonType);            
            writer.Write((int)m_UsageAccessLevel);
            writer.Write((int)m_MoveAccessLevel);
            writer.Write(m_Charges);
            writer.Write(m_MaxCharges);
            writer.Write(m_DamageMin);
            writer.Write(m_DamageMax);
            writer.Write(m_CreatureDamageScalar);
            writer.Write(m_TamedCreatureDamageScalar);
            writer.Write(m_PlayerDamageScalar);
            writer.Write(m_BoatDamageScalar);
            writer.Write(m_BreakableObjectDamageScalar);
            writer.Write(m_CooldownDelay);
            writer.Write(m_ReloadDelay);
            writer.Write(m_Range);
            writer.Write(m_ExplosionRadius);
            writer.Write((int)m_Facing);
            writer.Write(m_NextUsageAllowed);
            writer.Write(m_CannonballItemId);
            writer.Write(m_CannonballHue);
            writer.Write(m_SmokeHue);
            writer.Write(m_ExplosionHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_CannonType = (CannonSize)reader.ReadInt();               
                m_UsageAccessLevel = (AccessLevel)reader.ReadInt();
                m_MoveAccessLevel = (AccessLevel)reader.ReadInt();
                m_Charges = reader.ReadInt();
                m_MaxCharges = reader.ReadInt();
                m_DamageMin = reader.ReadInt();
                m_DamageMax = reader.ReadInt();
                m_CreatureDamageScalar = reader.ReadDouble();
                m_TamedCreatureDamageScalar = reader.ReadDouble();
                m_PlayerDamageScalar = reader.ReadDouble();
                m_BoatDamageScalar = reader.ReadDouble();
                m_BreakableObjectDamageScalar = reader.ReadDouble();
                m_CooldownDelay = reader.ReadDouble();
                m_ReloadDelay = reader.ReadDouble();
                m_Range = reader.ReadInt();
                m_ExplosionRadius = reader.ReadInt();
                m_Facing = (Direction)reader.ReadInt();
                m_NextUsageAllowed = reader.ReadDateTime();
                m_CannonballItemId = reader.ReadInt();
                m_CannonballHue = reader.ReadInt();
                m_SmokeHue = reader.ReadInt();
                m_ExplosionHue = reader.ReadInt();
            }
        }
    }
}