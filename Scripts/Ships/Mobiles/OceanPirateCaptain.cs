using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;

namespace Server.Custom.Pirates
{
    public class OceanPirateCaptain : OceanBaseCreature
	{
        private BaseBoat m_Boat;       

        public override string[] idleSpeech { get { return new string[] {       "Let none alive take the wind from our sails",
                                                                                "Ho, ho! The sea's our mistress and she be a good lass to us",
                                                                                "A pirate's takes what he wants and gives nothing back!",
                                                                                "Booty and plunder for us, misery and death to the rest!" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Give em' what-for, men!",
                                                                                "Leave none alive!",
                                                                                "Gutless cowards, all of em!",
                                                                                "Har, har!" 
                                                                                };}}          

		[Constructable]
		public OceanPirateCaptain(BaseBoat boat) : base()
		{
            m_Boat = boat;

            SpeechHue = 0x22;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                Title = "the pirate captain";
            }

            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                Title = "the pirate captain";
            }

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(8, 16);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Macing, 90);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 30);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

            Fame = 3000;
            Karma = -3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: AddItem(new LongPants(Utility.RandomDyedHue())); break;
                case 2: AddItem(new ShortPants(Utility.RandomDyedHue())); break;
            }

            switch (Utility.RandomMinMax(1, 2))
			{
                case 1: AddItem(new StuddedChest()); break;
                case 2: AddItem(new RingmailChest()); break;                      
			}

            AddItem(new Cloak(Utility.RandomDyedHue()));  
            AddItem(new TricorneHat(Utility.RandomDyedHue()));
            AddItem(new StuddedGorget());
            AddItem(new StuddedArms());
            AddItem(new BoneGloves());
            AddItem(new ThighBoots());               

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: { AddItem(new Cutlass()); AddItem(new MetalShield()); break; }
                case 2: { AddItem(new Scimitar()); AddItem(new MetalShield()); break; }                
                case 3: { AddItem(new WarFork()); AddItem(new MetalShield()); break; }
                case 4: { AddItem(new HammerPick()); AddItem(new MetalShield()); break; }
                case 5: { AddItem(new ExecutionersAxe()); break; }
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}

        public override void SetUniqueAI()
        {
        }

        public override int DoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        
		public OceanPirateCaptain( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); //Version
            writer.Write(m_Boat);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Boat = (BaseBoat)reader.ReadItem();
		}
	}
}