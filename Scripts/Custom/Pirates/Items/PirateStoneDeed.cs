using System;
using Server.Items;
using Server.Guilds;
using Server.Multis;
using Server.Mobiles;

namespace Server.Custom.Pirates
{
    public class PirateStoneDeed : Item
    {
        public override int PlayerClassCurrencyValue { get { return 2500; } }
        
        [Constructable]
        public PirateStoneDeed(): base(0x14F0)
        {
            Name = "a Pirate Stone Deed";
            Weight = 1.0;
            Hue = 0x455;

            PlayerClass = PlayerClass.Pirate;
            PlayerClassRestricted = true;
        }

        public PirateStoneDeed(Serial serial): base(serial)
        {
        }        

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            PlayerClassPersistance.PlayerClassSingleClick(this, from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            BaseGuildDock dock;

            PlayerMobile pm_From = from as PlayerMobile;

            if (!IsChildOf(from.Backpack))            
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.            
            
            else if (!pm_From.Pirate && this.PlayerClassOwner != pm_From)
            {
                from.SendMessage("Only the owner of this item may use it");
                return;
            }

            else if (from.AccessLevel >= AccessLevel.GameMaster)
            {
                from.SendMessage("You use your GM powers to read the deed.");

                Guild m_guild = null as Guild;
                PirateStone stone = new PirateStone(m_guild);
                stone.MoveToWorld(from.Location, from.Map);
                
                Delete();
            }

            else if (from.Guild == null)            
                from.SendMessage("You must be in a guild to place a pirate stone.");
            
            else if (PirateStone.m_PirateStoneDictionary.ContainsKey((Guild)from.Guild))            
                from.SendMessage("Your guild can only place one pirate stone.");            

            else if (((Guild)from.Guild).Leader != from)            
                from.SendMessage("Only guild leaders can place a pirate stone.");      
      
            else
            {
                BaseHouse house = BaseHouse.FindHouseAt(from);

                if (house == null)                
                    from.SendMessage("You must be in a house to place a pirate stone.");                

                else if (!house.IsCoOwner(from))                
                    from.SendMessage("You must be at least a co-owner of the house to place a pirate stone.");     
           
                else
                {
                    BaseGuildDock.m_GuildDockDictionary.TryGetValue((Guild)from.Guild, out dock);
                    if (dock == null || dock.Deleted)
                    {
                        this.Delete();

                        Guild m_guild = from.Guild as Guild;

                        PirateStone stone = new PirateStone(m_guild);
                        stone.MoveToWorld(from.Location, from.Map);
                    }

                    else
                    {
                        if (from.InRange(dock.Location, 35))
                        {
                            this.Delete();

                            Guild m_guild = from.Guild as Guild;

                            PirateStone stone = new PirateStone(m_guild);
                            stone.MoveToWorld(from.Location, from.Map);
                        }

                        else                        
                            from.SendMessage("Your pirate stone must be placed closer to your guild docks.");                        
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}