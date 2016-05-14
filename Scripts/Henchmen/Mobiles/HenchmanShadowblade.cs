using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;
using Server.Mobiles;

namespace Server.Custom
{
    public class HenchmanShadowblade : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "The very night itself is at your call...",
                                                                                };
            }
        }

        public override string[] idleSpeech { get { return new string[] {       "Death. Darkness. Despair.",
                                                                                "Come with me into darkness.",
                                                                                "*whispers*",
                                                                                "*peers into the shadows*" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Sleep, friend.",
                                                                                "Night has come for you.",
                                                                                "All things must return to darkness.",
                                                                                "Light will not save you." 
                                                                                };}}
        public DateTime m_NextStealthCheckAllowed = DateTime.UtcNow;
        public TimeSpan StealthDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextFlurryAllowed = DateTime.UtcNow;
        public TimeSpan FlurryDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        public DateTime m_NextDaggerAllowed;
        public TimeSpan NextDaggerDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(10, 20));

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Villain; } }

		[Constructable]
		public HenchmanShadowblade() : base()
		{
            SpeechHue = Utility.RandomNeutralHue();

            Title = "the shadowblade";                 

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(6, 12);

            AttackSpeed = 30;

            SetSkill(SkillName.Archery, 70);
            SetSkill(SkillName.Fencing, 70);
            SetSkill(SkillName.Swords, 70);
            SetSkill(SkillName.Macing, 70);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Parry, 5);

            SetSkill(SkillName.MagicResist, 50);

            VirtualArmor = 25;

			Fame = 1000;
			Karma = -2000;

            Tameable = true;
            ControlSlots = 5;
            MinTameSkill = 120;
		}
        
        //Animal Lore Display Info
        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 28; } }
        public override int TamedBaseMaxDamage { get { return 30; } }

        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseArchery { get { return 100; } }
        public override double TamedBaseFencing { get { return 100; } }
        public override double TamedBaseMacing { get { return 100; } }
        public override double TamedBaseSwords { get { return 100; } }

        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 75; } }
        public override int TamedBaseDex { get { return 100; } }
        public override int TamedBaseInt { get { return 25; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 100; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override void GenerateItems()
        {
            int colorTheme = Utility.RandomMetalHue();
            int customHue = 1107;
            int weaponHue = 2051;

            if (Female)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: m_Items.Add(new Skirt(colorTheme) { LootType = LootType.Blessed }); break;
                }
            }
            m_Items.Add(new ChainmailChest() { LootType = LootType.Blessed });

            m_Items.Add(new PlateGorget() { Hue = customHue, LootType = LootType.Blessed });          
            m_Items.Add(new RingmailArms() { Hue = customHue, LootType = LootType.Blessed });
            m_Items.Add(new RingmailGloves() { Hue = customHue, LootType = LootType.Blessed });
            m_Items.Add(new ChainmailLegs() { Hue = customHue, LootType = LootType.Blessed });
            m_Items.Add(new Boots() { Hue = customHue, LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 3))
            {
                case 1: m_Items.Add(new WarCleaver() { LootType = LootType.Blessed, Speed = 30, Hue = weaponHue, Name = "a reaverblade" }); break;
                case 2: m_Items.Add(new RadiantScimitar() { LootType = LootType.Blessed, Speed = 30, Hue = weaponHue, Name = "a greatblade" }); break;
                case 3: m_Items.Add(new NoDachi() { LootType = LootType.Blessed, Speed = 30, Hue = weaponHue, Name = "a warblade" }); break;
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override void SetUniqueAI()
        {

        }

        public override void SetTamedAI()
        {
            DictWanderAction[WanderAction.None] = 1;
            DictWanderAction[WanderAction.Stealth] = 2;

            SetSkill(SkillName.Hiding, 100);
            SetSkill(SkillName.Stealth, 100);

            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;

            BackstabDamageScalar = BaseCreature.TamedCreatureBackstabScalar;
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;
            
            double effectChance = .1;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = 0.0;

                    else
                        effectChance = .1;
                }
            }

            if (m_NextFlurryAllowed <= DateTime.UtcNow)
            {
                if (Utility.RandomDouble() <= effectChance)
                {
                    m_NextFlurryAllowed = DateTime.UtcNow + FlurryDelay;

                    SpecialAbilities.FrenzySpecialAbility(1.0, this, defender, 1.0, 5, -1, true, "", "", "*becomes a whirling fury of blades*");

                    AIObject.NextMove = DateTime.UtcNow;
                    LastSwingTime = DateTime.UtcNow;
                }
            }

            m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            if (!willKill)
                m_NextStealthCheckAllowed = DateTime.UtcNow + StealthDelay;

            base.OnDamage(amount, from, willKill);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Hidden)
            {
                m_NextDaggerAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(5, 10));
            }

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

            if (DateTime.UtcNow > m_NextDaggerAllowed && !CantWalk && !Frozen && !Hidden && Alive)
            {
                Mobile combatant = Combatant;

                if (combatant != null)
                {
                    if (combatant.Alive && CanBeHarmful(combatant) && InLOS(combatant) && GetDistanceToSqrt(combatant) <= 8)
                    {
                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, 1.5, true, Utility.RandomList(0x5D2, 0x5D3), false, "", "", "-1");

                        m_NextDaggerAllowed = DateTime.UtcNow + NextDaggerDelay;

                        Animate(31, 7, 1, true, false, 0);

                        Timer.DelayCall(TimeSpan.FromSeconds(.475), delegate
                        {
                            if (this == null) return;
                            if (!Alive || Deleted) return;
                            if (Combatant == null) return;
                            if (!Combatant.Alive || Combatant.Deleted) return;

                            MovingEffect(combatant, 3921, 18, 1, false, false);

                            double distance = this.GetDistanceToSqrt(combatant.Location);
                            double destinationDelay = (double)distance * .08;

                            Timer.DelayCall(TimeSpan.FromSeconds(destinationDelay), delegate
                            {
                                if (this == null) return;
                                if (!Alive || Deleted) return;
                                if (Combatant == null) return;
                                if (!Combatant.Alive || Combatant.Deleted) return;
                                if (!CanBeHarmful(Combatant)) return;

                                Effects.PlaySound(Location, Map, 0x145);

                                double baseDamage = (double)DamageMax;

                                if (Combatant is PlayerMobile)
                                    baseDamage *= BaseCreature.BreathDamageToPlayerScalar * PvPAbilityDamageScalar;

                                if (Combatant is BaseCreature)
                                    baseDamage *= BaseCreature.BreathDamageToCreatureScalar;

                                int finalDamage = (int)baseDamage;

                                if (finalDamage < 1)
                                    finalDamage = 1;

                                DoHarmful(combatant);

                                int finalAdjustedDamage = AOS.Damage(combatant, this, finalDamage, 100, 0, 0, 0, 0);
                                new Blood().MoveToWorld(combatant.Location, combatant.Map);
                            });
                        });
                    }
                }
            }
        }
        
        public HenchmanShadowblade(Serial serial): base(serial)
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
