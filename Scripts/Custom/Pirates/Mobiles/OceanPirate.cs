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
    public class OceanPirate : OceanBaseCreature
	{        
        public override string[] idleSpeech { get { return new string[] {       "Pillagin' and plunderin's the life...",
                                                                                "There's got to be some rum around here somewhere...",
                                                                                "*spins dagger*",
                                                                                "*spits*" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Filthy coward!",
                                                                                "I'll cut out yer heart!",
                                                                                "Scurvy dog!",
                                                                                "Another one for Davy' Jones!" 
                                                                                };}}
		[Constructable]
		public OceanPirate() : base()
		{
            SpeechHue = 0x22;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the pirate";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the pirate";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
            
            switch (Utility.RandomMinMax(0, 1))
			{
                case 0: AddItem(new LongPants(Utility.RandomDyedHue())); break;
                case 1: AddItem(new ShortPants(Utility.RandomDyedHue())); break;
			}

            switch (Utility.RandomMinMax(0, 3))
			{
                case 0: AddItem(new FancyShirt(Utility.RandomDyedHue())); break;
                case 1: AddItem(new Shirt(Utility.RandomDyedHue())); break;
                case 2: AddItem(new Doublet(Utility.RandomDyedHue())); break;
			}

            switch (Utility.RandomMinMax(0, 2))
			{
                case 0: AddItem(new Bandana(Utility.RandomDyedHue())); break;
                case 1: AddItem(new SkullCap(Utility.RandomDyedHue())); break;
			}

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { AddItem(new Cutlass()); AddItem(new Buckler()); break; }
                case 2: { AddItem(new Scimitar()); AddItem(new Buckler()); break; }
                case 3: { AddItem(new Club()); AddItem(new Buckler()); break; }
                case 4: { AddItem(new Kryss()); AddItem(new Buckler()); break; }
                case 5: { AddItem(new WarFork()); AddItem(new Buckler()); break; }
                case 6: { AddItem(new Pitchfork()); break; }  
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}

        public override void SetUniqueAI()
        {
        }

		public OceanPirate( Serial serial ) : base( serial )
		{
		}

        public override int OceanDoubloonValue { get { return 4; } }
        public override bool CanSwitchWeapons { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

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
