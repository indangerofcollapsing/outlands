using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a burrow beetle corpse" )]
	public class PetBattleBurrowBeetle : BaseCreature
	{
		[Constructable]
        public PetBattleBurrowBeetle(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
			Name = "Burrow Beetle";
			Body = 0x317;

            //Pet Battle Info           
            RangePerception = 25;

            PetBattleCreature = true;
            PetBattleItemId = 9743;
            PetBattleItemHue = 0;
            PetBattleStatueOffsetZ = 25;

            PetBattleCreatureSelectItemIdOffsetX = 5;
            PetBattleCreatureSelectItemIdOffsetY = 0;

            PetBattleGrimoireItemIdOffsetX = -20;
            PetBattleGrimoireItemIdOffsetY = 0;

            PetBattleTitle = "Burrow Beetle";
            PetBattleBriefDescription = "Subterranean armored beast";
            PetBattleDescription = "Burrow Beetles are extremely resilient creatures capable of drastically improving their armor and readily tearing through their opponent's armor in turn.";

            SetPetBattleBaseStats();

            SetPetBattleCreatureHealthLevel(7);
            SetPetBattleCreatureDamageLevel(3);
            SetPetBattleCreatureAttackLevel(2);
            SetPetBattleCreatureDefendLevel(2);
            SetPetBattleCreatureSpeedLevel(1);
            SetPetBattleCreatureArmorLevel(7);
            SetPetBattleCreatureResistLevel(5);
            NoKillAwards = true;
        }

        public override void SetPetBattleOffensiveAbilities()
        {
            m_PetBattleOffensiveAbilities.Add(new RendAbility(this));
            m_PetBattleOffensiveAbilities.Add(new PoisonAbility(this));
            m_PetBattleOffensiveAbilities.Add(new CarveAbility(this));
            m_PetBattleOffensiveAbilities.Add(new BurrowAbility(this));
            m_PetBattleOffensiveAbilities.Add(new TeardownAbility(this));
        }

        public override void SetPetBattleDefensiveAbilities()
        {
            m_PetBattleDefensiveAbilities.Add(new ToughnessAbility(this));
            m_PetBattleDefensiveAbilities.Add(new IronskinAbility(this));
            m_PetBattleDefensiveAbilities.Add(new CleanseAbility(this));
            m_PetBattleDefensiveAbilities.Add(new IgnorePainAbility(this));
            m_PetBattleDefensiveAbilities.Add(new StonewallAbility(this));
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
            m_PetBattle.PetBattleCreatureDeath(this);
        }

        public override bool AllowParagon { get { return base.AllowParagon; } }

        public override int GetAngerSound()
        {
            return 0x21D;
        }

        public override int GetIdleSound()
        {
            return 0x21D;
        }

        public override int GetAttackSound()
        {
            return 0x162;
        }

        public override int GetHurtSound()
        {
            return 0x163;
        }

        public override int GetDeathSound()
        {
            return 0x21D;
        }

        public PetBattleBurrowBeetle(Serial serial)
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
