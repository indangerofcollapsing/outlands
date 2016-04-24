using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 
	public class Executioner : BaseCreature 
	{ 
		[Constructable] 
		public Executioner() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 
			SpeechHue = Utility.RandomDyedHue(); 
			Title = "the executioner"; 
			Hue = Utility.RandomSkinHue(); 

			if ( this.Female = Utility.RandomBool() ) 
			{ 
				Body = 0x191; 
				Name = NameList.RandomName( "female" ); 
				AddItem( new Skirt( Utility.RandomRedHue() ) ); 
			} 

			else 
			{ 
				Body = 0x190; 
				Name = NameList.RandomName( "male" ); 
				AddItem( new ShortPants( Utility.RandomRedHue() ) ); 
			}

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(15, 25);

            SetSkill(SkillName.Swords, 120);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;            

			Fame = 5000;
			Karma = -5000;
   
			AddItem( new ExecutionersAxe());

			Utility.AssignRandomHair( this );
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(.2, this, defender, DamageMax, 8.0, -1, true, "", "Their strike causes you to bleed!", "-1");
        }

        public override bool AlwaysMurderer { get { return true; } }

		public override void OnDeath(Container c)
		{
			base.OnDeath(c);
		}

		public Executioner( Serial serial ) : base( serial ) 
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
