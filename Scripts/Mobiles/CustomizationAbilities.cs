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
using Server.Mobiles;

namespace Server
{
    public class CustomizationAbilities
    {
        public static void PlayerDeathExplosion(Point3D location, Map map, bool carnage, bool violentDeath)
        {
            List<int> m_MandatoryParts = new List<int>();
            List<int> m_ExtraParts = new List<int>();

            #region CorpseItems

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1:
                    m_MandatoryParts.Add(Utility.RandomList(7393, 7401, 7584, 7598)); //Head
                    m_MandatoryParts.Add(Utility.RandomList(7583, 7597)); //Torso
                break;
                case 2:
                     m_MandatoryParts.Add(Utility.RandomList(7398, 7392, 7392, 7390)); //Torso With Head
                break;
            }

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1:
                    m_MandatoryParts.Add(Utility.RandomList(7587, 7602, 7394, 7395)); //Left Leg
                    m_MandatoryParts.Add(Utility.RandomList(7588, 7601, 7394, 7395)); //Right Leg
                    break;
                case 2:
                    m_MandatoryParts.Add(Utility.RandomList(7391, 7396, 7399, 7403)); //Legs
                break;
            }

            m_MandatoryParts.Add(Utility.RandomList(7585, 7602, 7389, 7397)); //Left Arm
            m_MandatoryParts.Add(Utility.RandomList(7600, 7601, 7389, 7397)); //Right Arm

            m_MandatoryParts.Add(Utility.RandomList(7408)); //Brain
            m_MandatoryParts.Add(Utility.RandomList(7405)); //Heart
            m_MandatoryParts.Add(Utility.RandomList(7406)); //Heart
            m_MandatoryParts.Add(Utility.RandomList(6933, 6934)); //Pelvis
            m_MandatoryParts.Add(Utility.RandomList(6935, 6936)); //Ribcage
            m_MandatoryParts.Add(Utility.RandomList(6939, 6940)); //Spine

            m_ExtraParts.Add(Utility.RandomList(7407)); //Entrail
            m_ExtraParts.Add(Utility.RandomList(6929)); //Bones
            m_ExtraParts.Add(Utility.RandomList(6930)); //Bones
            m_ExtraParts.Add(Utility.RandomList(6937)); //Bones
            m_ExtraParts.Add(Utility.RandomList(6938)); //Bones
            m_ExtraParts.Add(Utility.RandomList(6931)); //Bones
            m_ExtraParts.Add(Utility.RandomList(6932)); //Bones

