using System;
using Server.Network;
using Server.Items;
using Server.Targeting;
using Server.Mobiles;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	[FlipableAttribute( 0xE81, 0xE82 )]
	public class ShepherdsCrook : BaseStaff
	{
        public override int BaseMinDamage { get { return 1; } }
        public override int BaseMaxDamage { get { return 2; } }
        public override int BaseSpeed { get { return 50; } }

        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 60; } }

        public override int IconItemId { get { return 5108; } }
        public override int IconHue { get { return Hue; } }
        public override int IconOffsetX { get { return -28; } }
        public override int IconOffsetY { get { return -10; } }

		[Constructable]
		public ShepherdsCrook() : base( 0xE81 )
		{
			Weight = 4.0;
		}

		public ShepherdsCrook( Serial serial ) : base( serial )
		{
		}

		public override WeaponAnimation GetAnimation()
		{
			WeaponAnimation animation = WeaponAnimation.Slash1H;

			Mobile attacker = this.Parent as Mobile;

			if (attacker != null)
			{
				if (attacker.FindItemOnLayer(Layer.TwoHanded) != null)
				{
					switch (Utility.RandomMinMax(1,4))
					{
						case 1: animation = WeaponAnimation.Bash2H;break;
						case 2: animation = WeaponAnimation.Bash2H;break;
						case 3: animation = WeaponAnimation.Slash2H;break;
						case 4: animation = WeaponAnimation.Pierce2H;break;
					}
				}
			}

			return animation;
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

		public override void OnDoubleClick( Mobile from )
		{
            PlayerMobile player = from as PlayerMobile;

            if (player == null)
                return;

            if (Core.TickCount - player.NextSkillTime < 0 || !player.CanBeginAction(typeof(ShepherdsCrook)))
			{
                player.SendMessage("You must wait a few moments before using another skill.");
                return;
			}

            if (player.AllFollowers.Count == 0)
            {
                player.SendMessage("You must have one or more followers in order to focus their aggression.");
                return;
            }

            int validFollowers = 0;

            foreach (BaseCreature creature in player.AllFollowers)
            {
                if (creature == null) continue;
                if (!creature.Alive) continue;
                if (Utility.GetDistance(player.Location, creature.Location) > 12) continue;

                validFollowers++;
            }

            if (validFollowers == 0)
            {
                player.SendMessage("You do not have any followers close enough to hear your command.");
                return;
            }

            player.SendMessage("What do you wish to focus your follower's aggression towards?");
            player.Target = new FocusedAggressionTarget();			
		}

		private class FocusedAggressionTarget : Target
		{
			public FocusedAggressionTarget() : base( 12, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object target )
			{
                PlayerMobile player = from as PlayerMobile;

                if (player == null)
                    return;

                Mobile mobile = target as Mobile;

                if (mobile == null)
                {
                    player.SendMessage("You cannot focus your follower's aggression against that.");
                    return;
                }

                if (!player.InLOS(mobile) || mobile.Hidden)
                {
                    player.SendMessage("That target is out of your line of sight.");
                    return;
                }

                if (!mobile.CanBeDamaged() || mobile.AccessLevel > AccessLevel.Player)
                {
                    player.SendMessage("That is not a valid target.");
                    return;
                }

                if (player.AllFollowers.Count == 0)
                {
                    player.SendMessage("You must have one or more followers in order to focus their aggression.");
                    return;
                }

                List<BaseCreature> m_FocusedCreatures = new List<BaseCreature>();

                int validFollowers = 0;

                foreach (BaseCreature creature in player.AllFollowers)
                {
                    if (creature == null) continue;
                    if (!creature.Alive) continue;
                    if (Utility.GetDistance(player.Location, creature.Location) > 12) continue;

                    m_FocusedCreatures.Add(creature);
                }

                if (m_FocusedCreatures.Count == 0)
                {
                    player.SendMessage("You do not have any followers close enough to hear your command.");
                    return;
                }

                if (player.CheckTargetSkill(SkillName.Herding, 0, 120, 1.0))
                {
                    double aggressionAmount = BaseCreature.HerdingFocusedAggressionDamageBonus * ((double)player.Skills.Herding.Value / 100);

                    foreach (BaseCreature creature in m_FocusedCreatures)
                    {
                        creature.FocusedAggressionTarget = mobile;
                        creature.FocusedAggresionValue = aggressionAmount;
                        creature.FocusedAggressionExpiration = DateTime.UtcNow + TimeSpan.FromMinutes(BaseCreature.HerdingFocusedAggressionDuration);                        
                    }                                       
                    
                    from.FixedParticles(0x373A, 10, 30, 5036, 2116, 0, EffectLayer.Head);
                    from.PlaySound(0x650);

                    mobile.PublicOverheadMessage(MessageType.Regular, 2117, false, "*focused aggression*");
                    mobile.FixedParticles(0x373A, 10, 30, 5036, 2116, 0, EffectLayer.Head);
                    mobile.PlaySound(0x650);

                    from.SendMessage("You focus your follower's aggression towards your target.");     
                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.HerdingSuccessCooldown * 1000);

                    from.BeginAction(typeof(ShepherdsCrook));
                    Timer.DelayCall(TimeSpan.FromMilliseconds(SkillCooldown.HerdingSuccessCooldown * 1000), delegate
                    {
                        if (from == null)
                            return;

                        from.EndAction(typeof(ShepherdsCrook));
                    });                    
                }

                else
                {
                    from.SendMessage("You fail to focus your followers aggression.");                  
                    from.NextSkillTime = Core.TickCount + (int)(SkillCooldown.HerdingFailureCooldown * 1000);

                    from.BeginAction(typeof(ShepherdsCrook));
                    Timer.DelayCall(TimeSpan.FromMilliseconds(SkillCooldown.HerdingFailureCooldown * 1000), delegate
                    {
                        if (from == null)
                            return;

                        from.EndAction(typeof(ShepherdsCrook));
                    }); 
                }
			}            
		}
	}
}
