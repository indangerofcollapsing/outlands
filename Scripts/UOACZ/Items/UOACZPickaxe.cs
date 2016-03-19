using System;
using Server.Items;
using Server.Network;
using Server.Engines.Harvest;
using Server.Mobiles;
using Server.Targeting;
using Server.Custom;
using Server.Spells;

namespace Server.Items
{
    [FlipableAttribute(0xE86, 0xE85)]
    public class UOACZPickaxe : BaseAxe, IUsesRemaining
    {
        public override HarvestSystem HarvestSystem { get { return Mining.System; } }

        public override WeaponAbility PrimaryAbility { get { return WeaponAbility.DoubleStrike; } }
        public override WeaponAbility SecondaryAbility { get { return WeaponAbility.Disarm; } }

        public override int AosStrengthReq { get { return 50; } }
        public override int AosMinDamage { get { return 13; } }
        public override int AosMaxDamage { get { return 15; } }
        public override int AosSpeed { get { return 35; } }
        public override float MlSpeed { get { return 3.00f; } }

        public override int OldStrengthReq { get { return 1; } }
        public override int OldMinDamage { get { return 5; } }
        public override int OldMaxDamage { get { return 10; } }
        public override int OldSpeed { get { return 40; } }

        public override int InitMinHits { get { return 31; } }
        public override int InitMaxHits { get { return 60; } }

        public override WeaponAnimation DefAnimation { get { return WeaponAnimation.Slash1H; } }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Durability : {0}", this.UsesRemaining);
            LabelTo(from, "pick axe");
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (!UOACZSystem.IsUOACZValidMobile(player)) return;
            if (!player.IsUOACZHuman) return;

            Item oneHand = from.FindItemOnLayer(Layer.OneHanded);
            Item firstValid = from.FindItemOnLayer(Layer.FirstValid);

            if (!(oneHand == this || firstValid == this))
            {
                from.SendMessage("You must equip this pickaxe in order to use it.");
                return;
            }

            if (!player.CanBeginAction(typeof(UOACZBaseScavengeObject)))
            {
                player.SendMessage("You must wait a moment before using that.");
                return;
            }

            from.SendMessage("Target the rock formation to mine.");
            from.Target = new UOACZMiningTarget(this);
        }

        public class UOACZMiningTarget : Target
        {
            private UOACZPickaxe m_UOACZPickaxe;

            public UOACZMiningTarget(UOACZPickaxe pickaxe): base(15, true, TargetFlags.None)
            {
                m_UOACZPickaxe = pickaxe;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_UOACZPickaxe.Deleted || m_UOACZPickaxe.RootParent != from) return;
                PlayerMobile player = from as PlayerMobile;

                if (!UOACZSystem.IsUOACZValidMobile(player)) return;
                if (!player.IsUOACZHuman) return;

                if (!player.CanBeginAction(typeof(UOACZBaseScavengeObject)))
                {
                    player.SendMessage("You must wait a moment before using that.");
                    return;
                }

                Item oneHand = from.FindItemOnLayer(Layer.OneHanded);
                Item firstValid = from.FindItemOnLayer(Layer.FirstValid);

                if (!(oneHand == m_UOACZPickaxe || firstValid == m_UOACZPickaxe))
                {
                    from.SendMessage("You must equip this pickaxe in order to use it.");
                    return;
                }              

                IPoint3D targetLocation = target as IPoint3D;

                if (targetLocation == null)
                    return;

                Map map = player.Map;

                if (map == null)
                    return;

                SpellHelper.GetSurfaceTop(ref targetLocation);

                Point3D location = new Point3D(targetLocation);

                UOACZScavengeOre scavengeOre = null;

                IPooledEnumerable nearbyItems = map.GetItemsInRange(location, 1);

                foreach (Item item in nearbyItems)
                {
                    if (item is UOACZScavengeOre)
                    {
                        scavengeOre = item as UOACZScavengeOre;
                        break;
                    }
                }

                nearbyItems.Free();

                if (target is UOACZScavengeOre)
                    scavengeOre = target as UOACZScavengeOre;

                if (scavengeOre != null)
                {
                    if (Utility.GetDistance(player.Location, scavengeOre.Location) > scavengeOre.InteractionRange)
                    {
                        from.SendMessage("You are too far away to use that.");
                        return;
                    }

                    int interactionCount = scavengeOre.GetInteractions(player);

                    if (scavengeOre.YieldsRemaining == 0 || interactionCount >= scavengeOre.MaxPlayerInteractions)
                    {
                        player.SendMessage(scavengeOre.NoYieldRemainingText);
                        return;
                    }

                    scavengeOre.Interact(player);
                }

                else
                {
                    from.SendMessage("The earth here is brittle and you must target a large ore formation in order to mine any significant amounts of ore.");
                    return;
                }
            }
        }

        public override WeaponAnimation GetAnimation()
        {
            WeaponAnimation animation = WeaponAnimation.Slash1H;

            Mobile attacker = this.Parent as Mobile;

            if (attacker != null)
            {
                switch (Utility.RandomMinMax(1, 4))
                {
                    case 1: animation = WeaponAnimation.Bash1H; break;
                    case 2: animation = WeaponAnimation.Bash2H; break;
                    case 3: animation = WeaponAnimation.Slash1H; break;
                    case 4: animation = WeaponAnimation.Slash2H;break;
                }
            }

            return animation;
        }

        public static int MaxUses = 50;

        [Constructable]
        public UOACZPickaxe(): base(0xE86)
        {
            Weight = 1.0;

            UsesRemaining = MaxUses;

            ShowUsesRemaining = true;
        }

        public UOACZPickaxe(Serial serial): base(serial)
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

            ShowUsesRemaining = true;
        }
    }
}