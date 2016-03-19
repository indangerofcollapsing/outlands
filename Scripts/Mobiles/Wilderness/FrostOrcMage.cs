using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using Server.Spells.Third;
using Server.Achievements;

namespace Server.Mobiles
{
	[CorpseName( "a frost orc mage corpse" )]
	public class FrostOrcMage : BaseCreature
	{
		public override InhumanSpeech SpeechType{ get{ return InhumanSpeech.Orc; } }

		[Constructable]
		public FrostOrcMage () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a frost orc mage";
			Body = 140;
			BaseSoundID = 0x45A;
            Hue = 2221;

            SetStr(50);
            SetDex(50);
            SetInt(75);

            SetHits(150);
            SetMana(1000);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 55);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 75);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            VirtualArmor = 25;

			Fame = 3000;
			Karma = -3000;
		}

        public override bool CanRummageCorpses { get { return true; } }
        public override int Meat { get { return 1; } }

		public override void OnDeath( Container c )
		{			
    		base.OnDeath( c );

    		AwardDailyAchievementForKiller(NewbCategory.KillOrcs);

            if (Utility.RandomMinMax(1, 5) == 1)
                c.AddItem(new CaveMoss());
		}

        public FrostOrcMage(Serial serial): base(serial)
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
