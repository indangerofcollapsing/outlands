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
    [CorpseName("a bloody zombie corpse")]
    public class UOACZBloodyZombie : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "brainssss.....",
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
		public UOACZBloodyZombie() : base()
		{
            Name = "a bloody zombie";
            Body = 3;
            Hue = 1779;
            BaseSoundID = 471;

            SetStr(50);
            SetDex(25);
            SetInt(25);

            SetHits(300);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 55);
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

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);

            int bloodItem = Utility.RandomMinMax(1, 2);

            for (int a = 0; a < bloodItem; a++)
            {
                new Blood().MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
            }            
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            SpecialAbilities.BleedSpecialAbility(0.2, this, defender, DamageMax, 8.0, Utility.RandomList(0x5D9, 0x5DB), true, "", "Their attack causes you to bleed!");
        }

        protected override bool OnMove(Direction d)
        {
            for (int a = 0; a < 1; a++)
            {
                new Blood().MoveToWorld(new Point3D(this.X + Utility.RandomMinMax(-1, 1), this.Y + Utility.RandomMinMax(-1, 1), this.Z), this.Map);
            }

            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));            

            return base.OnMove(d);
        }

        public override void OnDeath(Container c)
        {
            Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));

            int bloodItem = Utility.RandomMinMax(3, 5);

            for (int a = 0; a < bloodItem; a++)
            {
                new Blood().MoveToWorld(new Point3D(Location.X + Utility.RandomMinMax(-1, 1), Location.Y + Utility.RandomMinMax(-1, 1), Location.Z), Map);
            }            

            base.OnDeath(c);
        }  

        public UOACZBloodyZombie(Serial serial): base(serial)
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
