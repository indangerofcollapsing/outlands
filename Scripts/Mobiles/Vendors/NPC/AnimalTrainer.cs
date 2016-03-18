using System;
using System.Collections.Generic;
using Server;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Targeting;
using Server.Custom;

namespace Server.Mobiles
{
    public class AnimalTrainer : BaseVendor
    {
        private List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }

        [Constructable]
        public AnimalTrainer(): base("the animal trainer")
        {
            SetSkill(SkillName.AnimalLore, 64.0, 100.0);
            SetSkill(SkillName.AnimalTaming, 90.0, 100.0);
            SetSkill(SkillName.Veterinary, 65.0, 88.0);
        }

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new SBAnimalTrainer());
        }

        public override VendorShoeType ShoeType
        {
            get { return Female ? VendorShoeType.ThighBoots : VendorShoeType.Boots; }
        }

        public override int GetShoeHue()
        {
            return 0;
        }

        public override void InitOutfit()
        {
            base.InitOutfit();

            AddItem(Utility.RandomBool() ? (Item)new QuarterStaff() : (Item)new ShepherdsCrook());
        }

        private class StableEntry : ContextMenuEntry
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;

            public StableEntry(AnimalTrainer trainer, Mobile from)
                : base(6126, 12)
            {
                m_Trainer = trainer;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Trainer.BeginStable(m_From);
            }
        }

        private class ClaimListGump : Gump
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;
            private List<BaseCreature> m_List;

            public ClaimListGump(AnimalTrainer trainer, Mobile from, List<BaseCreature> list)
                : base(50, 50)
            {
                m_Trainer = trainer;
                m_From = from;
                m_List = list;

                from.CloseGump(typeof(ClaimListGump));

                AddPage(0);

                AddBackground(0, 0, 450, 50 + (list.Count * 20), 9250);
                AddAlphaRegion(5, 5, 440, 40 + (list.Count * 20));

                AddHtml(15, 15, 425, 20,
                    string.Format("<BASEFONT COLOR=#FFFFFF>Click to Retrieve a Follower (slot usage {0} out of {1}):</BASEFONT>", list.Count, AnimalTrainer.GetMaxStabled(from)), false, false);

                for (int i = 0; i < list.Count; ++i)
                {
                    BaseCreature pet = list[i];

                    if (pet == null || pet.Deleted || pet.IsHenchman)
                        continue;

                    AddButton(15, 39 + (i * 20), 10006, 10006, i + 1, GumpButtonType.Reply, 0);

                    string bondedText = "";
                    if (pet.IsBonded)
                        bondedText = "(bonded)";
                    //Make sure that the year is legit
                    else if (pet.BondingBegin.Year > 2014 && pet.BondingBegin.Year > 2014)
                    {
                        /* Calculate time left and send it.. */

                        TimeSpan timeleft = pet.BondingBegin + pet.BondingDelay - DateTime.UtcNow;
                        if (timeleft <= new TimeSpan(0))
                            bondedText = "(ready to be bonded)";
                        else
                            bondedText = string.Format("(bonded in {0} days, {1} hours, and {2} minutes)", timeleft.Days,
                                timeleft.Hours, timeleft.Minutes);
                    }
                    BaseCreature creature = (BaseCreature)Activator.CreateInstance(pet.GetType());
                    if (creature.Name != pet.RawName)
                    {
                        string sName = creature.Name;

                        if (sName.IndexOf("an ") > -1)
                            sName = sName.Substring(2, sName.Length - 2);
                        else if (sName.IndexOf("a ") > -1)
                            sName = sName.Substring(1, sName.Length - 1);

                        sName = sName.Trim();

                        AddHtml(32, 35 + (i * 20), 420, 18, String.Format("<BASEFONT COLOR=#C0C0EE>{0} the {1} {2}</BASEFONT>", pet.Name, sName, bondedText), false, false);
                    }
                    else
                        AddHtml(32, 35 + (i * 20), 420, 18, String.Format("<BASEFONT COLOR=#C0C0EE>{0} {1}</BASEFONT>", pet.Name, bondedText), false, false);
                }
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                int index = info.ButtonID - 1;

                if (index >= 0 && index < m_List.Count)
                    m_Trainer.EndClaimList(m_From, m_List[index]);
            }
        }

        private class ClaimAllEntry : ContextMenuEntry
        {
            private AnimalTrainer m_Trainer;
            private Mobile m_From;

            public ClaimAllEntry(AnimalTrainer trainer, Mobile from)
                : base(6127, 12)
            {
                m_Trainer = trainer;
                m_From = from;
            }

            public override void OnClick()
            {
                m_Trainer.Claim(m_From);
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
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return 0;

            double taming = from.Skills[SkillName.AnimalTaming].Value;
            double anlore = from.Skills[SkillName.AnimalLore].Value;
            double vetern = from.Skills[SkillName.Veterinary].Value;

            double sklsum = taming + anlore + vetern;

            int max;

            if (sklsum >= 240.0)
                max = 5;

            else if (sklsum >= 200.0)
                max = 4;

            else if (sklsum >= 160.0)
                max = 3;

            else
                max = 2;

            if (taming >= 100.0)
                max += (int)((taming - 90.0) / 10);            

            if (anlore >= 100.0)
                max += (int)((anlore - 90.0) / 10);

            if (vetern >= 100.0)
                max += (int)((vetern - 90.0) / 10);

            max += 3;   
            
            if (PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.Herdsman))
                max += 5;  

            if (PlayerEnhancementPersistance.IsCustomizationEntryActive(player, CustomizationType.Rancher))
                max += 10;
            
            return max;
        }

        private class StableTarget : Target
        {
            private AnimalTrainer m_Trainer;

            public StableTarget(AnimalTrainer trainer)
                : base(12, false, TargetFlags.None)
            {
                m_Trainer = trainer;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is BaseCreature)
                    m_Trainer.EndStable(from, (BaseCreature)targeted);
                else if (targeted == from)
                    m_Trainer.SayTo(from, 502672); // HA HA HA! Sorry, I am not an inn.
                else
                    m_Trainer.SayTo(from, 1048053); // You can't stable that!
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
                    pet.StabledBy = null;

                    pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

                    from.Stabled.RemoveAt(i);
                    --i;

                    continue;
                }

                if (!pet.IsHenchman)
                    list.Add(pet);
            }

            if (list.Count > 0)
                from.SendGump(new ClaimListGump(this, from, list));

            else
                SayTo(from, 502671); // But I have no animals stabled with me at the moment!
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
                SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.            
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
                /* I charge 30 gold per pet for a real week's stable time.
                                         * I will withdraw it from thy bank account.
                                         * Which animal wouldst thou like to stable here?
                                         */

                from.SendLocalizedMessage(1042558);
                from.Target = new StableTarget(this);
            }
        }

        public void EndStable(Mobile from, BaseCreature pet)
        {
            if (Deleted || !from.CheckAlive())
                return;

            if (pet.IsHenchman)
                SayTo(from, "Perhaps you should seek an innkeeper to offer that individual lodging.");  

            else if (!pet.Controlled)
                SayTo(from, 1048053); // You can't stable that!

            else if (pet.ControlMaster != from)
                SayTo(from, 1042562); // You do not own that pet!      

            else if (pet.IsDeadPet)
                SayTo(from, 1049668); // Living pets only, please.

            else if (pet.Summoned)
                SayTo(from, 502673); // I can not stable summoned creatures.

            else if ((pet is PackLlama || pet is PackHorse || pet is Beetle || pet is ArmoredPackBear) && (pet.Backpack != null && pet.Backpack.Items.Count > 0))
                SayTo(from, 1042563); // You need to unload your pet.

            else if (pet.Combatant != null && pet.InRange(pet.Combatant, 12) && pet.Map == pet.Combatant.Map)
                SayTo(from, 1042564); // I'm sorry.  Your pet seems to be busy.

            else if (from.Stabled.Count >= GetMaxStabled(from))
                SayTo(from, 1042565); // You have too many pets in the stables!

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
                    pet.StabledBy = from;

                    pet.OwnerAbandonTime = DateTime.MaxValue;

                    if (Core.SE)
                        pet.Loyalty = BaseCreature.MaxLoyalty; // Wonderfully happy

                    from.Stabled.Add(pet);

                    SayTo(from, Core.AOS ? 1049677 : 502679); // [AOS: Your pet has been stabled.] Very well, thy pet is stabled. Thou mayst recover it by saying 'claim' to me. In one real world week, I shall sell it off if it is not claimed!
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
                    pet.StabledBy = null;

                    pet.OwnerAbandonTime = DateTime.UtcNow + pet.AbandonDelay;

                    from.Stabled.RemoveAt(i);
                    --i;

                    continue;
                }

                if (pet.IsHenchman)
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
                    SayTo(from, 1049612, pet.Name); // ~1_NAME~ remained in the stables because you have too many followers.                
            }

            if (claimed)
                SayTo(from, 1042559); // Here you go... and good day to you!

            else if (stabled == 0)
                SayTo(from, 502671); // But I have no animals stabled with me at the moment!

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
            pet.StabledBy = null;

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

        public AnimalTrainer(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}