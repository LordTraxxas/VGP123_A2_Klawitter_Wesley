using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public Transform player;

    public float minXClamp = -28f;
    public float maxXClamp = 176.4f;
    public float minYClamp = -5.1f;
    public float maxYClamp = 51.49f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
            }
            else
            {
                Debug.LogError("Player GameObject not found. Please ensure the Player GameObject is tagged with 'Player'.");
            }
        }
        else
        {
            Vector3 cameraPos = transform.position;
        
            cameraPos.x = Mathf.Clamp(player.transform.position.x, minXClamp, maxXClamp);
            cameraPos.y = Mathf.Clamp(player.transform.position.y, minYClamp, maxYClamp);

            transform.position = cameraPos;

        }
        
    }
}
