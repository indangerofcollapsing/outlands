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
    [CorpseName("a rotting corpse")]
    public class UOACZRottingCorpse : UOACZBaseUndead
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

        public override BonePileType BonePile { get { return BonePileType.Medium; } }

        public override int DifficultyValue { get { return 8; } }

		[Constructable]
		public UOACZRottingCorpse() : base()
		{
            Name = "a rotting corpse";
            Body = 155;
            BaseSoundID = 471;

            SetStr(75);
            SetDex(25);
            SetInt(25);

            SetHits(700);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Poisoning, 20);

            VirtualArmor = 25;

            Fame = 6000;
            Karma = -6000;

            Tameable = true;
            ControlSlots = 3;
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 5;

            ActiveSpeed = 0.6;
            PassiveSpeed = 0.7;

            ResolveAcquireTargetDelay = 3;
        }

        public override Poison HitPoison { get { return Poison.Greater; } }

        public UOACZRottingCorpse(Serial serial): base(serial)
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
