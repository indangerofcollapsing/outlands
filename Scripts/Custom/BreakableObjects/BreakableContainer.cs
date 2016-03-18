using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class BreakableContainer : MetalChest
    {
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

        private double m_LockpickDamageScalar = 1.0;
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

        private double m_LumberjackingDamageScalar = 0.33;
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
            set { m_HitPoints = value; }
        }

        private int m_MaxHitPoints = 1000;
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

        private int m_HalfwaySound = 0x3B7;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HalfwaySound
        {
            get { return m_HalfwaySound; }
            set { m_HalfwaySound = value; }
        }

        private int m_BreakSound = 0x142;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BreakSound
        {
            get { return m_BreakSound; }
            set { m_BreakSound = value; }
        }

        private int m_HalfwayDamagedItemId = -1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HalfwayDamagedItemId
        {
            get { return m_HalfwayDamagedItemId; }
            set { m_HalfwayDamagedItemId = value; }
        }

        private int m_HalfwayDamagedHue = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int HalfwayDamagedHue
        {
            get { return m_HalfwayDamagedHue; }
            set { m_HalfwayDamagedHue = value; }
        }

        private string m_TimedStaticOnBreakName = "";
        [CommandProperty(AccessLevel.GameMaster)]
        public string TimedStaticOnBreakName
        {
            get { return m_TimedStaticOnBreakName; }
            set { m_TimedStaticOnBreakName = value; }
        }

        private int m_TimedStaticOnBreakItemId = -1;
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
        public BreakableContainer(): base()
        {
            Name = "";
            Movable = false;

            ItemID = 2475;

            Visible = true;
        }

        public BreakableContainer(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (from.NetState != null)
                LabelTo(from, Name);

            if (m_HitPoints > 0)
                LabelTo(from, "[Durability " + m_HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!(from is PlayerMobile))
                return;

            if (!from.InRange(this.GetWorldLocation(), m_InteractionRange))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
                return;
            }

            if (from.AccessLevel > AccessLevel.Player)
            {
                from.SendMessage("Using your godly powers you open the container.");
                Open(from);

                return;
            }

            if (m_HitPoints > 0)
            {
                InteractionType interactionType = InteractionType.None;

                double highestScalar = 0;

                if (InteractDamageScalar > 0)
                {
                    highestScalar = InteractDamageScalar;
                    interactionType = InteractionType.Normal;
                }

                BaseWeapon weapon = from.Weapon as BaseWeapon;
                if (weapon != null)
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

                    if (m_MiningDamageScalar > 0 && m_MiningDamageScalar > highestScalar && (weapon is Pickaxe || weapon is SturdyPickaxe || weapon is DiamondPickaxe))
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

        public void Interact(Mobile from, InteractionType interactionType)
        {
            if (from == null) return;

            if (!from.CanBeginAction(typeof(BreakableStatic)))
            {
                from.SendMessage("You must wait a few moments before attempting to use that again.");
                return;
            }

            from.RevealingAction();

            BeforeInteract(from, interactionType);

            double usageDelay = 0;

            BaseWeapon weapon = from.Weapon as BaseWeapon;

            switch (interactionType)
            {
                case InteractionType.None:
                    from.SendMessage("You cannot interact with that in that manner.");
                    return;
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

            from.BeginAction(typeof(BreakableStatic));

            Timer.DelayCall(TimeSpan.FromSeconds(usageDelay), delegate
            {
                if (from != null)
                    from.EndAction(typeof(BreakableStatic));
            });

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

                    if (weapon != null && !(weapon is Fists))
                    {
                        int minDamage = weapon.MinDamage;
                        int maxDamage = weapon.MaxDamage;

                        if (weapon is Pickaxe || weapon is Hatchet)
                        {
                            minDamage = 10;
                            maxDamage = 20;
                        }

                        if (minDamage < m_MinInteractDamage)
                            minDamage = m_MinInteractDamage;

                        if (maxDamage < m_MaxInteractDamage)
                            maxDamage = m_MaxInteractDamage;

                        damage = (int)(Math.Round((double)(Utility.RandomMinMax(weapon.MinDamage, weapon.MaxDamage)) * WeaponDamageScalar));
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
                    weapon = from.Weapon as BaseWeapon;

                    if (weapon != null && weapon is Pickaxe)
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

        public void ReceiveDamage(Mobile from, int damage, InteractionType interactionType)
        {
            bool wasAboveHalf = (((double)m_HitPoints / (double)m_MaxHitPoints) >= .5);

            m_HitPoints -= damage;

            bool isAboveHalf = (((double)m_HitPoints / (double)m_MaxHitPoints) >= .5);

            if (m_HitPoints < 0)
                m_HitPoints = 0;

            if (m_HitPoints == 0)
            {
                Effects.PlaySound(Location, Map, m_BreakSound);

                if (TimedStaticOnBreakItemId != -1)
                {
                    TimedStatic timedStatic = new TimedStatic(TimedStaticOnBreakItemId, TimedStaticDuration);
                    timedStatic.Name = TimedStaticOnBreakName;
                    timedStatic.Hue = TimedStaticOnBreakHue;
                    timedStatic.MoveToWorld(Location, Map);
                }

                BeforeBreak(from, interactionType);

                PublicOverheadMessage(MessageType.Regular, 0, false, "*breaks open*");

                if (from != null)
                {
                    if (from.InRange(GetWorldLocation(), m_InteractionRange))
                        Open(from);
                }

                AfterBreak(from, interactionType);
            }

            else
            {
                if (wasAboveHalf && !isAboveHalf)
                {
                    if (ItemID != HalfwayDamagedItemId)
                        ItemID = HalfwayDamagedItemId;

                    if (Hue != m_HalfwayDamagedHue)
                        Hue = m_HalfwayDamagedHue;

                    Effects.PlaySound(Location, Map, m_HalfwaySound);

                    AfterHalfway(from, interactionType);
                }

                from.SendMessage("You inflict " + damage.ToString() + " damage upon your target. [Remaining durability: " + m_HitPoints.ToString() + "/" + m_MaxHitPoints.ToString() + "]");
            }
        }

        public virtual void BeforeInteract(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void AfterInteract(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void BeforeHalfway(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void AfterHalfway(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void BeforeBreak(Mobile from, InteractionType interactionType)
        {
        }

        public virtual void AfterBreak(Mobile from, InteractionType interactionType)
        {
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(from is PlayerMobile))
                return false;

            if (m_HitPoints > 0)
            {
                if (from.AccessLevel > AccessLevel.Player)
                {
                    from.SendMessage("Using your godly powers you drop the item into the container.");

                    return base.OnDragDrop(from, dropped);
                }

                else
                {
                    from.SendMessage("That container is still sealed.");
                    return false;
                }
            }

            return base.OnDragDrop(from, dropped);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

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
            writer.Write(m_HitSound);
            writer.Write(m_HalfwaySound);
            writer.Write(m_BreakSound);
            writer.Write(m_HalfwayDamagedItemId);
            writer.Write(m_HalfwayDamagedHue);
            writer.Write(m_TimedStaticOnBreakItemId);
            writer.Write(m_TimedStaticOnBreakHue);
            writer.Write(m_TimedStaticDuration);
            writer.Write(m_TimedStaticOnBreakName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
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
                m_HitPoints = reader.ReadInt();
                m_MaxHitPoints = reader.ReadInt();
                m_HitSound = reader.ReadInt();
                m_HalfwaySound = reader.ReadInt();
                m_BreakSound = reader.ReadInt();
                m_HalfwayDamagedItemId = reader.ReadInt();
                m_HalfwayDamagedHue = reader.ReadInt();
                m_TimedStaticOnBreakItemId = reader.ReadInt();
                m_TimedStaticOnBreakHue = reader.ReadInt();
                m_TimedStaticDuration = reader.ReadDouble();
                m_TimedStaticOnBreakName = reader.ReadString();
            }
        }
    }
}