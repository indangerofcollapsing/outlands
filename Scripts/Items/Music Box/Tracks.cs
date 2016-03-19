using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.ContextMenus;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Items.MusicBox
{
    public class AmbrosiaSong : MusicBoxTrack
    {
        [Constructable]
        public AmbrosiaSong()
            : base(1075152)
        {
            Song = MusicName.Samlethe;
            //Name = "AmbrosiaSong";
        }

        public AmbrosiaSong(Serial s)
            : base(s)
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

    public class BoatTravelSong : MusicBoxTrack
    {
        [Constructable]
        public BoatTravelSong()
            : base(1075163)
        {
            Song = MusicName.Sailing;
            //Name = "Boat Travel";
        }

        public BoatTravelSong(Serial s)
            : base(s)
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

    public class BritainSong : MusicBoxTrack
    {
        [Constructable]
        public BritainSong()
            : base(1075145)
        {
            Song = MusicName.Britain2;
            //Name = "BritainSong";
        }

        public BritainSong(Serial s)
            : base(s)
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

    public class BritainPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public BritainPositiveSong()
            : base(1075144)
        {
            Song = MusicName.Britain1;
            //Name = "Britain (Positive)";
        }

        public BritainPositiveSong(Serial s)
            : base(s)
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

    public class BucsDenSong : MusicBoxTrack
    {
        [Constructable]
        public BucsDenSong()
            : base(1075146)
        {
            Song = MusicName.Bucsden;
            //Name = "Buc's Den";
        }

        public BucsDenSong(Serial s)
            : base(s)
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

    public class GoodvsEvilSong : MusicBoxTrack
    {
        [Constructable]
        public GoodvsEvilSong()
            : base(1075168)
        {
            Song = MusicName.Combat1;
            //Name = "Good vs. Evil";
        }

        public GoodvsEvilSong(Serial s)
            : base(s)
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

    public class ConversationWithGwennoSong : MusicBoxTrack
    {
        [Constructable]
        public ConversationWithGwennoSong()
            : base(1075131)
        {
            Song = MusicName.GwennoConversation;
            //Name = "Conversation With Gwenno";
        }

        public ConversationWithGwennoSong(Serial s)
            : base(s)
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

    public class CoveSong : MusicBoxTrack
    {
        [Constructable]
        public CoveSong()
            : base(1075176)
        {
            Song = MusicName.Cove;
            //Name = "CoveSong";
        }

        public CoveSong(Serial s)
            : base(s)
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

    public class DeathTuneSong : MusicBoxTrack
    {
        [Constructable]
        public DeathTuneSong()
            : base(1075171)
        {
            Song = MusicName.Death;
            //Name = "Death Tune";
        }

        public DeathTuneSong(Serial s)
            : base(s)
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

    public class DragonsHighSong : MusicBoxTrack
    {
        [Constructable]
        public DragonsHighSong()
            : base(1075160)
        {
            Song = MusicName.Dungeon9;
            //Name = "Dragons (High)";
        }

        public DragonsHighSong(Serial s)
            : base(s)
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

    public class DragonsLowSong : MusicBoxTrack
    {
        [Constructable]
        public DragonsLowSong()
            : base(1075175)
        {
            Song = MusicName.Dungeon2;
            //Name = "Dragons (Low)";
        }

        public DragonsLowSong(Serial s)
            : base(s)
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

    public class DreadHornAreaSong : MusicBoxTrack
    {
        [Constructable]
        public DreadHornAreaSong()
            : base(1075181)
        {
            Song = MusicName.DreadHornArea;
            //Name = "Dread Horn Area";
        }

        public DreadHornAreaSong(Serial s)
            : base(s)
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

    public class DungeonSong : MusicBoxTrack
    {
        [Constructable]
        public DungeonSong()
            : base(1075159)
        {
            Song = MusicName.Cave01;
            //Name = "DungeonSong";
        }

        public DungeonSong(Serial s)
            : base(s)
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

    public class ElfCitySong : MusicBoxTrack
    {
        [Constructable]
        public ElfCitySong()
            : base(1075182)
        {
            Song = MusicName.ElfCity;
            //Name = "Elf City";
        }

        public ElfCitySong(Serial s)
            : base(s)
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

    public class GargoylesSong : MusicBoxTrack
    {
        [Constructable]
        public GargoylesSong()
            : base(1075170)
        {
            Song = MusicName.Combat3;
            //Name = "GargoylesSong";
        }

        public GargoylesSong(Serial s)
            : base(s)
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

    public class GoodEndGameSong : MusicBoxTrack
    {
        [Constructable]
        public GoodEndGameSong()
            : base(1075132)
        {
            Song = MusicName.GoodEndGame;
            //Name = "Good End Game (U9)";
        }

        public GoodEndGameSong(Serial s)
            : base(s)
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

    public class GoodvsEvilSongU9 : MusicBoxTrack
    {
        [Constructable]
        public GoodvsEvilSongU9()
            : base(1075133)
        {
            Song = MusicName.GoodVsEvil;
            //Name = "Good vs. Evil (U9)";
        }

        public GoodvsEvilSongU9(Serial s)
            : base(s)
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

    public class GreatEarthSerpentsThemeSong : MusicBoxTrack
    {
        [Constructable]
        public GreatEarthSerpentsThemeSong()
            : base(1075134)
        {
            Song = MusicName.GreatEarthSerpents;
            //Name = "Great Earth Serpent's Theme";
        }

        public GreatEarthSerpentsThemeSong(Serial s)
            : base(s)
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

    public class GrizzleDungeonSong : MusicBoxTrack
    {
        [Constructable]
        public GrizzleDungeonSong()
            : base(1075186)
        {
            Song = MusicName.GrizzleDungeon;
            //Name = "Grizzle Dungeon";
        }

        public GrizzleDungeonSong(Serial s)
            : base(s)
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

    public class HumanoidsSong : MusicBoxTrack
    {
        [Constructable]
        public HumanoidsSong()
            : base(1075135)
        {
            Song = MusicName.Humanoids_U9;
            //Name = "HumanoidsSong (U9)";
        }

        public HumanoidsSong(Serial s)
            : base(s)
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

    public class JhelomSong : MusicBoxTrack
    {
        [Constructable]
        public JhelomSong()
            : base(1075147)
        {
            Song = MusicName.Jhelom;
            //Name = "JhelomSong";
        }

        public JhelomSong(Serial s)
            : base(s)
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

    public class LinelleSong : MusicBoxTrack
    {
        [Constructable]
        public LinelleSong()
            : base(1075185)
        {
            Song = MusicName.Linelle;
            //Name = "LinelleSong";
        }

        public LinelleSong(Serial s)
            : base(s)
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

    public class LordBritishsCastleSong : MusicBoxTrack
    {
        [Constructable]
        public LordBritishsCastleSong()
            : base(1075148)
        {
            Song = MusicName.LBCastle;
            //Name = "Lord British's Castle";
        }

        public LordBritishsCastleSong(Serial s)
            : base(s)
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

    public class MelisandesLairSong : MusicBoxTrack
    {
        [Constructable]
        public MelisandesLairSong()
            : base(1075183)
        {
            Song = MusicName.MelisandesLair;
            //Name = "Melisandes Lair";
        }

        public MelisandesLairSong(Serial s)
            : base(s)
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

    public class MinocNegativeSong : MusicBoxTrack
    {
        [Constructable]
        public MinocNegativeSong()
            : base(1075136)
        {
            Song = MusicName.MinocNegative;
            //Name = "Minoc (Negative) (U9)";
        }

        public MinocNegativeSong(Serial s)
            : base(s)
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

    public class MinocPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public MinocPositiveSong()
            : base(1075150)
        {
            Song = MusicName.Minoc;
            //Name = "Minoc (Positive)";
        }

        public MinocPositiveSong(Serial s)
            : base(s)
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

    public class MoonglowPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public MoonglowPositiveSong()
            : base(1075177)
        {
            Song = MusicName.Moonglow;
            //Name = "Moonglow (Positive)";
        }

        public MoonglowPositiveSong(Serial s)
            : base(s)
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

    public class NewMaginciaSong : MusicBoxTrack
    {
        [Constructable]
        public NewMaginciaSong()
            : base(1075149)
        {
            Song = MusicName.Magincia;
            //Name = "New Magincia";
        }

        public NewMaginciaSong(Serial s)
            : base(s)
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

    public class NujelmSong : MusicBoxTrack
    {
        [Constructable]
        public NujelmSong()
            : base(1075174)
        {
            Song = MusicName.Nujelm;
            //Name = "Nujel'm";
        }

        public NujelmSong(Serial s)
            : base(s)
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

    public class OverlordSong : MusicBoxTrack
    {
        [Constructable]
        public OverlordSong()
            : base(1075173)
        {
            Song = MusicName.BTCastle;
            //Name = "OverlordSong";
        }

        public OverlordSong(Serial s)
            : base(s)
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

    public class ParoxysmusLairSong : MusicBoxTrack
    {
        [Constructable]
        public ParoxysmusLairSong()
            : base(1075184)
        {
            Song = MusicName.ParoxysmusLair;
            //Name = "Paroxysmus Lair";
        }

        public ParoxysmusLairSong(Serial s)
            : base(s)
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

    public class PawsSong : MusicBoxTrack
    {
        [Constructable]
        public PawsSong()
            : base(1075137)
        {
            Song = MusicName.Paws;
            //Name = "PawsSong (U9)";
        }

        public PawsSong(Serial s)
            : base(s)
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

    public class PubTuneSong : MusicBoxTrack
    {
        [Constructable]
        public PubTuneSong()
            : base(1075167)
        {
            Song = MusicName.Tavern04;
            //Name = "Pub Tune";
        }

        public PubTuneSong(Serial s)
            : base(s)
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

    public class SelimsBarStrikeCommanderSong : MusicBoxTrack
    {
        [Constructable]
        public SelimsBarStrikeCommanderSong()
            : base(1075138)
        {
            Song = MusicName.SelimsBar;
            //Name = "Selim's Bar (Strike Commander)";
        }

        public SelimsBarStrikeCommanderSong(Serial s)
            : base(s)
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

    public class SkaraBraePositiveSong : MusicBoxTrack
    {
        [Constructable]
        public SkaraBraePositiveSong()
            : base(1075154)
        {
            Song = MusicName.Skarabra;
            //Name = "Skara Brae (Positive)";
        }

        public SkaraBraePositiveSong(Serial s)
            : base(s)
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

    public class StonesSong : MusicBoxTrack
    {
        [Constructable]
        public StonesSong()
            : base(1075143)
        {
            Song = MusicName.Serpents;
            //Name = "StonesSong";
        }

        public StonesSong(Serial s)
            : base(s)
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

    public class TaikoSong : MusicBoxTrack
    {
        [Constructable]
        public TaikoSong()
            : base(1075180)
        {
            Song = MusicName.Taiko;
            //Name = "TaikoSong";
        }

        public TaikoSong(Serial s)
            : base(s)
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

    public class Tavern1Song : MusicBoxTrack
    {
        [Constructable]
        public Tavern1Song()
            : base(1075164)
        {
            Song = MusicName.Tavern01;
            //Name = "Tavern 1";
        }

        public Tavern1Song(Serial s)
            : base(s)
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

    public class Tavern2Song : MusicBoxTrack
    {
        [Constructable]
        public Tavern2Song()
            : base(1075165)
        {
            Song = MusicName.Tavern02;
            //Name = "Tavern 2";
        }

        public Tavern2Song(Serial s)
            : base(s)
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

    public class Tavern3Song : MusicBoxTrack
    {
        [Constructable]
        public Tavern3Song()
            : base(1075166)
        {
            Song = MusicName.Tavern03;
            //Name = "Tavern 3";
        }

        public Tavern3Song(Serial s)
            : base(s)
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

    public class TokunoDungeonSong : MusicBoxTrack
    {
        [Constructable]
        public TokunoDungeonSong()
            : base(1075179)
        {
            Song = MusicName.TokunoDungeon;
            //Name = "Tokuno Dungeon";
        }

        public TokunoDungeonSong(Serial s)
            : base(s)
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

    public class TrinsicPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public TrinsicPositiveSong()
            : base(1075155)
        {
            Song = MusicName.Trinsic;
            //Name = "Trinsic (Positive)";
        }

        public TrinsicPositiveSong(Serial s)
            : base(s)
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

    public class TurfinSong : MusicBoxTrack
    {
        [Constructable]
        public TurfinSong()
            : base(1075142)
        {
            Song = MusicName.OldUlt02;
            //Name = "TurfinSong";
        }

        public TurfinSong(Serial s)
            : base(s)
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

    public class UltimaVIISerpentIsleCombatSong : MusicBoxTrack
    {
        [Constructable]
        public UltimaVIISerpentIsleCombatSong()
            : base(1075139)
        {
            Song = MusicName.SerpentIsleCombat_U7;
            //Name = @"Ultima VII / Serpent Isle Combat";
        }

        public UltimaVIISerpentIsleCombatSong(Serial s)
            : base(s)
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

    public class ValoriaPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public ValoriaPositiveSong()
            : base(1075151)
        {
            Song = MusicName.Ocllo;
            //Name = "Valoria (Positive)";
        }

        public ValoriaPositiveSong(Serial s)
            : base(s)
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

    public class ValoriaShipsSong : MusicBoxTrack
    {
        [Constructable]
        public ValoriaShipsSong()
            : base(1075140)
        {
            Song = MusicName.ValoriaShips;
            //Name = "Valoria Ships (U9)";
        }

        public ValoriaShipsSong(Serial s)
            : base(s)
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

    public class VesperSong : MusicBoxTrack
    {
        [Constructable]
        public VesperSong()
            : base(1075156)
        {
            Song = MusicName.Vesper;
            //Name = "VesperSong";
        }

        public VesperSong(Serial s)
            : base(s)
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

    public class VictorySong : MusicBoxTrack
    {
        [Constructable]
        public VictorySong()
            : base(1075172)
        {
            Song = MusicName.Victory;
            //Name = "VictorySong";
        }

        public VictorySong(Serial s)
            : base(s)
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

    public class YewSong : MusicBoxTrack
    {
        [Constructable]
        public YewSong()
            : base(1075157)
        {
            Song = MusicName.Wind;
            //Name = "YewSong";
        }

        public YewSong(Serial s)
            : base(s)
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

    public class YewPositiveSong : MusicBoxTrack
    {
        [Constructable]
        public YewPositiveSong()
            : base(1075158)
        {
            Song = MusicName.Yew;
            //Name = "Yew (Positive)";
        }

        public YewPositiveSong(Serial s)
            : base(s)
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

    public class ZentoSong : MusicBoxTrack
    {
        [Constructable]
        public ZentoSong()
            : base(1075178)
        {
            Song = MusicName.Zento;
            //Name = "ZentoSong";
        }

        public ZentoSong(Serial s)
            : base(s)
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


