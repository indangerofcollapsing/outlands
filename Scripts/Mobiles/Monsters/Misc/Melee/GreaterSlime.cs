using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a slimey corpse" )]
	public class GreaterSlime : BaseCreature
	{
		[Constructable]
		public GreaterSlime () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "an angry slime";
			Body = 51;
			BaseSoundID = 0;
            Hue = 472;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Poisoning, 15);

            VirtualArmor = 25;

            Fame = 1000;
            Karma = -1000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.25;
        }

        public override Poison PoisonImmune{ get{ return Poison.Regular; } }
        public override Poison HitPoison { get { return Poison.Regular; } }

        public override bool OnBeforeDeath()
        {
            if (this.Map != null && this.Combatant != null)
            {
                Map map = this.Map;

                if (map == null)
                    return false;                
                for (int k = 0; k < 3; ++k)
                {
                    BaseCreature spawn = new PoisonousSlime();
                    spawn.Team = this.Team;
                    bool validLocation = false;
                    Point3D loc = this.Location;                
                    for (int j = 0; !validLocation && j < 10; ++j)
                    {
                        int x = X + Utility.Random(3) - 1;
                        int y = Y + Utility.Random(3) - 1;
                        int z = map.GetAverageZ(x, y);

                        if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
                            loc = new Point3D(x, y, Z);
                        else if (validLocation = map.CanFit(x, y, z, 16, false, false))
                            loc = new Point3D(x, y, z);
                    }
                    this.PlaySound( Utility.RandomMinMax( 457, 459 ) );
                    spawn.MoveToWorld(loc, map);
                    spawn.Combatant = this.Combatant;
                }
            }
            this.Delete();
            return base.OnBeforeDeath();
        }

		public GreaterSlime( Serial serial ) : base( serial )
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