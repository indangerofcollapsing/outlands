using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class PrevaliaBankAndShipwrightCaptureLocation : FactionCaptureLocation
    {
        public override string CaptureLocationName { get { return "Prevalia Bank and Shipwright"; } }

        public override CaptureEventLocationType LocationType { get { return CaptureEventLocationType.Town; } }

        public override Point3D CaptureLocation1 { get { return new Point3D(1000, 1000, 0); } }
        public override Point3D CaptureLocation2 { get { return new Point3D(1250, 1250, 0); } }

        public PrevaliaBankAndShipwrightCaptureLocation()
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