using MoreMountains.Feedbacks;
using System.Collections.Generic;
using UnityEngine;

public class MeshColorizer : MonoBehaviour
{
    [SerializeField] MeshRenderer mesh;
    [SerializeField] List<Color> randomColors;

    private void Start()
    {
        TestIt();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TestIt();
        }
    }

    private void TestIt()
    {
      //  Material assigned_material = new Material(mesh.material);
        mesh.material.color = randomColors[UnityEngine.Random.Range(0, randomColors.Count)];
//        mesh.material = assigned_material;
    }
}
