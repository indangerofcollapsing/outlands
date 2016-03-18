using System;
using Server.Misc;
using Server.Network;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	public class KhaldunSummoner : BaseCreature
	{		
		[Constructable]
		public KhaldunSummoner():base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Body = 0x190;
			Name = "Zealot of Khaldun";
			Title = "the Summoner";

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(400);
            SetMana(2000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);           

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);
            
            VirtualArmor = 25;  

			Fame = 10000;
			Karma = -10000;

			LeatherGloves gloves = new LeatherGloves();
			gloves.Hue = 0x66D;
			AddItem( gloves );

			BoneHelm helm = new BoneHelm();
			helm.Hue = 0x835;
			AddItem( helm );

			Necklace necklace = new Necklace();
			necklace.Hue = 0x66D;
			AddItem( necklace );

			Cloak cloak = new Cloak();
			cloak.Hue = 0x66D;
			AddItem( cloak );

			Kilt kilt = new Kilt();
			kilt.Hue = 0x66D;
			AddItem( kilt );

			Sandals sandals = new Sandals();
			sandals.Hue = 0x66D;
			AddItem( sandals );
		}

        public override bool Unprovokable { get { return true; } }

        public override bool ClickTitle { get { return false; } }
        public override bool ShowFameTitle { get { return false; } }
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

		public KhaldunSummoner( Serial serial ) : base( serial )
		{
		}

		public override bool OnBeforeDeath()
		{
			BoneMagi rm = new BoneMagi();
			rm.Team = this.Team;
			rm.Combatant = this.Combatant;
			rm.NoKillAwards = true;

			if ( rm.Backpack == null )
			{
				Backpack pack = new Backpack();
				pack.Movable = false;
				rm.AddItem( pack );
			}

			for ( int i = 0; i < 2; i++ )
			{
				LootPack.FilthyRich.Generate( this, rm.Backpack, true );
				LootPack.FilthyRich.Generate( this, rm.Backpack, false );
			}

			Effects.PlaySound(this, Map, GetDeathSound());
			Effects.SendLocationEffect( Location, Map, 0x3709, 30, 10, 0x835, 0 );
			rm.MoveToWorld( Location, Map );

			Delete();
			return false;
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