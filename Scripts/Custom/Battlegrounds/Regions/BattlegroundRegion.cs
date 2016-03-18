using Server.Custom.Battlegrounds.Games;
using Server.Items;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.Fourth;
using Server.Spells.Seventh;
using Server.Spells.Sixth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Server.Custom.Battlegrounds.Regions
{
    class BattlegroundRegion : Region
    {
        protected Battleground m_Battleground;

        public Battleground Battleground { get { return m_Battleground; } }

        public BattlegroundRegion(XmlElement xml, Map map, Region parent)
            : base(xml, map, parent)
        {
            foreach (XmlAttribute attribute in xml.Attributes)
            {
                if (attribute.Name == "name")
                {
                    switch (attribute.Value)
                    {
                        // siege bgs
                        case "Treasure Cove":
                            m_Battleground = new TreasureCoveBattleground();
                            break;
                        case "Throne Siege":
                            m_Battleground = new ThroneSiegeBattleground();
                            break;
                        case "Sanctuary":
                            m_Battleground = new SanctuaryBattleground();
                            break;

                        //CTF bgs
                        case "Volcano":
                            m_Battleground = new LavaCTF();
                            break;
                        case "Fungal Fancies":
                            m_Battleground = new MushroomCTF(); ;
                            break;
                        case "Starry Expanse":
                            m_Battleground = new StarryCTF();
                            break;
                        case "Oasis":
                            m_Battleground = new JungleCTF();
                            break;
                    }
                    break;
                }

            }
        }

        public override bool AllowBeneficial(Mobile from, Mobile target)
        {
            if (from.Spectating || target.Spectating) return false;

            return base.AllowBeneficial(from, target);
        }

        public override bool OnSkillUse(Mobile m, int Skill)
        {
            if (m.Spectating) return false;

            return base.OnSkillUse(m, Skill);
        }

        public override bool OnTarget(Mobile m, Targeting.Target t, object o)
        {
            if (m.Spectating && m.AccessLevel == AccessLevel.Player) return false;

            return base.OnTarget(m, t, o);
        }

        public override bool OnDoubleClick(Mobile m, object o)
        {
            if (m.Spectating && m.AccessLevel == AccessLevel.Player) return false;

            return base.OnDoubleClick(m, o);
        }

        public override bool OnSingleClick(Mobile m, object o)
        {
            if (m.Spectating && m.AccessLevel == AccessLevel.Player) return false;

            return base.OnSingleClick(m, o);
        }

        public override bool AllowHarmful(Mobile from, Mobile target)
        {
            if (from.Spectating || target.Spectating) return false;

            // prevent team killing
            if (from is PlayerMobile && target is PlayerMobile)
                return !Battleground.OnSameTeam(from as PlayerMobile, target as PlayerMobile);

            return base.AllowHarmful(from, target);
        }

        public override bool CanUseStuckMenu(Mobile m)
        {
            Battleground.Leave(m as PlayerMobile);
            return false;
        }

        public override bool AllowHousing(Mobile from, Point3D p)
        {
            return false;
        }

        public override bool OnBeginSpellCast(Mobile m, ISpell s)
        {
            if (m.Spectating && m.AccessLevel == AccessLevel.Player) return false;

            // this is ugly lol
            if (s is RecallSpell ||
                s is GateTravelSpell ||
                s is MarkSpell ||
                s is SummonCreatureSpell ||
                s is BladeSpiritsSpell ||
                s is Spells.Eighth.AirElementalSpell ||
                s is Spells.Eighth.EarthElementalSpell ||
                s is Spells.Eighth.FireElementalSpell ||
                s is Spells.Eighth.SummonDaemonSpell ||
                s is Spells.Eighth.WaterElementalSpell ||
                s is Spells.Eighth.EnergyVortexSpell ||
                s is Spells.Seventh.EnergyFieldSpell ||
                s is Spells.Fifth.PoisonFieldSpell ||
                s is Spells.Fourth.FireFieldSpell ||
                s is Spells.Sixth.ParalyzeFieldSpell ||
                s is Spells.Eighth.EarthquakeSpell)
            {
                m.SendLocalizedMessage(501802);
                return false;
            }
            else
                return base.OnBeginSpellCast(m, s);
        }

        public void HandleDeath(PlayerMobile from, Container cont)
        {
            Reequip(from, cont);
            DelayResurrect(from, cont);
        }

        protected virtual void DelayResurrect(PlayerMobile from, Container cont)
        {
            Timer.DelayCall(Battleground.RespawnDelay, () =>
            {
                // allow players to be resed
                if (!from.Alive)
                {
                    if (m_Battleground.Active)
                    {
                        m_Battleground.MoveToSpawn(from);
                    }
                    Resurrect(from, cont);
                }
            });
        }

        protected virtual void Resurrect(PlayerMobile mob, Container cont)
        {
            if (!mob.Alive)
            {
                mob.Resurrect();

                DeathRobe robe = mob.FindItemOnLayer(Layer.OuterTorso) as DeathRobe;

                if (robe != null)
                    robe.Delete();

                if (cont is Corpse)
                {
                    Corpse corpse = (Corpse)cont;

                    for (int i = 0; i < corpse.EquipItems.Count; ++i)
                    {
                        Item item = corpse.EquipItems[i];

                        if (item.Movable && item.Layer != Layer.Hair && item.Layer != Layer.FacialHair && item.IsChildOf(mob.Backpack))
                            mob.EquipItem(item);
                    }
                }

                mob.Hits = mob.HitsMax;
                mob.Stam = mob.StamMax;
                mob.Mana = mob.ManaMax;

                mob.Poison = null;
            }
            cont.Delete();
        }

        protected virtual void Reequip(PlayerMobile from, Container cont)
        {
            Corpse corpse = cont as Corpse;

            if (corpse == null)
                return;

            List<Item> items = new List<Item>(corpse.Items);

            bool gathered = false;
            bool didntFit = false;

            Container pack = from.Backpack;

            for (int i = 0; !didntFit && i < items.Count; ++i)
            {
                Item item = items[i];
                Point3D loc = item.Location;

                if ((item.Layer == Layer.Hair || item.Layer == Layer.FacialHair) || !item.Movable)
                    continue;

                if (pack != null)
                {
                    pack.DropItem(item);
                    gathered = true;
                }
                else
                {
                    didntFit = true;
                }
            }

            if (gathered && !didntFit)
                from.SendLocalizedMessage(1062471); // You quickly gather all of your belongings.
            else if (gathered && didntFit)
                from.SendLocalizedMessage(1062472); // You gather some of your belongings. The rest remain on the corpse.
        }


    }
}
