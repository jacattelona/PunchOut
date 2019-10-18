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
        switch (vectorAction[1])
        {
            case 1:
                return MLAction.PUNCH_LEFT;
            case 2:
                return MLAction.PUNCH_RIGHT;
        }

        switch (vectorAction[0])
        {
            case 1:
                return MLAction.DODGE_LEFT;
            case 2:
                return MLAction.DODGE_RIGHT;
        }

        return MLAction.NOTHING;
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
                return new float[] { 0f, 0f };
            case MLAction.PUNCH_LEFT:
                return new float[] { 0f, 1f };
            case MLAction.PUNCH_RIGHT:
                return new float[] { 0f, 2f };
            case MLAction.DODGE_LEFT:
                return new float[] { 1f, 0f };
            case MLAction.DODGE_RIGHT:
                return new float[] { 2f, 0f };
        }
        return new float[] { 0f, 0f };
    }
}
