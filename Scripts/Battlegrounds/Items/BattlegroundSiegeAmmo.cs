using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Custom.Battlegrounds.Items
{
    class BattlegroundSiegeCatapultShot : BattlegroundSiegeCannonball 
    {
        public override int AnimationID { get { return 0x36D4; } }

        public BattlegroundSiegeCatapultShot() : this(1)
        {

        }

        public BattlegroundSiegeCatapultShot(int amount)
            : base(amount)
        {
            Range = 10;
            Area = 2;
            Name = "Battleground Catapult Shot";
        }

        public BattlegroundSiegeCatapultShot(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    class BattlegroundSiegeCannonball : ExplodingCannonball
    {
        //public override int AnimationID { get { return 0x36D4; } } // 0x36D4 default
        public override int AnimationHue { get { return 0; } }
        public override double MobDamageMultiplier { get { return 0.3; } }

		public BattlegroundSiegeCannonball()
			: this(1)
		{
		}

		public BattlegroundSiegeCannonball(int amount)
			: base(amount)
		{
			Range = 10;
			Area = 1;
			AccuracyBonus = 0;
			PhysicalDamage = 100;
			FireDamage = 0;
			FiringSpeed = 20;
			Name = "Battleground Cannonball";
		}

        public BattlegroundSiegeCannonball(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
    }

    public class BattlegroundSiegeLog : SiegeLog
    {
        public override double MobDamageMultiplier { get { return 0.6; } }

        public BattlegroundSiegeLog()
            : this(1)
        {
        }

        public BattlegroundSiegeLog(int amount)
            : base(amount)
        {
            Range = 4;
            Area = 0;
            AccuracyBonus = 0;
            PhysicalDamage = 50;
            FireDamage = 0;
            FiringSpeed = 15;
            Name = "Battleground Siege Log";
        }

        public BattlegroundSiegeLog(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
