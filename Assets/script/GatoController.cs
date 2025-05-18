using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GatoController : MonoBehaviour
{
    private bool encontroRaton = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Mouse") && !encontroRaton)
        {
            encontroRaton = true;
            StartCoroutine(VolverAlMenu());
        }
    }

    private IEnumerator VolverAlMenu()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MenuScene"); // Asegúrate de que "MenuPrincipal" sea el nombre correcto de tu escena de menú
    }
}
