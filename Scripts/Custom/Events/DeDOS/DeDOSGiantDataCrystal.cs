using System;
using Server;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    public class DeDOSGiantDataCrystal : DeDOSDataCrystal
    {
        [Constructable]
        public DeDOSGiantDataCrystal(): base()
        {
            Name = "a giant DeDOS data crystal";
            Hue = 0;

            ItemID = 12244;
        }

        public override void VisualEffect(bool send)
        {
            int projectiles = 6;
            int particleSpeed = 4;

            for (int a = 0; a < projectiles; a++)
            {
                Point3D newLocation = new Point3D(Location.X + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Y + Utility.RandomList(-5, -4, -3, -2, -1, 1, 2, 3, 4, 5), Location.Z);
                SpellHelper.AdjustField(ref newLocation, Map, 12, false);

                IEntity effectStartLocation;
                IEntity effectEndLocation;

                if (send)
                {
                    effectStartLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 20), Map);
                    effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 20), Map);
                }

                else
                {
                    effectStartLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + 20), Map);
                    effectEndLocation = new Entity(Serial.Zero, new Point3D(Location.X, Location.Y, Location.Z + 20), Map);
                }

                Effects.SendMovingParticles(effectStartLocation, effectEndLocation, 0x3818, particleSpeed, 0, false, false, 2603, 0, 9501, 0, 0, 0x100);
            }
        }

        public DeDOSGiantDataCrystal(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}