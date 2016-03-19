using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Regions;
using Server.Multis;

namespace Server.Custom
{
    public class CampingFirepit : Item
    {
        public int MaxCharges = 10;

        private int m_Charges = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }
        
        [Constructable]
        public CampingFirepit(): base(10756)
        {
            Name = "a camping firepit";
            Weight = 10;
        }

        public CampingFirepit(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);
            
            LabelTo(from, "[uses remaining: " + m_Charges + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");

                return;
            }

            from.SendMessage("Target the area you would like to set up a campsite at.");
            from.Target = new CampingFirePitTarget(this);

            return; 
        }

        public class CampingFirePitTarget : Target
        {
            private CampingFirepit m_CampingFirepit;
            private IEntity targetLocation;

            public CampingFirePitTarget(CampingFirepit campingFirepit): base(3, true, TargetFlags.None)
            {
                m_CampingFirepit = campingFirepit;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_CampingFirepit.Deleted || m_CampingFirepit.RootParent != from)
                    return;

                PlayerMobile pm = from as PlayerMobile;

                if (pm == null)
                    return;

                IPoint3D location = target as IPoint3D;

                if (location == null)
                    return;

                Map map = from.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref location);

                if (location is Mobile)
                    targetLocation = (Mobile)location;

                else
                    targetLocation = new Entity(Serial.Zero, new Point3D(location), map);

                if (!map.CanSpawnMobile(targetLocation.Location))
                {
                    from.SendLocalizedMessage(501942); // That location is blocked.
                    return;
                }
                                
                if (BaseBoat.IsWaterTile(targetLocation.Location, map))
                {
                    BaseBoat boatAtLocation = BaseBoat.FindBoatAt(targetLocation.Location, map);

                    if (boatAtLocation == null)
                    {
                        from.SendMessage("You may only place those on dry land.");
                        return;
                    }
                }

                Point3D newLocation = targetLocation.Location;
                SpellHelper.AdjustField(ref newLocation, map, 12, false);

                CampingFirepitPlaced firepitPlaced = new CampingFirepitPlaced();

                firepitPlaced.MoveToWorld(newLocation, map);
                firepitPlaced.Light(from);

                m_CampingFirepit.m_Charges--;

                if (m_CampingFirepit.m_Charges <= 0)                
                    m_CampingFirepit.Delete();                
            }
        }      
       
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            writer.Write(m_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();                              
            }
        }
    }
}
