using System;
using Server;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Custom.Townsystem
{
    public static class Revolt
    {

        public static bool CanBeginRevolt(RevoltStone stone, TimeSpan timeBetweenRevolts)
        {
            var town = stone.ActiveTown;

            if (town.Election.CurrentState != ElectionState.Pending)
                return false;
            else if (town.Election.NextStateTime < TimeSpan.FromDays(1))
                return false;
            else if (stone.LastRevoltAttempt + timeBetweenRevolts > DateTime.UtcNow)
                return false;
            else if (town.King == null)
                return false;

            return true;
        }

        public class RevoltStone : Item
        {
            public override string DefaultName { get { return string.Format("{0} Revolt Stone", _Town.Definition.FriendlyName); } }

            #region Private Variables
            private Town _Town;
            private RevoltTimer _Timer;
            private List<Mobile> _Revolters;
            private DateTime _LastRevoltAttempt;
            private TimeSpan _TimeBetweenRevolts = TimeSpan.FromDays(2);
            #endregion

            #region Getters and Setters

            [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
            public Town ActiveTown
            {
                get { return _Town; }
                set { _Town = value; }
            }

            public int RevolterCount { get { return _Revolters.Count; } }

            [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
            public int TotalVoted
            {
                get { return _Town.TotalVoted; }
                set { _Town.TotalVoted = value; }
            }

            [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
            public DateTime LastRevoltAttempt
            {
                get { return _LastRevoltAttempt; }
                set { _LastRevoltAttempt = value; }
            }

            #endregion

            [Constructable]
            public RevoltStone()
                : base(0xEDC)
            {
                Movable = false;
                _Revolters = new List<Mobile>();
                _LastRevoltAttempt = DateTime.MinValue;

                Timer.DelayCall(TimeSpan.FromTicks(1), delegate
                {
                    _Town = Town.FromRegion(Region.Find(Location, Map));
                    if (_Town == null || (_Town.TownRevoltStone != null && !_Town.TownRevoltStone.Deleted))
                    {
                        Delete();
                        return;
                    }

                    _Town.TownRevoltStone = this;
                });
            }

            public override void OnDelete()
            {
                if (_Timer != null)
                {
                    _Timer.Stop();
                    _Timer = null;
                }

                base.OnDelete();
            }

            public override void OnDoubleClick(Mobile from)
            {
                if (!Revolt.CanBeginRevolt(this, _TimeBetweenRevolts))
                {
                    from.SendMessage("A revolt cannot be started at this time.");
                    return;
                }
                if (!(from is PlayerMobile))
                    return;

                var pm = from as PlayerMobile;

                if (pm.Citizenship != _Town)
                {
                    from.SendMessage("You're not a citizen of this town!");
                    return;
                }
                else if (!CitizenshipState.MeetsRequirements(from))
                {
                    from.SendMessage("You do not meet the requirements to participate in a revolt.");
                    return;
                }
                
                if (_Revolters.Contains(from))
                {
                    from.SendMessage("You have removed yourself from the revolt!");
                    _Revolters.Remove(from);

                    if (_Revolters.Count == 0 && _Timer != null && _Timer.Running)
                        _Timer.Stop();
                }
                else
                {
                    from.SendGump(new RevoltGump(from, _Town));
                }
            }

            public void Failed()
            {
                _Revolters.Clear();
                _Town.AddTownCrierEntry(new string[] { "The revolt has failed!", "Long live the king!" }, TimeSpan.FromHours(1));
                _LastRevoltAttempt = DateTime.UtcNow;
            }

            public void Successful()
            {
                if (_Town == null)
                    return;

                _Town.AddTownCrierEntry(new string[] { "The citizens have revolted!", "A new King shall be elected!" }, TimeSpan.FromDays(1));
                _Town.King = null;
                _LastRevoltAttempt = DateTime.UtcNow;
                if (_Town.Election.CurrentState == ElectionState.Pending)
                    _Town.Election.State = ElectionState.Announced;
            }

            public RevoltStone(Serial serial)
                : base(serial)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);
                writer.WriteEncodedInt(0); //version

                Town.WriteReference(writer, _Town);

                writer.Write(_Revolters.Count);
                foreach (var revolter in _Revolters)
                {
                    writer.Write(revolter);
                }

                writer.Write(LastRevoltAttempt);
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);
                int version = reader.ReadEncodedInt();

                _Town = Town.ReadReference(reader);

                int count = reader.ReadInt();
                _Revolters = new List<Mobile>();
                for (int i = 0; i < count; i++)
                {
                    var revolter = reader.ReadMobile();
                    if (revolter != null)
                        _Revolters.Add(revolter);
                }

                if (count > 0)
                {
                    _Timer = new RevoltTimer(this);
                    _Timer.Start();
                }

                LastRevoltAttempt = reader.ReadDateTime();
            }

            public void AddRevolter(Mobile mobile)
            {
                if (!_Revolters.Contains(mobile))
                {
                    _Revolters.Add(mobile);
                    if (_Timer == null)
                        _Timer = new RevoltTimer(this);
                    if (!_Timer.Running)
                        _Timer.Start();
                }
            }
        }

        public class RevoltGump : Gump
        {
            private Mobile m_From;
            private Town m_Town;

            enum Buttons
            {
                OK,
                Cancel
            }

            public RevoltGump(Mobile from, Town town)
                : base(50, 50)
            {
                m_From = from;
                m_Town = town;

                AddPage(0);
                AddBackground(65, 52, 261, 159, 9270);
                AddLabel(143, 80, 37, string.Format("{0} Revolt", town.Definition.FriendlyName));
                AddLabel(116, 109, 1153, @"Are you sure you wish");
                AddLabel(127, 130, 1153, @" to join the revolt?");
                AddButton(125, 166, 247, 248, (int)Buttons.OK, GumpButtonType.Reply, 0);
                AddButton(200, 166, 242, 241, (int)Buttons.Cancel, GumpButtonType.Reply, 0);
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                Mobile from = sender.Mobile;

                switch ((Buttons)info.ButtonID)
                {
                    case Buttons.OK:
                        {
                            m_Town.TownRevoltStone.AddRevolter(m_From);
                            break;
                        }

                }
            }
        }

        public class RevoltTimer : Timer
        {
            private RevoltStone _stone;
            private DateTime _forfeitRevolt;
            private Town _town;

            public RevoltTimer(RevoltStone stone)
                : base(TimeSpan.Zero, TimeSpan.FromMinutes(1))
            {
                Priority = TimerPriority.OneMinute;
                _stone = stone;
                _town = stone.ActiveTown;
                _forfeitRevolt = DateTime.UtcNow + TimeSpan.FromHours(24);
            }

            protected override void OnTick()
            {
                if (_stone == null || _stone.Deleted)
                {
                    Stop();
                    return;
                }

                if (_forfeitRevolt < DateTime.UtcNow)
                {
                    _stone.Failed();
                    Stop();
                    return;
                }

                if (_stone.RevolterCount >= (_town.TotalVoted * (2 / 3)))
                {
                    _stone.Successful();
                    Stop();
                    return;
                }
            }
        }
    }
}
