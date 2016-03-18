using Server.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Items
{
    public class SkillManual : Item
    {
        private int _skill;
        private int _rarity;
        private string _skillLevel;

        [CommandProperty(AccessLevel.Administrator)]
        public int MaxCapOffset { get; set; }

        [Constructable]
        public SkillManual(int skillID, int rarity, string level)
            : base(0x1E23)
        {
            this.SkillID = skillID;
            this.Rarity = rarity;
            this.SkillLevel = level;
            Hue = SkillScroll.HueLookup[_rarity];
        }

        public SkillManual(Serial serial)
            : base(serial)
        {
        }

        public override string DefaultName
        {
            get
            {
                return SkillScroll.ColorLookup[Rarity] + 
                    " manual of the " + 
                    SkillLevel + " " + 
                    SkillInfo.Table[SkillID].Title.ToLower();
            }
        }

        // Match up base gain caps with skill titles
        protected int gainCap()
        {
            int index = Array.IndexOf(SkillScroll.LevelLookup, _skillLevel.ToLower());

            return (SkillScroll.GainCaps[Math.Min(Math.Max(index + MaxCapOffset, 0), SkillScroll.GainCaps.Length - 1)]);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("This manual must be in your backpack to use.");
                return;
            }

            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (player.Region is UOACZRegion)
                {
                    from.SendMessage("You cannot use skill manuals while in this region.");
                    return;
                }

                if (player.IsInTempStatLoss)
                {
                    from.SendMessage("You cannot use skill manuals while in temporary statloss.");
                    return;
                }

                if (player.IsInMilitia && (SkillID == (int)SkillName.AnimalTaming || SkillID == (int)SkillName.Begging))
                {
                    from.SendMessage(0x22, "Militia members may not pursue a career in animal taming or begging");
                    return;
                }
            }

            Skill skill = from.Skills[_skill];
            int gainFactor = SkillScroll.GainFactor[_rarity];
            int change = Math.Min(gainFactor, skill.CapFixedPoint - skill.BaseFixedPoint);
            int newFixedPoint = Math.Min(skill.BaseFixedPoint + change, skill.CapFixedPoint);
            int oldFixedPoint = skill.BaseFixedPoint;
            change = SkillScroll.Clamp(change, 0, skill.CapFixedPoint - skill.BaseFixedPoint);

            if (skill != null)
            {
                if (newFixedPoint > skill.CapFixedPoint)
                {
                    from.SendMessage(String.Format("This manual will not work for you because of your skill cap."));
                    return;
                }
                else if ((oldFixedPoint - 100) > gainCap())
                {
                    from.SendMessage("You are too experienced to be reading such material.");
                    return;
                }

                if (from.SkillsTotal + change > from.SkillsCap)
                {
                    bool atrophied = false;
                    Skill sk;
                    for (int i = 0; i < from.Skills.Length; i++)
                    {
                        sk = from.Skills[i];

                        if (sk != skill && sk.Lock == SkillLock.Down && sk.BaseFixedPoint >= change)
                        {
                            atrophied = true;
                            sk.BaseFixedPoint = (sk.BaseFixedPoint - change);
                            break;
                        }

                    }

                    if (!atrophied)
                    {
                        from.SendMessage("Using this manual will increase your skills beyond the skillcap. Please set skills to be atrophied by setting that skill's arrow facing down.");
                        return;
                    }
                }

                from.SendMessage("After reading the manual you have a better understanding of your practice.");

                Effects.PlaySound(from.Location, from.Map, 515);
                Effects.SendTargetParticles(from, 0x375A, 35, 30, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                skill.BaseFixedPoint = newFixedPoint;
                Delete();
            }


        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MaxCapOffset);
            writer.Write(_skillLevel);
            writer.Write(_skill);
            writer.Write(_rarity);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            MaxCapOffset = reader.ReadInt();
            _skillLevel = reader.ReadString();
            _skill = reader.ReadInt();
            _rarity = reader.ReadInt();
        }


        [CommandProperty(AccessLevel.GameMaster)]
        public int SkillID
        {
            get { return _skill; }
            set
            {
                if (_skill < 0 || _skill > (Enum.GetValues(typeof(SkillName)).Length - 1))
                    value = 1;
                _skill = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string SkillLevel
        {
            get { return _skillLevel; }
            set
            {
                _skillLevel = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Rarity
        {
            get { return _rarity; }
            set
            {
                if (value >= 0 && value <= 2)
                {
                    _rarity = value;
                    Hue = SkillScroll.HueLookup[value];
                }
            }
        }

    }
}
