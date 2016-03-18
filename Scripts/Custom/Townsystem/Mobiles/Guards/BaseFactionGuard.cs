using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Custom.Townsystem.AI;

namespace Server.Custom.Townsystem
{
    public abstract class BaseFactionGuard : BaseCreature
    {
        private Town m_Town;
        private Orders m_Orders;
        private Timer m_FreezeTimer;

        public virtual int GetBaseClothingHue() { return Town.HomeFaction.Definition.HuePrimary; }

        private bool m_DropCheck = true;
        public bool DropCheck { get { return m_DropCheck; } set { m_DropCheck = value; } }

        private bool m_KothGuard = false;
        public bool KothGuard { get { return m_KothGuard; } set { m_KothGuard = value; } }

        private const int ListenRange = 12;
        private DateTime m_OrdersEnd;

        public override int MaxDistanceAllowedFromHome { get { return 25; } }

        public BaseFactionGuard(string title): base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            m_Orders = new Orders(this);
            Title = title;

            SetTheme();
            StartFreeze(0);
        }

        public BaseFactionGuard(Serial serial): base(serial)
        {
            SetTheme();
            StartFreeze(0);
        }

        public void SetTheme()
        {
            Hue = Utility.RandomSkinHue();

            if (Items != null)
                foreach (Item ei in Items)
                    ei.Hue = GetBaseClothingHue();
        }

        public override void SetUniqueAI()
        {
            ResolveAcquireTargetDelay = 1.0;
            RangePerception = 12;
            RangeHome = 6;
        }

        public override void Heal(int amount, Mobile from, bool message)
        {
            if (!Blessed)
                base.Heal(amount, from, message);
            else
                from.SendMessage("You cannot perform beneficial actions on your target.");
        }

        public virtual void GenerateBody(bool isFemale, bool randomHair)
        {
            Hue = Utility.RandomSkinHue();

            if (isFemale)
            {
                Female = true;
                Body = 401;
                Name = NameList.RandomName("female");
            }

            else
            {
                Female = false;
                Body = 400;
                Name = NameList.RandomName("male");
            }

            if (randomHair)
                GenerateRandomHair();
        }

        public virtual void GenerateRandomHair()
        {
            Utility.AssignRandomHair(this);
            Utility.AssignRandomFacialHair(this, HairHue);
        }

        public override bool ImmuneToSpecialAttacks { get { return true; } }
        public override bool ImmuneToChargedSpells { get { return true; } }

        public override bool BardImmune { get { return true; } }

        public override bool DropsGold { get { return false; } }
        public override bool ClickTitle { get { return false; } }

        public Orders Orders
        {
            get { return m_Orders; }
        }

        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public Town Town
        {
            get { return m_Town; }
            set { Unregister(); m_Town = value; Register(); }
        }

        public void Register()
        {
            if (m_Town != null && !KothGuard)
                m_Town.RegisterGuard(this);
        }

        public void Unregister()
        {
            if (m_Town != null && !KothGuard)
                m_Town.UnregisterGuard(this);
        }

