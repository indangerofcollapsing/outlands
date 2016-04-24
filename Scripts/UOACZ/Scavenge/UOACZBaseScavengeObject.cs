using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;


namespace Server.Custom
{
    public class UOACZBaseScavengeObject : MetalChest
    {
        public enum ScavengeTrapType
        {
            None,
            Poison,            
            Hinder,
            Explosion,
            Undead
        }

        #region Properties

        public virtual TimeSpan ScavengeDuration { get { return TimeSpan.FromSeconds(3); } }
        public virtual TimeSpan ScavengeCooldown { get { return TimeSpan.FromSeconds(0); } }

        public virtual string NoYieldRemainingSingleClickText { get { return "(searched)"; } }
        public virtual string LockedSingleClickText { get { return "(locked)"; } }

        public virtual string NoYieldRemainingText { get { return "It appears to have been thoroughly searched."; } }
        public virtual string InteractText { get { return "You begin searching."; } }

        public virtual string ScavengeResultSuccessWithTrapText { get { return "Your search was successful, but something is amiss..."; } }
        public virtual string ScavengeResultSuccessText { get { return "Your search was successful."; } }
        public virtual string ScavengeResultFailWithTrapText { get { return "Your search turns up nothing, but something is amiss..."; } }
        public virtual string ScavengeResultFailText { get { return "You search for a while but fail to find anything."; } }

        public virtual string ScavengeUndeadTrapText { get { return "*noise from scavenging draws unwanted attention...*"; } }

        public int YieldsRemaining { get { return Items.Count; } }
        
        private int m_InteractionRange = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int InteractionRange
        {
            get { return m_InteractionRange; }
            set { m_InteractionRange = value; }
        }

        private int m_StartingYieldCount = 15;
        [CommandProperty(AccessLevel.GameMaster)]
        public int StartingYieldCount
        {
            get { return m_StartingYieldCount; }
            set { m_StartingYieldCount = value; }
        }

        private int m_MaxPlayerInteractions = 3;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxPlayerInteractions
        {
            get { return m_MaxPlayerInteractions; }
            set { m_MaxPlayerInteractions = value; }
        }

        private int m_ScavengeDifficulty = 75;
        [CommandProperty(AccessLevel.GameMaster)]
        public int ScavengeDifficulty
        {
            get { return m_ScavengeDifficulty; }
            set { m_ScavengeDifficulty = value; }
        }

        private ScavengeTrapType m_TrapType = ScavengeTrapType.None;
        [CommandProperty(AccessLevel.GameMaster)]
        public ScavengeTrapType TrapType
        {
            get { return m_TrapType; }
            set { m_TrapType = value; }
        }

        private int m_TrapDifficulty = 50;
        [CommandProperty(AccessLevel.GameMaster)]
        public int TrapDifficulty
        {
            get { return m_TrapDifficulty; }
            set { m_TrapDifficulty = value; }
        }

        private double m_TrapResolveChance = .50;
        [CommandProperty(AccessLevel.GameMaster)]
        public double TrapResolveChance
        {
            get { return m_TrapResolveChance; }
            set { m_TrapResolveChance = value; }
        }

        private bool m_Locked = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Locked
        {
            get { return m_Locked; }
            set { m_Locked = value; }
        }

        private int m_LockDifficulty = 100;
        [CommandProperty(AccessLevel.GameMaster)]
        public int LockDifficulty
        {
            get { return m_LockDifficulty; }
            set { m_LockDifficulty = value; }
        }

        private int m_HitPoints = 750;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get { return m_HitPoints; }
            set { m_HitPoints = value; }
        }

