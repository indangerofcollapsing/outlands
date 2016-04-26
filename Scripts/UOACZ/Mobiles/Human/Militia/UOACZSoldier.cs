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
    public class UOACZSoldier : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get { return new string[0]; } }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }
        
        public override int DifficultyValue { get { return 6; } }

        [Constructable]
		public UOACZSoldier() : base()
		{
            Title = "the soldier";

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(800);

            AttackSpeed = 40;

            SetDamage(15, 25);

            SetSkill(SkillName.Swords, 90);
            SetSkill(SkillName.Macing, 90);
            SetSkill(SkillName.Fencing, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 30);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 150;

            Fame = 2000;
            Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
           
            AddItem(new ChainmailCoif() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateGorget() { Movable = false, Hue = PrimaryHue });
            AddItem(new ChainmailChest() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateLegs() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateArms() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateGloves() { Movable = false, Hue = PrimaryHue });
            AddItem(new Kilt() { Movable = false, Hue = PrimaryHue });

            switch(Utility.RandomMinMax(1, 2))
            {
                case 1:
                    AddItem(new VikingSword() { Movable = false, Hue = SecondaryHue});
                    AddItem(new MetalKiteShield() { Movable = false, Hue = PrimaryHue });
                break;

                case 2:
                    AddItem(new Maul() { Movable = false, Hue = SecondaryHue });
                    AddItem(new MetalKiteShield() { Movable = false, Hue = PrimaryHue });
                break;
            }
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            SpecialAbilities.PhalanxSpecialAbility(1.0, null, this, 1.0, 50000, 0, false, "", "", "-1");
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Creatures.Contains(this))
                m_Creatures.Remove(this);

            if (m_Spawner != null)
                m_Spawner.m_Mobiles.Remove(this);
        }

        public UOACZSoldier(Serial serial): base(serial)
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
