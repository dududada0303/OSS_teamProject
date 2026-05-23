using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public int points = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), "Score :" + points);
    }
}
