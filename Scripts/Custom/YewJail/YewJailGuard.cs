using System;
using Server;
/***************************************************************************
 *                              YewJailGuard.cs
 *                            -------------------
 *   begin                : July 2010
 *   author               : Sean Stavropoulos
 *   email                : sean.stavro@gmail.com
 *
 *
 ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Items;
using Server.Engines.Quests;
using Server.YewJail;

public class YewJailGuard : BaseVendor
{
    public YewJailGuard()
        : base("the Jail Guard")
    {
    }

	private List<SBInfo> m_SBInfos = new List<SBInfo>();
	protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }
    public override void InitSBInfo()
    {
        //throw new NotImplementedException();
    }

    private List<Mobile> m_Hostiles = new List<Mobile>();
    public override bool IsInvulnerable { get { return false; } }
    public override bool IsActiveVendor { get { return false; } }
    public bool CanTeach { get { return false; } }

    public override void InitBody()
    {
        InitStats(75, 75, 25);

        Hue = Utility.RandomSkinHue();

        Female = false;
        Body = 0x190;
        Name = NameList.RandomName("male");
        Skills.Swords.Base = 100;
        Skills.Tactics.Base = 100;
        Skills.Anatomy.Base = 100;
        Skills.DetectHidden.Base = 80;
        
        AI = AIType.AI_Melee;
        FightMode = FightMode.Aggressor;
    }

    public override void InitOutfit()
    {
        Item i = new PlateChest();
        i.Movable = false;
        AddItem(i);
        i = new PlateArms();
        i.Movable = false;
        AddItem(i);
        i = new PlateGloves();
        i.Movable = false;
        AddItem(i);
        i = new PlateLegs();
        i.Movable = false;
        AddItem(i);

        i = RandomUsefulItem();
        AddItem(i);

        Utility.AssignRandomHair(this);
        Utility.AssignRandomFacialHair(this, HairHue);

        BaseWeapon weapon = RandomGuardWeapon();
        AddItem(weapon);

        
    }

    private static Server.Point3D m_lastseenLoc;
    public Server.Point3D LastSeenLocation { get { return m_lastseenLoc; } }
    private Timer m_Timer;
    private bool spoken = false;

    public override void OnMovement(Mobile m, Point3D oldLocation)
    {
        if (m.Alive && m is PlayerMobile && m_Timer == null)
        {
            if (m_Hostiles.Contains(m) && m.Alive && InRange(m, 12) && InLOS(m) && CanSee(m))
            {
                Attack(m);
                //Move(GetDirectionTo(m.Location));
                //ActiveSpeed = 3.0;
                Combatant = m;
                Warmode = true;
                if (m_Timer == null)
                {
                    m_Timer = new JailGuardInternalTimer(this);
                    m_Timer.Start();
                }

                if (spoken == false)
                {
                    spoken = true;

                    switch (Utility.Random(7))
                    {
                        case 0: { Yell("You're making a big mistake!"); } break;
                        case 1: { Yell("GUARDS! A prisoner escaped!"); } break;
                        case 2: { Yell("Get over here!"); } break;
                        case 3: { Yell("Stop the prisoner! Don't let it escape!"); } break;
                        case 4: { Yell("How the hell did you get out here?"); } break;
                        case 5: { Yell("A prison escapee? Not if I have something to say about it!"); } break;
                        case 6: { Yell("Taste my blade, you scoundrel!"); } break;
                        case 7: { Yell("You're under arrest! I order you to stop!"); } break;
                    }
                }
                //AggressiveAction(m);
                //m_lastseenLoc = m.Location;
            }
        }
    }

    public void AddHostileTarget(Mobile target)
    {
        m_Hostiles.Add(target);
    }

    private BaseWeapon RandomGuardWeapon()
    {
        switch (Utility.Random(4))
        {
            default:
            case 0: return new Bardiche();
            case 1: return new VikingSword();
            case 2: return new Broadsword();
            case 3: return new Longsword();
        }

    }

    private Item RandomUsefulItem()
    {
        switch (Utility.Random(4))
        {
            default:
            case 0: return new Bandage(3);
            case 1: return new LeatherLegs();
            case 2: return new BagOfAllReagents(4);
            case 3: return new LeatherChest();
            case 4: return new LeatherArms();
        }

    }

    public YewJailGuard(Serial serial)
        : base(serial)
    {
    }

    public override void OnAfterDelete()
    {
        if (m_Timer != null)
            m_Timer.Stop();
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

    public class JailGuardInternalTimer : Timer
    {
        private YewJailGuard m_Guard;

        public JailGuardInternalTimer(YewJailGuard guard)
            : base(TimeSpan.Zero, TimeSpan.FromMilliseconds(200))
        {
            m_Guard = guard;
            Priority = TimerPriority.FiftyMS;
        }

        protected override void OnTick()
        {
            if (m_Guard.Deleted || m_Guard.Combatant == null)
                return;

            if (m_Guard.InLOS(m_Guard.Combatant))
            {
                m_Guard.Move(m_Guard.GetDirectionTo(m_Guard.Combatant.Location));
            }
            else
            {
                if (m_Guard.InRange(m_Guard.Combatant.Location, 7))
                {


                }
            }
            //if (!m_Guard.InRange(m_Guard.LastSeenLocation, 1))
            //{
            //    m_Guard.Move(m_Guard.GetDirectionTo(m_Guard.LastSeenLocation));
            //}
        }
    }
}





