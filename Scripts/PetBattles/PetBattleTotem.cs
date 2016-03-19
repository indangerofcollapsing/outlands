using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom
{
    public class PetBattleTotem : Item
    {        
        public PetBattleSignupStone m_PetBattleSignupStone;
        [CommandProperty(AccessLevel.Administrator)]
        public PetBattleSignupStone PetBattleSignupStone
        {
            get { return m_PetBattleSignupStone; }
            set { m_PetBattleSignupStone = value; }
        }
       
        public PetBattle.Team m_Team;
        [CommandProperty(AccessLevel.Administrator)]
        public PetBattle.Team Team
        {
            get { return m_Team; }
            set { m_Team = value; }
        }
        
        public PlayerMobile m_Player;
        [CommandProperty(AccessLevel.Administrator)]
        public PlayerMobile Player
        {
            get { return m_Player; }
            set { m_Player = value; }
        }
        
        public BaseCreature m_Creature;
        [CommandProperty(AccessLevel.Administrator)]
        public BaseCreature Creature
        {
            get { return m_Creature; }
            set { m_Creature = value; }
        }

        public PetBattleStartTile m_PetBattleStartTile;
        [CommandProperty(AccessLevel.Administrator)]
        public PetBattleStartTile PetBattleStartTile
        {
            get { return m_PetBattleStartTile; }
            set { m_PetBattleStartTile = value; }
        }

        public List<Item> m_Items = new List<Item>();

        public List<PetBattleLantern> m_OffensiveLanterns = new List<PetBattleLantern>();
        public List<PetBattleLantern> m_DefensiveLanterns = new List<PetBattleLantern>();
        public List<PetBattleLantern> m_OpportunityLanterns = new List<PetBattleLantern>();

        public int offensivePowerMax = 1;
        public int defensivePowerMax = 1;
        public int opportunityPowerMax = 1;

        public int baseZ;
        public int offsetZ;

        public bool m_Active = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool Active
        {
            get { return m_Active; }
            set { m_Active = value; }
        }

        public bool m_Unlocked = false;
        [CommandProperty(AccessLevel.Administrator)]
        public bool Unlocked
        {
            get
            {
                return m_Unlocked; 
            }

            set
            { 
                m_Unlocked = value;
            }
        }

        private int m_TeamNumber = 1;
        [CommandProperty(AccessLevel.Administrator)]
        public int TeamNumber
        {
            get { return m_TeamNumber; }
            set { m_TeamNumber = value; }
        }

        private int m_PositionNumber = 1;
        [CommandProperty(AccessLevel.Administrator)]
        public int PositionNumber
        {
            get { return m_PositionNumber; }
            set { m_PositionNumber = value; }
        }

        private int m_OffensivePower = 0;
        [CommandProperty(AccessLevel.Administrator)]
        public int OffensivePower
        {
            get
            {
                return m_OffensivePower;
            }

            set
            {
                if (value > offensivePowerMax)
                    m_OffensivePower = offensivePowerMax;
                else if (value < 0)
                    m_OffensivePower = 0;
                else
                    m_OffensivePower = value;

                UpdateOffensivePower();
            }
        }

        private int m_DefensivePower = 0;
        [CommandProperty(AccessLevel.Administrator)]
        public int DefensivePower
        {
            get
            {
                return m_DefensivePower;
            }

            set
            {
                if (value > defensivePowerMax)
                    m_DefensivePower = defensivePowerMax;
                else if (value < 0)
                    m_DefensivePower = 0;
                else
                    m_DefensivePower = value;

                UpdateDefensivePower();
            }
        }

        private int m_OpportunityPower = 0;
        [CommandProperty(AccessLevel.Administrator)]
        public int OpportunityPower
        {
            get { return m_OpportunityPower; }
            set
            {
                m_OpportunityPower = value;

                UpdateOpportunityPower();
            }
        }

        [Constructable]
        public PetBattleTotem()
            : base(8501)
        {
            Visible = false;
            Movable = false;

            m_Items = new List<Item>();

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(AddComponents));
        }

        public PetBattleTotem(Serial serial)
            : base(serial)
        {
        }

        public void AddComponents()
        {
            if (Deleted)
                return;

            Item tileA = new Static(0x3F3);
            tileA.Name = "a pet battle totem";

            Item tileB = new Static(0x3F5);
            tileB.Name = "a pet battle totem";

            Item tileC = new Static(0x3F6);
            tileC.Name = "a pet battle totem";

            Item tileD = new Static(0x3F4);
            tileD.Name = "a pet battle totem";

            Item Pedestal = new Static(0x1223);
            Pedestal.Name = "a pet battle totem";
            Pedestal.Hue = 451;
            Pedestal.Movable = false;

            //Offensive Lanterns
            PetBattleLantern lantern1 = new PetBattleLantern(this, PetAbilityType.Offensive, 1);
            PetBattleLantern lantern2 = new PetBattleLantern(this, PetAbilityType.Offensive, 2);
            PetBattleLantern lantern3 = new PetBattleLantern(this, PetAbilityType.Offensive, 3);
            PetBattleLantern lantern4 = new PetBattleLantern(this, PetAbilityType.Offensive, 4);
            PetBattleLantern lantern5 = new PetBattleLantern(this, PetAbilityType.Offensive, 5);

            //Defensive Lanterns
            PetBattleLantern lantern6 = new PetBattleLantern(this, PetAbilityType.Defensive, 1);
            PetBattleLantern lantern7 = new PetBattleLantern(this, PetAbilityType.Defensive, 2);
            PetBattleLantern lantern8 = new PetBattleLantern(this, PetAbilityType.Defensive, 3);
            PetBattleLantern lantern9 = new PetBattleLantern(this, PetAbilityType.Defensive, 4);
            PetBattleLantern lantern10 = new PetBattleLantern(this, PetAbilityType.Defensive, 5);

            //Opportunity Lanterns
            PetBattleLantern lantern11 = new PetBattleLantern(this, PetAbilityType.Opportunity, 1);

            //Start Tiles
            PetBattleStartTile startTile = new PetBattleStartTile();
            startTile.Name = "a pet battle start tile";

            m_PetBattleStartTile = startTile;

            GroupItem(tileA, -1, -1, 0);
            GroupItem(tileB, 0, -1, 0);
            GroupItem(tileC, -1, 0, 0);
            GroupItem(tileD, 0, 0, 0);

            GroupItem(Pedestal, 0, 0, 11);

            GroupItem(startTile, 0, 1, 0);

            GroupItem(lantern1, -1, 0, 2);
            GroupItem(lantern2, -1, 0, 5);
            GroupItem(lantern3, -1, 0, 8);
            GroupItem(lantern4, -1, 0, 11);
            GroupItem(lantern5, -1, 0, 14);

            GroupItem(lantern6, 0, -1, 2);
            GroupItem(lantern7, 0, -1, 5);
            GroupItem(lantern8, 0, -1, 8);
            GroupItem(lantern9, 0, -1, 11);
            GroupItem(lantern10, 0, -1, 14);

            GroupItem(lantern11, 0, 0, 2);

            m_OffensiveLanterns.Add(lantern1);
            m_OffensiveLanterns.Add(lantern2);
            m_OffensiveLanterns.Add(lantern3);
            m_OffensiveLanterns.Add(lantern4);
            m_OffensiveLanterns.Add(lantern5);

            m_DefensiveLanterns.Add(lantern6);
            m_DefensiveLanterns.Add(lantern7);
            m_DefensiveLanterns.Add(lantern8);
            m_DefensiveLanterns.Add(lantern9);
            m_DefensiveLanterns.Add(lantern10);

            m_OpportunityLanterns.Add(lantern11);

            UpdateOffensivePower();
            UpdateDefensivePower();
            UpdateOpportunityPower();

            baseZ = this.Z;

            SetCreature(8501, -1, 25);

            Visible = false;
            Movable = false;
        }

        public virtual void GroupItem(Item item, int xOffset, int yOffset, int zOffset)
        {
            if (item != null)
                item.Movable = false;

            m_Items.Add(item);

            item.MoveToWorld(new Point3D(X + xOffset, Y + yOffset, Z + zOffset), Map);
        }

        public void Reset()
        {
            m_Team = null;
            m_Player = null;            

            if (m_Creature.Corpse != null)
                m_Creature.Corpse.Delete();
            m_Creature.Delete();
            m_Creature = null;

            offensivePowerMax = 1;
            defensivePowerMax = 1;
            opportunityPowerMax = 1;

            m_Active = false;
            m_Unlocked = false;

            OffensivePower = 0;
            DefensivePower = 0;
            OpportunityPower = 0;

            SetCreature(8501, -1, 25);

            Visible = false;
            Movable = false;

            foreach (PetBattleLantern offensiveLantern in m_OffensiveLanterns)
            {
                offensiveLantern.Visible = false;
            }

            foreach (PetBattleLantern defensiveLantern in m_DefensiveLanterns)
            {
                defensiveLantern.Visible = false;
            }

            foreach (PetBattleLantern opportunityLantern in m_OpportunityLanterns)
            {
                opportunityLantern.Visible = false;
            }
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            for (int i = 0; i < m_Items.Count; ++i)
                m_Items[i].Delete();

            if (m_Creature != null)
                m_Creature.Delete();

            this.Delete();
        }

        public void SetCreature(int creatureItemId, int hue, int newOffsetZ)
        {
            ItemID = creatureItemId;
            
            if (hue != -1)
                Hue = hue;

            offsetZ = newOffsetZ;
            Z = baseZ + offsetZ;

            Visible = true;
            Movable = false;
        }       

        public void IncreasePower()
        {
            if (!m_Unlocked || m_Creature == null)
                return;

            bool frozen = false;

            m_Creature.PetBattleAbilityEntryLookupInProgress = true;

            foreach (PetBattleAbilityEffectEntry entry in m_Creature.PetBattleAbilityEffectEntries)
            {
                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.Frozen)
                    return;
            }

            m_Creature.PetBattleAbilityEntryLookupInProgress = false;
            
            if (OffensivePower < offensivePowerMax && OffensivePower < 5)
                OffensivePower++;

            if (DefensivePower < defensivePowerMax && DefensivePower < 5)
                DefensivePower++;            
        }

        public void IncreaseOpportunityPower()
        {
            if (!m_Unlocked || m_Creature == null)
                return;
            
            if (OpportunityPower < opportunityPowerMax)
               OpportunityPower++;
        }

        public void UpdateOffensivePower()
        {
            for (int a = 0; a < m_OffensiveLanterns.Count; a++)
            {
                PetBattleLantern lantern = m_OffensiveLanterns[a];

                if (a < m_OffensivePower)
                    lantern.Ignite();
                else
                    lantern.Douse();
            }
        }

        public void UpdateDefensivePower()
        {
            for (int a = 0; a < m_DefensiveLanterns.Count; a++)
            {
                PetBattleLantern lantern = m_DefensiveLanterns[a];

                if (a < m_DefensivePower)
                    lantern.Ignite();
                else
                    lantern.Douse();
            }
        }

        public void UpdateOpportunityPower()
        {
            for (int a = 0; a < m_OpportunityLanterns.Count; a++)
            {
                PetBattleLantern lantern = m_OpportunityLanterns[a];

                if (a < m_OpportunityPower)
                    lantern.Ignite();
                else
                    lantern.Douse();
            }
        }

        public void LanternClick(Mobile from, PetBattleTotem petBattleLantern, PetAbilityType abilityType, int position)
        {
            if (from == null || petBattleLantern == null)
                return;

            switch (abilityType)
            {
                case PetAbilityType.Offensive:
                    if (OffensivePower < position)
                        from.SendMessage("Your creature does not have enough offensive power yet to use that ability.");
                    else
                    {
                        petBattleLantern.OffensivePower -= position;
                        petBattleLantern.m_Creature.m_PetBattleOffensiveAbilities[position - 1].m_Action.Invoke();
                    }
                break;

                case PetAbilityType.Defensive:
                    if (DefensivePower < position)
                        from.SendMessage("Your creature does not have enough defensive power yet to use that ability.");
                    else
                    {
                        petBattleLantern.DefensivePower -= position;
                        petBattleLantern.m_Creature.m_PetBattleDefensiveAbilities[position - 1].m_Action.Invoke();
                    }
                break;

                case PetAbilityType.Opportunity:
                    if (OpportunityPower < position)
                        from.SendMessage("You do not have an opportunity available.");
                    else
                    {
                        petBattleLantern.OpportunityPower -= position;
                        PetBattleOpportunityAbilities.UseOpportunityAbility(petBattleLantern.m_Creature, position);
                    }
                break;
            }
        }

        public override void OnSingleClick(Mobile from)
        {
            if (!m_Active)
            {
                LabelTo(from, "a pet battle creature totem");
                return;
            }

            if (m_Creature == null || m_Team == null)
            {
                LabelTo(from, "a pet battle creature totem");
                return;
            }

            if (m_Team.m_Player == null)
            {
                LabelTo(from, "a pet battle creature totem");
                return;
            }

            NetState ns = from.NetState;

            if (ns != null)
            {
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, m_Team.textHue, 3, "ENU", "", m_Creature.PetBattleTitle));
                ns.Send(new UnicodeMessage(Serial, ItemID, MessageType.Label, m_Team.textHue, 3, "ENU", "", "[" + m_Team.m_Player.RawName + "]"));             
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendMessage("Target a PetBattle signup stone to bind this to");
                from.Target = new PetBattleTotemTarget(this);

                return;
            }

            else
            {
                if (from == m_Player && m_Creature != null)
                {
                    PetBattleCreatureCollection playerCreatureCollection = PetBattlePersistance.GetPlayerPetBattleCreatureCollection(m_Player);
                    
                    for (int a = 0; a < playerCreatureCollection.m_CreatureEntries.Count; a++)
                    {
                        if (playerCreatureCollection.m_CreatureEntries[a].m_Type == this.m_Creature.GetType())
                        {
                            int page = (a * 2) + 1;

                            m_Player.CloseGump(typeof(Gumps.PetBattleGrimoireGump));
                            m_Player.SendGump(new Gumps.PetBattleGrimoireGump(page, m_Team, m_Player, playerCreatureCollection));

                            m_Player.PlaySound(0x055);

                            break;
                        }
                    }                                                         
                }
            }            
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version    

            writer.Write(m_PetBattleSignupStone);

            writer.Write(m_Player);

            writer.Write(m_PetBattleStartTile);
            writer.Write(m_Creature);
            
            writer.Write((int)m_Items.Count);
            foreach (Item item in m_Items)
            {
                writer.Write(item);
            }

            writer.Write((int)m_OffensiveLanterns.Count);
            foreach (Item lantern in m_OffensiveLanterns)
            {
                writer.Write(lantern);
            }

            writer.Write((int)m_DefensiveLanterns.Count);
            foreach (Item lantern in m_DefensiveLanterns)
            {
                writer.Write(lantern);
            }

            writer.Write((int)m_OpportunityLanterns.Count);
            foreach (Item lantern in m_OpportunityLanterns)
            {
                writer.Write(lantern);
            }

            writer.Write(offensivePowerMax);
            writer.Write(defensivePowerMax);
            writer.Write(opportunityPowerMax);

            writer.Write(baseZ);
            writer.Write(offsetZ);

            writer.Write(m_Active);
            writer.Write(m_Unlocked);
            writer.Write(m_TeamNumber);
            writer.Write(m_PositionNumber);

            writer.Write(m_OffensivePower);
            writer.Write(m_DefensivePower);
            writer.Write(m_OpportunityPower);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_Items = new List<Item>();
            m_OffensiveLanterns = new List<PetBattleLantern>();
            m_DefensiveLanterns = new List<PetBattleLantern>();

            if (version >= 0)
            {
                m_PetBattleSignupStone = reader.ReadItem() as PetBattleSignupStone;
                m_Player = reader.ReadMobile() as PlayerMobile;
                m_PetBattleStartTile = reader.ReadItem() as PetBattleStartTile;
                m_Creature = reader.ReadMobile() as BaseCreature;
                
                int itemsCount = reader.ReadInt();
                
                for (int i = 0; i < itemsCount; ++i)
                {
                    m_Items.Add(reader.ReadItem());
                }
                                
                int offensiveLanternsCount = reader.ReadInt();
                for (int i = 0; i < offensiveLanternsCount; ++i)
                {
                    m_OffensiveLanterns.Add(reader.ReadItem() as PetBattleLantern);
                }
                
                int defensiveLanternsCount = reader.ReadInt();
                for (int i = 0; i < defensiveLanternsCount; ++i)
                {
                    m_DefensiveLanterns.Add(reader.ReadItem() as PetBattleLantern);
                }

                int opportunityLanternsCount = reader.ReadInt();
                for (int i = 0; i < opportunityLanternsCount; ++i)
                {
                    m_OpportunityLanterns.Add(reader.ReadItem() as PetBattleLantern);
                }

                offensivePowerMax = reader.ReadInt();
                defensivePowerMax = reader.ReadInt();
                opportunityPowerMax = reader.ReadInt();

                baseZ = reader.ReadInt();
                offsetZ = reader.ReadInt();

                m_Active = reader.ReadBool();
                m_Unlocked = reader.ReadBool();
                m_TeamNumber = reader.ReadInt();
                m_PositionNumber = reader.ReadInt();

                m_OffensivePower = reader.ReadInt();
                m_DefensivePower = reader.ReadInt();
                m_OpportunityPower = reader.ReadInt();                        
            }
        }
    }

    public class PetBattleLantern : Lantern
    {
        PetBattleTotem m_PetBattleTotem;
        PetAbilityType m_AbilityType;
        int m_Position;

        [Constructable]
        public PetBattleLantern(PetBattleTotem petBattleTotem, PetAbilityType abilityType, int position)
        {
            m_PetBattleTotem = petBattleTotem;
            m_AbilityType = abilityType;
            m_Position = position;

            switch (m_AbilityType)
            {
                case PetAbilityType.Offensive:
                    Name = "a pet battle offensive ability lantern";
                    Hue = 601;           
                break;

                case PetAbilityType.Defensive:
                    Name = "a pet battle defensive ability lantern";
                    Hue = 460;
                break;

                case PetAbilityType.Opportunity:
                    Name = "a pet battle opportunity lantern";
                    Hue = 334;
                break;
            }     

            Movable = false;
            Visible = false;
        }

        public PetBattleLantern(Serial serial)
            : base(serial)
        {
        }

        public override void OnSingleClick(Mobile from)
        {
            if (m_PetBattleTotem == null)
                return;

            if (!m_PetBattleTotem.m_Active)
            {
                LabelTo(from, this.Name);
                return;
            }

            if (m_PetBattleTotem.m_Creature == null)
            {
                LabelTo(from, this.Name);
                return;
            }

            if (m_PetBattleTotem.m_Creature.m_PetBattleOffensiveAbilities == null)
            {
                LabelTo(from, this.Name);
                return;
            }

            if (m_PetBattleTotem.m_Creature.m_PetBattleDefensiveAbilities == null)
            {
                LabelTo(from, this.Name);
                return;
            }

            switch (m_AbilityType)
            {
                case PetAbilityType.Offensive:
                    if (m_PetBattleTotem.m_Creature.m_PetBattleOffensiveAbilities.Count >= m_Position)
                    {
                        string abilityName = m_PetBattleTotem.m_Creature.m_PetBattleOffensiveAbilities[m_Position - 1].m_Name;

                        LabelTo(from, abilityName);
                        LabelTo(from, "[Offensive Ability]");

                        m_PetBattleTotem.m_Player.SendMessage(m_PetBattleTotem.m_Creature.m_PetBattleOffensiveAbilities[m_Position - 1].m_Name + ": " + m_PetBattleTotem.m_Creature.m_PetBattleOffensiveAbilities[m_Position - 1].m_Description);
                    }
                break;

                case PetAbilityType.Defensive:
                    if (m_PetBattleTotem.m_Creature.m_PetBattleDefensiveAbilities.Count >= m_Position)
                    {
                        string abilityName = m_PetBattleTotem.m_Creature.m_PetBattleDefensiveAbilities[m_Position - 1].m_Name;

                        LabelTo(from, abilityName);
                        LabelTo(from, "[Defensive Ability]");

                        m_PetBattleTotem.m_Player.SendMessage(m_PetBattleTotem.m_Creature.m_PetBattleDefensiveAbilities[m_Position - 1].m_Name + ": " + m_PetBattleTotem.m_Creature.m_PetBattleDefensiveAbilities[m_Position - 1].m_Description);
                    }
                break;

                case PetAbilityType.Opportunity:
                    LabelTo(from, "Opportunity");

                    m_PetBattleTotem.m_Player.SendMessage("Opportunity: Grants your creature a randomized bonus");
                break;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null || m_PetBattleTotem == null)
                return;

            if (!m_PetBattleTotem.Unlocked)
                return;

            if (m_PetBattleTotem.m_Team == null)
                return;

            if (m_PetBattleTotem.m_Team.m_Player == null)
                return;

            bool frozen = false;

            m_PetBattleTotem.m_Creature.PetBattleAbilityEntryLookupInProgress = true;

            foreach (PetBattleAbilityEffectEntry entry in m_PetBattleTotem.m_Creature.PetBattleAbilityEffectEntries)
            {
                if (entry.m_PetBattleAbilityEffect == PetBattleAbilityEffect.Frozen)
                {
                    from.SendMessage("Your creature is under the effect of an ability preventing the use of powers.");
                    return;
                }
            }

            m_PetBattleTotem.m_Creature.PetBattleAbilityEntryLookupInProgress = false;

            if (from == m_PetBattleTotem.m_Team.m_Player)
                m_PetBattleTotem.LanternClick(from, m_PetBattleTotem, m_AbilityType, m_Position);

            else
                from.SendMessage("You are not allowed to use that.");
        }

        public override void Ignite()
        {
            if (!m_BurntOut)
            {
                Burning = true;
                ItemID = LitItemID;
                DoTimer(m_Duration);
            }
        }

        public override void Douse()
        {
            m_Burning = false;

            if (m_BurntOut && BurntOutItemID != 0)
                ItemID = BurntOutItemID;
            else
                ItemID = UnlitItemID;

            if (m_BurntOut)
                m_Duration = TimeSpan.Zero;
            else if (m_Duration != TimeSpan.Zero)
                m_Duration = m_End - DateTime.UtcNow;

            if (m_Timer != null)
                m_Timer.Stop();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_PetBattleTotem);
            writer.Write((int)(m_AbilityType));
            writer.Write(m_Position);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_PetBattleTotem = reader.ReadItem() as PetBattleTotem;
                m_AbilityType = (PetAbilityType)reader.ReadInt();
                m_Position = reader.ReadInt();
            }
        }
    }

    public class PetBattleStartTile : Item
    {
        private PetBattleTotem m_PetBattleTotem;
        [CommandProperty(AccessLevel.Administrator)]
        public PetBattleTotem PetBattleTotem
        {
            get { return m_PetBattleTotem; }
            set { m_PetBattleTotem = value; }
        }

        [Constructable]
        public PetBattleStartTile()
            : base(13854)
        {
            Name = "a Pet Battle start tile";
            Visible = false;
            Movable = false;
        }

        public PetBattleStartTile(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendMessage("Target a Pet Battle totem to bind this to");
                from.Target = new PetBattleStartTileTarget(this);
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); //version  

            writer.Write(m_PetBattleTotem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version >= 0)
            {
                m_PetBattleTotem = reader.ReadItem() as PetBattleTotem;
            }
        }
    }

    public class PetBattleTotemTarget : Target
    {
        public PetBattleTotem m_PetBattleTotem;

        public PetBattleTotemTarget(PetBattleTotem petBattleTotem)
            : base(-1, false, TargetFlags.None)
        {
            m_PetBattleTotem = petBattleTotem;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is PetBattleSignupStone && m_PetBattleTotem != null)
            {
                PetBattleSignupStone petBattleSignupStone = target as PetBattleSignupStone;

                m_PetBattleTotem.m_PetBattleSignupStone = petBattleSignupStone;
                petBattleSignupStone.AddTotem(m_PetBattleTotem);
            }

            else
            {
                from.SendMessage("That is not a Pet Battle signup stone.");
            }
        }
    }


    public class PetBattleStartTileTarget : Target
    {
        public PetBattleStartTile m_petBattleStartTile;

        public PetBattleStartTileTarget(PetBattleStartTile petBattleStartTile)
            : base(-1, false, TargetFlags.None)
        {
            m_petBattleStartTile = petBattleStartTile;
        }

        protected override void OnTarget(Mobile from, object target)
        {
            if (target is PetBattleTotem && m_petBattleStartTile != null)
            {
                PetBattleTotem petBattleTotem = target as PetBattleTotem;

                m_petBattleStartTile.PetBattleTotem = petBattleTotem;
                petBattleTotem.m_PetBattleStartTile = m_petBattleStartTile;
            }

            else
            {
                from.SendMessage("That is not a Pet Battle totem.");
            }
        }
    }
}