using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a desert ostard corpse" )]
	public class PetBattleDesertOstard : BaseMount
	{
		[Constructable]
		public PetBattleDesertOstard() : this( "a desert ostard" )
		{
		}

		[Constructable]
		public PetBattleDesertOstard( string name ) : base( name, 0xD2, 0x3EA3, AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
            Name = "Desert Ostard";
            BaseSoundID = 0x270;

            //Pet Battle Info            
            RangePerception = 25;

            PetBattleCreature = true;
            PetBattleItemId = 8501;
            PetBattleItemHue = 0;
            PetBattleStatueOffsetZ = 25;

            PetBattleCreatureSelectItemIdOffsetX = 15;
            PetBattleCreatureSelectItemIdOffsetY = 0;

            PetBattleGrimoireItemIdOffsetX = 0;
            PetBattleGrimoireItemIdOffsetY = 0;

            PetBattleTitle = "Desert Ostard";
            PetBattleBriefDescription = "Elusive, quick striker";
            PetBattleDescription = "Desert Ostards are fast-striking beasts, capable of dodging attacks and returning them rapidly in kind.";

            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(3);
            SetPetBattleCreatureDamageLevel(2);
            SetPetBattleCreatureAttackLevel(1);
            SetPetBattleCreatureDefendLevel(6);
            SetPetBattleCreatureSpeedLevel(6);
            SetPetBattleCreatureArmorLevel(2);
            SetPetBattleCreatureResistLevel(4);
            NoKillAwards = true;
        }

        public override void SetPetBattleOffensiveAbilities()
        {
            m_PetBattleOffensiveAbilities.Add(new RendAbility(this));
            m_PetBattleOffensiveAbilities.Add(new QuickenAbility(this));
            m_PetBattleOffensiveAbilities.Add(new FrenzyAbility(this));
            m_PetBattleOffensiveAbilities.Add(new PrecisionAbility(this));
            m_PetBattleOffensiveAbilities.Add(new BerserkAbility(this));
        }

        public override void SetPetBattleDefensiveAbilities()
        {
            m_PetBattleDefensiveAbilities.Add(new DodgeAbility(this));
            m_PetBattleDefensiveAbilities.Add(new CautionAbility(this));
            m_PetBattleDefensiveAbilities.Add(new EvasionAbility(this));
            m_PetBattleDefensiveAbilities.Add(new CleanseAbility(this));
            m_PetBattleDefensiveAbilities.Add(new CounterstrikesAbility(this));
        }

        public override void OnDeath(Container c)
        {            
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; } }

		public override int Meat{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Ostard; } }

        public PetBattleDesertOstard(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}