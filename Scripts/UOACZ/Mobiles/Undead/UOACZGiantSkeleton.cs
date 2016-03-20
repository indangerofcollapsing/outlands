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
    [CorpseName("a giant skeleton corpse")]
    public class UOACZGiantSkeleton : UOACZBaseUndead
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

        public override BonePileType BonePile { get { return BonePileType.Medium; } }

        public override int DifficultyValue { get { return 8; } }

		[Constructable]
		public UOACZGiantSkeleton() : base()
		{
            Name = "giant skeleton";

            Body = 308;
            Hue = 0;

            BaseSoundID = 0x48D;

            SetStr(100);
            SetDex(50);
            SetInt(25);

            SetHits(700);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            Fame = 10000;
            Karma = -10000;

            VirtualArmor = 250;

            Tameable = true;
            ControlSlots = 3;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double knockbackChance = .20;

            if (Utility.RandomDouble() <= knockbackChance)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*flings them aside*");

                double damage = DamageMax;

                SpecialAbilities.KnockbackSpecialAbility(1.0, Location, this, defender, damage, 10, -1, "", "The creature flings you aside!");

                Combatant = null;
            }
        }

        protected override bool OnMove(Direction d)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x11F, 0x120));

            if (Utility.RandomDouble() < .33)
            {
                TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);

                dirt.Name = "dirt";
                dirt.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            for (int a = 0; a < 20; a++)
            {
                TimedStatic bones = new TimedStatic(Utility.RandomList(6929, 6930, 6937, 6938, 6933, 6934, 6935, 6936, 6939, 6940, 6880, 6881, 6882, 6883), 5);

                bones.Name = "bones";
                
                Point3D bonesLocation = new Point3D(Location.X + Utility.RandomMinMax(-4, 4), Location.Y + Utility.RandomMinMax(-4, 4), Location.Z + 2);
                bones.MoveToWorld(bonesLocation, Map);
            }

            Effects.PlaySound(Location, Map, 0x1C7);

            return base.OnBeforeDeath();
        }

        public override int GetAngerSound() { return 0x4FE; }
        public override int GetIdleSound() { return 0x4ED; }
        public override int GetAttackSound() { return 0x627; }
        public override int GetHurtSound() { return 0x628; }
        public override int GetDeathSound() { return 0x489; }        

        public UOACZGiantSkeleton(Serial serial): base(serial)
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
