using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Collectable : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Agent") || collision.gameObject.CompareTag("Player"))
        {
            GameManager.instance.Score(collision.gameObject.GetComponent<Agent>(), this);
            Destroy(this.gameObject);
        }
    }
}
