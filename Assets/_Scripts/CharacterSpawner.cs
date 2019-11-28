using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpawner : MonoBehaviour
{
    [SerializeField] List<Material> characterMats;
    public GameObject characterObject;
    public Transform podiums;

    List<GameObject> characters;

    // Start is called before the first frame update
    void Start()
    {
        characters = new List<GameObject>();
        int i = 0;
        foreach(Transform podium in podiums) {
            foreach(Transform realPodium in podium) {
                foreach (Transform child in realPodium) {
                    if (child.name == "PlaceToStand") {
                        GameObject newChar = Instantiate(characterObject, child);
                        foreach(Transform sprite in newChar.transform) {
                            sprite.GetComponent<MeshRenderer>().material = characterMats[i];
                        }
                        characters.Add(newChar);
                        i++;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
