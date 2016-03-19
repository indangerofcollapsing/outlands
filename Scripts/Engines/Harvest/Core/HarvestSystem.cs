using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Targeting;
using Server.Achievements;
using Server.Multis;

using Server.Engines.BulkOrders;

namespace Server.Engines.Harvest
{
    public abstract class HarvestSystem
    {
        private List<HarvestDefinition> m_Definitions;

        public List<HarvestDefinition> Definitions { get { return m_Definitions; } }

        public HarvestSystem()
        {
            m_Definitions = new List<HarvestDefinition>();
        }

        public virtual bool CheckTool(Mobile from, Item tool)
        {
            bool wornOut = (tool == null || tool.Deleted || (tool is IUsesRemaining && ((IUsesRemaining)tool).UsesRemaining <= 0));

            if (wornOut)
                from.SendLocalizedMessage(1044038); // You have worn out your tool!

            return !wornOut;
        }

        public virtual bool CheckHarvest(Mobile from, Item tool)
        {
            return CheckTool(from, tool);
        }

        public virtual bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            return CheckTool(from, tool);
        }

        public virtual bool CheckRange(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            bool inRange = (from.Map == map && from.InRange(loc, def.MaxRange));

            if (!inRange)
                def.SendMessageTo(from, timed ? def.TimedOutOfRangeMessage : def.OutOfRangeMessage);

            return inRange;
        }

