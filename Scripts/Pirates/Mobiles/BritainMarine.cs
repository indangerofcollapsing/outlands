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
	public class BritainMarine : OceanBaseCreature
	{
        public override string[] idleSpeech { get { return new string[] {       "The only good pirate is a dead pirate",
                                                                                "We serve the crown",
                                                                                "Duty. Honor. Death",
                                                                                "*scowls*"
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "You will face justice",
                                                                                "Make peace with your gods",
                                                                                "Your death will be quick",
                                                                                "Let none escape"
                                                                                };}}        

		[Constructable]
		public BritainMarine() : base()
		{
            SpeechHue = 0;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the marine";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the marine";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(400);

            AttackSpeed = 30;

            SetDamage(10, 20);

            SetSkill(SkillName.Archery, 80);
            SetSkill(SkillName.Fencing, 80);
            SetSkill(SkillName.Swords, 80);
            SetSkill(SkillName.Macing, 80);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 25);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

            Fame = 2000;
            Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new BodySash() { Movable = false, Hue = 404 });
            AddItem(new PlateHelm() { Movable = true, Hue = 0 });
            AddItem(new PlateGorget() { Movable = true, Hue = 0 });
            AddItem(new PlateChest() { Movable = true, Hue = 0 });
            AddItem(new PlateArms() { Movable = true, Hue = 0 });
            AddItem(new PlateGloves() { Movable = true, Hue = 0 });
            AddItem(new PlateLegs() { Movable = true, Hue = 0 });

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { AddItem(new VikingSword() { Movable = true, Hue = 0 }); AddItem(new HeaterShield() { Movable = true, Hue = 0 }); break; }
                case 2: { AddItem(new WarMace() { Movable = true, Hue = 0 }); AddItem(new HeaterShield() { Movable = true, Hue = 0 }); break; }
                case 3: { AddItem(new Halberd() { Movable = true, Hue = 0 }); break; }
                case 4: { AddItem(new Bardiche() { Movable = true, Hue = 0 }); break; }
                case 5: { AddItem(new Spear() { Movable = true, Hue = 0 }); break; }
                case 6: { AddItem(new WarHammer() { Movable = true, Hue = 0 }); break; }
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}
        
        public override void SetUniqueAI()
        {
        }

        public override int OceanDoubloonValue { get { return 8; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public BritainMarine(Serial serial): base(serial)
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