        public override bool IsEnemy(Mobile m)
        {
            Town ourTown = m_Town;

            var playerM = m as PlayerMobile;

            if (playerM == null)
                return false;

            Town theirTown = playerM.Citizenship;

            if (ourTown != null && theirTown != null && ourTown != theirTown && playerM.IsInMilitia)
            {
                ReactionType reactionType = Orders.GetReaction(theirTown).Type;

                if (reactionType == ReactionType.Attack)
                    return true;

                if (theirTown != null)
                {
                    List<AggressorInfo> list = m.Aggressed;

                    for (int i = 0; i < list.Count; ++i)
                    {
                        AggressorInfo ai = list[i];

                        if (ai.Defender is BaseFactionGuard)
                        {
                            BaseFactionGuard bf = (BaseFactionGuard)ai.Defender;

                            if (bf.Town == ourTown)
                                return true;
                        }
                    }
                }
            }

            return false;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            Mobile from = e.Mobile;

            if (!e.Handled && InRange(from, ListenRange) && from.Alive && (WasNamed(e.Speech) || Insensitive.Contains(e.Speech, "all")))
            {
                if (from.AccessLevel > AccessLevel.GameMaster)
                {
                }

                else if (Town.FromRegion(this.Region) != m_Town)
                {
                    Say("Thou art not a member of our militia.");
                    return;
                }

                else if (m_Town == null || (!m_Town.IsKing(from) && !m_Town.IsCommander(from)))
                {
                    Say("I do not report to thee.");
                    return;
                }               

                bool validCommand = true;
                
                ReactionType newType = 0;

                if (Insensitive.Contains(e.Speech, "attack"))
                    newType = ReactionType.Attack;

                else if (Insensitive.Contains(e.Speech, "warn"))
                    newType = ReactionType.Warn;

                else if (Insensitive.Contains(e.Speech, "ignore"))
                    newType = ReactionType.Ignore;
                else
                    validCommand = false;

                if (validCommand)
                {
                    validCommand = false;

                    if (Insensitive.Contains(e.Speech, "civil"))
                    {
                        ChangeReaction(null, newType);
                        validCommand = true;
                    }

                    List<Town> towns = Town.Towns;

                    for (int i = 0; i < towns.Count; ++i)
                    {
                        Town town = towns[i];

                        if (town != m_Town && Insensitive.Contains(e.Speech, town.HomeFaction.Definition.Keyword))
                        {
                            ChangeReaction(town, newType);
                            validCommand = true;
                        }
                    }
                }

                else if (Insensitive.Contains(e.Speech, "patrol"))
                {                       
                    if (Town != null)
                    {
                        Region region = Region.Find(Location, Map);

                        if (Town.Region != null)
                        {
                            if (region == Town.Region)
                            {
                                Say("Beginning patrol!");

                                if( !IsFrozen() && m_Orders.Movement != MovementType.Patrol )
                                    StartFreeze(15.0);

                                m_Orders.Movement = MovementType.Patrol;

                                //Set Creature AI Order Patrol
                                ControlOrder = OrderType.Patrol;
                                Home = new Point3D(from.Location);
                                ControlDest = new Point3D(from.Location);
                                ControlTarget = e.Mobile; //Treat Effectively as Owner

                                validCommand = true;
                            }

                            else
                                Say("I beg your pardon sire, but that is outside our town limits.");
                        }
                    }
                }

                else if (Insensitive.Contains(e.Speech, "follow"))
                {
                    Unfreeze();
                    SetPreventAllActions(true);
                    m_Orders.Follow = from;
                    m_Orders.Movement = MovementType.Follow;

                    //Set Creature AI Order Follow
                    ControlOrder = OrderType.Follow;
                    ControlTarget = e.Mobile; //Treat Effectively as Owner

                    Say("Yes, sire. Lead on!");

                    validCommand = true;
                }

                if (validCommand && from is PlayerMobile)
                {
                    ((PlayerMobile)from).AssistedOwnMilitia = true;
                    from.SendMessage("You have ordered a militia guard and are now vulnerable to enemy militia members.");
                }

                if (!validCommand)
                    Say("I beg your pardon, but I did not understand your order, sire.");
                else if (IsFrozen())
                    Say("I am currently frozen!");
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m.Player && m.Alive && InRange(m, 10) && !InRange(oldLocation, 10) && InLOS(m) && m_Orders.GetReaction(Town.Find(m)).Type == ReactionType.Warn)
            {
                Direction = GetDirectionTo(m);

                string warning = null;

                switch (Utility.RandomMinMax(1, 6))
                {
                    case 1: warning = "I warn you, {0}, you would do well to leave this area before someone shows you the world of gray."; break;
                    case 2: warning = "It would be wise to leave this area, {0}, lest your head become my commanders' trophy."; break;
                    case 3: warning = "You are bold, {0}, for one of the meager {1}. Leave now, lest you be taught the taste of dirt."; break;
                    case 4: warning = "Your presence here is an insult, {0}. Be gone now, knave."; break;
                    case 5: warning = "Dost thou wish to be hung by your toes, {0}? Nay? Then come no closer."; break;
                    case 6: warning = "Hey, {0}. Yeah, you. Get out of here before I beat you with a stick."; break;
                }

                Town town = Town.Find(m);

                Say(warning, m.Name, town == null ? "civilians" : town.HomeFaction.Definition.FriendlyName);
            }
        }

