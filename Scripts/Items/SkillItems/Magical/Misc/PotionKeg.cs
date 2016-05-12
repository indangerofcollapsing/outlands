using System;
using Server;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class PotionKeg : Item
    {
        private PotionEffect m_Type;

        private int m_Held;
        [CommandProperty(AccessLevel.GameMaster)]
        public int Held
        {
            get
            {
                return m_Held;
            }

            set
            {
                if (m_Held != value)
                {
                    m_Held = value;

                    if (IsPotionBarrel)
                    {
                        if (m_Held == m_MaxHeld)
                            ItemID = 4014;

                        else
                            ItemID = 3703;
                    }

                    else
                        ItemID = 0x1940;

                    UpdateWeight();
                    InvalidateProperties();
                }
            }
        }

        private int m_MaxHeld = 100;
        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHeld
        {
            get
            {
                return m_MaxHeld;
            }

            set
            {
                m_MaxHeld = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PotionEffect Type
        {
            get
            {
                return m_Type;
            }

            set
            {
                m_Type = value;
                InvalidateProperties();
            }
        }

        private bool m_IsPotionBarrel = false;
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsPotionBarrel
        {
            get { return m_IsPotionBarrel; }
            set
            {
                m_IsPotionBarrel = value;

                if (value == true)
                {
                    if (m_Held == 0)
                        ItemID = 0xe77;
                    else
                        ItemID = 0x154d;
                }

                else
                    ItemID = 0x1940;
            }
        }

        [Constructable]
        public PotionKeg() : base(0x1940)
        {
            UpdateWeight();
        }

        public virtual void UpdateWeight()
        {
            Weight = Held;
        }

        public PotionKeg(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); //Version

            //Version 0
            writer.Write((int)m_Type);
            writer.Write((int)m_Held);

            //Version 1

            //Version 2
            writer.Write((int)m_MaxHeld);
            writer.Write(m_IsPotionBarrel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            //Version 0
            if (version > 0)
            {
                m_Type = (PotionEffect)reader.ReadInt();
                Held = reader.ReadInt();
            }

            //Version 1

            //Version 2
            if (version >= 2)
            {
                m_MaxHeld = reader.ReadInt();
                IsPotionBarrel = reader.ReadBool();
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            double fillPercentage = (double)m_Held / (double)m_MaxHeld;
            int number;

            if (fillPercentage <= 0)
                number = 502246; // The keg is empty.

            else if (fillPercentage < .05)
                number = 502248; // The keg is nearly empty.

            else if (fillPercentage < .20)
                number = 502249; // The keg is not very full.

            else if (fillPercentage < .30)
                number = 502250; // The keg is about one quarter full.

            else if (fillPercentage < .40)
                number = 502251; // The keg is about one third full.

            else if (fillPercentage < .47)
                number = 502252; // The keg is almost half full.

            else if (fillPercentage < .54)
                number = 502254; // The keg is approximately half full.

            else if (fillPercentage < .70)
                number = 502253; // The keg is more than half full.

            else if (fillPercentage < .80)
                number = 502255; // The keg is about three quarters full.

            else if (fillPercentage < .96)
                number = 502256; // The keg is very full.

            else if (fillPercentage < 1.0)
                number = 502257; // The liquid is almost to the top of the keg.

            else
                number = 502258; // The keg is completely full.

            list.Add(number);
        }

        public override void OnSingleClick(Mobile from)
        {            
            if (m_Held == 0)
                LabelTo(from, "a potion keg");
            else
                LabelTo(from, "a potion keg: " + BasePotion.GetName(m_Type));

            if (m_MaxHeld > 100)
                LabelTo(from, "(Capacity: " + m_Held.ToString() + " / " + m_MaxHeld.ToString() + ")");

            else
            {

                double fillPercentage = (float)m_Held / (float)m_MaxHeld;
                int number;

                if (fillPercentage <= 0)
                    number = 502246; // The keg is empty.

                else if (fillPercentage < .05)
                    number = 502248; // The keg is nearly empty.

                else if (fillPercentage < .20)
                    number = 502249; // The keg is not very full.

                else if (fillPercentage < .30)
                    number = 502250; // The keg is about one quarter full.

                else if (fillPercentage < .40)
                    number = 502251; // The keg is about one third full.

                else if (fillPercentage < .47)
                    number = 502252; // The keg is almost half full.

                else if (fillPercentage < .54)
                    number = 502254; // The keg is approximately half full.

                else if (fillPercentage < .70)
                    number = 502253; // The keg is more than half full.

                else if (fillPercentage < .80)
                    number = 502255; // The keg is about three quarters full.

                else if (fillPercentage < .96)
                    number = 502256; // The keg is very full.

                else if (fillPercentage < 1.0)
                    number = 502257; // The liquid is almost to the top of the keg.

                else
                    number = 502258; // The keg is completely full.

                LabelTo(from, number);
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from.InRange(GetWorldLocation(), 2))
            {
                if (m_Held > 0)
                {
                    Container pack = from.Backpack;

                    if (pack != null && pack.ConsumeTotal(typeof(Bottle), 1))
                    {
                        from.SendLocalizedMessage(502242); // You pour some of the keg's contents into an empty bottle...

                        BasePotion pot = FillBottle();

                        if (pack.TryDropItem(from, pot, false))
                        {
                            from.SendLocalizedMessage(502243); // ...and place it into your backpack.
                            from.PlaySound(0x240);

                            if (--Held == 0)
                            {
                                from.SendLocalizedMessage(502245); // The keg is now empty.
                                Hue = 0;
                            }
                        }

                        else
                        {
                            from.SendLocalizedMessage(502244); // ...but there is no room for the bottle in your backpack.
                            pot.Delete();
                        }
                    }
                }

                else
                    from.SendLocalizedMessage(502246); // The keg is empty.                
            }

            else
                from.LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1019045); // I can't reach that.            
        }

        public override bool OnDragDrop(Mobile from, Item item)
        {
            if (item is PolymorphPotion || item is IncognitoPotion || item is RabbitBomb)
                return false;

            if (item is BasePotion)
            {
                BasePotion pot = (BasePotion)item;

                int toHold = Math.Min(m_MaxHeld - m_Held, pot.Amount);

                if (toHold <= 0)
                {
                    from.SendLocalizedMessage(502233); // The keg will not hold any more!
                    return false;
                }

                else if (m_Held == 0)
                {
                    if (GiveBottle(from, toHold))
                    {
                        m_Type = pot.PotionEffect;
                        Held = toHold;

                        from.PlaySound(0x240);

                        from.SendLocalizedMessage(502237); // You place the empty bottle in your backpack.

                        item.Consume(toHold);

                        if (!item.Deleted)
                            item.Bounce(from);
                        
                        //Change the potion keg color if it is original (has not been dyed by furniture tub or change color by potion yet).
                        if (Hue == OriginalHue)
                        {
                            //Change the color hue based on the type of the potion
                            switch (item.GetType().BaseType.Name)
                            {
                                case "BaseMagicResistPotion": Hue = 1109; break;
                                case "BaseCurePotion": Hue = 44; break;
                                case "BaseAgilityPotion": Hue = 93; break;
                                case "BaseStrengthPotion": Hue = 956; break;
                                case "BasePoisonPotion": Hue = 363; break;
                                case "BaseRefreshPotion": Hue = 37; break;
                                case "BaseHealPotion": Hue = 253; break;
                                case "BaseExplosionPotion": Hue = 419; break;

                                default: break;
                            }

                            from.SendMessage("The potion keg has absorbed the potion and turned into its color.");
                        }                        

                        return true;
                    }

                    else
                    {
                        from.SendLocalizedMessage(502238); // You don't have room for the empty bottle in your backpack.
                        return false;
                    }
                }

                else if (pot.PotionEffect != m_Type)
                {
                    from.SendLocalizedMessage(502236); // You decide that it would be a bad idea to mix different types of potions.
                    return false;
                }

                else
                {
                    if (GiveBottle(from, toHold))
                    {
                        Held += toHold;

                        from.PlaySound(0x240);
                        from.SendLocalizedMessage(502237); // You place the empty bottle in your backpack.
                        item.Consume(toHold);

                        if (!item.Deleted)
                            item.Bounce(from);

                        return true;
                    }

                    else
                    {
                        from.SendLocalizedMessage(502238); // You don't have room for the empty bottle in your backpack.
                        return false;
                    }
                }
            }

            else
            {
                from.SendLocalizedMessage(502232); // The keg is not designed to hold that type of object.
                return false;
            }
        }

        public bool GiveBottle(Mobile m, int amount)
        {
            Container pack = m.Backpack;

            Bottle bottle = new Bottle(amount);

            if (pack == null || !pack.TryDropItem(m, bottle, false))
            {
                bottle.Delete();
                return false;
            }

            return true;
        }

        public BasePotion FillBottle()
        {
            switch (m_Type)
            {
                default:
                case PotionEffect.LesserMagicResistance: return new LesserMagicResistPotion();
                case PotionEffect.MagicResistance: return new MagicResistPotion();
                case PotionEffect.GreaterMagicResistance: return new GreaterMagicResistPotion();

                case PotionEffect.CureLesser: return new LesserCurePotion();
                case PotionEffect.Cure: return new CurePotion();
                case PotionEffect.CureGreater: return new GreaterCurePotion();

                case PotionEffect.Agility: return new AgilityPotion();
                case PotionEffect.AgilityGreater: return new GreaterAgilityPotion();

                case PotionEffect.Strength: return new StrengthPotion();
                case PotionEffect.StrengthGreater: return new GreaterStrengthPotion();

                case PotionEffect.PoisonLesser: return new LesserPoisonPotion();
                case PotionEffect.Poison: return new PoisonPotion();
                case PotionEffect.PoisonGreater: return new GreaterPoisonPotion();
                case PotionEffect.PoisonDeadly: return new DeadlyPoisonPotion();
                case PotionEffect.PoisonLethal: return new LethalPoisonPotion();

                case PotionEffect.Refresh: return new RefreshPotion();
                case PotionEffect.RefreshTotal: return new TotalRefreshPotion();

                case PotionEffect.HealLesser: return new LesserHealPotion();
                case PotionEffect.Heal: return new HealPotion();
                case PotionEffect.HealGreater: return new GreaterHealPotion();

                case PotionEffect.ExplosionLesser: return new LesserExplosionPotion();
                case PotionEffect.Explosion: return new ExplosionPotion();
                case PotionEffect.ExplosionGreater: return new GreaterExplosionPotion();
            }
        }

        public static void Initialize()
        {
            TileData.ItemTable[0x1940].Height = 4;
        }
    }
}