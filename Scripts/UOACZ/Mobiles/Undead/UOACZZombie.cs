using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Achievements;

namespace Server
{
    [CorpseName("a rotting corpse")]
    public class UOACZZombie : UOACZBaseUndead
	{
        public override string[] idleSpeech { get { return new string[] {       "brainnnnnnsss....",
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

        public override int DifficultyValue { get { return 1; } }

		[Constructable]
		public UOACZZombie() : base()
		{
            Name = "a zombie";
            Body = 3;
            BaseSoundID = 471;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 40);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

            Tameable = true;
            ControlSlots = 1;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 5;

            ActiveSpeed = 0.6;
            PassiveSpeed = 0.7;

            ResolveAcquireTargetDelay = 3;
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public UOACZZombie(Serial serial): base(serial)
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
