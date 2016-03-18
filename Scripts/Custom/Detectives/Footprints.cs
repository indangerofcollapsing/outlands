/***************************************************************************
 *                              Detective.cs
 *                            ------------------
 *   begin                : February 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;


namespace Server.Custom
{
    public class DetectiveFootprints : Item
    {
        private Direction _dir;

        [Constructable]
        public DetectiveFootprints()
            : this(Direction.North)
        {
        }

        public DetectiveFootprints(Direction d) : base( 0 )
		{
            Movable = false;
            _dir = d & Direction.Mask;
            Hue = 32;

            switch (d & Direction.Mask)
            {
                case Direction.Right:   ItemID = 0x1E03; break;
                case Direction.West:    ItemID = 0x1E03; break;

                case Direction.Up:      ItemID = 0x1E04; break;
                case Direction.North:   ItemID = 0x1E04; break;

                case Direction.Left:    ItemID = 0x1E05; break;
                case Direction.East:    ItemID = 0x1E05; break;

                case Direction.Down:    ItemID = 0x1E06; break;
                case Direction.South:   ItemID = 0x1E06; break;
            }


            Timer.DelayCall(TimeSpan.FromSeconds(60), delegate { Delete(); });
		}

        public DetectiveFootprints(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            string direction = _dir.ToString();

            if (direction == "Mask")
                direction = "Up";

            LabelTo(from, String.Format("the footprints lead {0}", direction));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version  = reader.ReadInt();

            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { Delete(); });
        }
    }
}
