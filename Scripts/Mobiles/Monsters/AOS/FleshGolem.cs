using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a flesh golem corpse" )]
	public class FleshGolem : BaseCreature
	{
		[Constructable]
		public FleshGolem() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a flesh golem";
			Body = 304;
			BaseSoundID = 0x36B;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -1800;
		}

        protected override bool OnMove(Direction d)
        {           
            if (Utility.RandomDouble() < .33)
            {
                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

                Item corpsePart = new Blood();
                corpsePart.Name = "fleshy bits";
                corpsePart.ItemID = Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600);

                corpsePart.MoveToWorld(Location, Map);
            }            

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x580));
            new Blood().MoveToWorld(Location, Map);

            int corpseItems = 8;

            for (int a = 0; a < corpseItems; a++)
            {
                Point3D point = new Point3D(Location.X + Utility.RandomList(-2, -1, 1, 2), Location.Y + Utility.RandomList(-2, -1, 1, 2), Location.Z);

                Item corpsePart = new Blood();
                corpsePart.ItemID = Utility.RandomList(7389, 7397, 7395, 7402, 7408, 7407, 7393, 7405, 7394, 7406, 7586, 7600);
                corpsePart.Name = "fleshy bits";

                if (Utility.RandomDouble() < .33)
                    new Blood().MoveToWorld(point, Map);

                corpsePart.MoveToWorld(point, Map);
            }

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x36B; }
        public override int GetIdleSound() { return 0x2FA; }
        public override int GetAttackSound() { return 0x2F8; }
        public override int GetHurtSound() { return 0x2F9; }
        public override int GetDeathSound() { return 0x2F7; }

		public FleshGolem( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}