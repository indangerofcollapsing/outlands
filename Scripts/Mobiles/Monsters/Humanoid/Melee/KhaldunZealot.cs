using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class KhaldunZealot : BaseCreature
	{
		[Constructable]
		public KhaldunZealot():base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x190;
			Name = "Zealot of Khaldun";
			Title = "the Knight";
			Hue = 0;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(15, 25);

            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Parry, 25);

            VirtualArmor = 25;              

			Fame = 10000;
			Karma = -10000;

			VikingSword weapon = new VikingSword();
			weapon.Hue = 0x835;
			weapon.Movable = false;
			AddItem( weapon );

			MetalShield shield = new MetalShield();
			shield.Hue = 0x835;
			shield.Movable = false;
			AddItem( shield );

			BoneHelm helm = new BoneHelm();
			helm.Hue = 0x835;
			AddItem( helm );

			BoneArms arms = new BoneArms();
			arms.Hue = 0x835;
			AddItem( arms );

			BoneGloves gloves = new BoneGloves();
			gloves.Hue = 0x835;
			AddItem( gloves );

			BoneChest tunic = new BoneChest();
			tunic.Hue = 0x835;
			AddItem( tunic );
			BoneLegs legs = new BoneLegs();
			legs.Hue = 0x835;
			AddItem( legs );

			AddItem( new Boots() );
		}
                
        public override bool AlwaysMurderer { get { return true; } }

		public override int GetIdleSound()
		{
			return 0x184;
		}

		public override int GetAngerSound()
		{
			return 0x286;
		}

		public override int GetDeathSound()
		{
			return 0x288;
		}

		public override int GetHurtSound()
		{
			return 0x19F;
		}		

		public KhaldunZealot( Serial serial ) : base( serial )
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