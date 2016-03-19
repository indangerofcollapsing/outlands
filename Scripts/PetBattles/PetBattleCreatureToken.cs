using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
	public class PetBattleCreatureToken : Item
	{
        public virtual Type m_Type { get { return null; } }
        
        [Constructable]
        public PetBattleCreatureToken(): base()
		{           
			Weight = 1.0;

            BaseCreature creature = (BaseCreature)Activator.CreateInstance(m_Type);

            Name = "pet battle " + creature.PetBattleTitle + " token";

            ItemID = creature.PetBattleItemId;
            Hue = creature.PetBattleItemHue;

            creature.Delete();          
		}

        public PetBattleCreatureToken(Serial serial): base(serial)
		{
		}

        public override void OnSingleClick(Mobile from)
        {
            BaseCreature creature = (BaseCreature)Activator.CreateInstance(m_Type);

            LabelTo(from, " a pet battle " + creature.PetBattleTitle + " token");

            creature.Delete();
        }

		public override void OnDoubleClick( Mobile from )
		{
            PlayerMobile pm_From = from as PlayerMobile;

            if (pm_From == null)           
                return;            

            if (from.InRange(this.GetWorldLocation(), 1))
            {
                from.SendMessage("Target the player you wish to enable this Pet Battle creature for.");
                from.Target = new InternalTarget(this);
            }

            else
            {
                from.SendMessage("That is too far away to use.");
                return;
            }
		}

		private class InternalTarget : Target
		{
            private PetBattleCreatureToken m_PetBattleCreatureToken;

            public InternalTarget(PetBattleCreatureToken PetBattleCreatureUnlocker): base(8, false, TargetFlags.None)
            {
                m_PetBattleCreatureToken = PetBattleCreatureUnlocker;
			}

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (m_PetBattleCreatureToken == null )
                    return;

                if (m_PetBattleCreatureToken.Deleted)
                    return;

                if (!(targeted is PlayerMobile))
                {
                    from.SendMessage("You cannot grant this pet battle creature to that");
                    return;
                }

                BaseCreature creature = (BaseCreature)Activator.CreateInstance(m_PetBattleCreatureToken.m_Type);

                if (creature == null)
                    return;

                PlayerMobile pm_Target = targeted as PlayerMobile;

                if ((pm_Target.LastPetBattleActivity + TimeSpan.FromSeconds(3)) > DateTime.UtcNow)
                {
                    from.SendMessage("That player is currently in the queue for or engaged in a Pet Battle and cannot receive new creatures at the moment.");                    
                    return;
                }

                if (PetBattlePersistance.AddPetBattleCreature(pm_Target, m_PetBattleCreatureToken.m_Type))
                {
                    from.SendMessage("That player already has " + creature.PetBattleTitle + " unlocked for Pet Battles on their account.");
                    return;
                }

                else
                {
                    if (from == pm_Target)
                    {
                        from.SendMessage(creature.PetBattleTitle + " has now been unlocked for Pet Battles on your account.");
                        from.PlaySound(creature.GetIdleSound());
                    }

                    else
                    {
                        from.SendMessage(creature.PetBattleTitle + " has now been unlocked for Pet Battles on their account.");
                        pm_Target.SendMessage(creature.PetBattleTitle + " has now been unlocked for Pet Battles on your account.");

                        pm_Target.PlaySound(creature.GetIdleSound());
                    }
                }  
                    
                creature.Delete();
                m_PetBattleCreatureToken.Delete();
            }
		}

        public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write((int)0); //Version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}