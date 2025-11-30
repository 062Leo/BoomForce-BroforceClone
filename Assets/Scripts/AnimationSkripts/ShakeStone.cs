using System.Collections;
using UnityEngine;
public class ShakeStone : MonoBehaviour
{
    public float shakeDuration = 0.3f;  // Gesamtdauer des Shakings
    public float shakeAmount = 0.08f;    // Maximale Verschiebung
    public float cycleTime = 0.1f;      // Zeit für einen vollständigen Hin- und Her-Zyklus
    private Vector3 originalPos;        // Ursprüngliche Position

    public void StartShake()
    {
        originalPos = transform.localPosition;
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        float elapsed = 0.0f;
        float halfCycle = cycleTime / 2f;  // Hälfte des Zyklus für eine vollständige Hin- und Her-Bewegung

        while (elapsed < shakeDuration)
        {
            float phase = (elapsed % cycleTime) / cycleTime * 2 * Mathf.PI;  // Phasenwinkel für die Sinusfunktion
            float x = Mathf.Sin(phase) * shakeAmount;  // Sinusfunktion für oszillierende Bewegung

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;  // Zurücksetzen der Position
    }
}