            m_ExtraParts.Add(Utility.RandomList(4650)); //Blood
            m_ExtraParts.Add(Utility.RandomList(4651)); //Blood
            m_ExtraParts.Add(Utility.RandomList(4652)); //Blood
            m_ExtraParts.Add(Utility.RandomList(4653)); //Blood
            m_ExtraParts.Add(Utility.RandomList(4654)); //Blood
            m_ExtraParts.Add(Utility.RandomList(5701)); //Blood
            m_ExtraParts.Add(Utility.RandomList(4655)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7439)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7438)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7436)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7433)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7431)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7428)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7425)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7410)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7415)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7416)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7418)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7420)); //Blood
            m_ExtraParts.Add(Utility.RandomList(7425)); //Blood

            #endregion

            double mandatoryPartChance = .33;
            double extraPartChance = .5;

            int radius = 2;
            int explosionSound = Utility.RandomList(0x4F1, 0x5D8, 0x5DA, 0x580);

            if (carnage && violentDeath)
            {
                mandatoryPartChance = .5;
                extraPartChance = .66;
                radius = 3;
            }

            Effects.PlaySound(location, map, explosionSound);

            int minRange = radius * -1;
            int maxRange = radius;

            List<Point3D> m_ExplosionPoints = new List<Point3D>();

            //Mandatory Parts
            for (int a = minRange; a < maxRange + 1; a++)
            {
                for (int b = minRange; b < maxRange + 1; b++)
                {
                    Point3D newPoint = new Point3D(location.X + a, location.Y + b, location.Z);
                    SpellHelper.AdjustField(ref newPoint, map, 12, false);

                    if (map.InLOS(location, newPoint))
                        m_ExplosionPoints.Add(newPoint);
                }
            }

            //Mandatory Parts
            for (int a = 0; a < m_ExplosionPoints.Count; a++)
            {
                if (Utility.RandomDouble() <= mandatoryPartChance)
                {
                    Point3D explosionPoint = m_ExplosionPoints[a];
                                        
                    int itemId = 0;                    

                    int distance = Utility.GetDistance(location, explosionPoint);

                    if (m_MandatoryParts.Count > 0)
                    {
                        int index = Utility.RandomMinMax(0, m_MandatoryParts.Count - 1);
                        
                        itemId = m_MandatoryParts[index];

                        m_MandatoryParts.RemoveAt(index);
                    }

                    else                    
                        itemId = m_ExtraParts[Utility.RandomMinMax(0, m_ExtraParts.Count - 1)];

                    Timer.DelayCall(TimeSpan.FromSeconds(distance * .15), delegate
                    {
                        TimedStatic gore = new TimedStatic(itemId, 10);
                        gore.Name = "gore";
                        gore.MoveToWorld(explosionPoint, map);

                    });
                }
            }

            //Extra Parts
            for (int a = 0; a < m_ExplosionPoints.Count; a++)
            {
                if (Utility.RandomDouble() <= extraPartChance)
                {
                    Point3D explosionPoint = m_ExplosionPoints[a];

                    int itemId = m_ExtraParts[Utility.RandomMinMax(0, m_ExtraParts.Count - 1)];

                    int distance = Utility.GetDistance(location, explosionPoint);

                    Timer.DelayCall(TimeSpan.FromSeconds(distance * .15), delegate
                    {
                        TimedStatic gore = new TimedStatic(itemId, 10);
                        gore.Name = "gore";
                        gore.MoveToWorld(explosionPoint, map);

                    });
                }
            }
        }

        public static void Artisan(Mobile from, Point3D location, Map map)
        {
            from.PlaySound(0x5AA);

            Point3D effectPoint = new Point3D(location.X, location.Y, location.Z + 10);
            
            TimedStatic flame = new TimedStatic(6571, 1.5);
            flame.Hue = 2498;
            flame.Name = "flame of the artisan";
            flame.MoveToWorld(effectPoint, map);

            TimedStatic hammer = new TimedStatic(9583, 1.5);
            hammer.Hue = 2498;
            hammer.Name = "hammer of the artisan";
            hammer.MoveToWorld(effectPoint, map);
        }

        public static void DrunkardThrowBottle(Mobile from)
        {
            if (from == null)
                return;

            Point3D location = from.Location;
            Map map = from.Map;

            int throwSound = 0x5D3;
            int itemId = Utility.RandomList(2459, 2463, 2503);
            int itemHue = 0;
            int hitSound = Utility.RandomList(0x38D, 0x38E, 0x38F, 0x390);

            bool foundLocation = false;
            Point3D targetLocation = new Point3D();

            List<Point3D> m_ValidLocations = SpecialAbilities.GetSpawnableTiles(from.Location, true, false, from.Location, from.Map, 1, 25, 2, 8, true);

            if (m_ValidLocations.Count > 0)
            {
                targetLocation = m_ValidLocations[Utility.RandomMinMax(0, m_ValidLocations.Count - 1)];
                foundLocation = true;

                from.Direction = from.GetDirectionTo(targetLocation);

                Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                {
                    if (from == null) return;
                    if (!from.Alive) return;

                    from.Animate(31, 7, 1, true, false, 0); //Throw Bottle
                    from.PublicOverheadMessage(MessageType.Regular, 0, false, "*drunkenly throws bottle*");
                });
            }
            
            if (!foundLocation)
            {
                from.PublicOverheadMessage(MessageType.Regular, 0, false, "*drops bottle*");

                TimedStatic bottle = new TimedStatic(Utility.RandomList(0x38D, 0x38E, 0x38F, 0x390), 1);
                bottle.Name = "a drunkard's bottle";
                bottle.MoveToWorld(from.Location, from.Map);

                Effects.PlaySound(targetLocation, map, hitSound);

                return;
            }

            Timer.DelayCall(TimeSpan.FromSeconds(.75), delegate
            {
                if (from == null) return;
                if (!from.Alive) return;    

                if (foundLocation)
                {
                    Effects.PlaySound(from.Location, from.Map, throwSound);

                    IEntity startLocation = new Entity(Serial.Zero, new Point3D(from.Location.X, from.Location.Y, from.Location.Z + 10), from.Map);
                    IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z + 5), from.Map);

                    Direction direction = from.GetDirectionTo(targetLocation);

                    double speedModifier = .66;

                    Effects.SendMovingEffect(startLocation, endLocation, itemId, (int)(15 * speedModifier), 0, true, false, itemHue, 0);

                    double distance = from.GetDistanceToSqrt(targetLocation);
                    double destinationDelay = (double)distance * .08 * (.5 / speedModifier);

                    Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                    {
                        Effects.PlaySound(targetLocation, map, hitSound);
                    });
                }
            });
        }

        public static void DrunkardVomit(Mobile from)
        {
            if (from == null)
                return;

            from.Animate(33, 5, 1, true, false, 0); //Touch Stomach
            from.PublicOverheadMessage(MessageType.Regular, 0, false, "*looks quite ill*");
            
            if (from.Female)
                from.PlaySound(Utility.RandomList(0x30E, 0x310, 0x311, 0x312, 0x32D)); 
            else
                from.PlaySound(Utility.RandomList(0x41D, 0x420, 0x421, 0x43F)); 

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (from == null) return;
                if (!from.Alive) return;

                from.PlaySound(0x5D8);

                Point3D newPoint = SpecialAbilities.GetPointByDirection(from.Location, from.Direction);
                SpellHelper.AdjustField(ref newPoint, from.Map, 12, false);

                TimedStatic vomit = new TimedStatic(Utility.RandomList(0x122A, 0x122B, 0x122C, 0x122D, 0x122E, 0x122F), 10);
                vomit.Name = "vomit";
                vomit.Hue = Utility.RandomList(2207, 2208, 2209, 2210, 2211, 2212);
                vomit.MoveToWorld(newPoint, from.Map);
            });
        }

        public static void Venomous(Mobile target)
        {
            if (target == null) return;
            if (!target.Alive) return;

            target.FixedEffect(0x372A, 10, 30, 2208, 0);
            Effects.PlaySound(target.Location, target.Map, 0x22F);

            int residueCount = Utility.RandomMinMax(2, 3);

            for (int a = 0; a < residueCount; a++)
            {
                Point3D poisonPoint = new Point3D(target.Location.X + Utility.RandomList(-1, 1), target.Location.Y + Utility.RandomList(-1, 1), target.Location.Z);
                SpellHelper.AdjustField(ref poisonPoint, target.Map, 12, false);

                Blood poisonResiude = new Blood();
                poisonResiude.Hue = 2208;
                poisonResiude.Name = "poison residue";
                poisonResiude.MoveToWorld(poisonPoint, target.Map);
            }
        }

        public static void Vanish(Mobile mobile)
        {
            if (mobile == null) return;

            mobile.PlaySound(0x657);

            int projectiles = 6;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(mobile.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), mobile.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), mobile.Z);
                SpellHelper.AdjustField(ref newLocation, mobile.Map, 12, false);

                IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(mobile.X, mobile.Y, mobile.Z + 5), mobile.Map);
                IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 5), mobile.Map);

                Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(0x3728), particleSpeed, 0, false, false, 0, 0);
            } 
        }

        public static void Reborn(Mobile mobile)
        {
            if (mobile == null) return;

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X - 1, mobile.Location.Y - 1, mobile.Location.Z + 50), mobile.Map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(mobile.Location.X, mobile.Location.Y, mobile.Location.Z), mobile.Map);

            Effects.SendMovingParticles(startLocation, endLocation, 0x373A, 4, 0, false, false, 2498, 0, 9501, 0, 0, 0);
            Effects.PlaySound(mobile.Location, mobile.Map, 0x5C9);

            mobile.Animate(22, 6, 1, false, false, 0); //Rise Up
        }

        public static void Tremor(Mobile mobile, int range)
        {
            if (mobile == null) return;

            int minRange = range * -1;
            int maxRange = range;

            List<Item> m_Items = new List<Item>();

            for (int a = minRange; a < maxRange + 1; a++)
            {
                for (int b = minRange; b < maxRange + 1; b++)
                {
                    if (Utility.RandomDouble() <= .15)
                    {
                        Point3D newPoint = new Point3D(mobile.X + a, mobile.Y + b, mobile.Z);
                        SpellHelper.AdjustField(ref newPoint, mobile.Map, 12, false);

                        newPoint.Z -= 10;

                        int itemId = Utility.RandomList(6001, 6003, 6004, 6005, 6006, 6007, 6008, 6009, 6001, 6012, 7681, 7682, 7677, 7679);

                        TimedStatic timedStatic = new TimedStatic(itemId, 3);
                        timedStatic.Hue = 2415;
                        timedStatic.Name = "rubble";
                        timedStatic.MoveToWorld(newPoint, mobile.Map);

                        m_Items.Add(timedStatic);
                    }

                    else if (Utility.RandomDouble() <= .15)
                    {
                        Point3D newPoint = new Point3D(mobile.X + a, mobile.Y + b, mobile.Z);
                        SpellHelper.AdjustField(ref newPoint, mobile.Map, 12, false);

                        newPoint.Z -= 10;

                        int itemId = Utility.RandomList(7681, 7682, 7677, 7679);

                        TimedStatic timedStatic = new TimedStatic(itemId, 5);
                        timedStatic.Hue = 2415;
                        timedStatic.Name = "dirt";
                        timedStatic.MoveToWorld(newPoint, mobile.Map);

                        m_Items.Add(timedStatic);
                    }
                }               
            }

            for (int a = 0; a < 10; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .05), delegate
                {
                    int itemCount = m_Items.Count;
                    
                    for (int b = 0; b < itemCount; b++)
                    {
                        if (m_Items.Count > b)
                        {
                            Item item = m_Items[b];

                            if (item != null)
                            {
                                if (!item.Deleted)
                                    item.Z++;
                            }                            
                        }
                    }
                });
            }
        }

        public static void Shielded(Mobile mobile)
        {
            if (mobile == null) return;

            mobile.PlaySound(0x64B); //0x456
            mobile.FixedEffect(0x37B9, 10, 5);

            int projectiles = 4;

            for (int a = 0; a < projectiles; a++)
            {
                int spellHue = PlayerEnhancementPersistance.GetSpellHueFor(mobile, HueableSpell.MagicReflect);

                Point3D newLocation = new Point3D(mobile.X + Utility.RandomList(-1, 1), mobile.Y + Utility.RandomList(-1, 1), mobile.Z);
                SpellHelper.AdjustField(ref newLocation, mobile.Map, 12, false);
                
                TimedStatic timedStatic = new TimedStatic(0x3779, .5);

                if (spellHue == 0)
                    timedStatic.Hue = Utility.RandomList(2587, 2570, 2576, 2623, 2572, 2615, 2515, 2505, 2499, 2500, 2117, 2618);

                else
                    timedStatic.Hue = spellHue;

                timedStatic.Name = "dissipated energy";
                timedStatic.MoveToWorld(newLocation, mobile.Map);
            }
        }

        public static void CustomerLoyalty(Mobile mobile)
        {
            if (mobile == null) return;

            switch(Utility.RandomMinMax(1, 2))
            {
                case 1:
                    mobile.Animate(32, 5, 1, true, false, 0); //Bow
                break;

                case 2:
                    mobile.Animate(33, 5, 1, true, false, 0); //Salute
                break;
            }
        }

        public static void SmoothSailing(Mobile mobile)
        {
            if (mobile == null) return;

            int oceanSound = 0;

            if (Utility.RandomDouble() < .025)
            {
                switch (Utility.RandomMinMax(1, 12))
                {
                    case 1: oceanSound = 0x010; break;
                    case 2: oceanSound = 0x011; break;
                    case 3: oceanSound = 0x026; break;
                    case 4: oceanSound = 0x025; break;

                    case 5: oceanSound = 0x012; break;
                    case 6: oceanSound = 0x012; break;
                    case 7: oceanSound = 0x012; break;
                    case 8: oceanSound = 0x012; break;
                    case 9: oceanSound = 0x013; break;
                    case 10: oceanSound = 0x013; break;
                    case 11: oceanSound = 0x013; break;
                    case 12: oceanSound = 0x013; break;
                }

                Effects.PlaySound(mobile.Location, mobile.Map, oceanSound); 
            }            
        }
    }     
}