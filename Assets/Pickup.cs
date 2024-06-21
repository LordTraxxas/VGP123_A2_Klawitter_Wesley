using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum PickupType
    {
        Life,
        PowerupSpeed,
        PowerupJump,
        Score,
        Health
    }

    [SerializeField] private PickupType type;
    
    public string targetTag = "Player";

    private void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            PlayerController pc = collision.gameObject.GetComponent<PlayerController>();
            if (type != PickupType.Health) 
            { 
                switch (type)
                {
                    case PickupType.Life:
                        pc.lives++;
                        break;
                    case PickupType.PowerupSpeed:
                    case PickupType.PowerupJump:
                        pc.PowerupValueChange(type);
                        break;
                    case PickupType.Score:
                        pc.score++;
                        break;
                }
                Destroy(gameObject); 
            }
            else
            {
                if (pc.health <= 6)
                {
                    pc.PowerupValueChange(type);
                    Destroy(gameObject);
                }
            }
        }

    }
}
