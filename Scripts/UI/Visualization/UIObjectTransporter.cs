using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.UI.Visualization
{
    public class UIObjectTransporter : MonoBehaviour
    {
        public RectTransform destinationTransform;  // The target location for the objects
        public GameObject objectPrefab;  // The prefab of the object to spawn
        public int objectSpawnCount = 10;  // Number of coins to spawn and move
        public float spawnInterval = 0.1f;  // Interval between spawning coins
        public float moveDuration = 1.0f;  // Duration of the move
        public AnimationCurve moveCurve;  // Optional curve for smooth movement
        public Vector2 spawnStartPos;  // Initial spawn position for the coins
        
        public void SS()
        {
            StartCoroutine(SpawnAndTransportCoins());
        }

        IEnumerator SpawnAndTransportCoins()
        {
            for (int i = 0; i < objectSpawnCount; i++)
            {
                GameObject coin = Instantiate(objectPrefab, transform);  // Instantiate the coin prefab
                RectTransform coinRect = coin.GetComponent<RectTransform>();
                coinRect.anchoredPosition = spawnStartPos;  // Set the initial position of the coin

                // Get the target position in canvas space
                Vector2 targetPos = destinationTransform.anchoredPosition;

                // Start moving the coin to the TopBar
                StartCoroutine(MoveCoin(coinRect, spawnStartPos, targetPos, moveDuration));

                yield return new WaitForSecondsRealtime(spawnInterval);  // Wait before spawning the next coin
            }
        }

        IEnumerator MoveCoin(RectTransform coin, Vector2 startPos, Vector2 endPos, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.unscaledDeltaTime;  // Use unscaled time to respect UI animations during pause
                float t = Mathf.Clamp01(elapsedTime / duration);
                float curveT = moveCurve != null ? moveCurve.Evaluate(t) : t;
                coin.anchoredPosition = Vector2.Lerp(startPos, endPos, curveT);
                yield return null;
            }

            // Ensure the coin ends exactly at the target position
            coin.anchoredPosition = endPos;

            // Optionally, destroy the coin after reaching the target
            Destroy(coin.gameObject);
        }
    }
}
