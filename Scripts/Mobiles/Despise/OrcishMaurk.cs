using Server.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("an orcish ma'urk corpse")]
    public class OrcishMaurk : BaseOrc
    {
        public DateTime m_NextVanishAllowed;
        public TimeSpan NextVanishDelay = TimeSpan.FromSeconds(15); 

        [Constructable]
        public OrcishMaurk(): base()
        {
            Name = "an orcish ma'urk";

            SetStr(75);
            SetDex(75);
            SetInt(25);

            SetHits(500);

            SetDamage(12, 18);

            SetSkill(SkillName.Archery, 105);
            SetSkill(SkillName.Tactics, 100);

            SetSkill(SkillName.MagicResist, 50);

            SetSkill(SkillName.Hiding, 95);
            SetSkill(SkillName.Stealth, 95);

            VirtualArmor = 25;

            Fame = 1500;
            Karma = -1500;

            AddItem(new OrcHelm() { Movable = false, Hue = 1175 });
            AddItem(new StuddedGorget() { Movable = false, Hue = 2051 });
            AddItem(new StuddedChest() { Movable = false, Hue = 0 });
            AddItem(new BodySash() { Movable = false, Hue = 2051 });
            AddItem(new PlateArms() { Movable = false, Hue = 2051 });
            AddItem(new StuddedGloves() { Movable = false, Hue = 2051 });
            AddItem(new StuddedLegs() { Movable = false, Hue = 2051 });
            AddItem(new HalfApron() { Movable = false, Hue = 2051 });
            AddItem(new Boots() { Movable = false, Hue = 1175 });            

            AddItem(new Bow());
            PackItem(new Arrow(20));
        }

        public override int OceanDoubloonValue { get { return 10; } }

        public override void SetUniqueAI()
        {
            if (Global_AllowAbilities)
                UniqueCreatureDifficultyScalar = 1.25;

            DictCombatAction[CombatAction.CombatSpecialAction] = 1;
            DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;
        }
        
        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            defender.PlaySound(0x234);

            SpecialAbilities.BleedSpecialAbility(.25, this, defender, DamageMax, 8.0, -1, true, "", "Their precise shot causes you to bleed!");
        }

        public override void OnThink()
        {
            base.OnThink();

            if (Global_AllowAbilities)
            {
                if (Utility.RandomDouble() < 0.05 && DateTime.UtcNow > m_NextVanishAllowed && BoatOccupied == null)
                {
                    if (Combatant != null && !Hidden && !Paralyzed && !BardProvoked && !BardPacified)
                    {
                        if (SpecialAbilities.VanishAbility(this, 1.0, true, -1, 5, 10, true, null))
                        {
                            PublicOverheadMessage(MessageType.Regular, 0, false, "*vanishes*");

                            switch (Utility.RandomMinMax(1, 4))
                            {
                                case 1:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 2:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 3:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 0;
                                    break;

                                case 4:
                                    DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 0;
                                    DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 10;
                                    break;
                            }
                        }

                        m_NextVanishAllowed = DateTime.UtcNow + NextVanishDelay;
                    }
                }
            }
        }

        public override int GetAttackSound() { return 0x238; }

        public OrcishMaurk(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version 
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
