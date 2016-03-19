using System;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Custom;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class UOACZRanger : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get{ return new string[0];} }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override bool IsRangedPrimary { get { return true; } }
        public override int WeaponSwitchRange { get { return 2; } }

        public override int DifficultyValue { get { return 6; } }
        
        [Constructable]
		public UOACZRanger() : base()
		{
            Title = "the ranger";

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(700);

            AttackSpeed = 40;

            SetDamage(12, 24);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Tactics, 100);
            
            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 75;

            Fame = 2000;
            Karma = 3000;

            AddItem(new StuddedGorget() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedChest() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedLegs() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedArms() { Movable = false, Hue = PrimaryHue });
            AddItem(new StuddedGloves() { Movable = false, Hue = PrimaryHue });
            AddItem(new BodySash() { Movable = false, Hue = PrimaryHue });
            AddItem(new Kilt() { Movable = false, Hue = PrimaryHue });
            AddItem(new Boots() { Movable = false, Hue = PrimaryHue });

            AddItem(new Bow() { Movable = false, MaxRange = 16, Hue = SecondaryHue }); 
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            RangePerception = 16;
            DefaultPerceptionRange = 16;            
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public UOACZRanger(Serial serial): base(serial)
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
