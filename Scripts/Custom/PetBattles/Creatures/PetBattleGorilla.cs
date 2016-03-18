using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gorilla corpse" )]
	public class PetBattleGorilla : BaseCreature
	{
		[Constructable]
		public PetBattleGorilla() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "Gorilla";
			Body = 0x1D;
			BaseSoundID = 0x9E;

            //Pet Battle Info            
            RangePerception = 25;

            PetBattleCreature = true;
            PetBattleItemId = 8437;
            PetBattleItemHue = 0;
            PetBattleStatueOffsetZ = 25;

            PetBattleCreatureSelectItemIdOffsetX = 25;
            PetBattleCreatureSelectItemIdOffsetY = 0;

            PetBattleGrimoireItemIdOffsetX = 5;
            PetBattleGrimoireItemIdOffsetY = 0;

            PetBattleTitle = "Gorilla";
            PetBattleBriefDescription = "Fierce brawler";
            PetBattleDescription = "Gorillas are fierce beasts capable of both dishing out extroardinary punishment and are tough enough to stand toe-to-toe with any foe";
            
            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(5);
            SetPetBattleCreatureDamageLevel(6);
            SetPetBattleCreatureAttackLevel(3);
            SetPetBattleCreatureDefendLevel(1);
            SetPetBattleCreatureSpeedLevel(3);
            SetPetBattleCreatureArmorLevel(2);
            SetPetBattleCreatureResistLevel(1);
            NoKillAwards = true;
        }

        public override void SetPetBattleOffensiveAbilities()
        {
            m_PetBattleOffensiveAbilities.Add(new SlamAbility(this));
            m_PetBattleOffensiveAbilities.Add(new GrappleAbility(this));
            m_PetBattleOffensiveAbilities.Add(new KnockdownAbility(this));
            m_PetBattleOffensiveAbilities.Add(new FuryAbility(this));
            m_PetBattleOffensiveAbilities.Add(new CrushAbility(this));
        }

        public override void SetPetBattleDefensiveAbilities()
        {
            m_PetBattleDefensiveAbilities.Add(new GritAbility(this));
            m_PetBattleDefensiveAbilities.Add(new ToughnessAbility(this));
            m_PetBattleDefensiveAbilities.Add(new StaredownAbility(this));
            m_PetBattleDefensiveAbilities.Add(new IgnorePainAbility(this));
            m_PetBattleDefensiveAbilities.Add(new RageAbility(this));
        }

        public override void OnDeath(Container c)
        {            
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; } }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 6; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

        public PetBattleGorilla(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}