using System;
using System.Collections.Generic;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Commands;
using Server.Targeting;

namespace Server.Custom
{
    public static class Detective
    {
        private static Dictionary<Mobile, KeyValuePair<Mobile,DateTime>> m_Table = new Dictionary<Mobile, KeyValuePair<Mobile,DateTime>>();//should be a separate class.. oh well
        private static Queue m_Queue = new Queue();
        private static Layer[] m_Layers = new Layer[]
		{
			Layer.Cloak, Layer.Bracelet, Layer.Ring, Layer.Shirt, Layer.Pants, Layer.InnerLegs,
			Layer.Shoes, Layer.Arms, Layer.InnerTorso, Layer.MiddleTorso, Layer.OuterLegs, 
            Layer.Neck, Layer.Waist, Layer.Gloves, Layer.OuterTorso, Layer.OneHanded,
			Layer.TwoHanded, Layer.FacialHair, Layer.Hair, Layer.Helm, Layer.Talisman
		};

        public static void Initialize()
        {
            CommandSystem.Register("CreateClue", AccessLevel.GameMaster, new CommandEventHandler(CreateClue_OnCommand));
        }

        [Usage("CreateClue")]
        [Description("")]
        public static void CreateClue_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            from.Target = new InternalTarget();

        }

        public class InternalTarget : Target
        {
            public InternalTarget()
                : base(12, false, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    CreateClue(from, (targeted as Mobile));
                }
            }
        }

        public static bool OnTargetCorpse(Mobile from, Corpse c)
        {
            if (from.Player && ((PlayerMobile)from).NpcGuild == NpcGuild.DetectivesGuild)
            {
                if (from.ShortTermMurders > 0)
                {
                    from.SendMessage("You have been suspended from the Detective's Guild.");
                    return false;
                }
                else if (c.Owner != null && c.Owner == from)
                {
                    from.SendMessage("You cannot gather clues from your own body.");
                    return false;
                }

                if (c.Owner == null || !c.Owner.Player)
                    return false;

                //Is the killer a murderer?
                if (c.Owner.ShortTermMurders < 5 && c.Killer != null && c.Killer.ShortTermMurders > 0 && !c.Aggressors.Contains(c.Killer))
                {
                    if (!c.IdentifiedKiller)
                    {
                        c.IdentifiedKiller = true;
                        c.PublicOverheadMessage(Network.MessageType.Regular, 0x0, true, String.Format("{0} has gathered evidence against {1} for murder.", from.Name, c.Killer.Name));
                        CreateClue(from, c.Killer);
                    }
                    else
                    {
                        c.LabelTo(from, String.Format("The evidence has already been gathered against {0}.", c.Killer.Name));
                        return true;
                    }
                }
                else
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "You find no signs of a murder.");
                }
            }
            return false;
        }

        private static void CreateClue(Mobile from, Mobile killer)
        {
            double skill = from.Skills.Forensics.Value;
            double rand = Utility.RandomDouble() * 100;

            int cq = 0;

            if (rand / 2 > skill)
                cq = 0;
            else if (rand > skill)
                cq = 1;
            else
                cq = 2;

            var clueBook = (ClueBook)from.Backpack.FindItemByType(typeof(ClueBook));

            int ct;

            if (clueBook == null)
                ct = Utility.Random(3);
            else
                ct = (int)clueBook.GetClueType(killer);

            Clue c = new Clue(killer, (Clue.ClueType)ct, (Clue.ClueQuality)cq);

            from.AddToBackpack(c);
        }

        public static void OnDetectHidden(Mobile from, Point3D target, int range, ref bool foundAnything)
        {
            IPooledEnumerable eable = Map.Felucca.GetItemsInRange(target, range);
            Queue q = new Queue();

            foreach (Item i in eable)
                if (i is Corpse && ((Corpse)i).Owner != null)
                    q.Enqueue(i);

            eable.Free();

            while (q.Count > 0)
            {
                Corpse c = (Corpse)q.Dequeue();

                if (!(c.Owner is PlayerMobile)) //player?
                    continue;

                if (c.Aggressors.Contains(c.Killer)) //not murdered
                    continue;

                Mobile killer = c.Killer;

                if (c.Killer == null || c.Killer.ShortTermMurders <= 0)
                    continue;

                foundAnything = true;
                FoundCorpse(from, target, c);
                break;
            }

            q.Clear();
        }
        
        private static void FoundCorpse(Mobile detective, Point3D target, Corpse corpse)
        {
            detective.PublicOverheadMessage(Network.MessageType.Emote, detective.EmoteHue, true, "You have uncovered a trail of blood!");
            new DetectiveFootprints(Utility.GetDirection(target,corpse.Location)).MoveToWorld(target, detective.Map);
        }

        public static bool OnBountyHunterKill(Mobile killed)
        {
            if (KillerInDatabase(killed))
            {
                KeyValuePair<Mobile,DateTime> entry;
                m_Table.TryGetValue(killed, out entry);

                if (entry.Key != null)
                {
                    GiveAllCorpseItemsTo(killed.Corpse, entry.Key);
                    entry.Key.SendMessage(String.Format("Bounty hunters have placed {0} under arrest! You have been awarded all items in his posession.",killed.Name));
                }
                
                m_Table.Remove(killed);
                return true;
            }
            return false;
        }

        private static void CleanDatabase()
        {
            foreach (KeyValuePair<Mobile, KeyValuePair<Mobile, DateTime>> entry in m_Table)
				if (entry.Value.Value + TimeSpan.FromMinutes(30) < DateTime.UtcNow)
                    m_Queue.Enqueue(entry.Key);


            while (m_Queue.Count > 0)
                m_Table.Remove((Mobile)m_Queue.Dequeue());

            m_Queue.Clear();
        }

        public static bool KillerInDatabase(Mobile killer)
        {
            CleanDatabase();

            return m_Table.ContainsKey(killer);
        }

        public static void Strip(Mobile target)
        {
            foreach (Layer l in m_Layers)
            {
                Item i = target.FindItemOnLayer(l);
                if (i != null)
                    target.PlaceInBackpack(i);
            }

        }

        public static void GiveAllPackItemsTo(Mobile from, Mobile to)
        {
            m_Queue.Clear();
            Bag bag = new Bag();
            bag.Name = String.Format("Reward bag for Detective {0}", to.Name);


            foreach (Item i in from.Backpack.Items)
            {
                if (i.LootType != LootType.Blessed && i.LootType != LootType.Newbied)
                    m_Queue.Enqueue(i);
            }

            while (m_Queue.Count > 0)
            {
                bag.DropItem((Item)m_Queue.Dequeue());
            }

            to.AddToBackpack(bag);
        }
            
        public static void GiveAllCorpseItemsTo(Container from, Mobile to)
        {
            if (from == null || to == null)
                return;

            m_Queue.Clear();
            Bag bag = new Bag();
            bag.Name = String.Format("Reward bag for Detective {0}", to.Name);
            bag.MoveToWorld(to.Location, to.Map);

            foreach (Item i in from.Items)
            {
                if (i.LootType != LootType.Blessed)
                    m_Queue.Enqueue(i);
            }

            while (m_Queue.Count > 0)
            {
                bag.DropItem((Item)m_Queue.Dequeue());
            }

            to.Backpack.DropItem(bag);

            from.Delete();
        }

        public static void AddDetectiveMurdererPair(Mobile killer, Mobile det)
        {
            if (m_Table.ContainsKey(killer))
                m_Table.Remove(killer);

			KeyValuePair<Mobile, DateTime> entry = new KeyValuePair<Mobile, DateTime>(det, DateTime.UtcNow);
            m_Table.Add(killer, entry);
        }

    }
}
