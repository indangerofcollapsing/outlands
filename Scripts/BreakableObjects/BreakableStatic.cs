using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Achievements;

namespace Server.Custom
{
    public class BreakableStatic : Item
    {
        #region Properties

        public enum DamageStateType
        {
            Normal,
            LightlyDamaged,
            HeavilyDamaged,
            Broken
        }

        private bool m_AllowRepair = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AllowRepair
        {
            get { return m_AllowRepair; }
            set { m_AllowRepair = value; }
        }

        private bool m_RequiresFullRepair = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RequiresFullRepair
        {
            get { return m_RequiresFullRepair; }
            set { m_RequiresFullRepair = value; }
        }

        private int m_RepairSound = 0x23D;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RepairSound
        {
            get { return m_RepairSound; }
            set { m_RepairSound = value; }
        }

        private bool m_PlayerPlaced;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool PlayerPlaced
        {
            get { return m_PlayerPlaced; }
            set { m_PlayerPlaced = value; }
        }

        private Direction m_Facing = Direction.North;
        [CommandProperty(AccessLevel.GameMaster)]
        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
        } 

        private DamageStateType m_DamageState = DamageStateType.Normal;
        [CommandProperty(AccessLevel.GameMaster)]
        public DamageStateType DamageState
        {
            get { return m_DamageState; }
            set
            { 
                m_DamageState = value;
            }
        }

        private int m_InteractionRange = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int InteractionRange
        {
            get { return m_InteractionRange; }
            set { m_InteractionRange = value; }
        }

        private double m_InteractionDelay = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public double InteractionDelay
        {
            get { return m_InteractionDelay; }
            set { m_InteractionDelay = value; }
        }

        private int m_MinInteractDamage = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MinInteractDamage
        {
            get { return m_MinInteractDamage; }
            set { m_MinInteractDamage = value; }
        }

        private int m_MaxInteractDamage = 10;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxInteractDamage
        {
            get { return m_MaxInteractDamage; }
            set { m_MaxInteractDamage = value; }
        }

        private double m_InteractDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double InteractDamageScalar
        {
            get { return m_InteractDamageScalar; }
            set { m_InteractDamageScalar = value; }
        }

        private double m_WeaponDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double WeaponDamageScalar
        {
            get { return m_WeaponDamageScalar; }
            set { m_WeaponDamageScalar = value; }
        }

        private double m_LockpickDamageScalar = 0.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double LockpickDamageScalar
        {
            get { return m_LockpickDamageScalar; }
            set { m_LockpickDamageScalar = value; }
        }        

        private double m_MiningDamageScalar = 0.33;
        [CommandProperty(AccessLevel.GameMaster)]
        public double MiningDamageScalar
        {
            get { return m_MiningDamageScalar; }
            set { m_MiningDamageScalar = value; }
        }

        private double m_LumberjackingDamageScalar = 0.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double LumberjackingDamageScalar
        {
            get { return m_LumberjackingDamageScalar; }
            set { m_LumberjackingDamageScalar = value; }
        }

        private double m_ObjectBreakingDeviceDamageScalar = 1.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public double ObjectBreakingDeviceDamageScalar
        {
            get { return m_ObjectBreakingDeviceDamageScalar; }
            set { m_ObjectBreakingDeviceDamageScalar = value; }
        }

