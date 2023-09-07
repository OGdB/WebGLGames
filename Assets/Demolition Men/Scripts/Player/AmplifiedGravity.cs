using UnityEngine;

public class AmplifiedGravity
{
    private Rigidbody2D rb;
    private float fallMultiplier;
    private float lowJumpMultiplier;

    public AmplifiedGravity(Rigidbody2D rb, float fallMultiplier, float lowJumpMultiplier)
    {
        this.rb = rb;
        this.fallMultiplier = fallMultiplier;
        this.lowJumpMultiplier = lowJumpMultiplier;
    }

    /// <summary>
    /// More platform-esque gravity. Increases the speed at which the character falls.
    /// </summary>
    /// <param name="jumpPressed">Slower fall when jump is pressed.</param>
    public void GravityAmplification(bool jumpPressed)
    {
        if (rb.velocity.y < 0)
        {
            rb.velocity += fallMultiplier * Time.deltaTime * Vector2.up;
        }
        else if (rb.velocity.y > 0 && !jumpPressed)
        {
            rb.velocity += lowJumpMultiplier * Time.deltaTime * Vector2.up;
        }
    }
}