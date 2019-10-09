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
}
