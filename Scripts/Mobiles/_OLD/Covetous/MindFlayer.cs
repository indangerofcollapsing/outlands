using System;
using Server;
using Server.Items;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Spells.Sixth;


namespace Server.Mobiles
{
	[CorpseName( "a mind flayer corpse" )]
	public class MindFlayer : BaseCreature
	{
        [Constructable]
		public MindFlayer() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mind flayer";
			Body = 721;
			BaseSoundID = 0x3E9;

			SetStr( 50 );
			SetDex( 75 );
			SetInt( 100 );

			SetHits( 900 );
            SetMana( 2000 );

			SetDamage(20, 30);

            SetSkill(SkillName.Wrestling, 100 );
            SetSkill(SkillName.Tactics, 100 );

            SetSkill(SkillName.Magery, 100);
			SetSkill(SkillName.EvalInt, 100 );
            SetSkill(SkillName.Meditation, 100);

			SetSkill(SkillName.MagicResist, 200 );

            VirtualArmor = 25;	

			Fame = 8000;
			Karma = -8000;					
		}
        
        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.05;
            
            DictCombatSpell[CombatSpell.SpellDamage5] = 8;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
           
            if (Utility.RandomDouble() < .5)
            {
                double manaLoss = Utility.RandomMinMax(30, 60);

                if (defender is PlayerMobile)
                    manaLoss /= 2;

                if (manaLoss < 1)
                    manaLoss = 1;

                defender.Mana -= (int)manaLoss;

                double manaPercent = (double)Mana / (double)ManaMax;
                double damageAmount = manaLoss * (1 - manaPercent);

                if (damageAmount < 1)
                    damageAmount = 1;

                defender.FixedParticles(0x374A, 10, 30, 5038, 0x075, 0, EffectLayer.Head);
                defender.PlaySound(0x581);

                SpecialAbilities.HinderSpecialAbility(1.0, this, defender, 1, 1.0, false, -1, false, "", "The creature feeds upon your brain!", "-1");
                new Blood().MoveToWorld(defender.Location, defender.Map);

                AOS.Damage(defender, this, (int)damageAmount, 0, 100, 0, 0, 0);
            }            
        }        

        public override int GetAngerSound(){return 0x381;}
        public override int GetIdleSound(){return 0x382;}
        public override int GetAttackSound(){return 0x384;}
        public override int GetHurtSound(){return 0x384;}
        public override int GetDeathSound(){return 0x56F;}

		public override bool CanRummageCorpses{ get{ return true; } }		

		public MindFlayer( Serial serial ) : base( serial )
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
