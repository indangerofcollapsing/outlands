using System;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class AquariumGump : Gump
	{
		public Aquarium m_Aquarium;
        
        public int m_FishItemCount = 0;
        public int m_DecorationItemCount = 0;
        public int m_TotalItemCount = 0;

        public int m_ItemIndex = 0;

		public AquariumGump( Aquarium aquarium, Mobile from, int index) : base( 10, 10 )
		{
            if (aquarium == null || from == null)
                return;

            if (aquarium.Deleted || from.Deleted || index < 0)
                return;

			m_Aquarium = aquarium;
            m_ItemIndex = index;

            m_FishItemCount = m_Aquarium.m_FishItems.Count;
            m_DecorationItemCount = m_Aquarium.m_DecorationItems.Count;
            m_TotalItemCount = m_FishItemCount + m_DecorationItemCount;

			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;

            AddImage(0, 0, 11414);
            AddImage(315, 0, 11414);            

            List<Point2D> m_FishLocations = new List<Point2D>();
            List<Point2D> m_DecorationLocations = new List<Point2D>();

            //Fish Spots
            m_FishLocations.Add(new Point2D(432, 62));
            m_FishLocations.Add(new Point2D(153, 87));
            m_FishLocations.Add(new Point2D(574, 31));
            m_FishLocations.Add(new Point2D(432, 96));
            m_FishLocations.Add(new Point2D(240, 95));
            m_FishLocations.Add(new Point2D(348, 35));            
            m_FishLocations.Add(new Point2D(562, 63));            
            m_FishLocations.Add(new Point2D(277, 22));
            m_FishLocations.Add(new Point2D(241, 63));
            m_FishLocations.Add(new Point2D(361, 86));
            m_FishLocations.Add(new Point2D(15, 15));
            m_FishLocations.Add(new Point2D(134, 57));
            m_FishLocations.Add(new Point2D(83, 34)); 
            m_FishLocations.Add(new Point2D(117, 27));
            m_FishLocations.Add(new Point2D(159, 28));
            m_FishLocations.Add(new Point2D(99, 95));
            m_FishLocations.Add(new Point2D(228, 36));            
            m_FishLocations.Add(new Point2D(427, 24));            
            m_FishLocations.Add(new Point2D(563, 110));
            m_FishLocations.Add(new Point2D(68, 63));

            //Decoration Locations
            m_DecorationLocations.Add(new Point2D(336, 116));
            m_DecorationLocations.Add(new Point2D(530, 135));
            m_DecorationLocations.Add(new Point2D(164, 139));
            m_DecorationLocations.Add(new Point2D(201, 115));
            m_DecorationLocations.Add(new Point2D(583, 120));
            m_DecorationLocations.Add(new Point2D(256, 132));
            m_DecorationLocations.Add(new Point2D(20, 144));
            m_DecorationLocations.Add(new Point2D(92, 127));
            m_DecorationLocations.Add(new Point2D(439, 121));
            m_DecorationLocations.Add(new Point2D(383, 139));

            for (int a = 0; a < aquarium.m_FishItems.Count; a++)
            {
                AquariumItem item = aquarium.m_FishItems[a]; 
                
                Point2D point = m_FishLocations[a];
                AddItem(point.X + item.OffsetX, point.Y + item.OffsetY, item.ItemID, item.Hue);
            }

            //Decoration
            AddItem(597, 31, 3377, 2600);
            AddItem(296, 134, 3365, 2600);
            AddItem(458, 14, 3302, 2600);
            AddItem(32, 66, 3314, 2600);
            AddItem(273, 11, 3320, 2600);
            AddItem(493, 129, 3367, 2600);
            AddItem(389, 16, 3313, 2600);
            AddItem(58, 131, 3366, 2600);
            AddItem(400, 130, 3367, 2600);
            AddItem(58, 135, 3379, 2600);
            AddItem(193, 41, 3376, 2600);
            AddItem(118, 123, 3374, 2600);
            AddItem(574, 140, 3372, 2600);
            AddItem(420, 117, 3379, 2600);

            for (int a = 0; a < aquarium.m_DecorationItems.Count; a++)
            {
                AquariumItem item = aquarium.m_DecorationItems[a];

                Point2D point = m_DecorationLocations[a];
                AddItem(point.X + item.OffsetX, point.Y + item.OffsetY, item.ItemID, item.Hue);                
            }

            int textHue = 2036;

            int rarityTextHue = textHue;

            if (m_ItemIndex < m_TotalItemCount && m_TotalItemCount > 0)
            {
                AddImage(79, 224, 103); //Wood Background

                AddImage(242, 186, 63); //Basket
                AddItem(212, 219, 3520);
                AddItem(275, 260, 3922);
                AddItem(250, 260, 3581);

                AquariumItem item;
                Point2D point;

                int iStartX = 150;
                int iStartY = 235;

                //Fish
                if (m_ItemIndex < m_FishItemCount)
                {
                    item = m_Aquarium.m_FishItems[m_ItemIndex];
                    point = m_FishLocations[m_ItemIndex];

                    if (item != null)
                    {
                        rarityTextHue = AquariumItem.GetRarityTextColor(item.ItemRarity);

                        AddItem(320 + item.OffsetX, 250 + item.OffsetY, item.ItemID, item.Hue); //Fish

                        AddLabel(iStartX - (item.DescriptionA.Length * 3), iStartY, textHue, item.DescriptionA);
                        iStartY += 20;

                        AddLabel(iStartX - (item.DescriptionB.Length * 3), iStartY, textHue, item.DescriptionB);
                        if (item.DescriptionB != "")
                            iStartY += 20;

                        string rarityText = AquariumItem.GetRarityTextName(item.ItemRarity);

                        AddLabel(iStartX - (rarityText.Length * 3), iStartY, rarityTextHue, rarityText);
                        iStartY += 20;

                        string weightText = item.Weight.ToString() + " lbs";

                        AddLabel(iStartX - (weightText.Length * 3), iStartY, textHue, weightText);
                        iStartY += 20;

                        AddImage(point.X - 18, point.Y - 30, 30063); //Pointer
                    }
                }

                //Decoration
                else
                {
                    item = m_Aquarium.m_DecorationItems[m_ItemIndex - m_FishItemCount];
                    point = m_DecorationLocations[m_ItemIndex - m_FishItemCount];

                    if (item != null)
                    {
                        rarityTextHue = AquariumItem.GetRarityTextColor(item.ItemRarity);

                        AddItem(320 + item.OffsetX, 250 + item.OffsetY, item.ItemID, item.Hue); //Decoration

                        AddLabel(iStartX - (item.DescriptionA.Length * 3), iStartY, textHue, item.DescriptionA);
                        iStartY += 20;

                        AddLabel(iStartX - (item.DescriptionB.Length * 3), iStartY, textHue, item.DescriptionB);
                        if (item.DescriptionB != "")
                            iStartY += 20;

                        string rarityText = AquariumItem.GetRarityTextName(item.ItemRarity);

                        AddLabel(iStartX - (rarityText.Length * 3), iStartY, rarityTextHue, rarityText);
                        iStartY += 20;

                        AddImage(point.X - 18, point.Y - 30, 30063); //Pointer
                    }
                }

                if (m_Aquarium.HasAccess(from))
                {
                    AddButton(456, 296, 2474, 2472, 3, GumpButtonType.Reply, 0);
                    AddLabel(490, 298, textHue, "Remove");
                }
            }

            if (m_ItemIndex > 0)
            {
                AddButton(455, 226, 4014, 4016, 1, GumpButtonType.Reply, 0);
                AddLabel(489, 229, textHue, "Previous");
            }

            if (m_ItemIndex < m_TotalItemCount - 1)
            {
                AddButton(455, 261, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(490, 264, textHue, "Next");
            }            
		}        

		public override void OnResponse( NetState sender, RelayInfo info )
		{
            Mobile from = sender.Mobile;

            if (from == null) return;
            if (from.Deleted) return;
            if (m_Aquarium == null) return;
            if (m_Aquarium.Deleted) return;

            //Aquarium Was Modified By Other Player 
            if (m_FishItemCount != m_Aquarium.m_FishItems.Count || m_DecorationItemCount != m_Aquarium.m_DecorationItems.Count)
            {
                from.SendMessage("The contents of the aquarium have changed.");

                from.CloseGump(typeof(AquariumGump));
                from.SendGump(new AquariumGump(m_Aquarium, from, 0));
            }
            
            //Previous
            if (info.ButtonID == 1)
            {
                from.SendSound(0x5AF);

                m_ItemIndex--;

                from.CloseGump(typeof(AquariumGump));
                from.SendGump(new AquariumGump(m_Aquarium, from, m_ItemIndex));

                return;
            }

            //Next
            if (info.ButtonID == 2)
            {
                from.SendSound(0x5AF);

                m_ItemIndex++;

                from.CloseGump(typeof(AquariumGump));
                from.SendGump(new AquariumGump(m_Aquarium, from, m_ItemIndex));

                return;
            }

            //Remove
            if (info.ButtonID == 3)
            {
                AquariumItem item;

                bool itemRemoved = false;

                //Fish
                if (m_ItemIndex < m_FishItemCount)
                {   
                    item = m_Aquarium.m_FishItems[m_ItemIndex];                    

                    if (from.AddToBackpack(item))
                    {
                        m_Aquarium.m_FishItems.RemoveAt(m_ItemIndex);

                        from.SendSound(0x5A4);
                        m_Aquarium.Splash();

                        from.SendMessage("You remove the item from the aquarium and place it in your backpack.");

                        itemRemoved = true;
                    }

                    else                    
                        from.SendMessage("Your backpack does not have enough space to hold that item.");                    
                }

                else
                {
                    int decorationIndex = m_ItemIndex - m_Aquarium.m_FishItems.Count;
                    item = m_Aquarium.m_DecorationItems[decorationIndex];

                    if (from.AddToBackpack(item))
                    {
                        m_Aquarium.m_DecorationItems.RemoveAt(decorationIndex);

                        from.SendSound(0x5A4);
                        m_Aquarium.Splash();

                        from.SendMessage("You remove the item from the aquarium and place it in your backpack.");

                        itemRemoved = true;
                    }

                    else
                        from.SendMessage("Your backpack does not have enough space to hold that item.");    
                }

                if (itemRemoved)
                {
                    m_FishItemCount = m_Aquarium.m_FishItems.Count;
                    m_DecorationItemCount = m_Aquarium.m_DecorationItems.Count;
                    m_TotalItemCount = m_FishItemCount + m_DecorationItemCount;

                    if (m_ItemIndex > m_TotalItemCount - 1 && m_TotalItemCount > 0)
                        m_ItemIndex--;
                }

                from.CloseGump(typeof(AquariumGump));
                from.SendGump(new AquariumGump(m_Aquarium, from, m_ItemIndex));

                return;
            }
		}
	}
}
