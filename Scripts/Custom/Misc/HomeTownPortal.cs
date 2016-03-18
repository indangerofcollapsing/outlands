using Server.Custom.Townsystem;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Spells;
using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Custom.Misc
{
    class HomeTownPortal : Teleporter
    {
        [Constructable]
        public HomeTownPortal()
            : base()
        {
        }

        public HomeTownPortal(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void DoTeleport(Mobile m)
        {
            if (m.Criminal)
            {
                m.SendLocalizedMessage(1005561, "", 0x22); // Thou'rt a criminal and cannot escape so easily.
            }
            else if (m.Spell != null)
            {
                m.SendLocalizedMessage(1049616); // You are too busy to do that at the moment.
            }
            // too many newbs complaining about this
            //else if (SpellHelper.CheckMilitiaCombat(m))
            //{
            //    m.SendLocalizedMessage(1005564, "", 0x22);
            //}
            else if (m.ShortTermMurders > 4)
            {
                Point3D bucs = new Point3D(2725, 2148, 0);
                teleport(m, bucs);
            }
            else if (Town.CheckCitizenship(m) is Cove)
            {
                Point3D cove = new Point3D(2257, 1201, 0);
                teleport(m, cove);
            }
            else if (Town.CheckCitizenship(m) is Jhelom)
            {
                Point3D jhelom = new Point3D(1341, 3763, 0);
                teleport(m, jhelom);
            }
            else if (Town.CheckCitizenship(m) is Minoc)
            {
                Point3D minoc = new Point3D(2501, 572, 0);
                teleport(m, minoc);
            }
            else if (Town.CheckCitizenship(m) is Magincia)
            {
                Point3D magincia = new Point3D(3708, 2140, 20);
                teleport(m, magincia);
            }
            else if (Town.CheckCitizenship(m) is Moonglow)
            {
                Point3D moonglow = new Point3D(4439, 1170, 0);
                teleport(m, moonglow);
            }
            else if (Town.CheckCitizenship(m) is Nujelm)
            {
                Point3D nujelm = new Point3D(3763, 1293, 0);
                teleport(m, nujelm);
            }
            else if (Town.CheckCitizenship(m) is Ocllo)
            {
                Point3D ocllo = new Point3D(3675, 2492, 0);
                teleport(m, ocllo);
            }
            else if (Town.CheckCitizenship(m) is SerpentsHold)
            {
                Point3D serps = new Point3D(3019, 3373, 15);
                teleport(m, serps);
            }
            else if (Town.CheckCitizenship(m) is SkaraBrae)
            {
                Point3D skara = new Point3D(573, 2135, 0);
                teleport(m, skara);
            }
            else if (Town.CheckCitizenship(m) is Vesper)
            {
                Point3D vesper = new Point3D(2889, 711, 0);
                teleport(m, vesper);
            }
            else if (Town.CheckCitizenship(m) is Trinsic)
            {
                Point3D trinsic = new Point3D(1824, 2817, 0);
                teleport(m, trinsic);
            }
            else if (Town.CheckCitizenship(m) is Yew)
            {
                Point3D yew = new Point3D(639, 878, 0);
                teleport(m, yew);
            }
            else
            {
                // default send to brit
                Point3D brit = new Point3D(1412, 1707, 30);
                teleport(m, brit);
            }
        }

        private void teleport(Mobile m, Point3D destination)
        {
            Server.Mobiles.BaseCreature.TeleportPets(m, destination, Map.Felucca);
            m.MoveToWorld(destination, Map.Felucca);
            //Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            //Effects.SendLocationEffect(m.Location, m.Map, 0x3728, 10, 10);

            //Effects.PlaySound(m.Location, m.Map, SoundID);
        }
    }
}
