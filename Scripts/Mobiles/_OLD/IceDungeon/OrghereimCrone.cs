using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an orghereim crone corpse" )] 
	public class OrghereimCrone : BaseOrghereim 
	{   
        [Constructable] 
		public OrghereimCrone() : base() 
		{          
            Name = "an orghereim crone";
			Body = 401;			

			SetStr( 50 );
			SetDex( 50 );
			SetInt( 100 );

            SetHits( 750 );
            SetMana( 2000 );

            SetDamage(12, 24);

            SetSkill(SkillName.Macing, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);           

            SetSkill(SkillName.MagicResist, 150);            

			Fame = 2500;
			Karma = -2500;

            VirtualArmor = 25;

            AddItem(new Robe() { Movable = false, Hue = itemHue });
            AddItem(new Cap() { Movable = false, Hue = itemHue });
            AddItem(new Sandals() { Movable = false, Hue = itemHue });

            AddItem(new WildStaff() { Movable = false, Hue = weaponHue, Layer = Layer.TwoHanded, Name = "an orghereim staff" });
		}

        public override void SetUniqueAI()
        {
            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 10; } }

		public OrghereimCrone( Serial serial ) : base( serial )
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