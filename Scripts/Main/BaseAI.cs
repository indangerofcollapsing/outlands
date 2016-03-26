using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if Framework_4_0
using System.Linq;
using System.Threading.Tasks;
#endif
using Server;
using Server.Items;
using Server.Targeting;
using Server.Targets;
using Server.Network;
using Server.Regions;
using Server.ContextMenus;
using MoveImpl = Server.Movement.MovementImpl;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Spells.Sixth;
using Server.Spells.Seventh;
using Server.Spells.Eighth;
using Server.Custom;
using Server.Multis;

namespace Server.Mobiles
{
    //No Longer Used
    public enum AIType
    {
        AI_Use_Default,
        AI_Melee,
        AI_Animal,
        AI_Archer,
        AI_Healer,
        AI_Vendor,
        AI_Mage,
        AI_Berserk,
        AI_Predator,
        AI_Thief,
        AI_Bot,
        AI_Script,
        AI_Generic
    }

    public enum AIGroupType
    {
        Unspecified,
        None,
        EvilMonster,
        NeutralMonster,
        GoodMonster,
        Undead,
        EvilHuman,
        NeutralHuman,
        GoodHuman,
        EvilAnimal,
        NeutralAnimal,
        GoodAnimal,
        FactionMonster,
        FactionHuman,
        FactionAnimal,
        Summoned,
        Boss
    }

    public enum AISubGroupType
    {
        Unspecified,
        None,
        MeleeMage1,
        MeleeMage2,
        MeleeMage3,
        MeleeMage4,
        MeleeMage5,
        MeleeMage6,
        Mage1,
        Mage2,
        Mage3,
        Mage4,
        Mage5,
        Mage6,
        GroupHealerMage1,
        GroupHealerMage2,
        GroupHealerMage3,
        GroupHealerMage4,
        GroupHealerMage5,
        GroupHealerMage6,
        GroupHealerMeleeMage1,
        GroupHealerMeleeMage2,
        GroupHealerMeleeMage3,
        GroupHealerMeleeMage4,
        GroupHealerMeleeMage5,
        GroupHealerMeleeMage6,
        GroupHealerMelee,
        GroupMedicMelee,
        GroupMedicRanged,
        SuperPredator,
        Predator,
        Prey,
        Hunter,
        Berserk,
        MeleePotion,
        Alchemist,
        AntiArmor,
        MageKiller,
        Ranged,
        Scout,
        Thief,
        Assassin,
        Bomber,
        WanderingHealer,
        GuardMelee,
        GuardRanged,
        Dispeller,
        BountyHunter,
        PureHealer,
        Sailor,
        ShipCaptain,
        Stealther,
        Swarm,
        Duelist,
        Poisoner
    }

    public enum ActionType
    {
        Wander,
        Combat,
        Guard,
        Flee,
        Backoff,
        Interact
    }

    public enum CombatTargeting
    {
        OpposingFaction,
        PlayerGood, //Includes ControlledMobiles
        PlayerCriminal, //Includes ControlledMobiles
        PlayerAny, //Includes ControlledMobiles
        SuperPredator,
        Predator,
        Prey,
        Good,
        Neutral,
        Evil,
        Aggressor,
        Any,
        None,
        UOACZHuman,
        UOACZIgnoreHumanSentry,
        UOACZHumanPlayer,
        UOACZEvilHumanPlayer,
        UOACZUndead,
        UOACZUndeadPlayer,
        UOACZWildlife,
        UOACZEvilWildlife,
    }

    public enum CombatTargetingWeight
    {
        CurrentCombatant,
        Closest,
        Player,
        Tamed,
        HighestHitPoints,
        LowestHitPoints,
        HighestArmor,
        LowestArmor,
        Ranged,
        Spellcaster,
        Summoned,
        Poisoner,
        WeakToPoison,
        MostCombatants,
        LeastCombatants,
        HardestToHit,
        EasiestToHit,
        HighestResist,
        LowestResist,
        AttackOrder,
        GuardOrder,
        PatrolOrder,
    }

    public enum CombatRange
    {
        WeaponAttackRange,
        SpellRange,
        Withdraw
    }

    public enum CombatFlee
    {
        None,
        Flee50,
        Flee25,
        Flee10,
        Flee5
    }

    public enum CombatAction
    {
        None,
        AttackOnly,
        CombatSpell,
        CombatHealSelf,
        CombatHealOther,
        CombatSpecialAction,
        CombatEpicAction
    }

    public enum CombatSpell
    {
        None,
        SpellDamage1,
        SpellDamage2,
        SpellDamage3,
        SpellDamage4,
        SpellDamage5,
        SpellDamage6,
        SpellDamage7,
        SpellDamageAOE7,
        SpellPoison,
        SpellNegative1to3,
        SpellNegative4to7,
        SpellSummon5,
        SpellSummon8,
        SpellDispelSummon,
        SpellHarmfulField,
        SpellNegativeField,
        SpellBeneficial1to2,
        SpellBeneficial3to5
    }

    public enum CombatHealSelf
    {
        None,
        SpellHealSelf100,
        SpellHealSelf75,
        SpellHealSelf50,
        SpellHealSelf25,
        SpellCureSelf,
        PotionHealSelf100,
        PotionHealSelf75,
        PotionHealSelf50,
        PotionHealSelf25,
        PotionCureSelf,
        BandageHealSelf100,
        BandageHealSelf75,
        BandageHealSelf50,
        BandageHealSelf25,
        BandageCureSelf
    }

    public enum CombatHealOther
    {
        None,
        SpellHealOther100,
        SpellHealOther75,
        SpellHealOther50,
        SpellHealOther25,
        SpellCureOther,
        BandageHealOther100,
        BandageHealOther75,
        BandageHealOther50,
        BandageHealOther25,
        BandageCureOther
    }

    public enum CombatSpecialAction
    {
        None,
        ApplyWeaponPoison,
        ThrowShipBomb,
        CauseWounds,
        FireBreathAttack,
        IceBreathAttack,
        PoisonBreathAttack
    }

    public enum CombatEpicAction
    {
        None,
        MeleeBleedAoE,
        MassiveFireBreathAttack,
        MassiveIceBreathAttack,
        MassivePoisonBreathAttack,
        MassiveBoneBreathAttack,
        MassivePlantBreathAttack
    }

    public enum GuardAction
    {
        None,
        DetectHidden,
        SpellDispelSummon,
        SpellReveal
    }

    public enum WanderAction
    {
        None,
        DetectHidden,
        SpellReveal,
        Tracking,
        Stealth,
        Stealing,
        SpellHealSelf100,
        BandageHealSelf100,
        PotionHealSelf100,
        SpellHealOther100,
        BandageHealOther100,
        SpellHealSelf50,
        BandageHealSelf50,
        PotionHealSelf50,
        SpellHealOther50,
        BandageHealOther50,
        SpellCureSelf,
        BandageCureSelf,
        PotionCureSelf,
        SpellCureOther,
        BandageCureOther
    }

    public enum WaypointAction
    {
        None,
        DetectHidden,
        Stealth
    }

    public enum InteractAction
    {
        None,
        Greeting
    }


    public enum CreatureSpecialAttackType
    {
        Paralyze,
        Slow,
        Pierce,
        Bleed,
        Stun
    }

    public abstract class BaseAI
    {
        public Timer m_Timer;
        protected ActionType m_Action;
        private DateTime m_NextStopGuard;
        private DateTime m_NextStopWithdraw;
        private double maxWithdrawDuration = 1.5; //Maximum time in seconds to do withdraw movement

        private DateTime m_WalkRandomOutsideHome = DateTime.UtcNow;
        private DateTime m_WalkTowardsHome = DateTime.UtcNow;
        private bool m_GuardModeTargetReady = false; //Whether Target was found in GuardMode (delay then fired off)
        private bool m_WanderModeTargetReady = false; //Whether Target was found in wanderMode (delay then fired off)
        private DateTime m_GuardModeTargetDelay; //Delay before acting upon discovering target while in GuardMode
        private DateTime m_WanderModeTargetDelay; //Delay before acting upon discovering target while in WanderMode

        private bool m_LowMana;

        public CombatAction currentCombatAction = CombatAction.AttackOnly;
        public CombatRange currentCombatRange = CombatRange.WeaponAttackRange;

        public double WanderModeAcquireTargetDelayAmount = 1.5;

        private double HealthFledAt = 0; //Will be set when a Mobile flees (used to determine when gained enough health to rally)

        public BaseCreature bc_Creature;

        public BaseAI(BaseCreature m)
        {
            bc_Creature = m;

            m_Timer = new AITimer(this);

            bool activate;

            if (!m.PlayerRangeSensitive)
                activate = true;
            else if (World.Loading)
                activate = false;
            else if (m.Map == null || m.Map == Map.Internal || !m.Map.GetSector(m).Active)
                activate = false;
            else
                activate = true;

            if (activate)
                m_Timer.Start();

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            Action = ActionType.Wander;
        }

        public TimeSpan DeactivateDelay = TimeSpan.FromMinutes(1);

        private class AITimer : Timer
        {
            private BaseAI m_Owner;

            public AITimer(BaseAI owner)
                : base(TimeSpan.FromSeconds(Utility.RandomDouble()), TimeSpan.FromSeconds(Math.Max(0.0, owner.bc_Creature.CurrentSpeed)))
            {
                m_Owner = owner;

                if (owner.bc_Creature != null)
                {
                    if (owner.bc_Creature.Controlled && owner.bc_Creature.ControlMaster is PlayerMobile)
                        Priority = TimerPriority.FiftyMS;
                }

                else if (owner.bc_Creature.ResolveAcquireTargetDelay != -1 || owner.bc_Creature.IsBoss() || owner.bc_Creature.IsMiniBoss() || owner.bc_Creature.IsLoHBoss() || owner.bc_Creature.IsEventBoss() || owner.bc_Creature.IsParagon || owner.bc_Creature is BladeSpirits || owner.bc_Creature is EnergyVortex)
                    Priority = TimerPriority.FiftyMS;

                else
                    Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                if (m_Owner.bc_Creature.Deleted)
                {
                    Stop();
                    return;
                }

                else if (m_Owner.bc_Creature.Map == null || m_Owner.bc_Creature.Map == Map.Internal)
                {
                    Stop();
                    return;
                }

                else if (m_Owner.bc_Creature.PlayerRangeSensitive)
                {
                    Sector sect = m_Owner.bc_Creature.Map.GetSector(m_Owner.bc_Creature);

                    if (!sect.Active)
                    {
                        if (m_Owner.bc_Creature.LastActivated + m_Owner.DeactivateDelay < DateTime.UtcNow)
                        {
                            m_Owner.Deactivate();

                            return;
                        }
                    }

                    else
                        m_Owner.bc_Creature.LastActivated = DateTime.UtcNow;
                }

                m_Owner.bc_Creature.OnThink();

                if (m_Owner.bc_Creature.Deleted)
                {
                    Stop();

                    return;
                }

                else if (m_Owner.bc_Creature.Map == null || m_Owner.bc_Creature.Map == Map.Internal)
                    return;

                else
                {
                    if (!m_Owner.Think())
                    {
                        Stop();

                        return;
                    }
                }
            }
        }

        public virtual void Activate()
        {
            if (!m_Timer.Running)
            {
                m_Timer.Delay = TimeSpan.Zero;
                m_Timer.Start();
            }
        }

        public virtual void Deactivate()
        {
            if (bc_Creature.PlayerRangeSensitive)
                m_Timer.Stop();
        }

        private void ReturnToHome()
        {
            SpawnEntry se = bc_Creature.Spawner as SpawnEntry;

            if (se != null)
            {
                Point3D loc = se.RandomSpawnLocation(16, !bc_Creature.CantWalk, bc_Creature.CanSwim);

                if (loc != Point3D.Zero)
                {
                    bc_Creature.MoveToWorld(loc, se.Region.Map);
                    return;
                }
            }
        }

        public ActionType Action
        {
            get { return m_Action; }
            set { m_Action = value; }
        }

        public virtual bool Think()
        {
            if (GetReactionToPlayerStatus())
                bc_Creature.OwnerAbandonTime = DateTime.UtcNow + bc_Creature.AbandonDelay;

            //Abandon Player Creature
            if (!bc_Creature.Summoned && bc_Creature.Controlled && bc_Creature.ControlMaster != null && bc_Creature.OwnerAbandonTime < DateTime.UtcNow)
            {
                if (bc_Creature.ControlMaster is PlayerMobile && bc_Creature.AIObject != null)
                    bc_Creature.AIObject.DoOrderRelease();
            }

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            CheckBardPacified();
            CheckBardProvoked();

            if (!bc_Creature.Controlled)
            {
                switch (Action)
                {
                    case ActionType.Combat: DoCombatAction(); break;
                    case ActionType.Guard: DoGuardAction(); break;
                    case ActionType.Wander: DoWanderAction(); break;
                    case ActionType.Flee: DoFleeAction(); break;
                    case ActionType.Interact: DoInteractAction(); break;
                }
            }

            else
            {
                switch (bc_Creature.ControlOrder)
                {
                    case OrderType.Attack: DoOrderAttack(); break;
                    case OrderType.Patrol: DoOrderPatrol(); break;
                    case OrderType.Guard: DoOrderGuard(); break;
                    case OrderType.Come: DoOrderCome(); break;
                    case OrderType.Follow: DoOrderFollow(); break;
                    case OrderType.Stay: DoOrderStay(); break;
                    case OrderType.Stop: DoOrderStop(); break;
                    case OrderType.None: DoOrderNone(); break;
                    case OrderType.Drop: DoOrderDrop(); break;
                    case OrderType.Friend: DoOrderFriend(); break;
                    case OrderType.Unfriend: DoOrderUnfriend(); break;
                    case OrderType.Release: DoOrderRelease(); break;
                    case OrderType.Transfer: DoOrderTransfer(); break;
                    case OrderType.Fetch: DoOrderFetch(); break;
                }
            }

            return true;
        }

        public void CombatMode()
        {
            Action = ActionType.Combat;

            bc_Creature.Warmode = true;
            bc_Creature.CurrentSpeed = bc_Creature.ActiveSpeed;

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            bc_Creature.NextDecisionTime = DateTime.UtcNow;
        }

        public void GuardMode()
        {
            Action = ActionType.Guard;

            bc_Creature.Warmode = false;
            bc_Creature.FocusMob = null;
            bc_Creature.Combatant = null;

            bc_Creature.CurrentSpeed = bc_Creature.ActiveSpeed;

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            m_NextStopGuard = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            bc_Creature.NextDecisionTime = DateTime.UtcNow;

            m_GuardModeTargetReady = false;
            m_GuardModeTargetDelay = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.GuardModeTargetDelay);
        }

        public void FleeMode()
        {
            Action = ActionType.Flee;

            bc_Creature.Warmode = true;
            bc_Creature.FocusMob = null;

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            if (bc_Creature.DoingBandage)
                AIHeal.BandageFail(bc_Creature);

            bc_Creature.CurrentSpeed = bc_Creature.PassiveSpeed * 1.5;

            if (bc_Creature.CurrentSpeed < 0.8)
                bc_Creature.CurrentSpeed = 0.8;

            m_NextStopWithdraw = DateTime.UtcNow;
            bc_Creature.NextDecisionTime = DateTime.UtcNow;
        }

