using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom.Pirates
{
    public class BritainSailor : OceanBaseCreature
	{
        public override string[] idleSpeech { get { return new string[] {       "*scans horizon*",
                                                                                "*inspects sail*",
                                                                                "*fastens rope*",
                                                                                "*stares blankly*"
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Die scum!",
                                                                                "No quarter!",
                                                                                "Run em' through!",
                                                                                "For Britain!" 
                                                                                };}}
		[Constructable]
		public BritainSailor() : base()
		{
			SpeechHue = 0;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the sailor";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the sailor";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 500;
			Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new ThighBoots() { Movable = true, Hue = 0 });
            AddItem(new Bandana() { Movable = true, Hue = 0 });
            AddItem(new FancyShirt() { Movable = true, Hue = 0 });
            AddItem(new ShortPants() { Movable = true, Hue = 0 });

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: { AddItem(new Cutlass() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 2: { AddItem(new Longsword() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 3: { AddItem(new Mace() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 4: { AddItem(new Maul() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }  
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}

        public override int DoubloonValue { get { return 4; } }
        public override bool CanSwitchWeapons { get { return true; } }
        
        public override void SetUniqueAI()
        {
        }

        public BritainSailor(Serial serial): base(serial)
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
