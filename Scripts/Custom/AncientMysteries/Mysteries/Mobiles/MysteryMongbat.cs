using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a mysterious mongbat corpse" )]
	public class MysteryMongbat : BaseCreature
	{
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextAIChangeAllowed;
        public TimeSpan NextAIChangeDelay = TimeSpan.FromSeconds(30);

        public DateTime m_NextReflectAllowed;
        public double NextReflectDelay = Utility.Random(1, 5);

        public bool IsMasterMongbat = false;

		[Constructable]
		public MysteryMongbat() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a mysterious mongbat";
			Body = 39;
			BaseSoundID = 422;

            Hue = 2587;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(400);
            SetMana(2000);

            SetDamage(10, 20);

            SetSkill(SkillName.Wrestling, 80);
            SetSkill(SkillName.Tactics, 100);          

            SetSkill(SkillName.MagicResist, 150);

            SetSkill(SkillName.Magery, 50);
            SetSkill(SkillName.EvalInt, 50);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            VirtualArmor = 25;

			Fame = 150;
			Karma = -150;

            //-------

            m_AncientMysteryCreature = true;
        }

        public override bool BardImmune { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override TimeSpan AutoDispelCooldown { get { return TimeSpan.FromSeconds(Utility.RandomMinMax(4, 6)); } }
        public override Poison PoisonImmune { get { return Poison.Lethal; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool RevealImmune { get { return true; } }

        public override int Meat { get { return 1; } }

        public override void SetUniqueAI()
        {           
            UniqueCreatureDifficultyScalar = 2.5;
            
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;

            switch(Utility.RandomMinMax(1, 3))
            {
                case 1: DictCombatSpecialAction[CombatSpecialAction.FireBreathAttack] = 1; break;
                case 2: DictCombatSpecialAction[CombatSpecialAction.IceBreathAttack] = 1; break;
                case 3: DictCombatSpecialAction[CombatSpecialAction.PoisonBreathAttack] = 1; break;
            }

            DictCombatRange[CombatRange.Withdraw] = 10;
            CreatureWithdrawRange = 12;

            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 18;
        }        

        public override void OnThink()
        {
            base.OnThink();

            if (Paralyzed)
                Paralyzed = false;

            if (Frozen)
                Frozen = false;

            if (CantWalk)
                CantWalk = false;

            if (!Hidden && DateTime.UtcNow > m_NextReflectAllowed && MagicDamageAbsorb < 1)
            {
                FixedParticles(0x375A, 10, 15, 5037, EffectLayer.Waist);
                MagicDamageAbsorb = 1;

                PlaySound(0x1E9);

                m_NextReflectAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(NextReflectDelay);
            }

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed)
            {
                if (Combatant != null && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 3, 6, true, null))
                    {
                        PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                        switch (Utility.RandomMinMax(1, 4))
                        {
                            case 1:
                                DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 10;
                                DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                            break;

                            case 2:
                                DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                                DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                            break;

                            case 3:
                                DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;
                                DictCombatTargetingWeight[CombatTargetingWeight.Player] = 0;
                            break;

                            case 4:
                                DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                DictCombatTargetingWeight[CombatTargetingWeight.Player] = 10;
                            break;
                        }
                    }

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }            
        }

        public override bool OnBeforeDeath()
        {
            if (IsMasterMongbat)            
                Boss = true;            

            return base.OnBeforeDeath();
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath(c);
            
            c.AddItem(new ExplodingMongbat());

            if (IsMasterMongbat)
            {
                int treasurePileLevel = 1;

                if (Utility.RandomMinMax(1, 3) == 1)
                    treasurePileLevel = 2;

                if (Utility.RandomMinMax(1, 6) == 1)
                    treasurePileLevel = 3;

                if (Utility.RandomMinMax(1, 10) == 1)
                    treasurePileLevel = 4;

                switch (treasurePileLevel)
                {
                    case 1: c.AddItem(new TreasurePileSmallAddonDeed()); break;
                    case 2: c.AddItem(new TreasurePileMediumAddonDeed()); break;
                    case 3: c.AddItem(new TreasurePileLargeAddonDeed()); break;
                    case 4: c.AddItem(new TreasurePileHugeAddonDeed()); break;
                }
            }
        }

        public MysteryMongbat(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );

            //Version 0
            writer.Write(IsMasterMongbat);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();    
        
            //Version 0 
            if (version >= 0)
            {
                IsMasterMongbat = reader.ReadBool();
            }
		}
	}
}
