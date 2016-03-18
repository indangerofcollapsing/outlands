using System;
using System.Collections.Generic;
using Server;
using Server.Custom;
using Server.Gumps;

namespace Server.Items
{
    public class MasterSpellBook : Item
    {
        private String[] spellNames = {"Chain Lightning", "Energy Field", "Flamestrike", "Gate Travel", "Mana Vampire", "Mass Dispel", "Meteor Swarm", "Polymorph",
                                        "Earthquake", "Energy Vortex", "Resurrection", "Air Elemental", "Summon Daemon", "Earth Elemental", "Fire Elemental", "Water Elemental"
                                      };

        public class MasterSpell
        {
            public String Name;
            public int ID;
            public int Charges;

            public MasterSpell(String name, int id, int charges)
            {
                Name = name;
                ID = id;
                Charges = charges;
            }

        }

        public override string DefaultName { get { return "Master Spell Book"; } }

        private List<MasterSpell> m_Data;
        public List<MasterSpell> Data { get { return m_Data; } set { m_Data = value; } }

        [Constructable]
        public MasterSpellBook(): base(0xEFA)
        {
            m_Data = new List<MasterSpell>();
            LootType = Server.LootType.Blessed;
            Weight = 3.0;
            Layer = Layer.OneHanded;
            Hue = 1340;
        }

        public MasterSpell getMaster(int id)
        {
            foreach (MasterSpell master in m_Data)
            {
                if (master.ID == id)
                {
                    return master;
                }
            }
            return null;
        }

        public String getScrollName(int id)
        {
            return spellNames[id - 48];
        }

        public void AddMaster(Item item)
        {
            var scroll = item as SpellScroll;
            int id = scroll.SpellID;
            MasterSpell master = getMaster(id);            
            if(master == null) 
            {
                m_Data.Add(new MasterSpell(getScrollName(id),id, scroll.UsesRemaining));
                m_Data.Sort((a, b) => a.ID.CompareTo(b.ID));
            }
            else 
            {
               master.Charges += scroll.UsesRemaining;
            }   
                
        }

        public void RemoveMaster(Mobile from, int id, int amount = 0)
        {
            MasterSpell master = getMaster(id);
            SpellScroll item;
            if ((master.Charges - amount) < 0)
            {
                from.SendMessage("You don't have that many charges.");
                return;
            }
            if ((amount == 0) || ((master.Charges - amount) == 0))
            {
                item = Activator.CreateInstance(Loot.RegularScrollTypes[master.ID]) as SpellScroll;
                item.MasterStatus = 2;
                item.UsesRemaining = master.Charges;
                from.AddToBackpack(item);
                from.SendMessage("You remove the master scroll from your book.");
                m_Data.Remove(master);
            }
            else
            {
                item = Activator.CreateInstance(Loot.RegularScrollTypes[master.ID]) as SpellScroll;
                item.MasterStatus = 2;
                item.UsesRemaining = amount;
                from.AddToBackpack(item);
                from.SendMessage("You carefully remove part of the master scroll.");
                master.Charges -= amount;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Data == null)
                return;
     
            LabelTo(from, "[{0} Spells]", m_Data.Count);
           
        }

        public override void OnDoubleClick(Mobile from)
        {
            Container pack = from.Backpack;

            if (Parent == from || (pack != null && Parent == pack))
            {
                if (from.HasGump(typeof(MasterSpellBookGump)))
                    from.CloseGump(typeof(MasterSpellBookGump));

                from.SendGump(new MasterSpellBookGump(from, this));
            }
            else
            {
                from.SendLocalizedMessage(1116249); // must be in your pack to use it.
                return;
            }
        }

        public override bool AllowEquipedCast(Mobile from)
        {
            return true;
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(dropped is SpellScroll))
                return false;

            var scroll = dropped as SpellScroll;

            if (scroll.MasterStatus == 0) //means it is a regular scroll
                return false;
            if (scroll.MasterStatus == 1) //Hasn't used the +1
            {
                from.SendMessage("You must make the initial copy before adding it to the book");
                return false;
            }
            scroll.Delete();

            from.PlaySound(0x55);
            from.SendMessage("You place the master scroll in your book.");
            AddMaster(dropped);
            return true;
        }

        public MasterSpellBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);

            writer.Write(m_Data.Count);
            foreach (MasterSpell master in m_Data)
            {
                writer.Write(master.Name);
                writer.Write(master.ID);
                writer.Write(master.Charges);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_Data = new List<MasterSpell>();
          
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++)
            {
                String name = reader.ReadString();
                int id = reader.ReadInt();
                int charges = reader.ReadInt();
                MasterSpell newSpell = new MasterSpell(name, id, charges);
                m_Data.Add(newSpell);
            }

        }
    }
}
