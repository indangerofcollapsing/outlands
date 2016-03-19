using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Multis;
using Server.Prompts;

namespace Server.Items
{
    public class ContainerRenameDeed : Item
    {
        private int m_Charges = 5;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; }
        }

        [Constructable]
        public ContainerRenameDeed(): base(0x14F0)
        {
            Name = "a container rename deed";
            Hue = 2599;
        }

        public ContainerRenameDeed(Serial serial): base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            LabelTo(from, "[charges: " + Charges.ToString() + "]");
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target a container to rename.");
            from.Target = new ContainerTarget(this);
        }

        public class ContainerTarget : Target
        {
            private IEntity targetLocation;
            private ContainerRenameDeed m_ContainerRenameDeed;

            public ContainerTarget(ContainerRenameDeed containerRenameDeed): base(25, false, TargetFlags.None)
            {
                m_ContainerRenameDeed = containerRenameDeed;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null) return;
                if (player.Deleted || !player.Alive) return;

                BaseContainer container = target as BaseContainer;

                if (container == null)
                {
                    player.SendMessage("That is not a container.");
                    return;
                }

                if (container is BaseAddonContainer || container == player.Backpack)
                {
                    player.SendMessage("That cannot be renamed.");
                    return;
                }

                bool validRename = false;

                //In Player's Backpack or Bank
                if (container.IsChildOf(player.Backpack) || container.IsChildOf(player.BankBox))
                    validRename = true;

                if (!validRename && (Utility.GetDistance(player.Location, container.Location) > 2))
                {
                    player.SendMessage("That container is too far away.");
                    return;
                }

                if (!validRename && !player.Map.InLOS(player.Location, container.Location))
                {
                    player.SendMessage("That container is not within in your line of sight.");
                    return;
                }

                //In Valid House
                if (container.IsLockedDown || container.IsSecure)
                {
                    BaseHouse house = BaseHouse.FindHouseAt(container.Location, from.Map, 64);

                    if (house == null)
                        return;

                    if (house.Owner != player)
                    {
                        from.SendMessage("Only the owner of this house may renamed a locked down or secure container within it.");
                        return;
                    }

                    validRename = true;
                }

                if (!validRename)
                {
                    player.SendMessage("You may only renamed containers in your pack, bank, or locked down in a house you own.");
                    return;
                }

                player.SendMessage("Please enter a description for this container.");
                player.Prompt = new RenamePrompt(m_ContainerRenameDeed, container);
            }
        }

        private class RenamePrompt : Prompt
        {
            private ContainerRenameDeed m_RenameDeed;
            private BaseContainer m_Container;

            public RenamePrompt(ContainerRenameDeed renameDeed, BaseContainer container)
            {
                m_RenameDeed = renameDeed;
                m_Container = container;
            }

            public override void OnResponse(Mobile from, string text)
            {
                PlayerMobile player = from as PlayerMobile;

                if (player == null) return;
                if (player.Deleted || !player.Alive) return;

                if (m_RenameDeed == null) return;
                if (m_RenameDeed.Deleted) return;

                if (m_Container == null) return;
                if (m_Container.Deleted) return;

                bool validRename = false;

                //In Player's Backpack or Bank
                if (m_Container.IsChildOf(player.Backpack) || m_Container.IsChildOf(player.BankBox))
                    validRename = true;

                if (!validRename && (Utility.GetDistance(player.Location, m_Container.Location) > 2))
                {
                    player.SendMessage("That container is too far away.");
                    return;
                }

                if (!validRename && !player.Map.InLOS(player.Location, m_Container.Location))
                {
                    player.SendMessage("That container is not within in your line of sight.");
                    return;
                }

                //In Valid House
                if (m_Container.IsLockedDown || m_Container.IsSecure)
                {
                    BaseHouse house = BaseHouse.FindHouseAt(m_Container.Location, from.Map, 64);

                    if (house == null)
                        return;

                    if (house.Owner != player)
                    {
                        from.SendMessage("Only the owner of this house may renamed a locked down or secure container within it.");
                        return;
                    }

                    validRename = true;
                }

                if (!validRename)
                {
                    player.SendMessage("You may only renamed containers in your pack, bank, or locked down in a house you own.");
                    return;
                }

                m_Container.Name = text;
                player.SendMessage("The name on the container has been changed.");

                m_RenameDeed.Charges--;

                if (m_RenameDeed.Charges <= 0)
                {
                    player.SendMessage("You use the last charge on the deed.");
                    m_RenameDeed.Delete();
                }
            }
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