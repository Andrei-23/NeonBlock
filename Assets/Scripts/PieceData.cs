using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PieceData
{

    private static PieceData _instance;
    public static PieceData Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PieceData();
            }
            return _instance;
        }
    }

    public int count { get; private set; }
    private int rnd_color_cnt = 0; // used for generating 'static' random color on piece read

    // id of pieces with this rarity
    private List<int> common_pieces = new List<int>();
    private List<int> rare_pieces = new List<int>();
    private List<int> epic_pieces = new List<int>();
    private List<int> legendary_pieces = new List<int>();
    private List<int> bad_pieces = new List<int>();
    //private List<int> special_pieces = new List<int>();

    private List<Data> piece_data = new List<Data>();

    public Color commonColor = new Color(1f, 1f, 1f);
    public Color rareColor = new Color(0.004f, 1f, 0.469f);
    public Color epicColor = new Color(0.679f, 0.2f, 0.943f);
    public Color legendaryColor = new Color(0.988f, 1f, 0.004f);
    public Color badColor = new Color(0.896f, 0.358f, 0.415f);
    public Color specialColor = new Color(0.018f, 0f, 1f);

    // probability of at least this rarity
    //private float legendaryProb = 0.05f;// 0.05
    //private float epicProb = 0.2f;     // 0.15
    //private float rareProb = 0.5f;      // 0.30
    // default prob is 0.50

    public enum Rarity
    {
        Common = 0,
        Rare,
        Epic,
        Legendary,
        Bad,
        Special,
    };

    // used for better understanding of pieces in list
    public enum ColorId
    {
        Blue = 0,
        Green,
        Orange,
        Pink,
        Yellow,
        Red, // bad pieces
    }

    private class Data
    {
        public Data()
        {
            piece = new Piece();
            //is_fixed = false;
            color = 0;
            //prob = 1f;
        }
        public Data(Piece piece, int lvl, int color, float prob = 1f)
        {
            this.piece = piece;
            //this.is_fixed = is_fixed;
            this.color = color;
            foreach(List<Block> bl in piece.shape)
            {
                foreach (Block b in bl)
                {
                    b.color = color;
                }
            }
        }

        public Piece piece = new Piece(); // shape in str
        //public bool is_fixed = false; // is rotation aviable
        public int color = 0; // color
        //public float prob = 1f; // P(this) = lvl_total_prob[lvl] / prob
        public Rarity rarity = Rarity.Common;
    }

    //public bool IsFixedId(int id)
    //{
    //    return piece_data[id].piece.HasBlock(Block.Type.Chained);
    //}

    public Piece ReadPiece(List<string> str)
    {
        return ReadPiece(str, new List<Block.Type>());
    }
    //private Piece ReadPiece(int color, List<string> str)
    //{
    //    return ReadPiece(color, str, new List<Block.Type>());
    //}
    public Piece ReadPiece(List<string> str, List<Block.Type> types)
    {
        rnd_color_cnt++;
        return ReadPiece(str, types, rnd_color_cnt % 5);
    }

    /// <summary>
    /// Convert string list to Piece
    /// </summary>
    /// <param name="color">Id of piece color</param>
    /// <param name="str">Piece int string list form. Example:
    /// "##",
    /// "-*",
    /// Each symbol stands for some block type,
    /// '*' converts to types[i]
    /// </param>
    /// <param name="types">Types of '*' blocks</param>
    public Piece ReadPiece(List<string> str, List<Block.Type> types, int color) {
        // all '*' blocks = types[i]
        Piece p = new Piece();
        int h = str.Count;
        int w = str[0].Length;
        if (w != h)
        {
            string msg = "Wrong piece data: w != h";
            foreach(string s in str)
            {
                msg += s + "\n";
            }
            Debug.LogError(msg);
        }
        p.w = w;

        p.shape = new List<List<Block>>();
        for (int i = 0; i < w; i++)
        {
            p.shape.Add(new List<Block>(w));
            for (int j = 0; j < w; j++)
            {
                int x = j, y = w - 1 - i;
                char c = str[y][x];
                if (c == '*' || (c >= '0' && c <= '9'))
                {
                    int id = (c == '*') ? 0 : (c - '0');
                    Block.Type st = Block.Type.Default;
                    if (id >= types.Count())
                    {
                        Debug.LogError("intcorrect input");
                    }
                    else
                    {
                        st = types[id];
                    }
                    p.shape[i].Add(new Block(st, color));
                }
                else
                {
                    p.shape[i].Add(new Block(c, color));
                }
            }
        }
        return p;
    }

    private PieceData()
    {

        // --- Default 7 figures ---
        // lvl = 1

        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "###",
                "   "
            }),
            color = 0,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "##",
                "##",
            }),
            color = 0,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "####",
                "    ",
                "    "
            }),
            color = 0,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                "###",
                "   "
            }),
            color = 1,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                "###",
                "   "
            }),
            color = 2,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "## ",
                " ##",
                "   "
            }),
            color = 1,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " ##",
                "## ",
                "   "
            }),
            color = 2,
        });


        // --- Other figures ---

        // COMMON
        // No effects
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "   ",
                " # "
            }),
            color = 0,
        });

        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "###",
                "   "
            }),
            color = 1,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "###",
                " # "
            }),
            color = 2,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "####",
                "#   ",
                "    "
            }),
            color = 1,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "####",
                "   #",
                "    "
            }),
            color = 2,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                " # ",
                "#  "
            }),
            color = 3,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "#  #",
                "    ",
                "    ",
            }),
            color = 3,
        });

        // Simple pieces with small effects
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "#$#",
                "   "
            }),
            color = 3,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "01",
                "01",
            }, new List<Block.Type> {
                Block.Type.TempBlock,
                Block.Type.ExtraEnergy,
            }),
            color = 3,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "##*#",
                "    ",
                "    "
            }, new List<Block.Type> {
                Block.Type.Heal,
            }),
            color = 4,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                "#*#",
                "   "
            }, new List<Block.Type> {
                Block.Type.ExtraEnergy,
            }),
            color = 4,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "## ",
                " -#",
                "   "
            }),
            color = 2,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " ##",
                "$# ",
                "   "
            }),
            color = 1,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "$*$",
                "   ",
            }, new List<Block.Type> {
                Block.Type.TempBlock,
            }),
            color = 4,
        });

        // Bad, but with effects
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                "$$$",
                "#  "
            }),
            color = 3,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#- ",
                "###",
                " -# "
            }),
            color = 4,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                "#+#",
                "#  "
            }),
            color = 4,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "* *",
                "###",
                "   ",
            }, new List<Block.Type> {
                Block.Type.ExtraEnergy,
            }),
            color = 4,
        });

        // Testing new blocks
        /*
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "#*#",
                "   ",
            }, new List<Block.Type> {
                Block.Type.VirusBlock,
            }),
        }); piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#*#",
                "*#*",
                "   ",
            }, new List<Block.Type> {
                Block.Type.VirusBlock,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "00 ",
                " 00",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Mirror,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " * ",
                "###",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Downloader,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "***",
                "*#*",
                "###",
            }, new List<Block.Type> {
                Block.Type.TempBlock,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "101",
                "000",
                " # ",
            }, new List<Block.Type> {
                Block.Type.TempBlock,
                Block.Type.Sand,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                "**#",
                "   ",
            }, new List<Block.Type> {
                Block.Type.TempEnergy,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*  ",
                "###",
                "   ",
            }, new List<Block.Type> {
                Block.Type.TempHeal,
            }),
        }); 
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " * ",
                "###",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Cleaner,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#*",
                "##",
            }, new List<Block.Type> {
                Block.Type.Miner,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "*##",
                "   ",
            }, new List<Block.Type> {
                Block.Type.SolarPanel,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "** ",
                " ##",
                "   ",
            }, new List<Block.Type> {
                Block.Type.SaveBlock,
            }),
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*  ",
                "###",
                "   ",
            }, new List<Block.Type> {
                Block.Type.SaveBlock,
            }),
        });
        */


        // RARE

        // no effect
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                "  #",
                "   ",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                "#  ",
                "   ",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "##",
                "# ",
            }),
            rarity = Rarity.Rare,
        });

        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "# #",
                "   ",
            }),
            color = 2,
            rarity = Rarity.Rare,
        });

        // sand
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "s#s",
                "   "
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "##",
                "ss",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " s ",
                "s#s",
                " s "
            }),
            rarity = Rarity.Rare,
        });
           
        // bush
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " #*",
                "## ",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Bush,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "#*#",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Bush,
            }),
            rarity = Rarity.Rare,
        });


        // stone, metal
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "##S",
                "   ",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "##",
                "**",
            }, new List<Block.Type> {
                Block.Type.Stone,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " *",
                "##"
            }, new List<Block.Type> {
                Block.Type.Metal,
            }),
            rarity = Rarity.Rare,
        });

        //fixed
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "**",
                "  "
            }, new List<Block.Type> {
                Block.Type.Chained,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "* ",
                "* ",
            }, new List<Block.Type> {
                Block.Type.Chained,
            }),
            rarity = Rarity.Rare,
        });

        // instant bombs
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*",
            }, new List<Block.Type> {
                Block.Type.Laser,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*",
            }, new List<Block.Type> {
                Block.Type.Grenade,
            }),
            rarity = Rarity.Rare,
        });

        // ghosts
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "#-#",
                "   "
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "-#-",
                " # "
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "##",
                "--",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "-#",
                "#-",
            }),
            rarity = Rarity.Rare,
        });

        //bombs
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#b",
                "#b",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "#b#",
                "   ",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "b# ",
                " #b",
                "   ",
            }),
            rarity = Rarity.Rare,
        });

        // collectables
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "$$$",
                "   ",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "$  ",
                "-##",
                "   ",
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  *",
                "##*",
                "  *",
            }, new List<Block.Type>
            {
                Block.Type.ExtraEnergy,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " * ",
                "*$*",
                "   ",
            }, new List<Block.Type>
            {
                Block.Type.ExtraEnergy,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*#",
                "**",
            }, new List<Block.Type>
            {
                Block.Type.Heal,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "* *",
                " * ",
                "   ",
            }, new List<Block.Type>
            {
                Block.Type.Heal,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "0#0",
                "   ",
                "1#1",
            }, new List<Block.Type>
            {
                Block.Type.Heal,
                Block.Type.ExtraEnergy,
            }),
            rarity = Rarity.Rare,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  *",
                "*##",
                "   ",
            }, new List<Block.Type>
            {
                Block.Type.TempEnergy,
            }),
            rarity = Rarity.Rare,
        });

        // Epic

        // no effect
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "     ",
                "     ",
                "#####",
                "     ",
                "     ",
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#"
            }),
            rarity = Rarity.Epic,
        });

        // bush, grass
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#*",
                "  ",
            },
            new List<Block.Type> {
                Block.Type.Bush
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*",
            },
            new List<Block.Type> {
                Block.Type.Bush
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "*#*",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.Bush,
            }),
            rarity = Rarity.Epic,
        });

        // instant bombs
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "**",
                "  ",
            },
            new List<Block.Type> {
                Block.Type.Laser,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "* *",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.Laser,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "* *",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.Grenade,
            }),
            rarity = Rarity.Epic,
        });

        // collectables
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "***",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.ExtraEnergy,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " * ",
                "*#*",
                " * ",
            },
            new List<Block.Type> {
                Block.Type.Heal,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "$$",
                "$$",
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "****",
                "    ",
                "    ",
            },
            new List<Block.Type> {
                Block.Type.TempEnergy,
            }),
            rarity = Rarity.Epic,
        });

        // cool blocks
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*",
            },
            new List<Block.Type> {
                Block.Type.Cleaner,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*#",
                "##",
            },
            new List<Block.Type> {
                Block.Type.Miner,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                "##*",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.SolarPanel,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "***",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.Mirror,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*",
            },
            new List<Block.Type> {
                Block.Type.Downloader,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "#*#",
                "   ",
            },
            new List<Block.Type> {
                Block.Type.SaveBlock,
            }),
            rarity = Rarity.Epic,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "**",
                "**",
            },
            new List<Block.Type> {
                Block.Type.Sand,
            }),
            rarity = Rarity.Epic,
        });


        // L E G E N D A R Y


        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "****",
                "    ",
                "    ",
            }, new List<Block.Type>
            {
                Block.Type.Bush,
            }),
            rarity = Rarity.Legendary,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "****",
                "    ",
                "    ",
            }, new List<Block.Type>
            {
                Block.Type.Sand,
            }),
            rarity = Rarity.Legendary,
        });
        //megastick
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "          ",
                "          ",
                "          ",
                "          ",
                "##########",
                "          ",
                "          ",
                "          ",
                "          ",
                "          ",
            }),
            rarity = Rarity.Legendary,
        });

        //ghost
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "---",
                "-#-",
                "---",
            }),
            rarity = Rarity.Legendary,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  -  ",
                "  -  ",
                "--#--",
                "  -  ",
                "  -  ",
            }),
            rarity = Rarity.Legendary,
        });

        // cool blocks
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " # ",
                "#*#",
                "   ",
            }, new List<Block.Type>
            {
                Block.Type.VirusBlock,
            }),
            rarity = Rarity.Legendary,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "***",
                "   ",
            }, new List<Block.Type>
            {
                Block.Type.Miner,
            }),
            rarity = Rarity.Legendary,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "     ",
                "     ",
                "#***#",
                "     ",
                "     ",
            }, new List<Block.Type>
            {
                Block.Type.Downloader,
            }),
            rarity = Rarity.Legendary,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "* *",
                "   ",
                "* *",
            }, new List<Block.Type>
            {
                Block.Type.SaveBlock,
            }),
            rarity = Rarity.Legendary,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "   ",
                "***",
                "   ",
            }, new List<Block.Type>
            {
                Block.Type.SolarPanel,
            }),
            rarity = Rarity.Legendary,
        });

        Debug.Log("pieces loaded: " + piece_data.Count().ToString());

        // Bad

        // bad shapes
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                "###",
                "#  ",
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                "###",
                "  #"
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "  #",
                "## ",
                " ##"
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#  ",
                " ##",
                "## "
            }),
            rarity = Rarity.Bad,
        });

        // stone, metal

        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "## ",
                " SS",
                "   "
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " M#",
                "#M ",
                "   "
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "MMMM",
                "    ",
                "    ",
            }),
            rarity = Rarity.Bad,
        });

        // fixed
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " * ",
                "***",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Chained,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "FFFF",
                "    ",
                "    ",
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "F  ",
                "F*F",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Fire,
            }),
            rarity = Rarity.Bad,
        });

        // damage
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "##",
                "#*",
            }, new List<Block.Type> {
                Block.Type.Fire,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " ##",
                "#* ",
                "   ",
            }, new List<Block.Type> {
                Block.Type.Fire,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "    ",
                "#**#",
                "    ",
                "    ",
            }, new List<Block.Type> {
                Block.Type.Fire,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "#*",
                "* ",
            }, new List<Block.Type> {
                Block.Type.Fire,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " 0 ",
                "010",
                " 0 ",
            }, new List<Block.Type> {
                Block.Type.Heal,
                Block.Type.Cursed,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "*"
            }, new List<Block.Type> {
                Block.Type.Cursed,
            }),
            rarity = Rarity.Bad,
        });

        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                " ## ",
                "*  1",
                " ## ",
                "    ",
            }, new List<Block.Type> {
                Block.Type.ExtraEnergy,
                Block.Type.Heal,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "####",
                "####",
                "####",
                "####",
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "* # ",
                " ss#",
                "#ss ",
                " # *",
            }, new List<Block.Type> {
                Block.Type.ExtraEnergy,
            }),
            rarity = Rarity.Bad,
        });
        piece_data.Add(new Data
        {
            piece = ReadPiece(new List<string>
            {
                "### ",
                "**##",
                "####",
                "# # ",
            }, new List<Block.Type> {
                Block.Type.Metal,
            }),
            rarity = Rarity.Bad,
        });




        count = piece_data.Count;
        for(int i = 0; i < count; i++)
        {
            piece_data[i].piece.SetRarity(piece_data[i].rarity);
            switch (piece_data[i].rarity)
            {
                case Rarity.Common:
                    common_pieces.Add(i);
                    break;
                case Rarity.Rare:
                    rare_pieces.Add(i);
                    break;
                case Rarity.Epic:
                    epic_pieces.Add(i);
                    break;
                case Rarity.Legendary:
                    legendary_pieces.Add(i);
                    break;
                case Rarity.Bad:
                    bad_pieces.Add(i);
                    break;
                default:
                    break;
            }
        }
    }

    public Piece GetPiece(int id)
    {
        return new Piece(piece_data[id].piece).Clone();
    }

    //private float legendaryProb = 0.05f;// 0.05
    //private float epicProb = 0.2f;     // 0.15
    //private float rareProb = 0.5f;      // 0.30

    /// <summary>
    /// Returns common, rare, epic or legendary Rarity
    /// </summary>
    /// <returns>Randomly chosen Rarity</returns>
    public Rarity GetRandomRarity()
    {
        float x = UnityEngine.Random.Range(0f, 1f);
        if (RelicsManager.Instance.IsActive(RelicsManager.RelicType.Diamond))
        {
            if (x <= 0.15) return Rarity.Legendary; // 0.15
            else if (x < 0.4) return Rarity.Epic; // 0.25
            else if (x < 0.75) return Rarity.Rare; // 0.35
            else return Rarity.Common; // 0.25
        }
        else
        {
            if (x <= 0.08f) return Rarity.Legendary; // 0.08
            else if (x < 0.28) return Rarity.Epic; // 0.2
            else if (x < 0.6f) return Rarity.Rare; // 0.32
            else return Rarity.Common; // 0.4
        }
    }
    public int GetRandomPieceId(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return common_pieces[Random.Range(0, common_pieces.Count)];
            case Rarity.Rare: return rare_pieces[Random.Range(0, rare_pieces.Count)];
            case Rarity.Epic: return epic_pieces[Random.Range(0, epic_pieces.Count)];
            case Rarity.Legendary: return legendary_pieces[Random.Range(0, legendary_pieces.Count)];
            case Rarity.Bad: return bad_pieces[Random.Range(0, bad_pieces.Count)];
            //case Rarity.Special: return speical_pieces[Random.Range(0, special_pieces.Count)];
            //default: return Random.Range(0, count);
            default: return 0;
        }
    }
    public int GetRandomPieceId()
    {
        return GetRandomPieceId(GetRandomRarity());
    }

    public Piece GetRandomPiece() // random piece [common, legendary] with some probability
    {
        int id = GetRandomPieceId();
        return piece_data[common_pieces[id]].piece;
    }
    public List<List<Block>> GetPieceShape(int id, int rot = 0)
    {
        if (rot >= 4 || rot < 0)
        {
            Debug.LogWarning("wrong rotation input");
            rot = 0;
        }
        if (id < 0 || id >= piece_data.Count)
        {
            Debug.LogError("wrong block id");
            id = 0;
        }

        return piece_data[id].piece.GetShape(rot);
    }

    public Rarity GetPieceRarity(int id)
    {
        return piece_data[id].rarity;
    }

    public Color GetRarityColor(Rarity rarity)
    {
        switch (rarity)
        {
            case Rarity.Common: return commonColor;
            case Rarity.Rare: return rareColor;
            case Rarity.Epic: return epicColor;
            case Rarity.Legendary: return legendaryColor;
            case Rarity.Bad: return badColor;
            case Rarity.Special: return specialColor;
            default: return Color.black;
        }
    }

}
