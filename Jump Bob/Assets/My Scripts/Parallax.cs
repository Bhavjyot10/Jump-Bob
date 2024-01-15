using UnityEngine;

public class Parallax : MonoBehaviour {

	private float length, startPosX, startPosY;
	public GameObject cam;
	public float parallexEffect;

	void Start () {
		startPosX = transform.position.x;
        startPosY = transform.position.y;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
	}
	
	void Update () {
		float temp = (cam.transform.position.x * (1-parallexEffect));
		float distX = (cam.transform.position.x*parallexEffect);
        float distY = (cam.transform.position.y * parallexEffect);

        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

		if      (temp > startPosX + length/2) startPosX += length;
		else if (temp < startPosX - length/2) startPosX -= length;
	}

}
