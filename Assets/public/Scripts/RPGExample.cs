using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPGWORLD;

public class RPGExample : RPGWorld {

	// Use this for initialization
	void Start () {
        world = new MyWorld();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

public class MyWorld : World
{
    public MyWorld() : base(10)
    {
        Debug.Log("Custom World OOF");
    }

    public override void Generate(int seed)
    {
        AddModel(new Chunk(100));
    }
}

public class Chunk : Model
{
    private int size;

    public Chunk(int size) : base() 
    {
        this.size = size;
    }

    public override void Render()
    {
        base.Render();
        Mesh m = new Mesh
        {
            vertices = new Vector3[] { new Vector3(), new Vector3(size, 0), new Vector3(size, size), new Vector3(0, size) },
            uv = new Vector2[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) },
            triangles = new int[] { 0, 2, 3, 1, 2, 0 }
        };
        UpdateMesh(m);
    }

}