using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Factions;

namespace Server
{
    public class AIDefinitions
    {
        public AIDefinitions()
        {
        }

        public void UpdateAI(BaseCreature target)
        {
            //BaseAI AIGroup
            switch (target.Group)
            {
                case AIGroup.None:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;
                    target.DictCombatRange[CombatRange.SpellRange] = 0;
                    target.DictCombatRange[CombatRange.Withdraw] = 0;

                    target.DictCombatAction[CombatAction.None] = 1;
                    target.DictCombatAction[CombatAction.AttackOnly] = 10;
                    target.DictCombatAction[CombatAction.CombatSpell] = 0;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 0;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 0;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 0;
                    target.DictCombatAction[CombatAction.CombatEpicAction] = 0;
                break;                

                case AIGroup.EvilMonster:
                    target.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.NeutralMonster:

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.GoodMonster:
                    target.DictCombatTargeting[CombatTargeting.Evil] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.Undead:
                    target.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;

                    target.DictCombatFlee[CombatFlee.Flee50] = 0;
                    target.DictCombatFlee[CombatFlee.Flee25] = 0;
                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;
                    break;

                case AIGroup.EvilHuman:
                    target.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.NeutralHuman:
                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.GoodHuman:
                    target.DictCombatTargeting[CombatTargeting.Evil] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.EvilAnimal:
                    target.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.NeutralAnimal:
                    target.DictCombatFlee[CombatFlee.Flee10] = 2;
                    target.DictCombatFlee[CombatFlee.Flee5] = 3;
                    break;

                case AIGroup.GoodAnimal:
                    target.DictCombatTargeting[CombatTargeting.Evil] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AIGroup.FactionMonster:
                    break;

                case AIGroup.FactionHuman:
                    target.DictCombatFlee[CombatFlee.Flee50] = 0;
                    target.DictCombatFlee[CombatFlee.Flee25] = 0;
                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;
                    break;

                case AIGroup.FactionAnimal:
                    break;

                case AIGroup.Summoned:
                    break;

                case AIGroup.Boss:
                    target.DictCombatTargeting[CombatTargeting.PlayerAny] = 1;
                    break;
            }

            //BaseAI AISubgroup
            switch (target.Subgroup)
            {
                case AISubgroup.None:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;
                    target.DictCombatRange[CombatRange.SpellRange] = 0;
                    target.DictCombatRange[CombatRange.Withdraw] = 0;

                    target.DictCombatAction[CombatAction.None] = 1;
                    target.DictCombatAction[CombatAction.AttackOnly] = 10;
                    target.DictCombatAction[CombatAction.CombatSpell] = 0;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 0;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 0;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 0;
                    target.DictCombatAction[CombatAction.CombatEpicAction] = 0;
                break;

                case AISubgroup.MeleeMage1:
                    target.SpellDelayMin = 7;
                    target.SpellDelayMax = 8;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 0;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.MeleeMage2:
                    target.SpellDelayMin = 5;
                    target.SpellDelayMax = 7;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 2;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.MeleeMage3:
                    target.SpellDelayMin = 4;
                    target.SpellDelayMax = 5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 2;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 3;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.MeleeMage4:
                    target.SpellDelayMin = 3;
                    target.SpellDelayMax = 4;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 4;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 2;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.MeleeMage5:
                    target.SpellDelayMin = 1;
                    target.SpellDelayMax = 2;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 5;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 3;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.MeleeMage6:
                    target.SpellDelayMin = .5;
                    target.SpellDelayMax = 1;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 5;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 6;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 4;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.Mage1:
                    target.SpellDelayMin = 4.5;
                    target.SpellDelayMax = 5.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.Mage2:
                    target.SpellDelayMin = 3.5;
                    target.SpellDelayMax = 4.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 2;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.Mage3:
                    target.SpellDelayMin = 2.5;
                    target.SpellDelayMax = 3.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 2;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 3;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.Mage4:
                    target.SpellDelayMin = 1.5;
                    target.SpellDelayMax = 2.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 4;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 2;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.Mage5:
                    target.SpellDelayMin = 0.5;
                    target.SpellDelayMax = 1.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 5;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 3;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.Mage6:
                    target.SpellDelayMin = 0;
                    target.SpellDelayMax = 1;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 15;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 5;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 6;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 4;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.GroupHealerMeleeMage1:
                    target.SpellDelayMin = 7;
                    target.SpellDelayMax = 8;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 2;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    break;

                case AISubgroup.GroupHealerMeleeMage2:
                    target.SpellDelayMin = 5;
                    target.SpellDelayMax = 7;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 2;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 2;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 0;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 2;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMeleeMage3:
                    target.SpellDelayMin = 4;
                    target.SpellDelayMax = 5;

                    target.DictCombatAction[CombatAction.AttackOnly] = 2;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 3;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 1;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 2;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 3;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMeleeMage4:
                    target.SpellDelayMin = 2;
                    target.SpellDelayMax = 4;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 2;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 4;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 2;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 3;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 6;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 6;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMeleeMage5:
                    target.SpellDelayMin = 1;
                    target.SpellDelayMax = 2;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 2;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 5;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 3;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 4;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 9;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 4;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 9;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMeleeMage6:
                    target.SpellDelayMin = .5;
                    target.SpellDelayMax = 1;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 2;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 6;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 4;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 5;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 12;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 5;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 12;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMage1:
                    target.SpellDelayMin = 4.5;
                    target.SpellDelayMax = 5.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 0;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 0;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMage2:
                    target.SpellDelayMin = 3.5;
                    target.SpellDelayMax = 4.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 2;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 0;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 0;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 2;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 0;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMage3:
                    target.SpellDelayMin = 2.5;
                    target.SpellDelayMax = 3.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 3;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 1;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 2;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 3;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMage4:
                    target.SpellDelayMin = 1.5;
                    target.SpellDelayMax = 2.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 4;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 2;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 3;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 6;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 6;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMage5:
                    target.SpellDelayMin = 0.5;
                    target.SpellDelayMax = 1.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 5;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 3;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 4;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 9;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 4;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 9;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMage6:
                    target.SpellDelayMin = 0;
                    target.SpellDelayMax = 1;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 0;
                    target.DictCombatAction[CombatAction.CombatSpell] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 4;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 8;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 6;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 6;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 4;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 5;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 12;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 5;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 12;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupHealerMelee:
                    target.SpellDelayMin = 5;
                    target.SpellDelayMax = 7;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 3;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 4;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;

                    target.DictCombatHealOther[CombatHealOther.SpellHealOther100] = 1;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther75] = 2;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther50] = 3;
                    target.DictCombatHealOther[CombatHealOther.SpellHealOther25] = 4;
                    target.DictCombatHealOther[CombatHealOther.SpellCureOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf75] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf50] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf25] = 4;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 5;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;
                    target.DictWanderAction[WanderAction.SpellHealOther100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureOther] = 1;
                    break;

                case AISubgroup.GroupMedicMelee:
                    target.SpellDelayMin = 2.5;
                    target.SpellDelayMax = 3.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;
                    target.DictCombatRange[CombatRange.SpellRange] = 0;
                    target.DictCombatRange[CombatRange.Withdraw] = 0;

                    target.DictCombatAction[CombatAction.AttackOnly] = 3;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 4;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;

                    target.DictCombatHealOther[CombatHealOther.BandageHealOther100] = 1;
                    target.DictCombatHealOther[CombatHealOther.BandageHealOther75] = 2;
                    target.DictCombatHealOther[CombatHealOther.BandageHealOther50] = 3;
                    target.DictCombatHealOther[CombatHealOther.BandageHealOther25] = 4;
                    target.DictCombatHealOther[CombatHealOther.BandageCureOther] = 5;

                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] = 4;
                    target.DictCombatHealSelf[CombatHealSelf.BandageCureSelf] = 5;

                    target.DictWanderAction[WanderAction.BandageHealOther100] = 1;
                    target.DictWanderAction[WanderAction.BandageHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.BandageCureOther] = 1;
                    target.DictWanderAction[WanderAction.BandageCureSelf] = 1;
                    break;

                case AISubgroup.GroupMedicRanged:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 4;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;

                    target.DictCombatHealOther[CombatHealOther.BandageHealOther100] = 1;
                    target.DictCombatHealOther[CombatHealOther.BandageHealOther75] = 2;
                    target.DictCombatHealOther[CombatHealOther.BandageHealOther50] = 3;
                    target.DictCombatHealOther[CombatHealOther.BandageHealOther25] = 4;
                    target.DictCombatHealOther[CombatHealOther.BandageCureOther] = 5;

                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf50] = 3;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf25] = 4;
                    target.DictCombatHealSelf[CombatHealSelf.BandageCureSelf] = 5;

                    target.DictWanderAction[WanderAction.BandageHealOther100] = 1;
                    target.DictWanderAction[WanderAction.BandageHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.BandageCureOther] = 1;
                    target.DictWanderAction[WanderAction.BandageCureSelf] = 1;
                    break;

                case AISubgroup.WanderingHealer:
                    target.SpellDelayMin = 1.5;
                    target.SpellDelayMax = 2.5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 1;
                    target.DictCombatAction[CombatAction.CombatSpell] = 10;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 5;

                    target.DictCombatSpell[CombatSpell.SpellDamage1] = 1;
                    target.DictCombatSpell[CombatSpell.SpellDamage2] = 2;
                    target.DictCombatSpell[CombatSpell.SpellDamage3] = 3;
                    target.DictCombatSpell[CombatSpell.SpellDamage4] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamage5] = 5;
                    target.DictCombatSpell[CombatSpell.SpellDamage6] = 6;
                    target.DictCombatSpell[CombatSpell.SpellDamage7] = 4;
                    target.DictCombatSpell[CombatSpell.SpellDamageAOE7] = 2;
                    target.DictCombatSpell[CombatSpell.SpellPoison] = 4;
                    target.DictCombatSpell[CombatSpell.SpellNegative1to3] = 1;
                    target.DictCombatSpell[CombatSpell.SpellNegative4to7] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon5] = 0;
                    target.DictCombatSpell[CombatSpell.SpellSummon8] = 0;
                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 0;
                    target.DictCombatSpell[CombatSpell.SpellHarmfulField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellNegativeField] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial1to2] = 0;
                    target.DictCombatSpell[CombatSpell.SpellBeneficial3to5] = 2;

                    target.DictCombatHealSelf[CombatHealSelf.SpellHealSelf100] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.SpellCureSelf] = 2;
                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf100] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.SpellHealSelf100] = 1;
                    target.DictWanderAction[WanderAction.SpellCureSelf] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;
                    break;

                case AISubgroup.Hunter:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatTargeting[CombatTargeting.SuperPredator] = 3;
                    target.DictCombatTargeting[CombatTargeting.Predator] = 2;
                    target.DictCombatTargeting[CombatTargeting.Prey] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.Stealth] = 1;

                    break;

                case AISubgroup.SuperPredator:
                    target.SuperPredator = true;

                    target.DictCombatTargeting[CombatTargeting.Aggressor] = 3;
                    target.DictCombatTargeting[CombatTargeting.Predator] = 1;
                    target.DictCombatTargeting[CombatTargeting.Prey] = 2;

                    target.DictCombatFlee[CombatFlee.Flee5] = 1;
                    break;

                case AISubgroup.Predator:
                    target.Predator = true;

                    target.DictCombatTargeting[CombatTargeting.Aggressor] = 2;
                    target.DictCombatTargeting[CombatTargeting.Prey] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 1;
                    target.DictCombatFlee[CombatFlee.Flee5] = 2;
                    break;

                case AISubgroup.Prey:
                    target.Prey = true;

                    target.DictCombatFlee[CombatFlee.Flee25] = 1;
                    target.DictCombatFlee[CombatFlee.Flee10] = 3;
                    target.DictCombatFlee[CombatFlee.Flee5] = 5;
                    break;

                case AISubgroup.Berserk:
                    target.DictCombatTargeting[CombatTargeting.Any] = 1;

                    target.DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant] = 10;
                break;

                case AISubgroup.MeleePotion:
                    target.DictCombatAction[CombatAction.AttackOnly] = 8;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.PotionHealSelf25] = 5;
                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;

                    target.CombatHealActionMinDelay = 15;
                    target.CombatHealActionMaxDelay = 30;
                    break;

                case AISubgroup.Swarm:
                    target.DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] = 10;
                    break;

                case AISubgroup.Duelist:
                    target.DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] = 10;
                    break;

                case AISubgroup.AntiArmor:
                    target.DictCombatTargetingWeight[CombatTargetingWeight.HighestArmor] = 10;
                    break;

                case AISubgroup.MageKiller:
                    target.DictCombatTargetingWeight[CombatTargetingWeight.Spellcaster] = 10;
                    break;

                case AISubgroup.Ranged:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;
                    break;

                case AISubgroup.Scout:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictGuardAction[GuardAction.None] = 1;
                    target.DictGuardAction[GuardAction.DetectHidden] = 3;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.DetectHidden] = 1;

                    target.DictWaypointAction[WaypointAction.None] = 0;
                    target.DictWaypointAction[WaypointAction.DetectHidden] = 1;

                    target.ResolveAcquireTargetDelay = 1;
                    break;

                case AISubgroup.Thief:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 5;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.Stealth] = 3;
                    break;

                case AISubgroup.Assassin:
                    target.DictCombatAction[CombatAction.AttackOnly] = 3;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 3;

                    target.DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 1;

                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.Stealth] = 1;
                    break;

                case AISubgroup.Poisoner:
                    target.DictCombatAction[CombatAction.AttackOnly] = 3;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 3;

                    target.DictCombatSpecialAction[CombatSpecialAction.ApplyWeaponPoison] = 1;
                    break;

                case AISubgroup.Stealther:
                    target.DictWanderAction[WanderAction.None] = 1;
                    target.DictWanderAction[WanderAction.Stealth] = 3;
                    break;

                case AISubgroup.Alchemist:
                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 0;
                    target.DictCombatRange[CombatRange.SpellRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 10;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 3;
                    break;

                case AISubgroup.Bomber:
                    target.DictCombatAction[CombatAction.AttackOnly] = 5;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 1;
                    break;

                case AISubgroup.GuardMelee:
                    target.DictCombatTargeting[CombatTargeting.PlayerCriminal] = 10;
                    target.DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 5;
                    target.DictCombatTargetingWeight[CombatTargetingWeight.HighestArmor] = 5;
                    target.DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 6;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 2;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 10;

                    target.DictCombatHealOther[CombatHealOther.BandageHealOther75] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;
                    break;

                case AISubgroup.GuardRanged:
                    target.DictCombatTargeting[CombatTargeting.PlayerCriminal] = 10;
                    target.DictCombatTargetingWeight[CombatTargetingWeight.Closest] = 7;
                    target.DictCombatTargetingWeight[CombatTargetingWeight.LowestArmor] = 5;
                    target.DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints] = 10;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 20;
                    target.DictCombatRange[CombatRange.Withdraw] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 10;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 3;
                    target.DictCombatAction[CombatAction.CombatHealOther] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.BandageHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 10;

                    target.DictCombatHealOther[CombatHealOther.BandageHealOther75] = 1;

                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;
                    break;

                case AISubgroup.Dispeller:
                    target.SpellDelayMin = 2.5;
                    target.SpellDelayMax = 3.5;

                    target.DictCombatTargetingWeight[CombatTargetingWeight.Summoned] = 5;

                    target.DictCombatRange[CombatRange.WeaponAttackRange] = 1;

                    target.DictCombatAction[CombatAction.AttackOnly] = 1;
                    target.DictCombatAction[CombatAction.CombatSpell] = 10;

                    target.DictCombatSpell[CombatSpell.SpellDispelSummon] = 10;
                    break;

                case AISubgroup.Sailor:
                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;

                    target.DictCombatAction[CombatAction.AttackOnly] = 20;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 3;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    target.DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 1;

                    target.DictCombatHealSelf[CombatHealSelf.PotionHealSelf50] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 5;
                break;

                case AISubgroup.ShipCaptain:
                    target.DictCombatFlee[CombatFlee.Flee10] = 0;
                    target.DictCombatFlee[CombatFlee.Flee5] = 0;

                    target.DictCombatAction[CombatAction.AttackOnly] = 15;
                    target.DictCombatAction[CombatAction.CombatSpecialAction] = 3;
                    target.DictCombatAction[CombatAction.CombatHealSelf] = 1;

                    target.DictCombatSpecialAction[CombatSpecialAction.ThrowShipBomb] = 5;

                    target.DictCombatHealSelf[CombatHealSelf.PotionHealSelf75] = 1;
                    target.DictCombatHealSelf[CombatHealSelf.PotionCureSelf] = 5;
                break;
            }
        }
    }
}