using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [SerializeField] private string Biz_Lobby;
    private bool isPlayerInPortal = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = true;
            Debug.Log("Appuyez sur F pour entrer dans le portail");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInPortal = false;
        }
    }

    private void Update()
    {
        if (isPlayerInPortal && Input.GetKeyDown(KeyCode.F))
        {
            LoadTargetScene();
        }
    }

    private void LoadTargetScene()
    {
        Debug.Log("Chargement scene : " + Biz_Lobby);
        SceneManager.LoadScene(Biz_Lobby);
    }
}
