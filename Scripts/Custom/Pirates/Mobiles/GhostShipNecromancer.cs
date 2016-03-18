using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.ContextMenus;
using Server.Multis;
using Server.Misc;
using Server.Network;

namespace Server.Custom.Pirates
{
    public class GhostShipNecromancer : OceanBaseCreature
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

        public DateTime m_NextPotionThrowAllowed;
        public TimeSpan NextPotionThrowDelay = TimeSpan.FromSeconds(10);

		[Constructable]
		public GhostShipNecromancer() : base()
		{
			SpeechHue = 2051;
            Hue = 1150;

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");

                Title = "the necromancer";
            }

            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");

                Title = "the necromancer";
            }

            SetStr(50);
            SetDex(50);
            SetInt(100);

            SetHits(400);

            SetDamage(4, 8);

            SetSkill(SkillName.Wrestling, 85);

            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.Magery, 100);
            SetSkill(SkillName.EvalInt, 100);
            SetSkill(SkillName.Meditation, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 25;

            Utility.AssignRandomHair(this, 1150);

            AddItem(new BoneHelm() { Movable = false, Hue = 0 });
            AddItem(new StuddedGorget() { Movable = false, Hue = 2051 });
            AddItem(new StuddedArms() { Movable = false, Hue = 2051 });
            AddItem(new BoneGloves() { Movable = false, Hue = 0 });
            AddItem(new Skirt() { Movable = false, Hue = 2051 });

            AddItem(new Spellbook() { Movable = false, Hue = 2630 });
		}

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.25;

            DictCombatAction[CombatAction.CombatSpecialAction] = 3;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }

        public override int OceanDoubloonValue { get { return 8; } }
        public override bool AlwaysMurderer { get { return true; } }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (DateTime.UtcNow > m_NextPotionThrowAllowed && AIObject.currentCombatRange != CombatRange.Withdraw && AIObject.Action != ActionType.Flee)
                {
                    if (Spell != null)
                        return;

                    Mobile combatant = this.Combatant;

                    if (combatant != null && !BardPacified)
                    {
                        BaseBoat thisBoat = BaseBoat.FindBoatAt(Location, Map);
                        BaseBoat combatantBoat = BaseBoat.FindBoatAt(combatant.Location, combatant.Map);

                        //Target isn't on a boat or is on a different boat than this one
                        if (combatantBoat == null || (thisBoat != null && combatantBoat != null && thisBoat != combatantBoat))
                        {
                            if (combatant.Alive && this.InLOS(combatant) && this.GetDistanceToSqrt(combatant) <= 10)
                            {
                                int potionType = Utility.RandomMinMax(1, 4);

                                switch (potionType)
                                {
                                    case 1: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Explosion, 1, 10, 15, 1, 0, true, true); break;
                                    case 2: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Poison, 1, 8, 12, Utility.RandomMinMax(1, 2), 1, true, true); break;
                                    case 3: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Frost, 1, 8, 12, .2, 10, true, true); break;
                                    case 4: SpecialAbilities.ThrowPotionAbility(this, combatant, 1.5, 1.5, PotionAbilityEffectType.Shrapnel, 1, 8, 12, 10, 0, true, true); break;
                                }

                                m_NextPotionThrowAllowed = DateTime.UtcNow + NextPotionThrowDelay;
                            }
                        }
                    }
                }
            }
        }

        public GhostShipNecromancer(Serial serial): base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); //Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}