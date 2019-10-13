/// <summary>
/// A class for parsing the input to a boxer's brain
/// </summary>
public class MLInput
{
    private readonly float[] input;

    private const float TRUE = 1;

    private const int PUNCH_READY_IDX = 0;
    private const int DODGE_READY_IDX = 1;

    private const int MY_COMBO_START_IDX = 2;
    private const int MY_COMBO_STOP_IDX = 9;

    private const int OPPONENT_COMBO_START_IDX = 10;
    private const int OPPONENT_COMBO_STOP_IDX = 17;

    private const int OPPONENT_LEFT_PUNCH_IDX = 18;
    private const int OPPONENT_RIGHT_PUNCH_IDX = 19;
    private const int OPPONENT_LEFT_DODGE_IDX = 20;
    private const int OPPONENT_RIGHT_DODGE_IDX = 21;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="input">The brain input of an agent</param>
    public MLInput(float[] input)
    {
        this.input = input;
    }

    /// <summary>
    /// Determines if the agent's punch action is available
    /// </summary>
    /// <returns>True if the agent's punch is ready</returns>
    public bool IsPunchReady()
    {
        return ToBool(input[PUNCH_READY_IDX]);
    }

    /// <summary>
    /// Determines if the agent's dodge action is available
    /// </summary>
    /// <returns>True if the agent's dodge is ready</returns>
    public bool IsDodgeReady()
    {
        return ToBool(input[DODGE_READY_IDX]);
    }

    /// <summary>
    /// Gets the current action that the opponent is performing
    /// </summary>
    /// <returns>The opponent's current action</returns>
    public MLAction GetOpponentAction()
    {
        if (ToBool(input[OPPONENT_LEFT_PUNCH_IDX]))
        {
            return MLAction.PUNCH_LEFT;
        }

        if (ToBool(input[OPPONENT_RIGHT_PUNCH_IDX]))
        {
            return MLAction.PUNCH_RIGHT;
        }

        if (ToBool(input[OPPONENT_LEFT_DODGE_IDX]))
        {
            return MLAction.DODGE_LEFT;
        }

        if (ToBool(input[OPPONENT_RIGHT_DODGE_IDX]))
        {
            return MLAction.DODGE_RIGHT;
        }

        return MLAction.NOTHING;
    }

    /// <summary>
    /// Gets the current combo state of the boxer
    /// </summary>
    /// <returns>The current combo state</returns>
    public int GetMyComboState()
    {
        int start = MY_COMBO_START_IDX;
        int stop = MY_COMBO_STOP_IDX;

        return GetComboState(start, stop);
    }

    /// <summary>
    /// Gets the current combo state of the opponent
    /// </summary>
    /// <returns>The current opponent's combo state</returns>
    public int GetOpponentComboState()
    {
        int start = OPPONENT_COMBO_START_IDX;
        int stop = OPPONENT_COMBO_STOP_IDX;

        return GetComboState(start, stop);
    }


    // Private helpers

    private int GetComboState(int start, int stop)
    {
        for (var i = start; i <= stop; i++)
        {
            bool isOn = ToBool(input[i]);
            if (isOn)
            {
                var offset = i - start;
                return offset + 1;
            }
        }
        return 0;
    }

    private bool ToBool(float value)
    {
        return value == TRUE;
    }


}
