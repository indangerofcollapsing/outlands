using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Misc;
using Server.Mobiles;
using Server.Items;
using Server.Multis;

namespace Server.Items
{
    public static class ShipCannons
    {
        public static void GenerateShipCannons(BaseBoat boat)
        {
            for (int a = 0; a < boat.m_CannonLocations().Count; a++)
            {
                Point3D cannonLocation = boat.m_CannonLocations()[a];

                Custom.Pirates.LightCannonDeed lightCannonDeed = new Custom.Pirates.LightCannonDeed();

                lightCannonDeed.Delete();

                Server.Custom.Pirates.BaseCannon cannon = lightCannonDeed.Addon;
                cannon.Visible = false;
                cannon.m_Boat = boat;

                cannon.xOffset = cannonLocation.X;
                cannon.yOffset = cannonLocation.Y;
                cannon.zOffset = cannonLocation.Z;

                cannonLocation = boat.GetRotatedLocation(cannonLocation.X, cannonLocation.Y, 0);

                cannon.MoveToWorld(new Point3D(boat.Location.X + cannonLocation.X, boat.Location.Y + cannonLocation.Y, boat.Location.Z + cannonLocation.Z), boat.Map);

                cannon.SetFacing(boat.Facing);

                cannon.Z = boat.Location.Z + cannonLocation.Z + boat.GetAdjustedCannonZOffset(cannon);

                cannon.Hue = boat.CannonHue;
                
                cannon.Visible = true;

                if (boat.MobileControlType != Multis.MobileControlType.Player)
                    cannon.CurrentCharges = Custom.Pirates.BaseCannon.MaxCharges;

                boat.AddCannon(cannon);
            }
        }
    }
}
