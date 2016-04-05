using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Spells;


namespace Server
{
    [CorpseName("a flaming corpse")]
    public class UOACZFlamingZombie : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "brainnnnnnsss...",
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

        public override int DifficultyValue { get { return 5; } }

		[Constructable]
		public UOACZFlamingZombie() : base()
		{
            Name = "a flaming zombie";
            Body = 3;
            Hue = 1359;
            BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(400);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Fame = 3500;
            Karma = -3500;

            Tameable = true;
            ControlSlots = 2;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 5;

            ActiveSpeed = 0.6;
            PassiveSpeed = 0.7;

            ResolveAcquireTargetDelay = 3;
        }        

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (Utility.RandomDouble() < .1)
            {
                Effects.PlaySound(Location, Map, 0x208);

                Point3D newPoint = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                SpellHelper.AdjustField(ref newPoint, Map, 12, false);

                UOACZFirefield fireField = new UOACZFirefield(this);
                fireField.MoveToWorld(newPoint, Map);
            }     
        }

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() < .15)
            {
                Effects.PlaySound(Location, Map, 0x208);

                UOACZFirefield fireField = new UOACZFirefield(this);

                fireField.MoveToWorld(Location, Map);
            }

            return base.OnMove(d);
        }

        public override bool OnBeforeDeath()
        {
            Effects.PlaySound(Location, Map, 0x208);

            int fireFields = Utility.RandomMinMax(2, 3);

            UOACZFirefield fireField = new UOACZFirefield(this);

            fireField.MoveToWorld(Location, Map);

            for (int a = 0; a < fireFields; a++)
            {
                Point3D newPoint = new Point3D(Location.X + Utility.RandomList(-1, 1), Location.Y + Utility.RandomList(-1, 1), Location.Z);
                SpellHelper.AdjustField(ref newPoint, Map, 12, false);

                new UOACZFirefield(this).MoveToWorld(newPoint, Map);
            }

            return base.OnBeforeDeath();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public UOACZFlamingZombie(Serial serial): base(serial)
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
