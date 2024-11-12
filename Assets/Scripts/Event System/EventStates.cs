public enum GameState
{
    Gameplay, // playing level
    Preview, // before level start
    Menu, // not in game scene: shop, new relic, map...
}

public enum PieceState
{
    Default, 
    Animation, // PutFigure called, waiting for animation
}

public enum PlayerLiveState
{
    Alive,
    Dead, // rip
}