using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static PieceData;
using static RelicsManager;
using Matrix = System.Collections.Generic.List<System.Collections.Generic.List<Block>>;

public class LevelDataManager
{

    private static LevelDataManager _instance;
    public static LevelDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LevelDataManager();
            }
            return _instance;
        }
    }

    private List<LevelData> data = new List<LevelData>();
    private List<List<int>> dataTypeId = new List<List<int>>();

    public int count => data.Count;
    private int glass_w => GlassManager.w;
    private int glass_h => GlassManager.h;

    public enum LevelType
    {
        standart = 0,
        miniboss,
        boss,
    }

    public class LevelData
    {
        public LevelData()
        {
            glass = new Matrix();
            task_extra = 0;
            turns_extra = 0;
            laser = 7;

            difficulty = 0;
            type = LevelType.standart;
            delete_from_top = false;

            pieces = new List<Piece>();
        }
        public LevelData(Matrix glass, int task_extra = 0, int difficulty = 0, int turns_extra = 0, int laser = 7, LevelType type = LevelType.standart, List<Piece> pieces = null, bool delete_from_top = false)
        {
            this.glass = glass;
            this.task_extra = task_extra;
            this.turns_extra = turns_extra;
            this.laser = laser;
            this.delete_from_top = delete_from_top;

            this.difficulty = difficulty;
            this.type = type;

            this.pieces = pieces ?? new List<Piece>();
        }

        public LevelData(LevelData other)
        {
            glass = other.glass;
            task_extra = other.task_extra;
            turns_extra = other.turns_extra;
            laser = other.laser;

            delete_from_top = other.delete_from_top;

            difficulty = other.difficulty;
            type = other.type;

            pieces = other.pieces;
        }

        public Matrix glass;
        public int task_extra;
        public int turns_extra;
        public int laser;
        public bool delete_from_top; // if true, lines delete above laser on overflow
        public int difficulty;
        public LevelType type;

        public List<Piece> pieces; // extra pieces that will shuffle in player's pieces
    }


    public LevelData GetRandomLevel(LevelType type = LevelType.standart, int difficulty = -1)
    {
        for(int t = 0; t < 100; t++)
        {
            int id_in_type = UnityEngine.Random.Range(0, dataTypeId[(int)type].Count);
            int id = dataTypeId[(int)type][id_in_type];
            if (difficulty == -1 || data[id].difficulty == difficulty)
            {
                return new LevelData(data[id]);
            }
        }
        Debug.LogError("Random level error");
        return new LevelData(data[dataTypeId[(int)type][0]]);
    }
    public LevelData GetRandomLevel()
    {
        //switch type
        int id = UnityEngine.Random.Range(0, count);
        return data[id];
    }

    //public Matrix GetRandomGlass(LevelType type = LevelType.standart, int difficulty = 0)
    //{
    //    int id = UnityEngine.Random.Range(0, count);
    //    return GetGlass(id);
    //}

    //public Matrix GetGlass(int id)
    //{
    //    return data[id].glass;
    //}

    private Matrix ReadLevel(List<string> str)
    {
        return ReadLevel(str, new List<Block.Type>());
    }
    /// <summary>
    /// Convert string list to level glass
    /// </summary>
    /// <param name="str">Glass in string list form. Width should be 10.
    /// Each symbol stands for some block type,
    /// '*' converts to types[i]
    /// </param>
    /// <param name="types">Types of '*' blocks</param>
    private Matrix ReadLevel(List<string> str, List<Block.Type> types)
    {
        // all '*' blocks = types[i]
        Matrix m = new Matrix();
        for(int i = 0; i < glass_h; i++)
        {
            m.Add(new List<Block>());
        }
        int str_h = str.Count();
        for (int y = 0; y < glass_h; y++)
        {
            if(y >= str_h)
            {
                for(int j = 0; j < glass_w; j++)
                {
                    m[y].Add(new Block(Block.Type.Empty));
                }
                continue;
            }

            int i = str_h - 1 - y;
            if (str[i].Length != glass_w)
            {
                Debug.LogError("Incorrect glass format");
            }
            foreach(char c in str[i])
            {
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
                    m[y].Add(new Block(st));
                }
                else
                {
                    m[y].Add(new Block(c));
                }
            }
        }
        return m;
    }

    private LevelDataManager()
    {
        // STANDART
        {
            // EASY
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "..........",
                })
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".#......#.",
                    "###....###",
                })
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "...#..#...",
                    "#..#..#..#",
                })
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "###....###",
                    "###....###",
                    "###....###",
                    "###....###",
                }),
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".b......b.",
                    "S.S....S.S",
                })
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "#........#",
                    "##......##",
                    "#........#",
                }),
                laser = 9,
            });

            // MEDIUM
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "..........",
                }),
                laser = 5,
                difficulty = 1,
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "..........",
                    "..........",
                    "....BB....",
                    "....BB....",
                    "....BB....",
                }),
                difficulty = 1,
                laser = 8,
                delete_from_top = true,
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "#........#",
                    "..........",
                    "#........#",
                    "..........",
                    "#........#",
                }),
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "###....###",
                    ".#.#..#.#.",
                }),
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "##......##",
                    ".#......#.",
                    ".#......#.",
                }),
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".########.",
                    "..........",
                    "..........",
                }),
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".........#",
                    "........##",
                    ".......###",
                    "......###.",
                    ".....###..",
                    "....###...",
                }),
                laser = 8,
            });

            // Sand
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "0000......",
                    "000000....",
                    "#######...",
                    "#.#####...",
                    ".##.###...",
                    ".#.####0..",
                }, new List<Block.Type>
                {
                    Block.Type.Sand,
                }),
                laser = 8,
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "..****....",
                    "********..",
                }, new List<Block.Type>
                {
                    Block.Type.Sand,
                }),
            });


            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "*S...S*S..",
                    ".SS*SS.SS*",
                    "*SS.SS*SS.",
                    ".S###S.S##",
                    "$$$$$$$$$.",
                }, new List<Block.Type>
                {
                    Block.Type.Bomb,
                }),
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "B........B",
                }),
                laser = 7,
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".#.#.#.#.#",
                    "#.#.#.#.#.",
                }),
                difficulty = 1,

            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "##......##",
                    "##......##",
                }),
                difficulty = 1,

                pieces =
                {
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "##",
                        "##",
                    }),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "##",
                        "##",
                    }),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "##",
                        "##",
                    }),
                }
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".#......#.",
                    "###....###",
                }),
                laser = 7,
                pieces = {
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "00",
                        "00",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " 0 ",
                        "000",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "    ",
                        "0000",
                        "    ",
                        "    ",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                }
            });
        }

        // MINIBOSSES
        {
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "#....#....",
                    "..#.##..#.",
                    "#..#..###.",
                    ".#.###.#.#",
                    "#.#.####.#",
                    "####.##.##",
                }),
                laser = 9,
                task_extra = 60,
                type = LevelType.miniboss,
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "#######...",
                    "##........",
                    "..........",
                    "..........",
                    "........##",
                    "...#######",
                }, new List<Block.Type>
                {
                    Block.Type.ExtraEnergy,
                }),
                laser = 9,
                task_extra = 50,
                type = LevelType.miniboss,
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "...####...",
                }),
                laser = 10,
                type = LevelType.miniboss,
                task_extra = 20,
                turns_extra = 0,
                pieces = {
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "###",
                        "***",
                        "###",
                    }, new List<Block.Type>{Block.Type.Ghost}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "##*",
                        "#*#",
                        "*##",
                    }, new List<Block.Type>{Block.Type.Coin}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "***",
                        "###",
                        "###",
                    }, new List<Block.Type>{Block.Type.Sand}),
                }
            });

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {

                    "..*....*..",
                    "..*....*..",
                    ".**.##.**.",
                    "###.#####.",
                }, new List<Block.Type>
                {
                    Block.Type.Mirror,
                }),
                laser = 7,
                type = LevelType.miniboss,
                task_extra = 50,
                turns_extra = 0,

                pieces =
                {
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "..*",
                        "***",
                        "...",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        ".*.",
                        "***",
                        "...",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "**.",
                        ".**",
                        "...",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "....",
                        "****",
                        "....",
                        "....",
                    }, new List<Block.Type>{Block.Type.Mirror}),
                }
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "#........#",
                    "#........#",
                    "##......##",
                }),
                laser = 8,
                type = LevelType.miniboss,
                task_extra = 50,
                turns_extra = 0,
                pieces = {
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " 0 ",
                        "000",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Chained}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " 0 ",
                        "00 ",
                        "0  ",
                    }, new List<Block.Type>{Block.Type.Chained}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " 00",
                        " 0 ",
                        " 0 ",
                    }, new List<Block.Type>{Block.Type.Chained}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "    ",
                        "0000",
                        "    ",
                        "    ",
                    }, new List<Block.Type>{Block.Type.Chained}),
                }
            });
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "....##....",
                    "...####...",
                }),
                laser = 8,
                type = LevelType.miniboss,
                task_extra = 60,
                turns_extra = 2,
                pieces = {
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " # ",
                        "#0#",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Cursed}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "0  ",
                        "###",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Cursed}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " 0#",
                        "## ",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Cursed}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "    ",
                        "0##0",
                        "    ",
                        "    ",
                    }, new List<Block.Type>{Block.Type.Cleaner}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "00",
                        "00",
                    }, new List<Block.Type>{Block.Type.Cleaner}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "## ",
                        " 00",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Cleaner}),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "  0",
                        "0##",
                        "   ",
                    }, new List<Block.Type>{Block.Type.Cleaner}),
                }
            });
        }

        // BOSSES
        {
            

            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    ".#.#.#.#.#",
                    "#.#.#.#.#.",
                    ".#.#.#.#.#",
                    "*.0.0.*.*.",
                }, new List<Block.Type>
                {
                    Block.Type.Protector,
                }),
                laser = 8,
                task_extra = 0,
                turns_extra = 3,
                delete_from_top = true,
                type = LevelType.boss,
            });

            List<Block.Type> oneKeyList = new List<Block.Type> { Block.Type.Key };
            List<Block.Type> oneLockList = new List<Block.Type> { Block.Type.Lock };
            // key & lock pieces
            data.Add(new LevelData()
            {
                glass = ReadLevel(new List<string> {
                    "..........",
                }),
                laser = 11,
                task_extra = 50,
                turns_extra = 5,
                delete_from_top = true,
                type = LevelType.boss,

                pieces = {
                    //locks
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "  #",
                        "#*#",
                        "   "
                    }, oneLockList),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "*  ",
                        "###",
                        "   "
                    }, oneLockList),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " #*",
                        "## ",
                        "   "
                    }, oneLockList),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "## ",
                        " *#",
                        "   "
                    }, oneLockList),


                    // keys
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        " * ",
                        "###",
                        "   "
                    }, oneKeyList),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "#*",
                        "##",
                    }, oneKeyList),
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "    ",
                        "#**#",
                        "    ",
                        "    ",
                    }, oneKeyList),

                    // cleaner
                    PieceData.Instance.ReadPiece(new List<string>
                    {
                        "*",
                    }, new List<Block.Type> { Block.Type.Cleaner }),
                }
            });
        }

        for (int i = 0; i < Enum.GetNames(typeof(LevelType)).Length; i++)
        {
            dataTypeId.Add(new List<int>());
        }
        for(int i = 0; i < count; i++)
        {
            dataTypeId[(int)data[i].type].Add(i);
        }
    }
}
