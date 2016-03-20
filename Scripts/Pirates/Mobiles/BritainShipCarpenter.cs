using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom.Pirates
{
    public class BritainShipCarpenter : OceanBaseCreature
	{
        public DateTime m_NextRepairCheck;
        public TimeSpan NextRepairCheckDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextRepairAllowed;
        public TimeSpan NextRepairDelay = TimeSpan.FromSeconds(60);
        
        public override string[] idleSpeech { get { return new string[] {       "*puts nail between teeth*",
                                                                                "*inspects board*",
                                                                                "*twirls hammer*",
                                                                                "*wipes sweat off brow*"
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Ey! Watch the woodwork!",
                                                                                "Drat! I just fixed that!",
                                                                                "We're going to need more nails!",
                                                                                "I'm paid to hammer boards, not heads!" 
                                                                                };}}        

		[Constructable]
		public BritainShipCarpenter() : base()
		{
            SpeechHue = 0;
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                Title = "the ship carpenter";
                Body = 0x191;
                Name = NameList.RandomName("female");
            }

            else
            {
                Title = "the ship carpenter";
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(300);

            SetDamage(6, 12);

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Carpentry, 80);

            VirtualArmor = 25;

            Fame = 500;
			Karma = 3000;

            Utility.AssignRandomHair(this, Utility.RandomHairHue());

            AddItem(new ShortPants() { Movable = true, Hue = 0 });
            AddItem(new HalfApron() { Movable = true, Hue = 0 });
            AddItem(new LeatherGloves() { Movable = true, Hue = 0 });
            AddItem(new LeatherArms() { Movable = true, Hue = 0 });
            AddItem(new Shoes() { Movable = true, Hue = 0 });

            AddItem(new Hatchet() { Movable = true, Hue = 0 });

            PackItem(new Bow() { Movable = true, Hue = 0 });
		}

        public override void SetUniqueAI()
        {            
            DictCombatAction[CombatAction.AttackOnly] = 1;
            DictCombatAction[CombatAction.CombatSpecialAction] = 3;

            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;

            DictCombatRange[CombatRange.WeaponAttackRange] = 1;
            DictCombatRange[CombatRange.SpellRange] = 0;
            DictCombatRange[CombatRange.Withdraw] = 10;
        }

        public override int OceanDoubloonValue { get { return 6; } }

        public override void OnThink()
        {
            base.OnThink();
           
            if (DateTime.UtcNow > m_NextRepairCheck)
            {
                m_NextRepairCheck = DateTime.UtcNow + NextRepairCheckDelay;

                BaseBoat m_Boat = BaseBoat.FindBoatAt(this.Location, this.Map);
                    
                if (DateTime.UtcNow > m_NextRepairAllowed && CheckIfBoatValid(m_Boat))
                {
                    if (m_Boat.HitPoints < m_Boat.MaxHitPoints || m_Boat.SailPoints < m_Boat.MaxSailPoints || m_Boat.GunPoints < m_Boat.MaxGunPoints)
                    {
                        Say("*begins repairing the ship*");

                        Effects.PlaySound(this.Location, this.Map, 0x23D);
                        Animate(12, 5, 1, true, false, 0);

                        double repairInterval = 3.5;

                        m_NextRepairAllowed = DateTime.UtcNow + NextRepairDelay;

                        AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(repairInterval);
                        NextCombatTime = NextCombatTime + TimeSpan.FromSeconds(repairInterval);

                        NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(repairInterval);
                        NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(repairInterval);
                        NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(repairInterval);
                        NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(repairInterval);

                        for (int a = 0; a < 4; a++)
                        {
                            Timer.DelayCall(TimeSpan.FromSeconds((a + 1) * 3), delegate
                            {
                                if (this == null) return;                                        
                                if (!this.Alive || this.Deleted) return;

                                if (CheckIfBoatValid(m_Boat))
                                {
                                    Effects.PlaySound(Location, Map, 0x23D);
                                    Animate(12, 5, 1, true, false, 0);

                                    AIObject.NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(repairInterval);
                                    NextCombatTime = DateTime.UtcNow + TimeSpan.FromSeconds(repairInterval);

                                    NextSpellTime = NextSpellTime + TimeSpan.FromSeconds(repairInterval);
                                    NextCombatHealActionAllowed = NextCombatHealActionAllowed + TimeSpan.FromSeconds(repairInterval);
                                    NextCombatSpecialActionAllowed = NextCombatSpecialActionAllowed + TimeSpan.FromSeconds(repairInterval);
                                    NextCombatEpicActionAllowed = NextCombatEpicActionAllowed + TimeSpan.FromSeconds(repairInterval);
                                }
                            });
                        }

                        Timer.DelayCall(TimeSpan.FromSeconds(15), delegate
                        {
                            if (this == null) return;
                            if (!this.Alive || this.Deleted) return;

                            if (CheckIfBoatValid(m_Boat))
                            {
                                Effects.PlaySound(Location, Map, 0x23D);
                                Animate(12, 5, 1, true, false, 0);

                                if (m_Boat.HitPoints < m_Boat.MaxHitPoints || m_Boat.SailPoints < m_Boat.MaxSailPoints || m_Boat.GunPoints < m_Boat.MaxGunPoints)
                                {
                                    int hitPointsRepairable = (int)(.05 * (double)m_Boat.MaxHitPoints);
                                    int sailPointsRepairable = (int)(.10 * (double)m_Boat.MaxSailPoints);
                                    int gunPointsRepairable = (int)(.10 * (double)m_Boat.MaxGunPoints);

                                    m_Boat.HitPoints += hitPointsRepairable;
                                    m_Boat.SailPoints += sailPointsRepairable;
                                    m_Boat.GunPoints += gunPointsRepairable;

                                    Say("Repairs completed!");
                                }
                            }
                        });
                    }
                }                    
            }            
        }

        public bool CheckIfBoatValid(BaseBoat boat)
        {
            if (boat == null) return false;
            if (boat.HitPoints == 0 || boat.Deleted) return false;

            if (boat.m_SinkTimer != null)
            {
                if (boat.m_SinkTimer.Running)
                    return false;
            }   

            return true;
        }

        public BritainShipCarpenter(Serial serial): base(serial)
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
