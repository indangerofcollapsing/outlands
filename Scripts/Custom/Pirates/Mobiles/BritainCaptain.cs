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
	public class BritainShipCaptain : OceanBaseCreature
	{
        private BaseBoat m_Boat;

        public override string[] idleSpeech { get { return new string[] {    "An extra ration of rum for any man that slays a pirate!",
                                                                                "Eyes on the horizon, men!",
                                                                                "Hoist the mainsail!",
                                                                                "Belay that line!",
                                                                                "Batten down those hatches!",
                                                                                "Set sail!"
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {  "For king and country!",
                                                                                "Give them hell, men!",
                                                                                "Stand your ground!",
                                                                                "Death to pirates and deserters!" 
                                                                                };}}        

		[Constructable]
		public BritainShipCaptain(BaseBoat boat) : base()
		{
            m_Boat = boat;
            
            SpeechHue = 0;
            Hue = Utility.RandomSkinHue();

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

            Fame = 4000;
            Karma = 5000;

            if (this.Female = Utility.RandomBool())
            {
                Title = "the navy captain";
                Body = 0x191;
                Name = NameList.RandomName("female");                
            }

            else
            {
                Title = "the navy captain";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new Cloak() { Movable = false, Hue = 404 });
            AddItem(new BodySash() { Movable = false, Hue = 404 });
            AddItem(new Kilt() { Movable = false, Hue = 404 });

            AddItem(new PlateArms() { Movable = true, Hue = 0 });
            AddItem(new PlateGloves() { Movable = true, Hue = 0 });
            AddItem(new ChainChest() { Movable = true, Hue = 0 });
            AddItem(new StuddedLegs() { Movable = true, Hue = 0 });
            AddItem(new Boots() { Movable = true, Hue = 0 });

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: { AddItem(new VikingSword() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 2: { AddItem(new Broadsword() { Movable = true, Hue = 0 }); AddItem(new HeaterShield() { Movable = true, Hue = 0 }); break; }
                case 3: { AddItem(new Maul() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 4: { AddItem(new Mace() { Movable = true, Hue = 0 }); AddItem(new HeaterShield() { Movable = true, Hue = 0 }); break; }
                case 5: { AddItem(new BlackStaff() { Movable = true, Hue = 0 }); break; }
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}
         
        public override void SetUniqueAI()
        {
        }
        
        public override int OceanDoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public BritainShipCaptain(Serial serial): base(serial)
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
