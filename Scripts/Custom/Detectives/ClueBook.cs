/***************************************************************************
 *                                ClueBook.cs
 *                            ------------------
 *   begin                : February 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using System.Collections.Generic;
using Server;
using Server.Custom;
using Server.Gumps;

namespace Server.Items
{
    public class ClueBook : Item
    {
        public class ClueBookData
        {
            public List<ClueBookEntry> Entries = new List<ClueBookEntry>();

            public ClueBookData()
            {
            }

            public ClueBookEntry FindEntry(Mobile killer)
            {
                foreach (ClueBookEntry entry in Entries)
                    if (entry.Killer == killer)
                        return entry;

                var newEntry = new ClueBookEntry(killer);

                Entries.Add(newEntry);

                return newEntry;
            }

            public void Serialize(GenericWriter writer)
            {
                writer.WriteEncodedInt(0); //version

                writer.Write(Entries.Count);
                foreach (ClueBookEntry entry in Entries)
                {
                    writer.Write(entry.Killer);
                    writer.Write(entry.Clues.Count);

                    foreach (ClueEntry clue in entry.Clues)
                    {
                        writer.Write((byte)clue.ClueForm);
                        writer.Write((byte)clue.ClueQuality);
                    }
                }
            }

            public static ClueBookData Deserialize(GenericReader reader)
            {
                int version = reader.ReadEncodedInt();

                var data = new ClueBookData();

                int count = reader.ReadInt();
                for (int i = 0; i < count; i++)
                {
                    Mobile killer = reader.ReadMobile();

                    var clues = new List<ClueEntry>();

                    int clueCount = reader.ReadInt();

                    for (int j = 0; j < clueCount; j++)
                    {
                        var type = (Clue.ClueType)reader.ReadByte();
                        var quality = (Clue.ClueQuality)reader.ReadByte();

                        var clue = new ClueEntry(type, quality);

                        clues.Add(clue);
                    }

                    if (killer != null && !killer.Deleted)
                        data.Entries.Add(new ClueBookEntry(killer, clues));
                }

                return data;
            }
        }
        public class ClueBookEntry
        {
            private Mobile m_Killer;
            private List<ClueEntry> m_Clues;

            public Mobile Killer { get { return m_Killer; } }
            public List<ClueEntry> Clues { get { return m_Clues; } }

            public ClueBookEntry(Mobile m, List<ClueEntry> clues = null)
            {
                m_Killer = m;

                if (clues == null)
                    m_Clues = new List<ClueEntry>();
                else
                    m_Clues = clues;
            }

            public void AddClue(Clue.ClueType form, Clue.ClueQuality quality)
            {
                m_Clues.Add(new ClueEntry(form, quality));
            }

            public void GetEvidence(out int footprints, out int weapon, out int blood)
            {
                footprints = weapon = blood = 0;

                foreach (ClueEntry clue in Clues)
                {
                    if (clue.ClueForm == Clue.ClueType.Footsteps)
                        footprints++;
                    else if (clue.ClueForm == Clue.ClueType.MurderWeapon)
                        weapon++;
                    else
                        blood++;
                }
            }
        }
        public class ClueEntry
        {
            private Clue.ClueType m_ClueForm;
            private Clue.ClueQuality m_ClueQuality;

            public Clue.ClueType ClueForm { get { return m_ClueForm; } set { m_ClueForm = value; } }
            public Clue.ClueQuality ClueQuality { get { return m_ClueQuality; } set { m_ClueQuality = value; } }

            public ClueEntry(Clue.ClueType form, Clue.ClueQuality quality)
            {
                m_ClueForm = form;
                m_ClueQuality = quality;
            }

        }

        public override string DefaultName { get { return "Detective's Cluebook"; } }

        private ClueBookData m_Data;
        public ClueBookData Data { get { return m_Data; } set { m_Data = value; } }

        [Constructable]
        public ClueBook()
            : base(0x238c)
        {
            m_Data = new ClueBookData();
            LootType = Server.LootType.Blessed;
            Weight = 3.0;
            Layer = Layer.OneHanded;
        }

        public Clue.ClueType GetClueType(Mobile killer)
        {
            if (m_Data != null && m_Data.Entries != null)
            {
                foreach (ClueBookEntry entry in m_Data.Entries)
                {
                    if (entry.Killer == killer)
                    {
                        bool footprints = false, weapon = false, blood = false;
                        foreach (ClueEntry clue in entry.Clues)
                        {
                            if (clue.ClueForm == Clue.ClueType.MurderWeapon)
                                weapon = true;
                            else if (clue.ClueForm == Clue.ClueType.Blood)
                                blood = true;
                            else
                                footprints = true;
                        }

                        if (Utility.RandomBool())
                        {
                            if (weapon == false)
                                return Clue.ClueType.MurderWeapon;
                            else if (blood == false)
                                return Clue.ClueType.Blood;
                            else
                                return Clue.ClueType.Footsteps;

                        }
                        else if (Utility.RandomBool())
                        {
                            if (blood == false)
                                return Clue.ClueType.Blood;
                            else if (footprints == false)
                                return Clue.ClueType.Footsteps;
                            else
                                return Clue.ClueType.MurderWeapon;
                        }
                        else
                        {
                            if (footprints == false)
                                return Clue.ClueType.Footsteps;
                            else if (weapon == false)
                                return Clue.ClueType.MurderWeapon;
                            else
                                return Clue.ClueType.Blood;
                        }
                    }
                }
            }

            return (Clue.ClueType)Utility.Random(3);
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Data == null || m_Data.Entries == null)
                return;

            var killers = m_Data.Entries;
            var noClues = 0;

            foreach (ClueBookEntry entry in killers)
                if (entry.Clues != null)
                    noClues += entry.Clues.Count;

            LabelTo(from, "[{0} murderers]", killers.Count);
            LabelTo(from, "[{0} clues]", noClues);
        }

        public override void OnDoubleClick(Mobile from)
        {
            Container pack = from.Backpack;

            if (Parent == from || (pack != null && Parent == pack))
            {
                if (from.HasGump(typeof(DetectiveBookGump)))
                    from.CloseGump(typeof(DetectiveBookGump));

                from.SendGump(new DetectiveBookGump(from, this));
            }
            else
            {
                from.SendLocalizedMessage(1116249); // must be in your pack to use it.
                return;
            }
        }

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (!(dropped is Clue))
                return false;

            Clue clue = dropped as Clue;

            var entry = m_Data.FindEntry(clue.Killer);

            entry.AddClue(clue.ClueForm, clue.Quality);

            clue.Delete();
            from.PlaySound(0x55);
            from.SendMessage("You record the clue in your cluebook.");
            return true;
        }

        public void Extract(Mobile from, int index, Clue.ClueType type)
        {
            Container pack = from.Backpack;

            if (Parent != from && (pack == null || Parent != pack))
            {
                from.SendLocalizedMessage(1116249); // must be in your pack to use it.
                return;
            }

            var entries = Data.Entries;

            if (entries.Count < index)
            {
                from.SendLocalizedMessage(1112587); //invalid entry.
                return;
            }

            var entry = entries[index];

            var clues = entry.Clues;

            ClueEntry clue = null;

            foreach (ClueEntry ce in clues)
            {
                if (ce.ClueForm == type)
                {
                    clue = ce;
                    break;
                }
            }

            if (clue != null && clues.Contains(clue))
            {
                clues.Remove(clue);
                from.AddToBackpack(new Clue(entry.Killer, clue.ClueForm, clue.ClueQuality));
                from.SendMessage("You have extracted the clue from your Detective's Book.");
            }

            if (clues.Count == 0)
                entries.Remove(entry);
        }

        public void CreateWantedNote(Mobile from, int index)
        {
            Container pack = from.Backpack;

            if (Parent != from && (pack == null || Parent != pack))
            {
                from.SendLocalizedMessage(1116249); // must be in your pack to use it.
                return;
            }

            var entries = Data.Entries;

            if (entries.Count < index)
            {
                from.SendLocalizedMessage(1112587); //invalid entry.
                return;
            }

            var entry = entries[index];

            var clues = entry.Clues;

            ClueEntry footprint = null, weapon = null, blood = null;

            foreach (ClueEntry ce in clues)
            {
                if (ce.ClueForm == Clue.ClueType.Footsteps && footprint == null)
                    footprint = ce;
                else if (ce.ClueForm == Clue.ClueType.MurderWeapon && weapon == null)
                    weapon = ce;
                else if (ce.ClueForm == Clue.ClueType.Blood && blood == null)
                    blood = ce;
                else if (footprint != null && weapon != null && blood != null)
                    break;
            }


            if (footprint == null || weapon == null || blood == null)
            {
                from.SendMessage("You must have one of each clue type in order to create a wanted note.");
                return;
            }

            clues.Remove(footprint);
            clues.Remove(weapon);
            clues.Remove(blood);

            int qual = 50;
            qual += ((int)footprint.ClueQuality - 1) * 15;
            qual += ((int)weapon.ClueQuality - 1) * 15;
            qual += ((int)blood.ClueQuality - 1) * 15;

            if (from != null && entry.Killer != null)
            {
                var wn = new WantedNote(entry.Killer, from, qual);
                from.AddToBackpack(wn);
                from.PublicOverheadMessage(Network.MessageType.Emote, from.EmoteHue, true, String.Format("*{0} has obtained indisputable evidence against {1} for murder*", from.Name, entry.Killer.RawName));
                from.SendMessage("A wanted note has been added to your backpack.");
            }

            if (entry.Clues.Count == 0)
                entries.Remove(entry);
        }

        public ClueBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);

            m_Data.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            m_Data = ClueBookData.Deserialize(reader);
        }
    }
}
