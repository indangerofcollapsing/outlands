using System; 
using System.Collections;
using System.Collections.Generic;
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "an orghereim beastmaster corpse" )] 
	public class OrghereimBeastmaster: BaseOrghereim 
	{
        public List<Mobile> m_BayingHounds = new List<Mobile>();
        
        [Constructable] 
		public OrghereimBeastmaster() : base() 
		{            
			Name = "an orghereim beastmaster";

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits(600);

            SetDamage(12, 24);            

            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Tactics, 100);

			SetSkill(SkillName.MagicResist, 50 );

            SetSkill(SkillName.Healing, 80);
            SetSkill(SkillName.Veterinary, 80);

            VirtualArmor = 25;

			Fame = 3500;
			Karma = -3500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new LeatherCap() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });

            AddItem(new StuddedChest() { Movable = false, Hue = 0 });
            AddItem(new StuddedArms() { Movable = false, Hue = 0 });
            AddItem(new StuddedLegs() { Movable = false, Hue = 0 });
            AddItem(new StuddedGorget() { Movable = false, Hue = 0 });
            AddItem(new StuddedGloves() { Movable = false, Hue = 0 });            

            AddItem(new ShepherdsCrook() { Movable = false, Hue = 0 });
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;
                        
            DictCombatAction[CombatAction.CombatHealSelf] = 0;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 5; } }        

        public override void OnDelete()
        {
 	        base.OnDelete();            
        }

		public OrghereimBeastmaster( Serial serial ) : base( serial ) 
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