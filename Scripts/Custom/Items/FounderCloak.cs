using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Items
{
    class FounderCloak : Cloak
    {
        public override string DefaultName { get { return "Founder's Cloak"; } }

        public DateTime LastUsage;

        private static TimeSpan m_CoolDown = TimeSpan.FromMinutes(10);

        [Constructable]
        public FounderCloak()
            : base()
        {
            Hue = 1772;
            LootType = Server.LootType.Blessed;
            LastUsage = DateTime.MinValue;
        }

        public FounderCloak(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(LastUsage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            LastUsage = reader.ReadDateTime();
            LootType = Server.LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.FindItemOnLayer(Server.Layer.Cloak) != this)
                from.SendMessage("You must be wearing that to use it!");
            else if (DateTime.UtcNow > LastUsage + m_CoolDown)
                BeginLaunch(from);
            else
                from.SendMessage("You cannot use that again yet.");
        }

        public void BeginLaunch(Mobile from)
        {
            Map map = from.Map;

            if (map == null || map == Map.Internal)
                return;


            for(int i = 0; i < 30; i++)
            {
                Point3D ourLoc = GetWorldLocation();

                Point3D startLoc = new Point3D(ourLoc.X, ourLoc.Y, ourLoc.Z + 10);
                Point3D endLoc = new Point3D(startLoc.X + Utility.RandomMinMax(-2, 2), startLoc.Y + Utility.RandomMinMax(-2, 2), startLoc.Z + 32);

                Effects.SendMovingEffect(new Entity(Serial.Zero, startLoc, map), new Entity(Serial.Zero, endLoc, map),
                    0x36E4, 5, 0, false, false, Hue, 0);

                Timer.DelayCall(TimeSpan.FromSeconds(1.0), new TimerStateCallback(FinishLaunch), new object[] { from, endLoc, map });
            }
            LastUsage = DateTime.UtcNow;
        }

        private void FinishLaunch(object state)
        {
            object[] states = (object[])state;

            Mobile from = (Mobile)states[0];
            Point3D endLoc = (Point3D)states[1];
            Map map = (Map)states[2];

            int hue = Utility.Random(40);

            if (Hue > 0)
                hue = Hue;
            else if (hue < 8)
                hue = 0x66D;
            else if (hue < 10)
                hue = 0x482;
            else if (hue < 12)
                hue = 0x47E;
            else if (hue < 16)
                hue = 0x480;
            else if (hue < 20)
                hue = 0x47F;
            else
                hue = 0;

            if (Utility.RandomBool())
                hue = Utility.RandomList(0x47E, 0x47F, 0x480, 0x482, 0x66D);

            int renderMode = Utility.RandomList(0, 2, 3, 4, 5, 7);

            Effects.PlaySound(endLoc, map, Utility.Random(0x11B, 4));
            Effects.SendLocationEffect(endLoc, map, 0x373A + (0x10 * Utility.Random(4)), 16, 10, hue, renderMode);
        }
    }
}
