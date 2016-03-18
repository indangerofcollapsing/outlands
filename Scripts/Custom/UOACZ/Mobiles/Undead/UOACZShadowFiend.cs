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
    [CorpseName("a shadow fiend corpse")]
    public class UOACZShadowFiend : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 5; } }

		[Constructable]
		public UOACZShadowFiend() : base()
		{
            Name = "a shadow fiend";

            Body = 199;
            Hue = 2052;

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(250);

            SetDamage(9, 18);

            SetSkill(SkillName.Wrestling, 75);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            Fame = 1000;
            Karma = -1000;                    
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 5;

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 3;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (Utility.RandomDouble() <= .2)
            {
                TimedStatic darkEssence = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                darkEssence.Hue = 2051;
                darkEssence.Name = "dark essence";
                darkEssence.MoveToWorld(defender.Location, Map);

                Point3D newPoint = new Point3D(defender.Location.X + Utility.RandomList(-1, 1), defender.Location.Y + Utility.RandomList(-1, 1), defender.Location.Z);
                SpellHelper.AdjustField(ref newPoint, defender.Map, 12, false);

                darkEssence = new TimedStatic(Utility.RandomList(4650, 4651, 4652, 4653, 4654, 4655), 5);
                darkEssence.Hue = 2051;
                darkEssence.Name = "dark essence";
                darkEssence.MoveToWorld(newPoint, defender.Map);

                PlaySound(0x379);

                SpecialAbilities.StunSpecialAbility(1.0, this, defender, .05, 20, -1, false, "", "You have been cursed by dark energy and you feel less accurate with your swings.");
            }
        }

        public override int GetIdleSound() { return 0x37A; }
        public override int GetAngerSound() { return 0x379; }
        public override int GetDeathSound() { return 0x381; }
        public override int GetAttackSound() { return 0x37F; }
        public override int GetHurtSound() { return 0x380; }

        public UOACZShadowFiend(Serial serial): base(serial)
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
