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
    [CorpseName("a shadow knight corpse")]
    public class UOACZShadowKnight : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 9; } }

        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(10);

		[Constructable]
		public UOACZShadowKnight() : base()
		{
            Name = "a shadow knight";
            Body = 311;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(700);

            SetDamage(12, 24);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            VirtualArmor = 25;

            Fame = 25000;
            Karma = -25000;                 
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 5;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, null))                        
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");   

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        public override int GetAngerSound() { return Utility.RandomList(0x300, 0x301); }
        public override int GetIdleSound() { return Utility.RandomList(0x300, 0x301); }
        public override int GetHurtSound() { return Utility.RandomList(0x302, 0x303); }
        public override int GetDeathSound() { return 0x2FA; }
        public override int GetAttackSound() { return 0x2BA; }

        public UOACZShadowKnight(Serial serial): base(serial)
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
