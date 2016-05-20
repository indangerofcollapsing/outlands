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
    public class SkeletalCaptain : OceanBaseCreature
	{
        private BaseBoat m_Boat;

        public override string[] idleSpeech { get { return new string[] {       "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "",
                                                                                "",
                                                                                "",
                                                                                "" 
                                                                                };}}             

		[Constructable]
		public SkeletalCaptain(BaseBoat boat) : base()
		{
            m_Boat = boat;

			SpeechHue = 2051;
            Hue = 1150;

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                Title = "the skeletal captain";
            }

            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                Title = "the skeletal captain";
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

            AddItem(new Cloak() { Movable = false, Hue = Utility.RandomDyedHue() });
            AddItem(new BoneHelm() { Movable = true, Hue = 0 });
            AddItem(new BoneChest() { Movable = true, Hue = 0 });
            AddItem(new StuddedGorget() { Movable = true, Hue = 0 });
            AddItem(new BoneArms() { Movable = true, Hue = 0 });
            AddItem(new BoneGloves() { Movable = true, Hue = 0 });
            AddItem(new BoneLegs() { Movable = true, Hue = 0 });
            AddItem(new ThighBoots() { Movable = true, Hue = 0 });

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { AddItem(new Cutlass() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 2: { AddItem(new Longsword() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 3: { AddItem(new Mace() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 4: { AddItem(new Maul() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 5: { AddItem(new Kryss() { Movable = true, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
                case 6: { AddItem(new OrnateAxe() { Movable = false, Speed = 30, Layer = Layer.FirstValid, Hue = 0 }); AddItem(new MetalShield() { Movable = true, Hue = 0 }); break; }
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}
                
        public override int DoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override int PoisonResistance { get { return 5; } }

        public override void SetUniqueAI()
        {
        }

        public SkeletalCaptain(Serial serial): base(serial)
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