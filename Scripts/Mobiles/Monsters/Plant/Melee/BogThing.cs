using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a plant corpse" )]
	public class BogThing : BaseCreature
	{
		[Constructable]
		public BogThing() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.6, 1.2 )
		{
			Name = "a bog thing";
			Body = 780;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(1500);

            SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

			Fame = 8000;
			Karma = -8000;
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.25;
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Hits > (HitsMax / 4))
            {
                if (0.25 >= Utility.RandomDouble())
                    SpawnBogling(attacker);
            }

            else if (0.25 >= Utility.RandomDouble())
            {
                EatBoglings();
            }
        }

        public void SpawnBogling(Mobile m)
        {
            Map map = this.Map;

            if (map == null)
                return;

            Bogling spawned = new Bogling();

            spawned.Team = this.Team;

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

            spawned.MoveToWorld(loc, map);
            spawned.Combatant = m;
        }

        public void EatBoglings()
        {
            ArrayList toEat = new ArrayList();

            IPooledEnumerable eable = this.GetMobilesInRange(2);

            foreach (Mobile m in eable)
            {
                if (m is Bogling)
                    toEat.Add(m);
            }

            eable.Free();

            if (toEat.Count > 0)
            {
                PlaySound(Utility.Random(0x3B, 2)); // Eat sound

                foreach (Mobile m in toEat)
                {
                    Hits += (m.Hits / 2);
                    m.Delete();
                }
            }
        }	

		public BogThing( Serial serial ) : base( serial )
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
