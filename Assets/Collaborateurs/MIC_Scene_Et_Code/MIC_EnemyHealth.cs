using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MIC_EnemyHealth : MonoBehaviour
{
    public int PV = 3; // Points de vie initiaux de l'ennemi
    public Material[] materials; // Tableau de matériaux pour changer l'apparence de l'ennemi
    public Mesh[] meshes; //Tableau de maillages pour les différentes formes de l'ennemi
    private int currentMeshIndex = 0; // Index du maillage actuel dans le tableau
    private MeshFilter meshFilter; // Référence au MeshFilter de l'ennemi
    private Renderer rend; // Référence au Renderer de l'ennemi
    private int currentMaterialIndex = 0; // Index du matériaux actuel dans le tableau

    public Sprite[] sprites; // Tableau de sprites pour les différentes formes de l'ennemi

    private int currentSpriteIndex = 0; // Index du sprite actuel dans le tableau
    private SpriteRenderer spriteRenderer; // Référence au SpriteRenderer de l'ennemi

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>(); // Récupérer le MeshFilter de l'ennemi
        meshFilter.mesh = meshes[currentMeshIndex]; //Appliquer le premier maillage
        rend = GetComponent<Renderer>(); //Récupérer le Renderer de l'ennemi
        rend.material = materials[currentMaterialIndex]; //Appliquer le premier matériaux
         // Récupérer le SpriteRenderer de l'enfant spécifique (remplacez "Sprite_Test_Enemy" par le nom de l'enfant contenant le SpriteRenderer)
        spriteRenderer = transform.Find("Sprite_Test_Enemy").GetComponent<SpriteRenderer>();

        // Vérifier si le SpriteRenderer a été trouvé
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer non trouvé sur l'enfant spécifié !");
        }
        else
        {
            spriteRenderer.sprite = sprites[currentSpriteIndex]; // Appliquer le premier sprite
        }
    }

    // Méthode pour infliger des dégâts à l'ennemi
    public void TakeDamage(int damage)
    {
        PV -= damage; // Réduire les points de vie de l'ennemi

        if (PV <= 0)
        {
            Debug.Log("L'ennemi est vaincu !");
            // Mettre ici tout code lié à la destruction de l'ennemi
            Destroy(gameObject); // Détruire l'ennemi lorsque les PV tombent à zéro ou moins
        }
        else
        {
            Debug.Log("L'ennemi subit des dégâts. PV restants : " + PV);
            ChangeMaterial(); //Changer le matériau de l'ennemi
            ChnageMesh();
            ChangeSprite(); // Changer le sprite de l'ennemi
        }
    }
    private void ChangeMaterial()
    {
        currentMaterialIndex = (currentMaterialIndex + 1) % materials.Length; // Choisir le prochain matériau dans le tableau circulairement
        rend.material = materials[currentMaterialIndex]; // Appliquer le nouveau matériau à l'ennemi
    }
    private void ChnageMesh()
    {
        currentMeshIndex = (currentMeshIndex + 1) % meshes.Length; //Choisir le prochain maillage dans le tableau circulairement
        meshFilter.mesh = meshes[currentMeshIndex]; // Appliquer le nouveau maillage à l'ennemi
    }
    // Méthode pour changer le sprite de l'ennemi
    private void ChangeSprite()
    {
        currentSpriteIndex = (currentSpriteIndex + 1) % sprites.Length; // Choisir le prochain sprite dans le tableau circulairement
        spriteRenderer.sprite = sprites[currentSpriteIndex]; // Appliquer le nouveau sprite à l'ennemi
    }
}
