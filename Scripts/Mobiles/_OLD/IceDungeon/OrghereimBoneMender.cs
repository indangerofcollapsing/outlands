using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an orghereim bone mender corpse" )] 
	public class OrghereimBoneMender : BaseOrghereim 
	{        
        [Constructable] 
		public OrghereimBoneMender() : base() 
		{           
            Name = "an orghereim bone mender";

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 100 );

            SetHits( 450 );
            SetMana( 1000 );

            SetDamage(7, 14);            

            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Tactics, 100);            

            SetSkill(SkillName.Magery, 75);
            SetSkill(SkillName.EvalInt, 75);

            SetSkill(SkillName.MagicResist, 75);

            VirtualArmor = 25;

            Fame = 2500;
            Karma = -2500;

            Utility.AssignRandomHair(this, hairHue);

            AddItem(new Doublet() { Movable = false, Hue = itemHue });
            AddItem(new Skirt() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });            

            AddItem(new WildStaff() { Movable = false, Hue = weaponHue, Layer = Layer.TwoHanded, Name = "an orghereim staff" });            
		}

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int DoubloonValue { get { return 8; } }

		public OrghereimBoneMender( Serial serial ) : base( serial )
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