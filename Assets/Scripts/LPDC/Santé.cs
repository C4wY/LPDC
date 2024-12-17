using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LPDC
{
    public class Santé : MonoBehaviour
    {
        public int viemax;
        public float compteurInvincibilité;

        public int PV = 3;

        // Référence au script de l'animation de dégâts
        public MIC_JGmO_Sprite_Dégât animationDégâts;
        public Text deathText; // Référence au Text affichant "Vous êtes mort"
        public Image background; // Référence à l'image de fond
        public GameObject gameOverScreen;

        // Reference to the CameraShake script
        public CameraShake cameraShake; // Add a reference to CameraShake

        void Update()
        {
            if (compteurInvincibilité > 0)
            {
                compteurInvincibilité -= Time.deltaTime;
            }
        }

        /// <summary>
        /// Tente d'appliquer des dégâts à l'avatar, return true if successful. 
        /// </summary>
        public bool FaireDégâts(int dégâts = 1)
        {
            // Check if the player actually loses health
            if (dégâts > 0)
            {
                Debug.Log("OUCH");
                PV -= dégâts;  // Reduce health

                // Appeler la méthode de l'animation de dégâts si elle est définie
                if (animationDégâts != null)
                {
                    animationDégâts.JoueurPrisDégâts(dégâts);
                }

                if (PV <= 0)
                {
                    // Afficher l'écran de fin de jeu avec le texte "Vous êtes mort"
                    Instantiate(gameOverScreen);
                    Time.timeScale = 0f;
                }
                return true;
            }

            return false;
        }
    }
}
