using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Multis.Deeds;

namespace Server.Multis
{
	public class PaladinKeep : BaseHouse 
	{
        public static Rectangle2D[] AreaArray = new Rectangle2D[] { new Rectangle2D(-13, -9, 12, 9) };
		public override Rectangle2D[] Area{ get{ return AreaArray; } }
		public override Point3D BaseBanLocation{ get{ return new Point3D( -1, 7, 7 ); } }
		public override int DefaultPrice{ get{ return 1200000; } }

		public PaladinKeep( Mobile owner ) : base( 0x14F, owner, 3000, 21 )
		{
            uint keyValue = CreateKeys(owner);
            SetSign(4, 6, 24);
            BaseHouseDoor door = (BaseHouseDoor)AddSouthDoor(false, -1, 3, 12, keyValue);
            this.ChangeSignType(0x0BD1);

		}

        public PaladinKeep(Serial serial)
            : base(serial)
		{
		}

		public override HouseDeed GetDeed() 
		{
            return new PaladinKeepDeed();
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
	}
}
