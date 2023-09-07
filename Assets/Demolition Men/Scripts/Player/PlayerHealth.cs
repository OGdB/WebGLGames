using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : Health
{
    [SerializeField]
    private Slider healthSlider;

    [Space(15)]
    public bool testBool = false;

    public override void OnDeath()
    {
        print("U DEAD BOII");
    }

    private void ChangeHealth(int amount)
    {
        CurrentHealth += amount;
        healthSlider.value = CurrentHealth;
    }

    private void Update()
    {
        if (testBool)
        {
            testBool = false;
            ChangeHealth(-10);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawLabel(healthSlider.transform.position, CurrentHealth.ToString());

        void DrawLabel(Vector3 position, string text)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;
            Handles.Label(position, text, style);
        }
    }
#endif
}
