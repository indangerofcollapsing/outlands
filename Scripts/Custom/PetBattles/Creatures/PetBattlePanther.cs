using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a panther corpse" )]	
    public class PetBattlePanther : BaseCreature
	{
		[Constructable]
		public PetBattlePanther() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "Panther";
			Body = 0xD6;
			Hue = 0x901;
			BaseSoundID = 0x462;

            //Pet Battle Info            
            RangePerception = 25;
                        
            PetBattleCreature = true;
            PetBattleItemId = 8450;
            PetBattleItemHue = 0;
            PetBattleStatueOffsetZ = 22;

            PetBattleCreatureSelectItemIdOffsetX = 10;
            PetBattleCreatureSelectItemIdOffsetY = 0;

            PetBattleGrimoireItemIdOffsetX = -10;
            PetBattleGrimoireItemIdOffsetY = 0;

            PetBattleTitle = "Panther";
            PetBattleBriefDescription = "Slashing, elusive predator";
            PetBattleDescription = "Panthers are fast, aggressive predators, relying on inflict bleeding attacks and rapid strikes to quickly dispatch their foes";
            
            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(3);
            SetPetBattleCreatureDamageLevel(4);
            SetPetBattleCreatureAttackLevel(5);
            SetPetBattleCreatureDefendLevel(2);
            SetPetBattleCreatureSpeedLevel(5);
            SetPetBattleCreatureArmorLevel(1);
            SetPetBattleCreatureResistLevel(3);
            NoKillAwards = true;
		}

        public override void SetPetBattleOffensiveAbilities()
        {
            m_PetBattleOffensiveAbilities.Add(new ShredAbility(this));
            m_PetBattleOffensiveAbilities.Add(new GrowlAbility(this));
            m_PetBattleOffensiveAbilities.Add(new HamstringAbility(this));
            m_PetBattleOffensiveAbilities.Add(new RavageAbility(this));
            m_PetBattleOffensiveAbilities.Add(new FerocityAbility(this));
        }

        public override void SetPetBattleDefensiveAbilities()
        {
            m_PetBattleDefensiveAbilities.Add(new DodgeAbility(this));
            m_PetBattleDefensiveAbilities.Add(new ToughnessAbility(this));
            m_PetBattleDefensiveAbilities.Add(new CautionAbility(this));
            m_PetBattleDefensiveAbilities.Add(new EvasionAbility(this));
            m_PetBattleDefensiveAbilities.Add(new SurvivalAbility(this));
        }

        public override void OnDeath(Container c)
        {            
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; }}

		//public override double MaxSkillScrollWorth { get { return 52.0; } }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 5; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

        public PetBattlePanther(Serial serial): base(serial)
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