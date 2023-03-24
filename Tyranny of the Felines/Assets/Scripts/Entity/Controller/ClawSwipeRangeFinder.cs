using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matryoshka.Entity.Controller;
using Matryoshka.Utils;

public class ClawSwipeRangeFinder : MonoBehaviour
{
    public bool hasTargetInClawRange = false;
    private int playersInRange = 0;
    private void Start() {
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player"))
        {
            // Sometimes this triggers twice and idk why.
            playersInRange++;
            Debug.Log($"Claw swipe {playersInRange}");
            hasTargetInClawRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playersInRange--;
            if (playersInRange <= 0)
            {
                playersInRange = 0;
                Debug.Log($"No Claw swipe {playersInRange}");
                hasTargetInClawRange = false;
            }
        }
    }

    public void SetPosition(Vector2 position, int phase){

        playersInRange = 0;
        hasTargetInClawRange = false;
        if (phase == 1){
            transform.rotation = Utils.CalculateRotationFromVector(Vector2.left);
        }
        if(phase == 2){
            transform.rotation = Utils.CalculateRotationFromVector(Vector2.right);
        }
        if(phase == 3 || phase == 0){
            transform.rotation = Utils.CalculateRotationFromVector(Vector2.down);
        }
        transform.position = position;

    }
}
