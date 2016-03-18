using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using Server.Regions;
using Server.ContextMenus;
using Server.Spells;
using Server.Custom;

namespace Server.Mobiles
{
    public enum SpecialAbilityEffect
    {
        Hinder,
        Petrify,
        Cripple,
        Pierce,
        Bleed,
        Stun,
        Entangle,
        Silence,
        Frenzy,
        Enrage,
        Disease,
        Courage,
        Fortitude,
        Backlash,

        Expertise,
        Evasion,
        Inspiration,
        EmergencyRepairs,
        IronFists,
        Provider,
        Scientist,
        Technician,
        Searcher,
        Phalanx,
        Hardy,
        RapidTreatment,
        SuperiorHealing,

        Ignite,
        Bile,
        ShieldOfBones
    }

    public enum PotionAbilityEffectType
    {
        Explosion,
        Paralyze,
        Poison,
        Frost,
        Void,
        Shrapnel
    }

    public enum BreathType
    {
        Fire,
        Ice,
        Poison,
        Bone,
        Plant,
        Electricity
    }

    public class SpecialAbilityEffectEntry
    {
        public SpecialAbilityEffect m_SpecialAbilityEffect;
        public Mobile m_Owner;
        public double m_Value;
        public DateTime m_Expiration;

        public SpecialAbilityEffectEntry(SpecialAbilityEffect specialAbilityEffect, Mobile owner, double value, DateTime expiration)
        {
            m_SpecialAbilityEffect = specialAbilityEffect;
            m_Owner = owner;
            m_Value = value;
            m_Expiration = expiration;
        }
    }

    public class SpecialAbilities
    {
        #region Utility Functions

        private static bool Debugging = false;

        public static void LogMethodCall(string info)
        {
            if (Debugging)
            {
                Console.Write(string.Format("\n{0}", info));
            }
        }

        public static void AddBloodEffect(Mobile target, int min, int max)
        {
            LogMethodCall("AddBloodEffect");
            new Blood().MoveToWorld(target.Location, target.Map);
            int extraBlood = Utility.RandomMinMax(min, max);
            for (int i = 0; i < extraBlood; i++)
            {
                new Blood().MoveToWorld(new Point3D(target.X + Utility.RandomMinMax(-1, 1), target.Y + Utility.RandomMinMax(-1, 1), target.Z), target.Map);
            }
        }

        public static bool MonsterCanDamage(Mobile creature, Mobile target)
        {
            if (!Exists(target)) return false;
            if (!ValidTarget(target)) return false;
            if (target.Hidden && target.Squelched) return false;

            if (target is PlayerMobile) return true;
            if (IsPlayerControlledCreature(target)) return true;

            if (creature != null)
            {
                if (creature.Combatant == target)
                    return true;

                foreach (AggressorInfo aggressor in creature.Aggressors)
                {
                    if (aggressor.Attacker == target || aggressor.Defender == target)
                        return true;
                }

                foreach (AggressorInfo aggressed in creature.Aggressed)
                {
                    if (aggressed.Attacker == target || aggressed.Defender == target)
                        return true;
                }
            }

            return false;
        }


        public static void HealingOccured(Mobile healer, Mobile patient, int amount)
        {
            if (patient == null)
                return;

            PlayerMobile pm_Healer = healer as PlayerMobile;
            PlayerMobile pm_Patient = patient as PlayerMobile;
            BaseCreature bc_Patient = patient as BaseCreature;

            string message = "";

            if (pm_Healer != null)
            {
                if (pm_Healer.m_ShowHealing == DamageDisplayMode.PrivateMessage)
                {
                    if (healer == patient)
                        message = "You heal yourself for " + amount.ToString() + ".";
                    else
                        message = "You heal " + patient.Name + " for " + amount.ToString() + ".";

                    pm_Healer.SendMessage(pm_Healer.PlayerHealingTextHue, message);
                }

                if (pm_Healer.m_ShowHealing == DamageDisplayMode.PrivateOverhead)
                    patient.PrivateOverheadMessage(MessageType.Regular, pm_Healer.PlayerHealingTextHue, false, "+" + amount.ToString(), pm_Healer.NetState);
            }

            if (pm_Patient != null && healer != patient)
            {
                if (pm_Patient.m_ShowHealing == DamageDisplayMode.PrivateMessage)
                {
                    if (healer != null)
                        message = "You are healed by " + healer.Name + " for " + amount.ToString() + ".";
                    else
                        message = "You are healed for " + amount.ToString() + ".";

                    pm_Patient.SendMessage(pm_Patient.PlayerHealingTextHue, message);
                }

                if (pm_Patient.m_ShowHealing == DamageDisplayMode.PrivateOverhead)
                    pm_Patient.PrivateOverheadMessage(MessageType.Regular, pm_Patient.PlayerHealingTextHue, false, "+" + amount.ToString(), pm_Patient.NetState);
            }

            if (bc_Patient != null && (healer == patient || healer == null))
            {
                if (bc_Patient.Controlled && bc_Patient.ControlMaster is PlayerMobile)
                {
                    PlayerMobile pm_ControlMaster = bc_Patient.ControlMaster as PlayerMobile;

                    if (pm_ControlMaster.m_ShowHealing == DamageDisplayMode.PrivateOverhead)
                        bc_Patient.PrivateOverheadMessage(MessageType.Regular, pm_ControlMaster.PlayerHealingTextHue, false, "+" + amount.ToString(), pm_ControlMaster.NetState);
                }
            }    
        }

        public static bool Exists(Mobile mobile)
        {
            if (mobile == null) return false;
            if (mobile.Deleted) return false;
            if (!mobile.Alive) return false;

            return true;
        }

        public static bool ValidTarget(Mobile mobile)
        {
            if (!mobile.CanBeDamaged()) return false;
            if (mobile.AccessLevel > AccessLevel.Player) return false;

            return true;
        }

        public static bool IsPlayerControlledCreature(Mobile mobile)
        {
            if (!(mobile is BaseCreature)) return false;

            BaseCreature bc_Creature = mobile as BaseCreature;

            if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                return true;

            return false;
        }

        public static bool IsDamagable(Mobile target)
        {
            if (target == null) return false;
            if (target.Deleted || !target.Alive) return false;
            if (!target.CanBeDamaged()) return false;

            return true;
        }

        public static bool CanBeHit(BaseCreature creature, Mobile target, bool canHitBaseCreatures, bool canHitCombatant)
        {
            LogMethodCall("CanBeHit");
            if (target == null)
                return false;

            bool validTarget = false;

            PlayerMobile player = target as PlayerMobile;
            BaseCreature bc_Target = target as BaseCreature;

            if (player != null)
            {
                if (player.AccessLevel == AccessLevel.Player)
                    return true;
            }

            if (bc_Target != null)
            {
                if (canHitBaseCreatures)
                    return true;

                if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                    return true;
            }

            if (creature != null)
            {
                if (creature.Combatant != null)
                {
                    if (creature.Combatant == target && canHitCombatant)
                        return true;
                }
            }

            return validTarget;
        }

        public static List<Point3D> GetSpawnableTiles(Point3D sourceLocation, bool needLOS, bool allowSameTile, Point3D startLocation, Map map, int locationsToGet, int maxLocationChecks, int minRadius, int maxRadius, bool checkMulti)
        {
            LogMethodCall("GetSpawnableTiles");
            List<Point3D> m_ValidLocation = new List<Point3D>();

            //Determine Valid Teleport Locations
            for (int a = 0; a < locationsToGet; a++)
            {
                bool foundValidSpot = false;
                Point3D newLocation = new Point3D();

                for (int b = 0; b < maxLocationChecks; b++)
                {
                    int x = startLocation.X;

                    int xOffset = Utility.RandomMinMax(minRadius, maxRadius);
                    if (Utility.RandomDouble() >= .5)
                        xOffset *= -1;

                    x += xOffset;

                    int y = startLocation.Y;

                    int yOffset = Utility.RandomMinMax(minRadius, maxRadius);
                    if (Utility.RandomDouble() >= .5)
                        yOffset *= -1;

                    y += yOffset;

                    newLocation.X = x;
                    newLocation.Y = y;
                    newLocation.Z = startLocation.Z;

                    bool canFit = SpellHelper.AdjustField(ref newLocation, map, 12, false);

                    if (map == null || !canFit)
                        continue;

                    if (checkMulti && SpellHelper.CheckMulti(newLocation, map))
                        continue;

                    if (!allowSameTile && startLocation.X == newLocation.X && startLocation.Y == newLocation.Y)
                        continue;

                    if (needLOS && !map.InLOS(startLocation, newLocation))
                    {
                        continue;
                    }

                    foundValidSpot = true;
                    break;
                }

                if (foundValidSpot)
                {
                    if (!m_ValidLocation.Contains(newLocation))
                        m_ValidLocation.Add(newLocation);
                }
            }

            return m_ValidLocation;
        }

        public static List<Point3D> GetPerpendicularPoints(Point3D startPoint, Point3D endPoint, int width)
        {
            LogMethodCall("GetPerpendicularPoints");
            List<Point3D> m_Points = new List<Point3D>();

            Point3D newPoint = startPoint;

            int dx = startPoint.X - endPoint.X;
            int dy = startPoint.Y - endPoint.Y;

            int rx = (dx - dy) * 44;
            int ry = (dx + dy) * 44;

            int ax = Math.Abs(rx);
            int ay = Math.Abs(ry);

            Direction direction;

            if (((ay >> 1) - ax) >= 0)
                direction = (ry > 0) ? Direction.Up : Direction.Down;
            else if (((ax >> 1) - ay) >= 0)
                direction = (rx > 0) ? Direction.Left : Direction.Right;
            else if (rx >= 0 && ry >= 0)
                direction = Direction.West;
            else if (rx >= 0 && ry < 0)
                direction = Direction.South;
            else if (rx < 0 && ry < 0)
                direction = Direction.East;
            else
                direction = Direction.North;

            switch (direction & Direction.Mask)
            {
                case Direction.North:
                    newPoint.Y--;
                    break;

                case Direction.Right:
                    newPoint.X++;
                    newPoint.Y--;
                    break;

                case Direction.East:
                    newPoint.X++;
                    break;

                case Direction.Down:
                    newPoint.X++;
                    newPoint.Y++;
                    break;

                case Direction.South:
                    newPoint.Y++;
                    break;

                case Direction.Left:
                    newPoint.X--;
                    newPoint.Y++;
                    break;

                case Direction.West:
                    newPoint.X--;
                    break;

                case Direction.Up:
                    newPoint.X--;
                    newPoint.Y--;
                    break;
            }

            for (int a = 1; a < width + 1; a++)
            {
                switch (direction)
                {
                    case Direction.North:
                        m_Points.Add(new Point3D(newPoint.X - a, newPoint.Y, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X + a, newPoint.Y, newPoint.Z));
                        break;

                    case Direction.Right:
                        m_Points.Add(new Point3D(newPoint.X - a, newPoint.Y, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y + a, newPoint.Z));
                        break;

                    case Direction.East:
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y - a, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y + a, newPoint.Z));
                        break;

                    case Direction.Down:
                        m_Points.Add(new Point3D(newPoint.X - a, newPoint.Y, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y - a, newPoint.Z));
                        break;

                    case Direction.South:
                        m_Points.Add(new Point3D(newPoint.X - a, newPoint.Y, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X + a, newPoint.Y, newPoint.Z));
                        break;

                    case Direction.Left:
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y - a, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X + a, newPoint.Y, newPoint.Z));
                        break;

