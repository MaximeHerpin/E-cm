using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovesCharacter : MonoBehaviour {
    private NavMeshAgent navMeshAgent;
    private float characterSpeed;

	// Use this for initialization
	void Start () {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        //characterSpeed = gameObject.GetComponent<CARAC.Speed>(); // Avec propriétés des persos sur script de Maxime
	}
	
	// Update is called once per frame
	void Update () {
        float forward = Input.GetAxis("Vertical");
        float turn = Input.GetAxis("Horizontal");
        Vector3 deplacement = new Vector3(turn/2, 0, forward/2);
        //Vector3 deplacement = new Vector3(turn*characterSpeed, 0, forward*characterSpeed);
        navMeshAgent.Move(deplacement);


    }
}
