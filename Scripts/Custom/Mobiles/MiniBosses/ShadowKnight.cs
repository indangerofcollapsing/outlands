using System;
using Server;
using Server.Items;
using Server.Network;
using System.Collections.Generic;

namespace Server.Mobiles
{
	[CorpseName( "the shadow knight's corpse" )]
	public class ShadowKnight : BaseCreature
	{
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public List<Point3D> m_BackupLocations = new List<Point3D>();

        public override Loot.LootTier LootTier { get { return Loot.LootTier.Eight; } }

		[Constructable]
		public ShadowKnight() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            Name = "The Shadow Knight";
			Body = 311;

            SetStr(100);
            SetDex(75);
            SetInt(25);

            SetHits(5000);
            SetStam(3000);

            SetDamage(30, 50);

            SetSkill(SkillName.Wrestling, 110);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            VirtualArmor = 25;

			Fame = 25000;
			Karma = -25000;

            SetBackupLocations();
		}

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 1.35;

            ResolveAcquireTargetDelay = 0.5;
            RangePerception = 24;
        }

        public void SetBackupLocations()
        {
            m_BackupLocations = new List<Point3D>();

            #region Locations

            m_BackupLocations.Add(new Point3D(5272, 684, 6));
            m_BackupLocations.Add(new Point3D(5280, 684, 5));
            m_BackupLocations.Add(new Point3D(5272, 667, 6));
            m_BackupLocations.Add(new Point3D(5280, 667, 5));
            m_BackupLocations.Add(new Point3D(5268, 675, 0));
            m_BackupLocations.Add(new Point3D(5282, 676, 0));

            #endregion
        }

        public override bool AlwaysMiniBoss { get { return true; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool RevealImmune { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
                {
                    if (Combatant != null && !Paralyzed && !BardProvoked && !BardPacified)
                    {
                        if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, m_BackupLocations))
                        {
                            PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                            switch (Utility.RandomMinMax(1, 4))
                            {
                                case 1:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 2:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 3:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 4:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 10;
                                    break;
                            }
                        }

                        m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                    }
                }
            }
        }

        public override int GetAngerSound() { return Utility.RandomList(0x300, 0x301); }
        public override int GetIdleSound() { return Utility.RandomList(0x300, 0x301); }
        public override int GetHurtSound() { return Utility.RandomList(0x302, 0x303); }
        public override int GetDeathSound() { return 0x2FA; }
		public override int GetAttackSound()	{return 0x2BA;}

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (Utility.RandomMinMax(1, 50) == 1)
                c.AddItem(new ForgottenGravestone());
        }
		
        public ShadowKnight( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            SetBackupLocations();
		}
	}
}