        public virtual bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);
            bool available = (bank != null && bank.Current >= def.ConsumedPerHarvest);

            if (!available)
                def.SendMessageTo(from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage);

            return available;
        }

        public virtual void OnBadHarvestTarget(Mobile from, Item tool, object toHarvest)
        {
        }

        public virtual object GetLock(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            /* Here we prevent multiple harvesting.
             * 
             * Some options:
             *  - 'return tool;' : This will allow the player to harvest more than once concurrently, but only if they use multiple tools. This seems to be as OSI.
             *  - 'return GetType();' : This will disallow multiple harvesting of the same type. That is, we couldn't mine more than once concurrently, but we could be both mining and lumberjacking.
             *  - 'return typeof( HarvestSystem );' : This will completely restrict concurrent harvesting.
             */

            return typeof(HarvestSystem);
        }

        public virtual void OnConcurrentHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
        }

        public virtual void OnHarvestStarted(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
        }

        public virtual bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!CheckHarvest(from, tool))
                return false;

            from.Target = new HarvestTarget(tool, this);
            return true;
        }

        public virtual void FinishHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked)
        {
            from.EndAction(locked);

            if (!CheckHarvest(from, tool))
                return;

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }
            else if (!def.Validate(tileID))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!CheckRange(from, tool, def, map, loc, true))
                return;
            else if (!CheckResources(from, tool, def, map, loc, true))
                return;
            else if (!CheckHarvest(from, tool, def, toHarvest))
                return;

            if (SpecialHarvest(from, tool, def, map, loc))
                return;

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null)
                return;

            HarvestVein vein = bank.Vein;

            if (vein != null)
                vein = MutateVein(from, tool, def, bank, toHarvest, vein);

            if (vein == null)
                return;

            HarvestResource primary = vein.PrimaryResource;
            HarvestResource fallback = vein.FallbackResource;
            HarvestResource resource = MutateResource(from, tool, def, map, loc, vein, primary, fallback);

            double skillBase = from.Skills[def.Skill].Base;
            double skillValue = from.Skills[def.Skill].Value;

            Type type = null;

            if (skillBase >= resource.ReqSkill && from.CheckSkill(def.Skill, resource.MinSkill, resource.MaxSkill))
            {
                type = GetResourceType(from, tool, def, map, loc, resource);

                if (type != null)
                    type = MutateType(type, from, tool, def, map, loc, resource);

                if (type != null)
                {
                    Item item = Construct(type, from, tool, def, bank, resource);

                    if (item == null)
                    {
                        type = null;
                    }
                    else
                    {
                        BaseBoat ownerBoat = BaseBoat.FindBoatAt(from.Location, from.Map);

                        // IPY ACHIEVEMENTS		- This should go to fishing.SendSuccessTo instead

                        if (item is PrizedFish)
                        {
                            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_CatchPrizedFish);

                            if (ownerBoat != null)
                            {
                                if (ownerBoat.IsOwner(from) || ownerBoat.IsCoOwner(from) || ownerBoat.IsFriend(from))
                                    ownerBoat.fishCaught++;
                            }
                        }

                        else if (item is WondrousFish)
                        {
                            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_CatchWondrousFish);

                            if (ownerBoat != null)
                            {
                                if (ownerBoat.IsOwner(from) || ownerBoat.IsCoOwner(from) || ownerBoat.IsFriend(from))
                                    ownerBoat.fishCaught++;
                            }
                        }

                        else if (item is TrulyRareFish)
                        {
                            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_CatchTrulyRareFish);

                            if (ownerBoat != null)
                            {
                                if (ownerBoat.IsOwner(from) || ownerBoat.IsCoOwner(from) || ownerBoat.IsFriend(from))
                                    ownerBoat.fishCaught++;
                            }
                        }

                        else if (item is PeculiarFish)
                        {
                            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_CatchHighlyPeculiarFish);

                            if (ownerBoat != null)
                            {
                                if (ownerBoat.IsOwner(from) || ownerBoat.IsCoOwner(from) || ownerBoat.IsFriend(from))
                                    ownerBoat.fishCaught++;
                            }
                        }

                        else if (item is MessageInABottle)
                        {
                            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_CatchMIB);                            
                        }

                        else if (item is Fish)
                        {
                            AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_CatchFish);

                            if (ownerBoat != null)
                            {
                                if (ownerBoat.IsOwner(from) || ownerBoat.IsCoOwner(from) || ownerBoat.IsFriend(from))
                                    ownerBoat.fishCaught++;
                            }
                        }

                        // IPY ACHIEVEMENTS
                        //The whole harvest system is kludgy and I'm sure this is just adding to it.
                        if (item.Stackable)
                        {
                            int amount = def.ConsumedPerHarvest;
                            int feluccaAmount = def.ConsumedPerFeluccaHarvest;

                            int racialAmount = (int)Math.Ceiling(amount * 1.1);
                            int feluccaRacialAmount = (int)Math.Ceiling(feluccaAmount * 1.1);

                            bool eligableForRacialBonus = (def.RaceBonus && from.Race == Race.Human);
                            bool inFelucca = (map == Map.Felucca);

                            if (eligableForRacialBonus && inFelucca && bank.Current >= feluccaRacialAmount && 0.1 > Utility.RandomDouble())
                                item.Amount = feluccaRacialAmount;
                            else if (inFelucca && bank.Current >= feluccaAmount)
                                item.Amount = feluccaAmount;
                            else if (eligableForRacialBonus && bank.Current >= racialAmount && 0.1 > Utility.RandomDouble())
                                item.Amount = racialAmount;
                            else
                                item.Amount = amount;
                        }

                        bank.Consume(item.Amount, from);

                        if (Give(from, item, def.PlaceAtFeetIfFull))
                        {
                            SendSuccessTo(from, item, resource);
                        }
                        else
                        {
                            SendPackFullTo(from, item, def, resource);
                            item.Delete();
                        }

                        BonusHarvestResource bonus = def.GetBonusResource();

                        if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
                        {
                            Item bonusItem = Construct(bonus.Type, from, tool, def, bank, resource);

                            if (Give(from, bonusItem, true))	//Bonuses always allow placing at feet, even if pack is full irregardless of def
                            {
                                bonus.SendSuccessTo(from);
                            }
                            else
                            {
                                item.Delete();
                            }
                        }

                        WearTool(from, tool, def);
                    }
                }
                if (type == null)
                {	// The only reason  for LJ/Mining to have a null type here is because of the custom harvesting for them
                    if ((def.Skill != SkillName.Mining && def.Skill != SkillName.Lumberjacking) || !Mining.NewIPYMining)
                    {
                        def.SendMessageTo(from, def.FailMessage);	// NOTE: Implemented as "failed any sort of harvesting"
                        AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_FailMiningAttempt);
                    }
                }
            }
            else
            {	// Failed
                def.SendMessageTo(from, def.FailMessage);	// NOTE: Implemented as "failed any sort of harvesting"
                AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_FailMiningAttempt);
            }
            OnHarvestFinished(from, tool, def, vein, bank, resource, toHarvest);
        }

        public static void WearTool(Mobile from, Item tool, HarvestDefinition def)
        {
            if (tool is IUsesRemaining)
            {
                IUsesRemaining toolWithUses = (IUsesRemaining)tool;

                toolWithUses.ShowUsesRemaining = true;

                if (toolWithUses.UsesRemaining > 0)
                    --toolWithUses.UsesRemaining;

                if (toolWithUses.UsesRemaining < 1)
                {
                    // IPY ACHIEVEMENT
                    if (tool is Pickaxe)
                        AchievementSystem.Instance.TickProgress(from, AchievementTriggers.Trigger_PickaxeBroke);
                    // IPY ACHIEVEMENT

                    tool.Delete();
                    def.SendMessageTo(from, def.ToolBrokeMessage);
                }
            }
        }

        public virtual void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
        }

        public virtual bool SpecialHarvest(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            return false;
        }

        public virtual Item Construct(Type type, Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, HarvestResource resource)
        {
            try { return Activator.CreateInstance(type) as Item; }
            catch { return null; }
        }

        public virtual HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            return vein;
        }

        public virtual void SendSuccessTo(Mobile from, Item item, HarvestResource resource)
        {
            resource.SendSuccessTo(from);
        }

        public virtual void SendPackFullTo(Mobile from, Item item, HarvestDefinition def, HarvestResource resource)
        {
            def.SendMessageTo(from, def.PackFullMessage);
        }

        public static bool GiveTo(Mobile m, Item item, bool placeAtFeet)
        {
            if (item is LargeBOD || item is SmallBOD)
            {
                if (item is LargeBOD)
                    m.SendGump(new LargeBODAcceptGump(m, (LargeBOD)item));
                else if (item is SmallBOD)
                    m.SendGump(new SmallBODAcceptGump(m, (SmallBOD)item));

                return false;
            }
            
            if (item is BaseLog)
                Custom.HarvestTracker.PlayerHarvest(m, Custom.HarvestType.Lumberjacking, item.Amount);

            else if (item is IronOre)
            {              
                Custom.HarvestTracker.PlayerHarvest(m, Custom.HarvestType.Mining, item.Amount);
            }

            else if (!(item is Container))
                Custom.HarvestTracker.PlayerHarvest(m, Custom.HarvestType.Fishing, item.Amount);

            if (item is BaseTreasureChest)
            {
                var chest = item as BaseTreasureChest;
                chest.UsedCaptcha = true;
                for (int i = chest.Items.Count - 1; i >= 0; i--)
                {
                    Item obj = chest.Items[i];
                    obj.Movable = true;
                }
                chest.OnDoubleClick(m);
                return true;
            }
            else if (m.PlaceInBackpack(item))
                return true;
            else
                item.MoveToWorld(m.Location, m.Map);
            return true;
        }

        public virtual bool Give(Mobile m, Item item, bool placeAtFeet)
        {
            return GiveTo(m, item, placeAtFeet);
        }

        public virtual Type MutateType(Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            return from.Region.GetResource(type);
        }

        public virtual Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            if (resource.Types.Length > 0)
                return resource.Types[Utility.Random(resource.Types.Length)];

            return null;
        }

        public virtual HarvestResource MutateResource(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestVein vein, HarvestResource primary, HarvestResource fallback)
        {
            bool racialBonus = (def.RaceBonus && from.Race == Race.Elf);

            if (vein.ChanceToFallback > (Utility.RandomDouble() + (racialBonus ? .20 : 0)))
                return fallback;

            double skillValue = from.Skills[def.Skill].Value;

            if (fallback != null && (skillValue < primary.ReqSkill || skillValue < primary.MinSkill))
                return fallback;

            return primary;
        }

        public virtual bool OnHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last)
        {
            if (!CheckHarvest(from, tool))
            {
                from.EndAction(locked);
                return false;
            }

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                from.EndAction(locked);
                OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            else if (!def.Validate(tileID))
            {
                from.EndAction(locked);
                OnBadHarvestTarget(from, tool, toHarvest);
                return false;
            }
            else if (!CheckRange(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }
            else if (!CheckResources(from, tool, def, map, loc, true))
            {
                from.EndAction(locked);
                return false;
            }
            else if (!CheckHarvest(from, tool, def, toHarvest))
            {
                from.EndAction(locked);
                return false;
            }

            DoHarvestingEffect(from, tool, def, map, loc);

            new HarvestSoundTimer(from, tool, this, def, toHarvest, locked, last).Start();

            return !last;
        }

        public virtual void DoHarvestingSound(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            //Changed from "from.PlaySound" to "Effects.PlaySound" - Sean
            if (def.EffectSounds.Length > 0)
                Effects.PlaySound(from.Location, from.Map, Utility.RandomList(def.EffectSounds));
        }

        public virtual void DoHarvestingEffect(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc)
        {
            from.Direction = from.GetDirectionTo(loc);

            if (!from.Mounted)
                from.Animate(Utility.RandomList(def.EffectActions), 5, 1, true, false, 0);
        }

        public virtual HarvestDefinition GetDefinition(int tileID)
        {
            HarvestDefinition def = null;

            for (int i = 0; def == null && i < m_Definitions.Count; ++i)
            {
                HarvestDefinition check = m_Definitions[i];

                if (check.Validate(tileID))
                    def = check;
            }

            return def;
        }

        public virtual void StartHarvesting(Mobile from, Item tool, object toHarvest)
        {
            if (!CheckHarvest(from, tool))
                return;

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            HarvestDefinition def = GetDefinition(tileID);

            if (def == null)
            {
                OnBadHarvestTarget(from, tool, toHarvest);
                return;
            }

            if (!CheckRange(from, tool, def, map, loc, false))
                return;
            else if (!CheckResources(from, tool, def, map, loc, false))
                return;
            else if (!CheckHarvest(from, tool, def, toHarvest))
                return;

            object toLock = GetLock(from, tool, def, toHarvest);

            if (!from.BeginAction(toLock))
            {
                OnConcurrentHarvest(from, tool, def, toHarvest);
                return;
            }

            new HarvestTimer(from, tool, this, def, toHarvest, toLock).Start();
            OnHarvestStarted(from, tool, def, toHarvest);
        }

        public virtual bool GetHarvestDetails(Mobile from, Item tool, object toHarvest, out int tileID, out Map map, out Point3D loc)
        {
            if (toHarvest is Static && !((Static)toHarvest).Movable)
            {
                Static obj = (Static)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = obj.Map;
                loc = obj.GetWorldLocation();
            }
            else if (toHarvest is StaticTarget)
            {
                StaticTarget obj = (StaticTarget)toHarvest;

                tileID = (obj.ItemID & 0x3FFF) | 0x4000;
                map = from.Map;
                loc = obj.Location;
            }
            else if (toHarvest is LandTarget)
            {
                LandTarget obj = (LandTarget)toHarvest;

                tileID = obj.TileID;
                map = from.Map;
                loc = obj.Location;
            }
            else
            {
                tileID = 0;
                map = null;
                loc = Point3D.Zero;
                return false;
            }

            return (map != null && map != Map.Internal);
        }
    }
}

namespace Server
{
    public interface IChopable
    {
        void OnChop(Mobile from);
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class FurnitureAttribute : Attribute
    {
        public static bool Check(Item item)
        {
            return (item != null && item.GetType().IsDefined(typeof(FurnitureAttribute), false));
        }

        public FurnitureAttribute()
        {
        }
    }
}