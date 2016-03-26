/***************************************************************************
 *                            LobsterFishing.cs
 *                            ------------------
 *   begin                : February 2011
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Engines.Harvest
{
	public class LobsterFishing : HarvestSystem
	{
		private static LobsterFishing m_System;

		public static LobsterFishing System
		{
			get
			{
				if ( m_System == null )
					m_System = new LobsterFishing();

				return m_System;
			}
		}

		private HarvestDefinition m_Definition;

		public HarvestDefinition Definition
		{
			get{ return m_Definition; }
		}

        private LobsterFishing()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region lobstering
			HarvestDefinition lobster = new HarvestDefinition();

			// Resource banks are every 8x8 tiles
			lobster.BankWidth = 8;
			lobster.BankHeight = 8;

			// Every bank holds from 5 to 15 lobster
			lobster.MinTotal = 5;
			lobster.MaxTotal = 15;

			// A resource bank will respawn its content every 10 to 20 minutes
			lobster.MinRespawn = TimeSpan.FromMinutes( 10.0 );
			lobster.MaxRespawn = TimeSpan.FromMinutes( 20.0 );

			// Skill checking is done on the lobstering skill
			lobster.Skill = SkillName.Fishing;

			// Set the list of harvestable tiles
			lobster.Tiles = m_WaterTiles;
			lobster.RangedTiles = true;

			// Players must be within 4 tiles to harvest
			lobster.MaxRange = 3;

			// One lobster per harvest action
			lobster.ConsumedPerHarvest = 1;
			lobster.ConsumedPerFeluccaHarvest = 1;

			// The lobstering
            lobster.EffectActions = new int[] { 0 };
            lobster.EffectSounds = new int[0];
            lobster.EffectCounts = new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26 };
			lobster.EffectDelay = TimeSpan.FromSeconds(10.0);
			lobster.EffectSoundDelay = TimeSpan.FromSeconds( 8.0 );

			lobster.NoResourcesMessage = "The lobsters don't seem to be biting here"; // The lobster don't seem to be biting here.
			lobster.FailMessage = "You are unable to catch any lobsters."; // You lobster a while, but fail to catch anything.
			lobster.OutOfRangeMessage = "The trap is too cumbersome to deploy that far away."; // You need to be closer to the water to lobster!
			lobster.PackFullMessage = "You do not have room in your backpack for a lobster."; // You do not have room in your backpack for a lobster.
			lobster.ToolBrokeMessage = ""; // Not Used
            lobster.TimedOutOfRangeMessage = ""; // Not Used

			res = new HarvestResource[]
				{
					new HarvestResource( 00.0, 00.0, 100.0, 1043297, typeof( RawLobster ) ),
                    new HarvestResource( 00.0, 00.0, 100.0, 1043297, typeof( RawCrab ) )
				};

			veins = new HarvestVein[]
				{
					new HarvestVein( 50.0, 0.0, res[0], null ), //Raw Lobster
                    new HarvestVein( 50.0, 0.0, res[1], res[0] ) //Raw Crab
				};

			lobster.Resources = res;
			lobster.Veins = veins;

			lobster.BonusResources = new BonusHarvestResource[]
			{
				new BonusHarvestResource( 0, 99.4, null, null ), //set to same chance as mining ml gems
				new BonusHarvestResource( 80.0, .6, 1072597, typeof( WhitePearl ) )
			};

			m_Definition = lobster;
			Definitions.Add( lobster );
			#endregion
		}

		private class MutateEntry
		{
			public double m_ReqSkill, m_MinSkill, m_MaxSkill;
			public bool m_DeepWater;
			public Type[] m_Types;

			public MutateEntry( double reqSkill, double minSkill, double maxSkill, bool deepWater, params Type[] types )
			{
				m_ReqSkill = reqSkill;
				m_MinSkill = minSkill;
				m_MaxSkill = maxSkill;
				m_DeepWater = deepWater;
				m_Types = types;
			}
		}

		private static MutateEntry[] m_MutateTable = new MutateEntry[]
			{
				new MutateEntry(  80.0,  80.0,  3480.0,  true, typeof( BigFish ) ),
			};

		public override Type MutateType( Type type, Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource )
		{
			bool deepWater = SpecialFishingNet.FullValidation( map, loc.X, loc.Y );

			double skillBase = from.Skills[SkillName.Fishing].Base;
			double skillValue = from.Skills[SkillName.Fishing].Value;

			for ( int i = 0; i < m_MutateTable.Length; ++i )
			{
				MutateEntry entry = m_MutateTable[i];

				if ( !deepWater && entry.m_DeepWater )
					continue;

				if ( skillBase >= entry.m_ReqSkill )
				{
					double chance = (skillValue - entry.m_MinSkill) / (entry.m_MaxSkill - entry.m_MinSkill);

					if ( chance > Utility.RandomDouble() )
						return entry.m_Types[Utility.Random( entry.m_Types.Length )];
				}
			}

			return type;
		}

		private static Map SafeMap( Map map )
		{
			if ( map == null || map == Map.Internal )
				return Map.Felucca;

			return map;
		}

        public override bool BeginHarvesting(Mobile from, Item tool)
        {
            if (!base.BeginHarvesting(from, tool))
                return false;

            from.SendLocalizedMessage(500974); // What water do you want to fish in?
            return true;
        }

        public override void StartHarvesting(Mobile from, Item tool, object toHarvest, bool searchForNearbyNode)
        {
            if (!CheckTool(from, tool))
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

            LobsterTrap trap = (LobsterTrap)tool;

            if (trap.Amount > 1)
            {
                trap.Consume();
                trap = new LobsterTrap();
            }

            from.RevealingAction();
            Effects.SendLocationEffect(loc, map, 0x352D, 16, 4);
            Effects.PlaySound(loc, map, 0x364);

            if (trap == null)
                return;

            LobsterBuoy buoy = new LobsterBuoy(trap);
            buoy.MoveToWorld(new Point3D(loc, loc.Z - 1), map);

            trap.BeginHarvest(buoy);
            trap.MoveToWorld(new Point3D(loc, loc.Z - 1), map);

            HarvestTimer timer = new HarvestTimer(from, trap, this, def, toHarvest, null);
            timer.Start();
            trap.Timer = timer;
        }

        public override bool CheckTool(Mobile from, Item tool)
        {
            if (tool is LobsterTrap)
                return !((LobsterTrap)tool).Using;

            return false;
        }

        public override bool CheckResources(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            bool available = false;

            if (Spells.SpellHelper.InBuccs(map, loc)) //Lobsters are only sighted around Bucc's
            {
                HarvestBank bank = def.GetBank(map, loc.X, loc.Y);
                available = (bank != null && bank.Current >= def.ConsumedPerHarvest);
            }

            if (!available)
                def.SendMessageTo(from, timed ? def.DoubleHarvestMessage : def.NoResourcesMessage);

            return available;
        }

        public override bool CheckRange(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, bool timed)
        {
            if (timed)
                return true;

            if (!base.CheckRange(from, tool, def, map, loc, timed))
                return false;

            IPooledEnumerable eable = map.GetItemsInRange(loc, 1);

            foreach (Item item in eable)
            {
                if (item is LobsterBuoy)
                {
                    eable.Free();
                    from.SendMessage("This location is too close to another trap.");
                    return false;
                }
            }

            eable.Free();
            return true;
        }

        public override bool OnHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked, bool last)
        {
            if (!CheckHarvest(from, tool, def, toHarvest))
                return false;

            new HarvestSoundTimer(from, tool, this, def, toHarvest, locked, last).Start();

            return !last;
        }

        public override bool CheckHarvest(Mobile from, Item tool, HarvestDefinition def, object toHarvest)
        {
            LobsterTrap trap = (LobsterTrap)tool;

            if (trap == null || from == null || !trap.Using)
                return false;

            if (trap.Timer == null || trap.Timer.Index < 6) //Nothing happens during the first minute
                return true;

            if (Utility.RandomDouble() < 0.50) //50% chance every 10 seconds to attempt to catch
                return true;

            int tileID;
            Map map;
            Point3D loc;

            if (!GetHarvestDetails(from, tool, toHarvest, out tileID, out map, out loc))
                return false;

            HarvestBank bank = def.GetBank(map, loc.X, loc.Y);

            if (bank == null)
                return false;

            bool available = (bank != null && bank.Current >= def.ConsumedPerHarvest);

            if (!available) //No more lobsters, but do not warn the fisherman.
                return true;

            HarvestVein vein = bank.Vein;

            if (vein != null)
                vein = MutateVein(from, tool, def, bank, toHarvest, vein);

            if (vein == null)
                return false;

            HarvestResource primary = vein.PrimaryResource;
            HarvestResource fallback = vein.FallbackResource;
            HarvestResource resource = MutateResource(from, tool, def, map, loc, vein, primary, fallback);

            double skillBase = from.Skills[def.Skill].Base;
            double skillValue = from.Skills[def.Skill].Value;

            Type type = null;

            if (skillBase >= resource.ReqSkill && CheckSkill(from, def, resource))
            {
                type = GetResourceType(from, tool, def, map, loc, resource);

                if (type != null)
                    type = MutateType(type, from, tool, def, map, loc, resource);

                if (type != null)
                {
                    bank.Consume(1, from);
                    trap.Catch(type);

                    BonusHarvestResource bonus = def.GetBonusResource();

                    if (bonus != null && bonus.Type != null && skillBase >= bonus.ReqSkill)
                        trap.Catch(bonus.Type);
                }
            }

            return true;
        }

        public bool CheckSkill(Mobile from, HarvestDefinition def, HarvestResource resource)
        {
            Skill skill = from.Skills[def.Skill];

            if (skill == null)
                return false;

            double value = skill.Value;

            if (value < resource.MinSkill)
                return false; // Too difficult
            else if (value >= resource.MaxSkill)
                return true; // No challenge

            double chance = (value - resource.MinSkill) / (resource.MaxSkill - resource.MinSkill);

            return (Utility.RandomDouble() < chance);
        }

        public override void FinishHarvesting(Mobile from, Item tool, HarvestDefinition def, object toHarvest, object locked)
        {
            if (tool is LobsterTrap && ((LobsterTrap)tool).Using)
                ((LobsterTrap)tool).SinkTrap();
        }
        
        private static int[] m_WaterTiles = new int[]
			{
				0x00A8, 0x00AB,
				0x0136, 0x0137,
				0x5797, 0x579C,
				0x746E, 0x7485,
				0x7490, 0x74AB,
				0x74B5, 0x75D5
			};
	}
}