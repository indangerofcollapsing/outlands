using System;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a rotworm larva corpse")]
    public class RotwormLarva : BaseCreature
    {
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

            Tameable = true;
            ControlSlots = 1;
            MinTameSkill = 115.1;
        }

        public override int TamedItemId { get { return 17053; } }
        public override int TamedItemHue { get { return 2501; } }
        public override int TamedItemXOffset { get { return 0; } }
        public override int TamedItemYOffset { get { return 0; } }

        public override int TamedBaseMaxHits { get { return 225; } }
        public override int TamedBaseMinDamage { get { return 8; } }
        public override int TamedBaseMaxDamage { get { return 10; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 0; } }

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

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SlayerGroupType SlayerGroup { get { return SlayerGroupType.Monstrous; } }
        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.VerySlow; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public override int PoisonResistance { get { return 4; } }

        public override bool IsHighSeasBodyType { get { return true; } }

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

                SpecialAbilities.BleedSpecialAbility(1.0, this, defender, damage, 30, 0x4F1, true, "", "", "-1");

                int projectiles = 6;
                int particleSpeed = 4;

                for (int a = 0; a < projectiles; a++)
                {
                    Point3D newLocation = SpecialAbilities.GetRandomAdjustedLocation(defender.Location, defender.Map, true, 4, false);

                    IEntity effectStartLocation = new Entity(Serial.Zero, new Point3D(defender.X, defender.Y, defender.Z + Utility.RandomMinMax(5, 10)), defender.Map);
                    IEntity effectEndLocation = new Entity(Serial.Zero, new Point3D(newLocation.X, newLocation.Y, newLocation.Z + Utility.RandomMinMax(10, 20)), defender.Map);

                    Effects.SendMovingEffect(effectStartLocation, effectEndLocation, Utility.RandomList(4651, 4652, 4653, 4654), particleSpeed, 0, false, false, 0, 0);
                }

                for (int a = 0; a < 4; a++)
                {
                    Point3D bloodLocation = SpecialAbilities.GetRandomAdjustedLocation(Location, Map, true, 1, false);
                    new Blood().MoveToWorld(bloodLocation, Map);
                }

                int selfDamage = 10;

                AOS.Damage(this, selfDamage, 0, 100, 0, 0, 0);
            }
        }

        public override void OnThink()
        {
            base.OnThink();
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }        

        protected override bool OnMove(Direction d)
        {
            if (Utility.RandomDouble() <= .25)
            {
                TimedStatic slime = new TimedStatic(Utility.RandomList(4651, 4652, 4653, 4654), 5);
                slime.Hue = 2623;
                slime.Name = "slime";

                slime.MoveToWorld(Location, Map);

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