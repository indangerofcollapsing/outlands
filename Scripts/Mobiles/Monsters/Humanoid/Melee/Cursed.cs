using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "an inhuman corpse" )]
	public class Cursed : BaseCreature
	{
		public override bool ClickTitle{ get{ return false; } }
		public override bool ShowFameTitle{ get{ return false; } }

		[Constructable]
		public Cursed() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Title = "the Cursed";

			Hue = Utility.RandomMinMax( 0x8596, 0x8599 );
			Body = 0x190;
			Name = NameList.RandomName( "male" );
			BaseSoundID = 471;

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 60);
            SetSkill(SkillName.Fencing, 60);
            SetSkill(SkillName.Macing, 60);
            SetSkill(SkillName.Swords, 60);
            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;

            AddItem(new ShortPants(Utility.RandomNeutralHue()));
            AddItem(new Shirt(Utility.RandomNeutralHue()));

            BaseWeapon weapon = Loot.RandomWeapon();
            weapon.Movable = false;
            AddItem(weapon);
		}

        public override bool AlwaysMurderer { get { return true; } }

		public override int GetAttackSound()
		{
			return -1;
		}

		public Cursed( Serial serial ) : base( serial )
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
