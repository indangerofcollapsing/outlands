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
    public class PirateSawbones : OceanBaseCreature
	{
        public override string[] idleSpeech { get { return new string[] {       "*drops foul smelling vial*",
                                                                                "Looks infected. Better get some leeches.",
                                                                                "*hand shakes noticibly*... *drinks",
                                                                                "*furrows brow*"
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Har, that's nothing!",
                                                                                "One leg's just as good as two.",
                                                                                "They're harder to cut open when their movin'!",
                                                                                "I wasn't trained fer this!" 
                                                                                };}}        

		[Constructable]
		public PirateSawbones() : base()
		{
            SpeechHue = 0x22;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the pirate sawbones";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the pirate sawbones";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Healing, 80);

            VirtualArmor = 25;

            Fame = 3000;
            Karma = -3000;   
         
            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new FancyShirt() { Movable = true, Hue = 0 });
            AddItem(new ShortPants() { Movable = true, Hue = 0 });
            AddItem(new SkullCap() { Movable = false, Hue = Utility.RandomRedHue() });
            AddItem(new LeatherGloves() { Movable = false, Hue = Utility.RandomRedHue() });
            AddItem(new FullApron() { Movable = true, Hue = 0 });

            AddItem(new ButcherKnife() { Movable = true, Hue = 0 });

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}

        public override void SetUniqueAI()
        {            
            DictCombatAction[CombatAction.AttackOnly] = 1;
            DictCombatAction[CombatAction.CombatSpecialAction] = 50;
            DictCombatAction[CombatAction.CombatHealOther] = 10;
            DictCombatAction[CombatAction.CombatHealSelf] = 5;

            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 6; } }
        public override bool CanSwitchWeapons { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }

        public PirateSawbones(Serial serial): base(serial)
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
