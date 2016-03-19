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
    public class ShipEpicAbilities
    {
        public static void HellfireAmmunition(Mobile from, BaseBoat boat, Custom.Pirates.BaseCannon cannon)
        {
            if (boat == null || cannon == null)
                return;

            int range = 6;

            Direction facing = cannon.Facing;

            Point3D nextPoint = SpecialAbilities.GetPointByDirection(cannon.Location, facing);
            Map map = cannon.Map;

            Point3D smokeLocation = cannon.Location;
            Point3D targetLocation = cannon.Location;

            switch (cannon.m_facing)
            {
                case Direction.North:
                {
                    targetLocation.Y -= range;
                }

                break;

                case Direction.East:
                    {
                        smokeLocation.X++;
                        targetLocation.X += range;
                    } 
                break;

                case Direction.South:
                    { 
                        smokeLocation.Y++;
                        targetLocation.Y += range;
                    }
                break;

                case Direction.West:
                    { 
                        smokeLocation.X--;
                        targetLocation.X -= range;
                    }
                break;
            }

            int cannonballItemID = 6251;
            int cannonballHue = 0;
            int smokeHue = 0;

            Effects.PlaySound(cannon.Location, cannon.Map, 0x664);
            Effects.SendLocationEffect(smokeLocation, cannon.Map, 0x36CB, 10, smokeHue, 0);
            
            double distance = Utility.GetDistanceToSqrt(cannon.Location, targetLocation);

            IEntity startLocation = new Entity(Serial.Zero, new Point3D(cannon.Location.X, cannon.Location.Y, cannon.Location.Z + 10), map);
            IEntity endLocation = new Entity(Serial.Zero, new Point3D(targetLocation.X, targetLocation.Y, targetLocation.Z), map);

            Effects.SendMovingEffect(startLocation, endLocation, cannonballItemID, 8, 0, false, false, cannonballHue, 0);
            double effectDelay = distance * .04;           

            Timer.DelayCall(TimeSpan.FromSeconds(effectDelay), delegate
            {
                Custom.Pirates.BaseCannon.Splash(targetLocation, map);
            }); 

            for (int a = 0; a < range; a++)
            {
                Timer.DelayCall(TimeSpan.FromSeconds(a * .075), delegate
                {
                    Effects.PlaySound(nextPoint, map, 0x5CF);
                    Effects.SendLocationParticles(EffectItem.Create(nextPoint, map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, 2613, 0, 5029, 0);
                    
                    if (Utility.RandomDouble() <= .33)
                    {
                        SingleFireField singleFireField = new SingleFireField(from, 2613, 1, 10, 4, 8, false, true, true, -1, true);
                        singleFireField.MoveToWorld(nextPoint, map);                       
                    }

                    Queue m_Queue = new Queue();
                    IPooledEnumerable m_MobilesOnTile = map.GetMobilesInRange(nextPoint, 0);

                    foreach (Mobile mobile in m_MobilesOnTile)
                    {
                        if (mobile.CanBeDamaged())
                            m_Queue.Enqueue(mobile);
                    }

                    m_MobilesOnTile.Free();

                    while (m_Queue.Count > 0)
                    {
                        Mobile mobile = (Mobile)m_Queue.Dequeue();

                        int damage = Utility.RandomMinMax(15, 25);

                        if (mobile is BaseCreature)
                            damage *= 2;

                        new Blood().MoveToWorld(mobile.Location, mobile.Map);
                        AOS.Damage(mobile, from, damage, 0, 100, 0, 0, 0);
                    }

                    nextPoint = SpecialAbilities.GetPointByDirection(nextPoint, facing);
                });
            }
        }
    }     
}
