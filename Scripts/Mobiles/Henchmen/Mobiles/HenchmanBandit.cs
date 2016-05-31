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
    public class HenchmanBandit : BaseHenchman
	{
        public override string[] recruitSpeech  { get { return new string[] {   "Together, we'll leave no highway safe!",
                                                                                };
            }
        }

        public override string[] idleSpeech { get { return new string[] {       "I hope you have a big score in mind...",
                                                                                "My coin purse is a bit light lately..",
                                                                                "*spins dagger*",
                                                                                "*figdgets angrily*" 
                                                                                };}}

        public override string[] combatSpeech { get { return new string[] {     "Your money or yer life! Nah, both!",
                                                                                "Die! Die! Die!",
                                                                                "Leave nothing behind!",
                                                                                "I smell gold on them!" 
                                                                                };}}

        public DateTime m_NextDaggerAllowed;
        public TimeSpan NextDaggerDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(15, 30));

        public override HenchmanGroupType HenchmanGroup { get { return HenchmanGroupType.Villain; } }

		[Constructable]
		public HenchmanBandit() : base()
		{
            SpeechHue = Utility.RandomNeutralHue();

            Title = "the bandit";                

            SetStr(50);
            SetDex(50);
            SetInt(25);

            SetHits(200);

            SetDamage(6, 12);

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
            ControlSlots = 1;
            MinTameSkill = 100;
		}

        public override string TamedDisplayName { get { return "Bandit"; } }

        public override int TamedItemId { get { return 8454; } }
        public override int TamedItemHue { get { return 0; } }
        public override int TamedItemXOffset { get { return 5; } }
        public override int TamedItemYOffset { get { return 0; } }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedBaseMaxHits { get { return 200; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }

        public override double TamedBaseWrestling { get { return 85; } }
        public override double TamedBaseArchery { get { return 85; } }
        public override double TamedBaseFencing { get { return 85; } }
        public override double TamedBaseMacing { get { return 85; } }
        public override double TamedBaseSwords { get { return 85; } }

        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 75; } }
        public override int TamedBaseDex { get { return 50; } }
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
            int colorTheme = Utility.RandomSkinHue();
            int customTheme = 0;

            if (Female)
            {
                switch (Utility.RandomMinMax(1, 3))
                {
                    case 1: m_Items.Add(new ShortPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                    case 2: m_Items.Add(new LongPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                    case 3: m_Items.Add(new Skirt(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                }
            }

            else
            {
                switch (Utility.RandomMinMax(1, 2))
                {
                    case 1: m_Items.Add(new ShortPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                    case 2: m_Items.Add(new LongPants(Utility.RandomNeutralHue()) { LootType = LootType.Blessed }); break;
                }
            }

            switch (Utility.RandomMinMax(1, 2))
            {
                case 1: m_Items.Add(new FancyShirt(Utility.RandomDyedHue()) { LootType = LootType.Blessed }); break;
                case 2: m_Items.Add(new Shirt(Utility.RandomDyedHue()) { LootType = LootType.Blessed }); break;
            }

            m_Items.Add(new Boots() { LootType = LootType.Blessed });
            m_Items.Add(new StuddedGloves() { LootType = LootType.Blessed });

            switch (Utility.RandomMinMax(1, 5))
            {
                case 1: { m_Items.Add(new Dagger() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { LootType = LootType.Blessed }); break; }
                case 2: { m_Items.Add(new Club() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { LootType = LootType.Blessed }); break; }
                case 3: { m_Items.Add(new Kryss() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { LootType = LootType.Blessed }); break; }
                case 4: { m_Items.Add(new WarFork() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { LootType = LootType.Blessed }); break; }
                case 5: { m_Items.Add(new Longsword() { LootType = LootType.Blessed }); m_Items.Add(new WoodenShield() { LootType = LootType.Blessed }); break; }
            }

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
        }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
            PvPMeleeDamageScalar = BaseCreature.BasePvPMeleeDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
            PvPAbilityDamageScalar = BaseCreature.BasePvPAbilityDamageScalar * BaseCreature.BasePvPHenchmenDamageScalar;
        }

        public override void OnThink()
        {
            base.OnThink();

            if (DateTime.UtcNow > m_NextDaggerAllowed && !CantWalk && !Frozen && !Hidden && Alive && Utility.RandomDouble() < .1)
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
        
        public HenchmanBandit(Serial serial): base(serial)
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
