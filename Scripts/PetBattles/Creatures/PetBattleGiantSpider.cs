using System;
using Server.Mobiles;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a spider corpse" )]	
    public class PetBattleGiantSpider : BaseCreature
	{
		[Constructable]
		public PetBattleGiantSpider() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
            Name = "Giant Spider";
            Body = 28;
            BaseSoundID = 0x388;

            //Pet Battle Info            
            RangePerception = 25;

            PetBattleCreature = true;
            PetBattleItemId = 8445;
            PetBattleItemHue = 0;
            PetBattleStatueOffsetZ = 22;

            PetBattleCreatureSelectItemIdOffsetX = 0;
            PetBattleCreatureSelectItemIdOffsetY = 0;

            PetBattleGrimoireItemIdOffsetX = -20;
            PetBattleGrimoireItemIdOffsetY = 0;

            PetBattleTitle = "Giant Spider";
            PetBattleBriefDescription = "Poisonous, debilitating striker";
            PetBattleDescription = "Giant Spiders cripple their opponents with poison and debilitating attacks and are capable of resisting their opponent's as well.";

            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(2);
            SetPetBattleCreatureDamageLevel(2);
            SetPetBattleCreatureAttackLevel(6);
            SetPetBattleCreatureDefendLevel(5);
            SetPetBattleCreatureSpeedLevel(3);
            SetPetBattleCreatureArmorLevel(2);
            SetPetBattleCreatureResistLevel(7);
            NoKillAwards = true;
        }

        public override void SetPetBattleOffensiveAbilities()
        {
            m_PetBattleOffensiveAbilities.Add(new BiteAbility(this));
            m_PetBattleOffensiveAbilities.Add(new PoisonAbility(this));
            m_PetBattleOffensiveAbilities.Add(new WeakenAbility(this));
            m_PetBattleOffensiveAbilities.Add(new LeechAbility(this));
            m_PetBattleOffensiveAbilities.Add(new WebAbility(this));            
        }

        public override void SetPetBattleDefensiveAbilities()
        {
            m_PetBattleDefensiveAbilities.Add(new ResistanceAbility(this));
            m_PetBattleDefensiveAbilities.Add(new DodgeAbility(this));
            m_PetBattleDefensiveAbilities.Add(new CleanseAbility(this));
            m_PetBattleDefensiveAbilities.Add(new EvasionAbility(this));
            m_PetBattleDefensiveAbilities.Add(new ToxicityAbility(this));           
        }

        public override void OnDeath(Container c)
        {           
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; } }

		//public override double MaxSkillScrollWorth { get { return 52.0; } }

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 5; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

        public PetBattleGiantSpider(Serial serial)
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