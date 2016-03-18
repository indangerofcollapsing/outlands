using System;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;
using Server.Mobiles;
using Server.Gumps;

namespace Server
{
    public abstract class UOACZConsumptionItem : Item
    {
        public enum ConsumptionMode
        {
            Food,
            Drink
        }

        public enum ConsumptionQuality
        {
            Normal,
            Raw,
            Corrupted
        }

        public virtual int MaxCharges { get { return 4; } }

        public virtual int ItemIdFor1Charges { get { return -1; } }
        public virtual int ItemIdFor2Charges { get { return -1; } }
        public virtual int ItemIdFor3Charges { get { return -1; } }
        public virtual int ItemIdFor4Charges { get { return -1; } }
        
        public virtual int WeightPerCharge { get { return 1; } }

        public virtual Type DropContainer { get { return null; } }   

        private int m_Charges = 1;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set
            {
                m_Charges = value;

                if (m_Charges > 1)
                {
                    Weight = m_Charges * WeightPerCharge;
                }

                if (m_Charges >= 4 && ItemIdFor4Charges != -1)                
                    ItemID = ItemIdFor4Charges;                

                if (m_Charges == 3 && ItemIdFor3Charges != -1)
                    ItemID = ItemIdFor3Charges;

                if (m_Charges == 2 && ItemIdFor2Charges != -1)
                    ItemID = ItemIdFor2Charges;

                if (m_Charges == 1 && ItemIdFor1Charges != -1)
                    ItemID = ItemIdFor1Charges; 
            }
        }
        
        public virtual ConsumptionMode ConsumptionType { get { return ConsumptionMode.Food; } }

        public virtual int HitsChange { get { return 0; } }
        public virtual int StamChange { get { return 0; } }
        public virtual int ManaChange { get { return 0; } }

        public virtual int HungerChange { get { return 10; } }
        public virtual int ThirstChange { get { return 0; } }

        public virtual ConsumptionQuality ConsumptionQualityType { get { return ConsumptionQuality.Normal; } }             

        public UOACZConsumptionItem( int itemID ) : base( itemID )
        {
            Name = "consumption item";
            Weight = 1;            
        }

        public override void OnSingleClick(Mobile from)
        {
            base.OnSingleClick(from);

            if (m_Charges > 1)
            {
                if (ConsumptionType == ConsumptionMode.Food)
                    LabelTo(from, "(" + m_Charges.ToString() + " bites remaining)");

                else
                    LabelTo(from, "(" + m_Charges.ToString() + " drinks remaining)");
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from == null) return;
            if (!Movable) return;

            if (!IsChildOf(from.Backpack))
            {
                from.SendMessage("That item must be in your pack in order to use it.");
                return;
            }
            
            if (!from.CanBeginAction(typeof(UOACZConsumptionItem)))
            {
                from.SendMessage("You must wait a few moments before consuming another item.");
                return;
            }

            from.BeginAction(typeof(UOACZConsumptionItem));

            Timer.DelayCall(TimeSpan.FromSeconds(1), delegate
            {
                if (from != null)
                    from.EndAction(typeof(UOACZConsumptionItem));
            });

            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            if (player.m_UOACZAccountEntry.ActiveProfile == UOACZAccountEntry.ActiveProfileType.Human)
            {
                if (ConsumptionQualityType == ConsumptionQuality.Raw || ConsumptionQualityType == ConsumptionQuality.Corrupted)
                {
                    player.CloseGump(typeof(UOACZConsumeFoodGump));
                    player.SendGump(new UOACZConsumeFoodGump(this, player));
                }

                else
                    Consume(player);
            }
        }

