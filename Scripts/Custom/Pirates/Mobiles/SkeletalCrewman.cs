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
    public class SkeletalCrewman : OceanBaseCreature
	{
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
		public SkeletalCrewman() : base()
		{
			SpeechHue = 2051;
            Hue = 1150;

            if (this.Female = Utility.RandomBool())
            {
                Title = "the skeletal crewman";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the skeletal crewman";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(225);

            SetDamage(6, 12);

            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);
            SetSkill(SkillName.Archery, 70);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;
            
            AddItem(new BoneHelm() { Movable = true, Hue = 0 });
            AddItem(new BoneChest() { Movable = true, Hue = 0 });
            AddItem(new BoneGloves() { Movable = true, Hue = 0 });
            AddItem(new BoneLegs() { Movable = true, Hue = 0 });

            switch (Utility.RandomMinMax(1, 6))
            {
                case 1: { AddItem(new Cutlass() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 2: { AddItem(new Longsword() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 3: { AddItem(new Mace() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 4: { AddItem(new Maul() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 5: { AddItem(new Kryss() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
                case 6: { AddItem(new WarFork() { Movable = true, Hue = 0 }); AddItem(new WoodenShield() { Movable = true, Hue = 0 }); break; }
            }

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}

        public override int OceanDoubloonValue { get { return 10; } }
        public override bool CanSwitchWeapons { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void SetUniqueAI()
        {
        }

        public SkeletalCrewman(Serial serial): base(serial)
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