        public void StartFreeze(double seconds)
        {
            Say(String.Format("It will take me about {0} seconds to finish the preparations for my new task, Sire.", (int)seconds));
            m_FreezeTimer = Timer.DelayCall(TimeSpan.FromSeconds(seconds), () => { this.OnFreezeDone(); });
            Frozen = true;
            SetPreventAllActions(true);
        }

        private void Unfreeze()
        {
            if (m_FreezeTimer != null)
                m_FreezeTimer.Stop();
            OnFreezeDone();
        }

        private bool IsFrozen()
        {
            return m_FreezeTimer.Running;
        }

        private void OnFreezeDone()
        {
            Say(1005146); // This spot looks like it needs protection!  I shall guard it with my life.
            Frozen = false;
            SetPreventAllActions(false);
        }

        private void SetPreventAllActions(bool prevent)
        {
            if (Name != null)
            {
                bool inactive_label = Name[0] == '(';
                if (prevent && !inactive_label)
                    Name = "(INACTIVE) " + Name;
                else if (!prevent && inactive_label)
                    Name = Name.Substring("(INACTIVE) ".Length);
            }

            Hue = prevent ? 0x4001 : Utility.RandomSkinHue();
            if (Items != null)
                foreach (Item ei in Items)
                    ei.Hue = prevent ? 0x4001 : GetBaseClothingHue();
            Blessed = prevent;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            if (InRange(from, ListenRange))
                return true;

            return base.HandlesOnSpeech(from);
        }

        private void ChangeReaction(Town town, ReactionType type)
        {
            if (town == null)
            {
                switch (type)
                {
                    case ReactionType.Ignore: Say("I shall not pay heed to citizens."); break;
                    case ReactionType.Warn: Say("Citizens shall be warned of their trespasses."); break;
                    case ReactionType.Attack: Say("I shall attack without hesitation."); return;
                }
            }

            else
            {
                TextDefinition def = null;

                switch (type)
                {
                    case ReactionType.Ignore: def = town.HomeFaction.Definition.GuardIgnore; break;
                    case ReactionType.Warn: def = town.HomeFaction.Definition.GuardWarn; break;
                    case ReactionType.Attack: def = town.HomeFaction.Definition.GuardAttack; break;
                }

                if (def != null && def.Number > 0)
                    Say(def.Number);

                else if (def != null && def.String != null)
                    Say(def.String);
            }

            m_Orders.SetReaction(town, type);
        }

