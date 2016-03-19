using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Items;
using Server.Misc;
using Server.Mobiles;


namespace Server.Items
{
    // I just stuck everything in one item, this isn't a very big system and it shouldn't be.
    // The whole item could use a basic code revision, but I don't see the point -- if anyone else feels like cleaning it up feel free.

    public class SkillScroll : Item
    {
        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private int _skill;
        private int _rarity;
        private string _skillLevel;
        private Mobile _owner;

        [CommandProperty(AccessLevel.Administrator)]
        public int MaxCapOffset { get; set; }

        public static String[] LevelLookup = { "neophyte", "novice", "apprentice", "journeyman", "expert", "adept", "master", "grandmaster", "elder" };
        public static String[] ColorLookup = { "a bronze", "a silver", "a gold" };
        public static int[] GainFactor = { 1, 2, 3 };
        public static int[] GainCaps = { 300, 400, 500, 600, 700, 800, 900, 1000, 1100 };
        public static int[] HueLookup = { 2418, 2301, 2213 };

        [Constructable]
        public SkillScroll(Mobile who, int skillID, int rarity, string level): base(0x1F65)
        {
            this.Owner = who;
            this.SkillID = skillID;
            this.Rarity = rarity;
            this.SkillLevel = level;
            Hue = HueLookup[_rarity];

            if (who.Region is UOACZRegion)
                Delete();
        }

        [Constructable]
        public SkillScroll()
        {
        }

        public SkillScroll(Serial serial)
            : base(serial)
        {
        }

        public static SkillScroll Generate(PlayerMobile pm, double maxLevel = 100.0, int rarity = 0, int skillID = -1)
        {
            if (pm == null)
                return null;

            if (pm.IsInTempStatLoss || pm.Region is UOACZRegion)
                return null;

            Skill skill;

            if (skillID == -1)
            {
                skill = pm.BestCombatSkill(maxLevel);
            }
            else
            {
                skill = pm.Skills[skillID];
            }

            if (skill == null)
                return null;

            string skillname = FameKarmaTitles.GetSkillLevelName(skill);

            return new SkillScroll(pm, skill.SkillID, rarity, skillname);
        }


        public override string DefaultName
        {
            get
            {
                if (Owner == null)
                    return "a skill scroll";

                string charName = Owner.Name;
                if (Owner != null)
                {
                    return ColorLookup[_rarity] + " scroll of the " + _skillLevel.ToLower() + " " + SkillInfo.Table[_skill].Title.ToLower() + " (for: " + charName + ")";
                }
                else
                {
                    return "a scroll";
                }
            }
        }

        // Match up base gain caps with skill titles
        protected int gainCap()
        {
            int index = Array.IndexOf(LevelLookup, _skillLevel.ToLower());

            return GainCaps[Math.Min(Math.Max(index + MaxCapOffset, 0), GainCaps.Length - 1)];
        }

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null)
            {
                if (player.Region is UOACZRegion)
                {
                    from.SendMessage("You cannot use skill scrolls while in this region.");
                    return;
                }

                if (player.IsInTempStatLoss)
                {
                    from.SendMessage("You cannot use skill scrolls while in temporary statloss.");
                    return;
                }
            }
            