        private int m_HitPoints = 1000;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_HitPoints; }
            set
            {
                m_HitPoints = value;

                if (m_HitPoints > m_MaxHitPoints)
                    m_HitPoints = m_MaxHitPoints;

                if (m_HitPoints <= 0)
                    m_HitPoints = 0;

                UpdateDamagedState();
            }
        }

        private int m_MaxHitPoints = 1000;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHitPoints; }
            set
            { 
                m_MaxHitPoints = value;

                if (m_MaxHitPoints < HitPoints)
                    HitPoints = m_MaxHitPoints;
            }
        }

        private bool m_AddLOSBlocker = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool AddLOSBlocker
        {
            get { return m_AddLOSBlocker; }
            set { m_AddLOSBlocker = value; }
        }

        private int m_HitSound = 0x3B7;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound
        {
            get { return m_HitSound; }
            set { m_HitSound = value; }
        }

        //Normal
        private int m_NormalItemId = 88;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NormalItemId
        {
            get { return m_NormalItemId; }
            set { m_NormalItemId = value; }
        }

        private int m_NormalHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int NormalHue
        {
            get { return m_NormalHue; }
            set { m_NormalHue = value; }
        }
       
        //Lightly Damaged
        private double m_LightlyDamagedPercent = .666;
        [CommandProperty(AccessLevel.GameMaster)]
        public double LightlyDamagedPercent
        {
            get { return m_LightlyDamagedPercent; }
            set { m_LightlyDamagedPercent = value; }
        }

        private int m_LightlyDamagedSound = 0x3B7;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LightlyDamagedSound
        {
            get { return m_LightlyDamagedSound; }
            set { m_LightlyDamagedSound = value; }
        }

        private int m_LightlyDamagedItemId = 95;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LightlyDamagedItemId
        {
            get { return m_LightlyDamagedItemId; }
            set { m_LightlyDamagedItemId = value; }
        }

        private int m_LightlyDamagedHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LightlyDamagedHue
        {
            get { return m_LightlyDamagedHue; }
            set { m_LightlyDamagedHue = value; }
        }

        //Heavily Damaged
        private double m_HeavilyDamagedPercent = .333;
        [CommandProperty(AccessLevel.GameMaster)]
        public double HeavilyDamagedPercent
        {
            get { return m_HeavilyDamagedPercent; }
            set { m_HeavilyDamagedPercent = value; }
        }

        private int m_HeavilyDamagedSound = 0x3B7;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HeavilyDamagedSound
        {
            get { return m_HeavilyDamagedSound; }
            set { m_HeavilyDamagedSound = value; }
        }

        private int m_HeavilyDamagedItemId = 105;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HeavilyDamagedItemId
        {
            get { return m_HeavilyDamagedItemId; }
            set { m_HeavilyDamagedItemId = value; }
        }

        private int m_HeavilyDamagedHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HeavilyDamagedHue
        {
            get { return m_HeavilyDamagedHue; }
            set { m_HeavilyDamagedHue = value; }
        }

        //Broken
        private int m_BrokenSound = 0x3B7;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BrokenSound
        {
            get { return m_BrokenSound; }
            set { m_BrokenSound = value; }
        }

        private int m_BrokenItemId = 6012;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BrokenItemId
        {
            get { return m_BrokenItemId; }
            set { m_BrokenItemId = value; }
        }

        private int m_BrokenHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BrokenHue
        {
            get { return m_BrokenHue; }
            set { m_BrokenHue = value; }
        }

        private bool m_HideOnBreak = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool HideOnBreak
        {
            get { return m_HideOnBreak; }
            set { m_HideOnBreak = value; }
        }

        private bool m_DeleteOnBreak = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DeleteOnBreak
        {
            get { return m_DeleteOnBreak; }
            set { m_DeleteOnBreak = value; }
        }        

        private bool m_CreateTimedStaticAfterBreak = true;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool CreateTimedStaticAfterBreak
        {
            get { return m_CreateTimedStaticAfterBreak; }
            set { m_CreateTimedStaticAfterBreak = value; }
        }

        private string m_TimedStaticOnBreakName = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string TimedStaticOnBreakName
        {
            get { return m_TimedStaticOnBreakName; }
            set { m_TimedStaticOnBreakName = value; }
        }

        private int m_TimedStaticOnBreakItemId = 6012;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TimedStaticOnBreakItemId
        {
            get { return m_TimedStaticOnBreakItemId; }
            set { m_TimedStaticOnBreakItemId = value; }
        }

        private int m_TimedStaticOnBreakHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TimedStaticOnBreakHue
        {
            get { return m_TimedStaticOnBreakHue; }
            set { m_TimedStaticOnBreakHue = value; }
        }

        private double m_TimedStaticDuration = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TimedStaticDuration
        {
            get { return m_TimedStaticDuration; }
            set { m_TimedStaticDuration = value; }
        }

        private bool m_RevealNearbyHiddenItemsOnBreak = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RevealNearbyHiddenItemsOnBreak
        {
            get { return m_RevealNearbyHiddenItemsOnBreak; }
            set { m_RevealNearbyHiddenItemsOnBreak = value; }
        }

        private int m_RevealNearbyHiddenItemsOnBreakRadius = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RevealNearbyHiddenItemsOnBreakRadius
        {
            get { return m_RevealNearbyHiddenItemsOnBreakRadius; }
            set { m_RevealNearbyHiddenItemsOnBreakRadius = value; }
        }

        private bool m_RefreshNearbyMovables = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool RefreshNearbyMovables
        {
            get { return m_RefreshNearbyMovables; }
            set 
            {
                m_RefreshNearbyMovables = value;

                if (m_RefreshNearbyMovables)
                {
                    if (m_Timer != null)
                    {
                        m_Timer = new InternalTimer(this);
                        m_Timer.Start();
                    }

                    else                    
                        m_Timer.Start();                    
                }

                else
                {
                    if (m_Timer != null)
                    {
                        m_Timer.Stop();
                        m_Timer = null;
                    }
                }               
            }
        }

        private int m_RefreshNearbyMovablesRadius = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int RefreshNearbyMovablesRadius
        {
            get { return m_RefreshNearbyMovablesRadius; }
            set { m_RefreshNearbyMovablesRadius = value; }
        }

        public LOSBlocker m_LOSBlocker;
        private Timer m_Timer;

        #endregion

        public enum InteractionType
        {
            None,
            Normal,
            Weapon,
            Lockpick,
            Mining,
            Lumberjacking,
            ObjectBreakingDevice
        }

        [Constructable]
        public BreakableStatic(): base()
        {
            Name = "";

            Movable = false;

            UpdateDamagedState();
            
            Timer.DelayCall(TimeSpan.FromMilliseconds(50), new TimerCallback(AddComponents));
        }

        [Constructable]
        public BreakableStatic(int itemId, int hue, bool AddLOSBlocker, bool visible): base()
        {
            Name = "";
             Movable = false;

            ItemID = itemId;
            Hue = hue;
            
            NormalItemId = itemId;
            NormalHue = hue;
           
            Timer.DelayCall(TimeSpan.FromMilliseconds(50), new TimerCallback(AddComponents));
        }

        public virtual void AddComponents()
        {
            if (this == null) return;
            if (Deleted) return;
            if (!m_AddLOSBlocker) return;

            LOSBlocker LOSBlocker = new LOSBlocker();

            LOSBlocker.MoveToWorld(Location, Map);
            m_LOSBlocker = LOSBlocker;
        } 

        public BreakableStatic(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                LabelTo(from, Name);

            if (DamageState != DamageStateType.Broken)
                LabelTo(from, "[Durability " + HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");
            
            else
            {
                if (RequiresFullRepair)
                    LabelTo(from, "[Construction: " + HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");

                else
                    LabelTo(from, "[Broken: " + HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            if (!from.InRange(GetWorldLocation(), m_InteractionRange))
            {
                from.SendMessage("That is too far away.");
                return;
            }

            if (HitPoints > 0)
            {
                InteractionType interactionType = InteractionType.None;

                double highestScalar = 0;

                if (InteractDamageScalar > 0)
                {
                    highestScalar = InteractDamageScalar;
                    interactionType = InteractionType.Normal;
                }

                bool validMiningItem = false;

                Item equippedItem = from.FindItemOnLayer(Layer.FirstValid);

                if (equippedItem is Pickaxe || equippedItem is SturdyPickaxe || equippedItem is DiamondPickaxe)
                    validMiningItem = true;

                BaseWeapon weapon = from.Weapon as BaseWeapon;

                if (weapon != null || validMiningItem)
                {
                    if (m_WeaponDamageScalar > 0)
                    {
                        highestScalar = m_WeaponDamageScalar;
                        interactionType = InteractionType.Weapon;
                    }

                    if (m_LumberjackingDamageScalar > 0 && m_LumberjackingDamageScalar > highestScalar && weapon is BaseAxe)
                    {
                        highestScalar = m_LumberjackingDamageScalar;
                        interactionType = InteractionType.Lumberjacking;
                    }

                    if (m_MiningDamageScalar > 0 && m_MiningDamageScalar > highestScalar && validMiningItem)
                    {
                        highestScalar = m_MiningDamageScalar;
                        interactionType = InteractionType.Mining;
                    }
                }

                Interact(from, interactionType);
            }

            else
                base.OnDoubleClick(from);
        }

        public TimeSpan GetInteractCooldown(Mobile from, InteractionType interactionType)
        {
            double usageDelay = 5;

            BaseWeapon weapon;

            switch (interactionType)
            {
                case InteractionType.None:                    
                    return TimeSpan.FromSeconds(usageDelay);
                break;

                case InteractionType.Normal:
                    usageDelay = InteractionDelay;
                break;

                case InteractionType.Weapon:
                    weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                        usageDelay = weapon.GetDelay(from, false).TotalSeconds;
                break;

                case InteractionType.Lockpick:
                    usageDelay = InteractionDelay;
                break;

                case InteractionType.Mining:
                    usageDelay = InteractionDelay;
                break;

                case InteractionType.Lumberjacking:
                    usageDelay = InteractionDelay;
                break;
            }

            return TimeSpan.FromSeconds(usageDelay);
        }

        public virtual void Interact(Mobile from, InteractionType interactionType)
        {
            if (from == null)
                return;

            if (!from.CanBeginAction(typeof(BreakableStatic)))
            {
                from.SendMessage("You must wait a few moments before attempting to use that again.");
                return;
            }

            if (interactionType == InteractionType.None)
            {
                from.SendMessage("That cannot be interacted with in that way.");
                return;
            }

            PlayerMobile player = from as PlayerMobile;
            BaseCreature bc_Creature = from as BaseCreature;

            from.RevealingAction();

            BeforeInteract(from, interactionType);

            TimeSpan usageDelay = GetInteractCooldown(from, interactionType);

            BaseWeapon weapon;
            
            from.BeginAction(typeof(BreakableStatic));

            Timer.DelayCall(usageDelay, delegate
            {
                if (from != null)
                    from.EndAction(typeof(BreakableStatic));
            });

            if (interactionType == InteractionType.Lumberjacking || interactionType == InteractionType.Mining || interactionType == InteractionType.Normal || interactionType == InteractionType.Weapon)
            {
                Timer.DelayCall(usageDelay + TimeSpan.FromSeconds(1), delegate
                {
                    if (player != null && !Deleted)                    
                        OnDoubleClick(player);                    
                });
            }

            int damage = 0;
            double randomScalar = (double)(Utility.RandomMinMax(75, 125)) / 100;

            Direction direction;

            switch (interactionType)
            {
                case InteractionType.None:
                    return;
                break;

                case InteractionType.Normal:
                    damage = (int)(Math.Round((double)(Utility.RandomMinMax(m_MinInteractDamage, m_MaxInteractDamage)) * InteractDamageScalar));

                    direction = from.GetDirectionTo(Location);

                    if (direction != from.Direction)
                        from.Direction = direction;

                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {
                        if (from == null) return;
                        if (!from.Alive) return;

                        from.Animate(12, 5, 1, true, false, 0);
                        from.RevealingAction();

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (from == null) return;
                            if (!from.Alive) return;

                            Effects.PlaySound(from.Location, from.Map, m_HitSound);
                        });
                    });
                break;

                case InteractionType.Weapon:
                    weapon = from.Weapon as BaseWeapon;

                    if (weapon != null)
                    {
                        int minDamage = weapon.MinDamage;
                        int maxDamage = weapon.MaxDamage;

                        if (bc_Creature != null)
                        {
                            minDamage = bc_Creature.DamageMin;
                            maxDamage = bc_Creature.DamageMax;
                        }

                        if ((weapon is Pickaxe || weapon is Hatchet))
                        {
                            minDamage = 10;
                            maxDamage = 20;
                        }

                        if (UOACZSystem.IsUOACZValidMobile(player))
                        {     
                            if (player.IsUOACZUndead)
                            {
                                minDamage = player.m_UOACZAccountEntry.UndeadProfile.DamageMin;
                                maxDamage = player.m_UOACZAccountEntry.UndeadProfile.DamageMax;
                            }
                        }

                        if (minDamage < m_MinInteractDamage)
                            minDamage = m_MinInteractDamage;

                        if (maxDamage < m_MaxInteractDamage)
                            maxDamage = m_MaxInteractDamage;

                        damage = (int)(Math.Round((double)(Utility.RandomMinMax(minDamage, maxDamage)) * WeaponDamageScalar));
                    }

                    direction = from.GetDirectionTo(Location);

                    if (direction != from.Direction)
                        from.Direction = direction;

                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {
                        if (from == null) return;
                        if (!from.Alive) return;

                        if (bc_Creature != null)
                        {
                            UOACZBaseUndead undeadCreature = bc_Creature as UOACZBaseUndead;

                            if (undeadCreature != null)                            
                                undeadCreature.m_LastActivity = DateTime.UtcNow;                            

                            if (weapon != null)
                                weapon.PlaySwingAnimation(bc_Creature);
                        }

                        else
                            from.Animate(12, 5, 1, true, false, 0);

                        from.RevealingAction();

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (from == null) return;
                            if (!from.Alive) return;

                            Effects.PlaySound(from.Location, from.Map, m_HitSound);
                        });
                    });
                break;

                case InteractionType.Lockpick:
                    damage = (int)(Math.Round(from.Skills.Lockpicking.Value * LockpickDamageScalar * randomScalar));

                    if (damage < 20)
                        damage = 20;

                    direction = from.GetDirectionTo(Location);

                    if (direction != from.Direction)
                        from.Direction = direction;

                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {  
                        if (from == null) return;
                        if (!from.Alive) return;

                        from.Animate(32, 5, 1, true, false, 0);
                        from.RevealingAction();

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {  
                            if (from == null) return;
                            if (!from.Alive) return;

                            Effects.PlaySound(from.Location, from.Map, 0x241);
                        });
                    });
                break;

                case InteractionType.Mining:  
                    bool validMiningItem = false;

                    Item equippedItem = from.FindItemOnLayer(Layer.FirstValid);

                    if (equippedItem is Pickaxe || equippedItem is SturdyPickaxe || equippedItem is DiamondPickaxe)
                        validMiningItem = true;
                    
                    if (validMiningItem)
                    {
                        damage = (int)(Math.Round(from.Skills.Mining.Value * MiningDamageScalar * randomScalar));

                        if (damage < 20)
                            damage = 20;
                    }

                    direction = from.GetDirectionTo(Location);

                    if (direction != from.Direction)
                        from.Direction = direction;

                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {  
                        if (from == null) return;
                        if (!from.Alive) return;

                        from.Animate(Utility.RandomList(11, 12), 5, 1, true, false, 0);                        

                        from.RevealingAction();

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (from == null) return;
                            if (!from.Alive) return;

                            Effects.PlaySound(from.Location, from.Map, m_HitSound);
                        });
                    });
                break;

                case InteractionType.Lumberjacking:
                    weapon = from.Weapon as BaseWeapon;

                    if (weapon != null && (weapon is BaseAxe))
                    {
                        damage = (int)(Math.Round(from.Skills.Lumberjacking.Value * LumberjackingDamageScalar * randomScalar));

                        if (damage < 20)
                            damage = 20;
                    }

                    direction = from.GetDirectionTo(Location);

                    if (direction != from.Direction)
                        from.Direction = direction;

                    Timer.DelayCall(TimeSpan.FromSeconds(.25), delegate
                    {  
                        if (from == null) return;
                        if (!from.Alive) return;

                        from.Animate(12, 5, 1, true, false, 0);
                        from.RevealingAction();

                        Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
                        {
                            if (from == null) return;
                            if (!from.Alive) return;

                            Effects.PlaySound(from.Location, from.Map, m_HitSound);
                        });
                    });
                break;
            }

            if (damage < 1)
                damage = 1;

            ReceiveDamage(from, damage, interactionType);

            AfterInteract(from, interactionType);
        }

        public virtual void ReceiveDamage(Mobile from, int damage, InteractionType interactionType)
        {
            int lightlyDamagedThreshold = (int)(Math.Round((double)MaxHitPoints * LightlyDamagedPercent));
            int heavilyDamagedThreshold = (int)(Math.Round((double)MaxHitPoints * HeavilyDamagedPercent)); 

            DamageStateType oldDamageStateType = DamageState;            
            DamageStateType newDamageStateType = DamageStateType.Normal;

            HitPoints -= damage;

            if (HitPoints <= lightlyDamagedThreshold)
                newDamageStateType = DamageStateType.LightlyDamaged;

            if (HitPoints <= heavilyDamagedThreshold)
                newDamageStateType = DamageStateType.HeavilyDamaged;

            if (HitPoints < 0)
                HitPoints = 0;

            if (HitPoints == 0)
            {
                PublicOverheadMessage(MessageType.Regular, 0, false, "*breaks*");
                
                Effects.PlaySound(Location, Map, m_BrokenSound);
                                
                if (m_CreateTimedStaticAfterBreak)
                {
                    TimedStatic timedStatic = new TimedStatic(TimedStaticOnBreakItemId, TimedStaticDuration);
                    timedStatic.Name = TimedStaticOnBreakName;
                    timedStatic.Hue = TimedStaticOnBreakHue;
                    timedStatic.MoveToWorld(Location, Map);
                }

                if (RevealNearbyHiddenItemsOnBreak)
                {
                    IPooledEnumerable itemsOnTile = Map.GetItemsInRange(Location, RevealNearbyHiddenItemsOnBreakRadius);

                    foreach (Item item in itemsOnTile)
                    {
                        if (!item.Visible)
                            item.Visible = true;
                    }

                    itemsOnTile.Free();
                }

                AfterReceiveDamage(from, damage, interactionType);

                BeforeBreak(from, interactionType);
                
                if (m_DeleteOnBreak)
                    Delete();

                AfterBreak(from, interactionType);                
            }

            else
            {
                if (oldDamageStateType != newDamageStateType)
                {
                    switch (newDamageStateType)
                    {
                        case DamageStateType.LightlyDamaged:
                            Effects.PlaySound(Location, Map, m_LightlyDamagedSound);
                            NormalToLightlyDamaged(from, interactionType);
                        break;

                        case DamageStateType.HeavilyDamaged:
                            Effects.PlaySound(Location, Map, m_HeavilyDamagedSound);
                            LightlyDamagedToHeavilyDamaged(from, interactionType);
                        break;
                    }                    
                }

                Effects.SendLocationParticles(EffectItem.Create(new Point3D(Location.X, Location.Y, Location.Z), Map, EffectItem.DefaultDuration), 0x377A, 10, 10, 9502);

                PublicOverheadMessage(MessageType.Regular, UOACZSystem.greyTextHue, false, "-" + damage.ToString());

                if (from != null)
                    from.SendMessage("You inflict " + damage.ToString() + " damage upon your target. [Remaining durability: " + HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");
            
                AfterReceiveDamage(from, damage, interactionType);
            }
        }

        public virtual void AfterReceiveDamage(Mobile from, int damage, InteractionType interactionType)
        {
        }

        public virtual bool Repair(Mobile from, Item item, double value)
        {
            if (!m_AllowRepair)
                return false;

            return true;
        }

        public virtual void UpdateDamagedState()
        {  
            double damagePercent = (double)HitPoints / (double)m_MaxHitPoints;

            if (HitPoints == 0)
                DamageState = DamageStateType.Broken;

            else if (damagePercent <= HeavilyDamagedPercent)
                DamageState = DamageStateType.HeavilyDamaged;

            else if (damagePercent <= LightlyDamagedPercent)
                DamageState = DamageStateType.LightlyDamaged;

            else
                DamageState = DamageStateType.Normal;

            if (HideOnBreak)
                Visible = true;

            switch (DamageState)
            {
                case DamageStateType.Normal:
                    ItemID = NormalItemId;
                    Hue = NormalHue;
                break;

                case DamageStateType.LightlyDamaged:
                    ItemID = LightlyDamagedItemId;
                    Hue = LightlyDamagedHue;
                break;

                case DamageStateType.HeavilyDamaged:
                    ItemID = HeavilyDamagedItemId;
                    Hue = HeavilyDamagedHue;
                break;

                case DamageStateType.Broken:
                    ItemID = BrokenItemId;
                    Hue = BrokenHue;

                    if (HideOnBreak)
                        Visible = false;
                break;
            }        
        }

        public virtual void BeforeInteract(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void AfterInteract(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void NormalToLightlyDamaged(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void LightlyDamagedToHeavilyDamaged(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void BeforeBreak(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void AfterBreak(Mobile from, InteractionType interactionType)
        {
        }

        private class InternalTimer : Timer
        {
            private BreakableStatic m_BreakableStatic;

            public InternalTimer(BreakableStatic breakableStatic): base(TimeSpan.FromSeconds(0), TimeSpan.FromMinutes(5))
            {
                Priority = TimerPriority.FiveSeconds;

                m_BreakableStatic = breakableStatic;
            }

            protected override void OnTick()
            {
                if (m_BreakableStatic == null)
                {
                    Stop();
                    return;
                }

                if (m_BreakableStatic.Deleted)
                {
                    Stop();
                    return;
                }

                if (m_BreakableStatic.RefreshNearbyMovables)
                {
                    IPooledEnumerable itemsOnTile = m_BreakableStatic.Map.GetItemsInRange(m_BreakableStatic.Location, m_BreakableStatic.RefreshNearbyMovablesRadius);

                    foreach (Item item in itemsOnTile)
                    {
                        if (item.Decays)
                            item.SetLastMoved();                       
                    }

                    itemsOnTile.Free();
                }
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_Timer != null)
            {
                m_Timer.Stop();
                m_Timer = null;
            }

            if (m_LOSBlocker != null)
            {
                if (!m_LOSBlocker.Deleted)
                    m_LOSBlocker.Delete();
            }

            if (!Deleted)
                Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version

            writer.Write(m_AllowRepair);
            writer.Write(m_RepairSound);
            writer.Write((int)m_DamageState);
            writer.Write(m_InteractionRange);
            writer.Write(m_InteractionDelay);
            writer.Write(m_MinInteractDamage);
            writer.Write(m_MaxInteractDamage);
            writer.Write(m_InteractDamageScalar);
            writer.Write(m_WeaponDamageScalar);
            writer.Write(m_LockpickDamageScalar);
            writer.Write(m_MiningDamageScalar);
            writer.Write(m_LumberjackingDamageScalar);
            writer.Write(m_ObjectBreakingDeviceDamageScalar);
            writer.Write(m_HitPoints);
            writer.Write(m_MaxHitPoints);
            writer.Write(m_AddLOSBlocker);            
            writer.Write(m_HitSound);

            writer.Write(m_NormalItemId);
            writer.Write(m_NormalHue);

            writer.Write(m_LightlyDamagedPercent);
            writer.Write(m_LightlyDamagedSound);
            writer.Write(m_LightlyDamagedItemId);
            writer.Write(m_LightlyDamagedHue);

            writer.Write(m_HeavilyDamagedPercent);
            writer.Write(m_HeavilyDamagedSound);
            writer.Write(m_HeavilyDamagedItemId);
            writer.Write(m_HeavilyDamagedHue);

            writer.Write(m_BrokenSound);
            writer.Write(m_BrokenItemId);
            writer.Write(m_BrokenHue);

            writer.Write(m_DeleteOnBreak);
            writer.Write(m_CreateTimedStaticAfterBreak);

            writer.Write(m_TimedStaticOnBreakItemId);
            writer.Write(m_TimedStaticOnBreakHue);
            writer.Write(m_TimedStaticDuration);
            writer.Write(m_TimedStaticOnBreakName);
            writer.Write(m_RevealNearbyHiddenItemsOnBreak);
            writer.Write(m_RevealNearbyHiddenItemsOnBreakRadius);
            writer.Write(m_RefreshNearbyMovables);
            writer.Write(m_RefreshNearbyMovablesRadius);
            writer.Write(m_LOSBlocker);

            //Version 1
            writer.Write(m_RequiresFullRepair);
            writer.Write(m_PlayerPlaced);
            writer.Write((int)m_Facing);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            int storedHitPoints = 1000;
            int storedMaxHitPoints = 1000;

            //Version 0
            if (version >= 0)
            {
                m_AllowRepair = reader.ReadBool();
                m_RepairSound = reader.ReadInt();
                m_DamageState = (DamageStateType)reader.ReadInt();
                m_InteractionRange = reader.ReadInt();
                m_InteractionDelay = reader.ReadDouble();
                m_MinInteractDamage = reader.ReadInt();
                m_MaxInteractDamage = reader.ReadInt();
                m_InteractDamageScalar = reader.ReadDouble();
                m_WeaponDamageScalar = reader.ReadDouble();
                m_LockpickDamageScalar = reader.ReadDouble();
                m_MiningDamageScalar = reader.ReadDouble();
                m_LumberjackingDamageScalar = reader.ReadDouble();
                m_ObjectBreakingDeviceDamageScalar = reader.ReadDouble();

                storedHitPoints = reader.ReadInt();
                storedMaxHitPoints = reader.ReadInt();

                m_AddLOSBlocker = reader.ReadBool();
                m_HitSound = reader.ReadInt();

                m_NormalItemId = reader.ReadInt();
                m_NormalHue = reader.ReadInt();

                m_LightlyDamagedPercent = reader.ReadDouble();
                m_LightlyDamagedSound = reader.ReadInt();
                m_LightlyDamagedItemId = reader.ReadInt();
                m_LightlyDamagedHue = reader.ReadInt();

                m_HeavilyDamagedPercent = reader.ReadDouble();
                m_HeavilyDamagedSound = reader.ReadInt();
                m_HeavilyDamagedItemId = reader.ReadInt();
                m_HeavilyDamagedHue = reader.ReadInt();

                m_BrokenSound = reader.ReadInt();
                m_BrokenItemId = reader.ReadInt();
                m_BrokenHue = reader.ReadInt();

                m_DeleteOnBreak = reader.ReadBool();
                m_CreateTimedStaticAfterBreak = reader.ReadBool();
              
                m_TimedStaticOnBreakItemId = reader.ReadInt();
                m_TimedStaticOnBreakHue = reader.ReadInt();
                m_TimedStaticDuration = reader.ReadDouble();
                m_TimedStaticOnBreakName = reader.ReadString();
                m_RevealNearbyHiddenItemsOnBreak = reader.ReadBool();
                m_RevealNearbyHiddenItemsOnBreakRadius = reader.ReadInt();
                m_RefreshNearbyMovables = reader.ReadBool();
                m_RefreshNearbyMovablesRadius = reader.ReadInt();
                m_LOSBlocker = reader.ReadItem() as LOSBlocker;
            }  
         
            //Version 1
            if (version >= 1)
            {
                m_RequiresFullRepair = reader.ReadBool();
                m_PlayerPlaced = reader.ReadBool();
                m_Facing = (Direction)reader.ReadInt();
            }


            //--------

            //Make Sure This Gets Fired Off Last
            MaxHitPoints = storedMaxHitPoints;
            HitPoints = storedHitPoints;
        }
    }
}
