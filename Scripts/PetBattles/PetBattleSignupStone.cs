using System;
using Server;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;

namespace Server.Custom
{
    public class PetBattleSignupStone : Item
    {
        public override string DefaultName { get { return "a pet battle signup stone"; } }

        public PetBattle m_PetBattle;
        [CommandProperty(AccessLevel.Administrator)]
        public PetBattle PetBattle
        {
            get { return m_PetBattle; }
            set { m_PetBattle = value; }
        }

        private Mobile m_Announcer;
        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Announcer
        {
            get { return m_Announcer; }
            set 
            {
                m_Announcer = value;

                PetBattle.Announcer = m_Announcer;
            }
        }

        public List<PetBattleTotem> m_PetBattleTotems = new List<PetBattleTotem>();
        [CommandProperty(AccessLevel.Administrator)]
        public List<PetBattleTotem> PetBattleTotems
        {
            get { return m_PetBattleTotems; }
            set { m_PetBattleTotems = value; }
        }
        
        [Constructable]
        public PetBattleSignupStone()
            : base(0x0EDE)
        {
            Hue = 761;
            Movable = false;

            m_PetBattle = new PetBattle(this);
            m_PetBattle.MoveToWorld(this.Location);
        }

        public PetBattleSignupStone(Serial serial)
            : base(serial)
        {
        }

        public bool AddTotem(PetBattleTotem petBattleTotem)
        {
            if (m_PetBattleTotems.IndexOf(petBattleTotem) < 0)
            {
                m_PetBattleTotems.Add(petBattleTotem);
                return true;
            }

            return false;
        }

        public override void OnSingleClick(Mobile from)
        {
            if (!(from is PlayerMobile) || m_PetBattle == null)
                return;

            var pm = from as PlayerMobile;

            if (m_PetBattle.FindPlayerInWaitingList(pm))
            {
                int playerPosition = m_PetBattle.WaitingList.IndexOf(pm) + 1;
                int totalPlayers = m_PetBattle.WaitingList.Count;

                from.SendMessage("You are player " + playerPosition.ToString() + " out of " + totalPlayers.ToString() + " in the Pet Battle queue.");
            }

            else
            {
                LabelTo(from, "a Pet Battle signup stone");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!PetBattlePersistance.Enabled)
            {
                from.SendMessage("Pet Battles are currently disabled.");
                return;
            }

            var pm = from as PlayerMobile;

            if (pm == null || m_PetBattle == null)
                return;

            //Either Waiting on Confirmation or Already Participating
            if (m_PetBattle.FindPlayerInNeedConfirmationList(pm) || m_PetBattle.FindPlayerInReadyList(pm))
            {
                switch (m_PetBattle.CurrentState)
                {
                    case PetBattleState.WaitingForPlayers:
                    break;
                    
                    case PetBattleState.ConfirmingPlayers:
                    if (m_PetBattle.FindPlayerInNeedConfirmationList(pm))
                    {
                        //NEED GUMP TO ALLOW PLAYER TO CONFIRM OR QUIT QUEUE
                        m_PetBattle.AddPlayerToReadyList(pm);
                        m_PetBattle.RemovePlayerFromNeedConfirmationList(pm);

                        from.SendMessage("You confirm your participation in the next Pet Battle.");
                    }
                    break;

                    case PetBattleState.DeterminingRules:
                    if (m_PetBattle.FindPlayerInReadyList(pm))
                    {
                        pm.CloseGump(typeof(Gumps.PetBattleDetermineRulesGump));
                        pm.SendGump(new Gumps.PetBattleDetermineRulesGump(m_PetBattle, pm));
                    }
                    break;

                    case PetBattleState.SelectingCreatures:
                    if (m_PetBattle.FindPlayerInReadyList(pm))
                    {                            
                        from.SendGump(new Gumps.PetBattleSelectCreaturesGump(m_PetBattle, pm));
                    }
                    break;

                    case PetBattleState.Battling:
                    if (m_PetBattle.FindPlayerInReadyList(pm))
                    {
                    }
                    break;

                    case PetBattleState.PostBattle:
                    if (m_PetBattle.FindPlayerInReadyList(pm))
                    {

                    }
                    break;
                }
            }

            //Not Yet Participating
            else
            {
                //If Already in Wait List
                if (m_PetBattle.FindPlayerInWaitingList(pm))
                {
                    //ADD PROMPT TO LEAVE WAITING LIST                        
                    m_PetBattle.RemovePlayerFromWaitingList(pm);
                    from.SendMessage("You have been removed from the Pet Battle queue.");

                    return;
                }

                //Not In Wait List
                else
                {
                    //Not Qualified
                    if (!MeetsSignupRequirements(pm))                    
                        return;

                    //Qualifies For Pet Battle
                    else
                    {
                        m_PetBattle.AddPlayerToWaitingList(pm);
                        from.SendMessage("You have entered the Pet Battle queue.");

                        return;
                    }
                }
            }
        }

        //Sign Up Criteria
        public bool MeetsSignupRequirements(PlayerMobile pm)
        {   
            if (!pm.PetBattleUnlocked)
            {
                pm.SendMessage("You have not yet purchased a membership for participation in the Pet Battle League.");
                return false;
            }

            if (Banker.GetBalance(pm) < PetBattle.baseFee[0])
            {
                pm.SendMessage("You do not have the minimum battle fee of " + PetBattle.baseFee[0] + " gold in your bank.");
                return false;
            }

            if ((pm.LastPetBattleActivity + TimeSpan.FromSeconds(3)) > DateTime.UtcNow)
            {
                pm.SendMessage("You are currently in the queue or engaged for another Pet Battle arena and cannot join this arena at the moment.");
                return false;
            }

            return true;
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if (m_PetBattle != null)
                m_PetBattle.Delete();

            this.Delete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); //version

            writer.Write(m_PetBattle);

            writer.Write((int)m_PetBattleTotems.Count);
            foreach (PetBattleTotem totem in m_PetBattleTotems)
            {
                writer.Write(totem);
            }

            //Version 1
            writer.Write(m_Announcer);            
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_PetBattleTotems = new List<PetBattleTotem>();

            //Version 0
            if (version >= 0)
            {
                m_PetBattle = reader.ReadItem() as PetBattle;

                int entriesCount = reader.ReadInt();                

                for (int i = 0; i < entriesCount; ++i)
                {
                    PetBattleTotem totem = reader.ReadItem() as PetBattleTotem;
                    m_PetBattleTotems.Add(totem);
                }
            }

            //Version 1
            if (version >= 1)
            {
                m_Announcer = reader.ReadMobile();
            }
        }
    }
}