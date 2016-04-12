using System;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("a fire beetle corpse")]
    public class FireBeetle : BaseCreature
    {
        [Constructable]
        public FireBeetle(): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "a fire beetle";
            Body = 791;
            Hue = 1359;

            SetStr(75);
            SetDex(50);
            SetInt(25);

            SetHits(500);

            SetDamage(15, 25);

            SetSkill(SkillName.Wrestling, 95);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 100);

            VirtualArmor = 150;

            Fame = 2000;
            Karma = -2000;

            Tameable = true;
            ControlSlots = 2;
            MinTameSkill = 110.1;
        }

        public override int TamedItemId { get { return 9743; } }
        public override int TamedItemHue { get { return 1359; } }
        public override int TamedItemXOffset { get { return -10; } }
        public override int TamedItemYOffset { get { return 5; } }

        public override int TamedBaseMaxHits { get { return 300; } }
        public override int TamedBaseMinDamage { get { return 14; } }
        public override int TamedBaseMaxDamage { get { return 16; } }
        public override double TamedBaseWrestling { get { return 95; } }
        public override double TamedBaseEvalInt { get { return 0; } }

        public override int TamedBaseStr { get { return 5; } }
        public override int TamedBaseDex { get { return 25; } }
        public override int TamedBaseInt { get { return 5; } }
        public override int TamedBaseMaxMana { get { return 0; } }
        public override double TamedBaseMagicResist { get { return 75; } }
        public override double TamedBaseMagery { get { return 0; } }
        public override double TamedBasePoisoning { get { return 0; } }
        public override double TamedBaseTactics { get { return 100; } }
        public override double TamedBaseMeditation { get { return 0; } }
        public override int TamedBaseVirtualArmor { get { return 150; } }

        public override void SetUniqueAI()
        {
        }

        public override void SetTamedAI()
        {
        }

        public override SpeedGroupType BaseSpeedGroup { get { return SpeedGroupType.Slow; } }
        public override AIGroupType AIBaseGroup { get { return AIGroupType.EvilMonster; } }
        public override AISubGroupType AIBaseSubGroup { get { return AISubGroupType.Melee; } }
        public override double BaseUniqueDifficultyScalar { get { return 1.0; } }

        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(Utility.RandomMinMax(50, 70));       
        
        public override bool RevealImmune { get { return !Controlled; } }       

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);
           
            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (defender is PlayerMobile)
                        effectChance = .05;
                    else
                        effectChance = .50;
                }
            }

            if (Utility.RandomDouble() <= effectChance)
                FlameAttack(defender);            
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);
            
            double effectChance = .25;

            if (Controlled && ControlMaster != null)
            {
                if (ControlMaster is PlayerMobile)
                {
                    if (attacker is PlayerMobile)
                        effectChance = .10;

                    else
                        effectChance = .50;
                }
            }

            if (Utility.RandomDouble() <= effectChance)
                FlameAttack(attacker);            
        }

        public void FlameAttack(Mobile target)
        {
            if (target == null) return;
            if (!target.Alive || target.Deleted)  return;            

            target.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
            target.PlaySound(Utility.RandomList(0x5CF));

            Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, TimeSpan.FromSeconds(0.5)), 0x3996, 50, 20, 5029);

            double damage = DamageMax;

            if (target is BaseCreature)
                damage *= 1.5;

            int finalAdjustedDamage = AOS.Damage(target, (int)damage, 0, 100, 0, 0, 0);

            DisplayFollowerDamage(target, finalAdjustedDamage);
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Controlled && ControlMaster is PlayerMobile)
                return;

            double hitsPercent = (double)Hits / (double)HitsMax;

            if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed && hitsPercent < .50)
            {
                if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                {
                    Point3D originalLocation = Location;

                    if (SpecialAbilities.VanishAbility(this, 5.0, false, 0x21D, 5, 10, true, null))
                    {
                        for (int a = 0; a < 3; a++)
                        {
                            if (Utility.RandomDouble() <= .5)
                            {
                                TimedStatic rocks = new TimedStatic(Utility.RandomList(4967, 4970, 4973), 5);
                                rocks.Name = "rocks";

                                Point3D rockLocation = SpecialAbilities.GetRandomAdjustedLocation(originalLocation, Map, true, 1, true);

                                rocks.MoveToWorld(rockLocation, Map);
                            }

                            else
                            {
                                TimedStatic dirt = new TimedStatic(Utility.RandomList(7681, 7682), 5);
                                dirt.Name = "dirt";

                                Point3D dirtLocation = SpecialAbilities.GetRandomAdjustedLocation(originalLocation, Map, true, 1, true);

                                dirt.MoveToWorld(dirtLocation, Map);
                            }
                        }

                        PublicOverheadMessage(MessageType.Regular, 0, false, "*burrows*");

                        Combatant = null;

                        Effects.PlaySound(Location, Map, GetIdleSound());
                    }

                    m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                }
            }
        }
        
        public override void OnDeath(Container c)
        {
            base.OnDeath(c);
        }

        public override int GetAngerSound() { return 0x4F3; }
        public override int GetIdleSound() { return 0x4F2; }
        public override int GetAttackSound() { return 0x607; }
        public override int GetHurtSound() { return 0x608; }
        public override int GetDeathSound() { return 0x4F0; }

        public FireBeetle(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}