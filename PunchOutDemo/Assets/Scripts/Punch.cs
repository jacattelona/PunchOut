/// <summary>
/// A representation of a punch
/// </summary>
public class Punch
{

    public static Punch NULL_PUNCH = new Punch(PunchType.NONE, Hand.NONE, 0);

    private PunchType type;
    private Hand side;
    private float damage;

    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="type">The type of the punch</param>
    /// <param name="side">The side of the punch</param>
    /// <param name="damage">The damage of the punch</param>
    public Punch(PunchType type, Hand side, float damage)
    {
        this.type = type;
        this.side = side;
        this.damage = damage;
    }

    /// <summary>
    /// The type of the punch
    /// </summary>
    /// <returns>The type of the punch</returns>
    public PunchType GetPunchType()
    {
        return this.type;
    }

    /// <summary>
    /// The side of the punch
    /// </summary>
    /// <returns>The side of the punch</returns>
    public Hand GetHand()
    {
        return this.side;
    }

    /// <summary>
    /// The damage of the punch
    /// </summary>
    /// <returns>The amount of damage caused by the punch</returns>
    public float GetDamage()
    {
        return this.damage;
    }

}
