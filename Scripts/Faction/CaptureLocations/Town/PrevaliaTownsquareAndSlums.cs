using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class PrevaliaTownsquareAndSlumsCaptureLocation : FactionCaptureLocation
    {
        public override string CaptureLocationName { get { return "Prevalia Townsquare and Slums"; } }

        public override CaptureEventLocationType LocationType { get { return CaptureEventLocationType.Town; } }

        public override Point3D CaptureLocation1 { get { return new Point3D(1500, 1500, 0); } }
        public override Point3D CaptureLocation2 { get { return new Point3D(1750, 1750, 0); } }

        public PrevaliaTownsquareAndSlumsCaptureLocation()
        {
        }

        public override void OnEventStart()
        {
        }

        public override void OnEventTick()
        {
        }

        public override void OnEventCompletetion()
        {
        }
    }
}