        private bool WasNamed(string speech)
        {
            string name = Name;

            if (Insensitive.StartsWith(name, "(INACTIVE) "))
                name = name.Substring("(INACTIVE) ".Length);

            return (name != null && Insensitive.StartsWith(speech, name));
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Town != null && Map == Faction.Facet)
                list.Add(1060846, m_Town.HomeFaction.Definition.PropName); // Guard: ~1_val~
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_Town != null && Map == Faction.Facet)
            {
                string text = String.Concat("(Guard, ", m_Town.HomeFaction.Definition.FriendlyName, ")");

                int hue = (Town.Find(from) == m_Town ? 98 : 38);                

                string order = "";

                switch(m_Orders.Movement)
                {
                    case MovementType.Follow: order = "*following*"; break; 
                    case MovementType.Patrol: order = "*patrolling*"; break;                     
                }

                if (order != "")
                    PrivateOverheadMessage(MessageType.Label, 0, true, order, from.NetState);

                PrivateOverheadMessage(MessageType.Label, hue, true, text, from.NetState);
            }

            base.OnSingleClick(from);
        }

        private static Type[] m_StrongPotions = new Type[]
		{
			typeof( GreaterHealPotion ), typeof( GreaterHealPotion ), typeof( GreaterHealPotion ),
			typeof( GreaterCurePotion ), typeof( GreaterCurePotion ), typeof( GreaterCurePotion ),
			typeof( GreaterStrengthPotion ), typeof( GreaterStrengthPotion ),
			typeof( GreaterAgilityPotion ), typeof( GreaterAgilityPotion ),
			typeof( TotalRefreshPotion ), typeof( TotalRefreshPotion ),
			typeof( GreaterExplosionPotion )
		};

        private static Type[] m_WeakPotions = new Type[]
		{
			typeof( HealPotion ), typeof( HealPotion ), typeof( HealPotion ),
			typeof( CurePotion ), typeof( CurePotion ), typeof( CurePotion ),
			typeof( StrengthPotion ), typeof( StrengthPotion ),
			typeof( AgilityPotion ), typeof( AgilityPotion ),
			typeof( RefreshPotion ), typeof( RefreshPotion ),
			typeof( ExplosionPotion )
		};

        public void PackStrongPotions(int min, int max)
        {
            PackStrongPotions(Utility.RandomMinMax(min, max));
        }

        public void PackStrongPotions(int count)
        {
            for (int i = 0; i < count; ++i)
                PackStrongPotion();
        }

        public void PackStrongPotion()
        {
            PackItem(Loot.Construct(m_StrongPotions));
        }

        public void PackWeakPotions(int min, int max)
        {
            PackWeakPotions(Utility.RandomMinMax(min, max));
        }

        public void PackWeakPotions(int count)
        {
            for (int i = 0; i < count; ++i)
                PackWeakPotion();
        }

        public void PackWeakPotion()
        {
            PackItem(Loot.Construct(m_WeakPotions));
        }

        public Item Immovable(Item item)
        {
            item.Movable = false;
            return item;
        }

        public Item Newbied(Item item)
        {
            item.LootType = LootType.Newbied;
            return item;
        }

        public Item Rehued(Item item, int hue)
        {
            item.Hue = hue;
            return item;
        }

        public Item Layered(Item item, Layer layer)
        {
            item.Layer = layer;
            return item;
        }

        public Item Resourced(BaseWeapon weapon, CraftResource resource)
        {
            weapon.Resource = resource;
            return weapon;
        }

        public Item Resourced(BaseArmor armor, CraftResource resource)
        {
            armor.Resource = resource;
            return armor;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();
            Unregister();
        }

        public override void OnDeath(Container c)
        {
            if (Town != null && DropCheck)
            {
                Mobile maxDamager = FindMostTotalDamger(false);

                if (maxDamager != null)
                {
                    Type thisType = GetType();

                    foreach (GuardList gl in Town.GuardLists)
                    {
                        if (gl.Definition.Type.IsEquivalentTo(thisType))
                        {
                            int price = gl.Definition.Price * Town.Definition.CostFactor / 100;

                            var check = new TreasuryCheck((int)(price * 0.75));

                            if (maxDamager.AddToBackpack(check))
                                maxDamager.SendMessage("You retrieve a large payment from the faction guard's corpse. Bring this to any town's treasury!");

                            else
                                check.Delete();
                        }
                    }
                }
            }

            base.OnDeath(c);

            c.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); //Version

            writer.Write(DropCheck);
            writer.Write(KothGuard);

            Town.WriteReference(writer, m_Town);

            m_Orders.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 2:
                    {
                        DropCheck = reader.ReadBool();
                        KothGuard = reader.ReadBool();

                        goto case 1;
                    }

                case 1:
                    goto case 0;

                case 0:
                    {
                        if (version < 1)
                            Faction.ReadReference(reader);

                        m_Town = Town.ReadReference(reader);
                        m_Orders = new Orders(this, reader);
                    }

                    break;
            }

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Register));
        }
    }

    public class VirtualMount : IMount
    {
        private VirtualMountItem m_Item;

        public Mobile Rider
        {
            get { return m_Item.Rider; }
            set { }
        }

        public VirtualMount(VirtualMountItem item)
        {
            m_Item = item;
        }

        public virtual void OnRiderDamaged(int amount, Mobile from, bool willKill)
        {
        }
    }

    public class VirtualMountItem : Item, IMountItem
    {
        private Mobile m_Rider;
        private VirtualMount m_Mount;

        public Mobile Rider { get { return m_Rider; } }

        public VirtualMountItem(Mobile mob)
            : base(0x3EA0)
        {
            Layer = Layer.Mount;

            m_Rider = mob;
            m_Mount = new VirtualMount(this);
        }

        public IMount Mount
        {
            get { return m_Mount; }
        }

        public VirtualMountItem(Serial serial)
            : base(serial)
        {
            m_Mount = new VirtualMount(this);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((Mobile)m_Rider);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Rider = reader.ReadMobile();

            if (m_Rider == null)
                Delete();
        }
    }
}