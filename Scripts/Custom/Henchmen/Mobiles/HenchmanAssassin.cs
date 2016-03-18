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
    public class HenchmanAssassin : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "Just point me at what needs to die.",
                                                                                };
            }
        }

        public override string[] idleSpeech { get { return new string[] {       "What shall I eliminate next?",
                                                                                "Give me something challenging to kill.",
                                                                                "*sharpens blade*",
                                                                                "*eyes narrow*" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "This is going to hurt. Alot.",
                                                                                "Resist. Surrender. I care not.",
                                                                                "No one will escape my blade.",
                                                                                "Your death will line my pockets." 
                                                                                };}}

        public DateTime m_NextStealthCheckAllowed = DateTime.UtcNow;
        public TimeSpan StealthDelay = TimeSpan.FromSeconds(10);

        public DateTime m_NextDaggerAllowed;
        public TimeSpan NextDaggerDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(15, 25));

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Villain; } }

		[Constructable]
		public HenchmanAssassin() : base()
		{
            SpeechHue = Utility.RandomNeutralHue();

            Title = "the assassin";      

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

            Tamable = true;
            ControlSlots = 3;
            MinTameSkill = 100;
		}
        
        //Animal Lore Display Info
        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 16; } }
        public override int TamedBaseMaxDamage { get { return 18; } }

        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseArchery { get { return 95; } }
        public override double TamedBaseFencing { get { return 95; } }
        public override double TamedBaseMacing { get { return 95; } }
        public override double TamedBaseSwords { get { return 95; } }

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
                    case 1: m_Items.Add(new Skirt() {Hue = colorTheme, LootType = LootType.Blessed }); break;
                }
            }

            m_Items.Add(new Cloak() { Hue = customHue, LootType = LootType.Blessed });
            m_Items.Add(new Sandals() { Hue = colorTheme, LootType = LootType.Blessed });            
            m_Items.Add(new Kilt() { Hue = colorTheme, LootType = LootType.Blessed });
            m_Items.Add(new BodySash() { Hue = colorTheme, LootType = LootType.Blessed });
            m_Items.Add(new LeatherArms() { Hue = colorTheme, LootType = LootType.Blessed });
            m_Items.Add(new LeatherGloves() { Hue = colorTheme, LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 4))
            {
                case 1: m_Items.Add(new Dagger() { Hue = weaponHue, LootType = LootType.Blessed }); break;
                case 2: m_Items.Add(new Kryss() { Hue = weaponHue, LootType = LootType.Blessed }); break;
                case 3: m_Items.Add(new WarFork() { Hue = weaponHue, LootType = LootType.Blessed }); break;
                case 4: m_Items.Add(new Katana() { Hue = weaponHue, LootType = LootType.Blessed }); break;
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
                m_NextDaggerAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(8, 12));
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
                        SpecialAbilities.HinderSpecialAbility(1.0, null, this, 1, 1.5, true, Utility.RandomList(0x5D2, 0x5D3), false, "", "");

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

                                DisplayFollowerDamage(combatant, finalAdjustedDamage);
                            });
                        });
                    }
                }
            }
        }
        
        public HenchmanAssassin(Serial serial): base(serial)
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
