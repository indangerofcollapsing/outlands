/***************************************************************************
 *                              LobsterTrap.cs
 *                            ------------------
 *   begin                : February 2011
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Items;
using Server.Engines.Harvest;
using System.Collections.Generic;
using Server.ContextMenus;

namespace Server.Items
{
	public class LobsterTrap : Item
	{
        private List<Type> m_Contents = new List<Type>(MaxItemCount);

        public static readonly int MaxItemCount = 5; // Maximum number of items a single trap can hold
        public override double DefaultWeight { get { return 2.0; } }
        public LobsterBuoy Buoy { get; set; }
        public List<Type> Contents { get { return m_Contents; } set { m_Contents = value; } }
        public HarvestTimer Timer { get; set; }

        public static readonly int EmptyItemID = 0x44D0;
        public static readonly int FullItemID = 0x44CF;


        public bool Using { 
            get {
                return (!Movable && !Visible);
            }
            set {
                if (value) {
                    Movable = Visible = false;
                }
                else {
                    Movable = Visible = true;
                }
            }
        }

        [Constructable]
        public LobsterTrap(): this(1) {}

		[Constructable]
		public LobsterTrap(int amount) : base( EmptyItemID )
		{
            Name = "Lobster Cage";
            Stackable = true;
            Amount = amount;
		}

        public override bool StackWith(Mobile from, Item dropped, bool playSound)
        {
            if (!(dropped is LobsterTrap))
                return false;

            LobsterTrap dropTrap = dropped as LobsterTrap;

            if ((Contents.Count > 0) || dropTrap.Contents.Count > 0)
                return false;

            return base.StackWith(from, dropped, playSound);
        }

		public override void OnDoubleClick( Mobile from )
		{
            if (Using)
                return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1080058); // This must be in your backpack for you to use it.
                return;
            }

            if (Contents.Count > 0)
            {
                ExtractContents(from);

                if (!from.Backpack.TryDropItem(from, this, false))
                    MoveToWorld(from.Location, from.Map);

                from.SendMessage("You empty the contents of the cage into your backpack.");
            }
            else
            {
                LobsterFishing.System.BeginHarvesting(from, this);
            }
		}

        public void ExtractContents(Mobile from)
        {
            Container cont = from.Backpack;

            if (cont == null)
                return;

            foreach (Type type in Contents)
            {
                Item i = LobsterFishing.System.Construct(type, from, null, null, null, null);

                if (!cont.TryDropItem(from, i, true))
                    i.MoveToWorld(from.Location, from.Map);
            }

            Contents.Clear();

            ItemID = EmptyItemID;
            Name = "Empty Lobster Cage";
        }

        public LobsterTrap(Serial serial)
            : base(serial)
		{
		}

        public void Catch(Type item)
        {
            if (m_Contents.Count == MaxItemCount)
                return;

            Contents.Add(item);

            if (Buoy != null && Utility.RandomBool())
                Buoy.Bob();

            if (ItemID == FullItemID)
                return;

            ItemID = FullItemID;
            Name = "Full Lobster Cage";
        }

        public void BeginHarvest(LobsterBuoy buoy)
        {
            Using = true;
            Buoy = buoy;
        }

        public void EndHarvest(Mobile from)
        {
            if (Buoy != null && !Buoy.Deleted)
                Buoy.Delete();

            if (Timer != null)
            {
                Timer.Stop();
                Timer = null;
            }

            Using = false;

            if (from == null)
                Delete();
            else if (!from.AddToBackpack(this))
            {
                MoveToWorld(from.Location, from.Map);
                GainSkills(from);
            }
        }

        public void GainSkills(Mobile from)
        {
            foreach (Type type in Contents)
            {
                for (int i = 0; i < LobsterFishing.System.Definition.Resources.Length; i++)
                {
                    var resource = LobsterFishing.System.Definition.Resources[i];
                    var types = resource.Types;

                    for (int j = 0; j < types.Length; j++)
                        if (type == types[j])
                            from.CheckSkill(LobsterFishing.System.Definition.Skill, resource.MinSkill, resource.MaxSkill, 1.0);
                }
           }
        }

        public void SinkTrap()
        {
            if (Buoy != null)
            {
                Effects.SendLocationEffect(Buoy.Location, Buoy.Map, 0x352D, 16, 4);
                Effects.PlaySound(Buoy.Location, Buoy.Map, 0x364);
            }

            EndHarvest(null);
            Delete();
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (Using)
                Server.Timer.DelayCall(TimeSpan.FromTicks(1), Delete);
		}
	}
}