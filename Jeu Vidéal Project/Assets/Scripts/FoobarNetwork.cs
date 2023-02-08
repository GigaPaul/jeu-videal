using UnityEngine;


// Script permettant de lancer le multi directement depuis le monde sans passer par le menu de départ
// Utile pour débugger sans avoir à charger la scene Menu
// Devra être supprimé plus tard 

public class FoobarNetwork : MonoBehaviour
{
    public NetworkManagerVideal NetworkManagerVideal;

    private void Start()
    {
        NetworkManagerVideal.StartHost();
    }
}
