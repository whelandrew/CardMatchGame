using UnityEngine;
using System.Collections;

public class CardActions : MonoBehaviour 
{
    bool isFlipped;
    bool isAnimating;

    public int rotateRate = 180;

    public float speed;
    
    Transform t;

	void Start () 
    {
        t = GetComponent<Transform>();

        StopAllCoroutines();
	}	
	
	void Update () 
    {        
        for (var i = 0; i < Input.touchCount; ++i)
        {
            if (Input.GetTouch(i).phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
                // Create a particle if hit
                if (Physics.Raycast(ray))
                    Debug.Log("TOUCHED!");
            }
        }
	}

    void OnMouseDown()
    {
        if (isAnimating)
            return;

        StartCoroutine(FlipCard());       
    }

    IEnumerator FlipCard()
    {
        isAnimating = true;

        bool done = false;
        while(!done)
        {
            float rot = rotateRate * Time.deltaTime;

            if (isFlipped)
                rot = -rot;

            t.Rotate(new Vector3(0, 0, rot));

            if (rotateRate < t.eulerAngles.z)
            {
                t.Rotate(new Vector3(0, 0, -rot));
                done = true;
            }
            else
                done = false;

            yield return new WaitForSeconds(speed);
        }
        
        isFlipped = !isFlipped;
        isAnimating = false;
    }
}
