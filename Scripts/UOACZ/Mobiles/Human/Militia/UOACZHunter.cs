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
    public class UOACZHunter : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get{ return new string[0];} }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override bool IsRangedPrimary { get { return true; } }
        public override int WeaponSwitchRange { get { return 2; } }

        public override int DifficultyValue { get { return 4; } }
        
        [Constructable]
		public UOACZHunter() : base()
		{
            Title = "the hunter";

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            AttackSpeed = 30;

            SetDamage(9, 18);

            SetSkill(SkillName.Archery, 80);
            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);

            SetSkill(SkillName.Tactics, 100);
            
            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;

            Fame = 2000;
            Karma = 3000;            

            AddItem(new Boots() { Movable = false, Hue = 0 });
            AddItem(new LeatherLegs() { Movable = false, Hue = 0 });
            AddItem(new LeatherChest() { Movable = false, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = false, Hue = 0 });            

            AddItem(new Bow() { Movable = false, MaxRange = 14, Hue = SecondaryHue });
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            RangePerception = 14;
            DefaultPerceptionRange = 14;            
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public UOACZHunter(Serial serial): base(serial)
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
