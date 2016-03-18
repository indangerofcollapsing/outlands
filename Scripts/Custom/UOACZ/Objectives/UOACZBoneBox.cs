using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;

namespace Server.Custom
{
    public class UOACZBoneBox : MetalChest
    {
        public static List<UOACZBoneBox> m_Instances = new List<UOACZBoneBox>();

        public static int m_DropSound = 0x3BD;

        private int m_BoneCount = 0;
        [CommandProperty(AccessLevel.GameMaster)]
        public int BoneCount
        {
            get { return m_BoneCount; }
            set { m_BoneCount = value; }
        }

        [Constructable]
        public UOACZBoneBox(): base()
        {
            Name = "bone collection chest";
            ItemID = 11763;
            Hue = 2500;            

            Movable = false;

            m_Instances.Add(this);
        }

        public UOACZBoneBox(Serial serial): base(serial)
        {
        }

        public static List<UOACZBoneBox> GetActiveInstances()
        {
            List<UOACZBoneBox> m_ActiveInstances = new List<UOACZBoneBox>();

            for (int a = 0; a < m_Instances.Count; a++)
            {
                UOACZBoneBox instance = m_Instances[a];

                if (instance.Deleted)
                    continue;

                if (UOACZRegion.ContainsItem(instance))
                    m_ActiveInstances.Add(instance);
            }

            return m_ActiveInstances;
        }

        public override void OnSingleClick(Mobile from)
        {
            LabelTo(from, "Bone Collection Chest");
            LabelTo(from, "(" + m_BoneCount.ToString() + " / " + UOACZSystem.HumanObjectiveBonesNeeded.ToString() + ")");
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendMessage("Drop bones into this container to complete the human objective.");            
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return false;

            if (!player.IsUOACZHuman)
                return false;

            if (m_BoneCount >= UOACZSystem.HumanObjectiveBonesNeeded)
            {
                from.SendMessage("All the bones neccessary for this objective have been collected.");
            }

            if (!(dropped is Bone))
            {
                from.SendMessage("Only bones may be placed in this container.");
                return false;
            }

            int dropAmount = dropped.Amount;

            if ((m_BoneCount + dropAmount) > UOACZSystem.HumanObjectiveBonesNeeded)
            {
                dropAmount = UOACZSystem.HumanObjectiveBonesNeeded - m_BoneCount;

                dropped.Amount -= dropAmount;
                m_BoneCount += dropAmount;

                player.SendMessage("You deposit " + dropAmount.ToString() + " bones into the collection box. ");

                CheckScore(player, dropAmount);

                from.PlaySound(UOACZBoneBox.m_DropSound);

                UOACZEvents.CollectBones();               

                return false;
            }

            CheckScore(player, dropAmount);

            m_BoneCount += dropAmount;
            player.PlaySound(UOACZBoneBox.m_DropSound);

            player.SendMessage("You deposit " + dropAmount.ToString() + " bones into the collection box. " + (UOACZSystem.HumanObjectiveBonesNeeded - m_BoneCount).ToString() + " more are needed to complete the warding ritual.");

            UOACZEvents.CollectBones();

            return true;
        }

        public void CheckScore(PlayerMobile player, int amount)
        {
            if (player == null || amount == 0)
                return;

            int score = (int)(Math.Floor((double)amount / (double)UOACZSystem.HumanBonesPerScore));
            int remainder = amount % UOACZSystem.HumanBonesPerScore;

            if (Utility.RandomDouble() <= ((double)remainder / (double)UOACZSystem.HumanBonesPerScore))
                score++;

            if (score > 0)            
                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.HumanScore, score, true);            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_BoneCount);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_BoneCount = reader.ReadInt();

            //-------

            m_Instances.Add(this);
        }
    }
}