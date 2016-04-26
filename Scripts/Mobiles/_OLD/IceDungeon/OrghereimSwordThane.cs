using System; 
using System.Collections; 
using Server.Items; 
using Server.ContextMenus; 
using Server.Misc; 
using Server.Network; 

namespace Server.Mobiles 
{ 	
	[CorpseName( "an orghereim sword-thane corpse" )] 
	public class OrghereimSwordThane : BaseOrghereim 
	{ 
		[Constructable] 
		public OrghereimSwordThane() : base() 
		{    
			Name = "an orghereim sword-thane"; 

			SetStr( 75 );
			SetDex( 50 );
			SetInt( 25 );

			SetHits( 500 );

			SetDamage( 11, 22 );

            SetSkill(SkillName.Archery, 95);
            SetSkill(SkillName.Swords, 95);
            SetSkill(SkillName.Tactics, 100);

			SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

			Fame = 1500;
			Karma = -1500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new ChainmailCoif() { Movable = false, Hue = itemHue });
            AddItem(new StuddedGorget() { Movable = false, Hue = itemHue });
            AddItem(new BodySash() { Movable = false, Hue = itemHue });
            AddItem(new Kilt() { Movable = false, Hue = itemHue });
            AddItem(new Boots() { Movable = false, Hue = itemHue });

            AddItem(new RingmailChest() { Movable = false, Hue = 0 });
            AddItem(new RingmailArms() { Movable = false, Hue = 0 });
            AddItem(new RingmailGloves() { Movable = false, Hue = 0 });
            AddItem(new RingmailLegs() { Movable = false, Hue = 0 });
            
            AddItem(new VikingSword() { Movable = false, Hue = 0 });
		}

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.1;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 8; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(0.15, this, defender, DamageMax, 8, -1, true, "", "Their deft swordcut causes you to bleed!", "-1");
        }

		public OrghereimSwordThane( Serial serial ) : base( serial ) 
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