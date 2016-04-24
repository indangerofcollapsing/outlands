using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server
{
    [CorpseName("a vampire bat corpse")]
    public class UOACZVampireBat : UOACZBaseUndead
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

        public override BonePileType BonePile { get { return BonePileType.Small; } }

        public override int DifficultyValue { get { return 4; } }

		[Constructable]
		public UOACZVampireBat() : base()
		{
            Name = "a vampire bat";
            Body = 317;
            Hue = 2105;
            BaseSoundID = 0x642;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(150);

            SetDamage(7, 14);

            SetSkill(SkillName.Wrestling, 65);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 500;
            Karma = -500;                 
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void SetTamedAI()
        {
            base.SetTamedAI();

            SetHits(125);

            SetDamage(6, 12);

            SetSkill(SkillName.Wrestling, 55);            
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
            
            double effectChance = .1;

            if (ControlMaster is PlayerMobile)
                effectChance = .02;

            if (Utility.RandomDouble() <= effectChance)
            {
                int healingAmount = (int)((double)HitsMax * .25);

                Hits += healingAmount;

                this.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);

                Blood blood = new Blood();
                blood.MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z + 1), Map);

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, DamageMax, 8.0, 0x44D, true, "", "The creature sinks its fangs into you, healing itself and causing you to bleed!", "-1");
            }            
        }

        public UOACZVampireBat(Serial serial): base(serial)
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
