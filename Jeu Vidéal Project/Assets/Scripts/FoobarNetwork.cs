using UnityEngine;


// Script permettant de lancer le multi directement depuis le monde sans passer par le menu de d�part
// Utile pour d�bugger sans avoir � charger la scene Menu
// Devra �tre supprim� plus tard 

public class FoobarNetwork : MonoBehaviour
{
    public NetworkManagerVideal NetworkManagerVideal;

    private void Start()
    {
        NetworkManagerVideal.StartHost();
    }
}
