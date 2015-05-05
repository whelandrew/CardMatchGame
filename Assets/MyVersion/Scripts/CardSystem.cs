using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardSystem : MonoBehaviour 
{
    public Transform UR, UL, DR, DL;

    public GameObject cache;
    List<CardActions> activeCards;
    public int maxCards;

    public float spacing;

    void Start()
    {
        activeCards = new List<CardActions>();

        PlaceCards();
    }

    void PlaceCards()
    {
        CardActions[] cards = cache.GetComponentsInChildren<CardActions>();

        int j=0;

        for(int i=0;i<maxCards;i++)
        {
            activeCards.Add(cards[i]);

            float newPos = (UR.position.x) + (spacing * i);

            if (i >=(maxCards / 2))
                j++;

            activeCards[i].transform.position = new Vector3(newPos, 0, (UR.position.z) + (spacing * j));
        }
    }

    //FOR EDITOR
    public Vector3 boardEdges, boardSize;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(boardEdges, boardSize);
    }
}
