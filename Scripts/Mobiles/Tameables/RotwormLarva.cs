using System;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a rotworm larva corpse")]
    public class RotwormLarva : BaseCreature
    {
        public override bool CanBeResurrectedThroughVeterinary { get { return false; } }

        [Constructable]
        public RotwormLarva(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a rotworm larva";
            Body = 732;
            Hue = 2501;

            BaseSoundID = 0xDB;

            SetStr(25);
            SetDex(25);
            SetInt(25);

            SetHits(200);

            SetDamage(5, 10);

            SetSkill(SkillName.Wrestling, 60);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 25);

            Fame = 300;
            Karma = -300;

            VirtualArmor = 25;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 115.1;
        }

        //Dynamic Stats and Skills (Scale Up With Creature XP)
        public override int TamedItemId { get { return 17053; } }
        public override int TamedItemHue { get { return 2501; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        //Static Stats and Skills (Do Not Scale Up With Creature XP)
        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 50; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 50; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 50; } }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override bool IsHighSeasBodyType { get { return true; } }

        public override void SetUniqueAI()
        {
            UniqueCreatureDifficultyScalar = 1.15;
        }

        public override void SetTamedAI()
        {
        }       

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            base.OnGaveMeleeAttack(defender);

            double effectChance = .10;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .02;
                    else
                        effectChance = .25;
                }
            }

            if (Utility.RandomDouble() <= effectChance)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*burrows into opponent*");

                defender.SendMessage("The creature burrows inside of you, causing you immense pain and discomfort!");

                double damage = DamageMax * 5;
                
                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 30, 0x4F1, true, "", "");

                int projectiles = 6;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = new Point3D(defender.X + Utility.RandomList( -4, -3, -2, -1, 1, 2, 3, 4), defender.Y + Utility.RandomList( -4, -3, -2, -1, 1, 2, 3, 4), defender.Z);
                    SpellHelper.AdjustField(ref newLocation, defender.Map, 12, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z + Utility.RandomMinMax(5, 10)), defender.Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + Utility.RandomMinMax(10, 20)), defender.Map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(4651, 4652, 4653, 4654), particleSpeed, 0, false, false, 0, 0);
                } 

                for (int a = 0; a < 4; a++)
                {
                    Point3D bloodLocation = new Point3D(X + Utility.RandomList(-1, 1), Y + Utility.RandomList(-1, 1), Z);
                    SpellHelper.AdjustField(ref bloodLocation, Map, 12, false);

                    new Blood().MoveToWorld(bloodLocation, Map);
                }

                int selfDamage = 10;

                AOS.Damage(this, selfDamage, 0, 100, 0, 0, 0);
            }
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

        public override int GetAngerSound() { return 0x581; }
        public override int GetIdleSound() { return 0x582; }
        public override int GetAttackSound() { return 0x580; }
        public override int GetHurtSound() { return 0x5DA; }
        public override int GetDeathSound() { return 0x57F; }

        public RotwormLarva(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}