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
    public class UOACZSentry : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get { return new string[0]; } }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override bool IsRangedPrimary { get { return true; } }
        public override int WeaponSwitchRange { get { return 2; } }

        public override int DifficultyValue { get { return 8; } }

        public override bool Sentry { get { return true; } }       

        [Constructable]
		public UOACZSentry() : base()
		{
            Title = "the sentry";

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(800);

            AttackSpeed = 30;

            SetDamage(15, 25);

            SetSkill(SkillName.Archery, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 150;

            Fame = 2000;
            Karma = 3000;

            AddItem(new PlateLegs() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateGorget() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateArms() { Movable = false, Hue = PrimaryHue });
            AddItem(new LeatherChest() { Movable = false, Hue = SecondaryHue });
            AddItem(new LeatherGloves() { Movable = false, Hue = SecondaryHue });
            AddItem(new BodySash() { Movable = false, Hue = PrimaryHue });
            AddItem(new Kilt() { Movable = false, Hue = PrimaryHue });

            AddItem(new HeavyCrossbow() { Movable = false, MaxRange = 14, Hue = 2500 });

            Frozen = true;
            CantWalk = true;
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

            Frozen = true;
            CantWalk = true;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public UOACZSentry(Serial serial): base(serial)
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
