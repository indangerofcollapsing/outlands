using System;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;
using Server.Custom;

namespace Server.Items
{
    public class TransmutationStone : Item
    {
        [Constructable]
        public TransmutationStone() : base(6253)
        {
            Name = "transmutation stone";

            Hue = 2603;
            Weight = 1;
        }

        public TransmutationStone(Serial serial): base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }

            from.SendMessage("Target the dungeon armor you wish to transform to another type.");
            from.Target = new DungeonArmorTransformTarget(this);
        }

        public class DungeonArmorTransformTarget : Target
        {
            private TransmutationStone m_TransmutationStone;

            public DungeonArmorTransformTarget(TransmutationStone TransmutationStone): base(2, false, TargetFlags.None)
            {
                m_TransmutationStone = TransmutationStone;
            }

            protected override void OnTarget(Mobile from, object target)
            {
                if (m_TransmutationStone.Deleted || m_TransmutationStone.RootParent != from)     
                if (from == null) return;

                if (target is Item)
                {
                    Item item = target as Item;

                    if (!item.IsChildOf(from.Backpack))
                    {
                        from.SendMessage("You must target an item in your backpack.");
                        return;
                    }

                    if (!(item is BaseDungeonArmor))
                    {
                        from.SendMessage("That item is not a piece of dungeon armor.");
                        return;
                    }

                    BaseDungeonArmor dungeonArmor = item as BaseDungeonArmor;

                    from.CloseGump(typeof(DungeonArmorTransformGump));
                    from.SendGump(new DungeonArmorTransformGump(m_TransmutationStone, dungeonArmor, dungeonArmor.Dungeon));
                }                

                else
                {
                    from.SendMessage("That is not a piece of dungeon armor.");
                    return;
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

            //Version 0
            if (version >= 0)
            {               
            }
        }
    }

    public class DungeonArmorTransformGump : Gump
    {
        TransmutationStone m_TransmutationStone;
        BaseDungeonArmor m_BaseDungeonArmor;
        BaseDungeonArmor.DungeonEnum m_DungeonArmorType;

        public DungeonArmorTransformGump(TransmutationStone transmutationStone, BaseDungeonArmor baseDungeonArmor, BaseDungeonArmor.DungeonEnum dungeon): base(10, 10)
        {
            m_TransmutationStone = transmutationStone;
            m_BaseDungeonArmor = baseDungeonArmor;
            m_DungeonArmorType = dungeon;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            int textHue = 2036;
            int boldHue = 149;
            int inactiveHue = 2301;            

            BaseDungeonArmor.DungeonArmorDetail detail = new BaseDungeonArmor.DungeonArmorDetail(m_DungeonArmorType, m_BaseDungeonArmor.Tier);

            if (detail == null)
                return;

            int dungeonArmorHue = detail.Hue;

            AddImageTiled(0, 0, 282, 403, 103);

            AddImage(7, 14, 3604, 2052);
            AddImage(7, 139, 3604, 2052);
            AddImage(7, 266, 3604, 2052);
            AddImage(108, 17, 3604, 2052);
            AddImage(108, 140, 3604, 2052);
            AddImage(108, 264, 3604, 2052);
            AddImage(144, 14, 3604, 2052);
            AddImage(145, 140, 3604, 2052);
            AddImage(145, 266, 3604, 2052);    

            AddImage(121, 40, 50529, dungeonArmorHue - 1);
            AddImage(120, 41, 50527, dungeonArmorHue - 1);
            AddImage(122, 42, 50528, dungeonArmorHue - 1);
            AddImage(119, 41, 50530, dungeonArmorHue - 1);
            AddImage(120, 38, 50531, dungeonArmorHue - 1);
            AddImage(119, 37, 60563, dungeonArmorHue - 1);

            AddLabel(41, 16, 149, @"Change The Armor's Dungeon To");

            int startY = 55;
            int dungeonCount = Enum.GetNames(typeof(BaseDungeonArmor.DungeonEnum)).Length;

            for (int a = 1; a < dungeonCount; a++)
            {
                if (m_BaseDungeonArmor.Dungeon != (BaseDungeonArmor.DungeonEnum)a)
                {
                    if (m_DungeonArmorType == (BaseDungeonArmor.DungeonEnum)a)
                        AddButton(25, startY, 2154, 2151, a, GumpButtonType.Reply, 0);
                    else
                        AddButton(25, startY, 2151, 2154, a, GumpButtonType.Reply, 0);
                }

                else
                {
                    if (m_DungeonArmorType == (BaseDungeonArmor.DungeonEnum)a)
                        AddButton(25, startY, 9724, 9721, a, GumpButtonType.Reply, 0);
                    else
                        AddButton(25, startY, 9721, 9724, a, GumpButtonType.Reply, 0);
                }                    

                if (m_DungeonArmorType == (BaseDungeonArmor.DungeonEnum)a)
                    AddLabel(65, startY, boldHue, ((BaseDungeonArmor.DungeonEnum)a).ToString());
                else
                    AddLabel(65, startY, textHue, ((BaseDungeonArmor.DungeonEnum)a).ToString());                

                startY += 30;
            }

            AddLabel(104, 333, 149, "Transform");
            AddButton(104, 358, 247, 248, 20, GumpButtonType.Reply, 0);
        }
        
        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            if (from == null)
                return;

            int dungeonCount = Enum.GetNames(typeof(BaseDungeonArmor.DungeonEnum)).Length;

            if (info.ButtonID >= 1 && info.ButtonID <= dungeonCount - 1)
            {
                m_DungeonArmorType = (BaseDungeonArmor.DungeonEnum)info.ButtonID;

                from.CloseGump(typeof(DungeonArmorTransformGump));
                from.SendGump(new DungeonArmorTransformGump(m_TransmutationStone, m_BaseDungeonArmor, m_DungeonArmorType));

                return;
            }

            if (info.ButtonID == 20)
            {
                if (from == null) return;
                if (from.Deleted || !from.Alive) return;

                bool transmutationStoneValid = true;

                if (m_TransmutationStone == null)
                    transmutationStoneValid = false;

                else if (m_TransmutationStone.Deleted || !m_TransmutationStone.IsChildOf(from.Backpack))
                    transmutationStoneValid = false;

                if (!transmutationStoneValid)
                {
                    from.SendMessage("The transmutation stone you were using no longer exists or no longer is in your backpack.");
                    
                    return;
                }

                bool dungeonArmorValid = true;

                if (m_BaseDungeonArmor == null)
                    dungeonArmorValid = false;

                else if (m_BaseDungeonArmor.Deleted || !m_BaseDungeonArmor.IsChildOf(from.Backpack))
                    dungeonArmorValid = false;

                if (!dungeonArmorValid)
                {
                    from.SendMessage("The dungeon armor you were targeting no longer exists or no longer is in your backpack.");

                    return;
                }

                if (m_BaseDungeonArmor.Dungeon == m_DungeonArmorType)
                {
                    from.SendMessage("That dungeon armor is already that type.");

                    from.CloseGump(typeof(DungeonArmorTransformGump));
                    from.SendGump(new DungeonArmorTransformGump(m_TransmutationStone, m_BaseDungeonArmor, m_DungeonArmorType));

                    return;
                }

                m_BaseDungeonArmor.Dungeon = m_DungeonArmorType;

                from.PlaySound(0X64e);
                Effects.SendLocationParticles(EffectItem.Create(from.Location, from.Map, TimeSpan.FromSeconds(0.25)), 0x3709, 10, 30, m_BaseDungeonArmor.Hue - 1, 0, 5029, 0);

                from.SendMessage("You transmute the armor into it's new type.");

                m_TransmutationStone.Delete();
               
            }            
        }
    }
}