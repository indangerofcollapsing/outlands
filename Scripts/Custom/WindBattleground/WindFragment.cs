using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Custom;
using Server.Spells;

namespace Server.Items
{
    public class WindFragment : Item
    {
        private static List<WindFragment> Instances = new List<WindFragment>();
        public override string DefaultName { get { return "Wind Fragment"; } }
        public override bool Decays { get { return false; } }

        //[CommandProperty(AccessLevel.Administrator)]
        //public bool EndGame { get { return false; } set { CTFBattleground.End(false, null); } }

        public static void ClearInstances()
        {
            foreach (var instance in Instances)
                instance.Delete();
        }

        public WindFragment()
            : base(13904)
        {
            Instances.Add(this);
        }

        public WindFragment(Serial serial)
            : base(serial)
        {
        }

        public override DeathMoveResult OnParentDeath(Mobile parent)
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { if (!Deleted) MoveToWorld(GetWorldLocation(), Map); });
            ((PlayerMobile)parent).DamageVulnerable = false;
            return DeathMoveResult.RemainEquiped;
        }

        public override DeathMoveResult OnInventoryDeath(Mobile parent)
        {
            Timer.DelayCall(TimeSpan.FromTicks(1), delegate { if (!Deleted) MoveToWorld(GetWorldLocation(), Map); });
            ((PlayerMobile)parent).DamageVulnerable = false;
            return DeathMoveResult.RemainEquiped;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (!WindBattleground.IsActive())
            {
                from.SendMessage("You cannot pick up the fragment at this time.");
            }
            else if (!(from is PlayerMobile) || (RootParentEntity is Mobile && RootParentEntity != from))
                return false;
            else if (IsChildOf(from.Backpack))
            {
                from.SendMessage("Where would you like to place the fragment?");
                from.Target = new InternalTarget(this);
            }
            else if (!from.InRange(GetWorldLocation(), 2))
            {
                from.SendLocalizedMessage(500446); // That is too far away.
            }
            else if (from is PlayerMobile && !((PlayerMobile)from).IsInMilitia)
            {
                from.SendMessage("You must be a militia member to pickup the fragment.");
                return false;
            }
            else
            {
                from.RevealingAction();
                from.AddToBackpack(this);
            }

            return false;
        }

        public Mobile FindOwner()
        {
            var parent = this.Parent;

            if (parent is Item)
                return ((Item)parent).RootParent as Mobile;

            if (parent is Mobile)
                return (Mobile)parent;

            return null;
        }

        public Mobile FindOwner(object parent)
        {
            if (parent is Item)
                return ((Item)parent).RootParent as Mobile;

            if (parent is Mobile)
                return (Mobile)parent;

            return null;
        }

        public static bool ExistsOn(Mobile mob)
        {
            Container pack = mob.Backpack;

            return (pack != null && pack.FindItemByType(typeof(WindFragment)) != null);
        }

        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);

            Mobile mob = FindOwner(parent);

            if (mob != null)
            {
                mob.SolidHueOverride = WindBattleground.CarrierHue;
                mob.SendMessage("You have the wind fragment!");
                WindBattleground.PickupFragment(mob);
                ((PlayerMobile)mob).DamageVulnerable = true;
            }
        }

        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            Mobile mob = FindOwner(parent);

            if (mob != null)
            {
                mob.SolidHueOverride = -1;
                mob.SendMessage("You no longer have the wind fragment!");
                WindBattleground.DropFragment(mob);
                ((PlayerMobile)mob).DamageVulnerable = false;
            }
        }

        public void PlaceFlag(Mobile from, object targeted)
        {
            IPoint3D p = targeted as IPoint3D;

            Point3D targ3D;

            if (p is Item)
                targ3D = ((Item)p).GetWorldLocation();
            else
                targ3D = new Point3D(p);

            if (Map != null)
            {
                int z = Map.GetAverageZ(targ3D.X, targ3D.Y);

                if (!Map.CanFit(targ3D.X, targ3D.Y, z, 16, true, true))
                {
                    from.SendLocalizedMessage(501025); //Something is blocking the location
                }
                else if (!from.InLOS(targ3D))
                {
                    from.SendLocalizedMessage(500237); // Target cannot be seen.
                }
                else
                {
                    MoveToWorld(targ3D, Map);
                }
            }
        }

        public void ReturnHome()
        {
            MoveToWorld(WindBattleground.FragmentSpawnLocations[Utility.Random(WindBattleground.FragmentSpawnLocations.Length)], Map.Felucca);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();

            WindBattleground.RegisterFragment(this);
            Instances.Add(this);
        }

        private class InternalTarget : Target
        {
            WindFragment _flag;

            public InternalTarget(WindFragment f)
                : base(2, true, TargetFlags.None)
            {
                _flag = f;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (_flag != null)
                    _flag.PlaceFlag(from, targeted);
            }
        }
    }
}
