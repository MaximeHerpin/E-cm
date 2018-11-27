using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI; // intelligence artificielle = navmesh agent slt pour l'instant

public class ClickToMove : MonoBehaviour
{ // C EST ICI QU ON DECLARE LES ELEMENTS DE NOTRE JEU
    private NavMeshAgent navMeshAgent;
    private Camera camera;
    private bool movingOrNotMoving; // Le personnage est-il en déjà en train de se déplacer? Utile quand on ajoutera une animation
    // private Animator animationsSet;

    // Use this for initialization
    void Start() // FONCTION QUI NE S EXECUTE QUE DANS START EN INTERNE!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        camera = Camera.main;
        movingOrNotMoving = false;
        //animationsSet=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition); // On repère la position de notre souris en lançant un rayon depuis la caméra
        RaycastHit hit; //  On récupère la position de notre souris dans une variable hit
        if (Input.GetMouseButtonDown(0)) // si on a cliqué gauche
        { 
            if (Physics.Raycast(ray, out hit, 100)) // si dans un rayon de 100 autour du hit on trouve qqchose
            {
                navMeshAgent.destination = hit.point; // on fait avancer le GameObject vers le point hit 

            }
        }

        if(navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance){ // Tant que notre GameObject n'est pas arrivé à destination
            movingOrNotMoving = false;
        }
        else
        {
            movingOrNotMoving = true;
        }

        // ANIMATIONS
        // utiliser un graphe pour organiser les animations entre elles
        // animationsSet.setBool("animation de déplacement", movingOrNotMoving);
    }
}
