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
    public class BritainShipSurgeon : OceanBaseCreature
	{
        public override string[] idleSpeech { get { return new string[] {       "*takes inventory of supplies*",
                                                                                "*examines vials*",
                                                                                "*fashions bandage*",
                                                                                "*adjusts glasses*"
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "It's just a scratch.",
                                                                                "Nothing a good doctor can't fix!",
                                                                                "That leg'll have to come off.",
                                                                                "Relax. You'll be fine." 
                                                                                };}}        

		[Constructable]
		public BritainShipSurgeon() : base()
		{
            SpeechHue = 0;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the ship surgeon";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the ship surgeon";
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

			Fame = 500;
			Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new FancyShirt() { Movable = true, Hue = 0 });
            AddItem(new ShortPants() { Movable = true, Hue = 0 });
            AddItem(new Shoes() { Movable = true, Hue = 0 });
            AddItem(new SkullCap() { Movable = false, Hue = 1150 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 1072 });
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

        public override int DoubloonValue { get { return 6; } }
        public override bool CanSwitchWeapons { get { return true; } }

        public BritainShipSurgeon(Serial serial): base(serial)
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
