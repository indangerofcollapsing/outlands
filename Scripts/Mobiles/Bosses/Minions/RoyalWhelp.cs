using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;

namespace Server.Mobiles
{
    [CorpseName("a royal whelp corpse")]
	public class RoyalWhelp : BaseCreature
	{
		[Constructable]
		public RoyalWhelp () : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
		{
            Name = "a royal whelp";

            Body = 718;
            Hue = 2504;

            BaseSoundID = 0x646;

            SetStr(50);
            SetDex(75);
            SetInt(25);

            SetHits(250);

            SetDamage(8, 16);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 50;            

            Fame = 1500;
            Karma = -1500;
        }

        public override int Meat { get { return 1; } }
        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Horned; } }
        public override bool CanFly { get { return true; } }

        public override void SetUniqueAI()
        {
            CombatSpecialActionMinDelay = 10;
            CombatSpecialActionMaxDelay = 20;
            
            DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 50;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1;
        }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int GetDeathSound() { return 0x2CD;}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

		public RoyalWhelp( Serial serial ) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
            base.Serialize(writer);
            writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
            base.Deserialize(reader);
			int version = reader.ReadInt();
        }
	}
} 
