/***************************************************************************
 *                                Clues.cs
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
using Server.Targeting;
using Server.Mobiles;

namespace Server.Custom
{
    public class Clue : Item
    {
        public enum ClueType
        {
            Footsteps,
            Blood,
            MurderWeapon
        }

        public enum ClueQuality
        {
            sketchy,
            regular,
            exceptional
        }

        private Mobile _killer;
        private ClueType _clueType;
        private ClueQuality _clueQuality;

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Killer
        {
            get { return _killer; }
            set {  _killer = value; Invalidate(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ClueQuality Quality
        {
            get { return _clueQuality; }
            set  {  _clueQuality = value; Invalidate(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public ClueType ClueForm
        {
            get { return _clueType; }
            set { _clueType = value; Invalidate(); }
        }

        [Constructable]
        public Clue()
            : this(null, ClueType.Blood, ClueQuality.regular)
        {
        }

        private void Invalidate()
        {
            String s = "";
            if (_clueType == ClueType.Blood)
            {
                ItemID = 0x122E;
                s = "a blood clue";
            }
            else if (_clueType == ClueType.Footsteps)
            {
                ItemID = 0x1E04;
                s = "a footprint clue";
            }
            else
            {
                ItemID = 0x0F51;
                s = "a murder weapon clue";
            }

            Name = String.Concat(s, String.Format(" of {1} quality{0}", _killer == null ? "" : String.Concat(" against ", _killer.Name), _clueQuality.ToString()));
            
        }

        public Clue(Mobile killer, ClueType ct, ClueQuality cq) : base( 0 )
		{
            _killer = killer;
            Stackable = false;
            _clueQuality = cq;

            ClueForm = ct;
		}

        public Clue(Serial serial)
            : base(serial)
        {
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (((PlayerMobile)from).NpcGuild == NpcGuild.DetectivesGuild)
            {
                from.SendMessage("Target the second clue you would like to combine this with.");
                from.Target = new InternalFirstTarget(this);
            }

        }

        public static void CombineClues(Mobile from, Clue c1, Clue c2, Clue c3)
        {
            if (c1._killer == c2._killer && c2._killer == c3._killer)
            {
                int qual = 50;
                qual += ((int)c1._clueQuality - 1) * 15;
                qual += ((int)c2._clueQuality - 1) * 15;
                qual += ((int)c3._clueQuality - 1) * 15;

                var wn = new WantedNote(c1._killer, from, qual);

                if (from != null && c1._killer != null)
                {
                    from.AddToBackpack(wn);
                    from.PublicOverheadMessage(Network.MessageType.Emote, from.EmoteHue, true, String.Format("*{0} has obtained indisputable evidence against {1} for murder*", from.Name, c1._killer.Name));
                    from.SendMessage("A wanted note has been added to your backpack.");
                }

                c1.Delete();
                c2.Delete();
                c3.Delete();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write(_killer);
            writer.Write((int)_clueType);
            writer.Write((int)_clueQuality);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version  = reader.ReadInt();

            _killer = reader.ReadMobile();
            _clueType = (ClueType)reader.ReadInt();
            _clueQuality = (ClueQuality)reader.ReadInt();

            if (version < 1) //Delete all clues in the world
                Timer.DelayCall(TimeSpan.FromTicks(1), Delete);
        }

        private class InternalFirstTarget : Target
        {
            private Clue clue1;

            public InternalFirstTarget(Clue c1)
                : base(6, false, TargetFlags.None)
            {
                clue1 = c1;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is Clue))
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "Only clues can be combined together.");
                }
                else if (clue1._killer != ((Clue)targeted)._killer)
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "These clues are not against the same murderer.");
                }
                else if (clue1.ClueForm == ((Clue)targeted).ClueForm)
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "You can only combine clues of different types.");
                }
                else
                {
                    from.SendMessage("Target the third clue you would like to combine these with.");
                    from.Target = new InternalSecondTarget(clue1, (Clue)targeted);
                }

            }
        }

        private class InternalSecondTarget : Target
        {
            private Clue clue1;
            private Clue clue2;

            public InternalSecondTarget(Clue c1, Clue c2)
                : base(6, false, TargetFlags.None)
            {
                clue1 = c1;
                clue2 = c2;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (!(targeted is Clue))
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "Only clues can be combined together.");
                }
                else if (clue1._killer != ((Clue)targeted)._killer)
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "These clues are not against the same murderer.");
                }
                else if (clue1.ClueForm == ((Clue)targeted).ClueForm || clue2.ClueForm == ((Clue)targeted).ClueForm)
                {
                    from.LocalOverheadMessage(Network.MessageType.Regular, 0x0, true, "You can only combine clues of different types.");
                }
                else
                {
                    CombineClues(from, clue1, clue2, (Clue)targeted);
                }

            }
        }
    }
}
