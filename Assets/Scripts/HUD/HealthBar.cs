using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

    public Image barra;
    public enum ColorMode {Single, Gradient}
    public ColorMode colorMode;
    public Color barColor = Color.white;
    public Gradient barGradient = new Gradient();
    private float porcentaje = 1.0f;

    // Update is called once per frame
    public void UpdateBar (int currentHealth, int maxHealth) {

        if (barra == null)
            return;

        // Fix the value to be a percentage.
        porcentaje = currentHealth / (float) maxHealth;

        // If the value is greater than 1 or less than 0, then fix the values to being min/max.
        if (porcentaje < 0 || porcentaje > 1)
            porcentaje = porcentaje < 0 ? 0 : 1;

        // Then just apply the target fill amount.
        barra.fillAmount = porcentaje;

        // Call the functions for the options.
        UpdateGradient();
	}

    public float GetCurrentFraction{
        get{
            return porcentaje;
        }
    }

    void UpdateGradient(){
        if (colorMode == ColorMode.Gradient){
            barra.color = barGradient.Evaluate(GetCurrentFraction);
		}
    }

}
