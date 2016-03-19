using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a grizzly bear corpse" )]
	public class PetBattleGrizzlyBear : BaseCreature
	{
		[Constructable]
		public PetBattleGrizzlyBear() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "Grizzly Bear";
			Body = 212;
			BaseSoundID = 0xA3;

            //Pet Battle Info           
            RangePerception = 25;

            PetBattleCreature = true;
            PetBattleItemId = 8478;
            PetBattleItemHue = 0;
            PetBattleStatueOffsetZ = 25;

            PetBattleCreatureSelectItemIdOffsetX = 20;
            PetBattleCreatureSelectItemIdOffsetY = 5;

            PetBattleGrimoireItemIdOffsetX = 0;
            PetBattleGrimoireItemIdOffsetY = 5;

            PetBattleTitle = "Grizzly Bear";
            PetBattleBriefDescription = "Tough and intimidating predator";
            PetBattleDescription = "Grizzly Bears are resilient opponents, and rely on a variety of intimidating attacks to cause their enemies to cower before them.";
            
            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(6);
            SetPetBattleCreatureDamageLevel(5);
            SetPetBattleCreatureAttackLevel(2);
            SetPetBattleCreatureDefendLevel(2);
            SetPetBattleCreatureSpeedLevel(2);
            SetPetBattleCreatureArmorLevel(3);
            SetPetBattleCreatureResistLevel(1);
            NoKillAwards = true;
        }

        public override void SetPetBattleOffensiveAbilities()
        {
            m_PetBattleOffensiveAbilities.Add(new ShredAbility(this));
            m_PetBattleOffensiveAbilities.Add(new KnockdownAbility(this));
            m_PetBattleOffensiveAbilities.Add(new MaulAbility(this));
            m_PetBattleOffensiveAbilities.Add(new RavageAbility(this));
            m_PetBattleOffensiveAbilities.Add(new RoarAbility(this));
        }

        public override void SetPetBattleDefensiveAbilities()
        {
            m_PetBattleDefensiveAbilities.Add(new ToughnessAbility(this));
            m_PetBattleDefensiveAbilities.Add(new StaredownAbility(this));
            m_PetBattleDefensiveAbilities.Add(new IronskinAbility(this));
            m_PetBattleDefensiveAbilities.Add(new IgnorePainAbility(this));
            m_PetBattleDefensiveAbilities.Add(new IntimidateAbility(this));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; } }

		//public override double MaxSkillScrollWorth { get { return 58.0; } }
		public override int Meat{ get{ return 2; } }
		public override int Hides{ get{ return 16; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish | FoodType.FruitsAndVegies | FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Bear; } }

        public PetBattleGrizzlyBear(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}