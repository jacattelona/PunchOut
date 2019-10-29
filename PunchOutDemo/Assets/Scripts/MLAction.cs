public enum MLAction
{
        NOTHING,
        PUNCH_LEFT,
        PUNCH_RIGHT,
        DODGE_LEFT,
        DODGE_RIGHT
}

public class MLActionFactory
{
    private MLActionFactory() { }

    /// <summary>
    /// Get the action from a Brain's vectorAction
    /// </summary>
    /// <param name="vectorAction">The vector action output of a Brain</param>
    /// <returns>The action that the Brain predicts</returns>
    public static MLAction GetAction(float[] vectorAction)
    {
        switch (vectorAction[0])
        {
            case 1:
                return MLAction.PUNCH_LEFT;
            case 2:
                return MLAction.PUNCH_RIGHT;
            case 3:
                return MLAction.DODGE_LEFT;
            case 4:
                return MLAction.DODGE_RIGHT;
        }

        return MLAction.NOTHING;
    }

    /// <summary>
    /// Determine if an action is a dodge
    /// </summary>
    /// <param name="action">The action</param>
    /// <returns>True if the action is a dodge, false otherwise</returns>
    public static bool IsDodge(MLAction action)
    {
        return action == MLAction.DODGE_LEFT || action == MLAction.DODGE_RIGHT;
    }

    /// <summary>
    /// Determine if an action is a punch
    /// </summary>
    /// <param name="action">The action</param>
    /// <returns>True if the action is a punch, false otherwise</returns>
    public static bool IsPunch(MLAction action)
    {
        return action == MLAction.PUNCH_LEFT || action == MLAction.PUNCH_RIGHT;
    }

    /// <summary>
    /// Get the vector action from an action
    /// </summary>
    /// <param name="action">The action</param>
    /// <returns>The vectorized form of the action</returns>
    public static float[] GetVectorAction(MLAction action)
    {
        switch (action)
        {
            case MLAction.NOTHING:
                return new float[] { 0f};
            case MLAction.PUNCH_LEFT:
                return new float[] { 1f };
            case MLAction.PUNCH_RIGHT:
                return new float[] { 2f };
            case MLAction.DODGE_LEFT:
                return new float[] { 3f };
            case MLAction.DODGE_RIGHT:
                return new float[] { 4f };
        }
        return new float[] { 0f };
    }
}
