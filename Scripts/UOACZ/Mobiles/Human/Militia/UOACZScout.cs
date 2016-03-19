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
    public class UOACZScout : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get { return new string[0]; } }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override int DifficultyValue { get { return 4; } }

        [Constructable]
		public UOACZScout() : base()
		{
            Title = "the scout";

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(600);

            AttackSpeed = 40;

            SetDamage(10, 20);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Fencing, 80);
            SetSkill(SkillName.Swords, 80);
            SetSkill(SkillName.Macing, 80);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 20);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 100;

            Fame = 2000;
            Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new Boots() { Movable = false, Hue = 0 });
            AddItem(new StuddedLegs() { Movable = false, Hue = 0 });
            AddItem(new StuddedChest() { Movable = false, Hue = 0 });
            AddItem(new StuddedArms() { Movable = false, Hue = 0 });
            AddItem(new StuddedGloves() { Movable = false, Hue = 0 });
            AddItem(new StuddedGorget() { Movable = false, Hue = 0 });            

            switch(Utility.RandomMinMax(1, 3))
            {
                case 1:
                    AddItem(new Broadsword() { Movable = false, Hue = 0});
                    AddItem(new MetalShield() { Movable = false, Hue = 0 });
                break;

                case 2:
                    AddItem(new Mace() { Movable = false, Hue = 0 });
                    AddItem(new MetalShield() { Movable = false, Hue = 0 });
                break;

                case 3:
                    AddItem(new Spear() { Movable = false, Hue = 0, Layer = Layer.FirstValid });
                    AddItem(new MetalShield() { Movable = false, Hue = 0 });
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

        public UOACZScout(Serial serial): base(serial)
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
