public enum GameState
{
    Gameplay,
    Paused
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