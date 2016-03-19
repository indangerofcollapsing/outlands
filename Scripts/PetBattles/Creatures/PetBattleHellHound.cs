using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Fourth;
using Server.Spells.Third;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a hellhound corpse" )]
	public class PetBattleHellHound : BaseCreature
	{
		[Constructable]
		public PetBattleHellHound() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Hellhound";
			Body = 98;
			BaseSoundID = 229;

            //Pet Battle Info            
            RangePerception = 25;

            PetBattleCreature = true;
            PetBattleItemId = 8482;
            PetBattleItemHue = 237;
            PetBattleStatueOffsetZ = 25;

            PetBattleCreatureSelectItemIdOffsetX = 20;
            PetBattleCreatureSelectItemIdOffsetY = 0;

            PetBattleGrimoireItemIdOffsetX = 0;
            PetBattleGrimoireItemIdOffsetY = 10;

            PetBattleTitle = "Hellhound";
            PetBattleBriefDescription = "Magical, fiery canine";
            PetBattleDescription = "Hellhounds use magical fire to incinerate their opponents quickly, and are quite resistant to magical forms of attack.";
            
            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(4);
            SetPetBattleCreatureDamageLevel(3);
            SetPetBattleCreatureAttackLevel(4);
            SetPetBattleCreatureDefendLevel(4);
            SetPetBattleCreatureSpeedLevel(4);
            SetPetBattleCreatureArmorLevel(2);
            SetPetBattleCreatureResistLevel(6);
            NoKillAwards = true;
        }

        public override void SetPetBattleOffensiveAbilities()
        {     
            m_PetBattleOffensiveAbilities.Add(new GnawAbility(this));
            m_PetBattleOffensiveAbilities.Add(new ScorchAbility(this));
            m_PetBattleOffensiveAbilities.Add(new GrowlAbility(this));
            m_PetBattleOffensiveAbilities.Add(new FireblastAbility(this));
            m_PetBattleOffensiveAbilities.Add(new IgniteAbility(this));             
        }

        public override void SetPetBattleDefensiveAbilities()
        {           
            m_PetBattleDefensiveAbilities.Add(new HowlAbility(this));
            m_PetBattleDefensiveAbilities.Add(new ResistanceAbility(this));
            m_PetBattleDefensiveAbilities.Add(new AuraAbility(this));
            m_PetBattleDefensiveAbilities.Add(new EvasionAbility(this));
            m_PetBattleDefensiveAbilities.Add(new FireskinAbility(this));           
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; } }

		//public override int GoldWorth { get { return Utility.RandomMinMax(12, 45); } }
		//public override double MaxSkillScrollWorth { get { return 87.5; } }		
		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

        public PetBattleHellHound(Serial serial)
            : base(serial)
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
