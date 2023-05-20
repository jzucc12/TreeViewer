namespace JZ.TreeViewer.Samples
{
    /// <summary>
    /// Events that can be called by HSMs to change state
    /// </summary>
    public enum StateEvent
    {
        idle,
        grounded,
        run,
        jump,
        inAir,
        dash,
        delay,
        collideEnter,
        collideExit
    }
}