        private int m_MaxHitPoints = 750;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get { return m_MaxHitPoints; }
            set { m_MaxHitPoints = value; }
        }

        private int m_HitSound = 0x3B7;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HitSound
        {
            get { return m_HitSound; }
            set { m_HitSound = value; }
        }

        private int m_BreakSound = 0x142;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BreakSound
        {
            get { return m_BreakSound; }
            set { m_BreakSound = value; }
        }

        private DateTime m_Expiration = DateTime.MaxValue;
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime Expiration
        {
            get { return m_Expiration; }
            set { m_Expiration = value; }
        }

        private bool m_Corrupted = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Corrupted
        {
            get { return m_Corrupted; }
            set { m_Corrupted = value; }
        }

        public Item m_CorruptionItem;

        public Dictionary<PlayerMobile, int> m_Interactors = new Dictionary<PlayerMobile, int>();
        public Dictionary<PlayerMobile, int> m_TrapImmunePlayers = new Dictionary<PlayerMobile, int>();

        public Timer m_Timer;

        #endregion

        [Constructable]
        public UOACZBaseScavengeObject(): base()
        {
            Name = "";
            Movable = false;

            ItemID = 2475;

            Init();

            m_Timer = new InternalTimer(this);
            m_Timer.Start();           
        }

        public UOACZBaseScavengeObject(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {         
            LabelTo(from, Name);

            if (from.AccessLevel > AccessLevel.Player)
            {
                LabelTo(from, "(" + YieldsRemaining.ToString() + " items remaining)");
            }

            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            int interactionCount = GetInteractions(player);

            if (from.AccessLevel == AccessLevel.Player)
            {
                if (YieldsRemaining == 0 || interactionCount >= MaxPlayerInteractions)
                    LabelTo(from, NoYieldRemainingSingleClickText);
            }

            if (Locked)
                LabelTo(from, LockedSingleClickText);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
            {
                Open(from);
                return;
            }

            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;
            
            if (Utility.GetDistance(player.Location, Location) > InteractionRange)
            {
                from.SendMessage("You are too far away to use that.");
                return;
            }

            if (!Map.InLOS(player.Location, Location))
            {
                 from.SendMessage("That is not within your line of sight.");
                return;
            }
            
            if (Locked)
            {
                if (!player.CanBeginAction(typeof(UOACZBaseScavengeObject)))
                {
                    player.SendMessage("You must wait a moment before using that.");
                    return;
                }

                ForceLock(player);
                return;
            }
            
            int searchCount = GetInteractions(player);

            if (YieldsRemaining == 0 || searchCount >= MaxPlayerInteractions)
            {
                player.SendMessage(NoYieldRemainingText);
                return;                
            }

            else
            {
                if (!player.CanBeginAction(typeof(UOACZBaseScavengeObject)))
                {
                    player.SendMessage("You must wait a moment before using that.");
                    return;
                }
                
                if (!CanInteract(player))
                    return;
                
                Interact(player);
                return;
            }
        }        

        public int GetInteractions(PlayerMobile player)
        {
            int count = 0;

            if (m_Interactors.ContainsKey(player))
                count = m_Interactors[player];

            return count;
        }
        
        public virtual void Interact(PlayerMobile player)
        {   
            if (!UOACZSystem.IsUOACZValidMobile(player)) 
                return;

            Direction direction = player.GetDirectionTo(Location);

            if (direction != player.Direction)
                player.Direction = direction;

            TimeSpan startDelay = TimeSpan.FromSeconds(.25);

            double totalInteractTime = startDelay.TotalSeconds + ScavengeDuration.TotalSeconds;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, totalInteractTime, true, 0, false, "", InteractText, "-1");

            player.BeginAction(typeof(UOACZBaseScavengeObject));

            Timer.DelayCall(TimeSpan.FromSeconds(totalInteractTime) + ScavengeCooldown, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                player.EndAction(typeof(UOACZBaseScavengeObject));
            });

            Timer.DelayCall(startDelay, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                DoAction(player);               

                Timer.DelayCall(ScavengeDuration, delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (Utility.GetDistance(player.Location, Location) > InteractionRange)
                    {
                        player.SendMessage("You are too far away to continue using that.");
                        return;
                    }

                    if (YieldsRemaining == 0)
                    {
                        player.SendMessage(NoYieldRemainingText);
                        return;
                    }

                    ScavengeResult(player, false);
                });
            });
        }

        public virtual void LockpickInteract(PlayerMobile player, UOACZLockpickKit lockpickKit)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            Direction direction = player.GetDirectionTo(Location);

            if (direction != player.Direction)
                player.Direction = direction;

            TimeSpan startDelay = TimeSpan.FromSeconds(.25);

            double totalInteractTime = startDelay.TotalSeconds + ScavengeDuration.TotalSeconds;

            lockpickKit.Charges--;

            if (lockpickKit.Charges <= 0)
                lockpickKit.Delete();

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, totalInteractTime, true, 0, false, "", InteractText, "-1");

            player.BeginAction(typeof(UOACZBaseScavengeObject));

            Timer.DelayCall(TimeSpan.FromSeconds(totalInteractTime) + ScavengeCooldown, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                player.EndAction(typeof(UOACZBaseScavengeObject));
            });

            Timer.DelayCall(startDelay, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                player.Animate(32, 5, 1, true, false, 0);
                player.RevealingAction();

                Effects.PlaySound(player.Location, player.Map, 0x241);

                Timer.DelayCall(ScavengeDuration, delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (Utility.GetDistance(player.Location, Location) > InteractionRange)
                    {
                        player.SendMessage("You are too far away to continue using that.");
                        return;
                    }

                    if (YieldsRemaining == 0)
                    {
                        player.SendMessage(NoYieldRemainingText);
                        return;
                    }

                    ScavengeResult(player, true);
                });
            });
        }

        public virtual void RemoveTrap(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            if (m_TrapImmunePlayers.ContainsKey(player))
            {
                player.SendMessage("You are confident you will not suffer any ill effects from whatever lies within.");
                player.SendSound(0x5AA);

                return;
            }

            Direction direction = player.GetDirectionTo(Location);

            if (direction != player.Direction)
                player.Direction = direction;

            TimeSpan startDelay = TimeSpan.FromSeconds(.25);

            double totalInteractTime = startDelay.TotalSeconds + ScavengeDuration.TotalSeconds;

            SpecialAbilities.HinderSpecialAbility(1.0, null, player, 1.0, totalInteractTime, true, 0, false, "", "You begin to examine the object.", "-1");

            player.BeginAction(typeof(UOACZBaseScavengeObject));

            Timer.DelayCall(TimeSpan.FromSeconds(totalInteractTime) + ScavengeCooldown, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                player.EndAction(typeof(UOACZBaseScavengeObject));
            });

            Timer.DelayCall(startDelay, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                player.Animate(32, 5, 1, true, false, 0);
                player.RevealingAction();

                double removeTrapSkill = player.Skills.RemoveTrap.Value / 100;

                if (Utility.RandomDouble() <= removeTrapSkill || TrapType == ScavengeTrapType.None)
                {
                    m_TrapImmunePlayers.Add(player, 500);
                    player.SendMessage("You are confident you will not suffer any ill effects from whatever lies within.");

                    player.SendSound(0x5AA);
                }

                else
                {
                    player.SendMessage("You are unsure of what lies within the object.");

                    return;
                }
            });
        }

        public virtual bool CanInteract(PlayerMobile player)
        {
            bool canInteract = true;

            if (!UOACZSystem.IsUOACZValidMobile(player))
                return false;            

            return canInteract;
        }

        public virtual void ScavengeResult(PlayerMobile player, bool lockpickAttempt)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            double searcherChance = player.GetSpecialAbilityEntryValue(SpecialAbilityEffect.Searcher);

            if (!lockpickAttempt)
            {
                if (Utility.RandomDouble() > searcherChance)
                {
                    if (m_Interactors.ContainsKey(player))
                        m_Interactors[player]++;

                    else
                        m_Interactors.Add(player, 1);
                }
            }

            bool success = false;
            bool trapped = false;

            if (TrapType != ScavengeTrapType.None)
                trapped = true;
            
            bool scavengeResult = GetScavengeResult(player, lockpickAttempt);

            bool ignoreTrap = false;

            if (YieldsRemaining == 0)
            {
                scavengeResult = false;
                ignoreTrap = true;
            }

            if (m_TrapImmunePlayers.ContainsKey(player))
                ignoreTrap = true;

            bool resolveTrap = false;

            if (TrapType != ScavengeTrapType.None && !ignoreTrap)
            {
                if (Utility.RandomDouble() <= m_TrapResolveChance)
                    resolveTrap = true;
                
                double removeTrapSkill = player.Skills.RemoveTrap.Value / 100;

                if (Utility.RandomDouble() <= removeTrapSkill)
                {  
                    resolveTrap = false;

                    if (player.NetState != null)
                        player.PrivateOverheadMessage(MessageType.Regular, 0, false, "*there is something amiss here*", player.NetState);
                }
            }

            if (scavengeResult)
            {
                Item item = ResolveLoot(player);

                if (item != null)
                {
                    if (trapped && resolveTrap && !ignoreTrap)
                        player.SendMessage(ScavengeResultSuccessWithTrapText);

                    else
                        player.SendMessage(ScavengeResultSuccessText);
                }

                string itemName = item.Name;

                if (itemName == "" || itemName == null)
                {
                    if (item is BaseWeapon)
                        itemName = "a weapon";

                    if (item is BaseArmor)
                        itemName = "some armor";

                    if (item is BaseShield)
                        itemName = "a shield";
                }

                if (item.Amount > 1)
                    player.SendMessage(UOACZSystem.lightGreenTextHue, "You find: " + itemName + " (" + item.Amount.ToString() + ")");
  
                else
                    player.SendMessage(UOACZSystem.lightGreenTextHue, "You find: " + itemName);               
            }

            else
            {
                if (trapped && resolveTrap && !ignoreTrap)
                    player.SendMessage(ScavengeResultFailWithTrapText);

                else
                    player.SendMessage(ScavengeResultFailText);
            }

           if (resolveTrap)
                ResolveTrap(player);
        }

        public virtual void ForceLock(PlayerMobile player)
        {
            if (!UOACZSystem.IsUOACZValidMobile(player))
                return;

            BaseWeapon weapon = player.Weapon as BaseWeapon;

            double usageDelay = ScavengeDuration.TotalSeconds;

            int damage = 5;

            if (weapon != null)
            {
                int minDamage = weapon.MinDamage;
                int maxDamage = weapon.MaxDamage;

                double strScalar = (double)player.Str / 100;
                double tacticsScalar = player.Skills.Tactics.Value / 100;

                if (weapon is Pickaxe || weapon is Hatchet)
                {
                    minDamage = 10;
                    maxDamage = 20;
                }

                damage = (int)(Math.Round(strScalar * tacticsScalar * (double)(Utility.RandomMinMax(weapon.MinDamage, weapon.MaxDamage))));

                if (damage < 1)
                    damage = 1;

                usageDelay = weapon.GetDelay(player, false).TotalSeconds;
            }

            Direction direction = player.GetDirectionTo(Location);

            if (direction != player.Direction)
                player.Direction = direction;

            TimeSpan startDelay = TimeSpan.FromSeconds(.25);
            TimeSpan endDelay = TimeSpan.FromSeconds(.5);

            double totalActionTime = startDelay.TotalSeconds + usageDelay;                        

            player.BeginAction(typeof(UOACZBaseScavengeObject));

            Timer.DelayCall(TimeSpan.FromSeconds(totalActionTime) + ScavengeCooldown, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                
                player.EndAction(typeof(UOACZBaseScavengeObject));
            });

            Timer.DelayCall(startDelay, delegate
            {
                if (!UOACZSystem.IsUOACZValidMobile(player)) return;

                player.Animate(12, 5, 1, true, false, 0);
                player.RevealingAction();

                Timer.DelayCall(endDelay, delegate
                {
                    if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                    if (Utility.GetDistance(player.Location, Location) > InteractionRange)
                    {
                        player.SendMessage("You are too far away to continue using that.");
                        return;
                    }

                    Effects.PlaySound(player.Location, player.Map, m_HitSound);
                    ReceiveDamage(player, damage);
                });
            });
        }

        public virtual void ReceiveDamage(PlayerMobile player, int damage)
        {
            if (!Locked)
                return;

            m_HitPoints -= damage;

            if (m_HitPoints < 0)
                m_HitPoints = 0;                        

            if (m_HitPoints == 0)
            {
                Effects.PlaySound(Location, Map, m_BreakSound);

                PublicOverheadMessage(MessageType.Regular, 0, false, "*lock breaks*");

                Locked = false;                   
            }

            else
            {
                PublicOverheadMessage(MessageType.Regular, UOACZSystem.greyTextHue, false, "-" + damage.ToString());

                player.SendMessage("You inflict " + damage.ToString() + " damage upon your target. [Remaining durability: " + m_HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");
            }
        }

        public virtual void DoAction(PlayerMobile player)
        {
            player.Animate(32, 5, 1, true, false, 0);
            player.RevealingAction();

            Effects.PlaySound(player.Location, player.Map, 0x241);
        }

        public virtual bool GetScavengeResult(PlayerMobile player, bool lockpickAttempt)
        {
            bool scavengeResult = false;

            return scavengeResult;
        }

        public virtual void Init()
        {
            Expiration = DateTime.UtcNow + UOACZSystem.ScavengeExpiration;
            CreateLoot();
        }

        public virtual void CreateLoot()
        {          
        }

        public virtual Item ResolveLoot(PlayerMobile player)
        {
            Item item = null;

            if (YieldsRemaining > 0)
            {
                item = Items[Utility.RandomMinMax(0, Items.Count - 1)];

                if (item is UOACZHumanUpgradeToken)
                {
                    UOACZHumanUpgradeToken token = item as UOACZHumanUpgradeToken;
                    token.Player = player;
                }

                if (item is UOACZSurvivalStone)
                {
                    UOACZSurvivalStone token = item as UOACZSurvivalStone;
                    token.Player = player;
                }

                if (!player.Backpack.TryDropItem(player, item, false))
                {
                    item.MoveToWorld(player.Location);
                    player.SendMessage("You do not have enough space in your backpack, and instead place the item at your feet.");
                }                               
            }

            if (YieldsRemaining == 0)
                m_Expiration = DateTime.UtcNow + UOACZSystem.AfterScavengeDeletion;

            return item;
        }

        public virtual void ResolveTrap(Mobile from)
        {
            switch (m_TrapType)
            {
                case ScavengeTrapType.Undead:
                    PublicOverheadMessage(MessageType.Regular, UOACZSystem.yellowTextHue, false, ScavengeUndeadTrapText);

                    int creatures = Utility.RandomMinMax(3, 5);

                    for (int a = 0; a < creatures; a++)
                    {
                        UOACZSystem.SpawnRandomCreature(from, Location, Map, UOACZPersistance.m_ThreatLevel - 50, 0, 3, false);
                    }
                break;

                case ScavengeTrapType.Explosion:
                    PublicOverheadMessage(MessageType.Regular, UOACZSystem.yellowTextHue, false, "*a trap is sprung*");

                    from.FixedParticles(0x36BD, 20, 10, 5044, 0, 0, EffectLayer.Head);
                    from.PlaySound(0x307);

                    int damageMin = (int)(Math.Round((double)TrapDifficulty / 4));
                    int damageMax = (int)(Math.Round((double)TrapDifficulty / 2));

                    int damage = Utility.RandomMinMax(damageMin, damageMax);

                    from.SendMessage("You have been hit by an explosive trap!");

                    new Blood().MoveToWorld(from.Location, from.Map);
                    AOS.Damage(from, damage, 0, 100, 0, 0, 0);
                break;

                case ScavengeTrapType.Hinder:
                    PublicOverheadMessage(MessageType.Regular, UOACZSystem.yellowTextHue, false, "*a trap is sprung*");

                    from.FixedEffect(0x376A, 10, 30, 0, 0);
                    from.PlaySound(0x204);

                    int duration = Utility.RandomMinMax(15, 30);

                    from.SendMessage("You are trapped in place!");

                    SpecialAbilities.EntangleSpecialAbility(1.0, null, from, 1.0, duration, -1, true, "", "", "-1");
                break;

                case ScavengeTrapType.Poison:
                    PublicOverheadMessage(MessageType.Regular, UOACZSystem.yellowTextHue, false, "*a trap is sprung*");

                    int poisonLevel = 0;

                    double poisonChance = Utility.RandomDouble();

                    if (poisonChance <= .25)
                        poisonLevel = 0;

                    else if (poisonChance <= .70)
                        poisonLevel = 1;

                    else if (poisonChance <= .95)
                        poisonLevel = 2;

                    else
                        poisonLevel = 3;
                    
                    Poison poison = Poison.GetPoison(poisonLevel);

                    from.SendMessage("You have been poisoned!");

                    from.FixedEffect(0x372A, 10, 30, 2208, 0);
                    Effects.PlaySound(from.Location, from.Map, 0x22F);
                  
                    from.ApplyPoison(from, poison);
                break;
            }

            m_TrapType = ScavengeTrapType.None;

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);
            player.m_UOACZAccountEntry.TrapsSprung++;             
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(from is PlayerMobile))
                return false;

            if (from.AccessLevel > AccessLevel.Player)
            {
                from.SendMessage("Using your godly powers you drop the item into the container.");

                return base.OnDragDrop(from, dropped);
            }

            return false;
        }

        public override void OnAfterDelete()
        {
            if (m_CorruptionItem != null)
            {
                if (!m_CorruptionItem.Deleted)
                    m_CorruptionItem.Delete();
            }

            base.OnAfterDelete();
        }

        private class InternalTimer : Timer
        {
            UOACZBaseScavengeObject m_BaseScavengeObject;

            public InternalTimer(UOACZBaseScavengeObject baseScavengeObject): base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.OneMinute;

                m_BaseScavengeObject = baseScavengeObject;
            }

            protected override void OnTick()
            {
                if (m_BaseScavengeObject == null)
                {
                    Stop();
                    return;
                }

                if (m_BaseScavengeObject.Deleted)
                {
                    Stop();
                    return;
                }

                if (DateTime.UtcNow >= m_BaseScavengeObject.m_Expiration)
                {
                    Stop();
                    m_BaseScavengeObject.Delete();

                    return;
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_InteractionRange);
            writer.Write(m_StartingYieldCount);
            writer.Write(m_MaxPlayerInteractions);
            writer.Write((int)m_TrapType);
            writer.Write(m_TrapDifficulty);
            writer.Write(m_Locked);
            writer.Write(m_LockDifficulty);
            writer.Write(m_HitPoints);
            writer.Write(m_MaxHitPoints);
            writer.Write(m_HitSound);
            writer.Write(m_BreakSound);
            writer.Write(m_ScavengeDifficulty);
            writer.Write(m_TrapResolveChance);
            writer.Write(m_Expiration);
            writer.Write(m_Corrupted);
            writer.Write(m_CorruptionItem);

            writer.Write(m_Interactors.Count);
            foreach (KeyValuePair<PlayerMobile, int> entry in m_Interactors)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value);
            }

            writer.Write(m_TrapImmunePlayers.Count);
            foreach (KeyValuePair<PlayerMobile, int> entry in m_TrapImmunePlayers)
            {
                writer.Write(entry.Key);
                writer.Write(entry.Value);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_InteractionRange = reader.ReadInt();
                m_StartingYieldCount = reader.ReadInt();
                m_MaxPlayerInteractions = reader.ReadInt();
                m_TrapType = (ScavengeTrapType)reader.ReadInt();
                m_TrapDifficulty = reader.ReadInt();
                m_Locked = reader.ReadBool();
                m_LockDifficulty = reader.ReadInt();
                m_HitPoints = reader.ReadInt();
                m_MaxHitPoints = reader.ReadInt();
                m_HitSound = reader.ReadInt();
                m_BreakSound = reader.ReadInt();
                m_ScavengeDifficulty = reader.ReadInt();
                m_TrapResolveChance = reader.ReadDouble();
                m_Expiration = reader.ReadDateTime();
                m_Corrupted = reader.ReadBool();
                m_CorruptionItem = reader.ReadItem();

                int interactors = reader.ReadInt();
                for (int a = 0; a < interactors; a++)
                {
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int interactions = reader.ReadInt();

                    m_Interactors.Add(player, interactions);
                }

                int trapImmunePlayers = reader.ReadInt();
                for (int a = 0; a < trapImmunePlayers; a++)
                {
                    PlayerMobile player = (PlayerMobile)reader.ReadMobile();
                    int interactions = reader.ReadInt();

                    m_TrapImmunePlayers.Add(player, interactions);
                }
            }

            //-------------

            m_Timer = new InternalTimer(this);
            m_Timer.Start();
        }
    }
}