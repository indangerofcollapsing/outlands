using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "an orghereim shield maiden corpse" )] 
	public class OrghereimShieldMaiden : BaseOrghereim 
	{        
        [Constructable] 
		public OrghereimShieldMaiden() : base() 
		{           
			Name = "an orghereim shield maiden"; 			

            Body = 0x191; 

			SetStr( 75 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 350 );

			SetDamage( 9, 18 );

            SetSkill(SkillName.Archery, 85);
            SetSkill(SkillName.Fencing, 85);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);            
           
			SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

			Fame = 2500;
			Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new PlateGorget() { Movable = false, Hue = itemHue });
            AddItem(new FemalePlateChest() { Movable = false, Hue = itemHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });
            AddItem(new ThighBoots() { Movable = false, Hue = itemHue });

            AddItem(new Spear() { Movable = false, Hue = itemHue, Layer = Layer.FirstValid});
            AddItem(new WoodenShield() { Movable = false, Hue = itemHue });
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.05;

            DictCombatTargeting[CombatTargeting.Predator] = 1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 6; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.PierceSpecialAbility(0.05, this, defender, 50, 10, -1, true, "", "Their spear pierces your armor!", "-1");
        }
		
		public OrghereimShieldMaiden( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); //version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	} 
}