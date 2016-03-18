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
    [CorpseName("a spectre corpse")]
    public class UOACZSpectre : UOACZBaseUndead
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

        public override int DifficultyValue { get { return 2; } }

        public DateTime m_NextStealthCheckAllowed = DateTime.UtcNow;
        public TimeSpan StealthDelay = TimeSpan.FromSeconds(10);

		[Constructable]
		public UOACZSpectre() : base()
		{
            Name = "a spectre";
            Body = 26;
            Hue = 0x4001;
            BaseSoundID = 0x482;

            SetStr(25);
            SetDex(50);
            SetInt(25);

            SetHits(150);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 70);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);

            VirtualArmor = 25;

            Fame = 4000;
            Karma = -4000;                    
		}

        public override void SetUniqueAI()
        {
            base.SetUniqueAI();
            
            DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 5;

            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 3;
        }

        public override void SetTamedAI()
        {
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 1;

            SetSkill(SkillName.Hiding, 120);
            SetSkill(SkillName.Stealth, 120);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Alive && Controlled && ControlMaster is PlayerMobile && ControlOrder != OrderType.Stop)
            {
                if (Hidden || Combatant != null)
                    m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;

                else if (DateTime.UtcNow > m_NextStealthCheckAllowed)
                {
                    bool stealthValid = true;

                    if (Combatant != null)
                        stealthValid = false;

                    if (stealthValid)
                    {
                        IPooledEnumerable eable = Map.GetMobilesInRange(Location, RangePerception);

                        foreach (Mobile mobile in eable)
                        {
                            if (mobile.InLOS(this) && mobile.CanSee(this))
                            {
                                if (mobile.Combatant == this)
                                {
                                    stealthValid = false;
                                    break;
                                }

                                bool aggressive = false;

                                foreach (AggressorInfo aggressorInfo in mobile.Aggressors)
                                {
                                    if (aggressorInfo.Attacker == this || aggressorInfo.Defender == this)
                                    {
                                        aggressive = true;
                                        break;
                                    }
                                }

                                if (aggressive)
                                {
                                    stealthValid = false;
                                    break;
                                }
                            }
                        }

                        eable.Free();
                    }

                    if (stealthValid)
                    {
                        AIMiscAction.DoStealth(this);
                        m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;
                    }
                }
            }
        }

        public UOACZSpectre(Serial serial): base(serial)
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