                    case Direction.West:
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y - a, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y + a, newPoint.Z));
                        break;

                    case Direction.Up:
                        m_Points.Add(new Point3D(newPoint.X + a, newPoint.Y, newPoint.Z));
                        m_Points.Add(new Point3D(newPoint.X, newPoint.Y + a, newPoint.Z));
                        break;
                }
            }

            return m_Points;
        }

        public static Point3D GetPointByDirection(Point3D currentPoint, Direction direction)
        {
            LogMethodCall("GetPointByDirection");

            Point3D newPoint = new Point3D(currentPoint.X, currentPoint.Y, currentPoint.Z);

            switch (direction & Direction.Mask)
            {
                case Direction.North:
                    newPoint.Y--;
                    break;

                case Direction.Right:
                    newPoint.X++;
                    newPoint.Y--;
                    break;

                case Direction.East:
                    newPoint.X++;
                    break;

                case Direction.Down:
                    newPoint.X++;
                    newPoint.Y++;
                    break;

                case Direction.South:
                    newPoint.Y++;
                    break;

                case Direction.Left:
                    newPoint.X--;
                    newPoint.Y++;
                    break;

                case Direction.West:
                    newPoint.X--;
                    break;

                case Direction.Up:
                    newPoint.X--;
                    newPoint.Y--;
                    break;
            }

            return newPoint;
        }

        #endregion

        public static bool Global_AllowAbilities = true;

        public static void ClearSpecialEffects(Mobile mobile)
        {
            PlayerMobile player = mobile as PlayerMobile;
            BaseCreature bc_Creature = mobile as BaseCreature;

            if (player != null)
            {
                foreach (SpecialAbilityEffectEntry entry in player.m_SpecialAbilityEffectEntries)
                {
                    entry.m_Expiration = DateTime.UtcNow;
                }
            }

            if (bc_Creature != null)
            {
                foreach (SpecialAbilityEffectEntry entry in bc_Creature.m_SpecialAbilityEffectEntries)
                {
                    entry.m_Expiration = DateTime.UtcNow;
                }
            }
        }

        public static void SpawnCreature(Type type, Point3D endLocation, Map map, bool onFailSpawnOnLocation, int minRadius, int maxRadius, out Mobile newCreature, out Point3D finalLocation)
        {
            LogMethodCall("SpawnCreature");

            newCreature = null;
            finalLocation = new Point3D();

            if (!Global_AllowAbilities)
                return;

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(endLocation, false, true, endLocation, map, 1, 25, minRadius, maxRadius, true);

            Point3D newLocation = new Point3D();

            if (m_ValidLocations.Count == 0)
            {
                if (onFailSpawnOnLocation)
                    newLocation = endLocation;
                else
                    return;
            }

            else
                newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

            Effects.PlaySound(newLocation, map, 0x4F1);

            BaseCreature creature = (BaseCreature)Activator.CreateInstance(type);

            creature.MoveToWorld(newLocation, map);

            newCreature = creature;
            finalLocation = newLocation;
        }

        public static bool DoMassiveBreathAttack(BaseCreature creature, Point3D startLocation, Direction breathDirection, int breathLength, bool animate, BreathType breathType, bool tamedCreature)
        {
            if (creature == null) return false;
            if (creature.Deleted) return false;

            creature.Direction = breathDirection;

            Timer.DelayCall(TimeSpan.FromSeconds(.1), delegate
            {
                if (creature == null) return;
                if (creature.Deleted) return;

                if (animate)
                {
                    if (creature.IsHighSeasBodyType)
                        creature.Animate(5, 10, 1, true, false, 0);

                    else if (creature.BodyValue == 308)
                        creature.Animate(5, 4, 1, true, false, 0);

                    else
                        creature.Animate(12, 5, 2, true, false, 0);
                }

                SpecialAbilities.HinderSpecialAbility(1.0, null, creature, 1.0, 2.5, true, 0, false, "", "");

                Effects.PlaySound(creature.Location, creature.Map, creature.GetAngerSound());

                Timer.DelayCall(TimeSpan.FromSeconds(1.1), delegate
                {
                    if (creature == null) return;
                    if (creature.Deleted) return;

                    MassiveBreathAttackStart(creature, startLocation, breathDirection, breathLength, breathType, tamedCreature);
                });
            });

            return true;
        }

        public static bool MassiveBreathAttackStart(BaseCreature creature, Point3D startLocation, Direction breathDirection, int breathLength, BreathType breathType, bool tamedCreature)
        {
            if (creature == null) return false;
            if (creature.Deleted) return false;

            int damageMin = creature.DamageMin;
            int damageMax = creature.DamageMax;

            Point3D location = startLocation;
            Map map = creature.Map;

            Effects.PlaySound(location, map, 0x227);

            Dictionary<Point3D, double> m_BreathTiles = new Dictionary<Point3D, double>();
            Dictionary<Mobile, double> m_DamagedMobiles = new Dictionary<Mobile, double>();

            int breathSound = 0;
            int effectSound = 0;

            switch (breathType)
            {
                case BreathType.Fire: breathSound = 0x227; effectSound = 0x208; break;
                case BreathType.Ice: breathSound = 0x64F; effectSound = 0x208; break;
                case BreathType.Poison: breathSound = 0x22F; effectSound = 0x208; break;
                case BreathType.Bone: breathSound = 0x222; effectSound = 0x208; break;
            }

            double tileDelay = .10;
            int distance = breathLength;

            Point3D previousPoint = location;
            Point3D nextPoint;

            m_BreathTiles.Add(startLocation, 0);

            for (int a = 0; a < distance; a++)
            {
                nextPoint = GetPointByDirection(previousPoint, breathDirection);

                bool canFit = SpellHelper.AdjustField(ref nextPoint, map, 12, false);

                if (canFit && map.InLOS(creature.Location, nextPoint))
                {
                    if (!m_BreathTiles.ContainsKey(nextPoint))
                        m_BreathTiles.Add(nextPoint, a * tileDelay);
                }

                List<Point3D> perpendicularPoints = GetPerpendicularPoints(previousPoint, nextPoint, a + 1);

                foreach (Point3D point in perpendicularPoints)
                {
                    Point3D ppoint = new Point3D(point.X, point.Y, point.Z);

                    canFit = SpellHelper.AdjustField(ref ppoint, map, 12, false);

                    if (canFit && creature.Map.InLOS(creature.Location, ppoint))
                    {
                        if (!m_BreathTiles.ContainsKey(ppoint))
                            m_BreathTiles.Add(ppoint, a * tileDelay);
                    }
                }

                previousPoint = nextPoint;
            }

            foreach (KeyValuePair<Point3D, double> pair in m_BreathTiles)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(pair.Value), delegate
                {
                    Point3D breathLocation = pair.Key;

                    int projectiles;
                    int particleSpeed;
                    double distanceDelayInterval;

                    int minRadius;
                    int maxRadius;

                    List<Point3D> m_ValidLocations = new List<Point3D>();

                    if (breathLocation != startLocation)
                    {
                        switch (breathType)
                        {
                            case BreathType.Fire:
                                Effects.PlaySound(breathLocation, map, breathSound);
                                Effects.SendLocationParticles(EffectItem.Create(breathLocation, map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 0, 0, 5052, 0);

                                if (!tamedCreature)
                                {
                                    Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                    {
                                        if (Utility.RandomDouble() <= .15)
                                        {
                                            SingleFireField singleFireField = new SingleFireField(creature, 0, 2, 30, 3, 5, false, false, true, -1, true);
                                            singleFireField.MoveToWorld(breathLocation, map);
                                        }

                                    });
                                }
                                break;

                            case BreathType.Ice:
                                Effects.PlaySound(breathLocation, map, breathSound);
                                Effects.SendLocationParticles(EffectItem.Create(breathLocation, map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 1150, 0, 5029, 0);

                                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                {
                                    Effects.PlaySound(breathLocation, map, effectSound);
                                    Effects.SendLocationParticles(EffectItem.Create(breathLocation, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 50, 1153, 0, 5029, 0);
                                });
                                break;

                            case BreathType.Poison:
                                Effects.PlaySound(breathLocation, map, breathSound);
                                Effects.SendLocationParticles(EffectItem.Create(breathLocation, map, TimeSpan.FromSeconds(1.0)), 0x3709, 10, 30, 2208, 0, 2212, 0);

                                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                                {
                                    Effects.PlaySound(breathLocation, map, effectSound);
                                    Effects.SendLocationParticles(EffectItem.Create(breathLocation, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 50, 1153, 0, 5029, 0);
                                });
                                break;

                            case BreathType.Bone:
                                Effects.PlaySound(breathLocation, map, 0x11D);

                                for (int a = 0; a < 5; a++)
                                {
                                    TimedStatic bones = new TimedStatic(Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), 5);
                                    bones.Name = "bones";

                                    Point3D dirtLocation = new Point3D(breathLocation.X + Utility.RandomList(-1, 1), breathLocation.Y + Utility.RandomList(-1, 1), breathLocation.Z);

                                    bones.MoveToWorld(dirtLocation, map);
                                }

                                projectiles = 10;
                                particleSpeed = 8;
                                distanceDelayInterval = .12;

                                minRadius = 1;
                                maxRadius = 5;

                                m_ValidLocations = SpecialAbilities.GetSpawnableTiles(breathLocation, true, false, breathLocation, map, projectiles, 20, minRadius, maxRadius, false);

                                if (m_ValidLocations.Count == 0)
                                    return;

                                for (int a = 0; a < projectiles; a++)
                                {
                                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(breathLocation.X, breathLocation.Y, breathLocation.Z + 2), map);
                                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                                    newLocation.Z += 5;

                                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), particleSpeed, 0, false, false, 0, 0);
                                }

                                if (Utility.RandomDouble() <= .25)
                                {
                                    IEntity locationEntity = new Entity(Serial.Zero, new Point3D(breathLocation.X, breathLocation.Y, breathLocation.Z - 1), map);
                                    Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x36BD, 0x36BF, 0x36CB, 0x36BC), 30, 7, 2497, 0, 5044, 0);
                                }
                                break;

                            case BreathType.Plant:
                                Effects.PlaySound(breathLocation, map, 0x65A); //0x228

                                TimedStatic vegetation = new TimedStatic(Utility.RandomList(3219, 3220, 3255, 3256, 3152, 3153, 3223, 6809, 6811, 3204, 3247, 3248, 3254, 3258, 3259, 3378,
                                3267, 3237, 3267, 9036, 3239, 3208, 3307, 3310, 3311, 3313, 3314, 3332, 3271, 3212, 3213), 5);
                                vegetation.Name = "vegetation";

                                Point3D vegetationLocation = new Point3D(breathLocation.X + Utility.RandomList(-1, 1), breathLocation.Y + Utility.RandomList(-1, 1), breathLocation.Z);

                                vegetation.MoveToWorld(vegetationLocation, map);

                                projectiles = 1;
                                particleSpeed = 8;
                                distanceDelayInterval = .12;

                                minRadius = 1;
                                maxRadius = 5;

                                m_ValidLocations = SpecialAbilities.GetSpawnableTiles(breathLocation, true, false, breathLocation, map, projectiles, 20, minRadius, maxRadius, false);

                                if (m_ValidLocations.Count == 0)
                                    return;

                                for (int a = 0; a < projectiles; a++)
                                {
                                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(breathLocation.X, breathLocation.Y, breathLocation.Z + 2), map);
                                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 50), map);

                                    newLocation.Z += 5;

                                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(3378, 3379, 3219, 3248), particleSpeed, 0, false, false, 0, 0);
                                }

                                if (Utility.RandomDouble() <= .25)
                                {
                                    IEntity locationEntity = new Entity(Serial.Zero, new Point3D(breathLocation.X, breathLocation.Y, breathLocation.Z - 1), map);
                                    Effects.SendLocationParticles(locationEntity, Utility.RandomList(0x3779), 30, 7, Utility.RandomList(2208, 2209, 2210, 2211), 0, 5044, 0);
                                }
                            break;

                            case BreathType.Electricity:
                                Effects.PlaySound(breathLocation, map, 0x5C3);

                                TimedStatic electricField = new TimedStatic(14695, 1);
                                electricField.Name = "electric field";
                                electricField.MoveToWorld(breathLocation, map); 
                            break;
                        }
                    }

                    IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(breathLocation, 0);

                    Queue m_Queue = new Queue();

                    foreach (Mobile mobile in nearbyMobiles)
                    {
                        if (mobile == creature)
                            continue;

                        if (tamedCreature)
                        {
                            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                continue;

                            bool validTarget = false;

                            if (creature != null)
                            {
                                if (creature.Combatant == mobile)
                                    validTarget = true;

                                if (creature.ControlMaster != null)
                                {
                                    if (mobile == creature.ControlMaster)
                                        continue;

                                    foreach (AggressorInfo aggressor in creature.ControlMaster.Aggressors)
                                    {
                                        if (aggressor.Attacker == mobile || aggressor.Defender == mobile)
                                        {
                                            validTarget = true;
                                            break;
                                        }
                                    }

                                    foreach (AggressorInfo aggressed in creature.ControlMaster.Aggressed)
                                    {
                                        if (aggressed.Attacker == mobile || aggressed.Defender == mobile)
                                        {
                                            validTarget = true;
                                            break;
                                        }
                                    }
                                }

                                foreach (AggressorInfo aggressor in creature.Aggressors)
                                {
                                    if (aggressor.Attacker == mobile || aggressor.Defender == mobile)
                                    {
                                        validTarget = true;
                                        break;
                                    }
                                }

                                foreach (AggressorInfo aggressed in creature.Aggressed)
                                {
                                    if (aggressed.Attacker == mobile || aggressed.Defender == mobile)
                                    {
                                        validTarget = true;
                                        break;
                                    }
                                }

                                if (validTarget)
                                    m_Queue.Enqueue(mobile);
                            }
                        }

                        else
                        {
                            if (!SpecialAbilities.MonsterCanDamage(creature, mobile)) continue;
                            if (mobile == creature) continue;

                            m_Queue.Enqueue(mobile);
                        }
                    }

                    nearbyMobiles.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        double damage = (double)(Utility.RandomMinMax(damageMin, damageMax));

                        if (tamedCreature)
                        {
                            if (mobile is PlayerMobile)
                                damage *= BaseCreature.BreathDamageToPlayerScalar * creature.PvPAbilityDamageScalar;

                            if (mobile is BaseCreature)
                                damage *= BaseCreature.BreathDamageToCreatureScalar;
                        }

                        BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(mobile, null);

                        if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat)
                            damage *= defenderDungeonArmor.DungeonArmorDetail.BreathDamageReceivedScalar;

                        if (creature != null)
                            creature.DoHarmful(mobile);

                        int finalDamage = 0;

                        switch (breathType)
                        {
                            case BreathType.Fire:
                                {
                                    damage *= 1.0;
                                }
                            break;

                            case BreathType.Ice:
                                {
                                    damage *= .75;

                                    if (mobile is PlayerMobile)
                                        SpecialAbilities.CrippleSpecialAbility(1.0, creature, mobile, .15, 10, -1, true, "", "The blast of ice has slowed your actions!");
                                    else
                                        SpecialAbilities.CrippleSpecialAbility(1.0, creature, mobile, .33, 10, -1, true, "", "The blast of ice has slowed your actions!");
                                }
                            break;

                            case BreathType.Poison:
                                {
                                    damage *= .50;

                                    int poisonLevel = 0;

                                    if (creature.HitPoison != null)
                                        poisonLevel = creature.HitPoison.Level;

                                    if (creature.Controlled && creature.ControlMaster is PlayerMobile && mobile is PlayerMobile)
                                        poisonLevel--;

                                    if (poisonLevel < 0)
                                        poisonLevel = 0;

                                    int poisonHue = 2208;

                                    poisonHue += poisonLevel;

                                    Poison poison = Poison.GetPoison(poisonLevel);
                                    mobile.ApplyPoison(mobile, poison);

                                    Effects.PlaySound(mobile.Location, mobile.Map, 0x22F);
                                    Effects.SendLocationParticles(EffectItem.Create(mobile.Location, mobile.Map, TimeSpan.FromSeconds(0.25)), 0x372A, 10, 20, poisonHue, 0, 5029, 0);
                                }
                            break;

                            case BreathType.Bone:
                                {
                                    damage *= .75;

                                    SpecialAbilities.PierceSpecialAbility(1.0, creature, mobile, .50, 30, -1, true, "", "Bone shards pierce your armor, reducing it's effectiveness!");
                                }
                            break;

                            case BreathType.Plant:
                                {
                                    damage *= .75;

                                    SpecialAbilities.EntangleSpecialAbility(1.0, creature, mobile, 1.0, 2.0, -1, true, "", "Vegetation grasps at your legs and holds you in place!");
                                }
                            break;

                            case BreathType.Electricity:
                                {
                                    damage *= .75;

                                    SpecialAbilities.HinderSpecialAbility(1.0, creature, mobile, 1.0, 1.0, true, 0x5C3, true, "", "You have been shocked by electricity!");                                    
                                }
                            break;
                        }

                        finalDamage = (int)(Math.Ceiling(damage));

                        if (finalDamage > 0)
                        {
                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            int finalAdjustedDamage = AOS.Damage(mobile, finalDamage, 100, 0, 0, 0, 0);

                            if (creature != null)
                                creature.DisplayFollowerDamage(mobile, finalAdjustedDamage);
                        }
                    }
                });
            }

            return true;
        }

        public static void KnockbackSpecialAbility(double chance, Point3D startLocation, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("KnockbackSpecialAbility");

            if (defender == null || chance == null || value == null || expirationSeconds == null || soundOverride == null) return;
            if (!defender.Alive) return;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            int distance = (int)expirationSeconds;
            double baseDamage = value;

            defender.FixedEffect(0x5683, 10, 20);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x5FC);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attackerMessage == null)
                attackerMessage = "";

            if (defenderMessage == null)
                defenderMessage = "";

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            double timePerTile = .10;
            double totalTime = timePerTile * (double)distance;

            int damagePerTile = (int)(baseDamage / (double)distance);

            if (damagePerTile < 1)
                damagePerTile = 1;

            Point3D sourceLocation = startLocation;
            Direction directionTo = Utility.GetDirection(startLocation, defender.Location);

            if (directionTo == null)
                return;

            SpecialAbilities.HinderSpecialAbility(1, attacker, defender, 1.0, 2.5, false, -1, false, "", "");

            if (pm_Defender != null)
            {
                if (pm_Defender.Female)
                    pm_Defender.PlaySound(0x14D);
                else
                    pm_Defender.PlaySound(0x156);
            }

            if (bc_Defender != null)
                bc_Defender.PlaySound(bc_Defender.GetAngerSound());

            for (int a = 0; a < distance; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * timePerTile), delegate
                {
                    if (!SpecialAbilities.Exists(defender)) return;

                    Point3D newPoint = GetPointByDirection(defender.Location, directionTo);

                    bool canFit = SpellHelper.AdjustField(ref newPoint, defender.Map, 16, false);

                    //Hit a Blocking Static
                    if (defender.Map == null || !canFit || SpellHelper.CheckMulti(newPoint, defender.Map) || !defender.Map.CanFit(newPoint, 16, false, false))
                    {
                        Effects.PlaySound(defender.Location, defender.Map, defender.GetHurtSound());

                        if (defender != null)
                        {
                            if (pm_Defender != null)
                                pm_Defender.Animate(21, 6, 1, true, false, 0);

                            else if (bc_Defender != null)
                            {
                                if (bc_Defender.IsHighSeasBodyType)
                                    bc_Defender.Animate(2, 14, 1, true, false, 0);

                                else if (bc_Defender.Body != null)
                                {
                                    if (bc_Defender.Body.IsHuman)
                                        bc_Defender.Animate(21, 6, 1, true, false, 0);

                                    else
                                        bc_Defender.Animate(2, 4, 1, true, false, 0);
                                }
                            }
                        }

                        if (attacker != null)
                            attacker.DoHarmful(defender);

                        AOS.Damage(defender, attacker, damagePerTile, 100, 0, 0, 0, 0);
                    }

                    //Continue Knockback
                    else
                    {
                        if (defender.Spell != null)
                            defender.Spell.OnCasterHurt();

                        defender.Location = newPoint;
                    }
                });
            }

            Timer.DelayCall(TimeSpan.FromSeconds(timePerTile * (double)distance + .25), delegate
            {
                if (!SpecialAbilities.Exists(defender))
                    return;

                if (pm_Defender != null)
                    pm_Defender.Animate(21, 6, 1, true, false, 0);

                else if (bc_Defender != null)
                {
                    if (bc_Defender.IsHighSeasBodyType)
                        bc_Defender.Animate(2, 14, 1, true, false, 0);

                    if (bc_Defender.Body != null)
                    {
                        if (bc_Defender.Body.IsHuman)
                            bc_Defender.Animate(21, 6, 1, true, false, 0);

                        else
                            bc_Defender.Animate(2, 4, 1, true, false, 0);
                    }
                }
            });
        }

        public static void MushroomExplosionAbility(BaseCreature creature, int minMushrooms, int maxMushrooms, int minRange, int maxRange, bool shoot)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("MushroomExplosionAbility");

            if (creature == null)
                return;

            Point3D location = creature.Location;
            Map map = creature.Map;

            bool shootMushrooms = shoot;

            int mushrooms = Utility.RandomMinMax(minMushrooms, maxMushrooms);

            if (shoot && creature.Alive)
            {
                creature.PlaySound(creature.GetAngerSound());

                creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(2);
                creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(2);

                if (creature.Body.IsHuman)
                    creature.Animate(31, 7, 1, true, false, 0);

                else if (creature.IsHighSeasBodyType)
                    creature.Animate(Utility.RandomList(27), 5, 1, true, false, 0);

                else
                    creature.Animate(4, 4, 1, true, false, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                {
                    if (creature == null)
                        shootMushrooms = false;

                    else
                    {
                        if (creature.Combatant == null)
                            shootMushrooms = false;
                        else
                        {
                            if (!creature.Alive || !creature.InLOS(creature.Combatant.Location))
                                shootMushrooms = false;
                        }
                    }

                    if (shootMushrooms)
                        StartShootMushrooms(location, creature.Combatant.Location, map, 25, minRange, maxRange, mushrooms);

                    else
                        StartShootMushrooms(location, location, map, 25, minRange, maxRange, mushrooms);
                });
            }

            else
                StartShootMushrooms(location, location, map, 25, minRange, maxRange, mushrooms);
        }

        public static void StartShootMushrooms(Point3D startLocation, Point3D endLocation, Map map, int maxLocationChecks, int minRadius, int maxRadius, int mushrooms)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("StartShootMushrooms");

            List<Point3D> m_ValidLocations = GetSpawnableTiles(startLocation, false, true, endLocation, map, mushrooms, maxLocationChecks, minRadius, maxRadius, true);

            if (m_ValidLocations.Count == 0)
                return;

            for (int a = 0; a < mushrooms; a++)
            {
                Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                Effects.PlaySound(newLocation, map, 0x4F1);

                IEntity effectStartLocation = new Entity(Serial.Zero, startLocation, map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), map);

                int particleSpeed = 5;

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, 3350, particleSpeed, 0, false, false, 0, 0);

                double distance = Utility.GetDistanceToSqrt(startLocation, newLocation);
                double destinationDelay = (double)distance * .08;

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    TimedStatic mushroom = new TimedStatic(0x1126, 1);
                    mushroom.Name = "exploding mushroom";
                    mushroom.MoveToWorld(newLocation, map);
                });

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay + 1), delegate
                {
                    Effects.PlaySound(newLocation, map, 0x22A);
                    Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(1.0)), 0x091D, 5, 10, 2597, 0, 5029, 0);

                    DetonateMushroom(newLocation, map);
                });
            }
        }

        public static void DetonateMushroom(Point3D location, Map map)
        {
            if (!Global_AllowAbilities)
                return;

            int minDamage = 5;
            int maxDamage = 15;

            Queue m_Queue = new Queue();

            IPooledEnumerable m_NearbyMobiles = map.GetMobilesInRange(location, 1);

            foreach (Mobile mobile in m_NearbyMobiles)
            {
                if (mobile is Myconid || mobile is MyconidTallstalk || mobile is TreeStalker || mobile is ArborealMyconid || mobile is TreeOfLife) continue;
                if (mobile.AccessLevel > AccessLevel.Player) continue;
                if (!mobile.CanBeDamaged()) continue;

                m_Queue.Enqueue(mobile);
            }

            m_NearbyMobiles.Free();

            while (m_Queue.Count > 0)
            {
                Mobile mobile = (Mobile)m_Queue.Dequeue();

                double damage = Utility.RandomMinMax(minDamage, maxDamage);

                if (mobile is BaseCreature)
                    damage *= 2;

                Effects.PlaySound(mobile, map, 0x4F1);
                Effects.SendLocationParticles(EffectItem.Create(mobile.Location, map, TimeSpan.FromSeconds(1.0)), 0x091D, 5, 10, 2597, 0, 5029, 0);

                new Blood().MoveToWorld(mobile.Location, mobile.Map);
                AOS.Damage(mobile, (int)damage, 100, 0, 0, 0, 0);
            }
        }

        public static void CorpseExplosionAbility(BaseCreature creature, Point3D startLocation, Point3D endLocation, bool needLOS, bool allowSameTile, Map map, int minRadius, int maxRadius, int minParts, int maxParts)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("CorpseExplosionAbility");

            int parts = Utility.RandomMinMax(minParts, maxParts);

            Point3D sourceLocation = new Point3D();

            if (creature == null)
                sourceLocation = startLocation;
            else
                sourceLocation = creature.Location;

            List<Point3D> m_ValidLocations = GetSpawnableTiles(sourceLocation, needLOS, allowSameTile, endLocation, map, parts, 25, minRadius, maxRadius, true);

            if (m_ValidLocations.Count == 0)
                return;

            for (int a = 0; a < parts; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .025), delegate
                {
                    Point3D newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];

                    Effects.PlaySound(newLocation, map, 0x4F1);

                    IEntity effectStartLocation = new Entity(Serial.Zero, startLocation, map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 2), map);

                    int itemId = Utility.RandomList
                        (
                        7389, 7397, 7398, 7395, 7402, 7408, 7407, 7393, 7584, 7405, 7585, 7600, 7587, 7602, 7394,
                        7404, 7391, 7396, 7399, 7403, 7406, 7586, 7599, 7588, 7601, 7392, 7392, 7583, 7597, 7390
                        );

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, itemId, 5, 0, false, false, 0, 0);

                    double distance = Utility.GetDistanceToSqrt(startLocation, newLocation);

                    double destinationDelay = (double)distance * .16;
                    double explosionDelay = ((double)distance * .16) + 1;

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        new Blood().MoveToWorld(new Point3D(newLocation.X + Utility.RandomMinMax(-1, 1), newLocation.Y + Utility.RandomMinMax(-1, 1), newLocation.Z + 2), map);
                        Effects.SendLocationParticles(EffectItem.Create(newLocation, map, TimeSpan.FromSeconds(2.0)), itemId, 0, 50, 0, 0, 5029, 0);
                    });

                    Timer.DelayCall(TimeSpan.FromSeconds(explosionDelay), delegate
                    {
                        DetonateCorpse(creature, newLocation, map);
                    });
                });
            }
        }

        public static void DetonateCorpse(Mobile from, Point3D location, Map map)
        {
            Effects.PlaySound(location, map, 0x22A);
            Effects.SendLocationParticles(EffectItem.Create(location, map, TimeSpan.FromSeconds(1.0)), 0x091D, 5, 10, 134, 0, 5029, 0);

            new Blood().MoveToWorld(location, map);

            Queue m_Queue = new Queue();

            IPooledEnumerable nearbyMobiles = map.GetMobilesInRange(location, 1);

            foreach (Mobile mobile in nearbyMobiles)
            {
                if (mobile == from)
                    continue;

                if (mobile is Maggot || mobile is Entrail || mobile is DiseasedViscera)
                    continue;

                if (!MonsterCanDamage(from, mobile))
                    continue;

                m_Queue.Enqueue(mobile);
            }

            nearbyMobiles.Free();

            while (m_Queue.Count > 0)
            {
                Mobile mobile = (Mobile)m_Queue.Dequeue();

                int minDamage = 10;
                int maxDamage = 20;

                double damage = (double)Utility.RandomMinMax(minDamage, maxDamage);

                if (mobile is BaseCreature)
                    damage *= 2;

                mobile.PlaySound(0x4F1);

                /*
                if (Utility.RandomDouble() < .5)
                {
                    Poison poison = Poison.GetPoison(Utility.RandomMinMax(0, 3));
                    mobile.ApplyPoison(mobile, poison);
                }
                */

                AOS.Damage(mobile, (int)damage, 100, 0, 0, 0, 0);
                new Blood().MoveToWorld(mobile.Location, mobile.Map);
            }
        }

        public static void AnimalExplosion(Mobile from, Point3D location, Map map, Type creatureType, int radius, int minDamage, int maxDamage, double goreDuration, int hue, bool affectMonsters, bool affectPlayers)
        {
            int minRange = radius * -1;
            int maxRange = radius;

            int effectHue = 2619;

            if (hue != -1)
                effectHue = hue;

            Effects.PlaySound(location, map, 0x309);

            for (int a = minRange; a < maxRange + 1; a++)
            {
                for (int b = minRange; b < maxRange + 1; b++)
                {
                    Point3D newPoint = new Point3D(location.X + a, location.Y + b, location.Z);
                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                    int distance = Utility.GetDistance(location, newPoint);

                    Timer.DelayCall(TimeSpan.FromSeconds(distance * .20), delegate
                    {
                        if (Utility.RandomDouble() < .5)
                        {
                            int itemId = Utility.RandomList(4650, 4651, 4652, 4653, 4654, 5701, 4655, 7439, 7438, 7436, 7433,
                                7431, 7428, 7425, 7410, 7415, 7416, 7418, 7420, 7425);

                            TimedStatic blood = new TimedStatic(itemId, goreDuration);
                            blood.Name = "blood";
                            blood.MoveToWorld(newPoint, map);
                        }

                        if (Utility.RandomDouble() < .33)
                        {
                            int itemId = Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 7389,
                                7395, 7408, 7407, 7405, 4655);

                            TimedStatic gore = new TimedStatic(itemId, goreDuration);
                            gore.Name = "gore";
                            gore.MoveToWorld(newPoint, map);
                        }

                        if (creatureType == typeof(Sheep))
                        {
                            if (Utility.RandomDouble() < .2)
                                new Wool().MoveToWorld(newPoint, map);
                        }

                        if (creatureType == typeof(Llama))
                        {
                            if (Utility.RandomDouble() < .2)
                                new Leather().MoveToWorld(newPoint, map);
                        }

                        if (creatureType == typeof(Bullvore))
                        {
                            if (Utility.RandomDouble() < .2)
                                new RawRibs().MoveToWorld(newPoint, map);
                        }

                        if (creatureType == typeof(Dragon))
                        {
                            if (Utility.RandomDouble() < .2)
                                new SulfurousAsh().MoveToWorld(newPoint, map);
                        }

                        if (creatureType == typeof(Mongbat))
                        {
                            if (Utility.RandomDouble() < .1)
                            {
                                switch (Utility.RandomMinMax(1, 5))
                                {
                                    case 1: new Gold(Utility.RandomMinMax(100, 250)).MoveToWorld(newPoint, map); break;
                                    case 2: new DragonCoin(Utility.RandomMinMax(5, 10)).MoveToWorld(newPoint, map); break;
                                    case 3: new GreaterExplosionPotion().MoveToWorld(newPoint, map); break;
                                    case 4: new JesterHat(Utility.RandomDyedHue()).MoveToWorld(newPoint, map); break;
                                    case 5: new JesterHat(Utility.RandomDyedHue()).MoveToWorld(newPoint, map); break;
                                }
                            }
                        }

                        Effects.PlaySound(newPoint, map, Utility.RandomList(0x4F1, 0x5D8, 0x5DA, 0x580));
                        Effects.SendLocationParticles(EffectItem.Create(newPoint, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, effectHue, 0, 5029, 0);

                        IPooledEnumerable mobilesOnTile = map.GetMobilesInRange(newPoint, 0);

                        Queue m_Queue = new Queue();

                        foreach (Mobile mobile in mobilesOnTile)
                        {
                            if (!mobile.CanBeDamaged() || !mobile.Alive || mobile.AccessLevel > AccessLevel.Player)
                                continue;

                            bool validTarget = false;

                            PlayerMobile pm_Target = mobile as PlayerMobile;
                            BaseCreature bc_Target = mobile as BaseCreature;

                            if (affectMonsters)
                            {
                                if (bc_Target != null)
                                {
                                    if (!(bc_Target.ControlMaster is PlayerMobile))
                                        validTarget = true;
                                }
                            }

                            if (affectPlayers)
                            {
                                if (pm_Target != null)
                                    validTarget = true;

                                if (bc_Target != null)
                                {
                                    if (bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                                        validTarget = true;
                                }
                            }

                            if (validTarget)
                                m_Queue.Enqueue(mobile);
                        }

                        mobilesOnTile.Free();

                        while (m_Queue.Count > 0)
                        {
                            Mobile mobile = (Mobile)m_Queue.Dequeue();

                            double damage = Utility.RandomMinMax(minDamage, maxDamage);

                            BaseCreature bc_Creature = mobile as BaseCreature;

                            if (affectPlayers && bc_Creature != null)
                            {
                                if (bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile)
                                    damage *= 2;
                            }

                            new Blood().MoveToWorld(mobile.Location, mobile.Map);
                            AOS.Damage(mobile, from, (int)damage, 0, 100, 0, 0, 0);
                        }
                    });
                }
            }
        }

        public static bool VanishAbility(BaseCreature creature, double actionsCooldown, bool newRandomLocation, int soundOverride, int minDistance, int maxDistance, bool hideIfFail, List<Point3D> presetLocations)
        {
            if (!Global_AllowAbilities)
                return false;

            LogMethodCall("VanishAbility");

            if (!SpecialAbilities.Exists(creature))
                return false;

            int soundEffect = 0x657;

            if (soundOverride != -1)
                soundEffect = soundOverride;

            if (newRandomLocation)
            {
                List<Point3D> m_ValidLocations = GetSpawnableTiles(creature.Location, true, false, creature.Location, creature.Map, 1, 15, minDistance, maxDistance, true);

                Point3D newLocation = new Point3D();

                bool foundLocation = false;

                if (m_ValidLocations.Count > 0)
                {
                    newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                    foundLocation = true;
                }

                if (!foundLocation)
                {
                    if (presetLocations != null)
                    {
                        if (presetLocations.Count > 0)
                        {
                            newLocation = presetLocations[Utility.RandomMinMax(0, presetLocations.Count - 1)];
                            foundLocation = true;
                        }
                    }
                }

                if (!foundLocation)
                {
                    if (hideIfFail)
                    {
                        newLocation = creature.Location;
                        foundLocation = true;
                    }
                }

                if (!foundLocation)
                    return false;

                creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(1);

                creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

                creature.Hidden = true;
                creature.IsStealthing = true;
                creature.StealthAttackReady = true;

                Effects.PlaySound(creature.Location, creature.Map, soundEffect);
                Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                creature.Location = newLocation;

                return true;
            }

            else
            {
                creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(actionsCooldown);

                creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

                creature.Hidden = true;
                creature.IsStealthing = true;
                creature.StealthAttackReady = true;

                Effects.PlaySound(creature.Location, creature.Map, 0x64A);
                Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                return true;
            }

            return false;
        }

        public static bool TeleportAbility(BaseCreature creature, double actionsCooldown, bool newRandomLocation, int soundOverride, int minDistance, int maxDistance, List<Point3D> presetLocations)
        {
            if (!Global_AllowAbilities)
                return false;

            LogMethodCall("TeleportAbility");

            if (!SpecialAbilities.Exists(creature))
                return false;

            int soundEffect = 0x657;

            if (soundOverride != -1)
                soundEffect = soundOverride;

            if (newRandomLocation)
            {
                List<Point3D> m_ValidLocations = GetSpawnableTiles(creature.Location, true, false, creature.Location, creature.Map, 1, 15, minDistance, maxDistance, true);

                Point3D newLocation = new Point3D();

                bool foundLocation = false;

                if (m_ValidLocations.Count > 0)
                {
                    newLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                    foundLocation = true;
                }

                if (!foundLocation)
                {
                    if (presetLocations != null)
                    {
                        if (presetLocations.Count > 0)
                        {
                            newLocation = presetLocations[Utility.RandomMinMax(0, presetLocations.Count - 1)];
                            foundLocation = true;
                        }
                    }
                }

                if (!foundLocation)
                    return false;

                creature.RevealingAction();

                Effects.PlaySound(creature.Location, creature.Map, soundEffect);
                Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                creature.Location = newLocation;

                return true;
            }

            else
            {
                creature.AIObject.NextMove = creature.AIObject.NextMove + TimeSpan.FromSeconds(actionsCooldown);

                creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
                creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

                creature.RevealingAction();

                Effects.PlaySound(creature.Location, creature.Map, 0x64A);
                Effects.SendLocationParticles(EffectItem.Create(creature.Location, creature.Map, TimeSpan.FromSeconds(5)), 0x3728, 10, 10, 2023);

                return true;
            }

            return false;
        }

        public static void ThrowObjectAbility(BaseCreature creature, Mobile target, double effectTime, double actionsCooldown, double hitChance, int damageMin, int damageMax, int itemIdA, int itemIdB, int itemHue, int throwSound, int hitSound, double speedModifier)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("ThrowObjectAbility");

            if (!SpecialAbilities.Exists(creature))
                return;

            if (!SpecialAbilities.Exists(target))
                return;

            creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(effectTime);
            creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(effectTime);

            creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

            if (creature.Body.IsHuman)
                creature.Animate(31, 7, 1, true, false, 0);

            else
                creature.Animate(4, 4, 1, true, false, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (!SpecialAbilities.Exists(creature))
                    return;

                if (!SpecialAbilities.Exists(target))
                    return;

                if (throwSound == -1)
                    throwSound = 0x5D3;

                Effects.PlaySound(creature.Location, creature.Map, throwSound);

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z + 10), creature.Map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.Location.X, target.Location.Y, target.Location.Z + 5), creature.Map);

                Direction direction = creature.GetDirectionTo(target.Location);

                int itemIdToUse = itemIdA;

                switch (creature.Direction)
                {
                    case Server.Direction.Up: itemIdToUse = itemIdB; break;
                    case Server.Direction.North: itemIdToUse = itemIdB; break;
                    case Server.Direction.Right: itemIdToUse = itemIdB; break;
                    case Server.Direction.East: itemIdToUse = itemIdB; break;
                    case Server.Direction.Down: itemIdToUse = itemIdA; break;
                    case Server.Direction.South: itemIdToUse = itemIdA; break;
                    case Server.Direction.Left: itemIdToUse = itemIdA; break;
                    case Server.Direction.West: itemIdToUse = itemIdA; break;
                }

                Effects.SendMovingEffect(startLocation, endLocation, itemIdToUse, (int)(15 * speedModifier), 0, true, false, itemHue, 0);

                double distance = creature.GetDistanceToSqrt(target.Location);
                double destinationDelay = (double)distance * .08 * (.5 / speedModifier);

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (!SpecialAbilities.Exists(creature))
                        return;

                    if (!SpecialAbilities.Exists(target))
                        return;

                    if (Utility.RandomDouble() < hitChance)
                    {
                        Effects.PlaySound(creature.Location, creature.Map, hitSound);

                        int damage = Utility.RandomMinMax(damageMin, damageMax);

                        if (damage < 1)
                            damage = 1;

                        creature.DoHarmful(target);
                        AOS.Damage(target, creature, damage, 100, 0, 0, 0, 0);
                        new Blood().MoveToWorld(target.Location, target.Map);
                    }
                });
            });
        }

        public static void ThrowPotionAbility(BaseCreature creature, Mobile target, double effectTime, double actionsCooldown, PotionAbilityEffectType potionEffectType, int radius, int minDamage, int maxDamage, double value, double duration, bool canHitCreatures, bool canHitCombatant)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("ThrowPotionAbility");

            if (!SpecialAbilities.Exists(creature))
                return;

            if (!SpecialAbilities.Exists(target))
                return;

            int itemId = 0;
            int itemHue = 0;

            switch (potionEffectType)
            {
                case PotionAbilityEffectType.Explosion: itemId = 0xF0D; itemHue = 0; break;
                case PotionAbilityEffectType.Paralyze: itemId = 3849; itemHue = 53; break;
                case PotionAbilityEffectType.Poison: itemId = 0xF0A; itemHue = 0; break;
                case PotionAbilityEffectType.Frost: itemId = 3849; itemHue = 1150; break;
                case PotionAbilityEffectType.Shrapnel: itemId = 3849; itemHue = 1758; break;
                case PotionAbilityEffectType.Void: itemId = 3849; itemHue = 0; break;
            }

            Point3D destination = target.Location;
            Map map = target.Map;

            creature.AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(effectTime);
            creature.NextCombatTime = creature.NextCombatTime + TimeSpan.FromSeconds(effectTime);

            creature.NextSpellTime = creature.NextSpellTime + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatHealActionAllowed = creature.NextCombatHealActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatSpecialActionAllowed = creature.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(actionsCooldown);
            creature.NextCombatEpicActionAllowed = creature.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(actionsCooldown);

            if (creature.Body.IsHuman)
                creature.Animate(31, 7, 1, true, false, 0);

            else
                creature.Animate(4, 4, 1, true, false, 0);

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (!SpecialAbilities.Exists(creature))
                    return;

                if (!SpecialAbilities.Exists(target))
                    return;

                Effects.PlaySound(creature.Location, creature.Map, 0x5D3);

                IEntity startLocation = new Entity(Serial.Zero, new Point3D(creature.Location.X, creature.Location.Y, creature.Location.Z + 10), creature.Map);
                IEntity endLocation = new Entity(Serial.Zero, new Point3D(target.Location.X, target.Location.Y, target.Location.Z + 5), creature.Map);

                Direction direction = creature.GetDirectionTo(target.Location);

                Effects.SendMovingEffect(startLocation, endLocation, itemId, 15, 0, true, false, itemHue, 0);

                double distance = creature.GetDistanceToSqrt(target.Location);
                double destinationDelay = (double)distance * .08;

                Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                {
                    if (!SpecialAbilities.Exists(creature))
                        return;

                    if (!SpecialAbilities.Exists(target))
                        return;

                    Effects.PlaySound(destination, map, 0x56E);

                    Dictionary<Point3D, double> m_ExplosionLocations = new Dictionary<Point3D, double>();

                    m_ExplosionLocations.Add(destination, 0);

                    for (int a = 1; a < radius + 1; a++)
                    {
                        m_ExplosionLocations.Add(new Point3D(destination.X - a, destination.Y - a, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X, destination.Y - a, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X + a, destination.Y - a, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X + a, destination.Y, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X + a, destination.Y + a, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X, destination.Y + a, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X - a, destination.Y + a, destination.Z), a);
                        m_ExplosionLocations.Add(new Point3D(destination.X - a, destination.Y, destination.Z), a);
                    }

                    foreach (KeyValuePair<Point3D, double> pair in m_ExplosionLocations)
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(pair.Value * .25), delegate
                        {
                            if (!SpecialAbilities.Exists(creature))
                                return;

                            if (!SpecialAbilities.Exists(target))
                                return;

                            switch (potionEffectType)
                            {
                                case PotionAbilityEffectType.Explosion:
                                    Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(0.5)), 0x36BD, 20, 10, 5044);
                                    break;

                                case PotionAbilityEffectType.Paralyze:
                                    Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(.5)), 0x3973, 10, 50, 5029);
                                    break;

                                case PotionAbilityEffectType.Poison:
                                    Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(0.5)), 0x372A, 10, 20, 59, 0, 5029, 0);
                                    break;

                                case PotionAbilityEffectType.Frost:
                                    Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(0.25)), 0x3779, 10, 20, 1153, 0, 5029, 0);
                                    break;

                                case PotionAbilityEffectType.Shrapnel:
                                    Effects.SendLocationParticles(EffectItem.Create(pair.Key, map, TimeSpan.FromSeconds(.25)), 14276, 50, 10, 2582);
                                    break;
                            }
                        });
                    }

                    List<Mobile> m_TargetsHit = new List<Mobile>();

                    IPooledEnumerable eable = map.GetMobilesInRange(destination, radius);

                    foreach (Mobile mobile in eable)
                    {
                        if (mobile == null) continue;
                        if (!mobile.Alive || mobile.Deleted) continue;

                        if (creature == null) continue;
                        if (!creature.Alive || creature.Deleted) continue;
                        if (creature == mobile) continue;

                        bool validTarget = CanBeHit(null, mobile, false, canHitCreatures);

                        if (validTarget)
                            m_TargetsHit.Add(mobile);
                    }

                    eable.Free();

                    int targets = m_TargetsHit.Count;

                    for (int a = 0; a < targets; a++)
                    {
                        double damage = 0;

                        Mobile mobile = m_TargetsHit[a];

                        if (mobile == null) continue;
                        if (!mobile.Alive || mobile.Deleted) continue;

                        switch (potionEffectType)
                        {
                            case PotionAbilityEffectType.Explosion:
                                damage = (double)(Utility.RandomMinMax(minDamage, maxDamage));

                                if (mobile is BaseCreature)
                                    damage *= 1.5;

                                AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                                break;

                            case PotionAbilityEffectType.Paralyze:
                                if (creature != null)
                                    SpecialAbilities.EntangleSpecialAbility(1.0, creature, mobile, 1, duration, -1, true, "", "You are held in place by mystical energy!");
                                break;

                            case PotionAbilityEffectType.Poison:
                                Poison poison = Poison.GetPoison((int)value);
                                mobile.ApplyPoison(mobile, poison);
                                break;

                            case PotionAbilityEffectType.Frost:
                                damage = (double)(Utility.RandomMinMax(minDamage, maxDamage));

                                if (mobile is BaseCreature)
                                    damage *= 1.5;

                                if (creature != null)
                                    SpecialAbilities.CrippleSpecialAbility(1.0, creature, mobile, value, duration, -1, true, "", "The blast of ice has slowed your actions!");

                                AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                                break;

                            case PotionAbilityEffectType.Void:
                                damage = (double)(Utility.RandomMinMax(minDamage, maxDamage));

                                if (mobile is BaseCreature)
                                    damage *= 1.5;

                                AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                                break;

                            case PotionAbilityEffectType.Shrapnel:
                                damage = (double)(Utility.RandomMinMax(minDamage, maxDamage));

                                if (mobile is BaseCreature)
                                    damage *= 1.5;

                                mobile.Stam -= (int)value;
                                mobile.Mana -= (int)value;

                                AOS.Damage(mobile, (int)damage, 0, 100, 0, 0, 0);
                                break;
                        }
                    }
                });
            });
        }

        public static void PetrifySpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("PetrifySpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            if (bc_Defender != null)
            {
                bool immune = false;

                if (bc_Defender.MovementRestrictionImmune)
                    immune = true;

                if (Utility.RandomDouble() <= bc_Defender.MovementImmunityChance)
                    immune = true;

                if (immune == true)
                {
                    if (attacker != null)
                        attacker.DoHarmful(defender);

                    if (soundOverride == -1)
                        Effects.PlaySound(bc_Defender.Location, bc_Defender.Map, 0x65A);

                    else if (soundOverride > 0)
                        Effects.PlaySound(bc_Defender.Location, bc_Defender.Map, soundOverride);

                    if (showEffect)
                        bc_Defender.FixedEffect(0x5683, 10, 20);

                    int damage = Utility.RandomMinMax(40, 60);

                    AOS.Damage(bc_Defender, attacker, damage, 100, 0, 0, 0, 0);

                    if (pm_Attacker != null)
                    {
                        attacker.SendMessage("Your target overpowered your petrify effect, and you instead strike a vicious hit against them.");

                        if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                            pm_Attacker.SendMessage(pm_Attacker.PlayerMeleeDamageTextHue, "Your vicious hit against " + defender.RawName + " deals " + damage.ToString() + " damage.");

                        if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                            defender.PrivateOverheadMessage(MessageType.Regular, pm_Attacker.PlayerMeleeDamageTextHue, false, "-" + damage.ToString(), pm_Attacker.NetState);
                    }

                    return;
                }

                double totalValue;

                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Petrify, out totalValue);

                if (totalValue <= 0)
                {
                    if (showEffect)
                        Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, 911, 0, 5029, 0);

                    if (soundOverride == -1)
                        Effects.PlaySound(defender.Location, defender.Map, 0x65A);

                    else if (soundOverride > 0)
                        Effects.PlaySound(defender.Location, defender.Map, soundOverride);

                    if (attacker != null)
                        attacker.SendMessage(attackerMessage);

                    defender.SendMessage(defenderMessage);

                    if (!(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsBoss() || bc_Defender.IsMiniBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                        expirationSeconds /= bc_Defender.PlayerProximityDifficultyDivisor;

                    bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Petrify, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));

                    if (bc_Defender.AIObject != null)
                    {
                        bc_Defender.Frozen = true;
                        bc_Defender.DelayNextMovement(expirationSeconds);
                        bc_Defender.DelayNextCombatTime(expirationSeconds);

                        if (bc_Defender.HueMod == -1)
                            bc_Defender.HueMod = 911;
                    }

                }
            }

            else if (pm_Defender != null)
            {
                double totalValue;

                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Petrify, out totalValue);

                if (totalValue <= 0)
                {
                    if (showEffect)
                        Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, 911, 0, 5029, 0);

                    if (soundOverride == -1)
                        Effects.PlaySound(defender.Location, defender.Map, 0x65A);

                    else if (soundOverride > 0)
                        Effects.PlaySound(defender.Location, defender.Map, soundOverride);

                    if (attacker != null)
                        attacker.SendMessage(attackerMessage);

                    defender.SendMessage(defenderMessage);

                    pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Petrify, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));

                    if (!KinPaint.IsWearingKinPaint(pm_Defender) || pm_Defender.HueMod == -1)
                        pm_Defender.HueMod = 911;

                    pm_Defender.Frozen = true;
                    pm_Defender.NextCombatTime = pm_Defender.NextCombatTime + TimeSpan.FromSeconds(expirationSeconds);
                }
            }
        }

        public static void EntangleSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("EntangleSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            bool immune = false;

            if (bc_Defender != null)
            {
                if (bc_Defender.MovementRestrictionImmune)
                    immune = true;

                if (Utility.RandomDouble() <= bc_Defender.MovementImmunityChance)
                    immune = true;

                if (immune == true)
                {
                    if (attacker != null)
                        attacker.DoHarmful(bc_Defender);

                    if (soundOverride == -1)
                        Effects.PlaySound(bc_Defender.Location, bc_Defender.Map, 0x525);

                    else if (soundOverride > 0)
                        Effects.PlaySound(bc_Defender.Location, bc_Defender.Map, soundOverride);

                    if (showEffect)
                        bc_Defender.FixedEffect(0x5683, 10, 20);

                    int damage = Utility.RandomMinMax(20, 40);

                    AOS.Damage(bc_Defender, attacker, damage, 100, 0, 0, 0, 0);

                    if (pm_Attacker != null)
                    {
                        attacker.SendMessage("Your target overpowered your entangle effect, and you instead strike a vicious hit against them.");

                        if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                            pm_Attacker.SendMessage(pm_Attacker.PlayerMeleeDamageTextHue, "Your vicious hit against " + defender.RawName + " deals " + damage.ToString() + " damage.");

                        if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                            defender.PrivateOverheadMessage(MessageType.Regular, pm_Attacker.PlayerMeleeDamageTextHue, false, "-" + damage.ToString(), pm_Attacker.NetState);
                    }

                    return;
                }

                double hinderValue;
                double entangleValue;

                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Entangle, out hinderValue);
                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Entangle, out entangleValue);

                if (hinderValue <= 0 || entangleValue <= 0)
                {
                    if (showEffect)
                        defender.FixedEffect(0x5683, 10, 20);

                    if (soundOverride == -1)
                        Effects.PlaySound(defender.Location, defender.Map, 0x51c);

                    else if (soundOverride > 0)
                        Effects.PlaySound(defender.Location, defender.Map, soundOverride);

                    if (attacker != null)
                        attacker.SendMessage(attackerMessage);

                    defender.SendMessage(defenderMessage);

                    if (!(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsBoss() || bc_Defender.IsMiniBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                        expirationSeconds /= bc_Defender.PlayerProximityDifficultyDivisor;

                    bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Entangle, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));

                    if (bc_Defender.AIObject != null)
                    {
                        bc_Defender.DelayNextMovement(expirationSeconds);
                    }
                }
            }

            else if (pm_Defender != null)
            {
                double hinderValue;
                double entangleValue;

                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Entangle, out hinderValue);
                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Entangle, out entangleValue);

                if (hinderValue <= 0 || entangleValue <= 0)
                {
                    if (showEffect)
                        defender.FixedEffect(0x5683, 10, 20);

                    if (soundOverride == -1)
                        Effects.PlaySound(defender.Location, defender.Map, 0x51c);

                    else if (soundOverride > 0)
                        Effects.PlaySound(defender.Location, defender.Map, soundOverride);

                    if (attacker != null)
                        attacker.SendMessage(attackerMessage);

                    defender.SendMessage(defenderMessage);

                    pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Entangle, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));

                    pm_Defender.CantWalk = true;
                }
            }
        }

        public static void StunSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("StunSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            if (showEffect)
                defender.FixedEffect(0x5683, 10, 20);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x5FC);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Defender != null)
            {
                if (!(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsBoss() || bc_Defender.IsMiniBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                    value /= bc_Defender.PlayerProximityDifficultyDivisor;

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Stun, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Stun, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void BacklashSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("BacklashSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            if (showEffect)
                defender.FixedEffect(0x91B, 10, 20, 2593, 0);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x5C5);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Defender != null)
            {
                if (!(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsMiniBoss() || bc_Defender.IsBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                    value /= bc_Defender.PlayerProximityDifficultyDivisor;

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Backlash, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Backlash, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void HinderSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, bool ignoreImmunity, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            LogMethodCall("HinderSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            if (bc_Defender != null)
            {
                bool immune = false;

                if (bc_Defender.MovementRestrictionImmune && !ignoreImmunity)
                    immune = true;

                if (Utility.RandomDouble() <= bc_Defender.MovementImmunityChance && !ignoreImmunity)
                    immune = true;

                if (immune == true)
                {
                    if (attacker != null)
                        attacker.DoHarmful(defender);

                    if (soundOverride == -1)
                        Effects.PlaySound(bc_Defender.Location, bc_Defender.Map, 0x525);

                    else if (soundOverride > 0)
                        Effects.PlaySound(bc_Defender.Location, bc_Defender.Map, soundOverride);

                    if (showEffect)
                        bc_Defender.FixedEffect(0x5683, 10, 20);

                    int damage = Utility.RandomMinMax(40, 60);

                    AOS.Damage(bc_Defender, attacker, damage, 100, 0, 0, 0, 0);

                    if (pm_Attacker != null)
                    {
                        attacker.SendMessage("Your target overpowered your hinder effect, and you instead strike a vicious hit against them.");

                        if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateMessage)
                            pm_Attacker.SendMessage(pm_Attacker.PlayerMeleeDamageTextHue, "Your vicious hit against " + defender.RawName + " deals " + damage.ToString() + " damage.");

                        if (pm_Attacker.m_ShowMeleeDamage == DamageDisplayMode.PrivateOverhead)
                            defender.PrivateOverheadMessage(MessageType.Regular, pm_Attacker.PlayerMeleeDamageTextHue, false, "-" + damage.ToString(), pm_Attacker.NetState);
                    }

                    return;
                }

                double totalValue;

                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Hinder, out totalValue);

                if (totalValue <= 0)
                {
                    if (bc_Defender.Spell != null)
                        bc_Defender.Spell = null;

                    if (showEffect)
                        defender.FixedEffect(0x5683, 10, 20);

                    if (soundOverride == -1)
                        Effects.PlaySound(defender.Location, defender.Map, 0x51c);

                    else if (soundOverride > 0)
                        Effects.PlaySound(defender.Location, defender.Map, soundOverride);

                    if (attacker != null)
                        attacker.SendMessage(attackerMessage);

                    defender.SendMessage(defenderMessage);

                    if (!ignoreImmunity && !(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsBoss() || bc_Defender.IsMiniBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                        expirationSeconds /= bc_Defender.PlayerProximityDifficultyDivisor;

                    bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Hinder, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));

                    if (bc_Defender.AIObject != null)
                    {
                        bc_Defender.Frozen = true;

                        bc_Defender.NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds);

                        bc_Defender.DelayNextMovement(expirationSeconds);
                        bc_Defender.DelayNextCombatTime(expirationSeconds);

                        bc_Defender.NextSpellTime = bc_Defender.NextSpellTime + TimeSpan.FromSeconds(expirationSeconds);
                        bc_Defender.NextCombatSpecialActionAllowed = bc_Defender.NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(expirationSeconds);
                        bc_Defender.NextCombatEpicActionAllowed = bc_Defender.NextCombatEpicActionAllowed + TimeSpan.FromSeconds(expirationSeconds);
                        bc_Defender.NextCombatHealActionAllowed = bc_Defender.NextCombatHealActionAllowed + TimeSpan.FromSeconds(expirationSeconds);
                    }
                }
            }

            else if (pm_Defender != null)
            {
                double totalValue;

                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Hinder, out totalValue);

                if (pm_Defender.Spell != null)
                    pm_Defender.Spell.OnCasterHurt();

                if (totalValue <= 0)
                {
                    if (showEffect)
                        defender.FixedEffect(0x5683, 10, 20);

                    if (soundOverride == -1)
                        Effects.PlaySound(defender.Location, defender.Map, 0x51c);

                    else if (soundOverride > 0)
                        Effects.PlaySound(defender.Location, defender.Map, soundOverride);

                    if (attacker != null)
                        attacker.SendMessage(attackerMessage);

                    defender.SendMessage(defenderMessage);

                    pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Hinder, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));

                    pm_Defender.Frozen = true;
                    pm_Defender.NextCombatTime = pm_Defender.NextCombatTime + TimeSpan.FromSeconds(expirationSeconds);
                }
            }
        }

        public static void CrippleSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("CrippleSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            if (showEffect)
                defender.FixedEffect(0x5683, 10, 20);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x510);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Defender != null)
            {
                if (!(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsBoss() || bc_Defender.IsMiniBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                    value /= bc_Defender.PlayerProximityDifficultyDivisor;

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Cripple, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Cripple, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void BleedSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("BleedSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (showEffect)
                defender.FixedEffect(0x5683, 10, 20);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x51e);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Attacker != null)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile && pm_Defender != null)
                    value *= bc_Attacker.PvPAbilityDamageScalar;
            }

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat)
                value *= defenderDungeonArmor.DungeonArmorDetail.BleedDamageReceivedScalar;

            int intervalFrequency = 2;
            int numberOfIntervals = (int)(Math.Floor(expirationSeconds / intervalFrequency));
            int damage = (int)((double)value / (double)numberOfIntervals);

            if (damage < 1)
                damage = 1;

            for (int a = 0; a < numberOfIntervals; a++)
            {
                if (bc_Defender != null)
                    bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Bleed, attacker, damage, DateTime.UtcNow + TimeSpan.FromSeconds((a + 1) * intervalFrequency)));

                else if (pm_Defender != null)
                    pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Bleed, attacker, damage, DateTime.UtcNow + TimeSpan.FromSeconds((a + 1) * intervalFrequency)));
            }
        }

        public static void DiseaseSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("DiseaseSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            double totalValue;

            if (bc_Defender != null)
            {
                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease, out totalValue);

                if (totalValue > 0)
                    return;
            }

            if (pm_Defender != null)
            {
                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease, out totalValue);

                if (totalValue > 0)
                    return;
            }

            if (showEffect)
                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(0.2)), 0x372A, 6, 20, 2053, 0, 5029, 0);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x5CC);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Attacker != null && bc_Defender != null)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile && pm_Defender != null)
                    value *= bc_Attacker.PvPAbilityDamageScalar;
            }

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat)
                value *= defenderDungeonArmor.DungeonArmorDetail.DiseaseDamageReceivedScalar;

            int intervalFrequency = 6;
            int numberOfIntervals = (int)(Math.Floor(expirationSeconds / intervalFrequency));

            for (int a = 0; a < numberOfIntervals; a++)
            {
                if (bc_Defender != null)
                    bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Disease, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds((a + 1) * intervalFrequency)));

                else if (pm_Defender != null)
                    pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Disease, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds((a + 1) * intervalFrequency)));
            }
        }

        public static void CrushingBlowSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("CrushingBlowSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (showEffect)
                defender.FixedEffect(0x5683, 10, 20);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x525);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Attacker != null)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile && pm_Defender != null)
                    value *= bc_Attacker.PvPAbilityDamageScalar;
            }

            double stamPercent = (double)defender.Stam / (double)defender.StamMax;

            double minDamage = value * .5;
            double damage = minDamage + (value * (1 - stamPercent));

            if (damage < 1)
                damage = 1;

            if (attacker != null)
                attacker.DoHarmful(defender);

            AOS.Damage(defender, attacker, (int)damage, 100, 0, 0, 0, 0);
        }

        public static void PierceSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("PierceSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            BaseDungeonArmor.PlayerDungeonArmorProfile defenderDungeonArmor = new BaseDungeonArmor.PlayerDungeonArmorProfile(pm_Defender, null);

            if (defenderDungeonArmor.MatchingSet && !defenderDungeonArmor.InPlayerCombat && attacker is BaseCreature && pm_Defender != null)
            {
                if (Utility.RandomDouble() <= defenderDungeonArmor.DungeonArmorDetail.SpecialEffectAvoidanceChance)
                {
                    Effects.PlaySound(pm_Defender.Location, pm_Defender.Map, 0x64B);
                    Effects.SendLocationParticles(EffectItem.Create(pm_Defender.Location, pm_Defender.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, defenderDungeonArmor.DungeonArmorDetail.EffectHue, 0, 5005, 0);

                    return;
                }
            }

            if (showEffect)
                defender.FixedEffect(0x5683, 10, 20);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x525);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Defender != null)
            {
                if (!(bc_Defender.ControlMaster is PlayerMobile) && (bc_Defender.InitialDifficulty >= BaseCreature.HighDifficultyThreshold || bc_Defender.IsMiniBoss() || bc_Defender.IsBoss() || bc_Defender.IsEventBoss() || bc_Defender.IsLoHBoss()))
                    value /= bc_Defender.PlayerProximityDifficultyDivisor;

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Pierce, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Pierce, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void FrenzySpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (!Global_AllowAbilities)
                return;

            LogMethodCall("FrenzySpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(attacker))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            BaseCreature bc_Attacker = attacker as BaseCreature;
            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            if (bc_Attacker != null)
            {
                double totalValue;

                bc_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Frenzy, out totalValue);

                if (totalValue > 0)
                    return;

                if (soundOverride == -1)
                    bc_Attacker.PlaySound(bc_Attacker.GetAngerSound());

                else if (soundOverride > 0)
                    bc_Attacker.PlaySound(soundOverride);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*becomes frenzied*");

                    else
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                bc_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Frenzy, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Attacker != null)
            {
                double totalValue;

                pm_Attacker.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Frenzy, out totalValue);

                if (totalValue > 0)
                    return;

                if (soundOverride == -1)
                    pm_Attacker.PlaySound(pm_Attacker.GetAngerSound());

                else if (soundOverride > 0)
                    pm_Attacker.PlaySound(soundOverride);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*becomes frenzied*");

                    else
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Frenzy, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void EnrageSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (bc_Defender != null)
            {
                if (soundOverride == -1)
                    bc_Defender.PlaySound(bc_Defender.GetAngerSound());

                else if (soundOverride > 0)
                    bc_Defender.PlaySound(soundOverride);

                if (showEffect)
                    bc_Defender.FixedParticles(0x373A, 10, 15, 5036, 2116, 0, EffectLayer.Head);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        bc_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*becomes enraged*");

                    else
                        bc_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Enrage, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
            {
                if (soundOverride == -1)
                    pm_Defender.PlaySound(pm_Defender.GetAngerSound());

                else if (soundOverride > 0)
                    pm_Defender.PlaySound(soundOverride);

                if (showEffect)
                    pm_Defender.FixedParticles(0x373A, 10, 15, 5036, 2116, 0, EffectLayer.Head);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*becomes enraged*");

                    else
                        pm_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Enrage, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void CourageSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            LogMethodCall("CourageSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(attacker))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            BaseCreature bc_Attacker = attacker as BaseCreature;
            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            int effectHue = 2503;

            if (bc_Attacker != null)
            {
                if (soundOverride == -1)
                    bc_Attacker.PlaySound(0x650);

                else if (soundOverride > 0)
                    bc_Attacker.PlaySound(soundOverride);

                if (showEffect)
                    bc_Attacker.FixedParticles(0x373A, 10, 15, 5036, effectHue, 0, EffectLayer.Head);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon courage*");
                    else
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                bc_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Courage, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Attacker != null)
            {
                if (soundOverride == -1)
                    pm_Attacker.PlaySound(0x650);

                else if (soundOverride > 0)
                    pm_Attacker.PlaySound(soundOverride);

                if (showEffect)
                    pm_Attacker.FixedParticles(0x373A, 10, 15, 5036, effectHue, 0, EffectLayer.Head);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon courage*");
                    else
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Courage, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void FortitudeSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            LogMethodCall("FortitudeSpecialAbility");

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            int effectHue = 2503;

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (bc_Defender != null)
            {
                if (soundOverride == -1)
                    bc_Defender.PlaySound(0x65A);

                else if (soundOverride > 0)
                    bc_Defender.PlaySound(soundOverride);

                if (showEffect)
                    bc_Defender.FixedParticles(0x375A, 10, 30, 5011, effectHue, 0, EffectLayer.Head);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        bc_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon fortitude*");
                    else
                        bc_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Fortitude, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
            {
                if (soundOverride == -1)
                    pm_Defender.PlaySound(0x65A);

                else if (soundOverride > 0)
                    pm_Defender.PlaySound(soundOverride);

                if (showEffect)
                    pm_Defender.FixedParticles(0x375A, 10, 30, 5011, effectHue, 0, EffectLayer.Head);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon fortitude*");
                    else
                        pm_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Fortitude, attacker, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void FlamestrikeSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            BaseCreature bc_Attacker = attacker as BaseCreature;
            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (showEffect)
                defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);

            if (soundOverride == -1)
                Effects.PlaySound(defender.Location, defender.Map, 0x5CF);

            else if (soundOverride > 0)
                Effects.PlaySound(defender.Location, defender.Map, soundOverride);

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            defender.SendMessage(defenderMessage);

            if (bc_Attacker != null)
            {
                if (bc_Attacker.Controlled && bc_Attacker.ControlMaster is PlayerMobile && pm_Defender != null)
                    value *= bc_Attacker.PvPAbilityDamageScalar;
            }

            if (attacker != null)
                attacker.DoHarmful(defender);

            AOS.Damage(defender, attacker, (int)value, 0, 100, 0, 0, 0);
        }

        public static void EnergySiphonSpecialAbility(double chance, Mobile attacker, Mobile defender, double scalar, int range, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage)
        {
            if (!Global_AllowAbilities)
                return;

            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(attacker))
                return;

            BaseCreature bc_Attacker = attacker as BaseCreature;

            bool defenderExists = true;

            if (!SpecialAbilities.Exists(defender))
                defenderExists = false;

            if (showEffect && defenderExists)
                Effects.SendLocationParticles(EffectItem.Create(defender.Location, defender.Map, TimeSpan.FromSeconds(0.2)), 0x372A, 6, 20, 2636, 0, 5029, 0);

            if (soundOverride == -1)
            {
                Effects.PlaySound(attacker.Location, defender.Map, 0x457);

                if (defenderExists)
                    Effects.PlaySound(defender.Location, defender.Map, 0x457);
            }

            else if (soundOverride > 0)
            {
                Effects.PlaySound(attacker.Location, defender.Map, soundOverride);

                if (defenderExists)
                    Effects.PlaySound(defender.Location, defender.Map, soundOverride);
            }

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defenderExists)
            {
                defender.SendMessage(defenderMessage);
                attacker.DoHarmful(defender);
            }

            int hitsRegen = (int)(Math.Round((double)attacker.HitsMax * .125 * scalar));
            int stamRegen = (int)(Math.Round((double)attacker.StamMax * .15 * scalar));
            int manaRegen = (int)(Math.Round((double)attacker.ManaMax * .10 * scalar));

            attacker.Heal(hitsRegen);
            attacker.Stam += stamRegen;
            attacker.Mana += manaRegen;
        }

        public static void ExpertiseSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(attacker))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Attacker = attacker as PlayerMobile;
            BaseCreature bc_Attacker = attacker as BaseCreature;

            int effectHue = 2588;

            if (bc_Attacker != null)
            {
                if (soundOverride == -1)
                    bc_Attacker.PlaySound(0x28E);

                else if (soundOverride > 0)
                    bc_Attacker.PlaySound(soundOverride);

                if (showEffect)
                    bc_Attacker.FixedParticles(0x375A, 10, 30, 5010, effectHue, 0, EffectLayer.Waist);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon expertise*");
                    else
                        bc_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                bc_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Expertise, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Attacker != null)
            {
                if (soundOverride == -1)
                    pm_Attacker.PlaySound(0x28E);

                else if (soundOverride > 0)
                    pm_Attacker.PlaySound(soundOverride);

                if (showEffect)
                    pm_Attacker.FixedParticles(0x375A, 10, 30, 5010, effectHue, 0, EffectLayer.Waist);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*draws upon expertise*");
                    else
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Expertise, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void EvasionSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (showEffect)
            {
                int projectiles = 6;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(defender.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), defender.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), defender.Z);
                    SpellHelper.AdjustField(ref newLocation, defender.Map, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z), defender.Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), defender.Map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x37B9), particleSpeed, 0, false, false, 0, 0);
                }
            }

            if (bc_Defender != null)
            {
                if (soundOverride == -1)
                    bc_Defender.PlaySound(0x512);

                else if (soundOverride > 0)
                    bc_Defender.PlaySound(soundOverride);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        bc_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*begins to evade*");
                    else
                        bc_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                bc_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Evasion, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }

            else if (pm_Defender != null)
            {
                if (soundOverride == -1)
                    pm_Defender.PlaySound(0x512);

                else if (soundOverride > 0)
                    pm_Defender.PlaySound(soundOverride);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, "*begins to evade*");

                    else
                        pm_Defender.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Evasion, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void InspirationSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Inspiration, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void ProviderSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Provider, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void ScientistSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Scientist, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void TechnicianSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Technician, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void EmergencyRepairsSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.EmergencyRepairs, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void SearcherSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Searcher, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void PhalanxSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Phalanx, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void HardySpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Hardy, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void RapidTreatmentSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.RapidTreatment, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void SuperiorHealingSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.SuperiorHealing, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void IronFistsSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(attacker))
                return;

            if (attacker != null)
                attacker.SendMessage(attackerMessage);

            if (defender != null)
                defender.SendMessage(defenderMessage);

            PlayerMobile pm_Attacker = attacker as PlayerMobile;

            if (pm_Attacker != null)
            {
                if (soundOverride == -1)
                    pm_Attacker.PlaySound(0x51A);

                else if (soundOverride > 0)
                    pm_Attacker.PlaySound(soundOverride);

                if (showEffect)
                    pm_Attacker.FixedParticles(0x3773, 10, 30, 5010, 2610, 0, EffectLayer.Waist);

                if (emoteMessage != "-1")
                {
                    if (emoteMessage == "")
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, "*fists become iron*");
                    else
                        pm_Attacker.PublicOverheadMessage(MessageType.Regular, 0, false, emoteMessage);
                }

                pm_Attacker.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.IronFists, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
            }
        }

        public static void IgniteSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            if (showEffect)
                defender.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);

            if (soundOverride == -1)
                defender.PlaySound(0x5CF);

            else if (soundOverride > 0)
                defender.PlaySound(soundOverride);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Ignite, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void BileSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            if (showEffect)
                defender.FixedEffect(0x372A, 10, 30, 2208, 0);

            if (soundOverride == -1)
                defender.PlaySound(0x22F);

            else if (soundOverride > 0)
                defender.PlaySound(soundOverride);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.Bile, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }

        public static void ShieldOfBonesSpecialAbility(double chance, Mobile attacker, Mobile defender, double value, double expirationSeconds, int soundOverride, bool showEffect, string attackerMessage, string defenderMessage, string emoteMessage)
        {
            if (Utility.RandomDouble() > chance)
                return;

            if (!SpecialAbilities.Exists(defender))
                return;

            if (defender != null)
                defender.SendMessage(defenderMessage);

            else if (soundOverride > 0)
                defender.PlaySound(soundOverride);

            PlayerMobile pm_Defender = defender as PlayerMobile;

            if (pm_Defender != null)
                pm_Defender.AddSpecialAbilityEffectEntry(new SpecialAbilityEffectEntry(SpecialAbilityEffect.ShieldOfBones, defender, value, DateTime.UtcNow + TimeSpan.FromSeconds(expirationSeconds)));
        }
    }
}