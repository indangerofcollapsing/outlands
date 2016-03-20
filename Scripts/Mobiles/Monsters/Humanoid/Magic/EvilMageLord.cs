using System; 
using Server;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "an evil mage lord corpse" )] 
	public class EvilMageLord : BaseCreature 
	{ 
		[Constructable] 
		public EvilMageLord() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 
			Name = NameList.RandomName( "evil mage lord" );
			Body = 0x190;    
        
			Hue = Utility.RandomSkinHue();			

            SetStr(50);
            SetDex(25);
            SetInt(100);

            SetHits(350);
            SetMana(2000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this);

            AddItem(new Robe(Utility.RandomMetalHue()));
            AddItem(new WizardsHat(Utility.RandomMetalHue()));

			Fame = 10500;
			Karma = -10500;

            AddItem(new Sandals());
		}

        public override bool CanRummageCorpses { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

		public override void OnDeath( Container c )
		{
			base.OnDeath(c);
		}

		public EvilMageLord( Serial serial ) : base( serial ) 
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
