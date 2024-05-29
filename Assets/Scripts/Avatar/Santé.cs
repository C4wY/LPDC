using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // Ajoutez cette directive pour utiliser la classe Text

namespace Avatar
{
    public class Santé : MonoBehaviour
    {
        public int viemax;
        public float compteurInvincibilité;

        public int PV = 3;

        // Référence au script de l'animation de dégâts
        public MIC_JGmO_Sprite_Dégât animationDégâts;

        public GameObject gameOverScreen; // Référence au GameObject de l'écran de fin de jeu
        public Text deathText; // Référence au Text affichant "Vous êtes mort"
        public Image background; // Référence à l'image de fond
        public GameObject BarredeVie; // Référence au GameObject de la barre de vie

        void Start()
        {
            // Désactivez l'écran de fin de jeu au démarrage
            gameOverScreen.SetActive(false);
        }

        void Update()
        {
            if (compteurInvincibilité > 0)
            {
                compteurInvincibilité -= Time.deltaTime;
            }
        }

        public void FaireDégâts(int dégâts = 1)
        {
            if (compteurInvincibilité <= 0)
            {
                Debug.Log("OUCH");
                PV += -dégâts;

                // Appeler la méthode de l'animation de dégâts si elle est définie
                if (animationDégâts != null)
                {
                    animationDégâts.JoueurPrisDégâts(dégâts);
                }

                if (PV == 0)
                {
                    // Afficher l'écran de fin de jeu avec le texte "Vous êtes mort"
                    gameOverScreen.SetActive(true);
                    // Désactiver la barre de vie
                    BarredeVie.SetActive(false);
                    Time.timeScale = 0f;
                    deathText.text = "Vous êtes mort";
                    background.enabled = true; // Activer l'image de fond
                                               // Mettre en pause le jeu ou effectuer d'autres actions nécessaires lorsque le joueur est mort




                }
            }
        }
    }
}

