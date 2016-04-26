using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;

namespace Server
{
    public class FactionCaptureLocation
    {
        public enum CaptureEventLocationType
        {
            Town,
            Wilderness,
            Dungeon
        }

        public virtual string CaptureLocationName { get { return "Capture Location Event Name"; } }

        public virtual CaptureEventLocationType LocationType { get { return CaptureEventLocationType.Town; } }

        public virtual Point3D CaptureLocation1 { get { return new Point3D(1000, 1000, 0); } }
        public virtual Point3D CaptureLocation2 { get { return new Point3D(1250, 1250, 0); } }
        
        public FactionCaptureLocation()
        {
        }

        public virtual void OnEventStart()
        {
        }

        public virtual void OnEventTick()
        {
        }

        public virtual void OnEventCompletetion()
        {
        }
    }
}