        public virtual void Consume(PlayerMobile player)
        {
            UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

            switch (ConsumptionType)
            {
                case ConsumptionMode.Food:
                    player.PlaySound(Utility.RandomList(0x5DA, 0x5A9, 0x5AB, 0x03A, 0x03B, 0x03C));
                    player.Animate(34, 5, 1, true, false, 0);

                    player.m_UOACZAccountEntry.FoodItemsConsumed++;
                break;

                case ConsumptionMode.Drink:
                    Effects.PlaySound(player.Location, player.Map, Utility.RandomList(0x030, 0x031, 0x050));
                    player.Animate(34, 5, 1, true, false, 0);

                    player.m_UOACZAccountEntry.DrinkItemsConsumed++;
                break;
            }            

            Charges--;

            DropContainerToPlayer(player);            

            Timer.DelayCall(TimeSpan.FromSeconds(.5), delegate
            {
                if (player == null) return;

                UOACZPersistance.CheckAndCreateUOACZAccountEntry(player);

                if (player.Deleted || !player.Alive) return;
                if (player.m_UOACZAccountEntry.ActiveProfile != UOACZAccountEntry.ActiveProfileType.Human) return;

                player.Heal(HitsChange);
                player.Stam += StamChange;
                player.Mana += ManaChange;

                int baseDiseaseAmount = 0;
                double diseaseAmount = 0;                

                if (ConsumptionQualityType == ConsumptionQuality.Raw)
                {
                    player.SendMessage("You eat the raw meat and wish it had been cooked first.");

                    baseDiseaseAmount = UOACZSystem.RawMeatDiseaseAmount;
                    diseaseAmount = UOACZSystem.RawMeatDiseaseAmount;
                }

                if (ConsumptionQualityType == ConsumptionQuality.Corrupted)
                {
                    player.SendMessage("You eat the corrupted meat and wish it had been purified alchemically first.");

                    baseDiseaseAmount = UOACZSystem.CorruptedMeatDiseaseAmount;
                    diseaseAmount = UOACZSystem.CorruptedMeatDiseaseAmount;
                }

                if (diseaseAmount > 0)
                {
                    bool foundDisease = false;

                    Queue m_EntriesToRemove = new Queue();

                    foreach (SpecialAbilityEffectEntry entry in player.m_SpecialAbilityEffectEntries)
                    {
                        if (entry.m_SpecialAbilityEffect == SpecialAbilityEffect.Disease && DateTime.UtcNow < entry.m_Expiration)
                        {
                            if (!foundDisease)
                            {
                                diseaseAmount += entry.m_Value;
                                foundDisease = true;
                            }

                            m_EntriesToRemove.Enqueue(entry);       
                        }
                    }

                    while (m_EntriesToRemove.Count > 0)
                    {
                        SpecialAbilityEffectEntry entry = (SpecialAbilityEffectEntry)m_EntriesToRemove.Dequeue();

                        player.m_SpecialAbilityEffectEntries.Remove(entry);
                    }
                    
                    if (!player.Hidden)
                    {
                        Effects.PlaySound(player.Location, player.Map, 0x5CB);
                        Effects.SendLocationParticles(EffectItem.Create(player.Location, player.Map, TimeSpan.FromSeconds(0.25)), 0x376A, 10, 20, 2199, 0, 5029, 0);

                        player.PublicOverheadMessage(MessageType.Regular, 1103, false, "*looks violently ill*");

                        Blood blood = new Blood();
                        blood.Hue = 2200;
                        blood.MoveToWorld(player.Location, player.Map);

                        int extraBlood = Utility.RandomMinMax(1, 2);

                        for (int i = 0; i < extraBlood; i++)
                        {
                            Blood moreBlood = new Blood();
                            moreBlood.Hue = 2200;
                            moreBlood.MoveToWorld(new Point3D(player.Location.X + Utility.RandomMinMax(-1, 1), player.Location.Y + Utility.RandomMinMax(-1, 1), player.Location.Z), player.Map);
                        }
                    }

                    AOS.Damage(player, null, baseDiseaseAmount, 0, 100, 0, 0, 0); 

                    SpecialAbilities.DiseaseSpecialAbility(1.0, player, player, diseaseAmount, UOACZSystem.FoodDiseaseSeconds, 0, false, "", "");
                }

                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.Hunger, HungerChange, true);
                UOACZSystem.ChangeStat(player, UOACZSystem.UOACZStatType.Thirst, ThirstChange, true);
            });
        }

        public virtual void DropContainerToPlayer(PlayerMobile player)
        {
            int x = X;
            int y = Y;

            if (Charges <= 0)
            {
                Delete();

                if (DropContainer != null && player.Backpack != null)
                {
                    Item[] matchingItems = player.Backpack.FindItemsByType(DropContainer);

                    if (matchingItems.Length > 0)
                    {
                        matchingItems[0].Amount++;
                    }

                    else
                    {
                        Item item = (Item)Activator.CreateInstance(DropContainer);

                        if (item != null)
                        {
                            player.Backpack.DropItem(item);

                            item.X = X;
                            item.Y = y;
                        }
                    }
                }
            }                
        }

        public UOACZConsumptionItem(Serial serial): base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); //version

            //Version 0
            writer.Write(m_Charges);           
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            //Version 0
            if (version >= 0)
            {
                m_Charges = reader.ReadInt();
            }
        }
    }
}