            if (from.Equals(_owner) || Owner == null)
            {
                Skill skill = from.Skills[_skill];
                int change = GainFactor[_rarity];
                int newFixedPoint = Math.Min(skill.BaseFixedPoint + change, skill.CapFixedPoint);
                int oldFixedPoint = skill.BaseFixedPoint;
                change = Clamp(change, 0, skill.CapFixedPoint - skill.BaseFixedPoint);

                if (skill != null)
                {
                    if (newFixedPoint > skill.CapFixedPoint)
                    {
                        from.SendMessage(String.Format("This scroll will not work for you because of your skill cap."));
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
                            from.SendMessage("Using this scroll will increase your skills beyond the skillcap. Please set skills to be atrophied by setting that skill's arrow facing down.");
                            return;
                        }
                    }

                    from.SendMessage("After reading the scroll you have a better understanding of your practice.");

                    Effects.PlaySound(from.Location, from.Map, 515);
                    Effects.SendTargetParticles(from, 0x375A, 35, 30, 0x00, 0x00, 9502, (EffectLayer)255, 0x100);

                    skill.BaseFixedPoint = newFixedPoint;
                    Delete();
                }

            }
            else
            {
                if (!IsChildOf(from.Backpack))
                {
                    ; // Non-owner trying to dbl click from ground or wherever
                }
                else if (_owner != null && !from.Equals(_owner))
                {
                    from.Emote("*a scroll melts in your hands*");
                    from.SendMessage("The skill scroll disintegrates and returns back to the void.");
                    Delete();
                }
            }
        }

        public override bool CheckLift(Mobile from, Item item, ref Server.Network.LRReason reject)
        {
            bool baseCheck = base.CheckLift(from, item, ref reject);
            bool liftResult = false;

            if (_owner == null || _owner.Equals(from) || IsChildOf(from.Backpack) || from.AccessLevel >= AccessLevel.GameMaster)
                liftResult = true;

            return baseCheck && liftResult;

        }

        // moving scroll onto a container instead of into
        public override bool OnDroppedOnto(Mobile from, Item target)
        {
            bool dropResult = false;

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                return base.OnDroppedOnto(from, target);
            }

            if (target is Container)
            {
                if (((Container)from.BankBox == (Container)target) || ((target.IsChildOf((Container)from.BankBox))))
                {
                    return base.OnDroppedOnto(from, target);
                }
            }

            if (target is Backpack)
            {
                if ((from.Backpack != null && from.Backpack.Equals(target)))
                {
                    return base.OnDroppedOnto(from, target);
                }
            }


            //return base.OnDroppedOnto(from, target);
            return dropResult;
        }

        // moving scroll within containers
        public override bool OnDroppedInto(Mobile from, Container target, Point3D p)
        {
            bool baseCheck = base.OnDroppedInto(from, target, p);
            bool dropResult = false;


            if ((_owner == null || from.Equals(_owner)) && (from.BankBox != null && ((Container)from.BankBox).Equals(target)) || (from.Backpack != null && from.Backpack.Equals(target)))
                dropResult = true;

            // sub containers of the bankbox of the owner of the scrolls
            if (target.IsChildOf((Container)from.BankBox))
                dropResult = true;

            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                dropResult = true;
            }

            // secure/lockdowns allowed if mobile can access them (owner/coowner/friend)
            //if ((target.IsSecure || target.IsLockedDown) && target.IsAccessibleTo(from))
            //    dropResult = true;

            // sub containers of lockdowns/secures
            //if((target.IsSecure || target.IsLockedDown) && target.IsChildOf(target.IsAccessibleTo(from)))
            //{
            //    dropResult = true;
            //}

            return baseCheck && dropResult;
        }

        public override bool OnDroppedToWorld(Mobile from, Point3D p)
        {
            if (base.OnDroppedToWorld(from, p) && _owner != null)
            {
                from.Emote("*a scroll melts in your hands*");
                from.SendMessage("The skill scroll disintegrates and returns back to the void.");
                Delete();
            }

            return true;
        }

        public override bool OnDroppedToMobile(Mobile from, Mobile target)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                return true;
            }

            if (_owner != null)
            {
                if (_owner != target)
                {
                    from.SendMessage("This item is bound to you and may not be traded and will be deleted on drop.");
                    return false;
                }
                else
                {
                    from.SendMessage("The scroll has been placed in your backpack.");
                    return true;
                }
                //return false;  //scotts old code
            }
            else
            {
                //return base.OnDroppedToMobile(from, target);
                return false;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            //version 1
            writer.Write(MaxCapOffset);

            //version 0
            writer.Write((string)_skillLevel);
            writer.Write((int)_skill);
            writer.Write((int)_rarity);
            writer.WriteMobile(_owner);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        MaxCapOffset = reader.ReadInt();
                        goto case 0;
                    }
                case 0:
                    {
                        _skillLevel = reader.ReadString();
                        _skill = reader.ReadInt();
                        _rarity = reader.ReadInt();
                        _owner = reader.ReadMobile();

                        break;
                    }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SkillID
        {
            get { return _skill; }
            set
            {
                if (_skill < 0 || _skill > 55)
                    value = 1;
                _skill = value;
            }
        }

        /*
        [CommandProperty(AccessLevel.GameMaster)]
        public int SkillLevel
        {
            get { return _skillLevel; }
            set 
            {
                if (value >= 0 && value <= 6)
                {
                    _skillLevel = value;
                }
            }
        }
        */

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
                    Hue = HueLookup[value];
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }
    }
}
