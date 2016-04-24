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
    public class UOACZFortCommander : UOACZBaseMilitia
	{
        public override string[] wildernessIdleSpeech { get { return new string[0]; } }
        public override string[] idleSpeech { get { return new string[0]; } }
        public override string[] undeadCombatSpeech { get { return new string[0]; } }
        public override string[] humanCombatSpeech { get { return new string[0]; } }

        public override int DifficultyValue { get { return 11; } }

        public DateTime m_NextAssaultAllowed;
        public TimeSpan NextAssaultDelay = TimeSpan.FromSeconds(60);

        [Constructable]
		public UOACZFortCommander() : base()
		{
            Title = "the fort commander";

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(15000);
            SetStam(5000);

            AttackSpeed = 30;

            SetDamage(30, 40);

            SetSkill(SkillName.Fencing, 100);
            SetSkill(SkillName.Swords, 100);
            SetSkill(SkillName.Macing, 100);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 30);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 200;

            Fame = 2000;
            Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new PlateChest() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateLegs() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateArms() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateGorget() { Movable = false, Hue = PrimaryHue });
            AddItem(new PlateGloves() { Movable = false, Hue = PrimaryHue });

            AddItem(new Cloak() { Movable = false, Hue = 2117 });
            AddItem(new BodySash() { Movable = false, Hue = 2117 });

            AddItem(new Lance() { Movable = false, Hue = PrimaryHue, Speed = 40, Name = "Fort Commander's Lance" });
            AddItem(new DupresShield() { Movable = false, Hue = PrimaryHue, Name = "Fort Commander's Shield" });

            Horse mount = new Horse();
            mount.Hue = 2405;
            mount.Rider = this;

            HairItemID = 8252;
            HairHue = 2118;
		}

        public override bool AlwaysBoss { get { return true; } }
               
        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            ActiveSpeed = .25;
            CurrentSpeed = .25;
            PassiveSpeed = .25;

            RangeHome = 60;

            SpecialAbilities.PhalanxSpecialAbility(1.0, null, this, 1.0, 50000, 0, false, "", "", "-1");
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            if (willKill)
                UOACZEvents.HumanBossDamaged(true);

            else
                UOACZEvents.HumanBossDamaged(false);
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() < .1 && DateTime.UtcNow >= m_NextAssaultAllowed)
            {
                SpecialAbilities.ExpertiseSpecialAbility(1.0, this, defender, .25, 20, -1, true, "", "", "*begins to strike with fury*");
                SpecialAbilities.EnrageSpecialAbility(1.0, null, this, .5, 20, 0, false, "", "", "-1");

                m_NextAssaultAllowed = DateTime.UtcNow + NextAssaultDelay;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                PrivateOverheadMessage(MessageType.Regular, 0x3B2, false, "(Human Boss)", from.NetState);

            base.OnSingleClick(from);
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override bool OnBeforeDeath()
        {
            IMount mount = this.Mount;

            if (mount != null)
                mount.Rider = null;

            if (mount is Mobile)
                ((Mobile)mount).Kill();

            return base.OnBeforeDeath();
        }

        public UOACZFortCommander(Serial serial): base(serial)
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
