using System; 
using System.Collections.Generic; 
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Mobiles 
{ 
	public class InnKeeper : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public InnKeeper() : base( "the innkeeper" ) 
		{ 
		} 

		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBInnKeeper() ); 
			
			if ( IsTokunoVendor )
				m_SBInfos.Add( new SBSEFood() );
		} 

		public override VendorShoeType ShoeType
		{
			get{ return Utility.RandomBool() ? VendorShoeType.Sandals : VendorShoeType.Shoes; }
		}

        private class StableEntry : ContextMenuEntry
        {
            private InnKeeper m_Innkeeper;
            private Mobile m_From;

            public StableEntry(InnKeeper innkeeper, Mobile from): base(6126, 12)
            {
                m_Innkeeper = innkeeper;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Innkeeper.BeginStable(m_From);
            }
        }

        private class ClaimListGump : Gump
        {
            private InnKeeper m_Innkeeper;
            private Mobile m_From;
            private List<BaseCreature> m_List;

            public ClaimListGump(InnKeeper innkeeper, Mobile from, List<BaseCreature> list): base(50, 50)
            {
                m_Innkeeper = innkeeper;
                m_From = from;
                m_List = list;

                from.CloseGump(typeof(ClaimListGump));

                AddPage(0);

                AddBackground(0, 0, 450, 50 + (list.Count * 20), 9250);
                AddAlphaRegion(5, 5, 440, 40 + (list.Count * 20));

                AddHtml(15, 15, 420, 20,
                    string.Format("<BASEFONT COLOR=#FFFFFF>Select a henchman to retrieve (slot usage {0} out of {1}):</BASEFONT>", list.Count, InnKeeper.GetMaxStabled(from)), false, false);

                for (int i = 0; i < list.Count; ++i)
                {
                    BaseCreature pet = list[i];

                    if (pet == null || pet.Deleted || !pet.IsHenchman)
                        continue;

                    AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);
                    
                    BaseCreature creature = (BaseCreature)Activator.CreateInstance(pet.GetType());

                    AddHtml(32, 35 + (i * 20), 420, 18, String.Format("<BASEFONT COLOR=#C0C0EE>{0} {1} </BASEFONT>", creature.Name, creature.Title), false, false);
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID - 1;

                if (index >= 0 && index < m_List.Count)
                    m_Innkeeper.EndClaimList(m_From, m_List[index]);
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private InnKeeper m_Innkeeper;
            private Mobile m_From;

            public ClaimAllEntry(InnKeeper innkeeper, Mobile from): base(6127, 12)
            {
                m_Innkeeper = innkeeper;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Innkeeper.Claim(m_From);
            }
        }

        public override void AddCustomContextEntries(Mobile from, List<ContextMenuEntry> list)
        {
            if (from.Alive)
            {
                list.Add(new StableEntry(this, from));

                if (from.Stabled.Count > 0)
                    list.Add(new ClaimAllEntry(this, from));
            }

            base.AddCustomContextEntries(from, list);
        }

        public static int GetMaxStabled(Mobile from)
        {
            //Add null check just in case
            if (from == null)
                return 0;

            double begging = from.Skills[SkillName.Begging].Value;
            double camping = from.Skills[SkillName.Camping].Value;

            double sklsum = begging + camping;

            int max;

            if (sklsum >= 240.0)
                max = 5;

            else if (sklsum >= 200.0)
                max = 4;

            else if (sklsum >= 160.0)
                max = 3;
            else
                max = 2;

            if (begging >= 100.0)
                max += (int)((begging - 90.0) / 10);

            if (camping >= 100.0)
                max += (int)((camping - 90.0) / 10);

            //Increase the baseline max stable slot by 3 (hence crafters with 0 taming/lore/vet will be able to stable 5 x 1 slot packies). 
            return max + 3;
        }

        private class StableTarget : Target
        {
            private InnKeeper m_Innkeeper;

            public StableTarget(InnKeeper innkeeper): base(12, false, TargetFlags.None)
            {
                m_Innkeeper = innkeeper;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                BaseCreature bc_Target = targeted as BaseCreature;

                if (bc_Target != null)
                {
                    if (bc_Target.IsHenchman)
                        m_Innkeeper.EndStable(from, bc_Target);
                    else
                        m_Innkeeper.SayTo(from, "I'm sorry, but I cannot offer housing to that.");
                }

                else
                    m_Innkeeper.SayTo(from, "I'm sorry, but I cannot offer housing to that.");
            }
        }

        private void CloseClaimList(Mobile from)
        {
            from.CloseGump(typeof(ClaimListGump));
        }

        public void BeginClaimList(Mobile from)
        {
            if (Deleted || !from.CheckAlive())
                return;

            List<BaseCreature> list = new List<BaseCreature>();

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;                    

                    pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

                    from.Stabled.RemoveAt(i);
                    --i;

                    continue;
                }

                if (pet.IsHenchman)
                    list.Add(pet);
            }

            if (list.Count > 0)
                from.SendGump(new ClaimListGump(this, from, list));

            else
                SayTo(from, "I have no individuals housed here currently on your behalf.");
        }

        public void EndClaimList(Mobile from, BaseCreature pet)
        {
            if (pet == null || pet.Deleted || from.Map != this.Map || !from.Stabled.Contains(pet) || !from.CheckAlive())
                return;

            if (!from.InRange(this, 14))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
                return;
            }

            if (CanClaim(from, pet))
            {
                DoClaim(from, pet);

                from.Stabled.Remove(pet);

                if (from is PlayerMobile)
                    ((PlayerMobile)from).AutoStabled.Remove(pet);
            }

            else
                SayTo(from, "You have too many followers currently to command that individual.");          
        }

        public void BeginStable(Mobile from)
        {
            if (Deleted || !from.CheckAlive())
                return;

            Container bank = from.FindBankNoCreate();

            if ((from.Backpack == null || from.Backpack.GetAmount(typeof(Gold)) < 30) && (bank == null || bank.GetAmount(typeof(Gold)) < 30))
                SayTo(from, 1042556); // Thou dost not have enough gold, not even in thy bank account.

            else
            {
                SayTo(from, "I charge 30 gold per individual for a real week's housing. I will withdraw it from thy bank account. Who shall I house here?");
                from.Target = new StableTarget(this);
            }
        }

        public void EndStable(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive())
                return;

            if (!pet.IsHenchman)
                SayTo(from, "Alas, I do not run a stables.");

            else if (!pet.Controlled)
                SayTo(from, "I cannot offer housing to that.");

            else if (pet.ControlMaster != from)
                SayTo(from, "That individual is not yours to command.");

            else if (pet.IsDeadPet)
                SayTo(from, "I can only offer housing to the living.");

            else if (pet.Summoned)
                SayTo(from, "I cannot offer housing to summoned beings.");

            else if (pet.Combatant != null && pet.InRange(pet.Combatant, 12) && pet.Map == pet.Combatant.Map)
                SayTo(from, "I am sorry, but that individual seems busy at the moment.");

            else if (from.Stabled.Count >= GetMaxStabled(from))
                SayTo(from, "I am sorry, but you already have too many individuals housed here currently.");

            else
            {
                Container bank = from.FindBankNoCreate();

                if ((from.Backpack != null && from.Backpack.ConsumeTotal(typeof(Gold), 30)) || (bank != null && bank.ConsumeTotal(typeof(Gold), 30)))
                {
                    pet.ControlTarget = null;
                    pet.ControlOrder = OrderType.Stay;
                    pet.Internalize();

                    pet.SetControlMaster(null);
                    pet.SummonMaster = null;

                    pet.IsStabled = true;                    

                    pet.OwnerAbandonTime = DateTime.UtcNow + TimeSpan.FromDays(1000);
                    
                    from.Stabled.Add(pet);

                    SayTo(from, "I shall offer this individual housing. You may retrieve them by saying 'claim' to me. In one real world I week, I shall remove them from the premises if they have not been claimed.");
                }

                else
                    SayTo(from, 502677); // But thou hast not the funds in thy bank account!                
            }
        }

        public void Claim(Mobile from)
        {
            Claim(from, null);
        }

        public void Claim(Mobile from, string petName)
        {
            if (Deleted || !from.CheckAlive())
                return;

            bool claimed = false;
            int stabled = 0;

            bool claimByName = (petName != null);

            for (int i = 0; i < from.Stabled.Count; ++i)
            {
                BaseCreature pet = from.Stabled[i] as BaseCreature;

                if (pet == null || pet.Deleted)
                {
                    pet.IsStabled = false;

                    pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

                    from.Stabled.RemoveAt(i);
                    --i;

                    continue;
                }

                if (!pet.IsHenchman)
                    continue;

                ++stabled;

                if (claimByName && !Insensitive.Equals(pet.Name, petName))
                    continue;

                if (CanClaim(from, pet))
                {
                    DoClaim(from, pet);

                    from.Stabled.RemoveAt(i);

                    if (from is PlayerMobile)
                        ((PlayerMobile)from).AutoStabled.Remove(pet);

                    --i;

                    claimed = true;
                }

                else
                    SayTo(from, "You have too many followers currently to command that individual.");                    
            }

            if (claimed)
                SayTo(from, 1042559); // Here you go... and good day to you!

            else if (stabled == 0)
                SayTo(from, "I have no individuals housed here currently on your behalf.");

            else if (claimByName)
                BeginClaimList(from);
        }

        public bool CanClaim(Mobile from, BaseCreature pet)
        {
            return ((from.Followers + pet.ControlSlots) <= from.FollowersMax);
        }

        private void DoClaim(Mobile from, BaseCreature pet)
        {
            pet.SetControlMaster(from);

            if (pet.Summoned)
                pet.SummonMaster = from;

            pet.ControlTarget = from;
            pet.ControlOrder = OrderType.Follow;

            pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

            pet.MoveToWorld(from.Location, from.Map);

            pet.IsStabled = false;

            pet.ApplyExperience();

            pet.Hits = pet.HitsMax;
            pet.Stam = pet.StamMax;
            pet.Mana = pet.ManaMax;
        }

        public override bool HandlesOnSpeech(Mobile from)
        {
            return true;
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            Mobile from = e.Mobile;

            //Custom
            string text = e.Speech.Trim().ToLower();

            if (from.Alive)
            {
                if (text.IndexOf("i wish to house") != -1)
                {
                    e.Handled = true;

                    CloseClaimList(e.Mobile);
                    BeginStable(e.Mobile);

                    return;
                }
            }

            if (from.Alive)
            {
                if (text.IndexOf("housing") != -1)
                {
                    e.Handled = true;

                    CloseClaimList(e.Mobile);
                    BeginStable(e.Mobile);

                    return;
                }
            }

            //Standard
            if (!e.Handled && e.HasKeyword(0x0008)) // *stable*
            {
                e.Handled = true;

                CloseClaimList(e.Mobile);
                BeginStable(e.Mobile);
            }

            else if (!e.Handled && e.HasKeyword(0x0009)) // *claim*
            {
                e.Handled = true;

                CloseClaimList(e.Mobile);

                int index = e.Speech.IndexOf(' ');

                if (index != -1)
                    Claim(e.Mobile, e.Speech.Substring(index).Trim());

                else
                    Claim(e.Mobile);
            }

            else
                base.OnSpeech(e);
        }

		public InnKeeper( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	} 
}