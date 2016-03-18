using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Seventh;
using Server.Spells.Fourth;
using Server.Spells.Sixth;
using Server.Spells.Fifth;
using Server.Spells.Eighth;

namespace Server.ArenaSystem
{
    /////////////////////////////////////////////
    // Code from Kobalo's Order.cs in SVN.
    public abstract class ArenaRestriction
    {
        /// <summary>
        /// Moves any items not allowed by the restriction into the player's bank box. Ideally, this should be moved to
        /// the internal Map so that the player's bank box isn't a factor.
        /// </summary>
        /// <param name="playerList"></param>
        /// <returns></returns>
        public abstract bool VerifyItems(List<PlayerMobile> playerList);
        public abstract bool OnSkillUse(Mobile from, int Skill);
        public abstract bool OnBeginSpellCast(Mobile m, ISpell s);
        public abstract void OnDeath(Mobile m);
        public abstract void OnEndMatch(bool isTemplated);
    }

    public class OrderRestrictions : ArenaRestriction
    {
        private List<Pair<Mobile, Item>> m_itemsWithheld;

        public OrderRestrictions()
        {
        }
        public override bool VerifyItems(List<PlayerMobile> playerList)
        {
            m_itemsWithheld = new List<Pair<Mobile, Item>>();

            List<Mobile> mobileList = new List<Mobile>();
            foreach (PlayerMobile pm in playerList)
            {
                mobileList.Add(pm);
            }

            foreach (Mobile m in mobileList)
            {
                List<Item> badItems = new List<Item> { };
                List<Item> allItems = new List<Item> { };
                List<Item> baseItems = new List<Item> { };

                if (m.Items != null) baseItems.AddRange(m.Items);
                if (m.Holding != null) baseItems.Add(m.Holding);
                if (m.Backpack != null && m.Backpack.Items != null) baseItems.AddRange(m.Backpack.Items);

                for (int i = 0; i < baseItems.Count; i++)
                    if (baseItems[i] is BaseContainer)
                    {
                        baseItems.AddRange(((BaseContainer)baseItems[i]).Items);
                        baseItems.Remove(baseItems[i]);
                        i--;
                    }

                allItems = baseItems;

                foreach (Item i in allItems)
                {
                    if ((i is BasePotion && !(i is BaseRefreshPotion)) || i is PotionKeg)
                        badItems.Add(i);
                    if (i is BaseWeapon)
                    {
                        BaseWeapon w = i as BaseWeapon;
                        if (w != null && (w.PoisonCharges > 0 || w.DamageLevel != WeaponDamageLevel.Regular ||
                            w.DurabilityLevel != WeaponDurabilityLevel.Regular || w.AccuracyLevel != WeaponAccuracyLevel.Regular))
                            badItems.Add(i);
                    }
                    if (i is BaseArmor)
                    {
                        BaseArmor a = i as BaseArmor;
                        if (a != null && (a.ProtectionLevel != ArmorProtectionLevel.Regular || a.Durability != ArmorDurabilityLevel.Regular))
                            badItems.Add(i);
                    }
                }


                if (badItems.Count > 0)
                {
                    //if (m.BankBox.Items.Count + badItems.Count + 1 > m.BankBox.MaxItems)
                    //{
                        
                    //    m.SendMessage("Some items in your inventory were not allowed, and there was no room in your bank box to move them.");
                    //    return false;
                    //}
                    

                    Bag b = new Bag();
                    foreach (Item i in badItems)
                    {
                        b.AddItem(i);
                    }
                    b.MoveToWorld(new Point3D(0, 0, 0), Map.Internal);

                    m_itemsWithheld.Add(new Pair<Mobile, Item>(m, b));

                    //m.BankBox.AddItem(b);
                    m.SendMessage("The items prohibited by this ladder are being withheld. They will be returned after the match.");
                }
            }
            return true;
        }
        public override bool OnSkillUse(Mobile from, int Skill)
        {
            switch (Skill)
            {
                case (int)SkillName.Stealing:
                    from.SendMessage("Stealing is not allowed in an Order match.");
                    return false;
            }
            return true;
        }
        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (s is RecallSpell || s is MarkSpell || s is GateTravelSpell || s is SummonCreatureSpell ||
                s is SummonDaemonSpell || s is AirElementalSpell || s is EarthElementalSpell ||
                s is WaterElementalSpell || s is FireElementalSpell || s is ResurrectionSpell ||
                s is ParalyzeSpell || s is ReactiveArmorSpell ||
                s is EnergyVortexSpell || s is BladeSpiritsSpell)
            {
                m.SendMessage("That spell can not be used in an Order match.");
                return false;
            }
            return true;
        }
        public override void OnDeath(Mobile m)
        {
            if (m.Corpse != null)
            {
                DeathRobe robe = m.FindItemOnLayer(Layer.OuterTorso) as DeathRobe;

                if (robe != null)
                    robe.Delete();

                Corpse corpse = (Corpse)m.Corpse;

                for (int i = 0; i < corpse.EquipItems.Count; ++i)
                {
                    Item item = corpse.EquipItems[i];

                    if (item.Movable && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.IsChildOf(m.Backpack))
                        m.EquipItem(item);
                }

                List<Item> items = new List<Item> { };
                foreach (Item i in corpse.Items)
                    items.Add(i);
                foreach (Item i in items)
                    m.AddToBackpack(i);
            }
        }
        public override void OnEndMatch(bool isTemplated)
        {
            // Return any items that were withheld during the match.
            foreach (Pair<Mobile, Item> pair in m_itemsWithheld)
            {
                PlayerMobile pm = pair.First as PlayerMobile;
                bool sentToBank = false;
                if (pair.Second is Container)
                {
                    var container = pair.Second as Container;
                    var bank = pm.BankBox;
                    for(int i = container.Items.Count - 1; i >= 0; i--)
                    {
                        Item item = container.Items[i];
                        if (item != null)
                        {
                            if (!pm.PlaceInBackpack(item))
                            {
                                if (bank != null)
                                {
                                    bank.AddItem(item);
                                    sentToBank = true;
                                }
                            }
                        }
                    }

                    pair.Second.Delete();
                }
                else
                    pm.AddToBackpack(pair.Second);

                if (sentToBank)
                    pm.SendMessage("Some restricted items have been sent to your bank box.");
            }
        }
    }
    public class ChaosRestrictions : ArenaRestriction
    {
        private List<PoisonStateDetails> m_poisonDetails;

        private struct PoisonStateDetails
        {
            public BaseWeapon m_item;
            public Poison m_poison;
            public int m_poisonCharges;

            public PoisonStateDetails(BaseWeapon item, Poison poison, int poisonCharges)
            {
                m_item = item;
                m_poison = poison;
                m_poisonCharges = poisonCharges;
            }
        }

        public ChaosRestrictions()
        {
            m_poisonDetails = new List<PoisonStateDetails>();
        }
        /// <summary>
        /// Because poisoning is on the templated skill list, players can potentially queue up just to 
        /// poison weapons. Restore any poisoned weapon back to it's original state.
        /// </summary>
        /// <param name="m"></param>
        private void RevertPoisonLevels()
        {
            foreach (PoisonStateDetails details in m_poisonDetails)
            {
                details.m_item.Poison = details.m_poison;
                details.m_item.PoisonCharges = details.m_poisonCharges;
            }
        }
        public override void OnEndMatch(bool isTemplated)
        {
            if (isTemplated)
            {
                RevertPoisonLevels();
            }
        }
        public override bool VerifyItems(List<PlayerMobile> playerList)
        {
            // To prevent poison exploitation, any weapon that enters the arena is going to have its poison state reverted.
            foreach (PlayerMobile m in playerList)
            {
                List<Item> poisonedWeapons = new List<Item> { };
                List<Item> baseItems = new List<Item> { };

                if (m.Items != null) baseItems.AddRange(m.Items);
                if (m.Holding != null) baseItems.Add(m.Holding);
                if (m.Backpack != null && m.Backpack.Items != null) baseItems.AddRange(m.Backpack.Items);

                for (int i = 0; i < baseItems.Count; i++)
                    if (baseItems[i] is BaseContainer)
                    {
                        baseItems.AddRange(((BaseContainer)baseItems[i]).Items);
                        baseItems.Remove(baseItems[i]);
                        i--;
                    }

                foreach (Item i in baseItems)
                {
                    if (i is BaseWeapon)
                    {
                        BaseWeapon w = i as BaseWeapon;
                        if (w != null)
                        {
                            m_poisonDetails.Add(new PoisonStateDetails(w, w.Poison, w.PoisonCharges));
                        }
                    }
                }
            }

            return true;
        }
        public override bool OnSkillUse(Mobile from, int Skill)
        {
            // No restrictions.
            return true;
        }
        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
             if (s is RecallSpell || s is MarkSpell || s is GateTravelSpell)
            {
                m.SendMessage("That spell can not be used in a Chaos match.");
                return false;
            }
            return true;
        }
        public override void OnDeath(Mobile m)
        {
            if (m.Corpse != null)
            {
                List<Item> items = new List<Item> { };
                foreach (Item i in m.Corpse.Items)
                    items.Add(i);
                foreach (Item i in items)
                    m.AddToBackpack(i);
            }
        }
    }
}