        public void WanderMode()
        {
            Action = ActionType.Wander;

            bc_Creature.Warmode = false;
            bc_Creature.Combatant = null;
            bc_Creature.FocusMob = null;
            bc_Creature.CurrentSpeed = bc_Creature.PassiveSpeed;

            bc_Creature.DictAutoDispelInstances.Clear();
            bc_Creature.MagicDamageAbsorb = 0;

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            bc_Creature.NextDecisionTime = DateTime.UtcNow;

            m_WalkRandomOutsideHome = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.WalkRandomOutsideHomeLimit);
            m_WalkTowardsHome = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.WalkRandomOutsideHomeLimit + bc_Creature.WalkTowardsHomeLimit);

            bc_Creature.NextCombatHealActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(bc_Creature.CombatHealActionMinDelay, bc_Creature.CombatHealActionMaxDelay));
            bc_Creature.NextWanderActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(bc_Creature.WanderActionMinDelay, bc_Creature.WanderActionMaxDelay));

            m_WanderModeTargetReady = false;
            m_WanderModeTargetDelay = DateTime.UtcNow + TimeSpan.FromSeconds(WanderModeAcquireTargetDelayAmount);

            if (bc_Creature.WaypointOrder == WayPointOrder.IdleForward)
                bc_Creature.WaypointOrder = WayPointOrder.Forward;

            if (bc_Creature.WaypointOrder == WayPointOrder.IdleReverse)
                bc_Creature.WaypointOrder = WayPointOrder.Reverse;

            if (bc_Creature.CurrentWaypoint != null)
            {
                double distanceToWaypoint = bc_Creature.GetDistanceToSqrt(bc_Creature.CurrentWaypoint.Location);
                double estimatedTimeNeeded = distanceToWaypoint * bc_Creature.PassiveSpeed * 8;

                bc_Creature.NextWaypointAction = DateTime.UtcNow + TimeSpan.FromSeconds(estimatedTimeNeeded);
            }

            else
                bc_Creature.NextWaypointAction = DateTime.UtcNow;
        }

        public void InteractMode()
        {
            Action = ActionType.Interact;

            bc_Creature.Warmode = false;
            bc_Creature.CurrentSpeed = bc_Creature.PassiveSpeed;

            if (bc_Creature.ResolveAcquireTargetDelay != -1)
                WanderModeAcquireTargetDelayAmount = bc_Creature.ResolveAcquireTargetDelay;

            bc_Creature.NextDecisionTime = DateTime.UtcNow;
        }

        public virtual void OnCurrentOrderChanged()
        {
            if (bc_Creature is EnergyVortex || bc_Creature is BladeSpirits)
                return;

            switch (bc_Creature.ControlOrder)
            {
                case OrderType.Attack:
                    bc_Creature.CurrentSpeed = bc_Creature.ActiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = true;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;
                    break;

                case OrderType.Patrol:
                    bc_Creature.CurrentSpeed = bc_Creature.ActiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;
                    break;

                case OrderType.Guard:
                    if (bc_Creature.ControlMaster != null)
                    {
                        string petname = String.Format("{0}", bc_Creature.Name);
                        bc_Creature.ControlMaster.SendLocalizedMessage(1049671, petname);	//~1_PETNAME~ is now guarding you.
                    }

                    bc_Creature.CurrentSpeed = bc_Creature.ActiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = true;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;
                    break;

                case OrderType.Come:
                    bc_Creature.CurrentSpeed = bc_Creature.ActiveTamedFollowModeSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;

                    bc_Creature.ControlDest = bc_Creature.ControlMaster != null ? bc_Creature.ControlMaster.Location : bc_Creature.Location;
                    break;

                case OrderType.Follow:
                    bc_Creature.CurrentSpeed = bc_Creature.ActiveTamedFollowModeSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;
                    break;

                case OrderType.Stay:
                    bc_Creature.CurrentSpeed = bc_Creature.PassiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.FocusMob = null;

                    bc_Creature.Home = bc_Creature.Location;
                    bc_Creature.ControlDest = bc_Creature.ControlMaster != null ? bc_Creature.ControlMaster.Location : bc_Creature.Home;
                    break;

                case OrderType.Stop:
                    if (!bc_Creature.IsBarded())
                        bc_Creature.Aggressors.Clear();

                    bc_Creature.CurrentSpeed = bc_Creature.PassiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;

                    bc_Creature.Home = bc_Creature.Location;
                    bc_Creature.ControlDest = bc_Creature.ControlMaster != null ? bc_Creature.ControlMaster.Location : bc_Creature.Home;
                    break;

                case OrderType.None:
                    bc_Creature.CurrentSpeed = bc_Creature.PassiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.FocusMob = null;
                    bc_Creature.Combatant = null;

                    bc_Creature.Home = bc_Creature.Location;
                    bc_Creature.ControlDest = bc_Creature.Location;
                    break;

                case OrderType.Drop:
                    bc_Creature.CurrentSpeed = bc_Creature.PassiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());
                    bc_Creature.Warmode = true;
                    bc_Creature.Combatant = null;
                    break;

                case OrderType.Friend:
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    break;

                case OrderType.Unfriend:
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    break;

                case OrderType.Release:
                    bc_Creature.CurrentSpeed = bc_Creature.PassiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.Combatant = null;
                    break;

                case OrderType.Transfer:
                    bc_Creature.CurrentSpeed = bc_Creature.PassiveTamedSpeed;
                    bc_Creature.NextDecisionTime = DateTime.UtcNow;
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    bc_Creature.Warmode = false;
                    bc_Creature.Combatant = null;
                    break;
            }
        }

        public bool DoCombatAction()
        {
            if (bc_Creature == null)
                return false;

            if (!bc_Creature.Alive || bc_Creature.Deleted || bc_Creature.IsDeadBondedPet || bc_Creature.Blessed)
                return false;

            bool validCombatant = false;
            bool validControlMaster = false;
            bool MobileIsFollower = false;
            bool bandagingOther = false;
            bool Provoked = false;
            bool validWeapon = false;

            double manaAmount = bc_Creature.Mana;
            double manaPercent = (manaAmount / bc_Creature.ManaMax) * 100;

            if (bc_Creature.BardPacified)
            {
                if (CheckBardPacified())
                {
                    WanderMode();
                    return true;
                }
            }

            if (bc_Creature.BardProvoked)
            {
                if (CheckBardProvoked())
                    Provoked = true;
            }

            if (bc_Creature.ControlOrder == OrderType.Attack)
            {
                if (bc_Creature.Combatant != null)
                {
                    if (bc_Creature.Combatant.Hidden && bc_Creature.Combatant.Alive)
                        return false;

                    if (bc_Creature.Combatant.GetDistanceToSqrt(bc_Creature) >= ((double)bc_Creature.RangePerception * 4))
                        return false;
                }
            }

            if (manaAmount < 20 || (manaPercent <= bc_Creature.LowManaPercent))
            {
                if (AICombatSpell.CreatureHasCastingAI(bc_Creature))
                    m_LowMana = true;
            }

            else
                m_LowMana = false;

            BaseWeapon weapon = bc_Creature.Weapon as BaseWeapon;

            if (weapon != null)
                validWeapon = true;

            if (bc_Creature.DoingBandage && bc_Creature.HealTarget != null && bc_Creature.HealTarget != bc_Creature && (DateTime.UtcNow > bc_Creature.BandageTimeout))
                AIHeal.BandageFail(bc_Creature);

            else if (bc_Creature.HealTarget != bc_Creature && Provoked)
                AIHeal.BandageFail(bc_Creature);

            if (bc_Creature.DoingBandage)
            {
                if (bc_Creature.HealTarget != null && (bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget) <= (double)bc_Creature.RangePerception) && bc_Creature.HealTarget.Alive && !bc_Creature.HealTarget.Deleted && bc_Creature.Map == bc_Creature.HealTarget.Map && !bc_Creature.HealTarget.IsDeadBondedPet && bc_Creature.CanSee(bc_Creature.HealTarget))
                {
                    if (bc_Creature.HealTarget == bc_Creature)
                    {
                    }

                    else if (bc_Creature.BandageOtherReady && bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget) < 2)
                    {
                        AIHeal.StartBandageHeal(bc_Creature);
                        bandagingOther = true;
                    }

                    else
                        bandagingOther = true;
                }

                else
                    AIHeal.BandageFail(bc_Creature);
            }

            if (bc_Creature.Combatant != null && bc_Creature.Combatant.Alive && !bc_Creature.Combatant.Deleted && bc_Creature.Map == bc_Creature.Combatant.Map && !bc_Creature.Combatant.IsDeadBondedPet)
                validCombatant = true;

            if (bc_Creature.Controlled)
            {
                MobileIsFollower = true;

                if (bc_Creature.ControlMaster != null && !bc_Creature.ControlMaster.Deleted && bc_Creature.ControlMaster.Map == bc_Creature.Map && bc_Creature.ControlMaster.Alive)
                    validControlMaster = true;

                else
                {
                    if (bc_Creature.BardProvoked)
                        validControlMaster = true;
                }
            }

            if (!MobileIsFollower)
            {
                FleeCheck();

                if (Action == ActionType.Flee)
                    return false;
            }

            if (m_NextStopWithdraw <= DateTime.UtcNow && currentCombatRange == CombatRange.Withdraw)
                currentCombatRange = GetDefaultCombatRange();

            else if (m_NextStopWithdraw > DateTime.UtcNow && currentCombatRange == CombatRange.Withdraw)
            {
                if (!validCombatant)
                {
                    m_NextStopWithdraw = DateTime.UtcNow;
                    currentCombatRange = GetDefaultCombatRange();
                }

                else
                {
                    if (bc_Creature.GetDistanceToSqrt(bc_Creature.Combatant) >= GetCreatureWithdrawDistance(bc_Creature))
                    {
                        m_NextStopWithdraw = DateTime.UtcNow;
                        currentCombatRange = GetDefaultCombatRange();
                    }
                }
            }

            if (DateTime.UtcNow >= m_NextStopWithdraw && currentCombatRange != CombatRange.Withdraw)
            {
                if ((DateTime.UtcNow > bc_Creature.NextDecisionTime) && bc_Creature.Spell == null)
                {
                    if (!MobileIsFollower)
                    {
                        if (!Provoked)
                        {
                            if (DateTime.UtcNow >= bc_Creature.m_NextAcquireTargetAllowed)
                            {
                                bc_Creature.m_NextAcquireTargetAllowed = DateTime.UtcNow + bc_Creature.m_NextAcquireTargetDelay;

                                if (AcquireFocusMob(true))
                                {
                                    bc_Creature.Combatant = bc_Creature.FocusMob;

                                    if (bc_Creature.Combatant != null)
                                    {
                                        bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);
                                        bc_Creature.LastEnemyLocation = bc_Creature.Combatant.Location;

                                        validCombatant = true;
                                    }

                                    else
                                    {
                                        GuardMode();

                                        return true;
                                    }
                                }

                                else
                                {
                                    if (Action == ActionType.Flee)
                                    {
                                        FleeMode();
                                        return false;
                                    }

                                    validCombatant = false;
                                    GuardMode();

                                    return true;
                                }
                            }
                        }

                        else
                        {
                            if (validCombatant)
                                bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);
                        }
                    }

                    else
                    {
                        if (validCombatant)
                            bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);
                    }

                    currentCombatAction = GetCombatAction();
                    currentCombatRange = GetCombatRange();

                    bc_Creature.NextDecisionTime = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.DecisionTimeDelay);
                }
            }

            bool validMovement = true;

            if (!CheckMove() || bc_Creature.DisallowAllMoves)
                return false;

            if (validCombatant && bc_Creature.Spell == null && !bandagingOther)
            {
                if (MobileIsFollower)
                {
                    if (bc_Creature.ControlOrder == OrderType.Attack || bc_Creature.ControlOrder == OrderType.Guard)
                    {
                        if (validControlMaster)
                        {
                            if (bc_Creature.GetDistanceToSqrt(bc_Creature.ControlMaster) > bc_Creature.RangePerception * 2)
                                validMovement = false;
                        }

                        else
                        {
                            if (bc_Creature.GetDistanceToSqrt(bc_Creature.Home) > bc_Creature.RangePerception * 2)
                                validMovement = false;
                        }
                    }

                    else if (bc_Creature.ControlOrder == OrderType.Patrol)
                    {
                        if (bc_Creature.GetDistanceToSqrt(bc_Creature.ControlDest) > bc_Creature.RangePerception * 2)
                            validMovement = false;

                        else if (validControlMaster && bc_Creature.GetDistanceToSqrt(bc_Creature.ControlMaster) > bc_Creature.RangePerception * 2)
                            validMovement = false;
                    }

                    else if (bc_Creature.ControlOrder == OrderType.Stay)
                        validMovement = false;
                }

                if ((validMovement || bc_Creature.BardProvoked) && validCombatant)
                {
                    int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.Combatant);

                    bool bRun = (iCurrDist > 2);

                    switch (currentCombatRange)
                    {
                        case CombatRange.WeaponAttackRange:
                            if (validWeapon)
                            {
                                if (iCurrDist > weapon.MaxRange || !bc_Creature.InLOS(bc_Creature.Combatant))
                                {
                                    if (bc_Creature.BoatOccupied != null)
                                    {
                                        if (Utility.RandomDouble() < .33)
                                        {
                                            WalkToMobile(bc_Creature.Combatant, 1, bRun, 0, 1);

                                            Direction newDirection = bc_Creature.GetDirectionTo(bc_Creature.Combatant);

                                            if (bc_Creature.Direction != newDirection)
                                                bc_Creature.Direction = newDirection;
                                        }

                                        else
                                        {
                                            Direction newDirection = bc_Creature.GetDirectionTo(bc_Creature.Combatant);

                                            if (bc_Creature.Direction != newDirection)
                                                bc_Creature.Direction = newDirection;
                                        }
                                    }

                                    else
                                        WalkToMobile(bc_Creature.Combatant, 1, bRun, 0, 1);
                                }
                            }

                            else
                                WalkToMobile(bc_Creature.Combatant, 1, bRun, 0, 1);

                            break;

                        case CombatRange.SpellRange:

                            if ((iCurrDist > bc_Creature.CreatureSpellRange) || !bc_Creature.InLOS(bc_Creature.Combatant))
                            {
                                if (bc_Creature.BoatOccupied != null)
                                {
                                    if (Utility.RandomDouble() < .33)
                                    {
                                        WalkToMobile(bc_Creature.Combatant, 1, bRun, 0, 1);

                                        Direction newDirection = bc_Creature.GetDirectionTo(bc_Creature.Combatant);

                                        if (bc_Creature.Direction != newDirection)
                                            bc_Creature.Direction = newDirection;
                                    }

                                    else
                                    {
                                        Direction newDirection = bc_Creature.GetDirectionTo(bc_Creature.Combatant);

                                        if (bc_Creature.Direction != newDirection)
                                            bc_Creature.Direction = newDirection;
                                    }
                                }

                                else
                                    WalkToMobile(bc_Creature.Combatant, 1, bRun, 0, 1);
                            }
                            break;

                        case CombatRange.Withdraw:
                            if (!bc_Creature.InLOS(bc_Creature.Combatant) && bc_Creature.BoatOccupied == null)
                            {
                                WalkToMobile(bc_Creature.Combatant, 1, bRun, 0, 1);

                                m_NextStopWithdraw = DateTime.UtcNow;
                                currentCombatRange = GetDefaultCombatRange();

                                break;
                            }

                            if (iCurrDist < (GetCreatureWithdrawDistance(bc_Creature) - 1) && bc_Creature.BoatOccupied == null)
                            {
                                Point3D newPoint;
                                Dictionary<Direction, int> DictNewDirection = new Dictionary<Direction, int>();

                                for (int a = 0; a < 8; a++)
                                {
                                    Direction direction = (Direction)a;

                                    newPoint = bc_Creature.GetPointByDirection(bc_Creature.Location, direction);

                                    int changeCreatureX = Math.Abs(bc_Creature.Combatant.X - bc_Creature.X);
                                    int changeCreatureY = Math.Abs(bc_Creature.Combatant.Y - bc_Creature.Y);

                                    int distTargetToCreature = Math.Max(changeCreatureX, changeCreatureY);

                                    int changeLocationX = Math.Abs(newPoint.X - bc_Creature.Combatant.X);
                                    int changeLocationY = Math.Abs(newPoint.Y - bc_Creature.Combatant.Y);

                                    int distTargetToLocation = Math.Max(changeLocationX, changeLocationY);

                                    if (distTargetToLocation >= distTargetToCreature)
                                    {
                                        IPoint3D p = newPoint as IPoint3D;
                                        SpellHelper.GetSurfaceTop(ref p);
                                        Point3D pP = new Point3D(newPoint);

                                        if (!Region.Find(newPoint, bc_Creature.Map).AllowSpawn())
                                            continue;

                                        if (!DictNewDirection.ContainsKey(direction))
                                        {
                                            distTargetToLocation *= 5;

                                            int directionalWeight = distTargetToLocation * distTargetToLocation;

                                            if (direction == bc_Creature.Combatant.GetDirectionTo(bc_Creature.Location))
                                                directionalWeight *= distTargetToLocation;

                                            DictNewDirection.Add(direction, directionalWeight);
                                        }
                                    }
                                }

                                int TotalValues = 0;

                                foreach (KeyValuePair<Direction, int> pair in DictNewDirection)
                                {
                                    TotalValues += pair.Value;
                                }

                                double ActionCheck = Utility.RandomDouble();
                                double CumulativeAmount = 0.0;
                                double AdditionalAmount = 0.0;

                                bool foundDirection = true;

                                foreach (KeyValuePair<Direction, int> pair in DictNewDirection)
                                {
                                    AdditionalAmount = (double)pair.Value / (double)TotalValues;

                                    if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                                    {
                                        Point3D newLocation = bc_Creature.GetPointByDirection(bc_Creature.Location, (Direction)pair.Key);
                                        WalkToLocation(newLocation, 1, bRun, 0, 0);

                                        break;
                                    }

                                    CumulativeAmount += AdditionalAmount;
                                }
                            }

                            else
                            {
                                m_NextStopWithdraw = DateTime.UtcNow;
                                currentCombatRange = GetDefaultCombatRange();
                            }
                            break;
                    }
                }
            }

            else if (bandagingOther)
            {
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget);
                bool bRun = (iCurrDist > 2);

                WalkToMobile(bc_Creature.HealTarget, 1, bRun, 0, 1);
            }

            return true;
        }

        public bool DoGuardAction()
        {
            bool bandagingOther = false;
            bool pacified = CheckBardPacified();

            if (!pacified)
            {
                if (DateTime.UtcNow >= bc_Creature.m_NextAcquireTargetAllowed)
                {
                    bc_Creature.m_NextAcquireTargetAllowed = DateTime.UtcNow + bc_Creature.m_NextAcquireTargetDelay;

                    if (AcquireFocusMob(false))
                    {
                        bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.FocusMob);

                        if (!m_GuardModeTargetReady)
                        {
                            m_GuardModeTargetReady = true;
                            NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.DecisionTimeDelay);

                            m_GuardModeTargetDelay = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.GuardModeTargetDelay);
                        }

                        else if (m_GuardModeTargetReady && DateTime.UtcNow > m_GuardModeTargetDelay)
                        {
                            m_GuardModeTargetReady = false;
                            bc_Creature.Combatant = bc_Creature.FocusMob;

                            bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                            CombatMode();
                        }

                        return true;
                    }

                    else
                    {
                        if (Action == ActionType.Flee)
                        {
                            FleeMode();
                            return false;
                        }
                    }
                }
            }

            else
            {
                m_GuardModeTargetReady = false;
                m_GuardModeTargetDelay = DateTime.UtcNow;
            }

            if (DateTime.UtcNow > m_GuardModeTargetDelay + TimeSpan.FromSeconds(bc_Creature.GuardModeTargetDelay * 2))
            {
                m_GuardModeTargetReady = false;
                m_GuardModeTargetDelay = DateTime.UtcNow;
            }

            if (DateTime.UtcNow > m_NextStopGuard || pacified)
            {
                WanderMode();
                return true;
            }

            if (bc_Creature.DoingBandage && bc_Creature.HealTarget != bc_Creature && (DateTime.UtcNow > bc_Creature.BandageTimeout))
                AIHeal.BandageFail(bc_Creature);

            if (bc_Creature.DoingBandage)
            {
                if (bc_Creature.HealTarget != null && (bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget) <= (double)bc_Creature.RangePerception) && bc_Creature.HealTarget.Alive && !bc_Creature.HealTarget.Deleted && bc_Creature.Map == bc_Creature.HealTarget.Map && !bc_Creature.HealTarget.IsDeadBondedPet && bc_Creature.CanSee(bc_Creature.HealTarget))
                {
                    if (bc_Creature.HealTarget == bc_Creature)
                    {
                    }

                    else if (bc_Creature.BandageOtherReady && bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget) < 2)
                    {
                        AIHeal.StartBandageHeal(bc_Creature);
                        bandagingOther = true;
                    }

                    else
                        bandagingOther = true;
                }

                else
                    AIHeal.BandageFail(bc_Creature);
            }

            GetGuardAction();

            if (!CheckMove() || bc_Creature.DisallowAllMoves)
                return false;

            if (bandagingOther)
            {
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget);

                bool bRun = (iCurrDist > 2);

                WalkToMobile(bc_Creature.HealTarget, 1, bRun, 0, 1);

                return true;
            }

            else
            {
                double distanceToLastEnemyLocation = bc_Creature.GetDistanceToSqrt(bc_Creature.LastEnemyLocation);

                if (!pacified && bc_Creature.LastEnemyLocation != new Point3D(0, 0, 0) && distanceToLastEnemyLocation < 50)
                {
                    if (distanceToLastEnemyLocation <= ((double)bc_Creature.RangePerception / 2))
                        WalkRandomAtPoint(bc_Creature.LastEnemyLocation, 2, 2, 1);

                    else
                    {
                        bool bRun = (distanceToLastEnemyLocation > 2);

                        WalkToLocation(bc_Creature.LastEnemyLocation, 1, bRun, 0, 1);
                    }
                }

                else
                    WalkRandom(2, 2, 1);
            }

            return true;
        }

        public bool DoWanderAction()
        {
            bool bandagingOther = false;
            bool pacified = false;

            if (bc_Creature.BardPacified)
            {
                if (CheckBardPacified())
                    pacified = true;
            }

            if (!pacified)
            {
                if (bc_Creature.Combatant != null)
                {
                    if (bc_Creature.Combatant.Alive)
                    {
                        CombatMode();

                        return true;
                    }
                }

                if (DateTime.UtcNow >= bc_Creature.m_NextAcquireTargetAllowed)
                {
                    bc_Creature.m_NextAcquireTargetAllowed = DateTime.UtcNow + bc_Creature.m_NextAcquireTargetDelay;

                    if (AcquireFocusMob(false))
                    {
                        bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.FocusMob);

                        if (!m_WanderModeTargetReady)
                        {
                            m_WanderModeTargetReady = true;

                            NextMove = DateTime.UtcNow + TimeSpan.FromSeconds(WanderModeAcquireTargetDelayAmount);
                            m_WanderModeTargetDelay = DateTime.UtcNow + TimeSpan.FromSeconds(WanderModeAcquireTargetDelayAmount);
                        }

                        else if (m_WanderModeTargetReady && DateTime.UtcNow > m_WanderModeTargetDelay)
                        {
                            bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                            m_WanderModeTargetReady = false;
                            bc_Creature.Combatant = bc_Creature.FocusMob;

                            CombatMode();
                        }

                        return true;
                    }

                    else
                    {
                        if (Action == ActionType.Flee)
                        {
                            DoFleeAction();
                            return false;
                        }
                    }

                    if (DateTime.UtcNow > m_WanderModeTargetDelay + TimeSpan.FromSeconds(WanderModeAcquireTargetDelayAmount))
                    {
                        m_WanderModeTargetReady = false;
                        m_WanderModeTargetDelay = DateTime.UtcNow;
                    }
                }
            }

            else
            {
                m_WanderModeTargetReady = false;
                m_WanderModeTargetDelay = DateTime.UtcNow;

                //Pacified Creatures No Longer Move
                return false;
            }

            if (bc_Creature.DoingBandage && bc_Creature.HealTarget != bc_Creature && (DateTime.UtcNow > bc_Creature.BandageTimeout))
                AIHeal.BandageFail(bc_Creature);

            if (bc_Creature.DoingBandage)
            {
                if (bc_Creature.HealTarget != null && (bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget) <= (double)bc_Creature.RangePerception) && bc_Creature.HealTarget.Alive && !bc_Creature.HealTarget.Deleted && bc_Creature.Map == bc_Creature.HealTarget.Map && !bc_Creature.HealTarget.IsDeadBondedPet && bc_Creature.CanSee(bc_Creature.HealTarget))
                {
                    if (bc_Creature.HealTarget == bc_Creature)
                    {
                    }

                    else if (bc_Creature.BandageOtherReady && bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget) < 2)
                    {
                        AIHeal.StartBandageHeal(bc_Creature);
                        bandagingOther = true;
                    }

                    else
                        bandagingOther = true;
                }

                else
                    AIHeal.BandageFail(bc_Creature);
            }

            if (DateTime.UtcNow > bc_Creature.NextWanderActionAllowed)
            {
                if (SpecialAbilities.Global_AllowAbilities)
                {
                    if (!bc_Creature.BardPacified && !bc_Creature.BardPacified)
                        GetWanderAction();
                }
            }

            if (!CheckMove() || bc_Creature.DisallowAllMoves)
                return false;

            if (bandagingOther)
            {
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.HealTarget);

                bool bRun = (iCurrDist > 2);

                WalkToMobile(bc_Creature.HealTarget, 1, bRun, 0, 1);

                return true;
            }

            else
            {
                if (CheckMove())
                {
                    if (bc_Creature.CurrentWaypoint != null && !bc_Creature.CantWalk)
                    {
                        #region WaypointHandling

                        bool allowJumpTo = true;

                        UOACZWayPoint uoaczWaypoint = bc_Creature.CurrentWaypoint as UOACZWayPoint;

                        if (uoaczWaypoint != null)
                        {
                            if (!uoaczWaypoint.AllowJumpTo)
                                allowJumpTo = false;
                        }

                        WayPoint waypoint = bc_Creature.CurrentWaypoint;
                        WayPoint previousWaypoint;

                        bool closeEnoughToWaypoint = false;
                        double distanceToWaypoint = bc_Creature.GetDistanceToSqrt(waypoint);

                        if (bc_Creature.WaypointOrder == WayPointOrder.IdleForward || bc_Creature.WaypointOrder == WayPointOrder.IdleReverse)
                            closeEnoughToWaypoint = true;

                        else
                        {
                            if (distanceToWaypoint <= bc_Creature.WaypointCompletionProximityNeeded)
                                closeEnoughToWaypoint = true;

                            else
                            {
                                if (bc_Creature.VisitedWaypoints != null)
                                {
                                    if (bc_Creature.VisitedWaypoints.Count > 0)
                                    {
                                        if (DateTime.UtcNow > bc_Creature.NextWaypointAction)
                                        {
                                            if (allowJumpTo)
                                            {
                                                closeEnoughToWaypoint = true;

                                                bc_Creature.MoveToWorld(bc_Creature.CurrentWaypoint.Location, bc_Creature.Map);
                                                //bc_Creature.SetLocation(bc_Creature.CurrentWaypoint.Location, false);
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if (closeEnoughToWaypoint)
                        {
                            if (bc_Creature.VisitedWaypoints.IndexOf(waypoint) < 0)
                                bc_Creature.VisitedWaypoints.Add(waypoint);

                            if (waypoint.DoWaypointAction)
                                GetWaypointAction();

                            bool considerNextWaypointType = true;

                            if (uoaczWaypoint != null)
                            {
                                if (uoaczWaypoint.WaypointType == UOACZWayPoint.UOACZWaypointType.UndeadTown)
                                {
                                    if (bc_Creature is UOACZBaseUndead)
                                    {
                                        UOACZBaseUndead undeadCreature = bc_Creature as UOACZBaseUndead;
                                        undeadCreature.InWilderness = false;
                                    }
                                }

                                if (uoaczWaypoint.m_GotoSplitGroup > -1)
                                {
                                    List<UOACZWayPoint> m_UOACZWaypoints = new List<UOACZWayPoint>();

                                    foreach (UOACZWayPoint targetWaypoint in UOACZWayPoint.m_UOACZWaypoints)
                                    {
                                        if (!UOACZRegion.ContainsItem(targetWaypoint)) continue;

                                        if (targetWaypoint.WaypointType == uoaczWaypoint.WaypointType && targetWaypoint.SplitGroup == uoaczWaypoint.GotoSplitGroup)
                                        {
                                            if (!targetWaypoint.Deleted)
                                                m_UOACZWaypoints.Add(targetWaypoint);
                                        }
                                    }

                                    if (m_UOACZWaypoints.Count > 0)
                                    {
                                        bc_Creature.CurrentWaypoint = m_UOACZWaypoints[Utility.RandomMinMax(0, m_UOACZWaypoints.Count - 1)];
                                        SetWaypointTimeout(bc_Creature);

                                        considerNextWaypointType = false;
                                    }
                                }
                            }

                            if (considerNextWaypointType)
                            {
                                switch (waypoint.Behavior)
                                {
                                    case WaypointBehavior.NextWaypoint:
                                        if (bc_Creature.WaypointOrder == WayPointOrder.Forward)
                                        {
                                            if (waypoint.NextWaypoint != null)
                                            {
                                                bc_Creature.CurrentWaypoint = waypoint.NextWaypoint;
                                                SetWaypointTimeout(bc_Creature);
                                            }

                                            else
                                                SendCreatureTowardsHome();
                                        }

                                        else if (bc_Creature.WaypointOrder == WayPointOrder.Reverse)
                                        {
                                            previousWaypoint = GetPreviousWaypoint(bc_Creature, waypoint);

                                            if (previousWaypoint != null)
                                            {
                                                bc_Creature.CurrentWaypoint = previousWaypoint;
                                                SetWaypointTimeout(bc_Creature);
                                            }

                                            else
                                            {
                                                if (waypoint.NextWaypoint != null)
                                                {
                                                    bc_Creature.WaypointOrder = WayPointOrder.Forward;
                                                    bc_Creature.CurrentWaypoint = waypoint.NextWaypoint;

                                                    SetWaypointTimeout(bc_Creature);
                                                }

                                                else
                                                    SendCreatureTowardsHome();
                                            }
                                        }
                                        break;

                                    case WaypointBehavior.ReverseOrder:
                                        bc_Creature.WaypointOrder = WayPointOrder.Reverse;

                                        previousWaypoint = GetPreviousWaypoint(bc_Creature, waypoint);

                                        if (previousWaypoint != null)
                                        {
                                            bc_Creature.CurrentWaypoint = previousWaypoint;
                                            SetWaypointTimeout(bc_Creature);
                                        }

                                        else
                                            SendCreatureTowardsHome();

                                        break;

                                    case WaypointBehavior.ReturnToStart:
                                        if (waypoint.XMLSpawners != null)
                                        {
                                            bool foundWaypoint = false;

                                            foreach (WayPoint visitedWaypoint in bc_Creature.VisitedWaypoints)
                                            {
                                                if (visitedWaypoint != null)
                                                {
                                                    bc_Creature.CurrentWaypoint = visitedWaypoint;
                                                    foundWaypoint = true;

                                                    SetWaypointTimeout(bc_Creature);

                                                    break;
                                                }
                                            }

                                            if (foundWaypoint == false)
                                                SendCreatureTowardsHome();
                                        }

                                        else
                                            SendCreatureTowardsHome();
                                        break;

                                    case WaypointBehavior.SetHomeHere:
                                        bc_Creature.Home = waypoint.Location;
                                        bc_Creature.CurrentWaypoint = null;
                                        break;

                                    case WaypointBehavior.Idle10secThenNextWaypoint:
                                        DoWaypointIdle(10, WaypointBehavior.NextWaypoint);
                                        break;

                                    case WaypointBehavior.Idle30secThenNextWaypoint:
                                        DoWaypointIdle(30, WaypointBehavior.NextWaypoint);
                                        break;

                                    case WaypointBehavior.Idle10secThenReverseOrder:
                                        DoWaypointIdle(10, WaypointBehavior.ReverseOrder);
                                        break;

                                    case WaypointBehavior.Idle30secThenReverseOrder:
                                        DoWaypointIdle(30, WaypointBehavior.ReverseOrder);
                                        break;
                                }
                            }
                        }

                        else if (waypoint.Map == bc_Creature.Map && waypoint.Parent == null && !waypoint.Deleted)
                            WalkToLocation(bc_Creature.CurrentWaypoint.Location, 1, false, 0, 0);

                        else
                            SendCreatureTowardsHome();

                        #endregion
                    }

                    else
                    {
                        if (bc_Creature.ReturnsHome && !pacified)
                        {
                            double distanceFromHome = bc_Creature.GetDistanceToSqrt(bc_Creature.Home);

                            if ((distanceFromHome <= (double)bc_Creature.RangeHome))
                            {
                                int adjustedWalkOutsideHomeLimit = bc_Creature.WalkRandomOutsideHomeLimit;
                                int adjustedWalkTowardsHomeLimit = bc_Creature.WalkTowardsHomeLimit;

                                m_WalkRandomOutsideHome = DateTime.UtcNow + TimeSpan.FromSeconds(adjustedWalkOutsideHomeLimit);
                                m_WalkTowardsHome = DateTime.UtcNow + TimeSpan.FromSeconds(adjustedWalkOutsideHomeLimit + adjustedWalkTowardsHomeLimit);

                                if (distanceFromHome > ((double)bc_Creature.RangeHome * .75))
                                {
                                    if (!bc_Creature.CheckIdle() && Utility.RandomDouble() < .50)
                                        DoMove(bc_Creature.GetDirectionTo(bc_Creature.Home));
                                }

                                else
                                {
                                    if (!bc_Creature.CheckIdle() && Utility.RandomDouble() < .50)
                                        WalkRandomInHome(2, 2, 1);
                                }
                            }

                            else
                            {
                                //Wander In Current Location
                                if (DateTime.UtcNow < m_WalkRandomOutsideHome)
                                    WalkRandom(2, 2, 1);

                                //Walk Towards Home
                                else if (DateTime.UtcNow > m_WalkRandomOutsideHome && DateTime.UtcNow < m_WalkTowardsHome)
                                    DoMove(bc_Creature.GetDirectionTo(bc_Creature.Home));

                                //Teleport Home
                                else
                                {
                                    if (!pacified && !(bc_Creature is BladeSpirits || bc_Creature is EnergyVortex))
                                    {
                                        if (bc_Creature.PostPeacemakingTeleportDelay <= DateTime.UtcNow)
                                        {
                                            Effects.SendLocationParticles(EffectItem.Create(bc_Creature.Location, bc_Creature.Map, TimeSpan.FromSeconds(5.0)), 0x3728, 10, 10, 2023);

                                            bc_Creature.Location = bc_Creature.Home;
                                            WalkRandom(2, 2, 1);
                                        }
                                    }
                                }
                            }
                        }

                        else if (bc_Creature.ReturnsHome && pacified)
                            WalkRandom(2, 2, 1);

                        else
                        {
                            if (!bc_Creature.CheckIdle())
                                WalkRandom(2, 2, 1);
                        }
                    }
                }
            }

            return true;
        }

        public void SetWaypointTimeout(BaseCreature creature)
        {
            if (creature == null)
                return;

            if (creature.CurrentWaypoint == null)
                return;

            double distanceToWaypoint = creature.GetDistanceToSqrt(creature.CurrentWaypoint.Location);
            double estimatedTimeNeeded = distanceToWaypoint * creature.PassiveSpeed * 8;

            creature.NextWaypointAction = DateTime.UtcNow + TimeSpan.FromSeconds(estimatedTimeNeeded);
        }

        public WayPoint GetPreviousWaypoint(BaseCreature creature, WayPoint currentWaypoint)
        {
            int currentWaypointPosition = creature.VisitedWaypoints.IndexOf(currentWaypoint);

            if (currentWaypointPosition == null)
                return null;

            if (currentWaypointPosition == 0)
                return null;

            for (int a = 1; a < currentWaypointPosition + 1; a++)
            {
                if (creature.VisitedWaypoints[currentWaypointPosition - a] != null)
                {
                    return creature.VisitedWaypoints[currentWaypointPosition - a];
                    break;
                }
            }

            return null;
        }

        public void SendCreatureTowardsHome()
        {
            bc_Creature.NextWaypointAction = DateTime.UtcNow;
            bc_Creature.CurrentWaypoint = null;

            m_WalkRandomOutsideHome = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.WalkRandomOutsideHomeLimit);
            m_WalkTowardsHome = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.WalkRandomOutsideHomeLimit + bc_Creature.WalkTowardsHomeLimit);
        }

        public bool DoFleeAction()
        {
            bool validCombatant = false;

            //If Still Fleeing
            if (FleeCheck())
            {
                bc_Creature.RevealingAction();

                //Current Combatant Was Set In FleeCheck(), But Still Check if Current Combatant is Valid
                if (bc_Creature.Combatant != null && bc_Creature.Combatant.Alive && !bc_Creature.Combatant.Deleted && bc_Creature.Combatant.Map == bc_Creature.Map && bc_Creature.GetDistanceToSqrt(bc_Creature.Combatant) <= bc_Creature.RangePerception)
                {
                    validCombatant = true;
                }

                //Has Valid Combatant to Flee From
                if (validCombatant)
                {
                    Direction d = bc_Creature.Combatant.GetDirectionTo(bc_Creature);

                    double FleeDirectionChance = Utility.RandomDouble();

                    if (FleeDirectionChance < .60)
                        d = (Direction)(int)d;

                    else if (FleeDirectionChance < .75)
                        d = (Direction)((int)d + Utility.RandomMinMax(-1, +1));

                    else if (FleeDirectionChance < .90)
                        d = (Direction)((int)d + Utility.RandomMinMax(-2, +2));

                    else
                        d = (Direction)((int)d + Utility.RandomMinMax(-3, +3));

                    //If Ready for Movement
                    if (CheckMove())
                    {
                        bc_Creature.Direction = d;
                        bc_Creature.Move(d);

                        TimeSpan delay = TimeSpan.FromSeconds(bc_Creature.CurrentSpeed);
                        NextMove += delay;
                    }
                }

                else
                {
                    Map map = bc_Creature.Map;
                    IPooledEnumerable eable = map.GetMobilesInRange(bc_Creature.Location, bc_Creature.RangePerception);

                    Mobile closest = null;

                    foreach (Mobile target in eable)
                    {
                        //If Target is Not Teammate (Self is Allowed!)
                        //if (!AITeamList.CheckTeam(bc_Creature, target))
                            //continue;

                        //Ignore If Deleted or Blessed
                        if (target.Deleted || target.Blessed)
                            continue;

                        //Ignore If Dead
                        if (!target.Alive || target.IsDeadBondedPet)
                            continue;

                        //Ignore ServerStaff
                        if (target.AccessLevel > AccessLevel.Player)
                            continue;

                        //Ignore If Target out of Immediate Area
                        if (!bc_Creature.CanSee(target))
                            continue;

                        //Ignore If Target out of LOS
                        if (!bc_Creature.InLOS(target))
                            continue;

                        if (closest == null)
                            closest = target;

                        else
                        {
                            //If Creature is Closer Than Current Closest Target
                            if (bc_Creature.GetDistanceToSqrt(target.Location) < bc_Creature.GetDistanceToSqrt(closest.Location))
                                closest = target;
                        }
                    }

                    //End Search
                    eable.Free();

                    Direction d;

                    //Random Direction
                    if (closest == null)
                        d = (Direction)Utility.RandomMinMax(0, 7);

                    //Opposite of Nearest Target
                    else
                        d = closest.GetDirectionTo(bc_Creature);

                    //If Ready for Movement
                    if (CheckMove())
                    {
                        bc_Creature.Direction = d;
                        bc_Creature.Move(d);

                        TimeSpan delay = TimeSpan.FromSeconds(bc_Creature.CurrentSpeed);
                        NextMove += delay;
                    }
                }
            }

            return true;
        }

        public bool DoInteractAction()
        {
            return true;
        }

        public bool GetReactionToPlayerStatus()
        {
            if (bc_Creature.Controlled == true && bc_Creature.ControlMaster != null)
            {
                if (bc_Creature.ControlOrder == OrderType.Release)
                    return true;

                if (bc_Creature.ControlMaster.Player && bc_Creature.Map == bc_Creature.ControlMaster.Map)
                {
                    PlayerMobile pm_Owner = bc_Creature.ControlMaster as PlayerMobile;

                    //Owner is Offline
                    if (pm_Owner.NetState == null)
                    {
                        if (bc_Creature.ControlOrder != OrderType.Stop)
                        {
                            bc_Creature.ControlOrder = OrderType.Stop;
                            bc_Creature.AIObject.DoOrderStop();
                        }

                        return false;
                    }

                    //Within Maximum Control Range
                    if (bc_Creature.GetDistanceToSqrt(bc_Creature.ControlMaster) <= ((double)bc_Creature.RangePerception * 4))
                    {
                        //Owner is Dead
                        if (!bc_Creature.ControlMaster.Alive && !bc_Creature.BardProvoked)
                        {
                            if (bc_Creature.ControlOrder == OrderType.Attack || bc_Creature.ControlOrder == OrderType.Patrol || bc_Creature.ControlOrder == OrderType.Guard)
                            {
                                bc_Creature.ControlOrder = OrderType.Stop;
                                bc_Creature.AIObject.DoOrderStop();
                            }

                            return false;
                        }

                        //Normal Order Handling
                        else
                            return true;
                    }

                    //Outside of Creature Control Range
                    else
                    {
                        if (!bc_Creature.BardProvoked)
                        {
                            //Was Previously Following, So Go Into Stop Mode (Won't Fight Enemies)
                            if (bc_Creature.ControlOrder == OrderType.Follow)
                                bc_Creature.ControlOrder = OrderType.Stop;

                            //Go Into Stay Mode (Or Stop Mode if Player is In Temp-Statloss
                            else
                            {
                                if (bc_Creature.ControlOrder != OrderType.Stop)
                                    bc_Creature.ControlOrder = OrderType.Stop;
                            }
                        }

                        return false;
                    }
                }
            }

            return false;
        }

        public bool ReceiveOrderCome(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.CheckControlChance(from))
            {
                bc_Creature.ControlOrder = OrderType.Come;
                bc_Creature.ControlDest = from.Location;

                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("Your follower is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");
                }

                return false;
            }
        }

        public bool ReceiveOrderFetch(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.Region is UOACZRegion)
            {
                from.SendMessage("That command is not usable while in UOACZ.");
                return false;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to issue that command.");
                return false;
            }

            if (bc_Creature.IsDeadPet)
            {
                if (bc_Creature.IsHenchman)
                    from.SendMessage("You cannot command a deceased individual to do that.");
                else
                    from.SendMessage("You cannot command a deceased pet to do that.");

                return false;
            }

            if (bc_Creature.CheckControlChance(from))
            {
                PlayerMobile player = from as PlayerMobile;

                if (bc_Creature.Backpack is StrongBackpack || bc_Creature is StrongBearBackpack)
                {
                    BeginPickTarget(from, OrderType.Fetch);

                    return true;
                }

                else
                {
                    from.SendMessage("That is not a pack animal.");
                    return false;
                }
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderFollow(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.CheckControlChance(from))
            {
                bc_Creature.ControlOrder = OrderType.Follow;
                bc_Creature.ControlTarget = from;

                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderGuard(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to issue that command.");
                return false;
            }

            PlayerMobile player = from as PlayerMobile;

            if (bc_Creature.IsDeadPet)
            {
                from.SendMessage("You cannot command a deceased pet to do that.");
                return false;
            }

            if (bc_Creature.CheckControlChance(from))
            {
                bc_Creature.ControlOrder = OrderType.Guard;
                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderStop(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.CheckControlChance(from))
            {
                bc_Creature.ControlOrder = OrderType.Stop;
                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderAttack(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.Region is UOACZRegion)
            {
                from.SendMessage("Swarm followers may only guard.");
                return false;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to issue that command.");
                return false;
            }

            PlayerMobile player = from as PlayerMobile;

            if (bc_Creature.IsDeadPet)
            {
                from.SendMessage("You cannot command a deceased pet to do that.");
                return false;
            }

            if (bc_Creature.CheckControlChance(from))
            {
                BeginPickTarget(from, OrderType.Attack);

                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderStay(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            PlayerMobile player = from as PlayerMobile;

            if (bc_Creature.CheckControlChance(from))
            {
                bc_Creature.ControlOrder = OrderType.Stay;
                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderRelease(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (!bc_Creature.Summoned)
                from.SendGump(new Gumps.ConfirmReleaseGump(from, bc_Creature));

            else
            {
                bc_Creature.ControlOrder = OrderType.Release;
                return true;
            }

            return false;
        }

        public bool ReceiveOrderTransfer(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.Region is UOACZRegion)
            {
                from.SendMessage("That command is not usable while in UOACZ.");
                return false;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to issue that command.");
                return false;
            }

            PlayerMobile player = from as PlayerMobile;

            if (bc_Creature.IsDeadPet)
            {
                from.SendMessage("You cannot command a deceased follower to do that.");
                return false;
            }

            if (bc_Creature.Summoned)
            {
                from.SendMessage("You cannot transfer a summoned creature.");
                return false;
            }

            if (from.HasTrade)
            {
                from.SendMessage("You cannot transfer a follower with a trade pending.");
                return false;
            }

            if (bc_Creature.CheckControlChance(from))
            {
                BeginPickTarget(from, OrderType.Transfer);
                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public bool ReceiveOrderPatrol(Mobile from)
        {
            if (from == null || bc_Creature == null)
                return false;

            if (bc_Creature.Region is UOACZRegion)
            {
                from.SendMessage("Swarm followers may only guard.");
                return false;
            }

            if (!from.Alive)
            {
                from.SendMessage("You must be alive to issue that command.");
                return false;
            }

            PlayerMobile player = from as PlayerMobile;

            if (bc_Creature.IsDeadPet)
            {
                from.SendMessage("You cannot command a deceased pet to do that.");
                return false;
            }

            if (bc_Creature.CheckControlChance(from))
            {
                BeginPickTarget(from, OrderType.Patrol);

                return true;
            }

            else
            {
                if (bc_Creature.IsBarded())
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("That individual is entranced and refuses your command!");
                    else
                        from.SendMessage("Your creature is entranced and refuses your command!");

                    return false;
                }

                else
                {
                    if (bc_Creature.IsHenchman)
                        from.SendMessage("You must have Begging and Camping skill equal to this individual's 'taming' difficulty in order to command this individual!");
                    else
                        from.SendMessage("You must have Animal Taming and Animal Lore skill equal to this creature's taming difficulty in order to command this creature!");

                    return false;
                }
            }

            return false;
        }

        public static void BeginPickAllPatrolTarget(Mobile from)
        {
            from.BeginTarget(15, true, TargetFlags.None, new TargetCallback(PickAllPatrolTarget));
            from.SendMessage("Click on the location where you wish your followers to patrol.");
        }

        private void PickPatrolTarget(Mobile from, object obj)
        {
            IPoint3D p = obj as IPoint3D;

            if (p == null)
                return;

            bc_Creature.ControlOrder = OrderType.Patrol;
            bc_Creature.Home = new Point3D(p);
            bc_Creature.ControlDest = new Point3D(p);
        }

        private static void PickAllPatrolTarget(Mobile from, object obj)
        {
            IPoint3D p = obj as IPoint3D;

            if (p == null)
                return;

            bool henchmenCommandFailed = false;
            bool animalCommandFailed = false;

            IPooledEnumerable eable = from.GetMobilesInRange(BaseCreature.DefaultPerceptionRange);

            foreach (Mobile target in eable)
            {
                BaseCreature bc_Target = target as BaseCreature;

                if (bc_Target == null || !bc_Target.Alive || bc_Target.Deleted || !bc_Target.Controlled || from.Map != bc_Target.Map || !from.CheckAlive())
                    continue;

                if (from != bc_Target.ControlMaster)
                    continue;

                if (bc_Target.CheckControlChance(from))
                {
                    bc_Target.ControlOrder = OrderType.Patrol;
                    bc_Target.Home = new Point3D(p);
                    bc_Target.ControlDest = new Point3D(p);
                }

                else
                {
                    if (bc_Target.IsHenchman)
                        henchmenCommandFailed = true;
                    else
                        animalCommandFailed = true;

                    continue;
                }
            }

            eable.Free();

            if (henchmenCommandFailed)
                from.SendMessage("Your lack of Begging or Camping skill made one or more of your followers unable to follow this command!");

            if (animalCommandFailed)
                from.SendMessage("Your lack of Animal Taming or Animal Lore skill made one or more of your creatures unable to follow this command!");
        }

        public static void BeginPickAllFetchTarget(Mobile from)
        {
            from.BeginTarget(15, true, TargetFlags.None, new TargetCallback(PickAllFetchTarget));
            from.SendMessage("Click the item you wish your followers to fetch");
        }

        private static void PickAllFetchTarget(Mobile from, object obj)
        {
            bool henchmenCommandFailed = false;
            bool aniamlCommandFailed = false;

            var mobiles = from.GetMobilesInRange(BaseCreature.DefaultPerceptionRange);

            try
            {
                foreach (Mobile target in mobiles)
                {
                    BaseCreature bc_Target = target as BaseCreature;

                    if (bc_Target == null || !bc_Target.Alive || bc_Target.Deleted || !bc_Target.Controlled || from.Map != bc_Target.Map || !from.CheckAlive())
                        continue;

                    bool isOwner = (from == bc_Target.ControlMaster);

                    if (!isOwner)
                        continue;

                    if (bc_Target.CheckControlChance(from))
                    {
                        Item objTarget = obj as Item;

                        if (objTarget == null)
                            return;

                        IPoint3D p = objTarget.Location as IPoint3D;

                        if (p == null)
                            return;

                        if (bc_Target.Backpack is StrongBackpack || bc_Target.Backpack is StrongBearBackpack)
                        {
                            bc_Target.ControlObject = objTarget;
                            bc_Target.ControlOrder = OrderType.Fetch;

                            if (bc_Target.AIObject != null)
                                bc_Target.AIObject.DoOrderFetch();
                        }
                    }

                    else
                    {
                        if (bc_Target.IsHenchman)
                            henchmenCommandFailed = true;

                        else
                            aniamlCommandFailed = true;

                        continue;
                    }
                }

            }

            catch (ArgumentOutOfRangeException ex)
            {
                Console.WriteLine("PickAllFetchTarget Failed: {0}", ex.Message);
            }

            mobiles.Free();

            if (henchmenCommandFailed)
                from.SendMessage("Your lack of Begging or Camping skill made one or more of your followers unable to follow this command!");

            if (aniamlCommandFailed)
                from.SendMessage("Your lack of Animal Taming or Animal Lore skill made one or more of your creatures unable to follow this command!");
        }

        public virtual bool DoOrderAttack()
        {
            bool validTarget = false;
            bool validControlMaster = false;

            if (bc_Creature.IsDeadPet)
                return true;

            if (bc_Creature.ControlTarget != null && !bc_Creature.ControlTarget.Deleted && bc_Creature.CanSee(bc_Creature.ControlTarget) && bc_Creature.ControlTarget.Map == bc_Creature.Map && bc_Creature.ControlTarget.Alive && !bc_Creature.ControlTarget.IsDeadBondedPet)
                validTarget = true;

            if (bc_Creature.ControlMaster != null && !bc_Creature.ControlMaster.Deleted && bc_Creature.ControlMaster.Map == bc_Creature.Map && bc_Creature.ControlMaster.Alive && !bc_Creature.IsDeadBondedPet)
                validControlMaster = true;

            else if (bc_Creature.BardProvoked)
                validControlMaster = true;

            if (validTarget && validControlMaster)
            {
                bc_Creature.Combatant = bc_Creature.ControlTarget;
                bc_Creature.DoHarmful(bc_Creature.ControlTarget);

                DoCombatAction();
            }

            else
            {
                bc_Creature.ControlTarget = null;

                if (bc_Creature.ControlOrder != OrderType.Stop)
                    bc_Creature.ControlOrder = OrderType.Stop;
            }

            return true;
        }

        public virtual bool DoOrderPatrol()
        {
            Mobile controlMaster = bc_Creature.ControlMaster;
            Mobile combatant = bc_Creature.Combatant;

            bool foundAggressors = false;
            bool validControlMaster = false;

            //If Creature Has Valid ControlMaster
            if (bc_Creature.ControlMaster != null && !bc_Creature.ControlMaster.Deleted && bc_Creature.ControlMaster.Map == bc_Creature.Map && bc_Creature.ControlMaster.Alive && !bc_Creature.IsDeadBondedPet)
                validControlMaster = true;

            bc_Creature.Combatant = GetAnyAggressor();

            if (bc_Creature.Combatant != null)
                foundAggressors = true;

            if (foundAggressors)
                DoCombatAction();
            else
            {
                if (validControlMaster && bc_Creature.GetDistanceToSqrt(controlMaster) <= bc_Creature.RangePerception)
                {
                    if (controlMaster.Combatant != null)
                    {
                        if (bc_Creature.GetDistanceToSqrt(controlMaster.Combatant) <= bc_Creature.RangePerception && !controlMaster.Combatant.Hidden)
                        {
                            bc_Creature.Combatant = controlMaster.Combatant;
                            bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);

                            DoCombatAction();

                            return true;
                        }
                    }
                }

                //Distance to Patrol Point
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.ControlDest);

                if (iCurrDist > bc_Creature.RangePerception)
                {
                    bool bRun = (iCurrDist > 2);

                    if (WalkToLocation(bc_Creature.ControlDest, 1, bRun, 0, 1))
                    {
                    }
                }

                else
                {
                    if ((double)iCurrDist > (double)(bc_Creature.RangePerception / 2))
                    {
                        if (WalkToLocation(bc_Creature.ControlDest, 1, false, 0, 1))
                        {
                        }
                    }

                    else
                        WalkRandomInHome(3, 2, 1);
                }

            }

            return true;
        }

        public virtual bool DoOrderGuard()
        {
            if (bc_Creature == null) return false;

            bool validControlMaster = false;
            bool foundAggressors = false;

            if (bc_Creature.ControlMaster != null)
            {
                if (!bc_Creature.ControlMaster.Deleted && bc_Creature.ControlMaster.Map == bc_Creature.Map && bc_Creature.ControlMaster.Alive && !bc_Creature.IsDeadBondedPet)
                {
                    validControlMaster = true;

                    if (bc_Creature.GetDistanceToSqrt(bc_Creature.ControlMaster) <= bc_Creature.RangePerception * 3)
                        bc_Creature.Home = bc_Creature.Location;
                }
            }

            bc_Creature.Combatant = GetAnyAggressor();

            if (bc_Creature.Combatant != null)
            {
                if (!bc_Creature.Combatant.Hidden)
                {
                    bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);
                    foundAggressors = true;
                }
            }

            else
            {
                if (validControlMaster)
                {
                    if (bc_Creature.ControlMaster.Combatant != null)
                    {
                        if (bc_Creature.GetDistanceToSqrt(bc_Creature.ControlMaster.Combatant) <= bc_Creature.RangePerception * 2)
                        {
                            bc_Creature.Combatant = bc_Creature.ControlMaster.Combatant;
                            bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);

                            foundAggressors = true;
                        }
                    }
                }
            }

            if (foundAggressors)
            {
                if (validControlMaster)
                {
                    if (bc_Creature.GetDistanceToSqrt(bc_Creature.ControlMaster) > bc_Creature.RangePerception * 2)
                    {
                        bc_Creature.ControlTarget = bc_Creature.ControlMaster;
                        DoOrderFollow();
                    }

                    else
                        DoCombatAction();
                }

                else
                    DoCombatAction();
            }

            else
            {
                if (validControlMaster)
                {
                    bc_Creature.ControlTarget = bc_Creature.ControlMaster;
                    DoOrderFollow();
                }

                else
                    DoOrderStay();
            }

            return true;
        }

        public virtual bool DoOrderCome()
        {
            //Valid Controller
            if (bc_Creature.ControlMaster != null && !bc_Creature.ControlMaster.Deleted && bc_Creature.ControlMaster.Map == bc_Creature.Map)
            {
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.ControlDest);

                if (!(iCurrDist > (bc_Creature.RangePerception * 4)))
                {
                    bool bRun = (iCurrDist > 2);

                    if (WalkToLocation(bc_Creature.ControlDest, 1, bRun, 0, 1))
                    {
                    }
                }
            }

            return true;
        }

        public virtual bool DoOrderFollow()
        {
            if (bc_Creature.Combatant != null)
                bc_Creature.Combatant = null;

            if (bc_Creature.ControlTarget != null && !bc_Creature.ControlTarget.Deleted && bc_Creature.ControlTarget != bc_Creature)
            {
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.ControlTarget);

                if (!(iCurrDist > (bc_Creature.RangePerception * 4)))
                {
                    bool bRun = (iCurrDist > 2);

                    if (WalkToMobile(bc_Creature.ControlTarget, 1, bRun, 0, 1))
                    {
                    }
                }
            }

            else
            {
                bc_Creature.ControlTarget = null;

                PlayerMobile pm_ControlMaster = bc_Creature.ControlMaster as PlayerMobile;

                if (pm_ControlMaster != null)
                {
                    if (bc_Creature.ControlOrder != OrderType.Stay)
                        bc_Creature.ControlOrder = OrderType.Stay;
                }

                else
                {
                    if (bc_Creature.ControlOrder != OrderType.Stay)
                        bc_Creature.ControlOrder = OrderType.Stay;
                }
            }

            return true;
        }

        public virtual bool DoOrderStay()
        {
            bc_Creature.Combatant = GetSelfAggressor();

            if (bc_Creature.Combatant != null)
            {
                bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);
                DoCombatAction();
            }

            else if (bc_Creature.ControlMaster != null)
                bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.ControlMaster);

            return true;
        }

        public virtual bool DoOrderStop()
        {
            if (bc_Creature.Hidden)
                bc_Creature.RevealingAction();

            return true;
        }

        public virtual bool DoOrderNone()
        {
            bc_Creature.Combatant = GetSelfAggressor();

            if (bc_Creature.Combatant != null)
                bc_Creature.Direction = bc_Creature.GetDirectionTo(bc_Creature.Combatant);
            else
                WalkRandomInHome(3, 2, 1);

            return true;
        }

        public virtual bool DoOrderDrop()
        {
            if (bc_Creature.IsDeadPet || !bc_Creature.CanDrop)
                return true;

            bc_Creature.DebugSay("I drop my stuff for my master");

            Container pack = bc_Creature.Backpack;

            if (pack != null)
            {
                List<Item> list = pack.Items;

                for (int i = list.Count - 1; i >= 0; --i)
                    if (i < list.Count)
                        list[i].MoveToWorld(bc_Creature.Location, bc_Creature.Map);
            }

            bc_Creature.ControlTarget = null;

            PlayerMobile pm_ControlMaster = bc_Creature.ControlMaster as PlayerMobile;

            if (pm_ControlMaster != null)
            {
                if (bc_Creature.ControlOrder != OrderType.Stay)
                    bc_Creature.ControlOrder = OrderType.Stay;
            }

            else
            {
                if (bc_Creature.ControlOrder != OrderType.Stay)
                    bc_Creature.ControlOrder = OrderType.Stay;
            }

            return true;
        }

        public virtual bool DoOrderFriend()
        {
            return true;
        }

        public virtual bool DoOrderUnfriend()
        {
            return true;
        }

        public virtual bool DoOrderTransfer()
        {
            if (bc_Creature == null)
                return false;

            Mobile from = bc_Creature.ControlMaster;
            Mobile to = bc_Creature.ControlTarget;

            if (from != to && from != null && !from.Deleted && to != null && !to.Deleted && to.Player)
            {
                bc_Creature.DebugSay("Begin transfer with {0}", to.Name);

                if (bc_Creature.IsDeadPet || !bc_Creature.Alive)
                {
                    from.SendMessage("You may only transfer living creatures or individuals.");

                    bc_Creature.ControlTarget = null;

                    if (bc_Creature.ControlOrder != OrderType.Stop)
                        bc_Creature.ControlOrder = OrderType.Stop;

                    return false;
                }

                if (bc_Creature.IsHenchman)
                {
                    if (to.Skills[SkillName.Begging].Value < bc_Creature.MinTameSkill || to.Skills[SkillName.Camping].Value < bc_Creature.MinTameSkill)
                    {
                        from.SendMessage("The individual refuses to be transferred to the new controller. They do not have enough begging skill and camping skill to command that individual.");
                        to.SendMessage("The individual refuses to be transferred to you. You do not have enough begging skill and camping skill to command that individual.");

                        bc_Creature.ControlTarget = null;

                        if (bc_Creature.ControlOrder != OrderType.Stop)
                            bc_Creature.ControlOrder = OrderType.Stop;

                        return false;
                    }

                    if (from.Skills[SkillName.Begging].Value < bc_Creature.MinTameSkill || from.Skills[SkillName.Camping].Value < bc_Creature.MinTameSkill)
                    {
                        from.SendMessage("The pet refuses to be transferred  to the new handler. You do not have enough taming skill and animal lore to command the creature.");
                        to.SendMessage("The pet refuses to be transferred to you. The owner does not have enough taming skill and animal lore to command the creature.");

                        bc_Creature.ControlTarget = null;

                        if (bc_Creature.ControlOrder != OrderType.Stop)
                            bc_Creature.ControlOrder = OrderType.Stop;

                        return false;
                    }
                }

                else
                {
                    if (to.Skills[SkillName.AnimalTaming].Value < bc_Creature.MinTameSkill || to.Skills[SkillName.AnimalLore].Value < bc_Creature.MinTameSkill)
                    {
                        from.SendMessage("The pet refuses to be transferred to the new handler. They do not have enough taming skill and animal lore to command the creature.");
                        to.SendMessage("The pet refuses to be transferred to you. You do not have enough taming skill and animal lore to command the creature.");

                        bc_Creature.ControlTarget = null;

                        if (bc_Creature.ControlOrder != OrderType.Stop)
                            bc_Creature.ControlOrder = OrderType.Stop;

                        return false;
                    }

                    if (from.Skills[SkillName.AnimalTaming].Value < bc_Creature.MinTameSkill || from.Skills[SkillName.AnimalLore].Value < bc_Creature.MinTameSkill)
                    {
                        from.SendMessage("The pet refuses to be transferred  to the new handler. You do not have enough taming skill and animal lore to command the creature.");
                        to.SendMessage("The pet refuses to be transferred to you. The owner does not have enough taming skill and animal lore to command the creature.");

                        bc_Creature.ControlTarget = null;

                        if (bc_Creature.ControlOrder != OrderType.Stop)
                            bc_Creature.ControlOrder = OrderType.Stop;

                        return false;
                    }
                }

                if (TransferItem.IsInCombat(bc_Creature))
                {
                    from.SendMessage("You may not transfer a pet that has recently been in combat.");
                    to.SendMessage("The pet may not be transfered to you because it has recently been in combat.");

                    bc_Creature.ControlTarget = null;

                    if (bc_Creature.ControlOrder != OrderType.Stop)
                        bc_Creature.ControlOrder = OrderType.Stop;

                    return false;
                }

                else
                {
                    NetState fromState = from.NetState, toState = to.NetState;

                    if (fromState != null && toState != null)
                    {
                        if (from.HasTrade)
                            from.SendLocalizedMessage(1010507); // You cannot transfer a pet with a trade pending                        

                        else if (to.HasTrade)
                            to.SendLocalizedMessage(1010507); // You cannot transfer a pet with a trade pending                        

                        else
                        {
                            Container c = fromState.AddTrade(toState);
                            c.DropItem(new TransferItem(bc_Creature));
                        }
                    }
                }
            }

            bc_Creature.ControlTarget = null;

            if (bc_Creature.ControlOrder != OrderType.Stop)
                bc_Creature.ControlOrder = OrderType.Stop;

            return true;
        }

        public virtual bool DoOrderRelease()
        {
            bc_Creature.PlaySound(bc_Creature.GetAngerSound());

            bc_Creature.SetControlMaster(null);
            bc_Creature.SummonMaster = null;

            bc_Creature.OwnerAbandonTime = DateTime.UtcNow;
            bc_Creature.IsBonded = false;

            SpawnEntry se = bc_Creature.Spawner as SpawnEntry;

            if (se != null && se.HomeLocation != Point3D.Zero)
            {
                bc_Creature.Home = se.HomeLocation;
                bc_Creature.RangeHome = se.HomeRange;
            }

            if (bc_Creature.DeleteOnRelease || bc_Creature.IsDeadPet)
                bc_Creature.Delete();

            bc_Creature.BeginDeleteTimer();

            return true;
        }

        public virtual bool DoOrderFetch()
        {
            if (bc_Creature.Combatant != null)
            {
                bc_Creature.Warmode = false;
                bc_Creature.Combatant = null;
            }

            if (bc_Creature.ControlObject == null)
            {
                DoFetchResult();

                return false;
            }

            Item item = bc_Creature.ControlObject as Item;

            if (item == null)
            {
                DoFetchResult();

                return false;
            }

            bool validItemLocation = true;

            if (!bc_Creature.InLOS(item.Location))
                validItemLocation = false;

            if (item.Map != bc_Creature.Map)
                validItemLocation = false;

            if (item.RootParent != null)
                validItemLocation = false;

            int iCurrDist = -1;

            if (validItemLocation)
            {
                iCurrDist = (int)bc_Creature.GetDistanceToSqrt(item.Location);

                if (iCurrDist > bc_Creature.RangePerception * 2)
                    validItemLocation = false;
            }

            //Item Determined to Be Non-Pursueable
            if (validItemLocation == false)
            {
                DoFetchResult();

                return false;
            }

            //Not Currently Within 2 Spaces of Target Location
            if (iCurrDist > 2)
            {
                bool bRun = true;

                if (WalkToLocation(item.Location, 1, bRun, 0, 1))
                {
                }
            }

            //Within 2 Spaces of Object
            else
            {
                bool validLift = true;

                //Creature Has No Backpack
                if (bc_Creature.Backpack == null)
                {
                    DoFetchResult();
                    return false;
                }

                int remainingItems = bc_Creature.Backpack.MaxItems - bc_Creature.Backpack.TotalItems;
                int remainingWeight = bc_Creature.Backpack.MaxWeight - bc_Creature.Backpack.TotalWeight;

                int amountToLift = 0;

                //Pack Full (Item Count or Weight)
                if (remainingItems == 0 || remainingWeight == 0)
                    validLift = false;

                //Container With Multiple Items or Potions
                if (item.TotalItems > 0)
                {
                    //Container
                    if (item is Container)
                    {
                        if (!(item.TotalWeight <= remainingWeight && (item.TotalItems + 1) <= remainingItems))
                            validLift = false;
                    }

                    //Potions
                    else
                    {
                        //How Much Can Be Lifted
                        amountToLift = (int)Math.Floor((double)remainingWeight / (double)item.Weight);

                        if (amountToLift > item.Amount)
                            amountToLift = item.Amount;

                        if (amountToLift > remainingItems)
                            amountToLift = remainingItems;
                    }
                }

                //Single Item
                else
                {
                    //Single Item of Type is Too Heavy
                    if (item.Weight > remainingWeight)
                        validLift = false;
                    else
                    {
                        amountToLift = (int)Math.Floor((double)remainingWeight / (double)item.Weight);

                        if (amountToLift > item.Amount)
                            amountToLift = item.Amount;
                    }
                }

                //Get Item If Possible
                bool rejected = true;
                LRReason reason;

                //Can Make Valid Lift Attempt
                if (validLift)
                {
                    bc_Creature.Lift(item, amountToLift, out rejected, out reason);
                }

                //Picks Up Item
                if (!rejected && bc_Creature.Drop(bc_Creature, new Point3D(0, 0, 0)))
                {
                    //Emote: Picks up Item                    
                    bc_Creature.PublicOverheadMessage(MessageType.Emote, bc_Creature.SpeechHue, true, "*picks up an item*", false);

                    DoFetchResult();
                }

                //Fails to Pick Up Item
                else
                {
                    bc_Creature.PlaySound(bc_Creature.GetIdleSound());

                    DoFetchResult();

                    return false;
                }
            }

            return true;
        }

        public void DoFetchResult()
        {
            bc_Creature.ControlObject = null;

            //Head Back to Master
            if (bc_Creature.ControlMaster != null)
            {
                bc_Creature.ControlTarget = bc_Creature.ControlMaster;
                bc_Creature.ControlOrder = OrderType.Follow;

                DoOrderFollow();
            }

            //Stay In Place
            else
            {
                PlayerMobile pm_ControlMaster = bc_Creature.ControlMaster as PlayerMobile;

                if (pm_ControlMaster != null)
                {
                    if (bc_Creature.ControlOrder != OrderType.Stay)
                    {
                        bc_Creature.ControlOrder = OrderType.Stay;
                        DoOrderStay();
                    }
                }

                else
                {
                    if (bc_Creature.ControlOrder != OrderType.Stay)
                    {
                        bc_Creature.ControlOrder = OrderType.Stay;
                        DoOrderStay();
                    }
                }
            }
        }

        //AI Decision Making-----------------------------------------------------------

        //Combat Range
        public bool CanDoWeaponAttackRange(BaseCreature creature)
        {
            if (creature.DictCombatRange[CombatRange.WeaponAttackRange] > 0)
                return true;

            return false;
        }

        public bool CanDoSpellRange(BaseCreature creature, Mobile target)
        {
            if (creature.DictCombatRange[CombatRange.SpellRange] > 0)
            {
                if (target != null)
                {
                    if (creature.InLOS(target))
                    {
                        if (creature.GetDistanceToSqrt(target) <= bc_Creature.CreatureSpellRange)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool CanDoWithdraw(BaseCreature creature, Mobile target)
        {
            if (creature.DictCombatRange[CombatRange.Withdraw] > 0)
            {
                if (target != null)
                {
                    if (creature.InLOS(target))
                    {
                        if (creature.GetDistanceToSqrt(target) < bc_Creature.CreatureWithdrawRange)
                            return true;
                    }
                }
            }

            return false;
        }


        //Combat Action
        public bool CanDoAttackOnly(BaseCreature creature)
        {
            return true;
        }

        public CombatRange GetDefaultCombatRange()
        {
            //Default Range
            CombatRange range = CombatRange.WeaponAttackRange;

            int rangeValue = 0;

            Dictionary<CombatRange, int> DictTemp = new Dictionary<CombatRange, int>();

            if (CanDoWeaponAttackRange(bc_Creature)) { DictTemp.Add(CombatRange.WeaponAttackRange, bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange]); }
            if (CanDoSpellRange(bc_Creature, bc_Creature.Combatant)) { DictTemp.Add(CombatRange.SpellRange, bc_Creature.DictCombatRange[CombatRange.SpellRange]); }
            if (CanDoWithdraw(bc_Creature, bc_Creature.Combatant)) { DictTemp.Add(CombatRange.Withdraw, bc_Creature.DictCombatRange[CombatRange.Withdraw]); }

            //Return the Highest Valued CombatRange: This Will Be The Default
            foreach (KeyValuePair<CombatRange, int> pair in DictTemp)
            {
                if (pair.Value > rangeValue)
                {
                    rangeValue = pair.Value;
                    range = pair.Key;
                }
            }

            return range;
        }

        public CombatRange GetCombatRange()
        {
            //Default Range
            CombatRange combatRange = CombatRange.WeaponAttackRange;

            //Default to WeaponAttackRange if Somehow Combatant is Gone
            if (bc_Creature.Combatant == null)
                return combatRange;

            //Default to WeaponAttackRange if Low on Mana: Will only be True if Caster
            if (m_LowMana && !(bc_Creature.Controlled && bc_Creature.ControlMaster is PlayerMobile))
                return combatRange;

            int TotalValues = 0;

            Dictionary<CombatRange, int> DictTemp = new Dictionary<CombatRange, int>();

            if (CanDoWeaponAttackRange(bc_Creature)) { DictTemp.Add(CombatRange.WeaponAttackRange, bc_Creature.DictCombatRange[CombatRange.WeaponAttackRange]); }
            if (CanDoSpellRange(bc_Creature, bc_Creature.Combatant)) { DictTemp.Add(CombatRange.SpellRange, bc_Creature.DictCombatRange[CombatRange.SpellRange]); }
            if (CanDoWithdraw(bc_Creature, bc_Creature.Combatant)) { DictTemp.Add(CombatRange.Withdraw, bc_Creature.DictCombatRange[CombatRange.Withdraw]); }

            //Calculate Total Values
            foreach (KeyValuePair<CombatRange, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double RangeCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine Desired CombatRange                      
            foreach (KeyValuePair<CombatRange, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (RangeCheck >= CumulativeAmount && RangeCheck < (CumulativeAmount + AdditionalAmount))
                {
                    combatRange = pair.Key;

                    switch (combatRange)
                    {
                        case CombatRange.WeaponAttackRange: break;
                        case CombatRange.SpellRange: break;
                        case CombatRange.Withdraw: m_NextStopWithdraw = DateTime.UtcNow + TimeSpan.FromSeconds(maxWithdrawDuration); break;
                    }

                    return combatRange;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return combatRange;
        }

        public int GetCreatureWithdrawDistance(BaseCreature creature)
        {
            int withdrawDistance = bc_Creature.CreatureWithdrawRange;

            //If Creature Casts Spells
            if (AICombatSpell.CreatureHasCastingAI(creature))
            {
                if (withdrawDistance < bc_Creature.CreatureSpellRange)
                    withdrawDistance = bc_Creature.CreatureSpellRange;
            }

            BaseWeapon weapon = creature.Weapon as BaseWeapon;
            BaseRanged rangedWeapon = creature.Weapon as BaseRanged;

            //If Mobile Has Weapon Equipped
            if (weapon != null)
            {
                //Can Back Up Further
                if (withdrawDistance < weapon.MaxRange)
                {
                    if (rangedWeapon != null)
                    {
                        withdrawDistance = rangedWeapon.MaxRange;

                        if (rangedWeapon.MaxRange > 4)
                            withdrawDistance = rangedWeapon.MaxRange - 1;
                    }

                    else
                        withdrawDistance = weapon.MaxRange;
                }
            }

            return withdrawDistance;
        }

        public CombatAction GetCombatAction()
        {
            CombatAction combatAction = CombatAction.AttackOnly;

            if (bc_Creature.IsStealthing && bc_Creature.DictCombatAction[CombatAction.AttackOnly] > 0)
                return combatAction;

            int TotalValues = 0;

            Dictionary<CombatAction, int> DictTemp = new Dictionary<CombatAction, int>();

            if (CanDoAttackOnly(bc_Creature)) { DictTemp.Add(CombatAction.AttackOnly, bc_Creature.DictCombatAction[CombatAction.AttackOnly]); }
            if (AICombatSpell.CanDoCombatSpell(bc_Creature)) { DictTemp.Add(CombatAction.CombatSpell, bc_Creature.DictCombatAction[CombatAction.CombatSpell]); }
            if (AICombatHealSelf.CanDoCombatHealSelf(bc_Creature)) { DictTemp.Add(CombatAction.CombatHealSelf, bc_Creature.DictCombatAction[CombatAction.CombatHealSelf]); }
            if (AICombatHealOther.CanDoCombatHealOther(bc_Creature)) { DictTemp.Add(CombatAction.CombatHealOther, bc_Creature.DictCombatAction[CombatAction.CombatHealOther]); }
            if (AICombatSpecialAction.CanDoCombatSpecialAction(bc_Creature)) { DictTemp.Add(CombatAction.CombatSpecialAction, bc_Creature.DictCombatAction[CombatAction.CombatSpecialAction]); }
            if (AICombatEpicAction.CanDoCombatEpicAction(bc_Creature)) { DictTemp.Add(CombatAction.CombatEpicAction, bc_Creature.DictCombatAction[CombatAction.CombatEpicAction]); }

            //Calculate Total Values
            foreach (KeyValuePair<CombatAction, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine CombatAction                      
            foreach (KeyValuePair<CombatAction, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    combatAction = pair.Key;

                    switch (combatAction)
                    {
                        case CombatAction.AttackOnly: break;
                        case CombatAction.CombatSpell: AICombatSpell.DoCombatSpell(bc_Creature); break;
                        case CombatAction.CombatHealSelf: AICombatHealSelf.DoCombatHealSelf(bc_Creature); break;
                        case CombatAction.CombatHealOther: AICombatHealOther.DoCombatHealOther(bc_Creature); break;
                        case CombatAction.CombatSpecialAction: AICombatSpecialAction.DoCombatSpecialAction(bc_Creature); break;
                        case CombatAction.CombatEpicAction: AICombatEpicAction.DoCombatEpicAction(bc_Creature); break;
                    }

                    return combatAction;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return combatAction;
        }

        public bool GetGuardAction()
        {
            GuardAction guardAction = GuardAction.None;

            Dictionary<GuardAction, int> DictTemp = new Dictionary<GuardAction, int>();

            if (AIMiscAction.CanDoDetectHidden(bc_Creature)) { DictTemp.Add(GuardAction.DetectHidden, bc_Creature.DictGuardAction[GuardAction.DetectHidden]); }
            if (AIMiscAction.CanDoSpellDispelSummon(bc_Creature)) { DictTemp.Add(GuardAction.SpellDispelSummon, bc_Creature.DictGuardAction[GuardAction.SpellDispelSummon]); }
            if (AIMiscAction.CanDoSpellReveal(bc_Creature)) { DictTemp.Add(GuardAction.SpellReveal, bc_Creature.DictGuardAction[GuardAction.SpellReveal]); }

            int TotalValues = 0;

            //Calculate Total Values
            foreach (KeyValuePair<GuardAction, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine GuardAction                      
            foreach (KeyValuePair<GuardAction, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    guardAction = pair.Key;

                    switch (guardAction)
                    {
                        case GuardAction.None: break;
                        case GuardAction.DetectHidden: break;
                        case GuardAction.SpellDispelSummon: break;
                        case GuardAction.SpellReveal: break;
                    }

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public bool GetWanderAction()
        {
            WanderAction wanderAction = WanderAction.None;

            int TotalValues = 0;

            Dictionary<WanderAction, int> DictTemp = new Dictionary<WanderAction, int>();

            DictTemp.Add(WanderAction.None, bc_Creature.DictWanderAction[WanderAction.None]);

            if (AIWanderHeal.CanDoWanderHealSelfSpellHealSelf(bc_Creature, 100)) { DictTemp.Add(WanderAction.SpellHealSelf100, bc_Creature.DictWanderAction[WanderAction.SpellHealSelf100]); }
            if (AIWanderHeal.CanDoWanderHealSelfPotionHealSelf(bc_Creature, 100)) { DictTemp.Add(WanderAction.PotionHealSelf100, bc_Creature.DictWanderAction[WanderAction.PotionHealSelf100]); }
            if (AIWanderHeal.CanDoWanderHealSelfBandageHealSelf(bc_Creature, 100)) { DictTemp.Add(WanderAction.BandageHealSelf100, bc_Creature.DictWanderAction[WanderAction.BandageHealSelf100]); }
            if (AIWanderHeal.CanDoWanderHealSelfSpellHealSelf(bc_Creature, 50)) { DictTemp.Add(WanderAction.SpellHealSelf50, bc_Creature.DictWanderAction[WanderAction.SpellHealSelf50]); }
            if (AIWanderHeal.CanDoWanderHealSelfPotionHealSelf(bc_Creature, 50)) { DictTemp.Add(WanderAction.PotionHealSelf50, bc_Creature.DictWanderAction[WanderAction.PotionHealSelf50]); }
            if (AIWanderHeal.CanDoWanderHealSelfBandageHealSelf(bc_Creature, 50)) { DictTemp.Add(WanderAction.BandageHealSelf50, bc_Creature.DictWanderAction[WanderAction.BandageHealSelf50]); }

            if (AIWanderHeal.CanDoWanderHealSelfSpellCureSelf(bc_Creature)) { DictTemp.Add(WanderAction.SpellCureSelf, bc_Creature.DictWanderAction[WanderAction.SpellCureSelf]); }
            if (AIWanderHeal.CanDoWanderHealSelfPotionCureSelf(bc_Creature)) { DictTemp.Add(WanderAction.PotionCureSelf, bc_Creature.DictWanderAction[WanderAction.PotionCureSelf]); }
            if (AIWanderHeal.CanDoWanderHealSelfBandageCureSelf(bc_Creature)) { DictTemp.Add(WanderAction.BandageCureSelf, bc_Creature.DictWanderAction[WanderAction.BandageCureSelf]); }

            if (AIWanderHeal.CanDoWanderHealOtherSpellHealOther(bc_Creature, 100)) { DictTemp.Add(WanderAction.SpellHealOther100, bc_Creature.DictWanderAction[WanderAction.SpellHealOther100]); }
            if (AIWanderHeal.CanDoWanderHealOtherBandageHealOther(bc_Creature, 100)) { DictTemp.Add(WanderAction.BandageHealOther100, bc_Creature.DictWanderAction[WanderAction.BandageHealOther100]); }
            if (AIWanderHeal.CanDoWanderHealOtherSpellHealOther(bc_Creature, 50)) { DictTemp.Add(WanderAction.SpellHealOther50, bc_Creature.DictWanderAction[WanderAction.SpellHealOther50]); }
            if (AIWanderHeal.CanDoWanderHealOtherBandageHealOther(bc_Creature, 50)) { DictTemp.Add(WanderAction.BandageHealOther50, bc_Creature.DictWanderAction[WanderAction.BandageHealOther50]); }
            if (AIWanderHeal.CanDoWanderHealOtherSpellCureOther(bc_Creature)) { DictTemp.Add(WanderAction.SpellCureOther, bc_Creature.DictWanderAction[WanderAction.SpellCureOther]); }
            if (AIWanderHeal.CanDoWanderHealOtherBandageCureOther(bc_Creature)) { DictTemp.Add(WanderAction.BandageCureOther, bc_Creature.DictWanderAction[WanderAction.BandageCureOther]); }

            if (AIMiscAction.CanDoDetectHidden(bc_Creature)) { DictTemp.Add(WanderAction.DetectHidden, bc_Creature.DictWanderAction[WanderAction.DetectHidden]); }
            if (AIMiscAction.CanDoSpellReveal(bc_Creature)) { DictTemp.Add(WanderAction.SpellReveal, bc_Creature.DictWanderAction[WanderAction.SpellReveal]); }
            if (AIMiscAction.CanDoStealing(bc_Creature)) { DictTemp.Add(WanderAction.Stealing, bc_Creature.DictWanderAction[WanderAction.Stealing]); }
            if (AIMiscAction.CanDoStealth(bc_Creature)) { DictTemp.Add(WanderAction.Stealth, bc_Creature.DictWanderAction[WanderAction.Stealth]); }
            if (AIMiscAction.CanDoTracking(bc_Creature)) { DictTemp.Add(WanderAction.Tracking, bc_Creature.DictWanderAction[WanderAction.Tracking]); }

            //Calculate Total Values
            foreach (KeyValuePair<WanderAction, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine WanderAction                      
            foreach (KeyValuePair<WanderAction, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    wanderAction = pair.Key;

                    switch (wanderAction)
                    {
                        //Heal Self
                        case WanderAction.SpellHealSelf100: AIHeal.DoSpellHeal(bc_Creature, bc_Creature); break;
                        case WanderAction.PotionHealSelf100: AIHeal.DoPotionHeal(bc_Creature); break;
                        case WanderAction.BandageHealSelf100: AIHeal.DoBandageHeal(bc_Creature, bc_Creature); break;
                        case WanderAction.SpellHealSelf50: AIHeal.DoSpellHeal(bc_Creature, bc_Creature); break;
                        case WanderAction.PotionHealSelf50: AIHeal.DoPotionHeal(bc_Creature); break;
                        case WanderAction.BandageHealSelf50: AIHeal.DoBandageHeal(bc_Creature, bc_Creature); break;
                        case WanderAction.SpellCureSelf: AIHeal.DoSpellCure(bc_Creature, bc_Creature); break;
                        case WanderAction.PotionCureSelf: AIHeal.DoPotionCure(bc_Creature); break;
                        case WanderAction.BandageCureSelf: AIHeal.DoBandageHeal(bc_Creature, bc_Creature); break;

                        //Heal Other
                        case WanderAction.SpellHealOther100: AIHeal.DoSpellHealOther(bc_Creature, 100); break;
                        case WanderAction.BandageHealOther100: AIHeal.DoBandageHealOther(bc_Creature, 100); break;
                        case WanderAction.SpellHealOther50: AIHeal.DoSpellHealOther(bc_Creature, 50); break;
                        case WanderAction.BandageHealOther50: AIHeal.DoBandageHealOther(bc_Creature, 50); break;
                        case WanderAction.SpellCureOther: AIHeal.DoSpellCureOther(bc_Creature); break;
                        case WanderAction.BandageCureOther: AIHeal.DoBandageCureOther(bc_Creature); break;

                        //Misc Actions
                        case WanderAction.DetectHidden: AIMiscAction.DoDetectHidden(bc_Creature); break;
                        case WanderAction.Stealth: AIMiscAction.DoStealth(bc_Creature); break;
                        case WanderAction.SpellReveal: AIMiscAction.DoSpellReveal(bc_Creature); break;
                        case WanderAction.Stealing: AIMiscAction.DoStealing(bc_Creature); break;
                        case WanderAction.Tracking: AIMiscAction.DoTracking(bc_Creature); break;
                    }

                    bc_Creature.NextWanderActionAllowed = DateTime.UtcNow + TimeSpan.FromSeconds(Utility.RandomMinMax(bc_Creature.WanderActionMinDelay, bc_Creature.WanderActionMaxDelay));

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public bool GetWaypointAction()
        {
            //Default WaypointAction
            WaypointAction waypointAction = WaypointAction.None;

            int TotalValues = 0;

            Dictionary<WaypointAction, int> DictTemp = new Dictionary<WaypointAction, int>();

            DictTemp.Add(WaypointAction.None, bc_Creature.DictWaypointAction[WaypointAction.None]);

            if (AIMiscAction.CanDoDetectHidden(bc_Creature)) { DictTemp.Add(WaypointAction.DetectHidden, bc_Creature.DictWaypointAction[WaypointAction.DetectHidden]); }
            if (AIMiscAction.CanDoStealth(bc_Creature)) { DictTemp.Add(WaypointAction.Stealth, bc_Creature.DictWaypointAction[WaypointAction.Stealth]); }

            //Calculate Total Values
            foreach (KeyValuePair<WaypointAction, int> pair in DictTemp)
            {
                TotalValues += pair.Value;
            }

            double ActionCheck = Utility.RandomDouble();
            double CumulativeAmount = 0.0;
            double AdditionalAmount = 0.0;

            //Determine WanderAction                      
            foreach (KeyValuePair<WaypointAction, int> pair in DictTemp)
            {
                AdditionalAmount = (double)pair.Value / (double)TotalValues;

                if (ActionCheck >= CumulativeAmount && ActionCheck < (CumulativeAmount + AdditionalAmount))
                {
                    waypointAction = pair.Key;

                    switch (waypointAction)
                    {
                        case WaypointAction.DetectHidden: AIMiscAction.DoDetectHidden(bc_Creature); break;
                        case WaypointAction.Stealth: AIMiscAction.DoStealth(bc_Creature); break;
                    }

                    return true;
                }

                CumulativeAmount += AdditionalAmount;
            }

            return false;
        }

        public Mobile GetSelfAggressor()
        {
            Mobile combatant = bc_Creature.Combatant;

            bool foundAggressors = false;

            int mobileAggressors = 0;

            List<AggressorInfo> aggressors = bc_Creature.Aggressors;

            mobileAggressors = (int)bc_Creature.Aggressors.Count;

            if (mobileAggressors > 0)
            {
                foundAggressors = true;

                aggressors = bc_Creature.Aggressors;

                for (int i = 0; i < aggressors.Count; ++i)
                {
                    AggressorInfo info = aggressors[i];
                    Mobile attacker = info.Attacker;

                    //Potential Combatant is Valid and Within Perception Range
                    if (attacker != null && !attacker.Deleted && attacker.GetDistanceToSqrt(bc_Creature) <= bc_Creature.RangePerception && !attacker.Hidden)
                    {
                        if (combatant == null)
                            combatant = attacker;

                        else if (attacker.GetDistanceToSqrt(bc_Creature) < combatant.GetDistanceToSqrt(bc_Creature))
                            combatant = attacker;
                    }
                }
            }

            if (foundAggressors)
                return combatant;

            return null;
        }

        public Mobile GetAnyAggressor()
        {
            Mobile controlMaster = bc_Creature.ControlMaster;
            Mobile combatant = bc_Creature.Combatant;

            //Check Controller's Aggresors
            bool validControlMaster = false;

            //If Mobile Has Valid ControlMaster
            if (bc_Creature.ControlMaster != null && !bc_Creature.ControlMaster.Deleted && bc_Creature.ControlMaster.Map == bc_Creature.Map && bc_Creature.ControlMaster.Alive && !bc_Creature.IsDeadBondedPet)
                validControlMaster = true;

            //Controller Exists: Check Their Aggressosr
            if (validControlMaster)
            {
                //Check ControlMaster's Aggressor
                List<AggressorInfo> controllerAggressors = bc_Creature.ControlMaster.Aggressors;

                if (controllerAggressors.Count > 0)
                {
                    for (int i = 0; i < controllerAggressors.Count; ++i)
                    {
                        AggressorInfo info = controllerAggressors[i];

                        Mobile attacker = info.Attacker;
                        Mobile defender = info.Defender;
                        Mobile target;

                        //Determine Which Mobile in Aggressor List is Target (and not our Creature)
                        if (attacker == bc_Creature.ControlMaster)
                            target = defender;

                        else
                            target = attacker;

                        //Potential Combatant is Valid and Within Perception Range
                        if (bc_Creature.CanSee(target) && !target.Hidden && !target.Deleted && bc_Creature.GetDistanceToSqrt(target) <= bc_Creature.RangePerception)
                        {
                            //Avoid Friendly Targeting
                            bool targetFriendly = false;

                            if (validControlMaster)
                            {
                                if (target == bc_Creature.ControlMaster)
                                    targetFriendly = true;

                                BaseCreature bc_Target = target as BaseCreature;

                                if (bc_Target != null)
                                {
                                    //Target is Follower Of Your ControlMaster
                                    if (bc_Target.ControlMaster == bc_Creature.ControlMaster)
                                        targetFriendly = true;
                                }
                            }

                            //Target Isn't Friendly
                            if (targetFriendly == false)
                            {
                                //Have No Current Combatant So Picking This One
                                if (combatant == null)
                                    combatant = target;

                                //Creature is In Guard Mode: Picks Closest Target to Controller
                                if (bc_Creature.ControlOrder == OrderType.Guard)
                                {
                                    double controllerToTarget = bc_Creature.ControlMaster.GetDistanceToSqrt(target);
                                    double controllerToComabatant = bc_Creature.ControlMaster.GetDistanceToSqrt(combatant);

                                    if (controllerToTarget < controllerToComabatant)
                                        combatant = target;
                                }

                                //Creature is In Any Other Mode: Picks Target Closest To Self
                                else
                                {
                                    if (bc_Creature.GetDistanceToSqrt(target) < combatant.GetDistanceToSqrt(bc_Creature))
                                        combatant = target;
                                }
                            }
                        }
                    }
                }
            }

            //Check Own Aggressors: Ignore If In Guard Mode And Already Found Something Aggressive to ControlMaster
            if (!(validControlMaster && bc_Creature.ControlOrder == OrderType.Guard && combatant != null))
            {
                List<AggressorInfo> ownAggressors = bc_Creature.Aggressors;

                if (ownAggressors.Count > 0)
                {
                    for (int i = 0; i < ownAggressors.Count; ++i)
                    {
                        AggressorInfo info = ownAggressors[i];

                        Mobile attacker = info.Attacker;
                        Mobile defender = info.Defender;
                        Mobile target;

                        //Determine Which Mobile in Aggressor List is Target (and not our Creature)
                        if (attacker == bc_Creature)
                            target = defender;

                        else
                            target = attacker;

                        //Potential Combatant is Valid and Within Perception Range
                        if (bc_Creature.CanSee(target) && !target.Deleted && bc_Creature.GetDistanceToSqrt(target) <= bc_Creature.RangePerception)
                        {
                            //Avoid Friendly Targeting
                            bool targetFriendly = false;

                            if (validControlMaster)
                            {
                                BaseCreature bc_Target = target as BaseCreature;

                                if (target == bc_Creature.ControlMaster)
                                    targetFriendly = true;

                                if (bc_Target != null)
                                {
                                    if (bc_Target.ControlMaster == bc_Creature.ControlMaster)
                                        targetFriendly = true;
                                }
                            }

                            //Target Isn't Friendly
                            if (targetFriendly == false)
                            {
                                //Have No Current Combatant So Picking This One
                                if (combatant == null)
                                    combatant = target;

                                //This Attacker is Closer Than My Current Combatant
                                else if (bc_Creature.GetDistanceToSqrt(target) < combatant.GetDistanceToSqrt(bc_Creature))
                                    combatant = target;
                            }
                        }
                    }
                }
            }

            //Check Controller's Other Followers for Aggressors: Ignore If In Guard Mode And Already Found Something Aggressive to ControlMaster
            if (validControlMaster && bc_Creature.ControlOrder == OrderType.Guard && combatant != null)
            {
            }

            else if (validControlMaster)
            {
                PlayerMobile pm = bc_Creature.ControlMaster as PlayerMobile;

                if (pm != null)
                {
                    //Check Other Followers of Creature's ControlMaster
                    foreach (Mobile follower in pm.AllFollowers)
                    {
                        bool foundAggressor = false;

                        BaseCreature bc_Follower = follower as BaseCreature;

                        if (bc_Follower != null)
                        {
                            //Check ControlMaster's Aggressor
                            List<AggressorInfo> followerAggressors = bc_Follower.Aggressors;

                            if (followerAggressors.Count > 0)
                            {
                                for (int i = 0; i < followerAggressors.Count; ++i)
                                {
                                    AggressorInfo info = followerAggressors[i];

                                    Mobile attacker = info.Attacker;
                                    Mobile defender = info.Defender;
                                    Mobile target;

                                    //Determine Which Mobile in Aggressor List is Target (and not our Creature)
                                    if (attacker == bc_Follower)
                                        target = defender;

                                    else
                                        target = attacker;

                                    //Potential Combatant is Valid and Within Perception Range
                                    if (bc_Creature.CanSee(target) && !target.Deleted && bc_Creature.GetDistanceToSqrt(target) <= bc_Creature.RangePerception)
                                    {
                                        //Avoid Friendly Targeting
                                        bool targetFriendly = false;

                                        if (validControlMaster)
                                        {
                                            BaseCreature bc_Target = target as BaseCreature;

                                            //Target is Control Master
                                            if (target == bc_Creature.ControlMaster)
                                                targetFriendly = true;

                                            if (bc_Target != null)
                                            {
                                                //Target Has The Same Control Master As Us 
                                                if (bc_Target.ControlMaster == bc_Creature.ControlMaster)
                                                    targetFriendly = true;
                                            }
                                        }

                                        //Target Isn't Friendly
                                        if (targetFriendly == false)
                                        {
                                            //Have No Current Combatant So Picking This One
                                            if (combatant == null)
                                                combatant = target;

                                            //This Attacker is Closer Than My Current Combatant
                                            else if (bc_Creature.GetDistanceToSqrt(target) < combatant.GetDistanceToSqrt(bc_Creature))
                                                combatant = target;
                                        }
                                    }
                                }
                            }
                        }

                        if (foundAggressor)
                            break;
                    }
                }
            }

            return combatant;
        }

        public bool FleeCheck()
        {
            //Immobile Creatures Cannot Flee
            if (bc_Creature.DisallowAllMoves)
                return false;

            //Will Not Flee if Pacified
            if (bc_Creature.BardPacified)
                return false;

            //Will Not Flee if Provoked
            if (bc_Creature.BardProvoked)
                return false;

            //Creature is Too Powerful to Flee
            if (bc_Creature.Difficulty >= 15 || bc_Creature.IsParagon || bc_Creature.Rare || bc_Creature.IsBoss() || bc_Creature.IsMiniBoss() || bc_Creature.IsLoHBoss() || bc_Creature.IsEventBoss())
                return false;

            //Minions Won't Flee
            if (bc_Creature.IsBossMinion() || bc_Creature.IsMiniBossMinion() || bc_Creature.IsLoHMinion() || bc_Creature.IsEventMinion())
                return false;

            //Mandatory Flee Duration in Place
            if (bc_Creature.EndFleeTime > DateTime.UtcNow)
                return true;

            //Already Fleeing: Check if Continue Fleeing or Rally
            if (Action == ActionType.Flee)
            {
                List<AggressorInfo> aggressors = bc_Creature.Aggressors;

                int validAggressors = 0;
                double ClosestCombatantRange = 1000;
                bool validCombatant = false;

                //Current Combatant Still Valid
                if (bc_Creature.Combatant != null && !bc_Creature.Combatant.Deleted && bc_Creature.Combatant.Map == bc_Creature.Map && bc_Creature.Combatant.Alive && bc_Creature.GetDistanceToSqrt(bc_Creature.Combatant) <= bc_Creature.RangePerception)
                {
                    validCombatant = true;
                }

                //Has Aggressors
                else if (aggressors.Count > 0)
                {
                    for (int i = 0; i < aggressors.Count; ++i)
                    {
                        AggressorInfo info = aggressors[i];
                        Mobile attacker = info.Attacker;
                        Mobile defender = info.Defender;

                        //Potential Combatant is Valid and Within Perception Range
                        if (attacker != null && !attacker.Deleted && attacker.Map == bc_Creature.Map && attacker.Alive && bc_Creature.GetDistanceToSqrt(attacker) <= bc_Creature.RangePerception)
                        {
                            validAggressors++;

                            //Set Closest Aggressor as Combatant (Will Flee Away From This Mobile)
                            if (bc_Creature.GetDistanceToSqrt(attacker) < ClosestCombatantRange)
                            {
                                bc_Creature.Combatant = attacker;
                                ClosestCombatantRange = bc_Creature.GetDistanceToSqrt(attacker);

                                validCombatant = true;
                            }
                        }

                        //Potential Combatant is Valid and Within Perception Range
                        if (defender != null && !defender.Deleted && defender.Map == bc_Creature.Map && defender.Alive && bc_Creature.GetDistanceToSqrt(defender) <= bc_Creature.RangePerception)
                        {
                            validAggressors++;

                            //Set Closest Aggressor as Combatant (Will Flee Away From This Mobile)
                            if (bc_Creature.GetDistanceToSqrt(defender) < ClosestCombatantRange)
                            {
                                bc_Creature.Combatant = defender;
                                ClosestCombatantRange = bc_Creature.GetDistanceToSqrt(defender);

                                validCombatant = true;
                            }
                        }
                    }
                }

                //Valid Combatant to Flee From
                if (validCombatant)
                {
                    //Has Regained Enough Health Since Last Fled                    
                    if (HealthRallyCheck())
                    {
                        GuardMode();
                        return false;
                    }

                    else
                        return true;
                }

                //No Valid Combatant to Flee From
                {
                    GuardMode();
                    return false;
                }
            }

            //Check To See If Should Flee
            else
            {
                double MobileHealth = (double)bc_Creature.Hits / (double)bc_Creature.HitsMax;
                int HighestFleePriority = 0;

                if (MobileHealth <= .05)
                {
                    if (bc_Creature.DictCombatFlee[CombatFlee.Flee5] > HighestFleePriority)
                    {
                        HealthFledAt = .0;
                        HighestFleePriority = bc_Creature.DictCombatFlee[CombatFlee.Flee5];
                    }
                }

                else if (MobileHealth <= .10)
                {
                    if (bc_Creature.DictCombatFlee[CombatFlee.Flee10] > HighestFleePriority)
                    {
                        HealthFledAt = .10;
                        HighestFleePriority = bc_Creature.DictCombatFlee[CombatFlee.Flee10];
                    }
                }

                else if (MobileHealth <= .25)
                {
                    if (bc_Creature.DictCombatFlee[CombatFlee.Flee25] > HighestFleePriority)
                    {
                        HealthFledAt = .25;
                        HighestFleePriority = bc_Creature.DictCombatFlee[CombatFlee.Flee25];
                    }
                }

                else if (MobileHealth <= .50)
                {
                    if (bc_Creature.DictCombatFlee[CombatFlee.Flee50] > HighestFleePriority)
                    {
                        HealthFledAt = .50;
                        HighestFleePriority = bc_Creature.DictCombatFlee[CombatFlee.Flee50];
                    }
                }

                double FleeChance = (double)HighestFleePriority / 10;
                double FleeCheck = Utility.RandomDouble();

                if (FleeChance >= FleeCheck)
                {
                    bc_Creature.BeginFlee(TimeSpan.FromSeconds(10));
                    return true;
                }
            }

            return false;
        }

        public bool HealthRallyCheck()
        {
            double MobileHealth = (double)bc_Creature.Hits / (double)bc_Creature.HitsMax;

            if (MobileHealth >= .25 && HealthFledAt == .10)
                return true;

            if (MobileHealth >= .50 && HealthFledAt == .25)
                return true;

            if (MobileHealth >= .75 && HealthFledAt == .50)
                return true;

            if (MobileHealth == 1)
                return true;

            return false;
        }

        public virtual bool AcquireFocusMob(bool CurrentlyInCombat)
        {
            if (bc_Creature.Deleted)
                return false;

            //Blessed Things or Vendors Don't Acquire Targets
            if (bc_Creature.Blessed || bc_Creature is BaseVendor)
            {
                //Wandering Healer Exception
                if (!(bc_Creature is WanderingHealer || bc_Creature is EvilWanderingHealer))
                    return false;
            }

            Mobile mobile = bc_Creature as Mobile;

            if (mobile is BaseGuard)
                return false;

            //Mobile is Controlled
            else if (bc_Creature.Controlled)
            {
                //Mobile Cannot Get Target From Its Master
                if (bc_Creature.ControlTarget == null || bc_Creature.ControlTarget.Deleted || bc_Creature.ControlTarget.Hidden || !bc_Creature.ControlTarget.Alive || bc_Creature.ControlTarget.IsDeadBondedPet || !bc_Creature.InRange(bc_Creature.ControlTarget, bc_Creature.RangePerception * 2))
                {
                    if (bc_Creature.ControlTarget != null && bc_Creature.ControlTarget != bc_Creature.ControlMaster)
                        bc_Creature.ControlTarget = null;

                    bc_Creature.FocusMob = null;

                    return false;
                }

                //Mobile's Target is its Master's Target
                else
                {
                    bc_Creature.FocusMob = bc_Creature.ControlTarget;
                    return (bc_Creature.FocusMob != null);
                }
            }

            //Mobile Has a Continuous Target: Currently Only Khadun Revenant
            if (bc_Creature.ConstantFocus != null)
            {
                bc_Creature.FocusMob = bc_Creature.ConstantFocus;
                return (bc_Creature.FocusMob != null);
            }

            //Create Needs Fully-Randomized Target
            if (bc_Creature.AcquireRandomizedTarget)
            {
                List<Mobile> m_MobilesToEvaluate = new List<Mobile>();

                IPooledEnumerable eable = bc_Creature.Map.GetMobilesInRange(bc_Creature.Location, bc_Creature.AcquireRandomizedTargetSearchRange);

                foreach (Mobile randomMobile in eable)
                {
                    if (randomMobile.Deleted) continue;
                    if (randomMobile.Map != bc_Creature.Map) continue;
                    if (randomMobile == bc_Creature) continue;
                    if (randomMobile.AccessLevel > AccessLevel.Player) continue;
                    if (!bc_Creature.CanBeHarmful(randomMobile)) continue;
                    if (!bc_Creature.CanSee(randomMobile)) continue;

                    double randomMobileValue = DetermineMobileTargetValue(randomMobile, null, CurrentlyInCombat);

                    if (randomMobileValue > 0)
                        m_MobilesToEvaluate.Add(randomMobile);
                }

                eable.Free();

                if (m_MobilesToEvaluate.Count > 0)
                {
                    bc_Creature.FocusMob = m_MobilesToEvaluate[Utility.RandomMinMax(0, m_MobilesToEvaluate.Count - 1)];
                    return true;
                }
            }

            //----------------

            Mobile firstMobileTarget = null;
            Mobile bestMobileTarget = null;

            double bestMobileTargetValue = 0.0;
            double CheckMobileTargetValue = 0.0;

            int validTargetCount = 0;

            Map map = bc_Creature.Map;

            bool foundFearfulTarget = false;

            if (map != null)
            {
                List<Mobile> m_MobilesToEvaluate = new List<Mobile>();

                if (bc_Creature.Combatant != null)
                    m_MobilesToEvaluate.Add(bc_Creature.Combatant);

                IPooledEnumerable eable = map.GetMobilesInRange(bc_Creature.Location, bc_Creature.RangePerception);

                foreach (Mobile target in eable)
                {
                    m_MobilesToEvaluate.Add(target);
                }

                eable.Free();

                foreach (Mobile target in m_MobilesToEvaluate)
                {
                    if (target.Deleted) continue;
                    if (target.Map != bc_Creature.Map) continue;
                    if (target == bc_Creature) continue;
                    if (target.AccessLevel > AccessLevel.Player) continue;
                    if (!bc_Creature.CanBeHarmful(target)) continue;
                    if (!bc_Creature.CanSee(target)) continue;

                    if (bc_Creature.Combatant == target && bc_Creature.GetDistanceToSqrt(target.Location) > bc_Creature.RangePerception * 2) continue;
                    if (bc_Creature.Combatant != target && bc_Creature.GetDistanceToSqrt(target.Location) > bc_Creature.RangePerception) continue;

                    CheckMobileTargetValue = DetermineMobileTargetValue(target, bestMobileTarget, CurrentlyInCombat);

                    //Can Be a Valid Target
                    if (CheckMobileTargetValue > 0)
                    {
                        //First Valid Target: Store Because It Can't Be Properly Compared (being compared to null for BonusWeights)
                        if (validTargetCount == 0)
                            firstMobileTarget = target;

                        validTargetCount++;

                        //If Target Has Better Targeting Value Than Current Best, It Becomes Best
                        if (CheckMobileTargetValue > bestMobileTargetValue)
                        {
                            bestMobileTargetValue = CheckMobileTargetValue;
                            bestMobileTarget = target;
                        }
                    }
                }

                //Check First Target Against Best Target: First Target Wasn't Properly Compared
                if (firstMobileTarget != null && firstMobileTarget != bestMobileTarget)
                {
                    CheckMobileTargetValue = DetermineMobileTargetValue(firstMobileTarget, bestMobileTarget, CurrentlyInCombat);

                    if (CheckMobileTargetValue > bestMobileTargetValue)
                        bestMobileTarget = firstMobileTarget;
                }

                //Fear-Inducing Item Found Player in Search Range
                if (foundFearfulTarget)
                {
                    //Whether Target Normally Flees Or Not
                    if (bc_Creature.DictCombatFlee[CombatFlee.Flee5] > 0 || bc_Creature.DictCombatFlee[CombatFlee.Flee10] > 0 || bc_Creature.DictCombatFlee[CombatFlee.Flee25] > 0 || bc_Creature.DictCombatFlee[CombatFlee.Flee50] > 0)
                    {
                        bc_Creature.BeginFlee(TimeSpan.FromSeconds(10));
                        return false;
                    }
                }

                if (bestMobileTarget != null)
                {
                    bc_Creature.FocusMob = bestMobileTarget;
                    return true;
                }
            }

            return false;
        }

        public double DetermineMobileTargetValue(Mobile target, Mobile currentBestTarget, bool CurrentlyInCombat)
        {
            int MobValue = 0;
            int BestTargetValue = 0;
            int BonusWeightValue = 0;

            if (target == null || bc_Creature == null)
                return 0;

            bool IsFollower = false;

            BaseCreature bc_Target = target as BaseCreature;
            PlayerMobile pm_Target = target as PlayerMobile;

            BaseWeapon targetWeapon = target.Weapon as BaseWeapon;

            if (bc_Target == null && pm_Target == null)
                return 0;

            if (bc_Target != null)
            {
                if (bc_Target.Controlled && bc_Target.ControlMaster != null)
                    IsFollower = true;
            }

            //Don't Attack Escortable Creatures
            if (IsFollower && bc_Target.ControlMaster is PlayerMobile && bc_Target is BaseEscortable)
                return 0;

            //Blade Spirits & Energy Vortex Ignore Each Other
            if ((this is BladeSpirits || this is EnergyVortex) && (target is BladeSpirits || target is EnergyVortex))
                return 0;

            //If AcquireFocusMob Occuring During Combat
            if (CurrentlyInCombat)
            {
                //If Target is Mobile's Current Combatant
                if (!(bc_Creature.Combatant != null && bc_Creature.Combatant == target))
                {
                    if (!bc_Creature.InLOS(target))
                        return 0;
                }
            }

            //Looking For Targets Outside of Combat
            else
            {
                //Target Isn't In Line of Sight
                if (!bc_Creature.InLOS(target))
                    return 0;
            }

            //Determine Targeting Value of Potential Target

            //None: Doesn't Want To Acquire Targets or Fightback
            if (bc_Creature.DictCombatTargeting[CombatTargeting.None] > 0)
                return 0;

            //PlayerGood
            if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerGood] > 0)
            {
                if (target.Player && !target.Criminal && Notoriety.Compute(bc_Creature, target) == Notoriety.Innocent)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerGood] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerGood];
                }

                else if (IsFollower)
                {
                    if (bc_Target.ControlMaster.Player && Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.Innocent)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerGood] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerGood];
                    }
                }
            }

            //PlayerCriminal or Target is Controlled by Player Criminal  
            if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerCriminal] > 0)
            {
                if (target.Player && (Notoriety.Compute(bc_Creature, target) == Notoriety.Criminal || Notoriety.Compute(bc_Creature, target) == Notoriety.Murderer))
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerCriminal] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerCriminal];
                }

                else if (IsFollower)
                {
                    if (bc_Target.ControlMaster.Player && (Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.Criminal || Notoriety.Compute(bc_Target.ControlMaster, target) == Notoriety.Murderer))
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerCriminal] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerCriminal];
                    }
                }
            }

            //PlayerAny or Target is Controlled by PlayerAny
            if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] > 0)
            {
                if (target.Player)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny];
                }

                else if (target is BladeSpirits || target is EnergyVortex)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny];
                }

                else if (IsFollower)
                {
                    if (bc_Target.ControlMaster.Player)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.PlayerAny];
                    }
                }
            }

            //Targets SuperPredators
            if (bc_Creature.DictCombatTargeting[CombatTargeting.SuperPredator] > 0)
            {
                if (bc_Target != null)
                {
                    if (bc_Target.SuperPredator == true && !bc_Target.Controlled)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.SuperPredator] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.SuperPredator];
                    }
                }

                //Targets Predators
                if (bc_Target != null)
                {
                    if (bc_Target.Predator == true && !bc_Target.Controlled && bc_Creature.Hunger == 0)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.Predator] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Predator];
                    }
                }
            }

            //Targets Prey    
            if (bc_Creature.DictCombatTargeting[CombatTargeting.Prey] > 0)
            {
                if (bc_Target != null)
                {
                    if (bc_Target.Prey == true && !bc_Target.Controlled && bc_Creature.Hunger == 0)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.Prey] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Prey];
                    }
                }
            }

            //Good
            if (bc_Creature.DictCombatTargeting[CombatTargeting.Good] > 0)
            {
                if (Notoriety.Compute(bc_Creature, target) == Notoriety.Innocent)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.Good] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Good];
                }

                else if (IsFollower)
                {
                    if (Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.Innocent)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.Good] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Good];
                    }
                }
            }

            //Neutral  
            if (bc_Creature.DictCombatTargeting[CombatTargeting.Neutral] > 0)
            {
                if (Notoriety.Compute(bc_Creature, target) == Notoriety.Criminal || Notoriety.Compute(bc_Creature, target) == Notoriety.CanBeAttacked)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.Neutral] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Neutral];
                }

                else if (IsFollower)
                {
                    if (Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.Criminal || Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.CanBeAttacked)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.Neutral] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Neutral];
                    }
                }
            }

            //Evil
            if (bc_Creature.DictCombatTargeting[CombatTargeting.Evil] > 0)
            {
                if (Notoriety.Compute(bc_Creature, target) == Notoriety.Murderer || Notoriety.Compute(bc_Creature, target) == Notoriety.Criminal)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.Evil] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Evil];
                }

                else if (IsFollower)
                {
                    if (Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.Murderer || Notoriety.Compute(bc_Creature, bc_Target.ControlMaster) == Notoriety.Criminal)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.Evil] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Evil];
                    }
                }
            }

            //Any Target
            if (bc_Creature.DictCombatTargeting[CombatTargeting.Any] > BestTargetValue)
                BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Any];

            //Aggressor
            if (bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor] > 0)
            {
                bool bAggressor = false;

                for (int a = 0; a < bc_Creature.Aggressors.Count; a++)
                {
                    if (bc_Creature.Aggressors[a].Attacker == target)
                    {
                        bAggressor = true;
                        break;
                    }
                }

                //This Creature is Controlled/Tamed
                if (bc_Creature.Controlled && bc_Creature.ControlMaster != null)
                {
                    PlayerMobile pm = bc_Creature.ControlMaster as PlayerMobile;

                    //Controlled by Player (AllFollowers currently Player-only Property)
                    if (pm != null)
                    {
                        //Check Other Followers of Creature's ControlMaster
                        foreach (Mobile follower in pm.AllFollowers)
                        {
                            bool foundAggressor = false;
                            BaseCreature bc_Follower = follower as BaseCreature;

                            if (bc_Follower != null)
                            {
                                //Check Each Aggressor of Creature to See If Target is One of Them
                                for (int a = 0; a < bc_Follower.Aggressors.Count; a++)
                                {
                                    if (bc_Creature.Aggressors[a].Attacker == target)
                                    {
                                        bAggressor = true;
                                        foundAggressor = true;
                                        break;
                                    }
                                }
                            }

                            if (foundAggressor)
                                break;
                        }
                    }
                }

                //If Target Was Aggressor
                if (bAggressor)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor];
                }

                //Mobile is a Follower (Commanded)
                else if (IsFollower)
                {
                    bAggressor = false;

                    for (int a = 0; !bAggressor && a < bc_Creature.Aggressors.Count; ++a)
                    {
                        bAggressor = (bc_Creature.Aggressors[a].Attacker == bc_Target.ControlMaster);
                    }

                    if (bAggressor)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.Aggressor];
                    }
                }

                //Not Aggressor
                else
                {
                    //If Is Teammate, Ignore It
                    //if (AITeamList.CheckTeam(bc_Creature, target))
                        //return 0;

                    //Ignore Player Kin Paint Allies
                    //else if (AIKinTeamList.CheckKinTeam(bc_Creature, target))
                        //return 0;
                }
            }

            #region UOACZ

            //Ignore Sentry
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZIgnoreHumanSentry] > 0)
            {
                if (target is UOACZBaseHuman)
                {
                    UOACZBaseHuman bc_Human = target as UOACZBaseHuman;

                    if (bc_Human.Sentry)
                        return 0;
                }
            }

            //UOACZ Human
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZHuman] > 0)
            {
                if (target is UOACZBaseHuman)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZHuman] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZHuman];
                }
            }

            //UOACZ Human Player
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZHumanPlayer] > 0)
            {
                if (pm_Target != null)
                {
                    if (pm_Target.IsUOACZHuman)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZHumanPlayer] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZHumanPlayer];
                    }
                }
            }

            //UOACZ Evil Human Player
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZEvilHumanPlayer] > 0)
            {
                if (pm_Target != null)
                {
                    if (pm_Target.IsUOACZHuman)
                    {
                        if (pm_Target.m_UOACZAccountEntry.HumanProfile.HonorPoints <= UOACZSystem.HonorAggressionThreshold)
                        {
                            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZEvilHumanPlayer] > BestTargetValue)
                                BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZEvilHumanPlayer];
                        }
                    }
                }
            }

            //UOACZ Undead
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZUndead] > 0)
            {
                if (target is UOACZBaseUndead)
                {
                    if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZUndead] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZUndead];
                }
            }

            //UOACZ Undead Player
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer] > 0)
            {
                if (pm_Target != null)
                {
                    if (pm_Target.IsUOACZUndead)
                    {
                        if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer] > BestTargetValue)
                            BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZUndeadPlayer];
                    }
                }
            }

            //UOACZ Wildlife
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZWildlife] > 0)
            {
                if (target is UOACZBaseWildlife)
                {
                    UOACZBaseWildlife wildlife = target as UOACZBaseWildlife;

                    if (!wildlife.Corrupted && bc_Creature.DictCombatTargeting[CombatTargeting.UOACZWildlife] > BestTargetValue)
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZWildlife];
                }
            }

            //UOACZ Evil Wildlife
            if (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZEvilWildlife] > 0)
            {
                if (target is UOACZBaseWildlife)
                {
                    UOACZBaseWildlife wildlife = target as UOACZBaseWildlife;

                    if (wildlife.Corrupted && (bc_Creature.DictCombatTargeting[CombatTargeting.UOACZEvilWildlife] > BestTargetValue))
                        BestTargetValue = bc_Creature.DictCombatTargeting[CombatTargeting.UOACZEvilWildlife];
                }
            }

            #endregion

            //Check Weights            
            if (currentBestTarget != null)
            {
                //Current Combatant 
                if (bc_Creature.Combatant != null)
                {
                    if (bc_Creature.Combatant == target)
                        BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.CurrentCombatant];
                }

                //Closest                
                if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Closest] > 0 && bc_Creature.RangePerception > 0)
                {
                    double distance = bc_Creature.GetDistanceToSqrt(target);
                    double ratio = 1 - (distance / bc_Creature.RangePerception);
                    double weightValue = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Closest];

                    if (ratio > 1)
                        ratio = 1;

                    if (ratio < 0)
                        ratio = 0;

                    double closestValue = weightValue * ratio;

                    BonusWeightValue += (int)closestValue;
                }

                //Player             
                if (pm_Target != null)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Player];

                //Creature
                if (bc_Target != null && !(bc_Target is BladeSpirits || bc_Target is EnergyVortex))
                {
                    //Controlled
                    if (bc_Target.Controlled && bc_Target.ControlMaster != null)
                    {
                        //Target is Controlled and Wants to Aggro This Creature: Use Target's Aggroing Weight Value

                        //Target is in Attack Order and It's Current Combatant is This Creature
                        if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.AttackOrder] > 0)
                        {
                            if (bc_Target.ControlOrder == OrderType.Attack && bc_Target.Combatant == bc_Creature)
                                BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.AttackOrder];
                        }

                        //Target is in Guard Order
                        if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.GuardOrder] > 0)
                        {
                            if (bc_Target.ControlOrder == OrderType.Guard)
                                BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.GuardOrder];
                        }

                        //Target is in Patrol Order
                        if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.PatrolOrder] > 0)
                        {
                            if (bc_Target.ControlOrder == OrderType.Patrol)
                                BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.PatrolOrder];
                        }
                    }

                    //Tamed
                    if (bc_Target.Tameable && bc_Target.Controlled && bc_Target.ControlMaster is PlayerMobile)
                        BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Tamed];
                }

                //Highest Hit Points
                if (target.Hits > currentBestTarget.Hits)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.HighestHitPoints];

                //Lowest Hit Points
                if (target.Hits < currentBestTarget.Hits)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.LowestHitPoints];

                //Higher Armor
                if (target.ArmorRating > currentBestTarget.ArmorRating)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.HighestArmor];

                //Lower Armor
                if (target.ArmorRating < currentBestTarget.ArmorRating)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.LowestArmor];

                //Ranged
                if (targetWeapon != null)
                {
                    if (targetWeapon is BaseRanged)
                        BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Ranged];
                }

                //SpellCaster
                if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Spellcaster] > 0)
                {
                    double SpellCasterBonusToDistribute = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Spellcaster];
                    double SpellCasterBonus = 0.0;

                    SpellCasterBonusToDistribute /= 4;

                    //Target Has Higher Magery Skill
                    if (target.Skills[SkillName.Magery].Value > currentBestTarget.Skills[SkillName.Magery].Value)
                        SpellCasterBonus += (2 * SpellCasterBonusToDistribute);

                    //Target Has More EvalInt
                    if (target.Skills[SkillName.EvalInt].Value > currentBestTarget.Skills[SkillName.EvalInt].Value)
                        SpellCasterBonus += SpellCasterBonusToDistribute;

                    //Target Has More Mana Remaining
                    if ((target.ManaMax - target.Mana) > (currentBestTarget.ManaMax - currentBestTarget.Mana))
                        SpellCasterBonus += SpellCasterBonusToDistribute;

                    BonusWeightValue += (int)(Math.Round(SpellCasterBonus));
                }

                //Summoned
                if (IsFollower && bc_Target != null)
                {
                    if (bc_Target.Summoned)
                        BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Summoned];
                }

                //Poisoner
                if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Poisoner] > 0)
                {
                    double PoisonerBonusToDistribute = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.Poisoner];
                    double PoisonerBonus = 0.0;

                    PoisonerBonusToDistribute /= 2;

                    //Target Has Poisoned Weapon Equipped
                    if (targetWeapon != null)
                    {
                        if (targetWeapon.PoisonCharges > 0)
                            PoisonerBonus += PoisonerBonusToDistribute;
                    }

                    //Target Has More Poisoning Skill
                    if (target.Skills[SkillName.Poisoning].Value > currentBestTarget.Skills[SkillName.Poisoning].Value)
                        PoisonerBonus += PoisonerBonusToDistribute;

                    BonusWeightValue += (int)(Math.Round(PoisonerBonus));

                    //Weak Against Poison
                    if (bc_Target != null)
                    {
                        if (bc_Creature.HitPoison != null && bc_Target.PoisonImmune != null)
                        {
                            if (bc_Creature.HitPoison.Level > bc_Target.PoisonImmune.Level)
                                BonusWeightValue += (int)(Math.Round(PoisonerBonus));
                        }
                    }

                    BonusWeightValue += (int)(Math.Round(PoisonerBonus));
                }

                //Most Combatants
                if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants] > 0)
                {
                    double SwarmBonusToDistribute = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.MostCombatants];
                    double SwarmBonus = 0.0;

                    //Target Has More Aggressors
                    if (target.Aggressors.Count > currentBestTarget.Aggressors.Count)
                        SwarmBonus += SwarmBonusToDistribute;

                    BonusWeightValue += (int)(Math.Round(SwarmBonus));
                }

                //Least Combatants
                if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants] > 0)
                {
                    double SoloBonusToDistribute = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.LeastCombatants];
                    double SoloBonus = 0.0;

                    //Target Has Less Aggressors
                    if (target.Aggressors.Count < currentBestTarget.Aggressors.Count)
                        SoloBonus += SoloBonusToDistribute;

                    BonusWeightValue += (int)(Math.Round(SoloBonus));
                }

                //Hardest or Easiest To Hit
                if (bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.HardestToHit] > 0 || bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit] > 0)
                {
                    double HardestToHitBonusToDistribute = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.HardestToHit];
                    double HardestToHitBonus = 0.0;

                    double EasiestToHitBonusToDistribute = (double)bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.EasiestToHit];
                    double EasiestToHitBonus = 0.0;

                    BaseWeapon currentBestTargetWeapon = currentBestTarget.Weapon as BaseWeapon;

                    if (currentBestTargetWeapon != null)
                    {
                        Skill currentBestTargetSkill = currentBestTarget.Skills[currentBestTargetWeapon.Skill];
                        double currentBestTargetAtkValue = currentBestTargetWeapon.GetAttackSkillValue(currentBestTarget, null);

                        Skill targetSkill = target.Skills[targetWeapon.Skill];
                        double targetAtkValue = currentBestTargetWeapon.GetAttackSkillValue(currentBestTarget, null);

                        if (targetAtkValue > currentBestTargetAtkValue)
                            HardestToHitBonus += HardestToHitBonusToDistribute;

                        BonusWeightValue += (int)(Math.Round(HardestToHitBonus));

                        if (targetAtkValue < currentBestTargetAtkValue)
                            EasiestToHitBonus += EasiestToHitBonusToDistribute;

                        BonusWeightValue += (int)(Math.Round(EasiestToHitBonus));
                    }
                }

                //Target Has More Resist
                if (target.Skills[SkillName.MagicResist].Value > currentBestTarget.Skills[SkillName.MagicResist].Value)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.HighestResist];

                //Target Has Less Resist
                if (target.Skills[SkillName.MagicResist].Value < currentBestTarget.Skills[SkillName.MagicResist].Value)
                    BonusWeightValue += bc_Creature.DictCombatTargetingWeight[CombatTargetingWeight.LowestResist];

                //Bonus Aggro Generated From Target
                if (target.AggroBonusExpiration > DateTime.UtcNow)
                {
                    bool ignoreBonusAggro = false;

                    if (bc_Target != null)
                    {
                        if (bc_Target.BoatOccupied != null)
                            ignoreBonusAggro = true;
                    }

                    if (pm_Target != null)
                    {
                        if (pm_Target.BoatOccupied != null)
                            ignoreBonusAggro = true;
                    }

                    if (!ignoreBonusAggro)
                        BonusWeightValue += target.AggroBonus;
                }
            }

            MobValue += BestTargetValue;

            if (MobValue > 0)
                MobValue += BonusWeightValue;

            return MobValue;
        }

        //Movement
        protected PathFollower m_Path;
        private DateTime m_NextMove = DateTime.UtcNow;
        private static Queue m_Obstacles = new Queue();

        public DateTime NextMove
        {
            get { return m_NextMove; }
            set { m_NextMove = value; }
        }

        public virtual void WalkRandom(int iChanceToNotMove, int iChanceToDir, int iSteps)
        {
            if (bc_Creature.Deleted || bc_Creature.DisallowAllMoves)
                return;

            for (int i = 0; i < iSteps; i++)
            {
                if (Utility.Random(8 * iChanceToNotMove) <= 8)
                {
                    int iRndMove = Utility.Random(0, 8 + (9 * iChanceToDir));

                    switch (iRndMove)
                    {
                        case 0: DoMove(Direction.Up); break;
                        case 1: DoMove(Direction.North); break;
                        case 2: DoMove(Direction.Left); break;
                        case 3: DoMove(Direction.West); break;
                        case 5: DoMove(Direction.Down); break;
                        case 6: DoMove(Direction.South); break;
                        case 7: DoMove(Direction.Right); break;
                        case 8: DoMove(Direction.East); break;
                        default: DoMove(bc_Creature.Direction); break;
                    }
                }
            }
        }

        public virtual bool WalkToLocation(Point3D location, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (bc_Creature.Deleted || bc_Creature.DisallowAllMoves)
                return false;

            if (bc_Creature.Hidden)
                bRun = false;

            if (bc_Creature != null)
            {
                for (int i = 0; i < iSteps; i++)
                {
                    // Get the curent distance
                    int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(location);

                    if (iCurrDist < iWantDistMin || iCurrDist > iWantDistMax)
                    {
                        bool needCloser = (iCurrDist > iWantDistMax);
                        bool needFurther = !needCloser;

                        if (needCloser && m_Path != null && m_Path.Goal == (IPoint3D)location)
                        {
                            if (m_Path.Follow(bRun, 1))
                                m_Path = null;
                        }

                        else
                        {
                            Direction dirTo;

                            dirTo = bc_Creature.GetDirectionTo(location);

                            // Add the run flag
                            if (bRun)
                                dirTo = dirTo | Direction.Running;

                            if (!DoMove(dirTo, true) && needCloser)
                            {
                                m_Path = new PathFollower(bc_Creature, location);
                                m_Path.Mover = new MoveMethod(DoMoveImpl);

                                if (m_Path.Follow(bRun, 1))
                                    m_Path = null;
                            }

                            else
                                m_Path = null;
                        }
                    }

                    else
                        return true;
                }

                // Get the curent distance
                int iNewDist = (int)bc_Creature.GetDistanceToSqrt(location);

                if (iNewDist >= iWantDistMin && iNewDist <= iWantDistMax)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public virtual bool WalkToMobile(Mobile m, int iSteps, bool bRun, int iWantDistMin, int iWantDistMax)
        {
            if (bc_Creature.Deleted || bc_Creature.DisallowAllMoves)
                return false;

            if (bc_Creature.Hidden)
                bRun = false;

            if (m != null)
            {
                for (int i = 0; i < iSteps; i++)
                {
                    // Get the curent distance
                    int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(m);

                    if (iCurrDist < iWantDistMin || iCurrDist > iWantDistMax)
                    {
                        bool needCloser = (iCurrDist > iWantDistMax);
                        bool needFurther = !needCloser;

                        if (needCloser && m_Path != null && m_Path.Goal == m)
                        {
                            if (m_Path.Follow(bRun, 1))
                                m_Path = null;
                        }

                        else
                        {
                            Direction dirTo;

                            if (iCurrDist > iWantDistMax)
                                dirTo = bc_Creature.GetDirectionTo(m);
                            else
                                dirTo = m.GetDirectionTo(bc_Creature);

                            // Add the run flag
                            if (bRun)
                                dirTo = dirTo | Direction.Running;

                            if (!DoMove(dirTo, true) && needCloser)
                            {
                                m_Path = new PathFollower(bc_Creature, m);
                                m_Path.Mover = new MoveMethod(DoMoveImpl);

                                if (m_Path.Follow(bRun, 1))
                                    m_Path = null;
                            }

                            else
                                m_Path = null;
                        }
                    }

                    else
                        return true;
                }

                // Get the curent distance
                int iNewDist = (int)bc_Creature.GetDistanceToSqrt(m);

                if (iNewDist >= iWantDistMin && iNewDist <= iWantDistMax)
                    return true;
                else
                    return false;
            }

            return false;
        }

        public virtual bool CheckMove()
        {
            return (DateTime.UtcNow >= NextMove);
        }

        public virtual bool DoMove(Direction d)
        {
            return DoMove(d, false);
        }

        public virtual bool DoMove(Direction d, bool badStateOk)
        {
            MoveResult res = DoMoveImpl(d);

            return (res == MoveResult.Success || res == MoveResult.SuccessAutoTurn || (badStateOk && res == MoveResult.BadState));
        }

        //Elbows - Add Obstacles property for access in overridden vitrual function
        public static Queue Obstacles
        {
            get { return m_Obstacles; }
        }

        public virtual MoveResult DoMoveImpl(Direction d)
        {
            if (bc_Creature.Deleted || bc_Creature.Frozen || bc_Creature.Paralyzed || (bc_Creature.Spell != null && bc_Creature.Spell.IsCasting) || bc_Creature.DisallowAllMoves)
                return MoveResult.BadState;

            else if (!CheckMove())
                return MoveResult.BadState;

            // This makes them always move one step, never any direction changes
            bc_Creature.Direction = d;

            TimeSpan delay = TimeSpan.FromSeconds(bc_Creature.CurrentSpeed);
            NextMove += delay;

            bc_Creature.Pushing = false;

            MoveImpl.IgnoreMovableImpassables = (bc_Creature.CanMoveOverObstacles && !bc_Creature.CanDestroyObstacles);

            if (bc_Creature.AlwaysRun)
                d = d | Direction.Running;

            if ((bc_Creature.Direction & Direction.Mask) != (d & Direction.Mask))
            {
                bool v = bc_Creature.Move(d);

                MoveImpl.IgnoreMovableImpassables = false;
                return (v ? MoveResult.Success : MoveResult.Blocked);
            }

            else if (!bc_Creature.Move(d))
            {
                bool wasPushing = bc_Creature.Pushing;

                bool blocked = true;

                bool canOpenDoors = bc_Creature.CanOpenDoors;
                bool canDestroyObstacles = bc_Creature.CanDestroyObstacles;

                if (canOpenDoors || canDestroyObstacles)
                {
                    bc_Creature.DebugSay("My movement was blocked, I will try to clear some obstacles.");

                    Map map = bc_Creature.Map;

                    if (map != null)
                    {
                        int x = bc_Creature.X, y = bc_Creature.Y;
                        Movement.Movement.Offset(d, ref x, ref y);

                        int destroyables = 0;

                        IPooledEnumerable eable = map.GetItemsInRange(new Point3D(x, y, bc_Creature.Location.Z), 1);

                        foreach (Item item in eable)
                        {
                            if (canOpenDoors && item is BaseDoor && (item.Z + item.ItemData.Height) > bc_Creature.Z && (bc_Creature.Z + 16) > item.Z)
                            {
                                if (item.X != x || item.Y != y)
                                    continue;

                                BaseDoor door = (BaseDoor)item;

                                if (!door.Locked || !door.UseLocks())
                                    m_Obstacles.Enqueue(door);

                                if (!canDestroyObstacles)
                                    break;
                            }

                            else if (!item.IsLockedDown)
                                m_Obstacles.Enqueue(item);

                            else if (canDestroyObstacles && item.Movable && item.ItemData.Impassable && (item.Z + item.ItemData.Height) > bc_Creature.Z && (bc_Creature.Z + 16) > item.Z)
                            {
                                if (!bc_Creature.InRange(item.GetWorldLocation(), 1))
                                    continue;

                                m_Obstacles.Enqueue(item);
                                ++destroyables;
                            }

                            else if ((canDestroyObstacles && item.Movable) && (item is FootStool || item is Candelabra))
                            {
                                if (!bc_Creature.InRange(item.GetWorldLocation(), 1))
                                    continue;

                                m_Obstacles.Enqueue(item);
                                ++destroyables;
                            }
                        }

                        eable.Free();

                        if (destroyables > 0)
                            Effects.PlaySound(new Point3D(x, y, bc_Creature.Z), bc_Creature.Map, 0x3B3);

                        if (m_Obstacles.Count > 0)
                            blocked = false; // retry movement

                        while (m_Obstacles.Count > 0)
                        {
                            Item item = (Item)m_Obstacles.Dequeue();

                            if (item is BaseDoor)
                            {
                                bc_Creature.DebugSay("Little do they expect, I've learned how to open doors. Didn't they read the script??");
                                bc_Creature.DebugSay("*twist*");

                                ((BaseDoor)item).Use(bc_Creature);
                            }

                            else
                            {
                                bc_Creature.DebugSay("Ugabooga. I'm so big and tough I can destroy it: {0}", item.GetType().Name);

                                if (item is Container)
                                {
                                    Container cont = (Container)item;

                                    for (int i = 0; i < cont.Items.Count; ++i)
                                    {
                                        Item check = cont.Items[i];

                                        if (check.Movable && check.ItemData.Impassable && (item.Z + check.ItemData.Height) > bc_Creature.Z)
                                            m_Obstacles.Enqueue(check);
                                    }

                                    cont.Destroy();
                                }

                                else
                                    item.Delete();
                            }
                        }

                        if (!blocked)
                            blocked = !bc_Creature.Move(d);
                    }
                }

                if (blocked)
                {
                    int offset = (Utility.RandomDouble() >= 0.6 ? 1 : -1);

                    for (int i = 0; i < 2; ++i)
                    {
                        bc_Creature.TurnInternal(offset);

                        if (bc_Creature.Move(bc_Creature.Direction))
                        {
                            MoveImpl.IgnoreMovableImpassables = false;
                            return MoveResult.SuccessAutoTurn;
                        }
                    }

                    MoveImpl.IgnoreMovableImpassables = false;
                    return (wasPushing ? MoveResult.BadState : MoveResult.Blocked);
                }

                else
                {
                    MoveImpl.IgnoreMovableImpassables = false;
                    return MoveResult.Success;
                }
            }

            MoveImpl.IgnoreMovableImpassables = false;
            return MoveResult.Success;
        }

        public virtual void WalkRandomAtPoint(Point3D location, int iChanceToNotMove, int iChanceToDir, int iSteps)
        {
            if (bc_Creature.Deleted || bc_Creature.DisallowAllMoves)
                return;

            for (int i = 0; i < iSteps; i++)
            {
                int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(location);

                if (iCurrDist < bc_Creature.RangeHome * 0.5)
                    WalkRandom(iChanceToNotMove, iChanceToDir, 1);

                else if (iCurrDist > bc_Creature.RangeHome)
                    DoMove(bc_Creature.GetDirectionTo(location));

                else
                {
                    if (Utility.Random(10) > 5)
                        DoMove(bc_Creature.GetDirectionTo(location));
                    else
                        WalkRandom(iChanceToNotMove, iChanceToDir, 1);
                }
            }
        }

        public virtual void WalkRandomInHome(int iChanceToNotMove, int iChanceToDir, int iSteps)
        {
            if (bc_Creature.Deleted || bc_Creature.DisallowAllMoves)
                return;

            if (bc_Creature.Home == Point3D.Zero)
            {
                if (bc_Creature.Spawner is SpawnEntry)
                {
                    Region region = ((SpawnEntry)bc_Creature.Spawner).Region;

                    if (bc_Creature.Region.AcceptsSpawnsFrom(region))
                    {
                        bc_Creature.WalkRegion = region;
                        WalkRandom(iChanceToNotMove, iChanceToDir, iSteps);
                        bc_Creature.WalkRegion = null;
                    }

                    else
                    {
                        if (region.GoLocation != Point3D.Zero && Utility.Random(10) > 5)
                            DoMove(bc_Creature.GetDirectionTo(region.GoLocation));
                        else
                            WalkRandom(iChanceToNotMove, iChanceToDir, 1);
                    }
                }

                else
                    WalkRandom(iChanceToNotMove, iChanceToDir, iSteps);
            }

            else
            {
                for (int i = 0; i < iSteps; i++)
                {
                    if (bc_Creature.RangeHome != 0)
                    {
                        int iCurrDist = (int)bc_Creature.GetDistanceToSqrt(bc_Creature.Home);

                        if (iCurrDist < bc_Creature.RangeHome * 2 / 3)
                            WalkRandom(iChanceToNotMove, iChanceToDir, 1);

                        else if (iCurrDist > bc_Creature.RangeHome)
                            DoMove(bc_Creature.GetDirectionTo(bc_Creature.Home));
                        else
                        {
                            if (Utility.Random(10) > 5)
                                DoMove(bc_Creature.GetDirectionTo(bc_Creature.Home));
                            else
                                WalkRandom(iChanceToNotMove, iChanceToDir, 1);
                        }
                    }

                    else
                    {
                        if (bc_Creature.Location != bc_Creature.Home)
                            DoMove(bc_Creature.GetDirectionTo(bc_Creature.Home));
                    }
                }
            }
        }

        public virtual void OnTeleported()
        {
            if (m_Path != null)
            {
                bc_Creature.DebugSay("Teleported; repathing");
                m_Path.ForceRepath();
            }
        }

        public virtual bool MoveTo(Mobile m, bool run, int range)
        {
            if (bc_Creature.Deleted || bc_Creature.DisallowAllMoves || m == null || m.Deleted)
                return false;

            if (bc_Creature.InRange(m, range))
            {
                m_Path = null;
                return true;
            }

            if (m_Path != null && m_Path.Goal == m)
            {
                if (m_Path.Follow(run, 1))
                {
                    m_Path = null;
                    return true;
                }
            }

            else if (!DoMove(bc_Creature.GetDirectionTo(m), true))
            {
                m_Path = new PathFollower(bc_Creature, m);
                m_Path.Mover = new MoveMethod(DoMoveImpl);

                if (m_Path.Follow(run, 1))
                {
                    m_Path = null;
                    return true;
                }
            }

            else
            {
                m_Path = null;
                return true;
            }

            return false;
        }

        public virtual void BeginPickTarget(Mobile from, OrderType order)
        {
            if (bc_Creature.Deleted || !bc_Creature.Controlled || !from.InRange(bc_Creature, 14) || from.Map != bc_Creature.Map)
                return;

            bool isOwner = (from == bc_Creature.ControlMaster);

            if (!isOwner)
                return;

            if (from.Target == null)
            {
                if (order == OrderType.Transfer)
                    from.SendLocalizedMessage(502038); // Click on the person to transfer ownership to.                

                else if (order == OrderType.Friend)
                    from.SendLocalizedMessage(502020); // Click on the player whom you wish to make a co-owner                

                else if (order == OrderType.Unfriend)
                    from.SendLocalizedMessage(1070948); // Click on the player whom you wish to remove as a co-owner.                

                if (order == OrderType.Patrol)
                {
                    from.BeginTarget(15, true, TargetFlags.None, new TargetCallback(PickPatrolTarget));
                    from.SendMessage("Click on the location where you wish your follower to patrol.");
                }

                else if (order == OrderType.Fetch)
                {
                    from.BeginTarget(15, true, TargetFlags.None, new TargetCallback(PickFetchTarget));
                    from.SendMessage("Click on the item you wish your followers to fetch.");
                }

                else
                    from.Target = new AIControlMobileTarget(this, order);
            }

            else if (from.Target is AIControlMobileTarget)
            {
                AIControlMobileTarget t = (AIControlMobileTarget)from.Target;

                if (t.Order == order)
                    t.AddAI(this);
            }
        }

        public virtual void EndPickTarget(Mobile from, Mobile target, OrderType order)
        {
            if (bc_Creature.Deleted || !bc_Creature.Controlled || !from.InRange(bc_Creature, 14) || from.Map != bc_Creature.Map || !from.Alive)
                return;

            bool isOwner = (from == bc_Creature.ControlMaster);

            if (!isOwner)
                return;

            if (order == OrderType.Attack)
            {
                if (target == bc_Creature)
                    return;

                if (target is BaseCreature && ((BaseCreature)target).IsScaryToPets && bc_Creature.IsScaredOfScaryThings)
                {
                    bc_Creature.SayTo(from, "Your pet refuses to attack this creature!");
                    return;
                }

                if (target is Factions.BaseFactionGuard)
                {
                    bc_Creature.SayTo(from, "Your pet refuses to attack the guard.");
                    return;
                }

                bool flagTamerToTarget = true;

                if (target is BaseCreature)
                {
                    BaseCreature bc_Target = target as BaseCreature;

                    if (!(bc_Target.ControlMaster is PlayerMobile) || bc_Target is BladeSpirits || bc_Target is EnergyVortex)
                        flagTamerToTarget = false;
                }

                if (flagTamerToTarget)
                    from.DoHarmful(target);
            }

            bc_Creature.ControlTarget = target;
            bc_Creature.ControlOrder = order;
        }

        private void PickFetchTarget(Mobile from, object obj)
        {
            Item objTarget = obj as Item;

            if (objTarget == null)
                return;

            IPoint3D p = objTarget.Location as IPoint3D;

            if (p == null)
                return;

            bc_Creature.ControlOrder = OrderType.Fetch;
            bc_Creature.ControlObject = objTarget;
        }

        public virtual bool CheckBardPacified()
        {
            //Currently Pacified and Timer Not Expired
            if (bc_Creature.BardPacified && bc_Creature.BardEndTime > DateTime.UtcNow)
            {
                bc_Creature.Combatant = null;
                bc_Creature.Warmode = false;

                return true;
            }

            //Was Pacified By Timer Has Now Expired
            else if (bc_Creature.BardPacified)
            {
                m_WalkRandomOutsideHome = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.WalkRandomOutsideHomeLimit);
                m_WalkTowardsHome = DateTime.UtcNow + TimeSpan.FromSeconds(bc_Creature.WalkRandomOutsideHomeLimit + bc_Creature.WalkTowardsHomeLimit);

                bc_Creature.BardPacified = false;

                bc_Creature.BardProvoked = false;
                bc_Creature.BardMaster = null;
                bc_Creature.BardTarget = null;

                bc_Creature.Combatant = null;
                bc_Creature.Warmode = false;

                bc_Creature.PostPeacemakingTeleportDelay = DateTime.UtcNow + TimeSpan.FromSeconds(5);
            }

            return false;
        }

        public virtual bool CheckBardProvoked()
        {
            bool Provoked = false;
            bool ProvokeTargetValid = false;

            //Currently Provoked
            if (bc_Creature.BardProvoked)
            {
                Provoked = true;
                ProvokeTargetValid = true;

                //Provocation Timer Expired
                if (bc_Creature.BardEndTime < DateTime.UtcNow)
                    Provoked = false;

                if (bc_Creature.BardMaster == null)
                    Provoked = false;
                else
                {
                    if (bc_Creature.BardMaster.Deleted)
                        Provoked = false;

                    if (!bc_Creature.BardMaster.Alive)
                        Provoked = false;

                    if (bc_Creature.BardMaster.Map != bc_Creature.Map)
                        Provoked = false;

                    if (bc_Creature.GetDistanceToSqrt(bc_Creature.BardMaster) > (bc_Creature.RangePerception * 2))
                        Provoked = false;
                }

                if (bc_Creature.BardTarget == null)
                {
                    Provoked = false;
                    ProvokeTargetValid = false;
                }

                else
                {
                    if (bc_Creature.BardTarget.Deleted)
                    {
                        Provoked = false;
                        ProvokeTargetValid = false;
                    }

                    if (!bc_Creature.BardTarget.Alive)
                    {
                        Provoked = false;
                        ProvokeTargetValid = false;
                    }

                    if (bc_Creature.Map != bc_Creature.BardTarget.Map)
                    {
                        Provoked = false;
                        ProvokeTargetValid = false;
                    }

                    if (!bc_Creature.CanSee(bc_Creature.BardTarget))
                        Provoked = false;

                    if (bc_Creature.GetDistanceToSqrt(bc_Creature.BardTarget) > (bc_Creature.RangePerception * 2))
                        Provoked = false;
                }

                if (Provoked && ProvokeTargetValid)
                {
                    bc_Creature.Combatant = bc_Creature.BardTarget;
                    return true;
                }

                //Was Provoked But No Longer
                else
                {
                    BaseCreature creatureBardTarget = bc_Creature.BardTarget as BaseCreature;

                    bc_Creature.BardProvoked = false;
                    bc_Creature.BardMaster = null;
                    bc_Creature.BardTarget = null;
                    bc_Creature.Combatant = null;

                    if (bc_Creature.Controlled && bc_Creature.ControlMaster != null)
                    {
                        if (bc_Creature.ControlOrder != OrderType.Stop)
                        {
                            bc_Creature.ControlOrder = OrderType.Stop;
                            bc_Creature.AIObject.DoOrderStop();
                        }
                    }

                    int aggressorsCount = bc_Creature.Aggressors.Count;
                    int position = 0;

                    for (int i = 0; i < aggressorsCount; ++i)
                    {
                        AggressorInfo info = bc_Creature.Aggressors[position];

                        Mobile attacker = info.Attacker;
                        Mobile defender = info.Defender;

                        if (creatureBardTarget != null)
                        {
                            if (attacker == creatureBardTarget || defender == creatureBardTarget)
                            {
                                bc_Creature.Aggressors.RemoveAt(position);
                                position--;
                            }

                            position++;
                        }
                    }

                    if (ProvokeTargetValid)
                    {
                        if (creatureBardTarget != null)
                        {
                            creatureBardTarget.BardProvoked = false;
                            creatureBardTarget.BardMaster = null;
                            creatureBardTarget.BardTarget = null;
                            creatureBardTarget.Combatant = null;

                            if (creatureBardTarget.Controlled && creatureBardTarget.ControlMaster != null)
                            {
                                if (creatureBardTarget.ControlOrder != OrderType.Stop)
                                {
                                    creatureBardTarget.ControlOrder = OrderType.Stop;
                                    creatureBardTarget.AIObject.DoOrderStop();
                                }
                            }

                            aggressorsCount = creatureBardTarget.Aggressors.Count;
                            position = 0;

                            for (int i = 0; i < aggressorsCount; ++i)
                            {
                                AggressorInfo info = creatureBardTarget.Aggressors[position];

                                Mobile attacker = info.Attacker;
                                Mobile defender = info.Defender;

                                if (attacker == bc_Creature || defender == bc_Creature)
                                {
                                    creatureBardTarget.Aggressors.RemoveAt(position);
                                    position--;
                                }

                                position++;
                            }

                            if (creatureBardTarget.AIObject != null)
                                creatureBardTarget.AIObject.GuardMode();
                        }
                    }

                    GuardMode();

                    return false;
                }
            }

            return false;
        }

        public virtual bool CheckHerding()
        {
            IPoint2D target = bc_Creature.TargetLocation;

            if (target == null)
                return false; // Creature is not being herded

            double distance = bc_Creature.GetDistanceToSqrt(target);

            if (distance < 1 || distance > 15)
            {
                if (distance < 1 && target.X == 1076 && target.Y == 450 && (bc_Creature is HordeMinionFamiliar))
                {
                    PlayerMobile pm = bc_Creature.ControlMaster as PlayerMobile;
                }

                bc_Creature.TargetLocation = null;
                return false; // At the target or too far away
            }

            DoMove(bc_Creature.GetDirectionTo(target));

            return true;
        }

        public virtual void OnCurrentSpeedChanged()
        {
            m_Timer.Stop();
            m_Timer.Delay = TimeSpan.FromSeconds(Utility.RandomDouble());
            m_Timer.Interval = TimeSpan.FromSeconds(Math.Max(0.0, bc_Creature.CurrentSpeed));
            m_Timer.Start();
        }

        private class InternalEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private BaseCreature bc_Creature;
            private BaseAI m_AI;
            private OrderType m_Order;

            public InternalEntry(Mobile from, int number, int range, BaseCreature creature, BaseAI ai, OrderType order)
                : base(number, range)
            {
                m_From = from;
                bc_Creature = creature;
                m_AI = ai;
                m_Order = order;
            }

            public override void OnClick()
            {
                if (m_From == null) return;
                if (m_From.Deleted) return;
                if (bc_Creature == null) return;
                if (bc_Creature.Deleted) return;
                if (!bc_Creature.Controlled) return;
                if (bc_Creature.ControlMaster != m_From) return;
                if (bc_Creature.AIObject == null) return;

                switch (m_Order)
                {
                    case OrderType.Attack:
                        bc_Creature.AIObject.ReceiveOrderAttack(m_From);
                        break;

                    case OrderType.Guard:
                        bc_Creature.AIObject.ReceiveOrderGuard(m_From);
                        break;

                    case OrderType.Patrol:
                        bc_Creature.AIObject.ReceiveOrderPatrol(m_From);
                        break;

                    case OrderType.Follow:
                        bc_Creature.AIObject.ReceiveOrderFollow(m_From);
                        break;

                    case OrderType.Come:
                        bc_Creature.AIObject.ReceiveOrderCome(m_From);
                        break;

                    case OrderType.Stay:
                        bc_Creature.AIObject.ReceiveOrderStay(m_From);
                        break;

                    case OrderType.Stop:
                        bc_Creature.AIObject.ReceiveOrderStop(m_From);
                        break;

                    case OrderType.Fetch:
                        bc_Creature.AIObject.ReceiveOrderFetch(m_From);
                        break;

                    case OrderType.Transfer:
                        bc_Creature.AIObject.ReceiveOrderTransfer(m_From);
                        break;

                    case OrderType.Release:
                        bc_Creature.AIObject.ReceiveOrderRelease(m_From);
                        break;
                }
            }
        }

        public virtual void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive && bc_Creature.Controlled && from.InRange(bc_Creature, 14))
            {
                if (bc_Creature.Region is UOACZRegion)
                {
                    list.Add(new InternalEntry(from, 6107, 14, bc_Creature, this, OrderType.Guard));  // Command: Guard                    
                    list.Add(new InternalEntry(from, 6108, 14, bc_Creature, this, OrderType.Follow)); // Command: Follow
                    list.Add(new InternalEntry(from, 6114, 14, bc_Creature, this, OrderType.Stay));   // Command: Stay
                    list.Add(new InternalEntry(from, 6112, 14, bc_Creature, this, OrderType.Stop));   // Command: Stop

                    list.Add(new InternalEntry(from, 6118, 14, bc_Creature, this, OrderType.Release)); // Release 
                }

                else if (from == bc_Creature.ControlMaster)
                {
                    list.Add(new InternalEntry(from, 6111, 14, bc_Creature, this, OrderType.Attack)); // Command: Kill
                    list.Add(new InternalEntry(from, 6107, 14, bc_Creature, this, OrderType.Guard));  // Command: Guard                    
                    list.Add(new InternalEntry(from, 6108, 14, bc_Creature, this, OrderType.Follow)); // Command: Follow
                    list.Add(new InternalEntry(from, 6114, 14, bc_Creature, this, OrderType.Stay));   // Command: Stay
                    list.Add(new InternalEntry(from, 6112, 14, bc_Creature, this, OrderType.Stop));   // Command: Stop

                    list.Add(new InternalEntry(from, 6118, 14, bc_Creature, this, OrderType.Release)); // Release 

                    if (!bc_Creature.Summoned)
                        list.Add(new InternalEntry(from, 6113, 14, bc_Creature, this, OrderType.Transfer)); // Transfer                   
                }
            }
        }

        private static SkillName[] m_KeywordTable = new SkillName[]
		{
			SkillName.Parry,
			SkillName.Healing,
			SkillName.Hiding,
			SkillName.Stealing,
			SkillName.Alchemy,
			SkillName.AnimalLore,
			SkillName.ItemID,
			SkillName.ArmsLore,
			SkillName.Begging,
			SkillName.Blacksmith,
			SkillName.Peacemaking,
			SkillName.Camping,
			SkillName.Carpentry,
			SkillName.Cartography,
			SkillName.Cooking,
			SkillName.DetectHidden,
			SkillName.Discordance,
			SkillName.EvalInt,
			SkillName.Fishing,
			SkillName.Provocation,
			SkillName.Lockpicking,
			SkillName.Magery,
			SkillName.MagicResist,
			SkillName.Tactics,
			SkillName.Snooping,
			SkillName.RemoveTrap,
			SkillName.Musicianship,
			SkillName.Poisoning,
			SkillName.Archery,
			SkillName.SpiritSpeak,
			SkillName.Tailoring,
			SkillName.AnimalTaming,
			SkillName.TasteID,
			SkillName.Tinkering,
			SkillName.Veterinary,
			SkillName.Forensics,
			SkillName.Herding,
			SkillName.Tracking,
			SkillName.Stealth,
			SkillName.Inscribe,
			SkillName.Swords,
			SkillName.Macing,
			SkillName.Fencing,
			SkillName.Wrestling,
			SkillName.Lumberjacking,
			SkillName.Mining,
			SkillName.Meditation
		};

        public virtual bool WasNamed(string speech)
        {
            string name = bc_Creature.Name;

            return (name != null && Insensitive.StartsWith(speech, name));
        }

        public virtual bool HandlesOnSpeech(Mobile from)
        {
            return true;
        }

        public virtual void OnSpeech(SpeechEventArgs e)
        {
            bool isNonHuman = false;

            if (bc_Creature != null && (bc_Creature.Body.IsAnimal || bc_Creature.Body.IsMonster || bc_Creature.Body.IsSea))
                isNonHuman = true;

            int[] keywords = e.Keywords;
            string speech = e.Speech;

            string customSpeech = e.Speech.Trim().ToLower();

            #region Special Instance Speech

            if (e.Mobile.Alive && e.Mobile.InRange(bc_Creature.Location, 3))
            {
                if (bc_Creature is BaseVendor && !e.Handled)
                {
                    if (customSpeech.IndexOf("trick or treat") > -1)
                    {
                        e.Handled = true;

                        HalloweenPersistance.TrickOrTreat(bc_Creature, e.Mobile);
                        return;
                    }

                    else if (customSpeech.IndexOf("trick or treat") > -1 && WasNamed(e.Speech))
                    {
                        e.Handled = true;

                        HalloweenPersistance.TrickOrTreat(bc_Creature, e.Mobile);
                        return;
                    }
                }
            }

            #endregion

            if (e.Mobile.Alive && e.Mobile.InRange(bc_Creature.Location, 3) && !isNonHuman)
            {
                if (bc_Creature is BaseVendor && !e.Handled)
                {
                    if (e.HasKeyword(0x14D)) // *vendor sell*
                    {
                        e.Handled = true;

                        ((BaseVendor)bc_Creature).VendorSell(e.Mobile);
                        bc_Creature.FocusMob = e.Mobile;
                    }

                    else if (e.HasKeyword(0x3C)) // *vendor buy*
                    {
                        e.Handled = true;

                        ((BaseVendor)bc_Creature).VendorBuy(e.Mobile);
                        bc_Creature.FocusMob = e.Mobile;
                    }

                    else if (WasNamed(e.Speech))
                    {
                        e.Handled = true;

                        if (e.HasKeyword(0x177)) // *sell*
                            ((BaseVendor)bc_Creature).VendorSell(e.Mobile);

                        else if (e.HasKeyword(0x171)) // *buy*
                            ((BaseVendor)bc_Creature).VendorBuy(e.Mobile);

                        else if (e.HasKeyword(0x9D)) //move
                        {
                            if (bc_Creature.Combatant != null)
                                bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);// I am too busy fighting to deal with thee!                    
                            else
                                WalkRandomInHome(2, 2, 1);
                        }

                        bc_Creature.FocusMob = e.Mobile;
                    }
                }

                else if (e.HasKeyword(0x9D) && WasNamed(e.Speech)) // *move*
                {
                    if (bc_Creature.Combatant != null)
                        bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);// I am too busy fighting to deal with thee!                    

                    else
                    {
                        bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501516); // Excuse me?
                        WalkRandomInHome(2, 2, 1);
                    }
                }

                else if (e.HasKeyword(0x9E) && WasNamed(e.Speech)) // *time*
                {
                    if (bc_Creature.Combatant != null)
                        bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482);// I am too busy fighting to deal with thee!                    

                    else
                    {
                        int generalNumber;
                        string exactTime;

                        Clock.GetTime(bc_Creature, out generalNumber, out exactTime);

                        bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, generalNumber);
                    }
                }

                else if (e.HasKeyword(0x6C) && WasNamed(e.Speech)) // *train
                {
                    if (bc_Creature.Combatant != null)
                        bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482); // I am too busy fighting to deal with thee!
                    else
                    {
                        bool foundSomething = false;

                        Skills ourSkills = bc_Creature.Skills;
                        Skills theirSkills = e.Mobile.Skills;

                        for (int i = 0; i < ourSkills.Length && i < theirSkills.Length; ++i)
                        {
                            Skill skill = ourSkills[i];
                            Skill theirSkill = theirSkills[i];

                            if (skill != null && theirSkill != null && skill.Base >= 60.0 && bc_Creature.CheckTeach(skill.SkillName, e.Mobile))
                            {
                                double toTeach = skill.Base / 3.0;

                                if (toTeach > 42.0)
                                    toTeach = 42.0;

                                if (toTeach > theirSkill.Base)
                                {
                                    int number = 1043059 + i;

                                    if (number > 1043107)
                                        continue;

                                    if (!foundSomething)
                                        bc_Creature.Say(1043058); // I can train the following:

                                    bc_Creature.Say(number);

                                    foundSomething = true;
                                }
                            }
                        }

                        if (!foundSomething)
                            bc_Creature.Say(501505); // Alas, I cannot teach thee anything.
                    }
                }

                else
                {
                    SkillName toTrain = (SkillName)(-1);

                    for (int i = 0; toTrain == (SkillName)(-1) && i < e.Keywords.Length; ++i)
                    {
                        int keyword = e.Keywords[i];

                        if (keyword == 0x154)
                            toTrain = SkillName.Anatomy;
                        else if (keyword >= 0x6D && keyword <= 0x9C)
                        {
                            int index = keyword - 0x6D;

                            if (index >= 0 && index < m_KeywordTable.Length)
                                toTrain = m_KeywordTable[index];
                        }
                    }

                    if (toTrain != (SkillName)(-1) && WasNamed(e.Speech))
                    {
                        if (bc_Creature.Combatant != null)
                            bc_Creature.PublicOverheadMessage(MessageType.Regular, 0x3B2, 501482); // I am too busy fighting to deal with thee!                        

                        else
                        {
                            Skills skills = bc_Creature.Skills;
                            Skill skill = skills[toTrain];

                            if (skill == null || skill.Base < 60.0 || !bc_Creature.CheckTeach(toTrain, e.Mobile))
                                bc_Creature.Say(501507); // 'Tis not something I can teach thee of.
                            else
                                bc_Creature.Teach(toTrain, e.Mobile, 0, false);
                        }
                    }
                }
            }

            //Creature Speech Handling
            if (bc_Creature.Controlled && bc_Creature.Commandable)
            {
                bc_Creature.DebugSay("Listening...");

                bool isOwner = (e.Mobile == bc_Creature.ControlMaster);

                string exceededFollowers = "You have currently exceeded your follower limit and they refuse to obey your command!";

                Mobile mobile = e.Mobile;
                PlayerMobile pm_Mobile = e.Mobile as PlayerMobile;

                if (isOwner)
                {
                    //If somehow player has more followers than allowed, all followers won't behave
                    if (mobile.Followers > mobile.FollowersMax)
                    {
                        mobile.SendMessage(exceededFollowers);
                        return;
                    }

                    //Fetch Command
                    if (customSpeech.IndexOf("all fetch") == -1 && customSpeech.IndexOf("fetch") > -1)
                    {
                        ReceiveOrderFetch(mobile);
                        return;
                    }

                    // First, check the all*
                    for (int i = 0; i < keywords.Length; ++i)
                    {
                        int keyword = keywords[i];

                        switch (keyword)
                        {
                            case 0x164: // all come
                                {
                                    ReceiveOrderCome(mobile);
                                    return;
                                }

                            case 0x165: // all follow
                                {
                                    ReceiveOrderFollow(mobile);
                                    return;
                                }

                            case 0x166: // all guard
                                {
                                    ReceiveOrderGuard(mobile);
                                    return;
                                }

                            case 0x16B: // all guard me
                                {
                                    ReceiveOrderGuard(mobile);
                                    return;
                                }

                            case 0x167: // all stop
                                {
                                    ReceiveOrderStop(mobile);
                                    return;
                                }

                            case 0x168: // all kill
                                {
                                    ReceiveOrderAttack(mobile);
                                    return;
                                }

                            case 0x169: // all attack
                                {
                                    ReceiveOrderAttack(mobile);
                                    return;
                                }

                            case 0x16C: // all follow me
                                {
                                    ReceiveOrderFollow(mobile);
                                    return;
                                }

                            case 0x170: // all stay
                                {
                                    ReceiveOrderStay(mobile);
                                    return;
                                }
                        }
                    }

                    // No all*, so check *command
                    for (int i = 0; i < keywords.Length; ++i)
                    {
                        int keyword = keywords[i];

                        switch (keyword)
                        {
                            case 0x155: // *come                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderCome(mobile);
                                    return;
                                }
                                break;

                            case 0x156: // *drop
                                break;

                            case 0x15A: // *follow                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderFollow(mobile);
                                    return;
                                }
                                break;

                            case 0x15B: // *friend
                                break;

                            case 0x15C: // *guard                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderGuard(mobile);
                                    return;
                                }
                                break;

                            case 0x15D: // *kill                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderAttack(mobile);
                                    return;
                                }
                                break;

                            case 0x15E: // *attack                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderAttack(mobile);
                                    return;
                                }
                                break;

                            case 0x15F: // *patrol                                
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderPatrol(mobile);
                                    return;
                                }
                                break;

                            case 0x161: // *stop                                
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderStop(mobile);
                                    return;
                                }
                                break;

                            case 0x163: // *follow me                                
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderFollow(mobile);
                                    return;
                                }
                                break;

                            case 0x16D: // *release                                
                                if (WasNamed(speech) && customSpeech.IndexOf("all release") == -1)
                                {
                                    ReceiveOrderRelease(mobile);
                                    return;
                                }
                                break;

                            case 0x16E: // *transfer                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderTransfer(mobile);
                                    return;
                                }
                                break;

                            case 0x16F: // *stay                            
                                if (WasNamed(speech))
                                {
                                    ReceiveOrderStay(mobile);
                                    return;
                                }
                                break;
                        }
                    }
                }
            }

            else
            {
                if (e.Mobile.AccessLevel >= AccessLevel.GameMaster)
                {
                    bc_Creature.DebugSay("It's from a GM");

                    if (bc_Creature.FindMyName(e.Speech, true))
                    {
                        string[] str = e.Speech.Split(' ');
                        int i;

                        for (i = 0; i < str.Length; i++)
                        {
                            string word = str[i];

                            if (Insensitive.Equals(word, "obey"))
                            {
                                bc_Creature.SetControlMaster(e.Mobile);

                                if (bc_Creature.Summoned)
                                    bc_Creature.SummonMaster = e.Mobile;

                                return;
                            }
                        }
                    }
                }
            }
        }

        private class TransferItem : Item
        {
            public static bool IsInCombat(BaseCreature creature)
            {
                return (creature != null && (creature.Aggressors.Count > 0 || creature.Aggressed.Count > 0));
            }

            private BaseCreature m_Creature;

            public TransferItem(BaseCreature creature)
                : base(ShrinkTable.Lookup(creature))
            {
                m_Creature = creature;

                Movable = false;

                if (!Core.AOS)
                    Name = creature.Name;
                else if (this.ItemID == ShrinkTable.DefaultItemID || creature.GetType().IsDefined(typeof(FriendlyNameAttribute), false))
                    Name = FriendlyNameAttribute.GetFriendlyNameFor(creature.GetType()).ToString();

                Hue = creature.Hue & 0x0FFF;
            }

            public TransferItem(Serial serial)
                : base(serial)
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

                Delete();
            }

            public override void GetProperties(ObjectPropertyList list)
            {
                base.GetProperties(list);

                list.Add(1041603); // This item represents a pet currently in consideration for trade
                list.Add(1041601, m_Creature.Name); // Pet Name: ~1_val~

                if (m_Creature.ControlMaster != null)
                    list.Add(1041602, m_Creature.ControlMaster.Name); // Owner: ~1_val~
            }

            public override bool AllowSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
            {
                if (!base.AllowSecureTrade(from, to, newOwner, accepted))
                    return false;

                if (Deleted || m_Creature == null || m_Creature.Deleted || m_Creature.ControlMaster != from || !from.CheckAlive() || !to.CheckAlive())
                    return false;

                if (from.Map != m_Creature.Map || !from.InRange(m_Creature, 14))
                    return false;

                if (!accepted)
                    return false;

                if (m_Creature.IsHenchman)
                {
                    if (to.Skills[SkillName.Begging].Value < m_Creature.MinTameSkill || to.Skills[SkillName.Camping].Value < m_Creature.MinTameSkill)
                    {
                        from.SendMessage("That individual refuses to be transferred to them. They do not have enough begging and camping skill to command them.");
                        to.SendMessage("That individual refuses to be transferred to you. You do not have enough begging and camping skill to command them.");

                        return false;
                    }

                    if (from.Skills[SkillName.Begging].Value < m_Creature.MinTameSkill || from.Skills[SkillName.Camping].Value < m_Creature.MinTameSkill)
                    {
                        from.SendMessage("That individual refuses to be transferred to them. You do not have enough begging and camping skill to command them.");
                        to.SendMessage("That individual refuses to be transferred to you. The controller does not have enough begging and camping skill to command them.");

                        return false;
                    }
                }

                else
                {
                    if (to.Skills[SkillName.AnimalTaming].Value < m_Creature.MinTameSkill || to.Skills[SkillName.AnimalLore].Value < m_Creature.MinTameSkill)
                    {
                        from.SendMessage("The pet refuses to be transferred to the new handler. They do not have enough taming skill and animal lore to command the creature.");
                        to.SendMessage("The pet refuses to be transferred to you. You do not have enough taming skill and animal lore to command the creature.");

                        return false;
                    }

                    if (from.Skills[SkillName.AnimalTaming].Value < m_Creature.MinTameSkill || from.Skills[SkillName.AnimalLore].Value < m_Creature.MinTameSkill)
                    {
                        from.SendMessage("The pet refuses to be transferred to the new handler. You do not have enough taming skill and animal lore to command the creature.");
                        to.SendMessage("The pet refuses to be transferred to you. The owner does not have enough taming skill and animal lore to command the creature.");

                        return false;
                    }
                }

                if ((to.Followers + m_Creature.ControlSlots) > to.FollowersMax)
                {
                    to.SendLocalizedMessage(1049607); // You have too many followers to control that creature.
                    return false;
                }

                if (IsInCombat(m_Creature))
                {
                    from.SendMessage("You may not transfer a pet that has recently been in combat.");
                    to.SendMessage("The pet may not be transfered to you because it has recently been in combat.");

                    return false;
                }

                return true;
            }

            public override void OnSecureTrade(Mobile from, Mobile to, Mobile newOwner, bool accepted)
            {
                if (Deleted)
                    return;

                Delete();

                if (m_Creature == null || m_Creature.Deleted || m_Creature.ControlMaster != from || !from.CheckAlive() || !to.CheckAlive())
                    return;

                if (from.Map != m_Creature.Map || !from.InRange(m_Creature, 14))
                    return;

                if (accepted)
                {
                    if (m_Creature.SetControlMaster(to))
                    {
                        if (m_Creature.Summoned)
                            m_Creature.SummonMaster = to;

                        m_Creature.OwnerAbandonTime = DateTime.UtcNow + m_Creature.AbandonDelay;
                        m_Creature.IsBonded = false;

                        m_Creature.PlaySound(m_Creature.GetIdleSound());

                        string args = String.Format("{0}\t{1}\t{2}", from.Name, m_Creature.Name, to.Name);

                        from.SendLocalizedMessage(1043253, args); // You have transferred your pet to ~3_GETTER~.
                        to.SendLocalizedMessage(1043252, args); // ~1_NAME~ has transferred the allegiance of ~2_PET_NAME~ to you.
                    }
                }
            }
        }

        public void DoWaypointIdle(double seconds, WaypointBehavior waypointBehavior)
        {
            WayPoint waypoint = bc_Creature.CurrentWaypoint;
            WayPoint previousWaypoint;

            double distanceToWaypoint = bc_Creature.GetDistanceToSqrt(waypoint.Location);

            //Idle Some Time Then NextWaypoint
            if (waypointBehavior == WaypointBehavior.NextWaypoint)
            {
                if (bc_Creature.WaypointOrder == WayPointOrder.IdleForward || bc_Creature.WaypointOrder == WayPointOrder.IdleReverse)
                {
                    //Idle Duration Expires
                    if (bc_Creature.NextWaypointAction < DateTime.UtcNow)
                    {
                        if (bc_Creature.WaypointOrder == WayPointOrder.IdleForward)
                        {
                            bc_Creature.WaypointOrder = WayPointOrder.Forward;

                            if (waypoint.NextWaypoint != null)
                            {
                                bc_Creature.CurrentWaypoint = waypoint.NextWaypoint;
                                SetWaypointTimeout(bc_Creature);
                            }

                            else
                                SendCreatureTowardsHome();
                        }

                        else if (bc_Creature.WaypointOrder == WayPointOrder.IdleReverse)
                        {
                            //Goto Previous Waypoint
                            previousWaypoint = GetPreviousWaypoint(bc_Creature, waypoint);

                            //If Has a Previous Waypoint
                            if (previousWaypoint != null)
                            {
                                bc_Creature.WaypointOrder = WayPointOrder.Reverse;
                                bc_Creature.CurrentWaypoint = previousWaypoint;

                                SetWaypointTimeout(bc_Creature);
                            }

                            //If Has No Previous Waypoint (Could possibly be at the first waypoint now)
                            else if (waypoint.NextWaypoint != null)
                            {
                                bc_Creature.WaypointOrder = WayPointOrder.Forward;
                                bc_Creature.CurrentWaypoint = waypoint.NextWaypoint;

                                SetWaypointTimeout(bc_Creature);
                            }

                            //No Valid Waypoint
                            else
                                SendCreatureTowardsHome();
                        }
                    }

                    else
                    {
                        if (Utility.RandomDouble() <= .125)
                            bc_Creature.FaceRandomDirection();
                    }
                }

                //Not Yet Waiting: Set Idle
                else
                {
                    bc_Creature.FaceRandomDirection();
                    bc_Creature.NextWaypointAction = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);

                    if (bc_Creature.WaypointOrder == WayPointOrder.Forward)
                        bc_Creature.WaypointOrder = WayPointOrder.IdleForward;

                    if (bc_Creature.WaypointOrder == WayPointOrder.Reverse)
                        bc_Creature.WaypointOrder = WayPointOrder.IdleReverse;
                }
            }

            //Idle Some Time Then Reverse Order of Waypoints
            if (waypointBehavior == WaypointBehavior.ReverseOrder)
            {
                if (bc_Creature.WaypointOrder == WayPointOrder.IdleReverse)
                {
                    //Idle Duration Expires
                    if (bc_Creature.NextWaypointAction < DateTime.UtcNow)
                    {
                        //Goto Previous Waypoint
                        previousWaypoint = GetPreviousWaypoint(bc_Creature, waypoint);

                        if (previousWaypoint != null)
                        {
                            bc_Creature.WaypointOrder = WayPointOrder.Reverse;
                            bc_Creature.CurrentWaypoint = previousWaypoint;

                            SetWaypointTimeout(bc_Creature);
                        }

                        //If No Previous Waypoint Exists Somehow (See if it can keep going)
                        else if (waypoint.NextWaypoint != null)
                        {
                            bc_Creature.WaypointOrder = WayPointOrder.Forward;
                            bc_Creature.CurrentWaypoint = waypoint.NextWaypoint;

                            SetWaypointTimeout(bc_Creature);
                        }

                        else
                            SendCreatureTowardsHome();
                    }

                    else
                    {
                        if (Utility.RandomDouble() <= .125)
                            bc_Creature.FaceRandomDirection();
                    }
                }

                //Not Yet Waiting: Set Idle
                else
                {
                    bc_Creature.FaceRandomDirection();
                    bc_Creature.NextWaypointAction = DateTime.UtcNow + TimeSpan.FromSeconds(seconds);
                    bc_Creature.WaypointOrder = WayPointOrder.IdleReverse;
                }
            }
        }
    }
}
