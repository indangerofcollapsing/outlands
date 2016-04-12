using System;
using Server.Mobiles;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "a giant rotworm corpse" )]
	public class GiantRotworm : BaseCreature
	{        
		[Constructable]
		public GiantRotworm() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a giant rotworm";

            Body = 287;
            Hue = 0;

            BaseSoundID = 0xDB;

            SetStr(100);
            SetDex(25);
            SetInt(25);

            SetHits(400);            

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 90);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            VirtualArmor = 25;

			Fame = 600;
			Karma = -600;

            Tameable = true;
            ControlSlots = 3;
            MinTameSkill = 115.1;
        }

        public override int TamedItemId { get { return 17038; } }
        public override int TamedItemHue { get { return 2509; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 400; } }
        public override int TamedBaseMinDamage { get { return 24; } }
        public override int TamedBaseMaxDamage { get { return 26; } }
        public override double TamedBaseWrestling { get { return 100; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 50; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 75; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.VerySlow; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override int PoisonResistance { get { return 4; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override int AttackAnimation { get { return Utility.RandomList(1, 5, 6); } }
        public override int AttackFrames { get { return 8; } }

        public override int HurtAnimation { get { return 10; } }
        public override int HurtFrames { get { return 6; } }

        public override int IdleAnimation { get { return 17; } }
        public override int IdleFrames { get { return 8; } }     
        
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .10;
                    else
                        effectChance = .67;
                }
            }

            BaseCreature bc_Defender = defender as BaseCreature;
            PlayerMobile pm_Defender = defender as PlayerMobile;

            double totalValue;

            if (bc_Defender != null)
            {
                bc_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease, out totalValue);

                if (totalValue > 0)
                    return;
            }

            if (pm_Defender != null)
            {
                pm_Defender.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Disease, out totalValue);

                if (totalValue > 0)
                    return;
            }

            if (Utility.RandomDouble() <= effectChance)
            {
                for (int a = 0; a < 3; a++)
                {
                    Blood disease = new Blood();
                    disease.Hue = 2052;
                    disease.Name = "disease";

                    Point3D diseaseLocation = new Point3D(X + Utility.RandomList(-1, 1), Y + Utility.RandomList(-1, 1), Z);
                    SpellHelper.AdjustField(ref diseaseLocation, Map, 12, false);

                    disease.MoveToWorld(diseaseLocation, defender.Map);
                }

                SpecialAbilities.DiseaseSpecialAbility(1.0, this, defender, 20, 60, 0x62B, true, "", "They has infected you with a horrific disease!");
            }
        }

        public override void OnDamage(int amount, Mobile from, bool willKill)
        {
            base.OnDamage(amount, from, willKill);
        }
        
        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() <= .25)
            {
                Blood blood = new Blood();
                blood.ItemID = Utility.RandomList(4651, 4652, 4653, 4654);
                blood.Hue = 2623;
                blood.Name = "slime";

                blood.MoveToWorld(Location, Map);

                Effects.PlaySound(Location, Map, Utility.RandomList(0x5D9, 0x5DB));
            }            

            return base.OnMove(d);
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetAngerSound() { return 0x581; }
        public override int GetIdleSound() { return 0x582; }
        public override int GetAttackSound() { return 0x580; }
        public override int GetHurtSound() { return 0x5DA; }
        public override int GetDeathSound() { return 0x57F; }

        public GiantRotworm(Serial serial): base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
            int version = reader.ReadInt();
		}
	}
}
