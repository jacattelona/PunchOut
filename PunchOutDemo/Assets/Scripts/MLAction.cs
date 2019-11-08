public enum MLAction
{
        NOTHING = 0,
        PUNCH_LEFT = 1,
        PUNCH_RIGHT = 2,
        DODGE_LEFT = 3,
        DODGE_RIGHT = 4
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
        if (vectorAction.Length < 1) return MLAction.NOTHING;
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
        float scale = 10000f;
        switch (action)
        {
            case MLAction.NOTHING:
                return new float[] { 0f, scale, 0f, 0f, 0f, 0f };
            case MLAction.PUNCH_LEFT:
                return new float[] { 1f, 0f, scale, 0f, 0f, 0f };
            case MLAction.PUNCH_RIGHT:
                return new float[] { 2f, 0f, 0f, scale, 0f, 0f };
            case MLAction.DODGE_LEFT:
                return new float[] { 3f, 0f, 0f, 0f, scale, 0f };
            case MLAction.DODGE_RIGHT:
                return new float[] { 4f, 0f, 0f, 0f, 0f, scale };
        }
        return new float[] { 0f, scale, 0f, 0f, 0f, 0f };
    }

    private static int ActionToInt(MLAction action)
    {
        switch (action)
        {
            case MLAction.NOTHING: return 0;
            case MLAction.PUNCH_LEFT: return 1;
            case MLAction.PUNCH_RIGHT: return 2;
            case MLAction.DODGE_LEFT: return 3;
            case MLAction.DODGE_RIGHT: return 4;
        }
        return 0;
    }

    /// <summary>
    /// Given the raw vectorActions, get the probability for a single action
    /// </summary>
    /// <param name="action">The action</param>
    /// <param name="vectorActions">The vector actions</param>
    /// <returns>The probability between 0 and 1 for performing the action</returns>
    public static float GetProbabilityFromVector(MLAction action, float[] vectorActions)
    {
        float scale = 10000;
        return vectorActions[ActionToInt(action) + 1] / scale;
    }
}
