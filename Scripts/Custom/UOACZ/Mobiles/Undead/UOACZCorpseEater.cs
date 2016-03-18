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
    [CorpseName("a corpse eater corpse")]
    public class UOACZCorpseEater : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 2; } }

		[Constructable]
		public UOACZCorpseEater() : base()
		{
            Name = "a corpse eater";
            Body = 732;
            Hue = 0;
            BaseSoundID = 268;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(125);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 50);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;                 
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();

            DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 5;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .2)
            {
                defender.SendMessage("The creature burrows inside of you, causing you immense pain and discomfort!");

                double damage = 30;

                if (defender is BaseCreature)
                    damage *= 3;

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 30, -1, true, "", "");

                Effects.PlaySound(Location, Map, 0x4F1);

                new Blood().MoveToWorld(new Point3D(defender.X, defender.Y, defender.Z), defender.Map);

                for (int a = 0; a < 4; a++)
                {
                    new Blood().MoveToWorld(new Point3D(defender.X + Utility.RandomMinMax(-1, 1), defender.Y + Utility.RandomMinMax(-1, 1), defender.Z), defender.Map);
                }

                Kill();
            }
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetAngerSound() { return 0x581; }
        public override int GetIdleSound() { return 0x582; }
        public override int GetAttackSound() { return 0x580; }
        public override int GetHurtSound() { return 0x5DA; }
        public override int GetDeathSound() { return 0x57F; }

        public UOACZCorpseEater(Serial serial): base(serial)
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
